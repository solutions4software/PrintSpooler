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
            cmbPrinters = new ComboBox();
            btnMonitor = new Button();
            lbSpoolChanges = new ListBox();
            SuspendLayout();
            // 
            // cmbPrinters
            // 
            cmbPrinters.FormattingEnabled = true;
            cmbPrinters.Location = new Point(12, 25);
            cmbPrinters.Name = "cmbPrinters";
            cmbPrinters.Size = new Size(259, 23);
            cmbPrinters.TabIndex = 0;
            // 
            // btnMonitor
            // 
            btnMonitor.Location = new Point(277, 25);
            btnMonitor.Name = "btnMonitor";
            btnMonitor.Size = new Size(115, 23);
            btnMonitor.TabIndex = 1;
            btnMonitor.Text = "Start Monitoring";
            btnMonitor.UseVisualStyleBackColor = true;
            btnMonitor.Click += btnMonitor_Click;
            // 
            // lbSpoolChanges
            // 
            lbSpoolChanges.FormattingEnabled = true;
            lbSpoolChanges.ItemHeight = 15;
            lbSpoolChanges.Location = new Point(12, 69);
            lbSpoolChanges.Name = "lbSpoolChanges";
            lbSpoolChanges.Size = new Size(783, 229);
            lbSpoolChanges.TabIndex = 2;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(807, 309);
            Controls.Add(lbSpoolChanges);
            Controls.Add(btnMonitor);
            Controls.Add(cmbPrinters);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
        }

        #endregion

        private ComboBox cmbPrinters;
        private Button btnMonitor;
        private ListBox lbSpoolChanges;
    }
}
