using PrintSpooler.Utilities;
using System.Printing;

namespace PrintSpooler
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());

            // Obtain a list of print servers.
            //Console.Write("Enter path and file name of CRLF-delimited list of print servers" +
            //    "\n(press Return for default \"C:\\PrintServers.txt\"): ");
            //String pathToListOfPrintServers = Console.ReadLine();
            //if (pathToListOfPrintServers == "")
            //{
            //    pathToListOfPrintServers = @"C:\PrintServers.txt";
            //}
            //StreamReader fileOfPrintServers = new StreamReader(pathToListOfPrintServers);

            //// Obtain the username of the person with the problematic print job.
            //Console.Write("\nEnter username of person that submitted print job" +
            //    "\n(press Return for the current user {0}: ", Environment.UserName);
            //String userName = Console.ReadLine();
            //if (userName == "")
            //{
            //    userName = Environment.UserName;
            //}

            //// Prompt user to determine the method that will be used to read the queue status.
            //Console.Write("\nEnter \"Y\" to check the problematic job using its JobStatus attributes." +
            //    "\nOtherwise, press Return and the job will be checked using its specific properties: ");
            //String useAttributesResponse = Console.ReadLine();

            //// Create list of all jobs submitted by user.
            //String line;
            //Boolean atLeastOne = false;
            //String jobList = "\n\nAll print jobs submitted by the user are listed here:\n\n";
            //while ((line = fileOfPrintServers.ReadLine()) != null)
            //{
            //    PrintServer myPS = new PrintServer(line, PrintSystemDesiredAccess.AdministrateServer);
            //    PrintQueueCollection myPrintQueues = myPS.GetPrintQueues();

            //    //<SnippetEnumerateJobsInQueues>
            //    foreach (PrintQueue pq in myPrintQueues)
            //    {
            //        pq.Refresh();
            //        PrintJobInfoCollection jobs = pq.GetPrintJobInfoCollection();
            //        foreach (PrintSystemJobInfo job in jobs)
            //        {
            //            // Since the user may not be able to articulate which job is problematic,
            //            // present information about each job the user has submitted.
            //            if (job.Submitter == userName)
            //            {
            //                atLeastOne = true;
            //                jobList = jobList + "\nServer:" + line;
            //                jobList = jobList + "\n\tQueue:" + pq.Name;
            //                jobList = jobList + "\n\tLocation:" + pq.Location;
            //                jobList = jobList + "\n\t\tJob: " + job.JobName + " ID: " + job.JobIdentifier;
            //            }
            //        }// end for each print job    

            //    }// end for each print queue
            //    //</SnippetEnumerateJobsInQueues>
            //}// end while list of print servers is not yet exhausted

            //fileOfPrintServers.Close();

            //if (!atLeastOne)
            //{
            //    jobList = "\n\nNo jobs submitted by " + userName + " were found.\n\n";
            //    Console.WriteLine(jobList);
            //}
            //else
            //{
            //    jobList = jobList + "\n\nIf multiple jobs are listed, use the information provided" +
            //        " above and by the user to identify the job needing diagnosis.\n\n";
            //    Console.WriteLine(jobList);
            //    //<SnippetIdentifyAndDiagnoseProblematicJob>
            //    // When the problematic print job has been identified, enter information about it.
            //    Console.Write("\nEnter the print server hosting the job (including leading slashes \\\\): " +
            //    "\n(press Return for the current computer \\\\{0}): ", Environment.MachineName);
            //    String pServer = Console.ReadLine();
            //    if (pServer == "")
            //    {
            //        pServer = "\\\\" + Environment.MachineName;
            //    }
            //    Console.Write("\nEnter the print queue hosting the job: ");
            //    String pQueue = Console.ReadLine();
            //    Console.Write("\nEnter the job ID: ");
            //    Int16 jobID = Convert.ToInt16(Console.ReadLine());

            //    // Create objects to represent the server, queue, and print job.
            //    PrintServer hostingServer = new PrintServer(pServer, PrintSystemDesiredAccess.AdministrateServer);
            //    PrintQueue hostingQueue = new PrintQueue(hostingServer, pQueue, PrintSystemDesiredAccess.AdministratePrinter);
            //    PrintSystemJobInfo theJob = hostingQueue.GetJob(jobID);

            //    if (useAttributesResponse == "Y")
            //    {
            //        TroubleSpotter.SpotTroubleUsingJobAttributes(theJob);
            //        // TroubleSpotter class is defined in the complete example.
            //    }
            //    else
            //    {
            //        TroubleSpotter.SpotTroubleUsingProperties(theJob);
            //    }

            //    TroubleSpotter.ReportQueueAndJobAvailability(theJob);
            //    //</SnippetIdentifyAndDiagnoseProblematicJob>           
            //}// end else at least one job was submitted by user

            //// End the program
            //Console.WriteLine("\nPress Return to end.");
            //Console.ReadLine();

        }// end Main


    }
}
