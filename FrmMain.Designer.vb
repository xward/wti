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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmMain))
        TmrUI = New Timer(components)
        statusLed = New ToolStripStatusLabel()
        statusLabel = New ToolStripStatusLabel()
        degiroLabel = New ToolStripStatusLabel()
        esterLabel = New ToolStripStatusLabel()
        StatusStrip = New StatusStrip()
        runType = New ToolStripStatusLabel()
        PictureLedRedOff = New PictureBox()
        PictureLedRedOn = New PictureBox()
        PictureLedGreenOff = New PictureBox()
        PictureLedGreenOn = New PictureBox()
        DataGridViewAssetPrices = New DataGridView()
        ticker = New DataGridViewTextBoxColumn()
        price = New DataGridViewTextBoxColumn()
        todayChange = New DataGridViewTextBoxColumn()
        BottomPanel = New Panel()
        Button1 = New Button()
        BtnTest = New Button()
        TopPanel = New Panel()
        ListBox1 = New ListBox()
        Label1 = New Label()
        TmerKeyIput = New Timer(components)
        TmerAutoStart = New Timer(components)
        StatusStrip.SuspendLayout()
        CType(PictureLedRedOff, ComponentModel.ISupportInitialize).BeginInit()
        CType(PictureLedRedOn, ComponentModel.ISupportInitialize).BeginInit()
        CType(PictureLedGreenOff, ComponentModel.ISupportInitialize).BeginInit()
        CType(PictureLedGreenOn, ComponentModel.ISupportInitialize).BeginInit()
        CType(DataGridViewAssetPrices, ComponentModel.ISupportInitialize).BeginInit()
        BottomPanel.SuspendLayout()
        TopPanel.SuspendLayout()
        SuspendLayout()
        ' 
        ' TmrUI
        ' 
        TmrUI.Enabled = True
        TmrUI.Interval = 200
        ' 
        ' statusLed
        ' 
        statusLed.AutoSize = False
        statusLed.BackgroundImage = CType(resources.GetObject("statusLed.BackgroundImage"), Image)
        statusLed.Margin = New Padding(0)
        statusLed.Name = "statusLed"
        statusLed.Size = New Size(32, 32)
        statusLed.Text = "        "
        ' 
        ' statusLabel
        ' 
        statusLabel.BackColor = SystemColors.ButtonFace
        statusLabel.Font = New Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        statusLabel.Name = "statusLabel"
        statusLabel.Size = New Size(70, 27)
        statusLabel.Text = "OFFLINE"
        ' 
        ' degiroLabel
        ' 
        degiroLabel.BackColor = SystemColors.ButtonFace
        degiroLabel.Font = New Font("Sitka Heading", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        degiroLabel.Name = "degiroLabel"
        degiroLabel.Size = New Size(55, 27)
        degiroLabel.Text = "degiro "
        ' 
        ' esterLabel
        ' 
        esterLabel.BackColor = SystemColors.ButtonFace
        esterLabel.Font = New Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        esterLabel.ImageScaling = ToolStripItemImageScaling.None
        esterLabel.Name = "esterLabel"
        esterLabel.Size = New Size(76, 27)
        esterLabel.Text = "Ester 3.66%"
        ' 
        ' StatusStrip
        ' 
        StatusStrip.Items.AddRange(New ToolStripItem() {statusLed, statusLabel, runType, degiroLabel, esterLabel})
        StatusStrip.Location = New Point(0, 739)
        StatusStrip.Name = "StatusStrip"
        StatusStrip.Size = New Size(649, 32)
        StatusStrip.TabIndex = 4
        ' 
        ' runType
        ' 
        runType.BackColor = SystemColors.ButtonFace
        runType.Font = New Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        runType.Name = "runType"
        runType.Size = New Size(60, 27)
        runType.Text = "DEBUG"
        ' 
        ' PictureLedRedOff
        ' 
        PictureLedRedOff.Image = CType(resources.GetObject("PictureLedRedOff.Image"), Image)
        PictureLedRedOff.Location = New Point(921, 583)
        PictureLedRedOff.Name = "PictureLedRedOff"
        PictureLedRedOff.Size = New Size(32, 32)
        PictureLedRedOff.SizeMode = PictureBoxSizeMode.AutoSize
        PictureLedRedOff.TabIndex = 5
        PictureLedRedOff.TabStop = False
        PictureLedRedOff.Visible = False
        ' 
        ' PictureLedRedOn
        ' 
        PictureLedRedOn.Image = CType(resources.GetObject("PictureLedRedOn.Image"), Image)
        PictureLedRedOn.Location = New Point(959, 583)
        PictureLedRedOn.Name = "PictureLedRedOn"
        PictureLedRedOn.Size = New Size(32, 32)
        PictureLedRedOn.SizeMode = PictureBoxSizeMode.AutoSize
        PictureLedRedOn.TabIndex = 6
        PictureLedRedOn.TabStop = False
        PictureLedRedOn.Visible = False
        ' 
        ' PictureLedGreenOff
        ' 
        PictureLedGreenOff.Image = CType(resources.GetObject("PictureLedGreenOff.Image"), Image)
        PictureLedGreenOff.Location = New Point(921, 621)
        PictureLedGreenOff.Name = "PictureLedGreenOff"
        PictureLedGreenOff.Size = New Size(32, 32)
        PictureLedGreenOff.SizeMode = PictureBoxSizeMode.AutoSize
        PictureLedGreenOff.TabIndex = 7
        PictureLedGreenOff.TabStop = False
        PictureLedGreenOff.Visible = False
        ' 
        ' PictureLedGreenOn
        ' 
        PictureLedGreenOn.Image = CType(resources.GetObject("PictureLedGreenOn.Image"), Image)
        PictureLedGreenOn.Location = New Point(959, 621)
        PictureLedGreenOn.Name = "PictureLedGreenOn"
        PictureLedGreenOn.Size = New Size(32, 32)
        PictureLedGreenOn.SizeMode = PictureBoxSizeMode.AutoSize
        PictureLedGreenOn.TabIndex = 8
        PictureLedGreenOn.TabStop = False
        PictureLedGreenOn.Visible = False
        ' 
        ' DataGridViewAssetPrices
        ' 
        DataGridViewAssetPrices.AllowUserToAddRows = False
        DataGridViewAssetPrices.AllowUserToDeleteRows = False
        DataGridViewAssetPrices.AllowUserToResizeColumns = False
        DataGridViewAssetPrices.AllowUserToResizeRows = False
        DataGridViewAssetPrices.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridViewAssetPrices.Columns.AddRange(New DataGridViewColumn() {ticker, price, todayChange})
        DataGridViewAssetPrices.Location = New Point(814, 411)
        DataGridViewAssetPrices.Name = "DataGridViewAssetPrices"
        DataGridViewAssetPrices.ReadOnly = True
        DataGridViewAssetPrices.Size = New Size(344, 90)
        DataGridViewAssetPrices.TabIndex = 10
        DataGridViewAssetPrices.Visible = False
        ' 
        ' ticker
        ' 
        ticker.HeaderText = "ticker"
        ticker.Name = "ticker"
        ticker.ReadOnly = True
        ' 
        ' price
        ' 
        price.HeaderText = "price"
        price.Name = "price"
        price.ReadOnly = True
        ' 
        ' todayChange
        ' 
        todayChange.HeaderText = "todayChange"
        todayChange.Name = "todayChange"
        todayChange.ReadOnly = True
        ' 
        ' BottomPanel
        ' 
        BottomPanel.Controls.Add(Button1)
        BottomPanel.Controls.Add(BtnTest)
        BottomPanel.Dock = DockStyle.Bottom
        BottomPanel.Location = New Point(0, 528)
        BottomPanel.Name = "BottomPanel"
        BottomPanel.Size = New Size(649, 211)
        BottomPanel.TabIndex = 12
        ' 
        ' Button1
        ' 
        Button1.Location = New Point(550, 137)
        Button1.Name = "Button1"
        Button1.Size = New Size(84, 42)
        Button1.TabIndex = 3
        Button1.Text = "Degiro scan"
        Button1.UseVisualStyleBackColor = True
        ' 
        ' BtnTest
        ' 
        BtnTest.Location = New Point(477, 137)
        BtnTest.Name = "BtnTest"
        BtnTest.Size = New Size(67, 42)
        BtnTest.TabIndex = 2
        BtnTest.Text = "test me"
        BtnTest.UseVisualStyleBackColor = True
        ' 
        ' TopPanel
        ' 
        TopPanel.BackColor = Color.SlateGray
        TopPanel.Controls.Add(ListBox1)
        TopPanel.Controls.Add(Label1)
        TopPanel.Dock = DockStyle.Fill
        TopPanel.Location = New Point(0, 0)
        TopPanel.Name = "TopPanel"
        TopPanel.Size = New Size(649, 528)
        TopPanel.TabIndex = 13
        ' 
        ' ListBox1
        ' 
        ListBox1.Font = New Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        ListBox1.FormattingEnabled = True
        ListBox1.ItemHeight = 25
        ListBox1.Location = New Point(351, 418)
        ListBox1.Name = "ListBox1"
        ListBox1.Size = New Size(295, 104)
        ListBox1.TabIndex = 1
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Font = New Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        Label1.Location = New Point(351, 385)
        Label1.Name = "Label1"
        Label1.Size = New Size(73, 30)
        Label1.TabIndex = 0
        Label1.Text = "Label1"
        ' 
        ' TmerKeyIput
        ' 
        TmerKeyIput.Enabled = True
        ' 
        ' TmerAutoStart
        ' 
        TmerAutoStart.Enabled = True
        TmerAutoStart.Interval = 5000
        ' 
        ' FrmMain
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.MidnightBlue
        ClientSize = New Size(649, 771)
        Controls.Add(TopPanel)
        Controls.Add(BottomPanel)
        Controls.Add(DataGridViewAssetPrices)
        Controls.Add(PictureLedGreenOn)
        Controls.Add(PictureLedGreenOff)
        Controls.Add(PictureLedRedOn)
        Controls.Add(PictureLedRedOff)
        Controls.Add(StatusStrip)
        Name = "FrmMain"
        Text = "not WTI"
        StatusStrip.ResumeLayout(False)
        StatusStrip.PerformLayout()
        CType(PictureLedRedOff, ComponentModel.ISupportInitialize).EndInit()
        CType(PictureLedRedOn, ComponentModel.ISupportInitialize).EndInit()
        CType(PictureLedGreenOff, ComponentModel.ISupportInitialize).EndInit()
        CType(PictureLedGreenOn, ComponentModel.ISupportInitialize).EndInit()
        CType(DataGridViewAssetPrices, ComponentModel.ISupportInitialize).EndInit()
        BottomPanel.ResumeLayout(False)
        TopPanel.ResumeLayout(False)
        TopPanel.PerformLayout()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents TmrUI As Timer
    Friend WithEvents statusLed As ToolStripStatusLabel
    Friend WithEvents statusLabel As ToolStripStatusLabel
    Friend WithEvents degiroLabel As ToolStripStatusLabel
    Friend WithEvents esterLabel As ToolStripStatusLabel
    Friend WithEvents StatusStrip As StatusStrip
    Friend WithEvents PictureLedRedOff As PictureBox
    Friend WithEvents PictureLedRedOn As PictureBox
    Friend WithEvents PictureLedGreenOff As PictureBox
    Friend WithEvents PictureLedGreenOn As PictureBox
    Friend WithEvents DataGridViewAssetPrices As DataGridView
    Friend WithEvents ticker As DataGridViewTextBoxColumn
    Friend WithEvents price As DataGridViewTextBoxColumn
    Friend WithEvents todayChange As DataGridViewTextBoxColumn
    Friend WithEvents BottomPanel As Panel
    Friend WithEvents BtnTest As Button
    Friend WithEvents TopPanel As Panel
    Friend WithEvents Label1 As Label
    Friend WithEvents ListBox1 As ListBox
    Friend WithEvents TmerKeyIput As Timer
    Friend WithEvents Button1 As Button
    Friend WithEvents runType As ToolStripStatusLabel
    Friend WithEvents TmerAutoStart As Timer

End Class
