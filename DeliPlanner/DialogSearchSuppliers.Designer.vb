<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DialogSearchSuppliers
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(DialogSearchSuppliers))
        Me.PanelSuppliers = New System.Windows.Forms.Panel()
        Me.TextBoxFilterSupplierName = New TOA.ToaTextBox()
        Me.ButtonClose = New System.Windows.Forms.Button()
        Me.PanelItemType = New System.Windows.Forms.Panel()
        Me.TextBoxItemTypeId = New TOA.ToaTextBox()
        Me.SuspendLayout()
        '
        'PanelSuppliers
        '
        Me.PanelSuppliers.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PanelSuppliers.Location = New System.Drawing.Point(11, 88)
        Me.PanelSuppliers.Name = "PanelSuppliers"
        Me.PanelSuppliers.Size = New System.Drawing.Size(573, 389)
        Me.PanelSuppliers.TabIndex = 17
        '
        'TextBoxFilterSupplierName
        '
        Me.TextBoxFilterSupplierName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxFilterSupplierName.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBoxFilterSupplierName.ImeMode = System.Windows.Forms.ImeMode.Hiragana
        Me.TextBoxFilterSupplierName.Location = New System.Drawing.Point(62, 63)
        Me.TextBoxFilterSupplierName.MaxByteLength = 40
        Me.TextBoxFilterSupplierName.Name = "TextBoxFilterSupplierName"
        Me.TextBoxFilterSupplierName.OldText = Nothing
        Me.TextBoxFilterSupplierName.Size = New System.Drawing.Size(501, 23)
        Me.TextBoxFilterSupplierName.TabIndex = 35
        Me.TextBoxFilterSupplierName.Text = "１２３４５６７８９０１２３４５６７８９０１２３４５６７８９０"
        '
        'ButtonClose
        '
        Me.ButtonClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonClose.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ButtonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.ButtonClose.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonClose.Image = Global.DeliPlanner.My.Resources.Resources.cross
        Me.ButtonClose.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ButtonClose.Location = New System.Drawing.Point(464, 489)
        Me.ButtonClose.Name = "ButtonClose"
        Me.ButtonClose.Size = New System.Drawing.Size(120, 30)
        Me.ButtonClose.TabIndex = 58
        Me.ButtonClose.TabStop = False
        Me.ButtonClose.Text = "キャンセル"
        Me.ButtonClose.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonClose.UseVisualStyleBackColor = False
        '
        'PanelItemType
        '
        Me.PanelItemType.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PanelItemType.Location = New System.Drawing.Point(12, 12)
        Me.PanelItemType.Name = "PanelItemType"
        Me.PanelItemType.Size = New System.Drawing.Size(572, 40)
        Me.PanelItemType.TabIndex = 59
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
        Me.TextBoxItemTypeId.TabIndex = 60
        Me.TextBoxItemTypeId.Visible = False
        '
        'DialogSearchSuppliers
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.ButtonClose
        Me.ClientSize = New System.Drawing.Size(596, 531)
        Me.Controls.Add(Me.TextBoxItemTypeId)
        Me.Controls.Add(Me.PanelItemType)
        Me.Controls.Add(Me.ButtonClose)
        Me.Controls.Add(Me.TextBoxFilterSupplierName)
        Me.Controls.Add(Me.PanelSuppliers)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "DialogSearchSuppliers"
        Me.Text = "仕入先選択"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents PanelSuppliers As Panel
    Friend WithEvents TextBoxFilterSupplierName As TOA.ToaTextBox
    Friend WithEvents ButtonClose As Button
    Friend WithEvents PanelItemType As Panel
    Friend WithEvents TextBoxItemTypeId As TOA.ToaTextBox
End Class
