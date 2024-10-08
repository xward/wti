﻿Imports System.Diagnostics
Imports System.IO

Namespace CST
	Public Module CST
		Public ROOT_PATH As String = Path.GetFullPath(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) & "/../../..")

		Public DATA_PATH As String = ROOT_PATH & "/data"

		Public COMPILED As Boolean = Not (Debugger.IsAttached)

		Public SCREEN As Rectangle = New Rectangle(0, 0, My.Computer.Screen.Bounds.Size.Width, My.Computer.Screen.Bounds.Size.Height - 32)

		Public HOST_NAME As hostNameEnum

		Public Enum hostNameEnum
			UNKNOWN
			GALACTICA
			VIPER
			' rip
			GHOST
		End Enum

		Public Sub init()
			Select Case My.Computer.Name
				Case "DESKTOP-58FECV7"
					HOST_NAME = hostNameEnum.GALACTICA
				Case "DESKTOP-PBM317F"
					HOST_NAME = hostNameEnum.VIPER
				Case "EPIA-GHOST"
					HOST_NAME = hostNameEnum.GHOST
			End Select

			dbg.info("Running on computer " & HOST_NAME.ToString)
		End Sub

	End Module
End Namespace
