<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class MainForm
    Inherits System.Windows.Forms.Form

    'Form 重写 Dispose，以清理组件列表。
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows 窗体设计器所必需的
    Private components As System.ComponentModel.IContainer

    '注意: 以下过程是 Windows 窗体设计器所必需的
    '可以使用 Windows 窗体设计器修改它。  
    '不要使用代码编辑器修改它。
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        Me.ClipboardNotifyIcon = New System.Windows.Forms.NotifyIcon(Me.components)
        Me.WebViewPanel = New System.Windows.Forms.Panel()
        Me.AddFileDialog = New System.Windows.Forms.OpenFileDialog()
        Me.SuspendLayout()
        '
        'ClipboardNotifyIcon
        '
        Me.ClipboardNotifyIcon.Text = "Clipboard"
        Me.ClipboardNotifyIcon.Visible = True
        '
        'WebViewPanel
        '
        Me.WebViewPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.WebViewPanel.Location = New System.Drawing.Point(0, 0)
        Me.WebViewPanel.Name = "WebViewPanel"
        Me.WebViewPanel.Size = New System.Drawing.Size(567, 1055)
        Me.WebViewPanel.TabIndex = 1
        '
        'AddFileDialog
        '
        Me.AddFileDialog.Multiselect = True
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(567, 1055)
        Me.Controls.Add(Me.WebViewPanel)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "MainForm"
        Me.Text = "Clipboard"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents ClipboardNotifyIcon As NotifyIcon
    Friend WithEvents WebViewPanel As Panel
    Friend WithEvents AddFileDialog As OpenFileDialog
End Class
