<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class DialogInventoryLogs
    Inherits TOA.ToaDialog

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

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.PanelFilter = New System.Windows.Forms.Panel()
        Me.DatePickerFrom = New System.Windows.Forms.DateTimePicker()
        Me.LabelTo = New System.Windows.Forms.Label()
        Me.DatePickerTo = New System.Windows.Forms.DateTimePicker()
        Me.ComboBoxStatus = New System.Windows.Forms.ComboBox()
        Me.ButtonSearch = New System.Windows.Forms.Button()
        Me.PanelItems = New System.Windows.Forms.Panel()
        Me.PanelBottom = New System.Windows.Forms.Panel()
        Me.ButtonClose = New System.Windows.Forms.Button()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.PanelFilter.SuspendLayout()
        Me.PanelBottom.SuspendLayout()
        Me.SuspendLayout()
        '
        'PanelFilter
        '
        Me.PanelFilter.Controls.Add(Me.Label1)
        Me.PanelFilter.Controls.Add(Me.Label3)
        Me.PanelFilter.Controls.Add(Me.DatePickerFrom)
        Me.PanelFilter.Controls.Add(Me.LabelTo)
        Me.PanelFilter.Controls.Add(Me.DatePickerTo)
        Me.PanelFilter.Controls.Add(Me.ComboBoxStatus)
        Me.PanelFilter.Controls.Add(Me.ButtonSearch)
        Me.PanelFilter.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelFilter.Location = New System.Drawing.Point(0, 0)
        Me.PanelFilter.Name = "PanelFilter"
        Me.PanelFilter.Size = New System.Drawing.Size(1184, 55)
        Me.PanelFilter.TabIndex = 0
        '
        'DatePickerFrom
        '
        Me.DatePickerFrom.Checked = False
        Me.DatePickerFrom.CustomFormat = "yyyy/MM/dd"
        Me.DatePickerFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.DatePickerFrom.Location = New System.Drawing.Point(112, 14)
        Me.DatePickerFrom.Name = "DatePickerFrom"
        Me.DatePickerFrom.ShowCheckBox = True
        Me.DatePickerFrom.Size = New System.Drawing.Size(155, 23)
        Me.DatePickerFrom.TabIndex = 1
        '
        'LabelTo
        '
        Me.LabelTo.Location = New System.Drawing.Point(274, 18)
        Me.LabelTo.Name = "LabelTo"
        Me.LabelTo.Size = New System.Drawing.Size(25, 25)
        Me.LabelTo.TabIndex = 2
        Me.LabelTo.Text = "～"
        '
        'DatePickerTo
        '
        Me.DatePickerTo.Checked = False
        Me.DatePickerTo.CustomFormat = "yyyy/MM/dd"
        Me.DatePickerTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.DatePickerTo.Location = New System.Drawing.Point(302, 14)
        Me.DatePickerTo.Name = "DatePickerTo"
        Me.DatePickerTo.ShowCheckBox = True
        Me.DatePickerTo.Size = New System.Drawing.Size(155, 23)
        Me.DatePickerTo.TabIndex = 3
        '
        'ComboBoxStatus
        '
        Me.ComboBoxStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBoxStatus.FormattingEnabled = True
        Me.ComboBoxStatus.Location = New System.Drawing.Point(591, 14)
        Me.ComboBoxStatus.Name = "ComboBoxStatus"
        Me.ComboBoxStatus.Size = New System.Drawing.Size(180, 24)
        Me.ComboBoxStatus.TabIndex = 5
        '
        'ButtonSearch
        '
        Me.ButtonSearch.Image = Global.DeliPlanner.My.Resources.Resources.search
        Me.ButtonSearch.Location = New System.Drawing.Point(804, 11)
        Me.ButtonSearch.Name = "ButtonSearch"
        Me.ButtonSearch.Size = New System.Drawing.Size(120, 30)
        Me.ButtonSearch.TabIndex = 6
        Me.ButtonSearch.Text = "検索"
        Me.ButtonSearch.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ButtonSearch.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonSearch.UseVisualStyleBackColor = True
        '
        'PanelItems
        '
        Me.PanelItems.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelItems.Location = New System.Drawing.Point(0, 55)
        Me.PanelItems.Name = "PanelItems"
        Me.PanelItems.Padding = New System.Windows.Forms.Padding(12, 0, 12, 0)
        Me.PanelItems.Size = New System.Drawing.Size(1184, 506)
        Me.PanelItems.TabIndex = 1
        '
        'PanelBottom
        '
        Me.PanelBottom.Controls.Add(Me.ButtonClose)
        Me.PanelBottom.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.PanelBottom.Location = New System.Drawing.Point(0, 561)
        Me.PanelBottom.Name = "PanelBottom"
        Me.PanelBottom.Size = New System.Drawing.Size(1184, 50)
        Me.PanelBottom.TabIndex = 2
        '
        'ButtonClose
        '
        Me.ButtonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.ButtonClose.Image = Global.DeliPlanner.My.Resources.Resources.cross
        Me.ButtonClose.Location = New System.Drawing.Point(12, 9)
        Me.ButtonClose.Name = "ButtonClose"
        Me.ButtonClose.Size = New System.Drawing.Size(120, 30)
        Me.ButtonClose.TabIndex = 0
        Me.ButtonClose.Text = "閉じる"
        Me.ButtonClose.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ButtonClose.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ButtonClose.UseVisualStyleBackColor = True
        '
        'Label3
        '
        Me.Label3.BackColor = System.Drawing.SystemColors.GradientInactiveCaption
        Me.Label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label3.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label3.Location = New System.Drawing.Point(12, 14)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(101, 23)
        Me.Label3.TabIndex = 65
        Me.Label3.Text = "処理日"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label1
        '
        Me.Label1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption
        Me.Label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label1.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(491, 15)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(101, 23)
        Me.Label1.TabIndex = 66
        Me.Label1.Text = "区分"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'DialogInventoryLogs
        '
        Me.AcceptButton = Me.ButtonSearch
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.ButtonClose
        Me.ClientSize = New System.Drawing.Size(1184, 611)
        Me.Controls.Add(Me.PanelItems)
        Me.Controls.Add(Me.PanelFilter)
        Me.Controls.Add(Me.PanelBottom)
        Me.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable
        Me.MinimumSize = New System.Drawing.Size(900, 450)
        Me.Name = "DialogInventoryLogs"
        Me.Text = "在庫履歴"
        Me.PanelFilter.ResumeLayout(False)
        Me.PanelBottom.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents PanelFilter As Panel
    Friend WithEvents DatePickerFrom As DateTimePicker
    Friend WithEvents LabelTo As Label
    Friend WithEvents DatePickerTo As DateTimePicker
    Friend WithEvents ComboBoxStatus As ComboBox
    Friend WithEvents ButtonSearch As Button
    Friend WithEvents PanelItems As Panel
    Friend WithEvents PanelBottom As Panel
    Friend WithEvents ButtonClose As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents Label3 As Label
End Class