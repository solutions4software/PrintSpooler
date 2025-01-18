namespace PrintSpooler
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lbSpoolChanges = new ListBox();
            printerGridView = new DataGridView();
            PrinterNameModel = new DataGridViewTextBoxColumn();
            SupportsColor = new DataGridViewTextBoxColumn();
            PapersLevel = new DataGridViewTextBoxColumn();
            TonerLevel = new DataGridViewTextBoxColumn();
            DrumLevel = new DataGridViewTextBoxColumn();
            PrinterStatus = new DataGridViewTextBoxColumn();
            lstConectedPrinters = new ListBox();
            lstMonitorPrinters = new ListBox();
            btnAddToMonitorList = new Button();
            label1 = new Label();
            label2 = new Label();
            btnRemoveFromList = new Button();
            ((System.ComponentModel.ISupportInitialize)printerGridView).BeginInit();
            SuspendLayout();
            // 
            // lbSpoolChanges
            // 
            lbSpoolChanges.FormattingEnabled = true;
            lbSpoolChanges.ItemHeight = 15;
            lbSpoolChanges.Location = new Point(12, 329);
            lbSpoolChanges.Name = "lbSpoolChanges";
            lbSpoolChanges.Size = new Size(916, 199);
            lbSpoolChanges.TabIndex = 2;
            // 
            // printerGridView
            // 
            printerGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            printerGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            printerGridView.Columns.AddRange(new DataGridViewColumn[] { PrinterNameModel, SupportsColor, PapersLevel, TonerLevel, DrumLevel, PrinterStatus });
            printerGridView.Location = new Point(12, 173);
            printerGridView.Name = "printerGridView";
            printerGridView.RowTemplate.Height = 25;
            printerGridView.Size = new Size(916, 150);
            printerGridView.TabIndex = 3;
            // 
            // PrinterNameModel
            // 
            PrinterNameModel.HeaderText = "Printer Name, Model";
            PrinterNameModel.Name = "PrinterNameModel";
            // 
            // SupportsColor
            // 
            SupportsColor.HeaderText = "Supports Color";
            SupportsColor.Name = "SupportsColor";
            // 
            // PapersLevel
            // 
            PapersLevel.HeaderText = "Papers Level";
            PapersLevel.Name = "PapersLevel";
            // 
            // TonerLevel
            // 
            TonerLevel.HeaderText = "Toner Level";
            TonerLevel.Name = "TonerLevel";
            // 
            // DrumLevel
            // 
            DrumLevel.HeaderText = "Drum Level";
            DrumLevel.Name = "DrumLevel";
            // 
            // PrinterStatus
            // 
            PrinterStatus.HeaderText = "Printer Status";
            PrinterStatus.Name = "PrinterStatus";
            // 
            // lstConectedPrinters
            // 
            lstConectedPrinters.FormattingEnabled = true;
            lstConectedPrinters.ItemHeight = 15;
            lstConectedPrinters.Location = new Point(12, 28);
            lstConectedPrinters.Name = "lstConectedPrinters";
            lstConectedPrinters.Size = new Size(432, 139);
            lstConectedPrinters.TabIndex = 4;
            // 
            // lstMonitorPrinters
            // 
            lstMonitorPrinters.FormattingEnabled = true;
            lstMonitorPrinters.ItemHeight = 15;
            lstMonitorPrinters.Location = new Point(512, 28);
            lstMonitorPrinters.Name = "lstMonitorPrinters";
            lstMonitorPrinters.Size = new Size(416, 139);
            lstMonitorPrinters.TabIndex = 5;
            // 
            // btnAddToMonitorList
            // 
            btnAddToMonitorList.Location = new Point(450, 70);
            btnAddToMonitorList.Name = "btnAddToMonitorList";
            btnAddToMonitorList.Size = new Size(56, 23);
            btnAddToMonitorList.TabIndex = 6;
            btnAddToMonitorList.Text = ">>";
            btnAddToMonitorList.UseVisualStyleBackColor = true;
            btnAddToMonitorList.Click += btnAddToMonitorList_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 10);
            label1.Name = "label1";
            label1.Size = new Size(108, 15);
            label1.TabIndex = 7;
            label1.Text = "Connected Printers";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(512, 10);
            label2.Name = "label2";
            label2.Size = new Size(106, 15);
            label2.TabIndex = 8;
            label2.Text = "Monitored Printers";
            // 
            // btnRemoveFromList
            // 
            btnRemoveFromList.Location = new Point(450, 99);
            btnRemoveFromList.Name = "btnRemoveFromList";
            btnRemoveFromList.Size = new Size(56, 23);
            btnRemoveFromList.TabIndex = 9;
            btnRemoveFromList.Text = "<<";
            btnRemoveFromList.UseVisualStyleBackColor = true;
            btnRemoveFromList.Click += btnRemoveFromList_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(940, 536);
            Controls.Add(btnRemoveFromList);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(btnAddToMonitorList);
            Controls.Add(lstMonitorPrinters);
            Controls.Add(lstConectedPrinters);
            Controls.Add(printerGridView);
            Controls.Add(lbSpoolChanges);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)printerGridView).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private ListBox lbSpoolChanges;
        private DataGridView printerGridView;
        private ListBox lstConectedPrinters;
        private ListBox lstMonitorPrinters;
        private Button btnAddToMonitorList;
        private Label label1;
        private Label label2;
        private Button btnRemoveFromList;
        private DataGridViewTextBoxColumn PrinterNameModel;
        private DataGridViewTextBoxColumn SupportsColor;
        private DataGridViewTextBoxColumn PapersLevel;
        private DataGridViewTextBoxColumn TonerLevel;
        private DataGridViewTextBoxColumn DrumLevel;
        private DataGridViewTextBoxColumn PrinterStatus;
    }
}
