Imports Clipboard.LangClass
Imports Clipboard.ClipboardApi
Imports Clipboard.JsonConverter
Imports Kyozy.MiniblinkNet

Module CmdFuncModule
    Public FuncWithCmd As IList(Of (
        CmdName As CommandName,
        CmdFunc As Func(Of IntPtr, JsValue)
        )) =
        New List(Of (
        CmdName As CommandName,
        CmdFunc As Func(Of IntPtr, JsValue)
        ))({
        (CommandName.ClearAll, Function(ByVal jsExecState As IntPtr) As JsValue
                                   DataList.Clear()
                                   UpdateSeriFile()
                                   DeleteFolderContent(ImageDataPath)
                                   ActionSpeech(SettingsName.SpeechClearall, SpecText(LanguageTextEnum.HaveClearedAll))

                                   Return JsValue.UndefinedValue
                               End Function),
        (CommandName.OpenSetting, Function(ByVal jsExecState As IntPtr) As JsValue
                                      ActionSpeech(SettingsName.SpeechSetting, SpecText(LanguageTextEnum.HaveOpenedSettings))

                                      Return JsValue.UndefinedValue
                                  End Function),
        (CommandName.SettingsJS, Function(ByVal jsExecState As IntPtr) As JsValue
                                     Return JsValue.StringValue(jsExecState, Object2Json(Settings))
                                 End Function),
        (CommandName.LanguageText, Function(ByVal jsExecState As IntPtr) As JsValue
                                       Return JsValue.StringValue(jsExecState, Object2Json(LanguageText(Settings.Language)))
                                   End Function),
        (CommandName.AddPre, Function(ByVal jsExecState As IntPtr) As JsValue
                                 Return JsValue.StringValue(jsExecState, Object2Json(DataList))
                             End Function),
        (CommandName.LanguageType, Function(ByVal jsExecState As IntPtr) As JsValue
                                       Return JsValue.StringValue(jsExecState, Object2Json(LanguageType))
                                   End Function),  '-------------------------------------------------------------------------------------------
        (CommandName.Copy, Function(ByVal jsExecState As IntPtr) As JsValue
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

                               Return JsValue.UndefinedValue
                           End Function),
        (CommandName.Ignore, Function(ByVal jsExecState As IntPtr) As JsValue
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

                                 Return JsValue.UndefinedValue
                             End Function),
        (CommandName.Edit, Function(ByVal jsExecState As IntPtr) As JsValue
                               Dim index As Integer = JsValue.Arg(jsExecState, 1).ToInt32(jsExecState)
                               ActionSpeech(SettingsName.SpeechEdit, $"{SpecText(LanguageTextEnum.EditingRecent)} {NumSuffix(DataList.Count - index)} {SpecText(LanguageTextEnum.Record)}")

                               Return JsValue.UndefinedValue
                           End Function),
        (CommandName.Opacity, Function(ByVal jsExecState As IntPtr) As JsValue
                                  Dim opacity As Integer = JsValue.Arg(jsExecState, 1).ToInt32(jsExecState)
                                  MainForm.Opacity = opacity / 100

                                  Return JsValue.UndefinedValue
                              End Function),
        (CommandName.Lock, Function(ByVal jsExecState As IntPtr) As JsValue
                               Dim index As Integer = JsValue.Arg(jsExecState, 1).ToInt32(jsExecState)
                               DataList(index).lock = Not DataList(index).lock
                               UpdateSeriFile()

                               Return JsValue.UndefinedValue
                           End Function),
        (CommandName.AddFile, Function(ByVal jsExecState As IntPtr) As JsValue
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
                              End Function),  '-------------------------------------------------------------------------------------------
        (CommandName.SettingsNET, Function(ByVal jsExecState As IntPtr) As JsValue
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
                                              MonitorableMenuItem.Text = SpecText(LanguageTextEnum.Monitorable)
                                              IsTopMenuItem.Text = $"{SpecText(LanguageTextEnum.IsTop)}(&T)"
                                              ExitMenuItem.Text = $"{SpecText(LanguageTextEnum.Exit)}(&E)"
                                              MonitorableAllMenuItem.Text = SpecText(LanguageTextEnum.All)
                                              ForEachMonitorableMenuItem(Sub(ByVal Item As MenuItem)
                                                                             Item.Text = SpecText(Item.Tag(0))
                                                                         End Sub)
                                              MainForm.AddFileDialog.Title = SpecText(LanguageTextEnum.Add)
                                              WriteCore(OtherSec, [property], Settings.Language.ToString)
                                          Case Else

                                      End Select

                                      Return JsValue.UndefinedValue
                                  End Function),
        (CommandName.EditText, Function(ByVal jsExecState As IntPtr) As JsValue
                                   Dim index As Integer = JsValue.Arg(jsExecState, 1).ToInt32(jsExecState)
                                   Dim text As String = JsValue.Arg(jsExecState, 2).ToString(jsExecState)
                                   DataList(index).text = text
                                   UpdateSeriFile()

                                   Return JsValue.UndefinedValue
                               End Function)
        })
End Module
