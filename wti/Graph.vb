public class Graph

    public asset as AssetInfos
    public parentPanel as Panel
    public fromDate as date
    public toDate as date

    public elapsed as double

    private xLegend as new Panel
    private yLegend as new Panel
    private pictureBox as new PictureBox

    private img as new Bitmap(1,1)
    private g As Graphics

    public sub init(parentPanel as Panel, asset as AssetInfos)
      me.parentPanel = parentPanel
      me.asset = asset

      fromDate = Date.UtcNow.addDays(-5)
      toDate = Date.UtcNow

with yLegend
      .parent = parentPanel
      .Width = 75
      .dock = Right
      .BackColor = Color.Green
end with

with xLegend
      .parent = parentPanel
      .dock = Bettom
      .Height = 50
      .BackColor = Color.Yellow
end with

      with pictureBox
        .parent = parentPanel
        .Enabled = true
        .dock = Fill
        .BackColor = Color.red
      end with

render()
    end sub

    public sub setDateRange(fromDate as date, toDate as date)
me.fromDate = fromDate
me.toDate=toDate
render()
    end sub

    private sub renderAssetPrices()


    end sub


public sub render()
dim start as double = Date.UtcNow()
checkResized()
 ' init
  paintItBlack()

  ' actual stuff
  renderAssetPrices()

  ' renderOrders()

  ' renderPositions()

  ' renderIndicators()

pictureBox.image = img
Application.DoEvents()
elapsed = Date.UtcNow.Subtract(start).TotalMilliseconds

end sub


private sub checkResized()
if pictureBox.size.ToString() <> img.size.ToString() then
img = New Bitmap(pictureBox.size.Width, pictureBox.size.Height)
                g = Graphics.FromImage(img)
end if

end sub

' -----------------------------------------------------------------------------------------------------
' Drawing

   private Sub paintItBlack()
            g.FillRectangle(New SolidBrush(Color.Black), New Rectangle(0, 0, img.Width, img.Height))
        End Sub
end class
