using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;

namespace PrintSpooler.Utilities
{
    class TroubleSpotter
    {
        // <SnippetSpotTroubleUsingJobProperties>
        // Check for possible trouble states of a print job using its properties
        internal static void SpotTroubleUsingProperties(PrintSystemJobInfo theJob)
        {
            if (theJob.IsBlocked)
            {
                Console.WriteLine("The job is blocked.");
            }
            if (theJob.IsCompleted || theJob.IsPrinted)
            {
                Console.WriteLine("The job has finished. Have user recheck all output bins and be sure the correct printer is being checked.");
            }
            if (theJob.IsDeleted || theJob.IsDeleting)
            {
                Console.WriteLine("The user or someone with administration rights to the queue has deleted the job. It must be resubmitted.");
            }
            if (theJob.IsInError)
            {
                Console.WriteLine("The job has errored.");
            }
            if (theJob.IsOffline)
            {
                Console.WriteLine("The printer is offline. Have user put it online with printer front panel.");
            }
            if (theJob.IsPaperOut)
            {
                Console.WriteLine("The printer is out of paper of the size required by the job. Have user add paper.");
            }

            if (theJob.IsPaused || theJob.HostingPrintQueue.IsPaused)
            {
                HandlePausedJob(theJob);
                //HandlePausedJob is defined in the complete example.
            }

            if (theJob.IsPrinting)
            {
                Console.WriteLine("The job is printing now.");
            }
            if (theJob.IsSpooling)
            {
                Console.WriteLine("The job is spooling now.");
            }
            if (theJob.IsUserInterventionRequired)
            {
                Console.WriteLine("The printer needs human intervention.");
            }

        }//end SpotTroubleUsingProperties
        // </SnippetSpotTroubleUsingJobProperties>

        // <SnippetSpotTroubleUsingJobAttributes>
        // Check for possible trouble states of a print job using the flags of the JobStatus property
        internal static void SpotTroubleUsingJobAttributes(PrintSystemJobInfo theJob)
        {
            if ((theJob.JobStatus & PrintJobStatus.Blocked) == PrintJobStatus.Blocked)
            {
                Console.WriteLine("The job is blocked.");
            }
            if (((theJob.JobStatus & PrintJobStatus.Completed) == PrintJobStatus.Completed)
                ||
                ((theJob.JobStatus & PrintJobStatus.Printed) == PrintJobStatus.Printed))
            {
                Console.WriteLine("The job has finished. Have user recheck all output bins and be sure the correct printer is being checked.");
            }
            if (((theJob.JobStatus & PrintJobStatus.Deleted) == PrintJobStatus.Deleted)
                ||
                ((theJob.JobStatus & PrintJobStatus.Deleting) == PrintJobStatus.Deleting))
            {
                Console.WriteLine("The user or someone with administration rights to the queue has deleted the job. It must be resubmitted.");
            }
            if ((theJob.JobStatus & PrintJobStatus.Error) == PrintJobStatus.Error)
            {
                Console.WriteLine("The job has errored.");
            }
            if ((theJob.JobStatus & PrintJobStatus.Offline) == PrintJobStatus.Offline)
            {
                Console.WriteLine("The printer is offline. Have user put it online with printer front panel.");
            }
            if ((theJob.JobStatus & PrintJobStatus.PaperOut) == PrintJobStatus.PaperOut)
            {
                Console.WriteLine("The printer is out of paper of the size required by the job. Have user add paper.");
            }

            if (((theJob.JobStatus & PrintJobStatus.Paused) == PrintJobStatus.Paused)
                ||
                ((theJob.HostingPrintQueue.QueueStatus & PrintQueueStatus.Paused) == PrintQueueStatus.Paused))
            {
                HandlePausedJob(theJob);
                //HandlePausedJob is defined in the complete example.
            }

            if ((theJob.JobStatus & PrintJobStatus.Printing) == PrintJobStatus.Printing)
            {
                Console.WriteLine("The job is printing now.");
            }
            if ((theJob.JobStatus & PrintJobStatus.Spooling) == PrintJobStatus.Spooling)
            {
                Console.WriteLine("The job is spooling now.");
            }
            if ((theJob.JobStatus & PrintJobStatus.UserIntervention) == PrintJobStatus.UserIntervention)
            {
                Console.WriteLine("The printer needs human intervention.");
            }

        }//end SpotTroubleUsingJobAttributes
        // </SnippetSpotTroubleUsingJobAttributes>

