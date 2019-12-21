namespace HaloMCCForgePatcher
{
    #region Using Directives

    using System.Drawing;
    using System.Windows.Forms;
    using Helpers;

    #endregion

    public sealed class BackupProgressForm : Form
    {
        public BackupProgressForm()
        {
            this.ClientSize      = new Size(305, 40);
            this.Text            = "Creating backup";
            this.StartPosition   = FormStartPosition.CenterScreen;
            this.Icon            = Resources.HaloMCC_v1;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox     = false;
            this.SizeGripStyle   = SizeGripStyle.Hide;

            this.PercentageProgressBar = new ProgressBar
            {
                Dock    = DockStyle.Fill,
                Maximum = 100,
                Style   = ProgressBarStyle.Continuous
            };

            this.Controls.Add(this.PercentageProgressBar);

            this.Closing += (sender, args) =>
            {
                args.Cancel = !MsgBoxHelpers.ConfirmYesNo(
                    "Are you sure you want to cancel the backup and proceed with patching?",
                    "Confirm Backup Cancel",
                    () =>
                    {
                        FileHelpers.CancelBackup();
                        return true;
                    },
                    () =>
                    {
                        // do nothing
                    });
            };
        }

        #region Properties

        /// <summary>
        /// Gets the progress bar control in this form.
        /// </summary>
        public ProgressBar PercentageProgressBar { get; }

        #endregion
    }
}
