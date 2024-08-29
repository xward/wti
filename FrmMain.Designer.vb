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
        LblDegiroState.Location = New Point(49, 79)
        LblDegiroState.Name = "LblDegiroState"
        LblDegiroState.Size = New Size(135, 25)
        LblDegiroState.TabIndex = 0
        LblDegiroState.Text = "LblDegiroState"
        ' 
        ' Button1
        ' 
        Button1.Location = New Point(286, 235)
        Button1.Name = "Button1"
        Button1.Size = New Size(36, 25)
        Button1.TabIndex = 1
        Button1.Text = "Button1"
        Button1.UseVisualStyleBackColor = True
        ' 
        ' FrmMain
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.LightSlateGray
        ClientSize = New Size(365, 527)
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

End Class
