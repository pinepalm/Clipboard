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
#End Region

    Friend AddFolderDialog As VistaFolderBrowserDialog  'Vista样式文件选择对话框
    Friend DataList As List(Of DataTag)                 '已复制的数据列表
    Friend Settings As Settings                         '设置结构
    Friend innerData As Integer = -1                    '解决复制时两次更新出现复制两次的情况
    Friend CloseFlag As Boolean = False                 '点击右上角关闭按钮最小化到托盘

    Friend Sub Initialize()
        MainForm.Size = New Size(442, 884)
        MainForm.MinimumSize = New Size(400, 400)
        AddFolderDialog = New VistaFolderBrowserDialog()
        '读取设置
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
        MainForm.Opacity = Settings.Opacity / 100
        MainForm.AddFileDialog.Title = SpecText(LanguageTextEnum.Add)
        AddFolderDialog.Title = SpecText(LanguageTextEnum.Add)

        MonitorableMenuItem = New MenuItem With {
            .Text = $"{SpecText(LanguageTextEnum.Monitorable)}(&L)",
            .Checked = True
        }
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
        MainForm.KeyPreview = True
    End Sub

    Friend Sub AddEventHandler()
        AddHandler MainForm.KeyUp,
            AddressOf MainForm_KeyUp

        AddHandler MainForm.ClipboardNotifyIcon.MouseClick,
            AddressOf ClipboardNotifyIcon_MouseClick

        AddHandler ExitMenuItem.Click,
            AddressOf ExitMenuItem_Click

        AddHandler IsTopMenuItem.Click,
            AddressOf IsTopMenuItem_Click

        AddHandler MonitorableMenuItem.Click,
            AddressOf MonitorableMenuItem_Click
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

    Friend Sub MonitorableMenuItem_Click(sender As Object, e As EventArgs)
        MonitorableMenuItem.Checked = Not MonitorableMenuItem.Checked
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
                    Item.AddAction(iData)
                    Exit For
                End If
            Next
        End If
    End Sub

#End Region

