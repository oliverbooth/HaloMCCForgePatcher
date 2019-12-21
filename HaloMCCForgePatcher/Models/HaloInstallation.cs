// ReSharper disable StringLiteralTypo

namespace HaloMCCForgePatcher.Models
{
    #region Using Directives

    using System;
    using System.Diagnostics;
    using System.IO;

    #endregion

    /// <summary>
    /// Represents a Halo: Master Chief Collection installation.
    /// </summary>
    public class HaloInstallation
    {
        #region Fields

        /// <summary>
        /// The relative path of the game executable.
        /// </summary>
        private readonly string relativeExePath =
            Path.Combine(@"MCC", @"Binaries", @"Win64", @"MCC-Win64-Shipping.exe");

        /// <summary>
        /// The relative path of the game executable.
        /// </summary>
        private readonly string relativePakPath =
            Path.Combine(@"MCC", @"Content", @"Paks", @"MCC-WindowsNoEditor.pak");

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HaloInstallation"/> class.
        /// </summary>
        /// <param name="installDirectory">The Halo installation directory.</param>
        public HaloInstallation(DirectoryInfo installDirectory)
        {
            if (installDirectory?.Exists ?? false)
            {
                this.InstallDirectory = installDirectory;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="DirectoryInfo"/> associated with the game's installation directory.
        /// </summary>
        public DirectoryInfo InstallDirectory { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the <see cref="FileInfo"/> associated with the game executable path.
        /// </summary>
        /// <returns>Returns an instance of <see cref="FileInfo"/>.</returns>
        public FileInfo GetGameExecutablePath()
        {
            string path = Path.Combine(this.InstallDirectory.ToString(), this.relativeExePath);
            return new FileInfo(Path.GetFullPath(path));
        }

        /// <summary>
        /// Gets the version of the game executable.
        /// </summary>
        /// <returns>Returns an instance of <see cref="Version"/>.</returns>
        public Version GetGameVersion()
        {
            FileVersionInfo info = FileVersionInfo.GetVersionInfo(this.GetGameExecutablePath().ToString());
            return new Version(info.FileVersion);
        }

        /// <summary>
        /// Gets the <see cref="FileInfo"/> associated with the game's PAK file.
        /// </summary>
        /// <returns>Returns an instance of <see cref="FileInfo"/>.</returns>
        public FileInfo GetPakFilePath()
        {
            string path = Path.Combine(this.InstallDirectory.ToString(), this.relativePakPath);
            return new FileInfo(Path.GetFullPath(path));
        }

        #endregion
    }
}
