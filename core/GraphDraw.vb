Namespace GraphDraw


    Module GraphDraw
        Public topPicture As New Bitmap(1, 1)
        Public bottomPicture As New Bitmap(1, 1)

        Public currentAsset As AssetInfos

        Dim gTop As Graphics = Graphics.FromImage(topPicture)
        Dim gBottom As Graphics = Graphics.FromImage(topPicture)

        Public Sub render()
            dbg.info("dra")
            resizePicture()

            paintItBlack(topPicture, gTop)
            paintItBlack(bottomPicture, gBottom)

            Dim b As New SolidBrush(Color.Black)




            ' topPicture.SetPixel(10, 10, Color.Red)

            FrmMain.PictureBoxTopGraph.Image = topPicture
            FrmMain.PictureBoxBottomGraph.Image = bottomPicture
            Application.DoEvents()
        End Sub


        '


        Private Sub resizePicture()
            If FrmMain.PanelGraphTop.Size.ToString <> topPicture.Size.ToString Then topPicture = New Bitmap(FrmMain.PanelGraphTop.Size.Width, FrmMain.PanelGraphTop.Size.Height)
            If FrmMain.PanelGraphBottom.Size.ToString <> bottomPicture.Size.ToString Then bottomPicture = New Bitmap(FrmMain.PanelGraphBottom.Size.Width, FrmMain.PanelGraphBottom.Size.Height)
        End Sub

        Public Sub paintItBlack(ByRef image As Bitmap, ByRef g As Graphics)
            g.FillRectangle(New SolidBrush(Color.Red), New Rectangle(0, 0, image.Width, image.Height))
        End Sub

    End Module

End Namespace

'Using br = New LinearGradientBrush(New Rectangle(50, 50, 10, 10), Color.Red, Color.Orange, 25)
'e.Graphics.FillRectangle(br, New Rectangle(50, 50, 10, 10))
'End Using