Imports Microsoft.VisualBasic

Public Class Lingo

	Public Const Help As String = (
		"Keyboard" + ControlChars.CrLf + ControlChars.CrLf +
		"[Escape]" + ControlChars.Tab + ControlChars.Tab + "Exit" + ControlChars.CrLf +
		"[⇦] [⇨]" + ControlChars.Tab + ControlChars.Tab + "Previous / next image" + ControlChars.CrLf +
		"[Home] [End]" + ControlChars.Tab + "First / last image" + ControlChars.CrLf +
		"[.],[#],[*]" + ControlChars.Tab + ControlChars.Tab + "Fit to window/screen" + ControlChars.CrLf +
		"[+] [-]" + ControlChars.Tab + ControlChars.Tab + "Zoom" + ControlChars.CrLf +
		"[,],[/],[.]" + ControlChars.Tab + ControlChars.Tab + "100% Zoom" + ControlChars.CrLf +
		"[0]" + ControlChars.Tab + ControlChars.Tab + "Toggle 100% Zoom / Fit to window" + ControlChars.CrLf +
		"[F]" + ControlChars.Tab + ControlChars.Tab + "Full screen / window" + ControlChars.CrLf +
		"[I]" + ControlChars.Tab + ControlChars.Tab + "Information display" + ControlChars.CrLf +
		"[L]" + ControlChars.Tab + ControlChars.Tab + "License" + ControlChars.CrLf +
		"[F2]" + ControlChars.Tab + ControlChars.Tab + "Rename"
	)

	Public Const NoImagesHere As String = (
		"No Images around here. " + ControlChars.CrLf +
		"Place the exe into a directory with pictures or use ""Open with..."" to open images using " + Versioning.Constants.AppName + "."
	)

End Class
