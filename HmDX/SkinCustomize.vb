Public Class SkinCustomize
    Private Sub BtnOK_Click(sender As Object, e As EventArgs) Handles BtnOK.Click
        Try
            MainUI.SkinRGB = NumR.Value & "," & NumG.Value & "," & NumB.Value
        Catch ex As Exception

        Finally
            Dispose()
        End Try
    End Sub
End Class