using System.ComponentModel;
using System.Management;
using System.Printing;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;

namespace PrintSpooler.Utilities
{
    //https://www.codeproject.com/script/Content/ViewAssociatedFile.aspx?rzp=%2FKB%2Fprinting%2FPrinterQueueMonitor%2F%2FPrintSpooler.zip&zep=PrintQueueMonitor%2FPrintQueueMonitor.cs&obid=51085&obtid=2&ovid=6
    public class PrintJobChangeEventArgs : EventArgs
    {
        #region private variables
        private int _jobID = 0;
        private string _jobName = "";
        private JOBSTATUS _jobStatus = new JOBSTATUS();
        private PrintSystemJobInfo _jobInfo = null;
        #endregion

        public int JobID { get { return _jobID; } }
        public string JobName { get { return _jobName; } }
        public JOBSTATUS JobStatus { get { return _jobStatus; } }
        public PrintSystemJobInfo JobInfo { get { return _jobInfo; } }
        public PrintJobChangeEventArgs(int intJobID, string strJobName, JOBSTATUS jStatus, PrintSystemJobInfo objJobInfo)
            : base()
        {
            _jobID = intJobID;
            _jobName = strJobName;
            _jobStatus = jStatus;
            _jobInfo = objJobInfo;
        }
    }

    public delegate void PrintJobStatusChanged(object Sender, PrintJobChangeEventArgs e);

