Public Module DegiroPositionm
    Public Structure DegiroPosition
        'unused
        Dim id As String
        ' 3OIL
        Dim ticker As String
        ' IE00BMTM6B32
        Dim isin As String

        Dim quantity As Integer
        'how much is it now
        Dim currentTotalValue As Double
        'how much you bought it
        Dim pru As Double

    End Structure

End Module
