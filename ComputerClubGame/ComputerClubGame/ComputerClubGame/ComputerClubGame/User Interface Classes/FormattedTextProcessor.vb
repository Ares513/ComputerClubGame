Public Class FormattedTextProcessor
    Private oldText As String ()
    Private numberVisibleLines As integer
    Private linkedWrittenTime As integer ()
    Private oldGameTime As Integer
    Private sf As SpriteFont
    Private fontHeight As integer
    Public Sub New(font As SpriteFont)
        ReDim oldText(1)
        oldText(0) = "Welcome to ComputerClubGame!"
        sf = font
        fontHeight = CInt(sf.MeasureString("haha").Y)
        ReDim linkedWrittenTime(1)
        linkedWrittenTime(0) = 0

        numberVisibleLines = 5
    End Sub

    Public Sub addLine(input As String)
        ReDim Preserve oldText(oldText.Length)
        oldText(oldText.Length-1) = input
        ReDim Preserve linkedWrittenTime(oldText.Length-1)
        linkedWrittenTime(linkedWrittenTime.Length-1) = 0
    End Sub

    Public Sub update(gt As GameTime)
        If oldGameTime <> gt.TotalGameTime.Seconds then
            For i As Integer = 0 to linkedWrittenTime.length-1 Step 1              
                linkedWrittenTime(i)+= 1
            Next
        End If

        oldGameTime = gt.TotalGameTime.Seconds
    End Sub
    Public Sub draw(sb As SpriteBatch)
        If (oldText.Length <= numberVisibleLines) Then
            For i As Integer = 0 To oldText.Length - 1 Step 1
                If linkedWrittenTime(i) < 5 Then
                    If IsNothing(oldText(i)) Then
                        oldText(i) = ""
                    End If
                    sb.DrawString(sf, oldText(i), New Vector2(150, CSng(1.5 * fontHeight * i)), Color.Gray)
                End If
            Next
        Else

            For i As Integer = oldText.Length - 1 To oldText.Length - (numberVisibleLines - 1) Step -1
                If linkedWrittenTime(i) < 5 Then
                    sb.DrawString(sf, oldText(i), New Vector2(150, CSng(1.5*fontHeight * (i - oldText.Length + numberVisibleLines))), Color.Gray)
                End If
            Next
        End If
    End Sub
End Class
