Public Class StringHelper
    ''' <summary>
    ''' Substitutes a number for a symbol. For example, 1 would become !
    ''' </summary>
    ''' <param name="input"></param>
    ''' <remarks></remarks>
    Public Shared Function ConvertNumberToSymbol(input As Char) As Char
        Select Case input
            Case "1"c
                Return "!"c
            Case "2"c
                Return "@"c
            Case "3"c
                Return "#"c
            Case "4"c
                Return "$"c
            Case "5"c
                Return "%"c
            Case "6"c
                Return "^"c
            Case "7"c
                Return "&"c
            Case "8"c
                Return "*"c
            Case "9"c
                Return "("c
            Case "0"c
                Return ")"c
            Case Else
                DebugManagement.WriteLineToLog("StringHelper was passed an invalid value for conversion.", SeverityLevel.WARNING)
                Return "0"c
        End Select
    End Function
End Class
