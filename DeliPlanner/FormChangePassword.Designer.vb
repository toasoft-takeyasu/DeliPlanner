<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormChangePassword
    Inherits TOA.ToaForm

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormChangePassword))
        Me.TextBoxCurrentPassword = New System.Windows.Forms.MaskedTextBox()
        Me.TextBoxNewPassword = New System.Windows.Forms.MaskedTextBox()
        Me.TextBoxConfirmPassword = New System.Windows.Forms.MaskedTextBox()
        Me.ButtonClose = New System.Windows.Forms.Button()
        Me.ButtonRegist = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'TextBoxCurrentPassword
        '
        Me.TextBoxCurrentPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxCurrentPassword.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBoxCurrentPassword.Location = New System.Drawing.Point(186, 28)
        Me.TextBoxCurrentPassword.Name = "TextBoxCurrentPassword"
        Me.TextBoxCurrentPassword.PasswordChar = Global.Microsoft.VisualBasic.ChrW(9679)
        Me.TextBoxCurrentPassword.Size = New System.Drawing.Size(170, 23)
        Me.TextBoxCurrentPassword.TabIndex = 4
        '
        'TextBoxNewPassword
        '
        Me.TextBoxNewPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxNewPassword.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBoxNewPassword.Location = New System.Drawing.Point(186, 63)
        Me.TextBoxNewPassword.Name = "TextBoxNewPassword"
        Me.TextBoxNewPassword.PasswordChar = Global.Microsoft.VisualBasic.ChrW(9679)
        Me.TextBoxNewPassword.Size = New System.Drawing.Size(170, 23)
        Me.TextBoxNewPassword.TabIndex = 5
        '
        'TextBoxConfirmPassword
        '
        Me.TextBoxConfirmPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxConfirmPassword.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBoxConfirmPassword.Location = New System.Drawing.Point(186, 100)
        Me.TextBoxConfirmPassword.Name = "TextBoxConfirmPassword"
        Me.TextBoxConfirmPassword.PasswordChar = Global.Microsoft.VisualBasic.ChrW(9679)
        Me.TextBoxConfirmPassword.Size = New System.Drawing.Size(170, 23)
        Me.TextBoxConfirmPassword.TabIndex = 6
        '
        'ButtonClose
        '
        Me.ButtonClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonClose.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonClose.Image = Global.DeliPlanner.My.Resources.Resources.cross
        Me.ButtonClose.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ButtonClose.Location = New System.Drawing.Point(237, 150)
        Me.ButtonClose.Name = "ButtonClose"
        Me.ButtonClose.Size = New System.Drawing.Size(120, 30)
        Me.ButtonClose.TabIndex = 8
        Me.ButtonClose.Text = "キャンセル"
        Me.ButtonClose.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonClose.UseVisualStyleBackColor = True
        '
        'ButtonRegist
        '
        Me.ButtonRegist.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonRegist.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonRegist.Image = Global.DeliPlanner.My.Resources.Resources.save
        Me.ButtonRegist.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ButtonRegist.Location = New System.Drawing.Point(111, 150)
        Me.ButtonRegist.Name = "ButtonRegist"
        Me.ButtonRegist.Size = New System.Drawing.Size(120, 30)
        Me.ButtonRegist.TabIndex = 7
        Me.ButtonRegist.Text = "登録"
        Me.ButtonRegist.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonRegist.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(25, 26)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(160, 22)
        Me.Label2.TabIndex = 17
        Me.Label2.Text = "現在のパスワード"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label3
        '
        Me.Label3.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label3.Location = New System.Drawing.Point(25, 63)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(160, 22)
        Me.Label3.TabIndex = 18
        Me.Label3.Text = "新しいパスワード"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label5
        '
        Me.Label5.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label5.Location = New System.Drawing.Point(25, 100)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(160, 22)
        Me.Label5.TabIndex = 19
        Me.Label5.Text = "新しいパスワード（確認）"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'FormChangePassword
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(378, 201)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.ButtonClose)
        Me.Controls.Add(Me.ButtonRegist)
        Me.Controls.Add(Me.TextBoxConfirmPassword)
        Me.Controls.Add(Me.TextBoxNewPassword)
        Me.Controls.Add(Me.TextBoxCurrentPassword)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "FormChangePassword"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "パスワード変更"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TextBoxCurrentPassword As MaskedTextBox
    Friend WithEvents TextBoxNewPassword As MaskedTextBox
    Friend WithEvents TextBoxConfirmPassword As MaskedTextBox
    Friend WithEvents ButtonClose As Button
    Friend WithEvents ButtonRegist As Button
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Label5 As Label
End Class
