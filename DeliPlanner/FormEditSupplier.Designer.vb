<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormEditSupplier
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TextBoxSupplierName = New TOA.ToaTextBox()
        Me.CheckBoxUnusedFlag = New System.Windows.Forms.CheckBox()
        Me.ButtonClose = New System.Windows.Forms.Button()
        Me.ButtonRegist = New System.Windows.Forms.Button()
        Me.ButtonDelete = New System.Windows.Forms.Button()
        Me.LabelMode = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.TextBoxPostNo = New TOA.ToaTextBox()
        Me.TextBoxAddress1 = New TOA.ToaTextBox()
        Me.TextBoxAddress2 = New TOA.ToaTextBox()
        Me.TextBoxAddress3 = New TOA.ToaTextBox()
        Me.TextBoxTelNo = New TOA.ToaTextBox()
        Me.TextBoxFaxNo = New TOA.ToaTextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label58 = New System.Windows.Forms.Label()
        Me.ComboBoxItemType = New System.Windows.Forms.ComboBox()
        Me.Label45 = New System.Windows.Forms.Label()
        Me.TextBoxSupplierCode = New TOA.ToaTextBox()
        Me.PanelMain.SuspendLayout()
        Me.SuspendLayout()
        '
        'PanelMain
        '
        Me.PanelMain.BackColor = System.Drawing.SystemColors.Control
        Me.PanelMain.Controls.Add(Me.Label45)
        Me.PanelMain.Controls.Add(Me.Label2)
        Me.PanelMain.Controls.Add(Me.TextBoxSupplierCode)
        Me.PanelMain.Controls.Add(Me.Label58)
        Me.PanelMain.Controls.Add(Me.ComboBoxItemType)
        Me.PanelMain.Controls.Add(Me.Label4)
        Me.PanelMain.Controls.Add(Me.Label6)
        Me.PanelMain.Controls.Add(Me.TextBoxFaxNo)
        Me.PanelMain.Controls.Add(Me.TextBoxTelNo)
        Me.PanelMain.Controls.Add(Me.TextBoxAddress3)
        Me.PanelMain.Controls.Add(Me.TextBoxAddress2)
        Me.PanelMain.Controls.Add(Me.TextBoxAddress1)
        Me.PanelMain.Controls.Add(Me.TextBoxPostNo)
        Me.PanelMain.Controls.Add(Me.Label3)
        Me.PanelMain.Controls.Add(Me.Label5)
        Me.PanelMain.Controls.Add(Me.Label1)
        Me.PanelMain.Controls.Add(Me.LabelMode)
        Me.PanelMain.Controls.Add(Me.ButtonDelete)
        Me.PanelMain.Controls.Add(Me.ButtonRegist)
        Me.PanelMain.Controls.Add(Me.ButtonClose)
        Me.PanelMain.Controls.Add(Me.CheckBoxUnusedFlag)
        Me.PanelMain.Controls.Add(Me.TextBoxSupplierName)
        Me.PanelMain.Size = New System.Drawing.Size(968, 691)
        Me.PanelMain.TabIndex = 0
        '
        'Label2
        '
        Me.Label2.BackColor = System.Drawing.SystemColors.GradientInactiveCaption
        Me.Label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label2.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(12, 48)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(100, 23)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "仕入先コード"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TextBoxSupplierName
        '
        Me.TextBoxSupplierName.BackColor = System.Drawing.SystemColors.Window
        Me.TextBoxSupplierName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxSupplierName.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBoxSupplierName.ImeMode = System.Windows.Forms.ImeMode.Hiragana
        Me.TextBoxSupplierName.Location = New System.Drawing.Point(111, 77)
        Me.TextBoxSupplierName.MaxByteLength = 40
        Me.TextBoxSupplierName.Name = "TextBoxSupplierName"
        Me.TextBoxSupplierName.OldText = "１２３４５６７８９０１２３４５６７８９０"
        Me.TextBoxSupplierName.Size = New System.Drawing.Size(332, 23)
        Me.TextBoxSupplierName.TabIndex = 4
        Me.TextBoxSupplierName.Text = "１２３４５６７８９０１２３４５６７８９０"
        '
        'CheckBoxUnusedFlag
        '
        Me.CheckBoxUnusedFlag.AutoSize = True
        Me.CheckBoxUnusedFlag.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.CheckBoxUnusedFlag.Location = New System.Drawing.Point(472, 80)
        Me.CheckBoxUnusedFlag.Name = "CheckBoxUnusedFlag"
        Me.CheckBoxUnusedFlag.Size = New System.Drawing.Size(75, 20)
        Me.CheckBoxUnusedFlag.TabIndex = 5
        Me.CheckBoxUnusedFlag.TabStop = False
        Me.CheckBoxUnusedFlag.Text = "未使用"
        Me.CheckBoxUnusedFlag.UseVisualStyleBackColor = True
        '
        'ButtonClose
        '
        Me.ButtonClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonClose.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ButtonClose.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonClose.Image = Global.DeliPlanner.My.Resources.Resources.cross
        Me.ButtonClose.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ButtonClose.Location = New System.Drawing.Point(836, 649)
        Me.ButtonClose.Name = "ButtonClose"
        Me.ButtonClose.Size = New System.Drawing.Size(120, 30)
        Me.ButtonClose.TabIndex = 19
        Me.ButtonClose.TabStop = False
        Me.ButtonClose.Text = "キャンセル"
        Me.ButtonClose.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonClose.UseVisualStyleBackColor = False
        '
        'ButtonRegist
        '
        Me.ButtonRegist.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonRegist.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ButtonRegist.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonRegist.Image = Global.DeliPlanner.My.Resources.Resources.save
        Me.ButtonRegist.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ButtonRegist.Location = New System.Drawing.Point(584, 649)
        Me.ButtonRegist.Name = "ButtonRegist"
        Me.ButtonRegist.Size = New System.Drawing.Size(120, 30)
        Me.ButtonRegist.TabIndex = 17
        Me.ButtonRegist.TabStop = False
        Me.ButtonRegist.Text = "登録"
        Me.ButtonRegist.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonRegist.UseVisualStyleBackColor = False
        '
        'ButtonDelete
        '
        Me.ButtonDelete.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonDelete.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ButtonDelete.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonDelete.Image = Global.DeliPlanner.My.Resources.Resources.trashcan
        Me.ButtonDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ButtonDelete.Location = New System.Drawing.Point(710, 649)
        Me.ButtonDelete.Name = "ButtonDelete"
        Me.ButtonDelete.Size = New System.Drawing.Size(120, 30)
        Me.ButtonDelete.TabIndex = 18
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
        Me.LabelMode.TabIndex = 0
        Me.LabelMode.Text = "モード"
        Me.LabelMode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label1
        '
        Me.Label1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption
        Me.Label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label1.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(12, 106)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(100, 23)
        Me.Label1.TabIndex = 6
        Me.Label1.Text = "郵便番号"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label5
        '
        Me.Label5.BackColor = System.Drawing.SystemColors.GradientInactiveCaption
        Me.Label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label5.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label5.Location = New System.Drawing.Point(12, 164)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(100, 23)
        Me.Label5.TabIndex = 11
        Me.Label5.Text = "建物名"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label3
        '
        Me.Label3.BackColor = System.Drawing.SystemColors.GradientInactiveCaption
        Me.Label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label3.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label3.Location = New System.Drawing.Point(12, 135)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(100, 23)
        Me.Label3.TabIndex = 8
        Me.Label3.Text = "住所"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TextBoxPostNo
        '
        Me.TextBoxPostNo.BackColor = System.Drawing.SystemColors.Window
        Me.TextBoxPostNo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxPostNo.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBoxPostNo.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.TextBoxPostNo.Location = New System.Drawing.Point(111, 106)
        Me.TextBoxPostNo.MaxByteLength = 40
        Me.TextBoxPostNo.Name = "TextBoxPostNo"
        Me.TextBoxPostNo.OldText = "１２３４５６７８９０１２３４５６７８９０"
        Me.TextBoxPostNo.Size = New System.Drawing.Size(67, 23)
        Me.TextBoxPostNo.TabIndex = 7
        Me.TextBoxPostNo.Text = "1234567"
        '
        'TextBoxAddress1
        '
        Me.TextBoxAddress1.BackColor = System.Drawing.SystemColors.Window
        Me.TextBoxAddress1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxAddress1.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBoxAddress1.ImeMode = System.Windows.Forms.ImeMode.Hiragana
        Me.TextBoxAddress1.Location = New System.Drawing.Point(111, 135)
        Me.TextBoxAddress1.MaxByteLength = 40
        Me.TextBoxAddress1.Name = "TextBoxAddress1"
        Me.TextBoxAddress1.OldText = "１２３４５６７８９０１２３４５６７８９０"
        Me.TextBoxAddress1.Size = New System.Drawing.Size(332, 23)
        Me.TextBoxAddress1.TabIndex = 9
        Me.TextBoxAddress1.Text = "１２３４５６７８９０１２３４５６７８９０"
        '
        'TextBoxAddress2
        '
        Me.TextBoxAddress2.BackColor = System.Drawing.SystemColors.Window
        Me.TextBoxAddress2.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBoxAddress2.ImeMode = System.Windows.Forms.ImeMode.Hiragana
        Me.TextBoxAddress2.Location = New System.Drawing.Point(442, 135)
        Me.TextBoxAddress2.MaxByteLength = 40
        Me.TextBoxAddress2.Name = "TextBoxAddress2"
        Me.TextBoxAddress2.OldText = "１２３４５６７８９０１２３４５６７８９０"
        Me.TextBoxAddress2.Size = New System.Drawing.Size(332, 23)
        Me.TextBoxAddress2.TabIndex = 10
        Me.TextBoxAddress2.Text = "１２３４５６７８９０１２３４５６７８９０"
        '
        'TextBoxAddress3
        '
        Me.TextBoxAddress3.BackColor = System.Drawing.SystemColors.Window
        Me.TextBoxAddress3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxAddress3.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBoxAddress3.ImeMode = System.Windows.Forms.ImeMode.Hiragana
        Me.TextBoxAddress3.Location = New System.Drawing.Point(111, 164)
        Me.TextBoxAddress3.MaxByteLength = 40
        Me.TextBoxAddress3.Name = "TextBoxAddress3"
        Me.TextBoxAddress3.OldText = "１２３４５６７８９０１２３４５６７８９０"
        Me.TextBoxAddress3.Size = New System.Drawing.Size(332, 23)
        Me.TextBoxAddress3.TabIndex = 12
        Me.TextBoxAddress3.Text = "１２３４５６７８９０１２３４５６７８９０"
        '
        'TextBoxTelNo
        '
        Me.TextBoxTelNo.BackColor = System.Drawing.SystemColors.Window
        Me.TextBoxTelNo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxTelNo.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBoxTelNo.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.TextBoxTelNo.Location = New System.Drawing.Point(111, 193)
        Me.TextBoxTelNo.MaxByteLength = 40
        Me.TextBoxTelNo.Name = "TextBoxTelNo"
        Me.TextBoxTelNo.OldText = "１２３４５６７８９０１２３４５６７８９０"
        Me.TextBoxTelNo.Size = New System.Drawing.Size(101, 23)
        Me.TextBoxTelNo.TabIndex = 14
        Me.TextBoxTelNo.Text = "12345678901"
        '
        'TextBoxFaxNo
        '
        Me.TextBoxFaxNo.BackColor = System.Drawing.SystemColors.Window
        Me.TextBoxFaxNo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxFaxNo.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBoxFaxNo.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.TextBoxFaxNo.Location = New System.Drawing.Point(111, 222)
        Me.TextBoxFaxNo.MaxByteLength = 40
        Me.TextBoxFaxNo.Name = "TextBoxFaxNo"
        Me.TextBoxFaxNo.OldText = "１２３４５６７８９０１２３４５６７８９０"
        Me.TextBoxFaxNo.Size = New System.Drawing.Size(101, 23)
        Me.TextBoxFaxNo.TabIndex = 16
        Me.TextBoxFaxNo.Text = "12345678901"
        '
        'Label4
        '
        Me.Label4.BackColor = System.Drawing.SystemColors.GradientInactiveCaption
        Me.Label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label4.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label4.Location = New System.Drawing.Point(12, 193)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(100, 23)
        Me.Label4.TabIndex = 13
        Me.Label4.Text = "電話番号"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label6
        '
        Me.Label6.BackColor = System.Drawing.SystemColors.GradientInactiveCaption
        Me.Label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label6.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label6.Location = New System.Drawing.Point(12, 222)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(100, 23)
        Me.Label6.TabIndex = 15
        Me.Label6.Text = "FAX番号"
        Me.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label58
        '
        Me.Label58.BackColor = System.Drawing.SystemColors.GradientInactiveCaption
        Me.Label58.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label58.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label58.Location = New System.Drawing.Point(12, 251)
        Me.Label58.Name = "Label58"
        Me.Label58.Size = New System.Drawing.Size(100, 23)
        Me.Label58.TabIndex = 17
        Me.Label58.Text = "品目区分"
        Me.Label58.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'ComboBoxItemType
        '
        Me.ComboBoxItemType.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ComboBoxItemType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBoxItemType.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ComboBoxItemType.FormattingEnabled = True
        Me.ComboBoxItemType.Location = New System.Drawing.Point(111, 251)
        Me.ComboBoxItemType.Name = "ComboBoxItemType"
        Me.ComboBoxItemType.Size = New System.Drawing.Size(101, 23)
        Me.ComboBoxItemType.TabIndex = 18
        '
        'Label45
        '
        Me.Label45.BackColor = System.Drawing.SystemColors.GradientInactiveCaption
        Me.Label45.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label45.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label45.Location = New System.Drawing.Point(11, 77)
        Me.Label45.Name = "Label45"
        Me.Label45.Size = New System.Drawing.Size(101, 23)
        Me.Label45.TabIndex = 3
        Me.Label45.Text = "仕入先名"
        Me.Label45.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TextBoxSupplierCode
        '
        Me.TextBoxSupplierCode.BackColor = System.Drawing.Color.White
        Me.TextBoxSupplierCode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxSupplierCode.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBoxSupplierCode.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.TextBoxSupplierCode.Location = New System.Drawing.Point(111, 48)
        Me.TextBoxSupplierCode.MaxByteLength = 10
        Me.TextBoxSupplierCode.Name = "TextBoxSupplierCode"
        Me.TextBoxSupplierCode.OldText = Nothing
        Me.TextBoxSupplierCode.Size = New System.Drawing.Size(90, 23)
        Me.TextBoxSupplierCode.TabIndex = 2
        Me.TextBoxSupplierCode.Text = "0000000000"
        '
        'FormEditSupplier
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CausesValidation = False
        Me.ClientSize = New System.Drawing.Size(968, 691)
        Me.Name = "FormEditSupplier"
        Me.Text = "仕入先登録"
        Me.PanelMain.ResumeLayout(False)
        Me.PanelMain.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Label2 As Label
    Friend WithEvents TextBoxSupplierName As TOA.ToaTextBox
    Friend WithEvents CheckBoxUnusedFlag As CheckBox
    Friend WithEvents ButtonClose As Button
    Friend WithEvents ButtonRegist As Button
    Friend WithEvents ButtonDelete As Button
    Friend WithEvents LabelMode As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents TextBoxFaxNo As TOA.ToaTextBox
    Friend WithEvents TextBoxTelNo As TOA.ToaTextBox
    Friend WithEvents TextBoxAddress3 As TOA.ToaTextBox
    Friend WithEvents TextBoxAddress2 As TOA.ToaTextBox
    Friend WithEvents TextBoxAddress1 As TOA.ToaTextBox
    Friend WithEvents TextBoxPostNo As TOA.ToaTextBox
    Friend WithEvents Label4 As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents Label58 As Label
    Friend WithEvents ComboBoxItemType As ComboBox
    Friend WithEvents Label45 As Label
    Friend WithEvents TextBoxSupplierCode As TOA.ToaTextBox
End Class
