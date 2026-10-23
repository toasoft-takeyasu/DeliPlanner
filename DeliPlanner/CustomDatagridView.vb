Imports TOA
Imports System.ComponentModel

Public Interface ISupportTabStop

    Property TabStop() As Boolean

End Interface

' フォーカスがあるセルに枠を描くための共通処理
Friend Module CellFocusFrame

    ''' <summary>フォーカス枠の太さ</summary>
    Private Const BORDER_WIDTH As Integer = 2

    ''' <summary>フォーカス枠の色</summary>
    Private ReadOnly BORDER_COLOR As Color = Color.FromArgb(0, 102, 204)

    ' セルの内側に枠を描く
    Friend Sub Draw(graphics As Graphics, cellBounds As Rectangle)

        If cellBounds.Width <= 6 OrElse cellBounds.Height <= 6 Then Return

        Dim rect As New Rectangle(cellBounds.X + 1, cellBounds.Y + 1,
                                  cellBounds.Width - 3, cellBounds.Height - 3)

        Using pen As New Pen(BORDER_COLOR, BORDER_WIDTH)
            pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset
            graphics.DrawRectangle(pen, rect)
        End Using

    End Sub

    ' 指定したセルにフォーカスがあるかどうか
    Friend Function IsFocused(dgv As DataGridView, columnIndex As Integer, rowIndex As Integer) As Boolean

        If dgv Is Nothing Then Return False
        If Not dgv.Focused Then Return False

        Dim address As Point = dgv.CurrentCellAddress
        Return address.X = columnIndex AndAlso address.Y = rowIndex

    End Function

End Module

Public Class CustomDataGridView
    Inherits ToaDataGridView

    Public Property IsRotate As Boolean = False '最後のセルの次に先頭のセルに戻るかどうか

    Public Enum EnterRow
        First
        Add
        None
    End Enum
    Public Property EnterRowPosition As EnterRow = EnterRow.None 'Enter時にどの行にフォーカスするか

    Private Sub Me_Enter(sender As Object, e As EventArgs) Handles Me.Enter

        Try
            If Me.Rows.Count = 0 Then Exit Sub

            Dim rowIndex As Integer = -1

            Select Case EnterRowPosition
                Case EnterRow.First
                    rowIndex = GetFirstRowIndex()
                Case EnterRow.Add
                    rowIndex = Me.NewRowIndex
                Case EnterRow.None
                    Exit Sub
            End Select

            If rowIndex < 0 Then Exit Sub

            Dim columnIndex As Integer = GetFirstColumnIndex()
            If columnIndex < 0 Then Exit Sub

            Me.FirstDisplayedScrollingRowIndex = rowIndex
            Me.CurrentCell = Me(columnIndex, rowIndex)
            '列の補正(TabStop = False のセルを飛ばす)は OnEnter が行う

        Catch ex As Exception
        End Try

    End Sub

    ' 直前にフォーカスがあったセル(枠を消すために覚えておく)
    Private _lastFocusCell As Point = New Point(-1, -1)

    Private Sub Me_CurrentCellChanged(sender As Object, e As EventArgs) Handles Me.CurrentCellChanged

        InvalidateFocusCell(_lastFocusCell)
        _lastFocusCell = Me.CurrentCellAddress
        InvalidateFocusCell(_lastFocusCell)

    End Sub

    Private Sub Me_FocusChanged(sender As Object, e As EventArgs) Handles Me.GotFocus, Me.LostFocus

        InvalidateFocusCell(Me.CurrentCellAddress)

    End Sub

    ' 指定したセルだけ描き直す
    Private Sub InvalidateFocusCell(address As Point)

        If address.X < 0 OrElse address.Y < 0 Then Return
        If address.X >= Me.Columns.Count OrElse address.Y >= Me.Rows.Count Then Return
        Me.InvalidateCell(address.X, address.Y)

    End Sub

    Private Sub Me_EnabledChanged(sender As Object, e As EventArgs) Handles Me.EnabledChanged

        If Not Me.Enabled Then
            Me.DefaultCellStyle.ForeColor = SystemColors.GrayText
            Me.ColumnHeadersDefaultCellStyle.BackColor = SystemColors.Control
            Me.ColumnHeadersDefaultCellStyle.ForeColor = SystemColors.GrayText
            Me.RowHeadersDefaultCellStyle.BackColor = SystemColors.Control
            Me.RowHeadersDefaultCellStyle.ForeColor = SystemColors.GrayText
        Else
            Me.DefaultCellStyle.ForeColor = SystemColors.ControlText
            Me.ColumnHeadersDefaultCellStyle.BackColor = SystemColors.Window
            Me.ColumnHeadersDefaultCellStyle.ForeColor = SystemColors.ControlText
            Me.RowHeadersDefaultCellStyle.BackColor = SystemColors.Window
            Me.RowHeadersDefaultCellStyle.ForeColor = SystemColors.ControlText
        End If

    End Sub

    ' チェックボックスセルにフォーカス枠を描く(標準のチェックボックスセルは見た目が変わらないため)
    Private Sub Me_CellPaintingCheckBox(sender As Object, e As DataGridViewCellPaintingEventArgs) Handles Me.CellPainting

        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then Return
        If Not TypeOf Me.Columns(e.ColumnIndex) Is DataGridViewCheckBoxColumn Then Return
        If Not CellFocusFrame.IsFocused(Me, e.ColumnIndex, e.RowIndex) Then Return

        '既定の描画をしてから、その上に枠を重ねる
        e.Paint(e.ClipBounds, e.PaintParts)
        CellFocusFrame.Draw(e.Graphics, e.CellBounds)
        e.Handled = True

    End Sub

    Public Sub NextCell()

        Me.TabToNextCell(Keys.Tab)

    End Sub

    Public Sub PreviousCell()

        Me.TabToPreviousCell(Keys.Shift Or Keys.Tab)

    End Sub

    Public Sub ScrollToBottom()

        If Me.RowCount <= 0 Then Return
        If Not Me.IsHandleCreated Then Return

        Me.FirstDisplayedScrollingRowIndex = Me.RowCount - 1

    End Sub

    Protected Overrides Sub OnCellValidating(e As DataGridViewCellValidatingEventArgs)

        MyBase.OnCellValidating(e)

    End Sub

