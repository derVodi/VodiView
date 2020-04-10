Imports System.Text

Class MainWindow

	Private _ImageCache As ImageRepository

	Private _IsInitialized As Boolean

	Public ReadOnly Property ImageCache As ImageRepository
		Get
			Return _ImageCache
		End Get
	End Property

	Protected Overrides Sub OnActivated(e As EventArgs)
		MyBase.OnActivated(e)
		If (Not _IsInitialized) Then
			_IsInitialized = True
			Me.Initialize()
		End If
	End Sub

	Private Sub Initialize()

		Dim presentationsource As PresentationSource = PresentationSource.FromVisual(Me)
		_DpiZoomW = presentationsource.CompositionTarget.TransformToDevice.M11
		_DpiZoomH = presentationsource.CompositionTarget.TransformToDevice.M22

		Dim args = Environment.GetCommandLineArgs()
		Dim startupFile As String = Nothing

		If (args.Length > 1) Then
			startupFile = args(1)
		End If

		_ImageCache = New ImageRepository()
		_ImageCache.WireUp(startupFile, Me)

		If (_ImageCache.Files.Count = 0) Then
			MessageBox.Show(Lingo.NoImagesHere, Versioning.Constants.AppName + " " + Versioning.Constants.AppVersion)
			End
		End If

	End Sub

	Private _ImageInformation As String

	Private Sub RefreshAllInformation()

		If (Not _ImageCache.IsReady) Then
			Exit Sub
		End If

		Dim bitmap = Me.CurrentImage
		Dim i As Integer = _ImageCache.CurrentIndex

		Dim sb As New StringBuilder
		sb.Append(i + 1)
		sb.Append(" / ")
		sb.Append(_ImageCache.Files.Count)
		sb.Append(" • """)
		sb.Append(_ImageCache.CurrentFileFullName)
		sb.Append(""" • {")
		sb.Append(_ImageCache.CurrentFileSize.ToString("N0"))
		sb.Append(" KB} • [")
		sb.Append(bitmap.PixelWidth)
		sb.Append(" x ")
		sb.Append(bitmap.PixelHeight)
		sb.Append("] • (")
		sb.Append(bitmap.DpiX)
		sb.Append(" x ")
		sb.Append(bitmap.DpiY)
		sb.Append(") DPI • ")
		_ImageInformation = sb.ToString
		RefreshScalingInformation()

		Me.Title = System.IO.Path.GetFileName(_ImageCache.CurrentFileFullName)

	End Sub

	Private Sub RefreshScalingInformation()
		gInformation.Content = _ImageInformation + Me.ScaleFactor.ToString + "%"
	End Sub

	Protected Overrides Sub OnMouseWheel(e As MouseWheelEventArgs)
		MyBase.OnMouseWheel(e)

		Dim direction As Integer

		If (e.Delta > 0) Then
			direction = 1 'up
		Else
			direction = -1 'down
		End If

		If (Keyboard.IsKeyDown(Key.LeftCtrl) OrElse Keyboard.IsKeyDown(Key.RightCtrl)) Then
			Zoom(direction)
		Else
			'Next/prev Image
			_ImageCache.CurrentIndex -= direction
		End If

	End Sub

	Private Sub GetOriginalSize(ByRef w As Double, ByRef h As Double)
		Dim bitmap = DirectCast(gImage.Source, BitmapImage)
		w = bitmap.PixelWidth / _DpiZoomW
		h = bitmap.PixelHeight / _DpiZoomH
	End Sub

	Public Sub OriginalSize()
		GetOriginalSize(gImage.Width, gImage.Height)
		Me.Center()
		Me.RefreshScalingInformation()
	End Sub

	Public ReadOnly Property W As Double
		Get
			Return DirectCast(Me.Content, Panel).ActualWidth
		End Get
	End Property

	Public ReadOnly Property H As Double
		Get
			Return DirectCast(Me.Content, Panel).ActualHeight
		End Get
	End Property

	Private _DpiZoomW As Double

	Private _DpiZoomH As Double

	Public Sub Fit(Optional doNotUpscale As Boolean = False)

		Dim windowWidth As Double = Me.W
		Dim windowHeight As Double = Me.H

		Dim originalWidth As Double
		Dim originalHeight As Double
		Me.GetOriginalSize(originalWidth, originalHeight)

		If (doNotUpscale AndAlso windowWidth >= originalWidth AndAlso windowHeight >= originalHeight) Then
			gImage.Width = originalWidth
			gImage.Height = originalHeight
			Me.Center()
			RefreshScalingInformation()
			Exit Sub
		End If

		Dim factor As Double = windowWidth / originalWidth
		Dim newHeight As Double = originalHeight * factor
		If (newHeight <= windowHeight) Then
			gImage.Width = windowWidth
			gImage.Height = newHeight
		Else
			factor = windowHeight / originalHeight
			gImage.Width = originalWidth * factor
			gImage.Height = windowHeight
		End If

		Me.Center()
		RefreshScalingInformation()

	End Sub

	Public ReadOnly Property ScaleFactor As Integer
		Get
			Dim originalWidth As Double
			Dim originalHeight As Double
			Me.GetOriginalSize(originalWidth, originalHeight)
			Return CInt(gImage.Width * 100 / originalWidth)
		End Get
	End Property

	''' <seealso> OnTextInput() </seealso>
	Protected Overrides Sub OnKeyDown(e As KeyEventArgs)
		MyBase.OnKeyDown(e)
		KeyboardHandler.OnKeyDown(e)
	End Sub

	Public Sub ToggleInformationDisplay()
		If (gInformation.Visibility <> Visibility.Visible) Then
			gInformation.Visibility = Visibility.Visible
		Else
			gInformation.Visibility = Visibility.Hidden
		End If
	End Sub

	''' <seealso> OnKeyDown() </seealso>
	Protected Overrides Sub OnTextInput(e As TextCompositionEventArgs)
		MyBase.OnTextInput(e)
		KeyboardHandler.OnTextInput(e)
	End Sub

	Public Property CurrentImage As BitmapImage
		Get
			Return DirectCast(gImage.Source, BitmapImage)
		End Get
		Set(value As BitmapImage)
			gImage.Source = value
			Me.Fit()
			Me.RefreshAllInformation()
		End Set
	End Property

	Public Shared ReadOnly Property Instance As MainWindow
		Get
			Return DirectCast(Application.Current.MainWindow, MainWindow)
		End Get
	End Property

	Public ReadOnly Property IsFitting() As Boolean
		Get
			Return ((gImage.Height = Me.H AndAlso gImage.Width <= Me.W) OrElse (gImage.Width = Me.W AndAlso gImage.Height <= Me.H))
		End Get
	End Property

	Public Sub FitOrOriginal()
		If (Me.IsFitting) Then
			Me.OriginalSize()
		Else
			Me.Fit()
		End If
	End Sub

	Public Sub Zoom(direction As Integer)
		Dim zoomSpeed As Integer = 10

		gImage.Width += direction * gImage.Width * zoomSpeed / 100
		gImage.Height += direction * gImage.Height * zoomSpeed / 100

		Center()
		RefreshScalingInformation()

	End Sub

	Protected Overrides Sub OnMouseDown(e As MouseButtonEventArgs)
		MyBase.OnMouseDown(e)
		_MouseXFromMe = e.GetPosition(Me).X
		_MouseYFromMe = e.GetPosition(Me).Y

		_MarginLeftBeforeMoving = gImage.Margin.Left
		_MarginTopBeforeMoving = gImage.Margin.Top
	End Sub

	Protected Overrides Sub OnMouseMove(e As MouseEventArgs)
		MyBase.OnMouseMove(e)
		Dim tDeltaX, tDeltaY As Double
		If (e.LeftButton = MouseButtonState.Pressed) Then

			tDeltaX = e.GetPosition(Me).X - _MouseXFromMe
			tDeltaY = e.GetPosition(Me).Y - _MouseYFromMe

			Dim tNewPos = New Thickness(_MarginLeftBeforeMoving + tDeltaX, _MarginTopBeforeMoving + tDeltaY, 0, 0)
			gImage.Margin = tNewPos
		End If
	End Sub

	Private Sub Center()
		Dim w As Double = DirectCast(Me.Content, Panel).ActualWidth
		Dim h As Double = DirectCast(Me.Content, Panel).ActualHeight

		Dim margin As New Thickness((w / 2) - (gImage.Width / 2), (h / 2) - (gImage.Height / 2), 0, 0)
		gImage.Margin = margin
	End Sub

	Private Sub Me_SizeChanged(sender As Object, e As SizeChangedEventArgs) Handles Me.SizeChanged
		If (Not _IsInitialized) Then
			Exit Sub
		End If
		Me.Fit()
	End Sub

	Private _MouseXFromMe As Double
	Private _MouseYFromMe As Double
	Private _MarginLeftBeforeMoving As Double
	Private _MarginTopBeforeMoving As Double

	Private _ReMaximizing As Boolean

	Public Sub ToggleFullScreen()
		'If (Me.WindowState = WindowState.Maximized) Then
		'	Me.WindowState = WindowState.Normal
		'Else
		'	Me.WindowState = WindowState.Maximized
		'End If
		If (Me.WindowStyle = WindowStyle.None) Then
			Me.WindowStyle = WindowStyle.SingleBorderWindow
			Me.ResizeMode = ResizeMode.CanResize
			gWindowBar.ShowWindowControls = True
		Else
			Me.WindowState = WindowState.Normal
			Me.WindowStyle = WindowStyle.None
			Me.ResizeMode = ResizeMode.NoResize
			Me.WindowState = WindowState.Maximized
			_ReMaximizing = True
		End If
	End Sub

	Protected Overrides Sub OnStateChanged(e As EventArgs)
		MyBase.OnStateChanged(e)

		'If (Me.WindowState = WindowState.Maximized) Then
		'	If (Me.WindowStyle = WindowStyle.None) Then
		'		gWindowBar.ShowWindowControls = True
		'	Else
		'		_ReMaximizing = True
		'		Me.WindowState = WindowState.Normal
		'	End If

		'Else
		'	If (Not _ReMaximizing) Then
		'		Me.WindowStyle = WindowStyle.SingleBorderWindow
		'		Me.ResizeMode = ResizeMode.CanResize
		'		gWindowBar.ShowWindowControls = False
		'	Else
		'		_ReMaximizing = False
		'		Me.ResizeMode = ResizeMode.NoResize
		'		Me.WindowStyle = WindowStyle.None
		'		Me.WindowState = WindowState.Maximized
		'	End If
		'End If

		'Me.Fit()
	End Sub

End Class