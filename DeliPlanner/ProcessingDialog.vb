Public NotInheritable Class ProcessingDialog

    Public WriteOnly Property ProcessName() As String
        Set( value As String)
            LabelProcess.Text = value
            Me.Update()
        End Set
    End Property

    Public WriteOnly Property Message1() As String
        Set( value As String)
            LabelMessage1.Text = value
            Me.Update()
        End Set
    End Property

    Public WriteOnly Property Message2() As String
        Set( value As String)
            LabelMessage2.Text = value
            Me.Update()
        End Set
    End Property

    Public Property ProgressBar() As ProgressBar
        Get
            Return ProgressBarStatus
        End Get
        Set( value As ProgressBar)
            ProgressBarStatus = ProgressBar
        End Set
    End Property

    Private Sub Me_Load( sender As System.Object,  e As System.EventArgs) Handles MyBase.Load

        Me.StartPosition = FormStartPosition.Manual
        If Me.Owner IsNot Nothing Then
            Me.Location = New System.Drawing.Point(Me.Owner.Location.X + (Me.Owner.Width - Me.Width) / 2, Me.Owner.Location.Y + (Me.Owner.Height - Me.Height) / 2)
        End If

        LabelProcess.Text = "処　理　中"
        LabelMessage1.Text = String.Empty
        LabelMessage2.Text = String.Empty

    End Sub

End Class
