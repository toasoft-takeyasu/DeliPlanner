Public Class FormEditSupplier

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

        ItemTypes.BindComboBox(ComboBoxItemType, True)

        ' フォームの初期化
        InitializeForm(True)

    End Sub

    Private Sub Me_Shown(sender As Object, e As System.EventArgs) Handles Me.Shown

        If Me.Mode = EditMode.Edit Then
            LoadData(_id)
        End If

        TextBoxSupplierCode.Focus()

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

        _menu.openChildForm(_menu.currentButton, New FormEditSuppliers(_menu, _id))

    End Sub

#End Region

#Region "メソッド"

    Private Sub LoadData(id As String)

        Dim supplier As Supplier = Suppliers.GetById(id)

        TextBoxSupplierCode.Text = supplier.SupplierCode
        TextBoxSupplierName.Text = supplier.SupplierName
        TextBoxPostNo.Text = supplier.PostNo
        TextBoxAddress1.Text = supplier.Address1
        TextBoxAddress2.Text = supplier.Address2
        TextBoxAddress3.Text = supplier.Address3
        TextBoxTelNo.Text = supplier.TelNo
        TextBoxFaxNo.Text = supplier.FaxNo
        CheckBoxUnusedFlag.Checked = IIf(supplier.UnusedFlag = 1, True, False)
        If Not IsNothing(supplier.ItemType.ItemTypeId) Then
            ComboBoxItemType.SelectedValue = supplier.ItemType.ItemTypeId
        End If

    End Sub

    Private Function CheckData() As Boolean

        Dim result As Boolean = True

        Try
            If String.IsNullOrEmpty(TextBoxSupplierName.Text) Then
                TextBoxSupplierName.Focus()
                Throw New ApplicationException("仕入先名が入力されていません。")
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
            Dim Supplier As New Supplier
            Supplier.SupplierId = _id
            Supplier.SupplierCode = TextBoxSupplierCode.Text
            Supplier.SupplierName = TextBoxSupplierName.Text
            Supplier.PostNo = TextBoxPostNo.Text
            Supplier.Address1 = TextBoxAddress1.Text
            Supplier.Address2 = TextBoxAddress2.Text
            Supplier.Address3 = TextBoxAddress3.Text
            Supplier.TelNo = TextBoxTelNo.Text
            Supplier.FaxNo = TextBoxFaxNo.Text
            If Not String.IsNullOrEmpty(ComboBoxItemType.Text) Then
                Supplier.ItemType.ItemTypeId = ComboBoxItemType.SelectedValue
            End If
            Supplier.UnusedFlag = IIf(CheckBoxUnusedFlag.Checked, 1, 0)
            Suppliers.Save(Supplier)
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try

    End Sub

    Private Sub DeleteData()

        Try
            Dim Supplier As New Supplier
            Supplier.SupplierId = _id
            Suppliers.Delete(Supplier)
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try

    End Sub

    Private Sub InitializeForm(Optional isLoading As Boolean = False)

        TextBoxSupplierCode.Clear()
        TextBoxSupplierName.Clear()
        CheckBoxUnusedFlag.Checked = False
        TextBoxPostNo.Clear()
        TextBoxAddress1.Clear()
        TextBoxAddress2.Clear()
        TextBoxAddress3.Clear()
        TextBoxTelNo.Clear()
        TextBoxFaxNo.Clear()
        ComboBoxItemType.Text = String.Empty

        If Not isLoading Then TextBoxSupplierCode.Focus()

    End Sub

#End Region

End Class