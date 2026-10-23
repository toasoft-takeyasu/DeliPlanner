<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class DialogPlan
    Inherits TOA.ToaDialog

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(DialogPlan))
        Me.TextBoxItemName = New TOA.ToaTextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.ComboBoxUnitType = New System.Windows.Forms.ComboBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TextBoxQuantity = New TOA.ToaTextBox()
        Me.DateBoxPlanDate = New TOA.ToaDateBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.ButtonRegist = New System.Windows.Forms.Button()
        Me.ButtonClose = New System.Windows.Forms.Button()
        Me.TextBoxItemId = New TOA.ToaTextBox()
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
        Me.Label1.Location = New System.Drawing.Point(12, 67)
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
        Me.ComboBoxUnitType.Location = New System.Drawing.Point(171, 67)
        Me.ComboBoxUnitType.Name = "ComboBoxUnitType"
        Me.ComboBoxUnitType.Size = New System.Drawing.Size(90, 23)
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
        Me.TextBoxQuantity.Location = New System.Drawing.Point(112, 67)
        Me.TextBoxQuantity.MaxByteLength = 7
        Me.TextBoxQuantity.MaxValue = "0"
        Me.TextBoxQuantity.MinValue = "-999999"
        Me.TextBoxQuantity.Name = "TextBoxQuantity"
        Me.TextBoxQuantity.NumericOnly = True
        Me.TextBoxQuantity.OldText = Nothing
        Me.TextBoxQuantity.Size = New System.Drawing.Size(60, 23)
        Me.TextBoxQuantity.TabIndex = 6
        Me.TextBoxQuantity.Text = "1,000"
        Me.TextBoxQuantity.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'DateBoxPlanDate
        '
        Me.DateBoxPlanDate.AcceptsTab = True
        Me.DateBoxPlanDate.BackColor = System.Drawing.Color.White
        Me.DateBoxPlanDate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.DateBoxPlanDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.DateBoxPlanDate.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.DateBoxPlanDate.Location = New System.Drawing.Point(112, 38)
        Me.DateBoxPlanDate.MaxLength = 10
        Me.DateBoxPlanDate.Name = "DateBoxPlanDate"
        Me.DateBoxPlanDate.OldText = Nothing
        Me.DateBoxPlanDate.Size = New System.Drawing.Size(104, 23)
        Me.DateBoxPlanDate.TabIndex = 4
        Me.DateBoxPlanDate.Text = "yyyy/MM/dd"
        Me.DateBoxPlanDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
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
        Me.Label3.Text = "製造予定日"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'ButtonRegist
        '
        Me.ButtonRegist.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonRegist.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ButtonRegist.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonRegist.Image = Global.DeliPlanner.My.Resources.Resources.save
        Me.ButtonRegist.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ButtonRegist.Location = New System.Drawing.Point(384, 93)
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
        Me.ButtonClose.Location = New System.Drawing.Point(510, 93)
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
        Me.TextBoxItemId.Text = "0"
        Me.TextBoxItemId.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TextBoxItemId.Visible = False
        '
        'DialogPlan
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.ButtonClose
        Me.ClientSize = New System.Drawing.Size(642, 132)
        Me.Controls.Add(Me.TextBoxItemId)
        Me.Controls.Add(Me.ButtonRegist)
        Me.Controls.Add(Me.ButtonClose)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.DateBoxPlanDate)
        Me.Controls.Add(Me.TextBoxItemName)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.ComboBoxUnitType)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.TextBoxQuantity)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "DialogPlan"
        Me.Text = "製造計画"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TextBoxItemName As TOA.ToaTextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents ComboBoxUnitType As ComboBox
    Friend WithEvents Label2 As Label
    Friend WithEvents TextBoxQuantity As TOA.ToaTextBox
    Friend WithEvents DateBoxPlanDate As TOA.ToaDateBox
    Friend WithEvents Label3 As Label
    Friend WithEvents ButtonRegist As Button
    Friend WithEvents ButtonClose As Button
    Friend WithEvents TextBoxItemId As TOA.ToaTextBox
End Class
