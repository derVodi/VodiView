Public Class CommandBar

	Protected Overrides Sub OnMouseEnter(e As MouseEventArgs)
		MyBase.OnMouseEnter(e)
		Me.Opacity = 1
	End Sub

	Protected Overrides Sub OnMouseLeave(e As MouseEventArgs)
		MyBase.OnMouseLeave(e)
		Me.Opacity = 0
	End Sub

	Private Sub gClose_Click(sender As Object, e As RoutedEventArgs) Handles gClose.Click
		End
	End Sub

	Private ReadOnly Property MainWin As MainWindow
		Get
			Return DirectCast(Application.Current.MainWindow, MainWindow)
		End Get
	End Property

	Private Sub gNormalize_Click(sender As Object, e As RoutedEventArgs) Handles gNormalize.Click
		DirectCast(Application.Current.MainWindow, MainWindow).ToggleFullScreen()
	End Sub

	Private Sub gShowInfo_Click(sender As Object, e As RoutedEventArgs) Handles gShowInfo.Click
		Me.MainWin.ToggleInformationDisplay()
	End Sub

	Public Property ShowWindowControls() As Boolean
		Get
			Return (gClose.Visibility = Visibility.Visible)
		End Get
		Set(value As Boolean)
			'gNormalize.Visibility = If(value, Visibility.Visible, Visibility.Collapsed)
			gClose.Visibility = If(value, Visibility.Visible, Visibility.Collapsed)
		End Set
	End Property

End Class
