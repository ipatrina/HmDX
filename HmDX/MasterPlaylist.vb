Public Class MasterPlaylist

    Public DefList As New List(Of String)
    Public TrackList As New List(Of String)
    Public UserClose As Boolean = True

    Private Sub MasterPlaylist_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            DefList = MainUI.DefList
            TrackList = MainUI.TrackList

            For Each _loc_1 In DefList
                Dim B As String = _loc_1.Split(vbTab)(0)
                Dim R As String = _loc_1.Split(vbTab)(1)
                If IsNumeric(B) Then
                    B = Int(Int(B) / 1024) & " kbps"
                Else
                    B = "未知"
                End If
                If R = "" Then
                    R = "未知"
                End If
                LstDefs.Items.Add("分辨率 " & R & ", 比特率 " & B)
            Next

            For Each _loc_2 In TrackList
                LstDefs.Items.Add(_loc_2.Split(vbTab)(0))
            Next
        Catch ex As Exception
            MainUI.WriteLog(ex.ToString)
            MsgBox("无法显示清晰度列表！", vbExclamation, "")
            MainUI.EnableControls()
            Dispose()
        End Try
    End Sub

    Private Sub MasterPlaylist_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        Try
            e.Cancel = 1
            If UserClose Then
                MainUI.EnableControls()
            End If
            Dispose()
        Catch ex As Exception
            MainUI.WriteLog(ex.ToString)
        End Try
    End Sub

    Private Sub LstDefs_DoubleClick(sender As Object, e As EventArgs) Handles LstDefs.DoubleClick
        Try
            Subm()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub LstDefs_KeyDown(sender As Object, e As KeyEventArgs) Handles LstDefs.KeyDown
        Try
            If e.KeyCode = Keys.Enter Then
                Subm()
            End If
        Catch ex As Exception
            MainUI.WriteLog(ex.ToString)
        End Try
    End Sub

    Private Sub Subm()
        Try
            UserClose = False
            MainUI.MasSelect(LstDefs.SelectedIndex)
            Dispose()
        Catch ex As Exception
            MainUI.WriteLog(ex.ToString)
        End Try
    End Sub
End Class