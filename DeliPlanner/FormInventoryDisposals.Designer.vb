<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormInventoryDisposals
    Inherits DeliPlanner.FormBase

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

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.LabelPeriod = New System.Windows.Forms.Label()
        Me.DatePickerFrom = New System.Windows.Forms.DateTimePicker()
        Me.LabelPeriodSeparator = New System.Windows.Forms.Label()
        Me.DatePickerTo = New System.Windows.Forms.DateTimePicker()
        Me.LabelItemType = New System.Windows.Forms.Label()
        Me.ComboBoxItemType = New System.Windows.Forms.ComboBox()
        Me.LabelItem = New System.Windows.Forms.Label()
        Me.TextBoxItemName = New TOA.ToaTextBox()
        Me.TextBoxItemId = New TOA.ToaTextBox()
        Me.ButtonSearchItem = New System.Windows.Forms.Button()
        Me.ButtonClearItem = New System.Windows.Forms.Button()
        Me.CheckBoxSummary = New System.Windows.Forms.CheckBox()
        Me.ButtonSearch = New System.Windows.Forms.Button()
        Me.PanelItems = New System.Windows.Forms.Panel()
        Me.LabelTotal = New System.Windows.Forms.Label()
        Me.PanelMain.SuspendLayout()
        Me.SuspendLayout()
        '
        'PanelMain
        '
        Me.PanelMain.Controls.Add(Me.LabelPeriod)
        Me.PanelMain.Controls.Add(Me.DatePickerFrom)
        Me.PanelMain.Controls.Add(Me.LabelPeriodSeparator)
        Me.PanelMain.Controls.Add(Me.DatePickerTo)
        Me.PanelMain.Controls.Add(Me.LabelItemType)
        Me.PanelMain.Controls.Add(Me.ComboBoxItemType)
        Me.PanelMain.Controls.Add(Me.LabelItem)
        Me.PanelMain.Controls.Add(Me.TextBoxItemName)
        Me.PanelMain.Controls.Add(Me.TextBoxItemId)
        Me.PanelMain.Controls.Add(Me.ButtonSearchItem)
        Me.PanelMain.Controls.Add(Me.ButtonClearItem)
        Me.PanelMain.Controls.Add(Me.CheckBoxSummary)
        Me.PanelMain.Controls.Add(Me.ButtonSearch)
        Me.PanelMain.Controls.Add(Me.PanelItems)
        Me.PanelMain.Controls.Add(Me.LabelTotal)
        Me.PanelMain.Size = New System.Drawing.Size(1368, 871)
        '
        'LabelPeriod
        '
        Me.LabelPeriod.BackColor = System.Drawing.SystemColors.GradientInactiveCaption
        Me.LabelPeriod.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LabelPeriod.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.LabelPeriod.Location = New System.Drawing.Point(12, 15)
        Me.LabelPeriod.Name = "LabelPeriod"
        Me.LabelPeriod.Size = New System.Drawing.Size(101, 23)
        Me.LabelPeriod.TabIndex = 0
        Me.LabelPeriod.Text = "廃棄日"
        Me.LabelPeriod.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'DatePickerFrom
        '
        Me.DatePickerFrom.CustomFormat = "yyyy/MM/dd"
        Me.DatePickerFrom.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.DatePickerFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.DatePickerFrom.Location = New System.Drawing.Point(112, 15)
        Me.DatePickerFrom.Name = "DatePickerFrom"
        Me.DatePickerFrom.ShowCheckBox = True
        Me.DatePickerFrom.Size = New System.Drawing.Size(145, 23)
        Me.DatePickerFrom.TabIndex = 1
        '
        'LabelPeriodSeparator
        '
        Me.LabelPeriodSeparator.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.LabelPeriodSeparator.Location = New System.Drawing.Point(257, 16)
        Me.LabelPeriodSeparator.Name = "LabelPeriodSeparator"
        Me.LabelPeriodSeparator.Size = New System.Drawing.Size(22, 23)
        Me.LabelPeriodSeparator.TabIndex = 2
        Me.LabelPeriodSeparator.Text = "～"
        Me.LabelPeriodSeparator.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'DatePickerTo
        '
        Me.DatePickerTo.CustomFormat = "yyyy/MM/dd"
        Me.DatePickerTo.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.DatePickerTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.DatePickerTo.Location = New System.Drawing.Point(279, 15)
        Me.DatePickerTo.Name = "DatePickerTo"
        Me.DatePickerTo.ShowCheckBox = True
        Me.DatePickerTo.Size = New System.Drawing.Size(145, 23)
        Me.DatePickerTo.TabIndex = 3
        '
        'LabelItemType
        '
        Me.LabelItemType.BackColor = System.Drawing.SystemColors.GradientInactiveCaption
        Me.LabelItemType.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LabelItemType.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.LabelItemType.Location = New System.Drawing.Point(441, 16)
        Me.LabelItemType.Name = "LabelItemType"
        Me.LabelItemType.Size = New System.Drawing.Size(101, 23)
        Me.LabelItemType.TabIndex = 4
        Me.LabelItemType.Text = "品目区分"
        Me.LabelItemType.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'ComboBoxItemType
        '
        Me.ComboBoxItemType.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ComboBoxItemType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBoxItemType.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ComboBoxItemType.FormattingEnabled = True
        Me.ComboBoxItemType.Location = New System.Drawing.Point(541, 16)
        Me.ComboBoxItemType.Name = "ComboBoxItemType"
        Me.ComboBoxItemType.Size = New System.Drawing.Size(100, 23)
        Me.ComboBoxItemType.TabIndex = 5
        '
        'LabelItem
        '
        Me.LabelItem.BackColor = System.Drawing.SystemColors.GradientInactiveCaption
        Me.LabelItem.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LabelItem.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.LabelItem.Location = New System.Drawing.Point(657, 16)
        Me.LabelItem.Name = "LabelItem"
        Me.LabelItem.Size = New System.Drawing.Size(101, 23)
        Me.LabelItem.TabIndex = 6
        Me.LabelItem.Text = "品目名"
        Me.LabelItem.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TextBoxItemName
        '
        Me.TextBoxItemName.BackColor = System.Drawing.SystemColors.Window
        Me.TextBoxItemName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxItemName.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBoxItemName.ImeMode = System.Windows.Forms.ImeMode.Hiragana
        Me.TextBoxItemName.Location = New System.Drawing.Point(757, 16)
        Me.TextBoxItemName.MaxByteLength = 40
        Me.TextBoxItemName.Name = "TextBoxItemName"
        Me.TextBoxItemName.OldText = Nothing
        Me.TextBoxItemName.ReadOnly = True
        Me.TextBoxItemName.Size = New System.Drawing.Size(248, 23)
        Me.TextBoxItemName.TabIndex = 7
        Me.TextBoxItemName.TabStop = False
        '
        'TextBoxItemId
        '
        Me.TextBoxItemId.BackColor = System.Drawing.Color.White
        Me.TextBoxItemId.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxItemId.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBoxItemId.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.TextBoxItemId.Location = New System.Drawing.Point(938, 16)
        Me.TextBoxItemId.MaxByteLength = 7
        Me.TextBoxItemId.Name = "TextBoxItemId"
        Me.TextBoxItemId.OldText = Nothing
        Me.TextBoxItemId.ReadOnly = True
        Me.TextBoxItemId.Size = New System.Drawing.Size(67, 23)
        Me.TextBoxItemId.TabIndex = 8
        Me.TextBoxItemId.TabStop = False
        Me.TextBoxItemId.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TextBoxItemId.Visible = False
        '
        'ButtonSearchItem
        '
        Me.ButtonSearchItem.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ButtonSearchItem.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonSearchItem.Image = Global.DeliPlanner.My.Resources.Resources.search
        Me.ButtonSearchItem.Location = New System.Drawing.Point(1005, 15)
        Me.ButtonSearchItem.Name = "ButtonSearchItem"
        Me.ButtonSearchItem.Size = New System.Drawing.Size(30, 25)
        Me.ButtonSearchItem.TabIndex = 9
        Me.ButtonSearchItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonSearchItem.UseVisualStyleBackColor = False
        '
        'ButtonClearItem
        '
        Me.ButtonClearItem.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ButtonClearItem.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonClearItem.Image = Global.DeliPlanner.My.Resources.Resources.cross
        Me.ButtonClearItem.Location = New System.Drawing.Point(1034, 15)
        Me.ButtonClearItem.Name = "ButtonClearItem"
        Me.ButtonClearItem.Size = New System.Drawing.Size(30, 25)
        Me.ButtonClearItem.TabIndex = 10
        Me.ButtonClearItem.TabStop = False
        Me.ButtonClearItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonClearItem.UseVisualStyleBackColor = False
        '
        'CheckBoxSummary
        '
        Me.CheckBoxSummary.Checked = True
        Me.CheckBoxSummary.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CheckBoxSummary.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.CheckBoxSummary.Location = New System.Drawing.Point(1081, 15)
        Me.CheckBoxSummary.Name = "CheckBoxSummary"
        Me.CheckBoxSummary.Size = New System.Drawing.Size(120, 25)
        Me.CheckBoxSummary.TabIndex = 11
        Me.CheckBoxSummary.Text = "品目別集計"
        Me.CheckBoxSummary.UseVisualStyleBackColor = True
        '
        'ButtonSearch
        '
        Me.ButtonSearch.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonSearch.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ButtonSearch.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonSearch.Image = Global.DeliPlanner.My.Resources.Resources.search
        Me.ButtonSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ButtonSearch.Location = New System.Drawing.Point(1236, 12)
        Me.ButtonSearch.Name = "ButtonSearch"
        Me.ButtonSearch.Size = New System.Drawing.Size(120, 30)
        Me.ButtonSearch.TabIndex = 12
        Me.ButtonSearch.Text = "検索"
        Me.ButtonSearch.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonSearch.UseVisualStyleBackColor = False
        '
        'PanelItems
        '
        Me.PanelItems.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PanelItems.Location = New System.Drawing.Point(12, 48)
        Me.PanelItems.Name = "PanelItems"
        Me.PanelItems.Size = New System.Drawing.Size(1344, 771)
        Me.PanelItems.TabIndex = 13
        '
        'LabelTotal
        '
        Me.LabelTotal.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LabelTotal.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.LabelTotal.Location = New System.Drawing.Point(12, 829)
        Me.LabelTotal.Name = "LabelTotal"
        Me.LabelTotal.Size = New System.Drawing.Size(1344, 30)
        Me.LabelTotal.TabIndex = 14
        Me.LabelTotal.Text = "廃棄件数：0件    廃棄金額合計：0円"
        Me.LabelTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'FormInventoryDisposals
        '
        Me.AcceptButton = Me.ButtonSearch
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CausesValidation = False
        Me.ClientSize = New System.Drawing.Size(1368, 871)
        Me.Name = "FormInventoryDisposals"
        Me.Text = "廃棄一覧"
        Me.PanelMain.ResumeLayout(False)
        Me.PanelMain.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents LabelPeriod As System.Windows.Forms.Label
    Friend WithEvents DatePickerFrom As System.Windows.Forms.DateTimePicker
    Friend WithEvents LabelPeriodSeparator As System.Windows.Forms.Label
    Friend WithEvents DatePickerTo As System.Windows.Forms.DateTimePicker
    Friend WithEvents LabelItemType As System.Windows.Forms.Label
    Friend WithEvents ComboBoxItemType As System.Windows.Forms.ComboBox
    Friend WithEvents LabelItem As System.Windows.Forms.Label
    Friend WithEvents TextBoxItemName As TOA.ToaTextBox
    Friend WithEvents TextBoxItemId As TOA.ToaTextBox
    Friend WithEvents ButtonSearchItem As System.Windows.Forms.Button
    Friend WithEvents ButtonClearItem As System.Windows.Forms.Button
    Friend WithEvents CheckBoxSummary As System.Windows.Forms.CheckBox
    Friend WithEvents ButtonSearch As System.Windows.Forms.Button
    Friend WithEvents PanelItems As System.Windows.Forms.Panel
    Friend WithEvents LabelTotal As System.Windows.Forms.Label

End Class