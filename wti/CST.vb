Imports System.Diagnostics
Imports System.IO

Namespace CST
	Module CST
		Public SLN As String = Path.GetFullPath(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) & "/../../..")
		Public COMPILED As Boolean = Not (Debugger.IsAttached)
	End Module
End Namespace


