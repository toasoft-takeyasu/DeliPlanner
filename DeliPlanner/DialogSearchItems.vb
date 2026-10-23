Imports Npgsql

Public Class DialogSearchItems

#Region "定義"

    Public Item As New Item
    Public ItemTypeFilter As String
    Public InitialSelectItemType As New ItemType

    Private ItemTypeList As New List(Of ItemType)
    Private WithEvents dataGridViewSearchItems As DataGridViewSearchItems
    Private Class RadioButtonItemTypeBase
        Inherits RadioButton

        Public Sub New()
            With Me
                .Appearance = Appearance.Button
                .ForeColor = SystemColors.ControlText
                .BackColor = SystemColors.Control
                .Font = New Font("MS UI Gothic", 12)
                .TextAlign = ContentAlignment.MiddleCenter
                .Size = New Size(70, 40)
            End With
        End Sub

    End Class

#End Region

#Region "イベント"

    Private Sub Me_Load(sender As Object, e As EventArgs) Handles Me.Load

        ItemTypeList = ItemTypes.GetAll(ItemTypeFilter)
        Dim itemTypeAll As New ItemType
        itemTypeAll.ItemTypeName = "すべて"
        itemTypeAll.OrderNo = 0
        itemTypeAll.ButtonBackColor = SystemColors.ButtonShadow
        ItemTypeList.Add(itemTypeAll)
        ItemTypeList = ItemTypeList.OrderBy(Function(x) x.OrderNo).ToList()

        ' 品目区分ボタンを件数に応じて複数段で表示
        CreateItemTypeButtons()

        dataGridViewSearchItems = New DataGridViewSearchItems()
        PanelItems.Controls.Add(dataGridViewSearchItems)

        ' フォームの初期化
        InitializeForm(True)

    End Sub

    Private Sub Me_Shown(sender As Object, e As System.EventArgs) Handles Me.Shown

        AddHandler TextBoxFilterItemName.TextChanged, AddressOf TextBoxFilter_TextChanged

    End Sub

    Private Sub RadioButtonItemType_CheckedChanged(sender As Object, e As EventArgs)

        For Each c As Control In PanelItemType.Controls
            c.ForeColor = SystemColors.ControlText
            c.BackColor = SystemColors.Control
        Next

        Dim itemType As ItemType = CType(sender.tag, ItemType)
        TextBoxItemTypeId.Text = itemType.ItemTypeId
        Me.BackColor = itemType.FormBackColor
        CType(sender, RadioButton).ForeColor = itemType.ButtonForeColor
        CType(sender, RadioButton).BackColor = itemType.ButtonBackColor

        dataGridViewSearchItems.Filter(TextBoxItemTypeId.Text, TextBoxFilterItemName.Text)

    End Sub

    Private Sub TextBoxFilter_TextChanged(sender As System.Object, e As System.EventArgs)

        dataGridViewSearchItems.Filter(TextBoxItemTypeId.Text, TextBoxFilterItemName.Text)

    End Sub

    Private Sub dataGridViewSearchItems_SelectClick(sender As Object, e As DataGridViewCellEventArgs) Handles dataGridViewSearchItems.SelectClick

        Dim dgv As DataGridViewSearchItems = dataGridViewSearchItems

        Item.ItemId = CInt(dgv(dgv.TextBoxColumnItemId.Name, e.RowIndex).Value)
        Item.ItemName = dgv(dgv.TextBoxColumnItemName.Name, e.RowIndex).Value
        Item.UnitType.UnitTypeId = dgv(dgv.TextBoxColumnUnitTypeId.Name, e.RowIndex).Value
        Item.ItemTypeId = dgv(dgv.TextBoxColumnItemTypeId.Name, e.RowIndex).Value
        Item.PieceUnitType.UnitTypeId = Nz(dgv(dgv.TextBoxColumnPieceUnitTypeId.Name, e.RowIndex).Value, Nothing)
        Item.PieceWeight = Nz(dgv(dgv.TextBoxColumnPieceWeight.Name, e.RowIndex).Value, Nothing)

        Me.DialogResult = DialogResult.OK
        Me.Close()

    End Sub

#End Region

#Region "メソッド"

    Private Sub InitializeForm(Optional isLoading As Boolean = False)

        TextBoxFilterItemName.Text = String.Empty

        ' リストの初期化
        dataGridViewSearchItems.DataBind(ItemTypeFilter)

        ' 品目タイプの選択
        If InitialSelectItemType IsNot Nothing Then
            For Each c As Control In PanelItemType.Controls
                Dim itemType As ItemType = CType(c.Tag, ItemType)
                If InitialSelectItemType.ItemTypeId = itemType.ItemTypeId Then
                    CType(c, RadioButton).Checked = True
                    Exit For
                End If
            Next
        Else
            CType(PanelItemType.Controls(0), RadioButton).Checked = True
        End If

    End Sub

    Private Sub CreateItemTypeButtons()

        Const buttonWidth As Integer = 80
        Const buttonHeight As Integer = 40

        PanelItemType.Controls.Clear()

        ' パネル内に横並びできるボタン数
        Dim columnCount As Integer = Math.Max(1, PanelItemType.ClientSize.Width \ buttonWidth)
        ' 必要な段数
        Dim rowCount As Integer = CInt(Math.Ceiling(ItemTypeList.Count / CDbl(columnCount)))
        ' 変更前のパネル下端
        Dim oldPanelBottom As Integer = PanelItemType.Bottom

        For i As Integer = 0 To ItemTypeList.Count - 1
            Dim itemType As ItemType = ItemTypeList(i)
            Dim columnIndex As Integer = i Mod columnCount
            Dim rowIndex As Integer = i \ columnCount
            Dim radioButtonItemType As New RadioButtonItemTypeBase
            With radioButtonItemType
                .Width = buttonWidth
                .Height = buttonHeight
                .Left = columnIndex * buttonWidth
                .Top = rowIndex * buttonHeight
                .Tag = itemType
                .Text = itemType.ItemTypeName
            End With
            PanelItemType.Controls.Add(radioButtonItemType)
            AddHandler radioButtonItemType.CheckedChanged, AddressOf RadioButtonItemType_CheckedChanged
        Next

        ' 必要な段数に合わせてパネルを広げる
        PanelItemType.Height = rowCount * buttonHeight

        ' パネルが下へ広がった量
        Dim offsetY As Integer = PanelItemType.Bottom - oldPanelBottom

        If offsetY > 0 Then
            ' 検索欄を下へ移動
            TextBoxItemTypeId.Top += offsetY
            TextBoxFilterItemName.Top += offsetY
            ' 一覧の上端を下げ、下端位置は変えない
            PanelItems.Top += offsetY
            PanelItems.Height -= offsetY
        End If

    End Sub

