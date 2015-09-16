Public Class Animation
    Private spriteSheet As Texture2D
    Private spritePosition As Vector2 = New Vector2(0, 0)
    Private CurrentFrame As Short
    Private frameSize As Size
    Private framesPerLine As Short
    Private frameLines As Short
    Private currentAnimation As AnimationDefinition
    Private availableAnimations() As AnimationDefinition
    Private ChangeAnimationOnFinish As Boolean
    Private AnimationToSetOnFinish As Short
    Private PreviousAnimationForOnFinish As String
    Private ReturnToPreviousAnim As Short
    Private ReturnToPreviousAnimOnComplete As Boolean
    Private StartNewAnimationAfterPlayAnimOnce As AnimationRevertState
    Private drawColor As Color = Color.White
    Private initialColor As Color = Color.White
    Private Paused As Boolean
    Private RemainingDelay As Integer
    Private CurrentFacing As Byte
    Private collisionMesh As Rectangle
    Private rollingDrawNoFacing As Boolean
    Private isVisible As Boolean = True
    Public Event AnimDone(definition As AnimationDefinition, currentFrame As Short)
    Public Event AnimChanged(changedFrom As AnimationDefinition, changedTo As AnimationDefinition)
    Public ReadOnly Property isPaused As Boolean
        Get
            Return Paused
        End Get
    End Property
    Public Property Visible As Boolean
        Get
            Return isVisible
        End Get
        Set(value As Boolean)
            isVisible = value
        End Set
    End Property
    ''' <summary>
    ''' Indicates that an image has no facing values and should instead be read row to row.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property NoFacingRollingDraw As Boolean
        Get
            Return rollingDrawNoFacing
        End Get
        Set(value As Boolean)
            rollingDrawNoFacing = value
        End Set
    End Property
    Public Shadows ReadOnly Property Texture As Texture2D
        Get
            Return spriteSheet
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
    Public ReadOnly Property DefaultColor As Color
        Get
            Return initialColor
        End Get
    End Property
    Public ReadOnly Property collisionBox As Rectangle
        Get
            Return collisionMesh
        End Get
    End Property
    Public ReadOnly Property Size As Size
        Get
            Return frameSize
        End Get
    End Property
    Public ReadOnly Property Frame As Short
        Get
            Return CurrentFrame
        End Get
    End Property
    Public Sub New(baseTexture As Texture2D, sizeOfEachFrame As Size, inputAnimationDefinitions As AnimationDefinition(), initialAnimationIndex As Short, initialCollisionBox As Rectangle)
        spriteSheet = baseTexture
        'Load the spriteSheet from the parameters offered from Public Sub New.
        collisionMesh = initialCollisionBox
        frameSize = sizeOfEachFrame
        framesPerLine = CShort(baseTexture.Width / sizeOfEachFrame.Width)
        frameLines = CShort(baseTexture.Height / sizeOfEachFrame.Height)
        availableAnimations = inputAnimationDefinitions
        Try
            currentAnimation = availableAnimations(initialAnimationIndex)
        Catch ex As Exception
            DebugManagement.WriteLineToLog("Error! An animation threw an error when attempting to set the default index! Setting to zero index.", SeverityLevel.CRITICAL)
            currentAnimation = availableAnimations(0)
        End Try

    End Sub
    Public Sub SetPauseState(value As Boolean)
        Paused = value
    End Sub
    Public Sub Pause()
        Paused = True
    End Sub
    Public Sub Unpause()
        Paused = False
    End Sub
    Protected Sub ResetCurrentAnimation()
        SetCurrentFrame(currentAnimation.StartFrame)
    End Sub
    Public ReadOnly Property Animations As AnimationDefinition()
        Get
            Return availableAnimations
        End Get
    End Property
    Public ReadOnly Property CurrentAnimationName As String
        Get
            Return currentAnimation.AnimationName
        End Get
    End Property
    Public Function getCurrentFramePosition(ByVal currentFrame As Short) As Rectangle
        Dim tempWidth As Integer
        tempWidth = currentFrame * frameSize.Width
        Dim counter As Integer
        While tempWidth > spriteSheet.Width
            'This is too long. Obviously we're on the next row. 
            tempWidth = tempWidth - (frameSize.Width * framesPerLine)
            counter += 1
            'Now we know which vertical line we're on for the NoFacingRollingDraw
        End While
        'If it's being drawn the 'simple way', without facing, then we'll just do it this way.
        If NoFacingRollingDraw Then
            Return New Rectangle(tempWidth, counter * frameSize.Height, frameSize.Width, frameSize.Height)
        Else
            Return New Rectangle(tempWidth, CurrentFacing * frameSize.Height, frameSize.Width, frameSize.Height)
        End If

    End Function
    Public Sub SetFacing(Facing As FacingTypes)
        CurrentFacing = Facing
    End Sub
    Public Sub AddAnimation(definition As AnimationDefinition)
        ReDim Preserve availableAnimations(availableAnimations.Length)
        availableAnimations(availableAnimations.Length - 1) = definition

    End Sub
    Public Sub SetCurrentFrame(frameVal As Short)
        If frameVal >= currentAnimation.StartFrame And frameVal <= currentAnimation.EndFrame Then
            CurrentFrame = frameVal
        Else
            DebugManagement.WriteLineToLog("An animation's frame was set to a value that doesn't exist!", SeverityLevel.WARNING)
        End If
    End Sub
    Public Sub AddAnimation(StartFrame As Short, EndFrame As Short, AnimationName As String, MilisecondsPerFrame As Integer)
        ReDim Preserve availableAnimations(availableAnimations.Length)
        availableAnimations(availableAnimations.Length - 1) = New AnimationDefinition(StartFrame, EndFrame, AnimationName, MilisecondsPerFrame)

    End Sub
    Public Sub SetAnimation(index As Short, finishCurrentAnim As Boolean)
        Dim previousAnim As AnimationDefinition = currentAnimation
        If Not finishCurrentAnim Then
            Try

                currentAnimation = availableAnimations(index)
            Catch ex As Exception
                DebugManagement.WriteLineToLog("Someone attempted to set an animation to a value that doesn't exist! Setting to zero-index animation definition.", SeverityLevel.CRITICAL)

            End Try
        Else
            AnimationToSetOnFinish = index

            ChangeAnimationOnFinish = True
            'When the AnimDone event is called, SetAnimationOnFinish is called to set the animation to the specified value.
        End If
        RaiseEvent AnimChanged(previousAnim, availableAnimations(index))

    End Sub
    Public Sub SetAnimation(animName As String, finishCurrentAnim As Boolean)
        Dim previousAnim As AnimationDefinition = currentAnimation
        If Not finishCurrentAnim Then
            Try
                Dim i As Integer
                For i = 0 To availableAnimations.Length - 1 Step 1
                    If availableAnimations(i).AnimationName = animName Then
                        currentAnimation = availableAnimations(i)
                        CurrentFrame = currentAnimation.StartFrame
                        AnimationToSetOnFinish = CShort(i)
                        Exit For
                    End If
                Next
            Catch ex As Exception
                DebugManagement.WriteLineToLog("Someone attempted to set an animation to a value that doesn't exist! Setting to zero-index animation definition.", SeverityLevel.CRITICAL)

            End Try
        Else
            'When the AnimDone event is called, SetAnimationOnFinish is called to set the animation to the specified value.
            Dim i As Integer
            For i = 0 To availableAnimations.Length - 1 Step 1
                If availableAnimations(i).AnimationName = animName Then
                    AnimationToSetOnFinish = CShort(i)
                    PreviousAnimationForOnFinish = CurrentAnimationName
                    Exit For
                End If
            Next

            ChangeAnimationOnFinish = True

        End If
        RaiseEvent AnimChanged(previousAnim, currentAnimation)

    End Sub

    Public Sub SetAnimationSpeed(speed As Integer, animName As String)
        For i = 0 To availableAnimations.Length - 1 Step 1
            If availableAnimations(i).AnimationName.ToLower() = animName.ToLower() Then
                Dim NumberOfFrames As Integer
                NumberOfFrames = availableAnimations(i).EndFrame - availableAnimations(i).StartFrame
                If NumberOfFrames = 0 Then
                    NumberOfFrames = 1
                End If
                availableAnimations(i).MilisecondsPerFrame = CInt(speed / NumberOfFrames)

            End If
        Next
    End Sub
    ''' <summary>
    ''' Plays an animation once.
    ''' </summary>
    ''' <param name="animationToPlay">The String value of the animation you intend to play.</param>
    ''' <param name="AllowRepetitivePlay">Whether or not to play the animation again if the current animation is already playing.</param>
    ''' <param name="StartNewAnimWhenDone">When the animation finishes, instead of switching animations or restarting, it pauses.</param>
    ''' <remarks></remarks>
    Public Sub PlayAnimationOnce(animationToPlay As String, Optional AllowRepetitivePlay As Boolean = True, Optional StartNewAnimWhenDone As AnimationRevertState = AnimationRevertState.RETURN_TO_LAST_ANIMATION)
        Dim i As Integer
        'selection search for the correct animation
        StartNewAnimationAfterPlayAnimOnce = StartNewAnimWhenDone
        If AllowRepetitivePlay Then

            For i = 0 To availableAnimations.Length - 1 Step 1
                If availableAnimations(i).AnimationName = CurrentAnimationName Then
                    ReturnToPreviousAnim = CShort(i)
                    Exit For
                End If
            Next
        ElseIf CurrentAnimationName <> animationToPlay Then
            For i = 0 To availableAnimations.Length - 1 Step 1
                If availableAnimations(i).AnimationName = CurrentAnimationName Then
                    ReturnToPreviousAnim = CShort(i)

                    Exit For
                End If
            Next
        End If

        If AllowRepetitivePlay Then
            SetAnimation(animationToPlay, False)

        ElseIf CurrentAnimationName <> animationToPlay Then
            SetAnimation(animationToPlay, False)
            ReturnToPreviousAnimOnComplete = True
        End If

    End Sub
    Public Sub Update(gt As GameTime)

        If Not Paused Then

            Dim temp = RemainingDelay - gt.ElapsedGameTime.Milliseconds
            If temp < 0 Then
                RemainingDelay = currentAnimation.MilisecondsPerFrame
                CurrentFrame += CShort(1)

            Else
                RemainingDelay = temp
            End If

            If CurrentFrame >= currentAnimation.EndFrame Then
                RaiseEvent AnimDone(currentAnimation, CurrentFrame)
                Select Case StartNewAnimationAfterPlayAnimOnce
                    Case AnimationRevertState.RETURN_TO_LAST_ANIMATION
                        CurrentFrame = currentAnimation.StartFrame
                    Case AnimationRevertState.START_NEW_ANIMATION

                    Case AnimationRevertState.PAUSE_ANIMATION
                        Pause()
                        StartNewAnimationAfterPlayAnimOnce = AnimationRevertState.RETURN_TO_LAST_ANIMATION
                        CurrentFrame -= CShort(1)
                End Select
            End If


        End If
    End Sub
    ''' <summary>
    ''' Draws the current frame of the animation.
    ''' </summary>
    ''' <param name="sb">SpriteBatch</param>
    ''' <param name="LocationAndSize">Rectangle indicating where in screen space to draw the image.</param>
    ''' <param name="layerDepth">The ScaleLayer. See Cam.ScaleLayer.</param>
    ''' <param name="overrideColor">Optional parameter to override the color property. By default, uses the color already specified by the class.</param>
    ''' <remarks></remarks>
    Public Sub Draw(sb As SpriteBatch, LocationAndSize As Rectangle, Optional layerDepth As Single = 0, Optional overrideColor As Color = Nothing)

        If isVisible Then
            Dim tempColor As Color
            If Not (overrideColor.Equals(Color.Transparent)) Then
                'Use isVisible to make something invisible. A value of zero for the overridecolor will be presumed to be empty.
                tempColor = overrideColor
            Else
                tempColor = drawColor
            End If
            sb.Draw(spriteSheet, LocationAndSize, getCurrentFramePosition(CurrentFrame), tempColor, 0, New Vector2(0), SpriteEffects.None, layerDepth)
        End If

    End Sub

    Private Sub NextAnimation() Handles Me.AnimDone
        If ChangeAnimationOnFinish Then
            SetAnimation(AnimationToSetOnFinish, False)
            ChangeAnimationOnFinish = False
        End If
        If ReturnToPreviousAnimOnComplete Then
            If PreviousAnimationForOnFinish = CurrentAnimationName Then
                'Only toggle to the next animation if the previous animation hasn't been changed in between.
                PreviousAnimationForOnFinish = ""
                SetAnimation(ReturnToPreviousAnim, False)

            End If
            ReturnToPreviousAnimOnComplete = False

        End If
    End Sub

End Class
Public Class AnimationDefinition
    Public StartFrame As Short
    Public EndFrame As Short
    Public AnimationName As String
    Public MilisecondsPerFrame As Integer
    Public Sub New(startFrame As Short, endFrame As Short, animName As String, frameLength As Integer)
        Me.StartFrame = startFrame
        Me.EndFrame = endFrame
        Me.AnimationName = animName
        MilisecondsPerFrame = frameLength
    End Sub
End Class
Public Enum FacingTypes As Byte
    WEST
    NORTHWEST
    NORTH
    NORTHEAST
    EAST
    SOUTHEAST
    SOUTH
    SOUTHWEST
End Enum
Public Enum AnimationRevertState As Byte
    RETURN_TO_LAST_ANIMATION = 0
    START_NEW_ANIMATION = 1
    PAUSE_ANIMATION = 2

End Enum