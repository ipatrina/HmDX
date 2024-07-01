Imports System.Text.RegularExpressions

Public Class PLUGIN_ForWebAppAPI
    Public APIHost As String = "https://api.forwebapp.com:2053/parser"

    Public Function Parse(InputHTML As String) As String
        Try
            Dim Site As String = ""
            Dim SubUrl As String = ""
            If InputHTML.Contains("youku.com/") Then
                Site = "YOUKU"
                SubUrl = "/youku?url="
            ElseIf InputHTML.Contains("iqiyi.com/") Then
                Site = "IQIYI"
                SubUrl = "/iqiyi?url="
            ElseIf InputHTML.Contains("tv.sohu.com/") Then
                Site = "SOHU"
                SubUrl = "/sohu?url="
            End If

            Dim MasterString As String = "#EXTM3U" & vbCrLf
            Dim WebPage As String = MainUI.GetWebpage(APIHost & SubUrl & InputHTML).Replace(" H", "_H").Replace(" ", "")

            Dim UrlsList As MatchCollection = Regex.Matches(WebPage, """url"":""(https?\:\/\/([\w+?\.\w+])+([a-zA-Z0-9\~\!\@\#\$\%\^\&\*\(\)_\-\=\+\\\/\?\.\:\;\'\,]*)?)""", RegexOptions.None)
            Dim DfnList As MatchCollection = Regex.Matches(WebPage, """dfn"":""([0-9A-Za-z _]*)""", RegexOptions.None)
            Dim EncodeList As MatchCollection = Regex.Matches(WebPage, """encode"":""([0-9A-Za-z]*)""", RegexOptions.None)
            Dim ResolutionList As MatchCollection = Regex.Matches(WebPage, """resolution"":""([0-9A-Za-z]*)""", RegexOptions.None)

            If Site = "YOUKU" Then
                For _loc_11 = 0 To UrlsList.Count - 1
                    MasterString += GetLine(DfnList(_loc_11).Groups(1).Value, ResolutionList(_loc_11).Groups(1).Value & " " & EncodeList(_loc_11).Groups(1).Value, UrlsList(_loc_11).Groups(1).Value)
                Next
            ElseIf Site = "IQIYI" Then
                For _loc_12 = 0 To UrlsList.Count - 1
                    MasterString += GetLine(EncodeList(_loc_12).Groups(1).Value, ResolutionList(_loc_12).Groups(1).Value & " " & DfnList(_loc_12).Groups(1).Value, UrlsList(_loc_12).Groups(1).Value)
                Next
            ElseIf Site = "SOHU" Then
                For _loc_13 = 0 To UrlsList.Count - 1
                    MasterString += GetLine(EncodeList(_loc_13).Groups(1).Value, DfnList(_loc_13).Groups(1).Value, UrlsList(_loc_13).Groups(1).Value)
                Next
            End If

            Return MasterString
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Private Function GetLine(TYPE As String, NAME As String, URI As String) As String
        Return "#EXT-X-MEDIA:TYPE=" & TYPE & ",NAME=""" & NAME & """,URI=""" & URI & """" & vbCrLf
    End Function

    Public Function GetYoukuKey(YKVid As String) As String
        Try
            Return Regex.Match(MainUI.GetWebpage(APIHost & "/youku/getKey?vids=" & YKVid).Replace(" ", ""), """data"":\[""(.*)""\]", RegexOptions.None).Groups(1).Value
        Catch ex As Exception
            Return ""
        End Try
    End Function
End Class
