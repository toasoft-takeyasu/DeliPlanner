Imports ClosedXML.Excel
Imports Npgsql
Imports NpgsqlTypes
Imports System.IO

' 製造計画書の1行分
Public Class PlanReportRow
    Public PlanDate As Date
    Public ItemTypeName As String
    Public ItemName As String
    Public SumQuantity As Double
    Public UnitTypeName As String
    Public StoreQuantities As New List(Of Double)
End Class

Public Class PlanReportExporter

    Private ReadOnly _storeList As List(Of Store)

    Public Sub New(storeList As List(Of Store))

        If storeList Is Nothing Then
            Throw New ArgumentNullException(NameOf(storeList))
        End If

        _storeList = storeList

    End Sub

    ' 製造計画書の初期ファイル名を取得する。
    Public Function GetDefaultFileName(startDate As Date, endDate As Date) As String

        Dim fileName As String = "製造計画書_" & startDate.ToString("yyyyMMdd")
        If startDate.Date <> endDate.Date Then
            fileName &= "-" & endDate.ToString("yyyyMMdd")
        End If

        Return fileName & ".xlsx"

    End Function

    ' 指定期間の製造計画書をExcelへ出力する。
    ' 戻り値は出力した明細件数。
    Public Function Export(startDate As Date, endDate As Date, savePath As String) As Integer

        If startDate.Date > endDate.Date Then
            Throw New ApplicationException("出力期間の開始日と終了日が逆になっています。")
        End If

        If String.IsNullOrWhiteSpace(savePath) Then
            Throw New ApplicationException("保存先が指定されていません。")
        End If

        Dim rowList As List(Of PlanReportRow) = ReadPlanRows(startDate, endDate)
        If rowList.Count = 0 Then
            Return 0
        End If

        Dim itemTypeBackColors As New Dictionary(Of String, Color)

        If sharedIniFile.PaintItemTypeColor = 1 Then
            For Each itemType As ItemType In ItemTypes.GetAll()
                itemTypeBackColors(itemType.ItemTypeName) = itemType.FormBackColor
            Next
        End If

        Try
            Using book As New XLWorkbook()
                Dim planDateList As List(Of Date) = rowList.Select(Function(x) x.PlanDate.Date).Distinct().OrderBy(Function(x) x).ToList()
                For Each planDate As Date In planDateList
                    Dim sheetRows As List(Of PlanReportRow) = rowList.Where(Function(x) x.PlanDate.Date = planDate.Date).ToList()
                    Dim sheet As IXLWorksheet = book.Worksheets.Add(GetSheetName(planDate))
                    BuildPlanSheet(sheet, planDate, sheetRows, itemTypeBackColors)
                Next
                book.SaveAs(savePath)
            End Using

        Catch ex As IOException
            Throw New ApplicationException("ファイルを保存できませんでした。" & "Excelで同じファイルを開いていないか確認してください。", ex)
        End Try

        Return rowList.Count

    End Function

    ' 指定期間の製造計画を読み込む
    Private Function ReadPlanRows(startDate As Date, endDate As Date) As List(Of PlanReportRow)

        Dim rowList As New List(Of PlanReportRow)

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Try
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String = String.Empty
                    SQLString = "select"
                    SQLString &= " Plans.PlanDate"
                    SQLString &= ",Plans.ItemId"
                    SQLString &= ",Items.ItemName"
                    SQLString &= ",ItemTypes.ItemTypeName"
                    SQLString &= ",Plans.UnitTypeId"
                    SQLString &= ",UnitTypes.UnitTypeName"
                    For Each store As Store In _storeList
                        SQLString &= ",sum("
                        SQLString &= " case"
                        SQLString &= " when PlanDetails.StoreId = " & CInt(store.StoreId).ToString()
                        SQLString &= " then PlanDetails.Quantity"
                        SQLString &= " else 0"
                        SQLString &= " end"
                        SQLString &= " ) as Quantity" & store.StoreId
                    Next
                    SQLString &= " from Plans"
                    SQLString &= " left join PlanDetails on Plans.PlanId = PlanDetails.PlanId"
                    SQLString &= " left join Items on Plans.ItemId = Items.ItemId"
                    SQLString &= " left join UnitTypes on Plans.UnitTypeId = UnitTypes.UnitTypeId"
                    SQLString &= " left join ItemTypes on Items.ItemTypeId = ItemTypes.ItemTypeId"
                    SQLString &= " where Plans.PlanDate >= @StartDate and Plans.PlanDate <= @EndDate"
                    SQLString &= " group by"
                    SQLString &= " Plans.PlanDate"
                    SQLString &= ",Plans.ItemId"
                    SQLString &= ",Items.ItemName"
                    SQLString &= ",Items.ItemTypeId"
                    SQLString &= ",ItemTypes.ItemTypeName"
                    SQLString &= ",ItemTypes.OrderNo"
                    SQLString &= ",Plans.UnitTypeId"
                    SQLString &= ",UnitTypes.UnitTypeName"
                    SQLString &= " order by"
                    SQLString &= " Plans.PlanDate"
                    ' 半製品を先、製品を後にする。
                    SQLString &= ",case"
                    SQLString &= " when Items.ItemTypeId = 2 then 0"
                    SQLString &= " else 1"
                    SQLString &= " end"
                    SQLString &= ",Plans.ItemId"
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    cmd.Parameters.AddWithValue("@StartDate", NpgsqlDbType.Date, startDate.Date)
                    cmd.Parameters.AddWithValue("@EndDate", NpgsqlDbType.Date, endDate.Date)
                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        While dr.Read()
                            Dim planRow As New PlanReportRow
                            planRow.PlanDate = CDate(dr("PlanDate"))
                            planRow.ItemTypeName = Convert.ToString(Nz(dr("ItemTypeName"), String.Empty))
                            planRow.ItemName = Convert.ToString(Nz(dr("ItemName"), String.Empty))
                            planRow.UnitTypeName = Convert.ToString(Nz(dr("UnitTypeName"), String.Empty))

                            ' 店舗別数量と合計
                            For Each store As Store In _storeList
                                Dim quantity As Double = CDbl(Nz(dr("Quantity" & store.StoreId), 0))
                                planRow.StoreQuantities.Add(quantity)
                                planRow.SumQuantity += quantity
                            Next
                            rowList.Add(planRow)
                        End While
                    End Using
                End Using
            Catch ex As Exception
                Throw New ApplicationException(ex.Message, ex)
            Finally
                If cn IsNot Nothing Then
                    cn.Close()
                End If
            End Try
        End Using

        Return rowList

    End Function

    ' シート名を作る(Excelで使えない文字を避ける)
    Private Function GetSheetName(planDate As Date) As String

        Dim culture As New Globalization.CultureInfo("ja-JP")

        ' 「/」「:」「\」「?」「*」「[」「]」はシート名に使えない
        Return planDate.ToString("MM-dd", culture) & "(" & planDate.ToString("ddd", culture) & ")"

    End Function

    ' 1日分の製造計画書シートを組み立てる
    Private Sub BuildPlanSheet(sheet As IXLWorksheet,
                               planDate As Date,
                               sheetRows As List(Of PlanReportRow),
                               itemTypeBackColors As Dictionary(Of String, Color))

        Dim culture As New Globalization.CultureInfo("ja-JP")

        ' 列の位置
        Const COL_ITEMTYPE As Integer = 1
        Const COL_ITEMNAME As Integer = 2
        Const COL_QUANTITY As Integer = 3
        Const COL_UNIT As Integer = 4
        Dim colStoreFirst As Integer = 5
        Dim colStoreLast As Integer = colStoreFirst + _storeList.Count - 1
        Const ROW_HEADER As Integer = 4
        Dim rowFirst As Integer = ROW_HEADER + 1

        sheet.Style.Font.FontName = "MS UI Gothic"

        ' ---- 表題 ----
        With sheet.Cell(1, COL_ITEMTYPE)
            .Value = "製造計画書"
            .Style.Font.Bold = True
            .Style.Font.FontSize = 14
        End With
        With sheet.Cell(2, COL_ITEMTYPE)
            .Value = "製造日  " & planDate.ToString("yyyy年MM月dd日", culture) & " (" & planDate.ToString("ddd", culture) & ")"
            .Style.Font.Bold = True
            .Style.Font.FontSize = 11
        End With
        With sheet.Cell(2, colStoreLast)
            .Value = "出力 " & Now.ToString("yyyy/MM/dd HH:mm")
            .Style.Font.FontSize = 9
            .Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right
        End With

        ' ---- 見出し ----
        sheet.Cell(ROW_HEADER, COL_ITEMTYPE).Value = "品目区分"
        sheet.Cell(ROW_HEADER, COL_ITEMNAME).Value = "品目名"
        sheet.Cell(ROW_HEADER, COL_QUANTITY).Value = "計画数量"
        sheet.Cell(ROW_HEADER, COL_UNIT).Value = "単位"
        For i As Integer = 0 To _storeList.Count - 1
            sheet.Cell(ROW_HEADER, colStoreFirst + i).Value = _storeList(i).StoreShortName
        Next

        With sheet.Range(ROW_HEADER, COL_ITEMTYPE, ROW_HEADER, colStoreLast).Style
            .Font.Bold = True
            .Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217)
            .Alignment.Horizontal = XLAlignmentHorizontalValues.Center
            .Alignment.Vertical = XLAlignmentVerticalValues.Center
        End With

        ' 店舗の見出しだけ色を変えて内訳だと分かるようにする
        sheet.Range(ROW_HEADER, colStoreFirst, ROW_HEADER, colStoreLast).Style.Fill.BackgroundColor = XLColor.FromArgb(221, 235, 247)

        ' ---- 明細 ----
        Dim rowIndex As Integer = rowFirst

        For Each planRow As PlanReportRow In sheetRows

            sheet.Cell(rowIndex, COL_ITEMTYPE).Value = planRow.ItemTypeName
            sheet.Cell(rowIndex, COL_ITEMNAME).Value = planRow.ItemName
            sheet.Cell(rowIndex, COL_UNIT).Value = planRow.UnitTypeName

            ' 数量は値ごとに書式を決める(整数なら小数点を出さない)
            With sheet.Cell(rowIndex, COL_QUANTITY)
                .Value = planRow.SumQuantity
            End With

            For i As Integer = 0 To planRow.StoreQuantities.Count - 1
                With sheet.Cell(rowIndex, colStoreFirst + i)
                    .Value = planRow.StoreQuantities(i)
                End With
            Next

            ' 品目区分の背景色
            Dim backColor As Color
            If TryGetItemTypeBackColor(itemTypeBackColors, planRow.ItemTypeName, backColor) Then
                sheet.Range(rowIndex, COL_ITEMTYPE, rowIndex, COL_ITEMNAME).Style.Fill.BackgroundColor =
                    XLColor.FromArgb(backColor.R, backColor.G, backColor.B)
            End If

            rowIndex += 1

        Next

        Dim rowLast As Integer = rowIndex - 1

        ' ---- 書式 ----
        sheet.Range(rowFirst, COL_UNIT, rowLast, COL_UNIT).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center

        ' ---- 罫線 ----
        With sheet.Range(ROW_HEADER, COL_ITEMTYPE, rowLast, colStoreLast).Style.Border
            .OutsideBorder = XLBorderStyleValues.Thin
            .InsideBorder = XLBorderStyleValues.Thin
        End With

        ' ---- 列幅 ----
        sheet.Column(COL_ITEMTYPE).Width = 10
        sheet.Column(COL_ITEMNAME).Width = 24
        sheet.Column(COL_QUANTITY).Width = 10
        sheet.Column(COL_UNIT).Width = 6
        For i As Integer = colStoreFirst To colStoreLast
            sheet.Column(i).Width = 8
        Next

        ' ---- ウィンドウ枠の固定 ----
        sheet.SheetView.FreezeRows(ROW_HEADER)

        ' ---- 印刷設定 ----
        With sheet.PageSetup
            .PageOrientation = XLPageOrientation.Landscape
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

    End Sub

End Class
