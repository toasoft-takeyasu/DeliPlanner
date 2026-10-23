Imports Npgsql

Public Class FormEditSuppliers
    Implements IRefreshable

#Region "定義"

    Private ReadOnly _menu As FormMenu
    Private WithEvents dataGridViewEditSuppliers As DataGridViewEditSuppliers
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

        dataGridViewEditSuppliers = New DataGridViewEditSuppliers()
        PanelSuppliers.Controls.Add(dataGridViewEditSuppliers)

        ' フォームの初期化
        InitializeForm(True)

    End Sub

    Private Sub Me_Shown(sender As Object, e As System.EventArgs) Handles Me.Shown

        AddHandler TextBoxFilterSupplierCode.TextChanged, AddressOf TextBoxFilter_TextChanged
        AddHandler TextBoxFilterSupplierName.TextChanged, AddressOf TextBoxFilter_TextChanged
        AddHandler ComboBoxFilerItemType.SelectedValueChanged, AddressOf TextBoxFilter_TextChanged

        ' 編集から戻ってきた場合はその行へ。無ければ最下行へ。
        If Not String.IsNullOrEmpty(_selectedId) AndAlso dataGridViewEditSuppliers.SelectRowById(_selectedId) Then
            ' 位置付け成功(何もしない)
        Else
            dataGridViewEditSuppliers.ScrollToBottom()
        End If

        TextBoxFilterSupplierCode.Focus()

    End Sub

    Private Sub TextBoxFilter_TextChanged(sender As System.Object, e As System.EventArgs)

        dataGridViewEditSuppliers.Filter(TextBoxFilterSupplierCode.Text, TextBoxFilterSupplierName.Text, Nz(ComboBoxFilerItemType.SelectedValue, ""))

    End Sub

    Private Sub dataGridViewEditSuppliers_SelectClick(sender As Object, e As DataGridViewCellEventArgs) Handles dataGridViewEditSuppliers.SelectClick

        If e.RowIndex = dataGridViewEditSuppliers.NewRowIndex Then
            _menu.openChildForm(_menu.currentButton, New FormEditSupplier(_menu, EditMode.Add))
        Else
            Dim id As String = dataGridViewEditSuppliers.Rows(e.RowIndex).Cells("TextBoxColumnSupplierId").Value.ToString
            _menu.openChildForm(_menu.currentButton, New FormEditSupplier(_menu, EditMode.Edit, id))
        End If
    End Sub

#End Region

#Region "メソッド"

    Private Sub InitializeForm(Optional isLoading As Boolean = False)

        TextBoxFilterSupplierCode.Text = String.Empty
        TextBoxFilterSupplierName.Text = String.Empty
        ComboBoxFilerItemType.Text = String.Empty

        ' リストの初期化
        dataGridViewEditSuppliers.DataBind()

        If Not isLoading Then TextBoxFilterSupplierCode.Focus()

    End Sub

    ' メニューの再表示ボタンから呼ばれる
    Public Sub RefreshView() Implements IRefreshable.RefreshView

        InitializeForm()

    End Sub

#End Region

End Class

Public Class DataGridViewEditSuppliers
    Inherits CustomDataGridView

#Region "定義"

    Public ButtonColumnDetail As New CustomDatagridViewButtonColumn
    Public TextBoxColumnSupplierCode As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnSupplierName As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnItemTypeName As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnSupplierId As New CustomDataGridViewTextBoxColumn

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
        With TextBoxColumnSupplierCode
            .Name = "TextBoxColumnSupplierCode"
            .DataPropertyName = "SupplierCode"
            .HeaderText = "仕入先コード"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            .Width = 100
        End With
        With TextBoxColumnSupplierName
            .Name = "TextBoxColumnSupplierName"
            .DataPropertyName = "SupplierName"
            .HeaderText = "仕入先名"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            .Width = 330
        End With
        With TextBoxColumnItemTypeName
            .Name = "TextBoxColumnItemTypeName"
            .DataPropertyName = "ItemTypeName"
            .HeaderText = "品目区分"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .Width = 80
        End With
        With TextBoxColumnSupplierId
            .Name = "TextBoxColumnSupplierId"
            .DataPropertyName = "SupplierId"
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
            .Columns.Add(TextBoxColumnSupplierCode)
            .Columns.Add(TextBoxColumnSupplierName)
            .Columns.Add(TextBoxColumnItemTypeName)
            .Columns.Add(TextBoxColumnSupplierId)

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

    Public Sub DataBind()

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Try
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String = String.Empty
                    SQLString = "select"
                    SQLString &= " Suppliers.SupplierId"
                    SQLString &= ",Suppliers.SupplierCode"
                    SQLString &= ",Suppliers.SupplierName"
                    SQLString &= ",Suppliers.ItemTypeId"
                    SQLString &= ",ItemTypes.ItemTypeName"
                    SQLString &= " from Suppliers"
                    SQLString &= " left join ItemTypes on Suppliers.ItemTypeId = ItemTypes.ItemTypeId"
                    SQLString &= " order by"
                    SQLString &= " Suppliers.SupplierName"
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

    Public Sub Filter(SupplierCode As String, SupplierName As String, itemTypeId As String)

        Me.dataView.RowFilter = String.Empty

        If Not String.IsNullOrEmpty(SupplierCode) Then
            Me.dataView.RowFilter &= IIf(Not String.IsNullOrEmpty(Me.dataView.RowFilter), " and ", "") &
                                     "(SupplierCode like '%" & SupplierCode & "%')"
        End If
        If Not String.IsNullOrEmpty(SupplierName) Then
            Me.dataView.RowFilter &= IIf(Not String.IsNullOrEmpty(Me.dataView.RowFilter), " and ", "") &
                                     "(SupplierName like '%" & SupplierName & "%')"
        End If
        If Not String.IsNullOrEmpty(itemTypeId) Then
            Me.dataView.RowFilter &= IIf(Not String.IsNullOrEmpty(Me.dataView.RowFilter), " and ", "") &
                                     "(ItemTypeId = " & itemTypeId & ")"
        End If

        Me(ButtonColumnDetail.Name, Me.NewRowIndex).Value = "新規"

        ScrollToBottom()

    End Sub

    Public Function SelectRowById(id As String) As Boolean

        If String.IsNullOrEmpty(id) Then Return False

        For Each row As DataGridViewRow In Me.Rows
            If row.IsNewRow Then Continue For

            If Convert.ToString(row.Cells(TextBoxColumnSupplierId.Name).Value) = id Then
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
