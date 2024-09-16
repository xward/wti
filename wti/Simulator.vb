
Namespace Simulator
    Module Simulator
        'helpers fonctions

        Private asset As AssetInfos

        Public Sub newSimulation(a As AssetInfos)
            asset = a
            status = StatusEnum.SIMU
            dbg.info("STARTING SIMULATION")
            Degiro.SIMU_init()
            FrmMain.ToolStripProgressBarSimu.Visible = True
            ' todo: from date to date
            replayInit(asset)
        End Sub

        Public Sub run(decision As Action)
            Dim start As Date = Date.UtcNow
            While status = StatusEnum.SIMU And replayNext(asset)
                FrmMain.mainGraph.asyncRender()

                decision()

                Degiro.SIMU_updateAll()
                Degiro.updateTradePanelUI()

            End While

            dbg.info("Simulation compeleted within " & Math.Round(Date.UtcNow.Subtract(start).TotalMilliseconds) & "ms")
            status = StatusEnum.OFFLINE
            FrmMain.ToolStripProgressBarSimu.Visible = False
            Application.DoEvents()
        End Sub
    End Module

End Namespace
