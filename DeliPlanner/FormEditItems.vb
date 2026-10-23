Imports Npgsql

Public Class FormEditItems
    Implements IRefreshable

#Region "定義"

    Private ReadOnly _menu As FormMenu
    Private WithEvents dataGridViewEditItems As DataGridViewEditItems
    Private _selectedId As String

#End Region

#Region "コンストラクタ"

    Public Sub New(parent As Form, Optional selectedId As String = Nothing)

        InitializeComponent()
        _menu = parent
        _selectedId = selectedId

    End Sub

#End Region

#Region "イベント"

    Private Sub Me_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        ItemTypes.BindComboBox(ComboBoxFilerItemType, True)
        MeasureTypes.BindComboBox(ComboBoxFilterMeasureType, True)

        dataGridViewEditItems = New DataGridViewEditItems()
        PanelItems.Controls.Add(dataGridViewEditItems)

        ' フォームの初期化
        InitializeForm(True)

    End Sub

    Private Sub Me_Shown(sender As Object, e As System.EventArgs) Handles Me.Shown

        AddHandler ComboBoxFilerItemType.SelectedValueChanged, AddressOf TextBoxFilter_TextChanged
        AddHandler TextBoxFilterItemCode.TextChanged, AddressOf TextBoxFilter_TextChanged
        AddHandler TextBoxFilterJanCode.TextChanged, AddressOf TextBoxFilter_TextChanged
        AddHandler TextBoxFilterItemName.TextChanged, AddressOf TextBoxFilter_TextChanged
        AddHandler ComboBoxFilterMeasureType.SelectedValueChanged, AddressOf TextBoxFilter_TextChanged

        ' 編集から戻ってきた場合はその行へ。無ければ最下行へ。
        If Not String.IsNullOrEmpty(_selectedId) AndAlso dataGridViewEditItems.SelectRowById(_selectedId) Then
            ' 位置付け成功(何もしない)
        Else
            dataGridViewEditItems.ScrollToBottom()
        End If

        ComboBoxFilerItemType.Focus()

    End Sub

    Private Sub TextBoxFilter_TextChanged(sender As System.Object, e As System.EventArgs)

        dataGridViewEditItems.Filter(Nz(ComboBoxFilerItemType.SelectedValue, ""), TextBoxFilterItemCode.Text, TextBoxFilterJanCode.Text,
                                     TextBoxFilterItemName.Text, Nz(ComboBoxFilterMeasureType.SelectedValue, ""))

    End Sub

    Private Sub dataGridViewEditItems_SelectClick(sender As Object, e As DataGridViewCellEventArgs) Handles dataGridViewEditItems.SelectClick

        If e.RowIndex = dataGridViewEditItems.NewRowIndex Then
            _menu.openChildForm(_menu.currentButton, New FormEditItem(_menu, EditMode.Add))
        Else
            Dim id As String = dataGridViewEditItems.Rows(e.RowIndex).Cells("TextBoxColumnItemId").Value.ToString
            _menu.openChildForm(_menu.currentButton, New FormEditItem(_menu, EditMode.Edit, id))

        End If

    End Sub

#End Region

#Region "メソッド"

    Private Sub InitializeForm(Optional isLoading As Boolean = False)

        ComboBoxFilerItemType.Text = String.Empty
        TextBoxFilterItemCode.Text = String.Empty
        TextBoxFilterJanCode.Text = String.Empty
        TextBoxFilterItemName.Text = String.Empty
        ComboBoxFilterMeasureType.Text = String.Empty

        ' リストの初期化
        dataGridViewEditItems.DataBind()

        If Not isLoading Then ComboBoxFilerItemType.Focus()

    End Sub

    ' メニューの再表示ボタンから呼ばれる
    Public Sub RefreshView() Implements IRefreshable.RefreshView

        InitializeForm()

    End Sub

#End Region

End Class

Public Class DataGridViewEditItems
    Inherits CustomDataGridView

#Region "定義"

    Public ButtonColumnDetail As New CustomDatagridViewButtonColumn
    Public TextBoxColumnItemTypeName As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnItemCode As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnJanCode As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnItemName As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnMeasureType As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnCostPrice As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnOriginalSellingPrice As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnInstoreSellingPrice As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnItemId As New CustomDataGridViewTextBoxColumn

    Public Event SelectClick(sender As Object, e As DataGridViewCellEventArgs)

    Private dataView As New DataView
    Private itemTypeBackColors As New Dictionary(Of String, Color)

#End Region

