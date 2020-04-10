Imports System.Linq
Imports Microsoft.VisualBasic

Public Class ImageRepository

	Private _Files As New List(Of String)

	Private _Sizes As New List(Of Long)

	Private _Display As MainWindow

	Public Sub WireUp(startupFileFullName As String, display As MainWindow)
		_Display = display

		If (Not String.IsNullOrEmpty(startupFileFullName)) Then
			Me.CurrentImage = Me.LoadImage(startupFileFullName)
		Else
			startupFileFullName = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName
		End If

		Me.FetchDirectory(startupFileFullName)

		If (_Files.Count = 0) Then
			Exit Sub
		End If

		_CurrentIndex = _Files.IndexOf(startupFileFullName)
	End Sub

	Private _CurrentImage As BitmapImage

	Private _CurrentIndex As Integer = -1

	Public ReadOnly Property Files As List(Of String)
		Get
			Return _Files
		End Get
	End Property

	Public ReadOnly Property IsReady As Boolean
		Get
			Return (_CurrentIndex <> -1)
		End Get
	End Property

	Public Property CurrentImage As BitmapImage
		Get
			Return _CurrentImage
		End Get
		Set(value As BitmapImage)
			If (value Is _CurrentImage) Then
				Exit Property
			End If
			_CurrentImage = value
			_Display.CurrentImage = value
		End Set
	End Property

	Public Sub DeleteCurrentFile()

		FileIO.FileSystem.DeleteFile(Me.CurrentFileFullName, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.SendToRecycleBin)

		_Files.RemoveAt(_CurrentIndex)

		If (_Files.Count - 1 < _CurrentIndex) Then
			_CurrentIndex -= 1
		End If
		If (_CurrentIndex < 0) Then
			End
		End If

		Me.Reload()
	End Sub

	Private Sub FetchDirectory(startupFileFullName As String)
		Dim startupDirectory = System.IO.Path.GetDirectoryName(startupFileFullName)
		Dim files = New System.IO.DirectoryInfo(startupDirectory).GetFiles().OrderBy(Function(x) x.Name)
		For Each file In files
			Select Case file.Extension.ToLower
				Case ".jpg", ".jpeg", ".gif", ".png"
					_Files.Add(file.FullName)
					_Sizes.Add(file.Length \ 1024)
			End Select
		Next
	End Sub

	Public ReadOnly Property CurrentFileFullName As String
		Get
			Return _Files(Me.CurrentIndex)
		End Get
	End Property

	Public ReadOnly Property CurrentFileSize As Long
		Get
			Return _Sizes(Me.CurrentIndex)
		End Get
	End Property

	Public Property CurrentIndex As Integer
		Get
			Return _CurrentIndex
		End Get
		Set(value As Integer)
			If (value = _CurrentIndex) Then
				Exit Property
			End If
			If (value < 0) Then
				Exit Property
			End If
			If (value > _Files.Count - 1) Then
				Exit Property
			End If
			_CurrentIndex = value
			Me.Reload()
		End Set
	End Property

	Private Sub Reload()
		Me.CurrentImage = Me.LoadImage(_Files(_CurrentIndex))
	End Sub

	Private Function LoadImage(fileFullName As String) As BitmapImage
		Dim bmi As New BitmapImage()
		bmi.BeginInit()
		bmi.CacheOption = BitmapCacheOption.OnLoad
		bmi.CreateOptions = BitmapCreateOptions.IgnoreColorProfile
		bmi.UriSource = New Uri(fileFullName, UriKind.Absolute)
		bmi.EndInit()
		bmi.Freeze()
		Return bmi
	End Function

End Class
