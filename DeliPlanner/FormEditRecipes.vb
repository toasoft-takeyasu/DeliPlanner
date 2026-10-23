Imports Npgsql

Public Class FormEditRecipes
    Implements IRefreshable

#Region "定義"

    Private ReadOnly _menu As FormMenu
    Private WithEvents dataGridViewEditRecipes As DataGridViewEditRecipes

#End Region

#Region "コンストラクタ"

    Public Sub New(parent As Form)

        InitializeComponent()
        _menu = parent

    End Sub

#End Region

#Region "イベント"

    Private Sub Me_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        ItemTypes.BindComboBox(ComboBoxFilerItemType, True)

        dataGridViewEditRecipes = New DataGridViewEditRecipes()
        PanelItems.Controls.Add(dataGridViewEditRecipes)

        ' フォームの初期化
        InitializeForm(True)

    End Sub

    Private Sub Me_Shown(sender As Object, e As System.EventArgs) Handles Me.Shown

        AddHandler ComboBoxFilerItemType.SelectedValueChanged, AddressOf TextBoxFilter_TextChanged
        AddHandler TextBoxFilterItemCode.TextChanged, AddressOf TextBoxFilter_TextChanged
        AddHandler TextBoxFilterItemName.TextChanged, AddressOf TextBoxFilter_TextChanged

    End Sub

    Private Sub TextBoxFilter_TextChanged(sender As System.Object, e As System.EventArgs)

        dataGridViewEditRecipes.Filter(Nz(ComboBoxFilerItemType.SelectedValue, ""), TextBoxFilterItemCode.Text, TextBoxFilterItemName.Text)

    End Sub

    Private Sub dataGridViewEditRecipes_SelectClick(sender As Object, e As DataGridViewCellEventArgs) Handles dataGridViewEditRecipes.SelectClick

        Dim id As String = dataGridViewEditRecipes.Rows(e.RowIndex).Cells("TextBoxColumnItemId").Value.ToString
        _menu.openChildForm(_menu.currentButton, New FormEditItem(_menu, EditMode.Edit, id))

    End Sub

#End Region

#Region "メソッド"

    Private Sub InitializeForm(Optional isLoading As Boolean = False)

        ComboBoxFilerItemType.Text = String.Empty
        TextBoxFilterItemCode.Text = String.Empty
        TextBoxFilterItemName.Text = String.Empty

        ' リストの初期化
        dataGridViewEditRecipes.DataBind()

        dataGridViewEditRecipes.Focus()

    End Sub

    ' メニューの再表示ボタンから呼ばれる
    Public Sub RefreshView() Implements IRefreshable.RefreshView

        InitializeForm()

    End Sub

#End Region

End Class

Public Class DataGridViewEditRecipes
    Inherits CustomDataGridView

#Region "定義"

    Public ButtonColumnDetail As New CustomDatagridViewButtonColumn
    Public TextBoxColumnItemTypeName As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnItemCode As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnItemName As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnRecipeRegist As New CustomDataGridViewTextBoxColumn
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
        With TextBoxColumnItemName
            .Name = "TextBoxColumnItemName"
            .DataPropertyName = "ItemName"
            .HeaderText = "品目名"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            .Width = 490
        End With
        With TextBoxColumnRecipeRegist
            .Name = "TextBoxColumnRecipeRegist"
            .DataPropertyName = "RecipeRegist"
            .HeaderText = "レシピ登録"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .Width = 100
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
            .Columns.Add(TextBoxColumnItemName)
            .Columns.Add(TextBoxColumnRecipeRegist)
            .Columns.Add(TextBoxColumnItemId)

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

        If Me.Columns(e.ColumnIndex) Is TextBoxColumnRecipeRegist Then
            If Convert.ToString(e.Value) = "未登録" Then
                e.CellStyle.ForeColor = Color.Red
                e.CellStyle.SelectionForeColor = Color.Red
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
                    SQLString &= ",Items.ItemTypeId"
                    SQLString &= ",Items.ItemCode"
                    SQLString &= ",Items.ItemName"
                    SQLString &= ",ItemTypes.ItemTypeName"
                    SQLString &= ",ItemTypes.OrderNo"
                    SQLString &= ",case when exists ("
                    SQLString &= " select 1"
                    SQLString &= " from Recipes"
                    SQLString &= " inner join RecipeDetails on Recipes.RecipeId = RecipeDetails.RecipeId"
                    SQLString &= " where Recipes.ItemId = Items.ItemId"
                    SQLString &= " ) then '登録済'"
                    SQLString &= " else '未登録'"
                    SQLString &= " end as RecipeRegist"
                    SQLString &= " from Items"
                    SQLString &= " inner join ItemTypes on Items.ItemTypeId = ItemTypes.ItemTypeId"
                    SQLString &= " where ItemTypes.RecipeNeedFlag = 1"
                    SQLString &= " order by"
                    SQLString &= " ItemTypes.OrderNo asc nulls last"
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

    Public Sub Filter(itemTypeId As String, itemCode As String, itemName As String)

        Me.dataView.RowFilter = String.Empty

        If Not String.IsNullOrEmpty(itemTypeId) Then
            Me.dataView.RowFilter &= IIf(Not String.IsNullOrEmpty(Me.dataView.RowFilter), " and ", "") &
                                     "(ItemTypeId = " & itemTypeId & ")"
        End If
        If Not String.IsNullOrEmpty(itemCode) Then
            Me.dataView.RowFilter &= IIf(Not String.IsNullOrEmpty(Me.dataView.RowFilter), " and ", "") &
                                     "(ItemCode like '%" & itemCode & "%')"
        End If
        If Not String.IsNullOrEmpty(itemName) Then
            Me.dataView.RowFilter &= IIf(Not String.IsNullOrEmpty(Me.dataView.RowFilter), " and ", "") &
                                     "(ItemName like '%" & itemName & "%')"
        End If

        Me(ButtonColumnDetail.Name, Me.NewRowIndex).Value = "新規"

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
