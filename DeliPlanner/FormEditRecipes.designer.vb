<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormEditRecipes
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
        Me.TextBoxFilterItemCode = New TOA.ToaTextBox()
        Me.ComboBoxFilerItemType = New System.Windows.Forms.ComboBox()
        Me.PanelItems = New System.Windows.Forms.Panel()
        Me.TextBoxFilterItemName = New TOA.ToaTextBox()
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
        Me.PanelDetail.Controls.Add(Me.TextBoxFilterItemCode)
        Me.PanelDetail.Controls.Add(Me.ComboBoxFilerItemType)
        Me.PanelDetail.Controls.Add(Me.PanelItems)
        Me.PanelDetail.Controls.Add(Me.TextBoxFilterItemName)
        Me.PanelDetail.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelDetail.Location = New System.Drawing.Point(0, 0)
        Me.PanelDetail.Name = "PanelDetail"
        Me.PanelDetail.Padding = New System.Windows.Forms.Padding(10, 0, 10, 0)
        Me.PanelDetail.Size = New System.Drawing.Size(1368, 871)
        Me.PanelDetail.TabIndex = 12
        '
        'TextBoxFilterItemCode
        '
        Me.TextBoxFilterItemCode.BackColor = System.Drawing.Color.White
        Me.TextBoxFilterItemCode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxFilterItemCode.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBoxFilterItemCode.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.TextBoxFilterItemCode.Location = New System.Drawing.Point(143, 20)
        Me.TextBoxFilterItemCode.MaxByteLength = 8
        Me.TextBoxFilterItemCode.Name = "TextBoxFilterItemCode"
        Me.TextBoxFilterItemCode.NumericOnly = True
        Me.TextBoxFilterItemCode.OldText = Nothing
        Me.TextBoxFilterItemCode.Size = New System.Drawing.Size(91, 22)
        Me.TextBoxFilterItemCode.TabIndex = 53
        Me.TextBoxFilterItemCode.Text = "0000000000"
        '
        'ComboBoxFilerItemType
        '
        Me.ComboBoxFilerItemType.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ComboBoxFilerItemType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBoxFilerItemType.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ComboBoxFilerItemType.FormattingEnabled = True
        Me.ComboBoxFilerItemType.Location = New System.Drawing.Point(63, 20)
        Me.ComboBoxFilerItemType.Name = "ComboBoxFilerItemType"
        Me.ComboBoxFilerItemType.Size = New System.Drawing.Size(81, 23)
        Me.ComboBoxFilerItemType.TabIndex = 38
        '
        'PanelItems
        '
        Me.PanelItems.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PanelItems.Location = New System.Drawing.Point(12, 46)
        Me.PanelItems.Name = "PanelItems"
        Me.PanelItems.Size = New System.Drawing.Size(1344, 813)
        Me.PanelItems.TabIndex = 37
        '
        'TextBoxFilterItemName
        '
        Me.TextBoxFilterItemName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxFilterItemName.Font = New System.Drawing.Font("ＭＳ ゴシック", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBoxFilterItemName.ImeMode = System.Windows.Forms.ImeMode.Hiragana
        Me.TextBoxFilterItemName.Location = New System.Drawing.Point(233, 20)
        Me.TextBoxFilterItemName.MaxByteLength = 40
        Me.TextBoxFilterItemName.Name = "TextBoxFilterItemName"
        Me.TextBoxFilterItemName.OldText = Nothing
        Me.TextBoxFilterItemName.Size = New System.Drawing.Size(491, 22)
        Me.TextBoxFilterItemName.TabIndex = 36
        '
        'FormEditRecipes
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CausesValidation = False
        Me.ClientSize = New System.Drawing.Size(1368, 871)
        Me.Name = "FormEditRecipes"
        Me.PanelMain.ResumeLayout(False)
        Me.PanelDetail.ResumeLayout(False)
        Me.PanelDetail.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents PanelDetail As System.Windows.Forms.Panel
    Friend WithEvents TextBoxFilterItemName As TOA.ToaTextBox
    Friend WithEvents PanelItems As Panel
    Friend WithEvents ComboBoxFilerItemType As ComboBox
    Friend WithEvents TextBoxFilterItemCode As TOA.ToaTextBox
End Class
