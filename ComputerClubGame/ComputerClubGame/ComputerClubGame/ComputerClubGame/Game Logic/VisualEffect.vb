''' <summary>
''' Simple class for attaching visual effects to the player.
''' </summary>
''' <remarks></remarks>
Public Class VisualEffect
    Public gt As GameTime
    Public WithEvents defaultAnim As Animation
    Private invalidValues As Boolean
    Public CD As Cooldown
    Public Size As Size
    Private EffectIdentifier As String
    Private OnlyRunOnce As Boolean
    Private Completed As Boolean
    Public OffsetValueVector3 As Vector3
    Public ReadOnly Property ID As String
        Get
            Return EffectIdentifier
        End Get
    End Property
    Public Sub New(CooldownValue As Cooldown, anim As Animation, drawSize As Size, Optional Identifier As String = "", Optional RunAnimationOnce As Boolean = True)
        Size = drawSize
        If IsNothing(anim) Then
            DebugManagement.WriteLineToLog("Someone passed an empty animation to VisualEffect. The VisualEffect will be ignored.", SeverityLevel.CRITICAL)
            invalidValues = True
        End If
        If IsNothing(CooldownValue) Then
            DebugManagement.WriteLineToLog("Someone passed an empty cooldown to VisualEffect. The VisualEffect will be ignored.", SeverityLevel.CRITICAL)
            invalidValues = True

        End If
        defaultAnim = anim
        CD = CooldownValue
        If Identifier <> "" Then
            EffectIdentifier = Identifier
        Else
            EffectIdentifier = CooldownValue.ID
        End If
        OnlyRunOnce = RunAnimationOnce
        If RunAnimationOnce Then
            defaultAnim.PlayAnimationOnce(defaultAnim.CurrentAnimationName, False, AnimationRevertState.PAUSE_ANIMATION)
        End If
    End Sub


    Public Sub Update(gt As GameTime)
        If invalidValues Then
            Exit Sub
        End If
        defaultAnim.Update(gt)
        CD.Update(gt)
    End Sub
    Public Overloads Sub Draw(sb As SpriteBatch, targetPosition As Vector3, Cam As IsoCamera)
        If invalidValues Then
            Exit Sub
        End If
        Dim screenPos As Vector2 = Cam.MapToScreen(targetPosition + OffsetValueVector3)
        If Not Completed Then
            screenPos.X -= CSng(Size.Width / 2)
            screenPos.Y -= CSng(Size.Height / 2)
            defaultAnim.Draw(sb, New Rectangle(CInt(screenPos.X), CInt(screenPos.Y), Size.Width, Size.Height), Cam.ScaleLayer(targetPosition.X + targetPosition.Y + targetPosition.Z + 1))
        End If
    End Sub
    Private Sub AnimDone() Handles defaultAnim.AnimDone
        If OnlyRunOnce Then
            Completed = True
        End If
    End Sub
End Class