        //<SnippetHandlePausedJob>
        internal static void HandlePausedJob(PrintSystemJobInfo theJob)
        {
            // If there's no good reason for the queue to be paused, resume it and 
            // give user choice to resume or cancel the job.
            Console.WriteLine("The user or someone with administrative rights to the queue" +
                 "\nhas paused the job or queue." +
                 "\nResume the queue? (Has no effect if queue is not paused.)" +
                 "\nEnter \"Y\" to resume, otherwise press return: ");
            String resume = Console.ReadLine();
            if (resume == "Y")
            {
                theJob.HostingPrintQueue.Resume();

                // It is possible the job is also paused. Find out how the user wants to handle that.
                Console.WriteLine("Does user want to resume print job or cancel it?" +
                    "\nEnter \"Y\" to resume (any other key cancels the print job): ");
                String userDecision = Console.ReadLine();
                if (userDecision == "Y")
                {
                    theJob.Resume();
                }
                else
                {
                    theJob.Cancel();
                }
            }//end if the queue should be resumed

        }//end HandlePausedJob
         //</SnippetHandlePausedJob>

        //<SnippetReportQueueAndJobAvailability>
        internal static void ReportQueueAndJobAvailability(PrintSystemJobInfo theJob)
        {
            if (!(ReportAvailabilityAtThisTime(theJob.HostingPrintQueue) && ReportAvailabilityAtThisTime(theJob)))
            {
                if (!ReportAvailabilityAtThisTime(theJob.HostingPrintQueue))
                {
                    Console.WriteLine("\nThat queue is not available at this time of day." +
                        "\nJobs in the queue will start printing again at {0}",
                         TimeConverter.ConvertToLocalHumanReadableTime(theJob.HostingPrintQueue.StartTimeOfDay).ToShortTimeString());
                    // TimeConverter class is defined in the complete sample
                }

                if (!ReportAvailabilityAtThisTime(theJob))
                {
                    Console.WriteLine("\nThat job is set to print only between {0} and {1}",
                        TimeConverter.ConvertToLocalHumanReadableTime(theJob.StartTimeOfDay).ToShortTimeString(),
                        TimeConverter.ConvertToLocalHumanReadableTime(theJob.UntilTimeOfDay).ToShortTimeString());
                }
                Console.WriteLine("\nThe job will begin printing as soon as it reaches the top of the queue after:");
                if (theJob.StartTimeOfDay > theJob.HostingPrintQueue.StartTimeOfDay)
                {
                    Console.WriteLine(TimeConverter.ConvertToLocalHumanReadableTime(theJob.StartTimeOfDay).ToShortTimeString());
                }
                else
                {
                    Console.WriteLine(TimeConverter.ConvertToLocalHumanReadableTime(theJob.HostingPrintQueue.StartTimeOfDay).ToShortTimeString());
                }

            }//end if at least one is not available

        }//end ReportQueueAndJobAvailability
        //</SnippetReportQueueAndJobAvailability>

        //<SnippetPrintQueueStartUntil>
        private static Boolean ReportAvailabilityAtThisTime(PrintQueue pq)
        {
            Boolean available = true;
            if (pq.StartTimeOfDay != pq.UntilTimeOfDay) // If the printer is not available 24 hours a day
            {
                DateTime utcNow = DateTime.UtcNow;
                Int32 utcNowAsMinutesAfterMidnight = (utcNow.TimeOfDay.Hours * 60) + utcNow.TimeOfDay.Minutes;

                // If now is not within the range of available times . . .
                if (!((pq.StartTimeOfDay < utcNowAsMinutesAfterMidnight)
                   &&
                   (utcNowAsMinutesAfterMidnight < pq.UntilTimeOfDay)))
                {
                    available = false;
                }
            }
            return available;
        }//end ReportAvailabilityAtThisTime
        //</SnippetPrintQueueStartUntil>

        // <SnippetUsingJobStartAndUntilTimes>
        private static Boolean ReportAvailabilityAtThisTime(PrintSystemJobInfo theJob)
        {
            Boolean available = true;
            if (theJob.StartTimeOfDay != theJob.UntilTimeOfDay) // If the job cannot be printed at all times of day
            {
                DateTime utcNow = DateTime.UtcNow;
                Int32 utcNowAsMinutesAfterMidnight = (utcNow.TimeOfDay.Hours * 60) + utcNow.TimeOfDay.Minutes;

                // If "now" is not within the range of available times . . .
                if (!((theJob.StartTimeOfDay < utcNowAsMinutesAfterMidnight)
                   &&
                   (utcNowAsMinutesAfterMidnight < theJob.UntilTimeOfDay)))
                {
                    available = false;
                }
            }
            return available;
        }//end ReportAvailabilityAtThisTime
        // </SnippetUsingJobStartAndUntilTimes>

    }//end TroubleSpotter class
}
