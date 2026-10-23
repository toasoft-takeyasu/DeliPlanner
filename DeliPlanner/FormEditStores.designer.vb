<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormEditStores
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
        Me.TextBoxFilterStoreName = New TOA.ToaTextBox()
        Me.PanelStores = New System.Windows.Forms.Panel()
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
        Me.PanelDetail.Controls.Add(Me.TextBoxFilterStoreName)
        Me.PanelDetail.Controls.Add(Me.PanelStores)
        Me.PanelDetail.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelDetail.Location = New System.Drawing.Point(0, 0)
        Me.PanelDetail.Name = "PanelDetail"
        Me.PanelDetail.Padding = New System.Windows.Forms.Padding(10, 0, 10, 0)
        Me.PanelDetail.Size = New System.Drawing.Size(1368, 871)
        Me.PanelDetail.TabIndex = 12
        '
        'TextBoxFilterStoreName
        '
        Me.TextBoxFilterStoreName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxFilterStoreName.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBoxFilterStoreName.ImeMode = System.Windows.Forms.ImeMode.Hiragana
        Me.TextBoxFilterStoreName.Location = New System.Drawing.Point(63, 30)
        Me.TextBoxFilterStoreName.MaxByteLength = 40
        Me.TextBoxFilterStoreName.Name = "TextBoxFilterStoreName"
        Me.TextBoxFilterStoreName.OldText = Nothing
        Me.TextBoxFilterStoreName.Size = New System.Drawing.Size(331, 22)
        Me.TextBoxFilterStoreName.TabIndex = 36
        Me.TextBoxFilterStoreName.Text = "１２３４５６７８９０１２３４５６７８９０"
        '
        'PanelStores
        '
        Me.PanelStores.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PanelStores.Location = New System.Drawing.Point(12, 56)
        Me.PanelStores.Name = "PanelStores"
        Me.PanelStores.Size = New System.Drawing.Size(1344, 803)
        Me.PanelStores.TabIndex = 37
        '
        'FormEditStores
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CausesValidation = False
        Me.ClientSize = New System.Drawing.Size(1368, 871)
        Me.Name = "FormEditStores"
        Me.PanelMain.ResumeLayout(False)
        Me.PanelDetail.ResumeLayout(False)
        Me.PanelDetail.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents PanelDetail As System.Windows.Forms.Panel
    Friend WithEvents TextBoxFilterStoreName As TOA.ToaTextBox
    Friend WithEvents PanelStores As Panel
End Class
