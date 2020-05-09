Imports Clipboard.LangClass
Imports Clipboard.ClipboardApi
Imports Clipboard.JsonConverter
Imports Kyozy.MiniblinkNet
Imports System.IO
Imports System.Speech.Synthesis
Imports System.Collections.Specialized

Public Class MainForm
    Friend WithEvents ClipboardUI As WebView    '显示UI
    Friend DataList As List(Of DataTag)         '已复制的数据列表
    Friend Settings As Settings                 '设置结构
    Friend innerData As Integer = -1            '解决复制时两次更新出现复制两次的情况

#Region "Js & Net"

    Private Sub InitClipboardUI()
        ClipboardUI = New WebView(WebViewPanel)
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

    Private Sub Notify(ByVal Msg As String, Optional ByVal Type As NotifyType = NotifyType.primary)
        'vant组件中的通知显示
        ClipboardUI.RunJS($"notify(""{Type}"",""{Msg}"")")
    End Sub

    Private Sub AddRecord()
        '判定内部复制
        If innerData <> -1 Then
            innerData -= 1
            Return
        End If

        Dim PreText As String
        Dim iData As IDataObject = Windows.Forms.Clipboard.GetDataObject()
        If iData IsNot Nothing Then
            If iData.GetDataPresent(DataFormats.Text) Then
                '判定前一段复制的文本与此时复制的是否相同
                PreText = CType(iData.GetData(DataFormats.Text), String)
                If DataList.Count = 0 OrElse Not PreText = DataList.Last.text Then
                    Dim Temp As New DataTag With {
                                        .type = 21,
                                        .time = Time(TimeFormatter),
                                        .text = PreText,
                                        .id = DataList.Count.ToString,
                                        .key = MD5Encode($"{ .type} { .time} { .text}"),
                                        .lock = False
                                    }
                    '添加记录
                    ClipboardUI.RunJS($"add('{Object2Json(Temp, True)}')")
                    DataList.Add(Temp)
                    UpdateSeriFile()
                End If
            ElseIf iData.GetDataPresent(DataFormats.FileDrop) Then
                Dim PreFileDrop As String() = iData.GetData(DataFormats.FileDrop)
                For i = 0 To PreFileDrop.Length - 1
                    PreFileDrop(i) &= GetFileType(PreFileDrop(i))
                Next
                PreText = String.Join("|", PreFileDrop)
                If DataList.Count = 0 OrElse Not PreText = DataList.Last.text Then
                    Dim Temp As New DataTag With {
                                        .type = 41,
                                        .time = Time(TimeFormatter),
                                        .text = PreText,
                                        .id = DataList.Count.ToString,
                                        .key = MD5Encode($"{ .type} { .time} { .text}"),
                                        .lock = False
                                    }
                    '添加记录
                    ClipboardUI.RunJS($"add('{Object2Json(Temp, True)}')")
                    DataList.Add(Temp)
                    UpdateSeriFile()
                End If
            ElseIf iData.GetDataPresent(DataFormats.Bitmap) Then
                Dim PreImage As Image = CType(iData.GetData(DataFormats.Bitmap), Image)
                CheckUiDirectory(ImageDataName, Sub()
                                                    Directory.CreateDirectory(ImageDataPath)
                                                End Sub)
                PreText = $"../{ImageDataName}/{DataList.Count}.png"
                Dim Temp As New DataTag With {
                                        .type = 43,
                                        .time = Time(TimeFormatter),
                                        .text = PreText,
                                        .id = DataList.Count.ToString,
                                        .key = MD5Encode($"{ .type} { .time} { .text}"),
                                        .lock = False
                                    }
                '添加记录
                PreImage.Save(Path.Combine(Application.StartupPath, $"{ImageDataName}\{DataList.Count}.png"), Imaging.ImageFormat.Png)
                ClipboardUI.RunJS($"add('{Object2Json(Temp, True)}')")
                DataList.Add(Temp)
                UpdateSeriFile()
                PreImage.Dispose()
            End If
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

            Case CommandName.AddFile
                Dim SendText As String = String.Empty
                If AddFileDialog.ShowDialog() = DialogResult.OK Then
                    Dim Temp As String() = AddFileDialog.FileNames.Clone()
                    For i = 0 To Temp.Length - 1
                        Temp(i) &= GetFileType(Temp(i))
                    Next
                    SendText = String.Join("|", Temp)
                End If
                Return JsValue.StringValue(jsExecState, SendText)

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
                Select Case DataList(index).type
                    Case LanguageTextEnum.Text
                        Windows.Forms.Clipboard.SetText(DataList(index).text)
                    Case LanguageTextEnum.Document
                        Dim TempFileDrop As New StringCollection()
                        Dim TempPaths As String() = DataList(index).text.Split(New Char() {"|"c})
                        For Each TempPath As String In TempPaths
                            Dim Path As String() = TempPath.Split(New Char() {"*"c})
                            If File.Exists(Path(0)) OrElse Directory.Exists(Path(0)) Then
                                TempFileDrop.Add(Path(0))
                            End If
                        Next
                        Windows.Forms.Clipboard.SetFileDropList(TempFileDrop)
                    Case LanguageTextEnum.Image
                        Try
                            Dim TempImage As Image = Image.FromFile(DataList(index).text.Substring(3))
                            Windows.Forms.Clipboard.SetImage(TempImage)
                            TempImage.Dispose()
                        Catch ex As Exception
                            LogRecord(ex.Message)
                        End Try
                End Select
                ActionSpeech(SettingsName.SpeechCopy, SpecText(LanguageTextEnum.CopySuccessfully))

            Case CommandName.Ignore
                Dim index As Integer = JsValue.Arg(jsExecState, 1).ToInt32(jsExecState)
                If DataList(index).type = LanguageTextEnum.Image Then
                    Try
                        File.Delete(Path.Combine(Application.StartupPath, DataList(index).text.Substring(3)))
                    Catch ex As Exception
                        LogRecord(ex.Message)
                    End Try
                End If
                DataList.RemoveAt(index)
                For i = index To DataList.Count - 1
                    DataList(i).id = i.ToString()
                Next
                UpdateSeriFile()
                ActionSpeech(SettingsName.SpeechIgnore, SpecText(LanguageTextEnum.HaveIgnored))

            Case CommandName.Edit
                Dim index As Integer = JsValue.Arg(jsExecState, 1).ToInt32(jsExecState)
                ActionSpeech(SettingsName.SpeechEdit, $"{SpecText(LanguageTextEnum.EditingRecent)} {NumSuffix(DataList.Count - index)} {SpecText(LanguageTextEnum.Record)}")

            Case CommandName.Opacity
                Dim opacity As Integer = JsValue.Arg(jsExecState, 1).ToInt32(jsExecState)
                Me.Opacity = opacity / 100

            Case CommandName.Lock
                Dim index As Integer = JsValue.Arg(jsExecState, 1).ToInt32(jsExecState)
                DataList(index).lock = Not DataList(index).lock
                UpdateSeriFile()

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
                        Settings.IsTop = value
                        IsTopToolStripMenuItem.Checked = Settings.IsTop
                        WriteCore(OtherSec, [property], value.ToString)
                    Case 8
                        Settings.Opacity = value
                        WriteCore(OtherSec, [property], value.ToString)
                    Case 9
                        Settings.Language = value
                        SpeechMsg.Culture = New Globalization.CultureInfo(Settings.Language.ToString.Replace("_", "-"))
                        MonitorableToolStripMenuItem.Text = $"{SpecText(LanguageTextEnum.Monitorable)}(&L)"
                        IsTopToolStripMenuItem.Text = SpecText(LanguageTextEnum.IsTop) & "(&T)"
                        ExitToolStripMenuItem.Text = SpecText(LanguageTextEnum.Exit) & "(&E)"
                        AddFileDialog.Title = SpecText(LanguageTextEnum.Add)
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

