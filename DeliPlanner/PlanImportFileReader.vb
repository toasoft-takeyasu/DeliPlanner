Imports System.Globalization
Imports System.IO
Imports System.Text

' 計画データファイルの読込結果
Public Class PlanImportResult

    Public Property RecordCount As Integer
    Public Property Items As New List(Of PlanImportItem)

    Public ReadOnly Property PlanDateCount As Integer
        Get
            Return Items.Select(Function(x) x.PlanDate.Date).Distinct().Count()
        End Get
    End Property

    Public ReadOnly Property StartPlanDate As Date
        Get
            If Items.Count = 0 Then Return Date.MinValue
            Return Items.Min(Function(x) x.PlanDate.Date)
        End Get
    End Property

    Public ReadOnly Property EndPlanDate As Date
        Get
            If Items.Count = 0 Then Return Date.MinValue
            Return Items.Max(Function(x) x.PlanDate.Date)
        End Get
    End Property

End Class

' 計画日・商品ごとの取込数量
Public Class PlanImportItem

    Public Property PlanDate As Date
    Public Property ProductCode As String

    ' Key：外部店舗コード
    ' Value：今回追加する数量
    Public Property Quantities As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

End Class

' 固定長ファイル上の店舗定義
Public Class PlanImportStoreDefinition

    Public Property StoreCode As String
    Public Property StoreName As String

    ' 数量開始位置。0始まり。
    Public Property QuantityStartPosition As Integer

    ' 符号開始位置。0始まり。
    Public Property SignStartPosition As Integer

End Class

' 計画データ取込の登録準備結果
Public Class PlanImportRegistration
    Public Property PlanList As New List(Of Plan)
    Public Property NewPlanCount As Integer
    Public Property UpdatePlanCount As Integer
    Public Property AddedQuantity As Long
    Public Property SkippedStartedPlans As New List(Of String)
End Class

