<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DialogPlanPeriod
    Inherits TOA.ToaDialog

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(DialogPlanPeriod))
        Me.Label3 = New System.Windows.Forms.Label()
        Me.DateBoxStartDate = New TOA.ToaDateBox()
        Me.DateBoxEndDate = New TOA.ToaDateBox()
        Me.ButtonExport = New System.Windows.Forms.Button()
        Me.ButtonClose = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'Label3
        '
        Me.Label3.BackColor = System.Drawing.SystemColors.GradientInactiveCaption
        Me.Label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label3.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label3.Location = New System.Drawing.Point(13, 14)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(101, 23)
        Me.Label3.TabIndex = 64
        Me.Label3.Text = "製造日"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'DateBoxStartDate
        '
        Me.DateBoxStartDate.AcceptsTab = True
        Me.DateBoxStartDate.BackColor = System.Drawing.Color.White
        Me.DateBoxStartDate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.DateBoxStartDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!)
        Me.DateBoxStartDate.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.DateBoxStartDate.Location = New System.Drawing.Point(113, 14)
        Me.DateBoxStartDate.MaxLength = 10
        Me.DateBoxStartDate.Name = "DateBoxStartDate"
        Me.DateBoxStartDate.OldText = Nothing
        Me.DateBoxStartDate.Size = New System.Drawing.Size(104, 23)
        Me.DateBoxStartDate.TabIndex = 63
        Me.DateBoxStartDate.Text = "yyyy/MM/dd"
        Me.DateBoxStartDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'DateBoxEndDate
        '
        Me.DateBoxEndDate.AcceptsTab = True
        Me.DateBoxEndDate.BackColor = System.Drawing.Color.White
        Me.DateBoxEndDate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.DateBoxEndDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!)
        Me.DateBoxEndDate.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.DateBoxEndDate.Location = New System.Drawing.Point(253, 14)
        Me.DateBoxEndDate.MaxLength = 10
        Me.DateBoxEndDate.Name = "DateBoxEndDate"
        Me.DateBoxEndDate.OldText = Nothing
        Me.DateBoxEndDate.Size = New System.Drawing.Size(104, 23)
        Me.DateBoxEndDate.TabIndex = 65
        Me.DateBoxEndDate.Text = "yyyy/MM/dd"
        Me.DateBoxEndDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'ButtonExport
        '
        Me.ButtonExport.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonExport.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ButtonExport.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonExport.Image = Global.DeliPlanner.My.Resources.Resources.excel
        Me.ButtonExport.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ButtonExport.Location = New System.Drawing.Point(112, 47)
        Me.ButtonExport.Name = "ButtonExport"
        Me.ButtonExport.Size = New System.Drawing.Size(120, 30)
        Me.ButtonExport.TabIndex = 66
        Me.ButtonExport.Text = "出力"
        Me.ButtonExport.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonExport.UseVisualStyleBackColor = False
        '
        'ButtonClose
        '
        Me.ButtonClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonClose.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ButtonClose.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonClose.Image = Global.DeliPlanner.My.Resources.Resources.cross
        Me.ButtonClose.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ButtonClose.Location = New System.Drawing.Point(238, 47)
        Me.ButtonClose.Name = "ButtonClose"
        Me.ButtonClose.Size = New System.Drawing.Size(120, 30)
        Me.ButtonClose.TabIndex = 67
        Me.ButtonClose.TabStop = False
        Me.ButtonClose.Text = "キャンセル"
        Me.ButtonClose.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonClose.UseVisualStyleBackColor = False
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("MS UI Gothic", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(216, 14)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(38, 22)
        Me.Label1.TabIndex = 68
        Me.Label1.Text = "～"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'DialogPlanPeriod
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(370, 89)
        Me.Controls.Add(Me.ButtonExport)
        Me.Controls.Add(Me.ButtonClose)
        Me.Controls.Add(Me.DateBoxEndDate)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.DateBoxStartDate)
        Me.Controls.Add(Me.Label1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "DialogPlanPeriod"
        Me.Text = "製造計画書"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label3 As Label
    Friend WithEvents DateBoxStartDate As TOA.ToaDateBox
    Friend WithEvents DateBoxEndDate As TOA.ToaDateBox
    Friend WithEvents ButtonExport As Button
    Friend WithEvents ButtonClose As Button
    Friend WithEvents Label1 As Label
End Class