#Region "Tab制御"

    Protected Overrides Function ProcessCmdKey(ByRef msg As System.Windows.Forms.Message, keyData As System.Windows.Forms.Keys) As Boolean

        Const WM_KEYDOWN As Integer = &H100

        If msg.Msg = WM_KEYDOWN Then

            Select Case keyData
                Case Keys.Tab, Keys.Enter
                    'カレントセルが無い(行が1件も無い)ときは検証せずに通常処理へ渡す
                    If Me.CurrentCell Is Nothing Then Return MyBase.ProcessCmdKey(msg, keyData)

                    ' セル移動前にCellValidatingイベントを発生させて値の検証を実施
                    ' ※これをしないとTabToNextCell内でCellValidatingイベントが無限ループとなる
                    Dim c As Reflection.ConstructorInfo = GetType(DataGridViewCellValidatingEventArgs).GetConstructors(Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic)(0)
                    Dim e As DataGridViewCellValidatingEventArgs =
                        DirectCast(c.Invoke(New Object() {Me.CurrentCell.ColumnIndex, Me.CurrentCell.RowIndex, CurrentCell.EditedFormattedValue}), DataGridViewCellValidatingEventArgs)
                    OnCellValidating(e)
                    If e.Cancel Then Return True

                    ' 移動できるセルが存在するか判定
                    If TabToNextCell(keyData) Then
                        Return True
                    Else
                        If Me.RowCount <> 0 Then
                            If IsRotate Then
                                Try
                                    ' 最初のセルに移動
                                    Me.CurrentCell = Me(GetFirstColumnIndex(), GetFirstRowIndex())
                                Catch ex As Exception
                                End Try
                                ' 最初のセルが TabStop = True かどうかを判定
                                If IsTabStopCell(Me.CurrentCellAddress) Then
                                    ' TabStop = True なら処理を抜ける
                                    Return True
                                Else
                                    ' TabStop = False なら TabStop = True のセルまで移動
                                    If TabToNextCell(keyData) Then
                                        Return True
                                    End If
                                End If
                            Else
                                Dim parentControl As Control = Me.Parent
                                Do While parentControl IsNot Nothing AndAlso TypeOf parentControl Is Panel
                                    parentControl = parentControl.Parent
                                Loop
                                If parentControl IsNot Nothing Then
                                    parentControl.SelectNextControl(Me, True, True, True, True)
                                End If
                            End If
                        End If
                        Return False
                    End If
                Case (Keys.Shift Or Keys.Tab), (Keys.Shift Or Keys.Enter)
                    'カレントセルが無い(行が1件も無い)ときは検証せずに通常処理へ渡す
                    If Me.CurrentCell Is Nothing Then Return MyBase.ProcessCmdKey(msg, keyData)

                    ' セル移動前にCellValidatingイベントを発生させて値の検証を実施
                    ' ※これをしないとTabToNextCell内でCellValidatingイベントが無限ループとなる
                    Dim c As Reflection.ConstructorInfo = GetType(DataGridViewCellValidatingEventArgs).GetConstructors(Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic)(0)
                    Dim e As DataGridViewCellValidatingEventArgs =
                        DirectCast(c.Invoke(New Object() {Me.CurrentCell.ColumnIndex, Me.CurrentCell.RowIndex, CurrentCell.EditedFormattedValue}), DataGridViewCellValidatingEventArgs)
                    OnCellValidating(e)
                    If e.Cancel Then Return True

                    ' 移動できるセルが存在するか判定
                    If TabToPreviousCell(keyData) Then
                        Return True
                    Else
                        If Me.RowCount <> 0 Then
                            If IsRotate Then
                                Try
                                    ' 最後のセルに移動
                                    Me.CurrentCell = Me(GetLastColumnIndex(), GetLastRowIndex())
                                Catch ex As Exception
                                End Try
                                ' 最後のセルが TabStop = True かどうかを判定
                                If IsTabStopCell(Me.CurrentCellAddress) Then
                                    ' TabStop = True なら処理を抜ける
                                    Return True
                                Else
                                    ' TabStop = False なら TabStop = True のセルまで移動
                                    If TabToPreviousCell(keyData) Then
                                        Return True
                                    End If
                                End If
                            End If
                        End If
                        Return False
                    End If
            End Select
        End If

        Return MyBase.ProcessCmdKey(msg, keyData)

    End Function

    Protected Overrides Sub OnEnter(e As EventArgs)

        '基底クラスがカレントセルを決めたあとに列を補正する
        MyBase.OnEnter(e)
        MoveToTabStopCell()

    End Sub

    ' カレントセルが TabStop = False なら、同じ行の TabStop = True のセルまで移す
    Private Sub MoveToTabStopCell()

        If Me.Rows.Count = 0 Then Return

        'マウスでクリックして入ってきたときは、クリックしたセルをそのまま使う
        If Control.MouseButtons <> MouseButtons.None Then Return

        Dim address As Point = Me.CurrentCellAddress
        If address.X < 0 OrElse address.Y < 0 Then Return
        If IsTabStopCell(address) Then Return

        Dim column As DataGridViewColumn = Me.Columns(address.X)
        Do
            column = Me.Columns.GetNextColumn(column,
                                              DataGridViewElementStates.Visible,
                                              DataGridViewElementStates.None)
            '移れるセルが無ければそのままにしておく
            If column Is Nothing Then Return
            If IsTabStopCell(New Point(column.Index, address.Y)) Then
                Me.CurrentCell = Me(column.Index, address.Y)
                Return
            End If
        Loop

    End Sub

    Private Function GetFirstColumnIndex() As Integer

        Dim column As DataGridViewColumn = Me.Columns.GetFirstColumn(DataGridViewElementStates.Visible)
        Dim index As Integer = If((column Is Nothing), -1, column.Index)
        Return index

    End Function

    Private Function GetFirstRowIndex() As Integer

        Return Me.Rows.GetFirstRow(DataGridViewElementStates.Visible)

    End Function

    Private Function GetLastColumnIndex() As Integer

        Dim column As DataGridViewColumn = Me.Columns.GetLastColumn(DataGridViewElementStates.Visible, DataGridViewElementStates.None)
        Dim index As Integer = If((column Is Nothing), -1, column.Index)
        Return index

    End Function

    Private Function GetLastRowIndex() As Integer

        Return Me.Rows.GetLastRow(DataGridViewElementStates.Visible)

    End Function

    Private Function GetNextTabPosition(currentCellAddress As Point) As Point

        Dim nextCellAddress As New Point(-1, -1)
        Dim nextColumnIndex As Integer = -1

        If Me.CurrentCellAddress.X <> -1 Then
            Dim column As DataGridViewColumn = Me.Columns.GetNextColumn(Me.Columns(currentCellAddress.X), DataGridViewElementStates.Visible, DataGridViewElementStates.None)
            If column IsNot Nothing Then
                nextColumnIndex = column.Index
            End If
        End If

        If nextColumnIndex = -1 Then
            nextCellAddress.X = GetFirstColumnIndex()
            nextCellAddress.Y = Me.Rows.GetNextRow(currentCellAddress.Y, DataGridViewElementStates.Visible)
            If nextCellAddress.Y = -1 Then
                nextCellAddress.Y = GetFirstRowIndex()
            End If
        Else
            nextCellAddress.X = nextColumnIndex
            nextCellAddress.Y = currentCellAddress.Y
        End If

        Return nextCellAddress

    End Function

    Private Function IsTabStopCell(cellAddress As Point) As Boolean

        '行や列が無いときは「移動先なし」として扱う
        If cellAddress.X < 0 OrElse cellAddress.Y < 0 Then Return False
        If cellAddress.Y >= Me.Rows.Count OrElse cellAddress.X >= Me.Columns.Count Then Return False

        Dim cell As ISupportTabStop = TryCast(Me.Rows(cellAddress.Y).Cells(cellAddress.X), ISupportTabStop)
        If cell Is Nothing Then Return True   'ISupportTabStop でないセルは常に止まる

        Return cell.TabStop

    End Function

    Private Function CanTabToNextCell() As Boolean

        Dim found As Boolean = False
        Dim currentCellAddress As Point = Me.CurrentCellAddress

        Do While (Not found)
            Dim nextCellAddress As Point = GetNextTabPosition(currentCellAddress)
            found = IsTabStopCell(nextCellAddress)
            If Not found AndAlso nextCellAddress.X <= currentCellAddress.X AndAlso nextCellAddress.Y <= currentCellAddress.Y Then
                Return False
            End If
            currentCellAddress = nextCellAddress
        Loop

        Return True

    End Function

    Private Function TabToNextCell(keyData As Keys) As Boolean

        If GetFirstColumnIndex() = -1 OrElse GetFirstRowIndex() = -1 Then Return False
        If Not CanTabToNextCell() Then Return False
        Return TabToCell(keyData)

    End Function

    Private Function GetPreviousTabPosition(currentCellAddress As Point) As Point

        Dim previousCellAddress As New Point(-1, -1)
        Dim previousColumnIndex As Integer = -1

        If Me.CurrentCellAddress.X <> -1 Then
            Dim column As DataGridViewColumn = Me.Columns.GetPreviousColumn(Me.Columns(currentCellAddress.X), DataGridViewElementStates.Visible, DataGridViewElementStates.None)
            If column IsNot Nothing Then
                previousColumnIndex = column.Index
            End If
        End If

        If previousColumnIndex = -1 Then
            previousCellAddress.X = GetLastColumnIndex()
            previousCellAddress.Y = Me.Rows.GetPreviousRow(currentCellAddress.Y, DataGridViewElementStates.Visible)
            If previousCellAddress.Y = -1 Then
                previousCellAddress.Y = GetLastRowIndex()
            End If
        Else
            previousCellAddress.X = previousColumnIndex
            previousCellAddress.Y = currentCellAddress.Y
        End If

        Return previousCellAddress

    End Function

    Private Function CanTabToPreviousCell() As Boolean

        Dim found As Boolean = False
        Dim currentCellAddress As Point = Me.CurrentCellAddress

        Do While (Not found)
            Dim previousCellAddress As Point = GetPreviousTabPosition(currentCellAddress)
            found = IsTabStopCell(previousCellAddress)
            If Not found AndAlso previousCellAddress.X >= currentCellAddress.X AndAlso previousCellAddress.Y >= currentCellAddress.Y Then
                Return False
            End If
            currentCellAddress = previousCellAddress
        Loop

        Return True

    End Function

    Private Function TabToPreviousCell(keyData As Keys) As Boolean

        If GetFirstColumnIndex() = -1 OrElse GetFirstRowIndex() = -1 Then Return False
        If Not CanTabToPreviousCell() Then Return False
        Return TabToCell(keyData)

    End Function

    Private Function TabToCell(keyData As Keys) As Boolean

        Dim found As Boolean = False
        Dim currentCellAddress As Point = Me.CurrentCellAddress

        Do While (Not found)
            Dim processed As Boolean = MyBase.ProcessTabKey(keyData)
            If Not processed Then
                Return False
            End If
            If TypeOf Me.CurrentCell Is ISupportTabStop Then
                Dim cell As ISupportTabStop = TryCast(Me.CurrentCell, ISupportTabStop)
                If cell.TabStop = True Then
                    Return True
                End If
            Else
                Return processed
            End If
        Loop

        Return False

    End Function

    Protected Overrides Function ProcessDataGridViewKey(e As KeyEventArgs) As Boolean

        Select Case e.KeyData
            Case Keys.Tab, Keys.Enter
                Return TabToNextCell(e.KeyData)
            Case (Keys.Shift Or Keys.Tab), (Keys.Shift Or Keys.Enter)
                Return TabToPreviousCell(e.KeyData)
        End Select

        Return MyBase.ProcessDataGridViewKey(e)

    End Function

