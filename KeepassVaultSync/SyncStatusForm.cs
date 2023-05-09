using System;
using System.Drawing;
using System.Windows.Forms;

namespace KeepassVaultSync
{
    public partial class SyncStatusForm : Form
    {
        public event EventHandler StartClicked;
        public event EventHandler StopClicked;
        
        private bool _started = false;

        public SyncStatusForm()
        {
            InitializeComponent();
            this.Closing += (sender, args) => { args.Cancel = true; Hide(); StopClicked?.Invoke(sender, args); };
            LogInfo("Initialized");
            UpdateStatusBar("Idle");
            startStopButton.Click += (sender, args) =>
            {
                _started = !_started;
                startStopButton.Text = _started ? "Stop" : "Start";
                UpdateStatusBar(_started ? "Running" : "Idle");
                if (_started)
                {
                    StartClicked?.Invoke(sender, args);
                }
                else
                {
                    StopClicked?.Invoke(sender, args);
                }
            };
            clearLogButton.Click += (sender, args) => { syncStatusLogTB.Clear(); };
        }

        public void LogStatusItem(string severity, string log)
        {
            LogStatusItem(severity, syncStatusLogTB.ForeColor, log);
        }
        public void LogStatusItem(string severity, Color severityColor, string log)
        {

            syncStatusLogTB.AppendText(severity, severityColor);
            syncStatusLogTB.AppendText($" [{DateTime.Now}] ");
            syncStatusLogTB.AppendText(log);
            syncStatusLogTB.AppendText(Environment.NewLine);
        }
        
        public void FinishSyncStatus()
        {
            UpdateStatusBar("Idle");
            syncProgressBar.Value = 0;
            startStopButton.Text = "Start";
            _started = false;
        }
        
        public void UpdateStatusBar(string status)
        {
            syncStatusBar.Text = status;
        }
        
        public void StartProgress(int total)
        {
            syncProgressBar.Minimum = 0;
            syncProgressBar.Maximum = total;
            syncProgressBar.Value = 0;
        }
        
        public void UpdateProgressCurrent(int current)
        {
            syncProgressBar.Value = current;
        }
        
        public void IncrementProgress(int delta)
        {
            syncProgressBar.Value += delta;
        }
        
        void ScrollLogToEnd()
        {
            syncStatusLogTB.SelectionStart = syncStatusLogTB.Text.Length;
            syncStatusLogTB.ScrollToCaret();
        }

        public void LogInfo(string log)
        {
            LogStatusItem("INFO",  log);
            ScrollLogToEnd();
        }
        
        public void LogSuccess(string log)
        {
            LogStatusItem("SUCCESS", Color.Green, log);
            ScrollLogToEnd();
        }
        
        public void LogWarning(string log)
        {
            LogStatusItem("WARNING", Color.Orange, log);
            ScrollLogToEnd();
        }

        public void LogError(string log)
        {
            LogStatusItem("ERROR", Color.Red, log);
            ScrollLogToEnd();
        }
    }
}