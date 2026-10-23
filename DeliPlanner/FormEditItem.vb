Imports Npgsql
Imports NpgsqlTypes

Public Class FormEditItem

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

    Private WithEvents dataGridViewEditRecipeDetails As DataGridViewEditRecipeDetails

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

    Private Sub Me_Load(sender As Object, e As EventArgs) Handles Me.Load

        ItemTypes.BindComboBox(ComboBoxItemType, False)
        MeasureTypes.BindComboBox(ComboBoxMeasureType, False)
        UnitTypes.BindComboBox(ComboBoxUnitType, True, "UnusedFlag = 0")
        UnitTypes.BindComboBox(ComboBoxPieceUnitType, True, "UnusedFlag = 0 and NonConvertibleFlag = 1")

        dataGridViewEditRecipeDetails = New DataGridViewEditRecipeDetails()
        PanelRecipeDetail.Controls.Add(dataGridViewEditRecipeDetails)

        ' フォームの初期化
        InitializeForm(True)

    End Sub

    Private Sub Me_Shown(sender As Object, e As System.EventArgs) Handles Me.Shown

        AddHandler ComboBoxItemType.SelectedIndexChanged, AddressOf ComboBoxItemType_SelectedIndexChanged

        If Me.Mode = EditMode.Edit Then
            LoadData(_id)
        End If

        ComboBoxItemType.Focus()

    End Sub

    Private Sub ComboBoxItemType_SelectedIndexChanged(sender As Object, e As EventArgs)

        Dim itemType As ItemType = ItemTypes.GetById(ComboBoxItemType.SelectedValue)

        PanelProduct.Visible = (itemType.ProductFlag = 1)
        PanelLossRate.Visible = (itemType.LossRateNeedFlag = 1)
        PanelPiece.Visible = (itemType.PieceNeedFlag = 1)
        PanelSellingPrice.Visible = (itemType.SellingPriceNeedFlag = 1)
        PanelMainSupplier.Visible = (itemType.MainSupplierNeedFlag = 1)
        PanelRecipeDetail.Visible = (itemType.RecipeNeedFlag = 1)
        PanelComment.Visible = PanelRecipeDetail.Visible
        Select Case itemType.ExpiryDateNeedFlag
            Case 0
                RadioButtonExpiryDateNeedFlag_0.Checked = True
            Case 1
                RadioButtonExpiryDateNeedFlag_1.Checked = True
        End Select

    End Sub

    Private Sub TextBoxShipment_Validating(sender As Object, e As CancelEventArgs) Handles TextBoxShipment.Validating

        With DirectCast(sender, ToaTextBox)
            If .ReadOnly = True Then Exit Sub
            ' 空白なら処理を抜ける
            If String.IsNullOrEmpty(.Text) Then
                Exit Sub
            End If
            ' 値が変更されていなければ処理を抜ける
            If .Text = .OldText Then Exit Sub
            ' 妥当性チェック
            Try
                ' 数値以外はエラー
                If .ErrorStatus = ToaTextBox.ErrorStatusType.NotNumeric Then
                    Throw New ApplicationException("数値を入力してください。")
                End If
                ' 範囲外はエラー
                If .ErrorStatus = ToaTextBox.ErrorStatusType.MinValueOver OrElse
                   .ErrorStatus = ToaTextBox.ErrorStatusType.MaxValueOver Then
                    Throw New ApplicationException(.MinValue & "～" & .MaxValue & "の範囲で入力してください。")
                End If
            Catch ex As Exception
                If ex.GetType Is GetType(ApplicationException) Then
                    MessageBox.Show(ex.Message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Else
                    SysMsgBox(ex.Message, Me.Text)
                End If
                e.Cancel = True
                .Focus()
                .OldText = String.Empty
                Exit Sub
            End Try
        End With

    End Sub

    Private Sub TextBoxPieceWeight_Validating(sender As Object, e As CancelEventArgs) Handles TextBoxPieceWeight.Validating

        With DirectCast(sender, ToaTextBox)
            If .ReadOnly = True Then Exit Sub
            ' 空白なら処理を抜ける
            If String.IsNullOrEmpty(.Text) Then
                Exit Sub
            End If
            ' 値が変更されていなければ処理を抜ける
            If .Text = .OldText Then Exit Sub
            ' 妥当性チェック
            Try
                ' 数値以外はエラー
                If .ErrorStatus = ToaTextBox.ErrorStatusType.NotNumeric Then
                    Throw New ApplicationException("数値を入力してください。")
                End If
                ' 範囲外はエラー
                If .ErrorStatus = ToaTextBox.ErrorStatusType.MinValueOver OrElse
                   .ErrorStatus = ToaTextBox.ErrorStatusType.MaxValueOver Then
                    Throw New ApplicationException(.MinValue & "～" & .MaxValue & "の範囲で入力してください。")
                End If
            Catch ex As Exception
                If ex.GetType Is GetType(ApplicationException) Then
                    MessageBox.Show(ex.Message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Else
                    SysMsgBox(ex.Message, Me.Text)
                End If
                e.Cancel = True
                .Focus()
                .OldText = String.Empty
                Exit Sub
            End Try
        End With

    End Sub

    Private Sub TextBoxLossRate_Validating(sender As Object, e As CancelEventArgs) Handles TextBoxLossRate.Validating

        With DirectCast(sender, ToaTextBox)
            If .ReadOnly = True Then Exit Sub
            ' 空白なら処理を抜ける
            If String.IsNullOrEmpty(.Text) Then
                Exit Sub
            End If
            ' 値が変更されていなければ処理を抜ける
            If .Text = .OldText Then Exit Sub
            ' 妥当性チェック
            Try
                ' 数値以外はエラー
                If .ErrorStatus = ToaTextBox.ErrorStatusType.NotNumeric Then
                    Throw New ApplicationException("数値を入力してください。")
                End If
                ' 範囲外はエラー
                If .ErrorStatus = ToaTextBox.ErrorStatusType.MinValueOver OrElse
                   .ErrorStatus = ToaTextBox.ErrorStatusType.MaxValueOver Then
                    Throw New ApplicationException(.MinValue & "～" & .MaxValue & "の範囲で入力してください。")
                End If
            Catch ex As Exception
                If ex.GetType Is GetType(ApplicationException) Then
                    MessageBox.Show(ex.Message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Else
                    SysMsgBox(ex.Message, Me.Text)
                End If
                e.Cancel = True
                .Focus()
                .OldText = String.Empty
                Exit Sub
            End Try
        End With

    End Sub

    Private Sub TextBoxPrice_Validating(sender As Object, e As CancelEventArgs) Handles _
        TextBoxCostPrice.Validating, TextBoxOriginalSellingPrice.Validating, TextBoxInstoreSellingPrice.Validating

        With DirectCast(sender, ToaTextBox)
            If .ReadOnly = True Then Exit Sub
            ' 空白なら処理を抜ける
            If String.IsNullOrEmpty(.Text) Then
                Exit Sub
            End If
            ' 値が変更されていなければ処理を抜ける
            If .Text = .OldText Then Exit Sub
            ' 妥当性チェック
            Try
                ' 数値以外はエラー
                If .ErrorStatus = ToaTextBox.ErrorStatusType.NotNumeric Then
                    Throw New ApplicationException("数値を入力してください。")
                End If
                ' 範囲外はエラー
                If .ErrorStatus = ToaTextBox.ErrorStatusType.MinValueOver OrElse
                   .ErrorStatus = ToaTextBox.ErrorStatusType.MaxValueOver Then
                    Throw New ApplicationException(.MinValue & "～" & .MaxValue & "の範囲で入力してください。")
                End If
            Catch ex As Exception
                If ex.GetType Is GetType(ApplicationException) Then
                    MessageBox.Show(ex.Message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Else
                    SysMsgBox(ex.Message, Me.Text)
                End If
                e.Cancel = True
                .Focus()
                .OldText = String.Empty
                Exit Sub
            End Try
        End With

    End Sub

    Private Sub ButtonSearchSupplier_Click(sender As Object, e As EventArgs) Handles ButtonSearchSupplier.Click

        Dim dialog As New DialogSearchSuppliers
        With dialog
            .ItemTypeFilter = "RecipeNeedFlag = 0"
            If Not String.IsNullOrEmpty(ComboBoxItemType.Text) Then
                .InitialSelectItemType = ItemTypes.GetById(ComboBoxItemType.SelectedValue)
            End If

            Dim targetBounds As Rectangle = PanelMain.RectangleToScreen(PanelMain.ClientRectangle)
            .StartPosition = FormStartPosition.Manual
            .Height = targetBounds.Height
            .Location = New Point(targetBounds.Right - .Width, targetBounds.Bottom - .Height)

            If .ShowDialog = DialogResult.OK Then
                TextBoxMainSupplierId.Text = .Supplier.SupplierId
                TextBoxMainSupplierName.Text = .Supplier.SupplierName
            End If
        End With

    End Sub

    Private Sub ButtonClearSupplier_Click(sender As Object, e As EventArgs) Handles ButtonClearSupplier.Click

        TextBoxMainSupplierName.Text = String.Empty
        TextBoxMainSupplierId.Text = String.Empty

    End Sub

    Public Sub dataGridViewEditRecipeDetails_SearchItemClick(sender As Object, e As DataGridViewCellEventArgs) Handles dataGridViewEditRecipeDetails.SearchItemClick

        Dim dgv As DataGridViewEditRecipeDetails = dataGridViewEditRecipeDetails

        Dim dialog As New DialogSearchItems
        With dialog
            .ItemTypeFilter = "SellingPriceNeedFlag = 0"

            Dim targetBounds As Rectangle = PanelMain.RectangleToScreen(PanelMain.ClientRectangle)
            .StartPosition = FormStartPosition.Manual
            .Height = targetBounds.Height
            .Location = New Point(targetBounds.Right - .Width, targetBounds.Bottom - .Height)

            If .ShowDialog = DialogResult.OK Then
                dgv.CurrentCell = dgv(dgv.ButtonColumnSearchItem.Name, e.RowIndex)
                dgv(dgv.TextBoxColumnItemId.Name, e.RowIndex).Value = .Item.ItemId
                dgv(dgv.TextBoxColumnItemName.Name, e.RowIndex).Value = .Item.ItemName
                dgv(dgv.TextBoxColumnItemTypeName.Name, e.RowIndex).Value = ItemTypes.GetById(.Item.ItemTypeId).ItemTypeName
                dgv(dgv.ComboBoxColumnUnitType.Name, e.RowIndex).Value = .Item.UnitType.UnitTypeId
                dgv.NotifyCurrentCellDirty(True)
                dgv.Focus()
                dgv.NextCell()
            End If
        End With

    End Sub

    Public Sub dataGridViewEditRecipeDetails_Quantity_Validating(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles dataGridViewEditRecipeDetails.Quantity_Validating

        Dim dgv As DataGridViewEditRecipeDetails = dataGridViewEditRecipeDetails

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

    Public Sub dataGridViewEditRecipeDetails_RowValidating(sender As Object, e As DataGridViewCellCancelEventArgs) Handles dataGridViewEditRecipeDetails.RowValidating

        Dim dgv As DataGridViewEditRecipeDetails = dataGridViewEditRecipeDetails

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
            If dgv(dgv.TextBoxColumnItemId.Name, e.RowIndex).Value.ToString = _id Then
                dgv.CurrentCell = dgv(dgv.ButtonColumnSearchItem.Name, e.RowIndex)
                Throw New ApplicationException("自分自身はレシピに登録できません。")
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

        _menu.openChildForm(_menu.currentButton, New FormEditItems(_menu, _id))

    End Sub

#End Region

#Region "メソッド"

    Private Sub LoadData(id As String)

        Dim item As Item = Items.GetById(id)

        ComboBoxItemType.SelectedValue = item.ItemTypeId
        ComboBoxItemType_SelectedIndexChanged(Nothing, Nothing)
        Select Case item.ExpiryDateNeedFlag
            Case 0
                RadioButtonExpiryDateNeedFlag_0.Checked = True
            Case 1
                RadioButtonExpiryDateNeedFlag_1.Checked = True
        End Select
        TextBoxItemCode.Text = item.ItemCode
        TextBoxJANCode.Text = item.JanCode
        ComboBoxMeasureType.SelectedValue = item.MeasureType
        TextBoxShipment.Text = item.Shipment
        TextBoxItemName.Text = item.ItemName
        ComboBoxUnitType.SelectedValue = item.UnitType.UnitTypeId
        TextBoxLossRate.Text = (item.LossRate * 100).ToString(TextBoxLossRate.Format)
        If Not IsNothing(item.PieceUnitType.UnitTypeId) Then
            ComboBoxPieceUnitType.SelectedValue = item.PieceUnitType.UnitTypeId
        End If
        TextBoxPieceWeight.Text = CDbl(Nz(item.PieceWeight, 0)).ToString(TextBoxPieceWeight.Format)
        TextBoxCostPrice.Text = CDbl(Nz(item.CostPrice, 0)).ToString(TextBoxCostPrice.Format)
        TextBoxOriginalSellingPrice.Text = CInt(Nz(item.OriginalSellingPrice, 0)).ToString(TextBoxOriginalSellingPrice.Format)
        TextBoxInstoreSellingPrice.Text = CInt(Nz(item.InstoreSellingPrice, 0)).ToString(TextBoxInstoreSellingPrice.Format)
        If Not String.IsNullOrEmpty(TextBoxMainSupplierId.Text) Then
            TextBoxMainSupplierId.Text = item.MainSupplier.SupplierId
            TextBoxMainSupplierName.Text = Suppliers.GetById(TextBoxMainSupplierId.Text).SupplierName
        End If
        CheckBoxUnusedFlag.Checked = IIf(item.UnusedFlag = 1, True, False)

        Dim recipe As Recipe = Recipes.GetByItemId(item.ItemId)
        If Not IsNothing(recipe) Then
            TextBoxRecipeId.Text = recipe.RecipeId
            TextBoxComment.Text = recipe.Comment
            dataGridViewEditRecipeDetails.DataBind(item.ItemId)
        End If

    End Sub

    Private Function CheckData() As Boolean

        Dim result As Boolean = True

        Dim dgv As DataGridViewEditRecipeDetails = dataGridViewEditRecipeDetails

        ' 編集中の内容をセルに確定させる
        dgv.EndEdit()

        Try
            If String.IsNullOrEmpty(TextBoxItemName.Text) Then
                TextBoxItemName.Focus()
                Throw New ApplicationException("品目名が入力されていません。")
            End If
            If String.IsNullOrEmpty(ComboBoxUnitType.Text) Then
                ComboBoxUnitType.Focus()
                Throw New ApplicationException("単位が選択されていません。")
            End If
            If PanelSellingPrice.Visible Then
                If String.IsNullOrEmpty(TextBoxOriginalSellingPrice.Text) Then
                    TextBoxOriginalSellingPrice.Focus()
                    Throw New ApplicationException("振出単価が入力されていません。")
                End If
                If String.IsNullOrEmpty(TextBoxInstoreSellingPrice.Text) Then
                    TextBoxInstoreSellingPrice.Focus()
                    Throw New ApplicationException("店頭売価が入力されていません。")
                End If
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
            Dim item As New Item
            item.ItemId = _id
            item.ItemCode = TextBoxItemCode.Text
            item.JanCode = TextBoxJANCode.Text
            item.Shipment = TextBoxShipment.Text
            item.ItemName = TextBoxItemName.Text
            item.ItemTypeId = ComboBoxItemType.SelectedValue
            item.UnitType.UnitTypeId = ComboBoxUnitType.SelectedValue
            item.LossRate = CInt(Nz(TextBoxLossRate.Text, 100)) / 100
            item.PieceUnitType.UnitTypeId = Nz(ComboBoxPieceUnitType.SelectedValue, "")
            item.PieceWeight = TextBoxPieceWeight.Text
            item.CostPrice = TextBoxCostPrice.Text
            item.OriginalSellingPrice = TextBoxOriginalSellingPrice.Text
            item.InstoreSellingPrice = TextBoxInstoreSellingPrice.Text
            item.MainSupplier.SupplierId = TextBoxMainSupplierId.Text
            Select Case True
                Case RadioButtonExpiryDateNeedFlag_0.Checked
                    item.ExpiryDateNeedFlag = 0
                Case RadioButtonExpiryDateNeedFlag_1.Checked
                    item.ExpiryDateNeedFlag = 1
            End Select
            item.MeasureType = ComboBoxMeasureType.SelectedValue
            item.UnusedFlag = IIf(CheckBoxUnusedFlag.Checked, 1, 0)

            Items.Save(item)

            ' レシピを更新
            If PanelRecipeDetail.Visible Then
                Dim recipe As New Recipe
                recipe.RecipeId = TextBoxRecipeId.Text
                recipe.Comment = TextBoxComment.Text
                recipe.Item = item

                Dim recipeDetailList As New List(Of RecipeDetail)
                For Each row As DataGridViewRow In dataGridViewEditRecipeDetails.Rows
                    If row.IsNewRow Then Exit For

                    Dim recipeDetail As New RecipeDetail
                    recipeDetail.DetailNo = row.Index + 1
                    recipeDetail.Item = Items.GetById(dataGridViewEditRecipeDetails(dataGridViewEditRecipeDetails.TextBoxColumnItemId.Name, row.Index).Value.ToString)
                    recipeDetail.Quantity = dataGridViewEditRecipeDetails(dataGridViewEditRecipeDetails.TextBoxColumnQuantity.Name, row.Index).Value.ToString
                    recipeDetail.UnitType.UnitTypeId = dataGridViewEditRecipeDetails(dataGridViewEditRecipeDetails.ComboBoxColumnUnitType.Name, row.Index).Value.ToString
                    recipeDetail.Comment = Nz(dataGridViewEditRecipeDetails(dataGridViewEditRecipeDetails.TextBoxColumnComment.Name, row.Index).Value, "")
                    recipeDetailList.Add(recipeDetail)
                Next
                recipe.RecipeDetails = recipeDetailList
                Recipes.Save(recipe)
            End If
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try

    End Sub

    Private Sub DeleteData()

        Try
            Dim recipe As Recipe = Recipes.GetByItemId(_id)
            Recipes.Delete(recipe)

            Dim item As New Item
            item.ItemId = _id
            Items.Delete(item)
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try

    End Sub

    Private Sub InitializeForm(Optional isLoading As Boolean = False)

        ComboBoxItemType.SelectedItem = 0
        ComboBoxItemType_SelectedIndexChanged(Nothing, Nothing)
        TextBoxItemCode.Text = String.Empty
        TextBoxJANCode.Text = String.Empty
        TextBoxShipment.Text = String.Empty
        TextBoxItemName.Text = String.Empty
        ComboBoxUnitType.Text = String.Empty
        TextBoxLossRate.Text = String.Empty
        CheckBoxUnusedFlag.Checked = False
        ComboBoxPieceUnitType.Text = String.Empty
        TextBoxPieceWeight.Text = String.Empty
        TextBoxCostPrice.Text = String.Empty
        TextBoxOriginalSellingPrice.Text = String.Empty
        TextBoxInstoreSellingPrice.Text = String.Empty
        TextBoxMainSupplierId.Text = String.Empty
        TextBoxMainSupplierName.Text = String.Empty

        TextBoxRecipeId.Text = String.Empty
        TextBoxComment.Text = String.Empty
        dataGridViewEditRecipeDetails.Clear()

        If Not isLoading Then ComboBoxItemType.Focus()

    End Sub

#End Region

End Class

Public Class DataGridViewEditRecipeDetails
    Inherits CustomDataGridView

#Region "定義"

    Public ButtonColumnRemove As New CustomDatagridViewButtonColumn
    Public TextBoxColumnItemTypeName As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnItemName As New CustomDataGridViewTextBoxColumn
    Public ButtonColumnSearchItem As New CustomDatagridViewButtonColumn
    Public TextBoxColumnQuantity As New CustomDataGridViewTextBoxColumn
    Public ComboBoxColumnUnitType As New CustomDataGridViewComboBoxColumn
    Public TextBoxColumnComment As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnItemId As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnRecipeDetailId As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnItemTypeId As New CustomDataGridViewTextBoxColumn

    Public Event SearchItemClick(sender As Object, e As DataGridViewCellEventArgs)
    Public Event Quantity_Validating(sender As Object, e As DataGridViewCellValidatingEventArgs)
    Public Event UnitType_SelectedIndexChanged(sender As Object, selectedValue As Object, e As EventArgs)

    Public isDataBinding As Boolean
    Private comboBoxEditingControl As New CustomDataGridViewComboBoxEditingControl

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
        End With
        With TextBoxColumnItemName
            .Name = "TextBoxColumnItemName"
            .DataPropertyName = "ItemName"
            .HeaderText = String.Empty '見出しは結合ヘッダーで描くので空
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            .DefaultCellStyle.SelectionForeColor = Color.Black
            .DefaultCellStyle.SelectionBackColor = Color.White
            .Width = 490
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
        With TextBoxColumnQuantity
            .Name = "TextBoxColumnQuantity"
            .DataPropertyName = "Quantity"
            .HeaderText = String.Empty '見出しは結合ヘッダーで描くので空
            .Width = 80
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .DefaultCellStyle.Format = "#,##0"
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .NumericOnly = True
            .AllowSymbolChar = False
            .MaxByteLength = 5
        End With
        With ComboBoxColumnUnitType
            .Name = "ComboBoxColumnUnitType"
            .DataPropertyName = "UnitTypeId"
            .HeaderText = String.Empty '見出しは結合ヘッダーで描くので空
            .Width = 80
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .TabStop = True

            UnitTypes.BindComboBox(ComboBoxColumnUnitType, True, "UnusedFlag = 0")
        End With
        With TextBoxColumnComment
            .Name = "TextBoxColumnComment"
            .DataPropertyName = "Comment"
            .HeaderText = "特記事項"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            .Width = 400
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        End With
        With TextBoxColumnItemId
            .Name = "TextBoxColumnItemId"
            .DataPropertyName = "ItemId"
            .Visible = False
        End With
        With TextBoxColumnRecipeDetailId
            .Name = "TextBoxColumnRecipeDetailId"
            .DataPropertyName = "RecipeDetailId"
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
            .ColumnHeadersHeight = 22
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
            .Columns.Add(TextBoxColumnQuantity)
            .Columns.Add(ComboBoxColumnUnitType)
            .Columns.Add(TextBoxColumnComment)
            .Columns.Add(TextBoxColumnItemId)
            .Columns.Add(TextBoxColumnRecipeDetailId)

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
        End With

        BuildMergedHeaders()

        ' 行の背景色を辞書化
        For Each itemType As ItemType In ItemTypes.GetAll()
            itemTypeBackColors(itemType.ItemTypeName) = itemType.FormBackColor
        Next

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
                    Dim value As Integer
                    If Integer.TryParse(Me(customDataGridViewTextBoxColumn.Name, e.RowIndex).FormattedValue.ToString, value) Then
                        Me(customDataGridViewTextBoxColumn.Name, e.RowIndex).Value = value.ToString(customDataGridViewTextBoxColumn.DefaultCellStyle.Format)
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
            Case TextBoxColumnQuantity.Name
                RaiseEvent Quantity_Validating(Me, e)
        End Select

    End Sub

    Private Sub Me_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles Me.CellValueChanged
    End Sub

    Private Sub Me_EditingControlShowing(sender As Object, e As DataGridViewEditingControlShowingEventArgs) Handles Me.EditingControlShowing

        Select Case CurrentCell.OwningColumn.Name
            Case TextBoxColumnQuantity.Name
                If IsNumeric(e.Control.Text) Then
                    e.Control.Text = CInt(e.Control.Text).ToString(Me.TextBoxColumnQuantity.DefaultCellStyle.Format.Replace(",", ""))
                End If
            Case ComboBoxColumnUnitType.Name
                Dim ctl As CustomDataGridViewComboBoxEditingControl =
                TryCast(e.Control, CustomDataGridViewComboBoxEditingControl)
                If ctl Is Nothing Then Return

                comboBoxEditingControl = ctl
                ctl.Name = ComboBoxColumnUnitType.Name

                ' 同じコントロールが使い回されるのでハンドラ重複を防ぐ
                RemoveHandler ctl.SelectedIndexChanged, AddressOf dataGridViewComboBox_SelectedIndexChanged
                AddHandler ctl.SelectedIndexChanged, AddressOf dataGridViewComboBox_SelectedIndexChanged
        End Select

    End Sub

    Private Sub dataGridViewComboBox_SelectedIndexChanged(sender As Object, e As EventArgs)

        comboBoxEditingControl.EditingControlDataGridView.NotifyCurrentCellDirty(True)

        Select Case sender.name
            Case ComboBoxColumnUnitType.Name
                RaiseEvent UnitType_SelectedIndexChanged(Me, comboBoxEditingControl.SelectedValue, e)
        End Select

        comboBoxEditingControl.EditingControlDataGridView.EndEdit()

    End Sub

#End Region

#Region "メソッド"

    Public Sub DataBind(itemId As Integer)

        RemoveHandler Me.CellValidating, AddressOf Me_CellValidating

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Try
                isDataBinding = True

                Me.Rows.Clear()

                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String = String.Empty
                    SQLString = "select"
                    SQLString &= " RecipeDetails.RecipeDetailId"
                    SQLString &= ",Items.ItemId"
                    SQLString &= ",Items.ItemName"
                    SQLString &= ",ItemTypes.ItemTypeName"
                    SQLString &= ",RecipeDetails.Quantity"
                    SQLString &= ",RecipeDetails.UnitTypeId"
                    SQLString &= ",RecipeDetails.Comment"
                    SQLString &= " from ((RecipeDetails"
                    SQLString &= " left join Recipes on RecipeDetails.RecipeId = Recipes.RecipeId)"
                    SQLString &= " left join Items on RecipeDetails.ItemId = Items.ItemId)"
                    SQLString &= " left join ItemTypes on Items.ItemTypeId = ItemTypes.ItemTypeId"
                    SQLString &= " where Recipes.ItemId = @ItemId"
                    SQLString &= " order by"
                    SQLString &= " RecipeDetails.DetailNo"
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    cmd.Parameters.AddWithValue("@ItemId", NpgsqlDbType.Integer, itemId)
                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        If dr.HasRows Then
                            While dr.Read()
                                Dim column As New List(Of Object)
                                column.Add(Nothing)
                                column.Add(dr("ItemTypeName"))
                                column.Add(dr("ItemName"))
                                column.Add(Nothing)
                                column.Add(Nz(dr("Quantity"), 0))
                                column.Add(dr("UnitTypeId"))
                                column.Add(dr("Comment"))
                                column.Add(dr("ItemId"))
                                column.Add(dr("RecipeDetailId"))
                                Dim rowIndex As Integer = Me.Rows.Add(column.ToArray())
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
        AddMergedHeader(TextBoxColumnQuantity, ComboBoxColumnUnitType, "数量")

    End Sub

#End Region

End Class
