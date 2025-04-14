namespace PrintSpooler.Utilities
{
    public class PrintJobDetails
    {
        public int JobID { get; set; } = -1;
        public string JobStatus { get; set; } = string.Empty;
        public string PrinterName { get; set; } = string.Empty;
        public int NoOfPages { get; set; } = 1;
        public int NoOfCopies { get; set; } = 1;
        public int NoOfSheets { get; set; } = 1;
        public bool IsColor { get; set; } = false;
        public string Color { get; set; } = string.Empty;

        override
        public string ToString()
        {
            return string.Format(
                    "Job ID: {0}\n" +
                    "Job Status: {1}\n" +
                    "Printer: {2}\n" +
                    "No. of Pages: {3}\n" +
                    "No. of Copies: {4}\n" +
                    "Color: {5}\n" +
                    "\nWould you like to continue printing?",
                    JobID,
                    JobStatus,
                    PrinterName,
                    NoOfPages,
                    NoOfCopies,
                    Color);
        }
    }
}
