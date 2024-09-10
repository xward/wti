Public Module DegiroOrderm
    Public Structure DegiroOrder
        'unused
        Dim id As String

        ' 3OIL
        Dim ticker As String
        ' IE00BMTM6B32
        Dim isin As String

        Dim dat As Date

        ' Vente Achat
        Dim orderAction As String

        Dim quantity As Integer

        Dim limit As Double
        Dim stopPrice As Double
    End Structure

End Module
