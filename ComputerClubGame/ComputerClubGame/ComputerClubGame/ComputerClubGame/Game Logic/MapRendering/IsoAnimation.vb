Public Class IsoAnimation
    Inherits Animation

    Implements IAnimation
    Public Shadows Event AnimDone(sender As IAnimation, e As EventArgs) Implements IAnimation.AnimationDone
    Private origin As Vector2

    Public Sub New(baseTexture As Texture2D, sizeOfEachFrame As Size, inputAnimationDefinitions As AnimationDefinition(),
                   initialAnimationIndex As Short, initialCollisionBox As Rectangle, originLoc As Vector2)
        MyBase.New(baseTexture, sizeOfEachFrame, inputAnimationDefinitions, initialAnimationIndex, initialCollisionBox)
        origin = originLoc
    End Sub

    Public Overloads Sub Update(dt As GameTime) Implements IAnimation.Update
        MyBase.Update(dt)
    End Sub
    Private Sub AnimationDone(def As AnimationDefinition, currentFrame As Short) Handles MyBase.AnimDone
        RaiseEvent AnimDone(Me, New EventArgs())
    End Sub
    Public Overloads Sub Draw(sb As SpriteBatch, loc As Vector2, layerDepth As Single, Optional overrideColor As Color = Nothing) Implements IAnimation.Draw
        Dim zero As Vector2 = loc - origin
        MyBase.Draw(sb, New Rectangle(CInt(zero.X), CInt(zero.Y), Size.Width, Size.Height), layerDepth, overrideColor)
    End Sub

    Public Overloads Sub Draw(sb As SpriteBatch, cam As IsoCamera, loc As Vector3)
        Draw(sb, cam.MapToScreen(loc), cam.ScaleLayer(loc.X + loc.Y + loc.Z))
    End Sub

    Public Overloads Sub SetAnimationSpeed(speed As Integer) Implements IAnimation.SetAnimationSpeed
        MyBase.SetAnimationSpeed(speed, CurrentAnimationName)
    End Sub

    Public Overloads Sub SetFacing(facing As FacingTypes) Implements IAnimation.SetFacing
        MyBase.SetFacing(facing)
    End Sub

    Public Sub Reset() Implements IAnimation.Reset
        MyBase.ResetCurrentAnimation()
    End Sub
    Public Sub SetColor(Color As Color) Implements IAnimation.SetColor
        MyBase.Color = Color
    End Sub
    Public Sub SetVisible(value As Boolean) Implements IAnimation.SetVisible
        MyBase.Visible = value
    End Sub
    Public Overloads ReadOnly Property Visible As Boolean Implements IAnimation.Visible
        Get
            Return MyBase.Visible
        End Get
    End Property
    Public Overloads ReadOnly Property Color As Color Implements IAnimation.Color
        Get
            Return MyBase.Color
        End Get
    End Property
End Class