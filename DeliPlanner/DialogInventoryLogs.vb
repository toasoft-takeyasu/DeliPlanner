Imports Npgsql
Imports NpgsqlTypes

Public Class DialogInventoryLogs

#Region "定義"

    Private ReadOnly _inventoryId As String
    Private ReadOnly _itemId As String

    Private WithEvents dataGridViewInventoryLogs As DataGridViewInventoryLogs

#End Region

#Region "コンストラクタ"

    Public Sub New(Optional inventoryId As String = Nothing, Optional itemId As String = Nothing)

        InitializeComponent()

        _inventoryId = inventoryId
        _itemId = itemId

    End Sub

#End Region

#Region "イベント"

    Private Sub Me_Load(sender As Object, e As EventArgs) Handles Me.Load

        dataGridViewInventoryLogs = New DataGridViewInventoryLogs()
        PanelItems.Controls.Add(dataGridViewInventoryLogs)

        InitializeStatus()
        InitializeForm(True)

    End Sub

    Private Sub ButtonSearch_Click(sender As Object, e As EventArgs) Handles ButtonSearch.Click

        LoadData()

    End Sub

#End Region

#Region "メソッド"

    Private Sub InitializeForm(Optional isLoading As Boolean = False)

        DatePickerFrom.Value = Date.Today.AddMonths(-1)
        DatePickerFrom.Checked = False
        DatePickerTo.Value = Date.Today
        DatePickerTo.Checked = False
        ComboBoxStatus.SelectedValue = 0

        LoadData()

    End Sub

    Private Sub InitializeStatus()

        Dim table As New DataTable
        table.Columns.Add("StatusId", GetType(Integer))
        table.Columns.Add("StatusName", GetType(String))
        table.Rows.Add(0, "すべて")

        For Each status As InventoryLogStatus In [Enum].GetValues(GetType(InventoryLogStatus))
            table.Rows.Add(CInt(status), DataGridViewInventoryLogs.GetStatusName(CInt(status)))
        Next

        With ComboBoxStatus
            .DisplayMember = "StatusName"
            .ValueMember = "StatusId"
            .DataSource = table
        End With

    End Sub

    Private Sub LoadData()

        Try
            If DatePickerFrom.Checked AndAlso DatePickerTo.Checked Then
                If DatePickerFrom.Value.Date > DatePickerTo.Value.Date Then
                    DatePickerFrom.Focus()
                    Throw New ApplicationException("処理日の範囲が正しくありません。")
                End If
            End If

            Dim dateFrom As Date? = Nothing
            Dim dateTo As Date? = Nothing

            If DatePickerFrom.Checked Then dateFrom = DatePickerFrom.Value.Date
            If DatePickerTo.Checked Then dateTo = DatePickerTo.Value.Date

            dataGridViewInventoryLogs.DataBind(_inventoryId, _itemId, dateFrom, dateTo, CInt(ComboBoxStatus.SelectedValue))

        Catch ex As Exception
            If ex.GetType Is GetType(ApplicationException) Then
                MessageBox.Show(ex.Message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Else
                SysMsgBox(ex.Message, Me.Text)
            End If
        End Try

    End Sub

#End Region

End Class

Public Class DataGridViewInventoryLogs
    Inherits CustomDataGridView

#Region "定義"

    Public TextBoxColumnTransactionDate As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnRegistDate As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnStatusName As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnAjustTypeName As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnInventoryId As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnQuantity As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnUnitTypeName As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnAfterQuantity As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnUnitPrice As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnSourceId As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnOperatorName As New CustomDataGridViewTextBoxColumn

    Private dataView As New DataView

#End Region

#Region "コンストラクタ"

    Public Sub New()

        With TextBoxColumnTransactionDate
            .Name = "TextBoxColumnTransactionDate"
            .DataPropertyName = "TransactionDate"
            .HeaderText = "処理日"
            .Width = 105
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .DefaultCellStyle.Format = "yyyy/MM/dd"
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .TabStop = False
        End With
        With TextBoxColumnRegistDate
            .Name = "TextBoxColumnRegistDate"
            .DataPropertyName = "RegistDate"
            .HeaderText = "登録日時"
            .Width = 165
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .DefaultCellStyle.Format = "yyyy/MM/dd HH:mm:ss"
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .TabStop = False
        End With
        With TextBoxColumnStatusName
            .Name = "TextBoxColumnStatusName"
            .DataPropertyName = "StatusName"
            .HeaderText = "区分"
            .Width = 145
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .TabStop = False
        End With
        With TextBoxColumnAjustTypeName
            .Name = "TextBoxColumnAjustTypeName"
            .DataPropertyName = "AjustTypeName"
            .HeaderText = "調整理由"
            .Width = 120
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .TabStop = False
        End With
        With TextBoxColumnInventoryId
            .Name = "TextBoxColumnInventoryId"
            .DataPropertyName = "InventoryId"
            .HeaderText = "在庫ID"
            .Width = 80
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .TabStop = False
        End With
        With TextBoxColumnQuantity
            .Name = "TextBoxColumnQuantity"
            .DataPropertyName = "Quantity"
            .HeaderText = "増減数量"
            .Width = 120
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .DefaultCellStyle.Format = "+#,##0.########;-#,##0.########;0"
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .TabStop = False
        End With
        With TextBoxColumnUnitTypeName
            .Name = "TextBoxColumnUnitTypeName"
            .DataPropertyName = "UnitTypeName"
            .HeaderText = "単位"
            .Width = 60
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .TabStop = False
        End With
        With TextBoxColumnAfterQuantity
            .Name = "TextBoxColumnAfterQuantity"
            .DataPropertyName = "AfterQuantity"
            .HeaderText = "変動後残高"
            .Width = 120
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .DefaultCellStyle.Format = "#,##0.########"
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .TabStop = False
        End With
        With TextBoxColumnUnitPrice
            .Name = "TextBoxColumnUnitPrice"
            .DataPropertyName = "UnitPrice"
            .HeaderText = "基準単価"
            .Width = 100
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .DefaultCellStyle.Format = "#,##0.########"
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .TabStop = False
        End With
        With TextBoxColumnSourceId
            .Name = "TextBoxColumnSourceId"
            .DataPropertyName = "SourceId"
            .HeaderText = "元データID"
            .Width = 100
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .TabStop = False
            .Visible = False
        End With
        With TextBoxColumnOperatorName
            .Name = "TextBoxColumnOperatorName"
            .DataPropertyName = "OperatorName"
            .HeaderText = "担当者名"
            .Width = 120
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .TabStop = False
        End With

        With Me
            ' デザイン関係のプロパティを変更
            .EnableHeadersVisualStyles = False
            .AutoSize = False
            .Dock = DockStyle.Fill
            .BackgroundColor = SystemColors.ActiveCaption
            .GridColor = SystemColors.ControlDarkDark
            .DefaultCellStyle.Font = New Font("ＭＳ ゴシック", 12)
            .DefaultCellStyle.NullValue = String.Empty
            .ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
            .ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single
            .ColumnHeadersDefaultCellStyle.Font = New Font("MS UI Gothic", 12)
            .ColumnHeadersHeight = 22
            .RowHeadersVisible = False
            .RowTemplate.Height = 22
            .NumberOnRowHeader = False
            .AutoGenerateColumns = False
            .ScrollBars = Windows.Forms.ScrollBars.Both

            .Columns.Add(TextBoxColumnTransactionDate)
            .Columns.Add(TextBoxColumnRegistDate)
            .Columns.Add(TextBoxColumnStatusName)
            .Columns.Add(TextBoxColumnAjustTypeName)
            .Columns.Add(TextBoxColumnInventoryId)
            .Columns.Add(TextBoxColumnQuantity)
            .Columns.Add(TextBoxColumnUnitTypeName)
            .Columns.Add(TextBoxColumnAfterQuantity)
            .Columns.Add(TextBoxColumnUnitPrice)
            .Columns.Add(TextBoxColumnSourceId)
            .Columns.Add(TextBoxColumnOperatorName)

            ' 動作関係のプロパティを変更
            .ReadOnly = True
            .AllowUserToOrderColumns = False
            .AllowUserToAddRows = False
            .AllowUserToDeleteRows = False
            .AllowUserToResizeColumns = False
            .AllowUserToResizeRows = False
            .EnterToTab = True
            .EditMode = DataGridViewEditMode.EditOnEnter
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            .MultiSelect = False
            .MouseRightButtonOnSelect = False
            .DrawCellPointer = False
            .DoubleBuffered = True
        End With

    End Sub

#End Region

#Region "メソッド"

    Public Sub DataBind(Optional inventoryId As String = Nothing, Optional itemId As String = Nothing, Optional dateFrom As Date? = Nothing, Optional dateTo As Date? = Nothing, Optional status As Integer = 0)

        Dim lotId As Integer
        Dim targetItemId As Integer

        If Not String.IsNullOrEmpty(inventoryId) Then
            If Not Integer.TryParse(inventoryId, lotId) Then
                Throw New ApplicationException("在庫IDが正しくありません。")
            End If
        End If

        If Not String.IsNullOrEmpty(itemId) Then
            If Not Integer.TryParse(itemId, targetItemId) Then
                Throw New ApplicationException("品目IDが正しくありません。")
            End If
        End If

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Try
                Using cmd As NpgsqlCommand = cn.CreateCommand()

                    Dim SQLString As String = String.Empty
                    SQLString = "select"
                    SQLString &= " InventoryLogs.InventoryLogId"
                    SQLString &= ",InventoryLogs.TransactionDate"
                    SQLString &= ",InventoryLogs.RegistDate"
                    SQLString &= ",InventoryLogs.TransactionStatus"
                    SQLString &= ",AjustTypes.AjustTypeName"
                    SQLString &= ",InventoryLogs.ItemId"
                    SQLString &= ",Items.ItemName"
                    SQLString &= ",InventoryLogs.InventoryId"
                    SQLString &= ",InventoryLogs.Quantity"
                    SQLString &= ",UnitTypes.UnitTypeName"
                    SQLString &= ",InventoryLogs.AfterQuantity"
                    SQLString &= ",InventoryLogs.UnitPrice"
                    SQLString &= ",InventoryLogs.SourceId"
                    SQLString &= ",Operators.OperatorName"
                    SQLString &= " from InventoryLogs"
                    SQLString &= " left join Items on InventoryLogs.ItemId = Items.ItemId"
                    SQLString &= " left join UnitTypes on InventoryLogs.UnitTypeId = UnitTypes.UnitTypeId"
                    SQLString &= " left join Operators on InventoryLogs.OperatorId = Operators.OperatorId"
                    SQLString &= " left join AjustTypes on InventoryLogs.SourceId = AjustTypes.AjustTypeId and InventoryLogs.TransactionStatus = @AdjustStatus"
                    SQLString &= " where InventoryLogs.InventoryLogId is not null"
                    cmd.Parameters.Clear()
                    cmd.Parameters.AddWithValue("@AdjustStatus", NpgsqlDbType.Integer, CInt(InventoryLogStatus.Adjust))
                    If Not String.IsNullOrEmpty(inventoryId) Then
                        SQLString &= " and InventoryLogs.InventoryId = @InventoryId"
                        cmd.Parameters.AddWithValue("@InventoryId", NpgsqlDbType.Integer, lotId)
                    End If
                    If Not String.IsNullOrEmpty(itemId) Then
                        SQLString &= " and InventoryLogs.ItemId = @ItemId"
                        cmd.Parameters.AddWithValue("@ItemId", NpgsqlDbType.Integer, targetItemId)
                    End If
                    If dateFrom.HasValue Then
                        SQLString &= " and InventoryLogs.TransactionDate >= @DateFrom"
                        cmd.Parameters.AddWithValue("@DateFrom", NpgsqlDbType.Date, dateFrom.Value.Date)
                    End If
                    If dateTo.HasValue Then
                        SQLString &= " and InventoryLogs.TransactionDate <= @DateTo"
                        cmd.Parameters.AddWithValue("@DateTo", NpgsqlDbType.Date, dateTo.Value.Date)
                    End If
                    If status <> 0 Then
                        SQLString &= " and InventoryLogs.TransactionStatus = @Status"
                        cmd.Parameters.AddWithValue("@Status", NpgsqlDbType.Integer, status)
                    End If
                    SQLString &= " order by"
                    SQLString &= " InventoryLogs.RegistDate desc"
                    SQLString &= ",InventoryLogs.InventoryLogId desc"
                    cmd.CommandText = SQLString
                    Dim dataTable As New DataTable
                    Using dataAdapter As New NpgsqlDataAdapter(cmd)
                        dataAdapter.Fill(dataTable)
                    End Using
                    dataTable.Columns.Add("StatusName", GetType(String))
                    For Each row As DataRow In dataTable.Rows
                        row("StatusName") = GetStatusName(CInt(Nz(row("TransactionStatus"), 0)))
                    Next
                    Me.dataView = dataTable.DefaultView
                    Me.DataSource = Me.dataView
                End Using
            Catch ex As Exception
                Throw
            End Try

        End Using

    End Sub

    Public Shared Function GetStatusName(status As Integer) As String

        Select Case status
            Case CInt(InventoryLogStatus.Arrival)
                Return "入荷"
            Case CInt(InventoryLogStatus.ArrivalCancel)
                Return "入荷修正・取消"
            Case CInt(InventoryLogStatus.Consume)
                Return "製造材料払い出し"
            Case CInt(InventoryLogStatus.Produce)
                Return "製造完成入庫"
            Case CInt(InventoryLogStatus.ProduceCancel)
                Return "製造完了取消"
            Case CInt(InventoryLogStatus.Adjust)
                Return "在庫調整"
            Case CInt(InventoryLogStatus.Stocktaking)
                Return "棚卸"
            Case CInt(InventoryLogStatus.Disposal)
                Return "廃棄"
            Case Else
                Return "不明（" & status.ToString() & "）"
        End Select

    End Function

    Public Sub Clear()

        If Me.dataView.Table IsNot Nothing Then
            Me.dataView.Table.Clear()
        Else
            Me.Rows.Clear()
        End If

    End Sub

#End Region

End Class