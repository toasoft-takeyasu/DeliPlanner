Imports Npgsql
Imports NpgsqlTypes

Public Class FormArrivals
    Implements IRefreshable

#Region "定義"

    Private ReadOnly _menu As FormMenu
    Private WithEvents dataGridViewArrivals As DataGridViewArrivals
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

        dataGridViewArrivals = New DataGridViewArrivals()
        PanelStores.Controls.Add(dataGridViewArrivals)

        ' フォームの初期化
        InitializeForm(True)

    End Sub

    Private Sub Me_Shown(sender As Object, e As System.EventArgs) Handles Me.Shown

        AddHandler DateBoxFilterOrderDate.TextChanged, AddressOf TextBoxFilter_TextChanged
        AddHandler DateBoxFilterArrivalDate.TextChanged, AddressOf TextBoxFilter_TextChanged
        AddHandler TextBoxFilterSupplierName.TextChanged, AddressOf TextBoxFilter_TextChanged
        AddHandler CheckBoxNotArrived.CheckedChanged, AddressOf TextBoxFilter_TextChanged

        TextBoxFilter_TextChanged(Nothing, Nothing)

        ' 編集から戻ってきた場合はその行へ。無ければ最下行へ。
        If Not String.IsNullOrEmpty(_selectedId) AndAlso dataGridViewArrivals.SelectRowById(_selectedId) Then
            ' 位置付け成功(何もしない)
        Else
            dataGridViewArrivals.ScrollToBottom()
        End If

    End Sub

    Private Sub DateBoxArrivalDate_Validated(sender As Object, e As EventArgs) Handles _
        DateBoxArrivalDateFrom.Validated, DateBoxArrivalDateTo.Validated

        dataGridViewArrivals.DataBind(DateBoxArrivalDateFrom.Text, DateBoxArrivalDateTo.Text)

    End Sub

    Private Sub TextBoxFilter_TextChanged(sender As System.Object, e As System.EventArgs)

        dataGridViewArrivals.Filter(DateBoxFilterOrderDate.Text, DateBoxFilterArrivalDate.Text, TextBoxFilterSupplierName.Text, CheckBoxNotArrived.Checked)

    End Sub

    Private Sub dataGridViewOrders_SelectClick(sender As Object, e As DataGridViewCellEventArgs) Handles dataGridViewArrivals.SelectClick

        If e.RowIndex = dataGridViewArrivals.NewRowIndex Then
            _menu.openChildForm(_menu.currentButton, New FormArrival(_menu, EditMode.Add))
        Else
            Dim id As String = dataGridViewArrivals.Rows(e.RowIndex).Cells("TextBoxColumnArrivalId").Value.ToString
            Dim orderId As String = dataGridViewArrivals.Rows(e.RowIndex).Cells("TextBoxColumnOrderId").Value.ToString
            If String.IsNullOrEmpty(id) Then
                _menu.openChildForm(_menu.currentButton, New FormArrival(_menu, EditMode.Add, Nothing, orderId))
            Else
                _menu.openChildForm(_menu.currentButton, New FormArrival(_menu, EditMode.Edit, id, orderId))
            End If
        End If
    End Sub

#End Region

#Region "メソッド"

    Private Sub InitializeForm(Optional isLoading As Boolean = False)

        DateBoxArrivalDateFrom.Text = Now.Date.AddMonths(-1).ToString("yyyy/MM/dd")
        DateBoxArrivalDateTo.Text = Now.Date.ToString("yyyy/MM/dd")

        DateBoxFilterOrderDate.Text = String.Empty
        DateBoxFilterArrivalDate.Text = String.Empty
        TextBoxFilterSupplierName.Text = String.Empty
        CheckBoxNotArrived.Checked = True

        ' リストの初期化
        dataGridViewArrivals.DataBind(DateBoxArrivalDateFrom.Text, DateBoxArrivalDateTo.Text)

        dataGridViewArrivals.Focus()

    End Sub

    ' メニューの再表示ボタンから呼ばれる
    Public Sub RefreshView() Implements IRefreshable.RefreshView

        InitializeForm()

    End Sub

#End Region

End Class

Public Class DataGridViewArrivals
    Inherits CustomDataGridView

#Region "定義"

    Public ButtonColumnDetail As New CustomDatagridViewButtonColumn
    Public TextBoxColumnOrderDate As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnArrivalDate As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnSupplierName As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnArrivalId As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnOrderId As New CustomDataGridViewTextBoxColumn

    Public Event SelectClick(sender As Object, e As DataGridViewCellEventArgs)

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
        With TextBoxColumnOrderDate
            .Name = "TextBoxColumnOrderDate"
            .DataPropertyName = "OrderDate"
            .HeaderText = "発注日"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .Width = 99
        End With
        With TextBoxColumnArrivalDate
            .Name = "TextBoxColumnArrivalDate"
            .DataPropertyName = "ArrivalDate"
            .HeaderText = "入荷日"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .Width = 99
        End With
        With TextBoxColumnSupplierName
            .Name = "TextBoxColumnSupplierName"
            .DataPropertyName = "SupplierName"
            .HeaderText = "仕入先"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            .Width = 330
        End With
        With TextBoxColumnOrderId
            .Name = "TextBoxColumnOrderId"
            .DataPropertyName = "OrderId"
            .HeaderText = "発注ID"
            .Visible = False
        End With
        With TextBoxColumnArrivalId
            .Name = "TextBoxColumnArrivalId"
            .DataPropertyName = "ArrivalId"
            .HeaderText = "入荷ID"
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
            .Columns.Add(TextBoxColumnOrderDate)
            .Columns.Add(TextBoxColumnArrivalDate)
            .Columns.Add(TextBoxColumnSupplierName)
            .Columns.Add(TextBoxColumnOrderId)
            .Columns.Add(TextBoxColumnArrivalId)

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

    End Sub

