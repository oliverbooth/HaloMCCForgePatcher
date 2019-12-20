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
        private static async Task Main(string[] args)
        {
            Form frm = new Form
            {
                Size            = new Size(0, 0),
                StartPosition   = FormStartPosition.CenterScreen,
                ShowInTaskbar   = false,
                ShowIcon        = false,
                FormBorderStyle = FormBorderStyle.None
            };

            frm.Show();
            frm.BringToFront();
            await new Patcher().RunAsync().ConfigureAwait(false);
            Environment.Exit(0);
        }
    }
}
