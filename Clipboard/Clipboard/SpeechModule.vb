Imports Clipboard.ClipboardApi
Imports System.Speech.Synthesis

Module SpeechModule

    Friend Speecher As SpeechSynthesizer
    Friend SpeechMsg As PromptBuilder

    Friend Sub ActionSpeech(ByVal Name As SettingsName, ByVal Msg As String)
        If Settings.Speech(0) AndAlso Settings.Speech(Name) Then
            Speecher.SpeakAsyncCancelAll()
            SpeechMsg.ClearContent()
            SpeechMsg.AppendText(Msg)
            Speecher.SpeakAsync(SpeechMsg)
        End If
    End Sub

End Module
