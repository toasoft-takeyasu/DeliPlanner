<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormChangeInventory
    Inherits DeliPlanner.FormBase

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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.ButtonRegist = New System.Windows.Forms.Button()
        Me.PanelInventories = New System.Windows.Forms.Panel()
        Me.ButtonExportInventoryList = New System.Windows.Forms.Button()
        Me.ButtonClearItem = New System.Windows.Forms.Button()
        Me.ButtonSearchItem = New System.Windows.Forms.Button()
        Me.TextBoxItemName = New TOA.ToaTextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.DateBoxInventoryDate = New TOA.ToaDateBox()
        Me.Label58 = New System.Windows.Forms.Label()
        Me.ComboBoxItemType = New System.Windows.Forms.ComboBox()
        Me.TextBoxItemId = New TOA.ToaTextBox()
        Me.ButtonImportStocktakingFile = New System.Windows.Forms.Button()
        Me.ButtonInventoryDatePrevious = New System.Windows.Forms.Button()
        Me.ButtonInventoryDateNext = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.ButtonExportStocktakingFile = New System.Windows.Forms.Button()
        Me.LabelInventoryTotal = New System.Windows.Forms.Label()
        Me.PanelMain.SuspendLayout()
        Me.SuspendLayout()
        '
        'PanelMain
        '
        Me.PanelMain.Controls.Add(Me.LabelInventoryTotal)
        Me.PanelMain.Controls.Add(Me.ButtonExportStocktakingFile)
        Me.PanelMain.Controls.Add(Me.Label14)
        Me.PanelMain.Controls.Add(Me.DateBoxInventoryDate)
        Me.PanelMain.Controls.Add(Me.Label1)
        Me.PanelMain.Controls.Add(Me.ButtonInventoryDatePrevious)
        Me.PanelMain.Controls.Add(Me.ButtonImportStocktakingFile)
        Me.PanelMain.Controls.Add(Me.ButtonInventoryDateNext)
        Me.PanelMain.Controls.Add(Me.TextBoxItemId)
        Me.PanelMain.Controls.Add(Me.Label58)
        Me.PanelMain.Controls.Add(Me.ComboBoxItemType)
        Me.PanelMain.Controls.Add(Me.ButtonClearItem)
        Me.PanelMain.Controls.Add(Me.ButtonSearchItem)
        Me.PanelMain.Controls.Add(Me.TextBoxItemName)
        Me.PanelMain.Controls.Add(Me.Label2)
        Me.PanelMain.Controls.Add(Me.ButtonExportInventoryList)
        Me.PanelMain.Controls.Add(Me.PanelInventories)
        Me.PanelMain.Controls.Add(Me.ButtonRegist)
        Me.PanelMain.Size = New System.Drawing.Size(1368, 871)
        '
        'ButtonRegist
        '
        Me.ButtonRegist.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonRegist.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ButtonRegist.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonRegist.Image = Global.DeliPlanner.My.Resources.Resources.save
        Me.ButtonRegist.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ButtonRegist.Location = New System.Drawing.Point(1236, 829)
        Me.ButtonRegist.Name = "ButtonRegist"
        Me.ButtonRegist.Size = New System.Drawing.Size(120, 30)
        Me.ButtonRegist.TabIndex = 13
        Me.ButtonRegist.TabStop = False
        Me.ButtonRegist.Text = "登録"
        Me.ButtonRegist.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonRegist.UseVisualStyleBackColor = False
        '
        'PanelInventories
        '
        Me.PanelInventories.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PanelInventories.Location = New System.Drawing.Point(12, 48)
        Me.PanelInventories.Name = "PanelInventories"
        Me.PanelInventories.Size = New System.Drawing.Size(1344, 771)
        Me.PanelInventories.TabIndex = 11
        '
        'ButtonExportInventoryList
        '
        Me.ButtonExportInventoryList.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.ButtonExportInventoryList.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ButtonExportInventoryList.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonExportInventoryList.Image = Global.DeliPlanner.My.Resources.Resources.csv
        Me.ButtonExportInventoryList.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ButtonExportInventoryList.Location = New System.Drawing.Point(12, 829)
        Me.ButtonExportInventoryList.Name = "ButtonExportInventoryList"
        Me.ButtonExportInventoryList.Size = New System.Drawing.Size(120, 30)
        Me.ButtonExportInventoryList.TabIndex = 12
        Me.ButtonExportInventoryList.TabStop = False
        Me.ButtonExportInventoryList.Text = "在庫一覧表"
        Me.ButtonExportInventoryList.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonExportInventoryList.UseVisualStyleBackColor = False
        '
        'ButtonClearItem
        '
        Me.ButtonClearItem.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ButtonClearItem.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonClearItem.Image = Global.DeliPlanner.My.Resources.Resources.cross
        Me.ButtonClearItem.Location = New System.Drawing.Point(913, 15)
        Me.ButtonClearItem.Name = "ButtonClearItem"
        Me.ButtonClearItem.Size = New System.Drawing.Size(30, 25)
        Me.ButtonClearItem.TabIndex = 8
        Me.ButtonClearItem.TabStop = False
        Me.ButtonClearItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonClearItem.UseVisualStyleBackColor = False
        '
        'ButtonSearchItem
        '
        Me.ButtonSearchItem.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ButtonSearchItem.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonSearchItem.Image = Global.DeliPlanner.My.Resources.Resources.search
        Me.ButtonSearchItem.Location = New System.Drawing.Point(884, 15)
        Me.ButtonSearchItem.Name = "ButtonSearchItem"
        Me.ButtonSearchItem.Size = New System.Drawing.Size(30, 25)
        Me.ButtonSearchItem.TabIndex = 7
        Me.ButtonSearchItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonSearchItem.UseVisualStyleBackColor = False
        '
        'TextBoxItemName
        '
        Me.TextBoxItemName.BackColor = System.Drawing.SystemColors.Window
        Me.TextBoxItemName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxItemName.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBoxItemName.ImeMode = System.Windows.Forms.ImeMode.Hiragana
        Me.TextBoxItemName.Location = New System.Drawing.Point(636, 16)
        Me.TextBoxItemName.MaxByteLength = 40
        Me.TextBoxItemName.Name = "TextBoxItemName"
        Me.TextBoxItemName.OldText = Nothing
        Me.TextBoxItemName.ReadOnly = True
        Me.TextBoxItemName.Size = New System.Drawing.Size(248, 23)
        Me.TextBoxItemName.TabIndex = 6
        Me.TextBoxItemName.TabStop = False
        Me.TextBoxItemName.Text = "１２３４５６７８９０１２３４５６７８９０１２３４５６７８９０"
        '
        'Label2
        '
        Me.Label2.BackColor = System.Drawing.SystemColors.GradientInactiveCaption
        Me.Label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label2.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(536, 16)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(101, 23)
        Me.Label2.TabIndex = 5
        Me.Label2.Text = "品目名"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'DateBoxInventoryDate
        '
        Me.DateBoxInventoryDate.AcceptsTab = True
        Me.DateBoxInventoryDate.BackColor = System.Drawing.Color.White
        Me.DateBoxInventoryDate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.DateBoxInventoryDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.DateBoxInventoryDate.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.DateBoxInventoryDate.Location = New System.Drawing.Point(164, 16)
        Me.DateBoxInventoryDate.MaxLength = 10
        Me.DateBoxInventoryDate.Name = "DateBoxInventoryDate"
        Me.DateBoxInventoryDate.OldText = Nothing
        Me.DateBoxInventoryDate.Size = New System.Drawing.Size(104, 23)
        Me.DateBoxInventoryDate.TabIndex = 1
        Me.DateBoxInventoryDate.Text = "yyyy/MM/dd"
        Me.DateBoxInventoryDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label58
        '
        Me.Label58.BackColor = System.Drawing.SystemColors.GradientInactiveCaption
        Me.Label58.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label58.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label58.Location = New System.Drawing.Point(323, 16)
        Me.Label58.Name = "Label58"
        Me.Label58.Size = New System.Drawing.Size(101, 23)
        Me.Label58.TabIndex = 3
        Me.Label58.Text = "品目区分"
        Me.Label58.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'ComboBoxItemType
        '
        Me.ComboBoxItemType.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ComboBoxItemType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBoxItemType.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ComboBoxItemType.FormattingEnabled = True
        Me.ComboBoxItemType.Location = New System.Drawing.Point(423, 16)
        Me.ComboBoxItemType.Name = "ComboBoxItemType"
        Me.ComboBoxItemType.Size = New System.Drawing.Size(90, 23)
        Me.ComboBoxItemType.TabIndex = 4
        '
        'TextBoxItemId
        '
        Me.TextBoxItemId.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextBoxItemId.BackColor = System.Drawing.Color.White
        Me.TextBoxItemId.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxItemId.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBoxItemId.Format = "#,###"
        Me.TextBoxItemId.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.TextBoxItemId.Location = New System.Drawing.Point(817, 16)
        Me.TextBoxItemId.MaxByteLength = 7
        Me.TextBoxItemId.MaxValue = "0"
        Me.TextBoxItemId.MinValue = "-999999"
        Me.TextBoxItemId.Name = "TextBoxItemId"
        Me.TextBoxItemId.NumericOnly = True
        Me.TextBoxItemId.OldText = Nothing
        Me.TextBoxItemId.Size = New System.Drawing.Size(67, 23)
        Me.TextBoxItemId.TabIndex = 9
        Me.TextBoxItemId.TabStop = False
        Me.TextBoxItemId.Text = "0"
        Me.TextBoxItemId.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TextBoxItemId.Visible = False
        '
        'ButtonImportStocktakingFile
        '
        Me.ButtonImportStocktakingFile.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonImportStocktakingFile.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ButtonImportStocktakingFile.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonImportStocktakingFile.Image = Global.DeliPlanner.My.Resources.Resources.import
        Me.ButtonImportStocktakingFile.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ButtonImportStocktakingFile.Location = New System.Drawing.Point(1236, 12)
        Me.ButtonImportStocktakingFile.Name = "ButtonImportStocktakingFile"
        Me.ButtonImportStocktakingFile.Size = New System.Drawing.Size(120, 30)
        Me.ButtonImportStocktakingFile.TabIndex = 10
        Me.ButtonImportStocktakingFile.TabStop = False
        Me.ButtonImportStocktakingFile.Text = "棚卸調査表"
        Me.ButtonImportStocktakingFile.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonImportStocktakingFile.UseVisualStyleBackColor = False
        '
        'ButtonInventoryDatePrevious
        '
        Me.ButtonInventoryDatePrevious.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ButtonInventoryDatePrevious.Font = New System.Drawing.Font("MS UI Gothic", 3.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonInventoryDatePrevious.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ButtonInventoryDatePrevious.Location = New System.Drawing.Point(268, 27)
        Me.ButtonInventoryDatePrevious.Name = "ButtonInventoryDatePrevious"
        Me.ButtonInventoryDatePrevious.Size = New System.Drawing.Size(33, 13)
        Me.ButtonInventoryDatePrevious.TabIndex = 16
        Me.ButtonInventoryDatePrevious.TabStop = False
        Me.ButtonInventoryDatePrevious.Text = "▼"
        Me.ButtonInventoryDatePrevious.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonInventoryDatePrevious.UseVisualStyleBackColor = False
        '
        'ButtonInventoryDateNext
        '
        Me.ButtonInventoryDateNext.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ButtonInventoryDateNext.Font = New System.Drawing.Font("MS UI Gothic", 3.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonInventoryDateNext.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ButtonInventoryDateNext.Location = New System.Drawing.Point(268, 15)
        Me.ButtonInventoryDateNext.Name = "ButtonInventoryDateNext"
        Me.ButtonInventoryDateNext.Size = New System.Drawing.Size(33, 13)
        Me.ButtonInventoryDateNext.TabIndex = 17
        Me.ButtonInventoryDateNext.TabStop = False
        Me.ButtonInventoryDateNext.Text = "▲"
        Me.ButtonInventoryDateNext.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonInventoryDateNext.UseVisualStyleBackColor = False
        '
        'Label1
        '
        Me.Label1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption
        Me.Label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label1.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(64, 16)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(101, 23)
        Me.Label1.TabIndex = 18
        Me.Label1.Text = "棚卸日"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label14
        '
        Me.Label14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label14.Font = New System.Drawing.Font("MS UI Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label14.Location = New System.Drawing.Point(997, 16)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(25, 22)
        Me.Label14.TabIndex = 12
        Me.Label14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.Label14.Visible = False
        '
        'ButtonExportStocktakingFile
        '
        Me.ButtonExportStocktakingFile.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.ButtonExportStocktakingFile.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ButtonExportStocktakingFile.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonExportStocktakingFile.Image = Global.DeliPlanner.My.Resources.Resources.excel
        Me.ButtonExportStocktakingFile.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ButtonExportStocktakingFile.Location = New System.Drawing.Point(138, 829)
        Me.ButtonExportStocktakingFile.Name = "ButtonExportStocktakingFile"
        Me.ButtonExportStocktakingFile.Size = New System.Drawing.Size(120, 30)
        Me.ButtonExportStocktakingFile.TabIndex = 19
        Me.ButtonExportStocktakingFile.TabStop = False
        Me.ButtonExportStocktakingFile.Text = "棚卸調査表"
        Me.ButtonExportStocktakingFile.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonExportStocktakingFile.UseVisualStyleBackColor = False
        '
        'LabelInventoryTotal
        '
        Me.LabelInventoryTotal.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LabelInventoryTotal.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.LabelInventoryTotal.Location = New System.Drawing.Point(323, 829)
        Me.LabelInventoryTotal.Name = "LabelInventoryTotal"
        Me.LabelInventoryTotal.Size = New System.Drawing.Size(290, 30)
        Me.LabelInventoryTotal.TabIndex = 20
        Me.LabelInventoryTotal.Text = "在庫金額合計：0 円"
        Me.LabelInventoryTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'FormChangeInventory
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CausesValidation = False
        Me.ClientSize = New System.Drawing.Size(1368, 871)
        Me.Name = "FormChangeInventory"
        Me.Text = "FormInventories"
        Me.PanelMain.ResumeLayout(False)
        Me.PanelMain.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ButtonRegist As Button
    Friend WithEvents PanelInventories As Panel
    Friend WithEvents ButtonExportInventoryList As Button
    Friend WithEvents ButtonClearItem As Button
    Friend WithEvents ButtonSearchItem As Button
    Friend WithEvents TextBoxItemName As TOA.ToaTextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents DateBoxInventoryDate As TOA.ToaDateBox
    Friend WithEvents Label58 As Label
    Friend WithEvents ComboBoxItemType As ComboBox
    Friend WithEvents TextBoxItemId As TOA.ToaTextBox
    Friend WithEvents ButtonImportStocktakingFile As Button
    Friend WithEvents ButtonInventoryDatePrevious As Button
    Friend WithEvents ButtonInventoryDateNext As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents Label14 As Label
    Friend WithEvents ButtonExportStocktakingFile As Button
    Friend WithEvents LabelInventoryTotal As Label
End Class
