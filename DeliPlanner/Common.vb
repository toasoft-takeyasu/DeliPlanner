Imports Npgsql
Imports NpgsqlTypes
Imports System.Runtime.InteropServices
Imports System.Text.RegularExpressions

Public Module Common

#Region "システム共通"

#Region "定数"

#End Region

#Region "変数"

    Public startForm As Form

    Public sharedIniFile As New IniFile("DeliPlanner.ini")
    Public sharedOperator As [Operator] = Nothing

#End Region

#Region "列挙型"

    Public Enum EditMode
        Add
        Edit
    End Enum

    Public Enum OperatorRole
        Administrator = 1   ' 管理者
        General = 2         ' 一般
    End Enum

    Public MeasureTypes As New Dictionary(Of MeasureType, String)
    Public Enum MeasureType
        Weighing = 0
        FixedPrice = 1
    End Enum

#End Region

#Region "関数"

    Public Sub SetCollections()

        MeasureTypes.Clear()
        MeasureTypes.Add(MeasureType.Weighing, "計量")
        MeasureTypes.Add(MeasureType.FixedPrice, "定額")

    End Sub

    ' 担当者コードとパスワードで認証する
    Public Function Login(operatorCode As String, password As String) As Boolean

        Try
            Dim target As [Operator] = Operators.GetByCode(operatorCode)

            ' 該当なし
            If target Is Nothing Then Return False

            ' 使用停止
            If CInt(Nz(target.UnusedFlag, 0)) = 1 Then Return False

            ' パスワード相違
            If Not Operators.VerifyPassword(target, password) Then Return False

            sharedOperator = target
            Operators.UpdateLastLoginDate(target.OperatorId)

        Catch ex As Exception
            SysMsgBox(ex.Message, Application.ProductName, Reflection.MethodBase.GetCurrentMethod.Name)
            Return False
        End Try

        Return True

    End Function

    ' ログアウトする
    Public Sub Logout()

        sharedOperator = Nothing

    End Sub

    ' ソルトを新しく作る
    Public Function NewPasswordSalt() As String

        Return Guid.NewGuid().ToString("N")

    End Function

    ' パスワードとソルトからハッシュ値(16進64文字)を作る
    Public Function HashPassword(password As String, salt As String) As String

        If password Is Nothing Then password = String.Empty

        Using derive As New Security.Cryptography.Rfc2898DeriveBytes(
                password, Text.Encoding.UTF8.GetBytes(salt), 10000)
            Return BitConverter.ToString(derive.GetBytes(32)).Replace("-", "").ToLower()
        End Using

    End Function

    Public Function TryGetItemTypeBackColor(colors As Dictionary(Of String, Color),
                                            itemTypeName As String,
                                            ByRef backColor As Color) As Boolean

        backColor = Color.Empty

        If colors Is Nothing Then Return False
        If String.IsNullOrEmpty(itemTypeName) Then Return False

        Return colors.TryGetValue(itemTypeName, backColor)

    End Function

    ' 入数は省略できる。省略時(Null / 空 / 0)は1として扱う
    Public Function EffectivePerCase(value As Object) As Double

        Dim perCase As Double
        If Not Double.TryParse(Convert.ToString(Nz(value, String.Empty)), perCase) Then Return 1
        If perCase <= 0 Then Return 1

        Return perCase

    End Function

    ' 基準単価は小数第2位で切り上げる(調味料などで0円になるのを防ぐ)
    Public Function CeilingPricePerUnit(value As Double) As Double

        If value <= 0 Then Return 0

        ' Double のまま100倍すると誤差で1つ上に繰り上がるのでDecimalで計算する
        Dim scaled As Decimal = CDec(value) * 100D

        Return CDbl(Math.Ceiling(scaled) / 100D)

    End Function

    ' 店舗別数量の表示書式を返す(0は空白にする)
    Public Function GetExcelStoreQuantityFormat(value As Double) As String

        ' 「正;負;ゼロ」の3セクション。ゼロを空にしている
        If value = Math.Truncate(value) Then Return "#,##0;-#,##0;"

        Return "#,##0.###;-#,##0.###;"

    End Function

    ' 日付セルの値をそのままToStringすると時刻が付くので整形する
    Public Function FormatCellDate(value As Object) As String

        Dim target As Date
        If Not Date.TryParse(Convert.ToString(value), target) Then Return String.Empty
        Return target.ToString("yyyy/MM/dd")

    End Function

    Public Sub SysMsgBox(errorMessage As String, title As String, Optional method As String = "")

        Dim message As String = "処理中に異常が発生しました。" & vbLf & "以下のメッセージをシステム担当者に連絡してください。" & vbLf & vbLf &
                                "「" & errorMessage & "」" & IIf(Not String.IsNullOrEmpty(method), vbLf & "->" & method, String.Empty)
        MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error)

    End Sub

#End Region

#End Region

#Region "製品->材料必要数取得"

    Private _calc As MaterialCalculator

    ' 初回アクセス時に生成し、マスタ更新時に破棄して作り直す
    Public ReadOnly Property calc As MaterialCalculator
        Get
            If _calc Is Nothing Then _calc = New MaterialCalculator()
            Return _calc
        End Get
    End Property

    ' 品目・レシピ・単位などのマスタが変わったら呼ぶ
    Public Sub InvalidateCalc()
        _calc = Nothing
    End Sub

    Public Class MaterialCalculator

        Private ReadOnly _recipe As New Dictionary(Of Integer, List(Of Material))       ' 親品目ID -> 構成(Material: 子品目ID, 数量, 単位ID)
        Private ReadOnly _recipeNeeded As New Dictionary(Of Integer, Boolean)           ' 品目ID -> レシピ要否(True = レシピあり)
        Private ReadOnly _weightNeeded As New Dictionary(Of Integer, Boolean)           ' 品目ID -> 重量管理要否(True = g単価で管理)
        Private ReadOnly _expiryDateNeeded As New Dictionary(Of Integer, Boolean)       ' 品目ID -> 賞味期限管理要否
        Private ReadOnly _inventory As New Dictionary(Of Integer, Double)               ' 品目ID -> 在庫数
        Private ReadOnly _itemName As New Dictionary(Of Integer, String)                ' 品目ID -> 品目名
        Private ReadOnly _itemTypeID As New Dictionary(Of Integer, Integer)             ' 品目ID -> 品目区分ID
        Private ReadOnly _lossRate As New Dictionary(Of Integer, Double)                ' 品目ID -> 歩留率(0〜1)。既定 1.0
        Private ReadOnly _itemTypeName As New Dictionary(Of Integer, String)            ' 品目区分ID -> 品目区分名
        Private ReadOnly _unitTypeName As New Dictionary(Of Integer, String)            ' 単位ID -> 単位名
        Private ReadOnly _itemUnitTypeId As New Dictionary(Of Integer, Integer)         ' 品目ID -> 品目自身の単位ID(Items.UnitTypeId)
        Private ReadOnly _inventoryUnitTypeId As New Dictionary(Of Integer, Integer)    ' 品目ID -> 在庫単位ID(Inventories.UnitTypeId)
        Private ReadOnly _unitCategory As New Dictionary(Of Integer, Integer)           ' 単位ID -> 換算グループ
        Private ReadOnly _unitFactor As New Dictionary(Of Integer, Double)              ' 単位ID -> 基準単位への係数
        Private ReadOnly _unitNonConvertible As New Dictionary(Of Integer, Boolean)     ' 単位ID -> 換算不可フラグ
        Private ReadOnly _memo As New Dictionary(Of Integer, Dictionary(Of MaterialKey, Double)) ' メモ化: 品目ID -> ((材料品目ID+単位ID) -> 1個あたり必要量)
        Private ReadOnly _finestUnit As New Dictionary(Of Integer, Integer)             ' 区分 -> 最小単位ID
        Private ReadOnly _levelCode As New Dictionary(Of Integer, Integer)              ' 品目ID -> 低位コード(0=最上位)

        Public Class Product
            Public ItemId As Integer
            Public Quantity As Double
            Public UnitTypeId As Integer

            Public Sub New(itemId As Integer, quantity As Double, unitTypeId As Integer)
                Me.ItemId = itemId
                Me.Quantity = quantity
                Me.UnitTypeId = unitTypeId
            End Sub
        End Class

        Public Class Material
            Public ItemId As Integer
            Public ItemName As String
            Public ItemTypeId As Integer
            Public ItemTypeName As String
            Public Quantity As Double
            Public UnitTypeId As Integer
            Public UnitTypeName As String

            Public Sub New(itemId As Integer, itemName As String, itemTypeId As Integer, itemTypeName As String, quantity As Double, unitTypeId As Integer, unitTypeName As String)
                Me.ItemId = itemId
                Me.ItemName = itemName
                Me.ItemTypeId = itemTypeId
                Me.ItemTypeName = itemTypeName
                Me.Quantity = quantity
                Me.UnitTypeId = unitTypeId
                Me.UnitTypeName = unitTypeName
            End Sub
        End Class

        ' 正味所要量の1行
        Public Class Requirement
            Public ItemId As Integer
            Public ItemName As String
            Public ItemTypeId As Integer
            Public ItemTypeName As String
            Public UnitTypeId As Integer            ' 品目自身の単位
            Public UnitTypeName As String
            Public GrossQuantity As Double          ' 総所要量(親から降りてきた分)
            Public PlanQuantity As Double           ' 製造計画に登録済みの数量
            Public InventoryQuantity As Double      ' 在庫
            Public ProductionQuantity As Double     ' 実際に作る/使う量
            Public NetQuantity As Double            ' 在庫を引いた不足量
            Public Exploded As Boolean              ' この品目を下位に展開したか(表示の除外に使う)
        End Class

        ' 集計キー: 品目ID + 単位ID(レシピ明細の単位)
        Private Class MaterialKey
            Implements IEquatable(Of MaterialKey)

            Public ReadOnly ItemId As Integer
            Public ReadOnly UnitTypeId As Integer

            Public Sub New(itemId As Integer, unitTypeId As Integer)
                Me.ItemId = itemId
                Me.UnitTypeId = unitTypeId
            End Sub

            Public Overloads Function Equals(other As MaterialKey) As Boolean Implements IEquatable(Of MaterialKey).Equals
                Return ItemId = other.ItemId AndAlso UnitTypeId = other.UnitTypeId
            End Function

            Public Overrides Function Equals(obj As Object) As Boolean
                Return TypeOf obj Is MaterialKey AndAlso Equals(DirectCast(obj, MaterialKey))
            End Function

            Public Overrides Function GetHashCode() As Integer
                Return (ItemId * 397) Xor UnitTypeId
            End Function
        End Class

        ' 辞書への加算ヘルパー
        Private Shared Sub AddTo(dict As Dictionary(Of MaterialKey, Double), key As MaterialKey, value As Double)
            Dim cur As Double
            If dict.TryGetValue(key, cur) Then
                dict(key) = cur + value
            Else
                dict(key) = value
            End If
        End Sub

        Public Sub New()

            LoadItems()   ' 品目区分・歩留率
            LoadRecipes()     ' レシピ構成
            LoadLevelCodes()          ' レシピの階層
            LoadInventories()     ' 在庫
            LoadInventoryWeights()
            LoadItemTypes() ' 品目区分
            LoadUnitTypes() ' 単位

        End Sub

        ' 品目マスタから 品目名、品目区分、レシピ要否、歩留率、単位ID を読み込む
        Private Sub LoadItems()

            _recipeNeeded.Clear()
            _itemName.Clear()
            _lossRate.Clear()
            _itemTypeID.Clear()
            _itemUnitTypeId.Clear()
            _weightNeeded.Clear()
            _expiryDateNeeded.Clear()

            Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
                cn.Open()
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String = String.Empty
                    SQLString = "select"
                    SQLString &= " Items.ItemId"
                    SQLString &= ",Items.ItemName"
                    SQLString &= ",Items.ItemTypeId"
                    SQLString &= ",ItemTypes.RecipeNeedFlag"
                    SQLString &= ",ItemTypes.WeightNeedFlag as WeightNeedFlag"
                    SQLString &= ",Items.ExpiryDateNeedFlag as ItemExpiryDateNeedFlag"
                    SQLString &= ",ItemTypes.ExpiryDateNeedFlag as ItemTypeExpiryDateNeedFlag"
                    SQLString &= ",Items.LossRate"
                    SQLString &= ",Items.UnitTypeId"
                    SQLString &= " from Items"
                    SQLString &= " left join ItemTypes on Items.ItemTypeId = ItemTypes.ItemTypeId"
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        While dr.Read()
                            Dim itemId As Integer = CInt(dr("ItemId"))

                            _recipeNeeded(itemId) = (CInt(Nz(dr("RecipeNeedFlag"), 0)) = 1)
                            _weightNeeded(itemId) = dr("WeightNeedFlag")
                            If Not IsDBNull(dr("ItemExpiryDateNeedFlag")) Then
                                _expiryDateNeeded(itemId) = dr("ItemExpiryDateNeedFlag")
                            Else
                                _expiryDateNeeded(itemId) = dr("ItemTypeExpiryDateNeedFlag")
                            End If
                            _itemName(itemId) = dr("ItemName")
                            _itemTypeID(itemId) = dr("ItemTypeId")
                            _itemUnitTypeId(itemId) = CInt(Nz(dr("UnitTypeId"), 0))

                            ' 歩留率（0またはNULLは1.0（ロスなし）扱い）
                            Dim lossRate As Double = CDbl(Nz(dr("LossRate"), 1))
                            If lossRate <= 0 Then lossRate = 1.0 ' 
                            _lossRate(itemId) = lossRate
                        End While
                    End Using
                End Using
            End Using

        End Sub

        ' レシピ(親)＋レシピ明細(子)を読み込み、辞書化する
        Private Sub LoadRecipes()

            _recipe.Clear()

            Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
                cn.Open()
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String = String.Empty
                    SQLString = "select"
                    SQLString &= " Recipes.ItemId as ParentItemId"
                    SQLString &= ",RecipeDetails.ItemId as ChildItemId"
                    SQLString &= ",Items.ItemName"
                    SQLString &= ",Items.ItemTypeId"
                    SQLString &= ",ItemTypes.ItemTypeName"
                    SQLString &= ",RecipeDetails.Quantity"
                    SQLString &= ",RecipeDetails.UnitTypeId"
                    SQLString &= ",UnitTypes.UnitTypeName"
                    SQLString &= " from Recipes"
                    SQLString &= " inner join RecipeDetails on Recipes.RecipeId = RecipeDetails.RecipeId"
                    SQLString &= " left join Items on RecipeDetails.ItemId = Items.ItemId"
                    SQLString &= " left join ItemTypes on Items.ItemTypeId = ItemTypes.ItemTypeId"
                    SQLString &= " left join UnitTypes on RecipeDetails.UnitTypeId = UnitTypes.UnitTypeId"
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        While dr.Read()
                            Dim parentItemId As Integer = CInt(dr("ParentItemId"))
                            Dim material As New Material(CInt(dr("ChildItemId")), dr("ItemName"), dr("ItemTypeId"), dr("ItemTypeName"), CDbl(Nz(dr("Quantity"), 0)), CInt(dr("UnitTypeId")), dr("UnitTypeName"))

                            Dim list As List(Of Material) = Nothing
                            If Not _recipe.TryGetValue(parentItemId, list) Then
                                list = New List(Of Material)
                                _recipe(parentItemId) = list
                            End If
                            list.Add(material)
                        End While
                    End Using
                End Using
            End Using

        End Sub

        ' レシピを辿って品目ごとの階層(低位コード)を求める
        Private Sub LoadLevelCodes()

            _levelCode.Clear()

            For Each parentItemId As Integer In _recipe.Keys
                AssignLevelCode(parentItemId, 0, New HashSet(Of Integer))
            Next

        End Sub

        Private Sub AssignLevelCode(itemId As Integer, level As Integer, path As HashSet(Of Integer))

            If Not path.Add(itemId) Then Exit Sub   ' 循環参照ガード

            Dim current As Integer
            If Not _levelCode.TryGetValue(itemId, current) OrElse level > current Then
                _levelCode(itemId) = level          ' 同じ品目が複数階層に出る場合は深いほうを採用
            End If

            If _recipe.ContainsKey(itemId) Then
                For Each material As Material In _recipe(itemId)
                    AssignLevelCode(material.ItemId, level + 1, path)
                Next
            End If

            path.Remove(itemId)

        End Sub

        Private Function GetLevelCode(itemId As Integer) As Integer

            Dim level As Integer
            If _levelCode.TryGetValue(itemId, level) Then Return level
            Return 0

        End Function

        ' 在庫を品目自身の単位で取得する
        Private Function GetInventoryInItemUnit(itemId As Integer) As Double

            Dim baseQuantity As Double = GetInventoryWeight(itemId)
            If baseQuantity = 0 Then Return 0

            Dim baseUnitId As Integer = GetBaseUnitTypeId(itemId)
            Dim itemUnitId As Integer = GetItemUnitTypeId(itemId)

            Dim converted As Double
            If TryConvert(baseQuantity, baseUnitId, itemUnitId, converted) Then Return converted
            Return baseQuantity

        End Function

        ' 在庫データを読み込み、辞書化する
        Private Sub LoadInventories()

            _inventory.Clear()
            _inventoryUnitTypeId.Clear()

            Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
                cn.Open()
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String = String.Empty
                    SQLString = " select"
                    SQLString &= " ItemId"
                    SQLString &= ",sum(Quantity) as Inventory"
                    SQLString &= ",UnitTypeId"
                    SQLString &= " from Inventories"
                    SQLString &= " group by"
                    SQLString &= " ItemId"
                    SQLString &= ",UnitTypeId"
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        While dr.Read()
                            Dim itemId As Integer = CInt(dr("ItemId"))
                            _inventory(itemId) = CDbl(Nz(dr("Inventory"), 0))
                            _inventoryUnitTypeId(itemId) = CInt(Nz(dr("UnitTypeId"), 0))
                        End While
                    End Using
                End Using
            End Using

        End Sub

        Private Sub LoadItemTypes()

            _itemTypeName.Clear()

            For Each itemType As ItemType In ItemTypes.GetAll()
                _itemTypeName(itemType.ItemTypeId) = itemType.ItemTypeName
            Next

        End Sub

        Private Sub LoadUnitTypes()

            _unitTypeName.Clear()
            _unitCategory.Clear()
            _unitFactor.Clear()
            _unitNonConvertible.Clear()

            Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
                cn.Open()
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String = String.Empty
                    SQLString = "select"
                    SQLString &= " UnitTypeId"
                    SQLString &= ",UnitTypeName"
                    SQLString &= ",UnitCategory"
                    SQLString &= ",BaseFactor"
                    SQLString &= ",NonConvertibleFlag"
                    SQLString &= " from UnitTypes"
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        While dr.Read()
                            Dim unitTypeId As Integer = CInt(dr("UnitTypeId"))
                            _unitTypeName(unitTypeId) = dr("UnitTypeName")
                            _unitCategory(unitTypeId) = CInt(Nz(dr("UnitCategory"), 0))

                            ' 係数が0/NULLは1.0扱い(換算しない)
                            Dim factor As Double = CDbl(Nz(dr("BaseFactor"), 1))
                            If factor <= 0 Then factor = 1.0
                            _unitFactor(unitTypeId) = factor

                            _unitNonConvertible(unitTypeId) = (CInt(Nz(dr("NonConvertibleFlag"), 0)) = 1)
                        End While
                    End Using
                End Using
            End Using

            _finestUnit.Clear()
            For Each category In _unitCategory.Values.Distinct()
                _finestUnit(category) = FinestUnitTypeId(category)
            Next

        End Sub

        ' 複数製品をまとめて算出。重複する材料/資材は合算する
        Public Function CalcRequiredMaterials(products As IEnumerable(Of Product), Optional recursive As Boolean = True) As List(Of Material)

            Dim totals As New Dictionary(Of MaterialKey, Double)

            If products IsNot Nothing Then
                For Each product In products
                    If product Is Nothing OrElse product.Quantity = 0 Then Continue For

                    Dim perUnit As Dictionary(Of MaterialKey, Double)
                    Dim quantity As Double = product.Quantity

                    If ShouldExplode(product.ItemId) Then
                        perUnit = If(recursive, ExplodePerUnit(product.ItemId, New HashSet(Of Integer)), DirectComponentsPerUnit(product.ItemId))
                        ' 指定単位 → 品目自身の単位 に換算
                        quantity = ToItemUnitQuantity(product.ItemId, product.Quantity, product.UnitTypeId)
                    Else
                        ' レシピの無い品目そのもの(渡された単位を使用)
                        perUnit = New Dictionary(Of MaterialKey, Double)
                        perUnit(New MaterialKey(product.ItemId, product.UnitTypeId)) = 1.0
                    End If

                    For Each kv In perUnit
                        AddTo(totals, kv.Key, kv.Value * quantity)
                    Next
                Next
            End If

            ' 単位換算して「同一品目 × 同一単位区分」でまとめる
            Dim groupBase As New Dictionary(Of String, Double)   ' groupKey -> 基準単位での合計
            Dim groupItem As New Dictionary(Of String, Integer)  ' groupKey -> 品目ID
            Dim groupCategory As New Dictionary(Of String, Integer) ' groupKey -> 単位区分
            Dim groupUnit As New Dictionary(Of String, Integer)  ' groupKey -> 単位ID(区分未設定用)
            Dim groupConvertible As New Dictionary(Of String, Boolean)
            Dim groupOrder As New List(Of String)

            For Each kv In totals
                Dim itemId As Integer = kv.Key.ItemId
                Dim unitId As Integer = kv.Key.UnitTypeId
                Dim qty As Double = kv.Value

                Dim convertible As Boolean = IsConvertibleUnit(unitId)   ' ← 単位で判定
                Dim category As Integer = GetUnitCategory(unitId)

                Dim factor As Double = 1.0
                Dim groupKey As String
                If convertible Then
                    factor = GetBaseFactor(unitId)
                    groupKey = itemId & "_c" & category
                Else
                    groupKey = itemId & "_u" & unitId        ' 換算しない単位は単位ごと
                End If

                Dim baseAdd As Double = qty * factor
                If groupBase.ContainsKey(groupKey) Then
                    groupBase(groupKey) += baseAdd
                Else
                    groupBase(groupKey) = baseAdd
                    groupItem(groupKey) = itemId
                    groupCategory(groupKey) = category
                    groupUnit(groupKey) = unitId
                    groupConvertible(groupKey) = convertible
                    groupOrder.Add(groupKey)
                End If
            Next

            Dim result As New List(Of Material)
            For Each groupKey In groupOrder
                Dim itemId As Integer = groupItem(groupKey)

                Dim displayUnitId As Integer
                Dim qty As Double
                If groupConvertible(groupKey) Then
                    displayUnitId = FinestUnitTypeId(groupCategory(groupKey))
                    qty = groupBase(groupKey) / GetBaseFactor(displayUnitId)
                Else
                    displayUnitId = groupUnit(groupKey)     ' レシピ明細の単位そのまま
                    qty = groupBase(groupKey)
                End If

                Dim itemTypeId As Integer = GetItemTypeId(itemId)
                result.Add(New Material(itemId, GetItemName(itemId), itemTypeId, GetItemTypeName(itemTypeId), qty, displayUnitId, GetUnitTypeName(displayUnitId)))
            Next
            Return result

        End Function

        ' 単一製品版
        Public Function CalcRequiredMaterials(itemId As Integer, quantity As Double, unitTypeId As Integer, Optional recursive As Boolean = True) As List(Of Material)

            Return CalcRequiredMaterials({New Product(itemId, quantity, unitTypeId)}, recursive)

        End Function

        ' 「品目1個(良品)あたり」に必要な材料/資材。キーは(品目ID+レシピ明細の単位)
        Private Function ExplodePerUnit(itemId As Integer, path As HashSet(Of Integer)) As Dictionary(Of MaterialKey, Double)

            Dim cached As Dictionary(Of MaterialKey, Double) = Nothing
            If _memo.TryGetValue(itemId, cached) Then Return cached

            Dim materials As New Dictionary(Of MaterialKey, Double)

            ' レシピが無ければ空(呼び出し側でガード済み)
            If Not _recipe.ContainsKey(itemId) Then Return materials

            ' 循環参照ガード
            If Not path.Add(itemId) Then Return materials

            ' 自身の歩留率(良品1個を得るための投入係数 = 1/Y)
            Dim invYield As Double = 1.0 / GetLossRate(itemId)

            For Each material As Material In _recipe(itemId)
                If ShouldExplode(material.ItemId) Then
                    ' 製品/半製品 → 子品目自身の単位に換算してから展開する
                    Dim childQuantity As Double = ToItemUnitQuantity(material.ItemId, material.Quantity, material.UnitTypeId)
                    Dim factor As Double = childQuantity * invYield

                    Dim childMaterials As Dictionary(Of MaterialKey, Double) = ExplodePerUnit(material.ItemId, path)
                    For Each kv In childMaterials
                        AddTo(materials, kv.Key, kv.Value * factor)
                    Next
                Else
                    ' 末端(材料/資材) → 材料自身の歩留率(下処理ロス)で割り戻す
                    Dim quantity As Double = material.Quantity * invYield / GetLossRate(material.ItemId)
                    AddTo(materials, New MaterialKey(material.ItemId, material.UnitTypeId), quantity)
                End If
            Next

            path.Remove(itemId)
            _memo(itemId) = materials
            Return materials

        End Function

        ' 直下の構成のみ(再帰しない)。レシピ明細の単位で集計
        Private Function DirectComponentsPerUnit(itemId As Integer) As Dictionary(Of MaterialKey, Double)

            Dim materials As New Dictionary(Of MaterialKey, Double)
            If Not _recipe.ContainsKey(itemId) Then Return materials

            Dim invYield As Double = 1.0 / GetLossRate(itemId)

            For Each comp As Material In _recipe(itemId)
                Dim quantity As Double = comp.Quantity * invYield

                ' 展開しない子(半製品)は、その子を作るときのロスなのでここでは効かせない
                If Not ShouldExplode(comp.ItemId) Then
                    quantity /= GetLossRate(comp.ItemId)
                End If

                AddTo(materials, New MaterialKey(comp.ItemId, comp.UnitTypeId), quantity)
            Next

            Return materials

        End Function

        ' RecipeNeedFlag = 1 かつ レシピ明細がある場合は展開する
        Private Function ShouldExplode(itemId As Integer) As Boolean

            Dim result As Boolean = False

            Dim needed As Boolean
            If _recipeNeeded.TryGetValue(itemId, needed) Then
                If needed AndAlso _recipe.ContainsKey(itemId) Then
                    result = True
                End If
            End If

            Return result

        End Function

        Private Function GetItemName(itemId As Integer) As String

            Dim result As String = Nothing

            Dim itemName As String = Nothing
            If _itemName.TryGetValue(itemId, itemName) Then
                result = itemName
            End If

            Return result

        End Function

        Private Function GetItemTypeId(itemId As Integer) As Integer

            Dim result As Integer = 0

            Dim itemTypeId As Integer
            If _itemTypeID.TryGetValue(itemId, itemTypeId) Then
                result = itemTypeId
            End If

            Return result

        End Function

        Private Function GetLossRate(itemId As Integer) As Double

            Dim result As Double = 1.0

            Dim lossRate As Double
            If _lossRate.TryGetValue(itemId, lossRate) AndAlso lossRate > 0 Then
                result = lossRate
            End If

            Return result

        End Function

        Private Function GetItemTypeName(itemTypeId As Integer) As String

            Dim result As String = Nothing

            Dim itemTypeName As String = Nothing
            If _itemTypeName.TryGetValue(itemTypeId, itemTypeName) Then
                result = itemTypeName
            End If

            Return result

        End Function

        Public Function GetUnitTypeName(unitTypeId As Integer) As String

            Dim result As String = Nothing

            Dim unitTypeName As String = Nothing
            If _unitTypeName.TryGetValue(unitTypeId, unitTypeName) Then
                result = unitTypeName
            End If

            Return result

        End Function

        ' 品目自身の単位ID(Items.UnitTypeId)を取得(無ければ0)
        Public Function GetItemUnitTypeId(itemId As Integer) As Integer

            Dim result As Integer = 0

            Dim unitTypeId As Integer
            If _itemUnitTypeId.TryGetValue(itemId, unitTypeId) Then
                result = unitTypeId
            End If

            Return result

        End Function

        ' 「指定単位での数量」を「品目自身の単位での数量」に換算する
        ' 例: ポテトサラダ(単位kg) を 10g 使う → 0.01(kg)
        Private Function ToItemUnitQuantity(itemId As Integer, quantity As Double, unitTypeId As Integer) As Double

            Dim itemUnitTypeId As Integer = GetItemUnitTypeId(itemId)
            If itemUnitTypeId = 0 OrElse itemUnitTypeId = unitTypeId Then Return quantity

            Dim converted As Double
            If TryConvert(quantity, unitTypeId, itemUnitTypeId, converted) Then Return converted

            Return quantity   ' 換算不能(区分違い等)ならそのまま

        End Function

        ' 品目IDから現在庫を取得(無ければ0)
        Public Function GetInventory(itemId As Integer) As Double

            Dim result As Double

            Dim quantity As Double
            If _inventory.TryGetValue(itemId, quantity) Then
                result = quantity
            End If

            Return result

        End Function

        ' 品目IDから在庫単位ID(Inventories.UnitTypeId)を取得(無ければ0)
        Public Function GetInventoryUnitTypeId(itemId As Integer) As Integer

            Dim result As Integer = 0
            Dim unitTypeId As Integer
            If _inventoryUnitTypeId.TryGetValue(itemId, unitTypeId) Then
                result = unitTypeId
            End If
            Return result

        End Function

        ' 品目IDから在庫単位名を取得
        Public Function GetInventoryUnitTypeName(itemId As Integer) As String

            Return GetUnitTypeName(GetInventoryUnitTypeId(itemId))

        End Function

        ' 単位区分を取得(無ければ0)
        Public Function GetUnitCategory(unitTypeId As Integer) As Integer

            Dim result As Integer = 0
            Dim category As Integer
            If _unitCategory.TryGetValue(unitTypeId, category) Then result = category
            Return result

        End Function

        ' 基準単位への換算係数を取得(無ければ1.0)
        Public Function GetBaseFactor(unitTypeId As Integer) As Double

            Dim result As Double = 1.0
            Dim factor As Double
            If _unitFactor.TryGetValue(unitTypeId, factor) AndAlso factor > 0 Then result = factor
            Return result

        End Function

        ' 区分内で最も細かい(係数が最小の)単位ID
        Public Function FinestUnitTypeId(category As Integer) As Integer

            Dim cached As Integer
            If _finestUnit.TryGetValue(category, cached) Then Return cached

            Dim bestUnit As Integer = 0
            Dim bestFactor As Double = Double.MaxValue
            For Each kv In _unitCategory
                If kv.Value <> category Then Continue For
                If Not IsConvertibleUnit(kv.Key) Then Continue For
                Dim factor As Double = GetBaseFactor(kv.Key)
                If factor < bestFactor Then
                    bestFactor = factor
                    bestUnit = kv.Key
                End If
            Next
            Return bestUnit

        End Function

        ' 区分内で最も細かい単位か(g / ml / 個・枚・匹など)
        Public Function IsFinestUnit(unitTypeId As Integer) As Boolean

            If unitTypeId = 0 Then Return True

            ' 換算不可の単位(個・枚・匹など)は常に最小単位扱い
            If Not IsConvertibleUnit(unitTypeId) Then Return True

            Return unitTypeId = FinestUnitTypeId(GetUnitCategory(unitTypeId))

        End Function

        ' 数量列に使う表示書式(最小単位は小数なし、上位単位は小数2桁まで)
        Public Function GetQuantityFormat(unitTypeId As Integer) As String

            If IsFinestUnit(unitTypeId) Then Return "#,##0"
            Return "#,##0.##"

        End Function

        ' 基準単位換算値(baseAmount)を超えない範囲で最大の単位を返す
        ' 例: 重量で baseAmount=1300(g相当) なら kg を返す
        Public Function PickDisplayUnitTypeId(category As Integer, baseAmount As Double) As Integer

            Dim bestUnit As Integer = 0
            Dim bestFactor As Double = -1
            Dim smallestUnit As Integer = 0
            Dim smallestFactor As Double = Double.MaxValue

            For Each kv In _unitCategory
                If kv.Value <> category Then Continue For
                If Not IsConvertibleUnit(kv.Key) Then Continue For

                Dim unitId As Integer = kv.Key
                Dim factor As Double = GetBaseFactor(unitId)

                If factor < smallestFactor Then
                    smallestFactor = factor
                    smallestUnit = unitId
                End If
                If factor <= baseAmount AndAlso factor > bestFactor Then
                    bestFactor = factor
                    bestUnit = unitId
                End If
            Next

            If bestUnit = 0 Then bestUnit = smallestUnit   ' どの単位にも満たない場合は最小単位
            Return bestUnit

        End Function

        ' fromUnit の数量を toUnit に換算する。
        ' 換算できた場合 True(result に換算値)、区分違い等で換算不能なら False。
        Public Function TryConvert(quantity As Double, fromUnitTypeId As Integer, toUnitTypeId As Integer, ByRef result As Double) As Boolean

            result = quantity
            If fromUnitTypeId = toUnitTypeId Then Return True

            Dim fromCat As Integer, toCat As Integer
            Dim fromFactor As Double, toFactor As Double

            If Not _unitCategory.TryGetValue(fromUnitTypeId, fromCat) Then Return False
            If Not _unitCategory.TryGetValue(toUnitTypeId, toCat) Then Return False
            If fromCat <> toCat Then Return False   ' 区分が違えば換算不可(重量↔個数など)
            If Not IsConvertibleUnit(fromUnitTypeId) Then Return False
            If Not IsConvertibleUnit(toUnitTypeId) Then Return False

            If Not _unitFactor.TryGetValue(fromUnitTypeId, fromFactor) Then Return False
            If Not _unitFactor.TryGetValue(toUnitTypeId, toFactor) Then Return False
            If toFactor = 0 Then Return False

            result = quantity * fromFactor / toFactor
            Return True

        End Function

        ' 在庫データ(レコード)が存在するか
        Public Function HasInventory(itemId As Integer) As Boolean

            Return _inventory.ContainsKey(itemId)

        End Function

        ' 単位が換算対象か(区分が正・かつ換算不可フラグが立っていない)
        Public Function IsConvertibleUnit(unitTypeId As Integer) As Boolean

            If GetUnitCategory(unitTypeId) <= 0 Then Return False
            Dim flag As Boolean
            If _unitNonConvertible.TryGetValue(unitTypeId, flag) AndAlso flag Then Return False
            Return True

        End Function

        ' レシピ要否(RecipeNeedFlag=1 → 製造する品目)。計画/発注の切り分けに使用
        Public Function NeedsRecipe(itemId As Integer) As Boolean

            Dim needed As Boolean
            If _recipeNeeded.TryGetValue(itemId, needed) Then Return needed
            Return False

        End Function

        ' 重量管理する品目か(ItemTypes.WeightNeedFlag = 1)
        Public Function NeedsWeight(itemId As Integer) As Boolean

            Dim needed As Boolean
            If _weightNeeded.TryGetValue(itemId, needed) Then Return needed

            Return False

        End Function

        ' 賞味期限管理する品目か(ItemTypes.ExpiryDateNeedFlag = 1)
        Public Function NeedsExpiryDate(itemId As Integer) As Boolean

            Dim needed As Boolean
            If _expiryDateNeeded.TryGetValue(itemId, needed) Then Return needed

            Return False

        End Function

        ' 換算グループ(= 管理区分)
        Public Const UnitCategoryWeight As Integer = 1   ' 重量管理(基準 g)
        Public Const UnitCategoryVolume As Integer = 2   ' 容量管理(基準 ml)
        Public Const UnitCategoryPiece As Integer = 3    ' その他 = 個数管理

        ' 品目の管理区分を単位から導出する(1=重量 / 2=容量 / 3=個数)
        Public Function GetManageType(itemId As Integer) As Integer

            Dim unitId As Integer = GetItemUnitTypeId(itemId)
            If unitId = 0 Then Return UnitCategoryPiece

            ' 換算できない単位(個・枚・匹など)は個数管理として扱う
            If Not IsConvertibleUnit(unitId) Then Return UnitCategoryPiece

            Dim category As Integer = GetUnitCategory(unitId)
            If category <> UnitCategoryWeight AndAlso category <> UnitCategoryVolume Then
                Return UnitCategoryPiece
            End If

            Return category

        End Function

        ' 個数管理かどうか
        Public Function IsPieceManaged(itemId As Integer) As Boolean

            Return GetManageType(itemId) = UnitCategoryPiece

        End Function

        ' 基準単位ID(重量=g / 容量=ml / 個数=品目の単位そのもの)
        Public Function GetBaseUnitTypeId(itemId As Integer) As Integer

            Dim category As Integer = GetManageType(itemId)

            If category = UnitCategoryPiece Then
                ' 個数管理は品目の単位をそのまま使う(個・枚・パックなど)
                Return GetItemUnitTypeId(itemId)
            End If

            Return FinestUnitTypeId(category)

        End Function

        ' 基準単位名(重量=g / 容量=ml / 個数=品目の単位そのもの)
        Public Function GetBaseUnitTypeName(itemId As Integer) As String

            Dim unitId As Integer = GetBaseUnitTypeId(itemId)
            If unitId = 0 Then Return "個"
            Return GetUnitTypeName(unitId)

        End Function

        Private _inventoryWeight As New Dictionary(Of Integer, Double)

        ' 在庫を基準単位(g / ml / 個)で集計する
        Private Sub LoadInventoryWeights()

            _inventoryWeight.Clear()

            Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
                cn.Open()
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String = String.Empty
                    SQLString = "select"
                    SQLString &= " t.ItemId"
                    SQLString &= ",sum(t.Weight) as TotalWeight"
                    SQLString &= " from ("
                    SQLString &= " select"
                    SQLString &= " ItemId"
                    SQLString &= ",Weight"
                    SQLString &= " from Inventories"
                    SQLString &= " where Weight > 0"
                    SQLString &= " union all"
                    ' 在庫不足のまま製造完了した分をマイナス在庫として扱う
                    SQLString &= " select"
                    SQLString &= " ItemId"
                    SQLString &= ",-Quantity as Weight"
                    SQLString &= " from InstructionConsumptions"
                    SQLString &= " where InventoryId is null"
                    SQLString &= " ) as t"
                    SQLString &= " group by"
                    SQLString &= " t.ItemId"
                    cmd.CommandText = SQLString
                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        While dr.Read()
                            _inventoryWeight(CInt(dr("ItemId"))) = CDbl(Nz(dr("TotalWeight"), 0))
                        End While
                    End Using
                End Using
            End Using

        End Sub

        ' 基準単位での在庫数
        Public Function GetInventoryWeight(itemId As Integer) As Double

            Dim value As Double
            If _inventoryWeight.TryGetValue(itemId, value) Then Return value
            Return 0

        End Function

        ' 数量を品目の基準単位へ換算する(換算できなければ False)
        Public Function TryToBaseQuantity(itemId As Integer, quantity As Double, unitTypeId As Integer, ByRef result As Double) As Boolean

            result = 0

            Dim manageType As Integer = GetManageType(itemId)

            If manageType = UnitCategoryPiece Then
                ' 個数管理: 換算できないので単位が一致しているときだけ通す
                If unitTypeId = 0 OrElse unitTypeId = GetItemUnitTypeId(itemId) Then
                    result = quantity
                    Return True
                End If
                Return False
            End If

            If unitTypeId = 0 Then
                result = quantity
                Return True
            End If
            If Not IsConvertibleUnit(unitTypeId) Then Return False
            If GetUnitCategory(unitTypeId) <> manageType Then Return False

            Dim factor As Double = GetBaseFactor(unitTypeId)
            If factor <= 0 Then factor = 1
            result = quantity * factor
            Return True

        End Function

        ' 在庫だけ読み直す(レシピのメモ化は維持する)
        Public Sub ReloadInventories()
            LoadInventories()
            LoadInventoryWeights()
        End Sub

        ' 製造計画を「供給」として扱い、レベル別に正味所要量を求める
        ' useInventory : True=在庫を差し引く(当日) / False=差し引かない(予定)
        Public Function CalcNetRequirements(plans As IEnumerable(Of Product),
                                            Optional useInventory As Boolean = True) As List(Of Requirement)

            ' 製造計画を品目自身の単位に揃えて集計する
            Dim planQuantity As New Dictionary(Of Integer, Double)
            If plans IsNot Nothing Then
                For Each plan In plans
                    If plan Is Nothing OrElse plan.Quantity = 0 Then Continue For
                    Dim quantity As Double = ToItemUnitQuantity(plan.ItemId, plan.Quantity, plan.UnitTypeId)
                    Dim current As Double
                    planQuantity.TryGetValue(plan.ItemId, current)
                    planQuantity(plan.ItemId) = current + quantity
                Next
            End If

            ' 従属所要(親から降りてくる量)。品目自身の単位で持つ
            Dim gross As New Dictionary(Of Integer, Double)

            ' 対象品目を低位コードの浅い順に並べる(製品 → 半製品 → 材料)
            Dim targets As New HashSet(Of Integer)(planQuantity.Keys)
            For Each itemId As Integer In _levelCode.Keys
                targets.Add(itemId)
            Next
            Dim ordered = targets.OrderBy(Function(x) GetLevelCode(x)).ThenBy(Function(x) x).ToList()

            Dim result As New List(Of Requirement)

            For Each itemId As Integer In ordered
                Dim grossQuantity As Double
                gross.TryGetValue(itemId, grossQuantity)

                Dim planned As Double
                planQuantity.TryGetValue(itemId, planned)

                If grossQuantity = 0 AndAlso planned = 0 Then Continue For

                Dim inventory As Double = If(useInventory, GetInventoryInItemUnit(itemId), 0)

                ' 在庫で足りない分。マイナスにはしない
                Dim netNeed As Double = grossQuantity - inventory
                If netNeed < 0 Then netNeed = 0

                ' ★二重計上の解消: 登録済みの計画は「供給」として吸収する
                Dim production As Double = Math.Max(planned, netNeed)

                ' 展開できる品目は必ず末端まで展開する
                Dim exploded As Boolean = ShouldExplode(itemId) AndAlso production > 0

                Dim req As New Requirement
                req.ItemId = itemId
                req.ItemName = GetItemName(itemId)
                req.ItemTypeId = GetItemTypeId(itemId)
                req.ItemTypeName = GetItemTypeName(req.ItemTypeId)
                req.UnitTypeId = GetItemUnitTypeId(itemId)
                req.UnitTypeName = GetUnitTypeName(req.UnitTypeId)
                req.GrossQuantity = grossQuantity
                req.PlanQuantity = planned
                req.InventoryQuantity = inventory
                req.ProductionQuantity = production
                req.NetQuantity = netNeed
                req.Exploded = exploded
                result.Add(req)

                ' 実際に作る量だけを1段下に展開する
                If exploded Then
                    Dim invYield As Double = 1.0 / GetLossRate(itemId)

                    For Each material As Material In _recipe(itemId)
                        Dim childQuantity As Double =
                            ToItemUnitQuantity(material.ItemId, material.Quantity, material.UnitTypeId) *
                            production * invYield

                        ' 末端材料は自分自身の歩留率(下処理ロス)で割り戻す
                        If Not ShouldExplode(material.ItemId) Then
                            childQuantity /= GetLossRate(material.ItemId)
                        End If

                        Dim current As Double
                        gross.TryGetValue(material.ItemId, current)
                        gross(material.ItemId) = current + childQuantity
                    Next
                End If
            Next

            Return result

        End Function

    End Class

