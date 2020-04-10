Public Class RenameWindow

	Public Property BaseDirectory As String

	Private _File As String

	Private _Directory As String

	Private _Extension As String

	Private _FileNameWithoutExtension As String

	Public Property File As String
		Get
			Return _File
		End Get
		Set(value As String)
			_File = value
			_Directory = System.IO.Path.GetDirectoryName(value)
			_Extension = System.IO.Path.GetExtension(value)
			Me.OriginalNameTextBox.Text = System.IO.Path.GetFileNameWithoutExtension(value)
			Me.NewNameTextBox.Text = Me.OriginalNameTextBox.Text
		End Set
	End Property

	Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
		System.IO.File.Move(_File, System.IO.Path.Combine(_Directory, Me.NewNameTextBox.Text + _Extension))
		' todo do not rename here but let cache do it
		Me.Close()
	End Sub

	Private Sub Window_Activated(sender As Object, e As EventArgs)
		Me.NewNameTextBox.Focus()
		Me.NewNameTextBox.SelectAll()
	End Sub

End Class
