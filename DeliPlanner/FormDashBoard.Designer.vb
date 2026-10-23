<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormDashboard
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
        Me.panelHeader = New System.Windows.Forms.Panel()
        Me.tableTiles = New System.Windows.Forms.TableLayoutPanel()
        Me.labelUpdated = New System.Windows.Forms.Label()
        Me.TableMain = New System.Windows.Forms.TableLayoutPanel()
        Me.PanelMain.SuspendLayout()
        Me.panelHeader.SuspendLayout()
        Me.SuspendLayout()
        '
        'PanelMain
        '
        Me.PanelMain.Controls.Add(Me.TableMain)
        Me.PanelMain.Controls.Add(Me.panelHeader)
        Me.PanelMain.Size = New System.Drawing.Size(1368, 871)
        '
        'panelHeader
        '
        Me.panelHeader.BackColor = System.Drawing.Color.Transparent
        Me.panelHeader.Controls.Add(Me.tableTiles)
        Me.panelHeader.Controls.Add(Me.labelUpdated)
        Me.panelHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.panelHeader.Location = New System.Drawing.Point(0, 0)
        Me.panelHeader.Name = "panelHeader"
        Me.panelHeader.Padding = New System.Windows.Forms.Padding(12, 8, 12, 0)
        Me.panelHeader.Size = New System.Drawing.Size(1368, 170)
        Me.panelHeader.TabIndex = 0
        '
        'tableTiles
        '
        Me.tableTiles.ColumnCount = 4
        Me.tableTiles.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.tableTiles.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.tableTiles.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.tableTiles.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.tableTiles.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.tableTiles.Location = New System.Drawing.Point(12, 60)
        Me.tableTiles.Name = "tableTiles"
        Me.tableTiles.RowCount = 1
        Me.tableTiles.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.tableTiles.Size = New System.Drawing.Size(1344, 110)
        Me.tableTiles.TabIndex = 2
        '
        'labelUpdated
        '
        Me.labelUpdated.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.labelUpdated.Font = New System.Drawing.Font("MS UI Gothic", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.labelUpdated.ForeColor = System.Drawing.Color.DimGray
        Me.labelUpdated.Location = New System.Drawing.Point(1019, 19)
        Me.labelUpdated.Name = "labelUpdated"
        Me.labelUpdated.Size = New System.Drawing.Size(337, 21)
        Me.labelUpdated.TabIndex = 0
        Me.labelUpdated.Text = "labelUpdated"
        Me.labelUpdated.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'TableMain
        '
        Me.TableMain.ColumnCount = 2
        Me.TableMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableMain.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableMain.Location = New System.Drawing.Point(0, 170)
        Me.TableMain.Name = "TableMain"
        Me.TableMain.Padding = New System.Windows.Forms.Padding(12, 0, 12, 12)
        Me.TableMain.RowCount = 1
        Me.TableMain.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableMain.Size = New System.Drawing.Size(1368, 701)
        Me.TableMain.TabIndex = 0
        '
        'FormDashboard
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CausesValidation = False
        Me.ClientSize = New System.Drawing.Size(1368, 871)
        Me.Name = "FormDashboard"
        Me.Text = "FormDashBoard"
        Me.PanelMain.ResumeLayout(False)
        Me.panelHeader.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents panelHeader As Panel
    Friend WithEvents TableMain As TableLayoutPanel
    Friend WithEvents labelUpdated As Label
    Friend WithEvents tableTiles As TableLayoutPanel
End Class
