Imports Npgsql
Imports NpgsqlTypes

Public Class FormInstructions
    Implements IRefreshable

#Region "定義"

    Private ReadOnly _menu As FormMenu
    Private WithEvents dataGridViewInstructions As DataGridViewInstructions

    Private storeList As New List(Of Store)

#End Region

#Region "コンストラクタ"

    Public Sub New(parent As Form)

        InitializeComponent()
        _menu = parent

    End Sub

#End Region

#Region "イベント"

    Private Sub Me_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        storeList = Stores.GetAll()

        dataGridViewInstructions = New DataGridViewInstructions(storeList)
        PanelInstructions.Controls.Add(dataGridViewInstructions)

        ' フォームの初期化
        InitializeForm(True)

    End Sub

    Private Sub Me_Shown(sender As Object, e As System.EventArgs) Handles Me.Shown

        AddHandler DateBoxStartDate.TextChanged, AddressOf DateBoxStartDate_TextChanged

        DateBoxStartDate.Focus()

    End Sub

    Private Sub DateBoxStartDate_TextChanged(sender As Object, e As EventArgs)

        If IsDate(DateBoxStartDate.Text) Then
            calc.ReloadInventories()
            dataGridViewInstructions.DataBind(DateBoxStartDate.Text)
        End If

    End Sub

    Private Sub ButtonStartDate_Click(sender As Object, e As EventArgs) Handles ButtonStartDateNext.Click, ButtonStartDatePrevious.Click

        If Not IsDate(DateBoxStartDate.Text) Then
            DateBoxStartDate.Text = Now.ToString("yyyy/MM/dd")
        Else
            If sender.Equals(ButtonStartDateNext) Then
                DateBoxStartDate.Text = CDate(DateBoxStartDate.Text).AddDays(1)
            Else
                DateBoxStartDate.Text = CDate(DateBoxStartDate.Text).AddDays(-1)
            End If
        End If
        DateBoxStartDate.Focus()

    End Sub

    Public Sub dataGridViewInstructions_RemoveClick(sender As Object, e As DataGridViewCellEventArgs) Handles dataGridViewInstructions.RemoveClick

        ' レコード削除
        If MessageBox.Show("削除します。よろしいですか？", Me.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = DialogResult.Yes Then
            Try
                DeleteData(e.RowIndex)
            Catch ex As Exception
                SysMsgBox(ex.Message, Me.Text)
                Exit Sub
            End Try
            InitializeForm()
        End If

    End Sub

    Public Sub dataGridViewInstructions_RealQuantity_Validating(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles dataGridViewInstructions.RealQuantity_Validating

        Dim dgv As DataGridViewInstructions = dataGridViewInstructions

        ' 空白なら処理を抜ける
        If String.IsNullOrEmpty(e.FormattedValue) Then
            Exit Sub
        End If
        ' 値が変更されていなければ処理を抜ける
        If Not dgv.IsCurrentCellDirty Then Exit Sub
        ' 妥当性チェック
        Try
            ' 数値以外はエラー
            If Not IsNumeric(e.FormattedValue) Then
                Throw New ApplicationException("数値を入力してください。")
            End If
            ' マイナスはエラー
            If CInt(e.FormattedValue) < 0 Then
                Throw New ApplicationException("マイナスは入力できません。")
            End If
        Catch ex As Exception
            If ex.GetType Is GetType(ApplicationException) Then
                MessageBox.Show(ex.Message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Else
                SysMsgBox(ex.Message, Me.Text)
            End If
            e.Cancel = True
            Exit Sub
        End Try

    End Sub

    Public Sub dataGridViewInstructions_RowValidating(sender As Object, e As DataGridViewCellCancelEventArgs) Handles dataGridViewInstructions.RowValidating

        Dim dgv As DataGridViewInstructions = dataGridViewInstructions

        If e.RowIndex = dgv.NewRowIndex OrElse Not dgv.IsCurrentRowDirty Then
            Exit Sub
        End If

        Try
            If String.IsNullOrEmpty(Nz(dgv(dgv.TextBoxColumnRealQuantity.Name, e.RowIndex).FormattedValue.ToString.Trim, "")) Then
                dgv.CurrentCell = dgv(dgv.TextBoxColumnRealQuantity.Name, e.RowIndex)
                Throw New ApplicationException("完成数量が入力されていません。")
            End If

            ' 合計数チェック
            Dim sumQuantity As Double = 0
            For i As Integer = 0 To storeList.Count - 1
                sumQuantity += Nz(dgv(dgv.TextBoxColumnQuantity(i).Name, e.RowIndex).Value, 0)
            Next
            If dgv(dgv.TextBoxColumnRealQuantity.Name, e.RowIndex).Value <> sumQuantity Then
                dgv.CurrentCell = dgv(dgv.TextBoxColumnRealQuantity.Name, e.RowIndex)
                Throw New ApplicationException("完成数量が一致しません。")
            End If
        Catch ex As Exception
            If ex.GetType Is GetType(ApplicationException) Then
                MessageBox.Show(ex.Message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Else
                SysMsgBox(ex.Message, Me.Text)
            End If
            e.Cancel = True
            Exit Sub
        End Try

    End Sub

    Private Sub ButtonRegist_Click(sender As Object, e As EventArgs) Handles ButtonRegist.Click

        ' 更新前チェック
        If CheckData() = False Then
            Exit Sub
        End If

        ' レコード更新
        If MessageBox.Show("登録します。よろしいですか？", Me.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = DialogResult.Yes Then
            Try
                If Not SaveData() Then Exit Sub
            Catch ex As Exception
                SysMsgBox(ex.Message, Me.Text)
                Exit Sub
            End Try
            InitializeForm()
        End If

    End Sub

#End Region

#Region "メソッド"

    Private Function CheckData() As Boolean

        Dim result As Boolean = True

        Dim dgv As DataGridViewInstructions = dataGridViewInstructions

        ' 編集中の内容をセルに確定させる
        dgv.EndEdit()

        Try
            If String.IsNullOrEmpty(DateBoxStartDate.Text) Then
                DateBoxStartDate.Focus()
                Throw New ApplicationException("製造日が入力されていません。")
            End If
        Catch ex As Exception
            If ex.GetType Is GetType(ApplicationException) Then
                MessageBox.Show(ex.Message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Else
                SysMsgBox(ex.Message, Me.Text)
            End If
            result = False
        End Try

        Return result

    End Function

    ' 画面の入力内容から保存用のリストを作る
    Private Function BuildInstructionList() As List(Of Instruction)

        Dim instructionList As New List(Of Instruction)

        For Each row As DataGridViewRow In dataGridViewInstructions.Rows
            If row.IsNewRow Then Exit For

            Dim instruction As New Instruction
            instruction.Item = Items.GetById(dataGridViewInstructions(dataGridViewInstructions.TextBoxColumnItemId.Name, row.Index).Value.ToString)
            instruction.PlanQuantity = dataGridViewInstructions(dataGridViewInstructions.TextBoxColumnPlanQuantity.Name, row.Index).Value.ToString
            If Not IsDBNull(dataGridViewInstructions(dataGridViewInstructions.TextBoxColumnRealQuantity.Name, row.Index).Value) Then
                instruction.RealQuantity = dataGridViewInstructions(dataGridViewInstructions.TextBoxColumnRealQuantity.Name, row.Index).Value.ToString
            Else
                instruction.RealQuantity = dataGridViewInstructions(dataGridViewInstructions.TextBoxColumnPlanQuantity.Name, row.Index).Value.ToString
            End If
            instruction.UnitType = UnitTypes.GetById(dataGridViewInstructions(dataGridViewInstructions.TextBoxColumnUnitTypeId.Name, row.Index).Value.ToString)
            instruction.StartDate = dataGridViewInstructions(dataGridViewInstructions.TextBoxColumnStartDate.Name, row.Index).Value.ToString
            If Not IsDBNull(dataGridViewInstructions(dataGridViewInstructions.TextBoxColumnFinishDate.Name, row.Index).Value) Then
                instruction.FinishDate = dataGridViewInstructions(dataGridViewInstructions.TextBoxColumnFinishDate.Name, row.Index).Value.ToString
            Else
                instruction.FinishDate = DateBoxStartDate.Text
            End If
            Dim i As Integer = 0
            For Each store As Store In storeList
                If Nz(dataGridViewInstructions(dataGridViewInstructions.TextBoxColumnQuantity(i).Name, row.Index).Value, 0) > 0 Then
                    Dim InstructionDetail As New InstructionDetail
                    InstructionDetail.Store = Stores.GetById(store.StoreId)
                    InstructionDetail.Quantity = dataGridViewInstructions(dataGridViewInstructions.TextBoxColumnQuantity(i).Name, row.Index).Value.ToString
                    instruction.InstructionDetails.Add(InstructionDetail)
                End If
                i += 1
            Next
            instruction.InstructionId = dataGridViewInstructions(dataGridViewInstructions.TextBoxColumnInstructionId.Name, row.Index).Value.ToString
            instruction.PlanId = dataGridViewInstructions(dataGridViewInstructions.TextBoxColumnPlanId.Name, row.Index).Value.ToString
            instruction.InstructionFinish = dataGridViewInstructions(dataGridViewInstructions.CheckBoxColumnInstructionFinish.Name, row.Index).Value
            instructionList.Add(instruction)
        Next

        Return instructionList

    End Function

    Private Function SaveData() As Boolean

        Try
            Instructions.Save(BuildInstructionList())

        Catch ex As ShortageException
            Dim message As String = "以下の材料が在庫不足です。" & vbCrLf & vbCrLf
            message &= String.Join(vbCrLf, ex.Shortages) & vbCrLf & vbCrLf
            message &= "在庫をマイナスにして製造完了しますか？" & vbCrLf
            message &= "(あとで入荷を登録すると在庫が正しくなります)"

            If MessageBox.Show(message, Me.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) <> DialogResult.OK Then
                Return False
            End If

            ' 1回目はRollbackされているので作り直してから強制保存する
            Instructions.Save(BuildInstructionList(), True)
        End Try

        Return True

    End Function

    Private Sub DeleteData(rowIndex As Integer)

        Try
            Dim instruction As New Instruction
            instruction.InstructionId = dataGridViewInstructions(dataGridViewInstructions.TextBoxColumnInstructionId.Name, rowIndex).Value

            Instructions.Delete(instruction)
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try

    End Sub

    Private Sub InitializeForm(Optional isLoading As Boolean = False)

        DateBoxStartDate.Text = Now.ToString("yyyy/MM/dd")

        ' リストの初期化
        calc.ReloadInventories()
        dataGridViewInstructions.DataBind(DateBoxStartDate.Text)

    End Sub

    ' メニューの再表示ボタンから呼ばれる
    Public Sub RefreshView() Implements IRefreshable.RefreshView

        InitializeForm()

    End Sub

#End Region

End Class

Public Class DataGridViewInstructions
    Inherits CustomDataGridView

#Region "定義"

    Public ButtonColumnRemove As New CustomDatagridViewButtonColumn
    Public CheckBoxColumnInstructionFinish As New DataGridViewCheckBoxColumn
    Public TextBoxColumnItemTypeName As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnItemName As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnPlanQuantity As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnPlanUnitTypeName As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnRealQuantity As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnRealUnitTypeName As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnQuantity() As CustomDataGridViewTextBoxColumn
    Public textBoxColumnUnitTypeName() As CustomDataGridViewTextBoxColumn
    Public TextBoxColumnCost As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnItemId As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnUnitTypeId As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnStartDate As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnFinishDate As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnInstructionId As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnPlanId As New CustomDataGridViewTextBoxColumn

    Public Event RemoveClick(sender As Object, e As DataGridViewCellEventArgs)
    Public Event RealQuantity_Validating(sender As Object, e As DataGridViewCellValidatingEventArgs)
    Public Event UnitType_SelectedIndexChanged(sender As Object, selectedValue As Object, e As EventArgs)

    Public isDataBinding As Boolean
    Private comboBoxEditingControl As New CustomDataGridViewComboBoxEditingControl

    Private _storeList As New List(Of Store)
    Private itemTypeBackColors As New Dictionary(Of String, Color)

#End Region

#Region "コンストラクタ"

    Public Sub New(storeList As List(Of Store))

        _storeList = storeList

        With ButtonColumnRemove
            .Name = "ButtonColumnRemove"
            .HeaderText = String.Empty
            .Width = 50
            .DefaultCellStyle.Font = New Font("MS UI Gothic", 11.25)
            .DefaultCellStyle.NullValue = "削除"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .UseColumnTextForButtonValue = True
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .TabStop = False
        End With
        With CheckBoxColumnInstructionFinish
            .Name = "CheckBoxColumnInstructionFinish"
            .HeaderText = "製造完了"
            .Width = 80
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .SortMode = DataGridViewColumnSortMode.NotSortable
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
        With TextBoxColumnPlanQuantity
            .Name = "TextBoxColumnPlanQuantity"
            .DataPropertyName = "PlanQuantity"
            .HeaderText = String.Empty          ' 見出しは結合ヘッダーで描くので空
            .Width = 50
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .DefaultCellStyle.Format = "#,##0"
            .DefaultCellStyle.SelectionForeColor = Color.Black
            .DefaultCellStyle.SelectionBackColor = Color.White
            .ReadOnly = True
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .TabStop = False
        End With
        With TextBoxColumnPlanUnitTypeName
            .Name = "TextBoxColumnPlanUnitTypeName"
            .DataPropertyName = "PlanUnitTypeName"
            .HeaderText = String.Empty          ' 見出しは結合ヘッダーで描くので空
            .Width = 60
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            .DefaultCellStyle.SelectionForeColor = Color.Black
            .DefaultCellStyle.SelectionBackColor = Color.White
            .ReadOnly = True
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .TabStop = False
        End With
        With TextBoxColumnRealQuantity
            .Name = "TextBoxColumnRealQuantity"
            .DataPropertyName = "RealQuantity"
            .HeaderText = String.Empty          ' 見出しは結合ヘッダーで描くので空
            .Width = 50
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .DefaultCellStyle.Format = "#,##0"
            .DefaultCellStyle.SelectionForeColor = Color.Black
            .DefaultCellStyle.SelectionBackColor = Color.White
            .ReadOnly = False
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .NumericOnly = True
            .TabStop = True
        End With
        With TextBoxColumnRealUnitTypeName
            .Name = "TextBoxColumnRealUnitTypeName"
            .DataPropertyName = "RealUnitTypeName"
            .HeaderText = String.Empty          ' 見出しは結合ヘッダーで描くので空
            .Width = 60
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            .DefaultCellStyle.SelectionForeColor = Color.Black
            .DefaultCellStyle.SelectionBackColor = Color.White
            .ReadOnly = True
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .TabStop = False
        End With

        ' 店舗ごとの(数量 + 単位)列を作成
        ReDim TextBoxColumnQuantity(_storeList.Count - 1)
        ReDim textBoxColumnUnitTypeName(_storeList.Count - 1)
        Dim i As Integer = 0
        For Each store As Store In _storeList
            TextBoxColumnQuantity(i) = New CustomDataGridViewTextBoxColumn
            With TextBoxColumnQuantity(i)
                .Name = "TextBoxColumnQuantity" & store.StoreId
                .DataPropertyName = "Quantity" & store.StoreId
                .HeaderText = String.Empty      ' 見出しは結合ヘッダーで描くので空
                .Width = 50
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
                .DefaultCellStyle.Format = "#,##0.##"
                .SortMode = DataGridViewColumnSortMode.NotSortable
                .NumericOnly = True
                .AllowSymbolChar = False
                .MaxByteLength = 5
            End With
            textBoxColumnUnitTypeName(i) = New CustomDataGridViewTextBoxColumn
            With textBoxColumnUnitTypeName(i)
                .Name = "TextBoxColumnUnitTypeName" & store.StoreId
                .DataPropertyName = "UnitTypeName" & store.StoreId
                .HeaderText = String.Empty      ' 見出しは結合ヘッダーで描くので空
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
        With TextBoxColumnCost
            .Name = "TextBoxColumnCost"
            .DataPropertyName = "Cost"
            .HeaderText = "原価"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .DefaultCellStyle.Format = "#,##0.##"
            .DefaultCellStyle.SelectionForeColor = Color.Black
            .DefaultCellStyle.SelectionBackColor = Color.White
            .Width = 80
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
        With TextBoxColumnUnitTypeId
            .Name = "TextBoxColumnUnitTypeId"
            .DataPropertyName = "UnitTypeId"
            .HeaderText = "単位ID"
            .Visible = False
        End With
        With TextBoxColumnStartDate
            .Name = "TextBoxColumnStartDate"
            .DataPropertyName = "StartDate"
            .HeaderText = "製造開始日"
            .DefaultCellStyle.Format = "yyyy/MM/dd"
            .Visible = False
        End With
        With TextBoxColumnFinishDate
            .Name = "TextBoxColumnFinishDate"
            .DataPropertyName = "FinishDate"
            .HeaderText = "製造完了日"
            .DefaultCellStyle.Format = "yyyy/MM/dd"
            .Visible = False
        End With
        With TextBoxColumnInstructionId
            .Name = "TextBoxColumnInstructionId"
            .DataPropertyName = "InstructionId"
            .HeaderText = "製造ID"
            .Visible = False
        End With
        With TextBoxColumnPlanId
            .Name = "TextBoxColumnPlanId"
            .DataPropertyName = "PlanId"
            .HeaderText = "計画ID"
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
            .ColumnHeadersHeight = 22          ' 1段(結合ヘッダーはこの高さいっぱいに描く)
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

            .Columns.Add(ButtonColumnRemove)
            .Columns.Add(CheckBoxColumnInstructionFinish)
            .Columns.Add(TextBoxColumnItemTypeName)
            .Columns.Add(TextBoxColumnItemName)
            .Columns.Add(TextBoxColumnPlanQuantity)
            .Columns.Add(TextBoxColumnPlanUnitTypeName)
            .Columns.Add(TextBoxColumnRealQuantity)
            .Columns.Add(TextBoxColumnRealUnitTypeName)
            For i = 0 To _storeList.Count - 1
                .Columns.Add(TextBoxColumnQuantity(i))
                .Columns.Add(textBoxColumnUnitTypeName(i))
            Next
            .Columns.Add(TextBoxColumnCost)
            .Columns.Add(TextBoxColumnItemId)
            .Columns.Add(TextBoxColumnUnitTypeId)
            .Columns.Add(TextBoxColumnStartDate)
            .Columns.Add(TextBoxColumnFinishDate)
            .Columns.Add(TextBoxColumnInstructionId)
            .Columns.Add(TextBoxColumnPlanId)

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
            Case ButtonColumnRemove.Name
                If Me(ButtonColumnRemove.Name, e.RowIndex).ReadOnly Then Return

                If Not IsNothing(Me(TextBoxColumnInstructionId.Name, e.RowIndex).Value) Then
                    RaiseEvent RemoveClick(sender, e)
                Else
                    If Not Me.Rows(e.RowIndex).IsNewRow Then
                        Me.Rows.RemoveAt(e.RowIndex)
                    End If
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

    End Sub

    Private Sub Me_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles Me.CellValidating
    End Sub

    Private Sub Me_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles Me.CellValueChanged

        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Return
        If isDataBinding Then Return

        Dim col As DataGridViewColumn = Me.Columns(e.ColumnIndex)

        If col Is CheckBoxColumnInstructionFinish Then
            If e.RowIndex < 0 OrElse e.RowIndex >= Me.Rows.Count Then Return

            ApplyRowLock(e.RowIndex)
        End If

    End Sub

    Private Sub Me_CurrentCellDirtyStateChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.CurrentCellDirtyStateChanged

        If Not Me.IsCurrentCellDirty Then Return

        If TypeOf Me.CurrentCell Is DataGridViewCheckBoxCell Then
            Me.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If

    End Sub

    Private Sub Me_EditingControlShowing(sender As Object, e As DataGridViewEditingControlShowingEventArgs) Handles Me.EditingControlShowing

        Select Case CurrentCell.OwningColumn.Name
            Case TextBoxColumnRealQuantity.Name
                If IsNumeric(e.Control.Text) Then
                    e.Control.Text = CInt(e.Control.Text).ToString(Me.TextBoxColumnRealQuantity.DefaultCellStyle.Format.Replace(",", ""))
                End If
        End Select

    End Sub

    Private Sub dataGridViewComboBox_SelectedIndexChanged(sender As Object, e As EventArgs)
    End Sub

#End Region

#Region "メソッド"

    Public Sub DataBind(planDate As Date)

        RemoveHandler Me.CellValidating, AddressOf Me_CellValidating

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Try
                isDataBinding = True

                Me.Rows.Clear()

                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String = String.Empty
                    SQLString = "select"
                    SQLString &= " Instructions.InstructionId"
                    SQLString &= ",Instructions.PlanId"
                    SQLString &= ",Instructions.ItemId"
                    SQLString &= ",Items.ItemName"
                    SQLString &= ",ItemTypes.ItemTypeName"
                    SQLString &= ",Instructions.PlanQuantity"
                    SQLString &= ",Instructions.RealQuantity"
                    SQLString &= ",Instructions.UnitTypeId"
                    SQLString &= ",UnitTypes.UnitTypeName"
                    SQLString &= ",Instructions.StartDate"
                    SQLString &= ",Instructions.FinishDate"
                    For Each store As Store In _storeList
                        SQLString &= ",InstructionSums.Quantity" & CInt(store.StoreId).ToString()
                    Next
                    For Each store As Store In _storeList
                        SQLString &= ",PlanSums.PlanQuantity" & CInt(store.StoreId).ToString()
                    Next
                    SQLString &= " from Instructions"

                    ' 製造指示明細の店舗別集計
                    SQLString &= " left join ("
                    SQLString &= " select InstructionDetails.InstructionId"

                    For Each store As Store In _storeList
                        Dim storeId As Integer = CInt(store.StoreId)
                        SQLString &= ",sum("
                        SQLString &= " case"
                        SQLString &= " when InstructionDetails.StoreId = " & storeId.ToString()
                        SQLString &= " then InstructionDetails.Quantity"
                        SQLString &= " else 0"
                        SQLString &= " end"
                        SQLString &= " ) as Quantity" & storeId.ToString()
                    Next

                    SQLString &= " from InstructionDetails"
                    SQLString &= " group by InstructionDetails.InstructionId"
                    SQLString &= " ) as InstructionSums"
                    SQLString &= " on Instructions.InstructionId = InstructionSums.InstructionId"

                    ' 製造計画明細の店舗別集計
                    SQLString &= " left join ("
                    SQLString &= " select PlanDetails.PlanId"

                    For Each store As Store In _storeList
                        Dim storeId As Integer = CInt(store.StoreId)
                        SQLString &= ",sum("
                        SQLString &= " case"
                        SQLString &= " when PlanDetails.StoreId = " & storeId.ToString()
                        SQLString &= " then PlanDetails.Quantity"
                        SQLString &= " else 0"
                        SQLString &= " end"
                        SQLString &= " ) as PlanQuantity" & storeId.ToString()
                    Next
                    SQLString &= " from PlanDetails"
                    SQLString &= " group by PlanDetails.PlanId"
                    SQLString &= " ) as PlanSums on Instructions.PlanId = PlanSums.PlanId"
                    SQLString &= " left join Items on Instructions.ItemId = Items.ItemId"
                    SQLString &= " left join UnitTypes on Instructions.UnitTypeId = UnitTypes.UnitTypeId"
                    SQLString &= " left join ItemTypes on Items.ItemTypeId = ItemTypes.ItemTypeId"
                    SQLString &= " where Instructions.StartDate = @StartDate"
                    SQLString &= " order by"
                    ' 完成（FinishDateあり）だけ下に落とす
                    SQLString &= " case"
                    SQLString &= " when Instructions.FinishDate is null then 0"
                    SQLString &= " else 1"
                    SQLString &= " end"
                    ' 品目区分の並び（半製品 → 製品）
                    SQLString &= ",case"
                    SQLString &= " when Items.ItemTypeId = 2 then 0"
                    SQLString &= " else 1"
                    SQLString &= " end"
                    SQLString &= ",Instructions.ItemId"
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    cmd.Parameters.AddWithValue("@StartDate", NpgsqlDbType.Date, CDate(planDate).Date)

                    ' 原価計算は行をまたいでロットのキャッシュを使い回す
                    Dim costCalc As New CostCalculator(calc)

                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        If dr.HasRows Then
                            While dr.Read()
                                Dim column As New List(Of Object)
                                column.Add(Nothing)
                                column.Add(If(IsDBNull(dr("FinishDate")), False, True))
                                column.Add(dr("ItemTypeName"))
                                column.Add(dr("ItemName"))
                                column.Add(dr("PlanQuantity"))
                                column.Add(dr("UnitTypeName"))
                                If IsDBNull(dr("FinishDate")) Then
                                    column.Add(dr("PlanQuantity"))
                                    column.Add(dr("UnitTypeName"))
                                Else
                                    column.Add(dr("RealQuantity"))
                                    column.Add(dr("UnitTypeName"))
                                End If
                                For Each store As Store In _storeList
                                    Dim quantity As Integer
                                    If IsDBNull(dr("FinishDate")) Then
                                        quantity = Nz(dr("PlanQuantity" & store.StoreId), 0)
                                    Else
                                        quantity = Nz(dr("Quantity" & store.StoreId), 0)
                                    End If
                                    column.Add(quantity)
                                    column.Add(dr("UnitTypeName"))
                                Next

                                '原価(行ごとに在庫を満タンに戻してから計算する)
                                costCalc.ResetRemaining()
                                Dim result As CostResult = costCalc.CalcUnitCost(CInt(dr("ItemId")), CInt(Nz(dr("UnitTypeId"), 0)))
                                column.Add(result.UnitCost)

                                column.Add(dr("ItemId"))
                                column.Add(dr("UnitTypeId"))
                                column.Add(dr("StartDate"))
                                column.Add(dr("FinishDate"))
                                column.Add(dr("InstructionId"))
                                column.Add(dr("PlanId"))
                                Dim rowIndex As Integer = Me.Rows.Add(column.ToArray())

                                ' 変更前の値を退避
                                Me(TextBoxColumnRealQuantity.Name, Rows.Count - 1).Tag = dr("PlanQuantity")
                                Dim i As Integer = 0
                                For Each store As Store In _storeList
                                    Me(TextBoxColumnQuantity(i).Name, Rows.Count - 1).Tag = Nz(dr("PlanQuantity" & store.StoreId), 0)
                                    i += 1
                                Next

                                ApplyRowLock(rowIndex)
                            End While
                        End If
                    End Using
                End Using
            Catch ex As Exception
                Throw New ApplicationException(ex.Message)
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
        AddMergedHeader(TextBoxColumnPlanQuantity, TextBoxColumnPlanUnitTypeName, "計画数量")
        AddMergedHeader(TextBoxColumnRealQuantity, TextBoxColumnRealUnitTypeName, "製造数量")
        For i As Integer = 0 To _storeList.Count - 1
            AddMergedHeader(TextBoxColumnQuantity(i), textBoxColumnUnitTypeName(i), _storeList(i).StoreShortName)
        Next

    End Sub

    Private Sub ApplyRowLock(rowIndex As Integer)

        If Me(CheckBoxColumnInstructionFinish.Name, rowIndex).Value = True Then
            Me(ButtonColumnRemove.Name, rowIndex).ReadOnly = True
            Me(TextBoxColumnRealQuantity.Name, rowIndex).ReadOnly = False
            For i As Integer = 0 To _storeList.Count - 1
                Me(TextBoxColumnQuantity(i).Name, rowIndex).ReadOnly = False
            Next
        Else
            Me(ButtonColumnRemove.Name, rowIndex).ReadOnly = False
            Me(TextBoxColumnRealQuantity.Name, rowIndex).ReadOnly = True
            Me(TextBoxColumnRealQuantity.Name, rowIndex).Value = Me(TextBoxColumnRealQuantity.Name, rowIndex).Tag
            For i As Integer = 0 To _storeList.Count - 1
                Me(TextBoxColumnQuantity(i).Name, rowIndex).ReadOnly = True
                Me(TextBoxColumnQuantity(i).Name, rowIndex).Value = Me(TextBoxColumnQuantity(i).Name, rowIndex).Tag
            Next
        End If

    End Sub

#End Region

End Class
