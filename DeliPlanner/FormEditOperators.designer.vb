<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormEditOperators
    Inherits DeliPlanner.FormBase

    'Form は、コンポーネント一覧に後処理を実行するために dispose をオーバーライドします。
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Windows フォーム デザイナで必要です。
    Private components As System.ComponentModel.IContainer

    'メモ: 以下のプロシージャは Windows フォーム デザイナで必要です。
    'Windows フォーム デザイナを使用して変更できます。  
    'コード エディタを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.PanelDetail = New System.Windows.Forms.Panel()
        Me.TextBoxFilterOperatorCode = New TOA.ToaTextBox()
        Me.TextBoxFilterOperatorName = New TOA.ToaTextBox()
        Me.PanelOperators = New System.Windows.Forms.Panel()
        Me.PanelMain.SuspendLayout()
        Me.PanelDetail.SuspendLayout()
        Me.SuspendLayout()
        '
        'PanelMain
        '
        Me.PanelMain.Controls.Add(Me.PanelDetail)
        Me.PanelMain.Size = New System.Drawing.Size(1368, 871)
        '
        'PanelDetail
        '
        Me.PanelDetail.BackColor = System.Drawing.SystemColors.Control
        Me.PanelDetail.Controls.Add(Me.TextBoxFilterOperatorCode)
        Me.PanelDetail.Controls.Add(Me.TextBoxFilterOperatorName)
        Me.PanelDetail.Controls.Add(Me.PanelOperators)
        Me.PanelDetail.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelDetail.Location = New System.Drawing.Point(0, 0)
        Me.PanelDetail.Name = "PanelDetail"
        Me.PanelDetail.Padding = New System.Windows.Forms.Padding(10, 0, 10, 0)
        Me.PanelDetail.Size = New System.Drawing.Size(1368, 871)
        Me.PanelDetail.TabIndex = 12
        '
        'TextBoxFilterOperatorCode
        '
        Me.TextBoxFilterOperatorCode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxFilterOperatorCode.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBoxFilterOperatorCode.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.TextBoxFilterOperatorCode.Location = New System.Drawing.Point(63, 30)
        Me.TextBoxFilterOperatorCode.MaxByteLength = 40
        Me.TextBoxFilterOperatorCode.Name = "TextBoxFilterOperatorCode"
        Me.TextBoxFilterOperatorCode.OldText = Nothing
        Me.TextBoxFilterOperatorCode.Size = New System.Drawing.Size(110, 22)
        Me.TextBoxFilterOperatorCode.TabIndex = 0
        Me.TextBoxFilterOperatorCode.Text = "12345"
        '
        'TextBoxFilterOperatorName
        '
        Me.TextBoxFilterOperatorName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxFilterOperatorName.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBoxFilterOperatorName.ImeMode = System.Windows.Forms.ImeMode.Hiragana
        Me.TextBoxFilterOperatorName.Location = New System.Drawing.Point(172, 30)
        Me.TextBoxFilterOperatorName.MaxByteLength = 40
        Me.TextBoxFilterOperatorName.Name = "TextBoxFilterOperatorName"
        Me.TextBoxFilterOperatorName.OldText = Nothing
        Me.TextBoxFilterOperatorName.Size = New System.Drawing.Size(220, 22)
        Me.TextBoxFilterOperatorName.TabIndex = 1
        Me.TextBoxFilterOperatorName.Text = "１２３４５６７８９０１２３４５６７８９０"
        '
        'PanelOperators
        '
        Me.PanelOperators.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PanelOperators.Location = New System.Drawing.Point(12, 56)
        Me.PanelOperators.Name = "PanelOperators"
        Me.PanelOperators.Size = New System.Drawing.Size(1344, 803)
        Me.PanelOperators.TabIndex = 2
        '
        'FormEditOperators
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CausesValidation = False
        Me.ClientSize = New System.Drawing.Size(1368, 871)
        Me.Name = "FormEditOperators"
        Me.PanelMain.ResumeLayout(False)
        Me.PanelDetail.ResumeLayout(False)
        Me.PanelDetail.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents PanelDetail As System.Windows.Forms.Panel
    Friend WithEvents TextBoxFilterOperatorName As TOA.ToaTextBox
    Friend WithEvents PanelOperators As Panel
    Friend WithEvents TextBoxFilterOperatorCode As TOA.ToaTextBox
End Class
