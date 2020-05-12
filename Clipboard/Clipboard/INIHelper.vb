Imports Clipboard.ClipboardApi
Imports System.Text

Public Class INIHelper

    ''' <summary>
    ''' 操作的INI文件地址
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property FilePath As String

    Public Sub New(ByVal FilePath As String)
        Me.FilePath = FilePath
    End Sub

    ''' <summary>
    ''' 读取键值
    ''' </summary>
    ''' <param name="Section">节点名</param>
    ''' <param name="Key">键名</param>
    ''' <param name="DefaultValue">读取失败返回的默认值</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Read(ByVal Section As String, ByVal Key As String, ByVal DefaultValue As String) As String
        Dim temp As New StringBuilder(1024)
        GetPrivateProfileString(Section, Key, DefaultValue, temp, 1024, FilePath)
        Return temp.ToString()
    End Function

    ''' <summary>
    ''' 写入键值
    ''' </summary>
    ''' <param name="Section">节点名</param>
    ''' <param name="Key">键名</param>
    ''' <param name="Value">键值</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Write(ByVal Section As String, ByVal Key As String, ByVal Value As String) As Integer
        Return WritePrivateProfileString(Section, Key, Value, FilePath)
    End Function

    ''' <summary>
    ''' 删除节点
    ''' </summary>
    ''' <param name="Section">节点名</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DeleteSection(ByVal Section As String) As Integer
        Return Write(Section, Nothing, Nothing)
    End Function

    ''' <summary>
    ''' 删除键值
    ''' </summary>
    ''' <param name="Section">节点名</param>
    ''' <param name="Key">键名</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DeleteKey(ByVal Section As String, ByVal Key As String) As Integer
        Return Write(Section, Key, Nothing)
    End Function

End Class
