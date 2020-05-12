Imports Clipboard.LangClass
Imports Clipboard.JsonConverter
Imports Clipboard.ClipboardApi
Imports System.IO

Module DataActionModule

    Friend MyRandom As New Random()       '给每一条记录设置唯一标识(Key)

    Public ActionWithFormat As IList(Of (Format As String, Action As Action(Of IDataObject))) =
        New List(Of (Format As String, Action As Action(Of IDataObject)))({
        (DataFormats.Text, Sub(ByVal iData As IDataObject)
                               Dim PreText As String
                               PreText = CType(iData.GetData(DataFormats.Text), String)
                               If MainForm.DataList.Count = 0 OrElse Not PreText = MainForm.DataList.Last.text Then
                                   Dim Temp As New DataTag With {
                                                       .type = LanguageTextEnum.Text,
                                                       .time = Time(TimeFormatter),
                                                       .text = PreText,
                                                       .id = MainForm.DataList.Count.ToString,
                                                       .key = MD5Encode($"{Time(TimeFormatter1)} {MyRandom.Next()}"),
                                                       .lock = False
                                                   }
                                   '添加记录
                                   MainForm.ClipboardUI.RunJS($"add('{Object2Json(Temp, True)}')")
                                   MainForm.DataList.Add(Temp)
                                   UpdateSeriFile()
                               End If
                           End Sub),
        (DataFormats.FileDrop, Sub(ByVal iData As IDataObject)
                                   Dim PreText As String
                                   Dim PreFileDrop As String() = iData.GetData(DataFormats.FileDrop)
                                   For i = 0 To PreFileDrop.Length - 1
                                       PreFileDrop(i) &= GetFileType(PreFileDrop(i))
                                   Next
                                   PreText = String.Join("|", PreFileDrop)
                                   If MainForm.DataList.Count = 0 OrElse Not PreText = MainForm.DataList.Last.text Then
                                       Dim Temp As New DataTag With {
                                                            .type = LanguageTextEnum.Document,
                                                            .time = Time(TimeFormatter),
                                                            .text = PreText,
                                                            .id = MainForm.DataList.Count.ToString,
                                                            .key = MD5Encode($"{Time(TimeFormatter1)} {MyRandom.Next()}"),
                                                            .lock = False
                                                        }
                                       '添加记录
                                       MainForm.ClipboardUI.RunJS($"add('{Object2Json(Temp, True)}')")
                                       MainForm.DataList.Add(Temp)
                                       UpdateSeriFile()
                                   End If
                               End Sub),
        (DataFormats.Bitmap, Sub(ByVal iData As IDataObject)
                                 Dim PreText As String
                                 Dim PreImage As Image = CType(iData.GetData(DataFormats.Bitmap), Image)
                                 Dim PreKey As String = MD5Encode($"{Time(TimeFormatter1)} {MyRandom.Next()}")
                                 CheckUiDirectory(ImageDataName, Sub()
                                                                     Directory.CreateDirectory(ImageDataPath)
                                                                 End Sub)
                                 PreText = $"../{ImageDataName}/{PreKey}.png"
                                 Dim Temp As New DataTag With {
                                                        .type = LanguageTextEnum.Image,
                                                        .time = Time(TimeFormatter),
                                                        .text = PreText,
                                                        .id = MainForm.DataList.Count.ToString,
                                                        .key = PreKey,
                                                        .lock = False
                                                    }
                                 '添加记录
                                 PreImage.Save(Path.Combine(Application.StartupPath, $"{ImageDataName}\{PreKey}.png"), Imaging.ImageFormat.Png)
                                 MainForm.ClipboardUI.RunJS($"add('{Object2Json(Temp, True)}')")
                                 MainForm.DataList.Add(Temp)
                                 UpdateSeriFile()
                                 PreImage.Dispose()
                             End Sub)
        })
End Module