#Region "Js -> Net"

    Public Function getCommand1(ByVal jsExecState As IntPtr, ByVal param As IntPtr) As Long
        '获取参数命令
        Dim Command As CommandName = JsValue.Arg(jsExecState, 0).ToInt32(jsExecState)
        Select Case Command
            Case CommandName.ClearAll
                DataList.Clear()
                UpdateSeriFile()
                DeleteFolderContent(ImageDataPath)
                ActionSpeech(SettingsName.SpeechClearall, SpecText(LanguageTextEnum.HaveClearedAll))

                Return JsValue.UndefinedValue

            Case CommandName.OpenSetting
                ActionSpeech(SettingsName.SpeechSetting, SpecText(LanguageTextEnum.HaveOpenedSettings))

                Return JsValue.UndefinedValue

            Case CommandName.SettingsJS
                Return JsValue.StringValue(jsExecState, Object2Json(Settings))

            Case CommandName.LanguageText
                Return JsValue.StringValue(jsExecState, Object2Json(LanguageText(Settings.Language)))

            Case CommandName.AddPre
                Return JsValue.StringValue(jsExecState, Object2Json(DataList))

            Case CommandName.LanguageType
                Return JsValue.StringValue(jsExecState, Object2Json(LanguageType))

            Case Else
                Return JsValue.UndefinedValue

        End Select
    End Function

    Public Function getCommand2(ByVal jsExecState As IntPtr, ByVal param As IntPtr) As Long
        '获取参数命令
        Dim Command As CommandName = JsValue.Arg(jsExecState, 0).ToInt32(jsExecState)
        Select Case Command
            Case CommandName.Copy
                Dim index As Integer = JsValue.Arg(jsExecState, 1).ToInt32(jsExecState)
                innerData = 1
                For Each Item As (
                Format As String,
                AddAction As Action(Of IDataObject),
                CopyAction As Action(Of Integer),
                IgnoreAction As Action(Of Integer),
                EditAction As Action(Of Integer)
                ) In ActionWithFormat
                    If DataList(index).type = Item.Format Then
                        Item.CopyAction(index)
                        Exit For
                    End If
                Next
                ActionSpeech(SettingsName.SpeechCopy, SpecText(LanguageTextEnum.CopySuccessfully))

            Case CommandName.Ignore
                Dim index As Integer = JsValue.Arg(jsExecState, 1).ToInt32(jsExecState)
                For Each Item As (
                Format As String,
                AddAction As Action(Of IDataObject),
                CopyAction As Action(Of Integer),
                IgnoreAction As Action(Of Integer),
                EditAction As Action(Of Integer)
                ) In ActionWithFormat
                    If DataList(index).type = Item.Format Then
                        Item.IgnoreAction(index)
                        Exit For
                    End If
                Next
                DataList.RemoveAt(index)
                UpdateSeriFile()
                ActionSpeech(SettingsName.SpeechIgnore, SpecText(LanguageTextEnum.HaveIgnored))

            Case CommandName.Edit
                Dim index As Integer = JsValue.Arg(jsExecState, 1).ToInt32(jsExecState)
                ActionSpeech(SettingsName.SpeechEdit, $"{SpecText(LanguageTextEnum.EditingRecent)} {NumSuffix(DataList.Count - index)} {SpecText(LanguageTextEnum.Record)}")

            Case CommandName.Opacity
                Dim opacity As Integer = JsValue.Arg(jsExecState, 1).ToInt32(jsExecState)
                MainForm.Opacity = opacity / 100

            Case CommandName.Lock
                Dim index As Integer = JsValue.Arg(jsExecState, 1).ToInt32(jsExecState)
                DataList(index).lock = Not DataList(index).lock
                UpdateSeriFile()

            Case CommandName.AddFile
                Dim SendText As String = String.Empty
                Dim fileOrfolder As Boolean = JsValue.Arg(jsExecState, 1).ToBoolean(jsExecState)
                If fileOrfolder Then
                    If AddFolderDialog.ShowDialog(MainForm) = DialogResult.OK Then
                        SendText = AddFolderDialog.DirectoryPath
                        SendText = If(String.IsNullOrEmpty(SendText), String.Empty, $"{SendText}{GetFileType(SendText)}")
                    End If
                Else
                    If MainForm.AddFileDialog.ShowDialog() = DialogResult.OK Then
                        Dim Temp As String() = MainForm.AddFileDialog.FileNames.Clone()
                        For i = 0 To Temp.Length - 1
                            Temp(i) &= GetFileType(Temp(i))
                        Next
                        SendText = String.Join("|", Temp)
                    End If
                End If
                Return JsValue.StringValue(jsExecState, SendText)

            Case Else

        End Select

        Return JsValue.UndefinedValue
    End Function

    Public Function getCommand3(ByVal jsExecState As IntPtr, ByVal param As IntPtr) As Long
        '获取参数命令
        Dim Command As CommandName = JsValue.Arg(jsExecState, 0).ToInt32(jsExecState)
        Select Case Command
            Case CommandName.SettingsNET
                Dim [property] As Integer = JsValue.Arg(jsExecState, 1).ToInt32(jsExecState)
                Dim value As Integer = JsValue.Arg(jsExecState, 2).ToInt32(jsExecState)

                Select Case [property]
                    Case 0 To 6
                        Settings.Speech([property]) = value
                        WriteCore(SpeechSec, [property], value.ToString)
                    Case 7
                        AdjustIsTop(value)
                    Case 8
                        Settings.Opacity = value
                        WriteCore(OtherSec, [property], value.ToString)
                    Case 9
                        Settings.Language = value
                        SpeechMsg.Culture = New Globalization.CultureInfo(Settings.Language.ToString.Replace("_", "-"))
                        MonitorableMenuItem.Text = $"{SpecText(LanguageTextEnum.Monitorable)}(&L)"
                        IsTopMenuItem.Text = SpecText(LanguageTextEnum.IsTop) & "(&T)"
                        ExitMenuItem.Text = SpecText(LanguageTextEnum.Exit) & "(&E)"
                        MainForm.AddFileDialog.Title = SpecText(LanguageTextEnum.Add)
                        WriteCore(OtherSec, [property], Settings.Language.ToString)
                    Case Else

                End Select

            Case CommandName.EditText
                Dim index As Integer = JsValue.Arg(jsExecState, 1).ToInt32(jsExecState)
                Dim text As String = JsValue.Arg(jsExecState, 2).ToString(jsExecState)
                DataList(index).text = text
                UpdateSeriFile()

            Case Else

        End Select

        Return JsValue.UndefinedValue
    End Function

#End Region

#End Region

End Module
