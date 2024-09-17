<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AnalyzeViewer
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        TopPanel = New Panel()
        Button2 = New Button()
        Button1 = New Button()
        PictureBoxAnalizeViewer = New PictureBox()
        TopPanel.SuspendLayout()
        CType(PictureBoxAnalizeViewer, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' TopPanel
        ' 
        TopPanel.Controls.Add(Button2)
        TopPanel.Controls.Add(Button1)
        TopPanel.Dock = DockStyle.Top
        TopPanel.Location = New Point(0, 0)
        TopPanel.Name = "TopPanel"
        TopPanel.Size = New Size(1865, 85)
        TopPanel.TabIndex = 0
        ' 
        ' Button2
        ' 
        Button2.Location = New Point(965, 12)
        Button2.Name = "Button2"
        Button2.Size = New Size(467, 47)
        Button2.TabIndex = 1
        Button2.Text = "generate % diff from max COUNT perf frequency 1/mois 1/5ans 1/10ans ..."
        Button2.UseVisualStyleBackColor = True
        ' 
        ' Button1
        ' 
        Button1.Location = New Point(537, 12)
        Button1.Name = "Button1"
        Button1.Size = New Size(363, 47)
        Button1.TabIndex = 0
        Button1.Text = "generate day/week/month  = f (% from max ever)"
        Button1.UseVisualStyleBackColor = True
        ' 
        ' PictureBoxAnalizeViewer
        ' 
        PictureBoxAnalizeViewer.Dock = DockStyle.Fill
        PictureBoxAnalizeViewer.Location = New Point(0, 85)
        PictureBoxAnalizeViewer.Name = "PictureBoxAnalizeViewer"
        PictureBoxAnalizeViewer.Size = New Size(1865, 667)
        PictureBoxAnalizeViewer.TabIndex = 1
        PictureBoxAnalizeViewer.TabStop = False
        ' 
        ' AnalyzeViewer
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1865, 752)
        Controls.Add(PictureBoxAnalizeViewer)
        Controls.Add(TopPanel)
        Name = "AnalyzeViewer"
        StartPosition = FormStartPosition.CenterScreen
        Text = "WTI - AnalizeViewer"
        TopPanel.ResumeLayout(False)
        CType(PictureBoxAnalizeViewer, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents TopPanel As Panel
    Friend WithEvents PictureBoxAnalizeViewer As PictureBox
    Friend WithEvents Button2 As Button
    Friend WithEvents Button1 As Button
End Class
