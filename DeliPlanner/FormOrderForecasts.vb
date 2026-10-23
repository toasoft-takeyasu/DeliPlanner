Imports Npgsql
Imports NpgsqlTypes

Public Class FormOrderForecasts
    Implements IRefreshable

#Region "定義"

    Private ReadOnly _menu As FormMenu
    Private WithEvents dataGridViewOrderForecasts As DataGridViewOrderForecasts

#End Region

#Region "コンストラクタ"

    Public Sub New(parent As Form)

        InitializeComponent()

        _menu = parent

    End Sub

#End Region

#Region "イベント"

    Private Sub Me_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        CheckBoxHalfProduct.Checked = True
        CheckBoxMaterial.Checked = True

        dataGridViewOrderForecasts = New DataGridViewOrderForecasts()
        PanelOrderForecasts.Controls.Add(dataGridViewOrderForecasts)

        ' フォームの初期化
        InitializeForm(True)

    End Sub

    Private Sub Me_Shown(sender As Object, e As System.EventArgs) Handles Me.Shown

        AddHandler DateBoxPlanDate.TextChanged, AddressOf DateBoxPlanDate_TextChanged
        AddHandler TextBoxItemId.TextChanged, AddressOf TextBoxItemId_TextChanged
        AddHandler CheckBoxHalfProduct.CheckedChanged, AddressOf CheckBoxItemType_CheckedChanged
        AddHandler CheckBoxMaterial.CheckedChanged, AddressOf CheckBoxItemType_CheckedChanged

    End Sub

    Private Sub DateBoxPlanDate_TextChanged(sender As Object, e As EventArgs)

        If IsDate(DateBoxPlanDate.Text) Then
            calc.ReloadInventories()
            RefreshGrid()
        End If

    End Sub

    Private Sub ButtonPlanDate_Click(sender As Object, e As EventArgs) Handles ButtonPlanDateNext.Click, ButtonPlanDatePrevious.Click

        If Not IsDate(DateBoxPlanDate.Text) Then
            DateBoxPlanDate.Text = Now.ToString("yyyy/MM/dd")
        Else
            If sender.Equals(ButtonPlanDateNext) Then
                DateBoxPlanDate.Text = CDate(DateBoxPlanDate.Text).AddDays(1)
            Else
                DateBoxPlanDate.Text = CDate(DateBoxPlanDate.Text).AddDays(-1)
            End If
        End If
        DateBoxPlanDate.Focus()

    End Sub

    Private Sub ButtonSearchItem_Click(sender As Object, e As EventArgs) Handles ButtonSearchItem.Click

        Dim dialog As New DialogSearchItems
        With dialog
            .ItemTypeFilter = "RecipeNeedFlag = 1"

            Dim targetBounds As Rectangle = PanelMain.RectangleToScreen(PanelMain.ClientRectangle)
            .StartPosition = FormStartPosition.Manual
            .Height = targetBounds.Height
            .Location = New Point(targetBounds.Right - .Width, targetBounds.Bottom - .Height)

            If .ShowDialog = DialogResult.OK Then
                TextBoxItemId.Text = .Item.ItemId
                TextBoxItemName.Text = .Item.ItemName
            End If
        End With

    End Sub

    Private Sub ButtonClearItem_Click(sender As Object, e As EventArgs) Handles ButtonClearItem.Click

        TextBoxItemId.Text = String.Empty
        TextBoxItemName.Text = String.Empty

    End Sub

    Private Sub CheckBoxItemType_CheckedChanged(sender As Object, e As EventArgs)

        ' 両方外すと空になるので、最後の1つは外せないようにする
        If Not CheckBoxHalfProduct.Checked AndAlso Not CheckBoxMaterial.Checked Then
            DirectCast(sender, CheckBox).Checked = True
            Exit Sub
        End If

        RefreshGrid()

    End Sub

    Private Sub TextBoxItemId_TextChanged(sender As Object, e As EventArgs)

        calc.ReloadInventories()
        RefreshGrid()

    End Sub

    Public Sub dataGridViewOrderForecasts_OrderClick(sender As Object, e As DataGridViewCellEventArgs) Handles dataGridViewOrderForecasts.OrderClick

        Dim dgv As DataGridViewOrderForecasts = dataGridViewOrderForecasts

        Dim dialog As New DialogOrder
        With dialog
            dialog.Item = Items.GetById(dgv(dgv.TextBoxColumnItemId.Name, e.RowIndex).Value)
            dialog.BaseControl = PanelMain

            Dim targetBounds As Rectangle = PanelMain.RectangleToScreen(PanelMain.ClientRectangle)
            .StartPosition = FormStartPosition.Manual
            .Location = New Point(targetBounds.Right - .Width, targetBounds.Top)

            If .ShowDialog = DialogResult.OK Then
                ' リストの初期化
                calc.ReloadInventories()
                RefreshGrid()
            End If
        End With

    End Sub

    Public Sub dataGridViewOrderForecasts_PlanClick(sender As Object, e As DataGridViewCellEventArgs) Handles dataGridViewOrderForecasts.PlanClick

        Dim dgv As DataGridViewOrderForecasts = dataGridViewOrderForecasts

        Dim dialog As New DialogPlan
        With dialog
            dialog.Item = Items.GetById(dgv(dgv.TextBoxColumnItemId.Name, e.RowIndex).Value)
            dialog.BaseControl = PanelMain

            ' 未計画数量を数量の初期値にする
            If Not String.IsNullOrEmpty(dgv("TextBoxColumnQuantity" & DataGridViewOrderForecasts.QuantityTypeToday, e.RowIndex).Value) Then
                .InitialQuantity = dgv("TextBoxColumnQuantity" & DataGridViewOrderForecasts.QuantityTypeToday, e.RowIndex).Value
                .InitialUnitTypeName = dgv("textBoxColumnUnitTypeName" & DataGridViewOrderForecasts.QuantityTypeToday, e.RowIndex).Value
            End If

            Dim targetBounds As Rectangle = PanelMain.RectangleToScreen(PanelMain.ClientRectangle)
            .StartPosition = FormStartPosition.Manual
            .Location = New Point(targetBounds.Right - .Width, targetBounds.Top)

            If .ShowDialog = DialogResult.OK Then
                ' リストの初期化
                calc.ReloadInventories()
                RefreshGrid()
            End If
        End With

    End Sub

