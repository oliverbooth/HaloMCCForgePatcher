// ReSharper disable StringLiteralTypo
// ReSharper disable CommentTypo

namespace HaloMCCForgePatcher.Models
{
    #region Using Directives

    using System.IO;
    using Newtonsoft.Json;

    #endregion

    /// <summary>
    /// Represents a Steam app (or game).
    /// </summary>
    public class SteamApp
    {
        #region Fields

        /// <summary>
        /// The library folder where this game is installed.
        /// </summary>
        internal DirectoryInfo LibraryFolder;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the app ID.
        /// </summary>
        [JsonProperty("appid")]
        public uint AppId { get; internal set; }

        /// <summary>
        /// Gets the directory at which this app is installed.
        /// </summary>
        public DirectoryInfo InstallDirectory =>
            new DirectoryInfo(Path.GetFullPath($"{this.LibraryFolder}/steamapps/common/{this.InstallDirProperty}"));

        /// <summary>
        /// Gets the name of this app.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the <see cref="Models.Steam"/> instance.
        /// </summary>
        public Steam Steam { get; internal set; }

        /// <summary>
        /// Gets the installdir property.
        /// </summary>
        [JsonProperty("installdir")]
        internal string InstallDirProperty { get; set; }

        #endregion
    }
}
