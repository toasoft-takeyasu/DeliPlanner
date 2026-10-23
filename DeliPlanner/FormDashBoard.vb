Imports Npgsql
Imports NpgsqlTypes

Public Class FormDashboard
    Implements IRefreshable

    '賞味期限アラートの日数
    Private Const EXPIRY_ALERT_DAYS As Integer = 3

    Private ReadOnly _menu As FormMenu

    Private WithEvents tilePlans As New KpiTile
    Private WithEvents tileArrivals As New KpiTile
    Private WithEvents tileMinus As New KpiTile
    Private WithEvents tileExpiry As New KpiTile
    Private WithEvents dataGrdViewPlans As New DataGridViewDashboardPlans
    Private WithEvents dataGridViewExpiries As New DataGridViewDashboardExpiries
    Private WithEvents timerAutoRefresh As New Timer

    Public Sub New(menu As FormMenu)

        InitializeComponent()
        _menu = menu
        InitializeLayout()

    End Sub

#Region "レイアウト"

    Private Sub InitializeLayout()

        tilePlans.Title = "本日の製造"
        tilePlans.AccentColor = Color.FromArgb(2, 112, 56)
        tilePlans.BackColor = Color.Honeydew
        tileArrivals.Title = "本日入荷予定"
        tileArrivals.AccentColor = Color.FromArgb(246, 130, 0)
        tileArrivals.BackColor = Color.FloralWhite
        tileMinus.Title = "在庫マイナス"
        tileMinus.AccentColor = Color.FromArgb(0, 102, 204)
        tileMinus.BackColor = Color.AliceBlue
        tileExpiry.Title = "賞味期限が近い在庫"
        tileExpiry.AccentColor = Color.FromArgb(236, 70, 82)
        tileExpiry.BackColor = Color.LavenderBlush

        For Each tile As KpiTile In New KpiTile() {tilePlans, tileArrivals, tileMinus, tileExpiry}
            tile.Dock = DockStyle.Fill
            tableTiles.Controls.Add(tile)
        Next

        TableMain.Controls.Add(CreateSection("本日の製造", dataGrdViewPlans), 0, 0)
        TableMain.Controls.Add(CreateSection(String.Format("賞味期限が近い在庫（{0}日以内）", EXPIRY_ALERT_DAYS), dataGridViewExpiries), 1, 0)

        timerAutoRefresh.Interval = 60000

    End Sub

    '見出し付きの枠を作る
    Private Function CreateSection(caption As String, grid As Control) As Control

        Dim panel As New Panel
        panel.Dock = DockStyle.Fill
        panel.Margin = New Padding(6)
        panel.BackColor = Color.White
        panel.Padding = New Padding(1)

        Dim label As New Label
        label.Text = caption
        label.Dock = DockStyle.Top
        label.Height = 32
        label.TextAlign = ContentAlignment.MiddleLeft
        label.Padding = New Padding(10, 0, 0, 0)
        label.Font = New Font("MS UI Gothic", 12, FontStyle.Bold)
        label.BackColor = Color.FromArgb(238, 240, 243)

        grid.Dock = DockStyle.Fill

        panel.Controls.Add(grid)
        panel.Controls.Add(label)

        Return panel

    End Function

#End Region