#End Region

#Region "結合ヘッダー描画"

    '  複数列を結合して 1つの見出しを表示するためのグループ
    Private Class MergedHeaderGroup
        Public Property LeftColumn As DataGridViewColumn
        Public Property RightColumn As DataGridViewColumn
        Public Property Text As String
    End Class

    Private ReadOnly _mergedHeaders As New List(Of MergedHeaderGroup)
    Private ReadOnly _centerFormat As New StringFormat() With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Center}

    ' ===== 派生クラスから使う登録API =====

    ' 結合ヘッダー定義をクリア
    Protected Sub ClearMergedHeaders()
        _mergedHeaders.Clear()
    End Sub

    ' 結合ヘッダーを1つ追加(左列 + 右列 を結合して text を中央表示)
    Protected Sub AddMergedHeader(leftColumn As DataGridViewColumn, rightColumn As DataGridViewColumn, text As String)
        _mergedHeaders.Add(New MergedHeaderGroup With {
        .LeftColumn = leftColumn, .RightColumn = rightColumn, .Text = text})
    End Sub

    ' ===== 描画イベント(基底で一元処理) =====

    ' 結合対象の列ヘッダーは背景・枠線だけ描き、ラベルは描かない(見出しは Me_Paint で結合表示)
    Private Sub Me_CellPainting(sender As Object, e As DataGridViewCellPaintingEventArgs) Handles Me.CellPainting
        If _mergedHeaders.Count = 0 Then Return
        If e.RowIndex <> -1 OrElse e.ColumnIndex < 0 Then Return

        Dim col As DataGridViewColumn = Me.Columns(e.ColumnIndex)
        If Not IsMergedColumn(col) Then Return

        e.PaintBackground(e.CellBounds, True)
        e.Handled = True
    End Sub

    ' 見出しをまとめて描画
    Private Sub Me_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        If _mergedHeaders.Count = 0 Then Return
        DrawMergedHeaders(e.Graphics)
    End Sub

    Private Sub Me_Scroll(sender As Object, e As ScrollEventArgs) Handles Me.Scroll
        If e.ScrollOrientation = ScrollOrientation.HorizontalScroll Then InvalidateHeader()
    End Sub

    Private Sub Me_SizeChanged(sender As Object, e As EventArgs) Handles Me.SizeChanged
        InvalidateHeader()
    End Sub

    ' ===== 内部処理 =====

    Private Sub DrawMergedHeaders(g As Graphics)
        Dim headerHeight As Integer = Me.ColumnHeadersHeight
        Dim splitX As Integer = Me.FrozenRight

        DrawMergedHeaderRegion(g, New Rectangle(0, 0, splitX, headerHeight), drawFrozen:=True)
        DrawMergedHeaderRegion(g, New Rectangle(splitX, 0, Math.Max(0, Me.Width - splitX), headerHeight), drawFrozen:=False)
    End Sub

    Private Sub DrawMergedHeaderRegion(g As Graphics, clipRect As Rectangle, drawFrozen As Boolean)
        If clipRect.Width <= 0 Then Return

        Dim oldClip As Region = g.Clip

        Using backBrush As New SolidBrush(HeaderBackColor),
          foreBrush As New SolidBrush(HeaderForeColor),
          gridPen As New Pen(Me.GridColor),
          topPen As New Pen(Me.GridColor, 2)

            g.SetClip(clipRect)

            For Each group As MergedHeaderGroup In _mergedHeaders
                If IsGroupFrozen(group) <> drawFrozen Then Continue For

                Dim band As Rectangle = GetBandRect(group)
                If band.IsEmpty Then Continue For

                g.FillRectangle(backBrush, band)
                g.DrawString(group.Text, HeaderFont, foreBrush, band, _centerFormat)

                Dim xLeft As Integer = band.Left
                Dim xRight As Integer = band.Right - 1
                Dim yTop As Integer = band.Top
                Dim yBottom As Integer = band.Bottom - 1

                g.DrawLine(topPen, xLeft, yTop + 1, xRight, yTop + 1)
                g.DrawLine(gridPen, xRight, yTop, xRight, yBottom)
            Next

            g.Clip = oldClip
        End Using
    End Sub

    Private Function IsGroupFrozen(group As MergedHeaderGroup) As Boolean
        Return group.LeftColumn IsNot Nothing AndAlso group.LeftColumn.Frozen
    End Function

    Private Sub InvalidateHeader()
        Me.Invalidate(New Rectangle(0, 0, Me.Width, Me.ColumnHeadersHeight))
    End Sub

    Private Function IsMergedColumn(col As DataGridViewColumn) As Boolean
        For Each group As MergedHeaderGroup In _mergedHeaders
            If col Is group.LeftColumn OrElse col Is group.RightColumn Then Return True
        Next
        Return False
    End Function

    Private Function GetBandRect(group As MergedHeaderGroup) As Rectangle
        If group.LeftColumn Is Nothing OrElse group.RightColumn Is Nothing Then Return Rectangle.Empty
        If Not group.LeftColumn.Visible OrElse Not group.RightColumn.Visible Then Return Rectangle.Empty
        Dim lRect As Rectangle = Me.GetCellDisplayRectangle(group.LeftColumn.Index, -1, True)
        Dim rRect As Rectangle = Me.GetCellDisplayRectangle(group.RightColumn.Index, -1, True)
        If lRect.Width = 0 AndAlso rRect.Width = 0 Then Return Rectangle.Empty
        Dim bandLeft As Integer = If(lRect.Width > 0, lRect.Left, rRect.Left)
        Dim bandRight As Integer = If(rRect.Width > 0, rRect.Right, lRect.Right)
        Return New Rectangle(bandLeft, 0, bandRight - bandLeft, Me.ColumnHeadersHeight)
    End Function

    Private Function GetLastMergedRight() As Integer
        Dim result As Integer = FrozenRight
        For Each group As MergedHeaderGroup In _mergedHeaders
            If group.RightColumn IsNot Nothing AndAlso group.RightColumn.Visible Then
                Dim r As Rectangle = Me.GetCellDisplayRectangle(group.RightColumn.Index, -1, True)
                If r.Width > 0 Then result = Math.Max(result, r.Right)
            End If
        Next
        Return result
    End Function

    Private ReadOnly Property FrozenRight As Integer
        Get
            Dim x As Integer = If(Me.RowHeadersVisible, Me.RowHeadersWidth, 0)
            For Each c As DataGridViewColumn In Me.Columns
                If c.Visible AndAlso c.Frozen Then x += c.Width
            Next
            Return x
        End Get
    End Property

    Private ReadOnly Property HeaderFont As Font
        Get
            Return Me.ColumnHeadersDefaultCellStyle.Font
        End Get
    End Property

    Private ReadOnly Property HeaderBackColor As Color
        Get
            Dim c As Color = Me.ColumnHeadersDefaultCellStyle.BackColor
            Return If(c.IsEmpty, SystemColors.Control, c)
        End Get
    End Property

    Private ReadOnly Property HeaderForeColor As Color
        Get
            Dim c As Color = Me.ColumnHeadersDefaultCellStyle.ForeColor
            Return If(c.IsEmpty, SystemColors.ControlText, c)
        End Get
    End Property

