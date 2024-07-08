Imports System.ComponentModel
Imports System.IO
Imports System.Media
Imports System.Net
Imports System.Net.Security
Imports System.Reflection
Imports System.Security.Cryptography
Imports System.Security.Cryptography.X509Certificates
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading
Imports Microsoft.Win32

Public Class MainUI

    Public Shared AppendQuery As Boolean = False
    Public Shared AutoClean As Boolean = True
    Public Shared AutoExit As Boolean = False
    Public Shared AutoPause As Boolean = False
    Public Shared AutoRedirect As Boolean = True
    Public Shared AutoRestart As Integer = 1800000
    Public Shared AutoSelect As Integer = -2
    Public Shared AutoStart As Boolean = False
    Public Shared BackupM3U8Path As String = ""
    Public Shared BackupM3U8Stream As Byte() = Nothing
    Public Shared BackupURL As String = ""
    Public Shared CheckboxInHandle As Boolean = False
    Public Shared ClipboardCache As String = ""
    Public Shared Cookie As String = ""
    Public Shared DefaultLiveTimeOut As Integer = 5000
    Public Shared DefaultTimeOut As Integer = 60000
    Public Shared DefaultUserAgent As String = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/125.0.0.0 Safari/537.36"
    Public Shared DefList As New List(Of String)
    Public Shared DLDoneAll As Boolean = False
    Public Shared DLIndex As Integer = 0
    Public Shared DoNotAutoRestart As Boolean = False
    Public Shared DoNotClean As Boolean = False
    Public Shared DoNotGetDol As Boolean = False
    Public Shared EmptyCmd As Boolean = True
    Public Shared EncodingTool As String = ""
    Public Shared EntireTs As Boolean = False
    Public Shared EntireTsTimeOut As Integer = 60
    Public Shared ErrCount As Integer = 0
    Public Shared ErrorExit As Boolean = False
    Public Shared ExpectedOutputFilename As String = ""
    Public Shared FFArg As String = ""
    Public Shared FFmpegAppend As String = ""
    Public Shared FFOutputPath As String = ""
    Public Shared FirstPassPath As String = ""
    Public Shared ForceNotAutoRedirect As Boolean = False
    Public Shared FormLoaded As Boolean = False
    Public Shared FromLocalGenerated As Boolean = False
    Public Shared GivenIV As String = ""
    Public Shared GivenKey As String = ""
    Public Shared Host As String = ""
    Public Shared HTTPHeader As New List(Of String)
    Public Shared IgnoreKey As Boolean = False
    Public Shared IgnoreSize As Boolean = False
    Public Shared IgnoreSohu As Boolean = False
    Public Shared IgnoreYouku As Boolean = False
    Public Shared InputByClipboardParser As Boolean = False
    Public Shared IsAiShang As Boolean = False
    Public Shared IsDol As Boolean = True
    Public Shared IsMGTV As Boolean = False
    Public Shared IsRedirect As Boolean = False
    Public Shared KeepAlive As Boolean = True
    Public Shared KeyList As New List(Of String)
    Public Shared LastKey As Byte() = Nothing
    Public Shared LastKeyURL As String = ""
    Public Shared LiveAPI As Boolean = False
    Public Shared LiveAPICurrentURL As String = ""
    Public Shared LiveInterruptionCount As Integer = 0
    Public Shared LiveInterval As Integer = 1000
    Public Shared LiveIntervalStopWatch As New Stopwatch
    Public Shared LiveLastFullURL As String = ""
    Public Shared LiveLastIndex As Integer = -1
    Public Shared LiveLastProgramDateTime As String = ""
    Public Shared LiveLastSegURL As String = ""
    Public Shared LiveStop As Boolean = False
    Public Shared LiveTimeOut As Integer = DefaultLiveTimeOut
    Public Shared LiveTimerStopwatch As New Stopwatch
    Public Shared LiveTransfer As Boolean = False
    Public Shared LogFileName As String = System.DateTime.Now.ToString("HHmmss") & GetRndString(5)
    Public Shared LooseMatch As Boolean = False
    Public Shared MaxErrCount As Integer = 100
    Public Shared MParentURL As String = ""
    Public Shared MTBuffer As New List(Of Byte())
    Public Shared MTControllerInUse As Boolean = False
    Public Shared MTControllerQueue As Boolean = False
    Public Shared MTIndex As Integer = 0
    Public Shared MTSpeed As Decimal = 0
    Public Shared MTSpeedCounter As Long = 0
    Public Shared MTSpeedStopWatch As New Stopwatch
    Public Shared MTStatus As New List(Of Integer)
    Public Shared MTTDefined As Boolean = False
    Public Shared MTThreads As Integer = 10
    Public Shared MTWorker As Integer = 0
    Public Shared Mute As Boolean = False
    Public Shared NoProxy As Boolean = False
    Public Shared NoStream As Boolean = False
    Public Shared Opts As String = ""
    Public Shared ParentURL As String = ""
    Public Shared ParseJSON As Boolean = False
    Public Shared PauseNow As Boolean = False
    Public Shared PLUGIN_DeYK As String = ""
    Public Shared PLUGIN_ThousandBillion As Boolean = False
    Public Shared PostExecution As String = ""
    Public Shared PostExecutionArguments As String = ""
    Public Shared ProgramDateTimeList As New List(Of String)
    Public Shared Proxy As String = ""
    Public Shared RangeList As New List(Of String)
    Public Shared RecentRedirect As String = ""
    Public Shared RedoSetOption As Boolean = False
    Public Shared Referer As String = ""
    Public Shared RefreshURL As Boolean = False
    Public Shared SegList As New List(Of String)
    Public Shared Seg2TS As Boolean = False
    Public Shared Skin As String = ""
    Public Shared SkinRGB As String = ""
    Public Shared Skip As String = ""
    Public Shared StartErrorExit As Boolean = False
    Public Shared TaskDoneAll As Boolean = False
    Public Shared TaskFail As Boolean = False
    Public Shared TempFolderName As String = "HmDX_Cache"
    Public Shared TempPath As String = ""
    Public Shared TimeoutList As New List(Of String)
    Public Shared TrackList As New List(Of String)
    Public Shared UserAgent As String = ""
    Public Shared UserTimeOut As Integer = 0
    Public Shared ValidSize As Integer = 131072
    Public Shared VersionStrings As String() = Application.ProductVersion.ToString.Split(".")
    Public Shared WindowDefaultTitle As String = ""
    Public Shared WorkingPath As String = Application.StartupPath & "\"
    Public Shared YangShiPinTimeMachineLength As String = 1000
    Public Shared YangShiPinTPUrl As String = ""
    Public Shared YangShiPinUID As String = ""

    Private Sub MainUI_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            WindowDefaultTitle = Text
            ParseRegOptions()
            ApplyBanner()
            ApplySkin()
            FormLoaded = True

            If KeyRead("ComplianceTest") = "" Then
                '.NET version compliance test
                If Not Environment.Version.ToString().StartsWith("4") Then
                    Hide()
                    MsgBox("Microsoft .NET Framework 版本检测不通过！" & vbCrLf & "请升级您的Microsoft .NET Framework至4.7.2版本后运行本软件。", vbExclamation, "HmDX 兼容性自检")
                    Dispose()
                    End
                End If

                'DPI compliance test
                If LblSubHeading.Left < LblTitle.Right Then
                    Hide()
                    MsgBox("DPI合规性检测不通过！" & vbCrLf & "请更换计算机或调整显示字体为标准大小后运行本软件。", vbExclamation, "HmDX 兼容性自检")
                    Dispose()
                    End
                End If

                'Font compliance test
                If Not (LblTitle.Font.Name.ToLower.Contains("雅黑") Or LblTitle.Font.Name.ToLower.Contains("yahei")) Then
                    Hide()
                    MsgBox("系统副本合规性检测不通过！" & vbCrLf & "请更换计算机或在未经修改、完整安装的操作系统中运行本软件。", vbExclamation, "HmDX 兼容性自检")
                    Dispose()
                    End
                End If
            End If

            If IsFFmpegExist() Then
                RadMP4.Checked = True
            End If

            Dim CommandStrings As String = Command()
            If CommandStrings = "" Or CommandStrings = Nothing Then
                WindowState = FormWindowState.Normal
                TmrClipboard.Enabled = True
            Else
                EmptyCmd = False
                If CommandStrings.StartsWith(Chr(34)) And CommandStrings.EndsWith(Chr(34)) Then
                    CommandStrings = CommandStrings.Substring(1, CommandStrings.Length - 2)
                End If
                '========Patch submitted by developer liuxiangBIT========
                If Not CommandStrings.Contains(">") Then
                    Dim filesplit As String() = CommandStrings.Split(Chr(34))
                    For fileidx = 0 To filesplit.Count - 1
                        If filesplit(fileidx).Trim.Length Then
                            Dim firstfile As String = filesplit(fileidx).Trim
                            Dim firstfilesplit As String() = firstfile.Split(" ")
                            If firstfilesplit(firstfilesplit.Length - 1).Contains("\") Then
                                firstfile = firstfilesplit(0)
                            End If
                            If My.Computer.FileSystem.FileExists(firstfile) Then
                                TxtURL.Text = "file:///" & firstfile
                            End If
                            Exit For
                        End If
                    Next
                Else
                    TxtOptions.Text = CommandStrings
                    SetOption(TxtOptions.Text, True)
                End If
                '========================================================
                If AutoStart Then
                    BtnStart_Click(sender, e)
                End If
            End If
        Catch ex As Exception
            WriteLog(ex.ToString)
        End Try
    End Sub

    Private Sub MainUI_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        Try
            e.Cancel = 1
            If BtnStart.Enabled = False Or BtnStart.Visible = False Then
                If Not TaskDoneAll Then
                    If MsgBox("任务正在进行，您确定要结束任务并退出程序吗？", vbQuestion + vbYesNo, Text) = vbNo Then
                        Exit Sub
                    End If
                End If
            End If
            Dispose()
            End
        Catch ex As Exception
            WriteLog(ex.ToString)
        End Try
    End Sub

    Private Sub TxtURL_KeyDown(sender As Object, e As KeyEventArgs) Handles TxtURL.KeyDown
        Try
            If e.Control And e.KeyCode = Keys.A Then
                TxtURL.SelectAll()
            End If
        Catch ex As Exception
            WriteLog(ex.ToString)
        End Try
    End Sub

    Private Sub BtnNewTask_Click(sender As Object, e As EventArgs) Handles BtnNewTask.Click
        CreateNewTask()
    End Sub

    Private Sub CreateNewTask()
        Try
            Process.Start(Application.ExecutablePath)
            If TaskDoneAll = True Then
                Dispose()
                End
            End If
        Catch ex As Exception
            WriteLog(ex.ToString)
        End Try
    End Sub

    Private Sub BtnMenu_Click(sender As Object, e As EventArgs) Handles BtnMenu.Click
        Try
            CMSMenu.Show(System.Windows.Forms.Control.MousePosition)
        Catch ex As Exception

        End Try
    End Sub

    Private Sub ChkLive_CheckedChanged(sender As Object, e As EventArgs) Handles ChkLive.CheckedChanged
        CheckCheckboxes()
    End Sub

    Private Sub ChkTwoPass_CheckedChanged(sender As Object, e As EventArgs) Handles ChkTwoPass.CheckedChanged
        CheckCheckboxes()
    End Sub

    Private Sub ChkLocal_CheckedChanged(sender As Object, e As EventArgs) Handles ChkLocal.CheckedChanged
        Try
            If Not FormLoaded Then Exit Sub
            If CheckboxInHandle Then Exit Sub
            CheckboxInHandle = True
            If ChkLocal.CheckState = CheckState.Checked Or ChkLocal.CheckState = CheckState.Unchecked Then
                ChkLocal.Checked = True
                ChkLocal.CheckState = CheckState.Indeterminate
                If OfdLocalM3U8.ShowDialog = DialogResult.OK Then
                    ApplyFileInput(OfdLocalM3U8.FileName)
                End If
            End If
            CheckboxInHandle = False
        Catch ex As Exception
            CheckboxInHandle = False
        End Try
    End Sub

    Private Sub CheckCheckboxes()
        Try
            If Not CheckboxInHandle Then
                CheckboxInHandle = True

                If ChkLive.Checked = True And ChkTwoPass.Checked = True Then
                    MsgBox("直播录制场景不支持2-Pass校验！", vbExclamation, Text)
                    ChkLive.Checked = False
                    ChkTwoPass.Checked = False
                    CheckboxInHandle = False
                    Exit Sub
                End If

                CheckboxInHandle = False
            End If
        Catch ex As Exception
            WriteLog(ex.ToString)
        End Try
    End Sub

    Private Sub ApplyFileInput(Input As String)
        Try
            Dim _loc_1 As New FileInfo(Input)
            If _loc_1.Length = 16 Then
                If TxtOptions.Text.Length > 0 And Not TxtOptions.Text.EndsWith("|") Then TxtOptions.Text += "|"
                TxtOptions.Text += "Key>" & BytesToHex(My.Computer.FileSystem.ReadAllBytes(Input))
            Else
                TxtURL.Text = "file:///" & Input
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub LblWorkPath_Click(sender As Object, e As EventArgs) Handles LblWorkPath.Click
        Try
            Process.Start("explorer.exe", WorkingPath)
        Catch ex As Exception

        End Try
    End Sub

    Private Sub LblOptions_Click(sender As Object, e As EventArgs) Handles LblOptions.Click
        Try
            If BtnStart.Enabled = False Or BtnStart.Visible = False Then
                RedoSetOption = True
                SetOption(TxtOptions.Text, False)
                MsgBox("参数已应用。", vbInformation, "应用参数")
            Else
                KeyWrite("Options", TxtOptions.Text)
                MsgBox("已存储为默认参数。", vbInformation, "保存参数")
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub BtnStart_Click(sender As Object, e As EventArgs) Handles BtnStart.Click
        Try
            ParentURL = ""
            GivenKey = ""
            GivenIV = ""
            YangShiPinTPUrl = ""
            HTTPHeader.Clear()

            My.Computer.FileSystem.CreateDirectory(WorkingPath & TempFolderName)
            WriteLog("[Initializing] HLS/m3u8 Downloader X, Version: " & VersionStrings(0) & "." & VersionStrings(1) & "." & VersionStrings(2) & " (20" & VersionStrings(3).Substring(0, 2) & "/" & VersionStrings(3).Substring(2, 2) & ")")
            SetOption(TxtOptions.Text, False)

            BtnStart.Visible = False
            BtnPause.Visible = True
            BtnWorkPathBrowse.Enabled = False
            BtnPathHistory.Enabled = False
            ChkLocal.Enabled = False
            LblDone.Visible = False
            LiveTransfer = False
            TmrClipboard.Enabled = False

            ParseURL()
        Catch ex As Exception
            WriteLog(ex.ToString)
            EnableControls()
        End Try
    End Sub

    Private Sub BtnPause_Click(sender As Object, e As EventArgs) Handles BtnPause.Click
        Try
            DoNotAutoRestart = True
            Pause()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub Pause()
        Try
            If ChkLive.Enabled Or (ChkLive.Enabled = False And PauseNow) Then
                If PauseNow = False Then
                    If ChkLive.Checked = True Then
                        MsgBox("直播节目不能暂停录制！", vbExclamation, Text)
                    Else
                        PauseNow = True
                        InvokeControl(LblWorking, Sub(x) x.Visible = False)
                        InvokeControl(LblSpeed, Sub(x) x.Visible = False)
                        InvokeControl(LblProgress, Sub(x) x.Text = "暂停...")
                        InvokeControl(BtnPause, Sub(x) x.Text = "继续(&C)")
                        WriteLog("Task paused.")
                        If AutoRestart < 0 And DoNotAutoRestart = False Then
                            WriteLog("Exit on error.")
                            Dispose()
                            End
                        ElseIf AutoRestart >= 100 And DoNotAutoRestart = False Then
                            Invoke(New DlgAutoRestartInterval(AddressOf TmrAutoRestartInterval), New Object() {AutoRestart})
                            Invoke(New DlgAutoRestartEnable(AddressOf TmrAutoRestartEnable), New Object() {True})
                            WriteLog("Re-start has been scheduled after " & TmrAutoRestart.Interval & " (ms).")
                        End If
                        DoNotAutoRestart = False
                    End If
                Else
                    TmrAutoRestart.Enabled = False
                    PauseNow = False
                    BtnPause.Text = "暂停(&P)"
                    ChkLive.Enabled = True
                    LblWorking.Visible = True
                    LblSpeed.Visible = True
                    WriteLog("Task re-start.")
                    MTController()
                End If
            Else
                LiveStop = True
                Comb()
            End If
        Catch ex As Exception
            WriteLog(ex.ToString)
        End Try
    End Sub

    Delegate Sub DlgAutoRestartEnable(param1 As Boolean)
    Delegate Sub DlgAutoRestartInterval(param1 As Integer)

    Private Sub TmrAutoRestartEnable(param1 As Boolean)
        TmrAutoRestart.Enabled = param1
    End Sub

    Private Sub TmrAutoRestartInterval(param1 As Integer)
        TmrAutoRestart.Interval = param1
    End Sub

    Private Sub BtnWorkPathBrowse_Click(sender As Object, e As EventArgs) Handles BtnWorkPathBrowse.Click
        Try
            If FbdWorkPath.ShowDialog = DialogResult.OK Then
                ApplyWorkPath(FbdWorkPath.SelectedPath)
                KeyWrite("WorkPath", WorkingPath)
                StorePathHistory(WorkingPath)
            End If
        Catch ex As Exception
            WriteLog(ex.ToString)
        End Try
    End Sub

    Private Sub ApplyWorkPath(UserInput As String)
        Try
            If BtnWorkPathBrowse.Enabled = False Then Exit Sub
            If Not UserInput = "" Then
                Dim append As String = ""
                If Not UserInput.EndsWith("\") Then
                    append = "\"
                End If
                TxtWorkPath.Text = UserInput & append
                If TxtWorkPath.Text = "\" Then
                    TxtWorkPath.Text = ""
                End If
                WorkingPath = TxtWorkPath.Text
            End If
        Catch ex As Exception
            WriteLog(ex.ToString)
        End Try
    End Sub

    Private Sub ApplyBanner()
        Try
            LblSlogan.Location = PicBanner.PointToClient(PointToScreen(LblSlogan.Location))
            LblSlogan.Parent = PicBanner

            Dim SloganList As New List(Of String) From {
                "落实科学发展观 保持党的先进性",
                "发挥先锋作用 永葆先进本色",
                "一颗红心跟党走 开创美好和谐风",
                "牢记党的光荣历史 发扬党的优良传统",
                "实现中国梦必须走中国道路",
                "实现中国梦必须弘扬中国精神",
                "实现中国梦必须凝聚中国力量",
                "跟着共产党 奔向中国梦",
                "贯彻落实新发展理念 建设现代化经济体系",
                "发展一带一路 提升经济密度 缩短经济距离 打破壁垒分割",
                "共建一带一路 共享发展成果",
                "中国的主权和领土完整不容侵犯和分割",
                "不忘初心跟党走 牢记使命当先锋 实干成就中国梦",
                "社会主义好 百姓日子好",
                "奋力走好新时代的长征路",
                "传递正能量 共筑中国梦",
                "不负伟大新时代 同心共筑中国梦",
                "同心共筑中国梦 不忘初心跟党走",
                "不忘初心 继续前进",
                "不忘初心 牢记使命",
                "新时代赋予新使命 新征程呼唤新作为",
                "新时代必须坚持新发展理念",
                "坚持把新发展理念贯彻到社会主义现代化建设全过程",
                "坚持人民当家作主",
                "人民当家作主是社会主义民主政治的本质和核心",
                "坚持全面依法治国",
                "坚持社会主义核心价值体系",
                "坚持人与自然和谐共生",
                "坚持总体国家安全观",
                "坚持党对人民军队的绝对领导",
                "全面贯彻党领导军队的一系列根本原则和制度",
                "坚持一国两制和推进祖国统一",
                "坚持全面从严治党",
                "奋力夺取新时代中国特色社会主义伟大胜利",
                "高举爱国主义社会主义旗帜 凝聚起实现中国梦的磅礴力量",
                "坚定不移高举中国特色社会主义伟大旗帜",
                "进行伟大斗争 建设伟大工程 推进伟大事业 实现伟大梦想",
                "实现中华民族伟大复兴 必须坚持中国共产党领导",
                "实现中华民族伟大复兴 必须坚持走中国特色社会主义道路",
                "实现中华民族伟大复兴 必须坚持以人民为中心",
                "实现中华民族伟大复兴 必须坚持斗争精神",
                "实现中华民族伟大复兴 必须坚定不移走和平发展道路",
                "牢固树立中国特色社会主义道路自信",
                "牢固树立中国特色社会主义理论自信",
                "牢固树立中国特色社会主义制度自信",
                "牢固树立中国特色社会主义文化自信",
                "牢固树立新时代中国特色社会主义教育自信",
                "牢固树立社会主义生态文明观",
                "推动形成人与自然和谐发展现代化建设新格局",
                "着力提高发展质量和效益 创新驱动引领高质量发展",
                "着力保障和改善民生 提升百姓获得感与幸福感",
                "着力加强和改善党的领导 用大力度夯实党建之基",
                "弘扬宪法精神 增强宪法意识",
                "大力弘扬宪法精神 维护宪法法律权威",
                "大力弘扬法治精神 增强全民法治观念",
                "弘扬宪法精神 履行宪法使命",
                "弘扬伟大民族精神 奋斗创造美好生活",
                "弘扬中华民族伟大的创造精神",
                "弘扬中华民族伟大的奋斗精神",
                "弘扬中华民族伟大的团结精神",
                "弘扬中华民族伟大的梦想精神",
                "坚持培育和践行社会主义核心价值观",
                "传承中华优秀传统文化 树立社会主义文化自信",
                "立足中华优秀传统文化 培育和弘扬社会主义核心价值观",
                "参军报国 无上光荣",
                "征兵固防 安国兴邦",
                "坚定不移跟党走 同心共筑中国梦",
                "坚定不移跟党走 牢记使命绘宏图",
                "坚定不移跟党走 砥砺奋进创新篇",
                "坚定不移跟党走 撸起袖子加油干",
                "坚定不移跟党走 建好家乡同奋斗",
                "新时代需要新担当 新征程要有新作为",
                "新思想引领新时代 新征程激发新责任",
                "汲取中国智慧 弘扬中国精神 传播中国价值",
                "严打黑恶犯罪 弘扬社会正气",
                "有黑扫黑 无黑除恶 无恶治乱",
                "有黑必扫 有恶必除 有伞必打",
                "有腐必反 有乱必治 除恶务尽",
                "依法严惩黑恶势力 维护社会和谐稳定",
                "依法严厉打击黑恶势力违法犯罪",
                "重拳出击扫黑除恶 依法严惩违法犯罪",
                "党在我心中 永远跟党走",
                "坚持和平发展道路 推动构建人类命运共同体",
                "坚定文化自信 推动社会主义文化繁荣兴盛",
                "文化兴国运兴 文化强民族强",
                "坚持走中国特色强军之路 全面推进国防和军队现代化",
                "大力弘扬和践行社会主义核心价值观",
                "大力弘扬伟大奋斗精神",
                "大力弘扬新时代共产党人的奉献精神",
                "奋力走好新时代的长征路",
                "新时代必须坚持新发展理念",
                "坚持以新发展理念引领高质量发展"
            }

            Randomize()
            Dim SloganItem As String = SloganList(Int(((SloganList.Count - 1) * Rnd())))
            Dim SloganFontSize As Integer = 8

            If SloganItem.Length <= 14 Then
                SloganFontSize = 16
            ElseIf SloganItem.Length <= 18 Then
                SloganFontSize = 14
            ElseIf SloganItem.Length <= 22 Then
                SloganFontSize = 12
            ElseIf SloganItem.Length <= 25 Then
                SloganFontSize = 11
            Else
                SloganFontSize = 10
            End If

            With LblSlogan.Font
                LblSlogan.Font = New System.Drawing.Font(.FontFamily, SloganFontSize, .Style, .Unit)
            End With

            LblSlogan.Text = SloganItem
        Catch ex As Exception

        End Try
    End Sub

    Private Sub ApplySkin()
        Try
            Dim ThemeColor As Color = Color.Black

            If Skin = "" Then
                'No skin is specified
            ElseIf Skin.Contains(",") Then
                Dim _loc_1 As String() = Skin.Split(")")
                Dim _loc_2 As String() = _loc_1(0).Replace("(", "").Replace(")", "").Split(",")
                Dim _loc_3 As Byte() = UnZip(Convert.FromBase64String(Skin.Substring(_loc_1(0).Length + 1, Skin.Length - (_loc_1(0).Length + 1))))
                Dim _loc_4 As New MemoryStream(_loc_3, 0, _loc_3.Length)
                ThemeColor = Color.FromArgb(Int(_loc_2(0)), Int(_loc_2(1)), Int(_loc_2(2)))
                BackgroundImage = Image.FromStream(_loc_4)
            ElseIf Skin = "None" Then
                PicBadge.Visible = False
                LblTitle.Visible = False
                LblSubHeading.Visible = False
                PicBanner.Visible = False
                LblSlogan.Visible = False
            ElseIf Skin = "Wave" Then
                ThemeColor = Color.AliceBlue
                BackgroundImage = Image.FromStream(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("HmDX.Wave_Background_0001.png"))
            ElseIf Skin = "Silver" Then
                BackgroundImage = Image.FromStream(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("HmDX.Silver_Background_0001.png"))
            ElseIf Skin = "Gold" Then
                ThemeColor = Color.Brown
                BackgroundImage = Image.FromStream(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("HmDX.Gold_Background_0001.png"))
            ElseIf Skin = "HatsuneMiku" Then
                LblSlogan.Visible = False
                Randomize()
                'Dim HatsuneMikuBackgroundID As Integer = Int(Math.Round(Rnd() * 2) + 1)
                Dim HatsuneMikuBackgroundID As Integer = 1
                BackgroundImage = Image.FromStream(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("HmDX.HatsuneMiku_Background_000" & HatsuneMikuBackgroundID & ".png"))
                PicBanner.Image = Image.FromStream(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("HmDX.HatsuneMiku_Banner_0001.png"))
                Randomize()
                PicBadge.Image = Image.FromStream(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("HmDX.HatsuneMiku_Badge_000" & Int(Math.Round(Rnd() * 4) + 1) & ".png"))
                If HatsuneMikuBackgroundID = 2 Then
                    ThemeColor = Color.LightYellow
                ElseIf HatsuneMikuBackgroundID = 3 Then
                    ThemeColor = Color.LightPink
                Else
                    ThemeColor = Color.DeepPink
                End If
                BtnNewTask.BackColor = ThemeColor
                BtnMenu.BackColor = ThemeColor
                LblSubHeading.BackColor = ThemeColor
                LblSubHeading.ForeColor = Color.White
            End If

            If Not ThemeColor = Color.Black Then
                LblTitle.ForeColor = ThemeColor
                LblURL.ForeColor = ThemeColor
                ChkLocal.ForeColor = ThemeColor
                ChkLive.ForeColor = ThemeColor
                ChkTwoPass.ForeColor = ThemeColor
                LblTaskName.ForeColor = ThemeColor
                LblWorkPath.ForeColor = ThemeColor
                LblContainerFormat.ForeColor = ThemeColor
                RadMP4.ForeColor = ThemeColor
                RadTS.ForeColor = ThemeColor
                RadBinary.ForeColor = ThemeColor
                LblOptions.ForeColor = ThemeColor
                LblProgress.ForeColor = ThemeColor
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub ParseURL()
        Try
            If TxtURL.Text.StartsWith("file:///") Then
                Dim FilePath As String = Regex.Match(TxtURL.Text.Replace("\/", "/"), "file:///(.*)?", RegexOptions.IgnoreCase).Groups(1).Value.Trim
                TxtURL.Text = "file:///" & FilePath.Replace("/", "\")
                If TxtTaskName.Text = "" And FromLocalGenerated = False Then TxtTaskName.Text = Path.GetFileNameWithoutExtension(FilePath)
                ParentURL = GetParent(TxtURL.Text)
                BackupM3U8Stream = My.Computer.FileSystem.ReadAllBytes(FilePath)
                FromLocalGenerated = False
                ParseSingleM3U8File(Encoding.UTF8.GetString(BackupM3U8Stream))
            ElseIf TxtURL.Text.StartsWith("#EXT") Then
                GenerateLocalPlaylist(TxtURL.Text.Replace("\/", "/").Replace("\n", vbCrLf))
                Exit Sub
            ElseIf TxtURL.Text.Contains(".amp4?") Or TxtURL.Text.Contains(".f4v?") Then
                Dim _loc_21 As String = "#EXTM3U" & vbCrLf
                For Each _loc_22 In TxtURL.Text.Split(vbCrLf)
                    If _loc_22.Contains(".amp4?") Or _loc_22.Contains(".f4v?") Then _loc_21 &= "#EXTINF:300," & vbCrLf & _loc_22.Trim & vbCrLf
                Next
                ParseJSON = True
                If TxtURL.Text.Contains(".f4v?") Then
                    Seg2TS = True
                    MTThreads = 1
                End If
                GenerateLocalPlaylist(_loc_21)
                Exit Sub
            Else
                LblProgress.Visible = True
                LblProgress.Text = "分析..."

                'Match HTTP and HTTPS URL
                TxtURL.Text = Regex.Match(TxtURL.Text.Replace("\/", "/").Replace(vbLf, vbTab).Replace(vbCr, vbTab), "https?\:\/\/([a-zA-Z0-9\~\!\@\#\$\%\^\&\*\(\)_\-\=\+\\\/\?\.\:\;\'\,\[\]]*)?", RegexOptions.IgnoreCase).Value.Trim

                'Attempt parse HTML
                'If (TxtURL.Text.Contains(".html") Or TxtURL.Text.Contains(".shtml")) And (TxtURL.Text.Contains("youku.com/") Or TxtURL.Text.Contains("iqiyi.com/") Or TxtURL.Text.Contains("tv.sohu.com/")) Then
                '    Dim ParseHTML As String = New PLUGIN_ForWebAppAPI().Parse(TxtURL.Text)
                '    If Not ParseHTML = "" Then
                '        GenerateLocalPlaylist(ParseHTML)
                '        Exit Sub
                '    End If
                'End If

                'Attempt parse YangShiPin TimeMachine
                If TxtURL.Text.Contains(".ysp.cctv.cn/") And TxtURL.Text.Contains(".ts?") Then
                    Dim YangShiPinTimeMachine As String = New PLUGIN_YangShiPin().TimeMachine(TxtURL.Text, YangShiPinTimeMachineLength)
                    If Not YangShiPinTimeMachine = "" Then
                        GenerateLocalPlaylist(YangShiPinTimeMachine)
                        Exit Sub
                    End If
                End If

                Try
                    If TxtURL.Text.Contains(".qq.com/") And TxtURL.Text.Contains(".ts?") Then
                        Dim _loc_1 As String() = TxtURL.Text.Split("?")(0).Split("/")
                        Dim _loc_2 As String = _loc_1(_loc_1.Count - 1).Substring(_loc_1(_loc_1.Count - 1).IndexOf("_") + 1)
                        Dim _loc_3 As String() = _loc_2.Split(".")
                        Dim _loc_4 As String = ""
                        For _loc_5 = 0 To _loc_3.Count - 3
                            _loc_4 += _loc_3(_loc_5) & "."
                        Next
                        _loc_4 += "ts.m3u8?ver=4"
                        _loc_1(_loc_1.Count - 1) = _loc_4
                        _loc_4 = ""
                        For _loc_6 = 0 To _loc_1.Count - 1
                            _loc_4 += _loc_1(_loc_6) & "/"
                        Next
                        TxtURL.Text = _loc_4.Substring(0, _loc_4.Length - 1)
                    End If
                Catch ex As Exception

                End Try

                If IgnoreYouku = False Then
                    Dim YKVid As String = ""
                    Try
                        YKVid = Regex.Match(TxtURL.Text, "vid=([0-9a-zA-Z=%]*)&", RegexOptions.None).Groups(1).Value
                        If (TxtURL.Text.Contains("cibntv.net/") Or TxtURL.Text.Contains("youku.com/")) And (Not YKVid = "") And TxtTaskName.Text = "" Then
                            TxtTaskName.Text = Regex.Match(GetWebpage("https://v.youku.com/v_show/id_" & YKVid & ".html"), "<meta name=""irTitle"" content=""(.*)"" ?\/>", RegexOptions.None).Groups(1).Value.Trim
                        End If
                    Catch ex As Exception

                    End Try

                    'If PLUGIN_DeYK.ToLower = "auto" And Not YKVid = "" Then PLUGIN_DeYK = New PLUGIN_ForWebAppAPI().GetYoukuKey(YKVid)
                End If

                If TxtURL.Text.Contains("sohu.com/") And IgnoreSohu = False Then
                    If TxtURL.Text.Contains("/m3u8v2/") Then
                        TxtURL.Text = TxtURL.Text.Replace("/m3u8v2/", "/ipad")
                    ElseIf TxtURL.Text.Contains("/m3u8v3/") Then
                        'TxtURL.Text = TxtURL.Text.Replace("&player=", "&_player=").Replace("&pt=", "&_pt=")
                    End If
                End If

                BackupURL = TxtURL.Text
                RecentRedirect = ""
                ForceNotAutoRedirect = True
                ParseSingleM3U8File(GetWebpage(BackupURL))
            End If
        Catch ex As Exception
            WriteLog(ex.ToString)
            If RefreshURL = False Then
                If (AutoStart And AutoExit) And (ErrCount < MaxErrCount) Then
                    ErrCount += 1
                    WriteLog("Error count: " & ErrCount)
                    Thread.Sleep(1000)
                    ParseURL()
                Else
                    StartError("ERROR_NO_HLS_DATA_URL")
                End If
            End If
        End Try
    End Sub

    Private Sub GenerateLocalPlaylist(Content As String)
        GenM3U8Path()
        My.Computer.FileSystem.WriteAllText(BackupM3U8Path, Content, False, Encoding.UTF8)
        TxtURL.Text = "file:///" & BackupM3U8Path
        FromLocalGenerated = True
        ParseURL()
    End Sub

    Private Sub ParseLive()
        Try
            If ChkLive.Checked = False Then
                Comb()
            Else
                Do
                    Thread.Sleep(100)
                Loop Until LiveInterval <= LiveIntervalStopWatch.Elapsed.TotalMilliseconds

                RecentRedirect = ""
                ForceNotAutoRedirect = True
                Dim _loc_1 As String = TxtURL.Text
                If LiveAPI Then
                    LiveAPICurrentURL = TxtURL.Text
                    _loc_1 = BackupURL
                End If
                ParseSingleM3U8File(GetWebpage(_loc_1))
            End If
        Catch ex As Exception
            ErrCount += 1
            WriteLog("Error count: " & ErrCount)
            Thread.Sleep(1000)
            ParseLive()
        End Try
    End Sub

    Private Sub GenM3U8Path()
        Try
            BackupM3U8Path = WorkingPath & TempFolderName & "\" & "PLST_" & System.DateTime.Now.ToString("HHmmss") & GetRndString(5) & ".m3u8"
            WriteLog("Creating backup playlist " & BackupM3U8Path)
        Catch ex As Exception
            WriteLog(ex.ToString)
        End Try
    End Sub

    Private Sub ParseSingleM3U8File(Data As String)
        Try
            If Not TxtTaskName.Text = "" And ChkLive.Enabled Then
                WriteLog("[Task name] " & TxtTaskName.Text)
            End If

            If NoStream Then
                My.Computer.FileSystem.WriteAllBytes(GenFileName(WorkingPath & RegulateFileName(TxtTaskName.Text) & ".m3u8", ".m3u8"), BackupM3U8Stream, False)
                Mute = True
                TaskDone()
                Exit Sub
            End If

            If Not RecentRedirect = "" Then
                InvokeControl(TxtURL, Sub(x) x.Text = RecentRedirect)
                RecentRedirect = ""
            End If

            ParentURL = GetParent(TxtURL.Text)
            If Not ParentURL = "" Then
                WriteLog("[Parent] " & ParentURL)
                MParentURL = ""
            End If

            InvokeControl(TxtURL, Sub(x) x.ReadOnly = True)

            If Data.StartsWith("VC") Then
                Data = Encoding.UTF8.GetString(New PLUGIN_LEM3U8Descrambler().Decrypt(BackupM3U8Stream))
            End If

            If Data.StartsWith("isenc") And IgnoreSohu = False Then
                Data = New PLUGIN_SOHUM3U8Descrambler().DecryptM3U8(Data)
            End If

            If ChkLive.Checked = False Then
                GenM3U8Path()
                My.Computer.FileSystem.WriteAllText(BackupM3U8Path, Data, False, Encoding.UTF8)
            End If

            Dim _loc_2 As Char = vbCrLf
            If Data.Contains(Chr(10)) Then
                _loc_2 = Chr(10)
            End If
            Dim _loc_1 As String() = Data.Split(_loc_2)

            DefList.Clear()
            KeyList.Clear()
            ProgramDateTimeList.Clear()
            RangeList.Clear()
            SegList.Clear()
            TimeoutList.Clear()
            TrackList.Clear()
            If RefreshURL = False Then
                MTBuffer.Clear()
                MTStatus.Clear()
            End If

            Dim NextKey As String = ""
            Dim NextIV As String = ""
            Dim NextProgramDateTime As String = ""
            Dim NextRange As String = ""
            Dim NextBandwidth As String = ""
            Dim NextResolution As String = ""
            LiveLastIndex = -1
            Dim LastSegURL As String = ""
            Dim StrEXTINF As String = ""
            Dim JumpToEXTINF As Boolean = False
            Dim PLSTHasKeyTag As Boolean = False
            Dim IsMasterPLST As Boolean = False
            Dim MasWaitForURL As Boolean = False
            '========Patch submitted by developer liuxiangBIT========
            Dim LastSegName As String = ""
            Dim NextSegName As String = ""
            Dim EntireTsURL As String = ""
            '========================================================

            If _loc_1(0).Contains("{") Then
                For Each Content In Data.Split(Chr(34))
                    If Content.StartsWith("#EXT") Then
                        My.Computer.FileSystem.WriteAllText(BackupM3U8Path, Content.Replace("\/", "/").Replace("\n", vbCrLf), False, Encoding.UTF8)
                        TxtURL.Text = "file:///" & BackupM3U8Path
                        ParseURL()
                        Exit Sub
                    End If
                Next
            End If

            If (Not Skip = "") And (Not Skip.Contains("<")) Then
                Dim _loc_4 As String = ""
                For Each _loc_3 In Skip.Split(",")
                    If _loc_3.Length > 0 Then
                        If _loc_3.Contains("-") Then
                            For _loc_5 = Int(_loc_3.Split("-")(0)) To Int(_loc_3.Split("-")(1))
                                _loc_4 += "<" & _loc_5 & ">"
                            Next
                        Else
                            _loc_4 += "<" & _loc_3 & ">"
                        End If
                    End If
                Next
                Skip = _loc_4
            End If

            Dim _loc_11 As Integer = 0
            For _loc_10 = 0 To _loc_1.Count - 1
                If Not IsMasterPLST Then
                    If _loc_1(_loc_10).Contains("#EXTINF") Or _loc_1(_loc_10).Contains("#EXT-X-MAP") Or JumpToEXTINF Then
                        JumpToEXTINF = False
                        If _loc_1(_loc_10).StartsWith("#EXTINF") Then StrEXTINF = _loc_1(_loc_10)

                        If _loc_1(_loc_10 + 1).StartsWith("#EXT") And Not _loc_1(_loc_10).StartsWith("#EXT-X-MAP") Then        'Extra information is provided for the current segment
                            If _loc_1(_loc_10 + 1).StartsWith("#EXT-X-BYTERANGE") Then
                                Try
                                    NextRange = ParseByteRange(_loc_1(_loc_10 + 1).Split(":")(1))
                                Catch ex As Exception

                                End Try
                            End If
                            JumpToEXTINF = True
                            GoTo nxt
                        End If

                        Dim NextSegURL As String = ""
                        If _loc_1(_loc_10).StartsWith("#EXT-X-MAP") Then
                            NextSegURL = Regex.Match(_loc_1(_loc_10).Replace("\/", "/"), "URI=\""(.*?)\""", RegexOptions.None).Groups(1).Value
                            Try
                                NextRange = ParseByteRange(Regex.Match(_loc_1(_loc_10), "BYTERANGE=([0-9@\""]*)", RegexOptions.None).Groups(1).Value)
                            Catch ex As Exception

                            End Try
                        Else
                            NextSegURL = _loc_1(_loc_10 + 1)
                        End If

                        Dim DoAdd As Boolean = True
                        _loc_11 += 1

                        If Not Skip = "" And Skip.Contains("<" & _loc_11 & ">") Then
                            DoAdd = False
                        End If

                        '========Patch submitted by developer liuxiangBIT========
                        Dim EntireTsAdded As Boolean = False
                        If EntireTs And (NextSegURL.Contains("iqiyi.com/") Or NextSegURL.Contains("ptqy.gitv.tv/")) Then
                            EntireTsURL = GetEntireTsURL(NextSegURL)
                            NextSegName = GetSegName(EntireTsURL)
                            If EntireTsURL.Length And NextSegName.Length Then
                                NextSegURL = EntireTsURL
                                If LastSegName = NextSegName Then
                                    DoAdd = False
                                Else
                                    LastSegName = NextSegName
                                    If Not MTTDefined Then
                                        MTThreads = 2
                                    End If
                                    EntireTsAdded = True
                                End If
                            End If
                        End If
                        '========================================================

                        Dim FullNextSegURL As String = IfAppendQuery(GetFullURL(NextSegURL))
                        If FullNextSegURL.Contains("/ad/") And (FullNextSegURL.Contains("cibntv.net/") Or FullNextSegURL.Contains("youku.com/")) And IgnoreYouku = False Then
                            DoAdd = False
                        End If

                        If DoAdd Then
                            LastSegURL = NextSegURL
                            If LiveTransfer And IfLooseMatch(LiveLastSegURL) = IfLooseMatch(NextSegURL) Then LiveLastIndex = SegList.Count

                            SegList.Add(FullNextSegURL)
                            RangeList.Add(NextRange)
                            ProgramDateTimeList.Add(NextProgramDateTime)
                            KeyList.Add(NextKey & vbTab & NextIV)
                            MTBuffer.Add(New Byte() {})
                            MTStatus.Add(0)
                        End If
                        NextRange = ""
                        NextProgramDateTime = ""

                        '========Patch submitted by developer liuxiangBIT========
                        Dim GivenTimeout As String = ""
                        If EntireTsAdded Then
                            GivenTimeout = EntireTsTimeOut
                        ElseIf StrEXTINF.Contains(":") Then
                            GivenTimeout = Regex.Match(StrEXTINF.Split(":")(1).Split(".")(0), "([0-9]*)", RegexOptions.None).Groups(1).Value
                        End If
                        '========================================================

                        If UserTimeOut > 0 Then
                            If DoAdd Then TimeoutList.Add(UserTimeOut)
                        Else
                            If IsNumeric(GivenTimeout) Then
                                Dim GivenTimeoutInt As Integer = Int(GivenTimeout) * 1000
                                Dim ExpectedTimeout As Integer = GivenTimeoutInt * 5
                                If ChkLive.Checked Then ExpectedTimeout = GivenTimeoutInt * 2
                                If ExpectedTimeout < 10000 Then
                                    If DoAdd Then TimeoutList.Add(DefaultTimeOut)
                                Else
                                    If DoAdd Then TimeoutList.Add(ExpectedTimeout)
                                End If

                                If GivenTimeoutInt <= DefaultLiveTimeOut Then
                                    DefaultLiveTimeOut = GivenTimeoutInt
                                Else
                                    LiveTimeOut = DefaultLiveTimeOut
                                End If
                            Else
                                If DoAdd Then TimeoutList.Add(DefaultTimeOut)
                            End If
                        End If
                    End If

                    If _loc_1(_loc_10).StartsWith("#EXT-X-KEY") And IgnoreKey = False Then        'The line is key information
                        PLSTHasKeyTag = True
                        If _loc_1(_loc_10).Contains("METHOD=AES-128") Then
                            'Get next Key URL
                            NextKey = GetFullURL(Regex.Match(_loc_1(_loc_10).Replace("\/", "/"), "URI=\""(.*?)\""", RegexOptions.None).Groups(1).Value)

                            'Get next IV
                            If _loc_1(_loc_10).Contains("IV=0x") Then
                                Dim n As Match = Regex.Match(_loc_1(_loc_10).Replace(" ", "").Replace("\/", "/"), "IV=0x(.*)", RegexOptions.None)
                                NextIV = n.Groups(1).Value.Trim.Substring(0, 32)
                            Else
                                NextIV = "00000000000000000000000000000000"
                            End If

                            If Not GivenKey = "" Then
                                NextKey = GivenKey
                            End If

                            If Not GivenIV = "" Then
                                NextIV = GivenIV
                            End If
                        ElseIf _loc_1(_loc_10).Contains("METHOD=NONE") Then
                            NextKey = ""
                            NextIV = ""
                        Else
                            If RefreshURL = False Then
                                StartError("ERROR_KEY_NO_SUPPORT")
                                Exit Sub
                            End If
                        End If
                    Else
                        If Not GivenKey = "" And PLSTHasKeyTag = False Then
                            NextKey = GivenKey
                            If GivenIV = "" Then
                                NextIV = "00000000000000000000000000000000"
                            Else
                                NextIV = GivenIV
                            End If
                        End If
                    End If

                    If _loc_1(_loc_10).StartsWith("#EXT-X-PROGRAM-DATE-TIME") Then
                        NextProgramDateTime = Regex.Match(_loc_1(_loc_10), "#EXT-X-PROGRAM-DATE-TIME:(.*)", RegexOptions.None).Groups(1).Value.Trim
                    End If

                    If _loc_1(_loc_10).Contains("#EXT-X-STREAM-INF") Or _loc_1(_loc_10).Contains("#EXT-X-MEDIA:") Then   'The playlist is a master playlist
                        IsMasterPLST = True
                        GoTo mas
                    End If
                Else                                                 'Master playlist handler
mas:                If MasWaitForURL Then
                        If Not _loc_1(_loc_10).StartsWith("#EXT") Then
                            DefList.Add(NextBandwidth & vbTab & NextResolution & vbTab & GetFullURL(_loc_1(_loc_10)))
                            MasWaitForURL = False
                        End If
                    Else
                        If _loc_1(_loc_10).Contains("#EXT-X-STREAM-INF") Then
                            Dim MatchB As Match = Regex.Match(_loc_1(_loc_10), "BANDWIDTH=([0-9]*)", RegexOptions.None)
                            Dim MatchR As Match = Regex.Match(_loc_1(_loc_10), "RESOLUTION=([0-9x]*)", RegexOptions.None)
                            Dim ValueB As String = ""
                            Dim ValueR As String = ""

                            Try
                                ValueB = MatchB.Groups(1).Value
                            Catch ex As Exception

                            End Try

                            Try
                                ValueR = MatchR.Groups(1).Value
                            Catch ex As Exception

                            End Try

                            NextBandwidth = ValueB
                            NextResolution = ValueR

                            MasWaitForURL = True
                        End If

                        If _loc_1(_loc_10).Contains("#EXT-X-MEDIA:") Then
                            Try
                                Dim MatchT As String = Regex.Match(_loc_1(_loc_10), "TYPE=([0-9A-Za-z]*)", RegexOptions.None).Groups(1).Value
                                Dim MatchN As String = Regex.Match(_loc_1(_loc_10), "NAME=\""(.*?)\""", RegexOptions.None).Groups(1).Value
                                Dim MatchU As String = Regex.Match(_loc_1(_loc_10).Replace("\/", "/"), "URI=\""(.*?)\""", RegexOptions.None).Groups(1).Value
                                TrackList.Add("类型 " & MatchT & ", 名称 " & MatchN & vbTab & GetFullURL(MatchU))
                            Catch ex As Exception

                            End Try
                        End If
                    End If
                End If
nxt:        Next

            If IsMasterPLST Then
                If DefList.Count <= 0 And TrackList.Count <= 0 Then
                    If RefreshURL = False Then
                        StartError("ERROR_NO_HLS_DATA_MASTER")
                    End If
                    Exit Sub
                End If

                'Sort definition list
                Dim BList As New List(Of String)
                For Each stream In DefList
                    Dim B As String = stream.Split(vbTab)(0)
                    If Not IsNumeric(B) Then
                        B = 0
                    End If
                    BList.Add(Int(B))
                Next

                Dim n As Integer = DefList.Count
                For i = 0 To n Step 1
                    For j = n - 1 To i + 1 Step -1
                        If (Int(BList(j)) < Int(BList(j - 1))) Then
                            Dim TempB As String = BList(j)
                            BList(j) = BList(j - 1)
                            BList(j - 1) = TempB
                            Dim TempDef As String = DefList(j)
                            DefList(j) = DefList(j - 1)
                            DefList(j - 1) = TempDef
                        End If
                    Next
                Next

                If AutoSelect = -2 Then
                    MasterPlaylist.ShowDialog()
                Else
                    MasSelect(AutoSelect)
                End If
                Exit Sub
            End If

            If RefreshURL = False Then
                If SegList.Count <= 0 Then
                    If ChkLive.Checked And LblSpeed.Visible Then
                        ErrCount += 1
                        WriteLog("[Live] No segment in playlist.")
                        WriteLog("Error count: " & ErrCount)
                        Thread.Sleep(1000)
                        ParseLive()
                    Else
                        If LiveTransfer Then
                            Comb()
                        Else
                            If (AutoStart And AutoExit) And (ErrCount < MaxErrCount) Then
                                ErrCount += 1
                                WriteLog("Error count: " & ErrCount)
                                Thread.Sleep(1000)
                                ParseSingleM3U8File(Data)
                                Exit Sub
                            Else
                                StartError("ERROR_NO_HLS_DATA")
                            End If
                        End If
                    End If
                Else
                    LiveLastSegURL = LastSegURL
                    If LiveTransfer Then
                        CheckLive()
                    Else
                        BeginDownloadVOD()
                    End If
                End If
            Else
                InvokeControl(LblProgress, Sub(x) x.Text = "分析完成")
            End If
        Catch ex As Exception
            WriteLog(ex.ToString)
            If RefreshURL = False Then
                If LiveTransfer Then
                    ErrCount += 1
                    WriteLog("[Live] Cannot parse playlist.")
                    WriteLog("Error count: " & ErrCount)
                    Thread.Sleep(1000)
                    ParseLive()
                Else
                    StartError("ERR_PARSE")
                End If
            End If
        End Try
    End Sub

    '========Patch submitted by developer liuxiangBIT========
    Public Function GetEntireTsURL(Input As String)
        Dim _loc_1 As String() = Input.Split("?")
        Dim _loc_2 As String = ""
        If _loc_1.Count = 2 Then
            Dim _loc_3 As String() = _loc_1(1).Split(Chr(38))
            For _loc_4 As Integer = 0 To _loc_3.Count - 1
                If _loc_3(_loc_4).StartsWith("start=") Or _loc_3(_loc_4).StartsWith("end=") Or _loc_3(_loc_4).StartsWith("contentlength=") Then

                ElseIf _loc_2.Length Then
                    _loc_2 = _loc_2 & Chr(38) & _loc_3(_loc_4)
                Else
                    _loc_2 = _loc_3(_loc_4)
                End If
            Next
            Return _loc_1(0) & "?" & _loc_2
        Else
            Return ""
        End If
    End Function

    Public Function GetSegName(Input As String)
        Dim _loc_1 As String() = Input.Split("?")(0).Split("/")
        If _loc_1(_loc_1.Count - 1).EndsWith(".ts") Or _loc_1(_loc_1.Count - 1).EndsWith(".265ts") Then
            Return _loc_1(_loc_1.Count - 1)
        End If
        Return ""
    End Function
    '========================================================

    Private Sub StartError(ErrorCode As String)
        Try
            If StartErrorExit Then
                If Not Mute Then PlaySound("Attention")
                Dispose()
                End
            Else
                Dim ErrorMessage As String = ""
                If ErrorCode = "ERR_PARSE" Then
                    ErrorMessage = "M3U8列表解析失败！"
                ElseIf ErrorCode = "ERROR_NO_HLS_DATA" Then
                    ErrorMessage = "无法从您的链接获取有效HLS数据！"
                ElseIf ErrorCode = "ERROR_NO_HLS_DATA_MASTER" Then
                    ErrorMessage = "M3U8大师列表中不存在有效HLS数据！"
                ElseIf ErrorCode = "ERROR_NO_HLS_DATA_URL" Then
                    ErrorMessage = "已检测到您的链接，但无法从该链接中获取有效HLS数据！"
                ElseIf ErrorCode = "ERROR_KEY_NO_SUPPORT" Then
                    ErrorMessage = "M3U8列表解析成功。不支持当前视频的加密方式！"
                End If
                MsgBox(ErrorMessage, vbExclamation, Text)
                EnableControls()
            End If
        Catch ex As Exception
            WriteLog(ex.ToString)
        End Try
    End Sub

    Public Sub MasSelect(idx As Integer)
        Try
            If idx = -1 Then
                idx = DefList.Count - 1
            End If

            If idx >= DefList.Count Then
                TxtURL.Text = IfAppendQuery(TrackList(idx - DefList.Count).Split(vbTab)(1))
            Else
                TxtURL.Text = IfAppendQuery(DefList(idx).Split(vbTab)(2))
            End If

            If My.Application.OpenForms.Item("MasterPlaylist") IsNot Nothing Then MasterPlaylist.Dispose()
            ParseURL()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub CheckLive()
        Try
            For _loc_1 = 0 To SegList.Count - 1
                If (LiveLastProgramDateTime = ProgramDateTimeList(_loc_1) And Not ProgramDateTimeList(_loc_1) = "") Or _loc_1 = LiveLastIndex Then
                    If _loc_1 = SegList.Count - 1 Then
                        LiveWait()
                        Exit Sub
                    Else
                        DLIndex = _loc_1 + 1
                        BeginDownloadLive()
                        Exit Sub
                    End If
                End If
            Next
            DLIndex = 0
            ShowLiveLostWarning()
            LiveInterruptionCount += 1
            WriteLog("======== Live stream interruption #" & LiveInterruptionCount & " ========" & vbCrLf & "[PREVIOUS] " & LiveLastFullURL & vbCrLf & "[NEXT] " & SegList(0))
            If ChkLive.Checked Then
                BeginDownloadLive()
            Else
                Comb()
            End If
        Catch ex As Exception
            WriteLog(ex.ToString)
        End Try
    End Sub

    Private Sub LiveWait()
        InvokeControl(LblProgress, Sub(x) x.Text = "挂起...")
        Thread.Sleep(LiveTimeOut)
        If LiveStop = False Then
            ParseSingleM3U8File(GetWebpage(TxtURL.Text))
        End If
    End Sub

    Private Sub ResetLiveIntervalStopwatch()
        Try
            If LiveIntervalStopWatch.IsRunning Then
                LiveIntervalStopWatch.Stop()
                LiveIntervalStopWatch.Reset()
            End If
            LiveIntervalStopWatch.Start()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub ShowLiveLostWarning()
        InvokeControl(LblLiveInterrupt, Sub(x) x.Visible = True)
    End Sub

    Private Sub HideLiveLostWarning()
        InvokeControl(LblLiveInterrupt, Sub(x) x.Visible = False)
    End Sub

    Private Sub BeginDownloadLive()
        Try
            DLDoneAll = False
            TaskDoneAll = False
            MTControllerInUse = False
            InvokeControl(LblWorking, Sub(x) x.Visible = True)
            ResetLiveIntervalStopwatch()
            MTController()
        Catch ex As Exception
            WriteLog(ex.ToString)
        End Try
    End Sub

    Private Sub BeginDownloadVOD()
        Try
            DLIndex = 0
            For _loc_1 = 0 To MTStatus.Count - 1
                MTStatus(_loc_1) = 0
            Next
            DLDoneAll = False
            TaskDoneAll = False
            MTControllerInUse = False
            InvokeControl(LblWorking, Sub(x) x.Visible = True)
            TempPath = WorkingPath & TempFolderName & "\" & "STRM_" & System.DateTime.Now.ToString("HHmmss") & GetRndString(5) & ".ts"
            WriteLog("Creating cache file " & TempPath)
            MTController()
        Catch ex As Exception
            WriteLog(ex.ToString)
        End Try
    End Sub

    Private Sub MTController()
        If MTControllerInUse Then
            MTControllerQueue = True
            Exit Sub
        End If
        MTControllerInUse = True
        Try
            If AutoPause Then
                AutoPause = False
                If PauseNow = False Then Pause()
            End If

            If TaskDoneAll Or DLDoneAll Or LiveStop Or PauseNow Then
                MTControllerInUse = False
                Exit Sub
            End If

            If ChkLive.Checked And ChkLive.Enabled And CheckboxInHandle = False Then
                InvokeControl(ChkLive, Sub(x) x.Enabled = False)
                InvokeControl(ChkTwoPass, Sub(x) x.Enabled = False)
                InvokeControl(BtnPause, Sub(x) x.Text = "停止(&P)")
                ResetLiveIntervalStopwatch()
                LiveTimerStopwatch.Reset()
                LiveTimerStopwatch.Start()
            End If

            If ChkLive.Checked = True And ChkLive.Enabled = True And CheckboxInHandle = False And ErrCount > 3 Then
                ErrCount = 0
                ParseLive()
                MTControllerInUse = False
                Exit Sub
            End If

            If ErrCount >= MaxErrCount And LiveTransfer = False Then
                WriteLog("[Task failure] Maximum number of retries reached.")
                ErrCount = 0
                If PauseNow = False Then Pause()
                If Not Mute Then PlaySound("Attention")
                MTControllerInUse = False
                If ErrorExit Then
                    Dispose()
                    End
                End If
                Exit Sub
            Else
                Do
                    If DLIndex >= SegList.Count Then Exit Do
                    If MTStatus(DLIndex) < 10 Then Exit Do
                    Try
                        My.Computer.FileSystem.WriteAllBytes(TempPath, MTBuffer(DLIndex), True)
                        MTBuffer(DLIndex) = New Byte() {}
                        DLIndex += 1
                    Catch ex As Exception
                        WriteLog(ex.ToString)
                        MTBuffer(DLIndex) = New Byte() {}
                        MTStatus(DLIndex) = 0
                    End Try
                Loop

                If MTSpeedStopWatch.IsRunning And (MTSpeedStopWatch.Elapsed.TotalMilliseconds >= 1000) Then
                    MTSpeedStopWatch.Stop()
                    MTSpeed = Math.Round((((MTSpeedCounter / 1048576) / MTSpeedStopWatch.Elapsed.TotalMilliseconds) * 1000), 2)
                    MTSpeedStopWatch.Reset()
                    MTSpeedCounter = 0
                End If
                InvokeControl(LblProgress, Sub(x) x.Visible = True)
                InvokeControl(LblSpeed, Sub(x) x.Visible = True)
                Dim CurrentSeg As Integer = Int(DLIndex)
                Dim CountSeg As Integer = Int(SegList.Count)
                Dim PercentSeg As Decimal = Math.Round((CurrentSeg) / CountSeg * 100, 2)
                InvokeControl(LblProgress, Sub(x) x.Text = CurrentSeg & "/" & CountSeg)
                InvokeControl(LblSpeed, Sub(x) x.Text = MTSpeed & " MB/s")
                Dim TitleTaskName As String = ""
                If Not TxtTaskName.Text = "" Then TitleTaskName = " - " & TxtTaskName.Text
                InvokeControl(Me, Sub(x) x.Text = GetPercentageLabel(PercentSeg) & TitleTaskName)
                If MTSpeedStopWatch.IsRunning = False Then MTSpeedStopWatch.Start()

                If DLIndex < SegList.Count Then
                    MTIndex = DLIndex
                    Do
                        If MTIndex >= SegList.Count Then Exit Do
                        If MTWorker >= MTThreads Then Exit Do
                        If MTIndex >= (DLIndex + MTThreads) Then Exit Do
                        If MTStatus(MTIndex) = 0 Then
                            MTStatus(MTIndex) = 1
                            Dim _loc_2 As New Threading.Thread(AddressOf DLSeg)
                            _loc_2.Start(MTIndex)
                        End If
                        MTIndex += 1
                    Loop
                Else
                    If DLDoneAll Then
                        MTControllerInUse = False
                        Exit Sub
                    End If
                    DLDoneAll = True
                    DLDone()
                End If
            End If
        Catch ex As Exception
            WriteLog(ex.ToString)
        Finally
            MTControllerInUse = False
            If MTControllerQueue Then
                MTControllerQueue = False
                MTController()
            End If
        End Try
    End Sub

    Private Function GetPercentageLabel(Percentage As Decimal) As String
        Try
            If LiveTimerStopwatch.IsRunning Then
                Dim _loc_1 As TimeSpan = TimeSpan.FromTicks(LiveTimerStopwatch.ElapsedTicks)
                Return String.Format("{0}:{1}:{2}", _loc_1.Hours.ToString().PadLeft(2, "0"), _loc_1.Minutes.ToString().PadLeft(2, "0"), _loc_1.Seconds.ToString().PadLeft(2, "0"))
            End If
            Return Percentage & "%"
        Catch ex As Exception
            Return Percentage & "%"
        End Try
    End Function

    Private Sub DLSeg(StreamIndex As Integer)
        Try
            MTWorker += 1
            WriteLog("[Segment " & StreamIndex & "] Download begin.")
            Dim _loc_1 As Byte() = GetWebpage(SegList(StreamIndex), "Bytes", RangeList(StreamIndex), "[Segment " & StreamIndex & "] ", StreamIndex)

            If ParseJSON Then
                If _loc_1(0) = &H7B And _loc_1(_loc_1.Length - 1) = &H7D Then
                    For Each _loc_7 In Encoding.UTF8.GetString(_loc_1).Replace("\/", "/").Split(Chr(34))
                        If _loc_7.StartsWith("http://") Or _loc_7.StartsWith("https://") Then
                            WriteLog("[Segment " & StreamIndex & "] JSON parsed.")
                            _loc_1 = GetWebpage(_loc_7, "Bytes", RangeList(StreamIndex), "[Segment " & StreamIndex & "] ", StreamIndex)
                            Exit For
                        End If
                    Next
                End If

                If _loc_1(0) = &H1F And _loc_1(1) = &H8B Then
                    _loc_1 = UnZip(_loc_1, True)
                End If
            End If

            If TaskDoneAll Or LiveStop Then Exit Sub

            If Seg2TS Then
                _loc_1 = SegToTS(_loc_1)
            End If

            MTSpeedCounter += _loc_1.Length

            If _loc_1.Length = 4 And Encoding.UTF8.GetString(_loc_1) = "null" Then
                MTStatus(StreamIndex) = 0
                DLErrVOD()
                Exit Sub
            End If

            WriteLog("[Segment " & StreamIndex & "] Fetched " & Math.Round(_loc_1.Length / 1048576, 2) & " MB.")

            'AES Decrypt
            If Not KeyList(StreamIndex) = vbTab Then                                'Segment is encrypted
                If LastKeyURL = KeyList(StreamIndex).Split(vbTab)(0) Then           'Key URI same as the previous one, no need to fetch again

                Else
                    Dim _loc_2 As Byte() = Nothing                                 'Variable for storing fetched Key bytes from a URI

                    If KeyList(StreamIndex).Split(vbTab)(0).ToLower.StartsWith("http") Or KeyList(StreamIndex).Split(vbTab)(0).ToLower.StartsWith("file") Then 'Need to fetch Key bytes from a URI
                        For _loc_3 = 1 To 3
                            _loc_2 = GetWebpage(KeyList(StreamIndex).Split(vbTab)(0), "Bytes")
                            If _loc_2.Length = 16 Then
                                Exit For
                            End If
                        Next
                    Else
                        LastKey = HexToBytes(KeyList(StreamIndex).Split(vbTab)(0)) 'Key bytes are local hex values
                        _loc_2 = HexToBytes("00")
                    End If

                    If _loc_2.Length > 16 Then
                        Dim _loc_7 As Byte() = New Byte(15) {}
                        Array.Copy(_loc_2, _loc_7, 16)
                        _loc_2 = _loc_7
                    End If

                    If _loc_2.Length = 16 Then
                        'Got Key
                        LastKeyURL = KeyList(StreamIndex).Split(vbTab)(0)
                        LastKey = _loc_2
                    End If
                End If

                _loc_1 = AES_Decrypt(_loc_1, LastKey, HexToBytes(KeyList(StreamIndex).Split(vbTab)(1).Substring(0, 32).ToUpper))
                WriteLog("[Segment " & StreamIndex & "] Decrypted.")
            End If

            If Not PLUGIN_DeYK = "" And IgnoreYouku = False Then
                If PLUGIN_DeYK.Contains(",") Then
                    Dim _loc_4 As String = ""
                    For Each _loc_5 In PLUGIN_DeYK.Replace("，", ",").Replace("{", "").Replace("}", "").Replace("[", "").Replace("]", "").Replace(" ", "").Split(",")
                        Dim _loc_6 As String = Hex(Int(_loc_5.Trim))
                        If _loc_6.Length = 1 Then _loc_4 += "0"
                        _loc_4 += _loc_6
                    Next
                    PLUGIN_DeYK = _loc_4
                Else
                    If Not PLUGIN_DeYK.Length = 32 Then
                        Try
                            PLUGIN_DeYK = BytesToHex(Convert.FromBase64String(PLUGIN_DeYK.Trim.Replace(Chr(34), "")))
                        Catch ex As Exception
                            PLUGIN_DeYK = PLUGIN_DeYK.Trim.Replace(" ", "").ToUpper
                        End Try
                    End If
                End If
                If PLUGIN_DeYK.Length = 32 Then
                    WriteLog("[Segment " & StreamIndex & "] [DeYK] Key=0x" & PLUGIN_DeYK)
                    _loc_1 = New PLUGIN_DeYK().DecryptData(_loc_1, PLUGIN_DeYK.Trim)
                End If
            End If

            MTBuffer(StreamIndex) = _loc_1
            MTStatus(StreamIndex) = 10
            WriteLog("[Segment " & StreamIndex & "] Download ended.")
            InvokeControl(LblDownError, Sub(x) x.Visible = False)
            ErrCount = 0
            MTWorker -= 1
            If PauseNow = False Then MTController()
        Catch ex As Exception
            WriteLog("[Segment " & StreamIndex & "] " & ex.ToString)
            If MTStatus(StreamIndex) < 10 Then MTStatus(StreamIndex) = 0
            DLErrVOD()
        End Try
    End Sub

    Private Sub DLErrVOD()
        Try
            If TaskDoneAll Or LiveStop Then Exit Sub
            ErrCount += 1
            WriteLog("Error count: " & ErrCount)
            InvokeControl(LblDownError, Sub(x) x.Visible = True)
            MTWorker -= 1
            If PauseNow = False Then MTController()
        Catch ex As Exception
            WriteLog(ex.ToString)
        End Try
    End Sub

    Private Sub DLDone()
        Try
            If TaskDoneAll Or LiveStop Then Exit Sub
            RefreshURL = False
            If ChkLive.Checked Then
                LiveLastFullURL = SegList(SegList.Count - 1)
                LiveLastProgramDateTime = ProgramDateTimeList(ProgramDateTimeList.Count - 1)
                LiveTransfer = True
                ErrCount = 0
                ParseLive()
            ElseIf ChkTwoPass.Checked = True Then
                If ChkTwoPass.Enabled = True Then      'First pass downloaded
                    InvokeControl(ChkTwoPass, Sub(x) x.Enabled = False)
                    FirstPassPath = TempPath
                    WriteLog("======== Begin 2-Pass ========")
                    BeginDownloadVOD()
                Else                                   'Second pass downloaded
                    InvokeControl(ChkTwoPass, Sub(x) x.Enabled = True)
                    Dim FLength1 As Long = New FileInfo(FirstPassPath).Length
                    Dim FLength2 As Long = New FileInfo(TempPath).Length
                    If FLength1 = FLength2 Then          'Second pass matched
                        WriteLog("[2-Pass verification succeed] File matched. Size: " & Math.Round(FLength1 / 1048576, 2) & " MB")
                        Comb()
                    Else                               'Third pass required
                        WriteLog("[2-Pass verification failure] Pass 1: " & Math.Round(FLength1 / 1048576, 2) & " MB, Pass 2: " & Math.Round(FLength2 / 1048576, 2) & " MB.")
                        WriteLog("[2-Pass verification failure] Reset.")
                        If AutoClean Then
                            Clean()
                        End If
                        HideLiveLostWarning()
                        InvokeControl(LblFileDamage, Sub(x) x.Visible = True)
                        BeginDownloadVOD()
                    End If
                End If
            Else
                Comb()
            End If
        Catch ex As Exception
            WriteLog(ex.ToString)
            MsgBox("下载成功。执行后续工作时出现错误！", vbExclamation, Text)
            InvokeControl(LblProgress, Sub(x) x.Visible = False)
            InvokeControl(LblSpeed, Sub(x) x.Visible = False)
        End Try
    End Sub

    Private Function SegToTS(Input As Byte()) As Byte()
        Try
            Dim _loc_1 As String = WorkingPath & TempFolderName & "\" & "STRM_" & System.DateTime.Now.ToString("HHmmss") & GetRndString(5) & ".dat"
            Dim _loc_2 As String = _loc_1 & ".ts"
            My.Computer.FileSystem.WriteAllBytes(_loc_1, Input, False)
            Dim FFmpegProcess As New Process()
            FFmpegProcess.StartInfo.FileName = "ffmpeg.exe"
            FFmpegProcess.StartInfo.Arguments = "-i " & Chr(34) & _loc_1 & Chr(34) & " -c copy -metadata service_name=""HTTP Live Streaming"" -metadata service_provider=""MPEG-2 Transport Stream"" -mpegts_service_type 0xBF -pat_period 1 -sdt_period 1 -y " & Chr(34) & _loc_2 & Chr(34)
            FFmpegProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
            FFmpegProcess.Start()
            FFmpegProcess.WaitForExit()
            FFmpegProcess.Close()
            FFmpegProcess.Dispose()
            Dim _loc_3 As Byte() = New Byte() {}
            If My.Computer.FileSystem.FileExists(_loc_2) Then
                _loc_3 = My.Computer.FileSystem.ReadAllBytes(_loc_2)
            Else
                _loc_3 = Input
            End If
            My.Computer.FileSystem.DeleteFile(_loc_1)
            My.Computer.FileSystem.DeleteFile(_loc_2)
            If _loc_3.Length >= 188 * 4 Then
                Return _loc_3
            Else
                Return Input
            End If
        Catch ex As Exception
            Return Input
        End Try
    End Function

    Private Sub Comb()
        Try
            If TaskDoneAll Then Exit Sub
            InvokeControl(TxtTaskName, Sub(x) x.ReadOnly = True)
            InvokeControl(LblSpeed, Sub(x) x.Visible = False)

            If Not My.Computer.FileSystem.FileExists(TempPath) Or My.Computer.FileSystem.GetFileInfo(TempPath).Length <= ValidSize Then
                TaskFail = True
                CombFail()
                Exit Sub
            End If

            WriteLog("[Finalizing] Start packaging process.")
            HideControls()

            InvokeControl(LblProgress, Sub(x) x.Text = "正在封装...")
            Dim FFArgAppend1 As String = ""
            Dim FFArgAppend2 As String = ""
            Dim FileExt As String = ".mp4"
            ExpectedOutputFilename = WorkingPath & RegulateFileName(TxtTaskName.Text)

            If RadMP4.Checked = True Then
                'MP4
                FileExt = ".mp4"
                ExpectedOutputFilename += FileExt
                If Not DoNotGetDol Then IsDol = GetDol()
                If Not IsDol Then FFArgAppend2 = "-bsf:a aac_adtstoasc"
                If Not EncodingTool = "" Then
                    If Not FFArgAppend2 = "" Then FFArgAppend2 &= " "
                    FFArgAppend2 &= "-metadata:g encoding_tool=" & Chr(34) & EncodingTool & Chr(34)
                End If
            Else
                'TS
                FileExt = ".ts"
                ExpectedOutputFilename += FileExt
                FFArgAppend2 = "-metadata service_name=" & Chr(34) & "HTTP Live Streaming" & Chr(34) & " -metadata service_provider=" & Chr(34) & "MPEG-2 Transport Stream" & Chr(34) & " -mpegts_service_type 0xBF -pat_period 1 -sdt_period 1"
            End If

            ExpectedOutputFilename = GenFileName(ExpectedOutputFilename, FileExt)

            WriteLog("Creating output file " & ExpectedOutputFilename)

            InvokeControl(Me, Sub(x) x.Text = GetPercentageLabel(100) & " - " & Path.GetFileName(ExpectedOutputFilename))

            If RadBinary.Checked = True Then
                My.Computer.FileSystem.MoveFile(TempPath, ExpectedOutputFilename)
                TaskDone()
                Exit Sub
            End If

            FFOutputPath = ExpectedOutputFilename
            If Not FFmpegAppend = "" And Not FFmpegAppend.EndsWith(" ") And FFmpegAppend.Length > 2 Then
                FFmpegAppend += " "
            End If

            FFArg = "-i " & Chr(34) & TempPath & Chr(34) & " " & FFmpegAppend & "-c copy -y " & FFArgAppend2 & " " & Chr(34) & FFOutputPath & Chr(34)
            WriteLog("[FFmpeg] " & FFArg)
            VideoWorker.RunWorkerAsync()
        Catch ex As Exception
            WriteLog(ex.ToString)
            CombFail()
        End Try
    End Sub

    Private Function GenFileName(ExpectedPath As String, FileExtension As String) As String
        If TxtTaskName.Text = "" Or File.Exists(ExpectedPath) Or Directory.Exists(ExpectedPath) Or Not IsValidFileNameOrPath(ExpectedPath) Then
            Return WorkingPath & Format(Now(), "yyyyMMddHHmmss") & GetRndString(4) & FileExtension
        Else
            Return ExpectedPath
        End If
    End Function

    Private Function RegulateFileName(FileName As String)
        Return FileName.Replace("/", "_").Replace("\", "_").Replace("*", "_").Replace("?", "_").Replace(":", "_").Replace("<", "_").Replace(">", "_").Replace(Chr(34), "_").Replace("|", "_")
    End Function

    Private Sub Clean()
        Try
            If DoNotClean Then
                DoNotClean = False
                Exit Sub
            End If

            If My.Computer.FileSystem.FileExists(TempPath) Then
                WriteLog("Removing cache file " & TempPath)
                My.Computer.FileSystem.DeleteFile(TempPath)
            End If

            If My.Computer.FileSystem.FileExists(FirstPassPath) Then
                WriteLog("Removing cache file " & FirstPassPath)
                My.Computer.FileSystem.DeleteFile(FirstPassPath)
            End If
        Catch ex As Exception
            WriteLog(ex.ToString)
        End Try
    End Sub

    Private Sub VideoWorker_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles VideoWorker.DoWork
        Try
            Dim FFmpegProcess As New Process()
            FFmpegProcess.StartInfo.FileName = "ffmpeg.exe"
            FFmpegProcess.StartInfo.Arguments = FFArg
            FFmpegProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
            FFmpegProcess.Start()
            FFmpegProcess.WaitForExit()
            FFmpegProcess.Close()
            FFmpegProcess.Dispose()
        Catch ex As Exception
            WriteLog(ex.ToString)
            CombFail()
        End Try
    End Sub

    Private Sub VideoWorker_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles VideoWorker.RunWorkerCompleted
        Try
            If My.Computer.FileSystem.FileExists(FFOutputPath) Then
                If My.Computer.FileSystem.GetFileInfo(FFOutputPath).Length <= ValidSize Then
                    WriteLog("Packaged file is abnormally small.")
                    CombFail()
                Else
                    TaskDone()
                End If
            Else
                WriteLog("Packaged file does not exist.")
                CombFail()
            End If
        Catch ex As Exception
            WriteLog(ex.ToString)
            CombFail()
        End Try
    End Sub

    Private Sub CombFail()
        Try
            If Not IsDol Then
                IsDol = True
                DoNotGetDol = True
                WriteLog("Packaging error, retrying once...")
                Comb()
                Exit Sub
            End If

            DoNotClean = True
            Try
                My.Computer.FileSystem.MoveFile(TempPath, ExpectedOutputFilename, True)
            Catch ex As Exception

            End Try
            TaskDone()
            WriteLog("Packaging error, abort.")
            If (My.Computer.FileSystem.FileExists(FFOutputPath) And My.Computer.FileSystem.GetFileInfo(FFOutputPath).Length <= ValidSize) Or TaskFail = True Then
                InvokeControl(LblDone, Sub(x) x.BackColor = Color.Red)
                InvokeControl(LblDone, Sub(x) x.Text = "任务失败")
            Else
                MsgBox("封装失败！已将下载的数据RAW输出。", vbExclamation, Text)
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub TaskDone()
        Try
            TaskDoneAll = True
            HideLiveLostWarning()
            HideControls()
            InvokeControl(LblWorking, Sub(x) x.Visible = False)
            InvokeControl(LblDownError, Sub(x) x.Visible = False)
            InvokeControl(LblFileDamage, Sub(x) x.Visible = False)
            InvokeControl(LblDone, Sub(x) x.Visible = True)
            InvokeControl(LblProgress, Sub(x) x.Visible = False)
            InvokeControl(LblSpeed, Sub(x) x.Visible = False)
            InvokeControl(ChkLive, Sub(x) x.Enabled = False)
            InvokeControl(ChkTwoPass, Sub(x) x.Enabled = False)
            LiveTimerStopwatch.Reset()
            WriteLog("[Task completion] Done.")

            If Not PostExecution = "" Then
                Try
                    Dim PostExecutionProcess As New Process()
                    PostExecutionProcess.StartInfo.FileName = PostExecution
                    PostExecutionProcess.StartInfo.Arguments = PostExecutionArguments
                    PostExecutionProcess.Start()
                    PostExecutionProcess.WaitForExit()
                    PostExecutionProcess.Close()
                Catch ex As Exception

                End Try
            End If

            If Not Mute Then PlaySound("Success")

            If AutoClean Then Clean()

            If AutoExit Then
                Dispose()
                End
                Exit Sub
            End If
        Catch ex As Exception
            WriteLog(ex.ToString)
        End Try
    End Sub

    Private Sub PlaySound(FileName As String)
        Try
            Dim _loc_1 As System.Reflection.Assembly = System.Reflection.Assembly.GetExecutingAssembly()
            Dim _loc_2 As System.IO.Stream = _loc_1.GetManifestResourceStream("HmDX." & FileName & ".wav")
            Dim _loc_3 As New SoundPlayer(_loc_2)
            _loc_3.Play()
            _loc_3.Dispose()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub SetOption(Opts As String, Optional FromCmd As Boolean = False)
        Try
            If FromCmd = False And Not Opts = "" Then
                WriteLog("[Set options] " & Opts)
            End If
            Cookie = ""
            Referer = ""
            UserAgent = ""

            For Each _loc_1 In Opts.Split("|")
                Dim _loc_2 As String = _loc_1
                If _loc_1.Split(">").Count = 1 Then
                    If _loc_1.StartsWith("!") Then
                        _loc_2 = _loc_1.Substring(1, _loc_1.Length - 1) & ">False"
                    Else
                        _loc_2 = _loc_1 & ">True"
                    End If
                End If
                If _loc_2.Split(">").Count > 1 Then
                    Dim Item As String = _loc_2.Split(">")(0).ToLower.Trim
                    Dim Value As String = _loc_2.Split(">")(1).Replace("<g", ">").Replace("<l", "<").Replace("<s", "|").Replace("<r", GetRndString(16)).Trim
                    If Item = "autoexit" Then
                        AutoExit = StringToBool(Value)
                    ElseIf Item = "autopause" Then
                        If RedoSetOption = False Then AutoPause = StringToBool(Value)
                    ElseIf Item = "autorestart" Then
                        AutoRestart = Int(Value)
                    ElseIf Item = "timeout" Then
                        UserTimeOut = Int(Value)
                    ElseIf Item = "livetimeout" Then
                        LiveTimeOut = Int(Value)
                    ElseIf Item = "liveinterval" Then
                        LiveInterval = Int(Value)
                    ElseIf Item = "errorcount" Then
                        MaxErrCount = Int(Value)
                    ElseIf Item.StartsWith("tempfolder") Then
                        If RedoSetOption = False And Not Value = "" Then
                            TempFolderName = Value
                        End If
                    ElseIf Item = ":" Then
                        If PauseNow Then
                            RefreshURL = True
                            TxtURL.Text = Value
                            ParseURL()
                        End If
                    ElseIf Item = "host" Then
                        Host = Value
                    ElseIf Item = "proxy" Then
                        Proxy = Value
                    ElseIf Item = "skip" Then
                        Skip = Value
                    ElseIf Item = "deyk" Then
                        If Not Value = "" Then
                            PLUGIN_DeYK = Value
                            IgnoreKey = True
                        End If
                    ElseIf Item = "tb" Then
                        PLUGIN_ThousandBillion = StringToBool(Value)
                    ElseIf Item = "islive" Then
                        If FromCmd Then ChkLive.Checked = StringToBool(Value)
                    ElseIf Item = "liveoverride" Then
                        ChkLive.Checked = StringToBool(Value)
                    ElseIf Item = "twopass" Then
                        If FromCmd Or EmptyCmd Then ChkTwoPass.Checked = StringToBool(Value)
                    ElseIf Item = "ignorekey" Then
                        IgnoreKey = StringToBool(Value)
                    ElseIf Item = "ignoresize" Then
                        IgnoreSize = StringToBool(Value)
                    ElseIf Item = "errorexit" Then
                        ErrorExit = StringToBool(Value)
                    ElseIf Item = "starterrorexit" Then
                        StartErrorExit = StringToBool(Value)
                    ElseIf Item = "liveapi" Then
                        LiveAPI = StringToBool(Value)
                    ElseIf Item = "ignoreyouku" Then
                        IgnoreYouku = StringToBool(Value)
                    ElseIf Item = "ignoresohu" Then
                        IgnoreSohu = StringToBool(Value)
                    ElseIf Item = "mgtv" Then
                        IsMGTV = StringToBool(Value)
                    ElseIf Item = "entirets" Then
                        EntireTs = StringToBool(Value)
                    ElseIf Item = "parsejson" Then
                        ParseJSON = StringToBool(Value)
                    ElseIf Item = "segtots" Then
                        Seg2TS = StringToBool(Value)
                    ElseIf Item = "noproxy" Then
                        NoProxy = StringToBool(Value)
                    ElseIf Item = "nostream" Then
                        NoStream = StringToBool(Value)
                    ElseIf Item = "appendquery" Then
                        AppendQuery = StringToBool(Value)
                    ElseIf Item = "loosematch" Then
                        LooseMatch = StringToBool(Value)
                    ElseIf Item = "autoselect" Then
                        If Value.ToLower() = "hi" Or Value.ToLower() = "high" Or Value.ToLower() = "best" Then
                            AutoSelect = -1
                        ElseIf Value.ToLower() = "lo" Or Value.ToLower() = "low" Or Value.ToLower() = "worst" Then
                            AutoSelect = 0
                        Else
                            AutoSelect = Int(Value - 1)
                        End If
                    ElseIf Item = "url" Then
                        If FromCmd Then
                            TxtURL.Text = Value
                        End If
                    ElseIf Item = "useragent" Then
                        UserAgent = Value
                    ElseIf Item.StartsWith("cookie") Then
                        Cookie = Value
                    ElseIf Item = "referer" Then
                        Referer = Value
                    ElseIf Item = "parent" Then
                        MParentURL = GetParent(Value, True)
                    ElseIf Item = "ffmpegappend" Then
                        FFmpegAppend = Value
                    ElseIf Item.StartsWith("yspl") Then
                        YangShiPinTimeMachineLength = Int(Value)
                    ElseIf Item = "postexecution" Then
                        PostExecution = Value
                    ElseIf Item = "postexecutionarguments" Then
                        PostExecutionArguments = Value
                    ElseIf Item = "key" Then
                        GivenKey = KeyRegulation(Value.ToUpper)
                    ElseIf Item = "iv" Then
                        GivenIV = KeyRegulation(Value.ToUpper)
                    ElseIf Item = "key64" Or Item = "k64" Then
                        GivenKey = BytesToHex(Convert.FromBase64String(Value)).ToUpper
                    ElseIf Item = "iv64" Or Item = "i64" Then
                        GivenIV = BytesToHex(Convert.FromBase64String(Value)).ToUpper
                    ElseIf Item = "taskname" Then
                        If FromCmd Then TxtTaskName.Text = Value
                    ElseIf Item = "container" Then
                        If FromCmd Or EmptyCmd Then
                            If Value.ToLower = "mp4" Then
                                RadMP4.Checked = True
                            ElseIf Value.ToLower = "ts" Then
                                RadTS.Checked = True
                            Else
                                RadBinary.Checked = True
                            End If
                        End If
                    ElseIf Item = "workpath" Then
                        If FromCmd Then ApplyWorkPath(Value)
                    ElseIf Item = "autostart" Then
                        AutoStart = StringToBool(Value)
                    ElseIf Item = "autoclean" Then
                        AutoClean = StringToBool(Value)
                    ElseIf Item = "minimize" Then
                        If StringToBool(Value) Then
                            WindowState = FormWindowState.Minimized
                        Else
                            WindowState = FormWindowState.Normal
                        End If
                    ElseIf Item = "redirecturl" Then
                        AutoRedirect = Not StringToBool(Value)
                    ElseIf Item = "mute" Then
                        Mute = StringToBool(Value)
                    ElseIf Item = "keepalive" Then
                        KeepAlive = StringToBool(Value)
                    ElseIf Item = "header" Then
                        If Value = "-1" Then HTTPHeader.Clear()
                        If FromCmd = False Then HTTPHeader.Add(Value)
                    ElseIf Item.StartsWith("th") Then
                        If Int(Value) > 0 And Int(Value) <= 1000 Then
                            MTThreads = Int(Value)
                            MTTDefined = True
                        End If
                    ElseIf Item = "batch" Then
                        If FromCmd Then
                            ShowBatch()
                            Batch.ApplyLocal(Value.Split("?"))
                        End If
                    End If
                End If
            Next
        Catch ex As Exception
            WriteLog(ex.ToString)
        Finally
            RedoSetOption = False
        End Try
    End Sub

    Public Function StringToBool(Input As String) As Boolean
        If Input.StartsWith("1") Or Input.ToLower.StartsWith("t") Or Input.ToLower.StartsWith("y") Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Function GetOption(Opt As String) As String
        Try
            For Each _loc_1 In Opt.Split("|")
                If _loc_1.Split(">")(0).ToLower = Opt.ToLower Then
                    Return _loc_1.Split(">")(1)
                End If
            Next
            Return ""
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Public Sub WriteLog(Content As String)
        Try
            Dim _loc_1 As New System.IO.StreamWriter(GetLogPath(), True, Encoding.Default)
            _loc_1.WriteLine("[" & Now & "]  " & Content)
            _loc_1.WriteLine("")
            _loc_1.Close()
            _loc_1.Dispose()
        Catch ex As Exception

        End Try
    End Sub

    Public Function GetLogPath() As String
        Return WorkingPath & TempFolderName & "\" & "LOGS_" & LogFileName & ".txt"
    End Function

    Public Sub EnableControls()
        Try
            InvokeControl(TxtURL, Sub(x) x.ReadOnly = False)
            InvokeControl(TxtTaskName, Sub(x) x.ReadOnly = False)
            InvokeControl(TxtOptions, Sub(x) x.ReadOnly = False)
            InvokeControl(LblProgress, Sub(x) x.Visible = False)
            InvokeControl(LblSpeed, Sub(x) x.Visible = False)
            InvokeControl(BtnStart, Sub(x) x.Visible = True)
            InvokeControl(BtnPause, Sub(x) x.Visible = False)
            InvokeControl(BtnWorkPathBrowse, Sub(x) x.Enabled = True)
            InvokeControl(BtnPathHistory, Sub(x) x.Enabled = True)
            InvokeControl(ChkLocal, Sub(x) x.Enabled = True)
            TmrClipboard.Enabled = True
        Catch ex As Exception
            WriteLog(ex.ToString)
        End Try
    End Sub

    Public Sub HideControls()
        Try
            InvokeControl(BtnStart, Sub(x) x.Visible = False)
            InvokeControl(BtnPause, Sub(x) x.Visible = False)
        Catch ex As Exception

        End Try
    End Sub

    Public Shared Function GetRndString(lngNum As Long) As String
        GetRndString = ""
        Dim i As Long
        Dim intLength As Integer
        Const STRINGSOURCE = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"
        intLength = Len(STRINGSOURCE) - 1

        Randomize()
        For i = 1 To lngNum
            GetRndString &= Mid(STRINGSOURCE, Int(Rnd() * intLength + 1), 1)
        Next
    End Function

    Public Function KeyRegulation(Input As String)
        Try
            Input = Input.Trim.Replace("，", ",").Replace(" ", "")
            If Input.Contains(",") Then
                Dim Output As String = ""
                For Each i In Input.Split(",")
                    If IsNumeric(i) Then Output += Hex(Int(i))
                Next
                Return Output
            Else
                Return Input
            End If
        Catch ex As Exception
            Return Input
        End Try
    End Function

    Public Sub InvokeControl(Of T As Control)(Control As T, Action As Action(Of T))
        If Control.InvokeRequired Then
            Control.Invoke(New Action(Of T, Action(Of T))(AddressOf InvokeControl), New Object() {Control, Action})
        Else
            Action(Control)
        End If
    End Sub

    Public Function GetWebpage(URL As String, Optional ReturnType As String = "String", Optional ByteRange As String = "", Optional LogPrefix As String = "", Optional StreamIndex As Integer = -1)
        Try
            If (URL.Contains("5club.cctv.cn/live/") Or URL.Contains("tp4k.cctv.cn/live/")) And URL.Contains(".m3u8") And UserAgent = "" Then
                If YangShiPinUID = "" Then YangShiPinUID = New PLUGIN_YangShiPin().GetIMEI()
                URL = New PLUGIN_YangShiPin().GetWsSecret(URL)
            End If

            WriteLog(LogPrefix & "[Attempt] " & URL)

            Dim TryParseFile As String = ""
            If URL.StartsWith("file:///") And ReturnType = "Bytes" Then TryParseFile = Regex.Match(URL.Replace("\/", "/"), "file:///(.*)?", RegexOptions.IgnoreCase).Groups(1).Value
            If Not TryParseFile = "" And My.Computer.FileSystem.FileExists(TryParseFile) Then Return My.Computer.FileSystem.ReadAllBytes(TryParseFile)

            If Not Host = "" Then
                URL = ReplaceHost(URL)
                WriteLog(LogPrefix & "[Rewrite] " & URL)
            End If

            ServicePointManager.ServerCertificateValidationCallback = New System.Net.Security.RemoteCertificateValidationCallback(AddressOf CheckValidationResult)
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 Or SecurityProtocolType.Tls11 Or SecurityProtocolType.Tls Or SecurityProtocolType.Ssl3
            Dim httpReq As System.Net.HttpWebRequest
            Dim httpResp As System.Net.HttpWebResponse
            Dim httpURL As New System.Uri(URL)
            httpReq = CType(WebRequest.Create(httpURL), HttpWebRequest)
            httpReq.Method = "GET"

            If NoProxy Then
                httpReq.Proxy = New WebProxy()
            ElseIf Not Proxy = "" Then
                httpReq.Proxy = New WebProxy(Proxy, False) With {.Credentials = System.Net.CredentialCache.DefaultCredentials}
            End If

            If StreamIndex >= 0 Then
                httpReq.Timeout = TimeoutList(StreamIndex)
            Else
                httpReq.Timeout = DefaultTimeOut
            End If
            WriteLog(LogPrefix & "[Timeout] " & httpReq.Timeout & " (ms).")
            httpReq.AllowAutoRedirect = AutoRedirect
            httpReq.KeepAlive = KeepAlive
            If ForceNotAutoRedirect Or IsAiShang Then httpReq.AllowAutoRedirect = False

            If URL.Contains("mgtv.com/") Or IsMGTV Then
                If Referer = "" Then Referer = "mgtv.com"
                If UserAgent = "" Then UserAgent = "Electron"
            End If

            If Not YangShiPinUID = "" Then
                DefaultUserAgent = "cctv_app_tv"
                httpReq.Referer = "api.cctv.cn"
                httpReq.Headers.Add("UID", YangShiPinUID)
            End If

            If Not UserAgent = "-1" Then
                If Not UserAgent = "" Then
                    httpReq.UserAgent = UserAgent
                Else
                    httpReq.UserAgent = DefaultUserAgent
                End If
            End If

            If Not Referer = "" Then httpReq.Referer = Referer

            If Not Cookie = "" Then httpReq.Headers.Add("Cookie", Cookie)

            Try
                If HTTPHeader.Count > 0 Then
                    For _loc_1 = 0 To HTTPHeader.Count - 1
                        Dim _loc_2 As String = HTTPHeader(_loc_1)
                        Dim _loc_3 As String = _loc_2.Split(":")(0)
                        Dim _loc_4 As String = _loc_2.Substring(_loc_3.Length + 1, _loc_2.Length - (_loc_3.Length + 1))
                        httpReq.Headers.Add(_loc_3.Trim, _loc_4.Trim)
                    Next
                End If
            Catch ex As Exception

            End Try

            If KeepAlive Then
                Dim _loc_8 = httpReq.ServicePoint
                Dim _loc_9 = _loc_8.[GetType]().GetProperty("HttpBehaviour", BindingFlags.Instance Or BindingFlags.NonPublic)
                _loc_9.SetValue(_loc_8, CByte(0), Nothing)
            End If

            If Not ByteRange = "" And ByteRange.Contains("-") Then
                WriteLog(LogPrefix & "[Range] " & ByteRange)
                httpReq.AddRange("bytes", Convert.ToInt64(ByteRange.Split("-")(0)), Convert.ToInt64(ByteRange.Split("-")(1)))
            End If

            httpResp = CType(httpReq.GetResponse(), HttpWebResponse)

            Dim RedirectHeader As String = httpResp.Headers("Location")
            Dim ContentLengthHeader As String = httpResp.Headers("Content-Length")
            Dim SetCookieHeader As String = httpResp.Headers("Set-Cookie")

            If Not SetCookieHeader = "" And (Cookie = "" Or IsAiShang) Then
                Cookie = SetCookieHeader
                If Not IsAiShang And SetCookieHeader.ToUpper().Contains("COLLCK") Then IsAiShang = True
            End If

            If RedirectHeader = "" Then
                IsRedirect = False

                Dim returnBuffer As Byte() = Nothing
                Dim reader As New BinaryReader(httpResp.GetResponseStream)
                If ContentLengthHeader = "" Then
                    returnBuffer = reader.ReadBytes(Int32.MaxValue / 2)
                    ContentLengthHeader = returnBuffer.Length
                Else
                    returnBuffer = reader.ReadBytes(ContentLengthHeader)
                End If

                If ReturnType = "Bytes" Then
                    If Not returnBuffer.Length = ContentLengthHeader And IgnoreSize = False Then
                        WriteLog(LogPrefix & "Content size does not match. Server reported length: " & ContentLengthHeader & " bytes, Received length: " & returnBuffer.Length & " bytes.")
                        Return Encoding.UTF8.GetBytes("null")
                    ElseIf returnBuffer.Length = 0 And IgnoreSize = False Then
                        WriteLog(LogPrefix & "Empty response.")
                        Return Encoding.UTF8.GetBytes("null")
                    Else
                        Return returnBuffer
                    End If
                Else
                    BackupM3U8Stream = returnBuffer
                    Return Encoding.UTF8.GetString(returnBuffer)
                End If
            Else
                IsRedirect = True
                If LiveAPI And LiveAPICurrentURL.Contains("ysp.cctv.cn/") And RedirectHeader.Contains("ysp.cctv.cn/") Then
                    RedirectHeader = New PLUGIN_YangShiPin().ReplaceVKey(LiveAPICurrentURL, RedirectHeader)
                End If
                RecentRedirect = RedirectHeader
                WriteLog(LogPrefix & "[Redirect from] " & URL)
                WriteLog(LogPrefix & "[Redirect to] " & RedirectHeader)
                Return GetWebpage(RedirectHeader, ReturnType, ByteRange, LogPrefix, StreamIndex)
            End If
        Catch ex As Exception
            WriteLog(LogPrefix & ex.ToString)

            If IsRedirect Then IsRedirect = False

            If ReturnType = "Bytes" Then
                Return Encoding.UTF8.GetBytes("null")
            Else
                Return ""
            End If
        Finally
            ForceNotAutoRedirect = False
        End Try
    End Function

    Public Function CheckValidationResult(sender As Object, certificate As X509Certificate, chain As X509Chain, errors As SslPolicyErrors) As Boolean
        Return True
    End Function

    Public Function GetParent(Input As String, Optional ForceDo As Boolean = False)
        If MParentURL = "" Or ForceDo Then
            Dim SplitChar As String = "/"
            If Input.StartsWith("file:///") Then SplitChar = "\"

            Dim _loc_1 As String() = Input.Split("?")(0).Split(SplitChar)
            Dim _loc_2 As String = ""
            For _loc_3 = 0 To _loc_1.Count - 2
                _loc_2 += _loc_1(_loc_3) & SplitChar
            Next
            Return _loc_2.Substring(0, _loc_2.Length)
        Else
            Return MParentURL
        End If
    End Function

    Public Function GetHost(Input As String)
        Dim _loc_1 As String() = Input.Split("?")(0).Split("/")
        Dim _loc_2 As String = ""
        For _loc_3 = 0 To 2
            _loc_2 += _loc_1(_loc_3) & "/"
        Next
        Return _loc_2.Substring(0, _loc_2.Length)
    End Function

    Public Function ReplaceHost(Input As String)
        Try
            Dim SplitChar As String = "/"
            Dim _loc_1 As String() = Input.Trim.Split(SplitChar)
            Dim _loc_2 As String = ""
            Dim _loc_3 As String() = _loc_1(2).Split(":")

            _loc_1(2) = Host
            If _loc_3.Count > 1 And (Not Host.Contains(":")) Then _loc_1(2) += ":" & _loc_3(1)

            For i = 0 To _loc_1.Count - 1
                _loc_2 += _loc_1(i) & SplitChar
            Next
            Return _loc_2.Substring(0, _loc_2.Length - 1)
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Public Function GetFullURL(Input As String)
        If Input.StartsWith("http") Then
            Return Input.Trim
        ElseIf Input.StartsWith("/") Then
            Return GetHost(ParentURL) & Input.Trim.Substring(1, Input.Trim.Length - 1)
        Else
            Return ParentURL & Input.Trim
        End If
    End Function

    Public Function GetNumerics(Input As String)
        Return New String((From c As Char In Input Select c Where Char.IsDigit(c)).ToArray())
    End Function

    Public Function ParseByteRange(Input As String)
        Dim _loc_1 As String() = Input.Split("@")
        Dim _loc_2 As Long = 0
        Dim _loc_3 As Long = GetNumerics(_loc_1(0))
        If _loc_1.Count > 1 Then
            _loc_2 = GetNumerics(_loc_1(1))
        End If
        Dim _loc_4 As Long = _loc_2 + _loc_3 - 1
        Return _loc_2 & "-" & _loc_4
    End Function

    Public Function IsValidFileNameOrPath(param1 As String) As Boolean
        If param1 Is Nothing Then
            Return False
        End If

        For Each _loc_1 As Char In System.IO.Path.GetInvalidPathChars
            If InStr(param1, _loc_1) > 0 Then
                Return False
            End If
        Next

        Return True
    End Function

    Public Function IfAppendQuery(param1 As String) As String
        If AppendQuery Or param1.Contains(".pstatic.net/") Or param1.Contains("dlsc.hcs.cmvideo.cn") Then
            If Not param1.Contains("?") And TxtURL.Text.Contains("?") Then
                param1 &= "?" & TxtURL.Text.Split("?")(1)
            End If
        End If
        Return param1
    End Function

    Public Function IfLooseMatch(param1 As String) As String
        If LooseMatch Or param1.Contains(".ysp.cctv.cn/") Or Not YangShiPinUID = "" Then
            Dim _loc_1 As String() = param1.Split("?")(0).Split("/")
            Return _loc_1(_loc_1.Count - 1)
        End If
        Return param1
    End Function

    Public Shared Function HexToBytes(param1 As String) As Byte()
        Return Enumerable.Range(0, param1.Length).Where(Function(x) x Mod 2 = 0).[Select](Function(x) Convert.ToByte(param1.Substring(x, 2), 16)).ToArray()
    End Function

    Public Shared Function BytesToHex(param1 As Byte()) As String
        Return BitConverter.ToString(param1).Replace("-", "").ToUpper
    End Function

    Public Shared Function BytesToBin(param1() As Byte) As String
        Dim _loc_1 As New StringBuilder
        For Each _loc_2 In param1
            _loc_1.Append(Convert.ToString(_loc_2, 2).PadLeft(8, "0"))
        Next
        Return _loc_1.ToString
    End Function

    Public Shared Function BinToBytes(param1 As String) As Byte()
        Dim _loc_1 As Integer = param1.Length / 8
        Dim _loc_2 As Byte() = New Byte(_loc_1 - 1) {}
        For _loc_3 As Integer = 0 To _loc_1 - 1
            _loc_2(_loc_3) = Convert.ToByte(param1.Substring(8 * _loc_3, 8), 2)
        Next
        Return _loc_2
    End Function

    Public Function AES_Decrypt(inBuff As Byte(), keyBytes As Byte(), IVBytes As Byte()) As Byte()
        If PLUGIN_ThousandBillion Then keyBytes = New PLUGIN_ThousandBillion().ThousandBillionDeAESKey(keyBytes)

        WriteLog("[Cipher] Key=0x" & BytesToHex(keyBytes) & " , IV=0x" & BytesToHex(IVBytes))
        Dim decrypter As System.Security.Cryptography.Aes = System.Security.Cryptography.Aes.Create("AES")
        decrypter.BlockSize = 128
        decrypter.KeySize = keyBytes.Length * 8
        decrypter.Key = keyBytes
        decrypter.IV = IVBytes
        decrypter.Mode = CipherMode.CBC
        decrypter.Padding = PaddingMode.PKCS7
        Dim crypter As ICryptoTransform = decrypter.CreateDecryptor()
        Dim returnBytes As Byte() = crypter.TransformFinalBlock(inBuff, 0, inBuff.Length)
        decrypter.Dispose()
        Return returnBytes
    End Function

    Private Function GetDol() As Boolean
        Dim _loc_1 As New Process()
        Try
            _loc_1.StartInfo.FileName = "ffmpeg.exe"
            _loc_1.StartInfo.Arguments = "-i " & Chr(34) & TempPath & Chr(34)
            _loc_1.StartInfo.UseShellExecute = False
            _loc_1.StartInfo.CreateNoWindow = True
            _loc_1.StartInfo.RedirectStandardError = True
            _loc_1.Start()
            _loc_1.WaitForExit()
            Dim Errorstr As String = _loc_1.StandardError.ReadToEnd()
            _loc_1.Close()

            If Errorstr.ToLower.Replace(" ", "").Contains("audio:aac") Then
                Return False
            Else
                Return True
            End If
        Catch ex As Exception
            Return True
        Finally
            _loc_1.Dispose()
        End Try
    End Function

    Public Function ReadResource(resname As String) As Byte()
        Dim Stream As Stream = Assembly.GetExecutingAssembly.GetManifestResourceStream(resname)
        Dim FileByte(Stream.Length - 1) As Byte
        With Stream
            .Read(FileByte, 0, FileByte.Length)
            .Close()
            .Dispose()
        End With
        Return FileByte
    End Function

    Private Sub ReleaseResource(resname As String, relpath As String)
        Dim Stream As Stream = Assembly.GetExecutingAssembly.GetManifestResourceStream(resname)
        Dim FileByte(Stream.Length - 1) As Byte
        With Stream
            .Read(FileByte, 0, FileByte.Length)
            .Close()
            .Dispose()
        End With

        Dim Writer As New System.IO.StreamWriter(relpath)
        With Writer
            .BaseStream.Write(FileByte, 0, FileByte.Length)
            .Close()
            .Dispose()
        End With
    End Sub

    Public Function Zip(Input As Byte()) As Byte()
        Dim buffer As Byte() = Input
        Dim ms As New MemoryStream()
        Using zipStream As New Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Compress, True)
            zipStream.Write(buffer, 0, buffer.Length)
        End Using

        ms.Position = 0

        Dim compressed As Byte() = New Byte(ms.Length - 1) {}
        ms.Read(compressed, 0, compressed.Length)

        Dim gzBuffer As Byte() = New Byte(compressed.Length + 3) {}
        System.Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length)
        System.Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzBuffer, 0, 4)
        Return gzBuffer
    End Function

    Public Function UnZip(Input As Byte(), Optional RearCounter As Boolean = False) As Byte()
        Dim gzBuffer As Byte() = Input
        Using ms As New MemoryStream()
            Dim msgLength As Integer = 0

            If RearCounter Then
                msgLength = BitConverter.ToInt32(gzBuffer, gzBuffer.Length - 4)
                ms.Write(gzBuffer, 0, gzBuffer.Length - 4)
            Else
                msgLength = BitConverter.ToInt32(gzBuffer, 0)
                ms.Write(gzBuffer, 4, gzBuffer.Length - 4)
            End If

            Dim buffer As Byte() = New Byte(msgLength - 1) {}

            ms.Position = 0
            Using zipStream As New Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Decompress)
                zipStream.Read(buffer, 0, buffer.Length)
            End Using

            Return buffer
        End Using
    End Function

    Public Function IsFFmpegExist() As Boolean
        Try
            Dim _loc_1 As New Process()
            _loc_1.StartInfo.FileName = "ffmpeg.exe"
            _loc_1.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
            _loc_1.Start()
            _loc_1.Close()
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Sub KeyWrite(KeyName As String, KeyValue As String)
        Try
            Dim _loc_1 As RegistryKey = Registry.CurrentUser.CreateSubKey("Software\HmDX")
            _loc_1.OpenSubKey("Software\HmDX", True)
            _loc_1.SetValue(KeyName, KeyValue, RegistryValueKind.String)
        Catch ex As Exception

        End Try
    End Sub

    Public Function KeyRead(KeyName As String) As String
        Try
            Dim _loc_1 As RegistryKey = Registry.CurrentUser.OpenSubKey("Software\HmDX", True)
            Return _loc_1.GetValue(KeyName)
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Private Sub TsiNewTaskWParam_Click(sender As Object, e As EventArgs) Handles TsiNewTaskWParam.Click
        Try
            'Rebuild options
            Dim Arg As String = ""
            Dim ArgContainer As String = ""
            If Not TxtOptions.Text = "" Then
                For Each Item In TxtOptions.Text.Split("|")
                    If Item.ToLower.StartsWith("url>") Or Item.ToLower.StartsWith("taskname>") Or Item.ToLower.StartsWith("workpath>") Or Item.ToLower.StartsWith("container>") Or Item.ToLower.StartsWith("islive>") Or Item.ToLower.StartsWith("twopass>") Or Item.ToLower.StartsWith("minimize>") Or Item = "" Then

                    Else
                        Arg += Item & "|"
                    End If
                Next
            End If

            If RadMP4.Checked = True Then
                ArgContainer = "MP4"
            ElseIf RadTS.Checked = True Then
                ArgContainer = "TS"
            Else
                ArgContainer = "RAW"
            End If

            Arg = "URL>" & TxtURL.Text.Replace(">", "<g").Replace("<", "<l").Replace("|", "<s") & "|" & "TaskName>" & TxtTaskName.Text & "|" & "WorkPath>" & TxtWorkPath.Text & "|" & "isLive>" & ChkLive.Checked & "|" & "TwoPass>" & ChkTwoPass.Checked & "|" & "Container>" & ArgContainer & "|" & "Minimize>False|" & Arg
            If Not Arg.EndsWith("|") Then Arg += "|"

            Process.Start(Application.ExecutablePath, Arg)
            If TaskDoneAll = True Then
                Dispose()
                End
            End If
        Catch ex As Exception
            WriteLog(ex.ToString)
        End Try
    End Sub

    Private Sub TsiAbout_Click(sender As Object, e As EventArgs) Handles TsiAbout.Click
        Try
            MsgBox("HLS/m3u8 Downloader X" & vbCrLf & "HmDX 很萌下载器" & vbCrLf & "M3U8网络视频下载工具" & vbCrLf & vbCrLf & "软件版本：" & VersionStrings(0) & "." & VersionStrings(1) & "." & VersionStrings(2) & vbCrLf & "更新时间：20" & VersionStrings(3).Substring(0, 2) & "年" & Int(VersionStrings(3).Substring(2, 2)) & "月" & vbCrLf & vbCrLf & "© 2018-20" & VersionStrings(3).Substring(0, 2) & " HmDX 版权所有", vbInformation, "关于 HmDX")
        Catch ex As Exception

        End Try
    End Sub

    Private Sub PicBadge_Click(sender As Object, e As EventArgs) Handles PicBadge.Click
        TsiAbout_Click(sender, e)
    End Sub

    Private Sub TsiUpgradeBatch_Click(sender As Object, e As EventArgs) Handles TsiUpgradeBatch.Click
        Try
            If (BtnStart.Enabled = False Or BtnStart.Visible = False) And TaskDoneAll = False Then
                MsgBox("任务正在进行，不能提升实例！", vbCritical, "提升失败")
            Else
                ShowBatch()
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub ShowBatch()
        Try
            TmrClipboard.Enabled = False
            Batch.Show()
            Hide()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub LblSlogan_Click(sender As Object, e As EventArgs) Handles LblSlogan.Click
        Try
            Process.Start("http://cpc.people.com.cn/")
        Catch ex As Exception

        End Try
    End Sub

    Private Sub TxtOptions_KeyDown(sender As Object, e As KeyEventArgs) Handles TxtOptions.KeyDown
        Try
            If e.Control And e.KeyCode = Keys.A Then
                TxtOptions.SelectAll()
            End If
        Catch ex As Exception
            WriteLog(ex.ToString)
        End Try
    End Sub

    Private Sub RadTS_CheckedChanged(sender As Object, e As EventArgs) Handles RadTS.CheckedChanged
        If Not RadBinary.Checked Then RadCheckChanged()
    End Sub

    Private Sub RadMP4_CheckedChanged(sender As Object, e As EventArgs) Handles RadMP4.CheckedChanged
        If Not RadBinary.Checked Then RadCheckChanged()
    End Sub

    Private Sub RadCheckChanged()
        Try
            If Not FormLoaded Then Exit Sub
            If CheckboxInHandle Then Exit Sub
            CheckboxInHandle = True
            If Not IsFFmpegExist() Then
                RadBinary.Checked = True
                If MsgBox("未检测到 FFmpeg 程序。封装MP4或TS格式需要 FFmpeg 程序。" & vbCrLf & "是否现在下载 FFmpeg 程序？", vbQuestion + vbYesNo, Text) = vbYes Then
                    Process.Start("https://www.gyan.dev/ffmpeg/builds/")
                    MsgBox("您的浏览器将显示 FFmpeg 程序的下载页面。" & vbCrLf & "www.gyan.dev 提供适用于 64 位 Windows 操作系统的 FFmpeg 编译版本。" & vbCrLf & "请您选择一个完整版本 (full_build) 下载并将压缩包 bin 目录下的 ffmpeg.exe 放置于当前目录或加入系统环境变量（例如，您可以将 ffmpeg.exe 复制到 system32 系统目录）。" & vbCrLf & "完成后，您可以再次选择封装为MP4或TS格式。" & vbCrLf & "注意：您在使用该第三方开源计算机程序时，必须同意并遵守其相关的协议和规定。", vbInformation, Text)
                End If
            End If
            CheckboxInHandle = False
        Catch ex As Exception
            CheckboxInHandle = False
        End Try
    End Sub

    Private Sub MainUI_DragEnter(sender As Object, e As DragEventArgs) Handles Me.DragEnter
        Try
            If e.Data.GetDataPresent(DataFormats.FileDrop) = True Then
                e.Effect = DragDropEffects.Copy
            Else
                e.Effect = DragDropEffects.None
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub MainUI_DragDrop(sender As Object, e As DragEventArgs) Handles Me.DragDrop
        Try
            Dim DragFilePath As String() = e.Data.GetData(DataFormats.FileDrop)
            If My.Computer.FileSystem.FileExists(DragFilePath(0)) And LblSpeed.Visible = False Then
                ApplyFileInput(DragFilePath(0))
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub ParseRegOptions()
        Try
            Dim UserSpecifiedWorkPath As String = KeyRead("WorkPath")
            If UserSpecifiedWorkPath = "" Then Exit Try
            UserSpecifiedWorkPath = UserSpecifiedWorkPath.Replace("/", "\")
            If My.Computer.FileSystem.DirectoryExists(UserSpecifiedWorkPath) Then
                If Not UserSpecifiedWorkPath.EndsWith("\") Then
                    UserSpecifiedWorkPath += "\"
                End If
                ApplyWorkPath(UserSpecifiedWorkPath)
            End If
        Catch ex As Exception

        End Try

        Try
            Skin = KeyRead("Skin")
        Catch ex As Exception

        End Try

        Try
            EncodingTool = KeyRead("EncodingTool")
        Catch ex As Exception

        End Try

        Try
            If StringToBool(KeyRead("TopMost")) Then TopMost = True
        Catch ex As Exception

        End Try

        Try
            If Command() = "" And TxtOptions.Text = "" Then TxtOptions.Text = KeyRead("Options")
        Catch ex As Exception

        End Try
    End Sub

    Private Sub SetSkin(SkinName As String)
        Try
            KeyWrite("Skin", SkinName)
            CreateNewTask()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub TsiSkinDefault_Click(sender As Object, e As EventArgs) Handles TsiSkinDefault.Click
        SetSkin("Default")
    End Sub

    Private Sub TsiSkinWave_Click(sender As Object, e As EventArgs) Handles TsiSkinWave.Click
        SetSkin("Wave")
    End Sub

    Private Sub TsiSkinSilver_Click(sender As Object, e As EventArgs) Handles TsiSkinSilver.Click
        SetSkin("Silver")
    End Sub

    Private Sub TsiSkinGold_Click(sender As Object, e As EventArgs) Handles TsiSkinGold.Click
        SetSkin("Gold")
    End Sub

    Private Sub TsiSkinHatsuneMiku_Click(sender As Object, e As EventArgs) Handles TsiSkinHatsuneMiku.Click
        SetSkin("HatsuneMiku")
    End Sub

    Private Sub TsiSkinNone_Click(sender As Object, e As EventArgs) Handles TsiSkinNone.Click
        SetSkin("None")
    End Sub

    Private Sub TsiCleanAllCache_Click(sender As Object, e As EventArgs) Handles TsiCleanAllCache.Click
        Try
            If MsgBox("您即将清理缓存文件夹。这将删除所有已完成和正在进行中的任务的日志、列表备份和封装失败的视频文件。如果您有正在进行的任务，执行此操作将导致这些任务全部失败。请确认您没有活动的任务，及需要保留的缓存文件。" & vbCrLf & "您无法撤销本操作！执行清理？", vbQuestion + vbOKCancel, "清理缓存") = vbOK Then
                My.Computer.FileSystem.DeleteDirectory(WorkingPath & TempFolderName, FileIO.DeleteDirectoryOption.DeleteAllContents)
                MsgBox("缓存已清理。", vbInformation, "清理缓存")
            End If
        Catch ex As Exception
            MsgBox("缓存文件夹清理失败！", vbExclamation, "清理缓存")
            WriteLog(ex.ToString)
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

    Private Sub TmrAutoRestart_Tick(sender As Object, e As EventArgs) Handles TmrAutoRestart.Tick
        Try
            TmrAutoRestart.Enabled = False
            WriteLog("Scheduled re-start time is up.")
            Pause()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub BtnPathHistory_Click(sender As Object, e As EventArgs) Handles BtnPathHistory.Click
        Try
            If KeyRead("WorkPathHistory") = "0" Then
                If MsgBox("此功能已被禁用。启用此功能后将保存近期使用的5个工作目录。" & vbCrLf & "是否现在启用？", vbQuestion + vbYesNo, "工作目录历史记录") = vbYes Then
                    KeyWrite("WorkPathHistory", "")
                End If
                Exit Sub
            End If
        Catch ex As Exception

        End Try

        Try
            Dim _loc_1 As String() = KeyRead("WorkPathHistory").Split("|")
            For _loc_2 = 0 To _loc_1.Count - 1
                If IsValidFileNameOrPath(_loc_2) Then
                    If _loc_2 = 0 Then
                        TsiPathHistory1.Text = _loc_1(_loc_2)
                    ElseIf _loc_2 = 1 Then
                        TsiPathHistory2.Text = _loc_1(_loc_2)
                    ElseIf _loc_2 = 2 Then
                        TsiPathHistory3.Text = _loc_1(_loc_2)
                    ElseIf _loc_2 = 3 Then
                        TsiPathHistory4.Text = _loc_1(_loc_2)
                    ElseIf _loc_2 = 4 Then
                        TsiPathHistory5.Text = _loc_1(_loc_2)
                    End If
                End If
            Next
        Catch ex As Exception

        Finally
            CMSPathHistory.Show(System.Windows.Forms.Control.MousePosition)
        End Try
    End Sub

    Private Sub StorePathHistory(Path As String)
        Try
            If KeyRead("WorkPathHistory") = "0" Then Exit Sub
        Catch ex As Exception

        End Try

        Try
            Dim MaxHistoryCount As Integer = 5
            Dim _loc_1 As String = Path
            Dim _loc_2 As Integer = 1
            Dim _loc_3 As String = KeyRead("WorkPathHistory")
            For Each _loc_4 In _loc_3.Split("|")
                If IsValidFileNameOrPath(_loc_4) Then
                    _loc_1 += "|" & _loc_4
                    _loc_2 += 1
                End If
                If _loc_2 >= MaxHistoryCount Then Exit For
            Next
            KeyWrite("WorkPathHistory", _loc_1)
        Catch ex As Exception

        End Try
    End Sub

    Private Sub TsiPathHistoryDisable_Click(sender As Object, e As EventArgs) Handles TsiPathHistoryDisable.Click
        Try
            TsiPathHistory1.Text = ""
            TsiPathHistory2.Text = ""
            TsiPathHistory3.Text = ""
            TsiPathHistory4.Text = ""
            TsiPathHistory5.Text = ""
            KeyWrite("WorkPathHistory", "0")
        Catch ex As Exception

        End Try
    End Sub

    Private Sub TsiPathHistory1_Click(sender As Object, e As EventArgs) Handles TsiPathHistory1.Click
        Try
            ApplyWorkPath(TsiPathHistory1.Text)
        Catch ex As Exception

        End Try
    End Sub

    Private Sub TsiPathHistory2_Click(sender As Object, e As EventArgs) Handles TsiPathHistory2.Click
        Try
            ApplyWorkPath(TsiPathHistory2.Text)
        Catch ex As Exception

        End Try
    End Sub

    Private Sub TsiPathHistory3_Click(sender As Object, e As EventArgs) Handles TsiPathHistory3.Click
        Try
            ApplyWorkPath(TsiPathHistory3.Text)
        Catch ex As Exception

        End Try
    End Sub

    Private Sub TsiPathHistory4_Click(sender As Object, e As EventArgs) Handles TsiPathHistory4.Click
        Try
            ApplyWorkPath(TsiPathHistory4.Text)
        Catch ex As Exception

        End Try
    End Sub

    Private Sub TsiPathHistory5_Click(sender As Object, e As EventArgs) Handles TsiPathHistory5.Click
        Try
            ApplyWorkPath(TsiPathHistory5.Text)
        Catch ex As Exception

        End Try
    End Sub

    Private Sub TsiSkinCustomize_Click(sender As Object, e As EventArgs) Handles TsiSkinCustomize.Click
        Try
            If OfdSkin.ShowDialog = DialogResult.OK Then
                Dim _loc_1 As String = OfdSkin.FileName
                If My.Computer.FileSystem.FileExists(_loc_1) Then
                    If New IO.FileInfo(_loc_1).Length > 10485760 Then
                        MsgBox("自定义图片文件体积过大！", vbExclamation, "自定义皮肤设置")
                    Else
                        SkinCustomize.ShowDialog()
                        If Not SkinRGB = "" Then
                            SetSkin("(" & SkinRGB & ")" & Convert.ToBase64String(Zip(My.Computer.FileSystem.ReadAllBytes(_loc_1))))
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            MsgBox("导入自定义图片文件失败！", vbCritical, "自定义皮肤设置")
        End Try
    End Sub

    Private Sub OpenLogFile()
        Try
            If My.Computer.FileSystem.FileExists(GetLogPath()) Then
                Process.Start(GetLogPath())
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub LblDownError_DoubleClick(sender As Object, e As EventArgs) Handles LblDownError.DoubleClick
        OpenLogFile()
    End Sub

    Private Sub LblFileDamage_DoubleClick(sender As Object, e As EventArgs) Handles LblFileDamage.DoubleClick
        OpenLogFile()
    End Sub

    Private Sub LblLiveInterrupt_DoubleClick(sender As Object, e As EventArgs) Handles LblLiveInterrupt.DoubleClick
        OpenLogFile()
    End Sub

    Private Sub PicPlay_DoubleClick(sender As Object, e As EventArgs) Handles PicPlay.DoubleClick
        Try
            If My.Computer.FileSystem.FileExists(TempPath) Then
                Process.Start(TempPath)
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub PicInfo_DoubleClick(sender As Object, e As EventArgs) Handles PicInfo.DoubleClick
        Try
            If IsFFmpegExist() Then
                Dim _loc_1 As New Process()
                Dim _loc_2 As String = TxtURL.Text
                If My.Computer.FileSystem.FileExists(TempPath) Then
                    _loc_2 = TempPath
                End If
                _loc_1.StartInfo.FileName = "cmd.exe"
                _loc_1.StartInfo.Arguments = "/k ffmpeg.exe -i " & Chr(34) & _loc_2 & Chr(34)
                _loc_1.StartInfo.UseShellExecute = False
                _loc_1.Start()
            End If
        Catch ex As Exception

        End Try
    End Sub
End Class
