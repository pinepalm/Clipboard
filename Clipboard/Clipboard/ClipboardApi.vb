Imports Clipboard.LangClass
Imports System.Text
Imports System.Security.Cryptography
Imports System.Runtime.InteropServices

Public Class ClipboardApi

    Public Const USER32 As String = "User32.dll"
    Public Const SHCORE As String = "SHCore.dll"
    Public Const KERNEL32 As String = "Kernel32.dll"
    Public Const SHELL32 As String = "Shell32.dll"
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

    <StructLayout(LayoutKind.Sequential)>
    Public Structure NotifyIconData
        Public cbSize As Integer
        Public hWnd As IntPtr
        Public uID As UInteger
        Public uFlags As NotifyIconFlag
        Public uCallbackMessage As UInteger
        Public hIcon As IntPtr
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=128)>
        Public szTip As String
        Public dwState As Integer
        Public dwStateMask As Integer
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=256)>
        Public szInfo As String
        Public uTimeoutOrVersion As UInteger
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=64)>
        Public szInfoTitle As String
        Public dwInfoFlags As NotifyIconInfoFlag
        Public guidItem As Guid
        Public hBalloonIcon As IntPtr
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

    Public Enum NotifyIconInfoFlag
        NIIF_NONE = &H0
        NIIF_INFO = &H1
        NIIF_WARNING = &H2
        NIIF_ERROR = &H3
        NIIF_USER = &H4
        NIIF_NOSOUND = &H10
        NIIF_LARGE_ICON = &H20
        NIIF_RESPECT_QUIET_TIME = &H80
        NIIF_ICON_MASK = &HF
    End Enum

    Public Enum NotifyIconFlag As UInteger
        NIF_MESSAGE = &H1
        NIF_ICON = &H2
        NIF_TIP = &H4
        NIF_STATE = &H8
        NIF_INFO = &H10
        NIF_GUID = &H20
        NIF_REALTIME = &H40
        NIF_SHOWTIP = &H80
    End Enum

    Public Enum NotifyIconMessage
        NIM_ADD = &H0
        NIM_MODIFY = &H1
        NIM_DELETE = &H2
        NIM_SETFOCUS = &H3
        NIM_SETVERSION = &H4
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

    Public Enum StdClipboardFormats
        ''' <summary>
        ''' A handle To a bitmap (HBITMAP).
        ''' </summary>
        CF_BITMAP = 2
        ''' <summary>
        ''' A memory Object containing a BITMAPINFO Structure followed by the bitmap bits.
        ''' </summary>
        CF_DIB = 8
        ''' <summary>
        ''' A memory Object containing a BITMAPV5HEADER Structure followed by the bitmap color space information And the bitmap bits.
        ''' </summary>
        CF_DIBV5 = 17
        ''' <summary>
        ''' Software Arts' Data Interchange Format.
        ''' </summary>
        CF_DIF = 5
        ''' <summary>
        ''' Bitmap display format associated With a Private format. The hMem parameter must be a handle To data that can be displayed In bitmap format In lieu Of the privately formatted data.
        ''' </summary>
        CF_DSPBITMAP = &H82
        ''' <summary>
        ''' Enhanced metafile display format associated With a Private format. The hMem parameter must be a handle To data that can be displayed In enhanced metafile format In lieu Of the privately formatted data.
        ''' </summary>
        CF_DSPENHMETAFILE = &H8E
        ''' <summary>
        ''' Metafile-picture display format associated With a Private format. The hMem parameter must be a handle To data that can be displayed In metafile-picture format In lieu Of the privately formatted data.
        ''' </summary>
        CF_DSPMETAFILEPICT = &H83
        ''' <summary>
        ''' Text display format associated With a Private format. The hMem parameter must be a handle To data that can be displayed In text format In lieu Of the privately formatted data.
        ''' </summary>
        CF_DSPTEXT = &H81
        ''' <summary>
        ''' A handle To an enhanced metafile (HENHMETAFILE).
        ''' </summary>
        CF_ENHMETAFILE = 14
        ''' <summary>
        ''' Start Of a range Of Integer values For application-defined GDI Object clipboard formats. The End Of the range Is CF_GDIOBJLAST.Handles associated with clipboard formats in this range are Not automatically deleted using the GlobalFree function when the clipboard Is emptied. Also, when using values in this range, the hMem parameter Is Not a handle to a GDI object, but Is a handle allocated by the GlobalAlloc function with the GMEM_MOVEABLE flag.
        ''' </summary>
        CF_GDIOBJFIRST = &H300
        ''' <summary>
        ''' See CF_GDIOBJFIRST.
        ''' </summary>
        CF_GDIOBJLAST = &H3FF
        ''' <summary>
        ''' A handle To type HDROP that identifies a list Of files. An application can retrieve information about the files by passing the handle To the DragQueryFile Function.
        ''' </summary>
        CF_HDROP = 15
        ''' <summary>
        ''' The data Is a handle To the locale identifier associated With text In the clipboard. When you close the clipboard, If it contains CF_TEXT data but no CF_LOCALE data, the system automatically sets the CF_LOCALE format To the current input language. You can use the CF_LOCALE format To associate a different locale With the clipboard text. 
        ''' An application that pastes text from the clipboard can retrieve this format To determine which character Set was used To generate the text.
        ''' Note that the clipboard does Not support plain text In multiple character sets. To achieve this, use a formatted text data type such As RTF instead.
        ''' The system uses the code page associated With CF_LOCALE To implicitly convert from CF_TEXT To CF_UNICODETEXT. Therefore, the correct code page table Is used For the conversion.
        ''' </summary>
        CF_LOCALE = 16
        ''' <summary>
        ''' Handle To a metafile picture format As defined by the METAFILEPICT Structure. When passing a CF_METAFILEPICT handle by means Of DDE, the application responsible For deleting hMem should also free the metafile referred To by the CF_METAFILEPICT handle.
        ''' </summary>
        CF_METAFILEPICT = 3
        ''' <summary>
        ''' Text format containing characters In the OEM character Set. Each line ends With a carriage Return/linefeed (CR-LF) combination. A null character signals the End Of the data.
        ''' </summary>
        CF_OEMTEXT = 7
        ''' <summary>
        ''' Owner-display format. The clipboard owner must display And update the clipboard viewer window, And receive the WM_ASKCBFORMATNAME, WM_HSCROLLCLIPBOARD, WM_PAINTCLIPBOARD, WM_SIZECLIPBOARD, And WM_VSCROLLCLIPBOARD messages. The hMem parameter must be NULL.
        ''' </summary>
        CF_OWNERDISPLAY = &H80
        ''' <summary>
        ''' Handle To a color palette. Whenever an application places data In the clipboard that depends On Or assumes a color palette, it should place the palette On the clipboard As well.
        ''' If the clipboard contains data In the CF_PALETTE (logical color palette) format, the application should use the SelectPalette And RealizePalette functions To realize (compare) any other data In the clipboard against that logical palette.
        ''' When displaying clipboard data, the clipboard always uses as its current palette any object on the clipboard that Is in the CF_PALETTE format.
        ''' </summary>
        CF_PALETTE = 9
        ''' <summary>
        ''' Data For the pen extensions To the Microsoft Windows For Pen Computing.
        ''' </summary>
        CF_PENDATA = 10
        ''' <summary>
        ''' Start Of a range Of Integer values For Private clipboard formats. The range ends With CF_PRIVATELAST. Handles associated With Private clipboard formats are Not freed automatically; the clipboard owner must free such Handles, typically In response To the WM_DESTROYCLIPBOARD message.
        ''' </summary>
        CF_PRIVATEFIRST = &H200
        ''' <summary>
        ''' See CF_PRIVATEFIRST.
        ''' </summary>
        CF_PRIVATELAST = &H2FF
        ''' <summary>
        ''' Represents audio data more complex than can be represented In a CF_WAVE standard wave format.
        ''' </summary>
        CF_RIFF = 11
        ''' <summary>
        ''' Microsoft Symbolic Link (SYLK) format.
        ''' </summary>
        CF_SYLK = 4
        ''' <summary>
        ''' Text format. Each line ends With a carriage Return/linefeed (CR-LF) combination. A null character signals the End Of the data. Use this format For ANSI text.
        ''' </summary>
        CF_TEXT = 1
        ''' <summary>
        ''' Tagged-image file format.
        ''' </summary>
        CF_TIFF = 6
        ''' <summary>
        ''' Unicode text format. Each line ends With a carriage Return/linefeed (CR-LF) combination. A null character signals the End Of the data.
        ''' </summary>
        CF_UNICODETEXT = 13
        ''' <summary>
        ''' Represents audio data In one Of the standard wave formats, such As 11 kHz Or 22 kHz PCM.
        ''' </summary>
        CF_WAVE = 12
    End Enum

#End Region

#Region "Win32Api"
    <DllImport(USER32, SetLastError:=True)>
    Public Shared Function AddClipboardFormatListener(ByVal hWnd As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport(USER32, SetLastError:=True)>
    Public Shared Function ChangeClipboardChain(ByVal hWndRemove As IntPtr, ByVal hWndNewNext As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport(USER32, SetLastError:=True)>
    Public Shared Function CloseClipboard() As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport(USER32, SetLastError:=True)>
    Public Shared Function EmptyClipboard() As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport(USER32, SetLastError:=True)>
    Public Shared Function OpenClipboard(ByVal hWndNewOwner As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport(USER32, SetLastError:=True)>
    Public Shared Function RemoveClipboardFormatListener(ByVal hWnd As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport(USER32, SetLastError:=True)>
    Public Shared Function SetClipboardViewer(ByVal hWndNewViewer As IntPtr) As IntPtr
    End Function

    <DllImport(USER32, SetLastError:=True)>
    Public Shared Function GetClipboardViewer() As IntPtr
    End Function

    <DllImport(USER32, SetLastError:=True)>
    Public Shared Function GetOpenClipboardWindow() As IntPtr
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

    <DllImport(SHELL32, SetLastError:=True)>
    Public Shared Function Shell_NotifyIcon(ByVal dwMessage As NotifyIconMessage, ByRef lpData As NotifyIconData) As Boolean
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
