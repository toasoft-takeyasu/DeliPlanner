<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormEditStore
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
        Me.TextBoxStoreName = New TOA.ToaTextBox()
        Me.CheckBoxUnusedFlag = New System.Windows.Forms.CheckBox()
        Me.ButtonClose = New System.Windows.Forms.Button()
        Me.ButtonRegist = New System.Windows.Forms.Button()
        Me.ButtonDelete = New System.Windows.Forms.Button()
        Me.LabelMode = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TextBoxStoreShortName = New TOA.ToaTextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.TextBoxOrderNo = New TOA.ToaTextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.CheckBoxCenterFlag = New System.Windows.Forms.CheckBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.TextBoxStoreCode = New TOA.ToaTextBox()
        Me.PanelMain.SuspendLayout()
        Me.SuspendLayout()
        '
        'PanelMain
        '
        Me.PanelMain.BackColor = System.Drawing.SystemColors.Control
        Me.PanelMain.Controls.Add(Me.Label4)
        Me.PanelMain.Controls.Add(Me.TextBoxStoreCode)
        Me.PanelMain.Controls.Add(Me.CheckBoxCenterFlag)
        Me.PanelMain.Controls.Add(Me.Label3)
        Me.PanelMain.Controls.Add(Me.Label5)
        Me.PanelMain.Controls.Add(Me.TextBoxOrderNo)
        Me.PanelMain.Controls.Add(Me.Label1)
        Me.PanelMain.Controls.Add(Me.TextBoxStoreShortName)
        Me.PanelMain.Controls.Add(Me.LabelMode)
        Me.PanelMain.Controls.Add(Me.ButtonDelete)
        Me.PanelMain.Controls.Add(Me.ButtonRegist)
        Me.PanelMain.Controls.Add(Me.ButtonClose)
        Me.PanelMain.Controls.Add(Me.CheckBoxUnusedFlag)
        Me.PanelMain.Controls.Add(Me.Label2)
        Me.PanelMain.Controls.Add(Me.TextBoxStoreName)
        Me.PanelMain.Size = New System.Drawing.Size(968, 691)
        Me.PanelMain.TabIndex = 0
        '
        'Label2
        '
        Me.Label2.BackColor = System.Drawing.SystemColors.GradientInactiveCaption
        Me.Label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label2.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(12, 77)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(100, 23)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "店舗名"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TextBoxStoreName
        '
        Me.TextBoxStoreName.BackColor = System.Drawing.SystemColors.Window
        Me.TextBoxStoreName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxStoreName.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBoxStoreName.ImeMode = System.Windows.Forms.ImeMode.Hiragana
        Me.TextBoxStoreName.Location = New System.Drawing.Point(111, 77)
        Me.TextBoxStoreName.MaxByteLength = 40
        Me.TextBoxStoreName.Name = "TextBoxStoreName"
        Me.TextBoxStoreName.OldText = "１２３４５６７８９０１２３４５６７８９０"
        Me.TextBoxStoreName.Size = New System.Drawing.Size(332, 23)
        Me.TextBoxStoreName.TabIndex = 4
        Me.TextBoxStoreName.Text = "１２３４５６７８９０１２３４５６７８９０"
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
        Me.ButtonClose.TabIndex = 12
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
        Me.ButtonRegist.TabIndex = 10
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
        Me.ButtonDelete.TabIndex = 11
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
        Me.Label1.Text = "略称"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TextBoxStoreShortName
        '
        Me.TextBoxStoreShortName.BackColor = System.Drawing.SystemColors.Window
        Me.TextBoxStoreShortName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxStoreShortName.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBoxStoreShortName.ImeMode = System.Windows.Forms.ImeMode.Hiragana
        Me.TextBoxStoreShortName.Location = New System.Drawing.Point(111, 106)
        Me.TextBoxStoreShortName.MaxByteLength = 40
        Me.TextBoxStoreShortName.Name = "TextBoxStoreShortName"
        Me.TextBoxStoreShortName.OldText = "１２３４５６７８９０１２３４５６７８９０"
        Me.TextBoxStoreShortName.Size = New System.Drawing.Size(67, 23)
        Me.TextBoxStoreShortName.TabIndex = 7
        Me.TextBoxStoreShortName.Text = "１２３"
        '
        'Label5
        '
        Me.Label5.BackColor = System.Drawing.SystemColors.GradientInactiveCaption
        Me.Label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label5.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label5.Location = New System.Drawing.Point(12, 164)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(100, 23)
        Me.Label5.TabIndex = 10
        Me.Label5.Text = "並び順"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TextBoxOrderNo
        '
        Me.TextBoxOrderNo.BackColor = System.Drawing.Color.White
        Me.TextBoxOrderNo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxOrderNo.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBoxOrderNo.Format = "##0"
        Me.TextBoxOrderNo.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.TextBoxOrderNo.Location = New System.Drawing.Point(111, 164)
        Me.TextBoxOrderNo.MaxByteLength = 7
        Me.TextBoxOrderNo.MaxValue = "0"
        Me.TextBoxOrderNo.MinValue = "-999999"
        Me.TextBoxOrderNo.Name = "TextBoxOrderNo"
        Me.TextBoxOrderNo.NumericOnly = True
        Me.TextBoxOrderNo.OldText = Nothing
        Me.TextBoxOrderNo.Size = New System.Drawing.Size(37, 23)
        Me.TextBoxOrderNo.TabIndex = 11
        Me.TextBoxOrderNo.TabStop = False
        Me.TextBoxOrderNo.Text = "##0"
        Me.TextBoxOrderNo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
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
        Me.Label3.Text = "センター"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'CheckBoxCenterFlag
        '
        Me.CheckBoxCenterFlag.AutoSize = True
        Me.CheckBoxCenterFlag.Font = New System.Drawing.Font("MS UI Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.CheckBoxCenterFlag.Location = New System.Drawing.Point(118, 138)
        Me.CheckBoxCenterFlag.Name = "CheckBoxCenterFlag"
        Me.CheckBoxCenterFlag.Size = New System.Drawing.Size(15, 14)
        Me.CheckBoxCenterFlag.TabIndex = 9
        Me.CheckBoxCenterFlag.TabStop = False
        Me.CheckBoxCenterFlag.UseVisualStyleBackColor = True
        '
        'Label4
        '
        Me.Label4.BackColor = System.Drawing.SystemColors.GradientInactiveCaption
        Me.Label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label4.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label4.Location = New System.Drawing.Point(12, 48)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(100, 23)
        Me.Label4.TabIndex = 1
        Me.Label4.Text = "店舗コード"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TextBoxStoreCode
        '
        Me.TextBoxStoreCode.BackColor = System.Drawing.Color.White
        Me.TextBoxStoreCode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxStoreCode.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBoxStoreCode.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.TextBoxStoreCode.Location = New System.Drawing.Point(111, 48)
        Me.TextBoxStoreCode.MaxByteLength = 10
        Me.TextBoxStoreCode.Name = "TextBoxStoreCode"
        Me.TextBoxStoreCode.OldText = Nothing
        Me.TextBoxStoreCode.Size = New System.Drawing.Size(90, 23)
        Me.TextBoxStoreCode.TabIndex = 2
        Me.TextBoxStoreCode.Text = "0000000000"
        '
        'FormEditStore
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CausesValidation = False
        Me.ClientSize = New System.Drawing.Size(968, 691)
        Me.Name = "FormEditStore"
        Me.Text = "店舗登録"
        Me.PanelMain.ResumeLayout(False)
        Me.PanelMain.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Label2 As Label
    Friend WithEvents TextBoxStoreName As TOA.ToaTextBox
    Friend WithEvents CheckBoxUnusedFlag As CheckBox
    Friend WithEvents ButtonClose As Button
    Friend WithEvents ButtonRegist As Button
    Friend WithEvents ButtonDelete As Button
    Friend WithEvents LabelMode As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents TextBoxStoreShortName As TOA.ToaTextBox
    Friend WithEvents Label5 As Label
    Friend WithEvents TextBoxOrderNo As TOA.ToaTextBox
    Friend WithEvents CheckBoxCenterFlag As CheckBox
    Friend WithEvents Label3 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents TextBoxStoreCode As ToaTextBox
End Class
