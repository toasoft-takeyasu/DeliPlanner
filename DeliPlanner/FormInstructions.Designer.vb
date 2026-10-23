<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormInstructions
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.ButtonRegist = New System.Windows.Forms.Button()
        Me.PanelInstructions = New System.Windows.Forms.Panel()
        Me.DateBoxStartDate = New TOA.ToaDateBox()
        Me.ButtonStartDatePrevious = New System.Windows.Forms.Button()
        Me.ButtonStartDateNext = New System.Windows.Forms.Button()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.ButtonExportPcrsShipment = New System.Windows.Forms.Button()
        Me.PanelMain.SuspendLayout()
        Me.SuspendLayout()
        '
        'PanelMain
        '
        Me.PanelMain.Controls.Add(Me.ButtonExportPcrsShipment)
        Me.PanelMain.Controls.Add(Me.Button1)
        Me.PanelMain.Controls.Add(Me.Label3)
        Me.PanelMain.Controls.Add(Me.DateBoxStartDate)
        Me.PanelMain.Controls.Add(Me.ButtonStartDatePrevious)
        Me.PanelMain.Controls.Add(Me.ButtonStartDateNext)
        Me.PanelMain.Controls.Add(Me.PanelInstructions)
        Me.PanelMain.Controls.Add(Me.ButtonRegist)
        Me.PanelMain.Size = New System.Drawing.Size(1368, 871)
        '
        'ButtonRegist
        '
        Me.ButtonRegist.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonRegist.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ButtonRegist.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonRegist.Image = Global.DeliPlanner.My.Resources.Resources.save
        Me.ButtonRegist.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ButtonRegist.Location = New System.Drawing.Point(1236, 829)
        Me.ButtonRegist.Name = "ButtonRegist"
        Me.ButtonRegist.Size = New System.Drawing.Size(120, 30)
        Me.ButtonRegist.TabIndex = 4
        Me.ButtonRegist.TabStop = False
        Me.ButtonRegist.Text = "登録"
        Me.ButtonRegist.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonRegist.UseVisualStyleBackColor = False
        '
        'PanelInstructions
        '
        Me.PanelInstructions.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PanelInstructions.Location = New System.Drawing.Point(12, 48)
        Me.PanelInstructions.Name = "PanelInstructions"
        Me.PanelInstructions.Size = New System.Drawing.Size(1344, 771)
        Me.PanelInstructions.TabIndex = 3
        '
        'DateBoxStartDate
        '
        Me.DateBoxStartDate.AcceptsTab = True
        Me.DateBoxStartDate.BackColor = System.Drawing.Color.White
        Me.DateBoxStartDate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.DateBoxStartDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.DateBoxStartDate.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.DateBoxStartDate.Location = New System.Drawing.Point(164, 16)
        Me.DateBoxStartDate.MaxLength = 10
        Me.DateBoxStartDate.Name = "DateBoxStartDate"
        Me.DateBoxStartDate.OldText = Nothing
        Me.DateBoxStartDate.Size = New System.Drawing.Size(104, 23)
        Me.DateBoxStartDate.TabIndex = 1
        Me.DateBoxStartDate.Text = "yyyy/MM/dd"
        Me.DateBoxStartDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'ButtonStartDatePrevious
        '
        Me.ButtonStartDatePrevious.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ButtonStartDatePrevious.Font = New System.Drawing.Font("MS UI Gothic", 3.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonStartDatePrevious.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ButtonStartDatePrevious.Location = New System.Drawing.Point(268, 27)
        Me.ButtonStartDatePrevious.Name = "ButtonStartDatePrevious"
        Me.ButtonStartDatePrevious.Size = New System.Drawing.Size(33, 13)
        Me.ButtonStartDatePrevious.TabIndex = 14
        Me.ButtonStartDatePrevious.TabStop = False
        Me.ButtonStartDatePrevious.Text = "▼"
        Me.ButtonStartDatePrevious.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonStartDatePrevious.UseVisualStyleBackColor = False
        '
        'ButtonStartDateNext
        '
        Me.ButtonStartDateNext.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ButtonStartDateNext.Font = New System.Drawing.Font("MS UI Gothic", 3.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonStartDateNext.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ButtonStartDateNext.Location = New System.Drawing.Point(268, 15)
        Me.ButtonStartDateNext.Name = "ButtonStartDateNext"
        Me.ButtonStartDateNext.Size = New System.Drawing.Size(33, 13)
        Me.ButtonStartDateNext.TabIndex = 15
        Me.ButtonStartDateNext.TabStop = False
        Me.ButtonStartDateNext.Text = "▲"
        Me.ButtonStartDateNext.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonStartDateNext.UseVisualStyleBackColor = False
        '
        'Label3
        '
        Me.Label3.BackColor = System.Drawing.SystemColors.GradientInactiveCaption
        Me.Label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label3.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label3.Location = New System.Drawing.Point(64, 16)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(101, 23)
        Me.Label3.TabIndex = 63
        Me.Label3.Text = "製造日"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Button1
        '
        Me.Button1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Button1.BackColor = System.Drawing.SystemColors.ControlLight
        Me.Button1.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Button1.Image = Global.DeliPlanner.My.Resources.Resources.import
        Me.Button1.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.Button1.Location = New System.Drawing.Point(1236, 12)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(120, 30)
        Me.Button1.TabIndex = 5
        Me.Button1.TabStop = False
        Me.Button1.Text = "実績データ"
        Me.Button1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.Button1.UseVisualStyleBackColor = False
        '
        'ButtonExportPcrsShipment
        '
        Me.ButtonExportPcrsShipment.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.ButtonExportPcrsShipment.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ButtonExportPcrsShipment.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonExportPcrsShipment.Image = Global.DeliPlanner.My.Resources.Resources.csv
        Me.ButtonExportPcrsShipment.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ButtonExportPcrsShipment.Location = New System.Drawing.Point(12, 829)
        Me.ButtonExportPcrsShipment.Name = "ButtonExportPcrsShipment"
        Me.ButtonExportPcrsShipment.Size = New System.Drawing.Size(120, 30)
        Me.ButtonExportPcrsShipment.TabIndex = 64
        Me.ButtonExportPcrsShipment.TabStop = False
        Me.ButtonExportPcrsShipment.Text = "実績データ"
        Me.ButtonExportPcrsShipment.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonExportPcrsShipment.UseVisualStyleBackColor = False
        '
        'FormInstructions
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CausesValidation = False
        Me.ClientSize = New System.Drawing.Size(1368, 871)
        Me.Name = "FormInstructions"
        Me.Text = "FormInstructions"
        Me.PanelMain.ResumeLayout(False)
        Me.PanelMain.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ButtonRegist As Button
    Friend WithEvents PanelInstructions As Panel
    Friend WithEvents DateBoxStartDate As TOA.ToaDateBox
    Friend WithEvents ButtonStartDatePrevious As Button
    Friend WithEvents ButtonStartDateNext As Button
    Friend WithEvents Label3 As Label
    Friend WithEvents Button1 As Button
    Friend WithEvents ButtonExportPcrsShipment As Button
End Class