' 計画データ固定長ファイル読込クラス
Public Class PlanImportFileReader

    Private Const RecordLength As Integer = 128
    Private Const QuantityLength As Integer = 5
    Private Const SignLength As Integer = 2

    Private ReadOnly _storeList As List(Of Store)

    Public Sub New(storeList As List(Of Store))

        If storeList Is Nothing Then
            Throw New ArgumentNullException(NameOf(storeList))
        End If

        _storeList = storeList

    End Sub

    ' 取込データからDB登録用の計画リストを作成する。
    ' この時点ではDBへ登録しない。
    Public Function CreateRegistration(importData As PlanImportResult) As PlanImportRegistration

        If importData Is Nothing Then
            Throw New ArgumentNullException(NameOf(importData))
        End If

        Dim storeIndexDictionary As Dictionary(Of String, Integer) = CreateStoreIndexDictionary()

        Return CreatePlanRegistration(importData, storeIndexDictionary)

    End Function

    ' 外部ファイルの店舗定義を返す。
    Public Shared Function GetStoreDefinitions() As List(Of PlanImportStoreDefinition)

        Return New List(Of PlanImportStoreDefinition) From {
            New PlanImportStoreDefinition With {
                .StoreCode = "02",
                .StoreName = "岩美店",
                .QuantityStartPosition = 46,
                .SignStartPosition = 51
            },
            New PlanImportStoreDefinition With {
                .StoreCode = "04",
                .StoreName = "郡家店",
                .QuantityStartPosition = 53,
                .SignStartPosition = 58
            },
            New PlanImportStoreDefinition With {
                .StoreCode = "05",
                .StoreName = "南店",
                .QuantityStartPosition = 60,
                .SignStartPosition = 65
            },
            New PlanImportStoreDefinition With {
                .StoreCode = "06",
                .StoreName = "岩倉店",
                .QuantityStartPosition = 67,
                .SignStartPosition = 72
            },
            New PlanImportStoreDefinition With {
                .StoreCode = "07",
                .StoreName = "青谷店",
                .QuantityStartPosition = 74,
                .SignStartPosition = 79
            },
            New PlanImportStoreDefinition With {
                .StoreCode = "08",
                .StoreName = "北園店",
                .QuantityStartPosition = 81,
                .SignStartPosition = 86
            },
            New PlanImportStoreDefinition With {
                .StoreCode = "09",
                .StoreName = "湖山店",
                .QuantityStartPosition = 88,
                .SignStartPosition = 93
            },
            New PlanImportStoreDefinition With {
                .StoreCode = "10",
                .StoreName = "東店",
                .QuantityStartPosition = 95,
                .SignStartPosition = 100
            },
            New PlanImportStoreDefinition With {
                .StoreCode = "11",
                .StoreName = "西店",
                .QuantityStartPosition = 102,
                .SignStartPosition = 107
            }
        }

    End Function

    ' 固定長計画データファイルを読み込む。
    Public Function Read(filePath As String) As PlanImportResult

        If String.IsNullOrWhiteSpace(filePath) Then
            Throw New ApplicationException("計画データファイルが指定されていません。")
        End If

        If Not File.Exists(filePath) Then
            Throw New ApplicationException("指定された計画データファイルが見つかりません。" & vbCrLf & filePath)
        End If

        Dim fileBytes As Byte() = File.ReadAllBytes(filePath)

        If fileBytes.Length = 0 Then
            Throw New ApplicationException("計画データファイルが空です。")
        End If

        If fileBytes.Length Mod RecordLength <> 0 Then
            Throw New ApplicationException("計画データファイルのサイズが不正です。" & vbCrLf &
                                           "ファイルサイズ：" & fileBytes.Length.ToString("#,##0") & "バイト" & vbCrLf &
                                           "1レコードは128バイトである必要があります。")
        End If

        Dim result As New PlanImportResult
        result.RecordCount = fileBytes.Length \ RecordLength

        ' Key：yyyyMMdd + 商品コード
        Dim itemDictionary As New Dictionary(Of String, PlanImportItem)(StringComparer.OrdinalIgnoreCase)

        Dim storeDefinitions As List(Of PlanImportStoreDefinition) = GetStoreDefinitions()

        For recordIndex As Integer = 0 To result.RecordCount - 1
            Dim recordNumber As Integer = recordIndex + 1

            Try
                Dim recordText As String = Encoding.ASCII.GetString(fileBytes, recordIndex * RecordLength, RecordLength)
                Dim planDate As Date = ParsePlanDate(recordText.Substring(0, 8))

                ' 商品コード欄は13桁。
                ' 短い商品コードは右側に空白が入る。
                Dim productCode As String = recordText.Substring(19, 13).Trim()

                If String.IsNullOrEmpty(productCode) Then
                    Throw New ApplicationException("商品コードが設定されていません。")
                End If

                Dim importItem As PlanImportItem = Nothing
                For Each definition As PlanImportStoreDefinition In storeDefinitions

                    Dim quantity As Integer = ParseQuantity(recordText, definition)
                    If quantity < 0 Then
                        Throw New ApplicationException(definition.StoreName & "の発注数がマイナスになっています。")
                    End If

                    ' 加算取込なので数量0は何もしない。
                    If quantity = 0 Then Continue For

                    If importItem Is Nothing Then
                        Dim dictionaryKey As String = planDate.ToString("yyyyMMdd") & ChrW(0) & productCode
                        If Not itemDictionary.TryGetValue(dictionaryKey, importItem) Then
                            importItem = New PlanImportItem
                            importItem.PlanDate = planDate.Date
                            importItem.ProductCode = productCode
                            itemDictionary.Add(dictionaryKey, importItem)
                        End If
                    End If

                    Dim currentQuantity As Integer = 0
                    If importItem.Quantities.TryGetValue(definition.StoreCode, currentQuantity) Then
                        ' 同じ日付・商品・店舗がファイル内に
                        ' 複数あれば合算する。
                        importItem.Quantities(definition.StoreCode) = currentQuantity + quantity
                    Else
                        importItem.Quantities.Add(definition.StoreCode, quantity)
                    End If
                Next
            Catch ex As ApplicationException
                Throw New ApplicationException(recordNumber.ToString("#,##0") & "レコード目：" & ex.Message, ex)
            Catch ex As Exception
                Throw New ApplicationException(recordNumber.ToString("#,##0") & "レコード目の形式が不正です。" & vbCrLf & ex.Message, ex)
            End Try
        Next

        For Each importItem As PlanImportItem In itemDictionary.Values
            result.Items.Add(importItem)
        Next

        result.Items = result.Items.OrderBy(Function(x) x.PlanDate).ThenBy(Function(x) x.ProductCode).ToList()

        Return result

    End Function

    ' yyyyMMddまたは00yyMMddを日付へ変換する。
    Private Function ParsePlanDate(rawValue As String) As Date

        Dim value As String = rawValue.Trim()

        If value.Length <> 8 OrElse Not value.All(Function(c As Char) Char.IsDigit(c)) Then
            Throw New ApplicationException("年月日の形式が不正です。" & vbCrLf & "値：" & rawValue)
        End If

        ' 例：00260915を2026/09/15として扱う。
        If value.StartsWith("00") Then
            Dim year As Integer
            Dim month As Integer
            Dim day As Integer

            If Not Integer.TryParse(value.Substring(2, 2), year) OrElse
               Not Integer.TryParse(value.Substring(4, 2), month) OrElse
               Not Integer.TryParse(value.Substring(6, 2), day) Then
                Throw New ApplicationException("年月日の形式が不正です。" & vbCrLf & "値：" & rawValue)
            End If

            Try
                Return New Date(2000 + year, month, day)
            Catch ex As Exception
                Throw New ApplicationException("年月日として存在しない値です。" & vbCrLf & "値：" & rawValue)
            End Try
        End If

        Dim result As Date
        If Not Date.TryParseExact(value, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, result) Then
            Throw New ApplicationException("年月日の形式が不正です。" & vbCrLf & "値：" & rawValue)
        End If

        Return result.Date

    End Function

    ' 店舗別数量を解析する。
    Private Function ParseQuantity(recordText As String, definition As PlanImportStoreDefinition) As Integer

        Dim quantityText As String = recordText.Substring(definition.QuantityStartPosition, QuantityLength).Trim()
        If String.IsNullOrEmpty(quantityText) Then
            Return 0
        End If

        Dim quantity As Integer
        If Not Integer.TryParse(quantityText, quantity) Then
            Throw New ApplicationException(definition.StoreName & "の発注数が数値ではありません。" & vbCrLf & "値：" & quantityText)
        End If

        Dim signText As String = recordText.Substring(definition.SignStartPosition, SignLength).Trim()
        If signText.Contains("-") Then
            quantity *= -1
        End If

        Return quantity

    End Function

    ' 外部店舗コードとDataGridView上の店舗列番号を対応付ける。
    Private Function CreateStoreIndexDictionary() As Dictionary(Of String, Integer)

        Dim result As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
        Dim errorList As New List(Of String)

        For Each definition As PlanImportStoreDefinition In PlanImportFileReader.GetStoreDefinitions()
            Dim targetName As String = NormalizeStoreName(definition.StoreName)
            Dim matchedIndexes As New List(Of Integer)

            For index As Integer = 0 To _storeList.Count - 1
                Dim store As Store = _storeList(index)
                Dim storeName As String = NormalizeStoreName(store.StoreName)
                Dim storeShortName As String = NormalizeStoreName(store.StoreShortName)
                If storeName = targetName OrElse storeShortName = targetName Then
                    matchedIndexes.Add(index)
                End If
            Next

            If matchedIndexes.Count = 0 Then
                errorList.Add(definition.StoreCode & " " & definition.StoreName & "：店舗マスタに該当する店舗がありません。")
            ElseIf matchedIndexes.Count > 1 Then
                errorList.Add(definition.StoreCode & " " & definition.StoreName & "：店舗マスタに複数の候補があります。")
            Else
                result.Add(definition.StoreCode, matchedIndexes(0))
            End If
        Next

        If errorList.Count > 0 Then
            Throw New ApplicationException("計画データの店舗を店舗マスタと照合できませんでした。" & vbCrLf &
            "店舗名または店舗略称を確認してください。" & vbCrLf & vbCrLf & String.Join(vbCrLf, errorList.ToArray()))
        End If

        Return result

    End Function

    ' 店舗名を照合用に正規化する。
    Private Function NormalizeStoreName(value As String) As String

        If value Is Nothing Then
            Return String.Empty
        End If

        Dim result As String = ToNarrow(value).Trim()
        result = result.Replace(" ", String.Empty)
        result = result.Replace("　", String.Empty)

        ' 「02岩美店」のようにコードが付いていれば取り除く。
        If result.Length >= 2 AndAlso Char.IsDigit(result(0)) AndAlso Char.IsDigit(result(1)) Then
            result = result.Substring(2)
        End If

        If result.EndsWith("店") Then
            result = result.Substring(0, result.Length - 1)
        End If

        Return result.Trim()

    End Function

    ' 取込データから、DB登録用の計画リストを作成する。
    ' この処理ではまだDBを更新しない。
    Private Function CreatePlanRegistration(importData As PlanImportResult, storeIndexDictionary As Dictionary(Of String, Integer)) As PlanImportRegistration

        Dim result As New PlanImportRegistration

        ' 商品マスタをJANコード・品目コード別に辞書化する。
        Dim janDictionary As New Dictionary(Of String, List(Of Item))(StringComparer.OrdinalIgnoreCase)
        Dim itemCodeDictionary As New Dictionary(Of String, List(Of Item))(StringComparer.OrdinalIgnoreCase)

        For Each item As Item In Items.GetAll()
            Dim janCode As String = NormalizeProductCode(item.JanCode)
            Dim itemCode As String = NormalizeProductCode(item.ItemCode)

            If Not String.IsNullOrEmpty(janCode) Then AddItemDictionary(janDictionary, janCode, item)
            If Not String.IsNullOrEmpty(itemCode) Then AddItemDictionary(itemCodeDictionary, itemCode, item)
        Next

        ' 同じ商品コードは、日付が違っても同じ商品に対応する。
        Dim resolvedItemDictionary As New Dictionary(Of String, Item)(StringComparer.OrdinalIgnoreCase)
        Dim unmatchedCodeSet As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        Dim duplicateCodeSet As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

        ' 最初に全商品を検証する。
        ' 未登録・重複がある場合はDB登録を開始しない。
        For Each importItem As PlanImportItem In importData.Items
            Dim productCode As String = NormalizeProductCode(importItem.ProductCode)
            If resolvedItemDictionary.ContainsKey(productCode) Then
                Continue For
            End If

            Dim matchedItems As List(Of Item) = Nothing

            ' JANコードを優先する。
            If Not janDictionary.TryGetValue(productCode, matchedItems) Then
                itemCodeDictionary.TryGetValue(productCode, matchedItems)
            End If

            If matchedItems Is Nothing OrElse matchedItems.Count = 0 Then
                unmatchedCodeSet.Add(productCode)
                Continue For
            End If

            If matchedItems.Count > 1 Then
                duplicateCodeSet.Add(productCode)
                Continue For
            End If

            resolvedItemDictionary.Add(productCode, matchedItems(0))
        Next

        If unmatchedCodeSet.Count > 0 OrElse duplicateCodeSet.Count > 0 Then
            Dim errorMessage As String = "商品マスタと照合できない計画データがあります。" & vbCrLf & "計画データは登録されていません。"

            If unmatchedCodeSet.Count > 0 Then
                Dim values As New List(Of String)(unmatchedCodeSet)
                values.Sort()
                errorMessage &= vbCrLf & vbCrLf & "【未登録商品：" & values.Count.ToString("#,##0") & "件】" & vbCrLf &
                CreateImportErrorListText(values)
            End If

            If duplicateCodeSet.Count > 0 Then
                Dim values As New List(Of String)(duplicateCodeSet)
                values.Sort()
                errorMessage &= vbCrLf & vbCrLf & "【商品コード重複：" & values.Count.ToString("#,##0") & "件】" &
                vbCrLf & CreateImportErrorListText(values)
            End If

            Throw New ApplicationException(errorMessage)
        End If

        For Each importItem As PlanImportItem In importData.Items
            Dim productCode As String = NormalizeProductCode(importItem.ProductCode)
            Dim item As Item = resolvedItemDictionary(productCode)
            Dim planDateText As String = importItem.PlanDate.ToString("yyyy/MM/dd")

            ' 同じ計画日・品目の既存計画を取得する。
            Dim plan As Plan = Plans.GetByKey(planDateText, item.ItemId)

            Dim isNewPlan As Boolean = plan Is Nothing

            If Not isNewPlan Then
                ' 製造指示が作成されている計画は変更しない。
                Dim instruction As Instruction = Instructions.GetByPlanId(plan.PlanId)

                If instruction IsNot Nothing Then
                    result.SkippedStartedPlans.Add(importItem.PlanDate.ToString("yyyy/MM/dd") & " " & item.ItemName & "（" & productCode & "）")
                    Continue For
                End If
            Else
                plan = New Plan
                plan.PlanDate = planDateText
                plan.Item = item
                plan.UnitType = item.UnitType
                plan.InstructionStart = False
            End If

            ' 既存のPlanDetailsを店舗別に辞書化する。
            ' 既存明細が重複していた場合も、ここで合算する。
            Dim detailDictionary As New Dictionary(Of String, PlanDetail)(StringComparer.OrdinalIgnoreCase)

            If plan.PlanDetails IsNot Nothing Then
                For Each detail As PlanDetail In plan.PlanDetails
                    If detail Is Nothing OrElse detail.Store Is Nothing Then
                        Continue For
                    End If

                    Dim storeId As String = Convert.ToString(detail.Store.StoreId).Trim()
                    If String.IsNullOrEmpty(storeId) Then
                        Continue For
                    End If

                    Dim existingDetail As PlanDetail = Nothing

                    If detailDictionary.TryGetValue(storeId, existingDetail) Then
                        Dim sumQuantity As Double = CDbl(Nz(existingDetail.Quantity, 0)) + CDbl(Nz(detail.Quantity, 0))
                        existingDetail.Quantity = sumQuantity.ToString()
                    Else
                        detailDictionary.Add(storeId, detail)
                    End If
                Next
            End If

            ' ファイル数量を既存数量へ加算する。
            For Each quantityEntry As KeyValuePair(Of String, Integer) In importItem.Quantities
                Dim StoreCode As String = quantityEntry.Key
                Dim importQuantity As Integer = quantityEntry.Value

                If importQuantity <= 0 Then
                    Continue For
                End If

                Dim storeIndex As Integer = storeIndexDictionary(StoreCode)
                Dim store As Store = _storeList(storeIndex)
                Dim storeId As String = Convert.ToString(store.StoreId).Trim()
                Dim detail As PlanDetail = Nothing

                If detailDictionary.TryGetValue(storeId, detail) Then
                    Dim currentQuantity As Double = CDbl(Nz(detail.Quantity, 0))
                    detail.Quantity = (currentQuantity + importQuantity).ToString()
                Else
                    detail = New PlanDetail
                    detail.Store = store
                    detail.Quantity = importQuantity.ToString()
                    detailDictionary.Add(storeId, detail)
                End If
                result.AddedQuantity += importQuantity
            Next

            ' Plans.SaveではPlanDetailsを一度削除して再登録するため、
            ' 対象外店舗を含む全既存明細を戻す。
            plan.PlanDetails = New List(Of PlanDetail)

            For Each detail As PlanDetail In detailDictionary.Values
                If CDbl(Nz(detail.Quantity, 0)) > 0 Then
                    plan.PlanDetails.Add(detail)
                End If
            Next
            result.PlanList.Add(plan)

            If isNewPlan Then
                result.NewPlanCount += 1
            Else
                result.UpdatePlanCount += 1
            End If
        Next

        Return result

    End Function

    ' 商品コードを照合用に正規化する。
    ' 先頭の0は商品コードの一部なので削除しない。
    Private Function NormalizeProductCode(value As String) As String

        If value Is Nothing Then
            Return String.Empty
        End If

        Return ToNarrow(value).Trim()

    End Function

    ' 商品コード辞書へ商品を追加する。
    ' 重複コードを検出するためListで保持する。
    Private Sub AddItemDictionary(dictionary As Dictionary(Of String, List(Of Item)), code As String, item As Item)

        Dim list As List(Of Item) = Nothing

        If Not dictionary.TryGetValue(code, list) Then
            list = New List(Of Item)
            dictionary.Add(code, list)
        End If

        list.Add(item)

    End Sub

    ' エラー一覧用の文字列を作成する。
    ' 最大20件まで表示する。
    Public Shared Function CreateImportErrorListText(values As List(Of String)) As String

        Const MaxDisplayCount As Integer = 20

        Dim displayList As New List(Of String)

        For index As Integer = 0 To Math.Min(values.Count, MaxDisplayCount) - 1
            displayList.Add("・" & values(index))
        Next

        If values.Count > MaxDisplayCount Then
            displayList.Add("・ほか " & (values.Count - MaxDisplayCount).ToString("#,##0") & "件")
        End If

        Return String.Join(vbCrLf, displayList.ToArray())

    End Function

End Class