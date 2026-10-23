Imports Npgsql
Imports TOA.VariousLibrary

Public Class BatchProcess

    Private _parentForm As Form
    Private _clickedButton As Button

    Public Sub New(ByVal parentForm As Form, Optional ByVal clickedButton As Button = Nothing)

        _parentForm = parentForm
        _clickedButton = clickedButton

    End Sub

    Public Sub TransCustomer(Optional ByVal customerCode As String = Nothing)

        Dim processStart As Boolean = True
        ' 確認ダイアログを表示
        If String.IsNullOrEmpty(customerCode) Then
            processStart = MessageBox.Show("得意先マスタの転送を実行します。本当によろしいですか？", _clickedButton.Text,
                                           MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.Yes
        End If

        If processStart Then
            Try
                _parentForm.Cursor = Cursors.WaitCursor

                Using dlg As New ProcessingDialog
                    ' 処理中ダイアログ表示
                    dlg.Show(_parentForm)
                    dlg.ProcessName = "処　　理　　中"
                    dlg.Message1 = "しばらくお待ちください。"
                    _parentForm.Refresh()

                    Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
                        cn.Open()
                        Try
                            Dim customerList As New List(Of PCA.BEMasterTms)

                            Using cmd As NpgsqlCommand = cn.CreateCommand()
                                Dim count As Integer

                                Dim SQLString As String
                                SQLString = "Select * from Customers"
                                SQLString &= " where SendFlag = 0"
                                If Not String.IsNullOrEmpty(customerCode) Then
                                    SQLString &= " and CustomerCode = @CustomerCode"
                                End If
                                SQLString &= " order by CustomerCode"
                                cmd.CommandText = SQLString
                                cmd.Parameters.Clear()
                                If Not String.IsNullOrEmpty(customerCode) Then
                                    cmd.Parameters.AddWithValue("@CustomerCode", CInt(customerCode))
                                End If

                                Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                                    While dr.Read
                                        Dim tms As New PCA.BEMasterTms
                                        tms.TokuisakiCode = dr("CustomerCode").ToString
                                        tms.TokuisakiMei1 = dr("CustomerName").ToString
                                        tms.TokuisakiKubun1 = dr("AreaCode").ToString
                                        tms.TokuisakiKubun2 = dr("OccupationCode").ToString
                                        tms.TokuisakiKubun3 = dr("CustomerType").ToString
                                        tms.TokuisakiKubun4 = dr("HobbyCode").ToString
                                        tms.YubinBango = dr("ZipCode").ToString
                                        tms.Jyusyo1 = dr("Address").ToString
                                        tms.Jyusyo2 = dr("AddressBuilding").ToString
                                        tms.AitesakiTelNo = dr("TelNo").ToString
                                        tms.Syutantosya = dr("SalesStaffCode").ToString
                                        customerList.Add(tms)

                                        count += 1
                                    End While
                                End Using
                                If count > 0 Then
                                    dlg.ProgressBar.Visible = True
                                    dlg.ProgressBar.Maximum = count
                                Else
                                    MessageBox.Show("転送する得意先はありません。", _clickedButton.Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
                                    Exit Sub
                                End If

                                Dim logFile As New LogWriter(sharedIniFile.LogPath, True)
                                logFile.AddTimeStamp = True

                                For Each customer As PCA.BEMasterTms In customerList
                                    Dim resultLine As String = "得意先コード：" & customer.TokuisakiCode & " / " & "得意先名：" & customer.TokuisakiMei1

                                    Dim masterTms As PCA.BEMasterTms = (New PCA.MasterTms).GetByCode(customer.TokuisakiCode)
                                    Dim result As New PCA.BEKonIntegrationResultOfBEMasterTms
                                    If IsNothing(masterTms) Then
                                        resultLine = "【登録】" & resultLine & vbTab & "結果："

                                        masterTms = New PCA.BEMasterTms
                                        masterTms.TokuisakiCode = customer.TokuisakiCode
                                        masterTms.TokuisakiMei1 = customer.TokuisakiMei1
                                        masterTms.TokuisakiKubun1 = customer.TokuisakiKubun1
                                        masterTms.TokuisakiKubun2 = customer.TokuisakiKubun2
                                        masterTms.TokuisakiKubun3 = customer.TokuisakiKubun3
                                        masterTms.TokuisakiKubun4 = customer.TokuisakiKubun4
                                        masterTms.YubinBango = customer.YubinBango
                                        masterTms.Jyusyo1 = customer.Jyusyo1
                                        masterTms.Jyusyo2 = customer.Jyusyo2
                                        masterTms.AitesakiTelNo = customer.AitesakiTelNo
                                        masterTms.Syutantosya = customer.Syutantosya
                                        result = (New PCA.MasterTms).Create(masterTms)
                                    Else
                                        resultLine = "【変更】" & resultLine & vbTab & "結果："
                                        masterTms.TokuisakiMei1 = customer.TokuisakiMei1
                                        masterTms.TokuisakiKubun1 = customer.TokuisakiKubun1
                                        masterTms.TokuisakiKubun2 = customer.TokuisakiKubun2
                                        masterTms.TokuisakiKubun3 = customer.TokuisakiKubun3
                                        masterTms.TokuisakiKubun4 = customer.TokuisakiKubun4
                                        masterTms.YubinBango = customer.YubinBango
                                        masterTms.Jyusyo1 = customer.Jyusyo1
                                        masterTms.Jyusyo2 = customer.Jyusyo2
                                        masterTms.AitesakiTelNo = customer.AitesakiTelNo
                                        masterTms.Syutantosya = customer.Syutantosya
                                        result = (New PCA.MasterTms).Modify(masterTms)
                                    End If
                                    If Not String.IsNullOrEmpty(result.ErrorCode) Then
                                        resultLine &= result.ErrorMessage
                                    Else
                                        SQLString = "update Customers set SendFlag = 1 where SendFlag = 0"
                                        SQLString &= " and CustomerCode = @CustomerCode"
                                        cmd.CommandText = SQLString
                                        cmd.Parameters.Clear()
                                        cmd.Parameters.AddWithValue("@CustomerCode", CInt(customer.TokuisakiCode))
                                        cmd.ExecuteNonQuery()

                                        resultLine &= "正常"
                                        logFile.PutLog(resultLine)
                                    End If

                                    dlg.ProgressBar.Value += 1
                                    Application.DoEvents()
                                Next
                            End Using

                            If String.IsNullOrEmpty(customerCode) Then
                                MessageBox.Show("得意先マスタの転送が完了しました。", _clickedButton.Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
                            End If

                            If String.IsNullOrEmpty(customerCode) Then
                                Dim p As System.Diagnostics.Process = System.Diagnostics.Process.Start(sharedIniFile.LogPath)
                            End If
                        Catch ex As Exception
                            Throw New Exception(ex.Message)
                        Finally
                            If cn IsNot Nothing Then
                                cn.Close()
                            End If
                        End Try
                    End Using
                End Using
            Catch ex As Exception
                MessageBox.Show("処理が異常終了しました。" & vbLf & vbLf &
                                "「" & ex.Message & "」", _clickedButton.Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                _parentForm.Cursor = Cursors.Default
            End Try
        End If

    End Sub

    Public Sub SyncOperator()

        ' 確認ダイアログを表示
        If MessageBox.Show("商魂・商管マスタの同期を実行します。本当によろしいですか？", _clickedButton.Text,
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.Yes Then

            Try
                _parentForm.Cursor = Cursors.WaitCursor

                Using dlg As New ProcessingDialog
                    ' 処理中ダイアログ表示
                    dlg.Show(_parentForm)
                    dlg.ProcessName = "処　　理　　中"
                    dlg.Message1 = "しばらくお待ちください。"
                    _parentForm.Refresh()

                    Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
                        cn.Open()
                        Dim trans As NpgsqlTransaction = cn.BeginTransaction
                        Try
                            Using cmd As NpgsqlCommand = cn.CreateCommand()
                                cmd.Transaction = trans

                                Dim SQLString As String

                                SQLString = "delete from Sections"
                                cmd.CommandText = SQLString
                                cmd.Parameters.Clear()
                                cmd.ExecuteNonQuery()
                                For Each bumon As PCA.BEMasterBumon In (New PCA.MasterBumon).GetAll
                                    SQLString = "insert into Sections ("
                                    SQLString &= " SectionCode"
                                    SQLString &= ",SectionName"
                                    SQLString &= " ) values ("
                                    SQLString &= " @SectionCode"
                                    SQLString &= ",@SectionName"
                                    SQLString &= ")"
                                    cmd.CommandText = SQLString
                                    cmd.Parameters.Clear()
                                    cmd.Parameters.AddWithValue("@SectionCode", bumon.BumonCode)
                                    cmd.Parameters.AddWithValue("@SectionName", bumon.BumonMei)
                                    cmd.ExecuteNonQuery()
                                Next

                                SQLString = "delete from Operators"
                                cmd.CommandText = SQLString
                                cmd.Parameters.Clear()
                                cmd.ExecuteNonQuery()
                                For Each tantosya As PCA.BEMasterTantosya In (New PCA.MasterTantosya).GetAll
                                    SQLString = "insert into Operators ("
                                    SQLString &= " OperatorCode"
                                    SQLString &= ",OperatorName"
                                    SQLString &= ",SectionCode"
                                    SQLString &= " ) values ("
                                    SQLString &= " @OperatorCode"
                                    SQLString &= ",@OperatorName"
                                    SQLString &= ",@SectionCode"
                                    SQLString &= ")"
                                    cmd.CommandText = SQLString
                                    cmd.Parameters.Clear()
                                    cmd.Parameters.AddWithValue("@OperatorCode", tantosya.TantosyaCode)
                                    cmd.Parameters.AddWithValue("@OperatorName", tantosya.TantosyaMei)
                                    cmd.Parameters.AddWithValue("@SectionCode", tantosya.BumonCode)
                                    cmd.ExecuteNonQuery()
                                Next

                                SQLString = "delete from Areas"
                                cmd.CommandText = SQLString
                                cmd.Parameters.Clear()
                                cmd.ExecuteNonQuery()
                                Dim conditionAreas As New PCA.BEMasterTokuisakiKubunCondition
                                conditionAreas.KubunId = "11"
                                For Each tokuisakiKubun As PCA.BEMasterTokuisakiKubun In (New PCA.MasterTokuisakiKubun).GetByCondition(conditionAreas)
                                    SQLString = "insert into Areas ("
                                    SQLString &= " AreaCode"
                                    SQLString &= ",AreaName"
                                    SQLString &= " ) values ("
                                    SQLString &= " @AreaCode"
                                    SQLString &= ",@AreaName"
                                    SQLString &= ")"
                                    cmd.CommandText = SQLString
                                    cmd.Parameters.Clear()
                                    cmd.Parameters.AddWithValue("@AreaCode", tokuisakiKubun.TokuisakiKubunCode)
                                    cmd.Parameters.AddWithValue("@AreaName", tokuisakiKubun.TokuisakiKubunMei)
                                    cmd.ExecuteNonQuery()
                                Next

                                SQLString = "delete from Occupations"
                                cmd.CommandText = SQLString
                                cmd.Parameters.Clear()
                                cmd.ExecuteNonQuery()
                                Dim conditionOccupations As New PCA.BEMasterTokuisakiKubunCondition
                                conditionOccupations.KubunId = "12"
                                For Each tokuisakiKubun As PCA.BEMasterTokuisakiKubun In (New PCA.MasterTokuisakiKubun).GetByCondition(conditionOccupations)
                                    SQLString = "insert into Occupations ("
                                    SQLString &= " OccupationCode"
                                    SQLString &= ",OccupationName"
                                    SQLString &= " ) values ("
                                    SQLString &= " @OccupationCode"
                                    SQLString &= ",@OccupationName"
                                    SQLString &= ")"
                                    cmd.CommandText = SQLString
                                    cmd.Parameters.Clear()
                                    cmd.Parameters.AddWithValue("@OccupationCode", tokuisakiKubun.TokuisakiKubunCode)
                                    cmd.Parameters.AddWithValue("@OccupationName", tokuisakiKubun.TokuisakiKubunMei)
                                    cmd.ExecuteNonQuery()
                                Next

                                SQLString = "delete from Hobbies"
                                cmd.CommandText = SQLString
                                cmd.Parameters.Clear()
                                cmd.ExecuteNonQuery()
                                Dim conditionHobbies As New PCA.BEMasterTokuisakiKubunCondition
                                conditionHobbies.KubunId = "14"
                                For Each tokuisakiKubun As PCA.BEMasterTokuisakiKubun In (New PCA.MasterTokuisakiKubun).GetByCondition(conditionHobbies)
                                    SQLString = "insert into Hobbies ("
                                    SQLString &= " HobbyCode"
                                    SQLString &= ",HobbyName"
                                    SQLString &= " ) values ("
                                    SQLString &= " @HobbyCode"
                                    SQLString &= ",@HobbyName"
                                    SQLString &= ")"
                                    cmd.CommandText = SQLString
                                    cmd.Parameters.Clear()
                                    cmd.Parameters.AddWithValue("@HobbyCode", tokuisakiKubun.TokuisakiKubunCode)
                                    cmd.Parameters.AddWithValue("@HobbyName", tokuisakiKubun.TokuisakiKubunMei)
                                    cmd.ExecuteNonQuery()
                                Next

                                trans.Commit()
                            End Using
                        Catch ex As Exception
                            trans.Rollback()
                            Throw New Exception(ex.Message)
                        Finally
                            If cn IsNot Nothing Then
                                cn.Close()
                            End If
                        End Try
                    End Using
                End Using
                MessageBox.Show("商魂・商管マスタの同期が完了しました。", _clickedButton.Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
            Catch ex As Exception
                MessageBox.Show("処理が異常終了しました。" & vbLf & vbLf &
                                "「" & ex.Message & "」", _clickedButton.Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                _parentForm.Cursor = Cursors.Default
            End Try
        End If

    End Sub

    Public Sub ImportCustomer()

        ' 確認ダイアログを表示
        If MessageBox.Show("得意先マスタの取込を実行します。本当によろしいですか？", _clickedButton.Text,
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.Yes Then

            Dim file As String = SelectOpenFile("選択されたファイルをインポート", , "csvファイル(*.csv)|*.csv")
            If String.IsNullOrWhiteSpace(file) Then
                Exit Sub
            End If

            Try
                _parentForm.Cursor = Cursors.WaitCursor

                Using dlg As New ProcessingDialog
                    ' 処理中ダイアログ表示
                    dlg.Show(_parentForm)
                    dlg.ProcessName = "処　　理　　中"
                    dlg.Message1 = "しばらくお待ちください。"
                    dlg.ProgressBar.Visible = True
                    Using reader As New IO.StreamReader(file, System.Text.Encoding.Default)
                        dlg.ProgressBar.Maximum = Split(reader.ReadToEnd, vbCrLf).Count - 1
                    End Using
                    _parentForm.Refresh()

                    Using cn As New NpgsqlConnection(sharedIniFile.ConnectionString)
                        cn.Open()
                        Try
                            Using cmd As NpgsqlCommand = cn.CreateCommand()
                                Dim SQLString As String

                                SQLString = "delete from Customers"
                                SQLString &= " where CustomerCode > 100"
                                cmd.CommandText = SQLString
                                cmd.Parameters.Clear()
                                cmd.ExecuteNonQuery()

                                SQLString = "update Numbers set CurrentNumber = 100"
                                SQLString &= " where NumberType = 3"
                                cmd.CommandText = SQLString
                                cmd.Parameters.Clear()
                                cmd.ExecuteNonQuery()

                                Using reader As New IO.StreamReader(file, System.Text.Encoding.Default)
                                    Dim readLine As String
                                    Dim readBuffer() As String
                                    Dim i As Integer
                                    Dim errorLog As New LogWriter(My.Application.Info.DirectoryPath & "\error.txt", True)
                                    While (reader.Peek() >= 0)
                                        i += 1
                                        readLine = reader.ReadLine
                                        dlg.ProgressBar.Value = i
                                        Application.DoEvents()

                                        ' 明細を読む
                                        readBuffer = readLine.Split(",")

                                        Dim dt As String = String.Empty
                                        If Not String.IsNullOrWhiteSpace(readBuffer(6)) AndAlso
                                           Not String.IsNullOrWhiteSpace(readBuffer(7)) AndAlso
                                           Not String.IsNullOrWhiteSpace(readBuffer(8)) AndAlso
                                           Not String.IsNullOrWhiteSpace(readBuffer(9)) Then
                                            Try
                                                dt = CType(readBuffer(6) & readBuffer(7) & "/" & readBuffer(8) & "/" & readBuffer(9), Date).Date.ToString("yyyy/MM/dd")
                                            Catch ex As Exception
                                                errorLog.PutLog(i & " " & readBuffer(0) & " " & readBuffer(6) & readBuffer(7) & "/" & readBuffer(8) & "/" & readBuffer(9))
                                                dt = String.Empty
                                            End Try
                                        ElseIf Not String.IsNullOrWhiteSpace(readBuffer(8)) AndAlso
                                               Not String.IsNullOrWhiteSpace(readBuffer(9)) Then
                                            dt = readBuffer(8) & "/" & readBuffer(9)
                                            Try
                                                CType(Now.Date.Year & "/" & readBuffer(8) & "/" & readBuffer(9), Date).Date.ToString("yyyy/MM/dd")
                                            Catch ex As Exception
                                                errorLog.PutLog(i & " " & readBuffer(0) & " " & readBuffer(8) & "/" & readBuffer(9))
                                                dt = String.Empty
                                            End Try
                                        Else
                                            dt = String.Empty
                                        End If

                                        Dim customerCode As Integer = (New Numbers).GetNextNumber(NumberType.CustomerCode, True)

                                        Dim hasRows As Boolean
                                        SQLString = "Select * from Customers"
                                        SQLString &= " where CustomerName = @CustomerName"
                                        SQLString &= " and Birthday = @Birthday"
                                        SQLString &= " and ZipCode = @ZipCode"
                                        cmd.CommandText = SQLString
                                        cmd.Parameters.Clear()
                                        cmd.Parameters.AddWithValue("@CustomerName", readBuffer(0).Trim)
                                        cmd.Parameters.AddWithValue("@Birthday", dt)
                                        cmd.Parameters.AddWithValue("@ZipCode", readBuffer(3).Trim)
                                        Using dr As NpgsqlDataReader = cmd.ExecuteReader()
                                            hasRows = dr.HasRows
                                        End Using

                                        If Not hasRows Then
                                            SQLString = "insert into Customers ("
                                            SQLString &= " CustomerCode"
                                            SQLString &= ",CustomerName"
                                            SQLString &= ",CustomerKana"
                                            SQLString &= ",Gender"
                                            SQLString &= ",Birthday"
                                            SQLString &= ",ZipCode"
                                            SQLString &= ",Address"
                                            SQLString &= ",TelNo"
                                            SQLString &= ") values ("
                                            SQLString &= " :CustomerCode"
                                            SQLString &= ",:CustomerName"
                                            SQLString &= ",:CustomerKana"
                                            SQLString &= ",:Gender"
                                            SQLString &= ",:Birthday"
                                            SQLString &= ",:ZipCode"
                                            SQLString &= ",:Address"
                                            SQLString &= ",:TelNo"
                                            SQLString &= " )"
                                            cmd.CommandText = SQLString
                                            cmd.Parameters.Clear()
                                            cmd.Parameters.AddWithValue(":CustomerCode", customerCode)
                                            cmd.Parameters.AddWithValue(":CustomerName", readBuffer(0).Trim)
                                            cmd.Parameters.AddWithValue(":CustomerKana", readBuffer(1))
                                            Select Case readBuffer(2)
                                                Case "男"
                                                    cmd.Parameters.AddWithValue(":Gender", CInt(Gender.Male))
                                                Case "女"
                                                    cmd.Parameters.AddWithValue(":Gender", CInt(Gender.Female))
                                                Case Else
                                                    cmd.Parameters.AddWithValue(":Gender", DBNull.Value)
                                            End Select
                                            cmd.Parameters.AddWithValue(":Birthday", dt)
                                            cmd.Parameters.AddWithValue(":BirthdayMonth", readBuffer(8).Trim)
                                            cmd.Parameters.AddWithValue(":ZipCode", readBuffer(3).Trim)
                                            cmd.Parameters.AddWithValue(":Address", readBuffer(4).Trim)
                                            cmd.Parameters.AddWithValue(":TelNo", readBuffer(5).Trim)
                                            cmd.ExecuteNonQuery()
                                        End If
                                    End While
                                End Using
                            End Using
                        Catch ex As Exception
                            Throw New Exception(ex.Message)
                        Finally
                            If cn IsNot Nothing Then
                                cn.Close()
                            End If
                        End Try
                    End Using
                End Using
                MessageBox.Show("得意先マスタの取込が完了しました。", _clickedButton.Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
            Catch ex As Exception
                MessageBox.Show("処理が異常終了しました。" & vbLf & vbLf &
                                "「" & ex.Message & "」", _clickedButton.Text, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                _parentForm.Cursor = Cursors.Default
            End Try
        End If

    End Sub

End Class
