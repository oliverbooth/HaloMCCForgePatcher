# HaloMCCForgePatcher
A patcher to enable Forge World in Halo: MCC (PC Edition)

## What does it do?
It enables the **Create > Forge** option in the game menu, allowing you build your own game maps in Custom games.

## How does it work?
HaloMCCForgePatcher works by altering the following 2 bytes in the game file `MCC-WindowsNoEditor.pak`:

* Offset `0x2FFC72D0` to `0x27`
* Offset `0x2FFD4110` to `0x27`

## How do I use it?
Just run **HaloMCCForgePatcher.exe** and it will automatically detect Halo from your Steam library, and patch your game instantly.

## What if I don't have the Steam version?
If the patcher fails to find the Steam library version of the game, it will prompt you to manually search for your Halo installation directory.

If you purchased *Halo: The Master Chief Collection* from the Microsoft Store, you will need to run UWPDumper to extract the game files in order for the patcher to work. There is a guide [here](https://www.reddit.com/r/halomods/comments/e5tsmu/dumping_the_ms_store_version_of_halo_mcc/) which explains this process in detail.

## Disclaimer
It is unlikely that this will get you banned. It does not change online play, nor give you any unfair advantage in any way. All it does is modify the game menu.

At any rate, this patcher is provided as is, and by using it you accept liability should this tool behave unexpectedly.
