Imports Npgsql
Imports NpgsqlTypes
Imports ClosedXML.Excel

Public Class FormChangeInventory

#Region "定義"

    Private ReadOnly _menu As FormMenu
    Private ReadOnly _mode As String
    Private WithEvents dataGridViewInventories As DataGridViewInventories

#End Region

#Region "コンストラクタ"

    Public Sub New(parent As Form, mode As String)

        InitializeComponent()
        _menu = parent
        _mode = mode

    End Sub

#End Region

#Region "イベント"

    Private Sub Me_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        If _mode = "棚卸入力" Then
            Label1.Text = "棚卸日"
        Else
            Label1.Text = "調整日"
        End If
        ButtonImportStocktakingFile.Visible = (_mode = "棚卸入力")
        ButtonExportStocktakingFile.Visible = (_mode = "棚卸入力")
        ItemTypes.BindComboBox(ComboBoxItemType, True)

        dataGridViewInventories = New DataGridViewInventories(_mode)
        PanelInventories.Controls.Add(dataGridViewInventories)

        ' フォームの初期化
        InitializeForm(True)

    End Sub

    Private Sub Me_Shown(sender As Object, e As System.EventArgs) Handles Me.Shown

        AddHandler ComboBoxItemType.SelectedValueChanged, AddressOf ComboBoxItemType_SelectedIndexChanged
        AddHandler TextBoxItemId.TextChanged, AddressOf TextBoxItemId_TextChanged

    End Sub

    Private Sub ButtonInventoryDate_Click(sender As Object, e As EventArgs) Handles ButtonInventoryDateNext.Click, ButtonInventoryDatePrevious.Click

        If Not IsDate(DateBoxInventoryDate.Text) Then
            DateBoxInventoryDate.Text = Now.ToString("yyyy/MM/dd")
        Else
            If sender.Equals(ButtonInventoryDateNext) Then
                DateBoxInventoryDate.Text = CDate(DateBoxInventoryDate.Text).AddDays(1)
            Else
                DateBoxInventoryDate.Text = CDate(DateBoxInventoryDate.Text).AddDays(-1)
            End If
        End If
        DateBoxInventoryDate.Focus()

    End Sub

    Private Sub ComboBoxItemType_SelectedIndexChanged(sender As Object, e As EventArgs)

        dataGridViewInventories.DataBind(Nz(ComboBoxItemType.SelectedValue, String.Empty), TextBoxItemId.Text)
        UpdateInventoryTotal()

    End Sub

    Private Sub ButtonSearchItem_Click(sender As Object, e As EventArgs) Handles ButtonSearchItem.Click

        Dim dialog As New DialogSearchItems
        With dialog
            .ItemTypeFilter = "RecipeNeedFlag = 0"

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

    Private Sub ButtonImportStocktakingFile_Click(sender As Object, e As EventArgs) Handles ButtonImportStocktakingFile.Click

        Using dialog As New OpenFileDialog()
            dialog.Title = "棚卸データの読み込み"
            dialog.Filter = "棚卸ファイル(*.xlsx;*.csv)|*.xlsx;*.csv" &
                            "|Excelファイル(*.xlsx)|*.xlsx" &
                            "|CSVファイル(*.csv)|*.csv" &
                            "|すべてのファイル(*.*)|*.*"
            dialog.Multiselect = False
            If dialog.ShowDialog(Me) <> DialogResult.OK Then Exit Sub

            Try
                ImportStocktakingFile(dialog.FileName)
            Catch ex As IO.IOException
                MessageBox.Show("ファイルを読み込めませんでした。" & vbCrLf &
                                "Excelで開いたままになっていないか確認してください。",
                                Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Catch ex As Exception
                SysMsgBox(ex.Message, Me.Text)
            End Try
        End Using

    End Sub

    Private Sub TextBoxItemId_TextChanged(sender As Object, e As EventArgs)

        dataGridViewInventories.DataBind(Nz(ComboBoxItemType.SelectedValue, String.Empty), TextBoxItemId.Text)
        UpdateInventoryTotal()

    End Sub

    Private Sub dataGridViewInventories_HistoryClick(sender As Object, e As DataGridViewCellEventArgs) Handles dataGridViewInventories.HistoryClick

        Dim dgv As DataGridViewInventories = dataGridViewInventories

        If Not dgv.EndEdit() Then Exit Sub

        Dim row As DataGridViewRow = dgv.Rows(e.RowIndex)
        Dim inventoryId As String = Convert.ToString(row.Cells(dgv.TextBoxColumnInventoryId.Name).Value)

        If String.IsNullOrEmpty(inventoryId) Then Exit Sub

        Using dialog As New DialogInventoryLogs(inventoryId)
            dialog.ShowDialog(Me)
        End Using

    End Sub

    Public Sub dataGridViewInventories_RealQuantity_Validating(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles dataGridViewInventories.RealQuantity_Validating

        Dim dgv As DataGridViewInventories = dataGridViewInventories

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
            If CDbl(e.FormattedValue) < 0 Then
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

    Public Sub dataGridViewInventories_AjustQuantity_Validating(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles dataGridViewInventories.AjustQuantity_Validating

        Dim dgv As DataGridViewInventories = dataGridViewInventories

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

    Public Sub dataGridViewInventories_RowValidating(sender As Object, e As DataGridViewCellCancelEventArgs) Handles dataGridViewInventories.RowValidating

        ' 入力の妥当性は登録時にまとめてチェックする
        ' (在庫調整モードでは実在庫の列自体が存在しないため、ここで参照すると例外になる)

    End Sub

    Private Sub ButtonExportInventoryList_Click(sender As Object, e As EventArgs) Handles ButtonExportInventoryList.Click

        ExportInventoryList()

    End Sub

    Private Sub ButtonExportStocktakingFile_Click(sender As Object, e As EventArgs) Handles ButtonExportStocktakingFile.Click

        ExportStocktakingFile()

    End Sub

    Private Sub ButtonRegist_Click(sender As Object, e As EventArgs) Handles ButtonRegist.Click

        ' 更新前チェック
        If CheckData() = False Then
            Exit Sub
        End If

        ' レコード更新
        If MessageBox.Show("登録します。よろしいですか？", Me.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = DialogResult.Yes Then
            Try
                SaveData()
            Catch ex As Exception
                SysMsgBox(ex.Message, Me.Text)
                Exit Sub
            End Try
            ' 在庫が変わったので計算用のキャッシュを読み直す
            calc.ReloadInventories()
            InitializeForm()
        End If

    End Sub

#End Region

#Region "メソッド"

    Private Function CheckData() As Boolean

        If _mode = "棚卸入力" Then
            Return CheckStocktaking()
        Else
            Return CheckAdjust()
        End If

    End Function

    Private Sub SaveData()

        If _mode = "棚卸入力" Then
            SaveStocktaking()
        Else
            SaveAdjust()
        End If

    End Sub

    Private Function CheckStocktaking() As Boolean

        Dim result As Boolean = True

        Dim dgv As DataGridViewInventories = dataGridViewInventories

        ' 編集中の内容をセルに確定させる
        If Not dgv.EndEdit() Then Return False

        Try
            If String.IsNullOrEmpty(DateBoxInventoryDate.Text) Then
                DateBoxInventoryDate.Focus()
                Throw New ApplicationException("棚卸日が入力されていません。")
            End If

            Dim countedRows As Integer = 0
            Dim blankRows As Integer = 0
            Dim diffRows As Integer = 0

            For Each row As DataGridViewRow In dgv.Rows
                If row.IsNewRow Then Exit For

                ' 実在庫が未入力の行は数えていないものとして扱う
                If String.IsNullOrEmpty(Convert.ToString(row.Cells(dgv.TextBoxColumnBaseRealQuantity.Name).Value).Trim) Then
                    blankRows += 1
                    Continue For
                End If

                Dim real As Double = Nz(row.Cells(dgv.TextBoxColumnBaseRealQuantity.Name).Value, 0)
                If real < 0 Then
                    dgv.CurrentCell = dgv(dgv.TextBoxColumnRealQuantity.Name, row.Index)
                    Throw New ApplicationException("実在庫にマイナスは入力できません。")
                End If

                countedRows += 1

                Dim logical As Double = Nz(row.Cells(dgv.TextBoxColumnBaseLogicalQuantity.Name).Value, 0)
                If real <> logical Then diffRows += 1
            Next

            If countedRows = 0 Then
                Throw New ApplicationException("実在庫が入力されていません。")
            End If
            If diffRows = 0 Then
                Throw New ApplicationException("論理在庫との差異がありません。")
            End If

            ' 数えていない在庫が残っている場合は確認する
            If blankRows > 0 Then
                If MessageBox.Show(blankRows.ToString("#,##0") & "件の実在庫が未入力です。" & vbCrLf &
                                   "未入力の在庫はそのまま残ります。よろしいですか？",
                                   Me.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Question,
                                   MessageBoxDefaultButton.Button2) <> DialogResult.OK Then
                    Return False
                End If
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

    Private Function CheckAdjust() As Boolean

        Dim result As Boolean = True

        Dim dgv As DataGridViewInventories = dataGridViewInventories

        ' 編集中の内容をセルに確定させる
        If Not dgv.EndEdit() Then Return False

        Try
            If String.IsNullOrEmpty(DateBoxInventoryDate.Text) Then
                DateBoxInventoryDate.Focus()
                Throw New ApplicationException("調整日が入力されていません。")
            End If

            Dim count As Integer = 0
            For Each row As DataGridViewRow In dgv.Rows
                If row.IsNewRow Then Exit For

                Dim adjust As Double = Nz(row.Cells(dgv.TextBoxColumnBaseAdjustQuantity.Name).Value, 0)
                Dim reason As String = Convert.ToString(row.Cells(dgv.ComboBoxColumnAdjustType.Name).Value)

                ' どちらも空の行は調整対象ではない
                If adjust = 0 AndAlso String.IsNullOrEmpty(reason) Then Continue For

                If adjust = 0 Then
                    dgv.CurrentCell = dgv(dgv.TextBoxColumnAdjustQuantity.Name, row.Index)
                    Throw New ApplicationException("調整数が入力されていません。")
                End If
                If String.IsNullOrEmpty(reason) Then
                    dgv.CurrentCell = dgv(dgv.ComboBoxColumnAdjustType.Name, row.Index)
                    Throw New ApplicationException("調整理由が選択されていません。")
                End If

                Dim logical As Double = Nz(row.Cells(dgv.TextBoxColumnBaseLogicalQuantity.Name).Value, 0)
                If logical + adjust < 0 Then
                    dgv.CurrentCell = dgv(dgv.TextBoxColumnAdjustQuantity.Name, row.Index)
                    Throw New ApplicationException("調整後の在庫がマイナスになります。")
                End If

                count += 1
            Next

            If count = 0 Then
                Throw New ApplicationException("調整する明細が入力されていません。")
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

    Private Sub SaveStocktaking()

        Dim dgv As DataGridViewInventories = dataGridViewInventories

        ' 数えた結果を集める
        Dim idList As New List(Of String)
        Dim realMap As New Dictionary(Of String, Double)

        For Each row As DataGridViewRow In dgv.Rows
            If row.IsNewRow Then Exit For

            ' 未入力の行は触らない
            If String.IsNullOrEmpty(Convert.ToString(row.Cells(dgv.TextBoxColumnBaseRealQuantity.Name).Value).Trim) Then Continue For

            Dim real As Double = Nz(row.Cells(dgv.TextBoxColumnBaseRealQuantity.Name).Value, 0)
            Dim logical As Double = Nz(row.Cells(dgv.TextBoxColumnBaseLogicalQuantity.Name).Value, 0)
            ' 差異がなければ更新も記録もしない
            If real = logical Then Continue For

            Dim lotId As Integer
            If Not Integer.TryParse(Convert.ToString(row.Cells(dgv.TextBoxColumnInventoryId.Name).Value), lotId) Then Continue For

            idList.Add(lotId.ToString)
            realMap(lotId.ToString) = real
        Next

        If idList.Count = 0 Then Exit Sub

        ' 対象ロットを読み直して、在庫数(Weight)を数えた値で置き換える
        Dim inventoryList As List(Of Inventory) =
            Inventories.GetAll("InventoryId", "InventoryId in (" & String.Join(",", idList) & ")")

        Dim saveList As New List(Of Inventory)
        For Each inventory As Inventory In inventoryList
            If Not realMap.ContainsKey(inventory.InventoryId) Then Continue For

            inventory.Weight = realMap(inventory.InventoryId).ToString
            saveList.Add(inventory)
        Next

        Inventories.Save(saveList, InventoryLogStatus.Stocktaking)

    End Sub

    Private Sub SaveAdjust()

        Dim dgv As DataGridViewInventories = dataGridViewInventories

        ' 調整対象のロットを集める
        Dim idList As New List(Of String)
        Dim adjustMap As New Dictionary(Of String, Double)
        Dim reasonMap As New Dictionary(Of String, String)

        For Each row As DataGridViewRow In dgv.Rows
            If row.IsNewRow Then Exit For

            Dim adjust As Double = Nz(row.Cells(dgv.TextBoxColumnBaseAdjustQuantity.Name).Value, 0)
            If adjust = 0 Then Continue For

            Dim lotId As Integer
            If Not Integer.TryParse(Convert.ToString(row.Cells(dgv.TextBoxColumnInventoryId.Name).Value), lotId) Then Continue For

            idList.Add(lotId.ToString)
            adjustMap(lotId.ToString) = adjust
            reasonMap(lotId.ToString) = Convert.ToString(row.Cells(dgv.ComboBoxColumnAdjustType.Name).Value)
        Next

        If idList.Count = 0 Then Exit Sub

        ' 対象ロットを読み直して、在庫数(Weight)だけを増減する
        Dim inventoryList As List(Of Inventory) = Inventories.GetAll("InventoryId", "InventoryId in (" & String.Join(",", idList) & ")")

        Dim saveList As New List(Of Inventory)
        Dim reasonList As New List(Of String)
        For Each inventory As Inventory In inventoryList
            If Not adjustMap.ContainsKey(inventory.InventoryId) Then Continue For

            inventory.Weight = (CDbl(Nz(inventory.Weight, 0)) + adjustMap(inventory.InventoryId)).ToString
            saveList.Add(inventory)
            reasonList.Add(reasonMap(inventory.InventoryId))
        Next

        Inventories.Save(saveList, InventoryLogStatus.Adjust, reasonList)

    End Sub

    Private Sub InitializeForm(Optional isLoading As Boolean = False)

        DateBoxInventoryDate.Text = Now.ToString("yyyy/MM/dd")
        ComboBoxItemType.SelectedIndex = -1
        TextBoxItemId.Text = String.Empty
        TextBoxItemName.Text = String.Empty

        ' リストの初期化
        dataGridViewInventories.DataBind(Nz(ComboBoxItemType.SelectedValue, String.Empty), TextBoxItemId.Text)
        UpdateInventoryTotal()

    End Sub

    ' 在庫一覧表をCSVで出力する
    Private Sub ExportInventoryList()

        ' 絞り込み中であることを知らせる
        If Not String.IsNullOrEmpty(Nz(ComboBoxItemType.SelectedValue, String.Empty)) OrElse
               Not String.IsNullOrEmpty(TextBoxItemId.Text) Then
            If MessageBox.Show("品目が絞り込まれています。表示中の在庫のみ出力します。" & vbCrLf &
                               "よろしいですか？",
                               Me.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) <> DialogResult.OK Then
                Exit Sub
            End If
        End If

        Dim dgv As DataGridViewInventories = dataGridViewInventories

        ' 滞留日数・残日数の基準日
        Dim baseDate As Date = Now.Date
        If Not String.IsNullOrEmpty(DateBoxInventoryDate.Text) Then
            Date.TryParse(DateBoxInventoryDate.Text, baseDate)
        End If

        Using dialog As New SaveFileDialog()
            dialog.Title = "在庫一覧表の出力"
            dialog.Filter = "CSVファイル(*.csv)|*.csv"
            dialog.FileName = "在庫一覧表_" & baseDate.ToString("yyyyMMdd") & ".csv"
            If dialog.ShowDialog(Me) <> DialogResult.OK Then Exit Sub

            Dim exportRows As Integer = 0

            Try
                Using writer As New IO.StreamWriter(dialog.FileName, False, System.Text.Encoding.GetEncoding("Shift_JIS"))
                    writer.WriteLine("在庫ID,品目区分,品目ID,品目名,入荷日,滞留日数,賞味期限,残日数,在庫数,単位,基準単価,在庫金額")

                    For Each row As DataGridViewRow In dgv.Rows
                        If row.IsNewRow Then Exit For

                        Dim column As New List(Of String)
                        column.Add(Convert.ToString(row.Cells(dgv.TextBoxColumnInventoryId.Name).Value))
                        column.Add(CsvField(Convert.ToString(row.Cells(dgv.TextBoxColumnItemTypeName.Name).Value)))
                        column.Add(Convert.ToString(row.Cells(dgv.TextBoxColumnItemId.Name).Value))
                        column.Add(CsvField(Convert.ToString(row.Cells(dgv.TextBoxColumnItemName.Name).Value)))

                        ' 入荷日と滞留日数
                        Dim arrivalDate As Date
                        If Date.TryParse(Convert.ToString(row.Cells(dgv.dateBoxColumnArrivalDate.Name).Value), arrivalDate) Then
                            column.Add(arrivalDate.ToString("yyyy/MM/dd"))
                            column.Add(baseDate.Subtract(arrivalDate).Days.ToString)
                        Else
                            column.Add(String.Empty)
                            column.Add(String.Empty)
                        End If

                        ' 賞味期限と残日数(期限を管理しない品目は空欄になる)
                        Dim expiryDate As Date
                        If Date.TryParse(Convert.ToString(row.Cells(dgv.dateBoxColumnExpiryDate.Name).Value), expiryDate) Then
                            column.Add(expiryDate.ToString("yyyy/MM/dd"))
                            column.Add(expiryDate.Subtract(baseDate).Days.ToString)
                        Else
                            column.Add(String.Empty)
                            column.Add(String.Empty)
                        End If

                        column.Add(CDbl(Nz(row.Cells(dgv.TextBoxColumnLogicalQuantity.Name).Value, 0)).ToString("0.########"))
                        column.Add(CsvField(Convert.ToString(row.Cells(dgv.textBoxColumnLogicalUnitTypeName.Name).Value)))
                        column.Add(Nz(row.Cells(dgv.TextBoxColumnPricePerGram.Name).Value, 0).ToString)
                        column.Add(Math.Round(CDbl(Nz(row.Cells(dgv.textBoxColumnInventoryValue.Name).Value, 0)), 2).ToString)

                        writer.WriteLine(String.Join(",", column))
                        exportRows += 1
                    Next
                End Using
            Catch ex As Exception
                SysMsgBox(ex.Message, Me.Text)
                Exit Sub
            End Try

            If exportRows = 0 Then
                MessageBox.Show("出力する在庫がありません。", Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            MessageBox.Show(exportRows.ToString("#,##0") & "件を出力しました。", Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Using

    End Sub

    ' 棚卸ファイル(Excel/CSV)を読み込んで実在庫列に反映する(登録はしない)
    Private Sub ImportStocktakingFile(fileName As String)

        Dim realMap As New Dictionary(Of String, Double)
        Dim errorList As New List(Of String)

        If IO.Path.GetExtension(fileName).ToLower = ".csv" Then
            ReadStocktakingCsv(fileName, realMap, errorList)
        Else
            ReadStocktakingExcel(fileName, realMap, errorList)
        End If

        If errorList.Count > 0 Then
            MessageBox.Show("読み込めない行があります。" & vbCrLf & vbCrLf &
                            String.Join(vbCrLf, errorList.Take(10)),
                            Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        ApplyStocktakingValues(realMap)

    End Sub

    ' 棚卸CSVから実在庫を読み取る
    Private Sub ReadStocktakingCsv(fileName As String,
                                   realMap As Dictionary(Of String, Double),
                                   errorList As List(Of String))

        Dim lineNo As Integer = 0

        Using parser As New FileIO.TextFieldParser(fileName, System.Text.Encoding.GetEncoding("Shift_JIS"))
            parser.TextFieldType = FileIO.FieldType.Delimited
            parser.SetDelimiters(",")
            parser.HasFieldsEnclosedInQuotes = True

            While Not parser.EndOfData
                Dim fields As String() = parser.ReadFields()
                lineNo += 1

                ' 1行目は見出しなので読み飛ばす
                If lineNo = 1 Then Continue While
                If fields.Length < 8 Then Continue While

                Dim lotId As Integer
                If Not Integer.TryParse(fields(0).Trim, lotId) Then
                    errorList.Add(lineNo.ToString & "行目: 在庫IDが正しくありません。")
                    Continue While
                End If

                ' 実在庫が空の行は数えていないものとして読み飛ばす
                Dim realText As String = fields(7).Trim
                If String.IsNullOrEmpty(realText) Then Continue While

                AddStocktakingValue(realMap, errorList, lineNo, lotId, realText)
            End While
        End Using

    End Sub

    ' 棚卸Excelから実在庫を読み取る
    Private Sub ReadStocktakingExcel(fileName As String, realMap As Dictionary(Of String, Double), errorList As List(Of String))

        Const ROW_HEADER As Integer = 4
        Const COL_INVENTORYID As Integer = 1
        Const COL_REAL As Integer = 8
        Const COL_REALUNIT As Integer = 9

        Using book As New XLWorkbook(fileName)
            Dim sheet As IXLWorksheet = Nothing
            For Each target As IXLWorksheet In book.Worksheets
                If target.Name = "棚卸" Then
                    sheet = target
                    Exit For
                End If
            Next

            If sheet Is Nothing Then sheet = book.Worksheet(1)

            Dim header As String = sheet.Cell(ROW_HEADER, COL_REALUNIT).GetString().Trim()
            Dim hasRealUnit As Boolean = header = "単位" OrElse header = "実在庫単位"

            If Not hasRealUnit AndAlso header <> "差異" Then
                errorList.Add("棚卸調査表の形式が正しくありません。")
                Exit Sub
            End If

            For Each row As IXLRow In sheet.RowsUsed()
                Dim lotId As Integer
                If Not Integer.TryParse(row.Cell(COL_INVENTORYID).GetString().Trim(), lotId) Then Continue For
                Dim realCell As IXLCell = row.Cell(COL_REAL)
                If realCell.IsEmpty() Then Continue For
                Dim realText As String = realCell.GetString().Trim()
                If String.IsNullOrEmpty(realText) Then Continue For

                If hasRealUnit Then
                    Dim unitName As String = row.Cell(COL_REALUNIT).GetString().Trim()
                    If String.IsNullOrEmpty(unitName) Then
                        errorList.Add(row.RowNumber.ToString() & "行目: 実在庫単位が入力されていません。")
                        Continue For
                    End If
                    AddStocktakingValue(realMap, errorList, row.RowNumber, lotId, realText, unitName)
                Else
                    ' 旧形式のExcelは、従来どおり基準数量として読み込む
                    AddStocktakingValue(realMap, errorList, row.RowNumber, lotId, realText)
                End If
            Next
        End Using

    End Sub

    ' 読み取った実在庫を検査して集計する(同じロットが複数行ある場合は合算する)
    Private Sub AddStocktakingValue(realMap As Dictionary(Of String, Double), errorList As List(Of String), lineNo As Integer, lotId As Integer, realText As String, Optional unitName As String = Nothing)

        If Not IsNumeric(realText) Then
            errorList.Add(lineNo.ToString() & "行目: 実在庫が数値ではありません。")
            Exit Sub
        End If

        Dim real As Double = CDbl(realText)
        If Double.IsNaN(real) OrElse Double.IsInfinity(real) Then
            errorList.Add(lineNo.ToString() & "行目: 実在庫が正しくありません。")
            Exit Sub
        End If
        If real < 0 Then
            errorList.Add(lineNo.ToString() & "行目: 実在庫がマイナスです。")
            Exit Sub
        End If

        If unitName IsNot Nothing Then
            Dim dgv As DataGridViewInventories = dataGridViewInventories
            Dim targetRow As DataGridViewRow = Nothing
            For Each row As DataGridViewRow In dgv.Rows
                If row.IsNewRow Then Continue For
                If Convert.ToString(row.Cells(dgv.TextBoxColumnInventoryId.Name).Value) = lotId.ToString() Then
                    targetRow = row
                    Exit For
                End If
            Next

            If targetRow Is Nothing Then
                errorList.Add(lineNo.ToString() & "行目: 対象の在庫が表示中の一覧にありません。対象品目を表示してから取り込んでください。")
                Exit Sub
            End If

            Dim itemId As Integer = CInt(targetRow.Cells(dgv.TextBoxColumnItemId.Name).Value)
            Dim units As List(Of UnitType) = dgv.GetQuantityUnits(itemId)
            Dim matches As List(Of UnitType) = units.Where(Function(x) String.Equals(x.UnitTypeName.Trim(), unitName.Trim(), StringComparison.OrdinalIgnoreCase)).ToList()

            If matches.Count <> 1 Then
                errorList.Add(lineNo.ToString() & "行目: この品目では実在庫単位「" & unitName & "」を特定できません。")
                Exit Sub
            End If

            real *= dgv.GetQuantityFactor(itemId, CInt(matches(0).UnitTypeId))
            If Double.IsNaN(real) OrElse Double.IsInfinity(real) Then
                errorList.Add(lineNo.ToString() & "行目: 換算後の実在庫が正しくありません。")
                Exit Sub
            End If
        End If

        If realMap.ContainsKey(lotId.ToString()) Then
            realMap(lotId.ToString()) += real
        Else
            realMap(lotId.ToString()) = real
        End If

    End Sub

    ' 読み取った実在庫をグリッドに反映する
    Private Sub ApplyStocktakingValues(realMap As Dictionary(Of String, Double))

        Dim dgv As DataGridViewInventories = dataGridViewInventories

        Dim importedRows As Integer = 0
        For Each row As DataGridViewRow In dgv.Rows
            If row.IsNewRow Then Exit For

            Dim lotId As String = Convert.ToString(row.Cells(dgv.TextBoxColumnInventoryId.Name).Value)
            If Not realMap.ContainsKey(lotId) Then Continue For

            ' 基準数量へ数値で反映する。表示と差異はCellValueChangedで更新する
            row.Cells(dgv.TextBoxColumnBaseRealQuantity.Name).Value = realMap(lotId)
            importedRows += 1
            realMap.Remove(lotId)
        Next

        Dim message As String = importedRows.ToString("#,##0") & "件を読み込みました。"
        If realMap.Count > 0 Then
            message &= vbCrLf & realMap.Count.ToString("#,##0") & "件は表示中の一覧に無いため取り込みませんでした。"
        End If
        MessageBox.Show(message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Information)

    End Sub

    ' 棚卸調査表を出力する(実在庫列は空で出す)
    Private Sub ExportStocktakingFile()

        Dim dgv As DataGridViewInventories = dataGridViewInventories

        If dgv.Rows.Count = 0 Then
            MessageBox.Show("出力する在庫がありません。", Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Dim baseDate As Date = Now.Date
        If Not String.IsNullOrEmpty(DateBoxInventoryDate.Text) Then
            Date.TryParse(DateBoxInventoryDate.Text, baseDate)
        End If

        Using dialog As New SaveFileDialog()
            dialog.Title = "棚卸調査表の出力"
            dialog.Filter = "Excelファイル(*.xlsx)|*.xlsx|CSVファイル(*.csv)|*.csv"
            dialog.FilterIndex = 1
            dialog.FileName = "棚卸調査表_" & baseDate.ToString("yyyyMMdd") & ".xlsx"
            If dialog.ShowDialog(Me) <> DialogResult.OK Then Exit Sub

            ' 拡張子が付いていないときは選択中のフィルタで補う
            Dim fileName As String = dialog.FileName
            Dim extension As String = IO.Path.GetExtension(fileName).ToLower
            If extension <> ".xlsx" AndAlso extension <> ".csv" Then
                If dialog.FilterIndex = 2 Then
                    extension = ".csv"
                Else
                    extension = ".xlsx"
                End If
                fileName &= extension
            End If

            Try
                If extension = ".csv" Then
                    ExportStocktakingCsv(fileName)
                Else
                    ExportStocktakingExcel(fileName, baseDate)
                End If
            Catch ex As IO.IOException
                MessageBox.Show("ファイルを保存できませんでした。" & vbCrLf &
                                "同じファイルをExcelで開いたままになっていないか確認してください。",
                                Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            Catch ex As Exception
                SysMsgBox(ex.Message, Me.Text)
                Exit Sub
            End Try

            MessageBox.Show("出力しました。", Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Using

    End Sub

    ' 棚卸調査表をCSVで出力する
    Private Sub ExportStocktakingCsv(fileName As String)

        Dim dgv As DataGridViewInventories = dataGridViewInventories

        Using writer As New IO.StreamWriter(fileName, False, System.Text.Encoding.GetEncoding("Shift_JIS"))
            writer.WriteLine("InventoryId,ItemId,ItemName,ExpiryDate,ArrivalDate,LogicalQuantity,UnitTypeName,RealQuantity")

            For Each row As DataGridViewRow In dgv.Rows
                If row.IsNewRow Then Exit For

                Dim column As New List(Of String)
                column.Add(Convert.ToString(row.Cells(dgv.TextBoxColumnInventoryId.Name).Value))
                column.Add(Convert.ToString(row.Cells(dgv.TextBoxColumnItemId.Name).Value))
                column.Add(CsvField(Convert.ToString(row.Cells(dgv.TextBoxColumnItemName.Name).Value)))
                column.Add(FormatCellDate(row.Cells(dgv.dateBoxColumnExpiryDate.Name).Value))
                column.Add(FormatCellDate(row.Cells(dgv.dateBoxColumnArrivalDate.Name).Value))
                column.Add(Nz(row.Cells(dgv.TextBoxColumnBaseLogicalQuantity.Name).Value, 0).ToString)
                column.Add(calc.GetBaseUnitTypeName(row.Cells(dgv.TextBoxColumnItemId.Name).Value))
                column.Add(String.Empty)

                writer.WriteLine(String.Join(",", column))
            Next
        End Using

    End Sub

    ' 棚卸調査表をExcelで出力する
    Private Sub ExportStocktakingExcel(fileName As String, baseDate As Date)

        Dim dgv As DataGridViewInventories = dataGridViewInventories

        Const ROW_HEADER As Integer = 4
        Const COL_INVENTORYID As Integer = 1
        Const COL_ITEMID As Integer = 2
        Const COL_ITEMNAME As Integer = 3
        Const COL_EXPIRYDATE As Integer = 4
        Const COL_ARRIVALDATE As Integer = 5
        Const COL_LOGICAL As Integer = 6
        Const COL_UNIT As Integer = 7
        Const COL_REAL As Integer = 8
        Const COL_REALUNIT As Integer = 9
        Const COL_DIFF As Integer = 10
        Const COL_DIFFUNIT As Integer = 11
        Const COL_BASELOGICAL As Integer = 12
        Const COL_LOGICALFACTOR As Integer = 13

        Using book As New XLWorkbook()

            Dim sheet As IXLWorksheet = book.Worksheets.Add("棚卸")
            Dim unitSheet As IXLWorksheet = book.Worksheets.Add("単位候補")
            sheet.Style.Font.FontName = "MS UI Gothic"

            ' 表題
            With sheet.Cell(1, COL_INVENTORYID)
                .Value = "棚卸調査表"
                .Style.Font.SetBold(True)
                .Style.Font.SetFontSize(14)
            End With
            With sheet.Cell(2, COL_INVENTORYID)
                .Value = "棚卸日: " & baseDate.ToString("yyyy/MM/dd")
                .Style.Font.SetBold(True)
                .Style.Font.SetFontSize(11)
            End With
            With sheet.Cell(2, COL_DIFFUNIT)
                .Value = "出力日時: " & Now.ToString("yyyy/MM/dd HH:mm")
                .Style.Font.SetFontSize(9)
                .Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right)
            End With
            With sheet.Cell(3, COL_INVENTORYID)
                .Value = "※ 実在庫と実在庫単位を記入してください。未確認の行は実在庫を空欄にします。単位変更時、入力数量は変わりません。"
                .Style.Font.SetFontSize(9)
            End With

            ' 見出し
            Dim headers As String() = {"在庫ID", "品目ID", "品目名", "賞味期限", "入荷日", "論理在庫", "単位", "実在庫", "単位", "差異", "単位"}
            For i As Integer = 0 To headers.Length - 1
                With sheet.Cell(ROW_HEADER, i + 1)
                    .Value = headers(i)
                    .Style.Font.SetBold(True)
                    .Style.Fill.SetBackgroundColor(XLColor.FromArgb(217, 217, 217))
                    .Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                End With
            Next
            ' 記入する列だけ色を変えて目立たせる
            sheet.Cell(ROW_HEADER, COL_REAL).Style.Fill.SetBackgroundColor(XLColor.FromArgb(255, 217, 102))
            sheet.Cell(ROW_HEADER, COL_REALUNIT).Style.Fill.SetBackgroundColor(XLColor.FromArgb(255, 217, 102))

            ' 明細
            Dim rowIndex As Integer = ROW_HEADER
            For Each row As DataGridViewRow In dgv.Rows
                If row.IsNewRow Then Exit For
                rowIndex += 1

                sheet.Cell(rowIndex, COL_INVENTORYID).Value = CInt(Nz(row.Cells(dgv.TextBoxColumnInventoryId.Name).Value, 0))
                sheet.Cell(rowIndex, COL_ITEMID).Value = CInt(Nz(row.Cells(dgv.TextBoxColumnItemId.Name).Value, 0))
                sheet.Cell(rowIndex, COL_ITEMNAME).Value = Convert.ToString(row.Cells(dgv.TextBoxColumnItemName.Name).Value)

                ' 賞味期限(管理しない品目は空欄になる)
                Dim expiryDate As Date
                If Date.TryParse(Convert.ToString(row.Cells(dgv.dateBoxColumnExpiryDate.Name).Value), expiryDate) Then
                    With sheet.Cell(rowIndex, COL_EXPIRYDATE)
                        .Value = expiryDate
                        .Style.DateFormat.Format = "yyyy/mm/dd"
                    End With
                End If

                ' 入荷日
                Dim arrivalDate As Date
                If Date.TryParse(Convert.ToString(row.Cells(dgv.dateBoxColumnArrivalDate.Name).Value), arrivalDate) Then
                    With sheet.Cell(rowIndex, COL_ARRIVALDATE)
                        .Value = arrivalDate
                        .Style.DateFormat.Format = "yyyy/mm/dd"
                    End With
                End If

                Dim itemId As Integer = CInt(row.Cells(dgv.TextBoxColumnItemId.Name).Value)
                Dim displayUnitId As Integer = CInt(row.Cells(dgv.TextBoxColumnDisplayUnitTypeId.Name).Value)
                Dim logicalFactor As Double = dgv.GetQuantityFactor(itemId, displayUnitId)
                Dim baseLogical As Double = CDbl(Nz(row.Cells(dgv.TextBoxColumnBaseLogicalQuantity.Name).Value, 0))
                Dim unitName As String = Convert.ToString(row.Cells(dgv.textBoxColumnLogicalUnitTypeName.Name).Value)
                Dim units As List(Of UnitType) = dgv.GetQuantityUnits(itemId)

                ' 論理在庫は画面と同じ数量・単位で出力する
                sheet.Cell(rowIndex, COL_LOGICAL).Value = baseLogical / logicalFactor
                sheet.Cell(rowIndex, COL_LOGICAL).Style.NumberFormat.Format = "#,##0.########"
                sheet.Cell(rowIndex, COL_UNIT).Value = unitName

                ' 差異計算用の基準数量・表示係数
                sheet.Cell(rowIndex, COL_BASELOGICAL).Value = baseLogical
                sheet.Cell(rowIndex, COL_LOGICALFACTOR).Value = logicalFactor

                ' 実在庫は空欄で出力する
                With sheet.Cell(rowIndex, COL_REAL)
                    .Style.Fill.SetBackgroundColor(XLColor.FromArgb(255, 242, 204))
                    .Style.NumberFormat.Format = "#,##0.########"
                    .Style.Protection.SetLocked(False)
                End With

                ' 実在庫単位の初期値は論理在庫と同じにする
                With sheet.Cell(rowIndex, COL_REALUNIT)
                    .Value = unitName
                    .Style.Fill.SetBackgroundColor(XLColor.FromArgb(255, 242, 204))
                    .Style.Protection.SetLocked(False)
                End With

                sheet.Cell(rowIndex, COL_DIFFUNIT).Value = unitName

                ' 品目ごとの単位名・換算係数を別シートへ出力する
                Dim unitRow As Integer = rowIndex * 2

                For i As Integer = 0 To units.Count - 1
                    unitSheet.Cell(unitRow, i + 1).Value = units(i).UnitTypeName
                    unitSheet.Cell(unitRow + 1, i + 1).Value = dgv.GetQuantityFactor(itemId, CInt(units(i).UnitTypeId))
                Next

                Dim unitNames As IXLRange = unitSheet.Range(unitRow, 1, unitRow, units.Count)
                Dim unitFactors As IXLRange = unitSheet.Range(unitRow + 1, 1, unitRow + 1, units.Count)
                Dim listName As String = "StockUnits" & rowIndex.ToString()

                ' 別シートの候補を、名前付き範囲で入力規則に使用する
                unitNames.AddToNamed(listName)

                Dim validation As IXLDataValidation = sheet.Cell(rowIndex, COL_REALUNIT).DataValidation
                validation.List(listName, True)
                validation.IgnoreBlanks = False
                validation.ShowErrorMessage = True

                ' 実在庫を基準数量へ換算してから、論理在庫との差異を求める
                Dim realAddress As String = sheet.Cell(rowIndex, COL_REAL).Address.ToStringRelative()
                Dim realUnitAddress As String = sheet.Cell(rowIndex, COL_REALUNIT).Address.ToStringRelative()
                Dim baseLogicalAddress As String = sheet.Cell(rowIndex, COL_BASELOGICAL).Address.ToStringRelative()
                Dim logicalFactorAddress As String = sheet.Cell(rowIndex, COL_LOGICALFACTOR).Address.ToStringRelative()
                Dim namesAddress As String = "'単位候補'!" & unitNames.FirstCell().Address.ToStringRelative() & ":" & unitNames.LastCell().Address.ToStringRelative()
                Dim factorsAddress As String = "'単位候補'!" & unitFactors.FirstCell().Address.ToStringRelative() & ":" & unitFactors.LastCell().Address.ToStringRelative()

                Dim differenceFormula As String = "(" & realAddress & "*SUMIF(" & namesAddress & "," & realUnitAddress & "," & factorsAddress & ")-" & baseLogicalAddress & ")/" & logicalFactorAddress
                Dim validUnitFormula As String = "COUNTIF(" & namesAddress & "," & realUnitAddress & ")=1"

                With sheet.Cell(rowIndex, COL_DIFF)
                    .FormulaA1 = "IF(" & realAddress & "="""","""",IF(" & validUnitFormula & "," & differenceFormula & ",""単位エラー""))"
                    .Style.NumberFormat.Format = "#,##0.########;-#,##0.########;0"
                End With
            Next

            Dim rowLast As Integer = rowIndex

            ' 罫線
            With sheet.Range(ROW_HEADER, COL_INVENTORYID, rowLast, COL_DIFFUNIT).Style.Border
                .OutsideBorder = XLBorderStyleValues.Thin
                .InsideBorder = XLBorderStyleValues.Thin
            End With

            ' 列幅
            sheet.Column(COL_INVENTORYID).Width = 9
            sheet.Column(COL_ITEMID).Width = 8
            sheet.Column(COL_ITEMNAME).Width = 30
            sheet.Column(COL_EXPIRYDATE).Width = 12
            sheet.Column(COL_ARRIVALDATE).Width = 12
            sheet.Column(COL_LOGICAL).Width = 12
            sheet.Column(COL_UNIT).Width = 6
            sheet.Column(COL_REAL).Width = 12
            sheet.Column(COL_REALUNIT).Width = 10
            sheet.Column(COL_DIFF).Width = 12
            sheet.Column(COL_DIFFUNIT).Width = 6

            ' 印刷設定
            sheet.SheetView.FreezeRows(ROW_HEADER)
            With sheet.PageSetup
                .PageOrientation = XLPageOrientation.Portrait
                .PaperSize = XLPaperSize.A4Paper
                .PagesWide = 1
                .PagesTall = 0
                .Margins.Top = 0.6
                .Margins.Bottom = 0.6
                .Margins.Left = 0.4
                .Margins.Right = 0.4
                .SetRowsToRepeatAtTop(ROW_HEADER, ROW_HEADER)
                .Footer.Right.AddText("&P / &N")
            End With

            ' 実在庫の欄だけ入力できるようにシートを保護する
            ' (在庫IDや理論在庫を書き換えられると取り込みが狂うため。不要ならこの1行を消す)
            ' 計算用の列と単位候補シートは表示しない
            sheet.Column(COL_BASELOGICAL).Hide()
            sheet.Column(COL_LOGICALFACTOR).Hide()
            unitSheet.Protect()
            unitSheet.Visibility = XLWorksheetVisibility.VeryHidden
            Dim printArea As String = sheet.Cell(1, COL_INVENTORYID).Address.ToStringRelative() & ":" &
                                      sheet.Cell(rowLast, COL_DIFFUNIT).Address.ToStringRelative()
            sheet.PageSetup.PrintAreas.Add(printArea)
            sheet.Protect()

            book.SaveAs(fileName)
        End Using

    End Sub

    ' 表示中の在庫金額を合計する
    Private Sub UpdateInventoryTotal()

        Dim total As Decimal = 0
        Dim dgv As DataGridViewInventories = dataGridViewInventories

        For Each row As DataGridViewRow In dgv.Rows
            If row.IsNewRow Then Continue For
            Dim value As Object = row.Cells(dgv.textBoxColumnInventoryValue.Name).Value
            If IsNumeric(value) Then
                ' 在庫一覧CSVと同じく、各行を小数点以下2桁に丸める
                total += Math.Round(CDec(value), 2)
            End If
        Next

        LabelInventoryTotal.Text = "在庫金額合計：" & total.ToString("#,##0") & " 円"

    End Sub

#End Region

End Class

Public Class DataGridViewInventories
    Inherits CustomDataGridView

#Region "定義"

    Public ButtonColumnHistory As New CustomDatagridViewButtonColumn
    Public TextBoxColumnItemTypeName As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnItemName As New CustomDataGridViewTextBoxColumn
    Public dateBoxColumnArrivalDate As New CustomDataGridViewDateBoxColumn
    Public dateBoxColumnExpiryDate As New CustomDataGridViewDateBoxColumn
    Public TextBoxColumnPricePerGram As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnLogicalQuantity As New CustomDataGridViewTextBoxColumn
    Public textBoxColumnLogicalUnitTypeName As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnRealQuantity As New CustomDataGridViewTextBoxColumn
    Public ComboBoxColumnRealUnitType As New CustomDataGridViewComboBoxColumn
    Public TextBoxColumnDifference As New CustomDataGridViewTextBoxColumn
    Public textBoxColumnDifferenceUnitTypeName As New CustomDataGridViewTextBoxColumn
    Public textBoxColumnInventoryValue As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnAdjustQuantity As New CustomDataGridViewTextBoxColumn
    Public ComboBoxColumnAdjustUnitType As New CustomDataGridViewComboBoxColumn
    Public ComboBoxColumnAdjustType As New CustomDataGridViewComboBoxColumn
    Public TextBoxColumnItemId As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnInventoryId As New CustomDataGridViewTextBoxColumn

    Public TextBoxColumnDisplayUnitTypeId As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnBaseLogicalQuantity As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnBaseRealQuantity As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnBaseAdjustQuantity As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnBaseDifference As New CustomDataGridViewTextBoxColumn

    Public Event SearchItemClick(sender As Object, e As DataGridViewCellEventArgs)
    Public Event RealQuantity_Validating(sender As Object, e As DataGridViewCellValidatingEventArgs)
    Public Event AjustQuantity_Validating(sender As Object, e As DataGridViewCellValidatingEventArgs)
    Public Event AdjustType_SelectedIndexChanged(sender As Object, selectedValue As Object, e As EventArgs)

    Private isUpdatingQuantity As Boolean
    Private unitTypeList As New List(Of UnitType)

    Public isDataBinding As Boolean
    Private comboBoxEditingControl As New CustomDataGridViewComboBoxEditingControl

    Private _mode As String
    Private itemTypeBackColors As New Dictionary(Of String, Color)

    Public Event HistoryClick(sender As Object, e As DataGridViewCellEventArgs)

#End Region

#Region "コンストラクタ"

    Public Sub New(mode As String)

        _mode = mode

        With ButtonColumnHistory
            .Name = "ButtonColumnHistory"
            .HeaderText = String.Empty
            .Width = 50
            .DefaultCellStyle.Font = New Font("MS UI Gothic", 11.25)
            .DefaultCellStyle.NullValue = "履歴"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .UseColumnTextForButtonValue = True
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .TabStop = False
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
        With dateBoxColumnArrivalDate
            .Name = "dateBoxColumnArrivalDate"
            .DataPropertyName = "ArrivalDate"
            .HeaderText = "入荷日"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .DefaultCellStyle.Format = "yyyy/MM/dd"
            .DefaultCellStyle.SelectionForeColor = Color.Black
            .DefaultCellStyle.SelectionBackColor = Color.White
            .Width = 100
            .ReadOnly = True
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .TabStop = False
        End With
        With dateBoxColumnExpiryDate
            .Name = "dateBoxColumnExpiryDate"
            .DataPropertyName = "ExpiryDate"
            .HeaderText = "賞味期限"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .DefaultCellStyle.Format = "yyyy/MM/dd"
            .DefaultCellStyle.SelectionForeColor = Color.Black
            .DefaultCellStyle.SelectionBackColor = Color.White
            .Width = 100
            .ReadOnly = True
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .TabStop = False
        End With
        With TextBoxColumnPricePerGram
            .Name = "TextBoxColumnPricePerGram"
            .DataPropertyName = "PricePerGram"
            .HeaderText = "基準単価"
            .Width = 100
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .DefaultCellStyle.Format = "#,##0.##"
            .DefaultCellStyle.SelectionForeColor = Color.Black
            .DefaultCellStyle.SelectionBackColor = Color.White
            .ReadOnly = True
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .TabStop = False
        End With
        With TextBoxColumnLogicalQuantity
            .Name = "TextBoxColumnLogical"
            .DataPropertyName = "LogicalQuantity"
            .HeaderText = String.Empty '見出しは結合ヘッダーで描くので空
            .Width = 100
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .DefaultCellStyle.Format = "#,##0.##"
            .DefaultCellStyle.SelectionForeColor = Color.Black
            .DefaultCellStyle.SelectionBackColor = Color.White
            .ReadOnly = True
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .TabStop = False
        End With
        With textBoxColumnLogicalUnitTypeName
            .Name = "textBoxColumnLogicalUnitTypeName"
            .DataPropertyName = "LogicalUnitTypeName"
            .HeaderText = String.Empty '見出しは結合ヘッダーで描くので空
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
            .HeaderText = String.Empty '見出しは結合ヘッダーで描くので空
            .Width = 100
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .DefaultCellStyle.Format = "#,##0.##"
            .DefaultCellStyle.SelectionForeColor = Color.Black
            .DefaultCellStyle.SelectionBackColor = Color.White
            .ReadOnly = False
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .NumericOnly = True
            .AllowSymbolChar = True
            .TabStop = True
        End With
        With ComboBoxColumnRealUnitType
            .Name = "ComboBoxColumnRealUnitType"
            .HeaderText = String.Empty
            .Width = 70
            .DisplayMember = "UnitTypeName"
            .ValueMember = "UnitTypeId"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .TabStop = True
        End With
        With TextBoxColumnDifference
            .Name = "TextBoxColumnDifference"
            .DataPropertyName = "Difference"
            .HeaderText = String.Empty
            .Width = 100
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .DefaultCellStyle.Format = "#,##0.##;-#,##0.##"
            .DefaultCellStyle.SelectionForeColor = Color.Black
            .DefaultCellStyle.SelectionBackColor = Color.White
            .ReadOnly = True
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .TabStop = False
        End With
        With textBoxColumnDifferenceUnitTypeName
            .Name = "textBoxColumnDifferenceUnitTypeName"
            .HeaderText = String.Empty
            .Width = 60
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            .DefaultCellStyle.SelectionForeColor = Color.Black
            .DefaultCellStyle.SelectionBackColor = Color.White
            .ReadOnly = True
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .TabStop = False
        End With
        With textBoxColumnInventoryValue
            .Name = "textBoxColumnInventoryValue"
            .DataPropertyName = "InventoryValue"
            .HeaderText = "在庫金額"
            .Width = 120
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .DefaultCellStyle.Format = "#,##0.##"
            .DefaultCellStyle.SelectionForeColor = Color.Black
            .DefaultCellStyle.SelectionBackColor = Color.White
            .ReadOnly = True
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .TabStop = False
        End With
        With TextBoxColumnAdjustQuantity
            .Name = "TextBoxColumnAdjustQuantity"
            .DataPropertyName = "AdjustQuantity"
            .HeaderText = String.Empty
            .Width = 100
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            .DefaultCellStyle.Format = "#,##0.##;-#,##0.##"
            .DefaultCellStyle.SelectionForeColor = Color.Black
            .DefaultCellStyle.SelectionBackColor = Color.White
            .ReadOnly = False
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .NumericOnly = True
            .AllowSymbolChar = True
            .TabStop = True
        End With
        With ComboBoxColumnAdjustUnitType
            .Name = "ComboBoxColumnAdjustUnitType"
            .HeaderText = String.Empty
            .Width = 70
            .DisplayMember = "UnitTypeName"
            .ValueMember = "UnitTypeId"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .TabStop = True
        End With
        With ComboBoxColumnAdjustType
            .Name = "ComboBoxColumnAdjustType"
            .DataPropertyName = "AdjustType"
            .HeaderText = "調整理由"
            .Width = 150
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .TabStop = True

            If mode <> "棚卸入力" Then AjustTypes.BindComboBox(ComboBoxColumnAdjustType, True, "UnusedFlag=0")
        End With

        With TextBoxColumnItemId
            .Name = "TextBoxColumnItemId"
            .DataPropertyName = "ItemId"
            .HeaderText = "品目ID"
            .Visible = False
        End With
        With TextBoxColumnInventoryId
            .Name = "TextBoxColumnInventoryId"
            .DataPropertyName = "InventoryId"
            .HeaderText = "在庫ID"
            .Visible = False
        End With

        With TextBoxColumnDisplayUnitTypeId
            .Name = "TextBoxColumnDisplayUnitTypeId"
            .DataPropertyName = "DisplayUnitTypeId"
            .Visible = False
        End With
        With TextBoxColumnBaseLogicalQuantity
            .Name = "TextBoxColumnBaseLogicalQuantity"
            .DataPropertyName = "BaseLogicalQuantity"
            .Visible = False
        End With
        With TextBoxColumnBaseRealQuantity
            .Name = "TextBoxColumnBaseRealQuantity"
            .DataPropertyName = "BaseRealQuantity"
            .Visible = False
        End With
        With TextBoxColumnBaseAdjustQuantity
            .Name = "TextBoxColumnBaseAdjustQuantity"
            .DataPropertyName = "BaseAdjustQuantity"
            .Visible = False
        End With
        With TextBoxColumnBaseDifference
            .Name = "TextBoxColumnBaseDifference"
            .DataPropertyName = "BaseDifference"
            .Visible = False
        End With

        unitTypeList = UnitTypes.GetAll()

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

            .Columns.Add(ButtonColumnHistory)
            .Columns.Add(TextBoxColumnItemTypeName)
            .Columns.Add(TextBoxColumnItemName)
            .Columns.Add(dateBoxColumnArrivalDate)
            .Columns.Add(dateBoxColumnExpiryDate)
            .Columns.Add(TextBoxColumnPricePerGram)
            .Columns.Add(TextBoxColumnLogicalQuantity)
            .Columns.Add(textBoxColumnLogicalUnitTypeName)
            .Columns.Add(textBoxColumnInventoryValue)
            If _mode = "棚卸入力" Then
                .Columns.Add(TextBoxColumnRealQuantity)
                .Columns.Add(ComboBoxColumnRealUnitType)
                .Columns.Add(TextBoxColumnDifference)
                .Columns.Add(textBoxColumnDifferenceUnitTypeName)
            End If
            If _mode <> "棚卸入力" Then
                .Columns.Add(TextBoxColumnAdjustQuantity)
                .Columns.Add(ComboBoxColumnAdjustUnitType)
                .Columns.Add(TextBoxColumnDifference)
                .Columns.Add(textBoxColumnDifferenceUnitTypeName)
                .Columns.Add(ComboBoxColumnAdjustType)
            End If
            .Columns.Add(TextBoxColumnItemId)
            .Columns.Add(TextBoxColumnInventoryId)

            .Columns.Add(TextBoxColumnDisplayUnitTypeId)
            .Columns.Add(TextBoxColumnBaseLogicalQuantity)
            .Columns.Add(TextBoxColumnBaseRealQuantity)
            .Columns.Add(TextBoxColumnBaseAdjustQuantity)
            .Columns.Add(TextBoxColumnBaseDifference)

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
        If Me.Rows(e.RowIndex).IsNewRow Then Return

        Select Case Me.Columns(e.ColumnIndex).Name
            Case ButtonColumnHistory.Name
                RaiseEvent HistoryClick(Me, e)
        End Select

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

    Private Sub Me_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles Me.CellEndEdit

        If IsNothing(Me.DataSource) Then
            Dim customDataGridViewTextBoxColumn As CustomDataGridViewTextBoxColumn = TryCast(Me.Columns(e.ColumnIndex), CustomDataGridViewTextBoxColumn)
            If Not IsNothing(customDataGridViewTextBoxColumn) Then
                ' CustomDataGridViewTextBoxColumnの場合
                If customDataGridViewTextBoxColumn.NumericOnly Then
                    Dim cell As DataGridViewCell = Me(customDataGridViewTextBoxColumn.Name, e.RowIndex)
                    If IsNumeric(cell.Value) Then
                        cell.Value = CDbl(cell.Value)
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

    Private Sub Me_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles Me.CellValidating

        Select Case Me.Columns(e.ColumnIndex).Name
            Case TextBoxColumnRealQuantity.Name
                RaiseEvent RealQuantity_Validating(Me, e)
            Case TextBoxColumnAdjustQuantity.Name
                RaiseEvent AjustQuantity_Validating(Me, e)
        End Select

    End Sub

    Private Sub Me_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles Me.CellValueChanged

        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Return
        If e.RowIndex >= Me.Rows.Count Then Return
        If isDataBinding OrElse isUpdatingQuantity Then Return

        Dim col As DataGridViewColumn = Me.Columns(e.ColumnIndex)
        Dim row As DataGridViewRow = Me.Rows(e.RowIndex)
        Dim quantityColumn As DataGridViewColumn = Nothing
        Dim baseColumn As DataGridViewColumn = Nothing
        Dim unitColumn As DataGridViewColumn = Nothing

        If col Is TextBoxColumnRealQuantity OrElse col Is ComboBoxColumnRealUnitType Then
            quantityColumn = TextBoxColumnRealQuantity
            baseColumn = TextBoxColumnBaseRealQuantity
            unitColumn = ComboBoxColumnRealUnitType
        ElseIf col Is TextBoxColumnAdjustQuantity OrElse col Is ComboBoxColumnAdjustUnitType Then
            quantityColumn = TextBoxColumnAdjustQuantity
            baseColumn = TextBoxColumnBaseAdjustQuantity
            unitColumn = ComboBoxColumnAdjustUnitType
        End If

        If baseColumn IsNot Nothing Then
            ' 数量・単位のどちらを変更しても、入力数量と現在の単位から基準数量を求める
            Dim value As Object = row.Cells(quantityColumn.Name).Value
            If String.IsNullOrEmpty(Convert.ToString(value).Trim()) Then
                value = Nothing
            Else
                If Not IsNumeric(value) Then Return
                If Not IsNumeric(row.Cells(unitColumn.Name).Value) Then Return

                Dim itemId As Integer = CInt(row.Cells(TextBoxColumnItemId.Name).Value)
                Dim unitId As Integer = CInt(row.Cells(unitColumn.Name).Value)
                Dim factor As Double = GetQuantityFactor(itemId, unitId)
                Dim oldValue As Object = row.Cells(baseColumn.Name).Value

                ' 同じ表示数量の再確定では、基準数量を計算し直さない
                If IsNumeric(oldValue) Then
                    If CDbl(value) = CDbl(oldValue) / factor Then Return
                End If
                value = CDbl(value) * factor
            End If

            isUpdatingQuantity = True
            Try
                row.Cells(baseColumn.Name).Value = value
            Finally
                isUpdatingQuantity = False
            End Try

            RefreshQuantityDisplay(row)
        ElseIf col Is TextBoxColumnBaseLogicalQuantity OrElse col Is TextBoxColumnBaseRealQuantity OrElse col Is TextBoxColumnBaseAdjustQuantity Then
            RefreshQuantityDisplay(row)
        End If

    End Sub

    Private Sub Me_EditingControlShowing(sender As Object, e As DataGridViewEditingControlShowingEventArgs) Handles Me.EditingControlShowing

        Select Case CurrentCell.OwningColumn.Name
            Case TextBoxColumnRealQuantity.Name, TextBoxColumnAdjustQuantity.Name
                Dim control As CustomDataGridViewTextBoxEditingControl = TryCast(e.Control, CustomDataGridViewTextBoxEditingControl)
                If control IsNot Nothing Then
                    control.NumericOnly = True
                    control.AllowSymbolChar = True
                    control.Format = String.Empty
                End If

                Dim value As Object = CurrentCell.Value
                If IsNumeric(value) Then
                    e.Control.Text = CDbl(value).ToString()
                Else
                    e.Control.Text = Convert.ToString(value)
                End If
            Case ComboBoxColumnAdjustType.Name,
                 ComboBoxColumnRealUnitType.Name,
                 ComboBoxColumnAdjustUnitType.Name
                Dim ctl As CustomDataGridViewComboBoxEditingControl =
                TryCast(e.Control, CustomDataGridViewComboBoxEditingControl)
                If ctl Is Nothing Then Return

                comboBoxEditingControl = ctl
                ctl.Name = Me.CurrentCell.OwningColumn.Name

                ' 同じコントロールが使い回されるのでハンドラ重複を防ぐ
                RemoveHandler ctl.SelectedIndexChanged, AddressOf dataGridViewComboBox_SelectedIndexChanged
                AddHandler ctl.SelectedIndexChanged, AddressOf dataGridViewComboBox_SelectedIndexChanged
        End Select

    End Sub

    Private Sub dataGridViewComboBox_SelectedIndexChanged(sender As Object, e As EventArgs)

        Dim ctl As CustomDataGridViewComboBoxEditingControl = TryCast(sender, CustomDataGridViewComboBoxEditingControl)
        If ctl Is Nothing OrElse ctl.EditingControlDataGridView Is Nothing Then Return
        If isDataBinding OrElse isUpdatingQuantity Then Return

        ctl.EditingControlDataGridView.NotifyCurrentCellDirty(True)

        Select Case ctl.Name
            Case ComboBoxColumnAdjustType.Name
                RaiseEvent AdjustType_SelectedIndexChanged(Me, ctl.SelectedValue, e)
        End Select

        ctl.EditingControlDataGridView.EndEdit()

    End Sub

#End Region

#Region "メソッド"

    Public Sub DataBind(itemTypeId As String, itemId As String)

        RemoveHandler Me.CellValidating, AddressOf Me_CellValidating

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Try
                isDataBinding = True

                Me.Rows.Clear()

                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String = String.Empty
                    SQLString = "select"
                    SQLString &= " Inventories.InventoryId"
                    SQLString &= ",Inventories.ItemId"
                    SQLString &= ",Items.ItemName"
                    SQLString &= ",ItemTypes.ItemTypeName"
                    SQLString &= ",Inventories.ArrivalDate"
                    SQLString &= ",Inventories.ExpiryDate"
                    SQLString &= ",Inventories.PricePerGram"
                    SQLString &= ",Inventories.Weight as LogicalQuantity"
                    SQLString &= " from Inventories"
                    SQLString &= " inner join Items on Inventories.ItemId = Items.ItemId"
                    SQLString &= " inner join ItemTypes on Items.ItemTypeId = ItemTypes.ItemTypeId"
                    SQLString &= " where coalesce(Inventories.Weight, 0) <> 0"
                    SQLString &= " and ItemTypes.RecipeNeedFlag = 0"
                    If Not String.IsNullOrEmpty(itemTypeId) Then
                        SQLString &= " and Items.ItemTypeId = @ItemTypeId"
                    End If
                    If Not String.IsNullOrEmpty(itemId) Then
                        SQLString &= " and Inventories.ItemId = @ItemId"
                    End If
                    SQLString &= " order by"
                    SQLString &= " ItemTypes.OrderNo asc nulls last"
                    SQLString &= ",Inventories.ItemId"
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    If Not String.IsNullOrEmpty(itemTypeId) Then
                        cmd.Parameters.AddWithValue("@ItemTypeId", NpgsqlDbType.integer, CInt(itemTypeId))
                    End If
                    If Not String.IsNullOrEmpty(itemId) Then
                        cmd.Parameters.AddWithValue("@ItemId", NpgsqlDbType.integer, CInt(itemId))
                    End If
                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        If dr.HasRows Then
                            While dr.Read()
                                ' 論理在庫(Weight)の単位は品目の基準単位(g / ml / 個)
                                Dim unitName As String = calc.GetBaseUnitTypeName(dr("ItemId"))
                                Dim remainDays As Integer = CDate(dr("ExpiryDate")).Subtract(Date.Today).Days

                                Dim column As New List(Of Object)
                                column.Add(Nothing)
                                column.Add(dr("ItemTypeName"))
                                column.Add(dr("ItemName"))
                                column.Add(dr("ArrivalDate"))
                                column.Add(dr("ExpiryDate"))
                                column.Add(dr("PricePerGram"))
                                column.Add(dr("LogicalQuantity"))
                                column.Add(unitName)
                                Dim inventoryValue As Double = Nz(dr("PricePerGram"), 0) * Nz(dr("LogicalQuantity"), 0)
                                column.Add(inventoryValue)
                                If _mode = "棚卸入力" Then
                                    column.Add(Nothing)                     '実在庫
                                    column.Add(Nothing)                     '実在庫単位ID(後で設定)
                                    column.Add(Nothing)                     '差異
                                    column.Add(unitName)                    '差異単位名
                                End If
                                If _mode <> "棚卸入力" Then
                                    column.Add(Nothing)                     '調整数
                                    column.Add(Nothing)                     '調整数単位ID(後で設定)
                                    column.Add(dr("LogicalQuantity"))       '調整後
                                    column.Add(unitName)                    '調整後単位名
                                    column.Add(DBNull.Value)                '調整理由
                                End If
                                column.Add(dr("ItemId"))
                                column.Add(dr("InventoryId"))
                                Dim rowIndex As Integer = Me.Rows.Add(column.ToArray())
                                Dim row As DataGridViewRow = Me.Rows(rowIndex)

                                row.Cells(TextBoxColumnBaseLogicalQuantity.Name).Value = CDbl(Nz(dr("LogicalQuantity"), 0))
                                row.Cells(TextBoxColumnBaseRealQuantity.Name).Value = Nothing
                                row.Cells(TextBoxColumnBaseAdjustQuantity.Name).Value = Nothing

                                InitializeQuantityUnits(row)
                                RefreshQuantityDisplay(row)
                                ApplyRemainColor(row, remainDays)
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
        If _mode = "棚卸入力" Then
            AddMergedHeader(TextBoxColumnLogicalQuantity, textBoxColumnLogicalUnitTypeName, "論理在庫")
            AddMergedHeader(TextBoxColumnRealQuantity, ComboBoxColumnRealUnitType, "実在庫")
            AddMergedHeader(TextBoxColumnDifference, textBoxColumnDifferenceUnitTypeName, "差異")
        Else
            AddMergedHeader(TextBoxColumnLogicalQuantity, textBoxColumnLogicalUnitTypeName, "調整前")
            AddMergedHeader(TextBoxColumnAdjustQuantity, ComboBoxColumnAdjustUnitType, "調整数")
            AddMergedHeader(TextBoxColumnDifference, textBoxColumnDifferenceUnitTypeName, "調整後")
        End If

    End Sub

    Private Sub RefreshQuantityDisplay(row As DataGridViewRow)

        If isUpdatingQuantity Then Return

        isUpdatingQuantity = True
        Try
            Dim itemId As Integer = CInt(row.Cells(TextBoxColumnItemId.Name).Value)
            Dim displayUnitId As Integer = CInt(row.Cells(TextBoxColumnDisplayUnitTypeId.Name).Value)
            Dim displayFactor As Double = GetQuantityFactor(itemId, displayUnitId)
            Dim displayUnitName As String = GetQuantityUnitName(itemId, displayUnitId)
            Dim logical As Double = Nz(row.Cells(TextBoxColumnBaseLogicalQuantity.Name).Value, 0)

            row.Cells(TextBoxColumnLogicalQuantity.Name).Value = logical / displayFactor
            row.Cells(textBoxColumnLogicalUnitTypeName.Name).Value = displayUnitName
            row.Cells(textBoxColumnDifferenceUnitTypeName.Name).Value = displayUnitName

            If _mode = "棚卸入力" Then
                Dim unitId As Integer = CInt(row.Cells(ComboBoxColumnRealUnitType.Name).Value)
                Dim factor As Double = GetQuantityFactor(itemId, unitId)
                Dim value As Object = row.Cells(TextBoxColumnBaseRealQuantity.Name).Value

                If String.IsNullOrEmpty(Convert.ToString(value).Trim()) Then
                    row.Cells(TextBoxColumnRealQuantity.Name).Value = Nothing
                    row.Cells(TextBoxColumnBaseDifference.Name).Value = Nothing
                    row.Cells(TextBoxColumnDifference.Name).Value = Nothing
                Else
                    Dim real As Double = CDbl(value)
                    Dim difference As Double = real - logical

                    row.Cells(TextBoxColumnRealQuantity.Name).Value = real / factor
                    row.Cells(TextBoxColumnBaseDifference.Name).Value = difference
                    row.Cells(TextBoxColumnDifference.Name).Value = difference / displayFactor
                End If
            Else
                Dim unitId As Integer = CInt(row.Cells(ComboBoxColumnAdjustUnitType.Name).Value)
                Dim factor As Double = GetQuantityFactor(itemId, unitId)
                Dim value As Object = row.Cells(TextBoxColumnBaseAdjustQuantity.Name).Value

                If String.IsNullOrEmpty(Convert.ToString(value).Trim()) Then
                    row.Cells(TextBoxColumnAdjustQuantity.Name).Value = Nothing
                Else
                    row.Cells(TextBoxColumnAdjustQuantity.Name).Value = CDbl(value) / factor
                End If

                Dim adjust As Double = Nz(value, 0)
                Dim afterQuantity As Double = logical + adjust

                row.Cells(TextBoxColumnBaseDifference.Name).Value = afterQuantity
                row.Cells(TextBoxColumnDifference.Name).Value = afterQuantity / displayFactor
            End If
        Finally
            isUpdatingQuantity = False
        End Try

    End Sub

    Private Sub ApplyRemainColor(row As DataGridViewRow, remainDays As Integer)

        If remainDays < 0 Then
            row.DefaultCellStyle.ForeColor = Color.Red
        ElseIf remainDays <= 1 Then
            row.DefaultCellStyle.ForeColor = Color.DarkOrange
        End If

    End Sub

    Public Function GetQuantityUnits(itemId As Integer) As List(Of UnitType)

        Dim result As New List(Of UnitType)
        Dim baseUnitId As Integer = calc.GetBaseUnitTypeId(itemId)
        Dim category As Integer = calc.GetManageType(itemId)

        For Each unit As UnitType In unitTypeList
            Dim unitId As Integer = CInt(unit.UnitTypeId)
            If unitId = baseUnitId Then
                result.Add(unit)
                Continue For
            End If

            If calc.IsPieceManaged(itemId) Then Continue For
            If CInt(Nz(unit.UnusedFlag, 0)) <> 0 Then Continue For
            If Not calc.IsConvertibleUnit(unitId) Then Continue For
            If calc.GetUnitCategory(unitId) <> category Then Continue For
            If Not IsNumeric(unit.BaseFactor) Then Continue For
            If CDbl(unit.BaseFactor) <= 0 Then Continue For

            result.Add(unit)
        Next

        ' 基準単位が取得できない品目も、従来の基準単位表示を維持する
        If Not result.Any(Function(x) CInt(x.UnitTypeId) = baseUnitId) Then
            result.Insert(0, New UnitType With {.UnitTypeId = baseUnitId.ToString(), .UnitTypeName = calc.GetBaseUnitTypeName(itemId), .BaseFactor = "1"})
        End If

        Return result

    End Function

    Public Function GetQuantityFactor(itemId As Integer, unitTypeId As Integer) As Double

        Dim baseUnitId As Integer = calc.GetBaseUnitTypeId(itemId)
        If unitTypeId = baseUnitId Then Return 1
        If calc.IsPieceManaged(itemId) Then Return 1

        Return calc.GetBaseFactor(unitTypeId) / calc.GetBaseFactor(baseUnitId)

    End Function

    Private Function GetQuantityUnitName(itemId As Integer, unitTypeId As Integer) As String

        Dim unit As UnitType = unitTypeList.FirstOrDefault(Function(x) CInt(x.UnitTypeId) = unitTypeId)
        If unit IsNot Nothing Then Return unit.UnitTypeName

        Return calc.GetBaseUnitTypeName(itemId)

    End Function

    Private Sub InitializeQuantityUnits(row As DataGridViewRow)

        Dim itemId As Integer = CInt(row.Cells(TextBoxColumnItemId.Name).Value)
        Dim units As List(Of UnitType) = GetQuantityUnits(itemId)
        Dim logical As Double = Math.Abs(CDbl(Nz(row.Cells(TextBoxColumnBaseLogicalQuantity.Name).Value, 0)))
        Dim displayUnitId As Integer = calc.GetBaseUnitTypeId(itemId)
        Dim displayFactor As Double = 1

        For Each unit As UnitType In units
            Dim unitId As Integer = CInt(unit.UnitTypeId)
            Dim factor As Double = GetQuantityFactor(itemId, unitId)
            If factor > displayFactor AndAlso logical >= factor Then
                displayUnitId = unitId
                displayFactor = factor
            End If
        Next

        isUpdatingQuantity = True
        Try
            row.Cells(TextBoxColumnDisplayUnitTypeId.Name).Value = displayUnitId

            Dim unitColumn As DataGridViewColumn = If(_mode = "棚卸入力", CType(ComboBoxColumnRealUnitType, DataGridViewColumn), ComboBoxColumnAdjustUnitType)
            Dim cell As DataGridViewComboBoxCell = DirectCast(row.Cells(unitColumn.Name), DataGridViewComboBoxCell)

            cell.DisplayMember = "UnitTypeName"
            cell.ValueMember = "UnitTypeId"
            cell.DataSource = units
            cell.Value = displayUnitId.ToString()
            cell.ReadOnly = units.Count <= 1
        Finally
            isUpdatingQuantity = False
        End Try

    End Sub

#End Region

End Class
