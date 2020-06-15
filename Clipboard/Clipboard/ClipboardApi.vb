Imports Clipboard.LangClass
Imports System.Text
Imports System.Security.Cryptography
Imports System.Runtime.InteropServices

Public Class ClipboardApi

    Public Const USER32 As String = "User32.dll"
    Public Const SHCORE As String = "SHCore.dll"
    Public Const KERNEL32 As String = "Kernel32.dll"
    Public Const UXTHEME As String = "Uxtheme.dll"
    Public Const THEME As String = "theme.dll"

    ''' <summary>
    ''' 数据对象
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable>
    Public Class DataTag
        Public type As String
        Public time As String
        Public text As String
        Public key As String
        Public lock As Boolean
    End Class

    ''' <summary>
    ''' 设置结构
    ''' </summary>
    ''' <remarks></remarks>
    Public Structure Settings
        Public Speech As Boolean()      '0-6
        Public IsTop As Boolean         '7
        Public Opacity As Integer       '8
        Public Language As Language     '9
    End Structure

#Region "Enum"

    Public Enum HRESULT
        S_OK = &H0
        S_FALSE = &H1
        E_INVALIDARG = CInt(&H80070057)
        E_OUTOFMEMORY = CInt(&H8007000E)
        E_NOINTERFACE = CInt(&H80004002)
        E_FAIL = CInt(&H80004005)
        E_ELEMENTNOTFOUND = CInt(&H80070490)
        TYPE_E_ELEMENTNOTFOUND = CInt(&H8002802B)
        NO_OBJECT = CInt(&H800401E5)
        WIN32ERROR_CANCELLED = 1223
        ERROR_CANCELLED = CInt(&H800704C7)
        RESOURCEINUSE = CInt(&H800700AA)
        ACCESSDENIED = CInt(&H80030005)
    End Enum

    Public Enum PreferredAppMode
        [Default]
        AllowDark
        ForceDark
        ForceLight
        Max
    End Enum

    ''' <summary>
    ''' 命令名称
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum CommandName
        Copy
        Ignore
        Edit
        Opacity
        Lock
        AddFile

        ClearAll
        OpenSetting
        SettingsJS
        LanguageText
        AddPre
        LanguageType

        SettingsNET
        EditText
    End Enum

    ''' <summary>
    ''' 设置名称
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum SettingsName

        '语音设置--------------------------------------------------------------------,0-6
        Speech  '-------------------------------------------------------------------,语音总设置
        SpeechTop
        SpeechEdit
        SpeechCopy
        SpeechIgnore
        SpeechClearall
        SpeechSetting

        '其它设置-------------------------------------------------------------------,7-8
        IsTop
        Opacity

        '语言设置-------------------------------------------------------------------,9
        Language

    End Enum

    ''' <summary>
    ''' Vant通知组件的通知类型
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum NotifyType
        primary
        success
        danger
        warning
    End Enum

    Public Enum DPI_AWARENESS_CONTEXT

        DPI_AWARENESS_CONTEXT_UNAWARE = 16

        DPI_AWARENESS_CONTEXT_SYSTEM_AWARE = 17

        DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE = 18

        DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2 = 34

    End Enum

    Public Enum PROCESS_DPI_AWARENESS

        Process_DPI_Unaware = 0

        Process_System_DPI_Aware = 1

        Process_Per_Monitor_DPI_Aware = 2

    End Enum

    Public Enum WindowMsg

        WM_CREATE = &H1

        WM_DESTROY = &H2

        WM_CLOSE = &H10

        WM_THEMECHANGED = &H31A

        WM_SETTINGCHANGE = &H1A
    End Enum
    Public Enum ClipboardMsg

        WM_CLEAR = &H303

        WM_COPY = &H301

        WM_CUT = &H300

        WM_PASTE = &H302

        '-------------------------------------------------------------------------------

        WM_ASKCBFORMATNAME = &H30C

        WM_CHANGECBCHAIN = &H30D

        WM_CLIPBOARDUPDATE = &H31D

        WM_DESTROYCLIPBOARD = &H307

        WM_DRAWCLIPBOARD = &H308

        WM_HSCROLLCLIPBOARD = &H30E

        WM_PAINTCLIPBOARD = &H309

        WM_RENDERALLFORMATS = &H306

        WM_RENDERFORMAT = &H305

        WM_SIZECLIPBOARD = &H30B

        WM_VSCROLLCLIPBOARD = &H30A

    End Enum

#End Region

