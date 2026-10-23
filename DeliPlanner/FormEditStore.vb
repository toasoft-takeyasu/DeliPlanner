Public Class FormEditStore

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
            Else
                LabelMode.Text = "修正"
                LabelMode.ForeColor = Color.White
                LabelMode.BackColor = Color.Green
                ButtonDelete.Enabled = True
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

        ' フォームの初期化
        InitializeForm(True)

    End Sub

    Private Sub Me_Shown(sender As Object, e As System.EventArgs) Handles Me.Shown

        If Me.Mode = EditMode.Edit Then
            LoadData(_id)
        End If

        TextBoxStoreName.Focus()

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
            If Me.Mode = EditMode.Add Then
                _id = Nothing
                InitializeForm()
            Else
                ButtonClose.PerformClick()
            End If
        End If

    End Sub

    Private Sub ButtonDelete_Click(sender As Object, e As EventArgs) Handles ButtonDelete.Click

        ' レコード削除
        If MessageBox.Show("削除します。よろしいですか？", Me.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = DialogResult.Yes Then
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

        _menu.openChildForm(_menu.currentButton, New FormEditStores(_menu, _id))

    End Sub

#End Region

#Region "メソッド"

    Private Sub LoadData(id As String)

        Dim store As Store = Stores.GetById(id)

        TextBoxStoreCode.Text = store.StoreCode
        TextBoxStoreName.Text = store.StoreName
        TextBoxStoreShortName.Text = store.StoreShortName
        CheckBoxCenterFlag.Checked = If(store.CenterFlag = 1, True, False)
        TextBoxOrderNo.Text = store.OrderNo
        CheckBoxUnusedFlag.Checked = IIf(store.UnusedFlag = 1, True, False)

        TextBoxStoreCode.Focus()

    End Sub

    Private Function CheckData() As Boolean

        Dim result As Boolean = True

        Try
            If String.IsNullOrEmpty(TextBoxStoreName.Text) Then
                TextBoxStoreName.Focus()
                Throw New ApplicationException("店舗名が入力されていません。")
            End If
            If String.IsNullOrEmpty(TextBoxStoreShortName.Text) Then
                TextBoxStoreShortName.Focus()
                Throw New ApplicationException("店舗名（略称）が入力されていません。")
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

    Private Sub SaveData()

        Try
            Dim store As New Store
            store.StoreId = _id
            store.StoreCode = TextBoxStoreCode.Text
            store.StoreName = TextBoxStoreName.Text
            store.StoreShortName = TextBoxStoreShortName.Text
            store.CenterFlag = If(CheckBoxCenterFlag.Checked, 1, 0)
            store.OrderNo = TextBoxOrderNo.Text
            store.UnusedFlag = IIf(CheckBoxUnusedFlag.Checked, 1, 0)
            Stores.Save(store)
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try

    End Sub

    Private Sub DeleteData()

        Try
            Dim store As New Store
            store.StoreId = _id
            Stores.Delete(store)
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try

    End Sub

    Private Sub InitializeForm(Optional isLoading As Boolean = False)

        TextBoxStoreCode.Clear()
        TextBoxStoreName.Clear()
        CheckBoxUnusedFlag.Checked = False
        TextBoxStoreShortName.Clear()
        CheckBoxCenterFlag.Checked = False
        TextBoxOrderNo.Clear()

        If Not isLoading Then TextBoxStoreName.Focus()

    End Sub

#End Region

End Class