Public Class KeyboardHandler

	Public Shared Sub OnTextInput(e As TextCompositionEventArgs)
		Select Case e.Text
			Case "+"
				MainWindow.Instance.Zoom(1)
			Case "-"
				MainWindow.Instance.Zoom(-1)
			Case "0"
				MainWindow.Instance.FitOrOriginal()
			Case "/", ","
				MainWindow.Instance.OriginalSize()
			Case ".", "#", "*"
				MainWindow.Instance.Fit()
			Case "i", "I"
				MainWindow.Instance.ToggleInformationDisplay()
			Case "f", "F"
				MainWindow.Instance.ToggleFullScreen()
			Case "l", "L"
				MessageBox.Show(Versioning.Constants.License, "License")
		End Select
	End Sub

	Public Shared Sub OnKeyDown(e As KeyEventArgs)

		Dim imageCache As ImageRepository = MainWindow.Instance.ImageCache

		Select Case e.Key

			Case Key.F1
				MessageBox.Show(Lingo.Help, Versioning.Constants.AppName + " " + Versioning.Constants.AppVersion)

			Case Key.F2
				Dim w As New RenameWindow
				w.Owner = MainWindow.Instance
				w.File = imageCache.CurrentFileFullName
				w.ShowDialog()

				' TODO: also rename in cache

			Case Key.Delete
				MainWindow.Instance.ImageCache.DeleteCurrentFile()

			Case Key.Escape
				MainWindow.Instance.Visibility = Visibility.Hidden
				End

			Case Key.End

				imageCache.CurrentIndex = imageCache.Files.Count - 1

			Case Key.Left, Key.Up, Key.PageUp

				imageCache.CurrentIndex -= 1

			Case Key.Home

				imageCache.CurrentIndex = 0

			Case Key.Right, Key.Down, Key.PageDown

				imageCache.CurrentIndex += 1

		End Select

	End Sub

End Class
