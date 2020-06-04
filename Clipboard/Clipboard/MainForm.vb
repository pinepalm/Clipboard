Imports Clipboard.ClipboardApi

Public Class MainForm

#Region "WndProc"

    Protected Overrides Sub WndProc(ByRef m As Message)
        Select Case m.Msg
            '构造窗体
            Case WindowMsg.WM_CREATE
                SetDpiAwareness()
                AddClipboardFormatListener(Handle)
                SetWindowHandle(Handle)
                UpdateThemeMode()

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
                If MonitorableMenuItem.Checked Then
                    AddRecord()
                End If

            Case WindowMsg.WM_SETTINGCHANGE
                UpdateThemeModeWithMsg(m.LParam)

        End Select
        MyBase.WndProc(m)
    End Sub

#End Region

#Region "MainForm"

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Initialize()
    End Sub

    Private Sub MainForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If e.CloseReason = CloseReason.UserClosing AndAlso Not CloseFlag Then
            e.Cancel = True
            Visible = False
        End If
    End Sub

#End Region

End Class
