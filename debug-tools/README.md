How to debug the Office COM Addin
===

In order to debug the Addin, such as attaching it to the Visual Studio debugger (hit F5 yay!), you will need to:

1. Register the COM DLL into the registry. Run the provided bat scripts (x86 or x64, depending on the **installed version of office**)

2. Add another key in the registry to tell PoewrPoint to load the DLL as an addin.

  Create the key: HKLM\Software\Microsoft\Office\PowerPoint\Addins\WiimoteAddin.Connect

  In this key, create a new DWORD 'LoadBehaviour' with a value of '3'

Adjust the VS project properties to suit if your PowerPoint exe location is different from mine (Debug tab > Start external program)

Now hit F5 :)