#Region "読み込み"

    Private Sub Me_Load(sender As Object, e As EventArgs) Handles Me.Load

        LoadAll()
        timerAutoRefresh.Start()

    End Sub

    Private Sub Me_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed

        timerAutoRefresh.Stop()

    End Sub

    Private Sub timerAutoRefresh_Tick(sender As Object, e As EventArgs) Handles timerAutoRefresh.Tick

        If Not Me.Visible Then Return
        LoadAll()

    End Sub

    'メニューの再表示ボタンから呼ばれる
    Public Sub RefreshView() Implements IRefreshable.RefreshView

        LoadAll()

    End Sub

    Private Sub LoadAll()

        Try
            Dim baseDate As Date = Date.Today
            calc.ReloadInventories()

            '一覧
            dataGrdViewPlans.DataBind(calc, baseDate)
            dataGridViewExpiries.DataBind(calc, EXPIRY_ALERT_DAYS)

            'タイル1 本日の製造
            tilePlans.Value = String.Format("{0} / {1}", dataGrdViewPlans.FinishedCount, dataGrdViewPlans.TotalCount)
            tilePlans.Unit = "件 完了"
            If dataGrdViewPlans.TotalCount = 0 Then
                tilePlans.SubText = "本日の製造計画はありません"
                tilePlans.Alert = False
            Else
                tilePlans.SubText = String.Format("未着手 {0} 件", dataGrdViewPlans.NotStartedCount)
                tilePlans.Alert = False
            End If

            'タイル2 本日入荷予定
            Dim supplierNameList As List(Of String) = LoadArrivalPlanSuppliers(baseDate)
            tilePlans.Refresh()
            tileArrivals.Value = supplierNameList.Count.ToString("#,##0")
            tileArrivals.Unit = "伝票"
            tileArrivals.SubText = SummarizeNames(supplierNameList, "予定なし")
            tileArrivals.Alert = False

            'タイル3 在庫マイナス
            Dim minusItemNameList As List(Of String) = LoadMinusInventoryItems()
            tileMinus.Value = minusItemNameList.Count.ToString("#,##0")
            tileMinus.Unit = "品目"
            tileMinus.SubText = SummarizeNames(minusItemNameList, "なし")
            tileMinus.Alert = minusItemNameList.Count > 0

            'タイル4 賞味期限が近い在庫
            tileExpiry.Value = dataGridViewExpiries.AlertCount.ToString("#,##0")
            tileExpiry.Unit = "ロット"
            If dataGridViewExpiries.ExpiredCount > 0 Then
                tileExpiry.SubText = String.Format("うち賞味期限切れ {0} 件", dataGridViewExpiries.ExpiredCount)
            Else
                tileExpiry.SubText = "賞味期限切れはありません"
            End If
            tileExpiry.Alert = dataGridViewExpiries.ExpiredCount > 0

            labelUpdated.Text = Date.Now.ToString("yyyy/MM/dd(ddd) HH:mm 現在")
            Me.PerformLayout()

        Catch ex As Exception
            SysMsgBox(ex.Message, Me.Text, "LoadAll")
        End Try

    End Sub

    '本日入荷予定（入荷予定日が本日で、まだ入荷が登録されていない発注）
    Private Function LoadArrivalPlanSuppliers(baseDate As Date) As List(Of String)

        Dim result As New List(Of String)
        Dim SQLString As String

        SQLString = "select"
        SQLString &= " Suppliers.SupplierName"
        SQLString &= " from Orders"
        SQLString &= " left join Suppliers on Orders.SupplierId = Suppliers.SupplierId"
        SQLString &= " where Orders.ArrivalPlanDate = @ArrivalPlanDate"
        SQLString &= " and Orders.OrderId not in (select OrderId from Arrivals where OrderId is not null)"
        SQLString &= " order by Suppliers.SupplierName"

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Using cmd As New NpgsqlCommand(SQLString, cn)
                cmd.Parameters.AddWithValue("@ArrivalPlanDate", NpgsqlDbType.Date, baseDate.Date)
                Using dr As NpgsqlDataReader = cmd.ExecuteReader
                    Do While dr.Read
                        result.Add(Nz(dr("SupplierName"), "（仕入先未設定）"))
                    Loop
                End Using
            End Using
        End Using

        Return result

    End Function

    '在庫がマイナスになっている品目（在庫ロットに紐づかない払い出しが残っている品目）
    Private Function LoadMinusInventoryItems() As List(Of String)

        Dim result As New List(Of String)
        Dim SQLString As String

        SQLString = "select"
        SQLString &= " Items.ItemName"
        SQLString &= " from Items"
        SQLString &= " inner join ("
        SQLString &= " select t.ItemId, sum(t.Weight) as TotalWeight from ("
        SQLString &= " select ItemId, Weight from Inventories where Weight > 0"
        SQLString &= " union all"
        SQLString &= " select ItemId, -Quantity as Weight from InstructionConsumptions where InventoryId is null"
        SQLString &= " ) as t"
        SQLString &= " group by t.ItemId"
        SQLString &= " having sum(t.Weight) < 0"
        SQLString &= " ) as m on Items.ItemId = m.ItemId"
        SQLString &= " order by Items.ItemName"

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Using cmd As New NpgsqlCommand(SQLString, cn)
                Using dr As NpgsqlDataReader = cmd.ExecuteReader
                    Do While dr.Read
                        result.Add(Nz(dr("ItemName"), ""))
                    Loop
                End Using
            End Using
        End Using

        Return result

    End Function

    '「にんじん ほか」の形にする
    Private Function SummarizeNames(nameList As List(Of String), emptyText As String) As String

        If nameList.Count = 0 Then Return emptyText
        If nameList.Count = 1 Then Return nameList(0)
        Return String.Format("{0} ほか{1}件", nameList(0), nameList.Count - 1)

    End Function

