<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormPlans
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
        Me.ButtonOutputPlanList = New System.Windows.Forms.Button()
        Me.ButtonRegist = New System.Windows.Forms.Button()
        Me.PanelPlans = New System.Windows.Forms.Panel()
        Me.DateBoxPlanDate = New TOA.ToaDateBox()
        Me.ButtonImportPlanData = New System.Windows.Forms.Button()
        Me.ButtonPlanDatePrevious = New System.Windows.Forms.Button()
        Me.ButtonPlanDateNext = New System.Windows.Forms.Button()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.ButtonExportPcrsShipment = New System.Windows.Forms.Button()
        Me.ButtonCopy = New System.Windows.Forms.Button()
        Me.PanelMain.SuspendLayout()
        Me.SuspendLayout()
        '
        'PanelMain
        '
        Me.PanelMain.Controls.Add(Me.ButtonExportPcrsShipment)
        Me.PanelMain.Controls.Add(Me.Label3)
        Me.PanelMain.Controls.Add(Me.DateBoxPlanDate)
        Me.PanelMain.Controls.Add(Me.ButtonPlanDatePrevious)
        Me.PanelMain.Controls.Add(Me.ButtonPlanDateNext)
        Me.PanelMain.Controls.Add(Me.ButtonImportPlanData)
        Me.PanelMain.Controls.Add(Me.PanelPlans)
        Me.PanelMain.Controls.Add(Me.ButtonRegist)
        Me.PanelMain.Controls.Add(Me.ButtonOutputPlanList)
        Me.PanelMain.Controls.Add(Me.ButtonCopy)
        Me.PanelMain.Size = New System.Drawing.Size(1368, 871)
        '
        'ButtonExportPlanList
        '
        Me.ButtonOutputPlanList.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.ButtonOutputPlanList.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ButtonOutputPlanList.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonOutputPlanList.Image = Global.DeliPlanner.My.Resources.Resources.excel
        Me.ButtonOutputPlanList.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ButtonOutputPlanList.Location = New System.Drawing.Point(12, 829)
        Me.ButtonOutputPlanList.Name = "ButtonExportPlanList"
        Me.ButtonOutputPlanList.Size = New System.Drawing.Size(120, 30)
        Me.ButtonOutputPlanList.TabIndex = 6
        Me.ButtonOutputPlanList.TabStop = False
        Me.ButtonOutputPlanList.Text = "製造計画書"
        Me.ButtonOutputPlanList.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonOutputPlanList.UseVisualStyleBackColor = False
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
        Me.ButtonRegist.TabIndex = 7
        Me.ButtonRegist.TabStop = False
        Me.ButtonRegist.Text = "登録"
        Me.ButtonRegist.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonRegist.UseVisualStyleBackColor = False
        '
        'PanelPlans
        '
        Me.PanelPlans.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PanelPlans.Location = New System.Drawing.Point(12, 48)
        Me.PanelPlans.Name = "PanelPlans"
        Me.PanelPlans.Size = New System.Drawing.Size(1344, 771)
        Me.PanelPlans.TabIndex = 5
        '
        'DateBoxPlanDate
        '
        Me.DateBoxPlanDate.AcceptsTab = True
        Me.DateBoxPlanDate.BackColor = System.Drawing.Color.White
        Me.DateBoxPlanDate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.DateBoxPlanDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.DateBoxPlanDate.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.DateBoxPlanDate.Location = New System.Drawing.Point(164, 16)
        Me.DateBoxPlanDate.MaxLength = 10
        Me.DateBoxPlanDate.Name = "DateBoxPlanDate"
        Me.DateBoxPlanDate.OldText = Nothing
        Me.DateBoxPlanDate.Size = New System.Drawing.Size(104, 23)
        Me.DateBoxPlanDate.TabIndex = 1
        Me.DateBoxPlanDate.Text = "yyyy/MM/dd"
        Me.DateBoxPlanDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Button3
        '
        Me.ButtonImportPlanData.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonImportPlanData.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ButtonImportPlanData.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonImportPlanData.Image = Global.DeliPlanner.My.Resources.Resources.import
        Me.ButtonImportPlanData.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ButtonImportPlanData.Location = New System.Drawing.Point(1236, 12)
        Me.ButtonImportPlanData.Name = "Button3"
        Me.ButtonImportPlanData.Size = New System.Drawing.Size(120, 30)
        Me.ButtonImportPlanData.TabIndex = 4
        Me.ButtonImportPlanData.TabStop = False
        Me.ButtonImportPlanData.Text = "計画データ"
        Me.ButtonImportPlanData.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonImportPlanData.UseVisualStyleBackColor = False
        '
        'ButtonPlanDatePrevious
        '
        Me.ButtonPlanDatePrevious.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ButtonPlanDatePrevious.Font = New System.Drawing.Font("MS UI Gothic", 3.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonPlanDatePrevious.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ButtonPlanDatePrevious.Location = New System.Drawing.Point(268, 27)
        Me.ButtonPlanDatePrevious.Name = "ButtonPlanDatePrevious"
        Me.ButtonPlanDatePrevious.Size = New System.Drawing.Size(33, 13)
        Me.ButtonPlanDatePrevious.TabIndex = 12
        Me.ButtonPlanDatePrevious.TabStop = False
        Me.ButtonPlanDatePrevious.Text = "▼"
        Me.ButtonPlanDatePrevious.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonPlanDatePrevious.UseVisualStyleBackColor = False
        '
        'ButtonPlanDateNext
        '
        Me.ButtonPlanDateNext.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ButtonPlanDateNext.Font = New System.Drawing.Font("MS UI Gothic", 3.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonPlanDateNext.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ButtonPlanDateNext.Location = New System.Drawing.Point(268, 15)
        Me.ButtonPlanDateNext.Name = "ButtonPlanDateNext"
        Me.ButtonPlanDateNext.Size = New System.Drawing.Size(33, 13)
        Me.ButtonPlanDateNext.TabIndex = 13
        Me.ButtonPlanDateNext.TabStop = False
        Me.ButtonPlanDateNext.Text = "▲"
        Me.ButtonPlanDateNext.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonPlanDateNext.UseVisualStyleBackColor = False
        '
        'Label3
        '
        Me.Label3.BackColor = System.Drawing.SystemColors.GradientInactiveCaption
        Me.Label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label3.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label3.Location = New System.Drawing.Point(64, 16)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(101, 23)
        Me.Label3.TabIndex = 62
        Me.Label3.Text = "製造日"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Button2
        '
        Me.ButtonExportPcrsShipment.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.ButtonExportPcrsShipment.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ButtonExportPcrsShipment.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonExportPcrsShipment.Image = Global.DeliPlanner.My.Resources.Resources.csv
        Me.ButtonExportPcrsShipment.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ButtonExportPcrsShipment.Location = New System.Drawing.Point(138, 829)
        Me.ButtonExportPcrsShipment.Name = "Button2"
        Me.ButtonExportPcrsShipment.Size = New System.Drawing.Size(120, 30)
        Me.ButtonExportPcrsShipment.TabIndex = 63
        Me.ButtonExportPcrsShipment.TabStop = False
        Me.ButtonExportPcrsShipment.Text = "出荷データ"
        Me.ButtonExportPcrsShipment.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonExportPcrsShipment.UseVisualStyleBackColor = False
        '
        'ButtonCopy
        '
        Me.ButtonCopy.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ButtonCopy.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonCopy.Image = Global.DeliPlanner.My.Resources.Resources.copy
        Me.ButtonCopy.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ButtonCopy.Location = New System.Drawing.Point(310, 12)
        Me.ButtonCopy.Name = "ButtonCopy"
        Me.ButtonCopy.Size = New System.Drawing.Size(120, 30)
        Me.ButtonCopy.TabIndex = 3
        Me.ButtonCopy.TabStop = False
        Me.ButtonCopy.Text = "前日分コピー"
        Me.ButtonCopy.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonCopy.UseVisualStyleBackColor = False
        '
        'FormPlans
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CausesValidation = False
        Me.ClientSize = New System.Drawing.Size(1368, 871)
        Me.Name = "FormPlans"
        Me.Text = "FormInstructionPlans"
        Me.PanelMain.ResumeLayout(False)
        Me.PanelMain.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ButtonOutputPlanList As Button
    Friend WithEvents ButtonCopy As Button
    Friend WithEvents ButtonRegist As Button
    Friend WithEvents PanelPlans As Panel
    Friend WithEvents DateBoxPlanDate As TOA.ToaDateBox
    Friend WithEvents ButtonImportPlanData As Button
    Friend WithEvents ButtonPlanDatePrevious As Button
    Friend WithEvents ButtonPlanDateNext As Button
    Friend WithEvents Label3 As Label
    Friend WithEvents ButtonExportPcrsShipment As Button
End Class
