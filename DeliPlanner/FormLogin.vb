Public Class FormLogin

#Region "定義"

    Private _failedCount As Integer = 0

#End Region

#Region "イベント"

    Private Sub Me_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Me.Text = Application.ProductName & " ログイン"

        ' フォームの初期化
        InitializeForm(True)

    End Sub

    Private Sub Me_Shown(sender As Object, e As EventArgs) Handles Me.Shown

        ' 担当者が1件も無ければ初期管理者を作る
        Try
            If Operators.GetAll.Count = 0 Then
                Operators.CreateDefaultAdministrator()
                MessageBox.Show("担当者が登録されていないため、初期管理者を作成しました。" & vbCrLf & vbCrLf &
                                "担当者コード：admin" & vbCrLf &
                                "パスワード　：admin" & vbCrLf & vbCrLf &
                                "ログイン後、必ずパスワードを変更してください。",
                                Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            SysMsgBox(ex.Message, Me.Text)
            Me.DialogResult = DialogResult.Cancel
            Me.Close()
            Exit Sub
        End Try

        TextBoxOperatorCode.Focus()

    End Sub

    Private Sub ButtonLogin_Click(sender As Object, e As EventArgs) Handles ButtonLogin.Click

        ' 更新前チェック
        If CheckData() = False Then
            Exit Sub
        End If

        If Login(TextBoxOperatorCode.Text.Trim, TextBoxPassword.Text) Then
            Me.DialogResult = DialogResult.OK
            Me.Close()
            Exit Sub
        End If

        ' 認証失敗
        _failedCount += 1
        LabelMessage.Text = "担当者コードまたはパスワードが違います。"

        If _failedCount >= 5 Then
            MessageBox.Show("ログインに5回失敗したため終了します。", Me.Text,
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Me.DialogResult = DialogResult.Cancel
            Me.Close()
            Exit Sub
        End If

        TextBoxPassword.Focus()

    End Sub

    Private Sub ButtonClose_Click(sender As Object, e As EventArgs) Handles ButtonClose.Click

        Me.DialogResult = DialogResult.Cancel
        Me.Close()

    End Sub

    Private Sub TextBox_TextChanged(sender As Object, e As EventArgs) _
        Handles TextBoxOperatorCode.TextChanged, TextBoxPassword.TextChanged

        LabelMessage.Text = String.Empty

    End Sub

#End Region

#Region "メソッド"

    Private Function CheckData() As Boolean

        Dim result As Boolean = True

        Try
            If String.IsNullOrEmpty(TextBoxOperatorCode.Text.Trim) Then
                TextBoxOperatorCode.Focus()
                Throw New ApplicationException("担当者コードを入力してください。")
            End If
        Catch ex As Exception
            LabelMessage.Text = ex.Message
            result = False
        End Try

        Return result

    End Function

    Private Sub InitializeForm(Optional isLoading As Boolean = False)

        TextBoxOperatorCode.Clear()
        TextBoxPassword.Clear()
        LabelMessage.Text = String.Empty

        If Not isLoading Then TextBoxOperatorCode.Focus()

    End Sub

#End Region

End Class