#End Region

#Region "イベント"

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

    Public Sub DataBind(arrivalDateFrom As String, arrivaldateTo As String)

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Try
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String = String.Empty
                    SQLString = "select"
                    SQLString &= " OrderAndArrivals.OrderId as ""OrderId"""
                    SQLString &= ",to_char(OrderAndArrivals.OrderDate, 'YYYY/MM/DD') as ""OrderDate"""
                    SQLString &= ",OrderAndArrivals.ArrivalId as ""ArrivalId"""
                    SQLString &= ",to_char(OrderAndArrivals.ArrivalDate, 'YYYY/MM/DD') as ""ArrivalDate"""
                    SQLString &= ",Suppliers.SupplierName as ""SupplierName"""
                    SQLString &= " from ("

                    ' 発注データありで入荷したもの
                    SQLString &= " select"
                    SQLString &= " Orders.OrderId"
                    SQLString &= ",Orders.OrderDate"
                    SQLString &= ",Arrivals.ArrivalId"
                    SQLString &= ",Arrivals.ArrivalDate"
                    SQLString &= ",Orders.SupplierId"
                    SQLString &= " from Orders"
                    SQLString &= " left join Arrivals"
                    SQLString &= " on Orders.OrderId = Arrivals.OrderId"
                    SQLString &= " where Arrivals.OrderId is not null"

                    SQLString &= " union all"

                    ' 発注データありで未入荷のもの
                    SQLString &= " select"
                    SQLString &= " Orders.OrderId"
                    SQLString &= ",Orders.OrderDate"
                    SQLString &= ",null as ArrivalId"
                    SQLString &= ",null as ArrivalDate"
                    SQLString &= ",Orders.SupplierId"
                    SQLString &= " from Orders"
                    SQLString &= " left join Arrivals"
                    SQLString &= " on Orders.OrderId = Arrivals.OrderId"
                    SQLString &= " where Arrivals.OrderId is null"

                    SQLString &= " union all"

                    ' 発注データなしで入荷したもの
                    SQLString &= " select"
                    SQLString &= " null as OrderId"
                    SQLString &= ",null as OrderDate"
                    SQLString &= ",Arrivals.ArrivalId"
                    SQLString &= ",Arrivals.ArrivalDate"
                    SQLString &= ",Arrivals.SupplierId"
                    SQLString &= " from Arrivals"
                    SQLString &= " where Arrivals.OrderId is null"

                    SQLString &= " ) as OrderAndArrivals"
                    SQLString &= " left join Suppliers"
                    SQLString &= " on OrderAndArrivals.SupplierId = Suppliers.SupplierId"
                    SQLString &= " where ("
                    SQLString &= " OrderAndArrivals.ArrivalDate between @ArrivalDateFrom and @ArrivalDateTo"
                    SQLString &= " or OrderAndArrivals.ArrivalDate is null"
                    SQLString &= " )"
                    SQLString &= " order by"
                    SQLString &= " OrderAndArrivals.OrderDate desc nulls last"
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    If String.IsNullOrEmpty(arrivalDateFrom) Then arrivalDateFrom = "1988/1/1"
                    If String.IsNullOrEmpty(arrivaldateTo) Then arrivaldateTo = "2999/12/31"
                    cmd.Parameters.AddWithValue("@ArrivalDateFrom", NpgsqlDbType.Date, CDate(arrivalDateFrom).Date)
                    cmd.Parameters.AddWithValue("@ArrivalDateTo", NpgsqlDbType.Date, CDate(arrivaldateTo).Date)
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

    Public Sub Filter(orderDate As String, arrivalDate As String, supplierName As String, notArrived As Boolean)

        Me.dataView.RowFilter = String.Empty

        If Not String.IsNullOrEmpty(orderDate) Then
            Me.dataView.RowFilter &= IIf(Not String.IsNullOrEmpty(Me.dataView.RowFilter), " and ", "") &
                                     "(OrderDate like '%" & orderDate & "%')"
        End If
        If Not String.IsNullOrEmpty(arrivalDate) Then
            Me.dataView.RowFilter &= IIf(Not String.IsNullOrEmpty(Me.dataView.RowFilter), " and ", "") &
                                     "(ArrivalDate like '%" & arrivalDate & "%')"
        End If
        If Not String.IsNullOrEmpty(supplierName) Then
            Me.dataView.RowFilter &= IIf(Not String.IsNullOrEmpty(Me.dataView.RowFilter), " and ", "") &
                                     "(SupplierName like '%" & supplierName & "%')"
        End If
        If notArrived Then
            Me.dataView.RowFilter &= IIf(Not String.IsNullOrEmpty(Me.dataView.RowFilter), " and ", "") &
                                     "(ArrivalDate is null or ArrivalDate = '')"
        End If

        Me(ButtonColumnDetail.Name, Me.NewRowIndex).Value = "新規"

        ScrollToBottom()

    End Sub

    Public Function SelectRowById(id As String) As Boolean

        If String.IsNullOrEmpty(id) Then Return False

        For Each row As DataGridViewRow In Me.Rows
            If row.IsNewRow Then Continue For

            If Convert.ToString(row.Cells(TextBoxColumnOrderId.Name).Value) = id Then
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
