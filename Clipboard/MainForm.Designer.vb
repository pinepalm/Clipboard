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
        Me.IconContextMenuStrip = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.IsTopToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.WebViewPanel = New System.Windows.Forms.Panel()
        Me.IconContextMenuStrip.SuspendLayout()
        Me.SuspendLayout()
        '
        'ClipboardNotifyIcon
        '
        Me.ClipboardNotifyIcon.ContextMenuStrip = Me.IconContextMenuStrip
        Me.ClipboardNotifyIcon.Text = "Clipboard"
        Me.ClipboardNotifyIcon.Visible = True
        '
        'IconContextMenuStrip
        '
        Me.IconContextMenuStrip.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.IconContextMenuStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.IsTopToolStripMenuItem, Me.ToolStripSeparator1, Me.ExitToolStripMenuItem})
        Me.IconContextMenuStrip.Name = "IconContextMenuStrip"
        Me.IconContextMenuStrip.Size = New System.Drawing.Size(132, 62)
        '
        'IsTopToolStripMenuItem
        '
        Me.IsTopToolStripMenuItem.CheckOnClick = True
        Me.IsTopToolStripMenuItem.Name = "IsTopToolStripMenuItem"
        Me.IsTopToolStripMenuItem.Size = New System.Drawing.Size(131, 26)
        Me.IsTopToolStripMenuItem.Text = "置顶(&T)"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(128, 6)
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Image = CType(resources.GetObject("ExitToolStripMenuItem.Image"), System.Drawing.Image)
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(131, 26)
        Me.ExitToolStripMenuItem.Text = "退出(&E)"
        '
        'WebViewPanel
        '
        Me.WebViewPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.WebViewPanel.Location = New System.Drawing.Point(0, 0)
        Me.WebViewPanel.Name = "WebViewPanel"
        Me.WebViewPanel.Size = New System.Drawing.Size(482, 762)
        Me.WebViewPanel.TabIndex = 1
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(482, 762)
        Me.Controls.Add(Me.WebViewPanel)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MinimumSize = New System.Drawing.Size(500, 809)
        Me.Name = "MainForm"
        Me.ShowInTaskbar = False
        Me.Text = "Clipboard"
        Me.IconContextMenuStrip.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents ClipboardNotifyIcon As NotifyIcon
    Friend WithEvents IconContextMenuStrip As ContextMenuStrip
    Friend WithEvents ExitToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents IsTopToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
    Friend WithEvents WebViewPanel As Panel
End Class