#Region "Win32Api"

    <DllImport(USER32, SetLastError:=True)>
    Public Shared Function AddClipboardFormatListener(ByVal hWnd As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport(USER32, SetLastError:=True)>
    Public Shared Function RemoveClipboardFormatListener(ByVal hWnd As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport(USER32, SetLastError:=True)>
    Public Shared Function SendMessage(ByVal hWnd As IntPtr, ByVal Msg As Integer, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As Integer
    End Function

    <DllImport(USER32, SetLastError:=True)>
    Public Shared Function SetProcessDpiAwarenessContext(ByVal dpiFlag As Integer) As Boolean
    End Function

    <DllImport(USER32, SetLastError:=True)>
    Public Shared Function SetProcessDPIAware() As Boolean
    End Function

    <DllImport(SHCORE, SetLastError:=True)>
    Public Shared Function SetProcessDpiAwareness(ByVal awareness As PROCESS_DPI_AWARENESS) As Boolean
    End Function

    <DllImport(KERNEL32, SetLastError:=True)>
    Public Shared Function GetPrivateProfileString(ByVal lpAppName As String, ByVal lpKeyName As String, ByVal lpDefault As String, ByVal lpReturnedString As StringBuilder, ByVal nSize As Integer, ByVal lpFileName As String) As Integer
    End Function

    <DllImport(KERNEL32, SetLastError:=True)>
    Public Shared Function WritePrivateProfileString(ByVal lpApplicationName As String, ByVal lpKeyName As String, ByVal lpString As String, ByVal lpFileName As String) As Integer
    End Function

    <DllImport(UXTHEME, SetLastError:=True, ExactSpelling:=True, CharSet:=CharSet.Unicode)>
    Public Shared Function SetWindowTheme(ByVal hWnd As IntPtr, ByVal pszSubAppName As String, ByVal pszSubIdList As String) As Integer
    End Function

    ''' <summary>
    ''' 设置操作的窗体句柄
    ''' </summary>
    ''' <param name="hWnd">窗体句柄</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DllImport(THEME, SetLastError:=True)>
    Public Shared Function SetWindowHandle(ByVal hWnd As IntPtr) As HRESULT
    End Function

    ''' <summary>
    ''' 设置窗体主题（暂只有<see cref="PreferredAppMode.[Default]"/>亮白主题和其它暗黑主题）
    ''' </summary>
    ''' <param name="Mode">主题选择</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DllImport(THEME, SetLastError:=True)>
    Public Shared Function SetThemeMode(ByVal Mode As PreferredAppMode) As HRESULT
    End Function

    ''' <summary>
    ''' 按系统主题更新
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DllImport(THEME, SetLastError:=True)>
    Public Shared Function UpdateThemeMode() As HRESULT
    End Function

    ''' <summary>
    ''' 按<see cref="WindowMsg.WM_SETTINGCHANGE"/>接收到的信息选择更新主题（与系统主题一致）
    ''' </summary>
    ''' <param name="lParam">窗口信息</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <DllImport(THEME, SetLastError:=True)>
    Public Shared Function UpdateThemeModeWithMsg(ByVal lParam As IntPtr) As HRESULT
    End Function

#End Region

#Region "Functions"

    ''' <summary>
    ''' 设置DPI自动感知
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub SetDpiAwareness()
        If Environment.OSVersion.Version >= New Version(6, 3, 0) Then
            ' win 8.1 added support for per monitor dpi
            If Environment.OSVersion.Version >= New Version(10, 0, 15063) Then
                ' win 10 creators update added support for per monitor v2
                SetProcessDpiAwarenessContext(DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2)
            Else
                SetProcessDpiAwareness(PROCESS_DPI_AWARENESS.Process_Per_Monitor_DPI_Aware)
            End If
        Else
            SetProcessDPIAware()
        End If
    End Sub

    Public Shared Function MD5Encode(ByVal [String] As String) As String
        Using md5Hash As MD5 = MD5.Create()
            Return GetMd5Hash(md5Hash, [String])
        End Using
    End Function

    Private Shared Function GetMd5Hash(ByVal Md5Hash As MD5, ByVal Input As String) As String
        Dim data As Byte() = Md5Hash.ComputeHash(Encoding.UTF8.GetBytes(Input))
        Dim sBuilder As New StringBuilder()
        For i = 0 To data.Length - 1
            sBuilder.Append(data(i).ToString("x2"))
        Next i
        Return sBuilder.ToString()
    End Function

#End Region

End Class
