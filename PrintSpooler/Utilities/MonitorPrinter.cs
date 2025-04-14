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

        Form1 form;

        public MonitorPrinter(string printer, Form1 form)
        {
            this.printer = printer;
            this.form = form;
            pqm = new PrintQueueMonitor(printer);
            pqm.OnJobStatusChange += new PrintJobStatusChanged(pqm_OnJobStatusChange);
            printerSettings = new PrinterSettings();
            printerSettings.PrinterName = printer;

            timer = new(1000 * 1);
            timer.Elapsed += new ElapsedEventHandler(Timer_Tick);
            //timer.Start();
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
                            PrintJobDetails jobDetails = pqm.GetJobDetails(jobId, job.JobStatus.ToString(), printer);
                            if (ShowJobDetails(jobDetails))
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
                timer.Stop();
                throw;
                //MessageBox.Show(ex.StackTrace);
            }
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

                    //if(!pausedJobs.Contains(job))
                    //{
                    //    pausedJobs.Add(job);
                    //}
                    //if(!pausedJobIds.Any(i => i == job.JobIdentifier))
                    //    pausedJobIds.Add(e.JobID);
                }
                else if (job.IsPaused && !job.IsSpooling)
                {
                    PrintJobDetails jobDetails = pqm.GetJobDetails(job.JobIdentifier, job.JobStatus.ToString(), printer);
                    if (ShowJobDetails(jobDetails))
                    {
                        job.Resume();
                    }
                    else
                    {
                        job.Cancel();
                    }
                }

            }
            catch (Exception ex)
            {
                //throw;
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        private PrintSystemJobInfo GetJobInQueue(int JobID)
        {
            PrintSystemJobInfo? job = null;
            try
            {
                PrintQueue pq = new(new PrintServer(), printer);
                job = pq.GetJob(JobID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return job;
        }

        private bool ShowJobDetails(PrintJobDetails pJobDetails)
        {
            if (pJobDetails.JobID == -1) return false;
            string jobDetails = string.Format(
                                    "Job Status: {0}\n" +
                                    "Printer: {1}\n" +
                                    "No. of Pages: {2}\n" +
                                    "No. of Copies: {3}\n" +
                                    "Color: {4}\n" +
                                    "\nWould you like to continue printing?",
                                    pJobDetails.JobStatus,
                                    pJobDetails.PrinterName,
                                    pJobDetails.NoOfPages,
                                    pJobDetails.NoOfCopies,
                                    pJobDetails.Color);

            DialogResult result = MessageBox.Show(jobDetails, "Job Details", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                return true;
            }
            return false;
        } 

    }
}