#End Region

#Region "画面遷移"

    'メニューのボタンを押したことにする（画面切替のルールを二重に持たないため）
    Private Sub tilePlans_Click(sender As Object, e As EventArgs) Handles tilePlans.Click

        _menu.ButtonInstructions.PerformClick()

    End Sub

    Private Sub tileArrivals_Click(sender As Object, e As EventArgs) Handles tileArrivals.Click

        _menu.ButtonArrivals.PerformClick()

    End Sub

    Private Sub tileMinus_Click(sender As Object, e As EventArgs) Handles tileMinus.Click

        _menu.ButtonOrderForecasts.PerformClick()

    End Sub

    Private Sub tileExpiry_Click(sender As Object, e As EventArgs) Handles tileExpiry.Click

        _menu.ButtonAjustInventory.PerformClick()

    End Sub

    Private Sub dataGridViewPlans_RowOpen(planDate As Date) Handles dataGrdViewPlans.RowOpen

        _menu.ButtonInstructionPlans.PerformClick()

    End Sub

    Private Sub dataGridViewExpiries_AdjustClick(inventoryId As Integer) Handles dataGridViewExpiries.AdjustClick

        _menu.ButtonAjustInventory.PerformClick()

    End Sub

#End Region

End Class

'ダッシュボードの数字タイル
Public Class KpiTile
    Inherits Panel

    Private ReadOnly _labelTitle As New Label
    Private ReadOnly _labelValue As New Label
    Private ReadOnly _labelUnit As New Label
    Private ReadOnly _labelSub As New Label

    Private _accentColor As Color = Color.FromArgb(2, 112, 56)
    Private _isHovered As Boolean = False
    Private _alert As Boolean = False

    Public Property Title As String
        Get
            Return _labelTitle.Text
        End Get
        Set(value As String)
            _labelTitle.Text = value
        End Set
    End Property

    Public Property Value As String
        Get
            Return _labelValue.Text
        End Get
        Set(value As String)
            _labelValue.Text = value
            LayoutUnit()
        End Set
    End Property

    Public Property Unit As String
        Get
            Return _labelUnit.Text
        End Get
        Set(value As String)
            _labelUnit.Text = value
            LayoutUnit()
        End Set
    End Property

    Public Property SubText As String
        Get
            Return _labelSub.Text
        End Get
        Set(value As String)
            _labelSub.Text = value
        End Set
    End Property

    Public Property AccentColor As Color
        Get
            Return _accentColor
        End Get
        Set(value As Color)
            _accentColor = value
            Me.Invalidate(True)
        End Set
    End Property

    Public Sub New()

        Me.BackColor = Color.White
        Me.Margin = New Padding(6)
        Me.Cursor = Cursors.Hand
        Me.DoubleBuffered = True

        _labelTitle.AutoSize = True
        _labelTitle.Location = New Point(25, 10)
        _labelTitle.Font = New Font("MS UI Gothic", 12)
        _labelTitle.ForeColor = Color.DimGray
        _labelTitle.BackColor = Color.Transparent

        _labelValue.AutoSize = True
        _labelValue.Location = New Point(23, 30)
        _labelValue.Font = New Font("MS UI Gothic", 24, FontStyle.Bold)
        _labelValue.ForeColor = Color.FromArgb(43, 48, 56)
        _labelValue.BackColor = Color.Transparent

        _labelUnit.AutoSize = True
        _labelUnit.Font = New Font("MS UI Gothic", 12)
        _labelUnit.ForeColor = Color.DimGray
        _labelUnit.BackColor = Color.Transparent

        _labelSub.AutoSize = True
        _labelSub.Location = New Point(25, 76)
        _labelSub.Font = New Font("MS UI Gothic", 12)
        _labelSub.ForeColor = Color.Gray
        _labelSub.BackColor = Color.Transparent

        Me.Controls.Add(_labelTitle)
        Me.Controls.Add(_labelValue)
        Me.Controls.Add(_labelUnit)
        Me.Controls.Add(_labelSub)

        HookMouse(Me)

    End Sub

    '注意を引きたいときは数字を赤にする
    Public Property Alert As Boolean
        Get
            Return _alert
        End Get
        Set(value As Boolean)
            _alert = value
            If _alert Then
                _labelValue.ForeColor = Color.FromArgb(198, 46, 58)
            Else
                _labelValue.ForeColor = Color.FromArgb(43, 48, 56)
            End If
        End Set
    End Property

    'ホバー状態の切り替えはここだけ
    Private Sub SetHover(value As Boolean)

        If _isHovered = value Then Return
        _isHovered = value
        Me.Invalidate(True)

    End Sub

    '2色を混ぜる（ratio が 1 に近いほど color2 寄り）
    Private Function Blend(color1 As Color, color2 As Color, ratio As Double) As Color

        Dim r As Integer = CInt(color1.R + (CInt(color2.R) - color1.R) * ratio)
        Dim g As Integer = CInt(color1.G + (CInt(color2.G) - color1.G) * ratio)
        Dim b As Integer = CInt(color1.B + (CInt(color2.B) - color1.B) * ratio)
        Return Color.FromArgb(r, g, b)

    End Function

    Private Sub LayoutUnit()

        _labelUnit.Left = _labelValue.Left + _labelValue.PreferredWidth + 2
        _labelUnit.Top = _labelValue.Top + _labelValue.PreferredHeight - _labelUnit.PreferredHeight - 6

    End Sub

    '子コントロールがクリックを食べてしまうので、すべてに配線する
    Private Sub HookMouse(target As Control)

        If Not target Is Me Then
            AddHandler target.Click, AddressOf Child_Click
        End If
        AddHandler target.MouseEnter, AddressOf Child_MouseEnter
        AddHandler target.MouseLeave, AddressOf Child_MouseLeave

        For Each child As Control In target.Controls
            HookMouse(child)
        Next

    End Sub

    Private Sub Child_Click(sender As Object, e As EventArgs)
        MyBase.OnClick(e)
    End Sub

    Private Sub Child_MouseEnter(sender As Object, e As EventArgs)
        SetHover(True)
    End Sub

    Private Sub Child_MouseLeave(sender As Object, e As EventArgs)
        'まだタイルの中（子コントロールの上を含む）にいるなら維持する
        SetHover(Me.ClientRectangle.Contains(Me.PointToClient(Cursor.Position)))
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)

        MyBase.OnPaint(e)

        If _isHovered Then
            Using brush As New SolidBrush(Blend(Me.BackColor, _accentColor, 0.12))
                e.Graphics.FillRectangle(brush, Me.ClientRectangle)
            End Using
        End If

        Using pen As New Pen(Color.FromArgb(220, 223, 228))
            e.Graphics.DrawRectangle(pen, 0, 0, Me.Width - 1, Me.Height - 1)
        End Using
        Using brush As New SolidBrush(_accentColor)
            e.Graphics.FillRectangle(brush, 1, 1, 8, Me.Height - 2)
        End Using

    End Sub

    Protected Overrides Sub OnResize(e As EventArgs)

        MyBase.OnResize(e)
        Me.Invalidate()

    End Sub

    'クリックで画面が切り替わると MouseLeave が来ないことがある
    Protected Overrides Sub OnClick(e As EventArgs)
        MyBase.OnClick(e)
        SetHover(False)
    End Sub

    '別画面に切り替わって隠れたときも解除する
    Protected Overrides Sub OnVisibleChanged(e As EventArgs)
        MyBase.OnVisibleChanged(e)
        If Not Me.Visible Then SetHover(False)
    End Sub

