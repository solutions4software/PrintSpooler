using PrintSpooler.Utilities;
using System.Drawing.Printing;
using System.Printing;
using System.Windows.Threading;

namespace PrintSpooler
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            PrintServer ps = new PrintServer();

            foreach (PrintQueue pq in ps.GetPrintQueues())
            {
                lstConectedPrinters.Items.Add(pq.Name);
            }

        }

        public void UpdatePrintInfoList(PrintSystemJobInfo theJob, PrintJobChangeEventArgs e)
        {
            try
            {

                MethodInvoker invoker = () =>
                {
                    lbSpoolChanges.Items.Add(e.JobID + " - " + e.JobName + " - " + e.JobStatus);
                    

                };
                if (lbSpoolChanges.InvokeRequired && theJob != null)
                {
                    Invoke(invoker);
                    
                    
                }
                else
                {
                    invoker();
                }

                

            }
            catch (Exception)
            {

            }
        }

        public void SpotTroubleUsingJobAttributes(PrintSystemJobInfo theJob)
        {
            Dispatcher.CurrentDispatcher.Invoke(delegate 
            {

                if ((theJob.JobStatus & PrintJobStatus.Blocked) == PrintJobStatus.Blocked)
                {
                    lbSpoolChanges.Items.Add("The job is blocked.");
                }
                if (((theJob.JobStatus & PrintJobStatus.Completed) == PrintJobStatus.Completed)
                    ||
                    ((theJob.JobStatus & PrintJobStatus.Printed) == PrintJobStatus.Printed))
                {
                    lbSpoolChanges.Items.Add("The job has finished. Have user recheck all output bins and be sure the correct printer is being checked.");
                }
                if (((theJob.JobStatus & PrintJobStatus.Deleted) == PrintJobStatus.Deleted)
                    ||
                    ((theJob.JobStatus & PrintJobStatus.Deleting) == PrintJobStatus.Deleting))
                {
                    lbSpoolChanges.Items.Add("The user or someone with administration rights to the queue has deleted the job. It must be resubmitted.");
                }
                if ((theJob.JobStatus & PrintJobStatus.Error) == PrintJobStatus.Error)
                {
                    lbSpoolChanges.Items.Add("The job has errored.");
                }
                if ((theJob.JobStatus & PrintJobStatus.Offline) == PrintJobStatus.Offline)
                {
                    lbSpoolChanges.Items.Add("The printer is offline. Have user put it online with printer front panel.");
                }
                if ((theJob.JobStatus & PrintJobStatus.PaperOut) == PrintJobStatus.PaperOut)
                {
                    lbSpoolChanges.Items.Add("The printer is out of paper of the size required by the job. Have user add paper.");
                }

                if (((theJob.JobStatus & PrintJobStatus.Paused) == PrintJobStatus.Paused)
                    ||
                    ((theJob.HostingPrintQueue.QueueStatus & PrintQueueStatus.Paused) == PrintQueueStatus.Paused))
                {
                    lbSpoolChanges.Items.Add("The job has paused.");
                    //HandlePausedJob(theJob);
                    //HandlePausedJob is defined in the complete example.
                }

                if ((theJob.JobStatus & PrintJobStatus.Printing) == PrintJobStatus.Printing)
                {
                    lbSpoolChanges.Items.Add("The job is printing now.");
                }
                if ((theJob.JobStatus & PrintJobStatus.Spooling) == PrintJobStatus.Spooling)
                {
                    lbSpoolChanges.Items.Add("The job is spooling now.");
                }
                if ((theJob.JobStatus & PrintJobStatus.UserIntervention) == PrintJobStatus.UserIntervention)
                {
                    lbSpoolChanges.Items.Add("The printer needs human intervention.");
                }

            });
            
        }//end SpotTroubleUsingJobAttributes

        internal static void HandlePausedJob(PrintSystemJobInfo theJob)
        {
            // If there's no good reason for the queue to be paused, resume it and 
            // give user choice to resume or cancel the job.
            //Console.WriteLine("The user or someone with administrative rights to the queue" +
            //     "\nhas paused the job or queue." +
            //     "\nResume the queue? (Has no effect if queue is not paused.)" +
            //     "\nEnter \"Y\" to resume, otherwise press return: ");
            //String resume = Console.ReadLine();
            if (MonitorPrinter.JobDetails(theJob.HostingPrintQueue.Name, theJob.JobIdentifier))
            {
                theJob.HostingPrintQueue.Resume();
                theJob.Resume();
                // It is possible the job is also paused. Find out how the user wants to handle that.
                //Console.WriteLine("Does user want to resume print job or cancel it?" +
                //    "\nEnter \"Y\" to resume (any other key cancels the print job): ");
                //String userDecision = Console.ReadLine();
                //if (userDecision == "Y")
                //{
                //    theJob.Resume();
                //}

            }//end if the queue should be resumed
            else
            {
                theJob.Cancel();
            }

        }//end HandlePausedJob

        List<MonitorPrinter> monitorPrinters = new List<MonitorPrinter>();

        private void btnAddToMonitorList_Click(object sender, EventArgs e)
        {
            if (lstConectedPrinters.Items.Count > 0)
            {
                if(lstConectedPrinters.SelectedIndex > -1)
                {
                    string? selectedPrinter = lstConectedPrinters.SelectedItem.ToString();
                    if (selectedPrinter == null) return;
                    
                    lstMonitorPrinters.Items.Add(selectedPrinter);
                    lstConectedPrinters.Items.Remove(selectedPrinter);

                    monitorPrinters.Add(new MonitorPrinter(selectedPrinter, this));

                    var server = new LocalPrintServer();

                    //Load queue for correct printer
                    PrintQueue queue = server.GetPrintQueue(selectedPrinter, new string[0] { });

                    PrinterSettings ps = new PrinterSettings();
                    ps.PrinterName = selectedPrinter;

                    PrintServer psrvr = queue.HostingPrintServer;
                    
                    string supportsColor = queue.GetPrintCapabilities().OutputColorCapability.Contains(OutputColor.Color)
                        ? "Yes" : "No";
                    string papers = ps.PrintRange.ToString();
                    string tonor = queue.HasToner ? queue.IsTonerLow ? "Low" : "Full" : "No";
                    string drum = queue.HasPaperProblem.ToString();
                    string status = queue.QueueStatus.ToString();

                    printerGridView.Rows.Add(selectedPrinter, supportsColor, papers, tonor, drum, status);
                }
            }
        }

        private void btnRemoveFromList_Click(object sender, EventArgs e)
        {
            if (lstMonitorPrinters.Items.Count > 0)
            {
                if (lstMonitorPrinters.SelectedIndex > -1)
                {
                    string? selectedPrinter = lstMonitorPrinters.SelectedItem.ToString();
                    if(selectedPrinter != null)
                    {
                        lstMonitorPrinters.Items.Remove(selectedPrinter);
                        lstConectedPrinters.Items.Add(selectedPrinter);
                    }
                    
                    MonitorPrinter? monitorPrinter = monitorPrinters.FirstOrDefault(p => p.printer == selectedPrinter);
                    if(monitorPrinter != null)
                    {
                        monitorPrinter.Stop();
                        monitorPrinters.Remove(monitorPrinter);
                    }
                    
                    foreach (DataGridViewRow row in printerGridView.Rows)
                    {
                        string printer = row.Cells[0].Value+"";
                        if(printer == selectedPrinter)
                        {
                            printerGridView.Rows.Remove(row);
                            printerGridView.Refresh();
                            break;
                        }
                    }

                }
            }
        }

    }
}