#End Region

#Region "メソッド"

    Private Sub InitializeForm(Optional isLoading As Boolean = False)

        DateBoxPlanDate.Text = Now.ToString("yyyy/MM/dd")
        TextBoxItemId.Text = String.Empty
        TextBoxItemName.Text = String.Empty

        ' リストの初期化
        calc.ReloadInventories()
        RefreshGrid()

        DateBoxPlanDate.Focus()

    End Sub

    ' 画面の条件でグリッドを再表示する
    Private Sub RefreshGrid()

        dataGridViewOrderForecasts.DataBind(DateBoxPlanDate.Text, TextBoxItemId.Text, CheckBoxHalfProduct.Checked, CheckBoxMaterial.Checked)

    End Sub

    ' メニューの再表示ボタンから呼ばれる
    Public Sub RefreshView() Implements IRefreshable.RefreshView

        InitializeForm()

    End Sub

#End Region

End Class

Public Class DataGridViewOrderForecasts
    Inherits CustomDataGridView

#Region "定義"

    Public ButtonColumnOrderOrPlan As New CustomDatagridViewButtonColumn
    Public TextBoxColumnItemTypeName As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnItemName As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnQuantity() As CustomDataGridViewTextBoxColumn
    Public textBoxColumnUnitTypeName() As CustomDataGridViewTextBoxColumn
    Public TextBoxColumnItemId As New CustomDataGridViewTextBoxColumn

    Public Event OrderClick(sender As Object, e As DataGridViewCellEventArgs)
    Public Event PlanClick(sender As Object, e As DataGridViewCellEventArgs)

    Public isDataBinding As Boolean
    Private comboBoxEditingControl As New CustomDataGridViewComboBoxEditingControl

    Private quantityTypeList As New Dictionary(Of Integer, String)
    Public Const QuantityTypeToday As Integer = 1      ' 当日必要量
    Public Const QuantityTypeIncoming As Integer = 7   ' 入荷予定
    Public Const QuantityTypeShortage As Integer = 8   ' 過不足
    Private itemTypeBackColors As New Dictionary(Of String, Color)

