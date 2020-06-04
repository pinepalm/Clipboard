Imports Clipboard.LangClass
Imports Clipboard.ClipboardApi
Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary

Module FileModule

    ''' <summary>
    ''' 语音设置节点
    ''' </summary>
    ''' <remarks></remarks>
    Friend Const SpeechSec As String = "Speech"

    ''' <summary>
    ''' 其它设置节点
    ''' </summary>
    ''' <remarks></remarks>
    Friend Const OtherSec As String = "Other"

    Friend Const TimeFormatter As String = "yyyy/MM/dd HH:mm"
    Friend Const TimeFormatter1 As String = "yyyy/MM/dd HH:mm:ss.fff"

    Friend FileTypeId As String = Convert.ToInt32(LanguageTextEnum.Document).ToString()
    Friend FolderTypeId As String = Convert.ToInt32(LanguageTextEnum.Folder).ToString()

    Friend SettingsHelper As INIHelper

    '-------------------------------------------------------------------------------
    Friend Const ImageDataName As String = "ImageData"
    Friend ImageDataPath As String = Path.Combine(Application.StartupPath, ImageDataName)

    Friend Const ClipboardUIDirName As String = "ClipboardUI"
    Friend ClipboardUIName As String = $"{ClipboardUIDirName}\ClipboardUI.html"
    Friend ClipboardUIPath As String = Path.Combine(Application.StartupPath, ClipboardUIName)

    Friend Const LogName As String = "Clipboard.log"
    Friend LogPath As String = Path.Combine(Application.StartupPath, LogName)

    Friend Const ININame As String = "Clipboard.ini"
    Friend INIPath As String = Path.Combine(Application.StartupPath, ININame)

    Friend Const DATName As String = "Clipboard.dat"
    Friend DATPath As String = Path.Combine(Application.StartupPath, DATName)

    Friend ReadOnly Property Time(Optional ByVal Formatter As String = "yyyy/MM/dd HH:mm:ss") As String
        Get
            Return Now.ToString(Formatter)
        End Get
    End Property

    Friend Function GetFileType(ByVal Path As String) As String
        If File.Exists(Path) Then
            Return $"*{FileTypeId}"
        ElseIf Directory.Exists(path) Then
            Return $"*{FolderTypeId}"
        End If
        Return $"*{FileTypeId}"
    End Function

    Private Sub ForEachInEnumerable(Of T)(ByRef Enumerable As IEnumerable(Of T), ByVal Action As Action(Of T))
        For Each item As T In Enumerable
            Action(item)
        Next
    End Sub

    Friend Sub DeleteFolderContent(ByVal Path As String)
        If Directory.Exists(Path) Then
            Dim Temp As New DirectoryInfo(Path)
            ForEachInEnumerable(Temp.EnumerateFiles(),
                                Sub(F) F.Delete()
                                    )
            ForEachInEnumerable(Temp.EnumerateDirectories(),
                                Sub(D) D.Delete(True)
                                    )
        End If
    End Sub

    Friend Sub CheckUiDirectory(ByVal DirectoryName As String, ByVal Action As Action)
        If Not Directory.Exists(Path.Combine(Application.StartupPath, DirectoryName)) Then
            Action()
        End If
    End Sub

#Region "INI"

    ''' <summary>
    ''' 读取简化
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Friend Function ReadCore(ByVal Section As String, ByVal Index As Integer, ByVal DefaultValue As String) As Integer
        Return Convert.ToInt32(SettingsHelper.Read(Section, CType(Index, SettingsName).ToString, DefaultValue))
    End Function

    ''' <summary>
    ''' 写入简化
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Friend Function WriteCore(ByVal Section As String, ByVal Index As Integer, ByVal Value As String) As Integer
        InitSettings()
        Return SettingsHelper.Write(Section, CType(Index, SettingsName).ToString, Value)
    End Function

    ''' <summary>
    ''' 初始化设置
    ''' </summary>
    ''' <remarks></remarks>
    Friend Sub InitSettings()
        If Not File.Exists(INIPath) Then
            Using Fs As FileStream = File.Create(INIPath)
                Fs.Close()
            End Using

            For i = 0 To 6
                SettingsHelper.Write(SpeechSec, CType(i, SettingsName).ToString, "1")
            Next
            SettingsHelper.Write(OtherSec, CType(7, SettingsName).ToString, "1")
            SettingsHelper.Write(OtherSec, CType(8, SettingsName).ToString, "100")
            SettingsHelper.Write(OtherSec, CType(9, SettingsName).ToString, Language.zh_cn.ToString)
        End If
    End Sub

    ''' <summary>
    ''' 读取设置
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Friend Function ReadSettings() As Settings

        InitSettings()

        Dim Speech As Boolean() = New Boolean(6) {}
        Dim IsTop As Boolean, Opacity As Integer
        Dim Language As String
        Dim Language1 As Language

        For i = 0 To 6
            Speech(i) = ReadCore(SpeechSec, i, "1")
        Next
        IsTop = ReadCore(OtherSec, 7, "1")
        Opacity = ReadCore(OtherSec, 8, "100")
        Language = SettingsHelper.Read(OtherSec, CType(9, SettingsName).ToString, "zh_cn")
        Opacity = If(Opacity < 0 OrElse Opacity > 100, 100, Opacity)
        Try
            Language1 = [Enum].Parse(GetType(Language), Language, True)
        Catch ex As Exception
            Language1 = LangClass.Language.zh_cn
        End Try

        Return New Settings With {
            .Speech = Speech,
            .IsTop = IsTop,
            .Opacity = Opacity,
            .Language = Language1
        }
    End Function

#End Region

#Region "DAT"

    ''' <summary>
    ''' 更新记录文件
    ''' </summary>
    ''' <remarks></remarks>
    Friend Sub UpdateSeriFile()
        Try
            Using SeriStream As Stream = File.Open(DATPath, FileMode.Create)
                Dim BinFormatter As New BinaryFormatter
                BinFormatter.Serialize(SeriStream, DataList)
                SeriStream.Flush()
                SeriStream.Close()
                BinFormatter = Nothing
            End Using
        Catch ex As Exception
            LogRecord(ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' 读取记录文件
    ''' </summary>
    ''' <remarks></remarks>
    Friend Sub ReadSeriFile()

        If Not File.Exists(DATPath) Then
            DataList = New List(Of DataTag)
            UpdateSeriFile()
            Return
        End If

        Try
            Using SeriStream As Stream = File.Open(DATPath, FileMode.Open)
                Dim BinFormatter As New BinaryFormatter

                DataList = BinFormatter.Deserialize(SeriStream)  'Result

                SeriStream.Close()
                BinFormatter = Nothing
            End Using
        Catch ex As Exception
            LogRecord(ex.Message)
            MessageBox.Show(SpecText(LanguageTextEnum.CatastrophicFailure), MainForm.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Application.Exit()
        End Try
    End Sub

#End Region

#Region "LOG"

    ''' <summary>
    ''' 日志记录
    ''' </summary>
    ''' <remarks></remarks>
    Friend Sub LogRecord(ByVal Msg As String)
        Try
            If Not File.Exists(LogPath) Then
                Using Fs As FileStream = File.Create(LogPath)
                    Fs.Close()
                End Using
            End If

            Using Sw As StreamWriter = New StreamWriter(LogPath, True)
                Sw.WriteLine($"{Time}->{Msg}")
                Sw.Flush()
                Sw.Close()
            End Using
        Catch ex As Exception

        End Try
    End Sub

#End Region

End Module
