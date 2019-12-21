# HaloMCCForgePatcher

A patcher to enable Forge World in Halo: MCC (PC Edition)

## How does it work?
HaloMCCForgePatcher works by altering the following 2 bytes in the game file `MCC-WindowsNoEditor.pak`:

* Offset `0x1E302110` to `0x27`
* Offset `0x1E2F52D0` to `0x27`

## How do I use it?
Just run **HaloMCCForgePatcher.exe** and it will automatically detect Halo from your Steam library, and patch your game instantly.

## What if I don't have the Steam version?
If the patcher fails to find the Steam library version of the game, it will prompt you to manually search for your Halo installation directory.

If you purchased *Halo: The Master Chief Collection* from the Microsoft Store, you will need to run UWPDumper to extract the game files in order for the patcher to work. There is a guide [here](https://www.reddit.com/r/halomods/comments/e5tsmu/dumping_the_ms_store_version_of_halo_mcc/) which explains this process in detail.

## Important notes
This patcher does not create a backup of your .PAK file. Please be sure you have a safe copy of the original available should you wish to revert this patch.

## Disclaimer
This patcher is provided as is, and by using it you accept liability should this tool behave unexpectedly.