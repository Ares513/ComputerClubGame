Public Class ProgressBar
    Protected emptyTexture, fullTexture As Texture2D
    Protected barSize As Size
    Public MinValue, MaxValue As Integer
    Protected CurrentValue As Integer
    Public Total As Integer
    Protected Done As Boolean
    Protected Pos As Vector2
    Protected DrawnTitle As String = ""
    Protected WriteTitle As Boolean
    Protected internalFont As SpriteFont
    Protected TextColor As Color = Color.White
    Protected DrawTitleAsCurrentValue As Boolean
    Protected requireMouseToShowTitle As Boolean
    Public Visible As Boolean = True
    Public Property WriteCurrentValueAsTitle As Boolean
        Get
            Return DrawTitleAsCurrentValue
        End Get
        Set(value As Boolean)
            DrawTitleAsCurrentValue = value
        End Set
    End Property
    Public Property FontColor As Color
        Get
            Return TextColor
        End Get
        Set(value As Color)
            TextColor = value
        End Set
    End Property

    Public Property Font As SpriteFont
        Get
            Return internalFont
        End Get
        Set(value As SpriteFont)
            internalFont = value
        End Set
    End Property
    Public Property ShowTitle As Boolean
        Get
            Return WriteTitle
        End Get
        Set(value As Boolean)
            WriteTitle = value
        End Set
    End Property
    Public Property Title As String
        Get
            Return DrawnTitle
        End Get
        Set(value As String)
            DrawnTitle = value
        End Set
    End Property
    Public ReadOnly Property Size As Size
        Get
            Return barSize
        End Get
    End Property
    Public Property Center As Vector2
        Get
            Return New Vector2(Pos.X + CInt(0.5 * Size.Width), Pos.Y + CInt(0.5 * Size.Height))
        End Get
        Set(value As Vector2)
            Dim newCenter As Vector2
            newCenter = value
            Pos.X = CInt(newCenter.X - (0.5 * Size.Width))
            Pos.Y = CInt(newCenter.Y - (0.5 * Size.Height))
        End Set
    End Property
    Protected drawColor As Color
    Protected backColor_ As Color = Color.White
    Event ProgressBarCompleted(ByVal totalTime As Integer)
    Event ProgressBarUpdated(ByVal CurrentValue As Integer, Percentage As Double)
    Public Paused As Boolean = False

    Public Property Position As Vector2
        Get
            Return Pos
        End Get
        Set(value As Vector2)
            Pos = value
        End Set
    End Property
    Public ReadOnly Property isDone As Boolean
        Get
            Return Done
        End Get
    End Property
    Public ReadOnly Property Current As Integer
        Get
            Return CurrentValue
        End Get
    End Property
    Public ReadOnly Property Percentage As Double
        Get
            Return CurrentValue / MaxValue
        End Get
    End Property
    Public Property Color As Color
        Get
            Return drawColor
        End Get
        Set(value As Color)
            drawColor = value
        End Set
    End Property
    Public Property BackColor As Color
        Get
            Return backColor_
        End Get
        Set(value As Color)
            backColor_ = value
        End Set
    End Property
    ''' <summary>
    ''' Creates a new CCG.ProgressBar instance.
    ''' </summary>
    ''' <param name="initEmptyTexture">The texture of the progress bar when completely empty.</param>
    ''' <param name="initFullTexture">The texture when completely full. Will overlay onto the other texture.</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal initEmptyTexture As Texture2D, ByVal initFullTexture As Texture2D, ByVal inSize As Size, inPosition As Vector2)
        emptyTexture = initEmptyTexture
        fullTexture = initFullTexture
        barSize = inSize
        Pos = inPosition
        MaxValue = 1
        CurrentValue = 1
        Color = Color.White
    End Sub
    Public Sub New(ByVal initEmptyTexture As Texture2D, ByVal initFullTexture As Texture2D, ByVal inSize As Size, inPosition As Vector2, progressBarMinimum As Integer, progressBarMaximum As Integer)
        emptyTexture = initEmptyTexture
        fullTexture = initFullTexture
        barSize = inSize
        Pos = inPosition
        MaxValue = progressBarMaximum
        CurrentValue = progressBarMinimum
    End Sub
    ''' <summary>
    ''' Increments the progressBar by a specific amount.
    ''' </summary>
    ''' <param name="amount">The amount to increase the progress bar by. Use gameTime.ElapsedTime.Miliseconds for a timer.</param>
    ''' <remarks>Will raise ProgressBarCompleted when the progres bar is full.</remarks>
    Public Sub Increment(amount As Integer)
        If Not Paused Then
            If Not Done Then
                If CurrentValue + amount <= MaxValue Then
                    CurrentValue += amount
                    RaiseEvent ProgressBarUpdated(CurrentValue, Percentage)
                Else 'The progress bar must be full.
                    CurrentValue = MaxValue
                    RaiseEvent ProgressBarCompleted(CurrentValue)
                    Total += CurrentValue
                    Done = True
                End If
            End If
        End If
    End Sub
    ''' <summary>
    ''' A value between zero and one that describes the percentage.
    ''' </summary>
    ''' <param name="inPercentage">Between 0 or 1</param>
    ''' <remarks></remarks>
    Public Sub SetPercentage(inPercentage As Double)
        If inPercentage > 1 Then
            DebugManagement.WriteLineToLog("Someone tried to make a progress bar's percentage more than 100%!", SeverityLevel.WARNING)
        End If
        CurrentValue = CInt(inPercentage * MaxValue)
    End Sub
    ''' <summary>
    ''' Stops the progress bar and raises an event for its completion.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Halt()
        Done = True
        RaiseEvent ProgressBarCompleted(CurrentValue)
    End Sub
    Public Sub Setup(minimum As Integer, maximum As Integer)
        MinValue = minimum
        MaxValue = maximum
    End Sub
    ''' <summary>
    ''' Restarts the timer with the specified minimum and maximum values.
    ''' </summary>
    ''' <param name="minimum"></param>
    ''' <param name="maximum"></param>
    ''' <remarks></remarks>
    Public Sub Reset(minimum As Integer, maximum As Integer)
        Done = False
        MinValue = minimum
        MaxValue = maximum
    End Sub

    ''' <summary>
    ''' Draws the progress bar based on the current minimum and maximum values.
    ''' </summary>
    ''' <param name="sb">SpriteBatch instance</param>
    ''' <param name="drawEmpty">Whether or not to draw the underlying empty ProgressBar texture.</param>
    ''' <remarks></remarks>
    Public Overridable Sub Draw(sb As SpriteBatch, drawEmpty As Boolean, ms As MouseState)
        If Not Visible Then
            Exit Sub
        End If
        'Always draw the empty texture.
        Dim destRectangle As Rectangle
        destRectangle = New Rectangle(CInt(Pos.X), CInt(Pos.Y), Size.Width, Size.Height)
        If IsNothing(emptyTexture) Then
            'Oops. No empty texture. Maybe they didn't want it drawn?
            DebugManagement.WriteLineToLog("ProgressBar encountered an error: emptyTexture is null!", SeverityLevel.WARNING)
        Else
            If drawEmpty Then
                destRectangle = New Rectangle(CInt(Pos.X), CInt(Pos.Y), Size.Width, Size.Height)
                sb.Draw(emptyTexture, destRectangle, BackColor)
            End If

        End If
        If IsNothing(fullTexture) Then
            '... Whoever instantiated this is an idiot
            DebugManagement.WriteLineToLog("ProgressBar encountered an error: fullTexture is null!", SeverityLevel.SEVERE)
            Exit Sub
        Else
            Dim partialRectangle As Rectangle
            partialRectangle = destRectangle
            partialRectangle.Width = CInt((Current / MaxValue) * Size.Width)
            sb.Draw(fullTexture, partialRectangle, Color)
            If WriteTitle Then
                If IsNothing(Font) Then
                    'Oops. Someone forgot to load the Font.
                    DebugManagement.WriteLineToLog("A Font wasn't loaded for the ProgressBar class!", SeverityLevel.WARNING)
                    Exit Sub

                Else
                    If DrawTitleAsCurrentValue Then
                        Dim message As String
                        message = Current.ToString() + " / " + MaxValue.ToString()
                        sb.DrawString(Font, message, New Vector2(Center.X - CInt((Font.MeasureString(message).X / 2)), Center.Y - Font.MeasureString(message).Y / 2), FontColor)
                    Else

                        sb.DrawString(Font, Title, New Vector2(Center.X - CInt((Font.MeasureString(Title).X / 2)), Center.Y - Font.MeasureString(Title).Y / 2), FontColor)
                    End If

                End If
            End If
        End If
    End Sub
End Class
