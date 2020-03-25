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

    /// <summary>
    /// Represents a class which implements the patcher.
    /// </summary>
    public class Patcher
    {
        #region Fields

        /// <summary>
        /// The dictionary of bytes to patch.
        /// </summary>
        private readonly Dictionary<long, byte> patchedBytes = new Dictionary<long, byte>
        {
            // {offset, byte}
            {0x2FFC72D0, 0x27},
            {0x2FFD4110, 0x27}
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
                    MsgBoxHelpers.Error(Resources.HaloNotInstalledViaSteam);
                    await this.ConfirmManualPatchAsync().ConfigureAwait(false);
                }
            }
            else
            {
                MsgBoxHelpers.Error(Resources.SteamNotInstalled);
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
            return MsgBoxHelpers.ConfirmYesNo(Resources.ConfirmBackup,
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
            await MsgBoxHelpers.ConfirmYesNo(Resources.ConfirmHaloInstallSearch,
                yes: async () => await this.PatchGameManuallyAsync().ConfigureAwait(false),
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
        /// Presents the user with a <see cref="FolderBrowserDialog"/> to manually find their Halo installation.
        /// </summary>
        /// <returns>Returns the <see cref="HaloInstallation"/> associated with the chosen directory, or
        /// <see langword="null"/>.</returns>
        private HaloInstallation ManuallyFindHaloInstallation()
        {
            string path = String.Empty;

            Thread thread = new Thread(() =>
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog
                {
                    Description  = Resources.HaloMCCTitle,
                    SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
                };

                while (true)
                {
                    DialogResult result = dialog.ShowDialog();
                    if (result == DialogResult.Cancel)
                    {
                        bool cancel = MsgBoxHelpers.ConfirmYesNo(Resources.ConfirmPatchCancel,
                            yes: () => true,
                            no: () => { } // do nothing
                        );

                        if (cancel)
                        {
                            Environment.Exit(0);
                        }
                    }
                    else if (result == DialogResult.OK)
                    {
                        if (Directory.Exists(dialog.SelectedPath))
                        {
                            HaloInstallation halo = new HaloInstallation(new DirectoryInfo(dialog.SelectedPath));
                            if (halo.GetGameExecutablePath().Exists && halo.GetPakFilePath().Exists)
                            {
                                break;
                            }
                        }

                        MsgBoxHelpers.Error(Resources.InvalidFolder);
                    }
                }

                path = dialog.SelectedPath;
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            return new HaloInstallation(new DirectoryInfo(path));
        }

        /// <summary>
        /// Runs <see cref="ManuallyFindHaloInstallation"/> and asynchronously runs <see cref="PatchAsync"/>.
        /// </summary>
        private async Task PatchGameManuallyAsync()
        {
            await this.PatchAsync(this.ManuallyFindHaloInstallation()).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously runs the patching process on the specified file.
        /// </summary>
        /// <param name="halo">The Halo installation.</param>
        private async Task PatchAsync(HaloInstallation halo)
        {
            FileInfo pakFile = halo.GetPakFilePath();

            if (!(pakFile?.Exists ?? false))
            {
                MsgBoxHelpers.Error(Resources.PakFileNotFound,
                    callback: () => Environment.Exit(0));
                return;
            }

            Stream stream = File.Open(pakFile.FullName, FileMode.Open);

            if (halo.GetGameVersion() > new Version(1, 1270, 0, 0))
            {
                bool abort = !MsgBoxHelpers.ConfirmYesNo(
                    String.Format(Resources.UntestedVersion, halo.GetGameVersion()),
                    icon: MessageBoxIcon.Warning,
                    yes: () => true,
                    no: () => { } // internally returns default(T)
                );

                if (abort)
                {
                    Environment.Exit(0);
                }
            }
            else
            {
                if (!this.ConfirmPatchNeeded(stream))
                {
                    MsgBoxHelpers.Info(Resources.GameAlreadyPatched, Resources.PatchSkipped, () => Environment.Exit(0));
                    return;
                }
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
                                              String.Format(Resources.CreatingBackupPercentage, percentage);

                                          Application.DoEvents();
                                      })
                                 .ConfigureAwait(false);

                // reopen stream for patching
                stream = File.Open(pakFile.FullName, FileMode.Open);
            }

            this.ApplyPatch(stream);
            stream.Close();
            stream.Dispose();

            MsgBoxHelpers.Info(Resources.PatchSuccessful, Resources.Done, () => Environment.Exit(0));
        }

        /// <summary>
        /// Asynchronously patches the game found in the Steam library.
        /// </summary>
        /// <param name="app">The <see cref="SteamApp"/> instance.</param>
        private async Task PatchSteamVersionAsync(SteamApp app)
        {
            if (!app.InstallDirectory.Exists)
            {
                MsgBoxHelpers.Error(String.Format(Resources.ManifestButNoInstall, app.Name));
                await this.ConfirmManualPatchAsync().ConfigureAwait(false);
                return;
            }

            await this.PatchAsync(new HaloInstallation(app.InstallDirectory)).ConfigureAwait(false);
        }

        #endregion
    }
}