End Class

Public Class DataGridViewDashboardPlans
    Inherits CustomDataGridView

#Region "定義"

    Public Event RowOpen(planDate As Date)

    Public TextBoxColumnItemTypeName As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnItemName As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnQuantity As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnUnitTypeName As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnStatus As New CustomDataGridViewTextBoxColumn

    Public ReadOnly Property TotalCount As Integer
    Public ReadOnly Property FinishedCount As Integer
    Public ReadOnly Property NotStartedCount As Integer

    Private itemTypeBackColors As New Dictionary(Of String, Color)

#End Region

#Region "コンストラクタ"

    Public Sub New()

        With TextBoxColumnItemTypeName
            .Name = "TextBoxColumnItemTypeName"
            .DataPropertyName = "ItemTypeName"
            .HeaderText = "品目区分"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .Width = 80
            If sharedIniFile.VisivleItemType = 0 Then
                .Visible = False
            End If
        End With
        With TextBoxColumnItemName
            .Name = "TextBoxColumnItemName"
            .DataPropertyName = "ItemName"
            .HeaderText = "品目名"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        End With
        With TextBoxColumnQuantity
            .Name = "TextBoxColumnQuantity"
            .DataPropertyName = "Quantity"
            .HeaderText = "数量"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .Width = 80
        End With
        With TextBoxColumnUnitTypeName
            .Name = "TextBoxColumnUnitTypeName"
            .DataPropertyName = "UnitTypeName"
            .HeaderText = "単位"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            .Width = 50
        End With
        With TextBoxColumnStatus
            .Name = "TextBoxColumnStatus"
            .DataPropertyName = "Status"
            .HeaderText = "状態"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .Width = 80
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

            .Columns.Add(TextBoxColumnItemTypeName)
            .Columns.Add(TextBoxColumnItemName)
            .Columns.Add(TextBoxColumnQuantity)
            .Columns.Add(TextBoxColumnUnitTypeName)
            .Columns.Add(TextBoxColumnStatus)

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

        ' 結合ヘッダーのグループを定義
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

    Private Sub Me_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles Me.CellFormatting

        If e.RowIndex < 0 Then Return

        If sharedIniFile.PaintItemTypeColor = 1 Then
            Dim itemTypeName As String = Convert.ToString(Me("TextBoxColumnItemTypeName", e.RowIndex).Value)
            Dim backColor As Color
            If TryGetItemTypeBackColor(itemTypeBackColors, itemTypeName, backColor) Then
                e.CellStyle.BackColor = backColor
            End If
        End If

    End Sub

    Private Sub Me_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles Me.CellDoubleClick

        If e.RowIndex < 0 Then Return
        RaiseEvent RowOpen(Date.Today)

    End Sub