#End Region

End Class

Public Class CustomDataGridViewTextBoxColumn
    Inherits DataGridViewTextBoxColumn

    Public Sub New()

        MyBase.New()
        MyBase.CellTemplate = New CustomDataGridViewTextBoxCell()

        '新しく追加したプロパティに初期値を設定する
        _NumericOnly = False
        _AllowSymbolChar = False
        _MaxByteLength = 65535
        _ImeMode = ImeMode.Disable

    End Sub

    Public Overrides Property CellTemplate() As DataGridViewCell

        Get
            Return MyBase.CellTemplate
        End Get
        Set(value As DataGridViewCell)
            'CustomDataGridViewTextBoxCellしかCellTemplateに設定できないようにする
            If Not (TypeOf value Is CustomDataGridViewTextBoxCell) Then
                Throw New InvalidCastException("CustomDataGridViewTextBoxCellオブジェクトを指定してください。")
            End If
            MyBase.CellTemplate = value
        End Set

    End Property

    Public Property NumericOnly() As Boolean

    Public Property AllowSymbolChar() As Boolean

    Public Property MaxByteLength() As Integer

    Public Property ImeMode() As ImeMode

    Public Property TabStop() As Boolean

        Get
            If customCellTemplate Is Nothing Then
                Throw New InvalidOperationException("CustomDataGridViewTextBoxColumnはCellTemplateがなくてはなりません。")
            End If
            Return customCellTemplate.TabStop
        End Get
        Set(value As Boolean)
            If customCellTemplate Is Nothing Then
                Throw New InvalidOperationException("CustomDataGridViewTextBoxColumnはCellTemplateがなくてはなりません。")
            End If
            customCellTemplate.TabStop = value
        End Set

    End Property

    Private ReadOnly Property customCellTemplate() As CustomDataGridViewTextBoxCell

        Get
            Return DirectCast(Me.CellTemplate, CustomDataGridViewTextBoxCell)
        End Get

    End Property

    Public Overrides Function Clone() As Object

        '新しいプロパティを追加しているため、Cloneメソッドをオーバーライドする必要がある
        Dim col As CustomDataGridViewTextBoxColumn = DirectCast(MyBase.Clone(), CustomDataGridViewTextBoxColumn)
        col.NumericOnly = Me.NumericOnly
        col.AllowSymbolChar = Me.AllowSymbolChar
        col.MaxByteLength = Me.MaxByteLength
        col.ImeMode = Me.ImeMode

        Return col

    End Function

End Class

