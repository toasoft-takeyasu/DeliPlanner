<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormEditOperator
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormEditOperator))
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TextBoxOperatorName = New TOA.ToaTextBox()
        Me.CheckBoxUnusedFlag = New System.Windows.Forms.CheckBox()
        Me.ButtonClose = New System.Windows.Forms.Button()
        Me.ButtonRegist = New System.Windows.Forms.Button()
        Me.ButtonDelete = New System.Windows.Forms.Button()
        Me.LabelMode = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TextBoxPassword = New TOA.ToaTextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.TextBoxOrderNo = New TOA.ToaTextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.ComboBoxRole = New System.Windows.Forms.ComboBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.TextBoxOperatorCode = New TOA.ToaTextBox()
        Me.ButtonResetPassword = New System.Windows.Forms.Button()
        Me.PanelMain.SuspendLayout()
        Me.SuspendLayout()
        '
        'PanelMain
        '
        Me.PanelMain.BackColor = System.Drawing.SystemColors.Control
        Me.PanelMain.Controls.Add(Me.ButtonResetPassword)
        Me.PanelMain.Controls.Add(Me.Label3)
        Me.PanelMain.Controls.Add(Me.TextBoxPassword)
        Me.PanelMain.Controls.Add(Me.Label1)
        Me.PanelMain.Controls.Add(Me.TextBoxOperatorCode)
        Me.PanelMain.Controls.Add(Me.Label4)
        Me.PanelMain.Controls.Add(Me.ComboBoxRole)
        Me.PanelMain.Controls.Add(Me.Label5)
        Me.PanelMain.Controls.Add(Me.TextBoxOrderNo)
        Me.PanelMain.Controls.Add(Me.LabelMode)
        Me.PanelMain.Controls.Add(Me.ButtonDelete)
        Me.PanelMain.Controls.Add(Me.ButtonRegist)
        Me.PanelMain.Controls.Add(Me.ButtonClose)
        Me.PanelMain.Controls.Add(Me.CheckBoxUnusedFlag)
        Me.PanelMain.Controls.Add(Me.Label2)
        Me.PanelMain.Controls.Add(Me.TextBoxOperatorName)
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
        Me.Label2.Text = "担当者名"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TextBoxOperatorName
        '
        Me.TextBoxOperatorName.BackColor = System.Drawing.SystemColors.Window
        Me.TextBoxOperatorName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxOperatorName.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBoxOperatorName.ImeMode = System.Windows.Forms.ImeMode.Hiragana
        Me.TextBoxOperatorName.Location = New System.Drawing.Point(111, 77)
        Me.TextBoxOperatorName.MaxByteLength = 40
        Me.TextBoxOperatorName.Name = "TextBoxOperatorName"
        Me.TextBoxOperatorName.OldText = "１２３４５６７８９０１２３４５６７８９０"
        Me.TextBoxOperatorName.Size = New System.Drawing.Size(332, 23)
        Me.TextBoxOperatorName.TabIndex = 4
        Me.TextBoxOperatorName.Text = "１２３４５６７８９０１２３４５６７８９０"
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
        Me.Label1.Location = New System.Drawing.Point(12, 135)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(100, 23)
        Me.Label1.TabIndex = 8
        Me.Label1.Text = "権限"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TextBoxPassword
        '
        Me.TextBoxPassword.BackColor = System.Drawing.SystemColors.Window
        Me.TextBoxPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxPassword.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBoxPassword.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.TextBoxPassword.Location = New System.Drawing.Point(111, 106)
        Me.TextBoxPassword.MaxByteLength = 40
        Me.TextBoxPassword.Name = "TextBoxPassword"
        Me.TextBoxPassword.OldText = "１２３４５６７８９０１２３４５６７８９０"
        Me.TextBoxPassword.Size = New System.Drawing.Size(170, 23)
        Me.TextBoxPassword.TabIndex = 5
        Me.TextBoxPassword.Text = "12345678901234567890"
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
        Me.TextBoxOrderNo.Text = "##0"
        Me.TextBoxOrderNo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label3
        '
        Me.Label3.BackColor = System.Drawing.SystemColors.GradientInactiveCaption
        Me.Label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label3.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label3.Location = New System.Drawing.Point(12, 106)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(100, 23)
        Me.Label3.TabIndex = 6
        Me.Label3.Text = "パスワード"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'ComboBoxRole
        '
        Me.ComboBoxRole.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ComboBoxRole.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBoxRole.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ComboBoxRole.FormattingEnabled = True
        Me.ComboBoxRole.Location = New System.Drawing.Point(111, 135)
        Me.ComboBoxRole.Name = "ComboBoxRole"
        Me.ComboBoxRole.Size = New System.Drawing.Size(90, 23)
        Me.ComboBoxRole.TabIndex = 9
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
        Me.Label4.Text = "担当者コード"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TextBoxOperatorCode
        '
        Me.TextBoxOperatorCode.BackColor = System.Drawing.SystemColors.Window
        Me.TextBoxOperatorCode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxOperatorCode.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBoxOperatorCode.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.TextBoxOperatorCode.Location = New System.Drawing.Point(111, 48)
        Me.TextBoxOperatorCode.MaxByteLength = 40
        Me.TextBoxOperatorCode.Name = "TextBoxOperatorCode"
        Me.TextBoxOperatorCode.OldText = "１２３４５６７８９０１２３４５６７８９０"
        Me.TextBoxOperatorCode.Size = New System.Drawing.Size(90, 23)
        Me.TextBoxOperatorCode.TabIndex = 2
        Me.TextBoxOperatorCode.Text = "1234567890"
        '
        'ButtonResetPassword
        '
        Me.ButtonResetPassword.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ButtonResetPassword.Font = New System.Drawing.Font("MS UI Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonResetPassword.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ButtonResetPassword.Location = New System.Drawing.Point(113, 105)
        Me.ButtonResetPassword.Name = "ButtonResetPassword"
        Me.ButtonResetPassword.Size = New System.Drawing.Size(120, 25)
        Me.ButtonResetPassword.TabIndex = 7
        Me.ButtonResetPassword.TabStop = False
        Me.ButtonResetPassword.Text = "パスワードリセット"
        Me.ButtonResetPassword.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonResetPassword.UseVisualStyleBackColor = False
        '
        'FormEditOperator
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CausesValidation = False
        Me.ClientSize = New System.Drawing.Size(968, 691)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "FormEditOperator"
        Me.Text = "担当者登録"
        Me.PanelMain.ResumeLayout(False)
        Me.PanelMain.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Label2 As Label
    Friend WithEvents TextBoxOperatorName As TOA.ToaTextBox
    Friend WithEvents CheckBoxUnusedFlag As CheckBox
    Friend WithEvents ButtonClose As Button
    Friend WithEvents ButtonRegist As Button
    Friend WithEvents ButtonDelete As Button
    Friend WithEvents LabelMode As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents TextBoxPassword As TOA.ToaTextBox
    Friend WithEvents Label5 As Label
    Friend WithEvents TextBoxOrderNo As TOA.ToaTextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents ComboBoxRole As ComboBox
    Friend WithEvents TextBoxOperatorCode As TOA.ToaTextBox
    Friend WithEvents Label4 As Label
    Friend WithEvents ButtonResetPassword As Button
End Class
