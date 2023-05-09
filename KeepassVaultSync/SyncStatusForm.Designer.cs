using System.ComponentModel;

namespace KeepassVaultSync
{
    partial class SyncStatusForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.syncStatusBar = new System.Windows.Forms.StatusBar();
            this.syncProgressBar = new System.Windows.Forms.ProgressBar();
            this.syncStatusLogTB = new System.Windows.Forms.RichTextBox();
            this.startStopButton = new System.Windows.Forms.Button();
            this.clearLogButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // syncStatusBar
            // 
            this.syncStatusBar.Location = new System.Drawing.Point(0, 428);
            this.syncStatusBar.Name = "syncStatusBar";
            this.syncStatusBar.Size = new System.Drawing.Size(800, 22);
            this.syncStatusBar.TabIndex = 1;
            this.syncStatusBar.Text = "Not Initialized";
            // 
            // syncProgressBar
            // 
            this.syncProgressBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.syncProgressBar.Location = new System.Drawing.Point(0, 405);
            this.syncProgressBar.Name = "syncProgressBar";
            this.syncProgressBar.Size = new System.Drawing.Size(800, 23);
            this.syncProgressBar.TabIndex = 2;
            // 
            // syncStatusLogTB
            // 
            this.syncStatusLogTB.Location = new System.Drawing.Point(19, 19);
            this.syncStatusLogTB.Margin = new System.Windows.Forms.Padding(10);
            this.syncStatusLogTB.Name = "syncStatusLogTB";
            this.syncStatusLogTB.ReadOnly = true;
            this.syncStatusLogTB.Size = new System.Drawing.Size(762, 324);
            this.syncStatusLogTB.TabIndex = 3;
            this.syncStatusLogTB.Text = "";
            // 
            // startStopButton
            // 
            this.startStopButton.Location = new System.Drawing.Point(87, 368);
            this.startStopButton.Name = "startStopButton";
            this.startStopButton.Size = new System.Drawing.Size(220, 30);
            this.startStopButton.TabIndex = 4;
            this.startStopButton.Text = "Start";
            this.startStopButton.UseVisualStyleBackColor = true;
            // 
            // clearLogButton
            // 
            this.clearLogButton.Location = new System.Drawing.Point(492, 368);
            this.clearLogButton.Name = "clearLogButton";
            this.clearLogButton.Size = new System.Drawing.Size(220, 30);
            this.clearLogButton.TabIndex = 5;
            this.clearLogButton.Text = "Clear Log";
            this.clearLogButton.UseVisualStyleBackColor = true;
            // 
            // SyncStatusForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.clearLogButton);
            this.Controls.Add(this.startStopButton);
            this.Controls.Add(this.syncStatusLogTB);
            this.Controls.Add(this.syncProgressBar);
            this.Controls.Add(this.syncStatusBar);
            this.Name = "SyncStatusForm";
            this.Text = "Sync Status";
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Button clearLogButton;

        private System.Windows.Forms.Button startStopButton;

        private System.Windows.Forms.StatusBar syncStatusBar;

        private System.Windows.Forms.RichTextBox syncStatusLogTB;

        private System.Windows.Forms.ProgressBar syncProgressBar;
        
        #endregion
    }
}