#Region "コンストラクタ"

    Public Sub New()

        With ButtonColumnDetail
            .Name = "ButtonColumnDetail"
            .HeaderText = String.Empty
            .Width = 50
            .Text = "選択"
            .DefaultCellStyle.Font = New Font("MS UI Gothic", 11.25)
            .DefaultCellStyle.NullValue = "新規"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .UseColumnTextForButtonValue = True
            .TabStop = False
        End With
        With TextBoxColumnItemTypeName
            .Name = "TextBoxColumnItemTypeName"
            .DataPropertyName = "ItemTypeName"
            .HeaderText = "品目区分"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .Width = 80
        End With
        With TextBoxColumnItemCode
            .Name = "TextBoxColumnItemCode"
            .DataPropertyName = "ItemCode"
            .HeaderText = "品目コード"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            .Width = 90
        End With
        With TextBoxColumnJanCode
            .Name = "TextBoxColumnJanCode"
            .DataPropertyName = "JanCode"
            .HeaderText = "JANコード"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            .Width = 115
        End With
        With TextBoxColumnItemName
            .Name = "TextBoxColumnItemName"
            .DataPropertyName = "ItemName"
            .HeaderText = "品目名"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            .Width = 490
        End With
        With TextBoxColumnMeasureType
            .Name = "TextBoxColumnMeasureType"
            .DataPropertyName = "MeasureType"
            .HeaderText = "計量区分"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .Width = 80
        End With
        With TextBoxColumnCostPrice
            .Name = "TextBoxColumnCostPrice"
            .DataPropertyName = "CostPrice"
            .HeaderText = "原価"
            .Width = 100
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .DefaultCellStyle.Format = "#,##0"
        End With
        With TextBoxColumnOriginalSellingPrice
            .Name = "TextBoxColumnOriginalSellingPrice"
            .DataPropertyName = "OriginalSellingPrice"
            .HeaderText = "振出売価"
            .Width = 100
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .DefaultCellStyle.Format = "#,##0"
        End With
        With TextBoxColumnInstoreSellingPrice
            .Name = "TextBoxColumnInstoreSellingPrice"
            .DataPropertyName = "InstoreSellingPrice"
            .HeaderText = "店頭売価"
            .Width = 100
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .DefaultCellStyle.Format = "#,##0"
        End With
        With TextBoxColumnItemId
            .Name = "TextBoxColumnItemId"
            .DataPropertyName = "ItemId"
            .HeaderText = "品目ID"
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

            .Columns.Add(ButtonColumnDetail)
            .Columns.Add(TextBoxColumnItemTypeName)
            .Columns.Add(TextBoxColumnItemCode)
            .Columns.Add(TextBoxColumnJanCode)
            .Columns.Add(TextBoxColumnItemName)
            .Columns.Add(TextBoxColumnMeasureType)
            .Columns.Add(TextBoxColumnCostPrice)
            .Columns.Add(TextBoxColumnOriginalSellingPrice)
            .Columns.Add(TextBoxColumnInstoreSellingPrice)
            .Columns.Add(TextBoxColumnItemId)

            ' 動作関係のプロパティを変更
            .ReadOnly = True
            .AllowUserToOrderColumns = False
            .AllowUserToAddRows = True
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

        ' 行の背景色を辞書化
        For Each itemType As ItemType In ItemTypes.GetAll()
            itemTypeBackColors(itemType.ItemTypeName) = itemType.FormBackColor
        Next

    End Sub

#End Region

#Region "イベント"

    Private Sub Me_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles Me.CellFormatting

        If e.RowIndex < 0 Then Return

        If sharedIniFile.PaintItemTypeColor = 1 Then
            Dim itemTypeName As String = Convert.ToString(Me("TextBoxColumnItemTypeName", e.RowIndex).Value)
            Dim backColor As Color
            If TryGetItemTypeBackColor(itemTypeBackColors, itemTypeName, backColor) Then
                e.CellStyle.BackColor = backColor
            End If
        End If

        If Me.Columns(e.ColumnIndex) Is TextBoxColumnMeasureType Then
            If Not IsNothing(e.Value) AndAlso Not IsDBNull(e.Value) Then
                e.Value = Common.MeasureTypes(e.Value)
                e.FormattingApplied = True
            End If
        End If

    End Sub

    Private Sub Me_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles Me.CellContentClick

        Select Case Me.Columns(e.ColumnIndex).Name
            Case ButtonColumnDetail.Name
                RaiseEvent SelectClick(Me, e)
        End Select

    End Sub

    Private Sub Me_CellDoubleClick(sender As System.Object, e As System.Windows.Forms.DataGridViewCellEventArgs) Handles Me.CellDoubleClick

        ' 列ヘッダーは無視する
        If e.RowIndex < 0 Then Exit Sub

        If e.RowIndex <> Me.NewRowIndex Then
            RaiseEvent SelectClick(Me, e)
        End If

    End Sub

