# Howto build the installer

## Setup

Install:

* Install WiX Toolset build tools and WiX Toolset Visual Studio Extension from <http://wixtoolset.org/releases/>
* Install IsWiX from <https://github.com/iswix-llc/iswix/releases>

Details on <https://github.com/iswix-llc/iswix-tutorials>

## Build

* Switch from Debug to Release and Build ClickPaste.
* A Post Debug Script copies bin/Release to Installer/Deploy
* Now RightClick `ClickPasteInstaller` Project in Tree and select Build
* You can find now your MSI at `Installer/ClickPasteInstaller/bin/Release/ClickPasteInstaller.msi`

## Changes/More Files

To add more files to installer you can use IsWiX GUI.

Open File `ClickPasteInstallerMM/ClickPasteInstallerMM.wxs` and then select in VisualStudio Menu Tools -&gt; Launch IsWiX
