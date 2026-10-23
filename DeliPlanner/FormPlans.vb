Imports Npgsql
Imports NpgsqlTypes
Imports System.IO
Imports System.Text

Public Class FormPlans

#Region "定義"

    Private ReadOnly _menu As FormMenu
    Private WithEvents dataGridViewPlans As DataGridViewPlans

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

        storeList = Stores.GetAll("UnusedFlag = 0")

        dataGridViewPlans = New DataGridViewPlans(storeList)
        PanelPlans.Controls.Add(dataGridViewPlans)

        ' フォームの初期化
        InitializeForm(True)

    End Sub

    Private Sub Me_Shown(sender As Object, e As System.EventArgs) Handles Me.Shown

        AddHandler DateBoxPlanDate.TextChanged, AddressOf DateBoxPlanDate_TextChanged

        DateBoxPlanDate.Focus()

    End Sub

    Private Sub DateBoxPlanDate_TextChanged(sender As Object, e As EventArgs)

        If IsDate(DateBoxPlanDate.Text) Then
            dataGridViewPlans.DataBind(DateBoxPlanDate.Text)
            ButtonCopy.Visible = (CDate(DateBoxPlanDate.Text) >= Now.Date)
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

    Private Sub ButtonCopy_Click(sender As Object, e As EventArgs) Handles ButtonCopy.Click

        If MessageBox.Show("前日データからコピーします。よろしいですか？", Me.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = DialogResult.Yes Then
            Try
                If Not IsDate(DateBoxPlanDate.Text) Then Exit Sub
                CopyData(CDate(DateBoxPlanDate.Text))
            Catch ex As Exception
                SysMsgBox(ex.Message, Me.Text)
                Exit Sub
            End Try
            InitializeForm()
        End If

    End Sub

    Public Sub dataGridViewPlans_RemoveClick(sender As Object, e As DataGridViewCellEventArgs) Handles dataGridViewPlans.RemoveClick

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

    Public Sub dataGridViewPlans_SearchItemClick(sender As Object, e As DataGridViewCellEventArgs) Handles dataGridViewPlans.SearchItemClick

        Dim dgv As DataGridViewPlans = dataGridViewPlans

        Dim dialog As New DialogSearchItems
        With dialog
            .ItemTypeFilter = "RecipeNeedFlag = 1"

            Dim targetBounds As Rectangle = PanelMain.RectangleToScreen(PanelMain.ClientRectangle)
            .StartPosition = FormStartPosition.Manual
            .Height = targetBounds.Height
            .Location = New Point(targetBounds.Right - .Width, targetBounds.Bottom - .Height)

            If .ShowDialog = DialogResult.OK Then
                dgv.CurrentCell = dgv(dgv.ButtonColumnSearchItem.Name, e.RowIndex)
                dgv(dgv.TextBoxColumnItemTypeName.Name, e.RowIndex).Value = ItemTypes.GetById(.Item.ItemTypeId).ItemTypeName
                dgv(dgv.TextBoxColumnItemId.Name, e.RowIndex).Value = .Item.ItemId
                dgv(dgv.TextBoxColumnItemName.Name, e.RowIndex).Value = .Item.ItemName
                dgv(dgv.TextBoxColumnUnitTypeId.Name, e.RowIndex).Value = .Item.UnitType.UnitTypeId
                dgv.InvalidateRow(e.RowIndex)
                dgv.NotifyCurrentCellDirty(True)
                dgv.Focus()
                dgv.NextCell()
            End If
        End With

    End Sub

    Public Sub dataGridViewPlans_Quantity_Validating(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles dataGridViewPlans.Quantity_Validating

        Dim dgv As DataGridViewPlans = dataGridViewPlans

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
            Dim value As Decimal = CDec(e.FormattedValue)
            ' マイナスはエラー
            If value < 0 Then
                Throw New ApplicationException("マイナスは入力できません。")
            End If
            ' 個数管理の場合は整数のみ
            If Not dgv.CanInputDecimal(e.RowIndex) Then
                If value <> Decimal.Truncate(value) Then
                    Throw New ApplicationException("個数管理の数量は整数で入力してください。")
                End If
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

    Public Sub dataGridViewPlans_RowValidating(sender As Object, e As DataGridViewCellCancelEventArgs) Handles dataGridViewPlans.RowValidating

        Dim dgv As DataGridViewPlans = dataGridViewPlans

        If e.RowIndex = dgv.NewRowIndex OrElse Not dgv.IsCurrentRowDirty Then
            Exit Sub
        End If

        Try
            If String.IsNullOrEmpty(Nz(dgv(dgv.TextBoxColumnItemId.Name, e.RowIndex).FormattedValue.ToString.Trim, "")) Then
                dgv.CurrentCell = dgv(dgv.ButtonColumnSearchItem.Name, e.RowIndex)
                Throw New ApplicationException("品目が選択されていません。")
            End If
            If String.IsNullOrEmpty(Nz(dgv(dgv.TextBoxColumnSumQuantity.Name, e.RowIndex).FormattedValue.ToString.Trim, "")) Then
                dgv.CurrentCell = dgv(dgv.TextBoxColumnQuantity(0).Name, e.RowIndex)
                Throw New ApplicationException("数量が入力されていません。")
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

    Private Sub ButtonImportPlanData_Click(sender As Object, e As EventArgs) Handles ButtonImportPlanData.Click

        Dim filePath As String = SelectOpenFile("計画データファイルを選択してください。", String.Empty, "計画データファイル (JS*)|JS*|すべてのファイル (*.*)|*.*")

        If String.IsNullOrEmpty(filePath) Then Exit Sub

        Try
            Dim reader As New PlanImportFileReader(storeList)
            Dim importData As PlanImportResult = reader.Read(filePath)

            If importData.Items.Count = 0 Then
                MessageBox.Show("追加数量が設定された計画データはありませんでした。", Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
                Exit Sub
            End If

            ' 商品照合・既存計画取得・加算後データ作成。
            ' この時点ではまだDBへ登録しない。
            Dim registration As PlanImportRegistration = reader.CreateRegistration(importData)

            If registration.PlanList.Count = 0 Then
                Dim noDataMessage As String = "登録可能な計画データがありませんでした。"
                If registration.SkippedStartedPlans.Count > 0 Then
                    noDataMessage &= vbCrLf & vbCrLf &
                                     "製造開始済みのため未登録：" & registration.SkippedStartedPlans.Count.ToString("#,##0") & "件" & vbCrLf &
                                     PlanImportFileReader.CreateImportErrorListText(registration.SkippedStartedPlans)
                End If
                MessageBox.Show(noDataMessage, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            Dim periodText As String
            If importData.StartPlanDate = importData.EndPlanDate Then
                periodText = importData.StartPlanDate.ToString("yyyy/MM/dd")
            Else
                periodText = importData.StartPlanDate.ToString("yyyy/MM/dd") & " ～ " & importData.EndPlanDate.ToString("yyyy/MM/dd")
            End If

            Dim confirmMessage As String = "計画データを追加登録します。" & vbCrLf & vbCrLf &
                                           "ファイル名：" & IO.Path.GetFileName(filePath) & vbCrLf &
                                           "対象期間：" & periodText & vbCrLf &
                                           "対象日数：" & importData.PlanDateCount.ToString("#,##0") & vbCrLf &
                                           "ファイルレコード数：" & importData.RecordCount.ToString("#,##0") & vbCrLf &
                                           "登録対象計画数：" & registration.PlanList.Count.ToString("#,##0") & vbCrLf &
                                           "新規計画：" & registration.NewPlanCount.ToString("#,##0") & vbCrLf &
                                           "既存計画への追加：" & registration.UpdatePlanCount.ToString("#,##0") & vbCrLf &
                                           "追加数量合計：" & registration.AddedQuantity.ToString("#,##0")
            If registration.SkippedStartedPlans.Count > 0 Then
                confirmMessage &= vbCrLf & "製造開始済みのため未登録：" & registration.SkippedStartedPlans.Count.ToString("#,##0")
            End If

            confirmMessage &= vbCrLf & vbCrLf &
                              "今回の数量は、登録済みの計画数量へ加算されます。" & vbCrLf &
                              "同じファイルを再度取り込むと、数量が重複して加算されます。" & vbCrLf & vbCrLf &
                              "登録してよろしいですか？"
            If MessageBox.Show(confirmMessage, Me.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) <> DialogResult.Yes Then
                Exit Sub
            End If

            ' 異なる計画日を含むPlanを一括登録する。
            Plans.Save(registration.PlanList)

            ' 現在画面で選択している日付だけ再表示する。
            If IsDate(DateBoxPlanDate.Text) Then
                dataGridViewPlans.DataBind(CDate(DateBoxPlanDate.Text))
            End If

            Dim completeMessage As String = "計画データを追加登録しました。" & vbCrLf & vbCrLf &
                                            "新規計画：" & registration.NewPlanCount.ToString("#,##0") & vbCrLf &
                                            "既存計画への追加：" & registration.UpdatePlanCount.ToString("#,##0") & vbCrLf &
                                            "追加数量合計：" & registration.AddedQuantity.ToString("#,##0")

            Dim completeIcon As MessageBoxIcon = MessageBoxIcon.Information

            If registration.SkippedStartedPlans.Count > 0 Then
                completeMessage &= vbCrLf & vbCrLf &
                                   "【製造開始済みのため未登録：" & registration.SkippedStartedPlans.Count.ToString("#,##0") & "件】" & vbCrLf &
                                   PlanImportFileReader.CreateImportErrorListText(registration.SkippedStartedPlans)

                completeIcon = MessageBoxIcon.Warning
            End If

            MessageBox.Show(completeMessage, Me.Text, MessageBoxButtons.OK, completeIcon)

        Catch ex As ApplicationException
            MessageBox.Show(ex.Message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Catch ex As Exception
            SysMsgBox(ex.Message, Me.Text)
        End Try

    End Sub

    Private Sub ButtonOutputPlanList_Click(sender As Object, e As EventArgs) Handles ButtonOutputPlanList.Click

        Dim planDate As Date = Now.Date
        Date.TryParse(DateBoxPlanDate.Text, planDate)

        Dim dialog As New DialogPlanPeriod
        With dialog
            .BaseControl = PanelPlans
            .StartDate = planDate
            .EndDate = planDate

            Dim targetBounds As Rectangle = PanelMain.RectangleToScreen(PanelMain.ClientRectangle)
            .StartPosition = FormStartPosition.Manual
            .Location = New Point(targetBounds.Right - .Width, targetBounds.Top)

            If .ShowDialog() <> DialogResult.OK Then
                Exit Sub
            End If

            Dim exporter As New PlanReportExporter(storeList)
            Dim fileName As String = exporter.GetDefaultFileName(.StartDate, .EndDate)
            Dim savePath As String = SelectSaveFile("製造計画書の出力", fileName, "Excelファイル (*.xlsx)|*.xlsx")
            If String.IsNullOrEmpty(savePath) Then
                Exit Sub
            End If

            Me.Cursor = Cursors.WaitCursor

            Try
                Dim outputCount As Integer = exporter.Export(.StartDate, .EndDate, savePath)
                If outputCount = 0 Then
                    MessageBox.Show("指定された期間に製造計画がありません。", Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Sub
                End If

                If MessageBox.Show(outputCount.ToString("#,##0") & "件を出力しました。" & vbCrLf &
                                   "ファイルを開きますか？", Me.Text,
                                                  MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2) = DialogResult.Yes Then
                    Process.Start(savePath)
                End If
            Catch ex As ApplicationException
                MessageBox.Show(ex.Message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Catch ex As Exception
                SysMsgBox(ex.Message, Me.Text)
            Finally
                Me.Cursor = Cursors.Default
            End Try
        End With

    End Sub

    Private Sub ButtonExportPcrsShipment_Click(sender As Object, e As EventArgs) Handles ButtonExportPcrsShipment.Click

        Dim baseDate As Date = Now.Date

        If IsDate(DateBoxPlanDate.Text) Then
            baseDate = CDate(DateBoxPlanDate.Text).Date
        End If

        Dim dialog As New DialogPlanPeriod
        With dialog
            .BaseControl = PanelPlans
            .StartDate = baseDate
            .EndDate = baseDate

            Dim targetBounds As Rectangle = PanelMain.RectangleToScreen(PanelMain.ClientRectangle)
            .StartPosition = FormStartPosition.Manual
            .Location = New Point(targetBounds.Right - .Width, targetBounds.Top)
            If .ShowDialog() <> DialogResult.OK Then
                Exit Sub
            End If

            Dim outputFolder As String = SelectFolder("出荷データの出力先を選択してください。")
            If String.IsNullOrEmpty(outputFolder) Then
                Exit Sub
            End If

            Try
                ExportPcrsShipmentData(.StartDate, .EndDate, outputFolder)
            Catch ex As ApplicationException
                MessageBox.Show(ex.Message, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Catch ex As Exception
                SysMsgBox(ex.Message, Me.Text)
            End Try
        End With

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
            InitializeForm()
        End If

    End Sub

#End Region

#Region "メソッド"

    Private Function CheckData() As Boolean

        Dim result As Boolean = True

        Dim dgv As DataGridViewPlans = dataGridViewPlans

        ' 編集中の内容をセルに確定させる
        dgv.EndEdit()

        Try
            If String.IsNullOrEmpty(DateBoxPlanDate.Text) Then
                DateBoxPlanDate.Focus()
                Throw New ApplicationException("製造日が入力されていません。")
            End If
            If dgv.Rows.Count <= 1 Then
                Throw New ApplicationException("明細が入力されていません。")
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

    Private Sub SaveData()

        Try
            Dim planList As New List(Of Plan)
            For Each row As DataGridViewRow In dataGridViewPlans.Rows
                If row.IsNewRow Then Exit For

                Dim plan As New Plan
                plan.PlanId = Nz(dataGridViewPlans(dataGridViewPlans.TextBoxColumnPlanId.Name, row.Index).Value, Nothing)
                plan.PlanDate = DateBoxPlanDate.Text
                plan.Item = Items.GetById(dataGridViewPlans(dataGridViewPlans.TextBoxColumnItemId.Name, row.Index).Value.ToString)
                plan.UnitType = UnitTypes.GetById(dataGridViewPlans(dataGridViewPlans.TextBoxColumnUnitTypeId.Name, row.Index).Value.ToString)
                Dim i As Integer = 0
                For Each store As Store In storeList
                    If Nz(dataGridViewPlans(dataGridViewPlans.TextBoxColumnQuantity(i).Name, row.Index).Value, 0) > 0 Then
                        Dim PlanDetail As New PlanDetail
                        PlanDetail.Store = Stores.GetById(store.StoreId)
                        PlanDetail.Quantity = dataGridViewPlans(dataGridViewPlans.TextBoxColumnQuantity(i).Name, row.Index).Value.ToString
                        plan.PlanDetails.Add(PlanDetail)
                    End If
                    i += 1
                Next
                plan.InstructionStart = dataGridViewPlans(dataGridViewPlans.CheckBoxColumnInstructionStart.Name, row.Index).Value
                planList.Add(plan)
            Next

            Plans.Save(planList)
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try

    End Sub

    Private Sub DeleteData(rowIndex As Integer)

        Try
            Dim plan As New Plan
            plan.PlanId = dataGridViewPlans(dataGridViewPlans.TextBoxColumnPlanId.Name, rowIndex).Value

            Plans.Delete(plan)
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try

    End Sub

    Private Sub CopyData(planDate As Date)

        Try
            If Plans.Count = 0 Then Exit Sub

            ' 過去の計画データを探す
            Dim planList As New List(Of Plan)
            Dim basePlanDate As Date = planDate
            Do Until planList.Count > 0
                basePlanDate = basePlanDate.AddDays(-1)
                planList = Plans.GetAll("PlanDate = #" & basePlanDate & "#")
            Loop

            ' 過去の計画データをコピーして対象日付のデータを作成
            For Each plan As Plan In planList
                plan.PlanId = String.Empty
                plan.PlanDate = planDate
            Next

            Plans.Save(planList)
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try

    End Sub

    Private Sub InitializeForm(Optional isLoading As Boolean = False)

        DateBoxPlanDate.Text = Now.ToString("yyyy/MM/dd")

        ' リストの初期化
        dataGridViewPlans.DataBind(DateBoxPlanDate.Text)

    End Sub

    ' 指定期間の製造計画からPCRS出荷データを出力する。
    Private Sub ExportPcrsShipmentData(startDate As Date, endDate As Date, outputFolder As String)

        If startDate.Date > endDate.Date Then
            Throw New ApplicationException("出力期間の開始日と終了日が逆になっています。")
        End If

        Dim pcrsData As New PcrsShipmentExporter
        Dim rowList As List(Of PcrsShipmentRow) = pcrsData.ReadPcrsShipmentRows(startDate, endDate)

        If rowList.Count = 0 Then
            Throw New ApplicationException("指定期間に出力対象の製造計画がありません。")
        End If

        ' 実際にファイルを作る前に全行を作成し、
        ' エラーがないことを確認する。
        Dim recordList As New List(Of String)

        For Each row As PcrsShipmentRow In rowList
            recordList.Add(pcrsData.CreatePcrsShipmentRecord(row))
        Next

        Dim fileName As String = sharedIniFile.PcrsOutputFileName
        If String.IsNullOrWhiteSpace(fileName) Then
            fileName = "JUCHU.CSV"
        End If

        ' INIにフォルダを含む名前が設定されていても、
        ' 選択された出力先フォルダから外へ出さない。
        fileName = Path.GetFileName(fileName)
        If String.IsNullOrWhiteSpace(fileName) Then
            Throw New ApplicationException("出力ファイル名が不正です。")
        End If

        Dim outputFile As String = Path.Combine(outputFolder, fileName)
        If File.Exists(outputFile) Then
            If MessageBox.Show("出力先に同名のファイルが存在します。" & vbCrLf & vbCrLf &
                               outputFile & vbCrLf & vbCrLf &
                               "上書きしてよろしいですか？", Me.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) <> DialogResult.Yes Then
                Exit Sub
            End If
        End If

        Dim encoding As Encoding = Encoding.GetEncoding("Shift_JIS")
        Using writer As New StreamWriter(outputFile, False, encoding)
            For Each recordText As String In recordList
                ' 1レコードごとにCR/LFを付ける。
                writer.Write(recordText & vbCrLf)
            Next
        End Using

        MessageBox.Show("出荷データを出力しました。" & vbCrLf & vbCrLf &
                        "対象期間：" & startDate.ToString("yyyy/MM/dd") & " ～ " & endDate.ToString("yyyy/MM/dd") & vbCrLf &
                        "出力件数：" & recordList.Count.ToString("#,##0") & "件" & vbCrLf &
                        "出力先：" & outputFile, Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

#End Region

End Class

Public Class DataGridViewPlans
    Inherits CustomDataGridView

#Region "定義"

    Public ButtonColumnRemove As New CustomDatagridViewButtonColumn
    Public TextBoxColumnStatus As New CustomDataGridViewTextBoxColumn
    Public CheckBoxColumnInstructionStart As New DataGridViewCheckBoxColumn
    Public TextBoxColumnItemTypeName As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnItemName As New CustomDataGridViewTextBoxColumn
    Public ButtonColumnSearchItem As New CustomDatagridViewButtonColumn
    Public TextBoxColumnSumQuantity As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnSumUnitTypeName As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnQuantity() As CustomDataGridViewTextBoxColumn
    Public textBoxColumnUnitTypeName() As CustomDataGridViewTextBoxColumn
    Public TextBoxColumnCost As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnItemId As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnUnitTypeId As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnPlanId As New CustomDataGridViewTextBoxColumn
    Public TextBoxColumnInstructionId As New CustomDataGridViewTextBoxColumn

    Public Event RemoveClick(sender As Object, e As DataGridViewCellEventArgs)
    Public Event SearchItemClick(sender As Object, e As DataGridViewCellEventArgs)
    Public Event Quantity_Validating(sender As Object, e As DataGridViewCellValidatingEventArgs)

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
        With TextBoxColumnStatus
            .Name = "TextBoxColumnStatus"
            .DataPropertyName = "Status"
            .HeaderText = "ステータス"
            .DefaultCellStyle.NullValue = String.Empty
            .DefaultCellStyle.BackColor = Color.White
            .DefaultCellStyle.ForeColor = Color.Black
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .DefaultCellStyle.SelectionForeColor = Color.Black
            .DefaultCellStyle.SelectionBackColor = Color.White
            .Width = 80
            .ReadOnly = True
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .TabStop = False
        End With
        With CheckBoxColumnInstructionStart
            .Name = "CheckBoxColumnInstructionStart"
            .HeaderText = "製造開始"
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
            .HeaderText = String.Empty          ' 見出しは結合ヘッダーで描くので空
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            .DefaultCellStyle.SelectionForeColor = Color.Black
            .DefaultCellStyle.SelectionBackColor = Color.White
            .Width = 200
            .ReadOnly = True
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .TabStop = False
        End With
        With ButtonColumnSearchItem
            .Name = "ButtonColumnSearchItem"
            .HeaderText = String.Empty          ' 見出しは結合ヘッダーで描くので空
            .Width = 50
            .DefaultCellStyle.Font = New Font("MS UI Gothic", 11.25)
            .DefaultCellStyle.NullValue = "検索"
            .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            .UseColumnTextForButtonValue = True
            .SortMode = DataGridViewColumnSortMode.NotSortable
            .TabStop = True
        End With
        With TextBoxColumnSumQuantity
            .Name = "TextBoxColumnSumQuantity"
            .DataPropertyName = "SumQuantity"
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
        With TextBoxColumnSumUnitTypeName
            .Name = "TextBoxColumnSumUnitType"
            .DataPropertyName = "SumUnitType"
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
                .DefaultCellStyle.Format = "#,##0"
                .SortMode = DataGridViewColumnSortMode.NotSortable
                .NumericOnly = True
                .AllowSymbolChar = False
                .MaxByteLength = 10
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
        With TextBoxColumnPlanId
            .Name = "TextBoxColumnPlanId"
            .DataPropertyName = "PlanId"
            .HeaderText = "計画ID"
            .Visible = False
        End With
        With TextBoxColumnInstructionId
            .Name = "TextBoxColumnInstructionId"
            .DataPropertyName = "InstructionId"
            .HeaderText = "製造ID"
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
            .Columns.Add(TextBoxColumnStatus)
            .Columns.Add(CheckBoxColumnInstructionStart)
            .Columns.Add(TextBoxColumnItemTypeName)
            .Columns.Add(TextBoxColumnItemName)
            .Columns.Add(ButtonColumnSearchItem)
            .Columns.Add(TextBoxColumnSumQuantity)
            .Columns.Add(TextBoxColumnSumUnitTypeName)
            For i = 0 To _storeList.Count - 1
                .Columns.Add(TextBoxColumnQuantity(i))
                .Columns.Add(textBoxColumnUnitTypeName(i))
            Next
            .Columns.Add(TextBoxColumnCost)
            .Columns.Add(TextBoxColumnItemId)
            .Columns.Add(TextBoxColumnUnitTypeId)
            .Columns.Add(TextBoxColumnPlanId)
            .Columns.Add(TextBoxColumnInstructionId)

            ' 動作関係のプロパティを変更
            .ReadOnly = False
            .AllowUserToOrderColumns = False
            .AllowUserToAddRows = True
            .AllowUserToDeleteRows = True
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

        TextBoxColumnSumUnitTypeName.Frozen = True

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
            Case ButtonColumnSearchItem.Name
                If Me(ButtonColumnSearchItem.Name, e.RowIndex).ReadOnly Then Return

                Select Case Me(TextBoxColumnStatus.Name, e.RowIndex).Value
                    Case "未製造", String.Empty
                        RaiseEvent SearchItemClick(sender, e)
                End Select
            Case ButtonColumnRemove.Name
                If Me(ButtonColumnRemove.Name, e.RowIndex).ReadOnly Then Return

                If Not IsNothing(Me(TextBoxColumnPlanId.Name, e.RowIndex).Value) Then
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
                    If IsQuantityColumn(customDataGridViewTextBoxColumn) AndAlso CanInputDecimal(e.RowIndex) Then
                        ' 重量・容量の数量：小数を保持する
                        If IsNumeric(Me(customDataGridViewTextBoxColumn.Name, e.RowIndex).Value) Then
                            Me(customDataGridViewTextBoxColumn.Name, e.RowIndex).Value = CDec(Me(customDataGridViewTextBoxColumn.Name, e.RowIndex).Value)
                        End If
                    Else
                        Dim value As Integer
                        If Integer.TryParse(Me(customDataGridViewTextBoxColumn.Name, e.RowIndex).FormattedValue.ToString(), value) Then
                            Me(customDataGridViewTextBoxColumn.Name, e.RowIndex).Value = value.ToString(customDataGridViewTextBoxColumn.DefaultCellStyle.Format)
                        End If
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

        If Me.Columns(e.ColumnIndex).Name = "TextBoxColumnStatus" Then
            Select Case e.Value
                Case "未製造"
                    e.CellStyle.BackColor = Color.Red
                    e.CellStyle.ForeColor = Color.White
                Case "製造中"
                    e.CellStyle.BackColor = Color.Yellow
                    e.CellStyle.ForeColor = Color.Black
                Case "完成"
                    e.CellStyle.BackColor = Color.Lime
                    e.CellStyle.ForeColor = Color.Black
            End Select
        End If

    End Sub

    Private Sub Me_CellValidating(sender As Object, e As DataGridViewCellValidatingEventArgs) Handles Me.CellValidating

        If IsQuantityColumn(Me.Columns(e.ColumnIndex)) Then
            RaiseEvent Quantity_Validating(Me, e)
        End If

    End Sub

    Private Sub Me_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles Me.CellValueChanged

        If isDataBinding Then Return
        If e.RowIndex < 0 OrElse e.RowIndex >= Me.Rows.Count Then Return

        Dim row As DataGridViewRow = Me.Rows(e.RowIndex)
        Dim col As DataGridViewColumn = Me.Columns(e.ColumnIndex)

        ' 単位ID列が変わったら、単位名を取得して合計・各店舗の単位列へ反映
        If col Is TextBoxColumnUnitTypeId Then
            ' 単位IDを取得(未設定なら単位名は空)
            Dim unitTypeId As Integer
            If Not Integer.TryParse(Convert.ToString(row.Cells(TextBoxColumnUnitTypeId.Name).Value), unitTypeId) Then
                row.Cells(TextBoxColumnSumUnitTypeName.Name).Value = Nothing
                For Each c As CustomDataGridViewTextBoxColumn In textBoxColumnUnitTypeName
                    row.Cells(c.Name).Value = Nothing
                Next
            Else
                Dim unitTypeName As String = UnitTypes.GetById(unitTypeId).UnitTypeName
                row.Cells(TextBoxColumnSumUnitTypeName.Name).Value = unitTypeName
                For Each c As CustomDataGridViewTextBoxColumn In textBoxColumnUnitTypeName
                    row.Cells(c.Name).Value = unitTypeName
                Next
            End If
            ApplyQuantityFormat(e.RowIndex)
        End If

        ' 品目が入った行にステータスを表示する(未入力行は空白のまま)
        If col Is TextBoxColumnItemId Then
            If String.IsNullOrEmpty(Convert.ToString(row.Cells(TextBoxColumnItemId.Name).Value)) Then
                row.Cells(TextBoxColumnStatus.Name).Value = Nothing
            ElseIf String.IsNullOrEmpty(Convert.ToString(row.Cells(TextBoxColumnStatus.Name).Value)) Then
                ' 先に製造開始がチェックされていれば製造中から始まる
                If CBool(Nz(row.Cells(CheckBoxColumnInstructionStart.Name).Value, False)) Then
                    row.Cells(TextBoxColumnStatus.Name).Value = "製造中"
                Else
                    row.Cells(TextBoxColumnStatus.Name).Value = "未製造"
                End If
            End If
        End If

        Dim isQuantityColumn As Boolean = False
        For i As Integer = 0 To _storeList.Count - 1
            If col Is TextBoxColumnQuantity(i) Then
                isQuantityColumn = True
                Exit For
            End If
        Next
        If isQuantityColumn Then
            Dim sumQuantity As Decimal = 0
            For i As Integer = 0 To _storeList.Count - 1
                Dim value As Object = row.Cells(TextBoxColumnQuantity(i).Name).Value
                If IsNumeric(value) Then
                    sumQuantity += CDec(value)
                End If
            Next
            row.Cells(TextBoxColumnSumQuantity.Name).Value = sumQuantity
        End If

        If col Is CheckBoxColumnInstructionStart Then
            If e.RowIndex < 0 OrElse e.RowIndex >= Me.Rows.Count Then Return

            ' 品目が入っている行だけステータスを切り替える(未入力行は空白のまま)
            If Not String.IsNullOrEmpty(Convert.ToString(row.Cells(TextBoxColumnItemId.Name).Value)) Then
                ' 完成した行は触らない
                If Convert.ToString(row.Cells(TextBoxColumnStatus.Name).Value) <> "完成" Then
                    If CBool(Nz(row.Cells(CheckBoxColumnInstructionStart.Name).Value, False)) Then
                        row.Cells(TextBoxColumnStatus.Name).Value = "製造中"
                    Else
                        row.Cells(TextBoxColumnStatus.Name).Value = "未製造"
                    End If
                End If
            End If

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

        If CurrentCell Is Nothing Then Return
        If Not IsQuantityColumn(CurrentCell.OwningColumn) Then Return

        Dim control As CustomDataGridViewTextBoxEditingControl = TryCast(e.Control, CustomDataGridViewTextBoxEditingControl)

        If control IsNot Nothing Then
            control.AllowSymbolChar = CanInputDecimal(CurrentCell.RowIndex)
            ' 編集中に整数書式などで丸めない
            control.Format = String.Empty
        End If

        If IsNumeric(e.Control.Text) Then
            e.Control.Text = CDec(e.Control.Text).ToString()
        End If

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
                    Dim instructionStatusSql As String
                    instructionStatusSql = "min(case when Instructions.InstructionId is null then 0" &
                                                   " when Instructions.FinishDate is null then 1" &
                                                   " else 2 end)"

                    Dim SQLString As String = String.Empty
                    SQLString = "select"
                    SQLString &= " Plans.ItemId"
                    SQLString &= ",Items.ItemName"
                    SQLString &= ",ItemTypes.ItemTypeName"
                    SQLString &= ",Plans.UnitTypeId"
                    SQLString &= ",UnitTypes.UnitTypeName"
                    SQLString &= ",Plans.PlanId"
                    SQLString &= ",Instructions.InstructionId"
                    SQLString &= ",Instructions.StartDate"

                    ' 店舗別の計画数量
                    For Each store As Store In _storeList
                        Dim storeId As Integer = CInt(store.StoreId)

                        SQLString &= ",sum("
                        SQLString &= " case"
                        SQLString &= " when PlanDetails.StoreId = " & storeId.ToString()
                        SQLString &= " then PlanDetails.Quantity"
                        SQLString &= " else 0"
                        SQLString &= " end"
                        SQLString &= " ) as Quantity" & storeId.ToString()
                    Next

                    ' 製造状態
                    SQLString &= ",case"
                    SQLString &= " when " & instructionStatusSql & " = 0 then '未製造'"
                    SQLString &= " when " & instructionStatusSql & " = 1 then '製造中'"
                    SQLString &= " else '完成'"
                    SQLString &= " end as Status"
                    SQLString &= " from Plans"
                    SQLString &= " left join PlanDetails on Plans.PlanId = PlanDetails.PlanId"
                    SQLString &= " left join Items on Plans.ItemId = Items.ItemId"
                    SQLString &= " left join UnitTypes on Plans.UnitTypeId = UnitTypes.UnitTypeId"
                    SQLString &= " left join ItemTypes on Items.ItemTypeId = ItemTypes.ItemTypeId"
                    SQLString &= " left join Instructions on Plans.PlanId = Instructions.PlanId"
                    SQLString &= " where Plans.PlanDate = @PlanDate"
                    SQLString &= " group by"
                    SQLString &= " Plans.ItemId"
                    SQLString &= ",Items.ItemName"
                    SQLString &= ",Items.ItemTypeId"
                    SQLString &= ",ItemTypes.ItemTypeName"
                    SQLString &= ",ItemTypes.OrderNo"
                    SQLString &= ",Plans.UnitTypeId"
                    SQLString &= ",UnitTypes.UnitTypeName"
                    SQLString &= ",Plans.PlanId"
                    SQLString &= ",Instructions.InstructionId"
                    SQLString &= ",Instructions.StartDate"
                    SQLString &= " order by"

                    ' 完成（2）だけ下に落とす
                    SQLString &= " case"
                    SQLString &= " when " & instructionStatusSql & " = 2 then 1"
                    SQLString &= " else 0"
                    SQLString &= " end"

                    ' 品目区分の並び（半製品 → 製品）
                    SQLString &= ",case"
                    SQLString &= " when Items.ItemTypeId = 2 then 0"
                    SQLString &= " else 1"
                    SQLString &= " end"

                    SQLString &= ",Plans.ItemId"
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    cmd.Parameters.AddWithValue("@PlanDate", NpgsqlDbType.Date, planDate.Date)

                    ' 原価計算は行をまたいでロットのキャッシュを使い回す
                    Dim costCalc As New CostCalculator(calc)

                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        If dr.HasRows Then
                            While dr.Read()
                                Dim column As New List(Of Object)
                                column.Add(Nothing)
                                column.Add(dr("Status"))
                                column.Add(If(IsDBNull(dr("StartDate")), False, True))
                                column.Add(dr("ItemTypeName"))
                                column.Add(dr("ItemName"))
                                column.Add(Nothing)
                                Dim sumIndex As Integer = column.Count
                                column.Add(Nothing)
                                column.Add(dr("UnitTypeName"))
                                Dim sumQuantity As Decimal = 0
                                For Each store As Store In _storeList
                                    Dim quantity As Decimal = CDec(Nz(dr("Quantity" & store.StoreId), 0))
                                    sumQuantity += quantity
                                    column.Add(quantity)
                                    column.Add(dr("UnitTypeName"))
                                Next
                                column(sumIndex) = sumQuantity

                                '原価(行ごとに在庫を満タンに戻してから計算する)
                                costCalc.ResetRemaining()
                                Dim result As CostResult = costCalc.CalcUnitCost(CInt(dr("ItemId")), CInt(Nz(dr("UnitTypeId"), 0)))
                                column.Add(result.UnitCost)

                                column.Add(dr("ItemId"))
                                column.Add(dr("UnitTypeId"))
                                column.Add(dr("PlanId"))
                                column.Add(Nz(dr("InstructionId"), ""))
                                Dim rowIndex As Integer = Me.Rows.Add(column.ToArray())

                                ApplyQuantityFormat(rowIndex)
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
        AddMergedHeader(TextBoxColumnItemName, ButtonColumnSearchItem, "品目名")
        AddMergedHeader(TextBoxColumnSumQuantity, TextBoxColumnSumUnitTypeName, "計画数量")
        For i As Integer = 0 To _storeList.Count - 1
            AddMergedHeader(TextBoxColumnQuantity(i), textBoxColumnUnitTypeName(i), _storeList(i).StoreShortName)
        Next

    End Sub

    Private Sub ApplyRowLock(rowIndex As Integer)

        If String.IsNullOrEmpty(Me(TextBoxColumnInstructionId.Name, rowIndex).Value) Then
            Me(ButtonColumnRemove.Name, rowIndex).ReadOnly = False
            Me(CheckBoxColumnInstructionStart.Name, rowIndex).ReadOnly = False
            Me(ButtonColumnSearchItem.Name, rowIndex).ReadOnly = False
            For i As Integer = 0 To _storeList.Count - 1
                Me(TextBoxColumnQuantity(i).Name, rowIndex).ReadOnly = False
            Next
        Else
            Me(ButtonColumnRemove.Name, rowIndex).ReadOnly = True
            Me(CheckBoxColumnInstructionStart.Name, rowIndex).ReadOnly = True
            Me(ButtonColumnSearchItem.Name, rowIndex).ReadOnly = True
            For i As Integer = 0 To _storeList.Count - 1
                Me(TextBoxColumnQuantity(i).Name, rowIndex).ReadOnly = True
            Next
        End If

    End Sub

    Private Function IsQuantityColumn(column As DataGridViewColumn) As Boolean

        If column Is Nothing OrElse TextBoxColumnQuantity Is Nothing Then Return False

        For Each c As CustomDataGridViewTextBoxColumn In TextBoxColumnQuantity
            If column Is c Then Return True
        Next

        Return False

    End Function

    ' 重量・容量の単位なら小数を許可する
    Public Function CanInputDecimal(rowIndex As Integer) As Boolean

        Dim value As Object = Me(TextBoxColumnUnitTypeId.Name, rowIndex).Value
        If Not IsNumeric(value) Then Return False

        Dim unitTypeId As Integer = CInt(value)
        Dim category As Integer = calc.GetUnitCategory(unitTypeId)
        Return calc.IsConvertibleUnit(unitTypeId) AndAlso (category = 1 OrElse category = 2)

    End Function

    ' 行ごとの数量表示書式を設定する
    Private Sub ApplyQuantityFormat(rowIndex As Integer)

        Dim format As String = If(CanInputDecimal(rowIndex), "#,###.##", "#,###")
        Dim row As DataGridViewRow = Me.Rows(rowIndex)
        row.Cells(TextBoxColumnSumQuantity.Name).Style.Format = format
        For Each column As CustomDataGridViewTextBoxColumn In TextBoxColumnQuantity
            row.Cells(column.Name).Style.Format = format
        Next

    End Sub

#End Region

End Class