#End Region

#Region "メソッド"

    Public Sub DataBind()

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Try
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String = String.Empty
                    SQLString = "select"
                    SQLString &= " Items.ItemId"
                    SQLString &= ",Items.ItemCode"
                    SQLString &= ",Items.JanCode"
                    SQLString &= ",Items.ItemName"
                    SQLString &= ",Items.CostPrice"
                    SQLString &= ",case when ItemTypes.ProductFlag = 1"
                    SQLString &= " then Items.OriginalSellingPrice"
                    SQLString &= " else null"
                    SQLString &= " end as OriginalSellingPrice"
                    SQLString &= ",case when ItemTypes.ProductFlag = 1"
                    SQLString &= " then Items.InstoreSellingPrice"
                    SQLString &= " else null"
                    SQLString &= " end as InstoreSellingPrice"
                    SQLString &= ",case when ItemTypes.ProductFlag = 1"
                    SQLString &= " then Items.MeasureType"
                    SQLString &= " else null"
                    SQLString &= " end as MeasureType"
                    SQLString &= ",Items.ItemTypeId"
                    SQLString &= ",ItemTypes.ItemTypeName"
                    SQLString &= " from Items"
                    SQLString &= " left join ItemTypes on Items.ItemTypeId = ItemTypes.ItemTypeId"
                    SQLString &= " order by"
                    SQLString &= " ItemTypes.OrderNo"
                    SQLString &= ",Items.ItemCode"
                    SQLString &= ",Items.ItemName"
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    Using DataAdapter = New NpgsqlDataAdapter(cmd)
                        Using dataTable = New DataTable
                            DataAdapter.Fill(dataTable)
                            Me.dataView = dataTable.DefaultView
                            Me.DataSource = Me.dataView
                        End Using
                    End Using
                End Using
            Catch ex As Exception
                Throw New ApplicationException(ex.Message)
            Finally
                If cn IsNot Nothing Then
                    cn.Close()
                End If
            End Try
        End Using

    End Sub

    Public Sub Filter(itemTypeId As String, itemCode As String, janCode As String, itemName As String, measureType As String)

        Me.dataView.RowFilter = String.Empty

        If Not String.IsNullOrEmpty(itemTypeId) Then
            Me.dataView.RowFilter &= IIf(Not String.IsNullOrEmpty(Me.dataView.RowFilter), " and ", "") &
                                     "(ItemTypeId = " & itemTypeId & ")"
        End If
        If Not String.IsNullOrEmpty(itemCode) Then
            Me.dataView.RowFilter &= IIf(Not String.IsNullOrEmpty(Me.dataView.RowFilter), " and ", "") &
                                     "(ItemCode like '%" & itemCode & "%')"
        End If
        If Not String.IsNullOrEmpty(janCode) Then
            Me.dataView.RowFilter &= IIf(Not String.IsNullOrEmpty(Me.dataView.RowFilter), " and ", "") &
                                     "(JanCode like '%" & janCode & "%')"
        End If
        If Not String.IsNullOrEmpty(itemName) Then
            Me.dataView.RowFilter &= IIf(Not String.IsNullOrEmpty(Me.dataView.RowFilter), " and ", "") &
                                     "(ItemName like '%" & itemName & "%')"
        End If
        If Not String.IsNullOrEmpty(measureType) Then
            Me.dataView.RowFilter &= IIf(Not String.IsNullOrEmpty(Me.dataView.RowFilter), " and ", "") &
                                     "(MeasureType = " & measureType & ")"
        End If

        Me(ButtonColumnDetail.Name, Me.NewRowIndex).Value = "新規"

        ScrollToBottom()

    End Sub

    Public Function SelectRowById(id As String) As Boolean

        If String.IsNullOrEmpty(id) Then Return False

        For Each row As DataGridViewRow In Me.Rows
            If row.IsNewRow Then Continue For

            If Convert.ToString(row.Cells(TextBoxColumnItemId.Name).Value) = id Then
                Me.CurrentCell = row.Cells(ButtonColumnDetail.Name)
                row.Selected = True
                If Me.IsHandleCreated Then
                    Me.FirstDisplayedScrollingRowIndex = row.Index
                End If
                Return True
            End If
        Next

        Return False

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
