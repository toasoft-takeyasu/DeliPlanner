<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormEditSuppliers
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
        Me.TextBoxFilterSupplierCode = New TOA.ToaTextBox()
        Me.TextBoxFilterSupplierName = New TOA.ToaTextBox()
        Me.ComboBoxFilerItemType = New System.Windows.Forms.ComboBox()
        Me.PanelSuppliers = New System.Windows.Forms.Panel()
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
        Me.PanelDetail.Controls.Add(Me.TextBoxFilterSupplierCode)
        Me.PanelDetail.Controls.Add(Me.TextBoxFilterSupplierName)
        Me.PanelDetail.Controls.Add(Me.ComboBoxFilerItemType)
        Me.PanelDetail.Controls.Add(Me.PanelSuppliers)
        Me.PanelDetail.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelDetail.Location = New System.Drawing.Point(0, 0)
        Me.PanelDetail.Name = "PanelDetail"
        Me.PanelDetail.Padding = New System.Windows.Forms.Padding(10, 0, 10, 0)
        Me.PanelDetail.Size = New System.Drawing.Size(1368, 871)
        Me.PanelDetail.TabIndex = 12
        '
        'TextBoxFilterSupplierCode
        '
        Me.TextBoxFilterSupplierCode.BackColor = System.Drawing.Color.White
        Me.TextBoxFilterSupplierCode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxFilterSupplierCode.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBoxFilterSupplierCode.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.TextBoxFilterSupplierCode.Location = New System.Drawing.Point(63, 30)
        Me.TextBoxFilterSupplierCode.MaxByteLength = 10
        Me.TextBoxFilterSupplierCode.Name = "TextBoxFilterSupplierCode"
        Me.TextBoxFilterSupplierCode.OldText = Nothing
        Me.TextBoxFilterSupplierCode.Size = New System.Drawing.Size(100, 22)
        Me.TextBoxFilterSupplierCode.TabIndex = 0
        Me.TextBoxFilterSupplierCode.Text = "0000000000"
        '
        'TextBoxFilterSupplierName
        '
        Me.TextBoxFilterSupplierName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxFilterSupplierName.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBoxFilterSupplierName.ImeMode = System.Windows.Forms.ImeMode.Hiragana
        Me.TextBoxFilterSupplierName.Location = New System.Drawing.Point(162, 30)
        Me.TextBoxFilterSupplierName.MaxByteLength = 40
        Me.TextBoxFilterSupplierName.Name = "TextBoxFilterSupplierName"
        Me.TextBoxFilterSupplierName.OldText = Nothing
        Me.TextBoxFilterSupplierName.Size = New System.Drawing.Size(331, 22)
        Me.TextBoxFilterSupplierName.TabIndex = 1
        Me.TextBoxFilterSupplierName.Text = "１２３４５６７８９０１２３４５６７８９０"
        '
        'ComboBoxFilerItemType
        '
        Me.ComboBoxFilerItemType.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ComboBoxFilerItemType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBoxFilerItemType.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ComboBoxFilerItemType.FormattingEnabled = True
        Me.ComboBoxFilerItemType.Location = New System.Drawing.Point(492, 29)
        Me.ComboBoxFilerItemType.Name = "ComboBoxFilerItemType"
        Me.ComboBoxFilerItemType.Size = New System.Drawing.Size(81, 23)
        Me.ComboBoxFilerItemType.TabIndex = 2
        '
        'PanelSuppliers
        '
        Me.PanelSuppliers.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PanelSuppliers.Location = New System.Drawing.Point(12, 56)
        Me.PanelSuppliers.Name = "PanelSuppliers"
        Me.PanelSuppliers.Size = New System.Drawing.Size(1344, 803)
        Me.PanelSuppliers.TabIndex = 3
        '
        'FormEditSuppliers
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CausesValidation = False
        Me.ClientSize = New System.Drawing.Size(1368, 871)
        Me.Name = "FormEditSuppliers"
        Me.PanelMain.ResumeLayout(False)
        Me.PanelDetail.ResumeLayout(False)
        Me.PanelDetail.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents PanelDetail As System.Windows.Forms.Panel
    Friend WithEvents TextBoxFilterSupplierName As TOA.ToaTextBox
    Friend WithEvents PanelSuppliers As Panel
    Friend WithEvents ComboBoxFilerItemType As ComboBox
    Friend WithEvents TextBoxFilterSupplierCode As TOA.ToaTextBox
End Class
