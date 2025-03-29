using System.Drawing.Printing;
using System.Management;
using System.Printing;
using System.Text.Json.Serialization;
using System.Timers;
using System.Windows.Threading;

namespace PrintSpooler.Utilities
{
    internal class MonitorPrinter
    {
        List<int> pausedJobIds = new();
        PrintQueueMonitor pqm;
        System.Timers.Timer timer;
        PrinterSettings printerSettings;
        readonly public string printer;
        IntPtr devmode = IntPtr.Zero;

        Form1 form;

        public MonitorPrinter(string printer, Form1 form)
        {
            this.printer = printer;
            this.form = form;
            pqm = new PrintQueueMonitor(printer);
            pqm.OnJobStatusChange += new PrintJobStatusChanged(pqm_OnJobStatusChange);
            printerSettings = new PrinterSettings();
            printerSettings.PrinterName = printer;
            devmode = printerSettings.GetHdevmode(new PageSettings());

            timer = new(1000 * 1);
            timer.Elapsed += new ElapsedEventHandler(Timer_Tick);
            timer.Start();
        }

        public void Stop()
        {
            pqm.OnJobStatusChange -= pqm_OnJobStatusChange;
            pqm = null;
        }

        private void Timer_Tick(object? sender, ElapsedEventArgs e)
        {
            try
            {
                if (pausedJobIds.Count > 0)
                {
                    timer.Stop();

                    foreach (int jobId in pausedJobIds.ToList())
                    {
                        PrintSystemJobInfo job = GetJobInQueue(jobId);
                        if (job == null) continue;
                        job.Refresh();
                        if (job.IsPaused && !job.IsSpooling)
                        {
                            PrintQueue pq = job.HostingPrintQueue;
                            pq.Refresh();
                            if (JobDetails(printer, job.JobIdentifier))
                            {
                                job.Resume();
                                pausedJobIds.Remove(jobId);
                            }
                            else
                            {
                                job.Cancel();
                                pausedJobIds.Remove(jobId);
                            }
                        }
                    }

                    timer.Start();
                }
            }
            catch (Exception ex)
            {
                //throw;
                MessageBox.Show(ex.StackTrace);
                timer.Stop();
            }
        }

        private void RefreshPrintQueue(PrintSystemJobInfo job)
        {
            PrintQueue pq = job.HostingPrintQueue;
            pq.Refresh();
        }

