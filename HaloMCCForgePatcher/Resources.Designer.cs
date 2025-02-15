﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HaloMCCForgePatcher {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("HaloMCCForgePatcher.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Would you like to create a backup?.
        /// </summary>
        internal static string ConfirmBackup {
            get {
                return ResourceManager.GetString("ConfirmBackup", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Are you sure you want to cancel the backup and proceed with patching?.
        /// </summary>
        internal static string ConfirmBackupCancel {
            get {
                return ResourceManager.GetString("ConfirmBackupCancel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Confirm Backup Cancel.
        /// </summary>
        internal static string ConfirmBackupCancelTitle {
            get {
                return ResourceManager.GetString("ConfirmBackupCancelTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Would you like to search for the Halo installation manually?.
        /// </summary>
        internal static string ConfirmHaloInstallSearch {
            get {
                return ResourceManager.GetString("ConfirmHaloInstallSearch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Are you sure you want to cancel the patch?.
        /// </summary>
        internal static string ConfirmPatchCancel {
            get {
                return ResourceManager.GetString("ConfirmPatchCancel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Creating backup ({0:0.0}%).
        /// </summary>
        internal static string CreatingBackupPercentage {
            get {
                return ResourceManager.GetString("CreatingBackupPercentage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Done.
        /// </summary>
        internal static string Done {
            get {
                return ResourceManager.GetString("Done", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Game is already patched!.
        /// </summary>
        internal static string GameAlreadyPatched {
            get {
                return ResourceManager.GetString("GameAlreadyPatched", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Icon similar to (Icon).
        /// </summary>
        internal static System.Drawing.Icon HaloMCC_v1 {
            get {
                object obj = ResourceManager.GetObject("HaloMCC_v1", resourceCulture);
                return ((System.Drawing.Icon)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Halo: Master Chief Collection.
        /// </summary>
        internal static string HaloMCCTitle {
            get {
                return ResourceManager.GetString("HaloMCCTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Halo is not currently installed via Steam..
        /// </summary>
        internal static string HaloNotInstalledViaSteam {
            get {
                return ResourceManager.GetString("HaloNotInstalledViaSteam", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified folder is not a Halo installation..
        /// </summary>
        internal static string InvalidFolder {
            get {
                return ResourceManager.GetString("InvalidFolder", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} has manifest file, but install directory does not exist..
        /// </summary>
        internal static string ManifestButNoInstall {
            get {
                return ResourceManager.GetString("ManifestButNoInstall", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Open {0}.
        /// </summary>
        internal static string OpenFileName {
            get {
                return ResourceManager.GetString("OpenFileName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to .PAK file could not be found. The patcher cannot continue..
        /// </summary>
        internal static string PakFileNotFound {
            get {
                return ResourceManager.GetString("PakFileNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Patch Skipped.
        /// </summary>
        internal static string PatchSkipped {
            get {
                return ResourceManager.GetString("PatchSkipped", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Patch successful!.
        /// </summary>
        internal static string PatchSuccessful {
            get {
                return ResourceManager.GetString("PatchSuccessful", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Steam is not currently installed, or its library folders could not be detected..
        /// </summary>
        internal static string SteamNotInstalled {
            get {
                return ResourceManager.GetString("SteamNotInstalled", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to UNTESTED VERSION
        ///
        ///Your Halo installation (version {0}) is untested and this patcher is not guaranteed to work.
        ///Proceeding with patch may cause unintended side effects.
        ///The next step will confirm if you want to create a backup. It is highly recommended you do so.
        ///
        ///Would you like to continue with patching?.
        /// </summary>
        internal static string UntestedVersion {
            get {
                return ResourceManager.GetString("UntestedVersion", resourceCulture);
            }
        }
    }
}
