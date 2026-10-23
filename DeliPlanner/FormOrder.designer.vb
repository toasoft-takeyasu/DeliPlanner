<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormOrder
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
        Me.PanelOrders = New System.Windows.Forms.Panel()
        Me.ButtonClearSupplier = New System.Windows.Forms.Button()
        Me.ButtonSearchSupplier = New System.Windows.Forms.Button()
        Me.TextBoxSupplierName = New TOA.ToaTextBox()
        Me.ButtonOrderDatePrevious = New System.Windows.Forms.Button()
        Me.ButtonOrderDateNext = New System.Windows.Forms.Button()
        Me.DateBoxOrderDate = New TOA.ToaDateBox()
        Me.TextBoxSupplierId = New TOA.ToaTextBox()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.ButtonDelete = New System.Windows.Forms.Button()
        Me.LabelMode = New System.Windows.Forms.Label()
        Me.ButtonClose = New System.Windows.Forms.Button()
        Me.DateBoxArrivalPlanDate = New TOA.ToaDateBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.ButtonArrivalPlanDatePrevious = New System.Windows.Forms.Button()
        Me.ButtonArrivalPlanDateNext = New System.Windows.Forms.Button()
        Me.TextBoxItemTypeId = New TOA.ToaTextBox()
        Me.PanelMain.SuspendLayout()
        Me.SuspendLayout()
        '
        'PanelMain
        '
        Me.PanelMain.Controls.Add(Me.ButtonClearSupplier)
        Me.PanelMain.Controls.Add(Me.ButtonSearchSupplier)
        Me.PanelMain.Controls.Add(Me.TextBoxItemTypeId)
        Me.PanelMain.Controls.Add(Me.DateBoxArrivalPlanDate)
        Me.PanelMain.Controls.Add(Me.ButtonArrivalPlanDatePrevious)
        Me.PanelMain.Controls.Add(Me.ButtonArrivalPlanDateNext)
        Me.PanelMain.Controls.Add(Me.DateBoxOrderDate)
        Me.PanelMain.Controls.Add(Me.ButtonOrderDatePrevious)
        Me.PanelMain.Controls.Add(Me.ButtonOrderDateNext)
        Me.PanelMain.Controls.Add(Me.Label3)
        Me.PanelMain.Controls.Add(Me.Label2)
        Me.PanelMain.Controls.Add(Me.ButtonClose)
        Me.PanelMain.Controls.Add(Me.LabelMode)
        Me.PanelMain.Controls.Add(Me.ButtonDelete)
        Me.PanelMain.Controls.Add(Me.Label1)
        Me.PanelMain.Controls.Add(Me.Button1)
        Me.PanelMain.Controls.Add(Me.TextBoxSupplierId)
        Me.PanelMain.Controls.Add(Me.TextBoxSupplierName)
        Me.PanelMain.Controls.Add(Me.PanelOrders)
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
        Me.ButtonRegist.Location = New System.Drawing.Point(984, 829)
        Me.ButtonRegist.Name = "ButtonRegist"
        Me.ButtonRegist.Size = New System.Drawing.Size(120, 30)
        Me.ButtonRegist.TabIndex = 12
        Me.ButtonRegist.TabStop = False
        Me.ButtonRegist.Text = "登録"
        Me.ButtonRegist.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonRegist.UseVisualStyleBackColor = False
        '
        'PanelOrders
        '
        Me.PanelOrders.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PanelOrders.Location = New System.Drawing.Point(12, 48)
        Me.PanelOrders.Name = "PanelOrders"
        Me.PanelOrders.Size = New System.Drawing.Size(1344, 771)
        Me.PanelOrders.TabIndex = 10
        '
        'ButtonClearSupplier
        '
        Me.ButtonClearSupplier.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ButtonClearSupplier.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonClearSupplier.Image = Global.DeliPlanner.My.Resources.Resources.cross
        Me.ButtonClearSupplier.Location = New System.Drawing.Point(769, 15)
        Me.ButtonClearSupplier.Name = "ButtonClearSupplier"
        Me.ButtonClearSupplier.Size = New System.Drawing.Size(30, 25)
        Me.ButtonClearSupplier.TabIndex = 8
        Me.ButtonClearSupplier.TabStop = False
        Me.ButtonClearSupplier.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.ButtonClearSupplier.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonClearSupplier.UseVisualStyleBackColor = False
        '
        'ButtonSearchSupplier
        '
        Me.ButtonSearchSupplier.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ButtonSearchSupplier.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonSearchSupplier.Image = Global.DeliPlanner.My.Resources.Resources.search
        Me.ButtonSearchSupplier.Location = New System.Drawing.Point(740, 15)
        Me.ButtonSearchSupplier.Name = "ButtonSearchSupplier"
        Me.ButtonSearchSupplier.Size = New System.Drawing.Size(30, 25)
        Me.ButtonSearchSupplier.TabIndex = 7
        Me.ButtonSearchSupplier.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ButtonSearchSupplier.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonSearchSupplier.UseVisualStyleBackColor = False
        '
        'TextBoxSupplierName
        '
        Me.TextBoxSupplierName.BackColor = System.Drawing.SystemColors.Window
        Me.TextBoxSupplierName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxSupplierName.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBoxSupplierName.ImeMode = System.Windows.Forms.ImeMode.Hiragana
        Me.TextBoxSupplierName.Location = New System.Drawing.Point(492, 16)
        Me.TextBoxSupplierName.MaxByteLength = 40
        Me.TextBoxSupplierName.Name = "TextBoxSupplierName"
        Me.TextBoxSupplierName.OldText = Nothing
        Me.TextBoxSupplierName.ReadOnly = True
        Me.TextBoxSupplierName.Size = New System.Drawing.Size(248, 23)
        Me.TextBoxSupplierName.TabIndex = 6
        Me.TextBoxSupplierName.TabStop = False
        Me.TextBoxSupplierName.Text = "１２３４５６７８９０１２３４５６７８９０１２３４５６７８９０"
        '
        'ButtonOrderDatePrevious
        '
        Me.ButtonOrderDatePrevious.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ButtonOrderDatePrevious.Font = New System.Drawing.Font("MS UI Gothic", 3.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonOrderDatePrevious.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ButtonOrderDatePrevious.Location = New System.Drawing.Point(337, 27)
        Me.ButtonOrderDatePrevious.Name = "ButtonOrderDatePrevious"
        Me.ButtonOrderDatePrevious.Size = New System.Drawing.Size(33, 13)
        Me.ButtonOrderDatePrevious.TabIndex = 0
        Me.ButtonOrderDatePrevious.TabStop = False
        Me.ButtonOrderDatePrevious.Text = "▼"
        Me.ButtonOrderDatePrevious.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonOrderDatePrevious.UseVisualStyleBackColor = False
        '
        'ButtonOrderDateNext
        '
        Me.ButtonOrderDateNext.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ButtonOrderDateNext.Font = New System.Drawing.Font("MS UI Gothic", 3.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonOrderDateNext.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ButtonOrderDateNext.Location = New System.Drawing.Point(337, 15)
        Me.ButtonOrderDateNext.Name = "ButtonOrderDateNext"
        Me.ButtonOrderDateNext.Size = New System.Drawing.Size(33, 13)
        Me.ButtonOrderDateNext.TabIndex = 2
        Me.ButtonOrderDateNext.TabStop = False
        Me.ButtonOrderDateNext.Text = "▲"
        Me.ButtonOrderDateNext.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonOrderDateNext.UseVisualStyleBackColor = False
        '
        'DateBoxOrderDate
        '
        Me.DateBoxOrderDate.AcceptsTab = True
        Me.DateBoxOrderDate.BackColor = System.Drawing.Color.White
        Me.DateBoxOrderDate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.DateBoxOrderDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.DateBoxOrderDate.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.DateBoxOrderDate.Location = New System.Drawing.Point(233, 16)
        Me.DateBoxOrderDate.MaxLength = 10
        Me.DateBoxOrderDate.Name = "DateBoxOrderDate"
        Me.DateBoxOrderDate.OldText = Nothing
        Me.DateBoxOrderDate.Size = New System.Drawing.Size(104, 23)
        Me.DateBoxOrderDate.TabIndex = 1
        Me.DateBoxOrderDate.Text = "yyyy/MM/dd"
        Me.DateBoxOrderDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'TextBoxSupplierId
        '
        Me.TextBoxSupplierId.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextBoxSupplierId.BackColor = System.Drawing.Color.White
        Me.TextBoxSupplierId.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxSupplierId.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBoxSupplierId.Format = "#,###"
        Me.TextBoxSupplierId.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.TextBoxSupplierId.Location = New System.Drawing.Point(673, 16)
        Me.TextBoxSupplierId.MaxByteLength = 7
        Me.TextBoxSupplierId.MaxValue = "0"
        Me.TextBoxSupplierId.MinValue = "-999999"
        Me.TextBoxSupplierId.Name = "TextBoxSupplierId"
        Me.TextBoxSupplierId.NumericOnly = True
        Me.TextBoxSupplierId.OldText = Nothing
        Me.TextBoxSupplierId.Size = New System.Drawing.Size(67, 23)
        Me.TextBoxSupplierId.TabIndex = 9
        Me.TextBoxSupplierId.TabStop = False
        Me.TextBoxSupplierId.Text = "0"
        Me.TextBoxSupplierId.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TextBoxSupplierId.Visible = False
        '
        'Button1
        '
        Me.Button1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Button1.BackColor = System.Drawing.SystemColors.ControlLight
        Me.Button1.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button1.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Button1.Location = New System.Drawing.Point(12, 829)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(120, 30)
        Me.Button1.TabIndex = 11
        Me.Button1.TabStop = False
        Me.Button1.Text = "発注書"
        Me.Button1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.Button1.UseVisualStyleBackColor = False
        '
        'Label1
        '
        Me.Label1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption
        Me.Label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label1.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(392, 16)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(101, 24)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "仕入先"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'ButtonDelete
        '
        Me.ButtonDelete.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonDelete.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ButtonDelete.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonDelete.Image = Global.DeliPlanner.My.Resources.Resources.trashcan
        Me.ButtonDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ButtonDelete.Location = New System.Drawing.Point(1110, 829)
        Me.ButtonDelete.Name = "ButtonDelete"
        Me.ButtonDelete.Size = New System.Drawing.Size(120, 30)
        Me.ButtonDelete.TabIndex = 56
        Me.ButtonDelete.TabStop = False
        Me.ButtonDelete.Text = "削除"
        Me.ButtonDelete.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonDelete.UseVisualStyleBackColor = False
        '
        'LabelMode
        '
        Me.LabelMode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LabelMode.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.LabelMode.Location = New System.Drawing.Point(12, 12)
        Me.LabelMode.Name = "LabelMode"
        Me.LabelMode.Size = New System.Drawing.Size(101, 30)
        Me.LabelMode.TabIndex = 57
        Me.LabelMode.Text = "モード"
        Me.LabelMode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'ButtonClose
        '
        Me.ButtonClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonClose.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ButtonClose.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonClose.Image = Global.DeliPlanner.My.Resources.Resources.cross
        Me.ButtonClose.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ButtonClose.Location = New System.Drawing.Point(1236, 829)
        Me.ButtonClose.Name = "ButtonClose"
        Me.ButtonClose.Size = New System.Drawing.Size(120, 30)
        Me.ButtonClose.TabIndex = 58
        Me.ButtonClose.TabStop = False
        Me.ButtonClose.Text = "キャンセル"
        Me.ButtonClose.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonClose.UseVisualStyleBackColor = False
        '
        'DateBoxArrivalPlanDate
        '
        Me.DateBoxArrivalPlanDate.AcceptsTab = True
        Me.DateBoxArrivalPlanDate.BackColor = System.Drawing.Color.White
        Me.DateBoxArrivalPlanDate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.DateBoxArrivalPlanDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.DateBoxArrivalPlanDate.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.DateBoxArrivalPlanDate.Location = New System.Drawing.Point(969, 16)
        Me.DateBoxArrivalPlanDate.MaxLength = 10
        Me.DateBoxArrivalPlanDate.Name = "DateBoxArrivalPlanDate"
        Me.DateBoxArrivalPlanDate.OldText = Nothing
        Me.DateBoxArrivalPlanDate.Size = New System.Drawing.Size(104, 23)
        Me.DateBoxArrivalPlanDate.TabIndex = 59
        Me.DateBoxArrivalPlanDate.Text = "yyyy/MM/dd"
        Me.DateBoxArrivalPlanDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label2
        '
        Me.Label2.BackColor = System.Drawing.SystemColors.GradientInactiveCaption
        Me.Label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label2.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(869, 16)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(101, 23)
        Me.Label2.TabIndex = 60
        Me.Label2.Text = "入荷予定日"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label3
        '
        Me.Label3.BackColor = System.Drawing.SystemColors.GradientInactiveCaption
        Me.Label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label3.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label3.Location = New System.Drawing.Point(133, 16)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(101, 23)
        Me.Label3.TabIndex = 61
        Me.Label3.Text = "発注日"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'ButtonArrivalPlanDatePrevious
        '
        Me.ButtonArrivalPlanDatePrevious.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ButtonArrivalPlanDatePrevious.Font = New System.Drawing.Font("MS UI Gothic", 3.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonArrivalPlanDatePrevious.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ButtonArrivalPlanDatePrevious.Location = New System.Drawing.Point(1073, 27)
        Me.ButtonArrivalPlanDatePrevious.Name = "ButtonArrivalPlanDatePrevious"
        Me.ButtonArrivalPlanDatePrevious.Size = New System.Drawing.Size(33, 13)
        Me.ButtonArrivalPlanDatePrevious.TabIndex = 62
        Me.ButtonArrivalPlanDatePrevious.TabStop = False
        Me.ButtonArrivalPlanDatePrevious.Text = "▼"
        Me.ButtonArrivalPlanDatePrevious.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonArrivalPlanDatePrevious.UseVisualStyleBackColor = False
        '
        'ButtonArrivalPlanDateNext
        '
        Me.ButtonArrivalPlanDateNext.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ButtonArrivalPlanDateNext.Font = New System.Drawing.Font("MS UI Gothic", 3.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonArrivalPlanDateNext.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ButtonArrivalPlanDateNext.Location = New System.Drawing.Point(1073, 15)
        Me.ButtonArrivalPlanDateNext.Name = "ButtonArrivalPlanDateNext"
        Me.ButtonArrivalPlanDateNext.Size = New System.Drawing.Size(33, 13)
        Me.ButtonArrivalPlanDateNext.TabIndex = 63
        Me.ButtonArrivalPlanDateNext.TabStop = False
        Me.ButtonArrivalPlanDateNext.Text = "▲"
        Me.ButtonArrivalPlanDateNext.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonArrivalPlanDateNext.UseVisualStyleBackColor = False
        '
        'TextBoxItemTypeId
        '
        Me.TextBoxItemTypeId.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextBoxItemTypeId.BackColor = System.Drawing.Color.White
        Me.TextBoxItemTypeId.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxItemTypeId.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBoxItemTypeId.Format = "#,###"
        Me.TextBoxItemTypeId.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.TextBoxItemTypeId.Location = New System.Drawing.Point(1112, 16)
        Me.TextBoxItemTypeId.MaxByteLength = 7
        Me.TextBoxItemTypeId.MaxValue = "0"
        Me.TextBoxItemTypeId.MinValue = "-999999"
        Me.TextBoxItemTypeId.Name = "TextBoxItemTypeId"
        Me.TextBoxItemTypeId.NumericOnly = True
        Me.TextBoxItemTypeId.OldText = Nothing
        Me.TextBoxItemTypeId.Size = New System.Drawing.Size(29, 23)
        Me.TextBoxItemTypeId.TabIndex = 64
        Me.TextBoxItemTypeId.TabStop = False
        Me.TextBoxItemTypeId.Text = "0"
        Me.TextBoxItemTypeId.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TextBoxItemTypeId.Visible = False
        '
        'FormOrder
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CausesValidation = False
        Me.ClientSize = New System.Drawing.Size(1368, 871)
        Me.Name = "FormOrder"
        Me.Text = "FormOrders"
        Me.PanelMain.ResumeLayout(False)
        Me.PanelMain.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ButtonRegist As Button
    Friend WithEvents PanelOrders As Panel
    Friend WithEvents ButtonClearSupplier As Button
    Friend WithEvents ButtonSearchSupplier As Button
    Friend WithEvents TextBoxSupplierName As TOA.ToaTextBox
    Friend WithEvents ButtonOrderDatePrevious As Button
    Friend WithEvents ButtonOrderDateNext As Button
    Friend WithEvents DateBoxOrderDate As TOA.ToaDateBox
    Friend WithEvents TextBoxSupplierId As TOA.ToaTextBox
    Friend WithEvents Button1 As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents ButtonDelete As Button
    Friend WithEvents LabelMode As Label
    Friend WithEvents ButtonClose As Button
    Friend WithEvents Label2 As Label
    Friend WithEvents DateBoxArrivalPlanDate As TOA.ToaDateBox
    Friend WithEvents Label3 As Label
    Friend WithEvents ButtonArrivalPlanDatePrevious As Button
    Friend WithEvents ButtonArrivalPlanDateNext As Button
    Friend WithEvents TextBoxItemTypeId As TOA.ToaTextBox
End Class
