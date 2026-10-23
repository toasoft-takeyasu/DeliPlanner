<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormArrival
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
        Me.PanelArrivals = New System.Windows.Forms.Panel()
        Me.ButtonRegist = New System.Windows.Forms.Button()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.ButtonClearSupplier = New System.Windows.Forms.Button()
        Me.ButtonSearchSupplier = New System.Windows.Forms.Button()
        Me.TextBoxSupplierName = New TOA.ToaTextBox()
        Me.ButtonArrivalDatePrevious = New System.Windows.Forms.Button()
        Me.ButtonArrivalDateNext = New System.Windows.Forms.Button()
        Me.DateBoxArrivalDate = New TOA.ToaDateBox()
        Me.TextBoxSupplierId = New TOA.ToaTextBox()
        Me.Label58 = New System.Windows.Forms.Label()
        Me.LabelMode = New System.Windows.Forms.Label()
        Me.ButtonClose = New System.Windows.Forms.Button()
        Me.ButtonDelete = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TextBoxItemTypeId = New TOA.ToaTextBox()
        Me.PanelMain.SuspendLayout()
        Me.SuspendLayout()
        '
        'PanelMain
        '
        Me.PanelMain.Controls.Add(Me.TextBoxItemTypeId)
        Me.PanelMain.Controls.Add(Me.DateBoxArrivalDate)
        Me.PanelMain.Controls.Add(Me.Label1)
        Me.PanelMain.Controls.Add(Me.ButtonClose)
        Me.PanelMain.Controls.Add(Me.ButtonDelete)
        Me.PanelMain.Controls.Add(Me.LabelMode)
        Me.PanelMain.Controls.Add(Me.TextBoxSupplierId)
        Me.PanelMain.Controls.Add(Me.Label58)
        Me.PanelMain.Controls.Add(Me.ButtonClearSupplier)
        Me.PanelMain.Controls.Add(Me.ButtonSearchSupplier)
        Me.PanelMain.Controls.Add(Me.TextBoxSupplierName)
        Me.PanelMain.Controls.Add(Me.ButtonArrivalDatePrevious)
        Me.PanelMain.Controls.Add(Me.ButtonArrivalDateNext)
        Me.PanelMain.Controls.Add(Me.Button2)
        Me.PanelMain.Controls.Add(Me.ButtonRegist)
        Me.PanelMain.Controls.Add(Me.PanelArrivals)
        Me.PanelMain.Size = New System.Drawing.Size(1368, 871)
        '
        'PanelArrivals
        '
        Me.PanelArrivals.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PanelArrivals.Location = New System.Drawing.Point(12, 48)
        Me.PanelArrivals.Name = "PanelArrivals"
        Me.PanelArrivals.Size = New System.Drawing.Size(1344, 771)
        Me.PanelArrivals.TabIndex = 12
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
        Me.ButtonRegist.TabIndex = 14
        Me.ButtonRegist.TabStop = False
        Me.ButtonRegist.Text = "登録"
        Me.ButtonRegist.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonRegist.UseVisualStyleBackColor = False
        '
        'Button2
        '
        Me.Button2.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Button2.BackColor = System.Drawing.SystemColors.ControlLight
        Me.Button2.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button2.Image = Global.DeliPlanner.My.Resources.Resources.import
        Me.Button2.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Button2.Location = New System.Drawing.Point(1236, 12)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(120, 30)
        Me.Button2.TabIndex = 11
        Me.Button2.TabStop = False
        Me.Button2.Text = "入荷データ"
        Me.Button2.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.Button2.UseVisualStyleBackColor = False
        '
        'ButtonClearSupplier
        '
        Me.ButtonClearSupplier.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ButtonClearSupplier.CausesValidation = False
        Me.ButtonClearSupplier.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonClearSupplier.Image = Global.DeliPlanner.My.Resources.Resources.cross
        Me.ButtonClearSupplier.Location = New System.Drawing.Point(769, 15)
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
        Me.ButtonSearchSupplier.Location = New System.Drawing.Point(740, 15)
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
        Me.TextBoxSupplierName.Location = New System.Drawing.Point(492, 16)
        Me.TextBoxSupplierName.MaxByteLength = 40
        Me.TextBoxSupplierName.Name = "TextBoxSupplierName"
        Me.TextBoxSupplierName.OldText = Nothing
        Me.TextBoxSupplierName.ReadOnly = True
        Me.TextBoxSupplierName.Size = New System.Drawing.Size(248, 23)
        Me.TextBoxSupplierName.TabIndex = 7
        Me.TextBoxSupplierName.TabStop = False
        Me.TextBoxSupplierName.Text = "１２３４５６７８９０１２３４５６７８９０１２３４５６７８９０"
        '
        'ButtonArrivalDatePrevious
        '
        Me.ButtonArrivalDatePrevious.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ButtonArrivalDatePrevious.Font = New System.Drawing.Font("MS UI Gothic", 3.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonArrivalDatePrevious.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ButtonArrivalDatePrevious.Location = New System.Drawing.Point(337, 27)
        Me.ButtonArrivalDatePrevious.Name = "ButtonArrivalDatePrevious"
        Me.ButtonArrivalDatePrevious.Size = New System.Drawing.Size(33, 13)
        Me.ButtonArrivalDatePrevious.TabIndex = 0
        Me.ButtonArrivalDatePrevious.TabStop = False
        Me.ButtonArrivalDatePrevious.Text = "▼"
        Me.ButtonArrivalDatePrevious.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonArrivalDatePrevious.UseVisualStyleBackColor = False
        '
        'ButtonArrivalDateNext
        '
        Me.ButtonArrivalDateNext.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ButtonArrivalDateNext.Font = New System.Drawing.Font("MS UI Gothic", 3.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonArrivalDateNext.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ButtonArrivalDateNext.Location = New System.Drawing.Point(337, 15)
        Me.ButtonArrivalDateNext.Name = "ButtonArrivalDateNext"
        Me.ButtonArrivalDateNext.Size = New System.Drawing.Size(33, 13)
        Me.ButtonArrivalDateNext.TabIndex = 2
        Me.ButtonArrivalDateNext.TabStop = False
        Me.ButtonArrivalDateNext.Text = "▲"
        Me.ButtonArrivalDateNext.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonArrivalDateNext.UseVisualStyleBackColor = False
        '
        'DateBoxArrivalDate
        '
        Me.DateBoxArrivalDate.AcceptsTab = True
        Me.DateBoxArrivalDate.BackColor = System.Drawing.Color.White
        Me.DateBoxArrivalDate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.DateBoxArrivalDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.DateBoxArrivalDate.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.DateBoxArrivalDate.Location = New System.Drawing.Point(233, 16)
        Me.DateBoxArrivalDate.MaxLength = 10
        Me.DateBoxArrivalDate.Name = "DateBoxArrivalDate"
        Me.DateBoxArrivalDate.OldText = Nothing
        Me.DateBoxArrivalDate.Size = New System.Drawing.Size(104, 23)
        Me.DateBoxArrivalDate.TabIndex = 1
        Me.DateBoxArrivalDate.Text = "yyyy/MM/dd"
        Me.DateBoxArrivalDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
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
        Me.TextBoxSupplierId.TabIndex = 10
        Me.TextBoxSupplierId.TabStop = False
        Me.TextBoxSupplierId.Text = "0"
        Me.TextBoxSupplierId.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TextBoxSupplierId.Visible = False
        '
        'Label58
        '
        Me.Label58.BackColor = System.Drawing.SystemColors.GradientInactiveCaption
        Me.Label58.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label58.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label58.Location = New System.Drawing.Point(392, 16)
        Me.Label58.Name = "Label58"
        Me.Label58.Size = New System.Drawing.Size(101, 23)
        Me.Label58.TabIndex = 4
        Me.Label58.Text = "仕入先"
        Me.Label58.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LabelMode
        '
        Me.LabelMode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LabelMode.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.LabelMode.Location = New System.Drawing.Point(12, 12)
        Me.LabelMode.Name = "LabelMode"
        Me.LabelMode.Size = New System.Drawing.Size(101, 30)
        Me.LabelMode.TabIndex = 58
        Me.LabelMode.Text = "モード"
        Me.LabelMode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'ButtonClose
        '
        Me.ButtonClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonClose.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ButtonClose.CausesValidation = False
        Me.ButtonClose.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonClose.Image = Global.DeliPlanner.My.Resources.Resources.cross
        Me.ButtonClose.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ButtonClose.Location = New System.Drawing.Point(1236, 829)
        Me.ButtonClose.Name = "ButtonClose"
        Me.ButtonClose.Size = New System.Drawing.Size(120, 30)
        Me.ButtonClose.TabIndex = 60
        Me.ButtonClose.TabStop = False
        Me.ButtonClose.Text = "キャンセル"
        Me.ButtonClose.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonClose.UseVisualStyleBackColor = False
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
        Me.ButtonDelete.TabIndex = 59
        Me.ButtonDelete.TabStop = False
        Me.ButtonDelete.Text = "削除"
        Me.ButtonDelete.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonDelete.UseVisualStyleBackColor = False
        '
        'Label1
        '
        Me.Label1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption
        Me.Label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label1.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(133, 16)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(101, 23)
        Me.Label1.TabIndex = 61
        Me.Label1.Text = "入荷日"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TextBoxItemTypeId
        '
        Me.TextBoxItemTypeId.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextBoxItemTypeId.BackColor = System.Drawing.Color.White
        Me.TextBoxItemTypeId.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxItemTypeId.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBoxItemTypeId.Format = "#,###"
        Me.TextBoxItemTypeId.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.TextBoxItemTypeId.Location = New System.Drawing.Point(853, 16)
        Me.TextBoxItemTypeId.MaxByteLength = 7
        Me.TextBoxItemTypeId.MaxValue = "0"
        Me.TextBoxItemTypeId.MinValue = "-999999"
        Me.TextBoxItemTypeId.Name = "TextBoxItemTypeId"
        Me.TextBoxItemTypeId.NumericOnly = True
        Me.TextBoxItemTypeId.OldText = Nothing
        Me.TextBoxItemTypeId.Size = New System.Drawing.Size(29, 23)
        Me.TextBoxItemTypeId.TabIndex = 62
        Me.TextBoxItemTypeId.TabStop = False
        Me.TextBoxItemTypeId.Text = "0"
        Me.TextBoxItemTypeId.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TextBoxItemTypeId.Visible = False
        '
        'FormArrival
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CausesValidation = False
        Me.ClientSize = New System.Drawing.Size(1368, 871)
        Me.Name = "FormArrival"
        Me.Text = "FormArrivals"
        Me.PanelMain.ResumeLayout(False)
        Me.PanelMain.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents PanelArrivals As Panel
    Friend WithEvents ButtonRegist As Button
    Friend WithEvents Button2 As Button
    Friend WithEvents ButtonClearSupplier As Button
    Friend WithEvents ButtonSearchSupplier As Button
    Friend WithEvents TextBoxSupplierName As TOA.ToaTextBox
    Friend WithEvents ButtonArrivalDatePrevious As Button
    Friend WithEvents ButtonArrivalDateNext As Button
    Friend WithEvents DateBoxArrivalDate As TOA.ToaDateBox
    Friend WithEvents TextBoxSupplierId As TOA.ToaTextBox
    Friend WithEvents Label58 As Label
    Friend WithEvents LabelMode As Label
    Friend WithEvents ButtonClose As Button
    Friend WithEvents ButtonDelete As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents TextBoxItemTypeId As TOA.ToaTextBox
End Class
