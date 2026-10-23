Imports Npgsql
Imports NpgsqlTypes
Imports System.Text

' PCRS出荷データ1レコード
Public Class PcrsShipmentRow
    Public Property CustomerCode As String      '取引先コード
    Public Property CustomerBranch As String    '取引先枝番
    Public Property ArrivalDate As Date         '店着日
    Public Property Shipment As String          '便・ブロック
    Public Property StoreCode As String         '店舗コード
    Public Property ItemCode As String          '商品コード
    Public Property MeasureType As Integer      '計量区分
    Public Property SellingPrice As Decimal     '売単価
    Public Property Quantity As Decimal         '数量
    Public Property SupplyPrice As Decimal      '供給単価
    Public Property MarkdownType As Integer     'マークダウン区分
    Public Property MarkdownPrice As Decimal    'マークダウン単価
    Public Property SpecialSaleType As Integer  '特売区分
    Public Property PopType As Integer          'POP区分

End Class

Public Class PcrsShipmentExporter

    ' PCRS出荷データの可変長CSV1レコードを作成する。
    Public Function CreatePcrsShipmentRecord(row As PcrsShipmentRow) As String

        If row Is Nothing Then
            Throw New ApplicationException("出荷データが設定されていません。")
        End If

        ' 当日より古い店着日はエラー。
        If row.ArrivalDate.Date < Now.Date Then
            Throw New ApplicationException("当日より古い店着日のデータは出力できません。" & vbCrLf &
                                           "店着日：" & row.ArrivalDate.ToString("yyyy/MM/dd") & vbCrLf & "商品コード：" & row.ItemCode)
        End If
        If String.IsNullOrWhiteSpace(row.StoreCode) Then
            Throw New ApplicationException("PCRS用の外部店舗コードが設定されていません。" & vbCrLf &
                                           "店着日：" & row.ArrivalDate.ToString("yyyy/MM/dd") & vbCrLf & "商品コード：" & row.ItemCode)
        End If
        If String.IsNullOrWhiteSpace(row.ItemCode) Then
            Throw New ApplicationException("JANコードと品目コードの両方が未設定です。" & vbCrLf &
                                           "店着日：" & row.ArrivalDate.ToString("yyyy/MM/dd") & vbCrLf & "店舗コード：" & row.StoreCode)
        End If
        If row.Quantity <= 0 Then
            Throw New ApplicationException("出荷数量が0以下です。" & vbCrLf & "商品コード：" & row.ItemCode)
        End If
        If row.Quantity <> Decimal.Truncate(row.Quantity) Then
            Throw New ApplicationException("PCRSへ出力する数量に小数が含まれています。" & vbCrLf &
                                           "商品コード：" & row.ItemCode & vbCrLf & "数量：" & row.Quantity.ToString())
        End If

        Dim shipment As String = FormatPcrsVariableNumeric(row.Shipment, 2, "便・ブロック")
        Dim shipmentNumber As Integer = Integer.Parse(shipment)
        If Not ((shipmentNumber >= 1 AndAlso shipmentNumber <= 4) OrElse (shipmentNumber >= 10 AndAlso shipmentNumber <= 49)) Then
            Throw New ApplicationException("便・ブロックは1～4、または10～49で設定してください。" & vbCrLf & "値：" & shipment)
        End If

        Dim customerBranch As String = Convert.ToString(row.CustomerBranch).Trim()
        ' 枝番未使用時は00。
        If String.IsNullOrEmpty(customerBranch) Then
            customerBranch = "00"
        End If

        Dim measureType As String = FormatPcrsVariableInteger(CDec(row.MeasureType), 1, "計量区分")
        Select Case measureType
            Case "0", "1", "2", "3", "9"                ' 使用可能。
            Case Else
                Throw New ApplicationException("計量区分は0、1、2、3、9のいずれかを設定してください。")
        End Select

        Dim markdownType As String = FormatPcrsVariableInteger(CDec(row.MarkdownType), 1, "マークダウン区分")
        Select Case markdownType
            Case "0", "1", "2", "3", "4"                ' 使用可能。
            Case Else
                Throw New ApplicationException("マークダウン区分は0～4で設定してください。")
        End Select

        Dim specialSaleType As String = FormatPcrsVariableInteger(CDec(row.SpecialSaleType), 1, "特売区分")
        Select Case specialSaleType
            Case "0", "1"                ' 使用可能。
            Case Else
                Throw New ApplicationException("特売区分は0または1を設定してください。")
        End Select

        Dim fields As New List(Of String)
        fields.Add(FormatPcrsVariableNumeric(row.CustomerCode, 8, "取引先コード"))
        fields.Add(FormatPcrsVariableNumeric(customerBranch, 2, "取引先枝番"))
        fields.Add(row.ArrivalDate.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture))
        fields.Add(shipment)
        fields.Add(FormatPcrsVariableNumeric(row.StoreCode, 8, "店舗コード"))
        fields.Add(FormatPcrsVariableText(row.ItemCode, 14, "商品コード"))
        fields.Add(measureType)
        fields.Add(FormatPcrsVariableInteger(CDec(row.SellingPrice), 6, "売単価"))
        fields.Add(FormatPcrsVariableInteger(row.Quantity, 4, "数量"))
        fields.Add(FormatPcrsVariableDecimal(CDec(row.SupplyPrice), 6, 2, "供給単価"))
        fields.Add(markdownType)
        fields.Add(FormatPcrsVariableInteger(CDec(row.MarkdownPrice), 6, "マークダウン単価"))
        fields.Add(specialSaleType)
        fields.Add(FormatPcrsVariableInteger(CDec(row.PopType), 3, "POP区分"))
        Return CsvLine(fields)

    End Function

    ' PCRS可変長CSVの数値項目を検証する。
    ' 0埋めは行わず、先頭の0を含めて指定値をそのまま返す。
    Private Function FormatPcrsVariableNumeric(value As String, maxLength As Integer, fieldName As String) As String

        Dim text As String = Convert.ToString(value).Trim()

        If String.IsNullOrEmpty(text) Then
            Throw New ApplicationException(fieldName & "が設定されていません。")
        End If

        For Each c As Char In text
            If Not Char.IsDigit(c) Then
                Throw New ApplicationException(fieldName & "には数字以外を設定できません。" & vbCrLf & "値：" & text)
            End If
        Next

        If text.Length > maxLength Then
            Throw New ApplicationException(fieldName & "が規定桁数を超えています。" & vbCrLf & "値：" & text & vbCrLf & "最大桁数：" & maxLength.ToString())
        End If

        Return text

    End Function

    ' PCRS可変長CSVの整数項目を検証する。
    Private Function FormatPcrsVariableInteger(value As Decimal, maxLength As Integer, fieldName As String) As String

        If value < 0 Then
            Throw New ApplicationException(fieldName & "にマイナスは設定できません。" & vbCrLf & "値：" & value.ToString())
        End If

        If value <> Decimal.Truncate(value) Then
            Throw New ApplicationException(fieldName & "に小数は設定できません。" & vbCrLf & "値：" & value.ToString())
        End If

        Dim text As String = value.ToString("0", System.Globalization.CultureInfo.InvariantCulture)

        Return FormatPcrsVariableNumeric(text, maxLength, fieldName)

    End Function

    ' PCRS可変長CSVの文字項目を検証する。
    Private Function FormatPcrsVariableText(value As String, maxByteLength As Integer, fieldName As String) As String

        Dim text As String = Convert.ToString(value).Trim()

        If String.IsNullOrEmpty(text) Then
            Throw New ApplicationException(fieldName & "が設定されていません。")
        End If

        If text.Contains(",") OrElse text.Contains(vbCr) OrElse text.Contains(vbLf) Then
            Throw New ApplicationException(fieldName & "にカンマまたは改行を含めることはできません。" & vbCrLf & "値：" & text)
        End If

        Dim encoding As Encoding = Encoding.GetEncoding("Shift_JIS")
        Dim byteLength As Integer = encoding.GetByteCount(text)
        If byteLength > maxByteLength Then
            Throw New ApplicationException(fieldName & "が規定桁数を超えています。" & vbCrLf & "値：" & text & vbCrLf & "最大桁数：" & maxByteLength.ToString())
        End If

        Return text

    End Function

    ' PCRS可変長CSVの小数項目を検証する。
    Private Function FormatPcrsVariableDecimal(value As Decimal, maxIntegerLength As Integer, maxDecimalLength As Integer, fieldName As String) As String

        If value < 0 Then
            Throw New ApplicationException(fieldName & "にマイナスは設定できません。" & vbCrLf & "値：" & value.ToString())
        End If

        ' 小数桁数を超える値は丸めずにエラーとする。
        If value <> Decimal.Round(value, maxDecimalLength) Then
            Throw New ApplicationException(fieldName & "の小数桁数が規定を超えています。" & vbCrLf & "値：" & value.ToString() & vbCrLf & "最大小数桁数：" & maxDecimalLength.ToString())
        End If

        Dim integerText As String = Decimal.Truncate(value).ToString("0", System.Globalization.CultureInfo.InvariantCulture)

        If integerText.Length > maxIntegerLength Then
            Throw New ApplicationException(fieldName & "の整数桁数が規定を超えています。" & vbCrLf & "値：" & value.ToString() & vbCrLf & "最大整数桁数：" & maxIntegerLength.ToString())
        End If

        Dim format As String = "0"

        If maxDecimalLength > 0 Then
            format &= "." & New String("#"c, maxDecimalLength)
        End If

        Return value.ToString(format, System.Globalization.CultureInfo.InvariantCulture)

    End Function

    ' 指定期間の製造計画明細をPCRS出荷データ用に取得する。
    Public Function ReadPcrsShipmentRows(startDate As Date, endDate As Date) As List(Of PcrsShipmentRow)

        Dim result As New List(Of PcrsShipmentRow)

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Using cmd As NpgsqlCommand = cn.CreateCommand()
                Dim SQLString As String = String.Empty
                SQLString = "select"
                SQLString &= " Plans.PlanDate"
                SQLString &= ",Items.JANCode"
                SQLString &= ",Items.ItemCode"
                SQLString &= ",Items.Shipment"
                SQLString &= ",Items.MeasureType"
                SQLString &= ",Items.OriginalSellingPrice"
                SQLString &= ",Items.InstoreSellingPrice"
                SQLString &= ",Stores.StoreCode"
                SQLString &= ",PlanDetails.Quantity"
                SQLString &= " from Plans"
                SQLString &= " inner join PlanDetails on Plans.PlanId = PlanDetails.PlanId"
                SQLString &= " inner join Items on Plans.ItemId = Items.ItemId"
                SQLString &= " inner join Stores on PlanDetails.StoreId = Stores.StoreId"
                SQLString &= " where Plans.PlanDate >= @StartDate"
                SQLString &= " and Plans.PlanDate <= @EndDate"
                SQLString &= " and PlanDetails.Quantity > 0"
                SQLString &= " order by"
                SQLString &= " Plans.PlanDate"
                SQLString &= ",Items.Shipment"
                SQLString &= ",Stores.StoreCode"
                SQLString &= ",Plans.ItemId"
                cmd.CommandText = SQLString
                cmd.Parameters.Clear()
                cmd.Parameters.AddWithValue("@StartDate", NpgsqlDbType.Date, startDate.Date)
                cmd.Parameters.AddWithValue("@EndDate", NpgsqlDbType.Date, endDate.Date)
                Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                    While dr.Read()
                        Dim row As New PcrsShipmentRow
                        row.CustomerCode = "001"
                        row.CustomerBranch = ""
                        row.ArrivalDate = CDate(dr("PlanDate")).Date
                        row.Shipment = Convert.ToString(dr("Shipment")).Trim()
                        row.StoreCode = Convert.ToString(dr("StoreCode")).Trim()
                        Dim janCode As String = Convert.ToString(dr("JANCode")).Trim()
                        Dim itemCode As String = Convert.ToString(dr("ItemCode")).Trim()
                        ' JANコードを優先する。
                        If Not String.IsNullOrEmpty(janCode) Then
                            row.ItemCode = janCode
                        Else
                            row.ItemCode = itemCode
                        End If
                        row.MeasureType = dr("MeasureType")
                        row.SellingPrice = CDec(Nz(dr("InstoreSellingPrice"), 0))
                        row.Quantity = CDec(Nz(dr("Quantity"), 0))
                        row.SupplyPrice = CDec(Nz(dr("OriginalSellingPrice"), 0))
                        row.MarkdownType = 0
                        row.MarkdownPrice = 0
                        row.SpecialSaleType = 0
                        row.PopType = 0
                        result.Add(row)
                    End While
                End Using
            End Using
        End Using

        Return result

    End Function

End Class
