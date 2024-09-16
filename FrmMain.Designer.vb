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
        ToolStripStatusSays = New ToolStripStatusLabel()
        ToolStripStatusLabelDrawFps = New ToolStripStatusLabel()
        ToolStripProgressBarSimu = New ToolStripProgressBar()
        PictureLedRedOff = New PictureBox()
        PictureLedRedOn = New PictureBox()
        PictureLedGreenOff = New PictureBox()
        PictureLedGreenOn = New PictureBox()
        DataGridViewAssetPrices = New DataGridView()
        ticker = New DataGridViewTextBoxColumn()
        price = New DataGridViewTextBoxColumn()
        todayChange = New DataGridViewTextBoxColumn()
        BottomPanel = New Panel()
        Panel1 = New Panel()
        Label1 = New Label()
        ListBoxLogEvents = New ListBox()
        Label2 = New Label()
        Label3 = New Label()
        PanelTrades = New Panel()
        LblActiveTrades = New Label()
        TopPanel = New Panel()
        PanelGraphBottom = New Panel()
        PanelGraphTop = New Panel()
        MainMenuStrip = New MenuStrip()
        TradesToolStripMenuItem = New ToolStripMenuItem()
        ShowAllToolStripMenuItem = New ToolStripMenuItem()
        ManualActionsToolStripMenuItem = New ToolStripMenuItem()
        TestMeToolStripMenuItem = New ToolStripMenuItem()
        DegiroScanToolStripMenuItem = New ToolStripMenuItem()
        GraphRenderToolStripMenuItem = New ToolStripMenuItem()
        FetchSpxFromYahooToolStripMenuItem = New ToolStripMenuItem()
        SimulationToolStripMenuItem = New ToolStripMenuItem()
        RunToolStripMenuItem = New ToolStripMenuItem()
        RunSp500LongToolStripMenuItem = New ToolStripMenuItem()
        AnalysisToolStripMenuItem = New ToolStripMenuItem()
        OpenAnalysisViewerToolStripMenuItem = New ToolStripMenuItem()
        TmerKeyIput = New Timer(components)
        TmerAutoStart = New Timer(components)
        ContextMenuStripGraph = New ContextMenuStrip(components)
        CoucouToolStripMenuItem = New ToolStripMenuItem()
        SetToDateHereToolStripMenuItem = New ToolStripMenuItem()
        StatusStrip.SuspendLayout()
        CType(PictureLedRedOff, ComponentModel.ISupportInitialize).BeginInit()
        CType(PictureLedRedOn, ComponentModel.ISupportInitialize).BeginInit()
        CType(PictureLedGreenOff, ComponentModel.ISupportInitialize).BeginInit()
        CType(PictureLedGreenOn, ComponentModel.ISupportInitialize).BeginInit()
        CType(DataGridViewAssetPrices, ComponentModel.ISupportInitialize).BeginInit()
        BottomPanel.SuspendLayout()
        Panel1.SuspendLayout()
        PanelTrades.SuspendLayout()
        TopPanel.SuspendLayout()
        MainMenuStrip.SuspendLayout()
        ContextMenuStripGraph.SuspendLayout()
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
        statusLabel.Name = "statusLabel"
        statusLabel.Size = New Size(66, 27)
        statusLabel.Text = "OFFLINE"
        ' 
        ' degiroLabel
        ' 
        degiroLabel.BackColor = SystemColors.ButtonFace
        degiroLabel.Name = "degiroLabel"
        degiroLabel.Size = New Size(127, 27)
        degiroLabel.Text = "<degiro stats here>"
        ' 
        ' esterLabel
        ' 
        esterLabel.BackColor = SystemColors.ButtonFace
        esterLabel.ImageScaling = ToolStripItemImageScaling.None
        esterLabel.Name = "esterLabel"
        esterLabel.Size = New Size(84, 27)
        esterLabel.Text = "Ester 3.66%"
        ' 
        ' StatusStrip
        ' 
        StatusStrip.Font = New Font("Cambria", 11.25F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        StatusStrip.Items.AddRange(New ToolStripItem() {statusLed, statusLabel, runType, degiroLabel, esterLabel, ToolStripStatusSays, ToolStripStatusLabelDrawFps, ToolStripProgressBarSimu})
        StatusStrip.Location = New Point(0, 978)
        StatusStrip.Name = "StatusStrip"
        StatusStrip.Padding = New Padding(1, 0, 16, 0)
        StatusStrip.Size = New Size(1940, 32)
        StatusStrip.TabIndex = 4
        ' 
        ' runType
        ' 
        runType.BackColor = SystemColors.ButtonFace
        runType.Name = "runType"
        runType.Size = New Size(55, 27)
        runType.Text = "DEBUG"
        ' 
        ' ToolStripStatusSays
        ' 
        ToolStripStatusSays.BackColor = SystemColors.Control
        ToolStripStatusSays.Name = "ToolStripStatusSays"
        ToolStripStatusSays.Size = New Size(35, 27)
        ToolStripStatusSays.Text = "says"
        ' 
        ' ToolStripStatusLabelDrawFps
        ' 
        ToolStripStatusLabelDrawFps.BackColor = Color.Transparent
        ToolStripStatusLabelDrawFps.Name = "ToolStripStatusLabelDrawFps"
        ToolStripStatusLabelDrawFps.Size = New Size(40, 27)
        ToolStripStatusLabelDrawFps.Text = "-- fps"
        ' 
        ' ToolStripProgressBarSimu
        ' 
        ToolStripProgressBarSimu.Name = "ToolStripProgressBarSimu"
        ToolStripProgressBarSimu.Size = New Size(100, 26)
        ToolStripProgressBarSimu.Visible = False
        ' 
        ' PictureLedRedOff
        ' 
        PictureLedRedOff.Image = CType(resources.GetObject("PictureLedRedOff.Image"), Image)
        PictureLedRedOff.Location = New Point(1053, 661)
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
        PictureLedRedOn.Location = New Point(1096, 661)
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
        PictureLedGreenOff.Location = New Point(1053, 703)
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
        PictureLedGreenOn.Location = New Point(1096, 703)
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
        DataGridViewAssetPrices.Location = New Point(930, 465)
        DataGridViewAssetPrices.Name = "DataGridViewAssetPrices"
        DataGridViewAssetPrices.ReadOnly = True
        DataGridViewAssetPrices.Size = New Size(393, 102)
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
        BottomPanel.Controls.Add(Panel1)
        BottomPanel.Controls.Add(PanelTrades)
        BottomPanel.Dock = DockStyle.Bottom
        BottomPanel.ForeColor = Color.White
        BottomPanel.Location = New Point(0, 739)
        BottomPanel.Name = "BottomPanel"
        BottomPanel.Size = New Size(1940, 239)
        BottomPanel.TabIndex = 12
        ' 
        ' Panel1
        ' 
        Panel1.Controls.Add(Label1)
        Panel1.Controls.Add(ListBoxLogEvents)
        Panel1.Controls.Add(Label2)
        Panel1.Controls.Add(Label3)
        Panel1.Dock = DockStyle.Fill
        Panel1.Location = New Point(462, 0)
        Panel1.Name = "Panel1"
        Panel1.Size = New Size(1478, 239)
        Panel1.TabIndex = 8
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(13, 77)
        Label1.Name = "Label1"
        Label1.Size = New Size(50, 17)
        Label1.TabIndex = 7
        Label1.Text = "Label1"
        ' 
        ' ListBoxLogEvents
        ' 
        ListBoxLogEvents.Dock = DockStyle.Bottom
        ListBoxLogEvents.Font = New Font("Cambria", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        ListBoxLogEvents.FormattingEnabled = True
        ListBoxLogEvents.ItemHeight = 19
        ListBoxLogEvents.Location = New Point(0, 102)
        ListBoxLogEvents.Name = "ListBoxLogEvents"
        ListBoxLogEvents.Size = New Size(1478, 137)
        ListBoxLogEvents.TabIndex = 4
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Location = New Point(3, 3)
        Label2.Name = "Label2"
        Label2.Size = New Size(271, 17)
        Label2.TabIndex = 5
        Label2.Text = "sp500: 5650, trend: 5300, <red> diff: 5.6%"
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Location = New Point(6, 29)
        Label3.Name = "Label3"
        Label3.Size = New Size(163, 34)
        Label3.TabIndex = 6
        Label3.Text = "wti 78$ -5% today" & vbCrLf & "platinum 76€ +3% today" & vbCrLf
        ' 
        ' PanelTrades
        ' 
        PanelTrades.AutoScroll = True
        PanelTrades.Controls.Add(LblActiveTrades)
        PanelTrades.Dock = DockStyle.Left
        PanelTrades.Location = New Point(0, 0)
        PanelTrades.Name = "PanelTrades"
        PanelTrades.Size = New Size(462, 239)
        PanelTrades.TabIndex = 7
        ' 
        ' LblActiveTrades
        ' 
        LblActiveTrades.AutoSize = True
        LblActiveTrades.Font = New Font("Cascadia Mono", 9.75F)
        LblActiveTrades.Location = New Point(3, 3)
        LblActiveTrades.Name = "LblActiveTrades"
        LblActiveTrades.Size = New Size(408, 17)
        LblActiveTrades.TabIndex = 0
        LblActiveTrades.Text = "PLACEHOLER TRY_BUY FAKE_3OIL q5 pru13.31 3.2% away"
        ' 
        ' TopPanel
        ' 
        TopPanel.BackColor = Color.FromArgb(CByte(12), CByte(12), CByte(12))
        TopPanel.Controls.Add(PanelGraphBottom)
        TopPanel.Controls.Add(PanelGraphTop)
        TopPanel.Controls.Add(MainMenuStrip)
        TopPanel.Dock = DockStyle.Fill
        TopPanel.Location = New Point(0, 0)
        TopPanel.Name = "TopPanel"
        TopPanel.Size = New Size(1940, 739)
        TopPanel.TabIndex = 13
        ' 
        ' PanelGraphBottom
        ' 
        PanelGraphBottom.BackColor = Color.Transparent
        PanelGraphBottom.BorderStyle = BorderStyle.FixedSingle
        PanelGraphBottom.Dock = DockStyle.Fill
        PanelGraphBottom.Location = New Point(0, 329)
        PanelGraphBottom.Name = "PanelGraphBottom"
        PanelGraphBottom.Size = New Size(1940, 410)
        PanelGraphBottom.TabIndex = 6
        ' 
        ' PanelGraphTop
        ' 
        PanelGraphTop.BackColor = Color.Transparent
        PanelGraphTop.BorderStyle = BorderStyle.FixedSingle
        PanelGraphTop.Dock = DockStyle.Top
        PanelGraphTop.Location = New Point(0, 24)
        PanelGraphTop.Name = "PanelGraphTop"
        PanelGraphTop.Size = New Size(1940, 305)
        PanelGraphTop.TabIndex = 5
        ' 
        ' MainMenuStrip
        ' 
        MainMenuStrip.Items.AddRange(New ToolStripItem() {TradesToolStripMenuItem, ManualActionsToolStripMenuItem, SimulationToolStripMenuItem, AnalysisToolStripMenuItem})
        MainMenuStrip.Location = New Point(0, 0)
        MainMenuStrip.Name = "MainMenuStrip"
        MainMenuStrip.Size = New Size(1940, 24)
        MainMenuStrip.TabIndex = 4
        MainMenuStrip.Text = "MainMenuStrip"
        ' 
        ' TradesToolStripMenuItem
        ' 
        TradesToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {ShowAllToolStripMenuItem})
        TradesToolStripMenuItem.Name = "TradesToolStripMenuItem"
        TradesToolStripMenuItem.Size = New Size(51, 20)
        TradesToolStripMenuItem.Text = "trades"
        ' 
        ' ShowAllToolStripMenuItem
        ' 
        ShowAllToolStripMenuItem.Name = "ShowAllToolStripMenuItem"
        ShowAllToolStripMenuItem.Size = New Size(117, 22)
        ShowAllToolStripMenuItem.Text = "show all"
        ' 
        ' ManualActionsToolStripMenuItem
        ' 
        ManualActionsToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {TestMeToolStripMenuItem, DegiroScanToolStripMenuItem, GraphRenderToolStripMenuItem, FetchSpxFromYahooToolStripMenuItem})
        ManualActionsToolStripMenuItem.Name = "ManualActionsToolStripMenuItem"
        ManualActionsToolStripMenuItem.Size = New Size(100, 20)
        ManualActionsToolStripMenuItem.Text = "manual actions"
        ' 
        ' TestMeToolStripMenuItem
        ' 
        TestMeToolStripMenuItem.Name = "TestMeToolStripMenuItem"
        TestMeToolStripMenuItem.Size = New Size(195, 22)
        TestMeToolStripMenuItem.Text = "test me"
        ' 
        ' DegiroScanToolStripMenuItem
        ' 
        DegiroScanToolStripMenuItem.Name = "DegiroScanToolStripMenuItem"
        DegiroScanToolStripMenuItem.Size = New Size(195, 22)
        DegiroScanToolStripMenuItem.Text = "degiro scan"
        ' 
        ' GraphRenderToolStripMenuItem
        ' 
        GraphRenderToolStripMenuItem.Name = "GraphRenderToolStripMenuItem"
        GraphRenderToolStripMenuItem.Size = New Size(195, 22)
        GraphRenderToolStripMenuItem.Text = "Render graph one time"
        ' 
        ' FetchSpxFromYahooToolStripMenuItem
        ' 
        FetchSpxFromYahooToolStripMenuItem.Name = "FetchSpxFromYahooToolStripMenuItem"
        FetchSpxFromYahooToolStripMenuItem.Size = New Size(195, 22)
        FetchSpxFromYahooToolStripMenuItem.Text = "fetch spx from yahoo"
        ' 
        ' SimulationToolStripMenuItem
        ' 
        SimulationToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {RunToolStripMenuItem, RunSp500LongToolStripMenuItem})
        SimulationToolStripMenuItem.Name = "SimulationToolStripMenuItem"
        SimulationToolStripMenuItem.Size = New Size(76, 20)
        SimulationToolStripMenuItem.Text = "Simulation"
        ' 
        ' RunToolStripMenuItem
        ' 
        RunToolStripMenuItem.Name = "RunToolStripMenuItem"
        RunToolStripMenuItem.Size = New Size(152, 22)
        RunToolStripMenuItem.Text = "run sp500"
        ' 
        ' RunSp500LongToolStripMenuItem
        ' 
        RunSp500LongToolStripMenuItem.Name = "RunSp500LongToolStripMenuItem"
        RunSp500LongToolStripMenuItem.Size = New Size(152, 22)
        RunSp500LongToolStripMenuItem.Text = "run sp500Long"
        ' 
        ' AnalysisToolStripMenuItem
        ' 
        AnalysisToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {OpenAnalysisViewerToolStripMenuItem})
        AnalysisToolStripMenuItem.Name = "AnalysisToolStripMenuItem"
        AnalysisToolStripMenuItem.Size = New Size(62, 20)
        AnalysisToolStripMenuItem.Text = "Analysis"
        ' 
        ' OpenAnalysisViewerToolStripMenuItem
        ' 
        OpenAnalysisViewerToolStripMenuItem.Name = "OpenAnalysisViewerToolStripMenuItem"
        OpenAnalysisViewerToolStripMenuItem.Size = New Size(186, 22)
        OpenAnalysisViewerToolStripMenuItem.Text = "Open Analysis viewer"
        ' 
        ' TmerKeyIput
        ' 
        TmerKeyIput.Enabled = True
        ' 
        ' TmerAutoStart
        ' 
        TmerAutoStart.Interval = 500
        ' 
        ' ContextMenuStripGraph
        ' 
        ContextMenuStripGraph.Items.AddRange(New ToolStripItem() {CoucouToolStripMenuItem, SetToDateHereToolStripMenuItem})
        ContextMenuStripGraph.Name = "ContextMenuStripGraph"
        ContextMenuStripGraph.Size = New Size(169, 48)
        ' 
        ' CoucouToolStripMenuItem
        ' 
        CoucouToolStripMenuItem.Name = "CoucouToolStripMenuItem"
        CoucouToolStripMenuItem.Size = New Size(168, 22)
        CoucouToolStripMenuItem.Text = "set fromDate here"
        ' 
        ' SetToDateHereToolStripMenuItem
        ' 
        SetToDateHereToolStripMenuItem.Name = "SetToDateHereToolStripMenuItem"
        SetToDateHereToolStripMenuItem.Size = New Size(168, 22)
        SetToDateHereToolStripMenuItem.Text = "set toDate here"
        ' 
        ' FrmMain
        ' 
        AutoScaleDimensions = New SizeF(8F, 17F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.MidnightBlue
        ClientSize = New Size(1940, 1010)
        Controls.Add(TopPanel)
        Controls.Add(BottomPanel)
        Controls.Add(DataGridViewAssetPrices)
        Controls.Add(PictureLedGreenOn)
        Controls.Add(PictureLedGreenOff)
        Controls.Add(PictureLedRedOn)
        Controls.Add(PictureLedRedOff)
        Controls.Add(StatusStrip)
        Font = New Font("Cambria", 11.25F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
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
        Panel1.ResumeLayout(False)
        Panel1.PerformLayout()
        PanelTrades.ResumeLayout(False)
        PanelTrades.PerformLayout()
        TopPanel.ResumeLayout(False)
        TopPanel.PerformLayout()
        MainMenuStrip.ResumeLayout(False)
        MainMenuStrip.PerformLayout()
        ContextMenuStripGraph.ResumeLayout(False)
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
    Friend WithEvents TopPanel As Panel
    Friend WithEvents TmerKeyIput As Timer
    Friend WithEvents runType As ToolStripStatusLabel
    Friend WithEvents TmerAutoStart As Timer
    Friend WithEvents ToolStripStatusSays As ToolStripStatusLabel
    Friend WithEvents Label3 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents MainMenuStrip As MenuStrip
    Friend WithEvents TradesToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ShowAllToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents PanelTrades As Panel
    Friend WithEvents LblActiveTrades As Label
    Friend WithEvents ManualActionsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents TestMeToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents DegiroScanToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents Panel1 As Panel
    Friend WithEvents ListBoxLogEvents As ListBox
    Friend WithEvents SimulationToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents RunToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents PanelGraphTop As Panel
    Friend WithEvents ToolStripStatusLabelDrawFps As ToolStripStatusLabel
    Friend WithEvents GraphRenderToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents PanelGraphBottom As Panel
    Friend WithEvents RunSp500LongToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents FetchSpxFromYahooToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents Label1 As Label
    Friend WithEvents ContextMenuStripGraph As ContextMenuStrip
    Friend WithEvents CoucouToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SetToDateHereToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripProgressBarSimu As ToolStripProgressBar
    Friend WithEvents AnalysisToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents OpenAnalysisViewerToolStripMenuItem As ToolStripMenuItem

End Class
