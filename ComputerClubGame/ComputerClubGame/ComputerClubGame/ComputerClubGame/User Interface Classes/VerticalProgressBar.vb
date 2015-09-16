''' <summary>
''' Allows creation of vertical progress bars to measure things like player life. 
''' 
''' </summary>
''' <remarks></remarks>
Public Class VerticalProgressBar
    Inherits ProgressBar
    Public Property MouseHoverShowsTitle As Boolean
        Get
            Return requireMouseToShowTitle
        End Get
        Set(value As Boolean)
            requireMouseToShowTitle = value
        End Set
    End Property
    Public Sub New(initEmptyTexture As Texture2D, initFullTexture As Texture2D, initSize As Size, initPosition As Vector2)
        MyBase.New(initEmptyTexture, initFullTexture, initSize, initPosition)

    End Sub

    ''' <summary>
    ''' Draws the progress bar based on the current minimum and maximum values.
    ''' </summary>
    ''' <param name="sb">SpriteBatch instance</param>
    ''' <param name="drawEmpty">Whether or not to draw the underlying empty ProgressBar texture.</param>
    ''' <remarks></remarks>
    Public Overrides Sub Draw(sb As SpriteBatch, drawEmpty As Boolean, ms As MouseState)
        'Always draw the empty texture.
        Dim destRectangle As Rectangle
        destRectangle = New Rectangle(CInt(Pos.X), CInt(Pos.Y), size.Width, size.Height)
        If IsNothing(emptyTexture) Then
            'Oops. No empty texture. Maybe they didn't want it drawn?
            DebugManagement.WriteLineToLog("ProgressBar encountered an error: emptyTexture is null!", SeverityLevel.WARNING)
        Else
            If drawEmpty Then
                destRectangle = New Rectangle(CInt(Pos.X), CInt(Pos.Y), size.Width, size.Height)
                sb.Draw(emptyTexture, destRectangle, backColor_)
            End If

        End If
        If IsNothing(fullTexture) Then
            '... Whoever instantiated this is an idiot.
            DebugManagement.WriteLineToLog("ProgressBar encountered an error: fullTexture is null!", SeverityLevel.SEVERE)
            Exit Sub

        Else
            Dim partialRectangle As Rectangle
            partialRectangle = New Rectangle(0, 0, fullTexture.Width, fullTexture.Height)
            'partialRectangle.Height = CInt((Current / MaxValue) * size.Height)
            ' partialRectangle.Y = partialRectangle.Height - CInt((Percentage) * partialRectangle.Height)
            ' partialRectangle.Height = CInt(fullTexture.Height * 0.5)
            'The partial rectangle represents the texture's rectangle. The size of the actual drawn object is irrelevant in this case.
            partialRectangle.Y = CInt(Math.Floor(fullTexture.Height * (1.0 - Percentage)))
            partialRectangle.Height = CInt(Math.Floor(fullTexture.Height * Percentage))
            destRectangle.Height = CInt(Math.Floor(destRectangle.Height * (Percentage)))
            'destRectangle.Y += CInt(destRectangle.Height * Percentage)
            destRectangle.Y = CInt(Math.Floor(Pos.Y + size.Height * (1.0 - Percentage)))
            '  partialRectangle = New Rectangle(0, CInt(fullTexture.Height - fullTexture.Height * Percentage), fullTexture.Width, CInt(fullTexture.Height * Percentage))
            'destRectangle.Y += 64
            'The plan is to draw from the 
            'Key change here. Makes the progress bar increase vertically, instead of horizontally.

            sb.Draw(fullTexture, destRectangle, partialRectangle, drawColor) ', 0.0, New Vector2(0, 0), SpriteEffects.FlipVertically, 0)
            If WriteTitle And Not IsNothing(Font) Then
                If IsNothing(Font) Then
                    'Oops. Someone forgot to load the Font.
                    DebugManagement.WriteLineToLog("A Font wasn't loaded for the ProgressBar class!", SeverityLevel.WARNING)
                    Exit Sub

                End If
                If DrawTitleAsCurrentValue Then
                    Dim message As String
                    message = Current.ToString() + " / " + MaxValue.ToString()
                    If requireMouseToShowTitle Then
                        If New Rectangle(CInt(Pos.X), CInt(Pos.Y), size.Width, size.Height).Contains(New Point(MS.X, MS.Y)) Then
                            sb.DrawString(Font, message, New Vector2(Center.X - CInt((Font.MeasureString(message).X / 2)), Pos.Y - Font.MeasureString(message).Y / 2), FontColor)
                        End If

                    Else
                        sb.DrawString(Font, message, New Vector2(Center.X - CInt((Font.MeasureString(message).X / 2)), Pos.Y - Font.MeasureString(message).Y / 2), FontColor)
                    End If

                Else

                    sb.DrawString(Font, Title, New Vector2(Center.X - CInt((Font.MeasureString(Title).X / 2)), Center.Y - Font.MeasureString(Title).Y / 2), FontColor)
                End If


            End If
        End If
    End Sub
End Class
