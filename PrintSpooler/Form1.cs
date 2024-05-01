using PrintSpooler.Utilities;
using System.Printing;
using System.Timers;


namespace PrintSpooler
{
    public partial class Form1 : Form
    {
        List<PausedJob> pausedJobs = new List<PausedJob>();
        PrintQueueMonitor pqm;
        System.Timers.Timer timer;

        public Form1()
        {
            InitializeComponent();
            timer = new(1000 * 1);
            //timer.Enabled = true;
            timer.Elapsed += new ElapsedEventHandler(Timer_Tick);
            //timer.AutoReset = true;
            //timer.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            PrintServer ps = new PrintServer();
            foreach (PrintQueue pq in ps.GetPrintQueues())
                cmbPrinters.Items.Add(pq.Name);
        }

        private void btnMonitor_Click(object sender, EventArgs e)
        {
            if (cmbPrinters.Enabled)
            {
                if (cmbPrinters.Text.Trim() == "") return;
                pqm = new PrintQueueMonitor(cmbPrinters.Text.Trim());
                pqm.OnJobStatusChange += new PrintJobStatusChanged(pqm_OnJobStatusChange);
                cmbPrinters.Enabled = false;
                btnMonitor.Text = "Stop Monitoring";
                if (!timer.Enabled)
                {
                    timer.Enabled = true;
                    timer.Start();
                }
            }
            else
            {
                if (pqm != null) pqm.Stop();
                pqm = null;
                cmbPrinters.Enabled = true;
                btnMonitor.Text = "Start Monitoring";
                if (timer.Enabled)
                {
                    timer.Stop();
                    timer.Enabled = false;
                }
                    
            }
        }

        void pqm_OnJobStatusChange(object Sender, PrintJobChangeEventArgs e)
        {
            try
            {
                PrintSystemJobInfo job = e.JobInfo;

                MethodInvoker invoker = () =>
                {
                    lbSpoolChanges.Items.Add(e.JobID + " - " + e.JobName + " - " + e.JobStatus);
                };
                if (lbSpoolChanges.InvokeRequired && job != null)
                {
                    Invoke(invoker);
                }
                else
                {
                    invoker();
                }

                if (e.JobStatus == JOBSTATUS.JOB_STATUS_SPOOLING && job != null)
                {
                    job.Pause();
                    pausedJobs.Add(new PausedJob(e.JobID, job));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        private void Timer_Tick(object? sender, ElapsedEventArgs e)
        {
            try
            {
                if(pausedJobs.Count > 0)
                {
                    foreach (PausedJob job in pausedJobs.ToList())
                    {
                        if (job.PausedTime <= 0)
                        {
                            ResumeJobInQueue(job.JobID);
                            pausedJobs.Remove(job);
                        }
                        else
                        {
                            job.UpdateTime();
                        }
                    }
                }
                else
                {
                    PrintServer ps = new();
                    foreach (PrintQueue pq in ps.GetPrintQueues())
                    {
                        pq.Refresh();
                    }
                }
            }
            catch (Exception)
            {
                throw;
                //MessageBox.Show(ex.StackTrace);
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

    }
}