#Region "WndProc"

    Protected Overrides Sub WndProc(ByRef m As Message)
        Select Case m.Msg
            '构造窗体
            Case WindowMsg.WM_CREATE
                SetDpiAwareness()
                AddClipboardFormatListener(Handle)

                LogRecord("START")
            '销毁窗体
            Case WindowMsg.WM_DESTROY
                RemoveClipboardFormatListener(Handle)
                If ClipboardUI IsNot Nothing Then
                    ClipboardUI.Dispose()
                End If
                Speecher.SpeakAsyncCancelAll()
                Speecher.Dispose()

                LogRecord("END")
            '收到剪贴板更新
            Case ClipboardMsg.WM_CLIPBOARDUPDATE
                If MonitorableToolStripMenuItem.Checked Then
                    AddRecord()
                End If

            Case Else

        End Select
        MyBase.WndProc(m)
    End Sub

#End Region

#Region "MainForm"

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ClipboardNotifyIcon.Icon = Icon.Clone()
        '读取设置
        CheckUiDirectory(ClipboardUIDirName, Sub()
                                                 LogRecord("UI FILE IS MISSING")
                                                 MessageBox.Show(SpecText(LanguageTextEnum.CatastrophicFailure), Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                                 Application.Exit()
                                             End Sub)
        CheckUiDirectory(ImageDataName, Sub()
                                            Directory.CreateDirectory(ImageDataPath)
                                        End Sub)
        SettingsHelper = New INIHelper(INIPath)
        Settings = ReadSettings()
        Opacity = Settings.Opacity / 100
        IsTopToolStripMenuItem.Checked = Settings.IsTop
        TopMost = IsTopToolStripMenuItem.Checked
        MonitorableToolStripMenuItem.Text = $"{SpecText(LanguageTextEnum.Monitorable)}(&L)"
        IsTopToolStripMenuItem.Text = $"{SpecText(LanguageTextEnum.IsTop)}(&T)"
        ExitToolStripMenuItem.Text = $"{SpecText(LanguageTextEnum.Exit)}(&E)"
        AddFileDialog.Title = SpecText(LanguageTextEnum.Add)
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

        AddHandler KeyUp,
            AddressOf MainForm_KeyUp

        AddHandler ClipboardNotifyIcon.MouseClick,
            AddressOf ClipboardNotifyIcon_MouseClick

        AddHandler ExitToolStripMenuItem.Click,
            AddressOf ExitToolStripMenuItem_Click

        AddHandler IsTopToolStripMenuItem.CheckedChanged,
            AddressOf IsTopToolStripMenuItem_CheckedChanged

        KeyPreview = True
    End Sub

    Private Sub MainForm_KeyUp(sender As Object, e As KeyEventArgs)
        Select Case e.Modifiers
            Case Keys.Alt
                Select Case e.KeyCode
                    Case Keys.T '设置置顶
                        IsTopToolStripMenuItem.Checked = Not IsTopToolStripMenuItem.Checked
                    Case Keys.E '退出
                        Close()
                End Select
        End Select
    End Sub

#End Region

#Region "ClipboardNotifyIcon"

    Private Sub ClipboardNotifyIcon_MouseClick(sender As Object, e As MouseEventArgs)
        If e.Button = MouseButtons.Left Then
            Visible = Not Visible
            If Visible Then
                Activate()
            End If
        End If
    End Sub

#End Region

#Region "IconContextMenuStrip"

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs)
        Close()
    End Sub

    Private Sub IsTopToolStripMenuItem_CheckedChanged(sender As Object, e As EventArgs)
        TopMost = IsTopToolStripMenuItem.Checked
        Dim Msg As String =
                        If(
                        IsTopToolStripMenuItem.Checked,
                        SpecText(LanguageTextEnum.Top),
                        SpecText(LanguageTextEnum.NonTop)
                        )
        Notify(Msg)
        ActionSpeech(SettingsName.SpeechTop, Msg)
    End Sub

#End Region

End Class
