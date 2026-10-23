<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DialogOrder
    Inherits TOA.ToaDialog

    'フォームがコンポーネントの一覧をクリーンアップするために dispose をオーバーライドします。
    <System.Diagnostics.DebuggerNonUserCode()> _
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(DialogOrder))
        Me.TextBoxItemName = New TOA.ToaTextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.ComboBoxUnitType = New System.Windows.Forms.ComboBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TextBoxQuantity = New TOA.ToaTextBox()
        Me.DateBoxOrderDate = New TOA.ToaDateBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.TextBoxSupplierId = New TOA.ToaTextBox()
        Me.ButtonClearSupplier = New System.Windows.Forms.Button()
        Me.ButtonSearchSupplier = New System.Windows.Forms.Button()
        Me.TextBoxSupplierName = New TOA.ToaTextBox()
        Me.ButtonRegist = New System.Windows.Forms.Button()
        Me.ButtonClose = New System.Windows.Forms.Button()
        Me.TextBoxItemId = New TOA.ToaTextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.DateBoxArrivalPlanDate = New TOA.ToaDateBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.ComboBoxNetWeightUnitType = New System.Windows.Forms.ComboBox()
        Me.TextBoxNeyWeight = New TOA.ToaTextBox()
        Me.TextBoxPerCase = New TOA.ToaTextBox()
        Me.TextBoxItemTypeId = New TOA.ToaTextBox()
        Me.SuspendLayout()
        '
        'TextBoxItemName
        '
        Me.TextBoxItemName.BackColor = System.Drawing.Color.White
        Me.TextBoxItemName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxItemName.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBoxItemName.ImeMode = System.Windows.Forms.ImeMode.Hiragana
        Me.TextBoxItemName.Location = New System.Drawing.Point(112, 9)
        Me.TextBoxItemName.MaxByteLength = 40
        Me.TextBoxItemName.Name = "TextBoxItemName"
        Me.TextBoxItemName.OldText = Nothing
        Me.TextBoxItemName.ReadOnly = True
        Me.TextBoxItemName.Size = New System.Drawing.Size(516, 23)
        Me.TextBoxItemName.TabIndex = 1
        Me.TextBoxItemName.TabStop = False
        Me.TextBoxItemName.Text = "１２３４５６７８９０１２３４５６７８９０１２３４５６７８９０"
        '
        'Label1
        '
        Me.Label1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption
        Me.Label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label1.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(12, 157)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(101, 23)
        Me.Label1.TabIndex = 5
        Me.Label1.Text = "数量"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'ComboBoxUnitType
        '
        Me.ComboBoxUnitType.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ComboBoxUnitType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBoxUnitType.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ComboBoxUnitType.FormattingEnabled = True
        Me.ComboBoxUnitType.Location = New System.Drawing.Point(191, 157)
        Me.ComboBoxUnitType.Name = "ComboBoxUnitType"
        Me.ComboBoxUnitType.Size = New System.Drawing.Size(80, 23)
        Me.ComboBoxUnitType.TabIndex = 7
        '
        'Label2
        '
        Me.Label2.BackColor = System.Drawing.SystemColors.GradientInactiveCaption
        Me.Label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label2.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(12, 9)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(101, 23)
        Me.Label2.TabIndex = 0
        Me.Label2.Text = "品目名"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TextBoxQuantity
        '
        Me.TextBoxQuantity.BackColor = System.Drawing.Color.White
        Me.TextBoxQuantity.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxQuantity.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBoxQuantity.Format = "#,###"
        Me.TextBoxQuantity.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.TextBoxQuantity.Location = New System.Drawing.Point(112, 157)
        Me.TextBoxQuantity.MaxByteLength = 7
        Me.TextBoxQuantity.MaxValue = "0"
        Me.TextBoxQuantity.MinValue = "-999999"
        Me.TextBoxQuantity.Name = "TextBoxQuantity"
        Me.TextBoxQuantity.NumericOnly = True
        Me.TextBoxQuantity.OldText = Nothing
        Me.TextBoxQuantity.Size = New System.Drawing.Size(80, 23)
        Me.TextBoxQuantity.TabIndex = 6
        Me.TextBoxQuantity.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'DateBoxOrderDate
        '
        Me.DateBoxOrderDate.AcceptsTab = True
        Me.DateBoxOrderDate.BackColor = System.Drawing.Color.White
        Me.DateBoxOrderDate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.DateBoxOrderDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.DateBoxOrderDate.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.DateBoxOrderDate.Location = New System.Drawing.Point(112, 38)
        Me.DateBoxOrderDate.MaxLength = 10
        Me.DateBoxOrderDate.Name = "DateBoxOrderDate"
        Me.DateBoxOrderDate.OldText = Nothing
        Me.DateBoxOrderDate.Size = New System.Drawing.Size(104, 23)
        Me.DateBoxOrderDate.TabIndex = 4
        Me.DateBoxOrderDate.Text = "yyyy/MM/dd"
        Me.DateBoxOrderDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label3
        '
        Me.Label3.BackColor = System.Drawing.SystemColors.GradientInactiveCaption
        Me.Label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label3.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label3.Location = New System.Drawing.Point(12, 38)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(101, 23)
        Me.Label3.TabIndex = 3
        Me.Label3.Text = "発注日"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label4
        '
        Me.Label4.BackColor = System.Drawing.SystemColors.GradientInactiveCaption
        Me.Label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label4.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label4.Location = New System.Drawing.Point(12, 67)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(101, 24)
        Me.Label4.TabIndex = 5
        Me.Label4.Text = "仕入先"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TextBoxSupplierId
        '
        Me.TextBoxSupplierId.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextBoxSupplierId.BackColor = System.Drawing.Color.White
        Me.TextBoxSupplierId.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxSupplierId.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBoxSupplierId.Format = "#,###"
        Me.TextBoxSupplierId.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.TextBoxSupplierId.Location = New System.Drawing.Point(293, 67)
        Me.TextBoxSupplierId.MaxByteLength = 7
        Me.TextBoxSupplierId.MaxValue = "0"
        Me.TextBoxSupplierId.MinValue = "-999999"
        Me.TextBoxSupplierId.Name = "TextBoxSupplierId"
        Me.TextBoxSupplierId.NumericOnly = True
        Me.TextBoxSupplierId.OldText = Nothing
        Me.TextBoxSupplierId.Size = New System.Drawing.Size(67, 23)
        Me.TextBoxSupplierId.TabIndex = 7
        Me.TextBoxSupplierId.TabStop = False
        Me.TextBoxSupplierId.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TextBoxSupplierId.Visible = False
        '
        'ButtonClearSupplier
        '
        Me.ButtonClearSupplier.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ButtonClearSupplier.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.ButtonClearSupplier.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonClearSupplier.Image = Global.DeliPlanner.My.Resources.Resources.cross
        Me.ButtonClearSupplier.Location = New System.Drawing.Point(389, 66)
        Me.ButtonClearSupplier.Name = "ButtonClearSupplier"
        Me.ButtonClearSupplier.Size = New System.Drawing.Size(30, 25)
        Me.ButtonClearSupplier.TabIndex = 9
        Me.ButtonClearSupplier.TabStop = False
        Me.ButtonClearSupplier.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonClearSupplier.UseVisualStyleBackColor = False
        '
        'ButtonSearchSupplier
        '
        Me.ButtonSearchSupplier.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ButtonSearchSupplier.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonSearchSupplier.Image = Global.DeliPlanner.My.Resources.Resources.search
        Me.ButtonSearchSupplier.Location = New System.Drawing.Point(360, 66)
        Me.ButtonSearchSupplier.Name = "ButtonSearchSupplier"
        Me.ButtonSearchSupplier.Size = New System.Drawing.Size(30, 25)
        Me.ButtonSearchSupplier.TabIndex = 8
        Me.ButtonSearchSupplier.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonSearchSupplier.UseVisualStyleBackColor = False
        '
        'TextBoxSupplierName
        '
        Me.TextBoxSupplierName.BackColor = System.Drawing.SystemColors.Window
        Me.TextBoxSupplierName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxSupplierName.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBoxSupplierName.ImeMode = System.Windows.Forms.ImeMode.Hiragana
        Me.TextBoxSupplierName.Location = New System.Drawing.Point(112, 67)
        Me.TextBoxSupplierName.MaxByteLength = 40
        Me.TextBoxSupplierName.Name = "TextBoxSupplierName"
        Me.TextBoxSupplierName.OldText = Nothing
        Me.TextBoxSupplierName.ReadOnly = True
        Me.TextBoxSupplierName.Size = New System.Drawing.Size(248, 23)
        Me.TextBoxSupplierName.TabIndex = 6
        Me.TextBoxSupplierName.TabStop = False
        Me.TextBoxSupplierName.Text = "１２３４５６７８９０１２３４５６７８９０１２３４５６７８９０"
        '
        'ButtonRegist
        '
        Me.ButtonRegist.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonRegist.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ButtonRegist.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonRegist.Image = Global.DeliPlanner.My.Resources.Resources.save
        Me.ButtonRegist.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ButtonRegist.Location = New System.Drawing.Point(384, 151)
        Me.ButtonRegist.Name = "ButtonRegist"
        Me.ButtonRegist.Size = New System.Drawing.Size(120, 30)
        Me.ButtonRegist.TabIndex = 8
        Me.ButtonRegist.Text = "登録"
        Me.ButtonRegist.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonRegist.UseVisualStyleBackColor = False
        '
        'ButtonClose
        '
        Me.ButtonClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonClose.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ButtonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.ButtonClose.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonClose.Image = Global.DeliPlanner.My.Resources.Resources.cross
        Me.ButtonClose.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ButtonClose.Location = New System.Drawing.Point(510, 151)
        Me.ButtonClose.Name = "ButtonClose"
        Me.ButtonClose.Size = New System.Drawing.Size(120, 30)
        Me.ButtonClose.TabIndex = 9
        Me.ButtonClose.TabStop = False
        Me.ButtonClose.Text = "キャンセル"
        Me.ButtonClose.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonClose.UseVisualStyleBackColor = False
        '
        'TextBoxItemId
        '
        Me.TextBoxItemId.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextBoxItemId.BackColor = System.Drawing.Color.White
        Me.TextBoxItemId.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxItemId.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBoxItemId.Format = "#,###"
        Me.TextBoxItemId.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.TextBoxItemId.Location = New System.Drawing.Point(561, 9)
        Me.TextBoxItemId.MaxByteLength = 7
        Me.TextBoxItemId.MaxValue = "0"
        Me.TextBoxItemId.MinValue = "-999999"
        Me.TextBoxItemId.Name = "TextBoxItemId"
        Me.TextBoxItemId.NumericOnly = True
        Me.TextBoxItemId.OldText = Nothing
        Me.TextBoxItemId.Size = New System.Drawing.Size(67, 23)
        Me.TextBoxItemId.TabIndex = 2
        Me.TextBoxItemId.TabStop = False
        Me.TextBoxItemId.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TextBoxItemId.Visible = False
        '
        'Label5
        '
        Me.Label5.BackColor = System.Drawing.SystemColors.GradientInactiveCaption
        Me.Label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label5.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label5.Location = New System.Drawing.Point(263, 38)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(101, 23)
        Me.Label5.TabIndex = 15
        Me.Label5.Text = "入荷予定日"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'DateBoxArrivalPlanDate
        '
        Me.DateBoxArrivalPlanDate.AcceptsTab = True
        Me.DateBoxArrivalPlanDate.BackColor = System.Drawing.Color.White
        Me.DateBoxArrivalPlanDate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.DateBoxArrivalPlanDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.DateBoxArrivalPlanDate.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.DateBoxArrivalPlanDate.Location = New System.Drawing.Point(363, 38)
        Me.DateBoxArrivalPlanDate.MaxLength = 10
        Me.DateBoxArrivalPlanDate.Name = "DateBoxArrivalPlanDate"
        Me.DateBoxArrivalPlanDate.OldText = Nothing
        Me.DateBoxArrivalPlanDate.Size = New System.Drawing.Size(104, 23)
        Me.DateBoxArrivalPlanDate.TabIndex = 16
        Me.DateBoxArrivalPlanDate.Text = "yyyy/MM/dd"
        Me.DateBoxArrivalPlanDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label6
        '
        Me.Label6.BackColor = System.Drawing.SystemColors.GradientInactiveCaption
        Me.Label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label6.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label6.Location = New System.Drawing.Point(12, 97)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(101, 24)
        Me.Label6.TabIndex = 0
        Me.Label6.Text = "内容量"
        Me.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label7
        '
        Me.Label7.BackColor = System.Drawing.SystemColors.GradientInactiveCaption
        Me.Label7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label7.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label7.Location = New System.Drawing.Point(12, 127)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(101, 24)
        Me.Label7.TabIndex = 3
        Me.Label7.Text = "入数"
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'ComboBoxNetWeightUnitType
        '
        Me.ComboBoxNetWeightUnitType.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ComboBoxNetWeightUnitType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBoxNetWeightUnitType.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ComboBoxNetWeightUnitType.FormattingEnabled = True
        Me.ComboBoxNetWeightUnitType.Location = New System.Drawing.Point(161, 98)
        Me.ComboBoxNetWeightUnitType.Name = "ComboBoxNetWeightUnitType"
        Me.ComboBoxNetWeightUnitType.Size = New System.Drawing.Size(80, 23)
        Me.ComboBoxNetWeightUnitType.TabIndex = 2
        '
        'TextBoxNeyWeight
        '
        Me.TextBoxNeyWeight.BackColor = System.Drawing.Color.White
        Me.TextBoxNeyWeight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxNeyWeight.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBoxNeyWeight.Format = "#,###"
        Me.TextBoxNeyWeight.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.TextBoxNeyWeight.Location = New System.Drawing.Point(112, 98)
        Me.TextBoxNeyWeight.MaxByteLength = 7
        Me.TextBoxNeyWeight.MaxValue = "0"
        Me.TextBoxNeyWeight.MinValue = "-999999"
        Me.TextBoxNeyWeight.Name = "TextBoxNeyWeight"
        Me.TextBoxNeyWeight.NumericOnly = True
        Me.TextBoxNeyWeight.OldText = Nothing
        Me.TextBoxNeyWeight.Size = New System.Drawing.Size(50, 23)
        Me.TextBoxNeyWeight.TabIndex = 1
        Me.TextBoxNeyWeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TextBoxPerCase
        '
        Me.TextBoxPerCase.BackColor = System.Drawing.Color.White
        Me.TextBoxPerCase.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxPerCase.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBoxPerCase.Format = "#,###"
        Me.TextBoxPerCase.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.TextBoxPerCase.Location = New System.Drawing.Point(112, 128)
        Me.TextBoxPerCase.MaxByteLength = 7
        Me.TextBoxPerCase.MaxValue = "0"
        Me.TextBoxPerCase.MinValue = "-999999"
        Me.TextBoxPerCase.Name = "TextBoxPerCase"
        Me.TextBoxPerCase.NumericOnly = True
        Me.TextBoxPerCase.OldText = Nothing
        Me.TextBoxPerCase.Size = New System.Drawing.Size(50, 23)
        Me.TextBoxPerCase.TabIndex = 4
        Me.TextBoxPerCase.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TextBoxItemTypeId
        '
        Me.TextBoxItemTypeId.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextBoxItemTypeId.BackColor = System.Drawing.Color.White
        Me.TextBoxItemTypeId.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxItemTypeId.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBoxItemTypeId.Format = "#,###"
        Me.TextBoxItemTypeId.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.TextBoxItemTypeId.Location = New System.Drawing.Point(561, 38)
        Me.TextBoxItemTypeId.MaxByteLength = 7
        Me.TextBoxItemTypeId.MaxValue = "0"
        Me.TextBoxItemTypeId.MinValue = "-999999"
        Me.TextBoxItemTypeId.Name = "TextBoxItemTypeId"
        Me.TextBoxItemTypeId.NumericOnly = True
        Me.TextBoxItemTypeId.OldText = Nothing
        Me.TextBoxItemTypeId.Size = New System.Drawing.Size(67, 23)
        Me.TextBoxItemTypeId.TabIndex = 17
        Me.TextBoxItemTypeId.TabStop = False
        Me.TextBoxItemTypeId.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TextBoxItemTypeId.Visible = False
        '
        'DialogOrder
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.ButtonClose
        Me.ClientSize = New System.Drawing.Size(642, 193)
        Me.Controls.Add(Me.TextBoxItemTypeId)
        Me.Controls.Add(Me.TextBoxPerCase)
        Me.Controls.Add(Me.ComboBoxNetWeightUnitType)
        Me.Controls.Add(Me.TextBoxNeyWeight)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.DateBoxArrivalPlanDate)
        Me.Controls.Add(Me.TextBoxItemId)
        Me.Controls.Add(Me.ButtonRegist)
        Me.Controls.Add(Me.ButtonClose)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.TextBoxSupplierId)
        Me.Controls.Add(Me.ButtonClearSupplier)
        Me.Controls.Add(Me.ButtonSearchSupplier)
        Me.Controls.Add(Me.TextBoxSupplierName)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.DateBoxOrderDate)
        Me.Controls.Add(Me.TextBoxItemName)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.ComboBoxUnitType)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.TextBoxQuantity)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "DialogOrder"
        Me.Text = "発注"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TextBoxItemName As TOA.ToaTextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents ComboBoxUnitType As ComboBox
    Friend WithEvents Label2 As Label
    Friend WithEvents TextBoxQuantity As TOA.ToaTextBox
    Friend WithEvents DateBoxOrderDate As TOA.ToaDateBox
    Friend WithEvents Label3 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents TextBoxSupplierId As TOA.ToaTextBox
    Friend WithEvents ButtonClearSupplier As Button
    Friend WithEvents ButtonSearchSupplier As Button
    Friend WithEvents TextBoxSupplierName As TOA.ToaTextBox
    Friend WithEvents ButtonRegist As Button
    Friend WithEvents ButtonClose As Button
    Friend WithEvents TextBoxItemId As TOA.ToaTextBox
    Friend WithEvents Label5 As Label
    Friend WithEvents DateBoxArrivalPlanDate As TOA.ToaDateBox
    Friend WithEvents Label6 As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents ComboBoxNetWeightUnitType As ComboBox
    Friend WithEvents TextBoxNeyWeight As TOA.ToaTextBox
    Friend WithEvents TextBoxPerCase As TOA.ToaTextBox
    Friend WithEvents TextBoxItemTypeId As TOA.ToaTextBox
End Class
