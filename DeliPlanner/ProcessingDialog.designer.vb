<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ProcessingDialog
    Inherits System.Windows.Forms.Form

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ProcessingDialog))
        Me.LabelMessage1 = New System.Windows.Forms.Label()
        Me.LabelMessage2 = New System.Windows.Forms.Label()
        Me.LabelProcess = New System.Windows.Forms.Label()
        Me.ProgressBarStatus = New System.Windows.Forms.ProgressBar()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'LabelMessage1
        '
        Me.LabelMessage1.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.LabelMessage1.ForeColor = System.Drawing.Color.Blue
        Me.LabelMessage1.Location = New System.Drawing.Point(121, 48)
        Me.LabelMessage1.Name = "LabelMessage1"
        Me.LabelMessage1.Size = New System.Drawing.Size(203, 23)
        Me.LabelMessage1.TabIndex = 0
        Me.LabelMessage1.Text = "ＮＮＮＮＮＮＮＮＮＮＮＮＮＮＮ"
        Me.LabelMessage1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'LabelMessage2
        '
        Me.LabelMessage2.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.LabelMessage2.ForeColor = System.Drawing.Color.Blue
        Me.LabelMessage2.Location = New System.Drawing.Point(121, 74)
        Me.LabelMessage2.Name = "LabelMessage2"
        Me.LabelMessage2.Size = New System.Drawing.Size(203, 23)
        Me.LabelMessage2.TabIndex = 1
        Me.LabelMessage2.Text = "ＮＮＮＮＮＮＮＮＮＮＮＮＮＮＮ"
        Me.LabelMessage2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'LabelProcess
        '
        Me.LabelProcess.BackColor = System.Drawing.Color.Black
        Me.LabelProcess.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.LabelProcess.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.LabelProcess.ForeColor = System.Drawing.Color.Yellow
        Me.LabelProcess.Location = New System.Drawing.Point(26, 15)
        Me.LabelProcess.Name = "LabelProcess"
        Me.LabelProcess.Size = New System.Drawing.Size(302, 23)
        Me.LabelProcess.TabIndex = 3
        Me.LabelProcess.Text = "処　理　中"
        Me.LabelProcess.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'ProgressBarStatus
        '
        Me.ProgressBarStatus.Location = New System.Drawing.Point(26, 106)
        Me.ProgressBarStatus.Name = "ProgressBarStatus"
        Me.ProgressBarStatus.Size = New System.Drawing.Size(302, 23)
        Me.ProgressBarStatus.TabIndex = 4
        Me.ProgressBarStatus.Visible = False
        '
        'PictureBox1
        '
        Me.PictureBox1.Location = New System.Drawing.Point(26, 35)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(89, 96)
        Me.PictureBox1.TabIndex = 5
        Me.PictureBox1.TabStop = False
        '
        'ProcessingDialog
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.ClientSize = New System.Drawing.Size(357, 144)
        Me.ControlBox = False
        Me.Controls.Add(Me.ProgressBarStatus)
        Me.Controls.Add(Me.LabelProcess)
        Me.Controls.Add(Me.LabelMessage2)
        Me.Controls.Add(Me.LabelMessage1)
        Me.Controls.Add(Me.PictureBox1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "ProcessingDialog"
        Me.Padding = New System.Windows.Forms.Padding(9, 8, 9, 8)
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents ProgressBarStatus As System.Windows.Forms.ProgressBar
    Private WithEvents LabelMessage1 As System.Windows.Forms.Label
    Private WithEvents LabelMessage2 As System.Windows.Forms.Label
    Private WithEvents LabelProcess As System.Windows.Forms.Label
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox

End Class
