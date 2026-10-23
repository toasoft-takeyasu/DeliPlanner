Public Class DialogPlan

#Region "定義"

    Public Item As New Item
    Public Property BaseControl As Control

    Public Property InitialQuantity As Double = 0
    Public Property InitialUnitTypeName As String = String.Empty
    Public Property InitialPlanDate As String = String.Empty

#End Region

#Region "コンストラクタ"

#End Region

#Region "イベント"

    Private Sub Me_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        UnitTypes.BindComboBox(ComboBoxUnitType, True, "UnusedFlag = 0")

        ' フォームの初期化
        InitializeForm(True)

    End Sub

    Private Sub Me_Shown(sender As Object, e As EventArgs) Handles Me.Shown

        TextBoxItemId.Text = Item.ItemId
        TextBoxItemName.Text = Item.ItemName
        ComboBoxUnitType.SelectedValue = Item.UnitType.UnitTypeId

        ' 呼び出し元から初期値が渡されていれば反映する
        If Not String.IsNullOrEmpty(InitialPlanDate) Then DateBoxPlanDate.Text = InitialPlanDate
        If Not String.IsNullOrEmpty(InitialUnitTypeName) Then ComboBoxUnitType.Text = InitialUnitTypeName
        If InitialQuantity > 0 Then TextBoxQuantity.Text = InitialQuantity.ToString("0.##")

        TextBoxQuantity.Focus()

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
            If String.IsNullOrEmpty(DateBoxPlanDate.Text) Then
                Throw New ApplicationException("製造予定日を入力してください。")
                DateBoxPlanDate.Focus()
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
            Dim plan As Plan = Plans.GetByKey(DateBoxPlanDate.Text, TextBoxItemId.Text)
            If IsNothing(plan) Then plan = New Plan
            plan.PlanDate = DateBoxPlanDate.Text
            plan.Item.ItemId = TextBoxItemId.Text
            plan.UnitType.UnitTypeId = ComboBoxUnitType.SelectedValue

            Dim planDetail As New PlanDetail
            planDetail.Store.StoreId = Stores.GetAll("CenterFlag = 1")(0).StoreId
            planDetail.Quantity = TextBoxQuantity.Text
            plan.PlanDetails.Add(planDetail)

            Plans.Save(plan)
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try

    End Sub

    Private Sub InitializeForm(Optional isLoading As Boolean = False)

        TextBoxItemId.Text = String.Empty
        TextBoxItemName.Text = String.Empty
        DateBoxPlanDate.Text = Now.ToString("yyyy/MM/dd")
        TextBoxQuantity.Text = String.Empty
        ComboBoxUnitType.Text = String.Empty

        If Not isLoading Then TextBoxQuantity.Focus()

    End Sub

#End Region

End Class