    public class PrintQueueMonitor
    {
        #region DLL Import Functions
        [DllImport("winspool.drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool OpenPrinter(string pPrinterName,
        out IntPtr phPrinter,
        IntPtr pDefault);

        [DllImport("winspool.drv", EntryPoint = "ClosePrinter",
            SetLastError = true,
            ExactSpelling = true,
            CallingConvention = CallingConvention.StdCall)]
        public static extern bool ClosePrinter
        (IntPtr hPrinter);

        [DllImport("winspool.drv",
        EntryPoint = "FindFirstPrinterChangeNotification",
        SetLastError = true, CharSet = CharSet.Ansi,
        ExactSpelling = true,
        CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr FindFirstPrinterChangeNotification
                            ([In()] IntPtr hPrinter,
                            [In()] int fwFlags,
                            [In()] int fwOptions,
                            [In(), MarshalAs(UnmanagedType.LPStruct)] PRINTER_NOTIFY_OPTIONS pPrinterNotifyOptions);

        [DllImport("winspool.drv", EntryPoint = "FindNextPrinterChangeNotification",
        SetLastError = true, CharSet = CharSet.Ansi,
        ExactSpelling = false,
        CallingConvention = CallingConvention.StdCall)]
        public static extern bool FindNextPrinterChangeNotification
                            ([In()] IntPtr hChangeObject,
                             [Out()] out int pdwChange,
                             [In(), MarshalAs(UnmanagedType.LPStruct)] PRINTER_NOTIFY_OPTIONS pPrinterNotifyOptions,
                            [Out()] out IntPtr lppPrinterNotifyInfo
                                 );
        #endregion

        #region Constants
        const int PRINTER_NOTIFY_OPTIONS_REFRESH = 1;
        #endregion
        #region Events
        public event PrintJobStatusChanged OnJobStatusChange;
        #endregion

        #region private variables
        private IntPtr _printerHandle = IntPtr.Zero;
        private string _spoolerName = "";
        private ManualResetEvent _mrEvent = new ManualResetEvent(false);
        private RegisteredWaitHandle _waitHandle = null;
        private IntPtr _changeHandle = IntPtr.Zero;
        private PRINTER_NOTIFY_OPTIONS _notifyOptions = new PRINTER_NOTIFY_OPTIONS();
        private Dictionary<int, string> objJobDict = new Dictionary<int, string>();
        private PrintQueue _spooler = null;
        #endregion

        #region constructor

        public PrintQueueMonitor(string strSpoolName)
        {
            // Let us open the printer and get the printer handle.
            _spoolerName = strSpoolName;
            //Start Monitoring
            Start(strSpoolName);

        }
        #endregion

        #region destructor
        ~PrintQueueMonitor()
        {
            Stop();
        }
        #endregion

        #region StartMonitoring
        public void Start(string printer)
        {
            if (_spoolerName != printer) return;

            OpenPrinter(_spoolerName, out _printerHandle, IntPtr.Zero);
            if (_printerHandle != IntPtr.Zero)
            {
                //We got a valid Printer handle.  Let us register for change notification....
                _changeHandle = FindFirstPrinterChangeNotification(_printerHandle, (int)PRINTER_CHANGES.PRINTER_CHANGE_JOB, 0, _notifyOptions);
                // We have successfully registered for change notification.  Let us capture the handle...
                _mrEvent.Handle = _changeHandle;
                //Now, let us wait for change notification from the printer queue...
                _waitHandle = ThreadPool.RegisterWaitForSingleObject(_mrEvent, new WaitOrTimerCallback(PrinterNotifyWaitCallback), _mrEvent, -1, false);
            }

            _spooler = new PrintQueue(new PrintServer(), _spoolerName);
            foreach (PrintSystemJobInfo psi in _spooler.GetPrintJobInfoCollection())
            {
                objJobDict[psi.JobIdentifier] = psi.Name;
            }
        }
        #endregion

        #region StopMonitoring
        public void Stop()
        {
            if (_printerHandle != IntPtr.Zero)
            {
                ClosePrinter((IntPtr)_printerHandle);
                _printerHandle = IntPtr.Zero;
            }
        }
        #endregion


        #region Callback Function
        public void PrinterNotifyWaitCallback(object state, bool timedOut)
        {
            if (_printerHandle == IntPtr.Zero) return;
            #region read notification details
            _notifyOptions.Count = 1;
            int pdwChange = 0;
            IntPtr pNotifyInfo = IntPtr.Zero;
            try
            {
                bool bResult = FindNextPrinterChangeNotification(_changeHandle, out pdwChange, _notifyOptions, out pNotifyInfo);
                //If the Printer Change Notification Call did not give data, exit code
                //if (bResult == false || (int)pNotifyInfo == 0) return;
                if (bResult == false || pNotifyInfo == IntPtr.Zero) return;
                //If the Change Notification was not relgated to job, exit code
                bool bJobRelatedChange = (pdwChange & PRINTER_CHANGES.PRINTER_CHANGE_ADD_JOB) == PRINTER_CHANGES.PRINTER_CHANGE_ADD_JOB ||
                                         (pdwChange & PRINTER_CHANGES.PRINTER_CHANGE_SET_JOB) == PRINTER_CHANGES.PRINTER_CHANGE_SET_JOB ||
                                         (pdwChange & PRINTER_CHANGES.PRINTER_CHANGE_DELETE_JOB) == PRINTER_CHANGES.PRINTER_CHANGE_DELETE_JOB ||
                                         (pdwChange & PRINTER_CHANGES.PRINTER_CHANGE_WRITE_JOB) == PRINTER_CHANGES.PRINTER_CHANGE_WRITE_JOB;
                if (!bJobRelatedChange) return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace);
            }
            #endregion 

            #region populate Notification Information
            try
            {
                //Now, let us initialize and populate the Notify Info data
                PRINTER_NOTIFY_INFO info = (PRINTER_NOTIFY_INFO)Marshal.PtrToStructure(pNotifyInfo, typeof(PRINTER_NOTIFY_INFO));
                long pData = (long)pNotifyInfo + (long)Marshal.OffsetOf(typeof(PRINTER_NOTIFY_INFO), "aData");
                PRINTER_NOTIFY_INFO_DATA[] data = new PRINTER_NOTIFY_INFO_DATA[info.Count];
                for (uint i = 0; i < info.Count; i++)
                {
                    data[i] = (PRINTER_NOTIFY_INFO_DATA)Marshal.PtrToStructure((IntPtr)pData, typeof(PRINTER_NOTIFY_INFO_DATA));
                    pData += Marshal.SizeOf(typeof(PRINTER_NOTIFY_INFO_DATA));
                }

                #region iterate through all elements in the data array
                for (int i = 0; i < data.Count(); i++)
                {
                    if (data[i].Field == (ushort)PRINTERJOBNOTIFICATIONTYPES.JOB_NOTIFY_FIELD_STATUS &&
                        data[i].Type == (ushort)PRINTERNOTIFICATIONTYPES.JOB_NOTIFY_TYPE)
                    {
                        JOBSTATUS jStatus = (JOBSTATUS)Enum.Parse(typeof(JOBSTATUS), data[i].NotifyData.Data.cbBuf.ToString());
                        int intJobID = (int)data[i].Id;
                        string strJobName = "";
                        PrintSystemJobInfo pji = null;
                        try
                        {
                            _spooler = new PrintQueue(new PrintServer(), _spoolerName);
                            pji = _spooler.GetJob(intJobID);
                            if (!objJobDict.ContainsKey(intJobID))
                                objJobDict[intJobID] = pji.Name;
                            strJobName = pji.Name;
                        }
                        catch
                        {
                            pji = null;
                            objJobDict.TryGetValue(intJobID, out strJobName);
                            if (strJobName == null) strJobName = "";
                        }

                        if (OnJobStatusChange != null)
                        {
                            //Let us raise the event
                            OnJobStatusChange(this, new PrintJobChangeEventArgs(intJobID, strJobName, jStatus, pji));
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace);
            }
            #endregion

            #region reset the Event and wait for the next event
            try
            {
                _mrEvent.Reset();
                _waitHandle = ThreadPool.RegisterWaitForSingleObject(_mrEvent, new WaitOrTimerCallback(PrinterNotifyWaitCallback), _mrEvent, -1, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace);
            }
            #endregion 

        }
        #endregion

        #region PrintJobHelper

        [DllImport("winspool.drv", EntryPoint = "GetJob", 
            SetLastError = true, CharSet = CharSet.Auto, 
            ExactSpelling = false, 
            CallingConvention = CallingConvention.StdCall)]
        public static extern bool GetJob([In] IntPtr hPrinter, [In] Int32 dwJobId, [In] Int32 Level, 
            [Out] byte[] lpJob, 
            [In] Int32 cbBuf, 
            ref Int32 lpbSizeNeeded);

        public int GetNumberOfCopies(int jobId)
        {
            int copies = 1;
            try
            {
                var BytesWritten = new Int32();
                var ptBuf = new byte[0];

                if (_printerHandle == IntPtr.Zero)
                {
                    MessageBox.Show("Printer not found!");
                    return 1;
                }

                // Get required buffer size
                if (!GetJob(_printerHandle, jobId, 2, ptBuf, 0, ref BytesWritten))
                {
                    if (BytesWritten == 0)
                    {
                        var ex = new Win32Exception();
                            throw new Exception("Job not found or access denied");
                    }
                }

                // \\ Allocate a buffer the right size
                if (BytesWritten > 0)
                    ptBuf = new byte[BytesWritten];

                // Allocate buffer and retrieve job info
                IntPtr pJob = Marshal.AllocHGlobal(BytesWritten);
                try
                {
                    if (!GetJob(_printerHandle, jobId, 2, ptBuf, BytesWritten, ref BytesWritten))
                    {
                        throw new Exception("GetJob for JOB_INFO_2 failed on handle: " + _printerHandle.ToString() + " for job: " + jobId + " and type: " + GetType().ToString());
                    }
                    else
                    {
                        GCHandle handle = GCHandle.Alloc(ptBuf, GCHandleType.Pinned);
                        JOB_INFO_2 jobInfo = (JOB_INFO_2)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(JOB_INFO_2));
                        handle.Free();
                        //Marshal.PtrToStructure(ptBuf, this);

                        //JOB_INFO_2 jobInfo = Marshal.PtrToStructure<JOB_INFO_2>(ptBuf, typeof(JOB_INFO_2));

                        // Extract copies from DEVMODE
                        if (jobInfo.pDevMode != IntPtr.Zero)
                        {
                            DEVMODE devMode = Marshal.PtrToStructure<DEVMODE>(jobInfo.pDevMode);
                            //MessageBox.Show(devMode.ToString());
                            return devMode.dmCopies;
                        }
                    }

                    //if (!GetJob(_printerHandle, (uint)jobId, 2, pJob, bytesNeeded, out _))
                    //    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());

                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    Marshal.FreeHGlobal(pJob);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            MessageBox.Show("Default to 1 copy if DEVMODE is unavailable");
            return copies;
        }

        #endregion

    }
}
