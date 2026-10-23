Public Class DialogOrder

#Region "定義"

    Public Item As New Item
    Public Property BaseControl As Control

#End Region

#Region "コンストラクタ"

#End Region

#Region "イベント"

    Private Sub Me_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        UnitTypes.BindComboBox(ComboBoxUnitType, True, "UnusedFlag = 0")
        UnitTypes.BindComboBox(ComboBoxNetWeightUnitType, True, "UnusedFlag = 0 and NonConvertibleFlag = 0")

        ' フォームの初期化
        InitializeForm(True)

    End Sub

    Private Sub Me_Shown(sender As Object, e As EventArgs) Handles Me.Shown

        TextBoxItemTypeId.Text = Item.ItemTypeId
        TextBoxItemId.Text = Item.ItemId
        TextBoxItemName.Text = Item.ItemName
        If Not IsNothing(Item.MainSupplier) Then
            TextBoxSupplierId.Text = Item.MainSupplier.SupplierId
            TextBoxSupplierName.Text = Item.MainSupplier.SupplierName
        End If
        ComboBoxUnitType.SelectedValue = Item.UnitType.UnitTypeId

        If String.IsNullOrEmpty(TextBoxSupplierName.Text) Then
            ButtonSearchSupplier.Focus()
        Else
            TextBoxQuantity.Focus()
        End If

    End Sub

    Private Sub ButtonSearchSupplier_Click(sender As Object, e As EventArgs) Handles ButtonSearchSupplier.Click

        Dim dialog As New DialogSearchSuppliers
        With dialog
            .ItemTypeFilter = "RecipeNeedFlag = 0"
            .InitialSelectItemType = ItemTypes.GetById(TextBoxItemTypeId.Text)

            Dim targetBounds As Rectangle = BaseControl.RectangleToScreen(BaseControl.ClientRectangle)
            .StartPosition = FormStartPosition.Manual
            .Height = targetBounds.Height
            .Location = New Point(targetBounds.Right - .Width, targetBounds.Bottom - .Height)

            If .ShowDialog = DialogResult.OK Then
                TextBoxSupplierId.Text = .Supplier.SupplierId
                TextBoxSupplierName.Text = .Supplier.SupplierName
            End If
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
        End If

        Me.DialogResult = DialogResult.OK
        Me.Close()

    End Sub

    Private Sub ButtonClose_Click(sender As Object, e As EventArgs) Handles ButtonClose.Click

        Me.DialogResult = DialogResult.Cancel
        Me.Close()

    End Sub

#End Region

#Region "メソッド"

    Private Function CheckData() As Boolean

        Dim result As Boolean = True

        Try
            If String.IsNullOrEmpty(DateBoxOrderDate.Text) Then
                Throw New ApplicationException("発注日を入力してください。")
                DateBoxOrderDate.Focus()
            End If
            If String.IsNullOrEmpty(DateBoxArrivalPlanDate.Text) Then
                Throw New ApplicationException("入荷予定日を入力してください。")
                DateBoxArrivalPlanDate.Focus()
            End If
            If String.IsNullOrEmpty(TextBoxSupplierName.Text) Then
                Throw New ApplicationException("仕入先を選択してください。")
                ButtonSearchSupplier.Focus()
            End If
            If String.IsNullOrEmpty(TextBoxQuantity.Text) Then
                Throw New ApplicationException("数量を入力してください。")
                TextBoxQuantity.Focus()
            End If
            If String.IsNullOrEmpty(ComboBoxUnitType.Text) Then
                Throw New ApplicationException("単位を選択してください。")
                ComboBoxUnitType.Focus()
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
            Dim order As Order = Orders.GetByKey(DateBoxOrderDate.Text, TextBoxSupplierId.Text)
            If IsNothing(order) Then order = New Order
            order.OrderDate = DateBoxOrderDate.Text
            order.ArrivalPlanDate = DateBoxArrivalPlanDate.Text
            order.Supplier.SupplierId = TextBoxSupplierId.Text

            Dim orderDetail As New OrderDetail
            orderDetail.DetailNo = order.OrderDetails.Count + 1
            orderDetail.Item = Item
            If Not String.IsNullOrEmpty(TextBoxNeyWeight.Text) Then
                orderDetail.NetWeight = TextBoxNeyWeight.Text
            End If
            If Not String.IsNullOrEmpty(Nz(ComboBoxNetWeightUnitType.SelectedValue, "")) Then
                orderDetail.NetWeightUnitType.UnitTypeId = ComboBoxNetWeightUnitType.SelectedValue
            End If
            If Not String.IsNullOrEmpty(TextBoxPerCase.Text) Then
                orderDetail.PerCase = TextBoxPerCase.Text
            End If
            orderDetail.Quantity = TextBoxQuantity.Text
            orderDetail.UnitType.UnitTypeId = ComboBoxUnitType.SelectedValue
            order.OrderDetails.Add(orderDetail)

            Orders.Save(order)
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try

    End Sub

    Private Sub InitializeForm(Optional isLoading As Boolean = False)

        TextBoxItemId.Text = String.Empty
        TextBoxItemName.Text = String.Empty
        DateBoxOrderDate.Text = Now.ToString("yyyy/MM/dd")
        DateBoxArrivalPlanDate.Text = Now.AddDays(1).ToString("yyyy/MM/dd")
        TextBoxSupplierId.Text = String.Empty
        TextBoxSupplierName.Text = String.Empty
        TextBoxNeyWeight.Text = String.Empty
        ComboBoxNetWeightUnitType.Text = String.Empty
        TextBoxPerCase.Text = String.Empty
        TextBoxQuantity.Text = String.Empty
        ComboBoxUnitType.Text = String.Empty

        If Not isLoading Then TextBoxQuantity.Focus()

    End Sub

#End Region

End Class