Public Class CustomDataGridViewTextBoxCell
    Inherits DataGridViewTextBoxCell
    Implements ISupportTabStop

    Public Sub New()

        MyBase.New()
        _TabStop = True

    End Sub

    Public Overrides ReadOnly Property EditType() As Type

        Get
            Return GetType(CustomDataGridViewTextBoxEditingControl)
        End Get

    End Property

    Public Overrides ReadOnly Property ValueType() As Type

        Get
            Return MyBase.ValueType
        End Get

    End Property

    Public Overrides ReadOnly Property DefaultNewRowValue() As Object

        Get
            Return MyBase.DefaultNewRowValue
        End Get

    End Property

    Public Property TabStop() As Boolean Implements ISupportTabStop.TabStop

    Public Overrides Sub InitializeEditingControl(rowIndex As Integer, initialFormattedValue As Object, dataGridViewCellStyle As DataGridViewCellStyle)

        MyBase.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle)

        Dim customDataGridViewTextBoxEditingControl As CustomDataGridViewTextBoxEditingControl = TryCast(Me.DataGridView.EditingControl, CustomDataGridViewTextBoxEditingControl)
        If customDataGridViewTextBoxEditingControl IsNot Nothing Then
            Dim column As CustomDataGridViewTextBoxColumn = TryCast(Me.OwningColumn, CustomDataGridViewTextBoxColumn)
            If column IsNot Nothing Then
                customDataGridViewTextBoxEditingControl.NumericOnly = column.NumericOnly
                customDataGridViewTextBoxEditingControl.AllowSymbolChar = column.AllowSymbolChar
                customDataGridViewTextBoxEditingControl.MaxByteLength = column.MaxByteLength
                customDataGridViewTextBoxEditingControl.ImeMode = column.ImeMode
            End If
        End If

    End Sub

    Public Overrides Function Clone() As Object

        Dim cell As CustomDataGridViewTextBoxCell = TryCast(MyBase.Clone(), CustomDataGridViewTextBoxCell)
        If cell IsNot Nothing Then
            cell.TabStop = TabStop
        End If
        Return cell

    End Function

End Class

Public Class CustomDataGridViewTextBoxEditingControl
    Inherits ToaTextBox
    Implements IDataGridViewEditingControl

    Private dataGridView As DataGridView
    Private rowIndex As Integer
    Private valueChanged As Boolean

    Public Sub New()
    End Sub

#Region "IDataGridViewEditingControl メンバ"

    Public Function GetEditingControlFormattedValue(context As DataGridViewDataErrorContexts) As Object Implements IDataGridViewEditingControl.GetEditingControlFormattedValue

        Return Me.Text

    End Function

    Public Property EditingControlFormattedValue() As Object Implements IDataGridViewEditingControl.EditingControlFormattedValue

        Get
            Return Me.GetEditingControlFormattedValue(DataGridViewDataErrorContexts.Formatting)
        End Get
        Set(value As Object)
            Me.Text = DirectCast(value, String)
        End Set

    End Property

    Public Sub ApplyCellStyleToEditingControl(dataGridViewCellStyle As DataGridViewCellStyle) Implements IDataGridViewEditingControl.ApplyCellStyleToEditingControl

        Me.Font = dataGridViewCellStyle.Font
        Me.ForeColor = dataGridViewCellStyle.ForeColor
        Me.BackColor = dataGridViewCellStyle.BackColor
        Select Case dataGridViewCellStyle.Alignment
            Case DataGridViewContentAlignment.BottomCenter,
                 DataGridViewContentAlignment.MiddleCenter,
                 DataGridViewContentAlignment.TopCenter

                Me.TextAlign = HorizontalAlignment.Center
                Exit Select
            Case DataGridViewContentAlignment.BottomRight,
                 DataGridViewContentAlignment.MiddleRight,
                 DataGridViewContentAlignment.TopRight

                Me.TextAlign = HorizontalAlignment.Right
                Exit Select
            Case Else
                Me.TextAlign = HorizontalAlignment.Left
                Exit Select
        End Select

    End Sub

    Public Property EditingControlDataGridView() As DataGridView Implements IDataGridViewEditingControl.EditingControlDataGridView

        Get
            Return Me.dataGridView
        End Get
        Set(value As DataGridView)
            Me.dataGridView = value
        End Set

    End Property

    Public Property EditingControlRowIndex() As Integer Implements IDataGridViewEditingControl.EditingControlRowIndex

        Get
            Return Me.rowIndex
        End Get
        Set(value As Integer)
            Me.rowIndex = value
        End Set

    End Property

    Public Property EditingControlValueChanged() As Boolean Implements IDataGridViewEditingControl.EditingControlValueChanged

        Get
            Return Me.valueChanged
        End Get
        Set(value As Boolean)
            Me.valueChanged = value
        End Set

    End Property

    Public Function EditingControlWantsInputKey(keyData As Keys, dataGridViewWantsInputKey As Boolean) As Boolean Implements IDataGridViewEditingControl.EditingControlWantsInputKey

        Select Case keyData And Keys.KeyCode
            Case Keys.Right, Keys.[End], Keys.Left, Keys.Home
                Return True
            Case Else
                Return Not dataGridViewWantsInputKey
        End Select

    End Function

    Public ReadOnly Property EditingPanelCursor() As Cursor Implements IDataGridViewEditingControl.EditingPanelCursor

        Get
            Return MyBase.Cursor
        End Get

    End Property

    Public Sub PrepareEditingControlForEdit(selectAll As Boolean) Implements IDataGridViewEditingControl.PrepareEditingControlForEdit

        If selectAll Then
            Me.SelectAll()
        Else
            Me.SelectionStart = Me.TextLength
        End If

    End Sub

    Public ReadOnly Property RepositionEditingControlOnValueChange() As Boolean Implements IDataGridViewEditingControl.RepositionEditingControlOnValueChange

        Get
            Return False
        End Get

    End Property

#End Region

    Protected Overrides Sub OnTextChanged(e As EventArgs)

        MyBase.OnTextChanged(e)
        Me.valueChanged = True
        Me.dataGridView.NotifyCurrentCellDirty(True)

    End Sub

End Class

Public Class CustomDataGridViewDateBoxColumn
    Inherits DataGridViewTextBoxColumn

    Public Sub New()

        MyBase.New()
        MyBase.CellTemplate = New CustomDataGridViewDateBoxCell()

        '新しく追加したプロパティに初期値を設定する
        _Format = "yyyy/MM/dd"
        _ImeMode = ImeMode.Disable

    End Sub

    Public Overrides Property CellTemplate() As DataGridViewCell

        Get
            Return MyBase.CellTemplate
        End Get
        Set(value As DataGridViewCell)
            'CustomDataGridViewDateBoxCellしかCellTemplateに設定できないようにする
            If Not (TypeOf value Is CustomDataGridViewDateBoxCell) Then
                Throw New InvalidCastException("CustomDataGridViewDateBoxCellオブジェクトを指定してください。")
            End If
            MyBase.CellTemplate = value
        End Set

    End Property

    Public Property Format() As String

    Public Property ImeMode() As ImeMode

    Public Property TabStop() As Boolean

        Get
            If customCellTemplate Is Nothing Then
                Throw New InvalidOperationException("CustomDataGridViewDateBoxColumnはCellTemplateがなくてはなりません。")
            End If
            Return customCellTemplate.TabStop
        End Get
        Set(value As Boolean)
            If customCellTemplate Is Nothing Then
                Throw New InvalidOperationException("CustomDataGridViewDateBoxColumnはCellTemplateがなくてはなりません。")
            End If
            customCellTemplate.TabStop = value
        End Set

    End Property

    Private ReadOnly Property customCellTemplate() As CustomDataGridViewDateBoxCell

        Get
            Return DirectCast(Me.CellTemplate, CustomDataGridViewDateBoxCell)
        End Get

    End Property

    Public Overrides Function Clone() As Object

        '新しいプロパティを追加しているため、Cloneメソッドをオーバーライドする必要がある
        Dim col As CustomDataGridViewDateBoxColumn = DirectCast(MyBase.Clone(), CustomDataGridViewDateBoxColumn)
        col.Format = Me.Format
        col.ImeMode = Me.ImeMode

        Return col

    End Function