#End Region

#Region "メソッド"

    Public Sub DataBind(calc As MaterialCalculator, planDate As Date)

        Me.Rows.Clear()
        _TotalCount = 0
        _FinishedCount = 0
        _NotStartedCount = 0

        Dim SQLString As String

        SQLString = "select"
        SQLString &= " Plans.PlanId"
        SQLString &= ",Plans.ItemId"
        SQLString &= ",Items.ItemName"
        SQLString &= ",ItemTypes.ItemTypeName"
        SQLString &= ",Plans.UnitTypeId"
        SQLString &= ",PlanSums.SumQuantity"
        SQLString &= ",Instructions.InstructionId"
        SQLString &= ",Instructions.FinishDate"
        SQLString &= " from Plans"
        SQLString &= " inner join Items on Plans.ItemId = Items.ItemId"
        SQLString &= " inner join ItemTypes on Items.ItemTypeId = ItemTypes.ItemTypeId"
        SQLString &= " left join ("
        SQLString &= " select PlanId, sum(Quantity) as SumQuantity"
        SQLString &= " from PlanDetails"
        SQLString &= " group by PlanId"
        SQLString &= " ) as PlanSums on Plans.PlanId = PlanSums.PlanId"
        SQLString &= " left join Instructions on Plans.PlanId = Instructions.PlanId"
        SQLString &= " where Plans.PlanDate = @PlanDate"
        SQLString &= " order by ItemTypes.OrderNo asc nulls last"
        SQLString &= ",Plans.ItemId"
        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Using cmd As New NpgsqlCommand(SQLString, cn)
                cmd.Parameters.AddWithValue("@PlanDate", NpgsqlDbType.Date, planDate.Date)
                Using dr As NpgsqlDataReader = cmd.ExecuteReader
                    Do While dr.Read

                        Dim itemId As Integer = CInt(Nz(dr("ItemId"), 0))
                        Dim quantity As Double = CDbl(Nz(dr("SumQuantity"), 0))
                        Dim unitTypeId As Integer = CInt(Nz(dr("UnitTypeId"), 0))

                        Dim status As Integer
                        If IsDBNull(dr("InstructionId")) Then
                            status = 0                                  '未製造
                        ElseIf IsDBNull(dr("FinishDate")) Then
                            status = 1                                  '製造中
                        Else
                            status = 2                                  '完成
                        End If

                        Dim index As Integer = Me.Rows.Add(
                            Nz(dr("ItemTypeName"), ""),
                            Nz(dr("ItemName"), ""),
                            quantity.ToString(QuantityFormat(quantity)),
                            calc.GetUnitTypeName(unitTypeId),
                            StatusName(status))

                        Me.Rows(index).Tag = itemId
                        ApplyStatusColor(Me.Rows(index), status)

                        _TotalCount += 1
                        If status = 2 Then _FinishedCount += 1
                        If status = 0 Then _NotStartedCount += 1
                    Loop
                End Using
            End Using
        End Using

        Me.ClearSelection()

    End Sub

    Private Function StatusName(status As Integer) As String

        Select Case status
            Case 1
                Return "製造中"
            Case 2
                Return "完成"
            Case Else
                Return "未製造"
        End Select

    End Function

    Private Sub ApplyStatusColor(row As DataGridViewRow, status As Integer)

        Dim cell As DataGridViewCell = row.Cells(TextBoxColumnStatus.Name)

        If sharedIniFile.PaintItemTypeColor = 1 Then
            Select Case status
                Case 1
                    cell.Style.ForeColor = Color.DarkOrange
                Case 2
                    cell.Style.ForeColor = Color.DarkGreen
                Case Else
                    cell.Style.ForeColor = Color.Red
            End Select
        Else
            Select Case status
                Case 1
                    cell.Style.BackColor = Color.Yellow
                    cell.Style.ForeColor = Color.Black
                Case 2
                    cell.Style.BackColor = Color.Lime
                    cell.Style.ForeColor = Color.Black
                Case Else
                    cell.Style.BackColor = Color.Firebrick
                    cell.Style.ForeColor = Color.White
            End Select
        End If

    End Sub

    Private Function QuantityFormat(value As Double) As String

        If value = Math.Truncate(value) Then Return "#,##0"
        Return "#,##0.###"

    End Function

    Private Sub BuildMergedHeaders()

        ClearMergedHeaders()
        AddMergedHeader(TextBoxColumnQuantity, TextBoxColumnUnitTypeName, "数量")

    End Sub

