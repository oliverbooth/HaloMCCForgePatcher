// ReSharper disable StringLiteralTypo

namespace HaloMCCForgePatcher
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Gameloop.Vdf;
    using Gameloop.Vdf.JsonConverter;
    using Microsoft.Win32;
    using Newtonsoft.Json.Linq;
    using static System.Windows.Forms.MessageBoxButtons;
    using static System.Windows.Forms.MessageBoxIcon;

    #endregion

    public class Patcher
    {
        #region Fields

        private const uint HaloAppId = 976730u;

        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously runs the patcher.
        /// </summary>
        public async Task RunAsync()
        {
            char   dirChar = Path.DirectorySeparatorChar;
            string steam   = GetSteamInstallPath();
            string useDir  = steam;

            if (!File.Exists(GetPotentialManifestFile(GetSteamAppsFolder(GetSteamInstallPath()))))
            {
                string libraryFoldersFile = $"{steam}{dirChar}steamapps{dirChar}libraryfolders.vdf";
                string contents;

                using (StreamReader streamReader = new StreamReader(libraryFoldersFile))
                {
                    contents = await streamReader.ReadToEndAsync().ConfigureAwait(false);
                }

                JToken       json      = VdfConvert.Deserialize(contents).Value.ToJson();
                List<string> checkDirs = new List<string>();

                for (int i = 1;; i++)
                {
                    JToken key = json[i.ToString()];
                    if (key is null)
                    {
                        break;
                    }

                    checkDirs.Add(key.Value<string>());
                }

                foreach (string checkDir in checkDirs)
                {
                    if (!File.Exists(GetPotentialManifestFile(GetSteamAppsFolder(checkDir))))
                    {
                        continue;
                    }

                    useDir = checkDir;
                    break;
                }
            }

            string haloInstallPath = GetPotentialInstallDirectory(GetSteamAppsFolder(useDir));
            if (!Directory.Exists(haloInstallPath))
            {
                MessageBox.Show("Halo: The Master Chief Collection could not be found in your Steam library.\n\n" +
                                "Please make sure you have the game installed before running the patcher.",
                    "Patcher Error",
                    OK, Error);

                Environment.Exit(0);
            }

            if (!File.Exists(GetPakFile(haloInstallPath)))
            {
                MessageBox.Show("Halo: The Master Chief Collection is installed, but appears to be corrupt.\n\n" +
                                "Please run Steam's integrity verification, then run this patcher again.",
                    "Patcher Error",
                    OK, Error);

                Environment.Exit(0);
            }

            try
            {
                await using Stream stream = File.Open(GetPakFile(haloInstallPath), FileMode.Open);

                bool a = PatchByte(stream, 0x1E302110);
                bool b = PatchByte(stream, 0x1E2F52D0);

                stream.Close();

                if (a || b)
                {
                    MessageBox.Show("Patch successful!", "Done", OK, Information);
                }
                else
                {
                    MessageBox.Show("Game is already patched!", "Done", OK, Information);
                }
            }
            catch (IOException)
            {
                MessageBox.Show("The patcher could not open the file for read. Is the game running?",
                    "Patcher Error",
                    OK, Error);

                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                StackTrace stack = new StackTrace(ex, true);
                StackFrame frame = stack.GetFrame(stack.FrameCount - 1);
                int        line  = frame.GetFileLineNumber();
                int        col   = frame.GetFileColumnNumber();
                string     file  = Path.GetFileNameWithoutExtension(frame.GetFileName()).ToUpper();

                MessageBox.Show("The patcher encountered an error while patching. Quote this exception ID:\n\n" +
                                $"{ex.GetType().Name.ToUpper()}_{file}_{line}:{col}",
                    "Patcher Error",
                    OK, Error);

                Environment.Exit(0);
            }
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

        /// <summary>
        /// Gets the pak file to patch.
        /// </summary>
        /// <param name="installPath">The Halo install path.</param>
        /// <returns>Returns the path to the Pak file.</returns>
        private static string GetPakFile(string installPath)
        {
            char dir = Path.DirectorySeparatorChar;
            return Path.GetFullPath($"{installPath}{dir}MCC{dir}Content{dir}Paks{dir}MCC-WindowsNoEditor.pak");
        }

        /// <summary>
        /// Gets the potential location at which Halo MCC might be installed, given a specific SteamApps path.
        /// </summary>
        /// <param name="steamApps">The SteamApps path.</param>
        /// <returns>Returns the potential Halo MCC path.</returns>
        private static string GetPotentialInstallDirectory(string steamApps)
        {
            char dir = Path.DirectorySeparatorChar;
            return Path.GetFullPath($"{steamApps}{dir}common{dir}Halo The Master Chief Collection");
        }

        /// <summary>
        /// Gets the potential manifest file for Halo MCC given a specific library folder.
        /// </summary>
        /// <param name="path">The library folder.</param>
        /// <returns>Returns the potential AppManifest file path.</returns>
        private static string GetPotentialManifestFile(string path)
        {
            char dir = Path.DirectorySeparatorChar;
            return Path.GetFullPath($"{path}{dir}appmanifest_{HaloAppId}.acf");
        }

        /// <summary>
        /// Gets the SteamApps folder for a particular library folder.
        /// </summary>
        /// <param name="path">The library folder.</param>
        /// <returns>Returns the SteamApps folder.</returns>
        private static string GetSteamAppsFolder(string path)
        {
            char dir = Path.DirectorySeparatorChar;
            return Path.GetFullPath($"{path}{dir}steamapps{dir}");
        }

        /// <summary>
        /// Gets the install directory.
        /// </summary>
        /// <returns></returns>
        private static string GetSteamInstallPath()
        {
            RegistryKey key  = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Valve\Steam");
            string      path = (key?.GetValue("InstallPath", String.Empty).ToString() ?? String.Empty).Trim();

            if (!String.IsNullOrWhiteSpace(path))
            {
                return path;
            }

            key  = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Valve\Steam");
            path = (key?.GetValue("SteamPath", String.Empty).ToString() ?? String.Empty).Trim();

            return Path.GetFullPath(path);
        }

        #endregion
    }
}