End Class

Public Class CustomDataGridViewDateBoxCell
    Inherits DataGridViewTextBoxCell
    Implements ISupportTabStop

    Public Sub New()

        MyBase.New()
        _TabStop = True

    End Sub

    Public Function ErrorStatus() As ToaDateBox.ErrorStatusType

        Dim result As ToaDateBox.ErrorStatusType

        If Not IsNothing(Me.DataGridView.EditingControl) Then
            result = TryCast(Me.DataGridView.EditingControl, CustomDataGridViewDateBoxEditingControl).CheckValue()
        End If

        Return result

    End Function

    Public Overrides ReadOnly Property EditType() As Type

        Get
            Return GetType(CustomDataGridViewDateBoxEditingControl)
        End Get

    End Property

    Public Overrides ReadOnly Property ValueType() As Type

        Get
            Return MyBase.ValueType
        End Get

    End Property

    Public Overrides ReadOnly Property DefaultNewRowValue() As Object

        Get
            Return MyBase.DefaultNewRowValue
        End Get

    End Property

    Public Property TabStop() As Boolean Implements ISupportTabStop.TabStop

    Public Overrides Sub InitializeEditingControl(rowIndex As Integer, initialFormattedValue As Object, dataGridViewCellStyle As DataGridViewCellStyle)

        MyBase.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle)

        Dim customDataGridViewDateBoxEditingControl As CustomDataGridViewDateBoxEditingControl = TryCast(Me.DataGridView.EditingControl, CustomDataGridViewDateBoxEditingControl)
        If customDataGridViewDateBoxEditingControl IsNot Nothing Then
            Dim column As CustomDataGridViewDateBoxColumn = TryCast(Me.OwningColumn, CustomDataGridViewDateBoxColumn)
            If column IsNot Nothing Then
                customDataGridViewDateBoxEditingControl.ImeMode = column.ImeMode
            End If
        End If

    End Sub

    Public Overrides Function Clone() As Object

        Dim cell As CustomDataGridViewDateBoxCell = TryCast(MyBase.Clone(), CustomDataGridViewDateBoxCell)
        If cell IsNot Nothing Then
            cell.TabStop = TabStop
        End If
        Return cell

    End Function

End Class

Public Class CustomDataGridViewDateBoxEditingControl
    Inherits ToaDateBox
    Implements IDataGridViewEditingControl

    Private dataGridView As DataGridView
    Private rowIndex As Integer
    Private valueChanged As Boolean

    Public Sub New()
    End Sub

#Region "IDataGridViewEditingControl メンバ"

    Public Function GetEditingControlFormattedValue(context As DataGridViewDataErrorContexts) As Object Implements IDataGridViewEditingControl.GetEditingControlFormattedValue

        Return Me.Text

    End Function

    Public Property EditingControlFormattedValue() As Object Implements IDataGridViewEditingControl.EditingControlFormattedValue

        Get
            Return Me.GetEditingControlFormattedValue(DataGridViewDataErrorContexts.Formatting)
        End Get
        Set(value As Object)
            Me.Text = DirectCast(value, String)
        End Set

    End Property

    Public Sub ApplyCellStyleToEditingControl(dataGridViewCellStyle As DataGridViewCellStyle) Implements IDataGridViewEditingControl.ApplyCellStyleToEditingControl

        Me.Font = dataGridViewCellStyle.Font
        Me.ForeColor = dataGridViewCellStyle.ForeColor
        Me.BackColor = dataGridViewCellStyle.BackColor
        Select Case dataGridViewCellStyle.Alignment
            Case DataGridViewContentAlignment.BottomCenter,
                 DataGridViewContentAlignment.MiddleCenter,
                 DataGridViewContentAlignment.TopCenter

                Me.TextAlign = HorizontalAlignment.Center
                Exit Select
            Case DataGridViewContentAlignment.BottomRight,
                 DataGridViewContentAlignment.MiddleRight,
                 DataGridViewContentAlignment.TopRight

                Me.TextAlign = HorizontalAlignment.Right
                Exit Select
            Case Else
                Me.TextAlign = HorizontalAlignment.Left
                Exit Select
        End Select

    End Sub

    Public Property EditingControlDataGridView() As DataGridView Implements IDataGridViewEditingControl.EditingControlDataGridView

        Get
            Return Me.dataGridView
        End Get
        Set(value As DataGridView)
            Me.dataGridView = value
        End Set

    End Property

    Public Property EditingControlRowIndex() As Integer Implements IDataGridViewEditingControl.EditingControlRowIndex

        Get
            Return Me.rowIndex
        End Get
        Set(value As Integer)
            Me.rowIndex = value
        End Set

    End Property

    Public Property EditingControlValueChanged() As Boolean Implements IDataGridViewEditingControl.EditingControlValueChanged

        Get
            Return Me.valueChanged
        End Get
        Set(value As Boolean)
            Me.valueChanged = value
        End Set

    End Property

    Public Function EditingControlWantsInputKey(keyData As Keys, dataGridViewWantsInputKey As Boolean) As Boolean Implements IDataGridViewEditingControl.EditingControlWantsInputKey

        Select Case keyData And Keys.KeyCode
            Case Keys.Right, Keys.[End], Keys.Left, Keys.Home
                Return True
            Case Else
                Return Not dataGridViewWantsInputKey
        End Select

    End Function

    Public ReadOnly Property EditingPanelCursor() As Cursor Implements IDataGridViewEditingControl.EditingPanelCursor

        Get
            Return MyBase.Cursor
        End Get

    End Property

    Public Sub PrepareEditingControlForEdit(selectAll As Boolean) Implements IDataGridViewEditingControl.PrepareEditingControlForEdit

        If selectAll Then
            Me.SelectAll()
        Else
            Me.SelectionStart = Me.TextLength
        End If

    End Sub

    Public ReadOnly Property RepositionEditingControlOnValueChange() As Boolean Implements IDataGridViewEditingControl.RepositionEditingControlOnValueChange

        Get
            Return False
        End Get

    End Property

#End Region

    Protected Overrides Sub OnTextChanged(e As EventArgs)

        MyBase.OnTextChanged(e)
        Me.valueChanged = True
        Me.dataGridView.NotifyCurrentCellDirty(True)

    End Sub

End Class

