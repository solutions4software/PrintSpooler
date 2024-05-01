using System.Printing;

namespace PrintSpooler.Utilities
{
    internal class PausedJob
    {
        public int JobID { get; private set; }
        public PrintSystemJobInfo JobInfo { get; private set; }
        public long PausedTime { get; private set; }
        
        public PausedJob(int jobID, PrintSystemJobInfo jobInfo)
        {
            JobID = jobID;
            JobInfo = jobInfo;
            PausedTime = 20 * 1000;
        }
        public PausedJob(int jobID, PrintSystemJobInfo jobInfo, long pausedTime)
        {
            JobID = jobID;
            JobInfo = jobInfo;
            PausedTime = pausedTime;
        }

        public void UpdateTime()
        {
            if (PausedTime > 0)
            {
                PausedTime -= 1000;
            } 
        }
    }
}
