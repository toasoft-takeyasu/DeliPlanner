Public Class FormMenu

#Region "定義"

    Public currentButton As Button = Nothing
    Private currentForm As Form = Nothing

    Private systemTimer As New System.Windows.Forms.Timer

#End Region

#Region "イベント"

    Private Sub Me_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        If System.IO.File.Exists(".\logo.png") Then
            PictureBoxLogo.Image = Image.FromFile(".\logo.png")
        End If
        PictureBoxLogo.Cursor = Cursors.Hand

        ' 表示用タイマを起動
        AddHandler systemTimer.Tick, AddressOf SystemTimer_Tick
        systemTimer.Interval = 100
        systemTimer.Start()

        hideSubMenu()

        If sharedIniFile.EnableLogin Then
            ' ログイン中の担当者を反映
            ApplyOperator()
        Else
            ToolStripMain.Items.Remove(ToolStripButtonChangePassword)
            StatusStrip1.Items.Remove(ToolStripStatusLabelOperator)
            PanelMaintenance.Controls.Remove(ButtonEditOperators)
        End If

        ' 残ったボタンの高さにサブメニューを合わせる
        ArrangeSubMenu(PanelMaintenance)

    End Sub

    Private Sub Me_Shown( sender As Object,  e As System.EventArgs) Handles Me.Shown

        Me.Refresh()

        ButtonDashBoard.PerformClick()

        SystemTimer_Tick(Nothing, Nothing)

    End Sub

    Private Sub SystemTimer_Tick( sender As Object,  e As System.EventArgs)

        ' ステータスバーに日時表示
        ToolStripStatusLabelNow.Text = Format(Now, "yyyy/MM/dd HH:mm")

    End Sub

    Private Sub ToolStripButtonRefresh_Click(sender As Object, e As EventArgs) Handles ToolStripButtonRefresh.Click

        Dim target As IRefreshable = TryCast(currentForm, IRefreshable)
        If target Is Nothing Then Return

        Dim beforeCursor As Cursor = Me.Cursor
        Me.Cursor = Cursors.WaitCursor
        Try
            ' 他の端末が変更したマスタと在庫を取り込む
            InvalidateCalc()
            target.RefreshView()
        Catch ex As Exception
            SysMsgBox(ex.Message, Me.Text, Reflection.MethodBase.GetCurrentMethod.Name)
        Finally
            Me.Cursor = beforeCursor
        End Try

    End Sub

    Private Sub ToolStripButtonChangePassword_Click(sender As Object, e As EventArgs) Handles ToolStripButtonChangePassword.Click

        If sharedOperator Is Nothing Then Exit Sub

        Using dialog As New FormChangePassword(sharedOperator)
            dialog.ShowDialog(Me)
        End Using

    End Sub

    Private Sub ToolStripButtonExit_Click(sender As Object, e As EventArgs) Handles ToolStripButtonExit.Click

        ButtonExit.PerformClick()

    End Sub

    Private Sub PictureBoxLogo_Click(sender As Object, e As EventArgs) Handles PictureBoxLogo.Click

        Dim message As String = My.Application.Info.ProductName
        If message = "" Then message = "DeliPlanner"
        message &= vbCrLf & "バージョン：" & My.Application.Info.Version.ToString()
        If My.Application.Info.CompanyName <> "" Then
            message &= vbCrLf & vbCrLf & My.Application.Info.CompanyName
        End If
        If My.Application.Info.Copyright <> "" Then
            message &= vbCrLf & My.Application.Info.Copyright
        End If

        MessageBox.Show(Me, message, "バージョン情報", MessageBoxButtons.OK, MessageBoxIcon.Information)

    End Sub

    Private Sub ButtonOrderForecasts_Click(sender As Object, e As EventArgs) Handles ButtonOrderForecasts.Click

        openChildForm(sender, New FormOrderForecasts(Me))

    End Sub

    Private Sub ButtonInstructionPlans_Click(sender As Object, e As EventArgs) Handles ButtonInstructionPlans.Click

        openChildForm(sender, New FormPlans(Me))

    End Sub

    Private Sub ButtonInstructions_Click(sender As Object, e As EventArgs) Handles ButtonInstructions.Click

        openChildForm(sender, New FormInstructions(Me))

    End Sub

    Private Sub ButtonOrders_Click(sender As Object, e As EventArgs) Handles ButtonOrders.Click

        openChildForm(sender, New FormOrders(Me))

    End Sub

    Private Sub ButtonArrivals_Click(sender As Object, e As EventArgs) Handles ButtonArrivals.Click

        openChildForm(sender, New FormArrivals(Me))

    End Sub

    Private Sub ButtonButtonAjustInventory_Click(sender As Object, e As EventArgs) Handles ButtonAjustInventory.Click

        openChildForm(sender, New FormChangeInventory(Me, ButtonAjustInventory.Text))

    End Sub

    Private Sub ButtonInventoryDisposals_Click(sender As Object, e As EventArgs) Handles ButtonInventoryDisposals.Click

        openChildForm(sender, New FormInventoryDisposals())

    End Sub

    Private Sub ButtonTakeInventoriy_Click(sender As Object, e As EventArgs) Handles ButtonTakeInventoriy.Click

        openChildForm(sender, New FormChangeInventory(Me, ButtonTakeInventoriy.Text))

    End Sub

    Private Sub ButtonMaintenance_Click(sender As Object, e As EventArgs) Handles ButtonMaintenance.Click

        showSubMenu(PanelMaintenance)

    End Sub

    Private Sub ButtonEditItems_Click(sender As Object, e As EventArgs) Handles ButtonEditItems.Click

        openChildForm(sender, New FormEditItems(Me))

    End Sub

    Private Sub ButtonEditRecipes_Click(sender As Object, e As EventArgs) Handles ButtonEditRecipes.Click

        openChildForm(sender, New FormEditRecipes(Me))

    End Sub

    Private Sub ButtonEditSupplyers_Click(sender As Object, e As EventArgs) Handles ButtonEditSupplyers.Click

        openChildForm(sender, New FormEditSuppliers(Me))

    End Sub

    Private Sub ButtonEditStores_Click(sender As Object, e As EventArgs) Handles ButtonEditStores.Click

        openChildForm(sender, New FormEditStores(Me))

    End Sub

    Private Sub ButtonEditOperators_Click(sender As Object, e As EventArgs) Handles ButtonEditOperators.Click

        openChildForm(sender, New FormEditOperators(Me))

    End Sub

    Private Sub ButtonExit_Click(sender As Object, e As EventArgs) Handles ButtonExit.Click

        If MessageBox.Show("終了してもよろしいですか？", Me.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = DialogResult.Yes Then
            Me.Close()
        End If

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles ButtonDashBoard.Click

        openChildForm(sender, New FormDashboard(Me))

    End Sub

#End Region

#Region "メソッド"

    Private Sub hideSubMenu()

        PanelMaintenance.Visible = False

    End Sub

    Private Sub showSubMenu(subMenu As Panel)

        If subMenu.Visible = False Then
            hideSubMenu()
            subMenu.Visible = True
        Else
            subMenu.Visible = False
        End If

    End Sub

    ' サブメニューの高さを中のボタンに合わせる(ボタンを取り除いたときに隙間が残らないようにする)
    Private Sub ArrangeSubMenu(subMenu As Panel)

        Dim height As Integer = subMenu.Padding.Vertical
        For Each item As Control In subMenu.Controls
            height += item.Height
        Next
        subMenu.Height = height

    End Sub

    Public Sub openChildForm(clickedButton As Button, childForm As Form, Optional editMode As EditMode = EditMode.Add, Optional id As Integer = Nothing)

        If Not IsNothing(currentButton) Then currentButton.BackColor = PanelSideMenu.BackColor
        currentButton = clickedButton
        If Not IsNothing(currentButton) Then currentButton.BackColor = PanelChildForm.BackColor

        If currentForm IsNot Nothing AndAlso currentForm IsNot childForm Then
            currentForm.Hide()
        End If
        currentForm = childForm
        If childForm.Parent Is Nothing Then
            childForm.TopLevel = False
            childForm.FormBorderStyle = FormBorderStyle.None
            childForm.Dock = DockStyle.Fill
            PanelChildForm.Controls.Add(childForm)
        End If
        childForm.BringToFront()
        childForm.Show()

        ' 再表示に対応している画面のときだけ再表示ボタンを押せるようにする
        ToolStripButtonRefresh.Enabled = TypeOf childForm Is IRefreshable

    End Sub

    ' ログイン中の担当者を画面に反映する
    Public Sub ApplyOperator()

        If sharedOperator Is Nothing Then Exit Sub

        ToolStripStatusLabelOperator.Text = sharedOperator.OperatorName & "（" & sharedOperator.RoleName & "）"

        ' マスタ保守は管理者のみ
        ButtonMaintenance.Enabled = sharedOperator.IsAdministrator
        If Not sharedOperator.IsAdministrator Then hideSubMenu()

    End Sub

#End Region

End Class