#End Region

#Region "原価計算"

    ' ロット1件の消費内訳
    Public Class CostLotUsage
        Public Property InventoryId As String
        Public Property ExpiryDate As Date?
        Public Property UsedQuantity As Double      ' 材料の単位での使用量
        Public Property UnitPrice As Double         ' 材料の単位あたり単価
        Public Property Cost As Double
        Public Property IsShortage As Boolean       ' 在庫不足分か
    End Class

    ' 材料1件の原価
    Public Class CostLine
        Public Property ItemId As Integer
        Public Property ItemName As String
        Public Property ItemTypeName As String
        Public Property RequiredQuantity As Double  ' 必要量
        Public Property UnitTypeId As Integer
        Public Property UnitTypeName As String
        Public Property UsedQuantity As Double      ' 在庫から充当できた量
        Public Property ShortageQuantity As Double  ' 在庫不足分
        Public Property Cost As Double
        Public Property Usages As New List(Of CostLotUsage)
    End Class

    ' 製品の原価
    Public Class CostResult
        Public Property ItemId As Integer
        Public Property Quantity As Double
        Public Property UnitTypeId As Integer
        Public Property TotalCost As Double
        Public Property Lines As New List(Of CostLine)

        ' 1個あたり原価
        Public ReadOnly Property UnitCost As Double
            Get
                If Quantity = 0 Then Return 0
                Return TotalCost / Quantity
            End Get
        End Property

        Public ReadOnly Property HasShortage As Boolean
            Get
                Return Lines.Any(Function(l) l.ShortageQuantity > 0)
            End Get
        End Property
    End Class

    Public Class CostCalculator

        Private ReadOnly _calc As MaterialCalculator
        Private ReadOnly _lots As New Dictionary(Of Integer, List(Of CostLot))

        ' 計算用の在庫ロット(すべて基準単位 g / ml / 個 で保持する)
        Private Class CostLot
            Public InventoryId As String
            Public ExpiryDate As Date?
            Public ArrivalDate As Date?
            Public BaseQuantity As Double    ' 在庫量(基準単位)
            Public UnitPrice As Double       ' 基準単位あたり単価(円/g など)
            Public Remaining As Double       ' 残量(基準単位)
        End Class

        Public Sub New(calc As MaterialCalculator)
            _calc = calc
        End Sub

        ' 製品1個あたりの原価
        Public Function CalcUnitCost(itemId As Integer, unitTypeId As Integer) As CostResult

            Return CalcCost(itemId, 1, unitTypeId)

        End Function

        ' 製品を quantity 作るときの原価
        Public Function CalcCost(itemId As Integer, quantity As Double, unitTypeId As Integer) As CostResult

            Dim result As New CostResult
            result.ItemId = itemId
            result.Quantity = quantity
            result.UnitTypeId = unitTypeId

            ' レシピを展開して所要材料を得る
            Dim materials As List(Of MaterialCalculator.Material) =
            _calc.CalcRequiredMaterials(itemId, quantity, unitTypeId)

            For Each material As MaterialCalculator.Material In materials
                Dim line As CostLine = ConsumeFifo(material)
                result.Lines.Add(line)
                result.TotalCost += line.Cost
            Next

            Return result

        End Function

        ' 賞味期限の古いロットから順に消費して原価を積み上げる
        Private Function ConsumeFifo(material As MaterialCalculator.Material) As CostLine

            Dim line As New CostLine
            line.ItemId = material.ItemId
            line.ItemName = material.ItemName
            line.ItemTypeName = material.ItemTypeName

            ' 必要量を在庫の基準単位に揃える
            Dim baseUnitId As Integer = _calc.GetBaseUnitTypeId(material.ItemId)
            Dim remain As Double = material.Quantity
            Dim converted As Double
            If _calc.TryConvert(material.Quantity, material.UnitTypeId, baseUnitId, converted) Then
                remain = converted
            End If

            line.RequiredQuantity = remain
            line.UnitTypeId = baseUnitId
            line.UnitTypeName = _calc.GetUnitTypeName(baseUnitId)

            For Each lot As CostLot In GetLots(material.ItemId)
                If remain <= 0 Then Exit For
                If lot.Remaining <= 0 Then Continue For

                ' 単位はロットも必要量も基準単位なので換算不要
                Dim take As Double = Math.Min(remain, lot.Remaining)
                remain -= take
                lot.Remaining -= take

                Dim cost As Double = take * lot.UnitPrice
                line.UsedQuantity += take
                line.Cost += cost

                line.Usages.Add(New CostLotUsage With {
            .InventoryId = lot.InventoryId,
            .ExpiryDate = lot.ExpiryDate,
            .UsedQuantity = take,
            .UnitPrice = lot.UnitPrice,
            .Cost = cost,
            .IsShortage = False
        })
            Next

            ' 在庫で賄いきれなかった分は代替単価で評価
            If remain > 0 Then
                Dim unitPrice As Double = GetFallbackUnitPrice(material.ItemId)
                line.ShortageQuantity = remain
                line.Cost += remain * unitPrice

                line.Usages.Add(New CostLotUsage With {
            .InventoryId = Nothing,
            .ExpiryDate = Nothing,
            .UsedQuantity = remain,
            .UnitPrice = unitPrice,
            .Cost = remain * unitPrice,
            .IsShortage = True
        })
            End If

            Return line

        End Function

        ' 在庫不足分に適用する単価(直近入荷ロットの単価)
        Private Function GetFallbackUnitPrice(itemId As Integer) As Double

            Dim latest As CostLot = GetLots(itemId).
                Where(Function(l) l.UnitPrice > 0).
                OrderByDescending(Function(l) If(l.ArrivalDate, Date.MinValue)).
                FirstOrDefault()

            If latest Is Nothing Then Return 0

            Return latest.UnitPrice

        End Function

        ' 在庫ロットを賞味期限の古い順に取得(初回のみDB)
        Private Function GetLots(itemId As Integer) As List(Of CostLot)

            Dim cached As List(Of CostLot) = Nothing
            If _lots.TryGetValue(itemId, cached) Then Return cached

            Dim list As New List(Of CostLot)

            Dim orderBy As String = "case when ExpiryDate is null then 1 else 0 end, ExpiryDate, ArrivalDate"
            Dim where As String = "ItemId = " & itemId & " and Weight > 0"

            For Each inventory As Inventory In Inventories.GetAll(orderBy, where)
                Dim lot As New CostLot
                lot.InventoryId = inventory.InventoryId
                lot.ExpiryDate = If(IsDate(inventory.ExpiryDate), CDate(inventory.ExpiryDate), Nothing)
                lot.ArrivalDate = If(IsDate(inventory.ArrivalDate), CDate(inventory.ArrivalDate), Nothing)

                ' Weight と PricePerGram は入荷検品で基準単位に揃えて保存されている
                lot.BaseQuantity = Nz(inventory.Weight, 0)
                lot.UnitPrice = Nz(inventory.PricePerGram, 0)
                lot.Remaining = lot.BaseQuantity

                If lot.BaseQuantity > 0 Then list.Add(lot)
            Next

            _lots(itemId) = list
            Return list

        End Function

        ' 計算をやり直す(ロットの残量をリセット)
        Public Sub Reset()

            _lots.Clear()

        End Sub

        ' ロットの残量だけを元に戻す(DBから読み込んだロットは再利用する)
        ' 品目ごとに独立した「1個あたり原価」を求めるときは、行ごとにこれを呼ぶ
        Public Sub ResetRemaining()

            For Each lots As List(Of CostLot) In _lots.Values
                For Each lot As CostLot In lots
                    lot.Remaining = lot.BaseQuantity
                Next
            Next

        End Sub

    End Class

#End Region

