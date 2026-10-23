Imports Npgsql

Public Class FormEditOperators

#Region "定義"

    Private ReadOnly _menu As FormMenu
    Private WithEvents dataGridViewEditOperators As DataGridViewEditOperators
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

        dataGridViewEditOperators = New DataGridViewEditOperators()
        PanelOperators.Controls.Add(dataGridViewEditOperators)

        ' フォームの初期化
        InitializeForm(True)

    End Sub

    Private Sub Me_Shown(sender As Object, e As System.EventArgs) Handles Me.Shown

        AddHandler TextBoxFilterOperatorCode.TextChanged, AddressOf TextBoxFilter_TextChanged
        AddHandler TextBoxFilterOperatorName.TextChanged, AddressOf TextBoxFilter_TextChanged

        ' 編集から戻ってきた場合はその行へ。無ければ最下行へ。
        If Not String.IsNullOrEmpty(_selectedId) AndAlso dataGridViewEditOperators.SelectRowById(_selectedId) Then
            ' 位置付け成功(何もしない)
        Else
            dataGridViewEditOperators.ScrollToBottom()
        End If

        TextBoxFilterOperatorCode.Focus()

    End Sub

    Private Sub TextBoxFilter_TextChanged(sender As System.Object, e As System.EventArgs)

        dataGridViewEditOperators.Filter(TextBoxFilterOperatorCode.Text, TextBoxFilterOperatorName.Text)

    End Sub

    Private Sub dataGridViewEditOperators_SelectClick(sender As Object, e As DataGridViewCellEventArgs) Handles dataGridViewEditOperators.SelectClick

        If e.RowIndex = dataGridViewEditOperators.NewRowIndex Then
            _menu.openChildForm(_menu.currentButton, New FormEditOperator(_menu, EditMode.Add))
        Else
            Dim id As String = dataGridViewEditOperators.Rows(e.RowIndex).Cells("TextBoxColumnOperatorId").Value.ToString
            _menu.openChildForm(_menu.currentButton, New FormEditOperator(_menu, EditMode.Edit, id))
        End If

    End Sub

#End Region

#Region "メソッド"

    Private Sub InitializeForm(Optional isLoading As Boolean = False)

        TextBoxFilterOperatorCode.Text = String.Empty
        TextBoxFilterOperatorName.Text = String.Empty

        ' リストの初期化
        dataGridViewEditOperators.DataBind()

        If Not isLoading Then TextBoxFilterOperatorCode.Focus()

    End Sub

#End Region

End Class

Public Class DataGridViewEditOperators
    Inherits CustomDataGridView

#Region "定義"

    Public ButtonColumnDetail As New CustomDatagridViewButtonColumn
    Public TextBoxColumnOperatorCode As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnOperatorName As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnRoleName As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnUnusedName As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnLastLoginDate As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnOperatorId As New CustomDataGridViewTextBoxColumn

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
        With TextBoxColumnOperatorCode
            .Name = "TextBoxColumnOperatorCode"
            .DataPropertyName = "OperatorCode"
            .HeaderText = "担当者コード"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            .Width = 110
        End With
        With TextBoxColumnOperatorName
            .Name = "TextBoxColumnOperatorName"
            .DataPropertyName = "OperatorName"
            .HeaderText = "担当者名"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            .Width = 220
        End With
        With TextBoxColumnRoleName
            .Name = "TextBoxColumnRoleName"
            .DataPropertyName = "RoleName"
            .HeaderText = "権限"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .Width = 80
        End With
        With TextBoxColumnUnusedName
            .Name = "TextBoxColumnUnusedName"
            .DataPropertyName = "UnusedName"
            .HeaderText = "使用可否"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .Width = 80
        End With
        With TextBoxColumnLastLoginDate
            .Name = "TextBoxColumnLastLoginDate"
            .DataPropertyName = "LastLoginDate"
            .HeaderText = "最終ログイン"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .DefaultCellStyle.Format = "yyyy/MM/dd HH:mm"
            .Width = 150
        End With
        With TextBoxColumnOperatorId
            .Name = "TextBoxColumnOperatorId"
            .DataPropertyName = "OperatorId"
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
            .Columns.Add(TextBoxColumnOperatorCode)
            .Columns.Add(TextBoxColumnOperatorName)
            .Columns.Add(TextBoxColumnRoleName)
            .Columns.Add(TextBoxColumnUnusedName)
            .Columns.Add(TextBoxColumnLastLoginDate)
            .Columns.Add(TextBoxColumnOperatorId)

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
                    SQLString &= " OperatorId"
                    SQLString &= ",OperatorCode"
                    SQLString &= ",OperatorName"
                    SQLString &= ",case RoleId when 1 then '管理者' else '一般' end as RoleName"
                    SQLString &= ",case UnusedFlag when 1 then '未使用' else '' end as UnusedName"
                    SQLString &= ",LastLoginDate"
                    SQLString &= " from Operators"
                    SQLString &= " order by"
                    SQLString &= " OrderNo"
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

    Public Sub Filter(operatorCode As String, operatorName As String)

        Me.dataView.RowFilter = String.Empty

        If Not String.IsNullOrEmpty(operatorCode) Then
            Me.dataView.RowFilter &= IIf(Not String.IsNullOrEmpty(Me.dataView.RowFilter), " and ", "") &
                                     "(OperatorCode like '%" & operatorCode & "%')"
        End If
        If Not String.IsNullOrEmpty(operatorName) Then
            Me.dataView.RowFilter &= IIf(Not String.IsNullOrEmpty(Me.dataView.RowFilter), " and ", "") &
                                     "(OperatorName like '%" & operatorName & "%')"
        End If

        Me(ButtonColumnDetail.Name, Me.NewRowIndex).Value = "新規"

        ScrollToBottom()

    End Sub

    Public Function SelectRowById(id As String) As Boolean

        If String.IsNullOrEmpty(id) Then Return False

        For Each row As DataGridViewRow In Me.Rows
            If row.IsNewRow Then Continue For

            If Convert.ToString(row.Cells(TextBoxColumnOperatorId.Name).Value) = id Then
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