        void pqm_OnJobStatusChange(object Sender, PrintJobChangeEventArgs e)
        {
            try
            {
                PrintSystemJobInfo job = e.JobInfo;

                if (job == null) return;

                form.UpdatePrintInfoList(job, e);
                
                if (job.IsSpooling && !job.IsPaused)
                {
                    job.Pause();
                    if(!pausedJobIds.Any(i => i == job.JobIdentifier))
                        pausedJobIds.Add(e.JobID);
                }

            }
            catch (Exception ex)
            {
                //throw;
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        private static void ResumeJobInQueue(int JobID)
        {
            try
            {
                PrintServer ps = new();
                foreach (PrintQueue pq in ps.GetPrintQueues())
                {
                    pq.Refresh();
                    PrintJobInfoCollection jobs = pq.GetPrintJobInfoCollection();
                    foreach (PrintSystemJobInfo job in jobs)
                    {

                        // Since the user may not be able to articulate which job is problematic,
                        // present information about each job the user has submitted.
                        if (job.JobIdentifier == JobID && job.IsPaused)
                        {
                            job.Resume();
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }
        }

        private static PrintSystemJobInfo GetJobInQueue(int JobID)
        {
            try
            {
                PrintServer ps = new();
                foreach (PrintQueue pq in ps.GetPrintQueues())
                {
                    pq.Refresh();
                    PrintJobInfoCollection jobs = pq.GetPrintJobInfoCollection();
                    foreach (PrintSystemJobInfo job in jobs)
                    {

                        // Since the user may not be able to articulate which job is problematic,
                        // present information about each job the user has submitted.
                        if (job.JobIdentifier == JobID)
                        {
                            return job;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }
            return null;
        }

        #region DeletePrintJob

        /// <summary>
        /// Cancel the print job. This functions accepts the job number.
        /// An exception will be thrown if access denied.
        /// </summary>
        /// <param name="printJobID">int: Job number to cancel printing for.</param>
        /// <returns>bool: true if cancel successfull, else false.</returns>
        public bool DeletePrintJob(int printJobID)
        {
            // Variable declarations.
            bool isActionPerformed = false;
            string searchQuery;
            String jobName;
            char[] splitArr;
            int prntJobID;
            ManagementObjectSearcher searchPrintJobs;
            ManagementObjectCollection prntJobCollection;
            try
            {
                // Query to get all the queued printer jobs.
                searchQuery = "SELECT * FROM Win32_PrintJob";
                // Create an object using the above query.
                searchPrintJobs = new ManagementObjectSearcher(searchQuery);
                // Fire the query to get the collection of the printer jobs.
                prntJobCollection = searchPrintJobs.Get();

                // Look for the job you want to delete/cancel.
                foreach (ManagementObject prntJob in prntJobCollection)
                {
                    jobName = prntJob.Properties["Name"].Value.ToString();
                    // Job name would be of the format [Printer name], [Job ID]
                    splitArr = new char[1];
                    splitArr[0] = Convert.ToChar(",");
                    // Get the job ID.
                    prntJobID = Convert.ToInt32(jobName.Split(splitArr)[1]);
                    // If the Job Id equals the input job Id, then cancel the job.
                    if (prntJobID == printJobID)
                    {
                        // Performs a action similar to the cancel
                        // operation of windows print console
                        prntJob.Delete();
                        isActionPerformed = true;
                        break;
                    }
                }
                return isActionPerformed;
            }
            catch (Exception ex)
            {
                //Logger.Error(ex);
                return false;
            }
        }

        #endregion DeletePrintJob

        public bool JobDetails(string printerName, int printJobID)
        {
            bool isContinuePrinting = false;
            try
            {
                string searchQuery = "SELECT * FROM Win32_PrintJob";
                ManagementObjectSearcher searchPrintJobs = new ManagementObjectSearcher(searchQuery);
                ManagementObjectCollection prntJobCollection = searchPrintJobs.Get();

                foreach (ManagementObject prntJob in prntJobCollection)
                {
                    System.String jobName = prntJob.Properties["Name"].Value.ToString();

                    //Job name would be of the format [Printer name], [Job ID]
                    char[] splitArr = new char[1];
                    splitArr[0] = Convert.ToChar(",");
                    string prnterName = jobName.Split(splitArr)[0];
                    int prntJobID = Convert.ToInt32(jobName.Split(splitArr)[1]);
                    string documentName = prntJob.Properties["Document"].Value.ToString();

                    if (String.Compare(prnterName, printerName, true) == 0)
                    {
                        if (prntJobID == printJobID)
                        {
                            // MessageBox.Show("PAGINAS : " + prntJob.Properties["TotalPages"].Value.ToString() + documentName + " " + prntJobID);
                            //prntJob.InvokeMethod("Pause", null);

                            string jobStatus = prntJob.Properties["JobStatus"].Value + "";
                            uint totalPages = (uint)prntJob.Properties["TotalPages"].Value;
                            string color = (prntJob.Properties["Color"].Value+"" == "Color") ? "Color" : "Black and white";
                            int copies = pqm.GetNumberOfCopies(prntJobID);

                            string jobDetails = string.Format(
                                    "Job Status: {0}\n" +
                                    "Printer: {1}\n" +
                                    "No. of Pages: {2}\n" +
                                    "No. of Copies: {3}\n" +
                                    "Color: {4}\n" +
                                    "\nWould you like to continue printing?",
                                    jobStatus,
                                    prnterName,
                                    totalPages,
                                    copies,
                                    color);

                            DialogResult result = MessageBox.Show(jobDetails, "Job Details", MessageBoxButtons.YesNo);
                            if (result == DialogResult.Yes)
                            {
                                isContinuePrinting = true;
                            }

                            //prntJob.InvokeMethod("Resume", null);
                            
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
                //MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            
            return isContinuePrinting;
        }

    }
}