#End Region

End Class

Public Class DataGridViewSearchItems
    Inherits CustomDataGridView

#Region "定義"

    Public ButtonColumnDetail As New CustomDatagridViewButtonColumn
    Public TextBoxColumnItemName As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnUnitTypeId As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnItemId As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnItemTypeId As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnPieceUnitTypeId As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnPieceWeight As New CustomDataGridViewTextBoxColumn

    Public Event SelectClick( sender As Object,  e As DataGridViewCellEventArgs)

    Private dataView As New DataView

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
        With TextBoxColumnItemName
            .Name = "TextBoxColumnItemName"
            .DataPropertyName = "ItemName"
            .HeaderText = "品目名"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            .Width = 500
        End With
        With TextBoxColumnUnitTypeId
            .Name = "TextBoxColumnUnitTypeId"
            .DataPropertyName = "UnitTypeId"
            .Visible = False
        End With
        With TextBoxColumnItemId
            .Name = "TextBoxColumnItemId"
            .DataPropertyName = "ItemId"
            .Visible = False
        End With
        With TextBoxColumnItemTypeId
            .Name = "TextBoxColumnItemTypeId"
            .DataPropertyName = "ItemTypeId"
            .Visible = False
        End With
        With TextBoxColumnPieceUnitTypeId
            .Name = "TextBoxColumnPieceUnitTypeId"
            .DataPropertyName = "PieceUnitTypeId"
            .Visible = False
        End With
        With TextBoxColumnPieceWeight
            .Name = "TextBoxColumnPieceWeight"
            .DataPropertyName = "PieceWeight"
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
            .Columns.Add(TextBoxColumnItemName)
            .Columns.Add(TextBoxColumnUnitTypeId)
            .Columns.Add(TextBoxColumnItemId)
            .Columns.Add(TextBoxColumnItemTypeId)
            .Columns.Add(TextBoxColumnPieceUnitTypeId)
            .Columns.Add(TextBoxColumnPieceWeight)

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

#Region "イベント"

    Private Sub Me_CellContentClick( sender As Object,  e As DataGridViewCellEventArgs) Handles Me.CellContentClick

        Select Case Me.Columns(e.ColumnIndex).Name
            Case ButtonColumnDetail.Name
                RaiseEvent SelectClick(Me, e)
        End Select

    End Sub

    Private Sub Me_CellDoubleClick( sender As System.Object,  e As System.Windows.Forms.DataGridViewCellEventArgs) Handles Me.CellDoubleClick

        ' 列ヘッダーは無視する
        If e.RowIndex < 0 Then Exit Sub

        If e.RowIndex <> Me.NewRowIndex Then
            RaiseEvent SelectClick(Me, e)
        End If

    End Sub

#End Region

#Region "メソッド"

    Public Sub DataBind(Optional itemTypeFilter As String = Nothing)

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Try
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String = String.Empty
                    SQLString = "select"
                    SQLString &= " Items.ItemId"
                    SQLString &= ",Items.ItemCode"
                    SQLString &= ",Items.ItemName"
                    SQLString &= ",Items.ItemTypeId"
                    SQLString &= ",ItemTypes.ItemTypeName"
                    SQLString &= ",Items.UnitTypeId"
                    SQLString &= ",Items.PieceUnitTypeId"
                    SQLString &= ",Items.PieceWeight"
                    SQLString &= " from Items"
                    SQLString &= " left join ItemTypes on Items.ItemTypeId = ItemTypes.ItemTypeId"
                    If Not String.IsNullOrEmpty(itemTypeFilter) Then
                        SQLString &= " where (" & itemTypeFilter & ")"
                    End If
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

    Public Sub Filter( itemTypeId As String,  itemName As String)

        Me.dataView.RowFilter = String.Empty

        If Not String.IsNullOrEmpty(itemTypeId) Then
            Me.dataView.RowFilter &= IIf(Not String.IsNullOrEmpty(Me.dataView.RowFilter), " and ", "") &
                                     "(ItemTypeId = " & itemTypeId & ")"
        End If
        If Not String.IsNullOrEmpty(itemName) Then
            Me.dataView.RowFilter &= IIf(Not String.IsNullOrEmpty(Me.dataView.RowFilter), " and ", "") &
                                     "(ItemName like '%" & itemName & "%')"
        End If

    End Sub

    Public Sub Clear()

        If Me.dataView.Table IsNot Nothing Then
            Me.dataView.Table.Clear()
        Else
            Me.Rows.Clear()
        End If

    End Sub

#End Region

End Class