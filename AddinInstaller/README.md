Installer
===

The Addin DLL is always built as AnyCPU

The installer configuration, however, is split between x86 and x64 (due to WOW64).
Fortunately, WiX will create the appropriate installer by simply picking what platform to build:

![](http://i.imgur.com/sLi0zZZ.jpg)

No changes or switch cases in the WiX code required :)