#End Region

#Region "コンストラクタ"

    Public Sub New()

        With ButtonColumnOrderOrPlan
            .Name = "ButtonColumnOrderOrPlan"
            .HeaderText = String.Empty
            .Width = 50
            .DefaultCellStyle.Font = New Font("MS UI Gothic", 11.25)
            .DefaultCellStyle.NullValue = String.Empty
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .UseColumnTextForButtonValue = False
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .TabStop = True
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
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            .ReadOnly = True
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .TabStop = False
        End With

        ' (数量 + 単位)列を作成
        quantityTypeList.Add(1, "当日" & vbLf & "必要量")
        quantityTypeList.Add(2, "未計画")
        quantityTypeList.Add(3, "計画済")
        quantityTypeList.Add(4, "現在庫")
        quantityTypeList.Add(5, "製造後" & vbLf & "在庫")
        quantityTypeList.Add(6, "予定" & vbLf & "必要量")
        quantityTypeList.Add(7, "入庫予定")
        quantityTypeList.Add(8, "過不足")

        ReDim TextBoxColumnQuantity(quantityTypeList.Count - 1)
        ReDim textBoxColumnUnitTypeName(quantityTypeList.Count - 1)
        Dim i As Integer = 0
        For Each pair As KeyValuePair(Of Integer, String) In quantityTypeList.OrderBy(Function(x) x.Key)
            TextBoxColumnQuantity(i) = New CustomDataGridViewTextBoxColumn
            With TextBoxColumnQuantity(i)
                .Name = "TextBoxColumnQuantity" & pair.Key
                .DataPropertyName = "Quantity" & pair.Key
                .HeaderText = String.Empty '見出しは結合ヘッダーで描くので空
                .Width = 100
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                .DefaultCellStyle.SelectionForeColor = Color.Black
                .DefaultCellStyle.SelectionBackColor = Color.White
                .DefaultCellStyle.Format = "#,##0.##"
                .ReadOnly = True
                .SortMode = DataGridViewColumnSortMode.NotSortable
                .TabStop = False
            End With
            textBoxColumnUnitTypeName(i) = New CustomDataGridViewTextBoxColumn
            With textBoxColumnUnitTypeName(i)
                .Name = "TextBoxColumnUnitTypeName" & pair.Key
                .DataPropertyName = "UnitTypeName" & pair.Key
                .HeaderText = String.Empty '見出しは結合ヘッダーで描くので空
                .Width = 60
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
                .DefaultCellStyle.SelectionForeColor = Color.Black
                .DefaultCellStyle.SelectionBackColor = Color.White
                .ReadOnly = True
                .SortMode = DataGridViewColumnSortMode.NotSortable
                .TabStop = False
            End With
            i += 1
        Next

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
            .ColumnHeadersHeight = 42          ' 2段(結合ヘッダーはこの高さいっぱいに描く)
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

            .Columns.Add(ButtonColumnOrderOrPlan)
            .Columns.Add(TextBoxColumnItemTypeName)
            .Columns.Add(TextBoxColumnItemName)
            For i = 0 To quantityTypeList.Count - 1
                .Columns.Add(TextBoxColumnQuantity(i))
                .Columns.Add(textBoxColumnUnitTypeName(i))
            Next
            .Columns.Add(TextBoxColumnItemId)

            ' 動作関係のプロパティを変更
            .ReadOnly = False
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
            .ResizeRedraw = True 'リサイズ時に全体を再描画(結合ヘッダーの崩れ防止)
        End With

        ' 結合ヘッダーのグループを定義(合計 + 各店舗)
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
            Case ButtonColumnOrderOrPlan.Name
                If Me(Me.ButtonColumnOrderOrPlan.Name, e.RowIndex).Value = "計画" Then
                    RaiseEvent PlanClick(sender, e)
                Else
                    RaiseEvent OrderClick(sender, e)
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

        ' 発注が必要な行(過不足マイナスかつ入荷予定なし)は文字を赤にする
        If IsOrderNeeded(e.RowIndex) Then
            e.CellStyle.ForeColor = Color.Red
            If Me(e.ColumnIndex, e.RowIndex).ReadOnly Then
                e.CellStyle.SelectionForeColor = Color.Red
            End If
        End If

    End Sub

    Private Sub Me_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles Me.CellValidating
    End Sub

    Private Sub Me_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles Me.CellValueChanged
    End Sub

    Private Sub Me_EditingControlShowing(sender As Object, e As DataGridViewEditingControlShowingEventArgs) Handles Me.EditingControlShowing
    End Sub

    Private Sub dataGridViewComboBox_SelectedIndexChanged(sender As Object, e As EventArgs)
    End Sub