#Region "共通関数"

    Public Function SelectSaveFile(title As String,
                                   Optional initialPath As String = "", Optional filter As String = "すべてのファイル (*.*)|*.*") As String

        Dim result As String = String.Empty

        Using dlg As New SaveFileDialog()
            dlg.Title = title
            If Not String.IsNullOrEmpty(initialPath) Then
                dlg.InitialDirectory = System.IO.Path.GetDirectoryName(initialPath)
                dlg.FileName = System.IO.Path.GetFileName(initialPath)
            End If
            dlg.Filter = filter
            dlg.RestoreDirectory = True
            If dlg.ShowDialog() = DialogResult.OK Then
                result = dlg.FileName
            End If
        End Using

        Return result

    End Function

    Public Function SelectFolder(Optional title As String = "", Optional initialPath As String = "") As String

        Dim result As String = String.Empty

        Using dlg As New FolderBrowserDialog()
            If Not String.IsNullOrEmpty(title) Then
                dlg.Description = title
            Else
                dlg.Description = "フォルダを選択してください。"
            End If
            If Not String.IsNullOrEmpty(initialPath) Then
                dlg.SelectedPath = initialPath
            End If
            If dlg.ShowDialog() = DialogResult.OK Then
                result = dlg.SelectedPath & "\"
            End If
        End Using

        Return result

    End Function

    Public Function SelectOpenFile(title As String,
                                   Optional initialPath As String = "", Optional filter As String = "すべてのファイル (*.*)|*.*") As String

        Dim result As String = String.Empty

        Using dlg As New OpenFileDialog()
            dlg.Title = title
            If Not String.IsNullOrEmpty(initialPath) Then
                dlg.InitialDirectory = System.IO.Path.GetDirectoryName(initialPath)
                dlg.FileName = System.IO.Path.GetFileName(initialPath)
            End If
            dlg.Filter = filter
            dlg.RestoreDirectory = True
            dlg.CheckFileExists = False
            If dlg.ShowDialog() = DialogResult.OK Then
                result = dlg.FileName
            End If
        End Using

        Return result

    End Function

    Public Function SelectOpenFiles(title As String,
                                    Optional initialPath As String = "", Optional filter As String = "すべてのファイル (*.*)|*.*") As String()

        Dim result As String() = Nothing

        Using dlg As New OpenFileDialog()
            dlg.Title = title
            dlg.InitialDirectory = initialPath
            dlg.FileName = String.Empty
            dlg.Filter = filter
            dlg.Multiselect = True
            dlg.RestoreDirectory = True
            If dlg.ShowDialog() = DialogResult.OK Then
                result = dlg.FileNames
                System.Threading.Thread.Sleep(3000)
            Else
                result = Nothing
            End If
        End Using

        Return result

    End Function

    Public Function GetFiles(rootPath As String, pattern As String, mostDeep As Boolean) As String()

        Dim StringCollection As New Specialized.StringCollection()

        ' このディレクトリ内のすべてのファイルを検索する
        For Each FilePath As String In System.IO.Directory.GetFiles(rootPath, pattern)
            StringCollection.Add(FilePath)
        Next FilePath

        If mostDeep = True Then
            ' ディレクトリ内のすべてのサブディレクトリを検索する (再帰)
            For Each DirPath As String In System.IO.Directory.GetDirectories(rootPath)
                Dim FilePaths As String() = GetFiles(DirPath, pattern, True)

                ' 条件に合致したファイルがあった場合は、ArrayList に加える
                If Not FilePaths Is Nothing Then
                    StringCollection.AddRange(FilePaths)
                End If
            Next DirPath
        End If

        ' StringCollection を 1 次元の String 配列にして返す
        Dim Returns As String() = New String(StringCollection.Count - 1) {}
        StringCollection.CopyTo(Returns, 0)

        Return Returns

    End Function

    Public Function EditDirPath(pathString As String) As String

        Dim result As String = pathString

        If Not pathString.EndsWith("\") Then
            result &= "\"
        End If

        Return result

    End Function

    Public Function InStrWithWildCard(String1 As String, String2 As String) As Boolean

        Dim regptn As String = Regex.Escape(String2)
        regptn = regptn.Replace("\*", ".*?")
        regptn = regptn.Replace("\?", ".")

        Dim reg As New Regex(regptn)
        Return reg.IsMatch(String1)

    End Function

    ' 左からバイト数で文字列を取得
    Public Function LeftB(value As String, length As Integer) As String
        Dim enc As System.Text.Encoding
        enc = System.Text.Encoding.GetEncoding("Shift_JIS")
        Dim bytes As Byte()
        bytes = enc.GetBytes(value.PadRight(length))
        Return (enc.GetString(bytes, 0, length))
    End Function

    Public Function ToNarrow(s As String) As String

        Dim result As String

        Dim regex As Regex = New Regex("[０-９Ａ-Ｚａ-ｚ：，－　]+")
        result = regex.Replace(s, AddressOf Replacer)

        Return result

    End Function

    Private Function Replacer(match As Match) As String

        Return Strings.StrConv(match.Value, VbStrConv.Narrow, 0)

    End Function

    ' valueがNullやNothing、""だった場合にはreplaceValueで指定した値を返す
    ' 上記以外の場合はそのまま返す
    Public Function Nz(value As Object, replaceValue As Object) As Object

        Dim result As Object

        If IsNothing(value) OrElse IsDBNull(value) OrElse String.IsNullOrEmpty(value) Then
            result = replaceValue
        Else
            result = value
        End If

        Return result

    End Function

    ' 1行分のCSV文字列を作る
    Public Function CsvLine(fields As List(Of String)) As String

        Dim list As New List(Of String)
        For Each field As String In fields
            list.Add(CsvField(field))
        Next

        Return String.Join(",", list.ToArray())

    End Function

    ' CSVの1項目をエスケープする
    Public Function CsvField(value As String) As String

        If value Is Nothing Then Return String.Empty

        If value.Contains(",") OrElse value.Contains("""") OrElse value.Contains(vbCr) OrElse value.Contains(vbLf) Then
            Return """" & value.Replace("""", """""") & """"
        End If

        Return value

    End Function

#End Region

End Module

#Region "設定ファイル"

Public Class IniFile

#Region "定義"

    <DllImport("KERNEL32.DLL")>
    Public Shared Function WritePrivateProfileString(
    lpAppName As String,
    lpKeyName As String,
    lpString As String,
    lpFileName As String) As Integer
    End Function

    <DllImport("KERNEL32.DLL", CharSet:=CharSet.Auto)>
    Public Shared Function GetPrivateProfileString(
    lpAppName As String,
    lpKeyName As String, lpDefault As String,
    lpReturnedString As System.Text.StringBuilder, nSize As Integer,
    lpFileName As String) As Integer
    End Function

    Public Property FileName As String

    Public Property Debug As Boolean
    Public Property LogPath As String
    Public Property EnableLogin As Boolean
    Public Property VisivleItemType As String
    Public Property PaintItemTypeColor As String

    Public Property ConnectionString As String
    Public Property PcrsOutputFileName As String

#End Region

#Region "コンストラクタ"

    Public Sub New(fileName As String)

        _FileName = fileName

        _Debug = IIf(GetSettingIni(My.Application.Info.DirectoryPath & "\" & _FileName, "System", "Debug") = 1, True, False)
        _LogPath = GetSettingIni(My.Application.Info.DirectoryPath & "\" & _FileName, "System", "LogPath")
        _EnableLogin = IIf(GetSettingIni(My.Application.Info.DirectoryPath & "\" & _FileName, "System", "EnableLogin") = 1, True, False)
        _VisivleItemType = GetSettingIni(My.Application.Info.DirectoryPath & "\" & _FileName, "System", "VisivleItemType")
        _PaintItemTypeColor = GetSettingIni(My.Application.Info.DirectoryPath & "\" & _FileName, "System", "PaintItemTypeColor")

        _ConnectionString = GetSettingIni(My.Application.Info.DirectoryPath & "\" & _FileName, "Database", "ConnectionString")

    End Sub

#End Region

#Region "メソッド"

    Private Function GetSettingIni(fileName As String, section As String, key As String, Optional [default] As Object = "") As String

        If IsError([default]) Then [default] = ""

        ' 文字列を読み出す
        Dim sb As System.Text.StringBuilder = New System.Text.StringBuilder(1023)
        GetPrivateProfileString(section, key, [default], sb, sb.Capacity, fileName)

        GetSettingIni = sb.ToString()

    End Function

    Private Sub PutSettingIni(fileName As String, section As String, key As String, value As String)

        WritePrivateProfileString(section, key, value, fileName)

    End Sub

#End Region

End Class

#End Region

#Region "品目区分"

Public Class ItemType
    Public Property ItemTypeId As String
    Public Property ItemTypeName As String
    Public Property OrderNo As String

    Public Property ProductFlag As String
    Public Property RecipeNeedFlag As String
    Public Property LossRateNeedFlag As String
    Public Property PieceNeedFlag As String
    Public Property SellingPriceNeedFlag As String
    Public Property MainSupplierNeedFlag As String
    Public Property ExpiryDateNeedFlag As String
    Public Property WeightNeedFlag As String

    Public Property FormBackColor As Color
    Public Property ButtonForeColor As Color
    Public Property ButtonBackColor As Color
End Class

Public Class ItemTypes

    Public Shared Function GetAll(Optional where As String = Nothing, Optional orderBy As String = Nothing) As List(Of ItemType)

        Dim result As New List(Of ItemType)

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Try
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String
                    SQLString = "select"
                    SQLString &= " ItemTypes.*"
                    SQLString &= " from ItemTypes"
                    SQLString &= " where ItemTypeId is not null"
                    If Not String.IsNullOrEmpty(where) Then
                        SQLString &= " and " & where
                    End If
                    If Not String.IsNullOrEmpty(orderBy) Then
                        SQLString &= " order by " & orderBy
                    Else
                        SQLString &= " order by OrderNo"
                    End If
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        While dr.Read
                            Dim itemType As ItemType = Mapping(dr)
                            result.Add(itemType)
                        End While
                    End Using
                End Using
            Catch ex As Exception
                SysMsgBox(ex.Message, "", Reflection.MethodBase.GetCurrentMethod.Name)
                Throw
            End Try
        End Using

        Return result

    End Function

    Public Shared Function GetById(id As Integer) As ItemType

        Dim result As New ItemType

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Try
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String
                    SQLString = "select"
                    SQLString &= " ItemTypes.*"
                    SQLString &= " from ItemTypes"
                    SQLString &= " where ItemTypeId = @ItemTypeId"
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    cmd.Parameters.AddWithValue("@ItemTypeId", NpgsqlDbType.Integer, id)
                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        If dr.HasRows Then
                            If dr.Read() Then result = Mapping(dr)
                        Else
                            result = Nothing
                        End If
                    End Using
                End Using
            Catch ex As Exception
                SysMsgBox(ex.Message, "", Reflection.MethodBase.GetCurrentMethod.Name)
                Throw
            End Try
        End Using

        Return result

    End Function

    Private Shared Function Mapping(dr As NpgsqlDataReader) As ItemType

        Dim result As New ItemType

        result.ItemTypeId = dr("ItemTypeId").ToString
        result.ItemTypeName = dr("ItemTypeName").ToString
        result.OrderNo = dr("OrderNo").ToString

        result.ProductFlag = Nz(dr("ProductFlag"), 0).ToString
        result.RecipeNeedFlag = Nz(dr("RecipeNeedFlag"), 0).ToString
        result.LossRateNeedFlag = Nz(dr("LossRateNeedFlag"), 0).ToString
        result.PieceNeedFlag = Nz(dr("PieceNeedFlag"), 0).ToString
        result.SellingPriceNeedFlag = Nz(dr("SellingPriceNeedFlag"), 0).ToString
        result.MainSupplierNeedFlag = Nz(dr("MainSupplierNeedFlag"), 0).ToString
        result.ExpiryDateNeedFlag = Nz(dr("ExpiryDateNeedFlag"), 0).ToString
        result.WeightNeedFlag = Nz(dr("WeightNeedFlag"), 0).ToString

        If Not String.IsNullOrEmpty(dr("FormBackColor").ToString) Then
            result.FormBackColor = ColorTranslator.FromHtml(dr("FormBackColor").ToString)
        Else
            result.FormBackColor = SystemColors.Control
        End If
        If Not String.IsNullOrEmpty(dr("ButtonForeColor").ToString) Then
            result.ButtonForeColor = ColorTranslator.FromHtml(dr("ButtonForeColor").ToString)
        Else
            result.ButtonForeColor = SystemColors.ControlText
        End If
        If Not String.IsNullOrEmpty(dr("ButtonBackColor").ToString) Then
            result.ButtonBackColor = ColorTranslator.FromHtml(dr("ButtonBackColor").ToString)
        Else
            result.ButtonBackColor = SystemColors.Control
        End If

        Return result

    End Function

    Public Shared Sub BindComboBox(targetComboBox As Object, Optional addItemEmpty As Boolean = False, Optional where As String = "")

        Dim dtItemTypes As New DataTable()
        dtItemTypes.Columns.Add("ItemTypeId", GetType(String))
        dtItemTypes.Columns.Add("ItemTypeName", GetType(String))

        If addItemEmpty Then dtItemTypes.Rows.Add(DBNull.Value, "")
        Dim itemTypeList As List(Of ItemType) = ItemTypes.GetAll(where)
        For Each ItemType As ItemType In itemTypeList
            dtItemTypes.Rows.Add(ItemType.ItemTypeId, ItemType.ItemTypeName)
        Next
        dtItemTypes.AcceptChanges()

        With targetComboBox
            .DisplayMember = "ItemTypeName"
            .ValueMember = "ItemTypeId"
            .DataSource = dtItemTypes
        End With

    End Sub

End Class

#End Region

#Region "品目"

Public Class Item
    Public Property ItemId As String
    Public Property ItemCode As String
    Public Property JanCode As String
    Public Property ItemName As String
    Public Property ItemTypeId As String
    Public Property UnitType As New UnitType
    Public Property LossRate As String
    Public Property Shipment As String
    Public Property PieceUnitType As New UnitType
    Public Property PieceWeight As String
    Public Property CostPrice As String
    Public Property OriginalSellingPrice As String
    Public Property InstoreSellingPrice As String
    Public Property MainSupplier As New Supplier
    Public Property ExpiryDateNeedFlag As String
    Public Property MeasureType As String
    Public Property UnusedFlag As String
End Class

Public Class Items

    Public Shared Function GetAll(Optional where As String = Nothing, Optional orderBy As String = Nothing) As List(Of Item)

        Dim result As New List(Of Item)

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Try
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String
                    SQLString = "select"
                    SQLString &= " Items.*"
                    SQLString &= " from Items"
                    SQLString &= " where ItemId is not null"
                    If Not String.IsNullOrEmpty(where) Then
                        SQLString &= " and " & where
                    End If
                    If Not String.IsNullOrEmpty(orderBy) Then
                        SQLString &= " order by " & orderBy
                    Else
                        SQLString &= " order by ItemCode"
                    End If
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        While dr.Read
                            Dim item As Item = Mapping(dr)
                            result.Add(item)
                        End While
                    End Using
                End Using
            Catch ex As Exception
                SysMsgBox(ex.Message, "", Reflection.MethodBase.GetCurrentMethod.Name)
                Throw
            End Try
        End Using

        Return result

    End Function

    Public Shared Function GetById(id As Integer) As Item

        Dim result As Item = Nothing

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Try
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String
                    SQLString = "select"
                    SQLString &= " Items.*"
                    SQLString &= ",ItemTypes.RecipeNeedFlag"
                    SQLString &= " from Items"
                    SQLString &= " inner join ItemTypes on Items.ItemTypeId = ItemTypes.ItemTypeId"
                    SQLString &= " where ItemId = @ItemId"
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    cmd.Parameters.AddWithValue("@ItemId", NpgsqlDbType.Integer, id)
                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        If dr.Read() Then result = Mapping(dr)
                    End Using
                End Using
            Catch ex As Exception
                SysMsgBox(ex.Message, "", Reflection.MethodBase.GetCurrentMethod.Name)
                Throw
            End Try
        End Using

        Return result

    End Function

    Private Shared Function Mapping(dr As NpgsqlDataReader) As Item

        Dim result As New Item

        result.ItemId = dr("ItemId").ToString
        result.ItemCode = dr("ItemCode").ToString
        result.JanCode = dr("JANCode").ToString
        result.ItemName = dr("ItemName").ToString
        result.ItemTypeId = dr("ItemTypeId").ToString
        result.UnitType = UnitTypes.GetById(dr("UnitTypeId").ToString)
        result.LossRate = dr("LossRate").ToString
        result.Shipment = dr("Shipment").ToString
        If Not IsNothing(Nz(dr("PieceUnitTypeId"), Nothing)) Then
            result.PieceUnitType = UnitTypes.GetById(dr("PieceUnitTypeId").ToString)
        End If
        result.PieceWeight = Nz(dr("PieceWeight"), Nothing)
        result.CostPrice = dr("CostPrice").ToString
        result.OriginalSellingPrice = dr("OriginalSellingPrice").ToString
        result.InstoreSellingPrice = dr("InstoreSellingPrice").ToString
        If Not IsNothing(Nz(dr("MainSupplierId"), Nothing)) Then
            result.MainSupplier = Suppliers.GetById(dr("MainSupplierId").ToString)
        End If
        result.ExpiryDateNeedFlag = Nz(dr("ExpiryDateNeedFlag"), 0).ToString
        result.MeasureType = Nz(dr("MeasureType"), 0).ToString
        result.UnusedFlag = dr("UnusedFlag").ToString

        Return result

    End Function

    Public Shared Function Save(item As Item) As Item

        Dim nowDate As Date = Now

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Using trans As NpgsqlTransaction = cn.BeginTransaction
                Try
                    Using cmd As NpgsqlCommand = cn.CreateCommand()
                        cmd.Transaction = trans

                        Dim SQLString As String
                        If String.IsNullOrEmpty(item.ItemId) Then
                            SQLString = "insert into Items ("
                            SQLString &= " ItemCode"
                            SQLString &= ",JANCode"
                            SQLString &= ",ItemName"
                            SQLString &= ",ItemTypeId"
                            SQLString &= ",UnitTypeId"
                            SQLString &= ",LossRate"
                            SQLString &= ",Shipment"
                            SQLString &= ",PieceUnitTypeId"
                            SQLString &= ",PieceWeight"
                            SQLString &= ",CostPrice"
                            SQLString &= ",OriginalSellingPrice"
                            SQLString &= ",InstoreSellingPrice"
                            SQLString &= ",MainSupplierId"
                            SQLString &= ",ExpiryDateNeedFlag"
                            SQLString &= ",MeasureType"
                            SQLString &= ",UnUsedFlag"
                            SQLString &= ",RegistDate"
                            SQLString &= ",ModifyDate"
                            SQLString &= " ) values ("
                            SQLString &= " @ItemCode"
                            SQLString &= ",@JANCode"
                            SQLString &= ",@ItemName"
                            SQLString &= ",@ItemTypeId"
                            SQLString &= ",@UnitTypeId"
                            SQLString &= ",@LossRate"
                            SQLString &= ",@Shipment"
                            SQLString &= ",@PieceUnitTypeId"
                            SQLString &= ",@PieceWeight"
                            SQLString &= ",@CostPrice"
                            SQLString &= ",@OriginalSellingPrice"
                            SQLString &= ",@InstoreSellingPrice"
                            SQLString &= ",@MainSupplierId"
                            SQLString &= ",@ExpiryDateNeedFlag"
                            SQLString &= ",@MeasureType"
                            SQLString &= ",@UnUsedFlag"
                            SQLString &= ",@RegistDate"
                            SQLString &= ",@ModifyDate"
                            SQLString &= ") returning ItemId"
                            cmd.CommandText = SQLString
                            cmd.Parameters.Clear()
                            cmd.Parameters.AddWithValue("@ItemCode", NpgsqlDbType.Varchar, item.ItemCode)
                            cmd.Parameters.AddWithValue("@JANCode", NpgsqlDbType.Varchar, item.JanCode)
                            cmd.Parameters.AddWithValue("@ItemName", NpgsqlDbType.Varchar, item.ItemName)
                            cmd.Parameters.AddWithValue("@ItemTypeId", NpgsqlDbType.Integer, CInt(item.ItemTypeId))
                            cmd.Parameters.AddWithValue("@UnitTypeId", NpgsqlDbType.Integer, CInt(item.UnitType.UnitTypeId))
                            If Not IsNothing(Nz(item.LossRate, Nothing)) Then
                                cmd.Parameters.AddWithValue("@LossRate", NpgsqlDbType.Double, CDbl(item.LossRate))
                            Else
                                cmd.Parameters.AddWithValue("@LossRate", NpgsqlDbType.Double, DBNull.Value)
                            End If
                            If Not IsNothing(Nz(item.Shipment, Nothing)) Then
                                cmd.Parameters.AddWithValue("@Shipment", NpgsqlDbType.Double, CDbl(item.Shipment))
                            Else
                                cmd.Parameters.AddWithValue("@Shipment", NpgsqlDbType.Double, DBNull.Value)
                            End If
                            If Not IsNothing(Nz(item.PieceUnitType.UnitTypeId, Nothing)) Then
                                cmd.Parameters.AddWithValue("@PieceUnitTypeId", NpgsqlDbType.Integer, CInt(item.PieceUnitType.UnitTypeId))
                            Else
                                cmd.Parameters.AddWithValue("@PieceUnitTypeId", NpgsqlDbType.Integer, DBNull.Value)
                            End If
                            If Not IsNothing(Nz(item.PieceWeight, Nothing)) Then
                                cmd.Parameters.AddWithValue("@PieceWeight", NpgsqlDbType.Double, CDbl(item.PieceWeight))
                            Else
                                cmd.Parameters.AddWithValue("@PieceWeight", NpgsqlDbType.Double, DBNull.Value)
                            End If
                            cmd.Parameters.AddWithValue("@CostPrice", NpgsqlDbType.Double, CDbl(Nz(item.CostPrice, 0)))
                            cmd.Parameters.AddWithValue("@OriginalSellingPrice", NpgsqlDbType.Integer, CInt(Nz(item.OriginalSellingPrice, 0)))
                            cmd.Parameters.AddWithValue("@InstoreSellingPrice", NpgsqlDbType.Integer, CInt(Nz(item.InstoreSellingPrice, 0)))
                            If Not IsNothing(Nz(item.MainSupplier.SupplierId, Nothing)) Then
                                cmd.Parameters.AddWithValue("@MainSupplierId", NpgsqlDbType.Integer, CInt(item.MainSupplier.SupplierId))
                            Else
                                cmd.Parameters.AddWithValue("@MainSupplierId", NpgsqlDbType.Integer, DBNull.Value)
                            End If
                            cmd.Parameters.AddWithValue("@ExpiryDateNeedFlag", NpgsqlDbType.Integer, CInt(item.ExpiryDateNeedFlag))
                            cmd.Parameters.AddWithValue("@MeasureType", NpgsqlDbType.Integer, CInt(item.MeasureType))
                            cmd.Parameters.AddWithValue("@UnusedFlag", NpgsqlDbType.Integer, CInt(item.UnusedFlag))
                            cmd.Parameters.AddWithValue("@RegistDate", NpgsqlDbType.Timestamp, nowDate)
                            cmd.Parameters.AddWithValue("@ModifyDate", NpgsqlDbType.Timestamp, nowDate)
                            item.ItemId = CInt(cmd.ExecuteScalar()).ToString()
                        Else
                            SQLString = "update Items set"
                            SQLString &= " ItemCode = @ItemCode"
                            SQLString &= ",JANCode = @JANCode"
                            SQLString &= ",ItemName = @ItemName"
                            SQLString &= ",ItemTypeId = @ItemTypeId"
                            SQLString &= ",UnitTypeId = @UnitTypeId"
                            SQLString &= ",LossRate = @LossRate"
                            SQLString &= ",Shipment = @Shipment"
                            SQLString &= ",PieceUnitTypeId = @PieceUnitTypeId"
                            SQLString &= ",PieceWeight = @PieceWeight"
                            SQLString &= ",CostPrice = @CostPrice"
                            SQLString &= ",OriginalSellingPrice = @OriginalSellingPrice"
                            SQLString &= ",InstoreSellingPrice = @InstoreSellingPrice"
                            SQLString &= ",MainSupplierId = @MainSupplierId"
                            SQLString &= ",ExpiryDateNeedFlag = @ExpiryDateNeedFlag"
                            SQLString &= ",MeasureType = @MeasureType"
                            SQLString &= ",UnUsedFlag = @UnUsedFlag"
                            SQLString &= ",ModifyDate = @ModifyDate"
                            SQLString &= " where ItemId = @ItemId"
                            cmd.CommandText = SQLString
                            cmd.Parameters.Clear()
                            cmd.Parameters.AddWithValue("@ItemCode", NpgsqlDbType.Varchar, item.ItemCode)
                            cmd.Parameters.AddWithValue("@JANCode", NpgsqlDbType.Varchar, item.JanCode)
                            cmd.Parameters.AddWithValue("@ItemName", NpgsqlDbType.Varchar, item.ItemName)
                            cmd.Parameters.AddWithValue("@ItemTypeId", NpgsqlDbType.Integer, CInt(item.ItemTypeId))
                            cmd.Parameters.AddWithValue("@UnitTypeId", NpgsqlDbType.Integer, CInt(item.UnitType.UnitTypeId))
                            If Not IsNothing(Nz(item.LossRate, Nothing)) Then
                                cmd.Parameters.AddWithValue("@LossRate", NpgsqlDbType.Double, CDbl(item.LossRate))
                            Else
                                cmd.Parameters.AddWithValue("@LossRate", NpgsqlDbType.Double, DBNull.Value)
                            End If
                            If Not IsNothing(Nz(item.Shipment, Nothing)) Then
                                cmd.Parameters.AddWithValue("@Shipment", NpgsqlDbType.Double, CDbl(item.Shipment))
                            Else
                                cmd.Parameters.AddWithValue("@Shipment", NpgsqlDbType.Double, DBNull.Value)
                            End If
                            If Not IsNothing(Nz(item.PieceUnitType.UnitTypeId, Nothing)) Then
                                cmd.Parameters.AddWithValue("@PieceUnitTypeId", NpgsqlDbType.Integer, CInt(item.PieceUnitType.UnitTypeId))
                            Else
                                cmd.Parameters.AddWithValue("@PieceUnitTypeId", NpgsqlDbType.Integer, DBNull.Value)
                            End If
                            If Not IsNothing(Nz(item.PieceWeight, Nothing)) Then
                                cmd.Parameters.AddWithValue("@PieceWeight", NpgsqlDbType.Double, CDbl(item.PieceWeight))
                            Else
                                cmd.Parameters.AddWithValue("@PieceWeight", NpgsqlDbType.Double, DBNull.Value)
                            End If
                            cmd.Parameters.AddWithValue("@CostPrice", NpgsqlDbType.Double, CDbl(Nz(item.CostPrice, 0)))
                            cmd.Parameters.AddWithValue("@OriginalSellingPrice", NpgsqlDbType.Integer, CInt(Nz(item.OriginalSellingPrice, 0)))
                            cmd.Parameters.AddWithValue("@InstoreSellingPrice", NpgsqlDbType.Integer, CInt(Nz(item.InstoreSellingPrice, 0)))
                            If Not IsNothing(Nz(item.MainSupplier.SupplierId, Nothing)) Then
                                cmd.Parameters.AddWithValue("@MainSupplierId", NpgsqlDbType.Integer, CInt(item.MainSupplier.SupplierId))
                            Else
                                cmd.Parameters.AddWithValue("@MainSupplierId", NpgsqlDbType.Integer, DBNull.Value)
                            End If
                            cmd.Parameters.AddWithValue("@ExpiryDateNeedFlag", NpgsqlDbType.Integer, CInt(item.ExpiryDateNeedFlag))
                            cmd.Parameters.AddWithValue("@MeasureType", NpgsqlDbType.Integer, CInt(item.MeasureType))
                            cmd.Parameters.AddWithValue("@UnusedFlag", NpgsqlDbType.Integer, CInt(item.UnusedFlag))
                            cmd.Parameters.AddWithValue("@ModifyDate", NpgsqlDbType.Timestamp, nowDate)
                            cmd.Parameters.AddWithValue("@ItemId", NpgsqlDbType.Integer, CInt(item.ItemId))
                            cmd.ExecuteNonQuery()
                        End If

                        trans.Commit()

                        ' マスタが変わったので次回アクセス時に読み直す
                        InvalidateCalc()
                    End Using
                Catch ex As Exception
                    trans.Rollback()
                    Throw
                End Try
            End Using
        End Using

        Return item

    End Function

    Public Shared Sub Delete(item As Item)

        Dim nowDate As Date = Now

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Using trans As NpgsqlTransaction = cn.BeginTransaction
                Try
                    Using cmd As NpgsqlCommand = cn.CreateCommand()
                        cmd.Transaction = trans

                        Dim SQLString As String
                        SQLString = "delete from RecipeDetails"
                        SQLString &= " where ItemId = @ItemId"
                        cmd.CommandText = SQLString
                        cmd.Parameters.Clear()
                        cmd.Parameters.AddWithValue("@ItemId", NpgsqlDbType.Integer, CInt(item.ItemId))
                        cmd.ExecuteNonQuery()

                        SQLString = "delete from Recipes"
                        SQLString &= " where ItemId = @ItemId"
                        cmd.CommandText = SQLString
                        cmd.Parameters.Clear()
                        cmd.Parameters.AddWithValue("@ItemId", NpgsqlDbType.Integer, CInt(item.ItemId))
                        cmd.ExecuteNonQuery()

                        SQLString = "delete from Items"
                        SQLString &= " where ItemId = @ItemId"
                        cmd.CommandText = SQLString
                        cmd.Parameters.Clear()
                        cmd.Parameters.AddWithValue("@ItemId", NpgsqlDbType.Integer, CInt(item.ItemId))
                        cmd.ExecuteNonQuery()
                    End Using

                    trans.Commit()

                    ' マスタが変わったので次回アクセス時に読み直す
                    InvalidateCalc()
                Catch ex As Exception
                    trans.Rollback()
                    Throw
                End Try
            End Using
        End Using

    End Sub

End Class

#End Region

#Region "レシピ"

Public Class Recipe
    Public Property RecipeId As String
    Public Property Item As New Item
    Public Property Comment As String
    Public Property RecipeDetails As New List(Of RecipeDetail)
End Class

Public Class Recipes

    Public Shared Function GetAll(Optional where As String = Nothing, Optional orderBy As String = Nothing) As List(Of Recipe)

        Dim result As New List(Of Recipe)

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Try
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String
                    SQLString = "select"
                    SQLString &= " Recipes.*"
                    SQLString &= " from Recipes"
                    SQLString &= " where RecipeId is not null"
                    If Not String.IsNullOrEmpty(where) Then
                        SQLString &= " and " & where
                    End If
                    If Not String.IsNullOrEmpty(orderBy) Then
                        SQLString &= " order by " & orderBy
                    End If
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        While dr.Read
                            Dim recipe As Recipe = Mapping(dr)
                            result.Add(recipe)
                        End While
                    End Using
                End Using
            Catch ex As Exception
                SysMsgBox(ex.Message, "", Reflection.MethodBase.GetCurrentMethod.Name)
                Throw
            End Try
        End Using

        Return result

    End Function

    Public Shared Function GetByItemId(itemId As Integer) As Recipe

        Dim result As Recipe = Nothing

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Try
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String
                    SQLString = "select"
                    SQLString &= " Recipes.*"
                    SQLString &= " from Recipes"
                    SQLString &= " where ItemId = @ItemId"
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    cmd.Parameters.AddWithValue("@ItemId", NpgsqlDbType.Integer, itemId)
                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        If dr.Read() Then result = Mapping(dr)
                    End Using
                End Using
            Catch ex As Exception
                SysMsgBox(ex.Message, "", Reflection.MethodBase.GetCurrentMethod.Name)
                Throw
            End Try
        End Using

        Return result

    End Function

    Private Shared Function Mapping(dr As NpgsqlDataReader) As Recipe

        Dim result As New Recipe

        result.RecipeId = dr("RecipeId").ToString
        result.Item = Items.GetById(dr("ItemId").ToString)
        result.Comment = dr("Comment").ToString
        result.RecipeDetails = RecipeDetails.GetByRecipeId(result.RecipeId)

        Return result

    End Function

    Public Shared Function Save(recipe As Recipe) As Recipe

        Dim nowDate As Date = Now

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Using trans As NpgsqlTransaction = cn.BeginTransaction
                Try
                    Using cmd As NpgsqlCommand = cn.CreateCommand()
                        cmd.Transaction = trans

                        Dim SQLString As String
                        If String.IsNullOrEmpty(recipe.RecipeId) Then
                            SQLString = "insert into Recipes ("
                            SQLString &= " ItemId"
                            SQLString &= ",Comment"
                            SQLString &= ",RegistDate"
                            SQLString &= ",ModifyDate"
                            SQLString &= " ) values ("
                            SQLString &= " @ItemId"
                            SQLString &= ",@Comment"
                            SQLString &= ",@RegistDate"
                            SQLString &= ",@ModifyDate"
                            SQLString &= ") returning RecipeId"
                            cmd.CommandText = SQLString
                            cmd.Parameters.Clear()
                            cmd.Parameters.AddWithValue("@ItemId", NpgsqlDbType.Integer, CInt(recipe.Item.ItemId))
                            cmd.Parameters.AddWithValue("@Comment", NpgsqlDbType.Varchar, recipe.Comment)
                            cmd.Parameters.AddWithValue("@RegistDate", NpgsqlDbType.Timestamp, nowDate)
                            cmd.Parameters.AddWithValue("@ModifyDate", NpgsqlDbType.Timestamp, nowDate)
                            recipe.RecipeId = CInt(cmd.ExecuteScalar()).ToString()
                        Else
                            SQLString = "update Recipes set"
                            SQLString &= " ItemId = @ItemId"
                            SQLString &= ",Comment = @Comment"
                            SQLString &= ",ModifyDate = @ModifyDate"
                            SQLString &= " where RecipeId = @RecipeId"
                            cmd.CommandText = SQLString
                            cmd.Parameters.Clear()
                            cmd.Parameters.AddWithValue("@ItemId", NpgsqlDbType.Integer, CInt(recipe.Item.ItemId))
                            cmd.Parameters.AddWithValue("@Comment", NpgsqlDbType.Varchar, recipe.Comment)
                            cmd.Parameters.AddWithValue("@ModifyDate", NpgsqlDbType.Timestamp, nowDate)
                            cmd.Parameters.AddWithValue("@RecipeId", NpgsqlDbType.Integer, CInt(recipe.RecipeId))
                            cmd.ExecuteNonQuery()
                        End If

                        SQLString = "delete from RecipeDetails"
                        SQLString &= " where RecipeId = @RecipeId"
                        cmd.CommandText = SQLString
                        cmd.Parameters.Clear()
                        cmd.Parameters.AddWithValue("@RecipeId", NpgsqlDbType.Integer, CInt(recipe.RecipeId))
                        cmd.ExecuteNonQuery()

                        For Each recipeDetail As RecipeDetail In recipe.RecipeDetails
                            SQLString = "insert into RecipeDetails ("
                            SQLString &= " RecipeId"
                            SQLString &= ",DetailNo"
                            SQLString &= ",ItemId"
                            SQLString &= ",Quantity"
                            SQLString &= ",UnitTypeId"
                            SQLString &= ",Comment"
                            SQLString &= ",RegistDate"
                            SQLString &= ",ModifyDate"
                            SQLString &= " ) values ("
                            SQLString &= " @RecipeId"
                            SQLString &= ",@DetailNo"
                            SQLString &= ",@ItemId"
                            SQLString &= ",@Quantity"
                            SQLString &= ",@UnitTypeId"
                            SQLString &= ",@Comment"
                            SQLString &= ",@RegistDate"
                            SQLString &= ",@ModifyDate"
                            SQLString &= ") returning RecipeId"
                            cmd.CommandText = SQLString
                            cmd.Parameters.Clear()
                            cmd.Parameters.AddWithValue("@RecipeId", NpgsqlDbType.Integer, CInt(recipe.RecipeId))
                            cmd.Parameters.AddWithValue("@DetailNo", NpgsqlDbType.Integer, CInt(recipeDetail.DetailNo))
                            cmd.Parameters.AddWithValue("@ItemId", NpgsqlDbType.Integer, CInt(recipeDetail.Item.ItemId))
                            cmd.Parameters.AddWithValue("@Quantity", NpgsqlDbType.Double, CDbl(recipeDetail.Quantity))
                            cmd.Parameters.AddWithValue("@UnitTypeId", NpgsqlDbType.Integer, CInt(recipeDetail.UnitType.UnitTypeId))
                            cmd.Parameters.AddWithValue("@Comment", NpgsqlDbType.Varchar, recipeDetail.Comment)
                            cmd.Parameters.AddWithValue("@RegistDate", NpgsqlDbType.Timestamp, nowDate)
                            cmd.Parameters.AddWithValue("@ModifyDate", NpgsqlDbType.Timestamp, nowDate)
                            cmd.ExecuteNonQuery()
                        Next
                    End Using

                    trans.Commit()

                    ' マスタが変わったので次回アクセス時に読み直す
                    InvalidateCalc()
                Catch ex As Exception
                    trans.Rollback()
                    Throw
                End Try
            End Using
        End Using

        Return recipe

    End Function

    Public Shared Sub Delete(recipe As Recipe)

        Dim nowDate As Date = Now

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Using trans As NpgsqlTransaction = cn.BeginTransaction
                Try
                    Using cmd As NpgsqlCommand = cn.CreateCommand()
                        cmd.Transaction = trans

                        Dim SQLString As String
                        SQLString = "delete from RecipeDetails"
                        SQLString &= " where RecipeId = @RecipeId"
                        cmd.CommandText = SQLString
                        cmd.Parameters.Clear()
                        cmd.Parameters.AddWithValue("@RecipeId", NpgsqlDbType.Integer, CInt(recipe.RecipeId))
                        cmd.ExecuteNonQuery()

                        SQLString = "delete from Recipes"
                        SQLString &= " where RecipeId = @RecipeId"
                        cmd.CommandText = SQLString
                        cmd.Parameters.Clear()
                        cmd.Parameters.AddWithValue("@RecipeId", NpgsqlDbType.Integer, CInt(recipe.RecipeId))
                        cmd.ExecuteNonQuery()
                    End Using

                    trans.Commit()

                    ' マスタが変わったので次回アクセス時に読み直す
                    InvalidateCalc()
                Catch ex As Exception
                    trans.Rollback()
                    Throw
                End Try
            End Using
        End Using

    End Sub

End Class

Public Class RecipeDetail
    Public Property RecipeDetailId As String
    Public Property RecipeId As String
    Public Property DetailNo As String
    Public Property Item As New Item
    Public Property Quantity As String
    Public Property UnitType As New UnitType
    Public Property Comment As String
End Class

Public Class RecipeDetails

    Public Shared Function GetByRecipeId(recipeId As Integer) As List(Of RecipeDetail)

        Dim result As New List(Of RecipeDetail)

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Try
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String
                    SQLString = "select"
                    SQLString &= " RecipeDetails.*"
                    SQLString &= " from RecipeDetails"
                    SQLString &= " where RecipeId = @RecipeId"
                    SQLString &= " order by"
                    SQLString &= " DetailNo"
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    cmd.Parameters.AddWithValue("@RecipeId", NpgsqlDbType.Integer, recipeId)
                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        While dr.Read
                            Dim recipeDetail As RecipeDetail = Mapping(dr)
                            result.Add(recipeDetail)
                        End While
                    End Using
                End Using
            Catch ex As Exception
                SysMsgBox(ex.Message, "", Reflection.MethodBase.GetCurrentMethod.Name)
                Throw
            End Try
        End Using

        Return result

    End Function

    Private Shared Function Mapping(dr As NpgsqlDataReader) As RecipeDetail

        Dim result As New RecipeDetail

        result.RecipeDetailId = dr("RecipeDetailId").ToString
        result.RecipeId = dr("RecipeId").ToString
        result.DetailNo = dr("DetailNo").ToString
        result.Item = Items.GetById(dr("ItemId").ToString)
        result.Quantity = dr("Quantity").ToString
        result.UnitType.UnitTypeId = dr("UnitTypeId").ToString
        result.Comment = dr("Comment").ToString

        Return result

    End Function

End Class

#End Region

#Region "店舗"

Public Class Store
    Public Property StoreId As String
    Public Property StoreCode As String
    Public Property StoreName As String
    Public Property StoreShortName As String
    Public Property CenterFlag As String
    Public Property OrderNo As String
    Public Property UnusedFlag As String
End Class

Public Class Stores

    Public Shared Function GetAll(Optional where As String = Nothing, Optional orderBy As String = Nothing) As List(Of Store)

        Dim result As New List(Of Store)

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Try
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String
                    SQLString = "select"
                    SQLString &= " Stores.*"
                    SQLString &= " from Stores"
                    SQLString &= " where StoreId is not null"
                    If Not String.IsNullOrEmpty(where) Then
                        SQLString &= " and " & where
                    End If
                    If Not String.IsNullOrEmpty(orderBy) Then
                        SQLString &= " order by " & orderBy
                    Else
                        SQLString &= " order by OrderNo"
                    End If
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        While dr.Read
                            Dim store As Store = Mapping(dr)
                            result.Add(store)
                        End While
                    End Using
                End Using
            Catch ex As Exception
                SysMsgBox(ex.Message, "", Reflection.MethodBase.GetCurrentMethod.Name)
                Throw
            End Try
        End Using

        Return result

    End Function

    Public Shared Function GetById(id As Integer) As Store

        Dim result As Store = Nothing

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Try
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String
                    SQLString = "select"
                    SQLString &= " Stores.*"
                    SQLString &= " from Stores"
                    SQLString &= " where StoreId = @StoreId"
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    cmd.Parameters.AddWithValue("@StoreId", NpgsqlDbType.Integer, id)
                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        If dr.Read() Then result = Mapping(dr)
                    End Using
                End Using
            Catch ex As Exception
                SysMsgBox(ex.Message, "", Reflection.MethodBase.GetCurrentMethod.Name)
                Throw
            End Try
        End Using

        Return result

    End Function

    Private Shared Function Mapping(dr As NpgsqlDataReader) As Store

        Dim result As New Store

        result.StoreId = dr("StoreId").ToString
        result.StoreCode = dr("StoreCode").ToString
        result.StoreName = dr("StoreName").ToString
        result.StoreShortName = dr("StoreShortName").ToString
        result.CenterFlag = dr("CenterFlag").ToString
        result.OrderNo = dr("OrderNo").ToString
        result.UnusedFlag = dr("UnusedFlag").ToString

        Return result

    End Function

    Public Shared Function Save(store As Store) As Store

        Dim nowDate As Date = Now

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Using trans As NpgsqlTransaction = cn.BeginTransaction
                Try
                    Using cmd As NpgsqlCommand = cn.CreateCommand()
                        cmd.Transaction = trans

                        Dim SQLString As String
                        If String.IsNullOrEmpty(store.StoreId) Then
                            SQLString = "insert into Stores ("
                            SQLString &= " StoreCode"
                            SQLString &= ",StoreName"
                            SQLString &= ",StoreShortName"
                            SQLString &= ",CenterFlag"
                            SQLString &= ",OrderNo"
                            SQLString &= ",UnUsedFlag"
                            SQLString &= ",RegistDate"
                            SQLString &= ",ModifyDate"
                            SQLString &= " ) values ("
                            SQLString &= " @StoreCode"
                            SQLString &= ",@StoreName"
                            SQLString &= ",@StoreShortName"
                            SQLString &= ",@CenterFlag"
                            SQLString &= ",@OrderNo"
                            SQLString &= ",@UnUsedFlag"
                            SQLString &= ",@RegistDate"
                            SQLString &= ",@ModifyDate"
                            SQLString &= ") returning StoreId"
                            cmd.CommandText = SQLString
                            cmd.Parameters.Clear()
                            cmd.Parameters.AddWithValue("@StoreCode", NpgsqlDbType.Varchar, store.StoreCode)
                            cmd.Parameters.AddWithValue("@StoreName", NpgsqlDbType.Varchar, store.StoreName)
                            cmd.Parameters.AddWithValue("@StoreShortName", NpgsqlDbType.Varchar, store.StoreShortName)
                            cmd.Parameters.AddWithValue("@CenterFlag", NpgsqlDbType.Integer, CInt(store.CenterFlag))
                            cmd.Parameters.AddWithValue("@OrderNo", NpgsqlDbType.Integer, CInt(Nz(store.OrderNo, 0)))
                            cmd.Parameters.AddWithValue("@UnusedFlag", NpgsqlDbType.Integer, CInt(store.UnusedFlag))
                            cmd.Parameters.AddWithValue("@RegistDate", NpgsqlDbType.Timestamp, nowDate)
                            cmd.Parameters.AddWithValue("@ModifyDate", NpgsqlDbType.Timestamp, nowDate)
                            store.StoreId = CInt(cmd.ExecuteScalar()).ToString()
                        Else
                            SQLString = "update Stores set"
                            SQLString &= " StoreCode = @StoreCode"
                            SQLString &= ",StoreName = @StoreName"
                            SQLString &= ",StoreShortName = @StoreShortName"
                            SQLString &= ",CenterFlag = @CenterFlag"
                            SQLString &= ",OrderNo = @OrderNo"
                            SQLString &= ",UnUsedFlag = @UnUsedFlag"
                            SQLString &= ",ModifyDate = @ModifyDate"
                            SQLString &= " where StoreId = @StoreId"
                            cmd.CommandText = SQLString
                            cmd.Parameters.Clear()
                            cmd.Parameters.AddWithValue("@StoreCode", NpgsqlDbType.Varchar, store.StoreCode)
                            cmd.Parameters.AddWithValue("@StoreName", NpgsqlDbType.Varchar, store.StoreName)
                            cmd.Parameters.AddWithValue("@StoreShortName", NpgsqlDbType.Varchar, store.StoreShortName)
                            cmd.Parameters.AddWithValue("@CenterFlag", NpgsqlDbType.Integer, CInt(store.CenterFlag))
                            cmd.Parameters.AddWithValue("@OrderNo", NpgsqlDbType.Integer, CInt(Nz(store.OrderNo, 0)))
                            cmd.Parameters.AddWithValue("@UnusedFlag", NpgsqlDbType.Integer, CInt(store.UnusedFlag))
                            cmd.Parameters.AddWithValue("@ModifyDate", NpgsqlDbType.Timestamp, nowDate)
                            cmd.Parameters.AddWithValue("@StoreId", NpgsqlDbType.Integer, CInt(store.StoreId))
                            cmd.ExecuteNonQuery()
                        End If

                        trans.Commit()
                    End Using
                Catch ex As Exception
                    trans.Rollback()
                    Throw
                End Try
            End Using
        End Using

        Return store

    End Function

    Public Shared Sub Delete(store As Store)

        Dim nowDate As Date = Now

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Using trans As NpgsqlTransaction = cn.BeginTransaction
                Try
                    Using cmd As NpgsqlCommand = cn.CreateCommand()
                        cmd.Transaction = trans

                        Dim SQLString As String
                        SQLString = "delete from Stores"
                        SQLString &= " where StoreId = @StoreId"
                        cmd.CommandText = SQLString
                        cmd.Parameters.Clear()
                        cmd.Parameters.AddWithValue("@StoreId", NpgsqlDbType.Integer, CInt(store.StoreId))
                        cmd.ExecuteNonQuery()
                    End Using

                    trans.Commit()
                Catch ex As Exception
                    trans.Rollback()
                    Throw
                End Try
            End Using
        End Using

    End Sub

End Class

#End Region

#Region "仕入先"

Public Class Supplier
    Public Property SupplierId As String
    Public Property SupplierCode As String
    Public Property SupplierName As String
    Public Property PostNo As String
    Public Property Address1 As String
    Public Property Address2 As String
    Public Property Address3 As String
    Public Property TelNo As String
    Public Property FaxNo As String
    Public Property ItemType As New ItemType
    Public Property UnusedFlag As String
End Class

Public Class Suppliers

    Public Shared Function GetAll(Optional where As String = Nothing, Optional orderBy As String = Nothing) As List(Of Supplier)

        Dim result As New List(Of Supplier)

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Try
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String
                    SQLString = "select"
                    SQLString &= " Suppliers.*"
                    SQLString &= " from Suppliers"
                    SQLString &= " where SupplierId is not null"
                    If Not String.IsNullOrEmpty(where) Then
                        SQLString &= " and " & where
                    End If
                    If Not String.IsNullOrEmpty(orderBy) Then
                        SQLString &= " order by " & orderBy
                    End If
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        While dr.Read
                            Dim Supplier As Supplier = Mapping(dr)
                            result.Add(Supplier)
                        End While
                    End Using
                End Using
            Catch ex As Exception
                SysMsgBox(ex.Message, "", Reflection.MethodBase.GetCurrentMethod.Name)
                Throw
            End Try
        End Using

        Return result

    End Function

    Public Shared Function GetById(id As Integer) As Supplier

        Dim result As Supplier = Nothing

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Try
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String
                    SQLString = "select"
                    SQLString &= " Suppliers.*"
                    SQLString &= " from Suppliers"
                    SQLString &= " where SupplierId = @SupplierId"
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    cmd.Parameters.AddWithValue("@SupplierId", NpgsqlDbType.Integer, id)
                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        If dr.Read() Then result = Mapping(dr)
                    End Using
                End Using
            Catch ex As Exception
                SysMsgBox(ex.Message, "", Reflection.MethodBase.GetCurrentMethod.Name)
                Throw
            End Try
        End Using

        Return result

    End Function

    Private Shared Function Mapping(dr As NpgsqlDataReader) As Supplier

        Dim result As New Supplier

        result.SupplierId = dr("SupplierId").ToString
        result.SupplierCode = dr("SupplierCode").ToString
        result.SupplierName = dr("SupplierName").ToString
        result.PostNo = dr("PostNo").ToString
        result.Address1 = dr("Address1").ToString
        result.Address2 = dr("Address2").ToString
        result.Address3 = dr("Address3").ToString
        result.TelNo = dr("TelNo").ToString
        result.FaxNo = dr("FaxNo").ToString
        If Not IsNothing(Nz(dr("ItemTypeId"), Nothing)) Then
            result.ItemType = ItemTypes.GetById(dr("ItemTypeId").ToString)
        End If
        result.UnusedFlag = dr("UnusedFlag").ToString

        Return result

    End Function

    Public Shared Function Save(supplier As Supplier) As Supplier

        Dim nowDate As Date = Now

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Using trans As NpgsqlTransaction = cn.BeginTransaction
                Try
                    Using cmd As NpgsqlCommand = cn.CreateCommand()
                        cmd.Transaction = trans

                        Dim SQLString As String
                        If String.IsNullOrEmpty(supplier.SupplierId) Then
                            SQLString = "insert into Suppliers ("
                            SQLString &= " SupplierCode"
                            SQLString &= ",SupplierName"
                            SQLString &= ",PostNo"
                            SQLString &= ",Address1"
                            SQLString &= ",Address2"
                            SQLString &= ",Address3"
                            SQLString &= ",TelNo"
                            SQLString &= ",FaxNo"
                            SQLString &= ",ItemTypeId"
                            SQLString &= ",UnUsedFlag"
                            SQLString &= ",RegistDate"
                            SQLString &= ",ModifyDate"
                            SQLString &= " ) values ("
                            SQLString &= " @SupplierCode"
                            SQLString &= ",@SupplierName"
                            SQLString &= ",@PostNo"
                            SQLString &= ",@Address1"
                            SQLString &= ",@Address2"
                            SQLString &= ",@Address3"
                            SQLString &= ",@TelNo"
                            SQLString &= ",@FaxNo"
                            SQLString &= ",@ItemTypeId"
                            SQLString &= ",@UnUsedFlag"
                            SQLString &= ",@RegistDate"
                            SQLString &= ",@ModifyDate"
                            SQLString &= ") returning SupplierId"
                            cmd.CommandText = SQLString
                            cmd.Parameters.Clear()
                            cmd.Parameters.AddWithValue("@SupplierCode", NpgsqlDbType.Varchar, supplier.SupplierCode)
                            cmd.Parameters.AddWithValue("@SupplierName", NpgsqlDbType.Varchar, supplier.SupplierName)
                            cmd.Parameters.AddWithValue("@PostNo", NpgsqlDbType.Varchar, supplier.PostNo)
                            cmd.Parameters.AddWithValue("@Address1", NpgsqlDbType.Varchar, supplier.Address1)
                            cmd.Parameters.AddWithValue("@Address2", NpgsqlDbType.Varchar, supplier.Address2)
                            cmd.Parameters.AddWithValue("@Address3", NpgsqlDbType.Varchar, supplier.Address3)
                            cmd.Parameters.AddWithValue("@TelNo", NpgsqlDbType.Varchar, supplier.TelNo)
                            cmd.Parameters.AddWithValue("@FaxNo", NpgsqlDbType.Varchar, supplier.FaxNo)
                            If Not IsNothing(Nz(supplier.ItemType.ItemTypeId, Nothing)) Then
                                cmd.Parameters.AddWithValue("@ItemTypeId", NpgsqlDbType.Integer, CInt(supplier.ItemType.ItemTypeId))
                            Else
                                cmd.Parameters.AddWithValue("@ItemTypeId", NpgsqlDbType.Integer, DBNull.Value)
                            End If
                            cmd.Parameters.AddWithValue("@UnusedFlag", NpgsqlDbType.Integer, CInt(supplier.UnusedFlag))
                            cmd.Parameters.AddWithValue("@RegistDate", NpgsqlDbType.Timestamp, nowDate)
                            cmd.Parameters.AddWithValue("@ModifyDate", NpgsqlDbType.Timestamp, nowDate)
                            supplier.SupplierId = CInt(cmd.ExecuteScalar()).ToString()
                        Else
                            SQLString = "update Suppliers set"
                            SQLString &= " SupplierCode = @SupplierCode"
                            SQLString &= ",SupplierName = @SupplierName"
                            SQLString &= ",PostNo = @PostNo"
                            SQLString &= ",Address1 = @Address1"
                            SQLString &= ",Address2 = @Address2"
                            SQLString &= ",Address3 = @Address3"
                            SQLString &= ",TelNo = @TelNo"
                            SQLString &= ",FaxNo = @FaxNo"
                            SQLString &= ",ItemTypeId = @ItemTypeId"
                            SQLString &= ",UnUsedFlag = @UnUsedFlag"
                            SQLString &= ",ModifyDate = @ModifyDate"
                            SQLString &= " where SupplierId = @SupplierId"
                            cmd.CommandText = SQLString
                            cmd.Parameters.Clear()
                            cmd.Parameters.AddWithValue("@SupplierCode", NpgsqlDbType.Varchar, supplier.SupplierCode)
                            cmd.Parameters.AddWithValue("@SupplierName", NpgsqlDbType.Varchar, supplier.SupplierName)
                            cmd.Parameters.AddWithValue("@PostNo", NpgsqlDbType.Varchar, supplier.PostNo)
                            cmd.Parameters.AddWithValue("@Address1", NpgsqlDbType.Varchar, supplier.Address1)
                            cmd.Parameters.AddWithValue("@Address2", NpgsqlDbType.Varchar, supplier.Address2)
                            cmd.Parameters.AddWithValue("@Address3", NpgsqlDbType.Varchar, supplier.Address3)
                            cmd.Parameters.AddWithValue("@TelNo", NpgsqlDbType.Varchar, supplier.TelNo)
                            cmd.Parameters.AddWithValue("@FaxNo", NpgsqlDbType.Varchar, supplier.FaxNo)
                            If Not IsNothing(Nz(supplier.ItemType.ItemTypeId, Nothing)) Then
                                cmd.Parameters.AddWithValue("@ItemTypeId", NpgsqlDbType.Integer, CInt(supplier.ItemType.ItemTypeId))
                            Else
                                cmd.Parameters.AddWithValue("@ItemTypeId", NpgsqlDbType.Integer, DBNull.Value)
                            End If
                            cmd.Parameters.AddWithValue("@UnusedFlag", NpgsqlDbType.Integer, CInt(supplier.UnusedFlag))
                            cmd.Parameters.AddWithValue("@ModifyDate", NpgsqlDbType.Timestamp, nowDate)
                            cmd.Parameters.AddWithValue("@SupplierId", NpgsqlDbType.Integer, CInt(supplier.SupplierId))
                            cmd.ExecuteNonQuery()
                        End If

                        trans.Commit()
                    End Using
                Catch ex As Exception
                    trans.Rollback()
                    Throw
                End Try
            End Using
        End Using

        Return supplier

    End Function

    Public Shared Sub Delete(supplier As Supplier)

        Dim nowDate As Date = Now

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Using trans As NpgsqlTransaction = cn.BeginTransaction
                Try
                    Using cmd As NpgsqlCommand = cn.CreateCommand()
                        cmd.Transaction = trans

                        Dim SQLString As String
                        SQLString = "delete from Suppliers"
                        SQLString &= " where SupplierId = @SupplierId"
                        cmd.CommandText = SQLString
                        cmd.Parameters.Clear()
                        cmd.Parameters.AddWithValue("@SupplierId", NpgsqlDbType.Integer, CInt(supplier.SupplierId))
                        cmd.ExecuteNonQuery()
                    End Using

                    trans.Commit()
                Catch ex As Exception
                    trans.Rollback()
                    Throw
                End Try
            End Using
        End Using

    End Sub

End Class

#End Region

#Region "単位"

Public Class UnitType
    Public Property UnitTypeId As String
    Public Property UnitTypeName As String
    Public Property OrderNo As String
    Public Property UnitCategory As String
    Public Property BaseFactor As String
    Public Property NonConvertibleFlag As String
    Public Property UnusedFlag As String
End Class

Public Class UnitTypes

    Public Shared Function GetAll(Optional where As String = Nothing, Optional orderBy As String = Nothing) As List(Of UnitType)

        Dim result As New List(Of UnitType)

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Try
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String
                    SQLString = "select"
                    SQLString &= " UnitTypes.*"
                    SQLString &= " from UnitTypes"
                    SQLString &= " where UnitTypeId is not null"
                    If Not String.IsNullOrEmpty(where) Then
                        SQLString &= " and " & where
                    End If
                    If Not String.IsNullOrEmpty(orderBy) Then
                        SQLString &= " order by " & orderBy
                    Else
                        SQLString &= " order by OrderNo"
                    End If
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        While dr.Read
                            Dim UnitType As UnitType = Mapping(dr)
                            result.Add(UnitType)
                        End While
                    End Using
                End Using
            Catch ex As Exception
                SysMsgBox(ex.Message, "", Reflection.MethodBase.GetCurrentMethod.Name)
                Throw
            End Try
        End Using

        Return result

    End Function

    Public Shared Function GetById(id As Integer) As UnitType

        Dim result As UnitType = Nothing

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Try
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String
                    SQLString = "select"
                    SQLString &= " UnitTypes.*"
                    SQLString &= " from UnitTypes"
                    SQLString &= " where UnitTypeId = @UnitTypeId"
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    cmd.Parameters.AddWithValue("@UnitTypeId", NpgsqlDbType.Integer, id)
                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        If dr.Read() Then result = Mapping(dr)
                    End Using
                End Using
            Catch ex As Exception
                SysMsgBox(ex.Message, "", Reflection.MethodBase.GetCurrentMethod.Name)
                Throw
            End Try
        End Using

        Return result

    End Function

    Private Shared Function Mapping(dr As NpgsqlDataReader) As UnitType

        Dim result As New UnitType

        result.UnitTypeId = dr("UnitTypeId").ToString
        result.UnitTypeName = dr("UnitTypeName").ToString
        result.OrderNo = dr("OrderNo").ToString
        result.UnitCategory = dr("UnitCategory").ToString
        result.BaseFactor = dr("BaseFactor").ToString
        result.NonConvertibleFlag = dr("NonConvertibleFlag").ToString
        result.UnusedFlag = dr("UnusedFlag").ToString

        Return result

    End Function

    Public Shared Sub BindComboBox(targetComboBox As Object, Optional addUnitEmpty As Boolean = False, Optional where As String = "")

        Dim dtUnitTypes As New DataTable()
        dtUnitTypes.Columns.Add("UnitTypeId", GetType(String))
        dtUnitTypes.Columns.Add("UnitTypeName", GetType(String))

        If addUnitEmpty Then dtUnitTypes.Rows.Add(DBNull.Value, "")
        Dim UnitTypeList As List(Of UnitType) = UnitTypes.GetAll(where)
        For Each UnitType As UnitType In UnitTypeList
            dtUnitTypes.Rows.Add(UnitType.UnitTypeId, UnitType.UnitTypeName)
        Next
        dtUnitTypes.AcceptChanges()

        With targetComboBox
            .DisplayMember = "UnitTypeName"
            .ValueMember = "UnitTypeId"
            .DataSource = dtUnitTypes
        End With

    End Sub

End Class

#End Region

#Region "担当者"

Public Class [Operator]
    Public Property OperatorId As String
    Public Property OperatorCode As String
    Public Property OperatorName As String
    Public Property PasswordHash As String
    Public Property PasswordSalt As String
    Public Property RoleId As String
    Public Property LastLoginDate As String
    Public Property OrderNo As String
    Public Property UnusedFlag As String
    ' 管理者かどうか
    Public ReadOnly Property IsAdministrator As Boolean
        Get
            Return CInt(Nz(RoleId, 0)) = OperatorRole.Administrator
        End Get
    End Property
    ' 権限の表示名
    Public ReadOnly Property RoleName As String
        Get
            Return If(IsAdministrator, "管理者", "一般")
        End Get
    End Property
End Class

Public Class Operators

    Public Shared Function GetAll(Optional where As String = Nothing, Optional orderBy As String = Nothing) As List(Of [Operator])

        Dim result As New List(Of [Operator])

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Try
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String
                    SQLString = "select"
                    SQLString &= " *"
                    SQLString &= " from Operators"
                    SQLString &= " where OperatorId is not null"
                    If Not String.IsNullOrEmpty(where) Then
                        SQLString &= " and " & where
                    End If
                    If Not String.IsNullOrEmpty(orderBy) Then
                        SQLString &= " order by " & orderBy
                    Else
                        SQLString &= " order by OrderNo"
                    End If
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        While dr.Read
                            result.Add(Mapping(dr))
                        End While
                    End Using
                End Using
            Catch ex As Exception
                SysMsgBox(ex.Message, "", Reflection.MethodBase.GetCurrentMethod.Name)
                Throw
            End Try
        End Using

        Return result

    End Function

    Public Shared Function GetById(id As Integer) As [Operator]

        Dim result As [Operator] = Nothing

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Try
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String
                    SQLString = "select"
                    SQLString &= " *"
                    SQLString &= " from Operators"
                    SQLString &= " where OperatorId = @OperatorId"
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    cmd.Parameters.AddWithValue("@OperatorId", NpgsqlDbType.Integer, id)
                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        If dr.Read() Then result = Mapping(dr)
                    End Using
                End Using
            Catch ex As Exception
                SysMsgBox(ex.Message, "", Reflection.MethodBase.GetCurrentMethod.Name)
                Throw
            End Try
        End Using

        Return result

    End Function

    Public Shared Function GetByCode(code As String) As [Operator]

        Dim result As [Operator] = Nothing

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Try
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String
                    SQLString = "select"
                    SQLString &= " *"
                    SQLString &= " from Operators"
                    SQLString &= " where OperatorCode = @OperatorCode"
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    cmd.Parameters.AddWithValue("@OperatorCode", NpgsqlDbType.Varchar, code)
                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        If dr.Read() Then result = Mapping(dr)
                    End Using
                End Using
            Catch ex As Exception
                SysMsgBox(ex.Message, "", Reflection.MethodBase.GetCurrentMethod.Name)
                Throw
            End Try
        End Using

        Return result

    End Function

    ' 担当者コードの重複チェック
    Public Shared Function ExistsCode(code As String, Optional excludeId As String = Nothing) As Boolean

        Dim result As Boolean = False

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Try
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String
                    SQLString = "select count(*) from Operators"
                    SQLString &= " where OperatorCode = @OperatorCode"
                    If Not String.IsNullOrEmpty(excludeId) Then
                        SQLString &= " and OperatorId <> @OperatorId"
                    End If
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    cmd.Parameters.AddWithValue("@OperatorCode", NpgsqlDbType.Varchar, code)
                    If Not String.IsNullOrEmpty(excludeId) Then
                        cmd.Parameters.AddWithValue("@OperatorId", NpgsqlDbType.Integer, CInt(excludeId))
                    End If
                    result = (CInt(cmd.ExecuteScalar()) > 0)
                End Using
            Catch ex As Exception
                SysMsgBox(ex.Message, "", Reflection.MethodBase.GetCurrentMethod.Name)
                Throw
            End Try
        End Using

        Return result

    End Function

    Private Shared Function Mapping(dr As NpgsqlDataReader) As [Operator]

        Dim result As New [Operator]

        result.OperatorId = dr("OperatorId").ToString
        result.OperatorCode = dr("OperatorCode").ToString
        result.OperatorName = dr("OperatorName").ToString
        result.PasswordHash = dr("PasswordHash").ToString
        result.PasswordSalt = dr("PasswordSalt").ToString
        result.RoleId = dr("RoleId").ToString
        result.LastLoginDate = dr("LastLoginDate").ToString
        result.OrderNo = dr("OrderNo").ToString
        result.UnusedFlag = dr("UnusedFlag").ToString

        Return result

    End Function

    ' 担当者を保存する(パスワードは新規登録時のみ書き込む)
    Public Shared Function Save([operator] As [Operator]) As [Operator]

        Dim nowDate As Date = Now

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Using trans As NpgsqlTransaction = cn.BeginTransaction
                Try
                    Using cmd As NpgsqlCommand = cn.CreateCommand()
                        cmd.Transaction = trans

                        Dim SQLString As String
                        If String.IsNullOrEmpty([operator].OperatorId) Then
                            SQLString = "insert into Operators ("
                            SQLString &= " OperatorCode"
                            SQLString &= ",OperatorName"
                            SQLString &= ",PasswordHash"
                            SQLString &= ",PasswordSalt"
                            SQLString &= ",RoleId"
                            SQLString &= ",OrderNo"
                            SQLString &= ",UnusedFlag"
                            SQLString &= ",RegistDate"
                            SQLString &= ",ModifyDate"
                            SQLString &= " ) values ("
                            SQLString &= " @OperatorCode"
                            SQLString &= ",@OperatorName"
                            SQLString &= ",@PasswordHash"
                            SQLString &= ",@PasswordSalt"
                            SQLString &= ",@RoleId"
                            SQLString &= ",@OrderNo"
                            SQLString &= ",@UnusedFlag"
                            SQLString &= ",@RegistDate"
                            SQLString &= ",@ModifyDate"
                            SQLString &= ") returning OperatorId"
                            cmd.CommandText = SQLString
                            cmd.Parameters.Clear()
                            cmd.Parameters.AddWithValue("@OperatorCode", NpgsqlDbType.Varchar, [operator].OperatorCode)
                            cmd.Parameters.AddWithValue("@OperatorName", NpgsqlDbType.Varchar, [operator].OperatorName)
                            cmd.Parameters.AddWithValue("@PasswordHash", NpgsqlDbType.Varchar, Nz([operator].PasswordHash, DBNull.Value))
                            cmd.Parameters.AddWithValue("@PasswordSalt", NpgsqlDbType.Varchar, Nz([operator].PasswordSalt, DBNull.Value))
                            cmd.Parameters.AddWithValue("@RoleId", NpgsqlDbType.Integer, CInt(Nz([operator].RoleId, OperatorRole.General)))
                            cmd.Parameters.AddWithValue("@OrderNo", NpgsqlDbType.Integer, CInt(Nz([operator].OrderNo, 0)))
                            cmd.Parameters.AddWithValue("@UnusedFlag", NpgsqlDbType.Integer, CInt(Nz([operator].UnusedFlag, 0)))
                            cmd.Parameters.AddWithValue("@RegistDate", NpgsqlDbType.Timestamp, nowDate)
                            cmd.Parameters.AddWithValue("@ModifyDate", NpgsqlDbType.Timestamp, nowDate)
                            [operator].OperatorId = CInt(cmd.ExecuteScalar()).ToString()
                        Else
                            SQLString = "update Operators set"
                            SQLString &= " OperatorCode = @OperatorCode"
                            SQLString &= ",OperatorName = @OperatorName"
                            SQLString &= ",RoleId = @RoleId"
                            SQLString &= ",OrderNo = @OrderNo"
                            SQLString &= ",UnusedFlag = @UnusedFlag"
                            SQLString &= ",ModifyDate = @ModifyDate"
                            SQLString &= " where OperatorId = @OperatorId"
                            cmd.CommandText = SQLString
                            cmd.Parameters.Clear()
                            cmd.Parameters.AddWithValue("@OperatorCode", NpgsqlDbType.Varchar, [operator].OperatorCode)
                            cmd.Parameters.AddWithValue("@OperatorName", NpgsqlDbType.Varchar, [operator].OperatorName)
                            cmd.Parameters.AddWithValue("@RoleId", NpgsqlDbType.Integer, CInt(Nz([operator].RoleId, OperatorRole.General)))
                            cmd.Parameters.AddWithValue("@OrderNo", NpgsqlDbType.Integer, CInt(Nz([operator].OrderNo, 0)))
                            cmd.Parameters.AddWithValue("@UnusedFlag", NpgsqlDbType.Integer, CInt(Nz([operator].UnusedFlag, 0)))
                            cmd.Parameters.AddWithValue("@ModifyDate", NpgsqlDbType.Timestamp, nowDate)
                            cmd.Parameters.AddWithValue("@OperatorId", NpgsqlDbType.Integer, CInt([operator].OperatorId))
                            cmd.ExecuteNonQuery()
                        End If

                        trans.Commit()
                    End Using
                Catch ex As Exception
                    trans.Rollback()
                    Throw
                End Try
            End Using
        End Using

        Return [operator]

    End Function

    Public Shared Sub Delete([operator] As [Operator])

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Using trans As NpgsqlTransaction = cn.BeginTransaction
                Try
                    Using cmd As NpgsqlCommand = cn.CreateCommand()
                        cmd.Transaction = trans

                        Dim SQLString As String
                        SQLString = "delete from Operators"
                        SQLString &= " where OperatorId = @OperatorId"
                        cmd.CommandText = SQLString
                        cmd.Parameters.Clear()
                        cmd.Parameters.AddWithValue("@OperatorId", NpgsqlDbType.Integer, CInt([operator].OperatorId))
                        cmd.ExecuteNonQuery()
                    End Using

                    trans.Commit()
                Catch ex As Exception
                    trans.Rollback()
                    Throw
                End Try
            End Using
        End Using

    End Sub

    ' パスワードを照合する
    Public Shared Function VerifyPassword(target As [Operator], password As String) As Boolean

        If target Is Nothing Then Return False

        ' パスワード未設定の担当者は空パスワードで通す
        If String.IsNullOrEmpty(target.PasswordSalt) Then
            Return String.IsNullOrEmpty(password)
        End If

        Return String.Equals(target.PasswordHash,
                             HashPassword(password, target.PasswordSalt),
                             StringComparison.OrdinalIgnoreCase)

    End Function

    ' パスワードを設定する(ソルトも作り直す)
    Public Shared Sub SavePassword(operatorId As String, newPassword As String)

        Dim nowDate As Date = Now
        Dim salt As String = NewPasswordSalt()
        Dim hash As String = HashPassword(newPassword, salt)

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Using trans As NpgsqlTransaction = cn.BeginTransaction
                Try
                    Using cmd As NpgsqlCommand = cn.CreateCommand()
                        cmd.Transaction = trans

                        Dim SQLString As String
                        SQLString = "update Operators set"
                        SQLString &= " PasswordHash = @PasswordHash"
                        SQLString &= ",PasswordSalt = @PasswordSalt"
                        SQLString &= ",ModifyDate = @ModifyDate"
                        SQLString &= " where OperatorId = @OperatorId"
                        cmd.CommandText = SQLString
                        cmd.Parameters.Clear()
                        cmd.Parameters.AddWithValue("@PasswordHash", NpgsqlDbType.Varchar, hash)
                        cmd.Parameters.AddWithValue("@PasswordSalt", NpgsqlDbType.Varchar, salt)
                        cmd.Parameters.AddWithValue("@ModifyDate", NpgsqlDbType.Timestamp, nowDate)
                        cmd.Parameters.AddWithValue("@OperatorId", NpgsqlDbType.Integer, CInt(operatorId))
                        cmd.ExecuteNonQuery()
                    End Using

                    trans.Commit()
                Catch ex As Exception
                    trans.Rollback()
                    Throw
                End Try
            End Using
        End Using

    End Sub

    ' 最終ログイン日時を更新する
    Public Shared Sub UpdateLastLoginDate(operatorId As Integer)

        Dim nowDate As Date = Now

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Try
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String
                    SQLString = "update Operators set"
                    SQLString &= " LastLoginDate = @LastLoginDate"
                    SQLString &= " where OperatorId = @OperatorId"
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    cmd.Parameters.AddWithValue("@LastLoginDate", NpgsqlDbType.Timestamp, nowDate)
                    cmd.Parameters.AddWithValue("@OperatorId", NpgsqlDbType.Integer, operatorId)
                    cmd.ExecuteNonQuery()
                End Using
            Catch ex As Exception
                ' ログイン自体は成功させたいので握りつぶす
            End Try
        End Using

    End Sub

    ' 担当者が1件も無いときに初期管理者を作る
    Public Shared Sub CreateDefaultAdministrator()

        Dim salt As String = NewPasswordSalt()

        Dim target As New [Operator]
        target.OperatorCode = "admin"
        target.OperatorName = "管理者"
        target.PasswordSalt = salt
        target.PasswordHash = HashPassword("admin", salt)
        target.RoleId = CInt(OperatorRole.Administrator).ToString
        target.OrderNo = "1"
        target.UnusedFlag = "0"

        Save(target)

    End Sub

End Class

#End Region

#Region "調整理由"

Public Class AjustType
    Public Property AjustTypeId As String
    Public Property AjustTypeName As String
    Public Property OrderNo As String
    Public Property UnusedFlag As String
End Class

Public Class AjustTypes

    Public Shared Function GetAll(Optional where As String = Nothing, Optional orderBy As String = Nothing) As List(Of AjustType)

        Dim result As New List(Of AjustType)

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Try
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String
                    SQLString = "select"
                    SQLString &= " AjustTypes.*"
                    SQLString &= " from AjustTypes"
                    SQLString &= " where AjustTypeId is not null"
                    If Not String.IsNullOrEmpty(where) Then
                        SQLString &= " and " & where
                    End If
                    If Not String.IsNullOrEmpty(orderBy) Then
                        SQLString &= " order by " & orderBy
                    Else
                        SQLString &= " order by OrderNo"
                    End If
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        While dr.Read
                            Dim AjustType As AjustType = Mapping(dr)
                            result.Add(AjustType)
                        End While
                    End Using
                End Using
            Catch ex As Exception
                SysMsgBox(ex.Message, "", Reflection.MethodBase.GetCurrentMethod.Name)
                Throw
            End Try
        End Using

        Return result

    End Function

    Public Shared Function GetById(id As Integer) As AjustType

        Dim result As New AjustType

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Try
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String
                    SQLString = "select"
                    SQLString &= " AjustTypes.*"
                    SQLString &= " from AjustTypes"
                    SQLString &= " where AjustTypeId = @AjustTypeId"
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    cmd.Parameters.AddWithValue("@AjustTypeId", NpgsqlDbType.Integer, id)
                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        If dr.HasRows Then
                            If dr.Read() Then result = Mapping(dr)
                        Else
                            result = Nothing
                        End If
                    End Using
                End Using
            Catch ex As Exception
                SysMsgBox(ex.Message, "", Reflection.MethodBase.GetCurrentMethod.Name)
                Throw
            End Try
        End Using

        Return result

    End Function

    Private Shared Function Mapping(dr As NpgsqlDataReader) As AjustType

        Dim result As New AjustType

        result.AjustTypeId = dr("AjustTypeId").ToString
        result.AjustTypeName = dr("AjustTypeName").ToString
        result.OrderNo = dr("OrderNo").ToString

        Return result

    End Function

    Public Shared Sub BindComboBox(targetComboBox As Object, Optional addAjustEmpty As Boolean = False, Optional where As String = "")

        Dim dtAjustTypes As New DataTable()
        dtAjustTypes.Columns.Add("AjustTypeId", GetType(String))
        dtAjustTypes.Columns.Add("AjustTypeName", GetType(String))

        If addAjustEmpty Then dtAjustTypes.Rows.Add(DBNull.Value, "")
        Dim AjustTypeList As List(Of AjustType) = AjustTypes.GetAll(where)
        For Each AjustType As AjustType In AjustTypeList
            dtAjustTypes.Rows.Add(AjustType.AjustTypeId, AjustType.AjustTypeName)
        Next
        dtAjustTypes.AcceptChanges()

        With targetComboBox
            .DisplayMember = "AjustTypeName"
            .ValueMember = "AjustTypeId"
            .DataSource = dtAjustTypes
        End With

    End Sub

End Class

#End Region

#Region "発注"

Public Class Order
    Public Property OrderId As String
    Public Property OrderDate As String
    Public Property ArrivalPlanDate As String
    Public Property Supplier As New Supplier
    Public Property OrderDetails As New List(Of OrderDetail)
End Class

Public Class Orders

    Public Shared Function GetById(id As Integer) As Order

        Dim result As Order = Nothing

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Try
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String
                    SQLString = "select"
                    SQLString &= " Orders.*"
                    SQLString &= " from Orders"
                    SQLString &= " where OrderId = @OrderId"
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    cmd.Parameters.AddWithValue("@OrderId", NpgsqlDbType.Integer, id)
                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        If dr.Read() Then result = Mapping(dr)
                    End Using
                End Using

                If result IsNot Nothing Then
                    result.OrderDetails = OrderDetails.GetByOrderId(result.OrderId)
                End If
            Catch ex As Exception
                SysMsgBox(ex.Message, "", Reflection.MethodBase.GetCurrentMethod.Name)
                Throw
            End Try
        End Using

        Return result

    End Function

    Public Shared Function GetByKey(orderDate As String, supplierId As String) As Order

        Dim result As Order = Nothing

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Try
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String
                    SQLString = "select"
                    SQLString &= " Orders.*"
                    SQLString &= " from Orders"
                    SQLString &= " where OrderDate = @OrderDate"
                    SQLString &= " and SupplierId = @SupplierId"
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    cmd.Parameters.AddWithValue("@OrderDate", NpgsqlDbType.Date, CDate(orderDate).Date)
                    cmd.Parameters.AddWithValue("@SupplierId", NpgsqlDbType.Integer, CInt(supplierId))
                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        If dr.Read() Then result = Mapping(dr)
                    End Using
                End Using

                If result IsNot Nothing Then
                    result.OrderDetails = OrderDetails.GetByOrderId(result.OrderId)
                End If
            Catch ex As Exception
                SysMsgBox(ex.Message, "", Reflection.MethodBase.GetCurrentMethod.Name)
                Throw
            End Try
        End Using

        Return result

    End Function

    Private Shared Function Mapping(dr As NpgsqlDataReader) As Order

        Dim result As New Order

        result.OrderId = dr("OrderId").ToString
        result.OrderDate = dr("OrderDate").ToString
        result.ArrivalPlanDate = dr("ArrivalPlanDate").ToString
        result.Supplier = Suppliers.GetById(dr("SupplierId").ToString)

        Return result

    End Function

    Public Shared Function Save(order As Order) As Order

        Dim nowDate As Date = Now

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Using trans As NpgsqlTransaction = cn.BeginTransaction()
                Try
                    Using cmd As NpgsqlCommand = cn.CreateCommand()
                        cmd.Transaction = trans

                        Dim SQLString As String
                        If String.IsNullOrEmpty(order.OrderId) Then
                            SQLString = "insert into Orders ("
                            SQLString &= " OrderDate"
                            SQLString &= ",ArrivalPlanDate"
                            SQLString &= ",SupplierId"
                            SQLString &= ",RegistDate"
                            SQLString &= ",ModifyDate"
                            SQLString &= " ) values ("
                            SQLString &= " @OrderDate"
                            SQLString &= ",@ArrivalPlanDate"
                            SQLString &= ",@SupplierId"
                            SQLString &= ",@RegistDate"
                            SQLString &= ",@ModifyDate"
                            SQLString &= ") returning OrderId"
                            cmd.CommandText = SQLString
                            cmd.Parameters.Clear()
                            cmd.Parameters.AddWithValue("@OrderDate", NpgsqlDbType.Date, CDate(order.OrderDate).Date)
                            cmd.Parameters.AddWithValue("@ArrivalPlanDate", NpgsqlDbType.Date, CDate(order.ArrivalPlanDate).Date)
                            cmd.Parameters.AddWithValue("@SupplierId", NpgsqlDbType.Integer, CInt(order.Supplier.SupplierId))
                            cmd.Parameters.AddWithValue("@RegistDate", NpgsqlDbType.Timestamp, nowDate)
                            cmd.Parameters.AddWithValue("@ModifyDate", NpgsqlDbType.Timestamp, nowDate)
                            order.OrderId = CInt(cmd.ExecuteScalar()).ToString()
                        Else
                            SQLString = "update Orders set"
                            SQLString &= " OrderDate = @OrderDate"
                            SQLString &= ",ArrivalPlanDate = @ArrivalPlanDate"
                            SQLString &= ",SupplierId = @SupplierId"
                            SQLString &= ",ModifyDate = @ModifyDate"
                            SQLString &= " where OrderId = @OrderId"
                            cmd.CommandText = SQLString
                            cmd.Parameters.Clear()
                            cmd.Parameters.AddWithValue("@OrderDate", NpgsqlDbType.Date, CDate(order.OrderDate).Date)
                            cmd.Parameters.AddWithValue("@ArrivalPlanDate", NpgsqlDbType.Date, CDate(order.ArrivalPlanDate).Date)
                            cmd.Parameters.AddWithValue("@SupplierId", NpgsqlDbType.Integer, CInt(order.Supplier.SupplierId))
                            cmd.Parameters.AddWithValue("@ModifyDate", NpgsqlDbType.Timestamp, nowDate)
                            cmd.Parameters.AddWithValue("@OrderId", NpgsqlDbType.Integer, CInt(order.OrderId))
                            cmd.ExecuteNonQuery()
                        End If

                        SQLString = "delete from OrderDetails"
                        SQLString &= " where OrderId = @OrderId"
                        cmd.CommandText = SQLString
                        cmd.Parameters.Clear()
                        cmd.Parameters.AddWithValue("@OrderId", NpgsqlDbType.Integer, CInt(order.OrderId))
                        cmd.ExecuteNonQuery()

                        For Each detail As OrderDetail In order.OrderDetails
                            SQLString = "insert into OrderDetails ("
                            SQLString &= " OrderId"
                            SQLString &= ",DetailNo"
                            SQLString &= ",ItemId"
                            SQLString &= ",NetWeight"
                            SQLString &= ",NetWeightUnitTypeId"
                            SQLString &= ",PerCase"
                            SQLString &= ",Quantity"
                            SQLString &= ",UnitTypeId"
                            SQLString &= ",UnitPrice"
                            SQLString &= ",Price"
                            SQLString &= ",RegistDate"
                            SQLString &= ",ModifyDate"
                            SQLString &= ") values ("
                            SQLString &= " @OrderId"
                            SQLString &= ",@DetailNo"
                            SQLString &= ",@ItemId"
                            SQLString &= ",@NetWeight"
                            SQLString &= ",@NetWeightUnitTypeId"
                            SQLString &= ",@PerCase"
                            SQLString &= ",@Quantity"
                            SQLString &= ",@UnitTypeId"
                            SQLString &= ",@UnitPrice"
                            SQLString &= ",@Price"
                            SQLString &= ",@RegistDate"
                            SQLString &= ",@ModifyDate"
                            SQLString &= ") returning OrderDetailId"
                            cmd.CommandText = SQLString
                            cmd.Parameters.Clear()
                            cmd.Parameters.AddWithValue("@OrderId", NpgsqlDbType.Integer, CInt(order.OrderId))
                            cmd.Parameters.AddWithValue("@DetailNo", NpgsqlDbType.Integer, CInt(detail.DetailNo))
                            cmd.Parameters.AddWithValue("@ItemId", NpgsqlDbType.Integer, CInt(detail.Item.ItemId))
                            If Not IsNothing(Nz(detail.NetWeight, Nothing)) Then
                                cmd.Parameters.AddWithValue("@NetWeight", NpgsqlDbType.Double, CDbl(detail.NetWeight))
                            Else
                                cmd.Parameters.AddWithValue("@NetWeight", NpgsqlDbType.Double, DBNull.Value)
                            End If
                            If Not IsNothing(Nz(detail.NetWeightUnitType.UnitTypeId, Nothing)) Then
                                cmd.Parameters.AddWithValue("@NetWeightUnitTypeId", NpgsqlDbType.Integer, CInt(detail.NetWeightUnitType.UnitTypeId))
                            Else
                                cmd.Parameters.AddWithValue("@NetWeightUnitTypeId", NpgsqlDbType.Integer, DBNull.Value)
                            End If
                            If Not IsNothing(Nz(detail.PerCase, Nothing)) Then
                                cmd.Parameters.AddWithValue("@PerCase", NpgsqlDbType.Double, CDbl(detail.PerCase))
                            Else
                                cmd.Parameters.AddWithValue("@PerCase", NpgsqlDbType.Double, DBNull.Value)
                            End If
                            cmd.Parameters.AddWithValue("@Quantity", NpgsqlDbType.Double, CDbl(detail.Quantity))
                            cmd.Parameters.AddWithValue("@UnitTypeId", NpgsqlDbType.Integer, CInt(detail.UnitType.UnitTypeId))
                            If Not IsNothing(Nz(detail.UnitPrice, Nothing)) Then
                                cmd.Parameters.AddWithValue("@UnitPrice", NpgsqlDbType.Double, CDbl(detail.UnitPrice))
                            Else
                                cmd.Parameters.AddWithValue("@UnitPrice", NpgsqlDbType.Double, DBNull.Value)
                            End If
                            If Not IsNothing(Nz(detail.Price, Nothing)) Then
                                cmd.Parameters.AddWithValue("@Price", NpgsqlDbType.Double, CDbl(detail.Price))
                            Else
                                cmd.Parameters.AddWithValue("@Price", NpgsqlDbType.Double, DBNull.Value)
                            End If
                            cmd.Parameters.AddWithValue("@RegistDate", NpgsqlDbType.Timestamp, nowDate)
                            cmd.Parameters.AddWithValue("@ModifyDate", NpgsqlDbType.Timestamp, nowDate)
                            detail.OrderDetailId = CInt(cmd.ExecuteScalar()).ToString()
                        Next
                    End Using

                    trans.Commit()
                Catch ex As Exception
                    trans.Rollback()
                    Throw
                End Try
            End Using
        End Using

        Return order

    End Function

    Public Shared Sub Delete(order As Order)

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Using trans As NpgsqlTransaction = cn.BeginTransaction()
                Try
                    Using cmd As NpgsqlCommand = cn.CreateCommand()
                        cmd.Transaction = trans

                        Dim SQLString As String
                        SQLString = "delete from OrderDetails"
                        SQLString &= " where OrderId = @OrderId"
                        cmd.CommandText = SQLString
                        cmd.Parameters.Clear()
                        cmd.Parameters.AddWithValue("@OrderId", NpgsqlDbType.Integer, CInt(order.OrderId))
                        cmd.ExecuteNonQuery()

                        SQLString = "delete from Orders"
                        SQLString &= " where OrderId = @OrderId"
                        cmd.CommandText = SQLString
                        cmd.Parameters.Clear()
                        cmd.Parameters.AddWithValue("@OrderId", NpgsqlDbType.Integer, CInt(order.OrderId))
                        cmd.ExecuteNonQuery()
                    End Using

                    trans.Commit()
                Catch ex As Exception
                    trans.Rollback()
                    Throw
                End Try
            End Using
        End Using

    End Sub

End Class

Public Class OrderDetail
    Public Property OrderDetailId As String
    Public Property OrderId As String
    Public Property DetailNo As String
    Public Property Item As New Item
    Public Property NetWeight As String
    Public Property NetWeightUnitType As New UnitType
    Public Property PerCase As String
    Public Property Quantity As String
    Public Property UnitType As New UnitType
    Public Property UnitPrice As String
    Public Property Price As String
End Class

Public Class OrderDetails

    Public Shared Function GetByOrderId(orderId As Integer) As List(Of OrderDetail)

        Dim result As New List(Of OrderDetail)

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Try
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String
                    SQLString = "select"
                    SQLString &= " OrderDetails.*"
                    SQLString &= " from OrderDetails"
                    SQLString &= " where OrderId = @OrderId"
                    SQLString &= " order by"
                    SQLString &= " DetailNo"
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    cmd.Parameters.AddWithValue("@OrderId", NpgsqlDbType.Integer, orderId)
                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        While dr.Read()
                            Dim orderDetail As OrderDetail = Mapping(dr)
                            result.Add(orderDetail)
                        End While
                    End Using
                End Using
            Catch ex As Exception
                SysMsgBox(ex.Message, "", Reflection.MethodBase.GetCurrentMethod.Name)
                Throw
            End Try
        End Using

        Return result

    End Function

    Private Shared Function Mapping(dr As NpgsqlDataReader) As OrderDetail

        Dim result As New OrderDetail

        result.OrderDetailId = dr("OrderDetailId").ToString
        result.OrderId = dr("OrderId").ToString
        result.DetailNo = dr("DetailNo").ToString
        result.Item = Items.GetById(dr("ItemId").ToString)
        result.NetWeight = Nz(dr("NetWeight"), 0)
        If Not String.IsNullOrEmpty(dr("NetWeightUnitTypeId").ToString) Then
            result.NetWeightUnitType = UnitTypes.GetById(dr("NetWeightUnitTypeId").ToString)
        End If
        result.PerCase = Nz(dr("PerCase"), 0)
        result.Quantity = Nz(dr("Quantity"), 0)
        result.UnitType = UnitTypes.GetById(dr("UnitTypeId").ToString)
        result.UnitPrice = Nz(dr("UnitPrice"), 0)
        result.Price = Nz(dr("Price"), 0)

        Return result

    End Function

End Class

#End Region

#Region "入荷"

Public Class Arrival
    Public Property ArrivalId As String
    Public Property ArrivalDate As String
    Public Property Supplier As New Supplier
    Public Property OrderId As String
    Public Property ArrivalDetails As New List(Of ArrivalDetail)
End Class

Public Class Arrivals

    Public Shared Function GetById(id As Integer) As Arrival

        Dim result As Arrival = Nothing

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Try
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String
                    SQLString = "select"
                    SQLString &= " Arrivals.*"
                    SQLString &= " from Arrivals"
                    SQLString &= " where ArrivalId = @ArrivalId"
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    cmd.Parameters.AddWithValue("@ArrivalId", NpgsqlDbType.Integer, id)
                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        If dr.Read() Then result = Mapping(dr)
                    End Using
                End Using

                If result IsNot Nothing Then
                    result.ArrivalDetails = ArrivalDetails.GetByArrivalId(result.ArrivalId)
                End If
            Catch ex As Exception
                SysMsgBox(ex.Message, "", Reflection.MethodBase.GetCurrentMethod.Name)
                Throw
            End Try
        End Using

        Return result

    End Function

    Public Shared Function GetByKey(arrivalDate As String, supplierId As String) As Arrival

        Dim result As Arrival = Nothing

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Try
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String
                    SQLString = "select"
                    SQLString &= " Arrivals.*"
                    SQLString &= " from Arrivals"
                    SQLString &= " where ArrivalDate = @ArrivalDate"
                    SQLString &= " and SupplierId = @SupplierId"
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    cmd.Parameters.AddWithValue("@ArrivalDate", NpgsqlDbType.Date, CDate(arrivalDate).Date)
                    cmd.Parameters.AddWithValue("@SupplierId", NpgsqlDbType.Integer, CInt(supplierId))
                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        If dr.Read() Then result = Mapping(dr)
                    End Using
                End Using

                If result IsNot Nothing Then
                    result.ArrivalDetails = ArrivalDetails.GetByArrivalId(result.ArrivalId)
                End If
            Catch ex As Exception
                SysMsgBox(ex.Message, "", Reflection.MethodBase.GetCurrentMethod.Name)
                Throw
            End Try
        End Using

        Return result

    End Function

    Private Shared Function Mapping(dr As NpgsqlDataReader) As Arrival

        Dim result As New Arrival

        result.ArrivalId = dr("ArrivalId").ToString
        result.ArrivalDate = dr("ArrivalDate").ToString
        result.Supplier = Suppliers.GetById(dr("SupplierId").ToString)
        result.OrderId = dr("OrderId").ToString

        Return result

    End Function

    Public Shared Function Save(arrival As Arrival) As Arrival

        Dim nowDate As Date = Now

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Using trans As NpgsqlTransaction = cn.BeginTransaction()
                Try
                    Using cmd As NpgsqlCommand = cn.CreateCommand()
                        cmd.Transaction = trans

                        Dim SQLString As String
                        If String.IsNullOrEmpty(arrival.ArrivalId) Then
                            SQLString = "insert into Arrivals ("
                            SQLString &= " ArrivalDate"
                            SQLString &= ",SupplierId"
                            SQLString &= ",OrderId"
                            SQLString &= ",RegistDate"
                            SQLString &= ",ModifyDate"
                            SQLString &= " ) values ("
                            SQLString &= " @ArrivalDate"
                            SQLString &= ",@SupplierId"
                            SQLString &= ",@OrderId"
                            SQLString &= ",@RegistDate"
                            SQLString &= ",@ModifyDate"
                            SQLString &= ") returning ArrivalId"
                            cmd.CommandText = SQLString
                            cmd.Parameters.Clear()
                            cmd.Parameters.AddWithValue("@ArrivalDate", NpgsqlDbType.Date, CDate(arrival.ArrivalDate).Date)
                            cmd.Parameters.AddWithValue("@SupplierId", NpgsqlDbType.Integer, CInt(arrival.Supplier.SupplierId))
                            If Not IsNothing(Nz(arrival.OrderId, Nothing)) Then
                                cmd.Parameters.AddWithValue("@OrderId", NpgsqlDbType.Integer, CInt(arrival.OrderId))
                            Else
                                cmd.Parameters.AddWithValue("@OrderId", NpgsqlDbType.Integer, DBNull.Value)
                            End If
                            cmd.Parameters.AddWithValue("@RegistDate", NpgsqlDbType.Timestamp, nowDate)
                            cmd.Parameters.AddWithValue("@ModifyDate", NpgsqlDbType.Timestamp, nowDate)
                            arrival.ArrivalId = CInt(cmd.ExecuteScalar()).ToString()
                        Else
                            SQLString = "update Arrivals set"
                            SQLString &= " ArrivalDate = @ArrivalDate"
                            SQLString &= ",SupplierId = @SupplierId"
                            SQLString &= ",OrderId = @OrderId"
                            SQLString &= ",ModifyDate = @ModifyDate"
                            SQLString &= " where ArrivalId = @ArrivalId"
                            cmd.CommandText = SQLString
                            cmd.Parameters.Clear()
                            cmd.Parameters.AddWithValue("@ArrivalDate", NpgsqlDbType.Date, CDate(arrival.ArrivalDate).Date)
                            cmd.Parameters.AddWithValue("@SupplierId", NpgsqlDbType.Integer, CInt(arrival.Supplier.SupplierId))
                            If Not IsNothing(Nz(arrival.OrderId, Nothing)) Then
                                cmd.Parameters.AddWithValue("@OrderId", NpgsqlDbType.Integer, CInt(arrival.OrderId))
                            Else
                                cmd.Parameters.AddWithValue("@OrderId", NpgsqlDbType.Integer, DBNull.Value)
                            End If
                            cmd.Parameters.AddWithValue("@ModifyDate", NpgsqlDbType.Timestamp, nowDate)
                            cmd.Parameters.AddWithValue("@ArrivalId", NpgsqlDbType.Integer, CInt(arrival.ArrivalId))
                            cmd.ExecuteNonQuery()
                        End If

                        If Not String.IsNullOrEmpty(arrival.ArrivalId) Then
                            ' 残すべき明細ID(新規行はIDが空なので対象外)
                            Dim keepIds As New List(Of String)
                            For Each detail As ArrivalDetail In arrival.ArrivalDetails
                                Dim id As Integer
                                If Integer.TryParse(detail.ArrivalDetailId, id) Then keepIds.Add(id.ToString())
                            Next

                            ' 明細IDが1件も取れないのは異常(隠し列の設定漏れ等)なので削除しない
                            If keepIds.Count > 0 OrElse arrival.ArrivalDetails.Count = 0 Then

                                Dim keepClause As String = String.Empty
                                If keepIds.Count > 0 Then
                                    keepClause = " and ArrivalDetails.ArrivalDetailId not in (" & String.Join(",", keepIds) & ")"
                                End If

                                ' 削除対象に払い出し済みが含まれていたら中断する
                                If HasConsumedDetails(cmd, arrival.ArrivalId, keepClause) Then
                                    Throw New ApplicationException("払い出しが始まっている明細が含まれているため削除できません。")
                                End If

                                ' 消えるロットを先に読んでおく
                                Dim removedIdList As New List(Of String)
                                Dim removedItemIdList As New List(Of String)
                                Dim removedWeightList As New List(Of Double)
                                Dim removedPriceList As New List(Of Object)
                                Dim removedDetailIdList As New List(Of String)

                                SQLString = "select"
                                SQLString &= " Inventories.InventoryId"
                                SQLString &= ",Inventories.ItemId"
                                SQLString &= ",Inventories.Weight"
                                SQLString &= ",Inventories.PricePerGram"
                                SQLString &= ",Inventories.ArrivalDetailId"
                                SQLString &= " from Inventories"
                                SQLString &= " where Inventories.ArrivalDetailId in ("
                                SQLString &= " select ArrivalDetails.ArrivalDetailId from ArrivalDetails"
                                SQLString &= " where ArrivalDetails.ArrivalId = @ArrivalId" & keepClause
                                SQLString &= ")"
                                cmd.CommandText = SQLString
                                cmd.Parameters.Clear()
                                cmd.Parameters.AddWithValue("@ArrivalId", NpgsqlDbType.Integer, CInt(arrival.ArrivalId))
                                Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                                    While dr.Read
                                        removedIdList.Add(dr("InventoryId").ToString)
                                        removedItemIdList.Add(dr("ItemId").ToString)
                                        removedWeightList.Add(CDbl(Nz(dr("Weight"), 0)))
                                        removedPriceList.Add(dr("PricePerGram"))
                                        removedDetailIdList.Add(dr("ArrivalDetailId").ToString)
                                    End While
                                End Using

                                ' 残すもの以外を削除
                                SQLString = "delete from Inventories"
                                SQLString &= " where ArrivalDetailId in ("
                                SQLString &= " select ArrivalDetails.ArrivalDetailId from ArrivalDetails"
                                SQLString &= " where ArrivalDetails.ArrivalId = @ArrivalId" & keepClause
                                SQLString &= ")"
                                cmd.CommandText = SQLString
                                cmd.Parameters.Clear()
                                cmd.Parameters.AddWithValue("@ArrivalId", NpgsqlDbType.Integer, CInt(arrival.ArrivalId))
                                cmd.ExecuteNonQuery()

                                SQLString = "delete from ArrivalDetails"
                                SQLString &= " where ArrivalId = @ArrivalId" & keepClause
                                cmd.CommandText = SQLString
                                cmd.Parameters.Clear()
                                cmd.Parameters.AddWithValue("@ArrivalId", NpgsqlDbType.Integer, CInt(arrival.ArrivalId))
                                cmd.ExecuteNonQuery()

                                ' 消したロットを記録する
                                For i As Integer = 0 To removedIdList.Count - 1
                                    InventoryLogs.Write(cmd, InventoryLogStatus.ArrivalCancel,
                                                        removedItemIdList(i), Nothing,
                                                        -removedWeightList(i),
                                                        removedPriceList(i), removedDetailIdList(i), nowDate)
                                Next
                            End If
                        End If

                        For Each detail As ArrivalDetail In arrival.ArrivalDetails

                            ' 払い出し済みの明細は一切触らない
                            If Not String.IsNullOrEmpty(detail.ArrivalDetailId) AndAlso IsConsumed(cmd, detail.ArrivalDetailId) Then
                                Continue For
                            End If

                            If String.IsNullOrEmpty(detail.ArrivalDetailId) Then
                                SQLString = "insert into ArrivalDetails ("
                                SQLString &= " ArrivalId"
                                SQLString &= ",DetailNo"
                                SQLString &= ",ItemId"
                                SQLString &= ",NetWeight"
                                SQLString &= ",NetWeightUnitTypeId"
                                SQLString &= ",PerCase"
                                SQLString &= ",Quantity"
                                SQLString &= ",UnitTypeId"
                                SQLString &= ",UnitPrice"
                                SQLString &= ",Price"
                                SQLString &= ",ExpiryDate"
                                SQLString &= ",PieceUnitTypeId"
                                SQLString &= ",PieceWeight"
                                SQLString &= ",Weight"
                                SQLString &= ",PricePerGram"
                                SQLString &= ",RegistDate"
                                SQLString &= ",ModifyDate"
                                SQLString &= ") values ("
                                SQLString &= " @ArrivalId"
                                SQLString &= ",@DetailNo"
                                SQLString &= ",@ItemId"
                                SQLString &= ",@NetWeight"
                                SQLString &= ",@NetWeightUnitTypeId"
                                SQLString &= ",@PerCase"
                                SQLString &= ",@Quantity"
                                SQLString &= ",@UnitTypeId"
                                SQLString &= ",@UnitPrice"
                                SQLString &= ",@Price"
                                SQLString &= ",@ExpiryDate"
                                SQLString &= ",@PieceUnitTypeId"
                                SQLString &= ",@PieceWeight"
                                SQLString &= ",@Weight"
                                SQLString &= ",@PricePerGram"
                                SQLString &= ",@RegistDate"
                                SQLString &= ",@ModifyDate"
                                SQLString &= ") returning ArrivalDetailId"
                                cmd.CommandText = SQLString
                                cmd.Parameters.Clear()
                                cmd.Parameters.AddWithValue("@ArrivalId", NpgsqlDbType.Integer, CInt(arrival.ArrivalId))
                                cmd.Parameters.AddWithValue("@DetailNo", NpgsqlDbType.Integer, CInt(detail.DetailNo))
                                cmd.Parameters.AddWithValue("@ItemId", NpgsqlDbType.Integer, CInt(detail.Item.ItemId))
                                If Not IsNothing(Nz(detail.NetWeight, Nothing)) Then
                                    cmd.Parameters.AddWithValue("@NetWeight", NpgsqlDbType.Double, CDbl(detail.NetWeight))
                                Else
                                    cmd.Parameters.AddWithValue("@NetWeight", NpgsqlDbType.Double, DBNull.Value)
                                End If
                                If Not IsNothing(Nz(detail.NetWeightUnitType.UnitTypeId, Nothing)) Then
                                    cmd.Parameters.AddWithValue("@NetWeightUnitTypeId", NpgsqlDbType.Integer, CInt(detail.NetWeightUnitType.UnitTypeId))
                                Else
                                    cmd.Parameters.AddWithValue("@NetWeightUnitTypeId", NpgsqlDbType.Integer, DBNull.Value)
                                End If
                                If Not IsNothing(Nz(detail.PerCase, Nothing)) Then
                                    cmd.Parameters.AddWithValue("@PerCase", NpgsqlDbType.Double, CDbl(detail.PerCase))
                                Else
                                    cmd.Parameters.AddWithValue("@PerCase", NpgsqlDbType.Double, DBNull.Value)
                                End If
                                If Not IsNothing(Nz(detail.Quantity, Nothing)) Then
                                    cmd.Parameters.AddWithValue("@Quantity", NpgsqlDbType.Double, CDbl(detail.Quantity))
                                Else
                                    cmd.Parameters.AddWithValue("@Quantity", NpgsqlDbType.Double, DBNull.Value)
                                End If
                                cmd.Parameters.AddWithValue("@UnitTypeId", NpgsqlDbType.Integer, CInt(detail.UnitType.UnitTypeId))
                                If Not IsNothing(Nz(detail.UnitPrice, Nothing)) Then
                                    cmd.Parameters.AddWithValue("@UnitPrice", NpgsqlDbType.Double, CDbl(detail.UnitPrice))
                                Else
                                    cmd.Parameters.AddWithValue("@UnitPrice", NpgsqlDbType.Double, DBNull.Value)
                                End If
                                If Not IsNothing(Nz(detail.Price, Nothing)) Then
                                    cmd.Parameters.AddWithValue("@Price", NpgsqlDbType.Double, CDbl(detail.Price))
                                Else
                                    cmd.Parameters.AddWithValue("@Price", NpgsqlDbType.Double, DBNull.Value)
                                End If
                                If Not IsNothing(Nz(detail.ExpiryDate, Nothing)) Then
                                    cmd.Parameters.AddWithValue("@ExpiryDate", NpgsqlDbType.Date, CDate(detail.ExpiryDate).Date)
                                Else
                                    cmd.Parameters.AddWithValue("@ExpiryDate", NpgsqlDbType.Date, DBNull.Value)
                                End If
                                If Not IsNothing(Nz(detail.PieceUnitType.UnitTypeId, Nothing)) Then
                                    cmd.Parameters.AddWithValue("@PieceUnitTypeId", NpgsqlDbType.Integer, CInt(detail.PieceUnitType.UnitTypeId))
                                Else
                                    cmd.Parameters.AddWithValue("@PieceUnitTypeId", NpgsqlDbType.Integer, DBNull.Value)
                                End If
                                If Not IsNothing(Nz(detail.PieceWeight, Nothing)) Then
                                    cmd.Parameters.AddWithValue("@PieceWeight", NpgsqlDbType.Double, CDbl(detail.PieceWeight))
                                Else
                                    cmd.Parameters.AddWithValue("@PieceWeight", NpgsqlDbType.Double, DBNull.Value)
                                End If
                                If Not IsNothing(Nz(detail.Weight, Nothing)) Then
                                    cmd.Parameters.AddWithValue("@Weight", NpgsqlDbType.Double, CDbl(detail.Weight))
                                Else
                                    cmd.Parameters.AddWithValue("@Weight", NpgsqlDbType.Double, DBNull.Value)
                                End If
                                If Not IsNothing(Nz(detail.PricePerGram, Nothing)) Then
                                    cmd.Parameters.AddWithValue("@PricePerGram", NpgsqlDbType.Double, CDbl(detail.PricePerGram))
                                Else
                                    cmd.Parameters.AddWithValue("@PricePerGram", NpgsqlDbType.Double, DBNull.Value)
                                End If
                                cmd.Parameters.AddWithValue("@RegistDate", NpgsqlDbType.Timestamp, nowDate)
                                cmd.Parameters.AddWithValue("@ModifyDate", NpgsqlDbType.Timestamp, nowDate)
                                detail.ArrivalDetailId = CInt(cmd.ExecuteScalar()).ToString()
                            Else
                                SQLString = "update ArrivalDetails set"
                                SQLString &= " DetailNo = @DetailNo"
                                SQLString &= ",ItemId = @ItemId"
                                SQLString &= ",NetWeight = @NetWeight"
                                SQLString &= ",NetWeightUnitTypeId = @NetWeightUnitTypeId"
                                SQLString &= ",PerCase = @PerCase"
                                SQLString &= ",Quantity = @Quantity"
                                SQLString &= ",UnitTypeId = @UnitTypeId"
                                SQLString &= ",UnitPrice = @UnitPrice"
                                SQLString &= ",Price = @Price"
                                SQLString &= ",ExpiryDate = @ExpiryDate"
                                SQLString &= ",PieceUnitTypeId = @PieceUnitTypeId"
                                SQLString &= ",PieceWeight = @PieceWeight"
                                SQLString &= ",Weight = @Weight"
                                SQLString &= ",PricePerGram = @PricePerGram"
                                SQLString &= ",ModifyDate = @ModifyDate"
                                SQLString &= " where ArrivalDetailId = @ArrivalDetailId"
                                cmd.CommandText = SQLString
                                cmd.Parameters.Clear()
                                cmd.Parameters.AddWithValue("@DetailNo", NpgsqlDbType.Integer, CInt(detail.DetailNo))
                                cmd.Parameters.AddWithValue("@ItemId", NpgsqlDbType.Integer, CInt(detail.Item.ItemId))
                                If Not IsNothing(Nz(detail.NetWeight, Nothing)) Then
                                    cmd.Parameters.AddWithValue("@NetWeight", NpgsqlDbType.Double, CDbl(detail.NetWeight))
                                Else
                                    cmd.Parameters.AddWithValue("@NetWeight", NpgsqlDbType.Double, DBNull.Value)
                                End If
                                If Not IsNothing(Nz(detail.NetWeightUnitType.UnitTypeId, Nothing)) Then
                                    cmd.Parameters.AddWithValue("@NetWeightUnitTypeId", NpgsqlDbType.Integer, CInt(detail.NetWeightUnitType.UnitTypeId))
                                Else
                                    cmd.Parameters.AddWithValue("@NetWeightUnitTypeId", NpgsqlDbType.Integer, DBNull.Value)
                                End If
                                If Not IsNothing(Nz(detail.PerCase, Nothing)) Then
                                    cmd.Parameters.AddWithValue("@PerCase", NpgsqlDbType.Double, CDbl(detail.PerCase))
                                Else
                                    cmd.Parameters.AddWithValue("@PerCase", NpgsqlDbType.Double, DBNull.Value)
                                End If
                                If Not IsNothing(Nz(detail.Quantity, Nothing)) Then
                                    cmd.Parameters.AddWithValue("@Quantity", NpgsqlDbType.Double, CDbl(detail.Quantity))
                                Else
                                    cmd.Parameters.AddWithValue("@Quantity", NpgsqlDbType.Double, DBNull.Value)
                                End If
                                cmd.Parameters.AddWithValue("@UnitTypeId", NpgsqlDbType.Integer, CInt(detail.UnitType.UnitTypeId))
                                If Not IsNothing(Nz(detail.UnitPrice, Nothing)) Then
                                    cmd.Parameters.AddWithValue("@UnitPrice", NpgsqlDbType.Double, CDbl(detail.UnitPrice))
                                Else
                                    cmd.Parameters.AddWithValue("@UnitPrice", NpgsqlDbType.Double, DBNull.Value)
                                End If
                                If Not IsNothing(Nz(detail.Price, Nothing)) Then
                                    cmd.Parameters.AddWithValue("@Price", NpgsqlDbType.Double, CDbl(detail.Price))
                                Else
                                    cmd.Parameters.AddWithValue("@Price", NpgsqlDbType.Double, DBNull.Value)
                                End If
                                If Not IsNothing(Nz(detail.ExpiryDate, Nothing)) Then
                                    cmd.Parameters.AddWithValue("@ExpiryDate", NpgsqlDbType.Date, CDate(detail.ExpiryDate).Date)
                                Else
                                    cmd.Parameters.AddWithValue("@ExpiryDate", NpgsqlDbType.Date, DBNull.Value)
                                End If
                                If Not IsNothing(Nz(detail.PieceUnitType.UnitTypeId, Nothing)) Then
                                    cmd.Parameters.AddWithValue("@PieceUnitTypeId", NpgsqlDbType.Integer, CInt(detail.PieceUnitType.UnitTypeId))
                                Else
                                    cmd.Parameters.AddWithValue("@PieceUnitTypeId", NpgsqlDbType.Integer, DBNull.Value)
                                End If
                                If Not IsNothing(Nz(detail.PieceWeight, Nothing)) Then
                                    cmd.Parameters.AddWithValue("@PieceWeight", NpgsqlDbType.Double, CDbl(detail.PieceWeight))
                                Else
                                    cmd.Parameters.AddWithValue("@PieceWeight", NpgsqlDbType.Double, DBNull.Value)
                                End If
                                If Not IsNothing(Nz(detail.Weight, Nothing)) Then
                                    cmd.Parameters.AddWithValue("@Weight", NpgsqlDbType.Double, CDbl(detail.Weight))
                                Else
                                    cmd.Parameters.AddWithValue("@Weight", NpgsqlDbType.Double, DBNull.Value)
                                End If
                                If Not IsNothing(Nz(detail.PricePerGram, Nothing)) Then
                                    cmd.Parameters.AddWithValue("@PricePerGram", NpgsqlDbType.Double, CDbl(detail.PricePerGram))
                                Else
                                    cmd.Parameters.AddWithValue("@PricePerGram", NpgsqlDbType.Double, DBNull.Value)
                                End If
                                cmd.Parameters.AddWithValue("@ModifyDate", NpgsqlDbType.Timestamp, nowDate)
                                cmd.Parameters.AddWithValue("@ArrivalDetailId", NpgsqlDbType.Integer, CInt(detail.ArrivalDetailId))
                                cmd.ExecuteNonQuery()
                            End If

                            ' 記録用に更新前のロットを見ておく
                            Dim beforeInventoryId As String = String.Empty
                            Dim beforeItemId As String = String.Empty
                            Dim beforeWeight As Double = 0
                            Dim beforePricePerGram As Double = 0
                            SQLString = "select"
                            SQLString &= " InventoryId"
                            SQLString &= ",ItemId"
                            SQLString &= ",Weight"
                            SQLString &= ",PricePerGram"
                            SQLString &= " from Inventories"
                            SQLString &= " where ArrivalDetailId = @ArrivalDetailId"
                            cmd.CommandText = SQLString
                            cmd.Parameters.Clear()
                            cmd.Parameters.AddWithValue("@ArrivalDetailId", NpgsqlDbType.Integer, CInt(detail.ArrivalDetailId))
                            Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                                If dr.Read() Then
                                    beforeInventoryId = dr("InventoryId").ToString
                                    beforeItemId = dr("ItemId").ToString
                                    beforeWeight = CDbl(Nz(dr("Weight"), 0))
                                    beforePricePerGram = CDbl(Nz(dr("PricePerGram"), 0))
                                End If
                            End Using

                            ' まず更新を試み、対象が無ければ挿入する
                            SQLString = "update Inventories set"
                            SQLString &= " ItemId = @ItemId"
                            SQLString &= ",ArrivalDate = @ArrivalDate"
                            SQLString &= ",ExpiryDate = @ExpiryDate"
                            SQLString &= ",Quantity = @Quantity"
                            SQLString &= ",UnitTypeId = @UnitTypeId"
                            SQLString &= ",Weight = @Weight"
                            SQLString &= ",PricePerGram = @PricePerGram"
                            SQLString &= ",ModifyDate = @ModifyDate"
                            SQLString &= " where ArrivalDetailId = @ArrivalDetailId"
                            cmd.CommandText = SQLString
                            cmd.Parameters.Clear()
                            cmd.Parameters.AddWithValue("@ItemId", NpgsqlDbType.Integer, CInt(detail.Item.ItemId))
                            cmd.Parameters.AddWithValue("@ArrivalDate", NpgsqlDbType.Date, CDate(arrival.ArrivalDate).Date)
                            If Not IsNothing(Nz(detail.ExpiryDate, Nothing)) Then
                                cmd.Parameters.AddWithValue("@ExpiryDate", NpgsqlDbType.Date, CDate(detail.ExpiryDate).Date)
                            Else
                                cmd.Parameters.AddWithValue("@ExpiryDate", NpgsqlDbType.Date, DBNull.Value)
                            End If
                            If Not IsNothing(Nz(detail.Quantity, Nothing)) Then
                                cmd.Parameters.AddWithValue("@Quantity", NpgsqlDbType.Double, CDbl(detail.Quantity))
                            Else
                                cmd.Parameters.AddWithValue("@Quantity", NpgsqlDbType.Double, DBNull.Value)
                            End If
                            cmd.Parameters.AddWithValue("@UnitTypeId", NpgsqlDbType.Integer, CInt(detail.UnitType.UnitTypeId))
                            If Not IsNothing(Nz(detail.Weight, Nothing)) Then
                                cmd.Parameters.AddWithValue("@Weight", NpgsqlDbType.Double, CDbl(detail.Weight))
                            Else
                                cmd.Parameters.AddWithValue("@Weight", NpgsqlDbType.Double, DBNull.Value)
                            End If
                            If Not IsNothing(Nz(detail.PricePerGram, Nothing)) Then
                                cmd.Parameters.AddWithValue("@PricePerGram", NpgsqlDbType.Double, CDbl(detail.PricePerGram))
                            Else
                                cmd.Parameters.AddWithValue("@PricePerGram", NpgsqlDbType.Double, DBNull.Value)
                            End If
                            cmd.Parameters.AddWithValue("@ModifyDate", NpgsqlDbType.Timestamp, nowDate)
                            cmd.Parameters.AddWithValue("@ArrivalDetailId", NpgsqlDbType.Integer, CInt(detail.ArrivalDetailId))
                            Dim inventoryId As String
                            Dim updateCount As Integer = cmd.ExecuteNonQuery()
                            If updateCount = 0 Then
                                SQLString = "insert into Inventories ("
                                SQLString &= " ItemId"
                                SQLString &= ",ArrivalDate"
                                SQLString &= ",ExpiryDate"
                                SQLString &= ",Quantity"
                                SQLString &= ",UnitTypeId"
                                SQLString &= ",Weight"
                                SQLString &= ",PricePerGram"
                                SQLString &= ",ArrivalDetailId"
                                SQLString &= ",RegistDate"
                                SQLString &= ",ModifyDate"
                                SQLString &= ") values ("
                                SQLString &= " @ItemId"
                                SQLString &= ",@ArrivalDate"
                                SQLString &= ",@ExpiryDate"
                                SQLString &= ",@Quantity"
                                SQLString &= ",@UnitTypeId"
                                SQLString &= ",@Weight"
                                SQLString &= ",@PricePerGram"
                                SQLString &= ",@ArrivalDetailId"
                                SQLString &= ",@RegistDate"
                                SQLString &= ",@ModifyDate"
                                SQLString &= ") returning InventoryId"
                                cmd.CommandText = SQLString
                                cmd.Parameters.Clear()
                                cmd.Parameters.AddWithValue("@ItemId", NpgsqlDbType.Integer, CInt(detail.Item.ItemId))
                                cmd.Parameters.AddWithValue("@ArrivalDate", NpgsqlDbType.Date, CDate(arrival.ArrivalDate).Date)
                                If Not IsNothing(Nz(detail.ExpiryDate, Nothing)) Then
                                    cmd.Parameters.AddWithValue("@ExpiryDate", NpgsqlDbType.Date, CDate(detail.ExpiryDate).Date)
                                Else
                                    cmd.Parameters.AddWithValue("@ExpiryDate", NpgsqlDbType.Date, DBNull.Value)
                                End If
                                If Not IsNothing(Nz(detail.Quantity, Nothing)) Then
                                    cmd.Parameters.AddWithValue("@Quantity", NpgsqlDbType.Double, CDbl(detail.Quantity))
                                Else
                                    cmd.Parameters.AddWithValue("@Quantity", NpgsqlDbType.Double, DBNull.Value)
                                End If
                                cmd.Parameters.AddWithValue("@UnitTypeId", NpgsqlDbType.Integer, CInt(detail.UnitType.UnitTypeId))
                                If Not IsNothing(Nz(detail.Weight, Nothing)) Then
                                    cmd.Parameters.AddWithValue("@Weight", NpgsqlDbType.Double, CDbl(detail.Weight))
                                Else
                                    cmd.Parameters.AddWithValue("@Weight", NpgsqlDbType.Double, DBNull.Value)
                                End If
                                If Not IsNothing(Nz(detail.PricePerGram, Nothing)) Then
                                    cmd.Parameters.AddWithValue("@PricePerGram", NpgsqlDbType.Double, CDbl(detail.PricePerGram))
                                Else
                                    cmd.Parameters.AddWithValue("@PricePerGram", NpgsqlDbType.Double, DBNull.Value)
                                End If
                                cmd.Parameters.AddWithValue("@ArrivalDetailId", NpgsqlDbType.Integer, CInt(detail.ArrivalDetailId))
                                cmd.Parameters.AddWithValue("@RegistDate", NpgsqlDbType.Timestamp, nowDate)
                                cmd.Parameters.AddWithValue("@ModifyDate", NpgsqlDbType.Timestamp, nowDate)
                                inventoryId = CInt(cmd.ExecuteScalar()).ToString()
                            Else
                                inventoryId = beforeInventoryId
                            End If

                            ' 在庫の増減を記録する
                            Dim afterWeight As Double = CDbl(Nz(detail.Weight, 0))
                            If Not String.IsNullOrEmpty(beforeItemId) AndAlso beforeItemId <> detail.Item.ItemId Then
                                ' 品目が差し替わった。旧品目を全部取り消して新品目を入れ直す
                                InventoryLogs.Write(cmd, InventoryLogStatus.ArrivalCancel,
                                                    beforeItemId, Nothing,
                                                    -beforeWeight,
                                                    beforePricePerGram, detail.ArrivalDetailId, nowDate)
                                InventoryLogs.Write(cmd, InventoryLogStatus.Arrival,
                                                    detail.Item.ItemId, inventoryId,
                                                    afterWeight,
                                                    detail.PricePerGram, detail.ArrivalDetailId, nowDate)
                            Else
                                Dim logStatus As InventoryLogStatus = InventoryLogStatus.Arrival
                                If afterWeight < beforeWeight Then logStatus = InventoryLogStatus.ArrivalCancel
                                InventoryLogs.Write(cmd, logStatus,
                                                    detail.Item.ItemId, inventoryId,
                                                    afterWeight - beforeWeight,
                                                    detail.PricePerGram, detail.ArrivalDetailId, nowDate)
                            End If
                        Next
                    End Using

                    trans.Commit()
                Catch ex As Exception
                    trans.Rollback()
                    Throw
                End Try
            End Using
        End Using

        Return arrival

    End Function

    Public Shared Sub Delete(arrival As Arrival)

        Dim nowDate As Date = Now

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Using trans As NpgsqlTransaction = cn.BeginTransaction()
                Try
                    Using cmd As NpgsqlCommand = cn.CreateCommand()
                        cmd.Transaction = trans

                        ' 払い出しが始まっていたら削除させない
                        If HasConsumedDetails(cmd, arrival.ArrivalId, String.Empty) Then
                            Throw New ApplicationException("払い出しが始まっているため削除できません。")
                        End If

                        Dim SQLString As String

                        ' 消えるロットを先に読んでおく
                        Dim removedIdList As New List(Of String)
                        Dim removedItemIdList As New List(Of String)
                        Dim removedWeightList As New List(Of Double)
                        Dim removedPriceList As New List(Of Object)
                        Dim removedDetailIdList As New List(Of String)

                        SQLString = "select"
                        SQLString &= " Inventories.InventoryId"
                        SQLString &= ",Inventories.ItemId"
                        SQLString &= ",Inventories.Weight"
                        SQLString &= ",Inventories.PricePerGram"
                        SQLString &= ",Inventories.ArrivalDetailId"
                        SQLString &= " from Inventories"
                        SQLString &= " where ArrivalDetailId in ("
                        SQLString &= " select ArrivalDetails.ArrivalDetailId from ArrivalDetails"
                        SQLString &= " where ArrivalDetails.ArrivalId = @ArrivalId"
                        SQLString &= ")"
                        cmd.CommandText = SQLString
                        cmd.Parameters.Clear()
                        cmd.Parameters.AddWithValue("@ArrivalId", NpgsqlDbType.Integer, CInt(arrival.ArrivalId))
                        Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                            While dr.Read
                                removedIdList.Add(dr("InventoryId").ToString)
                                removedItemIdList.Add(dr("ItemId").ToString)
                                removedWeightList.Add(CDbl(Nz(dr("Weight"), 0)))
                                removedPriceList.Add(dr("PricePerGram"))
                                removedDetailIdList.Add(dr("ArrivalDetailId").ToString)
                            End While
                        End Using

                        SQLString = "delete from Inventories"
                        SQLString &= " where ArrivalDetailId in ("
                        SQLString &= " select ArrivalDetails.ArrivalDetailId from ArrivalDetails"
                        SQLString &= " where ArrivalDetails.ArrivalId = @ArrivalId"
                        SQLString &= ")"
                        cmd.CommandText = SQLString
                        cmd.Parameters.Clear()
                        cmd.Parameters.AddWithValue("@ArrivalId", NpgsqlDbType.Integer, CInt(arrival.ArrivalId))
                        cmd.ExecuteNonQuery()

                        SQLString = "delete from ArrivalDetails"
                        SQLString &= " where ArrivalId = @ArrivalId"
                        cmd.CommandText = SQLString
                        cmd.Parameters.Clear()
                        cmd.Parameters.AddWithValue("@ArrivalId", NpgsqlDbType.Integer, CInt(arrival.ArrivalId))
                        cmd.ExecuteNonQuery()

                        SQLString = "delete from Arrivals"
                        SQLString &= " where ArrivalId = @ArrivalId"
                        cmd.CommandText = SQLString
                        cmd.Parameters.Clear()
                        cmd.Parameters.AddWithValue("@ArrivalId", NpgsqlDbType.Integer, CInt(arrival.ArrivalId))
                        cmd.ExecuteNonQuery()

                        ' 消したロットを記録する
                        For i As Integer = 0 To removedIdList.Count - 1
                            InventoryLogs.Write(cmd, InventoryLogStatus.ArrivalCancel,
                                                removedItemIdList(i), Nothing,
                                                -removedWeightList(i),
                                                removedPriceList(i), removedDetailIdList(i), nowDate)
                        Next
                    End Using

                    trans.Commit()
                Catch ex As Exception
                    trans.Rollback()
                    Throw
                End Try
            End Using
        End Using

    End Sub

    ' 入荷総数より在庫が減っていれば払い出し済み
    Private Shared Function IsConsumed(cmd As NpgsqlCommand, arrivalDetailId As String) As Boolean

        If String.IsNullOrEmpty(arrivalDetailId) Then Return False

        Dim SQLString As String
        SQLString = "select"
        SQLString &= " count(*)"
        SQLString &= " from ArrivalDetails"
        SQLString &= " inner join Inventories on ArrivalDetails.ArrivalDetailId = Inventories.ArrivalDetailId"
        SQLString &= " where ArrivalDetails.ArrivalDetailId = @ArrivalDetailId"
        SQLString &= " and Inventories.Weight < ArrivalDetails.Weight"
        cmd.CommandText = SQLString
        cmd.Parameters.Clear()
        cmd.Parameters.AddWithValue("@ArrivalDetailId", NpgsqlDbType.Integer, CInt(arrivalDetailId))

        Return CInt(cmd.ExecuteScalar()) > 0

    End Function

    ' 伝票内に払い出し済みの明細があるか(削除可否の判定用)
    Private Shared Function HasConsumedDetails(cmd As NpgsqlCommand, arrivalId As String, keepClause As String) As Boolean

        Dim SQLString As String
        SQLString = "select"
        SQLString &= " count(*)"
        SQLString &= " from ArrivalDetails"
        SQLString &= " inner join Inventories on ArrivalDetails.ArrivalDetailId = Inventories.ArrivalDetailId"
        SQLString &= " where ArrivalDetails.ArrivalId = @ArrivalId"
        SQLString &= " and Inventories.Weight < ArrivalDetails.Weight"
        SQLString &= keepClause
        cmd.CommandText = SQLString
        cmd.Parameters.Clear()
        cmd.Parameters.AddWithValue("@ArrivalId", NpgsqlDbType.Integer, CInt(arrivalId))

        Return CInt(cmd.ExecuteScalar()) > 0

    End Function

End Class

Public Class ArrivalDetail
    Public Property ArrivalDetailId As String
    Public Property ArrivalId As String
    Public Property DetailNo As String
    Public Property Item As New Item
    Public Property NetWeight As String
    Public Property NetWeightUnitType As New UnitType
    Public Property PerCase As String
    Public Property Quantity As String
    Public Property UnitType As New UnitType
    Public Property UnitPrice As String
    Public Property Price As String
    Public Property ExpiryDate As String
    Public Property PieceUnitType As New UnitType
    Public Property PieceWeight As String
    Public Property Weight As String
    Public Property PricePerGram As String
End Class

Public Class ArrivalDetails

    Public Shared Function GetByArrivalId(arrivalId As Integer) As List(Of ArrivalDetail)

        Dim result As New List(Of ArrivalDetail)

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Try
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String
                    SQLString = "select"
                    SQLString &= " ArrivalDetails.*"
                    SQLString &= " from ArrivalDetails"
                    SQLString &= " where ArrivalId = @ArrivalId"
                    SQLString &= " order by"
                    SQLString &= " DetailNo"
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    cmd.Parameters.AddWithValue("@ArrivalId", NpgsqlDbType.Integer, arrivalId)
                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        While dr.Read()
                            Dim arrivalDetail As ArrivalDetail = Mapping(dr)
                            result.Add(arrivalDetail)
                        End While
                    End Using
                End Using
            Catch ex As Exception
                SysMsgBox(ex.Message, "", Reflection.MethodBase.GetCurrentMethod.Name)
                Throw
            End Try
        End Using

        Return result

    End Function

    Private Shared Function Mapping(dr As NpgsqlDataReader) As ArrivalDetail

        Dim result As New ArrivalDetail

        result.ArrivalDetailId = dr("ArrivalDetailId").ToString
        result.ArrivalId = dr("ArrivalId").ToString
        result.DetailNo = dr("DetailNo").ToString
        result.Item = Items.GetById(dr("ItemId").ToString)
        result.NetWeight = Nz(dr("NetWeight"), 0)
        If Not String.IsNullOrEmpty(dr("NetWeightUnitTypeId").ToString) Then
            result.NetWeightUnitType = UnitTypes.GetById(dr("NetWeightUnitTypeId").ToString)
        End If
        result.PerCase = Nz(dr("PerCase"), 0)
        result.Quantity = Nz(dr("Quantity"), 0)
        result.UnitType = UnitTypes.GetById(dr("UnitTypeId").ToString)
        result.UnitPrice = Nz(dr("UnitPrice"), 0)
        result.Price = Nz(dr("Price"), 0)
        result.ExpiryDate = Nz(dr("ExpiryDate"), "")
        If Not String.IsNullOrEmpty(dr("PieceUnitTypeId").ToString) Then
            result.PieceUnitType = UnitTypes.GetById(dr("PieceUnitTypeId").ToString)
        End If
        result.PieceWeight = dr("PieceWeight").ToString
        result.Weight = Nz(dr("Weight"), "")
        result.PricePerGram = dr("PricePerGram").ToString

        Return result

    End Function

End Class

#End Region

#Region "在庫"

Public Class Inventory
    Public Property InventoryId As String
    Public Property Item As New Item
    Public Property ArrivalDate As String
    Public Property ExpiryDate As String
    Public Property Quantity As String
    Public Property UnitType As New UnitType
    Public Property Weight As String
    Public Property PricePerGram As String
    Public Property ArrivalDetailId As String
End Class

Public Class Inventories

    Public Shared Function GetAll(orderBy As String, Optional where As String = "") As List(Of Inventory)

        Dim result As New List(Of Inventory)

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Try
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String
                    SQLString = "select"
                    SQLString &= " Inventories.*"
                    SQLString &= " from Inventories"
                    SQLString &= " where InventoryId is not null"
                    If Not String.IsNullOrEmpty(where) Then
                        SQLString &= " and " & where
                    End If
                    SQLString &= " Order by " & orderBy
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        While dr.Read
                            Dim inventory As Inventory = Mapping(dr)
                            result.Add(inventory)
                        End While
                    End Using
                End Using
            Catch ex As Exception
                SysMsgBox(ex.Message, "", Reflection.MethodBase.GetCurrentMethod.Name)
                Throw
            End Try
        End Using

        Return result

    End Function

    Public Shared Function GetByArrivalDetailId(arrivalDetailId As Integer) As Inventory

        Dim result As Inventory = Nothing

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Try
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String
                    SQLString = "select"
                    SQLString &= " Inventories.*"
                    SQLString &= " from Inventories"
                    SQLString &= " where ArrivalDetailId = @ArrivalDetailId"
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    cmd.Parameters.AddWithValue("@ArrivalDetailId", NpgsqlDbType.Integer, arrivalDetailId)
                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        If dr.Read() Then result = Mapping(dr)
                    End Using
                End Using
            Catch ex As Exception
                SysMsgBox(ex.Message, "", Reflection.MethodBase.GetCurrentMethod.Name)
                Throw
            End Try
        End Using

        Return result

    End Function

    Private Shared Function Mapping(dr As NpgsqlDataReader) As Inventory

        Dim result As New Inventory

        result.InventoryId = dr("InventoryId").ToString
        result.Item = Items.GetById(dr("ItemId").ToString)
        result.ArrivalDate = dr("ArrivalDate").ToString
        result.ExpiryDate = dr("ExpiryDate").ToString
        result.Quantity = dr("Quantity").ToString
        result.UnitType = UnitTypes.GetById(dr("UnitTypeId").ToString)
        result.Weight = dr("Weight").ToString
        result.PricePerGram = dr("PricePerGram").ToString
        result.ArrivalDetailId = dr("ArrivalDetailId").ToString

        Return result

    End Function

    Public Shared Function Save(inventory As Inventory) As Inventory

        Dim nowDate As Date = Now

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Dim trans As NpgsqlTransaction = cn.BeginTransaction
            Try
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    cmd.Transaction = trans

                    Dim SQLString As String

                    ' 記録用に更新前の残高を見ておく
                    Dim beforeWeight As Double = 0
                    If Not String.IsNullOrEmpty(inventory.InventoryId) Then
                        SQLString = "select"
                        SQLString &= " Weight"
                        SQLString &= " from Inventories"
                        SQLString &= " where InventoryId = @InventoryId"
                        cmd.CommandText = SQLString
                        cmd.Parameters.Clear()
                        cmd.Parameters.AddWithValue("@InventoryId", NpgsqlDbType.Integer, CInt(inventory.InventoryId))
                        beforeWeight = CDbl(Nz(cmd.ExecuteScalar(), 0))
                    End If

                    If String.IsNullOrEmpty(inventory.InventoryId) Then
                        SQLString = "insert into Inventories ("
                        SQLString &= " ItemId"
                        SQLString &= ",ArrivalDate"
                        SQLString &= ",ExpiryDate"
                        SQLString &= ",Quantity"
                        SQLString &= ",UnitTypeId"
                        SQLString &= ",Weight"
                        SQLString &= ",PricePerGram"
                        SQLString &= ",ArrivalDetailId"
                        SQLString &= ",RegistDate"
                        SQLString &= ",ModifyDate"
                        SQLString &= " ) values ("
                        SQLString &= " @ItemId"
                        SQLString &= ",@ArrivalDate"
                        SQLString &= ",@ExpiryDate"
                        SQLString &= ",@Quantity"
                        SQLString &= ",@UnitTypeId"
                        SQLString &= ",@Weight"
                        SQLString &= ",@PricePerGram"
                        SQLString &= ",@ArrivalDetailId"
                        SQLString &= ",@RegistDate"
                        SQLString &= ",@ModifyDate"
                        SQLString &= ") returning InventoryId"
                        cmd.CommandText = SQLString
                        cmd.Parameters.Clear()
                        cmd.Parameters.AddWithValue("@ItemId", NpgsqlDbType.Integer, CInt(inventory.Item.ItemId))
                        cmd.Parameters.AddWithValue("@ArrivalDate", NpgsqlDbType.Date, CDate(inventory.ArrivalDate).Date)
                        If Not IsNothing(Nz(inventory.ExpiryDate, Nothing)) Then
                            cmd.Parameters.AddWithValue("@ExpiryDate", NpgsqlDbType.Date, CDate(inventory.ExpiryDate).Date)
                        Else
                            cmd.Parameters.AddWithValue("@ExpiryDate", NpgsqlDbType.Date, DBNull.Value)
                        End If
                        If Not IsNothing(Nz(inventory.Quantity, Nothing)) Then
                            cmd.Parameters.AddWithValue("@Quantity", NpgsqlDbType.Double, CDbl(inventory.Quantity))
                        Else
                            cmd.Parameters.AddWithValue("@Quantity", NpgsqlDbType.Double, DBNull.Value)
                        End If
                        cmd.Parameters.AddWithValue("@UnitTypeId", NpgsqlDbType.Integer, CInt(inventory.UnitType.UnitTypeId))
                        If Not IsNothing(Nz(inventory.Weight, Nothing)) Then
                            cmd.Parameters.AddWithValue("@Weight", NpgsqlDbType.Double, CDbl(inventory.Weight))
                        Else
                            cmd.Parameters.AddWithValue("@Weight", NpgsqlDbType.Double, DBNull.Value)
                        End If
                        If Not IsNothing(Nz(inventory.PricePerGram, Nothing)) Then
                            cmd.Parameters.AddWithValue("@PricePerGram", NpgsqlDbType.Double, CDbl(inventory.PricePerGram))
                        Else
                            cmd.Parameters.AddWithValue("@PricePerGram", NpgsqlDbType.Double, DBNull.Value)
                        End If
                        If Not IsNothing(Nz(inventory.ArrivalDetailId, Nothing)) Then
                            cmd.Parameters.AddWithValue("@ArrivalDetailId", NpgsqlDbType.Integer, CInt(inventory.ArrivalDetailId))
                        Else
                            cmd.Parameters.AddWithValue("@ArrivalDetailId", NpgsqlDbType.Integer, DBNull.Value)
                        End If
                        cmd.Parameters.AddWithValue("@RegistDate", NpgsqlDbType.Timestamp, nowDate)
                        cmd.Parameters.AddWithValue("@ModifyDate", NpgsqlDbType.Timestamp, nowDate)
                        inventory.InventoryId = CInt(cmd.ExecuteScalar()).ToString()
                    Else
                        SQLString = "update Inventories set"
                        SQLString &= " ItemId = @ItemId"
                        SQLString &= ",ArrivalDate = @ArrivalDate"
                        SQLString &= ",ExpiryDate = @ExpiryDate"
                        SQLString &= ",Quantity = @Quantity"
                        SQLString &= ",UnitTypeId = @UnitTypeId"
                        SQLString &= ",Weight = @Weight"
                        SQLString &= ",PricePerGram = @PricePerGram"
                        SQLString &= ",ArrivalDetailId = @ArrivalDetailId"
                        SQLString &= ",ModifyDate = @ModifyDate"
                        SQLString &= " where InventoryId = @InventoryId"
                        cmd.CommandText = SQLString
                        cmd.Parameters.Clear()
                        cmd.Parameters.AddWithValue("@ItemId", NpgsqlDbType.Integer, CInt(inventory.Item.ItemId))
                        cmd.Parameters.AddWithValue("@ArrivalDate", NpgsqlDbType.Date, CDate(inventory.ArrivalDate).Date)
                        If Not IsNothing(Nz(inventory.ExpiryDate, Nothing)) Then
                            cmd.Parameters.AddWithValue("@ExpiryDate", NpgsqlDbType.Date, CDate(inventory.ExpiryDate).Date)
                        Else
                            cmd.Parameters.AddWithValue("@ExpiryDate", NpgsqlDbType.Date, DBNull.Value)
                        End If
                        If Not IsNothing(Nz(inventory.Quantity, Nothing)) Then
                            cmd.Parameters.AddWithValue("@Quantity", NpgsqlDbType.Double, CDbl(inventory.Quantity))
                        Else
                            cmd.Parameters.AddWithValue("@Quantity", NpgsqlDbType.Double, DBNull.Value)
                        End If
                        cmd.Parameters.AddWithValue("@UnitTypeId", NpgsqlDbType.Integer, CInt(inventory.UnitType.UnitTypeId))
                        If Not IsNothing(Nz(inventory.Weight, Nothing)) Then
                            cmd.Parameters.AddWithValue("@Weight", NpgsqlDbType.Double, CDbl(inventory.Weight))
                        Else
                            cmd.Parameters.AddWithValue("@Weight", NpgsqlDbType.Double, DBNull.Value)
                        End If
                        If Not IsNothing(Nz(inventory.PricePerGram, Nothing)) Then
                            cmd.Parameters.AddWithValue("@PricePerGram", NpgsqlDbType.Double, CDbl(inventory.PricePerGram))
                        Else
                            cmd.Parameters.AddWithValue("@PricePerGram", NpgsqlDbType.Double, DBNull.Value)
                        End If
                        If Not IsNothing(Nz(inventory.ArrivalDetailId, Nothing)) Then
                            cmd.Parameters.AddWithValue("@ArrivalDetailId", NpgsqlDbType.Integer, CInt(inventory.ArrivalDetailId))
                        Else
                            cmd.Parameters.AddWithValue("@ArrivalDetailId", NpgsqlDbType.Integer, DBNull.Value)
                        End If
                        cmd.Parameters.AddWithValue("@ModifyDate", NpgsqlDbType.Timestamp, nowDate)
                        cmd.Parameters.AddWithValue("@InventoryId", NpgsqlDbType.Integer, CInt(inventory.InventoryId))
                        cmd.ExecuteNonQuery()
                    End If

                    ' 在庫の増減を記録する
                    InventoryLogs.Write(cmd, InventoryLogStatus.Adjust,
                                        inventory.Item.ItemId, inventory.InventoryId,
                                        CDbl(Nz(inventory.Weight, 0)) - beforeWeight,
                                        inventory.PricePerGram, inventory.InventoryId, nowDate)

                    trans.Commit()
                End Using
            Catch ex As Exception
                trans.Rollback()
                Throw
            End Try
        End Using

        Return inventory

    End Function

    Public Shared Function Save(inventoryList As List(Of Inventory),
                                Optional status As InventoryLogStatus = InventoryLogStatus.Adjust,
                                Optional sourceIdList As List(Of String) = Nothing) As List(Of Inventory)

        Dim nowDate As Date = Now

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Dim trans As NpgsqlTransaction = cn.BeginTransaction
            Try
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    cmd.Transaction = trans

                    Dim SQLString As String
                    Dim i As Integer = 0
                    For Each inventory As Inventory In inventoryList

                        ' 記録用に更新前の残高を見ておく
                        Dim beforeWeight As Double = 0
                        If Not String.IsNullOrEmpty(inventory.InventoryId) Then
                            SQLString = "select"
                            SQLString &= " Weight"
                            SQLString &= " from Inventories"
                            SQLString &= " where InventoryId = @InventoryId"
                            cmd.CommandText = SQLString
                            cmd.Parameters.Clear()
                            cmd.Parameters.AddWithValue("@InventoryId", NpgsqlDbType.Integer, CInt(inventory.InventoryId))
                            beforeWeight = CDbl(Nz(cmd.ExecuteScalar(), 0))
                        End If

                        If String.IsNullOrEmpty(inventory.InventoryId) Then
                            SQLString = "insert into Inventories ("
                            SQLString &= " ItemId"
                            SQLString &= ",ArrivalDate"
                            SQLString &= ",ExpiryDate"
                            SQLString &= ",Quantity"
                            SQLString &= ",UnitTypeId"
                            SQLString &= ",Weight"
                            SQLString &= ",PricePerGram"
                            SQLString &= ",ArrivalDetailId"
                            SQLString &= ",RegistDate"
                            SQLString &= ",ModifyDate"
                            SQLString &= " ) values ("
                            SQLString &= " @ItemId"
                            SQLString &= ",@ArrivalDate"
                            SQLString &= ",@ExpiryDate"
                            SQLString &= ",@Quantity"
                            SQLString &= ",@UnitTypeId"
                            SQLString &= ",@Weight"
                            SQLString &= ",@PricePerGram"
                            SQLString &= ",@ArrivalDetailId"
                            SQLString &= ",@RegistDate"
                            SQLString &= ",@ModifyDate"
                            SQLString &= ") returning InventoryId"
                            cmd.CommandText = SQLString
                            cmd.Parameters.Clear()
                            cmd.Parameters.AddWithValue("@ItemId", NpgsqlDbType.Integer, CInt(inventory.Item.ItemId))
                            cmd.Parameters.AddWithValue("@ArrivalDate", NpgsqlDbType.Date, CDate(inventory.ArrivalDate).Date)
                            If Not IsNothing(Nz(inventory.ExpiryDate, Nothing)) Then
                                cmd.Parameters.AddWithValue("@ExpiryDate", NpgsqlDbType.Date, CDate(inventory.ExpiryDate).Date)
                            Else
                                cmd.Parameters.AddWithValue("@ExpiryDate", NpgsqlDbType.Date, DBNull.Value)
                            End If
                            If Not IsNothing(Nz(inventory.Quantity, Nothing)) Then
                                cmd.Parameters.AddWithValue("@Quantity", NpgsqlDbType.Double, CDbl(inventory.Quantity))
                            Else
                                cmd.Parameters.AddWithValue("@Quantity", NpgsqlDbType.Double, DBNull.Value)
                            End If
                            cmd.Parameters.AddWithValue("@UnitTypeId", NpgsqlDbType.Integer, CInt(inventory.UnitType.UnitTypeId))
                            cmd.Parameters.AddWithValue("@Weight", Nz(inventory.Weight, DBNull.Value))
                            cmd.Parameters.AddWithValue("@PricePerGram", Nz(inventory.PricePerGram, DBNull.Value))
                            If Not IsNothing(Nz(inventory.ArrivalDetailId, Nothing)) Then
                                cmd.Parameters.AddWithValue("@ArrivalDetailId", NpgsqlDbType.Integer, CInt(inventory.ArrivalDetailId))
                            Else
                                cmd.Parameters.AddWithValue("@ArrivalDetailId", NpgsqlDbType.Integer, DBNull.Value)
                            End If
                            cmd.Parameters.AddWithValue("@RegistDate", NpgsqlDbType.Timestamp, nowDate)
                            cmd.Parameters.AddWithValue("@ModifyDate", NpgsqlDbType.Timestamp, nowDate)
                            inventory.InventoryId = CInt(cmd.ExecuteScalar()).ToString()
                        Else
                            SQLString = "update Inventories set"
                            SQLString &= " ItemId = @ItemId"
                            SQLString &= ",ArrivalDate = @ArrivalDate"
                            SQLString &= ",ExpiryDate = @ExpiryDate"
                            SQLString &= ",Quantity = @Quantity"
                            SQLString &= ",UnitTypeId = @UnitTypeId"
                            SQLString &= ",Weight = @Weight"
                            SQLString &= ",PricePerGram = @PricePerGram"
                            SQLString &= ",ArrivalDetailId = @ArrivalDetailId"
                            SQLString &= ",ModifyDate = @ModifyDate"
                            SQLString &= " where InventoryId = @InventoryId"
                            cmd.CommandText = SQLString
                            cmd.Parameters.Clear()
                            cmd.Parameters.AddWithValue("@ItemId", NpgsqlDbType.Integer, CInt(inventory.Item.ItemId))
                            cmd.Parameters.AddWithValue("@ArrivalDate", NpgsqlDbType.Date, CDate(inventory.ArrivalDate))
                            If Not IsNothing(Nz(inventory.ExpiryDate, Nothing)) Then
                                cmd.Parameters.AddWithValue("@ExpiryDate", NpgsqlDbType.Date, CDate(inventory.ExpiryDate).Date)
                            Else
                                cmd.Parameters.AddWithValue("@ExpiryDate", NpgsqlDbType.Date, DBNull.Value)
                            End If
                            If Not IsNothing(Nz(inventory.Quantity, Nothing)) Then
                                cmd.Parameters.AddWithValue("@Quantity", NpgsqlDbType.Double, CDbl(inventory.Quantity))
                            Else
                                cmd.Parameters.AddWithValue("@Quantity", NpgsqlDbType.Double, DBNull.Value)
                            End If
                            cmd.Parameters.AddWithValue("@UnitTypeId", NpgsqlDbType.Integer, CInt(inventory.UnitType.UnitTypeId))
                            If Not IsNothing(Nz(inventory.Weight, Nothing)) Then
                                cmd.Parameters.AddWithValue("@Weight", NpgsqlDbType.Double, CDbl(inventory.Weight))
                            Else
                                cmd.Parameters.AddWithValue("@Weight", NpgsqlDbType.Double, DBNull.Value)
                            End If
                            If Not IsNothing(Nz(inventory.PricePerGram, Nothing)) Then
                                cmd.Parameters.AddWithValue("@PricePerGram", NpgsqlDbType.Double, CDbl(inventory.PricePerGram))
                            Else
                                cmd.Parameters.AddWithValue("@PricePerGram", NpgsqlDbType.Double, DBNull.Value)
                            End If
                            If Not IsNothing(Nz(inventory.ArrivalDetailId, Nothing)) Then
                                cmd.Parameters.AddWithValue("@ArrivalDetailId", NpgsqlDbType.Integer, CInt(inventory.ArrivalDetailId))
                            Else
                                cmd.Parameters.AddWithValue("@ArrivalDetailId", NpgsqlDbType.Integer, DBNull.Value)
                            End If
                            cmd.Parameters.AddWithValue("@ModifyDate", NpgsqlDbType.Timestamp, nowDate)
                            cmd.Parameters.AddWithValue("@InventoryId", NpgsqlDbType.Integer, CInt(inventory.InventoryId))
                            cmd.ExecuteNonQuery()
                        End If

                        ' 在庫の増減を記録する
                        ' ・在庫調整の SourceId には調整理由(AjustTypeId)を入れる
                        ' ・棚卸は理由を持たないので SourceId は空になる
                        Dim sourceId As String = Nothing
                        If sourceIdList IsNot Nothing AndAlso i < sourceIdList.Count Then
                            sourceId = sourceIdList(i)
                        End If
                        InventoryLogs.Write(cmd, status,
                                            inventory.Item.ItemId, inventory.InventoryId,
                                            CDbl(Nz(inventory.Weight, 0)) - beforeWeight,
                                            inventory.PricePerGram, sourceId, nowDate)
                        i += 1
                    Next

                    trans.Commit()
                End Using
            Catch ex As Exception
                trans.Rollback()
                Throw
            End Try
        End Using

        Return inventoryList

    End Function

    Public Shared Sub Delete(inventory As Inventory)

        Dim nowDate As Date = Now

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Dim trans As NpgsqlTransaction = cn.BeginTransaction
            Try
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    cmd.Transaction = trans

                    Dim SQLString As String

                    ' 記録用に消える前の内容を見ておく
                    Dim beforeItemId As String = String.Empty
                    Dim beforeWeight As Double = 0
                    Dim beforePrice As Object = DBNull.Value
                    SQLString = "select"
                    SQLString &= " ItemId"
                    SQLString &= ",Weight"
                    SQLString &= ",PricePerGram"
                    SQLString &= " from Inventories"
                    SQLString &= " where InventoryId = @InventoryId"
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    cmd.Parameters.AddWithValue("@InventoryId", NpgsqlDbType.Integer, CInt(inventory.InventoryId))
                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        If dr.Read() Then
                            beforeItemId = dr("ItemId").ToString
                            beforeWeight = CDbl(Nz(dr("Weight"), 0))
                            beforePrice = dr("PricePerGram")
                        End If
                    End Using

                    SQLString = "delete from Inventories"
                    SQLString &= " where InventoryId = @InventoryId"
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    cmd.Parameters.AddWithValue("@InventoryId", NpgsqlDbType.Integer, CInt(inventory.InventoryId))
                    cmd.ExecuteNonQuery()

                    ' 消した在庫を記録する
                    InventoryLogs.Write(cmd, InventoryLogStatus.Adjust,
                                        beforeItemId, Nothing,
                                        -beforeWeight, beforePrice,
                                        Nothing, nowDate)

                    trans.Commit()
                End Using
            Catch ex As Exception
                trans.Rollback()
                Throw
            End Try
        End Using

    End Sub

End Class

#End Region

#Region "在庫履歴"

' 在庫の増減区分
Public Enum InventoryLogStatus
    Arrival = 1             ' 入荷
    ArrivalCancel = 2       ' 入荷修正・取消
    Consume = 3             ' 製造材料払い出し
    Produce = 4             ' 製造完成入庫
    ProduceCancel = 5       ' 製造完了取消
    Adjust = 6              ' 在庫調整
    Stocktaking = 7         ' 棚卸
    Disposal = 8            ' 廃棄
End Enum

Public Class InventoryLog
    Public Property InventoryLogId As String
    Public Property TransactionDate As String
    Public Property TransactionStatus As String
    Public Property Item As New Item
    Public Property InventoryId As String
    Public Property Quantity As String
    Public Property UnitType As New UnitType
    Public Property AfterQuantity As String
    Public Property UnitPrice As String
    Public Property SourceId As String
End Class

Public Class InventoryLogs

    ' 在庫の増減を1件記録する
    ' ・呼び出し側のトランザクションに乗せるため cmd をそのまま受け取る
    ' ・必ず在庫(Inventories)を更新した「後」に呼ぶこと(変動後残高を読み直すため)
    ' ・cmd の CommandText とパラメータを書き換えるので、リーダーを開いたまま呼んではいけない
    Public Shared Sub Write(cmd As NpgsqlCommand,
                            status As InventoryLogStatus,
                            itemId As String,
                            inventoryId As String,
                            quantity As Double,
                            unitPrice As Object,
                            sourceId As String,
                            nowDate As Date)

        ' 動きがないものは記録しない
        If quantity = 0 Then Return

        Dim id As Integer
        If Not Integer.TryParse(itemId, id) Then Return

        ' 履歴の単位は在庫の基準単位(g / ml / 個)で統一する
        Dim baseUnitId As Integer = calc.GetBaseUnitTypeId(id)

        Dim SQLString As String

        ' 変動後の残高を読み直す(ロットが消えている場合は0)
        Dim afterQuantity As Double = 0
        Dim lotId As Integer
        If Integer.TryParse(inventoryId, lotId) Then
            SQLString = "select"
            SQLString &= " Weight"
            SQLString &= " from Inventories"
            SQLString &= " where InventoryId = @InventoryId"
            cmd.CommandText = SQLString
            cmd.Parameters.Clear()
            cmd.Parameters.AddWithValue("@InventoryId", NpgsqlDbType.Integer, lotId)
            Dim value As Object = cmd.ExecuteScalar()
            If value IsNot Nothing Then afterQuantity = CDbl(Nz(value, 0))
        End If

        SQLString = "insert into InventoryLogs ("
        SQLString &= " TransactionDate"
        SQLString &= ",TransactionStatus"
        SQLString &= ",ItemId"
        SQLString &= ",InventoryId"
        SQLString &= ",Quantity"
        SQLString &= ",UnitTypeId"
        SQLString &= ",AfterQuantity"
        SQLString &= ",UnitPrice"
        SQLString &= ",SourceId"
        SQLString &= ",OperatorId"
        SQLString &= ",RegistDate"
        SQLString &= " ) values ("
        SQLString &= " @TransactionDate"
        SQLString &= ",@TransactionStatus"
        SQLString &= ",@ItemId"
        SQLString &= ",@InventoryId"
        SQLString &= ",@Quantity"
        SQLString &= ",@UnitTypeId"
        SQLString &= ",@AfterQuantity"
        SQLString &= ",@UnitPrice"
        SQLString &= ",@SourceId"
        SQLString &= ",@OperatorId"
        SQLString &= ",@RegistDate"
        SQLString &= ")"
        cmd.CommandText = SQLString
        cmd.Parameters.Clear()
        cmd.Parameters.AddWithValue("@TransactionDate", NpgsqlDbType.Date, nowDate.Date)
        cmd.Parameters.AddWithValue("@TransactionStatus", NpgsqlDbType.Integer, CInt(status))
        cmd.Parameters.AddWithValue("@ItemId", NpgsqlDbType.Integer, id)
        If Not IsNothing(Nz(inventoryId, Nothing)) Then
            cmd.Parameters.AddWithValue("@InventoryId", NpgsqlDbType.Integer, CInt(inventoryId))
        Else
            cmd.Parameters.AddWithValue("@InventoryId", NpgsqlDbType.Integer, DBNull.Value)
        End If
        cmd.Parameters.AddWithValue("@Quantity", NpgsqlDbType.Double, CDbl(quantity))
        cmd.Parameters.AddWithValue("@UnitTypeId", NpgsqlDbType.Integer, baseUnitId)
        cmd.Parameters.AddWithValue("@AfterQuantity", NpgsqlDbType.Double, CDbl(afterQuantity))
        If Not IsNothing(Nz(unitPrice, Nothing)) Then
            cmd.Parameters.AddWithValue("@UnitPrice", NpgsqlDbType.Double, CDbl(unitPrice))
        Else
            cmd.Parameters.AddWithValue("@UnitPrice", NpgsqlDbType.Double, DBNull.Value)
        End If
        If Not IsNothing(Nz(sourceId, Nothing)) Then
            cmd.Parameters.AddWithValue("@SourceId", NpgsqlDbType.Integer, CInt(sourceId))
        Else
            cmd.Parameters.AddWithValue("@SourceId", NpgsqlDbType.Integer, DBNull.Value)
        End If
        If sharedOperator IsNot Nothing AndAlso Not IsNothing(Nz(sharedOperator.OperatorId, Nothing)) Then
            cmd.Parameters.AddWithValue("@OperatorId", NpgsqlDbType.Integer, CInt(sharedOperator.OperatorId))
        Else
            cmd.Parameters.AddWithValue("@OperatorId", NpgsqlDbType.Integer, DBNull.Value)
        End If
        cmd.Parameters.AddWithValue("@RegistDate", NpgsqlDbType.Timestamp, nowDate)
        cmd.ExecuteNonQuery()

    End Sub

    Public Shared Function GetAll(orderBy As String, Optional where As String = "") As List(Of InventoryLog)

        Dim result As New List(Of InventoryLog)

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Try
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String
                    SQLString = "select"
                    SQLString &= " InventoryLogs.*"
                    SQLString &= " from InventoryLogs"
                    SQLString &= " where InventoryLogId is not null"
                    If Not String.IsNullOrEmpty(where) Then
                        SQLString &= " and " & where
                    End If
                    SQLString &= " order by " & orderBy
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        While dr.Read
                            result.Add(Mapping(dr))
                        End While
                    End Using
                End Using
            Catch ex As Exception
                SysMsgBox(ex.Message, "", Reflection.MethodBase.GetCurrentMethod.Name)
                Throw
            End Try
        End Using

        Return result

    End Function

    Private Shared Function Mapping(dr As NpgsqlDataReader) As InventoryLog

        Dim result As New InventoryLog

        result.InventoryLogId = dr("InventoryLogId").ToString
        result.TransactionDate = dr("TransactionDate").ToString
        result.TransactionStatus = dr("TransactionStatus").ToString
        result.Item = Items.GetById(dr("ItemId").ToString)
        result.InventoryId = dr("InventoryId").ToString
        result.Quantity = dr("Quantity").ToString
        result.UnitType = UnitTypes.GetById(dr("UnitTypeId").ToString)
        result.AfterQuantity = dr("AfterQuantity").ToString
        result.UnitPrice = dr("UnitPrice").ToString
        result.SourceId = dr("SourceId").ToString

        Return result

    End Function

End Class

#End Region

#Region "製造計画"

Public Class Plan
    Public Property PlanId As String
    Public Property PlanDate As String
    Public Property Item As New Item
    Public Property UnitType As New UnitType
    Public Property PlanDetails As New List(Of PlanDetail)
    Public Property InstructionStart As Boolean '製造開始フラグ
End Class

Public Class Plans

    Public Shared Function Count(Optional where As String = Nothing) As Integer

        Dim result As Integer

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Try
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String
                    SQLString = "select count(*) as RecCount"
                    SQLString &= "from Plans"
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        If dr.Read() Then result = dr("RecCount")
                    End Using
                End Using
            Catch ex As Exception
                SysMsgBox(ex.Message, "", Reflection.MethodBase.GetCurrentMethod.Name)
                Throw
            End Try
        End Using

        Return result

    End Function

    Public Shared Function GetAll(Optional where As String = Nothing, Optional orderBy As String = Nothing) As List(Of Plan)

        Dim result As New List(Of Plan)

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Try
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String
                    SQLString = "select"
                    SQLString &= " Plans.*"
                    SQLString &= " from Plans"
                    SQLString &= " where PlanId is not null"
                    If Not String.IsNullOrEmpty(where) Then
                        SQLString &= " and " & where
                    End If
                    If Not String.IsNullOrEmpty(orderBy) Then
                        SQLString &= " Order by " & orderBy
                    End If
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        While dr.Read
                            Dim instructionPlan As Plan = Mapping(dr)

                            If instructionPlan IsNot Nothing Then
                                instructionPlan.PlanDetails = PlanDetails.GetByPlanId(instructionPlan.PlanId)
                            End If

                            result.Add(instructionPlan)
                        End While
                    End Using
                End Using
            Catch ex As Exception
                SysMsgBox(ex.Message, "", Reflection.MethodBase.GetCurrentMethod.Name)
                Throw
            End Try
        End Using

        Return result

    End Function

    Public Shared Function GetById(id As Integer) As Plan

        Dim result As Plan = Nothing

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Try
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String
                    SQLString = "select"
                    SQLString &= " Plans.*"
                    SQLString &= " from Plans"
                    SQLString &= " where PlanId = @PlanId"
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    cmd.Parameters.AddWithValue("@PlanId", NpgsqlDbType.Integer, id)
                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        If dr.Read() Then result = Mapping(dr)
                    End Using

                    If result IsNot Nothing Then
                        result.PlanDetails = PlanDetails.GetByPlanId(result.PlanId)
                    End If
                End Using
            Catch ex As Exception
                SysMsgBox(ex.Message, "", Reflection.MethodBase.GetCurrentMethod.Name)
                Throw
            End Try
        End Using

        Return result

    End Function

    Public Shared Function GetByKey(planDate As String, itemId As String) As Plan

        Dim result As Plan = Nothing

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Try
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String
                    SQLString = "select"
                    SQLString &= " Plans.*"
                    SQLString &= " from Plans"
                    SQLString &= " where PlanDate = @PlanDate"
                    SQLString &= " and ItemId = @ItemId"
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    cmd.Parameters.AddWithValue("@PlanDate", NpgsqlDbType.Date, CDate(planDate).Date)
                    cmd.Parameters.AddWithValue("@ItemId", NpgsqlDbType.Integer, CInt(itemId))
                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        If dr.Read() Then result = Mapping(dr)
                    End Using
                End Using

                If result IsNot Nothing Then
                    result.PlanDetails = PlanDetails.GetByPlanId(result.PlanId)
                End If
            Catch ex As Exception
                SysMsgBox(ex.Message, "", Reflection.MethodBase.GetCurrentMethod.Name)
                Throw
            End Try
        End Using

        Return result

    End Function

    Private Shared Function Mapping(dr As NpgsqlDataReader) As Plan

        Dim result As New Plan

        result.PlanId = dr("PlanId").ToString
        result.PlanDate = dr("PlanDate").ToString
        result.Item = Items.GetById(dr("ItemId").ToString)
        result.UnitType = UnitTypes.GetById(dr("UnitTypeId").ToString)

        Return result

    End Function

    Public Shared Function Save(plan As Plan) As Plan

        Dim nowDate As Date = Now

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Using trans As NpgsqlTransaction = cn.BeginTransaction
                Try
                    Using cmd As NpgsqlCommand = cn.CreateCommand()
                        cmd.Transaction = trans

                        Dim SQLString As String
                        If String.IsNullOrEmpty(plan.PlanId) Then
                            SQLString = "insert into Plans ("
                            SQLString &= " PlanDate"
                            SQLString &= ",ItemId"
                            SQLString &= ",UnitTypeId"
                            SQLString &= ",RegistDate"
                            SQLString &= ",ModifyDate"
                            SQLString &= " ) values ("
                            SQLString &= " @PlanDate"
                            SQLString &= ",@ItemId"
                            SQLString &= ",@UnitTypeId"
                            SQLString &= ",@RegistDate"
                            SQLString &= ",@ModifyDate"
                            SQLString &= ") returning PlanId"
                            cmd.CommandText = SQLString
                            cmd.Parameters.Clear()
                            cmd.Parameters.AddWithValue("@PlanDate", NpgsqlDbType.Date, CDate(plan.PlanDate).Date)
                            cmd.Parameters.AddWithValue("@ItemId", NpgsqlDbType.Integer, CInt(plan.Item.ItemId))
                            cmd.Parameters.AddWithValue("@UnitTypeId", NpgsqlDbType.Integer, CInt(plan.UnitType.UnitTypeId))
                            cmd.Parameters.AddWithValue("@RegistDate", NpgsqlDbType.Timestamp, nowDate)
                            cmd.Parameters.AddWithValue("@ModifyDate", NpgsqlDbType.Timestamp, nowDate)
                            plan.PlanId = CInt(cmd.ExecuteScalar()).ToString()
                        Else
                            SQLString = "update Plans set"
                            SQLString &= " PlanDate = @PlanDate"
                            SQLString &= ",ItemId = @ItemId"
                            SQLString &= ",UnitTypeId = @UnitTypeId"
                            SQLString &= ",ModifyDate = @ModifyDate"
                            SQLString &= " where PlanId = @PlanId"
                            cmd.CommandText = SQLString
                            cmd.Parameters.Clear()
                            cmd.Parameters.AddWithValue("@PlanDate", NpgsqlDbType.Date, CDate(plan.PlanDate).Date)
                            cmd.Parameters.AddWithValue("@ItemId", NpgsqlDbType.Integer, CInt(plan.Item.ItemId))
                            cmd.Parameters.AddWithValue("@UnitTypeId", NpgsqlDbType.Integer, CInt(plan.UnitType.UnitTypeId))
                            cmd.Parameters.AddWithValue("@ModifyDate", NpgsqlDbType.Timestamp, nowDate)
                            cmd.Parameters.AddWithValue("@PlanId", NpgsqlDbType.Integer, CInt(plan.PlanId))
                            cmd.ExecuteNonQuery()
                        End If

                        SQLString = "delete from PlanDetails"
                        SQLString &= " where PlanId = @PlanId"
                        cmd.CommandText = SQLString
                        cmd.Parameters.Clear()
                        cmd.Parameters.AddWithValue("@PlanId", NpgsqlDbType.Integer, CInt(plan.PlanId))
                        cmd.ExecuteNonQuery()

                        Dim sumQuantity As Double
                        For Each detail As PlanDetail In plan.PlanDetails
                            SQLString = "insert into PlanDetails ("
                            SQLString &= " PlanId"
                            SQLString &= ",StoreId"
                            SQLString &= ",Quantity"
                            SQLString &= ",RegistDate"
                            SQLString &= ",ModifyDate"
                            SQLString &= ") values ("
                            SQLString &= " @PlanId"
                            SQLString &= ",@StoreId"
                            SQLString &= ",@Quantity"
                            SQLString &= ",@RegistDate"
                            SQLString &= ",@ModifyDate"
                            SQLString &= ") returning PlanDetailId"
                            cmd.CommandText = SQLString
                            cmd.Parameters.Clear()
                            cmd.Parameters.AddWithValue("@PlanId", NpgsqlDbType.Integer, CInt(plan.PlanId))
                            cmd.Parameters.AddWithValue("@StoreId", NpgsqlDbType.Integer, CInt(detail.Store.StoreId))
                            cmd.Parameters.AddWithValue("@Quantity", NpgsqlDbType.Double, CDbl(detail.Quantity))
                            cmd.Parameters.AddWithValue("@RegistDate", NpgsqlDbType.Timestamp, nowDate)
                            cmd.Parameters.AddWithValue("@ModifyDate", NpgsqlDbType.Timestamp, nowDate)
                            detail.PlanDetailId = CInt(cmd.ExecuteScalar()).ToString()

                            sumQuantity += detail.Quantity
                        Next

                        If plan.InstructionStart Then
                            If Not String.IsNullOrEmpty(plan.PlanId) AndAlso IsNothing(Instructions.GetByPlanId(plan.PlanId)) Then
                                SQLString = "insert into Instructions ("
                                SQLString &= " StartDate"
                                SQLString &= ",FinishDate"
                                SQLString &= ",ItemId"
                                SQLString &= ",PlanQuantity"
                                SQLString &= ",RealQuantity"
                                SQLString &= ",UnitTypeId"
                                SQLString &= ",PlanId"
                                SQLString &= ",RegistDate"
                                SQLString &= ",ModifyDate"
                                SQLString &= " ) values ("
                                SQLString &= " @StartDate"
                                SQLString &= ",@FinishDate"
                                SQLString &= ",@ItemId"
                                SQLString &= ",@PlanQuantity"
                                SQLString &= ",@RealQuantity"
                                SQLString &= ",@UnitTypeId"
                                SQLString &= ",@PlanId"
                                SQLString &= ",@RegistDate"
                                SQLString &= ",@ModifyDate"
                                SQLString &= ") returning InstructionId"
                                cmd.CommandText = SQLString
                                cmd.Parameters.Clear()
                                cmd.Parameters.AddWithValue("@StartDate", NpgsqlDbType.Date, CDate(plan.PlanDate).Date)
                                cmd.Parameters.AddWithValue("@FinishDate", NpgsqlDbType.Date, DBNull.Value)
                                cmd.Parameters.AddWithValue("@ItemId", NpgsqlDbType.Integer, CInt(plan.Item.ItemId))
                                cmd.Parameters.AddWithValue("@PlanQuantity", NpgsqlDbType.Double, CDbl(sumQuantity))
                                cmd.Parameters.AddWithValue("@RealQuantity", NpgsqlDbType.Double, DBNull.Value)
                                cmd.Parameters.AddWithValue("@UnitTypeId", NpgsqlDbType.Integer, CInt(plan.UnitType.UnitTypeId))
                                cmd.Parameters.AddWithValue("@PlanId", NpgsqlDbType.Integer, CInt(plan.PlanId))
                                cmd.Parameters.AddWithValue("@RegistDate", NpgsqlDbType.Timestamp, nowDate)
                                cmd.Parameters.AddWithValue("@ModifyDate", NpgsqlDbType.Timestamp, nowDate)
                                cmd.ExecuteNonQuery()
                            End If
                        End If
                    End Using

                    trans.Commit()
                Catch ex As Exception
                    trans.Rollback()
                    Throw
                End Try
            End Using
        End Using

        Return plan

    End Function

    Public Shared Function Save(planList As List(Of Plan)) As List(Of Plan)

        Dim nowDate As Date = Now

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Using trans As NpgsqlTransaction = cn.BeginTransaction
                Try
                    Using cmd As NpgsqlCommand = cn.CreateCommand()
                        cmd.Transaction = trans

                        Dim SQLString As String
                        For Each plan As Plan In planList
                            If String.IsNullOrEmpty(plan.PlanId) Then
                                SQLString = "insert into Plans ("
                                SQLString &= " PlanDate"
                                SQLString &= ",ItemId"
                                SQLString &= ",UnitTypeId"
                                SQLString &= ",RegistDate"
                                SQLString &= ",ModifyDate"
                                SQLString &= " ) values ("
                                SQLString &= " @PlanDate"
                                SQLString &= ",@ItemId"
                                SQLString &= ",@UnitTypeId"
                                SQLString &= ",@RegistDate"
                                SQLString &= ",@ModifyDate"
                                SQLString &= ") returning PlanId"
                                cmd.CommandText = SQLString
                                cmd.Parameters.Clear()
                                cmd.Parameters.AddWithValue("@PlanDate", NpgsqlDbType.Date, CDate(plan.PlanDate).Date)
                                cmd.Parameters.AddWithValue("@ItemId", NpgsqlDbType.Integer, CInt(plan.Item.ItemId))
                                cmd.Parameters.AddWithValue("@UnitTypeId", NpgsqlDbType.Integer, CInt(plan.UnitType.UnitTypeId))
                                cmd.Parameters.AddWithValue("@RegistDate", NpgsqlDbType.Timestamp, nowDate)
                                cmd.Parameters.AddWithValue("@ModifyDate", NpgsqlDbType.Timestamp, nowDate)
                                plan.PlanId = CInt(cmd.ExecuteScalar()).ToString()
                            Else
                                SQLString = "update Plans set"
                                SQLString &= " PlanDate = @PlanDate"
                                SQLString &= ",ItemId = @ItemId"
                                SQLString &= ",UnitTypeId = @UnitTypeId"
                                SQLString &= ",ModifyDate = @ModifyDate"
                                SQLString &= " where PlanId = @PlanId"
                                cmd.CommandText = SQLString
                                cmd.Parameters.Clear()
                                cmd.Parameters.AddWithValue("@PlanDate", NpgsqlDbType.Date, CDate(plan.PlanDate).Date)
                                cmd.Parameters.AddWithValue("@ItemId", NpgsqlDbType.Integer, CInt(plan.Item.ItemId))
                                cmd.Parameters.AddWithValue("@UnitTypeId", NpgsqlDbType.Integer, CInt(plan.UnitType.UnitTypeId))
                                cmd.Parameters.AddWithValue("@ModifyDate", NpgsqlDbType.Timestamp, nowDate)
                                cmd.Parameters.AddWithValue("@PlanId", NpgsqlDbType.Integer, CInt(plan.PlanId))
                                cmd.ExecuteNonQuery()
                            End If

                            SQLString = "delete from PlanDetails"
                            SQLString &= " where PlanId = @PlanId"
                            cmd.CommandText = SQLString
                            cmd.Parameters.Clear()
                            cmd.Parameters.AddWithValue("@PlanId", NpgsqlDbType.Integer, CInt(plan.PlanId))
                            cmd.ExecuteNonQuery()

                            Dim sumQuantity As Double = 0
                            For Each detail As PlanDetail In plan.PlanDetails
                                SQLString = "insert into PlanDetails ("
                                SQLString &= " PlanId"
                                SQLString &= ",StoreId"
                                SQLString &= ",Quantity"
                                SQLString &= ",RegistDate"
                                SQLString &= ",ModifyDate"
                                SQLString &= ") values ("
                                SQLString &= " @PlanId"
                                SQLString &= ",@StoreId"
                                SQLString &= ",@Quantity"
                                SQLString &= ",@RegistDate"
                                SQLString &= ",@ModifyDate"
                                SQLString &= ") returning PlanDetailId"
                                cmd.CommandText = SQLString
                                cmd.Parameters.Clear()
                                cmd.Parameters.AddWithValue("@PlanId", NpgsqlDbType.Integer, CInt(plan.PlanId))
                                cmd.Parameters.AddWithValue("@StoreId", NpgsqlDbType.Integer, CInt(detail.Store.StoreId))
                                cmd.Parameters.AddWithValue("@Quantity", NpgsqlDbType.Double, CDbl(detail.Quantity))
                                cmd.Parameters.AddWithValue("@RegistDate", NpgsqlDbType.Timestamp, nowDate)
                                cmd.Parameters.AddWithValue("@ModifyDate", NpgsqlDbType.Timestamp, nowDate)
                                detail.PlanDetailId = CInt(cmd.ExecuteScalar()).ToString()

                                sumQuantity += detail.Quantity
                            Next

                            If plan.InstructionStart Then
                                If Not String.IsNullOrEmpty(plan.PlanId) AndAlso IsNothing(Instructions.GetByPlanId(plan.PlanId)) Then
                                    SQLString = "insert into Instructions ("
                                    SQLString &= " StartDate"
                                    SQLString &= ",FinishDate"
                                    SQLString &= ",ItemId"
                                    SQLString &= ",PlanQuantity"
                                    SQLString &= ",RealQuantity"
                                    SQLString &= ",UnitTypeId"
                                    SQLString &= ",PlanId"
                                    SQLString &= ",RegistDate"
                                    SQLString &= ",ModifyDate"
                                    SQLString &= " ) values ("
                                    SQLString &= " @StartDate"
                                    SQLString &= ",@FinishDate"
                                    SQLString &= ",@ItemId"
                                    SQLString &= ",@PlanQuantity"
                                    SQLString &= ",@RealQuantity"
                                    SQLString &= ",@UnitTypeId"
                                    SQLString &= ",@PlanId"
                                    SQLString &= ",@RegistDate"
                                    SQLString &= ",@ModifyDate"
                                    SQLString &= ") returning InstructionId"
                                    cmd.CommandText = SQLString
                                    cmd.Parameters.Clear()
                                    cmd.Parameters.AddWithValue("@StartDate", NpgsqlDbType.Date, CDate(plan.PlanDate).Date)
                                    cmd.Parameters.AddWithValue("@FinishDate", NpgsqlDbType.Date, DBNull.Value)
                                    cmd.Parameters.AddWithValue("@ItemId", NpgsqlDbType.Integer, CInt(plan.Item.ItemId))
                                    cmd.Parameters.AddWithValue("@PlanQuantity", NpgsqlDbType.Double, CDbl(sumQuantity))
                                    cmd.Parameters.AddWithValue("@RealQuantity", NpgsqlDbType.Double, DBNull.Value)
                                    cmd.Parameters.AddWithValue("@UnitTypeId", NpgsqlDbType.Integer, CInt(plan.UnitType.UnitTypeId))
                                    cmd.Parameters.AddWithValue("@PlanId", NpgsqlDbType.Integer, CInt(plan.PlanId))
                                    cmd.Parameters.AddWithValue("@RegistDate", NpgsqlDbType.Timestamp, nowDate)
                                    cmd.Parameters.AddWithValue("@ModifyDate", NpgsqlDbType.Timestamp, nowDate)
                                    cmd.ExecuteNonQuery()
                                End If
                            End If
                        Next

                        trans.Commit()
                    End Using
                Catch ex As Exception
                    trans.Rollback()
                    Throw
                End Try
            End Using
        End Using

        Return planList

    End Function

    Public Shared Sub Delete(plan As Plan)

        Dim nowDate As Date = Now

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Using trans As NpgsqlTransaction = cn.BeginTransaction
                Try
                    Using cmd As NpgsqlCommand = cn.CreateCommand()
                        cmd.Transaction = trans

                        Dim SQLString As String
                        SQLString = "delete from PlanDetails"
                        SQLString &= " where PlanId = @PlanId"
                        cmd.CommandText = SQLString
                        cmd.Parameters.Clear()
                        cmd.Parameters.AddWithValue("@PlanId", NpgsqlDbType.Integer, CInt(plan.PlanId))
                        cmd.ExecuteNonQuery()

                        SQLString = "delete from Plans"
                        SQLString &= " where PlanId = @PlanId"
                        cmd.CommandText = SQLString
                        cmd.Parameters.Clear()
                        cmd.Parameters.AddWithValue("@PlanId", NpgsqlDbType.Integer, CInt(plan.PlanId))
                        cmd.ExecuteNonQuery()
                    End Using

                    trans.Commit()
                Catch ex As Exception
                    trans.Rollback()
                    Throw
                End Try
            End Using
        End Using

    End Sub

End Class

Public Class PlanDetail
    Public Property PlanDetailId As String
    Public Property PlanId As String
    Public Property Store As New Store
    Public Property Quantity As String
End Class

Public Class PlanDetails

    Public Shared Function GetByPlanId(planId As Integer) As List(Of PlanDetail)

        Dim result As New List(Of PlanDetail)

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Try
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String
                    SQLString = "select"
                    SQLString &= " PlanDetails.*"
                    SQLString &= " from PlanDetails"
                    SQLString &= " where PlanId = @PlanId"
                    SQLString &= " order by"
                    SQLString &= " StoreId"
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    cmd.Parameters.AddWithValue("@PlanId", NpgsqlDbType.Integer, planId)
                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        While dr.Read()
                            Dim instructionPlanDetail As PlanDetail = Mapping(dr)
                            result.Add(instructionPlanDetail)
                        End While
                    End Using
                End Using
            Catch ex As Exception
                SysMsgBox(ex.Message, "", Reflection.MethodBase.GetCurrentMethod.Name)
                Throw
            End Try
        End Using

        Return result

    End Function

    Private Shared Function Mapping(dr As NpgsqlDataReader) As PlanDetail

        Dim result As New PlanDetail

        result.PlanDetailId = dr("PlanDetailId").ToString
        result.Store = Stores.GetById(dr("StoreId").ToString)
        result.Quantity = Nz(dr("Quantity"), 0)

        Return result

    End Function

End Class

#End Region

#Region "製造"

' 在庫不足で製造完了できないときに投げる
Public Class ShortageException
    Inherits ApplicationException

    Public ReadOnly Shortages As New List(Of String)

End Class

Public Class Instruction
    Public Property InstructionId As String
    Public Property StartDate As String
    Public Property FinishDate As String
    Public Property Item As New Item
    Public Property PlanQuantity As String
    Public Property RealQuantity As String
    Public Property UnitType As New UnitType
    Public Property InstructionDetails As New List(Of InstructionDetail)
    Public Property PlanId As String
    Public Property InstructionFinish As Boolean '製造完了フラグ
End Class

Public Class Instructions

    Public Shared Function GetAll(Optional where As String = Nothing, Optional orderBy As String = Nothing) As List(Of Instruction)

        Dim result As New List(Of Instruction)

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Try
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String
                    SQLString = "select"
                    SQLString &= " Instructions.*"
                    SQLString &= " from Instructions"
                    SQLString &= " where InstructionId is not null"
                    If Not String.IsNullOrEmpty(where) Then
                        SQLString &= " and " & where
                    End If
                    If Not String.IsNullOrEmpty(orderBy) Then
                        SQLString &= " Order by " & orderBy
                    End If
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        While dr.Read
                            Dim instructionInstruction As Instruction = Mapping(dr)

                            If instructionInstruction IsNot Nothing Then
                                instructionInstruction.InstructionDetails = InstructionDetails.GetByInstructionId(instructionInstruction.InstructionId)
                            End If

                            result.Add(instructionInstruction)
                        End While
                    End Using
                End Using
            Catch ex As Exception
                SysMsgBox(ex.Message, "", Reflection.MethodBase.GetCurrentMethod.Name)
                Throw
            End Try
        End Using

        Return result

    End Function

    Public Shared Function GetById(id As Integer) As Instruction

        Dim result As Instruction = Nothing

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Try
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String
                    SQLString = "select"
                    SQLString &= " Instructions.*"
                    SQLString &= " from Instructions"
                    SQLString &= " where InstructionId = @InstructionId"
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    cmd.Parameters.AddWithValue("@InstructionId", NpgsqlDbType.Integer, id)
                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        If dr.Read() Then result = Mapping(dr)
                    End Using

                    If result IsNot Nothing Then
                        result.InstructionDetails = InstructionDetails.GetByInstructionId(result.InstructionId)
                    End If
                End Using
            Catch ex As Exception
                SysMsgBox(ex.Message, "", Reflection.MethodBase.GetCurrentMethod.Name)
                Throw
            End Try
        End Using

        Return result

    End Function

    Public Shared Function GetByPlanId(planId As Integer) As Instruction

        Dim result As Instruction = Nothing

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Try
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String
                    SQLString = "select"
                    SQLString &= " Instructions.*"
                    SQLString &= " from Instructions"
                    SQLString &= " where PlanId = @PlanId"
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    cmd.Parameters.AddWithValue("@PlanId", NpgsqlDbType.Integer, planId)
                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        If dr.Read() Then result = Mapping(dr)
                    End Using

                    If result IsNot Nothing Then
                        result.InstructionDetails = InstructionDetails.GetByInstructionId(result.InstructionId)
                    End If
                End Using
            Catch ex As Exception
                SysMsgBox(ex.Message, "", Reflection.MethodBase.GetCurrentMethod.Name)
                Throw
            End Try
        End Using

        Return result

    End Function

    Private Shared Function Mapping(dr As NpgsqlDataReader) As Instruction

        Dim result As New Instruction

        result.InstructionId = dr("InstructionId").ToString
        result.StartDate = dr("StartDate").ToString
        result.FinishDate = dr("FinishDate").ToString
        result.Item = Items.GetById(dr("ItemId").ToString)
        result.PlanQuantity = dr("PlanQuantity").ToString
        result.RealQuantity = dr("RealQuantity").ToString
        result.UnitType = UnitTypes.GetById(dr("UnitTypeId").ToString)
        result.PlanId = dr("PlanId").ToString

        Return result

    End Function

    Public Shared Function Save(instructionList As List(Of Instruction), Optional allowShortage As Boolean = False) As List(Of Instruction)

        Dim nowDate As Date = Now

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Using trans As NpgsqlTransaction = cn.BeginTransaction
                Try
                    Using cmd As NpgsqlCommand = cn.CreateCommand()
                        cmd.Transaction = trans

                        Dim SQLString As String
                        For Each instruction As Instruction In instructionList
                            ' 在庫を二重に動かさないため、更新前の状態を先に見る
                            Dim wasFinished As Boolean = IsFinished(cmd, instruction.InstructionId)

                            If instruction.InstructionFinish Then
                                ' 未完了 → 完了 のときだけ在庫を動かす
                                If Not wasFinished Then
                                    Dim cost As Double = ConsumeMaterials(cmd, instruction, nowDate, allowShortage)
                                    AddProductInventory(cmd, instruction, cost, nowDate)
                                End If

                                SQLString = "update Instructions set"
                                SQLString &= " FinishDate = @FinishDate"
                                SQLString &= ",RealQuantity = @RealQuantity"
                                SQLString &= ",ModifyDate = @ModifyDate"
                                SQLString &= " where InstructionId = @InstructionId"
                                cmd.CommandText = SQLString
                                cmd.Parameters.Clear()
                                cmd.Parameters.AddWithValue("@FinishDate", NpgsqlDbType.Date, CDate(instruction.FinishDate).Date)
                                cmd.Parameters.AddWithValue("@RealQuantity", NpgsqlDbType.Double, CDbl(instruction.RealQuantity))
                                cmd.Parameters.AddWithValue("@ModifyDate", NpgsqlDbType.Timestamp, nowDate)
                                cmd.Parameters.AddWithValue("@InstructionId", NpgsqlDbType.Integer, CInt(instruction.InstructionId))
                                cmd.ExecuteNonQuery()

                                SQLString = "delete from InstructionDetails"
                                SQLString &= " where InstructionId = @InstructionId"
                                cmd.CommandText = SQLString
                                cmd.Parameters.Clear()
                                cmd.Parameters.AddWithValue("@InstructionId", NpgsqlDbType.Integer, CInt(instruction.InstructionId))
                                cmd.ExecuteNonQuery()

                                For Each detail As InstructionDetail In instruction.InstructionDetails
                                    SQLString = "insert into InstructionDetails ("
                                    SQLString &= " InstructionId"
                                    SQLString &= ",StoreId"
                                    SQLString &= ",Quantity"
                                    SQLString &= ",RegistDate"
                                    SQLString &= ",ModifyDate"
                                    SQLString &= ") values ("
                                    SQLString &= " @InstructionId"
                                    SQLString &= ",@StoreId"
                                    SQLString &= ",@Quantity"
                                    SQLString &= ",@RegistDate"
                                    SQLString &= ",@ModifyDate"
                                    SQLString &= ") returning InstructionDetailId"
                                    cmd.CommandText = SQLString
                                    cmd.Parameters.Clear()
                                    cmd.Parameters.AddWithValue("@InstructionId", NpgsqlDbType.Integer, CInt(instruction.InstructionId))
                                    cmd.Parameters.AddWithValue("@StoreId", NpgsqlDbType.Integer, CInt(detail.Store.StoreId))
                                    cmd.Parameters.AddWithValue("@Quantity", NpgsqlDbType.Double, CDbl(detail.Quantity))
                                    cmd.Parameters.AddWithValue("@RegistDate", NpgsqlDbType.Timestamp, nowDate)
                                    cmd.Parameters.AddWithValue("@ModifyDate", NpgsqlDbType.Timestamp, nowDate)
                                    detail.InstructionDetailId = CInt(cmd.ExecuteScalar()).ToString()
                                Next
                            Else
                                ' 完了 → 未完了 のときは在庫を元に戻す
                                If wasFinished Then
                                    RevertProduction(cmd, instruction, nowDate)
                                End If

                                SQLString = "delete from InstructionDetails"
                                SQLString &= " where InstructionId = @InstructionId"
                                cmd.CommandText = SQLString
                                cmd.Parameters.Clear()
                                cmd.Parameters.AddWithValue("@InstructionId", NpgsqlDbType.Integer, CInt(instruction.InstructionId))
                                cmd.ExecuteNonQuery()

                                SQLString = "update Instructions set"
                                SQLString &= " FinishDate = @FinishDate"
                                SQLString &= ",RealQuantity = @RealQuantity"
                                SQLString &= ",ModifyDate = @ModifyDate"
                                SQLString &= " where InstructionId = @InstructionId"
                                cmd.CommandText = SQLString
                                cmd.Parameters.Clear()
                                cmd.Parameters.AddWithValue("@FinishDate", NpgsqlDbType.Date, DBNull.Value)
                                cmd.Parameters.AddWithValue("@RealQuantity", NpgsqlDbType.Double, DBNull.Value)
                                cmd.Parameters.AddWithValue("@ModifyDate", NpgsqlDbType.Timestamp, nowDate)
                                cmd.Parameters.AddWithValue("@InstructionId", NpgsqlDbType.Integer, CInt(instruction.InstructionId))
                                cmd.ExecuteNonQuery()
                            End If
                        Next

                        trans.Commit()
                    End Using
                Catch ex As Exception
                    trans.Rollback()
                    Throw
                End Try
            End Using
        End Using

        Return instructionList

    End Function

    Public Shared Sub Delete(Instruction As Instruction)

        Dim nowDate As Date = Now

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Using trans As NpgsqlTransaction = cn.BeginTransaction
                Try
                    Using cmd As NpgsqlCommand = cn.CreateCommand()
                        cmd.Transaction = trans

                        ' 在庫を元に戻してから削除する
                        RevertProduction(cmd, Instruction, nowDate)

                        Dim SQLString As String
                        SQLString = "delete from InstructionDetails"
                        SQLString &= " where InstructionId = @InstructionId"
                        cmd.CommandText = SQLString
                        cmd.Parameters.Clear()
                        cmd.Parameters.AddWithValue("@InstructionId", NpgsqlDbType.Integer, CInt(Instruction.InstructionId))
                        cmd.ExecuteNonQuery()

                        SQLString = "delete from Instructions"
                        SQLString &= " where InstructionId = @InstructionId"
                        cmd.CommandText = SQLString
                        cmd.Parameters.Clear()
                        cmd.Parameters.AddWithValue("@InstructionId", NpgsqlDbType.Integer, CInt(Instruction.InstructionId))
                        cmd.ExecuteNonQuery()
                    End Using

                    trans.Commit()
                Catch ex As Exception
                    trans.Rollback()
                    Throw
                End Try
            End Using
        End Using

    End Sub

    ' すでに製造完了になっているか(在庫を二重に動かさないための判定)
    Private Shared Function IsFinished(cmd As NpgsqlCommand, instructionId As Integer) As Boolean

        If String.IsNullOrEmpty(instructionId) Then Return False

        Dim SQLString As String
        SQLString = "select"
        SQLString &= " count(*)"
        SQLString &= " from Instructions"
        SQLString &= " where InstructionId = @InstructionId"
        SQLString &= " and FinishDate is not null"
        cmd.CommandText = SQLString
        cmd.Parameters.Clear()
        cmd.Parameters.AddWithValue("@InstructionId", NpgsqlDbType.Integer, instructionId)

        Return CInt(cmd.ExecuteScalar()) > 0

    End Function

    ' 製造で材料を払い出す。払い出した金額の合計を返す
    Private Shared Function ConsumeMaterials(cmd As NpgsqlCommand, instruction As Instruction, nowDate As Date, allowShortage As Boolean) As Double

        Dim quantity As Double = CDbl(Nz(instruction.RealQuantity, 0))
        If quantity <= 0 Then Return 0

        ' recursive:=False で直下の材料だけを払い出す
        ' 半製品は自分の製造完了で在庫になっているので、その在庫から引き当てる
        Dim materials As List(Of MaterialCalculator.Material) =
            calc.CalcRequiredMaterials(CInt(instruction.Item.ItemId),
                                       quantity,
                                       CInt(instruction.UnitType.UnitTypeId),
                                       False)

        Dim totalCost As Double = 0
        Dim ex As New ShortageException

        For Each material As MaterialCalculator.Material In materials
            Dim shortage As Double = 0
            totalCost += ConsumeLots(cmd, instruction, material, nowDate, allowShortage, shortage)

            If shortage > 0 Then
                ex.Shortages.Add(material.ItemName & "  不足 " &
                                 shortage.ToString("#,##0.##") &
                                 calc.GetUnitTypeName(calc.GetBaseUnitTypeId(material.ItemId)))
            End If
        Next

        ' 強制でなければ中断する。減らした在庫はRollbackで戻る
        If ex.Shortages.Count > 0 AndAlso Not allowShortage Then Throw ex

        Return totalCost

    End Function

    ' 賞味期限の古いロットから在庫を減らす。払い出した金額を返す
    Private Shared Function ConsumeLots(cmd As NpgsqlCommand, instruction As Instruction, material As MaterialCalculator.Material, nowDate As Date, allowShortage As Boolean, ByRef shortage As Double) As Double

        ' 必要量を在庫の基準単位に揃える
        Dim baseUnitId As Integer = calc.GetBaseUnitTypeId(material.ItemId)
        Dim remain As Double = material.Quantity
        Dim converted As Double
        If calc.TryConvert(material.Quantity, material.UnitTypeId, baseUnitId, converted) Then remain = converted
        If remain <= 0 Then Return 0

        ' 引き当て対象のロットを先に読み切る(リーダーを開いたままでは更新できない)
        Dim lotIdList As New List(Of String)
        Dim lotWeightList As New List(Of Double)
        Dim lotPriceList As New List(Of Double)

        Dim SQLString As String
        SQLString = "select"
        SQLString &= " InventoryId"
        SQLString &= ",Weight"
        SQLString &= ",PricePerGram"
        SQLString &= " from Inventories"
        SQLString &= " where ItemId = @ItemId"
        SQLString &= " and Weight > 0"
        SQLString &= " order by"
        SQLString &= " case"
        SQLString &= "   when ExpiryDate is null then 1"
        SQLString &= "   else 0"
        SQLString &= " end"
        SQLString &= ",ExpiryDate"
        SQLString &= ",ArrivalDate"
        cmd.CommandText = SQLString
        cmd.Parameters.Clear()
        cmd.Parameters.AddWithValue("@ItemId", NpgsqlDbType.Integer, CInt(material.ItemId))
        Using dr As NpgsqlDataReader = cmd.ExecuteReader()
            While dr.Read
                lotIdList.Add(dr("InventoryId").ToString)
                lotWeightList.Add(CDbl(Nz(dr("Weight"), 0)))
                lotPriceList.Add(CDbl(Nz(dr("PricePerGram"), 0)))
            End While
        End Using

        Dim cost As Double = 0
        For i As Integer = 0 To lotIdList.Count - 1
            If remain <= 0 Then Exit For

            Dim take As Double = Math.Min(remain, lotWeightList(i))
            remain -= take
            cost += take * lotPriceList(i)

            ' 在庫を減らす
            SQLString = "update Inventories set"
            SQLString &= " Weight = @Weight"
            SQLString &= ",ModifyDate = @ModifyDate"
            SQLString &= " where InventoryId = @InventoryId"
            cmd.CommandText = SQLString
            cmd.Parameters.Clear()
            cmd.Parameters.AddWithValue("@Weight", NpgsqlDbType.Double, CDbl(lotWeightList(i) - take))
            cmd.Parameters.AddWithValue("@ModifyDate", NpgsqlDbType.Timestamp, nowDate)
            cmd.Parameters.AddWithValue("@InventoryId", NpgsqlDbType.Integer, CInt(lotIdList(i)))
            cmd.ExecuteNonQuery()

            InventoryLogs.Write(cmd, InventoryLogStatus.Consume,
                                material.ItemId.ToString, lotIdList(i),
                                -take, lotPriceList(i),
                                instruction.InstructionId, nowDate)

            ' 製造完了を取り消せるように、払い出した明細を残す
            InsertConsumption(cmd, instruction, material.ItemId, lotIdList(i), take, baseUnitId, lotPriceList(i), nowDate)
        Next

        ' 在庫が足りなかった分
        If remain > 0 Then
            shortage = remain

            If allowShortage Then
                ' どのロットからも引けなかった量をInventoryId=Nullで記録する
                Dim unitPrice As Double = GetFallbackUnitPrice(cmd, material.ItemId)
                cost += remain * unitPrice
                InsertConsumption(cmd, instruction, material.ItemId, Nothing, remain, baseUnitId, unitPrice, nowDate)
            End If
        End If

        Return cost

    End Function

    ' 払い出し明細を1行残す。inventoryIdが空なら在庫不足分
    Private Shared Sub InsertConsumption(cmd As NpgsqlCommand, instruction As Instruction, itemId As Integer, inventoryId As String, quantity As Double, unitTypeId As Integer, unitPrice As Double, nowDate As Date)

        Dim SQLString As String
        SQLString = "insert into InstructionConsumptions ("
        SQLString &= " InstructionId"
        SQLString &= ",InventoryId"
        SQLString &= ",ItemId"
        SQLString &= ",Quantity"
        SQLString &= ",UnitTypeId"
        SQLString &= ",UnitPrice"
        SQLString &= ",RegistDate"
        SQLString &= ",ModifyDate"
        SQLString &= " ) values ("
        SQLString &= " @InstructionId"
        SQLString &= ",@InventoryId"
        SQLString &= ",@ItemId"
        SQLString &= ",@Quantity"
        SQLString &= ",@UnitTypeId"
        SQLString &= ",@UnitPrice"
        SQLString &= ",@RegistDate"
        SQLString &= ",@ModifyDate"
        SQLString &= ")"
        cmd.CommandText = SQLString
        cmd.Parameters.Clear()
        cmd.Parameters.AddWithValue("@InstructionId", NpgsqlDbType.Integer, CInt(instruction.InstructionId))
        If String.IsNullOrEmpty(inventoryId) Then
            cmd.Parameters.AddWithValue("@InventoryId", NpgsqlDbType.Integer, DBNull.Value)
        Else
            cmd.Parameters.AddWithValue("@InventoryId", NpgsqlDbType.Integer, CInt(inventoryId))
        End If
        cmd.Parameters.AddWithValue("@ItemId", NpgsqlDbType.Integer, itemId)
        cmd.Parameters.AddWithValue("@Quantity", NpgsqlDbType.Double, CDbl(quantity))
        cmd.Parameters.AddWithValue("@UnitTypeId", NpgsqlDbType.Integer, unitTypeId)
        cmd.Parameters.AddWithValue("@UnitPrice", NpgsqlDbType.Double, CDbl(unitPrice))
        cmd.Parameters.AddWithValue("@RegistDate", NpgsqlDbType.Timestamp, nowDate)
        cmd.Parameters.AddWithValue("@ModifyDate", NpgsqlDbType.Timestamp, nowDate)
        cmd.ExecuteNonQuery()

    End Sub

    ' 在庫が無い材料の単価は直近の入荷単価で代用する
    Private Shared Function GetFallbackUnitPrice(cmd As NpgsqlCommand, itemId As Integer) As Double

        Dim SQLString As String
        SQLString = "select"
        SQLString &= " PricePerGram"
        SQLString &= " from Inventories"
        SQLString &= " where ItemId = @ItemId"
        SQLString &= " and PricePerGram > 0"
        SQLString &= " order by"
        SQLString &= " ArrivalDate desc"
        SQLString &= " limit 1"
        cmd.CommandText = SQLString
        cmd.Parameters.Clear()
        cmd.Parameters.AddWithValue("@ItemId", NpgsqlDbType.Integer, itemId)

        Return CDbl(Nz(cmd.ExecuteScalar(), 0))

    End Function

    ' 製造した製品を在庫に入れる
    Private Shared Sub AddProductInventory(cmd As NpgsqlCommand, instruction As Instruction, cost As Double, nowDate As Date)

        Dim quantity As Double = CDbl(Nz(instruction.RealQuantity, 0))
        If quantity <= 0 Then Return

        ' 在庫の基準単位に換算した数量を Weight に入れる
        Dim baseUnitId As Integer = calc.GetBaseUnitTypeId(CInt(instruction.Item.ItemId))
        Dim weight As Double = quantity
        Dim converted As Double
        If calc.TryConvert(quantity, CInt(instruction.UnitType.UnitTypeId), baseUnitId, converted) Then weight = converted

        ' 実際に払い出した材料費から基準単位あたりの単価を出す
        Dim pricePerGram As Object = DBNull.Value
        If weight > 0 AndAlso cost > 0 Then pricePerGram = CeilingPricePerUnit(cost / weight)

        Dim SQLString As String
        SQLString = "insert into Inventories ("
        SQLString &= " ItemId"
        SQLString &= ",ArrivalDate"
        SQLString &= ",Quantity"
        SQLString &= ",UnitTypeId"
        SQLString &= ",Weight"
        SQLString &= ",PricePerGram"
        SQLString &= ",InstructionId"
        SQLString &= ",RegistDate"
        SQLString &= ",ModifyDate"
        SQLString &= " ) values ("
        SQLString &= " @ItemId"
        SQLString &= ",@ArrivalDate"
        SQLString &= ",@Quantity"
        SQLString &= ",@UnitTypeId"
        SQLString &= ",@Weight"
        SQLString &= ",@PricePerGram"
        SQLString &= ",@InstructionId"
        SQLString &= ",@RegistDate"
        SQLString &= ",@ModifyDate"
        SQLString &= ") returning InventoryId"
        cmd.CommandText = SQLString
        cmd.Parameters.Clear()
        cmd.Parameters.AddWithValue("@ItemId", NpgsqlDbType.Integer, CInt(instruction.Item.ItemId))
        cmd.Parameters.AddWithValue("@ArrivalDate", NpgsqlDbType.Date, CDate(instruction.FinishDate).Date)
        cmd.Parameters.AddWithValue("@Quantity", NpgsqlDbType.Double, CDbl(quantity))
        cmd.Parameters.AddWithValue("@UnitTypeId", NpgsqlDbType.Integer, CInt(instruction.UnitType.UnitTypeId))
        cmd.Parameters.AddWithValue("@Weight", NpgsqlDbType.Double, CDbl(weight))
        If Not IsNothing(Nz(pricePerGram, Nothing)) Then
            cmd.Parameters.AddWithValue("@PricePerGram", NpgsqlDbType.Double, CDbl(pricePerGram))
        Else
            cmd.Parameters.AddWithValue("@PricePerGram", NpgsqlDbType.Double, DBNull.Value)
        End If
        cmd.Parameters.AddWithValue("@InstructionId", NpgsqlDbType.Integer, CInt(instruction.InstructionId))
        cmd.Parameters.AddWithValue("@RegistDate", NpgsqlDbType.Timestamp, nowDate)
        cmd.Parameters.AddWithValue("@ModifyDate", NpgsqlDbType.Timestamp, nowDate)
        Dim inventoryId As String = CInt(cmd.ExecuteScalar()).ToString()

        InventoryLogs.Write(cmd, InventoryLogStatus.Produce,
                            instruction.Item.ItemId, inventoryId,
                            weight, pricePerGram,
                            instruction.InstructionId, nowDate)

    End Sub

    ' 製造完了を取り消して在庫を元に戻す
    Private Shared Sub RevertProduction(cmd As NpgsqlCommand, instruction As Instruction, nowDate As Date)

        Dim SQLString As String

        ' この製造で作った在庫を調べる
        Dim productInventoryId As String = String.Empty
        Dim productWeight As Double = 0
        SQLString = "select"
        SQLString &= " InventoryId"
        SQLString &= ",Weight"
        SQLString &= " from Inventories"
        SQLString &= " where InstructionId = @InstructionId"
        cmd.CommandText = SQLString
        cmd.Parameters.Clear()
        cmd.Parameters.AddWithValue("@InstructionId", NpgsqlDbType.Integer, CInt(instruction.InstructionId))
        Using dr As NpgsqlDataReader = cmd.ExecuteReader()
            If dr.Read() Then
                productInventoryId = dr("InventoryId").ToString
                productWeight = CDbl(Nz(dr("Weight"), 0))
            End If
        End Using

        If Not String.IsNullOrEmpty(productInventoryId) Then
            ' 作った製品を既に別の製造で使っていたら取り消せない
            SQLString = "select"
            SQLString &= " count(*)"
            SQLString &= " from InstructionConsumptions"
            SQLString &= " where InventoryId = @InventoryId"
            cmd.CommandText = SQLString
            cmd.Parameters.Clear()
            cmd.Parameters.AddWithValue("@InventoryId", NpgsqlDbType.Integer, CInt(productInventoryId))
            If CInt(cmd.ExecuteScalar()) > 0 Then
                Throw New ApplicationException(instruction.Item.ItemName &
                                               "は別の製造で使用済みのため、製造完了を取り消せません。")
            End If

            ' 作った製品の在庫を消す
            SQLString = "delete from Inventories"
            SQLString &= " where InventoryId = @InventoryId"
            cmd.CommandText = SQLString
            cmd.Parameters.Clear()
            cmd.Parameters.AddWithValue("@InventoryId", NpgsqlDbType.Integer, CInt(productInventoryId))
            cmd.ExecuteNonQuery()

            ' 消した製品在庫を記録する(削除前の残高を使う)
            InventoryLogs.Write(cmd, InventoryLogStatus.ProduceCancel,
                                instruction.Item.ItemId, Nothing,
                                -productWeight, DBNull.Value,
                                instruction.InstructionId, nowDate)
        End If

        ' 払い出した材料を元のロットに戻す(不足分の行は戻し先が無いので除く)
        Dim lotIdList As New List(Of String)
        Dim lotQuantityList As New List(Of Double)
        Dim lotItemIdList As New List(Of String)
        Dim lotPriceList As New List(Of Double)

        SQLString = "select"
        SQLString &= " InventoryId"
        SQLString &= ",ItemId"
        SQLString &= ",Quantity"
        SQLString &= ",UnitPrice"
        SQLString &= " from InstructionConsumptions"
        SQLString &= " where InstructionId = @InstructionId"
        SQLString &= " and InventoryId is not null"
        cmd.CommandText = SQLString
        cmd.Parameters.Clear()
        cmd.Parameters.AddWithValue("@InstructionId", NpgsqlDbType.Integer, CInt(instruction.InstructionId))
        Using dr As NpgsqlDataReader = cmd.ExecuteReader()
            While dr.Read
                lotIdList.Add(dr("InventoryId").ToString)
                lotItemIdList.Add(dr("ItemId").ToString)
                lotQuantityList.Add(CDbl(Nz(dr("Quantity"), 0)))
                lotPriceList.Add(CDbl(Nz(dr("UnitPrice"), 0)))
            End While
        End Using

        For i As Integer = 0 To lotIdList.Count - 1
            ' 使い切ったロットは Weight が 0 なので足し戻せば復活する
            ' 入荷の修正などでロットが消えていた場合は0件更新になるだけで害はない
            SQLString = "update Inventories set"
            SQLString &= " Weight ="
            SQLString &= " case"
            SQLString &= "   when Weight is null then 0"
            SQLString &= "   else Weight"
            SQLString &= " end"
            SQLString &= " + @Quantity"
            SQLString &= ",ModifyDate = @ModifyDate"
            SQLString &= " where InventoryId = @InventoryId"
            cmd.CommandText = SQLString
            cmd.Parameters.Clear()
            cmd.Parameters.AddWithValue("@Quantity", NpgsqlDbType.Double, lotQuantityList(i))
            cmd.Parameters.AddWithValue("@ModifyDate", NpgsqlDbType.Timestamp, nowDate)
            cmd.Parameters.AddWithValue("@InventoryId", NpgsqlDbType.Integer, CInt(lotIdList(i)))
            cmd.ExecuteNonQuery()

            ' 戻した材料を記録する
            InventoryLogs.Write(cmd, InventoryLogStatus.ProduceCancel,
                                lotItemIdList(i), lotIdList(i),
                                lotQuantityList(i), lotPriceList(i),
                                instruction.InstructionId, nowDate)
        Next

        ' 払い出し明細を消す
        SQLString = "delete from InstructionConsumptions"
        SQLString &= " where InstructionId = @InstructionId"
        cmd.CommandText = SQLString
        cmd.Parameters.Clear()
        cmd.Parameters.AddWithValue("@InstructionId", NpgsqlDbType.Integer, CInt(instruction.InstructionId))
        cmd.ExecuteNonQuery()

    End Sub

End Class

Public Class InstructionDetail
    Public Property InstructionDetailId As String
    Public Property InstructionId As String
    Public Property Store As New Store
    Public Property Quantity As String
End Class

Public Class InstructionDetails

    Public Shared Function GetByInstructionId(InstructionId As Integer) As List(Of InstructionDetail)

        Dim result As New List(Of InstructionDetail)

        Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
            cn.Open()
            Try
                Using cmd As NpgsqlCommand = cn.CreateCommand()
                    Dim SQLString As String
                    SQLString = "select"
                    SQLString &= " InstructionDetails.*"
                    SQLString &= " from InstructionDetails"
                    SQLString &= " where InstructionId = @InstructionId"
                    SQLString &= " order by"
                    SQLString &= " StoreId"
                    cmd.CommandText = SQLString
                    cmd.Parameters.Clear()
                    cmd.Parameters.AddWithValue("@InstructionId", NpgsqlDbType.Integer, InstructionId)
                    Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                        While dr.Read()
                            Dim instructionInstructionDetail As InstructionDetail = Mapping(dr)
                            result.Add(instructionInstructionDetail)
                        End While
                    End Using
                End Using
            Catch ex As Exception
                SysMsgBox(ex.Message, "", Reflection.MethodBase.GetCurrentMethod.Name)
                Throw
            End Try
        End Using

        Return result

    End Function

    Private Shared Function Mapping(dr As NpgsqlDataReader) As InstructionDetail

        Dim result As New InstructionDetail

        result.InstructionDetailId = dr("InstructionDetailId").ToString
        result.Store = Stores.GetById(dr("StoreId").ToString)
        result.Quantity = Nz(dr("Quantity"), 0)

        Return result

    End Function

End Class

#End Region

#Region "計量区分"

Public Class MeasureTypes

    Public Shared Sub BindComboBox(targetComboBox As Object, Optional addItemEmpty As Boolean = False, Optional where As String = "")

        Dim dtMeasureTypes As New DataTable()
        dtMeasureTypes.Columns.Add("MeasureTypeId", GetType(String))
        dtMeasureTypes.Columns.Add("MeasureTypeName", GetType(String))

        If addItemEmpty Then dtMeasureTypes.Rows.Add(DBNull.Value, "")
        For Each item As KeyValuePair(Of MeasureType, String) In Common.MeasureTypes
            dtMeasureTypes.Rows.Add(CInt(item.Key).ToString(), item.Value)
        Next
        dtMeasureTypes.AcceptChanges()

        With targetComboBox
            .DisplayMember = "MeasureTypeName"
            .ValueMember = "MeasureTypeId"
            .DataSource = dtMeasureTypes
        End With

    End Sub

End Class

#End Region