#End Region

End Class

Public Class DataGridViewDashboardExpiries
    Inherits CustomDataGridView

#Region "定義"

    Public Event AdjustClick(inventoryId As Integer)

    Public TextBoxColumnItemTypeName As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnExpiryDate As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnRemain As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnItemName As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnWeight As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnUnitTypeName As New CustomDataGridViewTextBoxColumn
    Public ButtonColumnAdjust As New CustomDatagridViewButtonColumn

    Public ReadOnly Property AlertCount As Integer
    Public ReadOnly Property ExpiredCount As Integer

    Private itemTypeBackColors As New Dictionary(Of String, Color)

#End Region

#Region "コンストラクタ"

    Public Sub New()

        With TextBoxColumnItemTypeName
            .Name = "TextBoxColumnItemTypeName"
            .DataPropertyName = "ItemTypeName"
            .HeaderText = "品目区分"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .Width = 80
            If sharedIniFile.VisivleItemType = 0 Then
                .Visible = False
            End If
        End With
        With TextBoxColumnExpiryDate
            .Name = "TextBoxColumnExpiryDate"
            .DataPropertyName = "ExpiryDate"
            .HeaderText = "賞味期限"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .Width = 100
        End With
        With TextBoxColumnRemain
            .Name = "TextBoxColumnRemain"
            .DataPropertyName = "Remain"
            .HeaderText = "残り日数"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .Width = 80
        End With
        With TextBoxColumnItemName
            .Name = "TextBoxColumnItemName"
            .DataPropertyName = "ItemName"
            .HeaderText = "品目"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        End With
        With TextBoxColumnWeight
            .Name = "TextBoxColumnWeight"
            .DataPropertyName = "Weight"
            .HeaderText = "在庫"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .Width = 100
        End With
        With TextBoxColumnUnitTypeName
            .Name = "TextBoxColumnUnitTypeName"
            .DataPropertyName = "UnitTypeName"
            .HeaderText = "単位"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            .Width = 50
        End With
        With ButtonColumnAdjust
            .Name = "ButtonColumnAdjust"
            .HeaderText = ""
            .Text = "調整"
            .UseColumnTextForButtonValue = True
            .Width = 50
            .DefaultCellStyle.Font = New Font("MS UI Gothic", 11.25)
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

            .Columns.Add(TextBoxColumnItemTypeName)
            .Columns.Add(TextBoxColumnExpiryDate)
            .Columns.Add(TextBoxColumnRemain)
            .Columns.Add(TextBoxColumnItemName)
            .Columns.Add(TextBoxColumnWeight)
            .Columns.Add(TextBoxColumnUnitTypeName)
            .Columns.Add(ButtonColumnAdjust)

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

        ' 結合ヘッダーのグループを定義
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

    Private Sub Me_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles Me.CellFormatting

        If e.RowIndex < 0 Then Return

        If sharedIniFile.PaintItemTypeColor = 1 Then
            Dim itemTypeName As String = Convert.ToString(Me("TextBoxColumnItemTypeName", e.RowIndex).Value)
            Dim backColor As Color
            If TryGetItemTypeBackColor(itemTypeBackColors, itemTypeName, backColor) Then
                e.CellStyle.BackColor = backColor
            End If
        End If

    End Sub

    Private Sub Me_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles Me.CellContentClick

        If e.RowIndex < 0 Then Return
        If Me.Columns(e.ColumnIndex).Name <> ButtonColumnAdjust.Name Then Return
        RaiseEvent AdjustClick(CInt(Nz(Me.Rows(e.RowIndex).Tag, 0)))

    End Sub

