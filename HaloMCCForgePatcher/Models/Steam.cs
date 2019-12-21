// ReSharper disable StringLiteralTypo

namespace HaloMCCForgePatcher.Models
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Gameloop.Vdf;
    using Gameloop.Vdf.JsonConverter;
    using Helpers;
    using Microsoft.Win32;
    using Newtonsoft.Json.Linq;

    #endregion

    /// <summary>
    /// Represents a class which contains information about a Steam installation.
    /// </summary>
    public class Steam
    {
        #region Properties

        /// <summary>
        /// Gets the directory at which Steam is installed.
        /// </summary>
        public DirectoryInfo InstallDirectory { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not Steam is installed.
        /// </summary>
        public bool IsInstalled =>
            this.InstallDirectory?.Exists ?? false;

        #endregion

        #region Methods

        /// <summary>
        /// Gets the current Steam installation.
        /// </summary>
        /// <returns>Returns a new instance of <see cref="Steam"/>.</returns>
        public static Steam GetSteamInstallation()
        {
            DirectoryInfo directory;
            const string  keyName = @"SOFTWARE\Valve\Steam";

            RegistryKey key = Registry.LocalMachine.OpenSubKey(keyName);
            string path = (key?.GetValue(@"InstallPath", String.Empty)
                               .ToString() ?? String.Empty).Trim();

            if (!String.IsNullOrWhiteSpace(path) && Directory.Exists(path))
            {
                directory = new DirectoryInfo(path);
            }
            else
            {
                key = Registry.CurrentUser.OpenSubKey(keyName);
                path = (key?.GetValue(@"SteamPath", String.Empty)
                            .ToString() ?? String.Empty).Trim();

                directory = String.IsNullOrWhiteSpace(path) ? null : new DirectoryInfo(path);
            }

            return new Steam {InstallDirectory = directory};
        }

        /// <summary>
        /// Asynchronously determines if the specified app is installed.
        /// </summary>
        /// <param name="appId">The app ID.</param>
        /// <returns>Returns <see langword="true"/> if the app manifest was found, <see langword="false"/>
        /// otherwise.</returns>
        public async Task<bool> IsAppInstalledAsync(uint appId)
        {
            IEnumerable<DirectoryInfo> libraryFolders = await this.GetLibraryFoldersAsync()
                                                                  .ConfigureAwait(false);

            return libraryFolders.Select(f => new FileInfo($@"{f}/steamapps/appmanifest_{appId}.acf"))
                                 .Any(f => f.Exists);
        }

        /// <summary>
        /// Gets the <see cref="SteamApp"/> instance which corresponds to the specified app ID, or
        /// <see langword="null"/>.
        /// </summary>
        /// <param name="appId">The app ID.</param>
        /// <returns>Returns an instance of <see cref="SteamApp"/>, or <see langword="null"/>.</returns>
        public async Task<SteamApp> GetAppAsync(uint appId)
        {
            string relativePath = $"/steamapps/appmanifest_{appId}.acf";

            if (!await this.IsAppInstalledAsync(appId).ConfigureAwait(false))
            {
                return null;
            }

            DirectoryInfo libraryFolder =
                (await this.GetLibraryFoldersAsync().ConfigureAwait(false))
               .FirstOrDefault(f => File.Exists(Path.GetFullPath($@"{f}{relativePath}")));

            using StreamReader reader = new StreamReader(Path.GetFullPath($@"{libraryFolder}{relativePath}"));

            string   contents = await reader.ReadToEndAsync().ConfigureAwait(false);
            SteamApp app      = VdfConvert.Deserialize(contents).Value.ToJson().ToObject<SteamApp>();
            app.Steam         = this;
            app.LibraryFolder = libraryFolder;
            return app;
        }

        /// <summary>
        /// Gets the library folders in this Steam installation.
        /// </summary>
        /// <returns>Returns an <see cref="IEnumerable{T}"/> of <see cref="DirectoryInfo"/> instances.</returns>
        public async Task<IEnumerable<DirectoryInfo>> GetLibraryFoldersAsync()
        {
            if (!this.IsInstalled)
            {
                return Array.Empty<DirectoryInfo>();
            }

            List<DirectoryInfo> libraryFolders = new List<DirectoryInfo>
            {
                // steam install directory is the default library folder
                this.InstallDirectory
            };

            string vdfPath = $@"{this.InstallDirectory}/steamapps/libraryfolders.vdf";
            using (StreamReader reader = new StreamReader(Path.GetFullPath(vdfPath)))
            {
                string contents = await reader.ReadToEndAsync().ConfigureAwait(false);
                JToken json     = VdfConvert.Deserialize(contents).Value.ToJson();

                libraryFolders.AddRange(JsonHelpers.IterateStringKeys(json)
                                                   .Select(s => new DirectoryInfo(s)));
            }

            return libraryFolders;
        }

        #endregion
    }
}
