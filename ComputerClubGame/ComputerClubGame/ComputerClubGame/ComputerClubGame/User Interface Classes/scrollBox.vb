Public Class scrollBox
    Inherits UIEntity
    Dim Font As SpriteFont   
    Dim myScrollWheel As ScrollWheel
    Dim myTextBox As TextProcessor
    Dim textSize As Vector2


    Public Sub New(myPos As Vector2, mySize As Size, message As String, fontType As SpriteFont, scrollWheelAssetNames() As string, CM As ContentManager)
        MyBase.New(New Size())
        
        Me.entitySize = mySize
        Me.position.X = myPos.X
        Me.position.Y = myPos.y

        textSize = fontType.MeasureString(message)

        myTextBox = New TextProcessor(mySize, myPos, fontType, CM, message, 0, 0)

        myScrollWheel = New ScrollWheel("textScroll", New Size (20,100), scrollWheelAssetNames, CM, New Vector2(myPos.y, myPos.y +mySize.Height), CInt((Me.entitySize.Height)/myTextBox.getNumOfStringLines))
        myScrollWheel.position.Y = myPos.Y
        myScrollWheel.position.X = myPos.X - 20
    End Sub

    Public Sub updateObjects(MS As MouseState, gameTime As Microsoft.Xna.Framework.GameTime)
        myScrollWheel.IsPressed(MS, gameTime)
    End Sub


    Public Shadows Sub draw(sb As SpriteBatch)
        Dim extraLines As Integer
        If (myTextBox.getNumOfStringLines * textSize.Y > Me.entitySize.Height) Then
            extraLines = CInt((myTextBox.getNumOfStringLines * textSize.Y - Me.entitySize.Height) / textSize.Y)
        Else
            extraLines = 0
        End If
        If (extraLines <= 0) Then
            myScrollWheel.Size.Height = Me.entitySize.Height
            myScrollWheel.Draw(sb)
            myTextBox.Draw(sb)
        Else
            ' change the scroll wheel size based on amount of extra lines
            ' draw text based on position of scroll wheel
            Dim setHeight As Double = ((myTextBox.getNumOfStringLines - extraLines) / myTextBox.getNumOfStringLines) * Me.Size.Height
            If (setHeight > 10) Then
                myScrollWheel.Size.Height = CInt(setHeight)
            Else
                myScrollWheel.Size.Height = 10
            End If

            Dim scrollLineHeight As Double
            scrollLineHeight = (Me.entitySize.Height) / myTextBox.getNumOfStringLines

            Dim addedLines As Integer
            addedLines = CInt((myScrollWheel.position.Y - Me.position.Y) / scrollLineHeight)

            myScrollWheel.Draw(sb)
            myTextBox.Draw(sb, 0 + addedLines, myTextBox.getNumOfStringLines - extraLines + addedLines - 1)
        End If

    End Sub
End Class