Public Class CustomDataGridViewComboBoxColumn
    Inherits DataGridViewComboBoxColumn

    Public Sub New()

        MyBase.New()
        MyBase.CellTemplate = New CustomDataGridViewComboBoxCell()

    End Sub

    Public Overrides Property CellTemplate() As DataGridViewCell

        Get
            Return MyBase.CellTemplate
        End Get
        Set(value As DataGridViewCell)
            'CustomDataGridViewComboBoxCellしかCellTemplateに設定できないようにする
            If Not (TypeOf value Is CustomDataGridViewComboBoxCell) Then
                Throw New InvalidCastException("CustomDataGridViewComboBoxCellオブジェクトを指定してください。")
            End If
            MyBase.CellTemplate = value
        End Set

    End Property

    Public Property TabStop() As Boolean

        Get
            If customCellTemplate Is Nothing Then
                Throw New InvalidOperationException("CustomDataGridViewComboBoxColumnはCellTemplateがなくてはなりません。")
            End If
            Return customCellTemplate.TabStop
        End Get
        Set(value As Boolean)
            If customCellTemplate Is Nothing Then
                Throw New InvalidOperationException("CustomDataGridViewComboBoxColumnはCellTemplateがなくてはなりません。")
            End If
            customCellTemplate.TabStop = value
        End Set

    End Property

    Private ReadOnly Property customCellTemplate() As CustomDataGridViewComboBoxCell

        Get
            Return DirectCast(Me.CellTemplate, CustomDataGridViewComboBoxCell)
        End Get

    End Property

End Class

Public Class CustomDataGridViewComboBoxCell
    Inherits DataGridViewComboBoxCell
    Implements ISupportTabStop

    Public Sub New()

        MyBase.New()
        _TabStop = True

    End Sub

    Public Overrides ReadOnly Property EditType() As Type

        Get
            Return GetType(CustomDataGridViewComboBoxEditingControl)
        End Get

    End Property

    Public Overrides ReadOnly Property ValueType() As Type

        Get
            Return GetType(Object)
        End Get

    End Property

    Public Overrides ReadOnly Property DefaultNewRowValue() As Object

        Get
            Return MyBase.DefaultNewRowValue
        End Get

    End Property

    Public Property TabStop() As Boolean Implements ISupportTabStop.TabStop

    Public Overrides Sub InitializeEditingControl(rowIndex As Integer, initialFormattedValue As Object, dataGridViewCellStyle As DataGridViewCellStyle)

        MyBase.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle)

    End Sub

    Public Overrides Function Clone() As Object

        Dim cell As CustomDataGridViewComboBoxCell = TryCast(MyBase.Clone(), CustomDataGridViewComboBoxCell)
        If cell IsNot Nothing Then
            cell.TabStop = TabStop
        End If
        Return cell

    End Function

End Class

Public Class CustomDataGridViewComboBoxEditingControl
    Inherits DataGridViewComboBoxEditingControl

End Class

Public Class CustomDataGridViewRadioButtonColumn
    Inherits DataGridViewColumn

    ''' <summary>ラジオボタンが属するグループ名</summary>
    Private _groupName As String

    ''' <summary>
    ''' 
    ''' </summary>
    Public Sub New()
        MyBase.New(New DataGridViewRadioButtonCell)
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Function Clone() As Object

        Dim col As CustomDataGridViewRadioButtonColumn = DirectCast(MyBase.Clone(), CustomDataGridViewRadioButtonColumn)

        ' プロパティを追加したとき、この場所に追記する
        ' Cloneメソッドに記述しない場合、プロパティブラウザで表示・編集は
        ' されるが、保存されないプロパティとなる
        col.GroupName = Me.GroupName

        Return col
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    Public Overrides Property CellTemplate() As DataGridViewCell
        Get
            Return MyBase.CellTemplate
        End Get
        Set(value As DataGridViewCell)
            If Not (value Is Nothing) AndAlso Not value.GetType().IsAssignableFrom(GetType(DataGridViewRadioButtonCell)) Then
                Throw New InvalidCastException("Must be a DataGridViewRadioButtonCell")
            End If
            MyBase.CellTemplate = value
        End Set
    End Property

    ''' <summary>
    ''' ラジオボタンが属するグループ名
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>
    ''' ここで設定されたグループ名により、同一行内・同一グループのチェック
    ''' 状態のラジオボタンがひとつになるよう制御される</remarks>
    <Browsable(True), DefaultValue(GetType(System.String), "defaultGroup")>
    Public Property GroupName As String
        Get
            Return _groupName
        End Get
        Set(value As String)
            _groupName = value
        End Set
    End Property
End Class

Public Class CustomDatagridViewButtonColumn
    Inherits DataGridViewButtonColumn

    Public Sub New()

        MyBase.New()
        MyBase.CellTemplate = New CustomDataGridViewButtonCell()

    End Sub

    Public Overrides Property CellTemplate() As DataGridViewCell

        Get
            Return MyBase.CellTemplate
        End Get
        Set(value As DataGridViewCell)
            'CustomDataGridViewButtonCellしかCellTemplateに設定できないようにする
            If Not (TypeOf value Is CustomDataGridViewButtonCell) Then
                Throw New InvalidCastException("CustomDataGridViewButtonCellオブジェクトを指定してください。")
            End If
            MyBase.CellTemplate = value
        End Set

    End Property

    Public Property TabStop() As Boolean

        Get
            If customCellTemplate Is Nothing Then
                Throw New InvalidOperationException("CustomDatagridViewButtonColumnはCellTemplateがなくてはなりません。")
            End If
            Return customCellTemplate.TabStop
        End Get
        Set(value As Boolean)
            If customCellTemplate Is Nothing Then
                Throw New InvalidOperationException("CustomDatagridViewButtonColumnはCellTemplateがなくてはなりません。")
            End If
            customCellTemplate.TabStop = value
        End Set

    End Property

    Private ReadOnly Property customCellTemplate() As CustomDataGridViewButtonCell

        Get
            Return DirectCast(Me.CellTemplate, CustomDataGridViewButtonCell)
        End Get

    End Property

End Class

Public Class CustomDataGridViewButtonCell
    Inherits DataGridViewButtonCell
    Implements ISupportTabStop

    Public Sub New()

        MyBase.New()
        _TabStop = True

    End Sub

    Public Overrides ReadOnly Property ValueType() As Type

        Get
            Return GetType(Object)
        End Get

    End Property

    Public Overrides ReadOnly Property DefaultNewRowValue() As Object

        Get
            Return MyBase.DefaultNewRowValue
        End Get

    End Property

    Public Property TabStop() As Boolean Implements ISupportTabStop.TabStop

    Public Overrides Function Clone() As Object

        Dim cell As CustomDataGridViewButtonCell = TryCast(MyBase.Clone(), CustomDataGridViewButtonCell)
        If cell IsNot Nothing Then
            cell.TabStop = TabStop
        End If
        Return cell

    End Function

    Protected Overrides Sub Paint(graphics As System.Drawing.Graphics,
                                  clipBounds As System.Drawing.Rectangle,
                                  cellBounds As System.Drawing.Rectangle,
                                  rowIndex As Integer,
                                  elementState As System.Windows.Forms.DataGridViewElementStates,
                                  value As Object,
                                  formattedValue As Object,
                                  errorText As String,
                                  cellStyle As System.Windows.Forms.DataGridViewCellStyle,
                                  advancedBorderStyle As System.Windows.Forms.DataGridViewAdvancedBorderStyle,
                                  paintParts As System.Windows.Forms.DataGridViewPaintParts)

        MyBase.Paint(graphics, clipBounds, cellBounds, rowIndex, elementState, value,
                     formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts)

        'フォーカスがあるときだけ枠を描く
        If Not CellFocusFrame.IsFocused(Me.DataGridView, Me.ColumnIndex, rowIndex) Then Return
        CellFocusFrame.Draw(graphics, cellBounds)

    End Sub

