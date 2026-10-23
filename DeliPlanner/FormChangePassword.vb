Public Class FormChangePassword

#Region "定義"

    Private ReadOnly _target As [Operator]

#End Region

#Region "コンストラクタ"

    Public Sub New(target As [Operator])

        InitializeComponent()
        _target = target

    End Sub

#End Region

#Region "イベント"

    Private Sub Me_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' フォームの初期化
        InitializeForm(True)

    End Sub

    Private Sub Me_Shown(sender As Object, e As EventArgs) Handles Me.Shown

        TextBoxCurrentPassword.Focus()

    End Sub

    Private Sub ButtonRegist_Click(sender As Object, e As EventArgs) Handles ButtonRegist.Click

        ' 更新前チェック
        If CheckData() = False Then
            Exit Sub
        End If

        ' レコード更新
        If MessageBox.Show("パスワードを変更します。よろしいですか？", Me.Text,
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                           MessageBoxDefaultButton.Button2) = DialogResult.Yes Then
            Try
                Operators.SavePassword(_target.OperatorId, TextBoxNewPassword.Text)
            Catch ex As Exception
                SysMsgBox(ex.Message, Me.Text)
                Exit Sub
            End Try

            ' ログイン中の担当者なら手元の情報も更新する
            If sharedOperator IsNot Nothing AndAlso sharedOperator.OperatorId = _target.OperatorId Then
                sharedOperator = Operators.GetById(_target.OperatorId)
            End If

            MessageBox.Show("パスワードを変更しました。", Me.Text,
                            MessageBoxButtons.OK, MessageBoxIcon.Information)

            Me.DialogResult = DialogResult.OK
            Me.Close()
        End If

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
            ' 現在のパスワード
            If Not Operators.VerifyPassword(_target, TextBoxCurrentPassword.Text) Then
                TextBoxCurrentPassword.Focus()
                Throw New ApplicationException("現在のパスワードが違います。")
            End If

            ' 新しいパスワード
            If String.IsNullOrEmpty(TextBoxNewPassword.Text) Then
                TextBoxNewPassword.Focus()
                Throw New ApplicationException("新しいパスワードを入力してください。")
            End If

            If TextBoxNewPassword.Text.Length < 4 Then
                TextBoxNewPassword.Focus()
                Throw New ApplicationException("新しいパスワードは4文字以上で入力してください。")
            End If

            ' 確認用
            If TextBoxNewPassword.Text <> TextBoxConfirmPassword.Text Then
                TextBoxConfirmPassword.Focus()
                Throw New ApplicationException("新しいパスワードが確認用と一致しません。")
            End If
        Catch ex As Exception
            SysMsgBox(ex.Message, Me.Text)
            result = False
        End Try

        Return result

    End Function

    Private Sub InitializeForm(Optional isLoading As Boolean = False)

        TextBoxCurrentPassword.Clear()
        TextBoxNewPassword.Clear()
        TextBoxConfirmPassword.Clear()

        If Not isLoading Then TextBoxCurrentPassword.Focus()

    End Sub

#End Region

End Class