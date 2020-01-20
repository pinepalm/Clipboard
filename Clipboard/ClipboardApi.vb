﻿Imports Clipboard.LangClass
Imports System.Runtime.InteropServices
Imports System.Text

Public Class ClipboardApi

    Public Const USER32 As String = "User32.dll"

    Public Const SHCORE As String = "SHCore.dll"

    Public Const KERNEL32 As String = "Kernel32.dll"

    ''' <summary>
    ''' 数据结构
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable>
    Public Structure DataTag
        Public type As Integer
        Public time As String
        Public text As String
        Public id As String
        Public lock As Boolean
    End Structure

    ''' <summary>
    ''' 设置结构
    ''' </summary>
    ''' <remarks></remarks>
    Public Structure Settings
        '0-6
        Public Speech As Boolean()
        '7
        Public IsTop As Boolean
        '8
        Public Opacity As Integer
        '9
        Public Language As Language
    End Structure

#Region "Enum"

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

        ClearAll
        OpenSetting
        SettingsJS
        LanguageText
        AddPre

        SettingsNET
        EditText
    End Enum

    ''' <summary>
    ''' 设置名称
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum SettingsName

        '语音设置-------------------------------------------------------------------,0-6
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

    <DllImport(USER32)>
    Public Shared Function ChangeClipboardChain(ByVal hWndRemove As IntPtr, ByVal hWndNewNext As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport(USER32)>
    Public Shared Function CloseClipboard() As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport(USER32)>
    Public Shared Function EmptyClipboard() As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport(USER32)>
    Public Shared Function OpenClipboard(ByVal hWndNewOwner As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport(USER32, SetLastError:=True)>
    Public Shared Function RemoveClipboardFormatListener(ByVal hWnd As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport(USER32)>
    Public Shared Function SetClipboardViewer(ByVal hWndNewViewer As IntPtr) As IntPtr
    End Function

    <DllImport(USER32)>
    Public Shared Function GetClipboardViewer() As IntPtr
    End Function

    <DllImport(USER32)>
    Public Shared Function GetOpenClipboardWindow() As IntPtr
    End Function

    <DllImport(USER32, SetLastError:=True)>
    Public Shared Function SendMessage(ByVal hWnd As IntPtr, ByVal Msg As Integer, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As Integer
    End Function

    <DllImport(USER32, SetLastError:=True)>
    Public Shared Function SetProcessDpiAwarenessContext(ByVal dpiFlag As Integer) As Boolean
    End Function
    <DllImport(USER32)>
    Public Shared Function SetProcessDPIAware() As Boolean
    End Function

    <DllImport(SHCORE, SetLastError:=True)>
    Public Shared Function SetProcessDpiAwareness(ByVal awareness As PROCESS_DPI_AWARENESS) As Boolean
    End Function

    <DllImport(KERNEL32)>
    Public Shared Function GetPrivateProfileString(ByVal lpAppName As String, ByVal lpKeyName As String, ByVal lpDefault As String, ByVal lpReturnedString As StringBuilder, ByVal nSize As Integer, ByVal lpFileName As String) As Integer
    End Function

    <DllImport(KERNEL32)>
    Public Shared Function WritePrivateProfileString(ByVal lpApplicationName As String, ByVal lpKeyName As String, ByVal lpString As String, ByVal lpFileName As String) As Integer
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

#End Region

End Class