<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormMenu
    Inherits System.Windows.Forms.Form

    'フォームがコンポーネントの一覧をクリーンアップするために dispose をオーバーライドします。
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows フォーム デザイナーで必要です。
    Private components As System.ComponentModel.IContainer

    'メモ: 以下のプロシージャは Windows フォーム デザイナーで必要です。
    'Windows フォーム デザイナーを使用して変更できます。  
    'コード エディターを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormMenu))
        Me.PanelChildForm = New System.Windows.Forms.Panel()
        Me.PanelSideMenu = New System.Windows.Forms.Panel()
        Me.ButtonExit = New System.Windows.Forms.Button()
        Me.PanelMaintenance = New System.Windows.Forms.Panel()
        Me.ButtonEditOperators = New System.Windows.Forms.Button()
        Me.ButtonEditStores = New System.Windows.Forms.Button()
        Me.ButtonEditSupplyers = New System.Windows.Forms.Button()
        Me.ButtonEditRecipes = New System.Windows.Forms.Button()
        Me.ButtonEditItems = New System.Windows.Forms.Button()
        Me.ButtonMaintenance = New System.Windows.Forms.Button()
        Me.ButtonTakeInventoriy = New System.Windows.Forms.Button()
        Me.ButtonInventoryDisposals = New System.Windows.Forms.Button()
        Me.ButtonAjustInventory = New System.Windows.Forms.Button()
        Me.ButtonArrivals = New System.Windows.Forms.Button()
        Me.ButtonOrders = New System.Windows.Forms.Button()
        Me.ButtonInstructions = New System.Windows.Forms.Button()
        Me.ButtonInstructionPlans = New System.Windows.Forms.Button()
        Me.ButtonOrderForecasts = New System.Windows.Forms.Button()
        Me.ButtonDashBoard = New System.Windows.Forms.Button()
        Me.PanelLogo = New System.Windows.Forms.Panel()
        Me.PictureBoxLogo = New System.Windows.Forms.PictureBox()
        Me.ToolStripMain = New System.Windows.Forms.ToolStrip()
        Me.ToolStripButtonRefresh = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripButtonChangePassword = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripButtonExit = New System.Windows.Forms.ToolStripButton()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.ToolStripStatusLabelMessage = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusLabelOperator = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusLabelNow = New System.Windows.Forms.ToolStripStatusLabel()
        Me.PanelSideMenu.SuspendLayout()
        Me.PanelMaintenance.SuspendLayout()
        Me.PanelLogo.SuspendLayout()
        CType(Me.PictureBoxLogo, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ToolStripMain.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'PanelChildForm
        '
        Me.PanelChildForm.BackColor = System.Drawing.SystemColors.Control
        Me.PanelChildForm.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.PanelChildForm.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelChildForm.Location = New System.Drawing.Point(200, 27)
        Me.PanelChildForm.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.PanelChildForm.Name = "PanelChildForm"
        Me.PanelChildForm.Size = New System.Drawing.Size(1584, 910)
        Me.PanelChildForm.TabIndex = 29
        '
        'PanelSideMenu
        '
        Me.PanelSideMenu.AutoScroll = True
        Me.PanelSideMenu.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.PanelSideMenu.Controls.Add(Me.ButtonExit)
        Me.PanelSideMenu.Controls.Add(Me.PanelMaintenance)
        Me.PanelSideMenu.Controls.Add(Me.ButtonMaintenance)
        Me.PanelSideMenu.Controls.Add(Me.ButtonTakeInventoriy)
        Me.PanelSideMenu.Controls.Add(Me.ButtonInventoryDisposals)
        Me.PanelSideMenu.Controls.Add(Me.ButtonAjustInventory)
        Me.PanelSideMenu.Controls.Add(Me.ButtonArrivals)
        Me.PanelSideMenu.Controls.Add(Me.ButtonOrders)
        Me.PanelSideMenu.Controls.Add(Me.ButtonInstructions)
        Me.PanelSideMenu.Controls.Add(Me.ButtonInstructionPlans)
        Me.PanelSideMenu.Controls.Add(Me.ButtonOrderForecasts)
        Me.PanelSideMenu.Controls.Add(Me.ButtonDashBoard)
        Me.PanelSideMenu.Controls.Add(Me.PanelLogo)
        Me.PanelSideMenu.Dock = System.Windows.Forms.DockStyle.Left
        Me.PanelSideMenu.Location = New System.Drawing.Point(0, 27)
        Me.PanelSideMenu.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.PanelSideMenu.Name = "PanelSideMenu"
        Me.PanelSideMenu.Padding = New System.Windows.Forms.Padding(8, 0, 8, 0)
        Me.PanelSideMenu.Size = New System.Drawing.Size(200, 910)
        Me.PanelSideMenu.TabIndex = 28
        '
        'ButtonExit
        '
        Me.ButtonExit.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.ButtonExit.Dock = System.Windows.Forms.DockStyle.Top
        Me.ButtonExit.FlatAppearance.BorderSize = 0
        Me.ButtonExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.ButtonExit.Font = New System.Drawing.Font("MS UI Gothic", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonExit.ForeColor = System.Drawing.Color.Black
        Me.ButtonExit.Location = New System.Drawing.Point(8, 710)
        Me.ButtonExit.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.ButtonExit.Name = "ButtonExit"
        Me.ButtonExit.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.ButtonExit.Size = New System.Drawing.Size(184, 45)
        Me.ButtonExit.TabIndex = 9
        Me.ButtonExit.TabStop = False
        Me.ButtonExit.Text = "終了"
        Me.ButtonExit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.ButtonExit.UseVisualStyleBackColor = False
        '
        'PanelMaintenance
        '
        Me.PanelMaintenance.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.PanelMaintenance.Controls.Add(Me.ButtonEditOperators)
        Me.PanelMaintenance.Controls.Add(Me.ButtonEditStores)
        Me.PanelMaintenance.Controls.Add(Me.ButtonEditSupplyers)
        Me.PanelMaintenance.Controls.Add(Me.ButtonEditRecipes)
        Me.PanelMaintenance.Controls.Add(Me.ButtonEditItems)
        Me.PanelMaintenance.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelMaintenance.Location = New System.Drawing.Point(8, 485)
        Me.PanelMaintenance.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.PanelMaintenance.Name = "PanelMaintenance"
        Me.PanelMaintenance.Size = New System.Drawing.Size(184, 225)
        Me.PanelMaintenance.TabIndex = 8
        '
        'ButtonEditOperators
        '
        Me.ButtonEditOperators.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.ButtonEditOperators.Dock = System.Windows.Forms.DockStyle.Top
        Me.ButtonEditOperators.FlatAppearance.BorderSize = 0
        Me.ButtonEditOperators.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.ButtonEditOperators.Font = New System.Drawing.Font("MS UI Gothic", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonEditOperators.ForeColor = System.Drawing.Color.Black
        Me.ButtonEditOperators.Location = New System.Drawing.Point(0, 180)
        Me.ButtonEditOperators.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.ButtonEditOperators.Name = "ButtonEditOperators"
        Me.ButtonEditOperators.Padding = New System.Windows.Forms.Padding(30, 0, 0, 0)
        Me.ButtonEditOperators.Size = New System.Drawing.Size(184, 45)
        Me.ButtonEditOperators.TabIndex = 4
        Me.ButtonEditOperators.TabStop = False
        Me.ButtonEditOperators.Text = "担当者登録"
        Me.ButtonEditOperators.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.ButtonEditOperators.UseVisualStyleBackColor = False
        '
        'ButtonEditStores
        '
        Me.ButtonEditStores.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.ButtonEditStores.Dock = System.Windows.Forms.DockStyle.Top
        Me.ButtonEditStores.FlatAppearance.BorderSize = 0
        Me.ButtonEditStores.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.ButtonEditStores.Font = New System.Drawing.Font("MS UI Gothic", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonEditStores.ForeColor = System.Drawing.Color.Black
        Me.ButtonEditStores.Location = New System.Drawing.Point(0, 135)
        Me.ButtonEditStores.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.ButtonEditStores.Name = "ButtonEditStores"
        Me.ButtonEditStores.Padding = New System.Windows.Forms.Padding(30, 0, 0, 0)
        Me.ButtonEditStores.Size = New System.Drawing.Size(184, 45)
        Me.ButtonEditStores.TabIndex = 3
        Me.ButtonEditStores.TabStop = False
        Me.ButtonEditStores.Text = "店舗登録"
        Me.ButtonEditStores.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.ButtonEditStores.UseVisualStyleBackColor = False
        '
        'ButtonEditSupplyers
        '
        Me.ButtonEditSupplyers.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.ButtonEditSupplyers.Dock = System.Windows.Forms.DockStyle.Top
        Me.ButtonEditSupplyers.FlatAppearance.BorderSize = 0
        Me.ButtonEditSupplyers.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.ButtonEditSupplyers.Font = New System.Drawing.Font("MS UI Gothic", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonEditSupplyers.ForeColor = System.Drawing.Color.Black
        Me.ButtonEditSupplyers.Location = New System.Drawing.Point(0, 90)
        Me.ButtonEditSupplyers.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.ButtonEditSupplyers.Name = "ButtonEditSupplyers"
        Me.ButtonEditSupplyers.Padding = New System.Windows.Forms.Padding(30, 0, 0, 0)
        Me.ButtonEditSupplyers.Size = New System.Drawing.Size(184, 45)
        Me.ButtonEditSupplyers.TabIndex = 2
        Me.ButtonEditSupplyers.TabStop = False
        Me.ButtonEditSupplyers.Text = "仕入先登録"
        Me.ButtonEditSupplyers.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.ButtonEditSupplyers.UseVisualStyleBackColor = False
        '
        'ButtonEditRecipes
        '
        Me.ButtonEditRecipes.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.ButtonEditRecipes.Dock = System.Windows.Forms.DockStyle.Top
        Me.ButtonEditRecipes.FlatAppearance.BorderSize = 0
        Me.ButtonEditRecipes.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.ButtonEditRecipes.Font = New System.Drawing.Font("MS UI Gothic", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonEditRecipes.ForeColor = System.Drawing.Color.Black
        Me.ButtonEditRecipes.Location = New System.Drawing.Point(0, 45)
        Me.ButtonEditRecipes.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.ButtonEditRecipes.Name = "ButtonEditRecipes"
        Me.ButtonEditRecipes.Padding = New System.Windows.Forms.Padding(30, 0, 0, 0)
        Me.ButtonEditRecipes.Size = New System.Drawing.Size(184, 45)
        Me.ButtonEditRecipes.TabIndex = 1
        Me.ButtonEditRecipes.TabStop = False
        Me.ButtonEditRecipes.Text = "レシピ登録"
        Me.ButtonEditRecipes.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.ButtonEditRecipes.UseVisualStyleBackColor = False
        '
        'ButtonEditItems
        '
        Me.ButtonEditItems.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.ButtonEditItems.Dock = System.Windows.Forms.DockStyle.Top
        Me.ButtonEditItems.FlatAppearance.BorderSize = 0
        Me.ButtonEditItems.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.ButtonEditItems.Font = New System.Drawing.Font("MS UI Gothic", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonEditItems.ForeColor = System.Drawing.Color.Black
        Me.ButtonEditItems.Location = New System.Drawing.Point(0, 0)
        Me.ButtonEditItems.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.ButtonEditItems.Name = "ButtonEditItems"
        Me.ButtonEditItems.Padding = New System.Windows.Forms.Padding(30, 0, 0, 0)
        Me.ButtonEditItems.Size = New System.Drawing.Size(184, 45)
        Me.ButtonEditItems.TabIndex = 0
        Me.ButtonEditItems.TabStop = False
        Me.ButtonEditItems.Text = "品目登録"
        Me.ButtonEditItems.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.ButtonEditItems.UseVisualStyleBackColor = False
        '
        'ButtonMaintenance
        '
        Me.ButtonMaintenance.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.ButtonMaintenance.Dock = System.Windows.Forms.DockStyle.Top
        Me.ButtonMaintenance.FlatAppearance.BorderSize = 0
        Me.ButtonMaintenance.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.ButtonMaintenance.Font = New System.Drawing.Font("MS UI Gothic", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonMaintenance.ForeColor = System.Drawing.Color.Black
        Me.ButtonMaintenance.Location = New System.Drawing.Point(8, 440)
        Me.ButtonMaintenance.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.ButtonMaintenance.Name = "ButtonMaintenance"
        Me.ButtonMaintenance.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.ButtonMaintenance.Size = New System.Drawing.Size(184, 45)
        Me.ButtonMaintenance.TabIndex = 7
        Me.ButtonMaintenance.TabStop = False
        Me.ButtonMaintenance.Text = "マスタ保守"
        Me.ButtonMaintenance.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.ButtonMaintenance.UseVisualStyleBackColor = False
        '
        'ButtonTakeInventoriy
        '
        Me.ButtonTakeInventoriy.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.ButtonTakeInventoriy.Dock = System.Windows.Forms.DockStyle.Top
        Me.ButtonTakeInventoriy.FlatAppearance.BorderSize = 0
        Me.ButtonTakeInventoriy.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.ButtonTakeInventoriy.Font = New System.Drawing.Font("MS UI Gothic", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonTakeInventoriy.ForeColor = System.Drawing.Color.Black
        Me.ButtonTakeInventoriy.Location = New System.Drawing.Point(8, 395)
        Me.ButtonTakeInventoriy.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.ButtonTakeInventoriy.Name = "ButtonTakeInventoriy"
        Me.ButtonTakeInventoriy.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.ButtonTakeInventoriy.Size = New System.Drawing.Size(184, 45)
        Me.ButtonTakeInventoriy.TabIndex = 13
        Me.ButtonTakeInventoriy.TabStop = False
        Me.ButtonTakeInventoriy.Text = "棚卸入力"
        Me.ButtonTakeInventoriy.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.ButtonTakeInventoriy.UseVisualStyleBackColor = False
        '
        'ButtonInventoryDisposals
        '
        Me.ButtonInventoryDisposals.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.ButtonInventoryDisposals.Dock = System.Windows.Forms.DockStyle.Top
        Me.ButtonInventoryDisposals.FlatAppearance.BorderSize = 0
        Me.ButtonInventoryDisposals.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.ButtonInventoryDisposals.Font = New System.Drawing.Font("MS UI Gothic", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonInventoryDisposals.ForeColor = System.Drawing.Color.Black
        Me.ButtonInventoryDisposals.Location = New System.Drawing.Point(8, 395)
        Me.ButtonInventoryDisposals.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.ButtonInventoryDisposals.Name = "ButtonInventoryDisposals"
        Me.ButtonInventoryDisposals.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.ButtonInventoryDisposals.Size = New System.Drawing.Size(184, 45)
        Me.ButtonInventoryDisposals.TabIndex = 14
        Me.ButtonInventoryDisposals.TabStop = False
        Me.ButtonInventoryDisposals.Text = "廃棄リスト"
        Me.ButtonInventoryDisposals.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.ButtonInventoryDisposals.UseVisualStyleBackColor = False
        '
        'ButtonAjustInventory
        '
        Me.ButtonAjustInventory.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.ButtonAjustInventory.Dock = System.Windows.Forms.DockStyle.Top
        Me.ButtonAjustInventory.FlatAppearance.BorderSize = 0
        Me.ButtonAjustInventory.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.ButtonAjustInventory.Font = New System.Drawing.Font("MS UI Gothic", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonAjustInventory.ForeColor = System.Drawing.Color.Black
        Me.ButtonAjustInventory.Location = New System.Drawing.Point(8, 350)
        Me.ButtonAjustInventory.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.ButtonAjustInventory.Name = "ButtonAjustInventory"
        Me.ButtonAjustInventory.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.ButtonAjustInventory.Size = New System.Drawing.Size(184, 45)
        Me.ButtonAjustInventory.TabIndex = 6
        Me.ButtonAjustInventory.TabStop = False
        Me.ButtonAjustInventory.Text = "在庫調整入力"
        Me.ButtonAjustInventory.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.ButtonAjustInventory.UseVisualStyleBackColor = False
        '
        'ButtonArrivals
        '
        Me.ButtonArrivals.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.ButtonArrivals.Dock = System.Windows.Forms.DockStyle.Top
        Me.ButtonArrivals.FlatAppearance.BorderSize = 0
        Me.ButtonArrivals.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.ButtonArrivals.Font = New System.Drawing.Font("MS UI Gothic", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonArrivals.ForeColor = System.Drawing.Color.Black
        Me.ButtonArrivals.Location = New System.Drawing.Point(8, 305)
        Me.ButtonArrivals.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.ButtonArrivals.Name = "ButtonArrivals"
        Me.ButtonArrivals.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.ButtonArrivals.Size = New System.Drawing.Size(184, 45)
        Me.ButtonArrivals.TabIndex = 11
        Me.ButtonArrivals.TabStop = False
        Me.ButtonArrivals.Text = "入荷検品入力"
        Me.ButtonArrivals.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.ButtonArrivals.UseVisualStyleBackColor = False
        '
        'ButtonOrders
        '
        Me.ButtonOrders.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.ButtonOrders.Dock = System.Windows.Forms.DockStyle.Top
        Me.ButtonOrders.FlatAppearance.BorderSize = 0
        Me.ButtonOrders.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.ButtonOrders.Font = New System.Drawing.Font("MS UI Gothic", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonOrders.ForeColor = System.Drawing.Color.Black
        Me.ButtonOrders.Location = New System.Drawing.Point(8, 260)
        Me.ButtonOrders.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.ButtonOrders.Name = "ButtonOrders"
        Me.ButtonOrders.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.ButtonOrders.Size = New System.Drawing.Size(184, 45)
        Me.ButtonOrders.TabIndex = 12
        Me.ButtonOrders.TabStop = False
        Me.ButtonOrders.Text = "発注入力"
        Me.ButtonOrders.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.ButtonOrders.UseVisualStyleBackColor = False
        '
        'ButtonInstructions
        '
        Me.ButtonInstructions.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.ButtonInstructions.Dock = System.Windows.Forms.DockStyle.Top
        Me.ButtonInstructions.FlatAppearance.BorderSize = 0
        Me.ButtonInstructions.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.ButtonInstructions.Font = New System.Drawing.Font("MS UI Gothic", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonInstructions.ForeColor = System.Drawing.Color.Black
        Me.ButtonInstructions.Location = New System.Drawing.Point(8, 215)
        Me.ButtonInstructions.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.ButtonInstructions.Name = "ButtonInstructions"
        Me.ButtonInstructions.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.ButtonInstructions.Size = New System.Drawing.Size(184, 45)
        Me.ButtonInstructions.TabIndex = 10
        Me.ButtonInstructions.TabStop = False
        Me.ButtonInstructions.Text = "完成入力"
        Me.ButtonInstructions.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.ButtonInstructions.UseVisualStyleBackColor = False
        '
        'ButtonInstructionPlans
        '
        Me.ButtonInstructionPlans.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.ButtonInstructionPlans.Dock = System.Windows.Forms.DockStyle.Top
        Me.ButtonInstructionPlans.FlatAppearance.BorderSize = 0
        Me.ButtonInstructionPlans.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.ButtonInstructionPlans.Font = New System.Drawing.Font("MS UI Gothic", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonInstructionPlans.ForeColor = System.Drawing.Color.Black
        Me.ButtonInstructionPlans.Location = New System.Drawing.Point(8, 170)
        Me.ButtonInstructionPlans.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.ButtonInstructionPlans.Name = "ButtonInstructionPlans"
        Me.ButtonInstructionPlans.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.ButtonInstructionPlans.Size = New System.Drawing.Size(184, 45)
        Me.ButtonInstructionPlans.TabIndex = 4
        Me.ButtonInstructionPlans.TabStop = False
        Me.ButtonInstructionPlans.Text = "製造計画入力"
        Me.ButtonInstructionPlans.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.ButtonInstructionPlans.UseVisualStyleBackColor = False
        '
        'ButtonOrderForecasts
        '
        Me.ButtonOrderForecasts.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.ButtonOrderForecasts.Dock = System.Windows.Forms.DockStyle.Top
        Me.ButtonOrderForecasts.FlatAppearance.BorderSize = 0
        Me.ButtonOrderForecasts.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.ButtonOrderForecasts.Font = New System.Drawing.Font("MS UI Gothic", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonOrderForecasts.ForeColor = System.Drawing.Color.Black
        Me.ButtonOrderForecasts.Location = New System.Drawing.Point(8, 125)
        Me.ButtonOrderForecasts.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.ButtonOrderForecasts.Name = "ButtonOrderForecasts"
        Me.ButtonOrderForecasts.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.ButtonOrderForecasts.Size = New System.Drawing.Size(184, 45)
        Me.ButtonOrderForecasts.TabIndex = 2
        Me.ButtonOrderForecasts.TabStop = False
        Me.ButtonOrderForecasts.Text = "発注予測照会"
        Me.ButtonOrderForecasts.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.ButtonOrderForecasts.UseVisualStyleBackColor = False
        '
        'ButtonDashBoard
        '
        Me.ButtonDashBoard.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.ButtonDashBoard.Dock = System.Windows.Forms.DockStyle.Top
        Me.ButtonDashBoard.FlatAppearance.BorderSize = 0
        Me.ButtonDashBoard.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.ButtonDashBoard.Font = New System.Drawing.Font("MS UI Gothic", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonDashBoard.ForeColor = System.Drawing.Color.Black
        Me.ButtonDashBoard.Location = New System.Drawing.Point(8, 80)
        Me.ButtonDashBoard.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.ButtonDashBoard.Name = "ButtonDashBoard"
        Me.ButtonDashBoard.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.ButtonDashBoard.Size = New System.Drawing.Size(184, 45)
        Me.ButtonDashBoard.TabIndex = 14
        Me.ButtonDashBoard.TabStop = False
        Me.ButtonDashBoard.Text = "ダッシュボード"
        Me.ButtonDashBoard.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.ButtonDashBoard.UseVisualStyleBackColor = False
        '
        'PanelLogo
        '
        Me.PanelLogo.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.PanelLogo.Controls.Add(Me.PictureBoxLogo)
        Me.PanelLogo.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelLogo.Location = New System.Drawing.Point(8, 0)
        Me.PanelLogo.Margin = New System.Windows.Forms.Padding(5, 4, 5, 4)
        Me.PanelLogo.Name = "PanelLogo"
        Me.PanelLogo.Padding = New System.Windows.Forms.Padding(5, 8, 5, 8)
        Me.PanelLogo.Size = New System.Drawing.Size(184, 80)
        Me.PanelLogo.TabIndex = 1
        '
        'PictureBoxLogo
        '
        Me.PictureBoxLogo.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.PictureBoxLogo.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PictureBoxLogo.Image = CType(resources.GetObject("PictureBoxLogo.Image"), System.Drawing.Image)
        Me.PictureBoxLogo.Location = New System.Drawing.Point(5, 8)
        Me.PictureBoxLogo.Margin = New System.Windows.Forms.Padding(0)
        Me.PictureBoxLogo.Name = "PictureBoxLogo"
        Me.PictureBoxLogo.Size = New System.Drawing.Size(174, 64)
        Me.PictureBoxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.PictureBoxLogo.TabIndex = 0
        Me.PictureBoxLogo.TabStop = False
        '
        'ToolStripMain
        '
        Me.ToolStripMain.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.ToolStripMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripButtonRefresh, Me.ToolStripSeparator1, Me.ToolStripButtonChangePassword, Me.ToolStripButtonExit})
        Me.ToolStripMain.Location = New System.Drawing.Point(0, 0)
        Me.ToolStripMain.Name = "ToolStripMain"
        Me.ToolStripMain.Padding = New System.Windows.Forms.Padding(0, 0, 2, 0)
        Me.ToolStripMain.Size = New System.Drawing.Size(1784, 27)
        Me.ToolStripMain.TabIndex = 30
        Me.ToolStripMain.Text = "ToolStrip1"
        '
        'ToolStripButtonRefresh
        '
        Me.ToolStripButtonRefresh.Enabled = False
        Me.ToolStripButtonRefresh.Font = New System.Drawing.Font("MS UI Gothic", 9.75!)
        Me.ToolStripButtonRefresh.Image = CType(resources.GetObject("ToolStripButtonRefresh.Image"), System.Drawing.Image)
        Me.ToolStripButtonRefresh.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButtonRefresh.Name = "ToolStripButtonRefresh"
        Me.ToolStripButtonRefresh.Size = New System.Drawing.Size(70, 24)
        Me.ToolStripButtonRefresh.Text = "再表示"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 27)
        '
        'ToolStripButtonChangePassword
        '
        Me.ToolStripButtonChangePassword.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButtonChangePassword.Name = "ToolStripButtonChangePassword"
        Me.ToolStripButtonChangePassword.Size = New System.Drawing.Size(79, 24)
        Me.ToolStripButtonChangePassword.Text = "パスワード変更"
        '
        'ToolStripButtonExit
        '
        Me.ToolStripButtonExit.Font = New System.Drawing.Font("MS UI Gothic", 9.75!)
        Me.ToolStripButtonExit.Image = CType(resources.GetObject("ToolStripButtonExit.Image"), System.Drawing.Image)
        Me.ToolStripButtonExit.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButtonExit.Name = "ToolStripButtonExit"
        Me.ToolStripButtonExit.Size = New System.Drawing.Size(57, 24)
        Me.ToolStripButtonExit.Text = "終了"
        '
        'StatusStrip1
        '
        Me.StatusStrip1.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabelMessage, Me.ToolStripStatusLabelOperator, Me.ToolStripStatusLabelNow})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 937)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Padding = New System.Windows.Forms.Padding(2, 0, 23, 0)
        Me.StatusStrip1.Size = New System.Drawing.Size(1784, 24)
        Me.StatusStrip1.TabIndex = 4
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'ToolStripStatusLabelMessage
        '
        Me.ToolStripStatusLabelMessage.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right
        Me.ToolStripStatusLabelMessage.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.ToolStripStatusLabelMessage.Name = "ToolStripStatusLabelMessage"
        Me.ToolStripStatusLabelMessage.Size = New System.Drawing.Size(1512, 19)
        Me.ToolStripStatusLabelMessage.Spring = True
        Me.ToolStripStatusLabelMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'ToolStripStatusLabelOperator
        '
        Me.ToolStripStatusLabelOperator.Name = "ToolStripStatusLabelOperator"
        Me.ToolStripStatusLabelOperator.Size = New System.Drawing.Size(119, 19)
        Me.ToolStripStatusLabelOperator.Text = "ToolStripStatusLabel1"
        '
        'ToolStripStatusLabelNow
        '
        Me.ToolStripStatusLabelNow.AutoSize = False
        Me.ToolStripStatusLabelNow.Name = "ToolStripStatusLabelNow"
        Me.ToolStripStatusLabelNow.Size = New System.Drawing.Size(128, 19)
        Me.ToolStripStatusLabelNow.Text = "yyyy/MM/dd HH:mm:ss"
        '
        'FormMenu
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1784, 961)
        Me.Controls.Add(Me.PanelChildForm)
        Me.Controls.Add(Me.PanelSideMenu)
        Me.Controls.Add(Me.ToolStripMain)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MinimumSize = New System.Drawing.Size(1600, 1000)
        Me.Name = "FormMenu"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "DeliPlanner"
        Me.PanelSideMenu.ResumeLayout(False)
        Me.PanelMaintenance.ResumeLayout(False)
        Me.PanelLogo.ResumeLayout(False)
        CType(Me.PictureBoxLogo, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ToolStripMain.ResumeLayout(False)
        Me.ToolStripMain.PerformLayout()
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents PanelChildForm As Panel
    Friend WithEvents PanelSideMenu As Panel
    Friend WithEvents ButtonExit As Button
    Friend WithEvents PanelMaintenance As Panel
    Friend WithEvents ButtonEditStores As Button
    Friend WithEvents ButtonEditSupplyers As Button
    Friend WithEvents ButtonEditRecipes As Button
    Friend WithEvents ButtonEditItems As Button
    Friend WithEvents ButtonMaintenance As Button
    Friend WithEvents ButtonAjustInventory As Button
    Friend WithEvents ButtonArrivals As Button
    Friend WithEvents ButtonInstructions As Button
    Friend WithEvents ButtonInstructionPlans As Button
    Friend WithEvents ButtonOrderForecasts As Button
    Friend WithEvents PanelLogo As Panel
    Friend WithEvents PictureBoxLogo As PictureBox
    Friend WithEvents ToolStripMain As ToolStrip
    Friend WithEvents ToolStripButtonRefresh As ToolStripButton
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
    Friend WithEvents ToolStripButtonExit As ToolStripButton
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents ToolStripStatusLabelMessage As ToolStripStatusLabel
    Friend WithEvents ToolStripStatusLabelNow As ToolStripStatusLabel
    Friend WithEvents ButtonOrders As Button
    Friend WithEvents ButtonTakeInventoriy As Button
    Friend WithEvents ToolStripButtonChangePassword As ToolStripButton
    Friend WithEvents ButtonEditOperators As Button
    Friend WithEvents ToolStripStatusLabelOperator As ToolStripStatusLabel
    Friend WithEvents ButtonDashBoard As Button
    Friend WithEvents ButtonInventoryDisposals As Button
End Class
