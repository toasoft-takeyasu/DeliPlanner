Imports Npgsql
Imports NpgsqlTypes

Public Class FormInventoryDisposals

#Region "定義"

    Private WithEvents dataGridViewInventoryDisposals As DataGridViewInventoryDisposals

#End Region

#Region "コンストラクタ"

    Public Sub New()

        InitializeComponent()

    End Sub

#End Region

#Region "イベント"

    Private Sub Me_Load(sender As Object, e As EventArgs) Handles Me.Load

        ItemTypes.BindComboBox(ComboBoxItemType, True, "RecipeNeedFlag = 0")

        dataGridViewInventoryDisposals = New DataGridViewInventoryDisposals()
        PanelItems.Controls.Add(dataGridViewInventoryDisposals)

        InitializeForm()

    End Sub

    Private Sub ButtonSearch_Click(sender As Object, e As EventArgs) Handles ButtonSearch.Click

        LoadData()

    End Sub

    Private Sub ButtonSearchItem_Click(sender As Object, e As EventArgs) Handles ButtonSearchItem.Click

        Using dialog As New DialogSearchItems()
            dialog.ItemTypeFilter = "RecipeNeedFlag = 0"

            If dialog.ShowDialog(Me) = DialogResult.OK Then
                TextBoxItemId.Text = dialog.Item.ItemId
                TextBoxItemName.Text = dialog.Item.ItemName
            End If
        End Using

    End Sub

    Private Sub ButtonClearItem_Click(sender As Object, e As EventArgs) Handles ButtonClearItem.Click

        TextBoxItemId.Text = String.Empty
        TextBoxItemName.Text = String.Empty

    End Sub

    Private Sub CheckBoxSummary_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxSummary.CheckedChanged

        If dataGridViewInventoryDisposals Is Nothing Then Exit Sub

        dataGridViewInventoryDisposals.ShowData(CheckBoxSummary.Checked)

    End Sub

    Private Sub dataGridViewInventoryDisposals_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dataGridViewInventoryDisposals.CellDoubleClick

        ' 品目別集計のデータ行をダブルクリックしたときだけ処理する
        If Not CheckBoxSummary.Checked Then Exit Sub
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Exit Sub

        Dim dgv As DataGridViewInventoryDisposals = dataGridViewInventoryDisposals
        Dim row As DataGridViewRow = dgv.Rows(e.RowIndex)

        If row.IsNewRow Then Exit Sub

        Dim itemId As String = Convert.ToString(row.Cells(dgv.TextBoxColumnItemId.Name).Value)
        Dim itemName As String = Convert.ToString(row.Cells(dgv.TextBoxColumnItemName.Name).Value)

        If String.IsNullOrEmpty(itemId) Then Exit Sub

        ' 選択した品目を検索条件に設定して明細を表示する
        TextBoxItemId.Text = itemId
        TextBoxItemName.Text = itemName
        CheckBoxSummary.Checked = False

        ButtonSearch.PerformClick()

    End Sub

#End Region

