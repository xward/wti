<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FrmMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        TmrUI = New Timer(components)
        LblDegiroState = New Label()
        Button1 = New Button()
        Button2 = New Button()
        Label1 = New Label()
        SuspendLayout()
        ' 
        ' TmrUI
        ' 
        TmrUI.Enabled = True
        ' 
        ' LblDegiroState
        ' 
        LblDegiroState.AutoSize = True
        LblDegiroState.Font = New Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        LblDegiroState.Location = New Point(12, 9)
        LblDegiroState.Name = "LblDegiroState"
        LblDegiroState.Size = New Size(135, 25)
        LblDegiroState.TabIndex = 0
        LblDegiroState.Text = "LblDegiroState"
        ' 
        ' Button1
        ' 
        Button1.Location = New Point(286, 235)
        Button1.Name = "Button1"
        Button1.Size = New Size(67, 42)
        Button1.TabIndex = 1
        Button1.Text = "Button1"
        Button1.UseVisualStyleBackColor = True
        ' 
        ' Button2
        ' 
        Button2.Location = New Point(286, 283)
        Button2.Name = "Button2"
        Button2.Size = New Size(67, 41)
        Button2.TabIndex = 2
        Button2.Text = "Button2"
        Button2.UseVisualStyleBackColor = True
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(36, 399)
        Label1.Name = "Label1"
        Label1.Size = New Size(41, 15)
        Label1.TabIndex = 3
        Label1.Text = "Label1"
        ' 
        ' FrmMain
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.LightSlateGray
        ClientSize = New Size(365, 527)
        Controls.Add(Label1)
        Controls.Add(Button2)
        Controls.Add(Button1)
        Controls.Add(LblDegiroState)
        Name = "FrmMain"
        Text = "WTI"
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents TmrUI As Timer
    Friend WithEvents LblDegiroState As Label
    Friend WithEvents Button1 As Button
    Friend WithEvents Button2 As Button
    Friend WithEvents Label1 As Label

End Class
