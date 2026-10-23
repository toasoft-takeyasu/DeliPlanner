Public Class Main

#Region "定義"

    Private Shared RefreshTimer As New System.Windows.Forms.Timer

#End Region

#Region "コンストラクタ"

    Public Shared Sub Main()

        ' システムアップデート
        'SystemUpdate()

        ' 初期設定
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)

        SetCollections()

        If sharedIniFile.EnableLogin Then
            ' ログイン
            Using login As New FormLogin
                If login.ShowDialog() <> DialogResult.OK Then Exit Sub
            End Using
        Else
            Dim [operator] As [Operator] = Operators.GetByCode("admin")
            If IsNothing([operator]) Then
                Operators.CreateDefaultAdministrator()
            End If
            sharedOperator = Operators.GetByCode("admin")
        End If

        startForm = New FormMenu
        Application.Run(startForm)

    End Sub

#End Region

#Region "メソッド"

    Private Shared Sub SystemUpdate()

    End Sub

#End Region

End Class
