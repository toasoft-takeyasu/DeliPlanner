<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DialogSearchItems
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(DialogSearchItems))
        Me.PanelItems = New System.Windows.Forms.Panel()
        Me.TextBoxFilterItemName = New TOA.ToaTextBox()
        Me.TextBoxItemTypeId = New TOA.ToaTextBox()
        Me.PanelItemType = New System.Windows.Forms.Panel()
        Me.ButtonClose = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'PanelItems
        '
        Me.PanelItems.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PanelItems.Location = New System.Drawing.Point(11, 88)
        Me.PanelItems.Name = "PanelItems"
        Me.PanelItems.Size = New System.Drawing.Size(572, 389)
        Me.PanelItems.TabIndex = 17
        '
        'TextBoxFilterItemName
        '
        Me.TextBoxFilterItemName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxFilterItemName.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBoxFilterItemName.ImeMode = System.Windows.Forms.ImeMode.Hiragana
        Me.TextBoxFilterItemName.Location = New System.Drawing.Point(62, 63)
        Me.TextBoxFilterItemName.MaxByteLength = 40
        Me.TextBoxFilterItemName.Name = "TextBoxFilterItemName"
        Me.TextBoxFilterItemName.OldText = Nothing
        Me.TextBoxFilterItemName.Size = New System.Drawing.Size(501, 23)
        Me.TextBoxFilterItemName.TabIndex = 35
        Me.TextBoxFilterItemName.Text = "１２３４５６７８９０１２３４５６７８９０１２３４５６７８９０"
        '
        'TextBoxItemTypeId
        '
        Me.TextBoxItemTypeId.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxItemTypeId.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBoxItemTypeId.ImeMode = System.Windows.Forms.ImeMode.Hiragana
        Me.TextBoxItemTypeId.Location = New System.Drawing.Point(12, 63)
        Me.TextBoxItemTypeId.MaxByteLength = 40
        Me.TextBoxItemTypeId.Name = "TextBoxItemTypeId"
        Me.TextBoxItemTypeId.OldText = Nothing
        Me.TextBoxItemTypeId.Size = New System.Drawing.Size(34, 23)
        Me.TextBoxItemTypeId.TabIndex = 36
        Me.TextBoxItemTypeId.Visible = False
        '
        'PanelItemType
        '
        Me.PanelItemType.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PanelItemType.Location = New System.Drawing.Point(12, 12)
        Me.PanelItemType.Name = "PanelItemType"
        Me.PanelItemType.Size = New System.Drawing.Size(571, 40)
        Me.PanelItemType.TabIndex = 37
        '
        'ButtonClose
        '
        Me.ButtonClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonClose.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ButtonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.ButtonClose.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonClose.Image = Global.DeliPlanner.My.Resources.Resources.cross
        Me.ButtonClose.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ButtonClose.Location = New System.Drawing.Point(463, 489)
        Me.ButtonClose.Name = "ButtonClose"
        Me.ButtonClose.Size = New System.Drawing.Size(120, 30)
        Me.ButtonClose.TabIndex = 58
        Me.ButtonClose.TabStop = False
        Me.ButtonClose.Text = "キャンセル"
        Me.ButtonClose.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonClose.UseVisualStyleBackColor = False
        '
        'DialogSearchItems
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.ButtonClose
        Me.ClientSize = New System.Drawing.Size(595, 531)
        Me.Controls.Add(Me.ButtonClose)
        Me.Controls.Add(Me.PanelItemType)
        Me.Controls.Add(Me.TextBoxItemTypeId)
        Me.Controls.Add(Me.TextBoxFilterItemName)
        Me.Controls.Add(Me.PanelItems)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "DialogSearchItems"
        Me.Text = "品目選択"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents PanelItems As Panel
    Friend WithEvents TextBoxFilterItemName As TOA.ToaTextBox
    Friend WithEvents TextBoxItemTypeId As TOA.ToaTextBox
    Friend WithEvents PanelItemType As Panel
    Friend WithEvents ButtonClose As Button
End Class