#End Region

#Region "メソッド"

    Public Sub DataBind(calc As MaterialCalculator, alertDays As Integer)

        Me.Rows.Clear()
        _AlertCount = 0
        _ExpiredCount = 0

        Dim SQLString As String

        SQLString = "select"
        SQLString &= " Inventories.InventoryId"
        SQLString &= ",Inventories.ItemId"
        SQLString &= ",ItemTypes.ItemTypeName"
        SQLString &= ",Items.ItemName"
        SQLString &= ",Inventories.ExpiryDate"
        SQLString &= ",Inventories.Weight"
        SQLString &= " from Inventories"
        SQLString &= " inner join Items on Inventories.ItemId = Items.ItemId"
        SQLString &= " left join ItemTypes on Items.ItemTypeId = ItemTypes.ItemTypeId"
        SQLString &= " where Inventories.Weight > 0"
        SQLString &= " and Inventories.ExpiryDate is not null"
        SQLString &= " and Inventories.ExpiryDate <= @LimitDate"
        SQLString &= " order by Inventories.ExpiryDate"
        SQLString &= ",Inventories.ItemId"
        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Using cmd As New NpgsqlCommand(SQLString, cn)
                cmd.Parameters.AddWithValue("@LimitDate", NpgsqlDbType.Date, Date.Today.AddDays(alertDays).Date)
                Using dr As NpgsqlDataReader = cmd.ExecuteReader
                    Do While dr.Read

                        Dim inventoryId As Integer = CInt(Nz(dr("InventoryId"), 0))
                        Dim itemId As Integer = CInt(Nz(dr("ItemId"), 0))
                        Dim expiryDate As Date = CDate(dr("ExpiryDate"))
                        Dim weight As Double = CDbl(Nz(dr("Weight"), 0))
                        Dim remainDays As Integer = expiryDate.Subtract(Date.Today).Days

                        Dim rowIndex As Integer = Me.Rows.Add(
                            Nz(dr("ItemTypeName"), ""),
                            expiryDate.ToString("yyyy/MM/dd"),
                            RemainName(remainDays),
                            Nz(dr("ItemName"), ""),
                            weight.ToString(QuantityFormat(weight)),
                            calc.GetBaseUnitTypeName(itemId),
                            "調整")

                        Me.Rows(rowIndex).Tag = inventoryId
                        ApplyRemainColor(Me.Rows(rowIndex), remainDays)

                        _AlertCount += 1
                        If remainDays < 0 Then _ExpiredCount += 1
                    Loop
                End Using
            End Using
        End Using

        Me.ClearSelection()

    End Sub

    Private Function RemainName(remainDays As Integer) As String

        If remainDays < 0 Then Return "期限切れ"
        If remainDays = 0 Then Return "本日"
        Return String.Format("あと{0}日", remainDays)

    End Function

    Private Sub ApplyRemainColor(row As DataGridViewRow, remainDays As Integer)

        If remainDays < 0 Then
            row.DefaultCellStyle.ForeColor = Color.Red
        ElseIf remainDays <= 1 Then
            row.DefaultCellStyle.ForeColor = Color.DarkOrange
        End If

    End Sub

    Private Function QuantityFormat(value As Double) As String

        If value = Math.Truncate(value) Then Return "#,##0"
        Return "#,##0.###"

    End Function

    Private Sub BuildMergedHeaders()

        ClearMergedHeaders()
        AddMergedHeader(TextBoxColumnWeight, TextBoxColumnUnitTypeName, "在庫数量")

    End Sub

#End Region

End Class