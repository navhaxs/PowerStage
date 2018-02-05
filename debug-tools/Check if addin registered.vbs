set o = CreateObject("WiimoteAddin.Connect")

'
' Active X component        --->    The Addin is not registered
' can't create component            (i.e. uninstalled/registered under different name)
'
'
' NO error message          --->    The Addin loads correctly!
'
'
' An attempt was made to    --->    Make sure the Add-in DLL is built
' load a program with an            for the 'AnyCPU' platform
' incorrect format.
'