#Region "メソッド"

    Private Sub InitializeForm()

        DatePickerFrom.Value = New Date(Date.Today.Year, Date.Today.Month, 1)
        DatePickerFrom.Checked = True
        DatePickerTo.Value = Date.Today
        DatePickerTo.Checked = True

        ComboBoxItemType.SelectedIndex = -1
        TextBoxItemId.Text = String.Empty
        TextBoxItemName.Text = String.Empty
        CheckBoxSummary.Checked = True

        LoadData()

    End Sub

    Private Sub LoadData()

        Try
            If DatePickerFrom.Checked AndAlso DatePickerTo.Checked Then
                If DatePickerFrom.Value.Date > DatePickerTo.Value.Date Then
                    DatePickerFrom.Focus()
                    Throw New ApplicationException("廃棄日の範囲が正しくありません。")
                End If
            End If

            Dim dateFrom As Date? = Nothing
            Dim dateTo As Date? = Nothing

            If DatePickerFrom.Checked Then dateFrom = DatePickerFrom.Value.Date
            If DatePickerTo.Checked Then dateTo = DatePickerTo.Value.Date

            Dim itemTypeId As String = Convert.ToString(Nz(ComboBoxItemType.SelectedValue, String.Empty))
            Dim itemId As String = TextBoxItemId.Text.Trim()

            dataGridViewInventoryDisposals.DataBind(itemTypeId, itemId, dateFrom, dateTo)
            dataGridViewInventoryDisposals.ShowData(CheckBoxSummary.Checked)

            UpdateTotal()

        Catch ex As Exception
            If ex.GetType Is GetType(ApplicationException) Then
                MessageBox.Show(ex.Message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Else
                SysMsgBox(ex.Message, Me.Text)
            End If
        End Try

    End Sub

    Private Sub UpdateTotal()

        Dim dgv As DataGridViewInventoryDisposals = dataGridViewInventoryDisposals

        LabelTotal.Text = "廃棄件数：" & dgv.RecordCount.ToString("#,##0") & "件"

        If dgv.UnknownAmountCount = 0 Then
            LabelTotal.Text &= "    廃棄金額合計：" & dgv.TotalAmount.ToString("#,##0.##") & "円"
            LabelTotal.ForeColor = SystemColors.ControlText
        Else
            LabelTotal.Text &= "    廃棄金額（判明分）：" & dgv.TotalAmount.ToString("#,##0.##") & "円"
            LabelTotal.Text &= "    ※単価未設定の履歴が" & dgv.UnknownAmountCount.ToString("#,##0") & "件あります"
            LabelTotal.ForeColor = Color.DarkRed
        End If

    End Sub

#End Region

End Class

Public Class DataGridViewInventoryDisposals
    Inherits CustomDataGridView

#Region "定義"

    Public TextBoxColumnTransactionDate As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnItemTypeName As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnItemId As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnItemName As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnInventoryId As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnQuantity As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnUnitTypeName As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnUnitPrice As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnDisposalAmount As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnRecordCount As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnUnknownAmountCount As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnOperatorName As New CustomDataGridViewTextBoxColumn

    Private detailTable As DataTable
    Private summaryTable As DataTable
    Private unitTypeList As List(Of UnitType)

    Private _recordCount As Integer
    Private _totalAmount As Decimal
    Private _unknownAmountCount As Integer

    Private itemTypeBackColors As New Dictionary(Of String, Color)

    Public ReadOnly Property RecordCount As Integer
        Get
            Return _recordCount
        End Get
    End Property

    Public ReadOnly Property TotalAmount As Decimal
        Get
            Return _totalAmount
        End Get
    End Property

    Public ReadOnly Property UnknownAmountCount As Integer
        Get
            Return _unknownAmountCount
        End Get
    End Property
#End Region

#Region "コンストラクタ"

    Public Sub New()

        With TextBoxColumnTransactionDate
            .Name = "TextBoxColumnTransactionDate"
            .DataPropertyName = "TransactionDate"
            .HeaderText = "廃棄日"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .DefaultCellStyle.Format = "yyyy/MM/dd"
            .DefaultCellStyle.SelectionForeColor = Color.Black
            .DefaultCellStyle.SelectionBackColor = Color.White
            .Width = 100
            .ReadOnly = True
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
            .HeaderText = "品目名"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            .DefaultCellStyle.SelectionForeColor = Color.Black
            .DefaultCellStyle.SelectionBackColor = Color.White
            .Width = 200
            .ReadOnly = True
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .TabStop = False
        End With
        With TextBoxColumnQuantity
            .Name = "TextBoxColumnQuantity"
            .DataPropertyName = "Quantity"
            .HeaderText = String.Empty
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .DefaultCellStyle.Format = "#,##0.##"
            .DefaultCellStyle.SelectionForeColor = Color.Black
            .DefaultCellStyle.SelectionBackColor = Color.White
            .Width = 100
            .ReadOnly = True
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .TabStop = False
        End With
        With TextBoxColumnUnitTypeName
            .Name = "TextBoxColumnUnitTypeName"
            .DataPropertyName = "UnitTypeName"
            .HeaderText = String.Empty
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            .DefaultCellStyle.SelectionForeColor = Color.Black
            .DefaultCellStyle.SelectionBackColor = Color.White
            .Width = 60
            .ReadOnly = True
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .TabStop = False
        End With
        With TextBoxColumnUnitPrice
            .Name = "TextBoxColumnUnitPrice"
            .DataPropertyName = "BaseUnitPrice"
            .HeaderText = "基準単価"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .DefaultCellStyle.Format = "#,##0.########"
            .DefaultCellStyle.NullValue = "不明"
            .DefaultCellStyle.SelectionForeColor = Color.Black
            .DefaultCellStyle.SelectionBackColor = Color.White
            .Width = 100
            .ReadOnly = True
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .TabStop = False
        End With
        With TextBoxColumnDisposalAmount
            .Name = "TextBoxColumnDisposalAmount"
            .DataPropertyName = "DisposalAmount"
            .HeaderText = "廃棄金額"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .DefaultCellStyle.Format = "#,##0.##"
            .DefaultCellStyle.NullValue = "不明"
            .DefaultCellStyle.SelectionForeColor = Color.Black
            .DefaultCellStyle.SelectionBackColor = Color.White
            .Width = 120
            .ReadOnly = True
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .TabStop = False
        End With
        With TextBoxColumnRecordCount
            .Name = "TextBoxColumnRecordCount"
            .DataPropertyName = "RecordCount"
            .HeaderText = "廃棄件数"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .DefaultCellStyle.Format = "#,##0"
            .DefaultCellStyle.SelectionForeColor = Color.Black
            .DefaultCellStyle.SelectionBackColor = Color.White
            .Width = 90
            .ReadOnly = True
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .TabStop = False
            .Visible = False
        End With
        With TextBoxColumnUnknownAmountCount
            .Name = "TextBoxColumnUnknownAmountCount"
            .DataPropertyName = "UnknownAmountCount"
            .HeaderText = "金額不明件数"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .DefaultCellStyle.Format = "#,##0"
            .DefaultCellStyle.SelectionForeColor = Color.Black
            .DefaultCellStyle.SelectionBackColor = Color.White
            .Width = 120
            .ReadOnly = True
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .TabStop = False
            .Visible = False
        End With
        With TextBoxColumnOperatorName
            .Name = "TextBoxColumnOperatorName"
            .DataPropertyName = "OperatorName"
            .HeaderText = "担当者"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            .DefaultCellStyle.SelectionForeColor = Color.Black
            .DefaultCellStyle.SelectionBackColor = Color.White
            .Width = 120
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
        With TextBoxColumnInventoryId
            .Name = "TextBoxColumnInventoryId"
            .DataPropertyName = "InventoryId"
            .HeaderText = "在庫ID"
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
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .DefaultCellStyle.NullValue = String.Empty
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
            .RowTemplate.Height = 22
            .NumberOnRowHeader = False
            .AutoGenerateColumns = False
            .ScrollBars = Windows.Forms.ScrollBars.Both

            .Columns.Add(TextBoxColumnTransactionDate)
            .Columns.Add(TextBoxColumnItemTypeName)
            .Columns.Add(TextBoxColumnItemName)
            .Columns.Add(TextBoxColumnQuantity)
            .Columns.Add(TextBoxColumnUnitTypeName)
            .Columns.Add(TextBoxColumnUnitPrice)
            .Columns.Add(TextBoxColumnDisposalAmount)
            .Columns.Add(TextBoxColumnRecordCount)
            .Columns.Add(TextBoxColumnUnknownAmountCount)
            .Columns.Add(TextBoxColumnOperatorName)
            .Columns.Add(TextBoxColumnItemId)
            .Columns.Add(TextBoxColumnInventoryId)

            ' 動作関係のプロパティを変更
            .ReadOnly = True
            .AllowUserToOrderColumns = False
            .AllowUserToAddRows = False
            .AllowUserToDeleteRows = False
            .AllowUserToResizeColumns = False
            .AllowUserToResizeRows = False
            .EnterToTab = True
            .EditMode = DataGridViewEditMode.EditOnEnter
            .SelectionMode = DataGridViewSelectionMode.CellSelect
            .MultiSelect = False
            .MouseRightButtonOnSelect = False
            .DrawCellPointer = False
            .DoubleBuffered = True
            .ResizeRedraw = True
        End With

        ' 数量と単位の結合ヘッダーを定義
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

    Private Sub Me_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles Me.CellFormatting

        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Return

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

#End Region

#Region "メソッド"

    Public Sub DataBind(Optional itemTypeId As String = Nothing, Optional itemId As String = Nothing, Optional dateFrom As Date? = Nothing, Optional dateTo As Date? = Nothing)

        Dim targetItemTypeId As Integer
        Dim targetItemId As Integer

        If Not String.IsNullOrEmpty(itemTypeId) Then
            If Not Integer.TryParse(itemTypeId, targetItemTypeId) Then
                Throw New ApplicationException("品目区分IDが正しくありません。")
            End If
        End If

        If Not String.IsNullOrEmpty(itemId) Then
            If Not Integer.TryParse(itemId, targetItemId) Then
                Throw New ApplicationException("品目IDが正しくありません。")
            End If
        End If

        Dim table As New DataTable

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()

            Using cmd As NpgsqlCommand = cn.CreateCommand()
                Dim SQLString As String = String.Empty

                SQLString = "select"
                SQLString &= " InventoryLogs.InventoryLogId"
                SQLString &= ",InventoryLogs.TransactionDate"
                SQLString &= ",InventoryLogs.RegistDate"
                SQLString &= ",InventoryLogs.ItemId"
                SQLString &= ",Items.ItemName"
                SQLString &= ",ItemTypes.ItemTypeName"
                SQLString &= ",InventoryLogs.InventoryId"
                SQLString &= ",-InventoryLogs.Quantity as BaseQuantity"
                SQLString &= ",InventoryLogs.UnitTypeId as BaseUnitTypeId"
                SQLString &= ",UnitTypes.UnitTypeName as BaseUnitTypeName"
                SQLString &= ",InventoryLogs.UnitPrice as BaseUnitPrice"
                SQLString &= ",Operators.OperatorName"
                SQLString &= " from InventoryLogs"
                SQLString &= " inner join Items on InventoryLogs.ItemId = Items.ItemId"
                SQLString &= " inner join ItemTypes on Items.ItemTypeId = ItemTypes.ItemTypeId"
                SQLString &= " left join UnitTypes on InventoryLogs.UnitTypeId = UnitTypes.UnitTypeId"
                SQLString &= " left join Operators on InventoryLogs.OperatorId = Operators.OperatorId"
                SQLString &= " where InventoryLogs.TransactionStatus = @Status"
                SQLString &= " and InventoryLogs.SourceId = @AjustTypeId"
                SQLString &= " and InventoryLogs.Quantity < 0"
                SQLString &= " and ItemTypes.RecipeNeedFlag = 0"

                cmd.Parameters.Clear()
                cmd.Parameters.AddWithValue("@Status", NpgsqlDbType.Integer, CInt(InventoryLogStatus.Adjust))
                cmd.Parameters.AddWithValue("@AjustTypeId", NpgsqlDbType.Integer, 2)

                If Not String.IsNullOrEmpty(itemTypeId) Then
                    SQLString &= " and Items.ItemTypeId = @ItemTypeId"
                    cmd.Parameters.AddWithValue("@ItemTypeId", NpgsqlDbType.Integer, targetItemTypeId)
                End If
                If Not String.IsNullOrEmpty(itemId) Then
                    SQLString &= " and InventoryLogs.ItemId = @ItemId"
                    cmd.Parameters.AddWithValue("@ItemId", NpgsqlDbType.Integer, targetItemId)
                End If
                If dateFrom.HasValue Then
                    SQLString &= " and InventoryLogs.TransactionDate >= @DateFrom"
                    cmd.Parameters.AddWithValue("@DateFrom", NpgsqlDbType.Timestamp, dateFrom.Value.Date)
                End If
                If dateTo.HasValue Then
                    SQLString &= " and InventoryLogs.TransactionDate < @DateToExclusive"
                    cmd.Parameters.AddWithValue("@DateToExclusive", NpgsqlDbType.Timestamp, dateTo.Value.Date.AddDays(1))
                End If

                SQLString &= " order by InventoryLogs.TransactionDate desc"
                SQLString &= ",InventoryLogs.RegistDate desc"
                SQLString &= ",InventoryLogs.InventoryLogId desc"

                cmd.CommandText = SQLString

                Using adapter As New NpgsqlDataAdapter(cmd)
                    adapter.Fill(table)
                End Using
            End Using
        End Using

        unitTypeList = UnitTypes.GetAll()

        table.Columns.Add("Quantity", GetType(Double))
        table.Columns.Add("UnitTypeName", GetType(String))
        table.Columns.Add("DisposalAmount", GetType(Decimal))
        table.Columns.Add("RecordCount", GetType(Integer))
        table.Columns.Add("UnknownAmountCount", GetType(Integer))

        Dim total As Decimal = 0
        Dim unknown As Integer = 0

        For Each row As DataRow In table.Rows
            row("RecordCount") = 1
            row("UnknownAmountCount") = 0

            If IsDBNull(row("BaseUnitPrice")) Then
                row("DisposalAmount") = DBNull.Value
                row("UnknownAmountCount") = 1
                unknown += 1
            Else
                Dim amount As Decimal = CDec(row("BaseQuantity")) * CDec(row("BaseUnitPrice"))
                row("DisposalAmount") = amount
                total += amount
            End If

            SetDisplayQuantity(row)
        Next

        Dim summary As DataTable = CreateSummary(table)

        detailTable = table
        summaryTable = summary
        _recordCount = table.Rows.Count
        _totalAmount = total
        _unknownAmountCount = unknown

    End Sub

    Public Sub ShowData(isSummary As Boolean)

        If detailTable Is Nothing Then Exit Sub

        Me.DataSource = Nothing

        TextBoxColumnItemId.Visible = False
        TextBoxColumnInventoryId.Visible = False
        TextBoxColumnItemTypeName.Visible = (sharedIniFile.VisivleItemType <> 0)
        TextBoxColumnTransactionDate.Visible = Not isSummary
        TextBoxColumnUnitPrice.Visible = Not isSummary
        TextBoxColumnOperatorName.Visible = Not isSummary
        TextBoxColumnRecordCount.Visible = isSummary
        TextBoxColumnUnknownAmountCount.Visible = isSummary AndAlso UnknownAmountCount > 0

        TextBoxColumnDisposalAmount.HeaderText = If(isSummary AndAlso UnknownAmountCount > 0, "廃棄金額（判明分）", "廃棄金額")

        If isSummary Then
            Dim dataView As New DataView(summaryTable)
            dataView.Sort = "DisposalAmount DESC, ItemId ASC"
            Me.DataSource = dataView
        Else
            Me.DataSource = detailTable.DefaultView
        End If

        BuildMergedHeaders()

    End Sub

    Private Function CreateSummary(table As DataTable) As DataTable

        Dim result As DataTable = table.Clone()
        Dim rowMap As New Dictionary(Of String, DataRow)

        For Each row As DataRow In table.Rows
            ' 基準単位が異なる履歴の数量は混ぜない
            Dim key As String = Convert.ToString(row("ItemId")) & ":" & Convert.ToString(row("BaseUnitTypeId"))
            Dim summaryRow As DataRow = Nothing

            If Not rowMap.TryGetValue(key, summaryRow) Then
                summaryRow = result.NewRow()

                summaryRow("ItemId") = row("ItemId")
                summaryRow("ItemName") = row("ItemName")
                summaryRow("ItemTypeName") = row("ItemTypeName")
                summaryRow("BaseUnitTypeId") = row("BaseUnitTypeId")
                summaryRow("BaseUnitTypeName") = row("BaseUnitTypeName")
                summaryRow("BaseQuantity") = 0.0
                summaryRow("DisposalAmount") = DBNull.Value
                summaryRow("RecordCount") = 0
                summaryRow("UnknownAmountCount") = 0

                result.Rows.Add(summaryRow)
                rowMap.Add(key, summaryRow)
            End If

            summaryRow("BaseQuantity") = CDbl(summaryRow("BaseQuantity")) + CDbl(row("BaseQuantity"))
            summaryRow("RecordCount") = CInt(summaryRow("RecordCount")) + 1
            summaryRow("UnknownAmountCount") = CInt(summaryRow("UnknownAmountCount")) + CInt(row("UnknownAmountCount"))

            If Not IsDBNull(row("DisposalAmount")) Then
                summaryRow("DisposalAmount") = CDec(Nz(summaryRow("DisposalAmount"), 0)) + CDec(row("DisposalAmount"))
            End If
        Next

        For Each row As DataRow In result.Rows
            SetDisplayQuantity(row)
        Next

        Return result

    End Function

    Private Sub SetDisplayQuantity(row As DataRow)

        Dim quantity As Double = CDbl(row("BaseQuantity"))
        Dim baseUnitId As Integer = CInt(Nz(row("BaseUnitTypeId"), 0))
        Dim unitName As String = Convert.ToString(Nz(row("BaseUnitTypeName"), String.Empty))
        Dim displayFactor As Double = 1

        ' 履歴に保存された単位を基準に換算する
        Dim category As Integer = calc.GetUnitCategory(baseUnitId)

        ' 単位区分：1=重量、2=容量。それ以外は換算しない
        If baseUnitId > 0 AndAlso (category = 1 OrElse category = 2) AndAlso calc.IsConvertibleUnit(baseUnitId) Then
            Dim baseFactor As Double = calc.GetBaseFactor(baseUnitId)

            For Each unit As UnitType In unitTypeList
                Dim unitId As Integer = CInt(unit.UnitTypeId)

                If CInt(Nz(unit.UnusedFlag, 0)) <> 0 Then Continue For
                If Not calc.IsConvertibleUnit(unitId) Then Continue For
                If calc.GetUnitCategory(unitId) <> category Then Continue For
                If Not IsNumeric(unit.BaseFactor) Then Continue For
                If CDbl(unit.BaseFactor) <= 0 Then Continue For

                Dim factor As Double = CDbl(unit.BaseFactor) / baseFactor

                If factor > displayFactor AndAlso quantity >= factor Then
                    displayFactor = factor
                    unitName = unit.UnitTypeName
                End If
            Next
        End If

        row("Quantity") = quantity / displayFactor
        row("UnitTypeName") = If(String.IsNullOrEmpty(unitName), "不明", unitName)

    End Sub

    Private Sub BuildMergedHeaders()

        ClearMergedHeaders()
        AddMergedHeader(TextBoxColumnQuantity, TextBoxColumnUnitTypeName, "廃棄数量")

    End Sub

#End Region

End Class