End Class

Public Class DataGridViewRadioButtonCell
    Inherits DataGridViewCell

    ''' <summary>ラジオボタンの描画サイズ</summary>
    Private Const RADIO_SIZE As Integer = 16

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        MyBase.New()
    End Sub

    ''' <summary>
    ''' 1
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function Clone() As Object
        Dim cell = DirectCast(MyBase.Clone(), DataGridViewRadioButtonCell)
        Return cell
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="value"></param>
    ''' <param name="rowIndex"></param>
    ''' <param name="cellStyle"></param>
    ''' <param name="valueTypeConverter"></param>
    ''' <param name="formattedValueTypeConverter"></param>
    ''' <param name="context"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overrides Function GetFormattedValue(value As Object, rowIndex As Integer, ByRef cellStyle As DataGridViewCellStyle, valueTypeConverter As TypeConverter, formattedValueTypeConverter As TypeConverter, context As DataGridViewDataErrorContexts) As Object
        Return _val(value)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="v"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function _val(v As Object) As Boolean
        Try
            Return If(IsDBNull(v) OrElse IsNothing(v), False, Convert.ToBoolean(v))
        Catch ex As Exception
            Return False
        End Try
    End Function
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="graphics"></param>
    ''' <param name="clipBounds"></param>
    ''' <param name="cellBounds"></param>
    ''' <param name="rowIndex"></param>
    ''' <param name="elementState"></param>
    ''' <param name="value"></param>
    ''' <param name="formattedValue"></param>
    ''' <param name="errorText"></param>
    ''' <param name="cellStyle"></param>
    ''' <param name="advancedBorderStyle"></param>
    ''' <param name="paintParts"></param>
    ''' <remarks></remarks>
    Protected Overrides Sub Paint(graphics As System.Drawing.Graphics,
                                  clipBounds As System.Drawing.Rectangle,
                                  cellBounds As System.Drawing.Rectangle,
                                  rowIndex As Integer,
                                  elementState As System.Windows.Forms.DataGridViewElementStates,
                                  value As Object,
                                  formattedValue As Object,
                                  errorText As String,
                                  cellStyle As System.Windows.Forms.DataGridViewCellStyle,
                                  advancedBorderStyle As System.Windows.Forms.DataGridViewAdvancedBorderStyle,
                                  paintParts As System.Windows.Forms.DataGridViewPaintParts)

        MyBase.Paint(graphics, clipBounds, cellBounds, rowIndex, elementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts)

        ' 背景色の描画
        If (paintParts And DataGridViewPaintParts.Background) = DataGridViewPaintParts.Background Then
            Dim brush As New SolidBrush(If(elementState And DataGridViewElementStates.Selected, cellStyle.SelectionBackColor, cellStyle.BackColor))
            graphics.FillRectangle(brush, cellBounds)
            brush.Dispose()
        End If

        ' 境界線の描画
        If (paintParts And DataGridViewPaintParts.Border) = DataGridViewPaintParts.Border Then
            PaintBorder(graphics, clipBounds, cellBounds, cellStyle, advancedBorderStyle)
        End If

        ' セルの幅、高さがラジオボタンの描画サイズより小さい場合は、描画処理を飛ばす
        If cellBounds.Width < RADIO_SIZE OrElse cellBounds.Height < RADIO_SIZE Then
            Return
        End If

        ' ラジオボタンをセルの真ん中に描画(描画サイズは固定)
        Dim rect As Rectangle = New Rectangle(New Point(cellBounds.Left + (cellBounds.Width - RADIO_SIZE) / 2,
                                                        cellBounds.Top + (cellBounds.Height - RADIO_SIZE) / 2),
                                                        New Size(RADIO_SIZE, RADIO_SIZE))


        Dim v = _val(value)
        ControlPaint.DrawRadioButton(graphics, rect, If(v, ButtonState.Checked, ButtonState.Inactive))

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="e"></param>
    ''' <param name="rowIndex"></param>
    ''' <remarks></remarks>
    Protected Overrides Sub OnKeyPress(e As System.Windows.Forms.KeyPressEventArgs, rowIndex As Integer)

        MyBase.OnKeyPress(e, rowIndex)

        If [ReadOnly] Then
            Return
        End If

        ' スペースキーが押されたとき、ラジオボタンのチェック状態の更新
        If e.KeyChar = Microsoft.VisualBasic.ChrW(Keys.Space) Then
            updateValues(DataGridView.CurrentCell.ColumnIndex, rowIndex)
        End If

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Overrides Sub OnClick(e As System.Windows.Forms.DataGridViewCellEventArgs)

        MyBase.OnClick(e)

        If [ReadOnly] Then
            Return
        End If

        ' ラジオボタンのチェック状態の更新
        updateValues(e.ColumnIndex, e.RowIndex)

    End Sub

    ''' <summary>
    ''' ラジオボタンのチェック状態を更新
    ''' </summary>
    ''' <param name="columnIndex"></param>
    ''' <param name="rowIndex"></param>
    ''' <remarks></remarks>
    Private Sub updateValues(columnIndex As Integer, rowIndex As Integer)

        Dim currentColumn As CustomDataGridViewRadioButtonColumn = TryCast(Me.DataGridView.Columns(columnIndex), CustomDataGridViewRadioButtonColumn)

        For Each column As DataGridViewColumn In Me.DataGridView.Columns

            ' ラジオボタンセル以外は処理を飛ばす
            If Not (TypeOf (column) Is CustomDataGridViewRadioButtonColumn) Then
                Continue For
            End If

            ' カレントセルと異なるグループ名は処理を飛ばす
            Dim radioColumn As CustomDataGridViewRadioButtonColumn = TryCast(column, CustomDataGridViewRadioButtonColumn)
            If radioColumn.GroupName <> currentColumn.GroupName Then
                Continue For
            End If

            Dim dgvc = Me.DataGridView.Rows(rowIndex).Cells(column.Index)

            ' 更新後のチェック状態を求める
            Dim v = If(radioColumn.Index = currentColumn.Index, True, False)

            ' 値の更新が生じない場合は処理を飛ばす
            Dim prev = _val(dgvc.Value)
            If prev = v Then
                Continue For
            End If

            dgvc.Value = v

            ' イベントを発生させる
            ' ※チェック状態を更新した回数分イベントが発生する
            Dim newEa = New DataGridViewCellEventArgs(columnIndex, rowIndex)
            RaiseCellValueChanged(newEa)

            Me.DataGridView.InvalidateCell(dgvc)
            Me.DataGridView.NotifyCurrentCellDirty(True)
        Next

    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides ReadOnly Property DefaultNewRowValue As Object
        Get
            Return False
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Property ValueType As System.Type
        Get
            Return GetType(Boolean)
        End Get
        Set(value As System.Type)
            Throw New InvalidOperationException
        End Set
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides ReadOnly Property FormattedValueType As Type
        Get
            Return GetType(Boolean)
        End Get
    End Property

End Class

