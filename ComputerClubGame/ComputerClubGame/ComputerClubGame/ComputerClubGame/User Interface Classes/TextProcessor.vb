Public Class TextProcessor
    Inherits UIEntity

    Protected Font As SpriteFont
    Protected displayMessage As String
    Protected corLengthStrings As String()
    Protected FontColor As Color
    Protected InsetAmountHeight As Integer
    Protected InsetAmountWidth As Integer
    Public Property BorderHeight As Integer
        Get
            Return InsetAmountHeight
        End Get
        Set(value As Integer)
            InsetAmountHeight = value
        End Set
    End Property
    Public Property BorderWidth As Integer
        Get
            Return InsetAmountWidth
        End Get
        Set(value As Integer)
            InsetAmountWidth = value
        End Set
    End Property
    Public Property Color As Color
        Get
            Return FontColor
        End Get
        Set(value As Color)
            FontColor = value
        End Set
    End Property
    Public Property Message As String
        Get
            Return displayMessage
        End Get
        Set(value As String)
            displayMessage = value
            corLengthStrings = wrapTextLines(Message, Me.entitySize.Width)
        End Set
    End Property
    Public Sub New(boxSize As Size, givenPosition As Vector2, fontAsset As SpriteFont, CM As ContentManager, message As String, borderAmountWidth As Integer, borderAmountHeight As Integer)
        MyBase.New(New Size())
        Me.entitySize = boxSize
        Font = fontAsset
        displayMessage = message
        Me.position.X = givenPosition.X
        Me.position.Y = givenPosition.Y
        Color = Color.Gray
        corLengthStrings = wrapTextLines(message, Me.entitySize.Width)
        BorderWidth = borderAmountWidth
        BorderHeight = borderAmountHeight
    End Sub

    Public Function getNumOfStringLines() As Integer
        Return corLengthStrings.Length
    End Function
    ''' <summary>
    ''' Normal draw method
    ''' </summary>
    ''' <param name="sb"></param>
    ''' <remarks></remarks>
    Public Overloads Sub Draw(sb As SpriteBatch)
        Dim textRect As Rectangle
        textRect = New Rectangle(CInt(position.X), CInt(position.Y), Size.Width, Size.Height)
        Dim textSize As Vector2 = Font.MeasureString(displayMessage)
        For i As Integer = 0 To corLengthStrings.Length - 1 Step 1
            Dim textPos As Vector2 = New Vector2(position.X + BorderWidth, position.Y + BorderHeight + i * textSize.Y)
            sb.DrawString(Font, corLengthStrings(i), textPos, Color)
        Next

    End Sub
    ''' <summary>
    ''' draw method for scroll box and limited lines
    ''' </summary>
    ''' <param name="sb"></param>
    ''' <param name="startingLine"></param>
    ''' <param name="endLine"></param>
    ''' <remarks></remarks>
    Public Overloads Sub Draw(sb As SpriteBatch, startingLine As Integer, endLine As Integer)
        Dim textRect As Rectangle
        textRect = New Rectangle(CInt(position.X), CInt(position.Y), Size.Width, Size.Height)
        Dim textSize As Vector2 = Font.MeasureString(displayMessage)
        For i As Integer = startingLine To endLine Step 1
            Dim textPos As Vector2 = New Vector2(position.X, position.Y + (i-startingLine) * textSize.Y)
            sb.DrawString(Font, corLengthStrings(i), textPos, Color)
        Next

    End Sub
    Protected Function wrapTextLines(text As String, boxLength As Integer) As String()
        Dim returnStrings As String()
        Dim lastword As String = ""
        ReDim returnStrings(0)
        returnStrings(0) = "null"
        Dim workingText As String = text
        Dim spaces As Integer()
        ReDim spaces(0)
        spaces(0) = 0
        'HERE IS THE ERROR
        'NEVER LEAVES THIS WHILE
        If (text.IndexOf(" ") = -1) Then
            ReDim returnStrings(0)
            returnStrings(0) = text
            Return returnStrings
        End If
        While (spaces(spaces.Length - 1) <> text.LastIndexOf(" "))
            ReDim Preserve spaces(spaces.Length)
            spaces(spaces.Length - 1) = text.IndexOf(" ", spaces(spaces.Length - 2) + 1)
        End While
        For i As Integer = 0 To spaces.Length - 1 Step 1
            If (i>0)
                lastword = workingText.Substring(spaces(i)-1, 1)
            End If
            Dim textLengthInPixels As Vector2 = Font.MeasureString(workingText.Substring(0, spaces(i)))
            If (textLengthInPixels.X > boxLength) Then
                If (returnStrings(returnStrings.Length - 1).Equals("null")) Then
                    'do nothing so that the null value is replaced with the real string
                Else
                    ReDim Preserve returnStrings(returnStrings.Length)
                End If
                
                returnStrings(returnStrings.Length - 1) = workingText.Substring(0, spaces(i))
                workingText = workingText.Substring(spaces(i), workingText.Length - spaces(i))
                For j As Integer = i+1  To spaces.Length - 1 Step 1
                    spaces(j) -= spaces(i)
                Next
            Else If (lastword = "`")
                If (returnStrings(returnStrings.Length - 1).Equals("null")) Then
                    'do nothing so that the null value is replaced with the real string
                Else
                    ReDim Preserve returnStrings(returnStrings.Length)
                End If
                
                returnStrings(returnStrings.Length - 1) = workingText.Substring(0, spaces(i-1))
                workingText = workingText.Substring(spaces(i), workingText.Length - spaces(i))
                For j As Integer = i+1  To spaces.Length - 1 Step 1
                    spaces(j) -= spaces(i)
                Next
            End If
        Next
        ReDim Preserve returnStrings(returnStrings.Length)
        returnStrings(returnStrings.Length - 1) = workingText
        Return returnStrings
        'Dim textLengthInPixels As Vector2 = Font.MeasureString(text)
        'Dim textPos As Vector2 = New Vector2(position.X + (Math.Abs(Size.Width - textLengthInPixels.X) / 2), position.Y + (Math.Abs(Size.Height - textLengthInPixels.Y) / 2))

    End Function


End Class