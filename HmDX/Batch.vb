Imports System.Text.RegularExpressions
Imports System.Threading

Public Class Batch
    Public ClipboardCache As String = ""
    Public FormLoaded As Boolean = False
    Public IdentifierRenames As New List(Of String)
    Public InputByClipboardParser As Boolean = False
    Public NextTaskName As String = ""
    Public Running As Integer = 0
    Public SeqHold As Boolean = False
    Public StrQueueStatus1 As String = "□"
    Public StrQueueStatus2 As String = "■"
    Public TaskAdded As Boolean = False

    Private Sub Batch_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            ParseRegOptions()
            FormLoaded = True
            TxtWorkPath.Text = MainUI.TxtWorkPath.Text
            TmrClipboard.Enabled = True

            If MainUI.IsFFmpegExist() Then
                RadMP4.Checked = True
            End If

            TotIdentifier.SetToolTip(TxtIdentifier, "使用""!""排除字符；使用""<""设定名称；使用""|""设定新的关键字；先使用""!""再使用""<""最后使用""|""；首位标记""?""仅匹配一次")
            TotTaskName.SetToolTip(TxtTaskName, "使用""\s""表示序列；使用""\r""表示鉴别名称")
        Catch ex As Exception

        End Try
    End Sub

    Private Sub Batch_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        Try
            e.Cancel = 1
            If MsgBox("您确定要退出批量任务控制器？", vbQuestion + vbYesNo, "") = vbNo Then
                Exit Sub
            End If
            MainUI.Dispose()
            End
        Catch ex As Exception
            MainUI.Dispose()
            End
        End Try
    End Sub

    Private Sub LblWorkPath_Click(sender As Object, e As EventArgs) Handles LblWorkPath.Click
        Try
            Process.Start("explorer.exe", TxtWorkPath.Text)
        Catch ex As Exception

        End Try
    End Sub

    Private Sub BtnWorkPathBrowse_Click(sender As Object, e As EventArgs) Handles BtnWorkPathBrowse.Click
        Try
            If FbdWorkPath.ShowDialog = DialogResult.OK Then
                TxtWorkPath.Text = FbdWorkPath.SelectedPath
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub BtnRemove_Click(sender As Object, e As EventArgs) Handles BtnRemove.Click
        Try
            If MsgBox("您确定要移除选中的队列项目？", vbQuestion + vbYesNo, "") = vbNo Then Exit Sub
            For Each _loc_1 In LsvQueue.SelectedItems
                LsvQueue.Items.Remove(_loc_1)
            Next
        Catch ex As Exception

        End Try
    End Sub

    Private Sub BtnRemoveAll_Click(sender As Object, e As EventArgs) Handles BtnRemoveAll.Click
        Try
            If MsgBox("您确定要移除所有队列项目？", vbQuestion + vbYesNo, "") = vbNo Then Exit Sub
            LsvQueue.Items.Clear()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub ChkTaskSuspend_CheckedChanged(sender As Object, e As EventArgs) Handles ChkTaskSuspend.CheckedChanged
        Try
            If FormLoaded Then
                If ChkTaskSuspend.Checked Then
                    TmrQueue.Enabled = False
                Else
                    TmrQueue.Enabled = True
                    TmrQueue_Tick(sender, e)
                End If
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Function BuildArgs()
        Try
            '========Patch submitted by developer liuxiangBIT========
            Dim renameType As String = ""
            Dim urlSplit As String() = TxtURL.Text.Split(vbTab)
            TxtURL.Text = urlSplit(0)
            If urlSplit.Count > 1 Then
                renameType = urlSplit(1).Split(vbCrLf)(0)
            End If
            '========================================================

            Dim Arg As String = ""

            'Append URL
            Arg += "URL>" & TxtURL.Text.Replace(">", "<g").Replace("<", "<l").Replace("|", "<s") & "|"

            'Append TaskName
            Dim Seq As String = NumSeqCurrent.Value
            If Seq.Length < NumSeqSupplement.Value Then
                For i = 1 To Int(NumSeqSupplement.Value - Seq.Length)
                    Seq = "0" & Seq
                Next
            End If

            NextTaskName = TxtTaskName.Text.Replace("\s", Seq).Replace("\r", renameType)
            Arg += "TaskName>" & NextTaskName & "|"
            If SeqHold = False Then NumSeqCurrent.Value += NumSeqAscend.Value

            'Append WorkPath
            Arg += "WorkPath>" & TxtWorkPath.Text & "|"

            'Append Container
            Dim Container As String = "RAW"
            If RadMP4.Checked Then
                Container = "MP4"
            ElseIf RadTS.Checked Then
                Container = "TS"
            End If
            Arg += "Container>" & Container & "|"

            'Append AutoStart
            If ChkAutoStart.Checked Then
                Arg += "AutoStart>" & ChkAutoStart.Checked & "|"
            End If

            'Append AutoExit
            If ChkAutoExit.Checked Then
                Arg += "AutoExit>" & ChkAutoExit.Checked & "|"
            End If

            'Append Minimize
            Arg += "Minimize>" & ChkMinimize.Checked & "|"

            'Append isLive
            If ChkIsLive.Checked Then
                Arg += "isLive>" & ChkIsLive.Checked & "|"
            End If

            'Append 2-Pass
            If ChkTwoPass.Checked Then
                Arg += "TwoPass>" & ChkTwoPass.Checked & "|"
            End If

            'Append Mute
            If ChkMute.Checked Then
                Arg += "Mute>" & ChkMute.Checked & "|"
            End If

            'Append AutoClean
            If ChkNoAutoClean.Checked Then
                Arg += "AutoClean>False|"
            End If

            'Append NoStream
            If ChkNoStream.Checked Then
                Arg += "NoStream>" & ChkNoStream.Checked & "|"
            End If

            'Append AutoSelect Best
            If ChkAutoSelectHigh.Checked = True Then
                Arg += "AutoSelect>Best|"
            End If

            'Append Timeout
            If IsNumeric(TxtTimeout.Text) Then
                Arg += "Timeout>" & Int(TxtTimeout.Text) & "|"
            End If

            'Append Threads
            If IsNumeric(TxtThreads.Text) Then
                Arg += "Threads>" & Int(TxtThreads.Text) & "|"
            End If

            Arg += TxtOptions.Text

            Return Arg
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Private Sub BtnTaskStart_Click(sender As Object, e As EventArgs) Handles BtnTaskStart.Click
        Try
            If TxtIdentifier.Text.Contains("|") Then
                MsgBox("当URL包含多组鉴别字符串时，无法直接启动任务！请使用队列。", vbExclamation, "注意")
                Exit Sub
            End If

            ParseIdentifier()
            If Not TxtURL.Text = "" Or Not TxtTaskName.Text = "" Then
                Dim Arg As String = BuildArgs()
                If Not Arg = "" Then
                    Process.Start(Application.ExecutablePath, Arg)
                    TxtURL.Text = ""
                End If
            End If
        Catch ex As Exception

        Finally
            TmrClipboard.Enabled = True
        End Try
    End Sub

    Private Sub BtnTaskQueue_Click(sender As Object, e As EventArgs) Handles BtnTaskQueue.Click
        Try
            BtnTaskQueueHandler()
        Catch ex As Exception

        Finally
            TmrClipboard.Enabled = True
        End Try
    End Sub

    Private Sub BtnTaskQueueHandler()
        Try
            ParseIdentifier()
            SeqHold = True
            For Each i In TxtURL.Text.Split(vbCrLf)
                TaskAdded = False
                ParseLinkLine(i.Trim)
                If TaskAdded = True Then
                    NumSeqCurrent.Value += NumSeqAscend.Value
                End If
            Next
            SeqHold = False
            If Not TxtTaskName.Text.Contains("\") Then TxtTaskName.Text = ""
        Catch ex As Exception

        End Try
    End Sub

    Private Sub BtnTaskOverride_Click(sender As Object, e As EventArgs) Handles BtnTaskOverride.Click
        Try
            If TxtIdentifier.Text.Contains("|") Then
                If MsgBox("当URL包含多组鉴别字符串时，重新应用参数将无法再次应用正确的序列及鉴别名称！" & vbCrLf & "是否继续？", vbExclamation + vbYesNo, "注意") = vbNo Then
                    Exit Sub
                End If
            End If

            For _loc_1 = 0 To LsvQueue.Items.Count - 1
                TxtURL.Text = LsvQueue.Items(_loc_1).SubItems(2).Text
                If Not TxtTaskName.Text.Contains("\") Then
                    TxtTaskName.Text = LsvQueue.Items(_loc_1).SubItems(1).Text
                End If
                Dim Arg As String = BuildArgs()
                If Not Arg = "" Then
                    LsvQueue.Items(_loc_1).SubItems(3).Text = Arg
                    LsvQueue.Items(_loc_1).SubItems(1).Text = NextTaskName
                End If
            Next
            If Not TxtTaskName.Text.Contains("\") Then TxtTaskName.Text = ""
            TxtURL.Text = ""
        Catch ex As Exception

        End Try
    End Sub

    Private Sub BtnAddLocal_Click(sender As Object, e As EventArgs) Handles BtnAddLocal.Click
        Try
            If OfdAddLocal.ShowDialog() = DialogResult.OK Then
                ApplyLocal(OfdAddLocal.FileNames)
            End If
            TxtURL.Text = ""
        Catch ex As Exception

        End Try
    End Sub

    Public Sub ApplyLocal(Input As String())
        Try
            For Each fName In Input
                If My.Computer.FileSystem.FileExists(fName) Then
                    Dim Lines() As String = IO.File.ReadAllLines(fName, System.Text.Encoding.UTF8)

                    If Lines(0).StartsWith("<") Then
                        ImportRegulated(Lines)
                        GoTo nxt
                    End If

                    If Lines(0).StartsWith(">") Then
                        ImportOptionLines(Lines)
                        GoTo nxt
                    End If

                    For Each Line In Lines
                        If Line.Trim.StartsWith("#EXT") Then
                            TxtURL.Text = "file:///" & fName
                            AddQueue()
                            GoTo nxt
                        ElseIf Line.Contains("<--->") Then
                            Dim Separator As Integer = Line.IndexOf("<--->")
                            Dim Arg1 As String = Line.Substring(0, Separator).Trim
                            Dim Arg2 As String = Line.Substring(Separator + 5, Line.Length - Separator - 5).Trim
                            If Arg1.Contains("://") Then
                                TxtTaskName.Text = Arg2
                                TxtURL.Text = Arg1
                            Else
                                TxtTaskName.Text = Arg1
                                TxtURL.Text = Arg2
                            End If
                            AddQueue()
                            TxtTaskName.Text = ""
                            TxtURL.Text = ""
                        Else
                            If Not TxtIdentifier.Text = "" Then
                                TxtURL.Text = My.Computer.FileSystem.ReadAllText(fName, System.Text.Encoding.UTF8)
                                BtnTaskQueueHandler()
                                TxtURL.Text = ""
                                GoTo nxt
                            End If
                            ParseLinkLine(Line.Trim)
                        End If
                    Next
                End If
                If fName.ToLower = "start" Then ChkTaskSuspend.Checked = False
nxt:        Next
        Catch ex As Exception

        End Try
    End Sub

    Private Sub ImportRegulated(param1 As String())
        Try
            For _loc_1 = 0 To param1.Count - 1
                If param1(_loc_1).StartsWith("<") Then
                    If param1(_loc_1 + 4).StartsWith(">") Then
                        NextTaskName = param1(_loc_1 + 2)
                        TxtURL.Text = param1(_loc_1 + 1)
                        AddQueue(System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(param1(_loc_1 + 3))))
                    End If
                End If
            Next
        Catch ex As Exception

        End Try
    End Sub

    Private Sub ImportOptionLines(param1 As String())
        Try
            For _loc_1 = 1 To param1.Count - 1
                If _loc_1 <= LsvQueue.Items.Count Then
                    LsvQueue.Items(_loc_1 - 1).SubItems(3).Text &= "|" & param1(_loc_1)
                End If
            Next
        Catch ex As Exception

        End Try
    End Sub

    Private Sub ParseLinkLine(Line As String)
        Try
            If Line.Trim.StartsWith("http") Or Line.Trim.StartsWith("file") Then
                TxtURL.Text = Line.Trim
                AddQueue()
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub AddQueue(Optional GivenArgs As String = "")
        Try
            Dim Arg As String = ""
            If GivenArgs = "" Then
                Arg = BuildArgs()
            Else
                Arg = GivenArgs
            End If

            If Not Arg = "" Then
                Dim idx As Integer = LsvQueue.Items.Count
                Dim lvi As ListViewItem = LsvQueue.Items.Add(StrQueueStatus1)
                lvi.EnsureVisible()
                LsvQueue.Items(idx).SubItems.Add(NextTaskName)
                LsvQueue.Items(idx).SubItems.Add(TxtURL.Text)
                LsvQueue.Items(idx).SubItems.Add(Arg)
                TxtURL.Text = ""
                TaskAdded = True
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub ParseIdentifier()
        Try
            '========Patch submitted by developer liuxiangBIT========
            'Start task without identifier
            Dim matchOnce As Boolean = False
            Dim identifier As String = TxtIdentifier.Text
            If identifier <> "" Then
                If identifier.Substring(0, 1) = "?" And identifier <> "?" Then
                    identifier = identifier.Substring(1)
                    matchOnce = True
                ElseIf identifier = "?" Then
                    identifier = ""
                End If
            End If

            Dim urlTmp As String = ""
            Dim filterParams As New List(Of String())
            Dim params As String() = identifier.Split("|")
            For Each param In params
                Dim incString As String = ""
                Dim excString As String = ""
                Dim renameType As String = ""
                Dim filterType As String = ""
                Dim splitType As String() = param.Split("<")
                If splitType.Count > 1 Then
                    renameType = splitType(1)
                    param = splitType(0)
                End If
                Dim splitExc As String() = param.Split("!")
                incString = splitExc(0).Replace("\e", "!")
                If splitExc.Count > 1 Then
                    excString = splitExc(1)
                End If
                If renameType.Length < 1 Then
                    renameType = incString
                    If excString.Length > 0 Then
                        renameType = renameType & "!" & excString
                    End If
                End If
                If incString.Length > 0 And excString.Length > 0 Then
                    filterType = "11"
                ElseIf incString.Length > 0 Then
                    filterType = "10"
                ElseIf excString.Length > 0 Then
                    filterType = "01"
                Else
                    filterType = "00"
                End If
                Dim resParams As String() = {incString, excString, renameType, filterType}
                filterParams.Add(resParams)
            Next
            Dim LinkMatches As MatchCollection = Regex.Matches(TxtURL.Text.Replace("\/", "/").Replace(vbLf, vbTab).Replace(vbCr, vbTab), "(.*?)(https?\:\/\/([\w+?\.\w+])+([a-zA-Z0-9\~\!\@\#\$\%\^\&\*\(\)_\-\=\+\\\/\?\.\:\;\'\,]*)?)", RegexOptions.IgnoreCase)
            If matchOnce = False Or filterParams.Count = 0 Then
                For Each Match As Match In LinkMatches
                    If filterParams.Count > 0 Then
                        For Each filterParam In filterParams
                            Dim isSelected As Boolean = False
                            Select Case filterParam(3)
                                Case "11"
                                    If Not Match.Groups(1).Value.Contains(filterParam(1)) And Match.Groups(1).Value.Contains(filterParam(0)) Then
                                        isSelected = True
                                    End If
                                Case "10"
                                    If Match.Groups(1).Value.Contains(filterParam(0)) Then
                                        isSelected = True
                                    End If
                                Case "01"
                                    If Not Match.Groups(1).Value.Contains(filterParam(1)) Then
                                        isSelected = True
                                    End If
                                Case "00"
                                    isSelected = True
                            End Select
                            If isSelected = True Then
                                If filterParam(2).Length > 0 Then
                                    urlTmp += Match.Groups(2).Value.Trim & vbTab & filterParam(2) & vbCrLf
                                Else
                                    urlTmp += Match.Groups(2).Value.Trim & vbCrLf
                                End If
                                Exit For
                            End If
                        Next
                    Else
                        'Start task without identifier
                        urlTmp += Match.Groups(2).Value.Trim & vbCrLf
                    End If
                Next
            Else
                Dim isFind As Boolean = False
                For Each filterParam In filterParams
                    For Each Match As Match In LinkMatches
                        Dim isSelected As Boolean = False
                        Select Case filterParam(3)
                            Case "11"
                                If Not Match.Groups(1).Value.Contains(filterParam(1)) And Match.Groups(1).Value.Contains(filterParam(0)) Then
                                    isSelected = True
                                End If
                            Case "10"
                                If Match.Groups(1).Value.Contains(filterParam(0)) Then
                                    isSelected = True
                                End If
                            Case "01"
                                If Not Match.Groups(1).Value.Contains(filterParam(1)) Then
                                    isSelected = True
                                End If
                            Case "00"
                                isSelected = True
                        End Select
                        If isSelected = True Then
                            isFind = True
                            If filterParam(2).Length > 0 Then
                                urlTmp += Match.Groups(2).Value.Trim & vbTab & filterParam(2) & vbCrLf
                            Else
                                urlTmp += Match.Groups(2).Value.Trim & vbCrLf
                            End If
                        End If
                    Next
                    If isFind = True Then
                        Exit For
                    End If
                Next
            End If
            TxtURL.Text = urlTmp
            '========================================================
        Catch ex As Exception
            MsgBox(ex.ToString, vbCritical, "鉴别失败")
        End Try
    End Sub

    Private Sub TmrQueue_Tick(sender As Object, e As EventArgs) Handles TmrQueue.Tick
        Try
            If Running < NumConcurrent.Value Then
                Dim Arg As String = ""
                For i = 0 To LsvQueue.Items.Count - 1
                    If LsvQueue.Items(i).SubItems(0).Text = StrQueueStatus1 Then
                        LsvQueue.Items(i).SubItems(0).Text = StrQueueStatus2
                        Arg = LsvQueue.Items(i).SubItems(3).Text
                        Exit For
                    End If
                Next

                If Arg = "" Then Exit Try

                Dim thread As New Thread(
                  Sub()
                      Dim p As New Process()
                      p.StartInfo.FileName = Application.ExecutablePath
                      p.StartInfo.Arguments = Arg
                      p.StartInfo.WindowStyle = ProcessWindowStyle.Normal
                      p.Start()
                      p.WaitForExit()
                      p.Close()
                      Running -= 1
                  End Sub
                )
                Running += 1
                thread.Start()
            End If
        Catch ex As Exception

        End Try

        Try
            If ChkAutoPowerOff.Checked And LsvQueue.Items(LsvQueue.Items.Count - 1).SubItems(0).Text = StrQueueStatus2 And Running = 0 Then
                Dim DoPowerOff As Boolean = True
                For i = 0 To LsvQueue.Items.Count - 1
                    If LsvQueue.Items(i).SubItems(0).Text = StrQueueStatus1 Then
                        DoPowerOff = False
                    End If
                Next
                If LsvQueue.Items.Count > 0 And DoPowerOff Then
                    PowerOff()
                End If
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub PowerOff()
        Try
            Process.Start("shutdown", "-s -c ""HmDX 很萌下载器批量任务完成，自动关机。""")
            TmrQueue.Enabled = False
            MsgBox("取消自动关机", vbQuestion, "")
            Process.Start("shutdown", "-a")
            ChkAutoPowerOff.Checked = False
            TmrQueue.Enabled = True
        Catch ex As Exception

        End Try
    End Sub

    Private Sub LsvQueue_DoubleClick(sender As Object, e As EventArgs) Handles LsvQueue.DoubleClick
        Try
            Process.Start(Application.ExecutablePath, LsvQueue.FocusedItem.SubItems(3).Text)
            LsvQueue.Items.Remove(LsvQueue.FocusedItem)
        Catch ex As Exception

        End Try
    End Sub

    Private Sub TxtURL_KeyDown(sender As Object, e As KeyEventArgs) Handles TxtURL.KeyDown
        Try
            If e.Control And e.KeyCode = Keys.A Then
                TxtURL.SelectAll()
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub TxtOptions_KeyDown(sender As Object, e As KeyEventArgs) Handles TxtOptions.KeyDown
        Try
            If e.Control And e.KeyCode = Keys.A Then
                TxtOptions.SelectAll()
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub BtnIdentifierClear_Click(sender As Object, e As EventArgs) Handles BtnIdentifierClear.Click
        Try
            TxtIdentifier.Text = ""
        Catch ex As Exception

        End Try
    End Sub

    Private Sub Batch_DragEnter(sender As Object, e As DragEventArgs) Handles Me.DragEnter
        Try
            If e.Data.GetDataPresent(DataFormats.FileDrop) = True Then
                e.Effect = DragDropEffects.Copy
            Else
                e.Effect = DragDropEffects.None
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub Batch_DragDrop(sender As Object, e As DragEventArgs) Handles Me.DragDrop
        Try
            Dim DragFilePath As String() = e.Data.GetData(DataFormats.FileDrop)
            ApplyLocal(DragFilePath)
        Catch ex As Exception

        End Try
    End Sub

    Private Sub ParseRegOptions()
        Try
            Dim _loc_1 As String = MainUI.KeyRead("QueueInterval")
            If Not _loc_1 = "" Then TmrQueue.Interval = Int(_loc_1)
        Catch ex As Exception

        End Try

        Try
            Dim _loc_2 As String = MainUI.KeyRead("Parallel")
            If Not _loc_2 = "" Then NumConcurrent.Value = Int(_loc_2)
        Catch ex As Exception

        End Try

        Try
            TxtOptions.Text = MainUI.KeyRead("Options")
        Catch ex As Exception

        End Try
    End Sub

    Private Sub TmrClipboard_Tick(sender As Object, e As EventArgs) Handles TmrClipboard.Tick
        Try
            If ClipboardCache <> Clipboard.GetText() Then
                ClipboardCache = Clipboard.GetText()
                ParseClipboard()
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub ParseClipboard()
        Try
            If Clipboard.GetText().Contains("http://") Or Clipboard.GetText().Contains("https://") Then
                InputByClipboardParser = False
                If TxtURL.Text = "" Then
                    TxtURL.Text = Clipboard.GetText()
                Else
                    TmrClipboard.Enabled = False
                    Exit Sub
                End If
                InputByClipboardParser = True
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub TxtURL_TextChanged(sender As Object, e As EventArgs) Handles TxtURL.TextChanged
        Try
            If InputByClipboardParser Then
                InputByClipboardParser = False
                TmrClipboard.Enabled = False
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub NumConcurrent_ValueChanged(sender As Object, e As EventArgs) Handles NumConcurrent.ValueChanged
        Try
            If FormLoaded Then MainUI.KeyWrite("Parallel", NumConcurrent.Value)
        Catch ex As Exception

        End Try
    End Sub

    Private Sub BtnExport_Click(sender As Object, e As EventArgs) Handles BtnExport.Click
        Try
            If SFDExport.ShowDialog = DialogResult.OK Then
                Dim UTF8NoBOM As New System.Text.UTF8Encoding(False)
                Dim FWriter As New System.IO.StreamWriter(SFDExport.FileName, False, UTF8NoBOM)
                Dim _loc_1 As Integer = 0
                For _loc_1 = 0 To LsvQueue.Items.Count - 1
                    FWriter.WriteLine("<")
                    FWriter.WriteLine(LsvQueue.Items(_loc_1).SubItems(2).Text)
                    FWriter.WriteLine(LsvQueue.Items(_loc_1).SubItems(1).Text)
                    FWriter.WriteLine(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(LsvQueue.Items(_loc_1).SubItems(3).Text)))
                    FWriter.WriteLine(">")
                Next
                FWriter.Close()
                FWriter.Dispose()
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub BtnTop_Click(sender As Object, e As EventArgs) Handles BtnTop.Click
        Try
            If LsvQueue.SelectedItems.Count > 0 Then
                Dim _loc_1 As Integer = LsvQueue.SelectedItems.Count
                For _loc_2 = 1 To _loc_1
                    Dim _loc_3 As ListViewItem = LsvQueue.SelectedItems(_loc_1 - 1)
                    LsvQueue.Items.Remove(_loc_3)
                    LsvQueue.Items.Insert(0, _loc_3)
                Next
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub BtnBottom_Click(sender As Object, e As EventArgs) Handles BtnBottom.Click
        If LsvQueue.SelectedItems.Count > 0 Then
            Dim _loc_1 As Integer = LsvQueue.SelectedItems.Count
            For _loc_2 = 1 To _loc_1
                Dim _loc_3 As ListViewItem = LsvQueue.SelectedItems(0)
                LsvQueue.Items.Remove(_loc_3)
                LsvQueue.Items.Insert(LsvQueue.Items.Count, _loc_3)
            Next
        End If
    End Sub
End Class