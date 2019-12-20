namespace HaloMCCForgePatcher
{
    #region Using Directives

    using System;
    using System.Drawing;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    #endregion

    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        private static async Task Main()
        {
            // Display a form so that message boxes appear topmost.
            Form frm = new Form
            {
                Size            = Size.Empty,
                StartPosition   = FormStartPosition.CenterScreen,
                ShowInTaskbar   = false,
                ShowIcon        = false,
                FormBorderStyle = FormBorderStyle.None,
                Opacity         = 0,
                TopMost         = true
            };

            frm.Show();
            frm.BringToFront();
            frm.Size = Size.Empty;

            await new Patcher().RunAsync().ConfigureAwait(false);
            Environment.Exit(0);
        }
    }
}
