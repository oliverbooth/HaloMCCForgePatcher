// ReSharper disable StringLiteralTypo

namespace HaloMCCForgePatcher
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Helpers;
    using Models;

    #endregion

    public class Patcher
    {
        #region Fields

        /// <summary>
        /// The dictionary of bytes to patch.
        /// </summary>
        private readonly Dictionary<long, byte> patchedBytes = new Dictionary<long, byte>
        {
            // {offset, byte}
            {0x1E302110, 0x27},
            {0x1E2F52D0, 0x27}
        };

        /// <summary>
        /// The filename of the PAK file to patch.
        /// </summary>
        public const string PakFileName = @"MCC-WindowsNoEditor.pak";

        /// <summary>
        /// The store app ID for Halo: The Master Chief Collection.
        /// </summary>
        public const uint HaloAppId = 976730u;

        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously runs the patcher.
        /// </summary>
        public async Task RunAsync()
        {
            Steam steam = Steam.GetSteamInstallation();
            if (steam.IsInstalled)
            {
                bool isHaloInstalled = await steam.IsAppInstalledAsync(HaloAppId)
                                                  .ConfigureAwait(false);

                if (isHaloInstalled)
                {
                    SteamApp haloApp = await steam.GetAppAsync(HaloAppId).ConfigureAwait(false);
                    await this.PatchSteamVersionAsync(haloApp).ConfigureAwait(false);
                }
                else
                {
                    MsgBoxHelpers.Error("Halo is not currently installed via Steam.");
                    await this.ConfirmManualPatchAsync().ConfigureAwait(false);
                }
            }
            else
            {
                MsgBoxHelpers.Error("Steam is not currently installed, or its library folders could not be detected.");
                await this.ConfirmManualPatchAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Patches the bytes at the offsets set in <see cref="patchedBytes"/>.
        /// </summary>
        /// <param name="stream">The stream.</param>
        private void ApplyPatch(Stream stream)
        {
            foreach ((long offset, byte @byte) in this.patchedBytes)
            {
                stream.Position = offset;
                stream.WriteByte(@byte);
            }
        }

        /// <summary>
        /// Requests confirmation from the user as to whether or not they would like to create a backup.
        /// </summary>
        /// <returns>Returns <see langword="true"/> if the user selected Yes, <see langword="false"/>
        /// otherwise.</returns>
        private bool ConfirmBackup()
        {
            return MsgBoxHelpers.ConfirmYesNo("Would you like to create a backup?",
                yes: () => true,
                no: () => { } // do nothing
            );
        }

        /// <summary>
        /// Requests confirmation from the user as to whether or not they would like to proceed with manual .PAK
        /// loading, and if they accept then proceeds. If the user declines, the application exits.
        /// </summary>
        private async Task ConfirmManualPatchAsync()
        {
            await MsgBoxHelpers.ConfirmYesNo("Would you like to search for the .PAK file manually?",
                yes: async () => await this.PatchManualPakFileAsync().ConfigureAwait(false),
                no: () => Environment.Exit(0)
            ).ConfigureAwait(false);
        }

        /// <summary>
        /// Determines whether patching is necessary.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <returns>Returns <see langword="true"/> if the patch is necessary, <see langword="false"/>
        /// otherwise.</returns>
        private bool ConfirmPatchNeeded(Stream stream)
        {
            foreach ((long offset, byte @byte) in this.patchedBytes)
            {
                stream.Position = offset;
                if (stream.ReadByte() != @byte)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Presents the user with an <see cref="OpenFileDialog"/> to manually find a .PAK file.
        /// </summary>
        /// <returns>Returns the <see cref="FileInfo"/> associated with the chosen file, or
        /// <see langword="null"/>.</returns>
        private FileInfo ManuallyFindPakFile()
        {
            ManualResetEvent reset    = new ManualResetEvent(false);
            DialogResult     result   = DialogResult.None;
            string           filename = String.Empty;

            Thread thread = new Thread(() =>
            {
                OpenFileDialog dialog = new OpenFileDialog
                {
                    Title       = $"Open {PakFileName}",
                    Filter      = $"{PakFileName}|{PakFileName}",
                    Multiselect = false
                };

                result   = dialog.ShowDialog();
                filename = dialog.FileName;
                reset.Set();
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            return result == DialogResult.OK ? new FileInfo(filename) : null;
        }

        /// <summary>
        /// Runs <see cref="ManuallyFindPakFile"/> and asynchronously runs <see cref="PatchAsync"/>.
        /// </summary>
        private async Task PatchManualPakFileAsync()
        {
            FileInfo pakFile = this.ManuallyFindPakFile();
            await this.PatchAsync(pakFile).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously runs the patching process on the specified file.
        /// </summary>
        /// <param name="pakFile">The <see cref="FileInfo"/> associated with the .PAK file.</param>
        private async Task PatchAsync(FileInfo pakFile)
        {
            if (pakFile == null || !pakFile.Exists)
            {
                MsgBoxHelpers.Error(".PAK file could not be found. The patcher cannot continue.",
                    callback: () => Environment.Exit(0));
                return;
            }

            Stream stream = File.Open(pakFile.FullName, FileMode.Open);

            if (this.ConfirmPatchNeeded(stream))
            {
                MsgBoxHelpers.Info("Game is already patched!", "Patch Skipped", () => Environment.Exit(0));
                return;
            }

            if (this.ConfirmBackup())
            {
                // must close stream for backup to take place
                stream.Close();
                stream.Dispose();

                BackupProgressForm progressForm = new BackupProgressForm();
                progressForm.Show();
                progressForm.BringToFront();

                await FileHelpers.CreateBackupAsync(pakFile.FullName,
                                      percentage =>
                                      {
                                          progressForm.PercentageProgressBar.Value = (int) Math.Floor(percentage);
                                          progressForm.Text =
                                              $"Creating backup ({percentage:0.0}%)";

                                          Application.DoEvents();
                                      })
                                 .ConfigureAwait(false);

                // reopen stream for patching
                stream = File.Open(pakFile.FullName, FileMode.Open);
            }

            this.ApplyPatch(stream);
            stream.Close();
            stream.Dispose();

            MsgBoxHelpers.Info("Patch successful!", "Done", () => Environment.Exit(0));
        }

        /// <summary>
        /// Asynchronously patches the game found in the Steam library.
        /// </summary>
        /// <param name="app">The <see cref="SteamApp"/> instance.</param>
        private async Task PatchSteamVersionAsync(SteamApp app)
        {
            if (!app.InstallDirectory.Exists)
            {
                MsgBoxHelpers.Error($"{app.Name} has manifest file, but install directory does not exist.");
                await this.ConfirmManualPatchAsync().ConfigureAwait(false);
                return;
            }

            await this.PatchAsync
                       (new FileInfo(Path.GetFullPath($"{app.InstallDirectory}/MCC/Content/Paks/{PakFileName}")))
                      .ConfigureAwait(false);
        }

        #endregion
    }
}