#End Region

#Region "メソッド"

    Public Sub DataBind(planDate As Date, Optional itemId As String = Nothing,
                        Optional showHalfProduct As Boolean = True, Optional showMaterial As Boolean = True)

        RemoveHandler Me.CellValidating, AddressOf Me_CellValidating

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Try
                isDataBinding = True

                Me.Rows.Clear()

                Dim todayProducts As New List(Of MaterialCalculator.Product)
                Dim futureProducts As New List(Of MaterialCalculator.Product)

                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String = String.Empty
                    SQLString = "select"
                    SQLString &= " PlanQuantities.ItemId as ItemId"
                    SQLString &= ",sum(PlanQuantities.Quantity1) as TodayQuantity"
                    SQLString &= ",sum(PlanQuantities.Quantity2) as FutureQuantity"
                    SQLString &= ",PlanQuantities.UnitTypeId as UnitTypeId"
                    SQLString &= " from ("

                    ' 当日の製造計画
                    SQLString &= " select"
                    SQLString &= " Plans.ItemId"
                    SQLString &= ",sum(PlanDetails.Quantity) as Quantity1"
                    SQLString &= ",0::double precision as Quantity2"
                    SQLString &= ",Plans.UnitTypeId"
                    SQLString &= " from Plans"
                    SQLString &= " left join PlanDetails on Plans.PlanId = PlanDetails.PlanId"
                    SQLString &= " where Plans.PlanDate = @PlanDate"
                    If Not String.IsNullOrEmpty(itemId) Then
                        SQLString &= " and Plans.ItemId = @ItemId"
                    End If
                    SQLString &= " group by"
                    SQLString &= " Plans.ItemId"
                    SQLString &= ",Plans.UnitTypeId"

                    SQLString &= " union"

                    ' 翌日以降の製造計画
                    SQLString &= " select"
                    SQLString &= " Plans.ItemId"
                    SQLString &= ",0::double precision as Quantity1"
                    SQLString &= ",sum(PlanDetails.Quantity) as Quantity2"
                    SQLString &= ",Plans.UnitTypeId"
                    SQLString &= " from Plans"
                    SQLString &= " left join PlanDetails on Plans.PlanId = PlanDetails.PlanId"
                    SQLString &= " where Plans.PlanDate > @PlanDate"
                    If Not String.IsNullOrEmpty(itemId) Then
                        SQLString &= " and Plans.ItemId = @ItemId"
                    End If
                    SQLString &= " group by"
                    SQLString &= " Plans.ItemId"
                    SQLString &= ",Plans.UnitTypeId"

                    SQLString &= " ) as PlanQuantities"
                    SQLString &= " group by"
                    SQLString &= " PlanQuantities.ItemId"
                    SQLString &= ",PlanQuantities.UnitTypeId"
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    cmd.Parameters.AddWithValue("@PlanDate", NpgsqlDbType.Date, planDate.Date)
                    If Not String.IsNullOrEmpty(itemId) Then
                        cmd.Parameters.AddWithValue("@ItemId", NpgsqlDbType.Integer, CInt(itemId))
                    End If
                    cmd.Parameters.AddWithValue("@PlanDate", NpgsqlDbType.Date, planDate.Date)
                    If Not String.IsNullOrEmpty(itemId) Then
                        cmd.Parameters.AddWithValue("@ItemId", NpgsqlDbType.Integer, CInt(itemId))
                    End If
                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        If dr.HasRows Then
                            While dr.Read()
                                Dim currentItemId As Integer = CInt(dr("ItemId"))
                                Dim unitTypeId As Integer = CInt(Nz(dr("UnitTypeId"), 0))
                                Dim todayQuantity As Double = CDbl(Nz(dr("TodayQuantity"), 0))
                                Dim futureQuantity As Double = CDbl(Nz(dr("FutureQuantity"), 0))

                                If todayQuantity <> 0 Then todayProducts.Add(New MaterialCalculator.Product(currentItemId, todayQuantity, unitTypeId))
                                If futureQuantity <> 0 Then futureProducts.Add(New MaterialCalculator.Product(currentItemId, futureQuantity, unitTypeId))
                            End While
                        End If
                    End Using
                End Using

                ' 入荷予定(発注単位のまま。伝票単位でしか突合できないので分納は考慮しない)
                Dim incomingQuantity As New Dictionary(Of Integer, Double)
                Dim incomingUnitId As New Dictionary(Of Integer, Integer)

                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String = String.Empty
                    SQLString = "select"
                    SQLString &= " OrderDetails.ItemId as ItemId"
                    SQLString &= ",OrderDetails.UnitTypeId as UnitTypeId"
                    SQLString &= ",sum(OrderDetails.Quantity) as Quantity"
                    SQLString &= " from Orders"
                    SQLString &= " inner join OrderDetails on Orders.OrderId = OrderDetails.OrderId"
                    SQLString &= " where Orders.ArrivalPlanDate >= @PlanDate"
                    SQLString &= " and Orders.OrderId not in ("
                    SQLString &= " select Arrivals.OrderId"
                    SQLString &= " from Arrivals"
                    SQLString &= " where Arrivals.OrderId is not null"
                    SQLString &= " )"
                    SQLString &= " group by"
                    SQLString &= " OrderDetails.ItemId"
                    SQLString &= ",OrderDetails.UnitTypeId"
                    SQLString &= " order by"
                    SQLString &= " OrderDetails.ItemId"
                    SQLString &= ",sum(OrderDetails.Quantity) desc"
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    cmd.Parameters.AddWithValue("@PlanDate", NpgsqlDbType.Date, planDate.Date)
                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        While dr.Read()
                            Dim orderItemId As Integer = CInt(dr("ItemId"))
                            Dim quantity As Double = CDbl(Nz(dr("Quantity"), 0))
                            If quantity = 0 Then Continue While

                            If incomingQuantity.ContainsKey(orderItemId) Then
                                ' 単位が混在している場合は数量の多い単位を代表にして合算しない
                                Continue While
                            End If

                            incomingQuantity(orderItemId) = quantity
                            incomingUnitId(orderItemId) = CInt(Nz(dr("UnitTypeId"), 0))
                        End While
                    End Using
                End Using

                ' 当日は在庫を差し引く。予定は差し引かない(在庫を二重に使わないため)
                Dim todayReqs As List(Of MaterialCalculator.Requirement) =
                    calc.CalcNetRequirements(todayProducts, True)
                Dim futureReqs As List(Of MaterialCalculator.Requirement) =
                    calc.CalcNetRequirements(futureProducts, False)

                ' 品目IDだけで突き合わせる(単位は品目自身の単位に統一されている)
                Dim meta As New Dictionary(Of Integer, MaterialCalculator.Requirement)
                Dim today As New Dictionary(Of Integer, Double)
                Dim future As New Dictionary(Of Integer, Double)
                Dim futurePlanned As New Dictionary(Of Integer, Double)
                Dim planned As New Dictionary(Of Integer, Double)
                Dim production As New Dictionary(Of Integer, Double)

                For Each r In todayReqs
                    ' 下位に展開した品目のうち、誰からも要求されていないもの(=計画そのもの)は表示しない
                    If r.Exploded AndAlso r.GrossQuantity = 0 Then Continue For
                    meta(r.ItemId) = r
                    today(r.ItemId) = r.GrossQuantity
                    planned(r.ItemId) = r.PlanQuantity
                    production(r.ItemId) = r.ProductionQuantity
                Next
                For Each r In futureReqs
                    ' 明日以降の製造計画は入庫予定として使うので、表示対象外の行でも拾っておく
                    If r.PlanQuantity <> 0 Then futurePlanned(r.ItemId) = r.PlanQuantity

                    If r.Exploded AndAlso r.GrossQuantity = 0 Then Continue For
                    If Not meta.ContainsKey(r.ItemId) Then meta(r.ItemId) = r
                    future(r.ItemId) = r.GrossQuantity
                Next

                ' 当日必要量を持つ行のキー集合(かたまり判定用)
                Dim hasTodaySet As New HashSet(Of Integer)
                For Each kv In today
                    If kv.Value <> 0 Then hasTodaySet.Add(kv.Key)
                Next

                ' 品目区分マスタの並び順を辞書化する
                Dim itemTypeOrders As New Dictionary(Of String, Integer)
                Dim itemTypeOrder As Integer = 0
                For Each itemType As ItemType In ItemTypes.GetAll() _
                        .OrderBy(Function(x) CInt(Nz(x.OrderNo, 0))) _
                        .ThenBy(Function(x) CInt(Nz(x.ItemTypeId, 0))).ToList()
                    itemTypeOrder += 1
                    itemTypeOrders(Convert.ToString(itemType.ItemTypeId)) = itemTypeOrder
                Next

                ' 当日必要量あり(0)→なし(1) の順、各かたまり内は 品目区分マスタの並び順→品目ID 順
                Dim sorted = meta.Values _
                    .OrderBy(Function(x) If(hasTodaySet.Contains(x.ItemId), 0, 1)) _
                    .ThenBy(Function(x) GetItemTypeOrder(itemTypeOrders, x.ItemTypeId)) _
                    .ThenBy(Function(x) x.ItemId) _
                    .ToList()

                For Each m In sorted
                    ' 表示する品目区分をチェックボックスで絞り込む
                    If calc.NeedsRecipe(m.ItemId) Then
                        If Not showHalfProduct Then Continue For
                    Else
                        If Not showMaterial Then Continue For
                    End If

                    Dim todayQuantity As Double = If(today.ContainsKey(m.ItemId), today(m.ItemId), 0)
                    Dim futureQuantity As Double = If(future.ContainsKey(m.ItemId), future(m.ItemId), 0)
                    Dim planQuantity As Double = If(planned.ContainsKey(m.ItemId), planned(m.ItemId), 0)
                    ' 未計画 = 作るべき量 − すでに登録済みの計画数量
                    Dim productionQuantity As Double = If(production.ContainsKey(m.ItemId), production(m.ItemId), 0)
                    Dim unplannedQuantity As Double = productionQuantity - planQuantity
                    If unplannedQuantity < 0 Then unplannedQuantity = 0

                    ' 発注品目は「計画」の対象ではないので未計画を出さない(過不足で見る)
                    If Not calc.NeedsRecipe(m.ItemId) Then unplannedQuantity = 0

                    ' すべて基準単位(g / ml / 個)に揃える
                    Dim baseUnitId As Integer = calc.GetBaseUnitTypeId(m.ItemId)
                    Dim baseUnitName As String = calc.GetBaseUnitTypeName(m.ItemId)

                    Dim todayBase As Double = 0
                    Dim futureBase As Double = 0
                    Dim planBase As Double = 0
                    Dim unplannedBase As Double = 0
                    Dim canConvert As Boolean = calc.TryToBaseQuantity(m.ItemId, todayQuantity, m.UnitTypeId, todayBase)
                    If canConvert Then
                        calc.TryToBaseQuantity(m.ItemId, futureQuantity, m.UnitTypeId, futureBase)
                        calc.TryToBaseQuantity(m.ItemId, planQuantity, m.UnitTypeId, planBase)
                        calc.TryToBaseQuantity(m.ItemId, unplannedQuantity, m.UnitTypeId, unplannedBase)
                    End If

                    Dim inventoryBase As Double = 0
                    Dim afterInventory As Double = 0
                    Dim shortage As Double = 0

                    If canConvert Then
                        ' 現在庫は Inventories.Weight の合計(在庫調整・棚卸画面と同じ基準)
                        inventoryBase = calc.GetInventoryWeight(m.ItemId)
                        ' 製造後在庫 = 現在庫 − 当日必要量 + 当日の製造計画(これから作る分)
                        afterInventory = inventoryBase - todayBase + planBase
                        shortage = afterInventory - futureBase           ' 過不足数
                    Else
                        ' 換算できない品目は必要量だけ元の単位で表示する
                        todayBase = todayQuantity
                        futureBase = futureQuantity
                        planBase = planQuantity
                        unplannedBase = unplannedQuantity
                        baseUnitId = m.UnitTypeId
                        baseUnitName = m.UnitTypeName
                    End If

                    Dim column As New List(Of Object)
                    column.Add(If(calc.NeedsRecipe(m.ItemId), "計画", "発注"))
                    column.Add(m.ItemTypeName)
                    column.Add(m.ItemName)
                    column.Add(If(todayBase = 0, "", todayBase))                                    '当日必要量
                    column.Add(If(todayBase = 0, "", baseUnitName))
                    column.Add(If(unplannedBase = 0, "", unplannedBase))                            '未計画 
                    column.Add(If(unplannedBase = 0, "", baseUnitName))
                    column.Add(If(planBase = 0, "", planBase))                                      '計画済
                    column.Add(If(planBase = 0, "", baseUnitName))
                    column.Add(If(Not canConvert, "", inventoryBase))                               '現在庫
                    column.Add(If(Not canConvert, "", baseUnitName))
                    column.Add(If(Not canConvert OrElse afterInventory = 0, "", afterInventory))    '製造後在庫
                    column.Add(If(Not canConvert OrElse afterInventory = 0, "", baseUnitName))
                    column.Add(If(futureBase = 0, "", futureBase))                                  '予定必要量
                    column.Add(If(futureBase = 0, "", baseUnitName))

                    ' 入庫予定(発注品目は発注単位のまま、製造品目は基準単位に換算する)
                    Dim incoming As Double = 0
                    Dim incomingUnitName As String = String.Empty

                    If calc.NeedsRecipe(m.ItemId) Then
                        Dim futurePlanQuantity As Double = If(futurePlanned.ContainsKey(m.ItemId), futurePlanned(m.ItemId), 0)
                        If futurePlanQuantity <> 0 Then
                            If canConvert Then
                                calc.TryToBaseQuantity(m.ItemId, futurePlanQuantity, m.UnitTypeId, incoming)
                            Else
                                incoming = futurePlanQuantity
                            End If
                            incomingUnitName = baseUnitName
                        End If
                    Else
                        If incomingQuantity.ContainsKey(m.ItemId) Then
                            incoming = incomingQuantity(m.ItemId)
                            incomingUnitName = calc.GetUnitTypeName(incomingUnitId(m.ItemId))
                        End If
                    End If

                    column.Add(If(incoming = 0, "", incoming))
                    column.Add(If(incoming = 0, "", incomingUnitName))

                    column.Add(If(Not canConvert OrElse shortage = 0, "", shortage))                '過不足
                    column.Add(If(Not canConvert OrElse shortage = 0, "", baseUnitName))
                    column.Add(m.ItemId)
                    Dim rowIndex As Integer = Me.Rows.Add(column.ToArray())

                    Dim quantityFormat As String = calc.GetQuantityFormat(baseUnitId)
                    For Each pair In quantityTypeList
                        Me("TextBoxColumnQuantity" & pair.Key, rowIndex).Style.Format = quantityFormat
                    Next
                    ' 発注単位は上位単位(箱・ケース)なので小数を出さない
                    If Not calc.NeedsRecipe(m.ItemId) Then
                        Me("TextBoxColumnQuantity" & QuantityTypeIncoming, rowIndex).Style.Format = "#,##0.##"
                    End If
                Next
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
        Dim keys() As Integer = quantityTypeList.Keys.OrderBy(Function(x) x).ToArray()
        For i As Integer = 0 To keys.Length - 1
            AddMergedHeader(TextBoxColumnQuantity(i), textBoxColumnUnitTypeName(i), quantityTypeList(keys(i)))
        Next

    End Sub

    ' 品目区分の並び順を返す(マスタに無い区分は最後に回す)
    Private Function GetItemTypeOrder(itemTypeOrders As Dictionary(Of String, Integer), itemTypeId As Object) As Integer

        Dim key As String = Convert.ToString(itemTypeId)
        If itemTypeOrders.ContainsKey(key) Then Return itemTypeOrders(key)
        Return Integer.MaxValue

    End Function

    ' 発注が必要な行か(過不足がマイナスで、かつ入荷予定が無い)
    Private Function IsOrderNeeded(rowIndex As Integer) As Boolean

        ' レシピを持つ品目は製造して補うので発注対象外
        Dim itemId As Integer
        If Integer.TryParse(Convert.ToString(Me(TextBoxColumnItemId.Name, rowIndex).Value), itemId) Then
            If calc.NeedsRecipe(itemId) Then Return False
        End If

        ' 過不足(空欄は0または換算不可なので対象外)
        Dim shortage As Double
        If Not TryGetQuantity(rowIndex, QuantityTypeShortage, shortage) Then Return False
        If shortage >= 0 Then Return False

        ' 入荷予定(空欄なら発注されていない = 発注が必要)
        Dim incoming As Double
        If Not TryGetQuantity(rowIndex, QuantityTypeIncoming, incoming) Then Return True

        Return incoming = 0

    End Function

    ' 数量列の値を取り出す(空欄なら False)
    Private Function TryGetQuantity(rowIndex As Integer, quantityType As Integer, ByRef quantity As Double) As Boolean

        quantity = 0

        Dim value As Object = Me("TextBoxColumnQuantity" & quantityType, rowIndex).Value
        If value Is Nothing OrElse IsDBNull(value) Then Return False

        ' 書式適用後の "-1,600" も拾えるように NumberStyles.Any を指定する
        Return Double.TryParse(Convert.ToString(value),
                               Globalization.NumberStyles.Any,
                               Globalization.CultureInfo.CurrentCulture,
                               quantity)

    End Function

#End Region

End Class