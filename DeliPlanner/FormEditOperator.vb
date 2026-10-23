Imports Npgsql

Public Class FormEditOperator

#Region "定義"

    Private ReadOnly _menu As FormMenu
    Private _mode As EditMode
    Private Property Mode As EditMode
        Get
            Return _mode
        End Get
        Set(value As EditMode)
            _mode = value
            If _mode = EditMode.Add Then
                LabelMode.Text = "新規追加"
                LabelMode.ForeColor = Color.White
                LabelMode.BackColor = Color.Blue
                ButtonDelete.Enabled = False
                TextBoxPassword.Visible = True
                ButtonResetPassword.Visible = False
            Else
                LabelMode.Text = "修正"
                LabelMode.ForeColor = Color.White
                LabelMode.BackColor = Color.Green
                ButtonDelete.Enabled = True
                TextBoxPassword.Visible = False
                ButtonResetPassword.Visible = True
            End If
        End Set
    End Property
    Private _id As String

#End Region

#Region "コンストラクタ"

    Public Sub New(menu As FormMenu, mode As EditMode, Optional id As String = Nothing)

        InitializeComponent()
        _menu = menu

        _id = id
        Me.Mode = mode

    End Sub

#End Region

#Region "イベント"

    Private Sub Me_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        ' 権限コンボの初期化
        ComboBoxRole.Items.Clear()
        ComboBoxRole.Items.Add("一般")
        ComboBoxRole.Items.Add("管理者")
        ComboBoxRole.DropDownStyle = ComboBoxStyle.DropDownList

        ' フォームの初期化
        InitializeForm(True)

    End Sub

    Private Sub Me_Shown(sender As Object, e As System.EventArgs) Handles Me.Shown

        If Me.Mode = EditMode.Edit Then
            LoadData(_id)
        End If

        TextBoxOperatorCode.Focus()

    End Sub

    Private Sub ButtonRegist_Click(sender As Object, e As EventArgs) Handles ButtonRegist.Click

        ' 更新前チェック
        If CheckData() = False Then
            Exit Sub
        End If

        ' レコード更新
        If MessageBox.Show("登録します。よろしいですか？", Me.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = DialogResult.Yes Then
            Try
                SaveData()
            Catch ex As Exception
                SysMsgBox(ex.Message, Me.Text)
                Exit Sub
            End Try

            ' 自分自身を修正した場合は手元の情報も更新する
            If sharedOperator IsNot Nothing AndAlso sharedOperator.OperatorId = _id Then
                sharedOperator = Operators.GetById(_id)
                _menu.ApplyOperator()
            End If

            If Me.Mode = EditMode.Add Then
                _id = Nothing
                InitializeForm()
            Else
                ButtonClose.PerformClick()
            End If
        End If

    End Sub

    Private Sub ButtonResetPassword_Click(sender As Object, e As EventArgs) Handles ButtonResetPassword.Click

        Dim newPassword As String = InputBox("新しいパスワードを入力してください。", Me.Text, String.Empty)

        If String.IsNullOrEmpty(newPassword) Then Exit Sub

        If newPassword.Length < 4 Then
            SysMsgBox("パスワードは4文字以上で入力してください。", Me.Text)
            Exit Sub
        End If

        Try
            Operators.SavePassword(_id, newPassword)
        Catch ex As Exception
            SysMsgBox(ex.Message, Me.Text)
            Exit Sub
        End Try

        MessageBox.Show("パスワードを初期化しました。", Me.Text,
                        MessageBoxButtons.OK, MessageBoxIcon.Information)

    End Sub

    Private Sub ButtonDelete_Click(sender As Object, e As EventArgs) Handles ButtonDelete.Click

        ' レコード削除
        If MessageBox.Show("削除します。よろしいですか？" & vbCrLf &
                           "在庫履歴に残っている担当者は、削除せず「使用停止」にしてください。",
                           Me.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = DialogResult.Yes Then
            Try
                DeleteData()
            Catch ex As Exception
                SysMsgBox(ex.Message, Me.Text)
                Exit Sub
            End Try
            ButtonClose.PerformClick()
        End If

    End Sub

    Private Sub ButtonClose_Click(sender As Object, e As EventArgs) Handles ButtonClose.Click

        _menu.openChildForm(_menu.currentButton, New FormEditOperators(_menu, _id))

    End Sub

#End Region

#Region "メソッド"

    Private Sub LoadData(id As String)

        Dim target As [Operator] = Operators.GetById(id)

        TextBoxOperatorCode.Text = target.OperatorCode
        TextBoxOperatorName.Text = target.OperatorName
        ComboBoxRole.SelectedIndex = If(target.IsAdministrator, 1, 0)
        TextBoxOrderNo.Text = target.OrderNo
        CheckBoxUnusedFlag.Checked = IIf(CInt(Nz(target.UnusedFlag, 0)) = 1, True, False)

        TextBoxOperatorCode.Focus()

    End Sub

    Private Function CheckData() As Boolean

        Dim result As Boolean = True

        Try
            If String.IsNullOrEmpty(TextBoxOperatorCode.Text.Trim) Then
                TextBoxOperatorCode.Focus()
                Throw New ApplicationException("担当者コードを入力してください。")
            End If

            If Operators.ExistsCode(TextBoxOperatorCode.Text.Trim, _id) Then
                TextBoxOperatorCode.Focus()
                Throw New ApplicationException("この担当者コードは既に使われています。")
            End If

            If String.IsNullOrEmpty(TextBoxOperatorName.Text.Trim) Then
                TextBoxOperatorName.Focus()
                Throw New ApplicationException("担当者名を入力してください。")
            End If

            If Me.Mode = EditMode.Add Then
                If String.IsNullOrEmpty(TextBoxPassword.Text) OrElse TextBoxPassword.Text.Length < 4 Then
                    TextBoxPassword.Focus()
                    Throw New ApplicationException("初期パスワードを4文字以上で入力してください。")
                End If
            End If

            ' 自分自身を「一般」または「使用停止」にはできない
            If sharedOperator IsNot Nothing AndAlso sharedOperator.OperatorId = _id Then
                If ComboBoxRole.SelectedIndex <> 1 Then
                    ComboBoxRole.Focus()
                    Throw New ApplicationException("ログイン中の担当者の権限は変更できません。")
                End If
                If CheckBoxUnusedFlag.Checked Then
                    CheckBoxUnusedFlag.Focus()
                    Throw New ApplicationException("ログイン中の担当者を使用停止にはできません。")
                End If
            End If
        Catch ex As Exception
            SysMsgBox(ex.Message, Me.Text)
            result = False
        End Try

        Return result

    End Function

    Private Sub SaveData()

        Try
            Dim target As New [Operator]
            target.OperatorId = _id
            target.OperatorCode = TextBoxOperatorCode.Text.Trim
            target.OperatorName = TextBoxOperatorName.Text.Trim
            target.RoleId = CInt(If(ComboBoxRole.SelectedIndex = 1, OperatorRole.Administrator, OperatorRole.General)).ToString
            target.OrderNo = TextBoxOrderNo.Text
            target.UnusedFlag = IIf(CheckBoxUnusedFlag.Checked, 1, 0).ToString

            ' 新規登録時のみ初期パスワードを設定する
            If Me.Mode = EditMode.Add Then
                target.PasswordSalt = NewPasswordSalt()
                target.PasswordHash = HashPassword(TextBoxPassword.Text, target.PasswordSalt)
            End If

            Operators.Save(target)
            _id = target.OperatorId
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try

    End Sub

    Private Sub DeleteData()

        Try
            Dim target As New [Operator]
            target.OperatorId = _id
            Operators.Delete(target)
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try

    End Sub

    Private Sub InitializeForm(Optional isLoading As Boolean = False)

        TextBoxOperatorCode.Clear()
        TextBoxOperatorName.Clear()
        ComboBoxRole.SelectedIndex = 0
        TextBoxOrderNo.Clear()
        CheckBoxUnusedFlag.Checked = False
        TextBoxPassword.Clear()

        If Not isLoading Then TextBoxOperatorCode.Focus()

    End Sub

#End Region

End Class