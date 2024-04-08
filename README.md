# Create native Mir Window Example
* Writen in C or C#
* NOTE: C# proj for legacy MonoDevelop ```.NETFW 4``` and one for targeting ```.NET 8```
* Backed up Paint and GLES2 demos
* Tested with Ubuntu 17.04 / Unity8 on ```Intel Celeron N3350``` and Unity 16.04 UBPorts on ```BQ Aquaris M10```

## To Build
* sudo apt install libmirclient-dev
* sudo apt install libgles2-mesa-dev
* NOTE: build.sh scripts live in C folders
* NOTE: Projects setup for MonoDevelop but can just use terminal

## To Run
* [my-app].desktop file is required in ```~/.local/share/applications``` or ```usr/share/applications``` folder to run
* Desktop file will look something like this below...
```
[Desktop Entry]
Version=1.0
Name=Basic
Exec=<path-to-app>
Type=Application
Icon=<path-to-icon.png>
Categories=WebBrowser;
StartupNotify=true
Path=<path-to-app-folder>
```

## Ubuntu Touch (UBPorts)
* Allow "sudo apt" commands etc: ```sudo mount -o remount,rw /```
* NOTE: after this you can install build tools on ARM64 devices directly