Imports Clipboard.ClipboardApi
Imports Clipboard.JsonConverter
Imports Clipboard.LangClass
Imports Kyozy.MiniblinkNet
Imports System.IO
Imports System.Speech.Synthesis

Public Class MainForm

    Friend WithEvents ClipboardUI As WebView

    '已复制的数据列表
    Friend DataList As List(Of DataTag)

    '设置结构
    Friend Settings As Settings

    '解决复制时两次更新出现复制两次的情况
    Friend innerData As Integer = -1

    '确定是否初始化
    Friend init As Boolean = True

#Region "Js & Net"

    Private Sub InitClipboardUI()
        ClipboardUI = New WebView(WebViewPanel)

        '只有参数命令的交互函数
        JsValue.BindFunction("getCommand1", New wkeJsNativeFunction(AddressOf getCommand1), 1)

        '除了参数命令外，还有一个参数的交互函数
        JsValue.BindFunction("getCommand2", New wkeJsNativeFunction(AddressOf getCommand2), 2)

        '除了参数命令外，还有两个参数的交互函数
        JsValue.BindFunction("getCommand3", New wkeJsNativeFunction(AddressOf getCommand3), 3)

        ClipboardUI.LoadURL(Path.Combine(Application.StartupPath, "ClipboardUI\ClipboardUI.html"))
    End Sub

#Region "Net -> Js"

    Private Sub Notify(ByVal Msg As String, Optional ByVal Type As NotifyType = NotifyType.primary)
        'vant组件中的通知显示
        ClipboardUI.RunJS(String.Format("notify(""{0}"",""{1}"")", Type.ToString(), Msg))
    End Sub

    Private Sub AddRecord()

        '判定内部复制
        If innerData <> -1 Then
            innerData -= 1
            Return
        End If

        Dim iData As IDataObject = Windows.Forms.Clipboard.GetDataObject()
        If iData IsNot Nothing AndAlso iData.GetDataPresent(DataFormats.Text) Then

            '判定前一段复制的文本与此时复制的是否相同
            Dim PreText As String = CType(iData.GetData(DataFormats.Text), String)
            If DataList.Count = 0 OrElse Not PreText = DataList.Last.text Then
                Dim Temp As New DataTag With {
                                    .type = 21,
                                    .time = Now.ToString("yyyy/MM/dd HH:mm"),
                                    .text = PreText,
                                    .id = DataList.Count.ToString,
                                    .lock = False
                                }
                '添加记录
                ClipboardUI.RunJS(String.Format("add('{0}')", Object2Json(Temp, True)))
                DataList.Add(Temp)
                UpdateSeriFile()
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
                ActionSpeech(SettingsName.SpeechClearall, SpecText(25))

                Return JsValue.UndefinedValue

            Case CommandName.OpenSetting
                ActionSpeech(SettingsName.SpeechSetting, SpecText(26))

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
                Windows.Forms.Clipboard.SetText(DataList(index).text)
                ActionSpeech(SettingsName.SpeechCopy, SpecText(19))

            Case CommandName.Ignore
                Dim index As Integer = JsValue.Arg(jsExecState, 1).ToInt32(jsExecState)
                DataList.RemoveAt(index)
                For i = index To DataList.Count - 1
                    DataList(i) = New DataTag With {
                                            .type = DataList(i).type,
                                            .time = DataList(i).time,
                                            .text = DataList(i).text,
                                            .id = i.ToString,
                                            .lock = DataList(i).lock
                                        }
                Next
                UpdateSeriFile()
                ActionSpeech(SettingsName.SpeechIgnore, SpecText(24))

            Case CommandName.Edit
                Dim index As Integer = JsValue.Arg(jsExecState, 1).ToInt32(jsExecState)
                ActionSpeech(SettingsName.SpeechEdit, SpecText(27) & " " & NumSuffix(DataList.Count - index) & " " & SpecText(28))

            Case CommandName.Opacity
                Dim opacity As Integer = JsValue.Arg(jsExecState, 1).ToInt32(jsExecState)
                Me.Opacity = opacity / 100

            Case CommandName.Lock
                Dim index As Integer = JsValue.Arg(jsExecState, 1).ToInt32(jsExecState)
                DataList(index) = New DataTag With {
                                            .type = DataList(index).type,
                                            .time = DataList(index).time,
                                            .text = DataList(index).text,
                                            .id = index.ToString,
                                            .lock = Not DataList(index).lock
                                        }
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
                        IsTopToolStripMenuItem.Text = SpecText(9) & "(&T)"
                        ExitToolStripMenuItem.Text = SpecText(23) & "(&E)"
                        WriteCore(OtherSec, [property], Settings.Language.ToString)
                    Case Else

                End Select

            Case CommandName.EditText
                Dim index As Integer = JsValue.Arg(jsExecState, 1).ToInt32(jsExecState)
                Dim text As String = JsValue.Arg(jsExecState, 2).ToString(jsExecState)
                DataList(index) = New DataTag With {
                                            .type = DataList(index).type,
                                            .time = DataList(index).time,
                                            .text = text,
                                            .id = DataList(index).id,
                                            .lock = DataList(index).lock
                                        }
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
                AddRecord()

            Case Else

        End Select

        MyBase.WndProc(m)
    End Sub

#End Region

#Region "MainForm"

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ClipboardNotifyIcon.Icon = Icon.Clone()
        SettingsHelper = New INIHelper(INIPath)
        '读取设置
        Settings = ReadSettings()
        Opacity = Settings.Opacity / 100
        IsTopToolStripMenuItem.Checked = Settings.IsTop
        IsTopToolStripMenuItem.Text = SpecText(9) & "(&T)"
        ExitToolStripMenuItem.Text = SpecText(23) & "(&E)"
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

        KeyPreview = True
        init = False
    End Sub

    Private Sub MainForm_KeyUp(sender As Object, e As KeyEventArgs) Handles MyBase.KeyUp

        '设置置顶
        If e.Modifiers = Keys.Alt AndAlso e.KeyCode = Keys.T Then

            IsTopToolStripMenuItem.Checked = Not IsTopToolStripMenuItem.Checked

            '退出
        ElseIf e.Modifiers = Keys.Alt AndAlso e.KeyCode = Keys.E Then

            Close()

        End If

    End Sub

#End Region

#Region "ClipboardNotifyIcon"

    Private Sub ClipboardNotifyIcon_MouseClick(sender As Object, e As MouseEventArgs) Handles ClipboardNotifyIcon.MouseClick
        If e.Button = MouseButtons.Left Then
            Visible = Not Visible
            If Visible Then
                Activate()
            End If
        End If
    End Sub

#End Region

#Region "IconContextMenuStrip"

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        Close()
    End Sub

    Private Sub IsTopToolStripMenuItem_CheckedChanged(sender As Object, e As EventArgs) Handles IsTopToolStripMenuItem.CheckedChanged
        TopMost = IsTopToolStripMenuItem.Checked
        If init = True Then
            init = False
        Else
            Dim Msg As String =
                If(
                IsTopToolStripMenuItem.Checked,
                SpecText(20),
                SpecText(22)
                )
            Notify(Msg)
            ActionSpeech(SettingsName.SpeechTop, Msg)
        End If
    End Sub

#End Region

End Class
