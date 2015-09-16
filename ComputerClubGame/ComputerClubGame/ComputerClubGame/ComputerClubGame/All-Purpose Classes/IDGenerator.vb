Public Class IDGenerator
    Public Shared Function Generate(r As Random) As String
        Dim s As String = "abcdefghijklmnopqrstuvwxyz0123456789"
        Dim sb As String
        sb = ""
        Dim cnt As Integer = 8
        For i As Integer = 1 To cnt
            Dim idx As Integer = r.Next(0, s.Length)
            sb += (s.Substring(idx, 1))
        Next
        Return sb.ToString()
    End Function
End Class
