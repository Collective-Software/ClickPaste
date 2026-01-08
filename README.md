# ClickPaste
Windows 10/11 notification area app in C# that can paste clipboard contents as keystrokes to whatever location you click.

## Starting

* Launch ClickPaste.exe, or put it in your Startup folder.
* If you intend to paste onto windows of UAC-elevated processes, you need ClickPaste.exe to be running UAC-elevated as well.  You can do this by making a shortcut and setting its properties to "Run as Administrator"

 ![How to run as administrator](./doc/RunAsAdmin.png)

## Usage

First of course, you need to have some text in your clipboard.  Then:

1. Click the target notification icon to begin:

 ![Click to choose a target](./doc/ClickToTarget.png)

2. Click to choose a location you want to paste the text:

 ![Click to choose a location to paste](./doc/ClickToPaste.png)
    
3. Your clipboard contents should be typed as keystrokes onto the window you selected:

 ![Your clipboard is typed as keystrokes onto the window you selected](./doc/Pasted.png)

## Settings

Right-click the notification icon and select Settings.

* You can change between key typing modes,
* Set how much delay there is before and between keystrokes, 
* Trigger a confirmation when the clipboard contains more than a chosen number of characters,
* Configure what "hot key" combination will be used (Clear the key textbox with delete or backspace if you wish to have *no* hotkey),
* Configure whether the "hot key" will activate the target selector to pick a paste location or just start typing immediately. 

![How to change settings](./doc/RightClickForSettings.png)
![Settings dialog](./doc/Settings.png)

## Stopping

* Right-click the notification icon and select Exit.

 ![How to exit](./doc/RightClickToExit.png)
 
## Download 

* [Go to Releases page](../../releases/latest)