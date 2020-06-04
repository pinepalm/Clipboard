Imports Clipboard.ClipboardApi
Imports Clipboard.JsonConverter
Imports System.IO
Imports System.Collections.Specialized

Module DataActionModule

    Friend MyRandom As New Random()             '给每一条记录设置唯一标识(Key)

    Public ActionWithFormat As IList(Of (
        Format As String,
        AddAction As Action(Of IDataObject),    '指示如何添加记录的操作
        CopyAction As Action(Of Integer),       '指示如何复制记录的操作
        IgnoreAction As Action(Of Integer),     '指示除忽略记录操作外的附加操作
        EditAction As Action(Of Integer)        '指示如何设置传回的记录Text值的操作
        )) =
        New List(Of (
        Format As String,
        AddAction As Action(Of IDataObject),
        CopyAction As Action(Of Integer),
        IgnoreAction As Action(Of Integer),
        EditAction As Action(Of Integer)
        ))({
        (DataFormats.Text, Sub(ByVal iData As IDataObject)
                               Dim PreText As String
                               PreText = CType(iData.GetData(DataFormats.Text), String)
                               If DataList.Count = 0 OrElse Not PreText = DataList.Last.text Then
                                   Dim Temp As New DataTag With {
                                                       .type = DataFormats.Text,
                                                       .time = Time(TimeFormatter),
                                                       .text = PreText,
                                                       .key = MD5Encode($"{Time(TimeFormatter1)} {MyRandom.Next()}"),
                                                       .lock = False
                                                   }
                                   '添加记录
                                   ClipboardUI.RunJS($"add('{Object2Json(Temp, True)}')")
                                   DataList.Add(Temp)
                                   UpdateSeriFile()
                               End If
                           End Sub,
                           Sub(ByVal Index As Integer)
                               Windows.Forms.Clipboard.SetText(DataList(Index).text)
                           End Sub,
                           Sub(ByVal Index As Integer)

                           End Sub,
                           Sub(ByVal Index As Integer)

                           End Sub),
        (DataFormats.FileDrop, Sub(ByVal iData As IDataObject)
                                   Dim PreText As String
                                   Dim PreFileDrop As String() = iData.GetData(DataFormats.FileDrop)
                                   For i = 0 To PreFileDrop.Length - 1
                                       PreFileDrop(i) &= GetFileType(PreFileDrop(i))
                                   Next
                                   PreText = String.Join("|", PreFileDrop)
                                   If DataList.Count = 0 OrElse Not PreText = DataList.Last.text Then
                                       Dim Temp As New DataTag With {
                                                            .type = DataFormats.FileDrop,
                                                            .time = Time(TimeFormatter),
                                                            .text = PreText,
                                                            .key = MD5Encode($"{Time(TimeFormatter1)} {MyRandom.Next()}"),
                                                            .lock = False
                                                        }
                                       '添加记录
                                       ClipboardUI.RunJS($"add('{Object2Json(Temp, True)}')")
                                       DataList.Add(Temp)
                                       UpdateSeriFile()
                                   End If
                               End Sub,
                               Sub(ByVal Index As Integer)
                                   Dim TempFileDrop As New StringCollection()
                                   Dim TempPaths As String() = DataList(Index).text.Split(New Char() {"|"c})
                                   For Each TempPath As String In TempPaths
                                       Dim Path As String() = TempPath.Split(New Char() {"*"c})
                                       If File.Exists(Path(0)) OrElse Directory.Exists(Path(0)) Then
                                           TempFileDrop.Add(Path(0))
                                       End If
                                   Next
                                   Windows.Forms.Clipboard.SetFileDropList(TempFileDrop)
                               End Sub,
                               Sub(ByVal Index As Integer)

                               End Sub,
                               Sub(ByVal Index As Integer)

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
                                                        .type = DataFormats.Bitmap,
                                                        .time = Time(TimeFormatter),
                                                        .text = PreText,
                                                        .key = PreKey,
                                                        .lock = False
                                                    }
                                 '添加记录
                                 PreImage.Save(Path.Combine(Application.StartupPath, $"{ImageDataName}\{PreKey}.png"), Imaging.ImageFormat.Png)
                                 ClipboardUI.RunJS($"add('{Object2Json(Temp, True)}')")
                                 DataList.Add(Temp)
                                 UpdateSeriFile()
                                 PreImage.Dispose()
                             End Sub,
                             Sub(ByVal Index As Integer)
                                 Try
                                     Dim TempImage As Image = Image.FromFile(DataList(Index).text.Substring(3))
                                     Windows.Forms.Clipboard.SetImage(TempImage)
                                     TempImage.Dispose()
                                 Catch ex As Exception
                                     LogRecord(ex.Message)
                                 End Try
                             End Sub,
                             Sub(ByVal Index As Integer)
                                 Try
                                     File.Delete(Path.Combine(Application.StartupPath, DataList(Index).text.Substring(3)))
                                 Catch ex As Exception
                                     LogRecord(ex.Message)
                                 End Try
                             End Sub,
                             Sub(ByVal Index As Integer)

                             End Sub)
        })
End Module
