Imports Npgsql
Imports NpgsqlTypes

Public Class FormOrder

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

    Private WithEvents dataGridViewOrderDetails As DataGridViewOrderDetails

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

        dataGridViewOrderDetails = New DataGridViewOrderDetails()
        PanelOrders.Controls.Add(dataGridViewOrderDetails)

        ' フォームの初期化
        InitializeForm(True)

    End Sub

    Private Sub Me_Shown(sender As Object, e As System.EventArgs) Handles Me.Shown

        If Me.Mode = EditMode.Edit Then
            LoadData(_id)
        End If

        AddHandler DateBoxOrderDate.TextChanged, AddressOf Key_TextChanged
        AddHandler TextBoxSupplierId.TextChanged, AddressOf Key_TextChanged

        DateBoxOrderDate.Focus()

    End Sub

    Private Sub Key_TextChanged(sender As Object, e As EventArgs)

        If IsDate(DateBoxOrderDate.Text) AndAlso IsNumeric(TextBoxSupplierId.Text) Then
            Dim order As Order = Orders.GetByKey(DateBoxOrderDate.Text, TextBoxSupplierId.Text)
            If IsNothing(order) Then
                _id = Nothing
                Me.Mode = EditMode.Add
            Else
                _id = order.OrderId
                Me.Mode = EditMode.Edit
            End If
            dataGridViewOrderDetails.DataBind(DateBoxOrderDate.Text, TextBoxSupplierId.Text)
        End If

    End Sub

    Private Sub ButtonOrderDate_Click(sender As Object, e As EventArgs) Handles ButtonOrderDateNext.Click, ButtonOrderDatePrevious.Click

        If Not IsDate(DateBoxOrderDate.Text) Then
            DateBoxOrderDate.Text = Now.ToString("yyyy/MM/dd")
        Else
            If sender.Equals(ButtonOrderDateNext) Then
                DateBoxOrderDate.Text = CDate(DateBoxOrderDate.Text).AddDays(1)
            Else
                DateBoxOrderDate.Text = CDate(DateBoxOrderDate.Text).AddDays(-1)
            End If
        End If
        DateBoxOrderDate.Focus()

    End Sub

    Private Sub ButtonSearchSupplier_Click(sender As Object, e As EventArgs) Handles ButtonSearchSupplier.Click

        Dim dialog As New DialogSearchSuppliers
        With dialog
            .ItemTypeFilter = "RecipeNeedFlag = 0"

            Dim targetBounds As Rectangle = PanelMain.RectangleToScreen(PanelMain.ClientRectangle)
            .StartPosition = FormStartPosition.Manual
            .Height = targetBounds.Height
            .Location = New Point(targetBounds.Right - .Width, targetBounds.Bottom - .Height)

            If .ShowDialog = DialogResult.OK Then
                TextBoxSupplierId.Text = .Supplier.SupplierId
                TextBoxSupplierName.Text = .Supplier.SupplierName
                If Not IsNothing(.Supplier.ItemType) Then
                    TextBoxItemTypeId.Text = Nz(.Supplier.ItemType.ItemTypeId, "")
                Else
                    TextBoxItemTypeId.Text = String.Empty
                End If
            End If
        End With

    End Sub

    Private Sub ButtonClearSupplier_Click(sender As Object, e As EventArgs) Handles ButtonClearSupplier.Click

        TextBoxSupplierName.Text = String.Empty
        TextBoxSupplierId.Text = String.Empty

    End Sub

    Private Sub ButtonArrivalPlanDate_Click(sender As Object, e As EventArgs) Handles ButtonArrivalPlanDateNext.Click, ButtonArrivalPlanDatePrevious.Click

        If Not IsDate(DateBoxArrivalPlanDate.Text) Then
            DateBoxArrivalPlanDate.Text = Now.ToString("yyyy/MM/dd")
        Else
            If sender.Equals(ButtonArrivalPlanDateNext) Then
                DateBoxArrivalPlanDate.Text = CDate(DateBoxArrivalPlanDate.Text).AddDays(1)
            Else
                DateBoxArrivalPlanDate.Text = CDate(DateBoxArrivalPlanDate.Text).AddDays(-1)
            End If
        End If
        DateBoxArrivalPlanDate.Focus()

    End Sub

    Public Sub dataGridViewOrderDetails_SearchItemClick(sender As Object, e As DataGridViewCellEventArgs) Handles dataGridViewOrderDetails.SearchItemClick

        Dim dgv As DataGridViewOrderDetails = dataGridViewOrderDetails

        Dim dialog As New DialogSearchItems
        With dialog
            .ItemTypeFilter = "RecipeNeedFlag = 0"
            If Not String.IsNullOrEmpty(TextBoxItemTypeId.Text) Then
                .InitialSelectItemType = ItemTypes.GetById(TextBoxItemTypeId.Text)
            End If

            Dim targetBounds As Rectangle = PanelMain.RectangleToScreen(PanelMain.ClientRectangle)
            .StartPosition = FormStartPosition.Manual
            .Height = targetBounds.Height
            .Location = New Point(targetBounds.Right - .Width, targetBounds.Top)

            If .ShowDialog = DialogResult.OK Then
                dgv.CurrentCell = dgv(dgv.ButtonColumnSearchItem.Name, e.RowIndex)
                dgv(dgv.TextBoxColumnItemId.Name, e.RowIndex).Value = .Item.ItemId
                dgv(dgv.TextBoxColumnItemName.Name, e.RowIndex).Value = .Item.ItemName
                dgv(dgv.TextBoxColumnItemTypeId.Name, e.RowIndex).Value = .Item.ItemTypeId

                Dim itemType As ItemType = ItemTypes.GetById(.Item.ItemTypeId)
                If itemType IsNot Nothing Then
                    dgv(dgv.TextBoxColumnItemTypeName.Name, e.RowIndex).Value = itemType.ItemTypeName
                End If

                ' 数量の単位: 未選択のときだけ品目マスタの単位を初期値にする
                If .Item.UnitType IsNot Nothing AndAlso Not String.IsNullOrEmpty(Nz(.Item.UnitType.UnitTypeId, "")) Then
                    If String.IsNullOrEmpty(Convert.ToString(dgv(dgv.ComboBoxColumnUnitType.Name, e.RowIndex).Value)) Then
                        dgv(dgv.ComboBoxColumnUnitType.Name, e.RowIndex).Value = .Item.UnitType.UnitTypeId
                    End If
                End If

                dgv.NotifyCurrentCellDirty(True)
                dgv.Focus()
                dgv.NextCell()
            End If
        End With

    End Sub

    Public Sub dataGridViewOrderDetails_NetWeight_Validating(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles dataGridViewOrderDetails.NetWeight_Validating

        Dim dgv As DataGridViewOrderDetails = dataGridViewOrderDetails

        ' 空白なら処理を抜ける
        If String.IsNullOrEmpty(e.FormattedValue) Then
            Exit Sub
        End If
        ' 値が変更されていなければ処理を抜ける
        If Not dgv.IsCurrentCellDirty Then Exit Sub
        ' 妥当性チェック
        Try
            ' 数値以外はエラー
            If Not IsNumeric(e.FormattedValue) Then
                Throw New ApplicationException("数値を入力してください。")
            End If
            Dim value As Decimal = CDec(e.FormattedValue)
            ' マイナスはエラー
            If value < 0 Then
                Throw New ApplicationException("マイナスは入力できません。")
            End If
            ' 個数管理の場合は整数のみ
            If Not dgv.CanInputDecimal(e.RowIndex, dgv.TextBoxColumnNetWeight) Then
                If value <> Decimal.Truncate(value) Then
                    Throw New ApplicationException("個数管理の内容量は整数で入力してください。")
                End If
            End If
        Catch ex As Exception
            If ex.GetType Is GetType(ApplicationException) Then
                MessageBox.Show(ex.Message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Else
                SysMsgBox(ex.Message, Me.Text)
            End If
            e.Cancel = True
            Exit Sub
        End Try

    End Sub

    Public Sub dataGridViewOrderDetails_PerCase_Validating(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles dataGridViewOrderDetails.PerCase_Validating

        Dim dgv As DataGridViewOrderDetails = dataGridViewOrderDetails

        ' 空白なら処理を抜ける
        If String.IsNullOrEmpty(e.FormattedValue) Then
            Exit Sub
        End If
        ' 値が変更されていなければ処理を抜ける
        If Not dgv.IsCurrentCellDirty Then Exit Sub
        ' 妥当性チェック
        Try
            ' 数値以外はエラー
            If Not IsNumeric(e.FormattedValue) Then
                Throw New ApplicationException("数値を入力してください。")
            End If
            ' マイナスはエラー
            If CInt(e.FormattedValue) < 0 Then
                Throw New ApplicationException("マイナスは入力できません。")
            End If
        Catch ex As Exception
            If ex.GetType Is GetType(ApplicationException) Then
                MessageBox.Show(ex.Message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Else
                SysMsgBox(ex.Message, Me.Text)
            End If
            e.Cancel = True
            Exit Sub
        End Try

    End Sub

    Public Sub dataGridViewOrderDetails_Quantity_Validating(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles dataGridViewOrderDetails.Quantity_Validating

        Dim dgv As DataGridViewOrderDetails = dataGridViewOrderDetails

        ' 空白なら処理を抜ける
        If String.IsNullOrEmpty(e.FormattedValue) Then
            Exit Sub
        End If
        ' 値が変更されていなければ処理を抜ける
        If Not dgv.IsCurrentCellDirty Then Exit Sub
        ' 妥当性チェック
        Try
            ' 数値以外はエラー
            If Not IsNumeric(e.FormattedValue) Then
                Throw New ApplicationException("数値を入力してください。")
            End If
            Dim value As Decimal = CDec(e.FormattedValue)
            ' マイナスはエラー
            If value < 0 Then
                Throw New ApplicationException("マイナスは入力できません。")
            End If
            ' 個数管理の場合は整数のみ
            If Not dgv.CanInputDecimal(e.RowIndex, dgv.TextBoxColumnQuantity) Then
                If value <> Decimal.Truncate(value) Then
                    Throw New ApplicationException("個数管理の数量は整数で入力してください。")
                End If
            End If
        Catch ex As Exception
            If ex.GetType Is GetType(ApplicationException) Then
                MessageBox.Show(ex.Message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Else
                SysMsgBox(ex.Message, Me.Text)
            End If
            e.Cancel = True
            Exit Sub
        End Try

    End Sub

    Public Sub dataGridViewOrderDetails_UnitPrice_Validating(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles dataGridViewOrderDetails.UnitPrice_Validating

        Dim dgv As DataGridViewOrderDetails = dataGridViewOrderDetails

        ' 空白なら処理を抜ける
        If String.IsNullOrEmpty(e.FormattedValue) Then
            Exit Sub
        End If
        ' 値が変更されていなければ処理を抜ける
        If Not dgv.IsCurrentCellDirty Then Exit Sub
        ' 妥当性チェック
        Try
            ' 数値以外はエラー
            If Not IsNumeric(e.FormattedValue) Then
                Throw New ApplicationException("数値を入力してください。")
            End If
            ' マイナスはエラー
            If CInt(e.FormattedValue) < 0 Then
                Throw New ApplicationException("マイナスは入力できません。")
            End If
        Catch ex As Exception
            If ex.GetType Is GetType(ApplicationException) Then
                MessageBox.Show(ex.Message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Else
                SysMsgBox(ex.Message, Me.Text)
            End If
            e.Cancel = True
            Exit Sub
        End Try

    End Sub

    Public Sub dataGridViewOrderDetails_RowValidating(sender As Object, e As DataGridViewCellCancelEventArgs) Handles dataGridViewOrderDetails.RowValidating

        Dim dgv As DataGridViewOrderDetails = dataGridViewOrderDetails

        If e.RowIndex = dgv.NewRowIndex OrElse Not dgv.IsCurrentRowDirty Then
            Exit Sub
        End If

        Try
            If String.IsNullOrEmpty(Nz(dgv(dgv.TextBoxColumnItemId.Name, e.RowIndex).FormattedValue.ToString.Trim, "")) Then
                dgv.CurrentCell = dgv(dgv.ButtonColumnSearchItem.Name, e.RowIndex)
                Throw New ApplicationException("品目が選択されていません。")
            End If
            If String.IsNullOrEmpty(Nz(dgv(dgv.TextBoxColumnQuantity.Name, e.RowIndex).FormattedValue.ToString.Trim, "")) Then
                dgv.CurrentCell = dgv(dgv.TextBoxColumnQuantity.Name, e.RowIndex)
                Throw New ApplicationException("数量が入力されていません。")
            End If
            If String.IsNullOrEmpty(Nz(dgv(dgv.ComboBoxColumnUnitType.Name, e.RowIndex).FormattedValue.ToString.Trim, "")) Then
                dgv.CurrentCell = dgv(dgv.ComboBoxColumnUnitType.Name, e.RowIndex)
                Throw New ApplicationException("単位が選択されていません。")
            End If
        Catch ex As Exception
            If ex.GetType Is GetType(ApplicationException) Then
                MessageBox.Show(ex.Message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Else
                SysMsgBox(ex.Message, Me.Text)
            End If
            e.Cancel = True
            Exit Sub
        End Try

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
                If ex.GetType Is GetType(ApplicationException) Then
                    MessageBox.Show(ex.Message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Else
                    SysMsgBox(ex.Message, Me.Text)
                End If
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
                If ex.GetType Is GetType(ApplicationException) Then
                    MessageBox.Show(ex.Message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Else
                    SysMsgBox(ex.Message, Me.Text)
                End If
                Exit Sub
            End Try
            ButtonClose.PerformClick()
        End If

    End Sub

    Private Sub ButtonClose_Click(sender As Object, e As EventArgs) Handles ButtonClose.Click

        _menu.openChildForm(_menu.currentButton, New FormOrders(_menu, _id))

    End Sub

#End Region

#Region "メソッド"

    Private Sub LoadData(id As String)

        Dim order As Order = Orders.GetById(id)

        DateBoxOrderDate.Text = CDate(order.OrderDate).ToString(DateBoxOrderDate.Format)
        TextBoxSupplierId.Text = order.Supplier.SupplierId
        TextBoxSupplierName.Text = Suppliers.GetById(order.Supplier.SupplierId).SupplierName
        If Not String.IsNullOrEmpty(order.ArrivalPlanDate) Then
            DateBoxArrivalPlanDate.Text = CDate(order.ArrivalPlanDate).ToString(DateBoxArrivalPlanDate.Format)
        End If

        dataGridViewOrderDetails.DataBind(DateBoxOrderDate.Text, TextBoxSupplierId.Text)

    End Sub

    Private Function CheckData() As Boolean

        Dim result As Boolean = True

        Dim dgv As DataGridViewOrderDetails = dataGridViewOrderDetails

        ' 編集中の内容をセルに確定させる
        If Not dgv.EndEdit() Then Return False

        Try
            If String.IsNullOrEmpty(DateBoxOrderDate.Text) Then
                DateBoxOrderDate.Focus()
                Throw New ApplicationException("発注日が入力されていません。")
            End If
            If String.IsNullOrEmpty(TextBoxSupplierName.Text) Then
                TextBoxSupplierName.Focus()
                Throw New ApplicationException("仕入先が選択されていません。")
            End If
            If String.IsNullOrEmpty(DateBoxArrivalPlanDate.Text) Then
                DateBoxArrivalPlanDate.Focus()
                Throw New ApplicationException("入荷予定日が入力されていません。")
            End If
            If dgv.Rows.Count <= 1 Then
                Throw New ApplicationException("明細が入力されていません。")
            End If

            For Each row As DataGridViewRow In dgv.Rows
                If row.IsNewRow Then Continue For

                Dim itemId As Integer
                ' 品目
                If Not Integer.TryParse(Convert.ToString(row.Cells(dgv.TextBoxColumnItemId.Name).Value), itemId) Then
                    dgv.CurrentCell = row.Cells(dgv.ButtonColumnSearchItem.Name)
                    dgv.Focus()
                    Throw New ApplicationException("品目が選択されていません。")
                End If
                ' 数量
                Dim quantityText As String = Convert.ToString(row.Cells(dgv.TextBoxColumnQuantity.Name).Value)
                dgv.CurrentCell = row.Cells(dgv.TextBoxColumnQuantity.Name)
                If String.IsNullOrEmpty(quantityText) Then
                    Throw New ApplicationException("数量が入力されていません。")
                End If
                If Not IsNumeric(quantityText) Then
                    Throw New ApplicationException("数量は数値で入力してください。")
                End If
                Dim quantity As Decimal = CDec(quantityText)
                If quantity < 0 Then
                    Throw New ApplicationException("数量にマイナスは入力できません。")
                End If
                ' 数量単位は必須
                If String.IsNullOrEmpty(Convert.ToString(row.Cells(dgv.ComboBoxColumnUnitType.Name).Value)) Then
                    dgv.CurrentCell = row.Cells(dgv.ComboBoxColumnUnitType.Name)
                    Throw New ApplicationException("数量の単位が選択されていません。")
                End If
                ' 個数管理の数量は整数のみ
                If Not dgv.CanInputDecimal(row.Index, dgv.TextBoxColumnQuantity) Then
                    If quantity <> Decimal.Truncate(quantity) Then
                        dgv.CurrentCell = row.Cells(dgv.TextBoxColumnQuantity.Name)
                        Throw New ApplicationException("個数管理の数量は整数で入力してください。")
                    End If
                End If
                ' 内容量は入力されている場合だけチェック
                Dim netWeightText As String = Convert.ToString(row.Cells(dgv.TextBoxColumnNetWeight.Name).Value)
                If Not String.IsNullOrEmpty(netWeightText) Then
                    dgv.CurrentCell = row.Cells(dgv.TextBoxColumnNetWeight.Name)
                    If Not IsNumeric(netWeightText) Then
                        Throw New ApplicationException("内容量は数値で入力してください。")
                    End If
                    Dim netWeight As Decimal = CDec(netWeightText)
                    If netWeight < 0 Then
                        Throw New ApplicationException("内容量にマイナスは入力できません。")
                    End If
                    ' 内容量単位が未入力なら省略を許可する
                    ' 選択されていて個数管理なら整数のみ
                    If Not dgv.CanInputDecimal(row.Index, dgv.TextBoxColumnNetWeight) Then
                        If netWeight <> Decimal.Truncate(netWeight) Then
                            Throw New ApplicationException("個数管理の内容量は整数で入力してください。")
                        End If
                    End If
                End If
            Next
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
            Dim order As New Order
            order.OrderId = _id
            order.OrderDate = DateBoxOrderDate.Text
            order.Supplier.SupplierId = TextBoxSupplierId.Text
            order.ArrivalPlanDate = DateBoxArrivalPlanDate.Text

            Dim orderDetailList As New List(Of OrderDetail)
            Dim dgv As DataGridViewOrderDetails = dataGridViewOrderDetails
            For Each row As DataGridViewRow In dgv.Rows
                If row.IsNewRow Then Exit For

                Dim orderDetail As New OrderDetail
                orderDetail.DetailNo = row.Index + 1
                orderDetail.Item = Items.GetById(dgv(dgv.TextBoxColumnItemId.Name, row.Index).Value)
                orderDetail.Quantity = dgv(dgv.TextBoxColumnQuantity.Name, row.Index).Value
                If Not String.IsNullOrEmpty(Nz(dgv(dgv.TextBoxColumnNetWeight.Name, row.Index).Value, "")) Then
                    orderDetail.NetWeight = dgv(dgv.TextBoxColumnNetWeight.Name, row.Index).Value
                End If
                If Not String.IsNullOrEmpty(Nz(dgv(dgv.ComboBoxColumnNetWeightUnitType.Name, row.Index).Value, "")) Then
                    orderDetail.NetWeightUnitType = UnitTypes.GetById(dgv(dgv.ComboBoxColumnNetWeightUnitType.Name, row.Index).Value)
                End If
                If Not String.IsNullOrEmpty(Nz(dgv(dgv.TextBoxColumnPerCase.Name, row.Index).Value, "")) Then
                    orderDetail.PerCase = dgv(dgv.TextBoxColumnPerCase.Name, row.Index).Value
                End If
                orderDetail.UnitType = UnitTypes.GetById(dgv(dgv.ComboBoxColumnUnitType.Name, row.Index).Value)
                If Not String.IsNullOrEmpty(Nz(dgv(dgv.TextBoxColumnUnitPrice.Name, row.Index).Value, "")) Then
                    orderDetail.UnitPrice = dgv(dgv.TextBoxColumnUnitPrice.Name, row.Index).Value
                End If
                If Not String.IsNullOrEmpty(Nz(dgv(dgv.TextBoxColumnPrice.Name, row.Index).Value, "")) Then
                    orderDetail.Price = dgv(dgv.TextBoxColumnPrice.Name, row.Index).Value
                End If
                orderDetailList.Add(orderDetail)
            Next
            order.OrderDetails = orderDetailList

            Orders.Save(order)
        Catch ex As Exception
            Throw
        End Try

    End Sub

    Private Sub DeleteData()

        Try
            Dim order As New Order
            order.OrderId = _id

            Orders.Delete(order)
        Catch ex As Exception
            Throw
        End Try

    End Sub

    Private Sub InitializeForm(Optional isLoading As Boolean = False)

        DateBoxOrderDate.Text = Now.ToString("yyyy/MM/dd")
        TextBoxSupplierId.Text = String.Empty
        TextBoxSupplierName.Text = String.Empty
        TextBoxItemTypeId.Text = String.Empty
        DateBoxArrivalPlanDate.Text = String.Empty
        dataGridViewOrderDetails.Clear()

        If Not isLoading Then DateBoxOrderDate.Focus()

    End Sub

#End Region

End Class

Public Class DataGridViewOrderDetails
    Inherits CustomDataGridView

#Region "定義"

    Public ButtonColumnRemove As New CustomDatagridViewButtonColumn
    Public TextBoxColumnItemTypeName As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnItemName As New CustomDataGridViewTextBoxColumn
    Public ButtonColumnSearchItem As New CustomDatagridViewButtonColumn
    Public TextBoxColumnNetWeight As New CustomDataGridViewTextBoxColumn
    Public ComboBoxColumnNetWeightUnitType As New CustomDataGridViewComboBoxColumn
    Public TextBoxColumnPerCase As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnQuantity As New CustomDataGridViewTextBoxColumn
    Public ComboBoxColumnUnitType As New CustomDataGridViewComboBoxColumn
    Public TextBoxColumnUnitPrice As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnPrice As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnItemId As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnItemTypeId As New CustomDataGridViewTextBoxColumn

    Public Event SearchItemClick(sender As Object, e As DataGridViewCellEventArgs)
    Public Event NetWeight_Validating(sender As Object, e As DataGridViewCellValidatingEventArgs)
    Public Event NetWeightUnitType_SelectedIndexChanged(sender As Object, selectedValue As Object, e As EventArgs)
    Public Event PerCase_Validating(sender As Object, e As DataGridViewCellValidatingEventArgs)
    Public Event Quantity_Validating(sender As Object, e As DataGridViewCellValidatingEventArgs)
    Public Event UnitType_SelectedIndexChanged(sender As Object, selectedValue As Object, e As EventArgs)
    Public Event UnitPrice_Validating(sender As Object, e As DataGridViewCellValidatingEventArgs)

    Public isDataBinding As Boolean
    Private comboBoxEditingControl As New CustomDataGridViewComboBoxEditingControl

    Private storeList As New List(Of Store)
    Private itemTypeBackColors As New Dictionary(Of String, Color)

#End Region

#Region "コンストラクタ"

    Public Sub New()

        With ButtonColumnRemove
            .Name = "ButtonColumnRemove"
            .HeaderText = String.Empty
            .Width = 50
            .DefaultCellStyle.Font = New Font("MS UI Gothic", 11.25)
            .DefaultCellStyle.NullValue = "削除"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .UseColumnTextForButtonValue = True
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .TabStop = False
        End With
        With TextBoxColumnItemTypeName
            .Name = "TextBoxColumnItemTypeName"
            .DataPropertyName = "ItemTypeName"
            .HeaderText = "品目区分"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .DefaultCellStyle.SelectionForeColor = Color.Black
            .DefaultCellStyle.SelectionBackColor = Color.White
            .Width = 80
            .ReadOnly = True
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .TabStop = False
            If sharedIniFile.VisivleItemType = 0 Then
                .Visible = False
            End If
        End With
        With TextBoxColumnItemName
            .Name = "TextBoxColumnItemName"
            .DataPropertyName = "ItemName"
            .HeaderText = String.Empty '見出しは結合ヘッダーで描くので空
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            .DefaultCellStyle.SelectionForeColor = Color.Black
            .DefaultCellStyle.SelectionBackColor = Color.White
            .Width = 200
            .ReadOnly = True
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .TabStop = False
        End With
        With ButtonColumnSearchItem
            .Name = "ButtonColumnSearchItem"
            .HeaderText = String.Empty '見出しは結合ヘッダーで描くので空
            .Width = 50
            .DefaultCellStyle.Font = New Font("MS UI Gothic", 11.25)
            .DefaultCellStyle.NullValue = "検索"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .UseColumnTextForButtonValue = True
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .TabStop = True
        End With
        With TextBoxColumnNetWeight
            .Name = "TextBoxColumnNetWeight"
            .DataPropertyName = "NetWeight"
            .HeaderText = String.Empty '見出しは結合ヘッダーで描くので空
            .Width = 50
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .DefaultCellStyle.Format = "#,###"
            .DefaultCellStyle.SelectionForeColor = Color.Black
            .DefaultCellStyle.SelectionBackColor = Color.White
            .ReadOnly = False
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .NumericOnly = True
            .TabStop = True
        End With
        With ComboBoxColumnNetWeightUnitType
            .Name = "ComboBoxColumnNetWeightUnitType"
            .DataPropertyName = "NetWeightUnitType"
            .HeaderText = String.Empty '見出しは結合ヘッダーで描くので空
            .Width = 80
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .TabStop = True

            UnitTypes.BindComboBox(ComboBoxColumnNetWeightUnitType, True, "UnusedFlag = 0 and NonConvertibleFlag = 0")
        End With
        With TextBoxColumnPerCase
            .Name = "TextBoxColumnPerCase"
            .DataPropertyName = "PerCase"
            .HeaderText = "入数"
            .Width = 50
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .DefaultCellStyle.Format = "#,###"
            .DefaultCellStyle.SelectionForeColor = Color.Black
            .DefaultCellStyle.SelectionBackColor = Color.White
            .ReadOnly = False
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .NumericOnly = True
            .TabStop = True
        End With
        With TextBoxColumnQuantity
            .Name = "TextBoxColumnQuantity"
            .DataPropertyName = "Quantity"
            .HeaderText = String.Empty '見出しは結合ヘッダーで描くので空
            .Width = 80
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .DefaultCellStyle.Format = "#,##0"
            .DefaultCellStyle.SelectionForeColor = Color.Black
            .DefaultCellStyle.SelectionBackColor = Color.White
            .ReadOnly = False
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .NumericOnly = True
            .TabStop = True
        End With
        With ComboBoxColumnUnitType
            .Name = "ComboBoxColumnUnitType"
            .DataPropertyName = "UnitType"
            .HeaderText = String.Empty '見出しは結合ヘッダーで描くので空
            .Width = 80
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .TabStop = True

            UnitTypes.BindComboBox(ComboBoxColumnUnitType, True, "UnusedFlag = 0")
        End With
        With TextBoxColumnUnitPrice
            .Name = "TextBoxColumnUnitPrice"
            .DataPropertyName = "UnitPrice"
            .HeaderText = "単価"
            .Width = 80
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .DefaultCellStyle.Format = "#,###"
            .DefaultCellStyle.SelectionForeColor = Color.Black
            .DefaultCellStyle.SelectionBackColor = Color.White
            .ReadOnly = False
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .NumericOnly = True
            .TabStop = True
        End With
        With TextBoxColumnPrice
            .Name = "TextBoxColumnPrice"
            .DataPropertyName = "Price"
            .HeaderText = "金額"
            .Width = 100
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .DefaultCellStyle.Format = "#,###"
            .DefaultCellStyle.SelectionForeColor = Color.Black
            .DefaultCellStyle.SelectionBackColor = Color.White
            .ReadOnly = True
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .TabStop = False
        End With

        With TextBoxColumnItemId
            .Name = "TextBoxColumnItemId"
            .DataPropertyName = "ItemId"
            .HeaderText = "品目ID"
            .Visible = False
        End With
        With TextBoxColumnItemTypeId
            .Name = "TextBoxColumnItemTypeId"
            .DataPropertyName = "ItemTypeId"
            .HeaderText = "品目区分ID"
            .Visible = False
        End With

        With Me
            ' デザイン関係のプロパティを変更
            .EnableHeadersVisualStyles = False
            .AutoSize = False
            .Dock = DockStyle.Fill
            .BackgroundColor = SystemColors.ActiveCaption
            .GridColor = SystemColors.ControlDarkDark
            .DefaultCellStyle.Font = New Font("ＭＳ ゴシック", 12)
            .ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
            .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single
            .ColumnHeadersDefaultCellStyle.Font = New Font("MS UI Gothic", 12)
            .ColumnHeadersHeight = 22          ' 1段(結合ヘッダーはこの高さいっぱいに描く)
            .RowHeadersVisible = False
            .RowHeadersWidth = 50
            .RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing
            .RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single
            .RowHeadersDefaultCellStyle.Font = New Font("MS UI Gothic", 12)
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .AllowUserToResizeRows = False
            .RowTemplate.Height = 22
            .NumberOnRowHeader = False
            .AutoGenerateColumns = False
            .ScrollBars = Windows.Forms.ScrollBars.Both

            .Columns.Add(ButtonColumnRemove)
            .Columns.Add(TextBoxColumnItemTypeName)
            .Columns.Add(TextBoxColumnItemName)
            .Columns.Add(ButtonColumnSearchItem)
            .Columns.Add(TextBoxColumnNetWeight)
            .Columns.Add(ComboBoxColumnNetWeightUnitType)
            .Columns.Add(TextBoxColumnPerCase)
            .Columns.Add(TextBoxColumnQuantity)
            .Columns.Add(ComboBoxColumnUnitType)
            .Columns.Add(TextBoxColumnUnitPrice)
            .Columns.Add(TextBoxColumnPrice)
            .Columns.Add(TextBoxColumnItemId)
            .Columns.Add(TextBoxColumnItemTypeId)

            ' 動作関係のプロパティを変更
            .ReadOnly = False
            .AllowUserToOrderColumns = False
            .AllowUserToAddRows = True
            .AllowUserToDeleteRows = True
            .AllowUserToResizeColumns = False
            .AllowUserToResizeRows = False
            .EnterToTab = True
            .EditMode = DataGridViewEditMode.EditOnEnter
            .SelectionMode = DataGridViewSelectionMode.CellSelect
            .MultiSelect = False
            .MouseRightButtonOnSelect = False
            .DrawCellPointer = False
            .DoubleBuffered = True
            .ResizeRedraw = True 'リサイズ時に全体を再描画(結合ヘッダーの崩れ防止)
        End With

        BuildMergedHeaders()

        If sharedIniFile.PaintItemTypeColor = 1 Then
            ' 行の背景色を辞書化
            For Each itemType As ItemType In ItemTypes.GetAll()
                itemTypeBackColors(itemType.ItemTypeName) = itemType.FormBackColor
            Next
        End If

    End Sub

#End Region

#Region "イベント"

    Private Sub Me_SelectionChanged(sender As Object, e As EventArgs) Handles Me.SelectionChanged

        Me.ClearSelection()

    End Sub

    Private Sub Me_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles Me.CellContentClick

        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Return

        Select Case Me.Columns(e.ColumnIndex).Name
            Case ButtonColumnSearchItem.Name
                RaiseEvent SearchItemClick(sender, e)
            Case ButtonColumnRemove.Name
                If Not Me.Rows(e.RowIndex).IsNewRow Then
                    Me.Rows.RemoveAt(e.RowIndex)
                End If
        End Select

    End Sub

    Private Sub Me_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles Me.CellEndEdit

        If IsNothing(Me.DataSource) Then
            Dim customDataGridViewTextBoxColumn As CustomDataGridViewTextBoxColumn = TryCast(Me.Columns(e.ColumnIndex), CustomDataGridViewTextBoxColumn)
            If Not IsNothing(customDataGridViewTextBoxColumn) Then
                ' CustomDataGridViewTextBoxColumnの場合
                If customDataGridViewTextBoxColumn.NumericOnly Then
                    If CanInputDecimal(e.RowIndex, customDataGridViewTextBoxColumn) Then
                        ' 重量・容量の内容量／数量：小数を保持する
                        If IsNumeric(Me(customDataGridViewTextBoxColumn.Name, e.RowIndex).Value) Then
                            Me(customDataGridViewTextBoxColumn.Name, e.RowIndex).Value = CDec(Me(customDataGridViewTextBoxColumn.Name, e.RowIndex).Value)
                        End If
                    Else
                        ' 個数・入数・単価：従来の整数処理
                        Dim value As Integer
                        If Integer.TryParse(Me(customDataGridViewTextBoxColumn.Name, e.RowIndex).FormattedValue.ToString(), value) Then
                            Me(customDataGridViewTextBoxColumn.Name, e.RowIndex).Value = value.ToString(customDataGridViewTextBoxColumn.DefaultCellStyle.Format)
                        End If
                    End If
                End If
            End If

            Dim customDataGridViewDateBoxColumn As CustomDataGridViewDateBoxColumn = TryCast(Me.Columns(e.ColumnIndex), CustomDataGridViewDateBoxColumn)
            If Not IsNothing(customDataGridViewDateBoxColumn) Then
                ' CustomDataGridViewDateBoxColumnの場合
                Dim value As Date
                If Date.TryParse(Me(customDataGridViewDateBoxColumn.Name, e.RowIndex).FormattedValue.ToString, value) Then
                    Me(customDataGridViewDateBoxColumn.Name, e.RowIndex).Value = value.ToString(customDataGridViewDateBoxColumn.DefaultCellStyle.Format)
                End If
            End If

        End If

        If comboBoxEditingControl IsNot Nothing Then
            RemoveHandler comboBoxEditingControl.SelectedIndexChanged, AddressOf dataGridViewComboBox_SelectedIndexChanged
            comboBoxEditingControl = Nothing
        End If

    End Sub

    Private Sub Me_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles Me.CellFormatting

        If e.RowIndex < 0 Then Return

        If sharedIniFile.PaintItemTypeColor = 1 Then
            Dim itemTypeName As String = Convert.ToString(Me("TextBoxColumnItemTypeName", e.RowIndex).Value)
            Dim backColor As Color
            If TryGetItemTypeBackColor(itemTypeBackColors, itemTypeName, backColor) Then
                e.CellStyle.BackColor = backColor
                If Me(e.ColumnIndex, e.RowIndex).ReadOnly Then
                    e.CellStyle.SelectionBackColor = backColor
                End If
            End If
        End If

    End Sub

    Private Sub Me_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles Me.CellValidating

        Select Case Me.Columns(e.ColumnIndex).Name
            Case TextBoxColumnNetWeight.Name
                RaiseEvent NetWeight_Validating(Me, e)
            Case TextBoxColumnPerCase.Name
                RaiseEvent PerCase_Validating(Me, e)
            Case TextBoxColumnQuantity.Name
                RaiseEvent Quantity_Validating(Me, e)
            Case TextBoxColumnUnitPrice.Name
                RaiseEvent UnitPrice_Validating(Me, e)
        End Select

    End Sub

    Private Sub Me_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles Me.CellValueChanged

        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Return
        If isDataBinding Then Return

        Dim col As DataGridViewColumn = Me.Columns(e.ColumnIndex)

        ' 単位を変更したら、内容量・数量の書式を更新する
        If col Is ComboBoxColumnNetWeightUnitType OrElse col Is ComboBoxColumnUnitType Then
            ApplyQuantityFormat(e.RowIndex)
        End If

        ' 金額 = 数量 × 単価
        If col Is TextBoxColumnQuantity OrElse col Is TextBoxColumnUnitPrice Then
            If e.RowIndex < 0 OrElse e.RowIndex >= Me.Rows.Count Then Return
            Dim row As DataGridViewRow = Me.Rows(e.RowIndex)

            Dim quantity As Double = Nz(row.Cells(TextBoxColumnQuantity.Name).Value, 0)
            Dim unitPrice As Double = Nz(row.Cells(TextBoxColumnUnitPrice.Name).Value, 0)

            If quantity = 0 OrElse unitPrice = 0 Then
                row.Cells(TextBoxColumnPrice.Name).Value = Nothing
            Else
                Dim price As Double = quantity * unitPrice
                row.Cells(TextBoxColumnPrice.Name).Value = price.ToString(TextBoxColumnPrice.DefaultCellStyle.Format)
            End If
        End If

    End Sub

    Private Sub Me_EditingControlShowing(sender As Object, e As DataGridViewEditingControlShowingEventArgs) Handles Me.EditingControlShowing

        Select Case CurrentCell.OwningColumn.Name
            Case TextBoxColumnNetWeight.Name,
                 TextBoxColumnQuantity.Name
                Dim column As CustomDataGridViewTextBoxColumn = DirectCast(CurrentCell.OwningColumn, CustomDataGridViewTextBoxColumn)
                Dim allowDecimal As Boolean = CanInputDecimal(CurrentCell.RowIndex, column)
                column.AllowSymbolChar = allowDecimal

                Dim control As CustomDataGridViewTextBoxEditingControl = TryCast(e.Control, CustomDataGridViewTextBoxEditingControl)
                If control IsNot Nothing Then
                    control.NumericOnly = True
                    control.AllowSymbolChar = allowDecimal
                    control.Format = String.Empty
                End If

                If IsNumeric(e.Control.Text) Then
                    If allowDecimal Then
                        ' 小数処理
                        e.Control.Text = CDec(e.Control.Text).ToString()
                    Else
                        ' 整数処理
                        e.Control.Text = CInt(e.Control.Text).ToString(column.DefaultCellStyle.Format.Replace(",", ""))
                    End If
                End If
            Case TextBoxColumnPerCase.Name
                If IsNumeric(e.Control.Text) Then
                    e.Control.Text = CInt(e.Control.Text).ToString(Me.TextBoxColumnPerCase.DefaultCellStyle.Format.Replace(",", ""))
                End If
            Case TextBoxColumnUnitPrice.Name
                If IsNumeric(e.Control.Text) Then
                    e.Control.Text = CInt(e.Control.Text).ToString(Me.TextBoxColumnUnitPrice.DefaultCellStyle.Format.Replace(",", ""))
                End If
            Case ComboBoxColumnNetWeightUnitType.Name,
                 ComboBoxColumnUnitType.Name
                Dim ctl As CustomDataGridViewComboBoxEditingControl =
                TryCast(e.Control, CustomDataGridViewComboBoxEditingControl)
                If ctl Is Nothing Then Return

                comboBoxEditingControl = ctl
                ctl.Name = Me.CurrentCell.OwningColumn.Name

                ' 同じコントロールが使い回されるのでハンドラ重複を防ぐ
                RemoveHandler ctl.SelectedIndexChanged, AddressOf dataGridViewComboBox_SelectedIndexChanged
                AddHandler ctl.SelectedIndexChanged, AddressOf dataGridViewComboBox_SelectedIndexChanged
        End Select

    End Sub

    Private Sub dataGridViewComboBox_SelectedIndexChanged(sender As Object, e As EventArgs)

        Dim ctl As CustomDataGridViewComboBoxEditingControl = TryCast(sender, CustomDataGridViewComboBoxEditingControl)
        If ctl Is Nothing OrElse ctl.EditingControlDataGridView Is Nothing Then Return

        ctl.EditingControlDataGridView.NotifyCurrentCellDirty(True)

        Select Case ctl.Name
            Case ComboBoxColumnNetWeightUnitType.Name
                RaiseEvent NetWeightUnitType_SelectedIndexChanged(Me, ctl.SelectedValue, e)
            Case ComboBoxColumnUnitType.Name
                RaiseEvent UnitType_SelectedIndexChanged(Me, ctl.SelectedValue, e)
        End Select

        ctl.EditingControlDataGridView.EndEdit()

    End Sub

#End Region

#Region "メソッド"

    Public Sub DataBind(orderDate As Date, supplierId As String)

        RemoveHandler Me.CellValidating, AddressOf Me_CellValidating

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Try
                isDataBinding = True

                Me.Rows.Clear()

                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String = String.Empty
                    SQLString = "select"
                    SQLString &= " OrderDetails.ItemId"
                    SQLString &= ",Items.ItemName"
                    SQLString &= ",Items.ItemTypeId"
                    SQLString &= ",ItemTypes.ItemTypeName"
                    SQLString &= ",OrderDetails.NetWeight"
                    SQLString &= ",OrderDetails.NetWeightUnitTypeID"
                    SQLString &= ",OrderDetails.PerCase"
                    SQLString &= ",OrderDetails.Quantity"
                    SQLString &= ",OrderDetails.UnitTypeId"
                    SQLString &= ",OrderDetails.UnitPrice"
                    SQLString &= ",OrderDetails.Price"
                    SQLString &= ",UnitTypes.UnitTypeName"
                    SQLString &= " from (((Orders"
                    SQLString &= " left join OrderDetails on Orders.OrderId = OrderDetails.OrderId)"
                    SQLString &= " left join Items on OrderDetails.ItemId = Items.ItemId)"
                    SQLString &= " left join UnitTypes on OrderDetails.UnitTypeId = UnitTypes.UnitTypeId)"
                    SQLString &= " left join ItemTypes on Items.ItemTypeId = ItemTypes.ItemTypeId"
                    SQLString &= " where OrderDate = @OrderDate"
                    SQLString &= " and SupplierId = @SupplierId"
                    SQLString &= " order by"
                    SQLString &= " OrderDetails.DetailNo"
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    cmd.Parameters.AddWithValue("@OrderDate", NpgsqlDbType.Date, orderDate.Date)
                    cmd.Parameters.AddWithValue("@SupplierId", NpgsqlDbType.integer, CInt(supplierId))
                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        If dr.HasRows Then
                            While dr.Read()
                                Dim column As New List(Of Object)
                                column.Add(Nothing)
                                column.Add(dr("ItemTypeName"))
                                column.Add(dr("ItemName"))
                                column.Add(Nothing)
                                column.Add(Nz(dr("NetWeight"), 0))
                                column.Add(dr("NetWeightUnitTypeID"))
                                column.Add(Nz(dr("PerCase"), 0))
                                column.Add(Nz(dr("Quantity"), 0))
                                column.Add(dr("UnitTypeId"))
                                column.Add(Nz(dr("UnitPrice"), 0))
                                column.Add(Nz(dr("Price"), 0))
                                column.Add(dr("ItemId"))
                                column.Add(dr("ItemTypeId"))
                                Dim rowIndex As Integer = Me.Rows.Add(column.ToArray())
                                ApplyQuantityFormat(rowIndex)
                            End While
                        End If
                    End Using
                End Using
            Catch ex As Exception
                Throw New ApplicationException(ex.Message)
            Finally
                If cn IsNot Nothing Then
                    cn.Close()
                End If

                isDataBinding = False
            End Try
        End Using

        AddHandler Me.CellValidating, AddressOf Me_CellValidating

    End Sub

    Public Sub Clear()

        Me.Rows.Clear()

    End Sub

    Private Sub BuildMergedHeaders()

        ClearMergedHeaders()
        AddMergedHeader(TextBoxColumnItemName, ButtonColumnSearchItem, "品目名")
        AddMergedHeader(TextBoxColumnNetWeight, ComboBoxColumnNetWeightUnitType, "内容量")
        AddMergedHeader(TextBoxColumnQuantity, ComboBoxColumnUnitType, "数量")

    End Sub

    ' 内容量・数量の単位に応じて、小数入力を許可する
    Public Function CanInputDecimal(rowIndex As Integer, column As DataGridViewColumn) As Boolean

        Dim unitColumn As DataGridViewColumn
        If column Is TextBoxColumnNetWeight Then
            unitColumn = ComboBoxColumnNetWeightUnitType
        ElseIf column Is TextBoxColumnQuantity Then
            unitColumn = ComboBoxColumnUnitType
        Else
            Return False
        End If

        Dim value As Object = Me(unitColumn.Name, rowIndex).Value
        ' 単位が未入力なら小数入力を許可する
        If String.IsNullOrEmpty(Convert.ToString(value)) Then
            Return True
        End If
        If Not IsNumeric(value) Then Return False

        Dim unitTypeId As Integer = CInt(value)
        Dim category As Integer = calc.GetUnitCategory(unitTypeId)
        Return calc.IsConvertibleUnit(unitTypeId) AndAlso (category = 1 OrElse category = 2)

    End Function

    ' 内容量・数量の表示書式
    Private Sub ApplyQuantityFormat(rowIndex As Integer)

        Dim row As DataGridViewRow = Me.Rows(rowIndex)
        row.Cells(TextBoxColumnNetWeight.Name).Style.Format = If(CanInputDecimal(rowIndex, TextBoxColumnNetWeight), "#,###.##", "#,###")
        row.Cells(TextBoxColumnQuantity.Name).Style.Format = If(CanInputDecimal(rowIndex, TextBoxColumnQuantity), "#,###.##", "#,###")

    End Sub

#End Region

End Class
