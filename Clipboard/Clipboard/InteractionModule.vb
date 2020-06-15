Imports Clipboard.LangClass
Imports Clipboard.ClipboardApi
Imports Clipboard.JsonConverter
Imports Kyozy.MiniblinkNet
Imports System.IO
Imports System.Speech.Synthesis

Module InteractionModule

    Friend WithEvents ClipboardUI As WebView            '显示UI

#Region "ContextMenu"
    Friend WithEvents IconContextMenu As ContextMenu    '任务栏图标右键菜单
    Friend WithEvents MonitorableMenuItem As MenuItem   '监控菜单项
    Friend WithEvents IsTopMenuItem As MenuItem         '置顶菜单项
    Friend WithEvents SeparatorMenuItem As MenuItem     '分隔菜单项
    Friend WithEvents ExitMenuItem As MenuItem          '退出菜单项

    Friend WithEvents MonitorableAllMenuItem As MenuItem
    Friend MonitorableMenuItems As MenuItem()           '监控菜单子项
#End Region

    Friend AddFolderDialog As VistaFolderBrowserDialog  'Vista样式文件选择对话框
    Friend DataList As List(Of DataTag)                 '已复制的数据列表
    Friend Settings As Settings                         '设置结构
    Friend innerData As Integer = -1                    '解决复制时两次更新出现复制两次的情况
    Friend CloseFlag As Boolean = False                 '点击右上角关闭按钮最小化到托盘
    Friend AllDisabled As Integer = 1                   '监控子项是否全未开启

    Private Sub LoadSettings()
        CheckUiDirectory(ClipboardUIDirName, Sub()
                                                 LogRecord("UI FILE IS MISSING")
                                                 MessageBox.Show(SpecText(LanguageTextEnum.CatastrophicFailure), MainForm.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                                 Application.Exit()
                                             End Sub)
        CheckUiDirectory(ImageDataName, Sub()
                                            Directory.CreateDirectory(ImageDataPath)
                                        End Sub)
        SettingsHelper = New INIHelper(INIPath)
        Settings = ReadSettings()
    End Sub

    Friend Sub ForEachMonitorableMenuItem(ByVal Action As Action(Of MenuItem))
        For Each Item As MenuItem In MonitorableMenuItems
            Action(Item)
        Next
    End Sub

    Private Sub AddEventHandler()
        AddHandler MainForm.KeyUp,
            AddressOf MainForm_KeyUp

        AddHandler MainForm.ClipboardNotifyIcon.MouseClick,
            AddressOf ClipboardNotifyIcon_MouseClick

        AddHandler ExitMenuItem.Click,
            AddressOf ExitMenuItem_Click

        AddHandler IsTopMenuItem.Click,
            AddressOf IsTopMenuItem_Click

        AddHandler MonitorableAllMenuItem.Click,
            AddressOf MonitorableAllMenuItem_Click

        ForEachMonitorableMenuItem(Sub(ByVal Item As MenuItem)
                                       AddHandler Item.Click,
                                            AddressOf MonitorableMenuItem_Click
                                   End Sub)
    End Sub

    Private Sub InitDynamicControls()
        AddFolderDialog = New VistaFolderBrowserDialog With {
            .Title = SpecText(LanguageTextEnum.Add)
        }
        MainForm.AddFileDialog.Title = SpecText(LanguageTextEnum.Add)

        MonitorableAllMenuItem = New MenuItem With {
            .Text = SpecText(LanguageTextEnum.All),
            .Checked = True
        }
        MonitorableMenuItems = New MenuItem() {
            New MenuItem With {
                .Text = SpecText(LanguageTextEnum.Text),
                .Tag = New Object() {LanguageTextEnum.Text, DataFormats.Text},
                .Checked = True
            },
            New MenuItem With {
                .Text = SpecText(LanguageTextEnum.Document),
                .Tag = New Object() {LanguageTextEnum.Document, DataFormats.FileDrop},
                .Checked = True
            },
            New MenuItem With {
                .Text = SpecText(LanguageTextEnum.Image),
                .Tag = New Object() {LanguageTextEnum.Image, DataFormats.Bitmap},
                .Checked = True
            }
        }
        MonitorableMenuItem = New MenuItem With {
            .Text = SpecText(LanguageTextEnum.Monitorable)
        }
        MonitorableMenuItem.MenuItems.Add(MonitorableAllMenuItem)
        MonitorableMenuItem.MenuItems.AddRange(MonitorableMenuItems)
        IsTopMenuItem = New MenuItem With {
            .Text = $"{SpecText(LanguageTextEnum.IsTop)}(&T)",
            .Checked = Settings.IsTop
        }
        MainForm.TopMost = IsTopMenuItem.Checked
        SeparatorMenuItem = New MenuItem("-")
        ExitMenuItem = New MenuItem With {
            .Text = $"{SpecText(LanguageTextEnum.Exit)}(&E)"
        }
        IconContextMenu = New ContextMenu()
        IconContextMenu.MenuItems.AddRange(New MenuItem() {
            MonitorableMenuItem,
            IsTopMenuItem,
            SeparatorMenuItem,
            ExitMenuItem
                                           })
        MainForm.ClipboardNotifyIcon.Icon = MainForm.Icon.Clone()
        MainForm.ClipboardNotifyIcon.ContextMenu = IconContextMenu
        '读取记录
        ReadSeriFile()

        '-----------------------------------------------------------------------------------------
        '设置语音
        Speecher = New SpeechSynthesizer With {
            .Rate = 0,
            .Volume = 100
            }
        Speecher.SetOutputToDefaultAudioDevice()
        SpeechMsg = New PromptBuilder With {
            .Culture = New Globalization.CultureInfo(Settings.Language.ToString.Replace("_", "-"))
            }

        '初始化UI
        InitClipboardUI()

        AddEventHandler()

    End Sub

    Private Sub InitMainForm()
        MainForm.Size = New Size(442, 884)
        MainForm.MinimumSize = New Size(400, 400)
        MainForm.Opacity = Settings.Opacity / 100
        MainForm.KeyPreview = True
        InitDynamicControls()
    End Sub

    Friend Sub Initialize()
        LoadSettings()
        InitMainForm()
    End Sub

    Friend Sub MainForm_KeyUp(sender As Object, e As KeyEventArgs)
        Select Case e.Modifiers
            Case Keys.Alt
                Select Case e.KeyCode
                    Case Keys.T '设置置顶
                        IsTopMenuItem.Checked = Not IsTopMenuItem.Checked
                    Case Keys.E '退出
                        CloseFlag = True
                        MainForm.Close()
                End Select
        End Select
    End Sub

#Region "ClipboardNotifyIcon"

    Friend Sub ClipboardNotifyIcon_MouseClick(sender As Object, e As MouseEventArgs)
        If e.Button = MouseButtons.Left Then
            MainForm.Visible = Not MainForm.Visible
            If MainForm.Visible Then
                MainForm.Activate()
            End If
        End If
    End Sub

#End Region

#Region "IconContextMenu"

    Friend Sub AdjustIsTop(ByVal Checked As Boolean)
        IsTopMenuItem.Checked = Checked
        Settings.IsTop = IsTopMenuItem.Checked
        WriteCore(OtherSec, 7, Convert.ToInt32(IsTopMenuItem.Checked).ToString())
        MainForm.TopMost = IsTopMenuItem.Checked
        Dim Msg As String =
                        If(
                        IsTopMenuItem.Checked,
                        SpecText(LanguageTextEnum.Top),
                        SpecText(LanguageTextEnum.NonTop)
                        )
        Notify(Msg)
        ActionSpeech(SettingsName.SpeechTop, Msg)
    End Sub

    Friend Sub ExitMenuItem_Click(sender As Object, e As EventArgs)
        CloseFlag = True
        MainForm.Close()
    End Sub

    Friend Sub IsTopMenuItem_Click(sender As Object, e As EventArgs)
        AdjustIsTop(Not IsTopMenuItem.Checked)
    End Sub

    Friend Sub MonitorableAllMenuItem_Click(sender As Object, e As EventArgs)
        MonitorableAllMenuItem.Checked = Not MonitorableAllMenuItem.Checked
        If AllDisabled = 0 Then
            ForEachMonitorableMenuItem(Sub(ByVal Item As MenuItem)
                                           Item.Enabled = MonitorableAllMenuItem.Checked
                                           Item.Checked = True
                                       End Sub)
            AllDisabled = 1
        Else
            ForEachMonitorableMenuItem(Sub(ByVal Item As MenuItem)
                                           Item.Enabled = MonitorableAllMenuItem.Checked
                                       End Sub)
        End If
    End Sub

    Friend Sub MonitorableMenuItem_Click(sender As Object, e As EventArgs)
        AllDisabled = 0
        sender.Checked = Not sender.Checked
        ForEachMonitorableMenuItem(Sub(ByVal Item As MenuItem)
                                       AllDisabled += Convert.ToInt32(Item.Checked)
                                   End Sub)
        If AllDisabled = 0 Then
            MonitorableAllMenuItem.Checked = False
            ForEachMonitorableMenuItem(Sub(ByVal Item As MenuItem)
                                           Item.Enabled = False
                                       End Sub)
        Else
            AllDisabled = 1
        End If
    End Sub

#End Region

#Region "Js & Net"

    Friend Sub InitClipboardUI()
        ClipboardUI = New WebView(MainForm.WebViewPanel)
        '只有参数命令的交互函数
        JsValue.BindFunction("getCommand1",
                             New wkeJsNativeFunction(AddressOf getCommand1), 1)
        '除了参数命令外，还有一个参数的交互函数
        JsValue.BindFunction("getCommand2",
                             New wkeJsNativeFunction(AddressOf getCommand2), 2)
        '除了参数命令外，还有两个参数的交互函数
        JsValue.BindFunction("getCommand3",
                             New wkeJsNativeFunction(AddressOf getCommand3), 3)
        ClipboardUI.LoadURL(ClipboardUIPath)
    End Sub

#Region "Net -> Js"

    Friend Sub Notify(ByVal Msg As String, Optional ByVal Type As NotifyType = NotifyType.primary)
        'vant组件中的通知显示
        ClipboardUI.RunJS($"notify(""{Type}"",""{Msg}"")")
    End Sub

    Friend Sub AddRecord()
        '判定内部复制
        If innerData <> -1 Then
            innerData -= 1
            Return
        End If

        If MonitorableAllMenuItem.Checked Then
            Dim iData As IDataObject = Windows.Forms.Clipboard.GetDataObject()
            If iData IsNot Nothing Then
                For Each Item As (
                    Format As String,
                    AddAction As Action(Of IDataObject),
                    CopyAction As Action(Of Integer),
                    IgnoreAction As Action(Of Integer),
                    EditAction As Action(Of Integer)
                    ) In ActionWithFormat
                    If iData.GetDataPresent(Item.Format) Then
                        For Each CheckItem As MenuItem In MonitorableMenuItems
                            If CheckItem.Tag(1) = Item.Format Then
                                If CheckItem.Checked Then
                                    Item.AddAction(iData)
                                End If
                                Exit For
                            End If
                        Next
                        Exit For
                    End If
                Next
            End If
        End If
    End Sub

#End Region

#Region "Js -> Net"

#Disable Warning IDE0060 ' 删除未使用的参数
    Private Function getCommand(ByVal jsExecState As IntPtr, ByVal param As IntPtr) As Long
#Enable Warning IDE0060 ' 删除未使用的参数

        '获取参数命令
        Dim Command As CommandName = JsValue.Arg(jsExecState, 0).ToInt32(jsExecState)
        For Each Cmd As (
            CmdName As CommandName,
            CmdFunc As Func(Of IntPtr, JsValue)
            ) In FuncWithCmd
            If Command = Cmd.CmdName Then
                Return Cmd.CmdFunc(jsExecState)
            End If
        Next
        Return JsValue.UndefinedValue
    End Function

    Public Function getCommand1(ByVal jsExecState As IntPtr, ByVal param As IntPtr) As Long
        Return getCommand(jsExecState, param)
    End Function

    Public Function getCommand2(ByVal jsExecState As IntPtr, ByVal param As IntPtr) As Long
        Return getCommand(jsExecState, param)
    End Function

    Public Function getCommand3(ByVal jsExecState As IntPtr, ByVal param As IntPtr) As Long
        Return getCommand(jsExecState, param)
    End Function

#End Region

#End Region

End Module
