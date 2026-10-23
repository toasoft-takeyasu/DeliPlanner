<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormOrders
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
        Me.DateBoxFilterArrivalDate = New TOA.ToaDateBox()
        Me.DateBoxOrderDateTo = New TOA.ToaDateBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.DateBoxOrderDateFrom = New TOA.ToaDateBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.CheckBoxNotArrived = New System.Windows.Forms.CheckBox()
        Me.DateBoxFilterOrderDate = New TOA.ToaDateBox()
        Me.TextBoxFilterSupplierName = New TOA.ToaTextBox()
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
        Me.PanelDetail.Controls.Add(Me.DateBoxFilterArrivalDate)
        Me.PanelDetail.Controls.Add(Me.DateBoxOrderDateTo)
        Me.PanelDetail.Controls.Add(Me.Label3)
        Me.PanelDetail.Controls.Add(Me.DateBoxOrderDateFrom)
        Me.PanelDetail.Controls.Add(Me.Label1)
        Me.PanelDetail.Controls.Add(Me.CheckBoxNotArrived)
        Me.PanelDetail.Controls.Add(Me.DateBoxFilterOrderDate)
        Me.PanelDetail.Controls.Add(Me.TextBoxFilterSupplierName)
        Me.PanelDetail.Controls.Add(Me.PanelStores)
        Me.PanelDetail.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelDetail.Location = New System.Drawing.Point(0, 0)
        Me.PanelDetail.Name = "PanelDetail"
        Me.PanelDetail.Padding = New System.Windows.Forms.Padding(10, 0, 10, 0)
        Me.PanelDetail.Size = New System.Drawing.Size(1368, 871)
        Me.PanelDetail.TabIndex = 12
        '
        'DateBoxFilterArrivalDate
        '
        Me.DateBoxFilterArrivalDate.AcceptsTab = True
        Me.DateBoxFilterArrivalDate.BackColor = System.Drawing.Color.White
        Me.DateBoxFilterArrivalDate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.DateBoxFilterArrivalDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.DateBoxFilterArrivalDate.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.DateBoxFilterArrivalDate.Location = New System.Drawing.Point(162, 58)
        Me.DateBoxFilterArrivalDate.MaxLength = 10
        Me.DateBoxFilterArrivalDate.Name = "DateBoxFilterArrivalDate"
        Me.DateBoxFilterArrivalDate.OldText = Nothing
        Me.DateBoxFilterArrivalDate.Size = New System.Drawing.Size(100, 23)
        Me.DateBoxFilterArrivalDate.TabIndex = 8
        Me.DateBoxFilterArrivalDate.Text = "yyyy/MM/dd"
        Me.DateBoxFilterArrivalDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'DateBoxOrderDateTo
        '
        Me.DateBoxOrderDateTo.AcceptsTab = True
        Me.DateBoxOrderDateTo.BackColor = System.Drawing.Color.White
        Me.DateBoxOrderDateTo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.DateBoxOrderDateTo.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.DateBoxOrderDateTo.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.DateBoxOrderDateTo.Location = New System.Drawing.Point(299, 16)
        Me.DateBoxOrderDateTo.MaxLength = 10
        Me.DateBoxOrderDateTo.Name = "DateBoxOrderDateTo"
        Me.DateBoxOrderDateTo.OldText = Nothing
        Me.DateBoxOrderDateTo.Size = New System.Drawing.Size(100, 23)
        Me.DateBoxOrderDateTo.TabIndex = 3
        Me.DateBoxOrderDateTo.Text = "yyyy/MM/dd"
        Me.DateBoxOrderDateTo.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label3
        '
        Me.Label3.BackColor = System.Drawing.SystemColors.GradientInactiveCaption
        Me.Label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Label3.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label3.Location = New System.Drawing.Point(64, 16)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(100, 23)
        Me.Label3.TabIndex = 0
        Me.Label3.Text = "発注日"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'DateBoxOrderDateFrom
        '
        Me.DateBoxOrderDateFrom.AcceptsTab = True
        Me.DateBoxOrderDateFrom.BackColor = System.Drawing.Color.White
        Me.DateBoxOrderDateFrom.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.DateBoxOrderDateFrom.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.DateBoxOrderDateFrom.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.DateBoxOrderDateFrom.Location = New System.Drawing.Point(163, 16)
        Me.DateBoxOrderDateFrom.MaxLength = 10
        Me.DateBoxOrderDateFrom.Name = "DateBoxOrderDateFrom"
        Me.DateBoxOrderDateFrom.OldText = Nothing
        Me.DateBoxOrderDateFrom.Size = New System.Drawing.Size(100, 23)
        Me.DateBoxOrderDateFrom.TabIndex = 1
        Me.DateBoxOrderDateFrom.Text = "yyyy/MM/dd"
        Me.DateBoxOrderDateFrom.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(262, 16)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(38, 22)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "～"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'CheckBoxNotArrived
        '
        Me.CheckBoxNotArrived.AutoSize = True
        Me.CheckBoxNotArrived.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.CheckBoxNotArrived.Location = New System.Drawing.Point(615, 59)
        Me.CheckBoxNotArrived.Name = "CheckBoxNotArrived"
        Me.CheckBoxNotArrived.Size = New System.Drawing.Size(134, 20)
        Me.CheckBoxNotArrived.TabIndex = 4
        Me.CheckBoxNotArrived.Text = "未入荷のみ表示"
        Me.CheckBoxNotArrived.UseVisualStyleBackColor = True
        '
        'DateBoxFilterOrderDate
        '
        Me.DateBoxFilterOrderDate.AcceptsTab = True
        Me.DateBoxFilterOrderDate.BackColor = System.Drawing.Color.White
        Me.DateBoxFilterOrderDate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.DateBoxFilterOrderDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.DateBoxFilterOrderDate.ImeMode = System.Windows.Forms.ImeMode.Disable
        Me.DateBoxFilterOrderDate.Location = New System.Drawing.Point(63, 58)
        Me.DateBoxFilterOrderDate.MaxLength = 10
        Me.DateBoxFilterOrderDate.Name = "DateBoxFilterOrderDate"
        Me.DateBoxFilterOrderDate.OldText = Nothing
        Me.DateBoxFilterOrderDate.Size = New System.Drawing.Size(100, 23)
        Me.DateBoxFilterOrderDate.TabIndex = 5
        Me.DateBoxFilterOrderDate.Text = "yyyy/MM/dd"
        Me.DateBoxFilterOrderDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'TextBoxFilterSupplierName
        '
        Me.TextBoxFilterSupplierName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBoxFilterSupplierName.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TextBoxFilterSupplierName.ImeMode = System.Windows.Forms.ImeMode.Hiragana
        Me.TextBoxFilterSupplierName.Location = New System.Drawing.Point(261, 58)
        Me.TextBoxFilterSupplierName.MaxByteLength = 40
        Me.TextBoxFilterSupplierName.Name = "TextBoxFilterSupplierName"
        Me.TextBoxFilterSupplierName.OldText = Nothing
        Me.TextBoxFilterSupplierName.Size = New System.Drawing.Size(331, 23)
        Me.TextBoxFilterSupplierName.TabIndex = 6
        Me.TextBoxFilterSupplierName.Text = "１２３４５６７８９０１２３４５６７８９０"
        '
        'PanelStores
        '
        Me.PanelStores.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PanelStores.Location = New System.Drawing.Point(12, 81)
        Me.PanelStores.Name = "PanelStores"
        Me.PanelStores.Size = New System.Drawing.Size(1344, 778)
        Me.PanelStores.TabIndex = 7
        '
        'FormOrders
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CausesValidation = False
        Me.ClientSize = New System.Drawing.Size(1368, 871)
        Me.Name = "FormOrders"
        Me.PanelMain.ResumeLayout(False)
        Me.PanelDetail.ResumeLayout(False)
        Me.PanelDetail.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents PanelDetail As System.Windows.Forms.Panel
    Friend WithEvents TextBoxFilterSupplierName As TOA.ToaTextBox
    Friend WithEvents PanelStores As Panel
    Friend WithEvents DateBoxFilterOrderDate As TOA.ToaDateBox
    Friend WithEvents CheckBoxNotArrived As CheckBox
    Friend WithEvents DateBoxOrderDateTo As TOA.ToaDateBox
    Friend WithEvents Label3 As Label
    Friend WithEvents DateBoxOrderDateFrom As TOA.ToaDateBox
    Friend WithEvents Label1 As Label
    Friend WithEvents DateBoxFilterArrivalDate As TOA.ToaDateBox
End Class
