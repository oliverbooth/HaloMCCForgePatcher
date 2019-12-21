namespace HaloMCCForgePatcher.Helpers
{
    #region Using Directives

    using System;
    using System.Windows.Forms;

    #endregion

    public static class MsgBoxHelpers
    {
        /// <summary>
        /// The default message box title.
        /// </summary>
        private const string DefaultTitle = @"Halo MCC Forge Patcher";

        public static T ConfirmYesNo<T>(string text, string title = @"", Func<T> yes = null, Action no = null)
        {
            if (String.IsNullOrWhiteSpace(title))
            {
                title = DefaultTitle;
            }

            DialogResult result = MessageBox.Show(text, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            switch (result)
            {
                case DialogResult.Yes:
                    if (yes != null)
                    {
                        return yes.Invoke();
                    }

                    break;
                case DialogResult.No:
                    if (no != null)
                    {
                        no.Invoke();
                    }
                    else
                    {
                        Environment.Exit(0);
                    }

                    break;
            }

            return default;
        }

        public static DialogResult Error(string text, string title = "", Action callback = null)
        {
            if (String.IsNullOrWhiteSpace(title))
            {
                title = DefaultTitle;
            }

            DialogResult result = MessageBox.Show(text, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            callback?.Invoke();
            return result;
        }

        public static DialogResult Info(string text, string title = "", Action callback = null)
        {
            if (String.IsNullOrWhiteSpace(title))
            {
                title = DefaultTitle;
            }

            DialogResult result = MessageBox.Show(text, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
            callback?.Invoke();
            return result;
        }
    }
}
