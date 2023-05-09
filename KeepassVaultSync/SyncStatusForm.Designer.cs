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
            this.optionsListBox = new System.Windows.Forms.CheckedListBox();
            this.refreshSyncsButton = new System.Windows.Forms.Button();
            this.syncItemSelector = new System.Windows.Forms.CheckedListBox();
            this.SuspendLayout();
            // 
            // syncStatusBar
            // 
            this.syncStatusBar.Location = new System.Drawing.Point(0, 1006);
            this.syncStatusBar.Name = "syncStatusBar";
            this.syncStatusBar.Size = new System.Drawing.Size(985, 22);
            this.syncStatusBar.TabIndex = 1;
            this.syncStatusBar.Text = "Not Initialized";
            // 
            // syncProgressBar
            // 
            this.syncProgressBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.syncProgressBar.Location = new System.Drawing.Point(0, 983);
            this.syncProgressBar.Name = "syncProgressBar";
            this.syncProgressBar.Size = new System.Drawing.Size(985, 23);
            this.syncProgressBar.TabIndex = 2;
            // 
            // syncStatusLogTB
            // 
            this.syncStatusLogTB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.syncStatusLogTB.Location = new System.Drawing.Point(70, 28);
            this.syncStatusLogTB.Margin = new System.Windows.Forms.Padding(10);
            this.syncStatusLogTB.Name = "syncStatusLogTB";
            this.syncStatusLogTB.ReadOnly = true;
            this.syncStatusLogTB.Size = new System.Drawing.Size(829, 456);
            this.syncStatusLogTB.TabIndex = 3;
            this.syncStatusLogTB.Text = "";
            // 
            // startStopButton
            // 
            this.startStopButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.startStopButton.AutoSize = true;
            this.startStopButton.Location = new System.Drawing.Point(70, 891);
            this.startStopButton.Name = "startStopButton";
            this.startStopButton.Size = new System.Drawing.Size(339, 62);
            this.startStopButton.TabIndex = 4;
            this.startStopButton.Text = "Start";
            this.startStopButton.UseVisualStyleBackColor = true;
            // 
            // clearLogButton
            // 
            this.clearLogButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.clearLogButton.Location = new System.Drawing.Point(560, 891);
            this.clearLogButton.Name = "clearLogButton";
            this.clearLogButton.Size = new System.Drawing.Size(339, 62);
            this.clearLogButton.TabIndex = 5;
            this.clearLogButton.Text = "Clear Log";
            this.clearLogButton.UseVisualStyleBackColor = true;
            // 
            // optionsListBox
            // 
            this.optionsListBox.CheckOnClick = true;
            this.optionsListBox.FormattingEnabled = true;
            this.optionsListBox.Items.AddRange(new object[] { "Inhibit Database Lock In Sync" });
            this.optionsListBox.Location = new System.Drawing.Point(560, 550);
            this.optionsListBox.Name = "optionsListBox";
            this.optionsListBox.Size = new System.Drawing.Size(338, 193);
            this.optionsListBox.TabIndex = 7;
            // 
            // refreshSyncsButton
            // 
            this.refreshSyncsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.refreshSyncsButton.Location = new System.Drawing.Point(70, 799);
            this.refreshSyncsButton.Name = "refreshSyncsButton";
            this.refreshSyncsButton.Size = new System.Drawing.Size(339, 62);
            this.refreshSyncsButton.TabIndex = 8;
            this.refreshSyncsButton.Text = "Reload Configurations";
            this.refreshSyncsButton.UseVisualStyleBackColor = true;
            // 
            // syncItemSelector
            // 
            this.syncItemSelector.CheckOnClick = true;
            this.syncItemSelector.FormattingEnabled = true;
            this.syncItemSelector.Location = new System.Drawing.Point(70, 550);
            this.syncItemSelector.Name = "syncItemSelector";
            this.syncItemSelector.Size = new System.Drawing.Size(339, 193);
            this.syncItemSelector.TabIndex = 9;
            // 
            // SyncStatusForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(985, 1028);
            this.Controls.Add(this.syncItemSelector);
            this.Controls.Add(this.refreshSyncsButton);
            this.Controls.Add(this.optionsListBox);
            this.Controls.Add(this.clearLogButton);
            this.Controls.Add(this.startStopButton);
            this.Controls.Add(this.syncStatusLogTB);
            this.Controls.Add(this.syncProgressBar);
            this.Controls.Add(this.syncStatusBar);
            this.Name = "SyncStatusForm";
            this.Text = "Sync Status";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.CheckedListBox syncItemSelector;

        private System.Windows.Forms.Button refreshSyncsButton;

        private System.Windows.Forms.CheckedListBox optionsListBox;
        
        private System.Windows.Forms.Button clearLogButton;

        private System.Windows.Forms.Button startStopButton;

        private System.Windows.Forms.StatusBar syncStatusBar;

        private System.Windows.Forms.RichTextBox syncStatusLogTB;

        private System.Windows.Forms.ProgressBar syncProgressBar;
        
        #endregion
    }
}