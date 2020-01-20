Imports System.IO
Imports System.Runtime.Serialization.Json
Imports System.Text

Public Class JsonConverter

    ''' <summary>
    ''' 对象转为Json字符串
    ''' </summary>
    ''' <param name="[Object]">要转为Json的对象</param>
    ''' <param name="Complete">是否直接转为显式Json字符串</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function Object2Json(ByVal [Object] As Object, Optional ByVal Complete As Boolean = False) As String
        Try
            Dim Converter As New DataContractJsonSerializer([Object].GetType())
            Using DataStream As New MemoryStream()
                Converter.WriteObject(DataStream, [Object])
                Dim Bytes(DataStream.Length - 1) As Byte
                DataStream.Position = 0
                DataStream.Read(Bytes, 0, Bytes.Length)
                Return If(Complete, Encoding.UTF8.GetString(Bytes).Replace("\", "\\").Replace("'", "\'"), Encoding.UTF8.GetString(Bytes))
            End Using
        Catch ex As Exception
            Return String.Empty
        End Try
    End Function

    ''' <summary>
    ''' Json字符串转为对象
    ''' </summary>
    ''' <param name="Type">对象对应的类型</param>
    ''' <param name="DataStream">Json数据流</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function Json2Object(ByVal Type As Type, ByVal DataStream As Stream) As Object
        Dim Converter As New DataContractJsonSerializer(Type)
        Return Converter.ReadObject(DataStream)
    End Function

    ''' <summary>
    ''' Json字符串转为对象
    ''' </summary>
    ''' <param name="Type">对象对应的类型</param>
    ''' <param name="Json">Json字符串</param>
    ''' <param name="StreamEncoding">流编码</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function Json2Object(ByVal Type As Type, ByVal Json As String, ByVal StreamEncoding As Encoding) As Object
        Using DataStream As New MemoryStream(StreamEncoding.GetBytes(Json))
            Return Json2Object(Type, DataStream)
        End Using
    End Function

    ''' <summary>
    ''' Json字符串转为对象
    ''' </summary>
    ''' <param name="Type">对象对应的类型</param>
    ''' <param name="Json">Json字符串</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function Json2Object(ByVal Type As Type, ByVal Json As String) As Object
        Return Json2Object(Type, Json, Encoding.UTF8)
    End Function

End Class
