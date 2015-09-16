Public Class FloatingFadingText
    Public Message As String
    Private DrawColor As Color
    Private Font As SpriteFont
    Private Pos As Vector2
    Private doShiftTextUpwards As Boolean
    Private ShiftMult As Single
    Public Property isDone As Boolean

        Get
            If IsNothing(cd) Then
                Return False
            Else
                Return cd.isExpired()
            End If
        End Get
        Set(value As Boolean)

        End Set
    End Property
    Private ReadOnly Property MaxTextShift As Single
        Get
            Return Font.MeasureString(Message).Y * ShiftMult
        End Get
 
    End Property
    Private cd As Cooldown
    ''' <summary>
    ''' Creates a new FloatingFadingText instance.
    ''' </summary>
    ''' <param name="InitialLocation">Where to start the floatingText.</param>
    ''' <param name="textValue">The text to display..</param>
    ''' <param name="duration">The time the text remains in the air. It will become increasingly transparent until it completes and fades entirely.</param>
    ''' <param name="FontVal">SpriteFont instance</param>
    ''' <param name="dc">The color to draw.</param>
    ''' <param name="shiftTextUpwards">Whether or not to shift the text upwards by the UpwardsShiftMaxMultiplier value times the height of the message. </param>
    ''' <param name="UpwardsShiftMaxMultiplier" >How much to allow the text to shift upwards by. A value of 3 indicates that the text will shift 3 times the height of the text.</param>
    ''' <remarks></remarks>
    Public Sub New(InitialLocation As Vector2, textValue As String, duration As Integer, FontVal As SpriteFont, dc As Color, Optional shiftTextUpwards As Boolean = True, Optional UpwardsShiftMaxMultiplier As Single = 3.0)
        cd = New Cooldown(duration, duration, False, IDGenerator.Generate(New Random()) + " floatingFading instance")
        Message = textValue
        DrawColor = dc
        Font = FontVal
        Pos = InitialLocation
        Pos.X -= CInt(Font.MeasureString(Message).X / 2)
        doShiftTextUpwards = shiftTextUpwards
        ShiftMult = UpwardsShiftMaxMultiplier
    End Sub

    Public Sub Update(gt As GameTime, sb As SpriteBatch)
        cd.Update(gt)

        Dim finalPos As Vector2 = Pos



        Dim percentage As Single = CSng(cd.Percentage)
        If Not isDone Then
            If doShiftTextUpwards Then

                finalPos.Y -= percentage * MaxTextShift

            End If
            Dim finalColor As Color = DrawColor
            finalColor.A = CByte(255) - CByte(percentage * 255)
            sb.DrawString(Font, Message, finalPos, finalColor)

        End If
    End Sub
End Class
