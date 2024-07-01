Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Web

Public Class PLUGIN_YangShiPin
    Public Function ReplaceVKey(OldURL As String, NewURL As String) As String
        Try
            Dim _loc_1 As String = FindVKey(OldURL)
            Dim _loc_2 As String = FindVKey(NewURL)
            If Not _loc_1 = "" And Not _loc_2 = "" Then
                Dim _loc_5 As String = OldURL
                If OldURL.Contains("?") And NewURL.Contains("?") Then
                    _loc_5 = _loc_5.Replace("?" & OldURL.Split("?")(1), "?" & NewURL.Split("?")(1))
                End If
                Return _loc_5.Replace(_loc_1, _loc_2)
            End If
            Return NewURL
        Catch ex As Exception
            Return NewURL
        End Try
    End Function

    Public Function FindVKey(Input As String)
        Dim SplitChar As String = "/"
        Dim _loc_1 As String() = Input.Split("?")(0).Split(SplitChar)
        For _loc_3 = 2 To _loc_1.Count - 2
            If Not _loc_1(_loc_3).Contains(".") Then
                Return _loc_1(_loc_3)
            End If
        Next
        Return ""
    End Function

    Public Function GetWsSecret(URL As String) As String
        Try
            Dim httpReq As System.Net.HttpWebRequest
            Dim httpResp As System.Net.HttpWebResponse
            Dim httpURL As New System.Uri("https://ytpvdn.cctv.cn/cctvmobileinf/rest/cctv/videoliveUrl/getstream")
            Dim TPUrl As String = MainUI.YangShiPinTPUrl
            If TPUrl = "" Then
                TPUrl = URL
                MainUI.YangShiPinTPUrl = URL
            End If
            Dim postData As Byte() = Encoding.UTF8.GetBytes("url=" & HttpUtility.UrlEncode(URL))
            httpReq = CType(WebRequest.Create(httpURL), HttpWebRequest)
            httpReq.AllowAutoRedirect = True
            httpReq.ContentType = "application/x-www-form-urlencoded"
            httpReq.ContentLength = postData.Length
            httpReq.KeepAlive = True
            httpReq.Method = "POST"
            httpReq.UserAgent = "cctv_app_tv"
            httpReq.Referer = "api.cctv.cn"
            httpReq.Headers.Add("UID", MainUI.YangShiPinUID)
            Dim PostStream As Stream = httpReq.GetRequestStream()
            PostStream.Write(postData, 0, postData.Length)
            PostStream.Close()
            httpResp = CType(httpReq.GetResponse(), HttpWebResponse)
            Dim reader As New StreamReader(httpResp.GetResponseStream, System.Text.Encoding.UTF8)
            Dim _loc_1 As String() = reader.ReadToEnd.Split(Chr(34))
            For _loc_2 = 0 To _loc_1.Count - 3
                If _loc_1(_loc_2) = "url" Then
                    Return URL.Split("?")(0) & "?" & _loc_1(_loc_2 + 2).Trim.Split("?")(1)
                End If
            Next
            Return URL
        Catch ex As Exception
            Return URL
        End Try
    End Function

    Public Function GetIMEI() As String
        Try
            Return "355325022" & Int(Math.Ceiling(Rnd() * 999999) + 1).ToString.PadLeft(6, "0")
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Public Function TimeMachine(URL As String, SegCount As Integer)
        Try
            Dim _loc_1 As Match = Regex.Match(URL, "-([0-9]*).ts?", RegexOptions.None)
            Dim _loc_2 As String = _loc_1.Groups(0).Value
            Dim _loc_3 As Long = Convert.ToInt64(_loc_1.Groups(1).Value)
            Dim _loc_4 As String = "#EXTM3U" & vbCrLf
            For _loc_5 = 1 To SegCount
                Dim _loc_6 As Long = _loc_3 + _loc_5
                Dim _loc_7 As String = _loc_2.Replace(_loc_3.ToString, _loc_6.ToString)
                _loc_4 += "#EXTINF:10," & vbCrLf & URL.Replace(_loc_2, _loc_7) & vbCrLf
            Next
            Return _loc_4
        Catch ex As Exception
            Return ""
        End Try
    End Function
End Class
