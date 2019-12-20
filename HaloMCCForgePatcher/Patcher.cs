// ReSharper disable StringLiteralTypo

namespace HaloMCCForgePatcher
{
    #region Using Directives

    using System;
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

            await using Stream stream = File.Open(pakFile.FullName, FileMode.Open);

            bool changedA = PatchByte(stream, 0x1E302110);
            bool changedB = PatchByte(stream, 0x1E2F52D0);

            stream.Close();

            if (changedA || changedB)
            {
                MsgBoxHelpers.Info("Patch successful!", "Done", () => Environment.Exit(0));
            }
            else
            {
                MsgBoxHelpers.Info("Game is already patched!", "Patch Skipped", () => Environment.Exit(0));
            }
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

        /// <summary>
        /// Patches the byte at the specified offset.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>Returns whether or not the byte was patched.</returns>
        private static bool PatchByte(Stream stream, int offset)
        {
            stream.Position = offset;
            if (stream.ReadByte() == 0x27)
            {
                return false;
            }

            stream.Position = offset;
            stream.WriteByte(0x27);
            return true;
        }

        #endregion
    }
}
