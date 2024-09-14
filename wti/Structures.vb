Imports System.IO
Imports System.Reflection

Public Module Structures
    'please take care  to sell trade with this strat

    Public status As StatusEnum = StatusEnum.NONE

    Public Enum StatusEnum
        NONE
        OFFLINE
        ONLINE
        SIMU
        LIVE
        INTERRUPT
        'just collect data from trading view
        'COLLECT
    End Enum

    ' ----------------------------------------------------------------------------------------


    'Public Enum OrderTypeEnum
    '    LIMIT_SELL
    '    LIMIT_BUY
    '    ' stop loss
    '    STOP_SELL
    '    STOP_BUY
    'End Enum

    Public Function StructToString(obj As Object) As String
        Dim structString As String = ""
        Dim i As Integer
        Dim myType As Type = obj.GetType()
        Dim myField As FieldInfo() = myType.GetFields()
        For i = 0 To myField.Length - 1
            structString &= myField(i).Name & ":" & myField(i).GetValue(obj) & " "
        Next i

        Return structString
    End Function


    Public Function formatPrice(amount As Double) As String
        If amount > 1000 Then Return Math.Round(amount)
        If amount > 10 Then Return Math.Round(amount * 10) / 10
        If amount > 1 Then Return Math.Round(amount * 100) / 100
        Return Math.Round(amount * 10000) / 10000
    End Function

    'Public Function degiroOrderToString(o As DegiroOrder) As String
    '    Return o.ticker & " " & o.isin & " " & o.dat.ToString & " " & o.orderAction & " " & o.quantity & " " & o.limit & " " & o.stopPrice
    'End Function


End Module
