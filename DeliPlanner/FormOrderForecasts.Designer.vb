<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormOrderForecasts
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
        Me.PanelOrderForecasts = New System.Windows.Forms.Panel()
        Me.ButtonPlanDatePrevious = New System.Windows.Forms.Button()
        Me.ButtonPlanDateNext = New System.Windows.Forms.Button()
        Me.DateBoxPlanDate = New TOA.ToaDateBox()
        Me.TextBoxItemName = New TOA.ToaTextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.ButtonSearchItem = New System.Windows.Forms.Button()
        Me.TextBoxItemId = New TOA.ToaTextBox()
        Me.ButtonClearItem = New System.Windows.Forms.Button()
        Me.CheckBoxHalfProduct = New System.Windows.Forms.CheckBox()
        Me.CheckBoxMaterial = New System.Windows.Forms.CheckBox()
        Me.PanelMain.SuspendLayout()
        Me.SuspendLayout()
        '
        'PanelMain
        '
        Me.PanelMain.Controls.Add(Me.CheckBoxMaterial)
        Me.PanelMain.Controls.Add(Me.CheckBoxHalfProduct)
        Me.PanelMain.Controls.Add(Me.DateBoxPlanDate)
        Me.PanelMain.Controls.Add(Me.ButtonClearItem)
        Me.PanelMain.Controls.Add(Me.TextBoxItemId)
        Me.PanelMain.Controls.Add(Me.ButtonSearchItem)
        Me.PanelMain.Controls.Add(Me.TextBoxItemName)
        Me.PanelMain.Controls.Add(Me.Label2)
        Me.PanelMain.Controls.Add(Me.ButtonPlanDatePrevious)
        Me.PanelMain.Controls.Add(Me.ButtonPlanDateNext)
        Me.PanelMain.Controls.Add(Me.PanelOrderForecasts)
        Me.PanelMain.Size = New System.Drawing.Size(1368, 871)
        '
        'PanelOrderForecasts
        '
        Me.PanelOrderForecasts.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PanelOrderForecasts.Location = New System.Drawing.Point(12, 48)
        Me.PanelOrderForecasts.Name = "PanelOrderForecasts"
        Me.PanelOrderForecasts.Size = New System.Drawing.Size(1344, 811)
        Me.PanelOrderForecasts.TabIndex = 0
        '
        'ButtonPlanDatePrevious
        '
        Me.ButtonPlanDatePrevious.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ButtonPlanDatePrevious.Font = New System.Drawing.Font("MS UI Gothic", 3.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonPlanDatePrevious.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ButtonPlanDatePrevious.Location = New System.Drawing.Point(168, 27)
        Me.ButtonPlanDatePrevious.Name = "ButtonPlanDatePrevious"
        Me.ButtonPlanDatePrevious.Size = New System.Drawing.Size(33, 13)
        Me.ButtonPlanDatePrevious.TabIndex = 0
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
        Me.ButtonPlanDateNext.Location = New System.Drawing.Point(168, 15)
        Me.ButtonPlanDateNext.Name = "ButtonPlanDateNext"
        Me.ButtonPlanDateNext.Size = New System.Drawing.Size(33, 13)
        Me.ButtonPlanDateNext.TabIndex = 2
        Me.ButtonPlanDateNext.TabStop = False
        Me.ButtonPlanDateNext.Text = "▲"
        Me.ButtonPlanDateNext.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonPlanDateNext.UseVisualStyleBackColor = False
        '
        'DateBoxPlanDate
        '
        Me.DateBoxPlanDate.AcceptsTab = True
        Me.DateBoxPlanDate.BackColor = System.Drawing.Color.White
        Me.DateBoxPlanDate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.DateBoxPlanDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.DateBoxPlanDate.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.DateBoxPlanDate.Location = New System.Drawing.Point(64, 16)
        Me.DateBoxPlanDate.MaxLength = 10
        Me.DateBoxPlanDate.Name = "DateBoxPlanDate"
        Me.DateBoxPlanDate.OldText = Nothing
        Me.DateBoxPlanDate.Size = New System.Drawing.Size(104, 23)
        Me.DateBoxPlanDate.TabIndex = 1
        Me.DateBoxPlanDate.Text = "yyyy/MM/dd"
        Me.DateBoxPlanDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'TextBoxItemName
        '
        Me.TextBoxItemName.BackColor = System.Drawing.SystemColors.Window
        Me.TextBoxItemName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxItemName.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBoxItemName.ImeMode = System.Windows.Forms.ImeMode.Hiragana
        Me.TextBoxItemName.Location = New System.Drawing.Point(313, 16)
        Me.TextBoxItemName.MaxByteLength = 40
        Me.TextBoxItemName.Name = "TextBoxItemName"
        Me.TextBoxItemName.OldText = Nothing
        Me.TextBoxItemName.ReadOnly = True
        Me.TextBoxItemName.Size = New System.Drawing.Size(248, 23)
        Me.TextBoxItemName.TabIndex = 5
        Me.TextBoxItemName.TabStop = False
        Me.TextBoxItemName.Text = "１２３４５６７８９０１２３４５６７８９０１２３４５６７８９０"
        '
        'Label2
        '
        Me.Label2.BackColor = System.Drawing.SystemColors.GradientInactiveCaption
        Me.Label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label2.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(213, 16)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(101, 23)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "品目名"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'ButtonSearchItem
        '
        Me.ButtonSearchItem.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ButtonSearchItem.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonSearchItem.Image = Global.DeliPlanner.My.Resources.Resources.search
        Me.ButtonSearchItem.Location = New System.Drawing.Point(561, 15)
        Me.ButtonSearchItem.Name = "ButtonSearchItem"
        Me.ButtonSearchItem.Size = New System.Drawing.Size(30, 25)
        Me.ButtonSearchItem.TabIndex = 6
        Me.ButtonSearchItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonSearchItem.UseVisualStyleBackColor = False
        '
        'TextBoxItemId
        '
        Me.TextBoxItemId.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextBoxItemId.BackColor = System.Drawing.Color.White
        Me.TextBoxItemId.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxItemId.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBoxItemId.Format = "#,###"
        Me.TextBoxItemId.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.TextBoxItemId.Location = New System.Drawing.Point(494, 16)
        Me.TextBoxItemId.MaxByteLength = 7
        Me.TextBoxItemId.MaxValue = "0"
        Me.TextBoxItemId.MinValue = "-999999"
        Me.TextBoxItemId.Name = "TextBoxItemId"
        Me.TextBoxItemId.NumericOnly = True
        Me.TextBoxItemId.OldText = Nothing
        Me.TextBoxItemId.Size = New System.Drawing.Size(67, 23)
        Me.TextBoxItemId.TabIndex = 9
        Me.TextBoxItemId.TabStop = False
        Me.TextBoxItemId.Text = "0"
        Me.TextBoxItemId.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.TextBoxItemId.Visible = False
        '
        'ButtonClearItem
        '
        Me.ButtonClearItem.BackColor = System.Drawing.SystemColors.ControlLight
        Me.ButtonClearItem.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ButtonClearItem.Image = Global.DeliPlanner.My.Resources.Resources.cross
        Me.ButtonClearItem.Location = New System.Drawing.Point(590, 15)
        Me.ButtonClearItem.Name = "ButtonClearItem"
        Me.ButtonClearItem.Size = New System.Drawing.Size(30, 25)
        Me.ButtonClearItem.TabIndex = 7
        Me.ButtonClearItem.TabStop = False
        Me.ButtonClearItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonClearItem.UseVisualStyleBackColor = False
        '
        'CheckBoxHalfProduct
        '
        Me.CheckBoxHalfProduct.AutoSize = True
        Me.CheckBoxHalfProduct.Font = New System.Drawing.Font("MS UI Gothic", 12.0!)
        Me.CheckBoxHalfProduct.Location = New System.Drawing.Point(740, 16)
        Me.CheckBoxHalfProduct.Name = "CheckBoxHalfProduct"
        Me.CheckBoxHalfProduct.Size = New System.Drawing.Size(75, 20)
        Me.CheckBoxHalfProduct.TabIndex = 10
        Me.CheckBoxHalfProduct.Text = "半製品"
        Me.CheckBoxHalfProduct.UseVisualStyleBackColor = True
        '
        'CheckBoxMaterial
        '
        Me.CheckBoxMaterial.AutoSize = True
        Me.CheckBoxMaterial.Font = New System.Drawing.Font("MS UI Gothic", 12.0!)
        Me.CheckBoxMaterial.Location = New System.Drawing.Point(831, 16)
        Me.CheckBoxMaterial.Name = "CheckBoxMaterial"
        Me.CheckBoxMaterial.Size = New System.Drawing.Size(99, 20)
        Me.CheckBoxMaterial.TabIndex = 11
        Me.CheckBoxMaterial.Text = "材料・資材"
        Me.CheckBoxMaterial.UseVisualStyleBackColor = True
        '
        'FormOrderForecasts
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CausesValidation = False
        Me.ClientSize = New System.Drawing.Size(1368, 871)
        Me.Name = "FormOrderForecasts"
        Me.Text = "FormOrderForecasts"
        Me.PanelMain.ResumeLayout(False)
        Me.PanelMain.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents PanelOrderForecasts As Panel
    Friend WithEvents ButtonPlanDatePrevious As Button
    Friend WithEvents ButtonPlanDateNext As Button
    Friend WithEvents DateBoxPlanDate As TOA.ToaDateBox
    Friend WithEvents TextBoxItemName As TOA.ToaTextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents ButtonSearchItem As Button
    Friend WithEvents TextBoxItemId As TOA.ToaTextBox
    Friend WithEvents ButtonClearItem As Button
    Friend WithEvents CheckBoxMaterial As CheckBox
    Friend WithEvents CheckBoxHalfProduct As CheckBox
End Class
