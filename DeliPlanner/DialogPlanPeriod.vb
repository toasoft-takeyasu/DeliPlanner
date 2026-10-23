Public Class DialogPlanPeriod

#Region "定義"

    Public Property BaseControl As Control
    Public Property StartDate As Date
    Public Property EndDate As Date

#End Region

#Region "イベント"

    Private Sub Me_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        ' フォームの初期化
        InitializeForm(True)

    End Sub

    Private Sub Me_Shown(sender As Object, e As EventArgs) Handles Me.Shown

        DateBoxStartDate.Focus()

    End Sub

    Private Sub ButtonExport_Click(sender As Object, e As EventArgs) Handles ButtonExport.Click

        ' 更新前チェック
        If CheckData() = False Then
            Exit Sub
        End If

        StartDate = CDate(DateBoxStartDate.Text)
        EndDate = CDate(DateBoxEndDate.Text)

        Me.DialogResult = DialogResult.OK
        Me.Close()

    End Sub

    Private Sub ButtonClose_Click(sender As Object, e As EventArgs) Handles ButtonClose.Click

        Me.DialogResult = DialogResult.Cancel
        Me.Close()

    End Sub

#End Region

#Region "メソッド"

    Private Function CheckData() As Boolean

        Dim result As Boolean = True

        Try
            If String.IsNullOrEmpty(DateBoxStartDate.Text) Then
                DateBoxStartDate.Focus()
                Throw New ApplicationException("開始日を入力してください。")
            End If
            If Not IsDate(DateBoxStartDate.Text) Then
                DateBoxStartDate.Focus()
                Throw New ApplicationException("開始日が正しくありません。")
            End If
            If String.IsNullOrEmpty(DateBoxEndDate.Text) Then
                DateBoxEndDate.Focus()
                Throw New ApplicationException("終了日を入力してください。")
            End If
            If Not IsDate(DateBoxEndDate.Text) Then
                DateBoxEndDate.Focus()
                Throw New ApplicationException("終了日が正しくありません。")
            End If
            If CDate(DateBoxStartDate.Text) > CDate(DateBoxEndDate.Text) Then
                DateBoxEndDate.Focus()
                Throw New ApplicationException("終了日は開始日以降を入力してください。")
            End If
            If CDate(DateBoxEndDate.Text).Subtract(CDate(DateBoxStartDate.Text)).Days > 92 Then
                DateBoxEndDate.Focus()
                Throw New ApplicationException("期間は約3か月以内で指定してください。")
            End If
        Catch ex As Exception
            If ex.GetType Is GetType(ApplicationException) Then
                MessageBox.Show(ex.Message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Else
                SysMsgBox(ex.Message, Me.Text)
            End If
            result = False
        End Try

        Return result

    End Function

    Private Sub InitializeForm(Optional isLoading As Boolean = False)

        ' 呼び出し側から日付が渡されていなければ今日
        If StartDate = Date.MinValue Then StartDate = Now.Date
        If EndDate = Date.MinValue Then EndDate = StartDate

        DateBoxStartDate.Text = StartDate.ToString("yyyy/MM/dd")
        DateBoxEndDate.Text = EndDate.ToString("yyyy/MM/dd")

        If Not isLoading Then DateBoxStartDate.Focus()

    End Sub

#End Region

End Class
