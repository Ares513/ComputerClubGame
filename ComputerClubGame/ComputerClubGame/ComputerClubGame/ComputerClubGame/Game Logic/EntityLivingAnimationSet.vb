Public Class EntityLivingAnimationSet
    'Implements IAnimation
    Public Enum Animations
        Stand
        Walk
        Melee
        Ranged
        Spell
        Die
        Dead
    End Enum

    Private Facing As FacingTypes = FacingTypes.WEST
    Private ani As Dictionary(Of Animations, IAnimation)
    Public Property Anims(key As Animations) As IAnimation
        Get
            Dim ret As IAnimation = Nothing
            If ani.TryGetValue(key, ret) Then
                Return ret
            Else
                ani(key) = Nothing
                Return Nothing
            End If
        End Get
        Set(value As IAnimation)
            ani(key) = value
        End Set
    End Property

    Public Sub New()
        curAni = Animations.Stand
        ani = New Dictionary(Of Animations, IAnimation)
        'Anims(Animations.Stand) = Nothing
    End Sub

    Public Sub New(Stand As IAnimation)
        MyClass.New()
        Anims(Animations.Stand) = Stand
    End Sub

    Private curAni As Animations
    Public ReadOnly Property CurrentAnimation As Animations
        Get
            Return curAni
        End Get
    End Property

    Public Sub StartAnimation(type As Animations, length As TimeSpan)
        curAni = type
        Dim anim As IAnimation = CurIAnim
        If Not IsNothing(anim) Then
            anim.SetAnimationSpeed(CInt(length.TotalMilliseconds))
            anim.SetFacing(Facing)
            anim.Reset()
        End If
    End Sub

    Private ReadOnly Property CurIAnim As IAnimation
        Get
            Return Anims(curAni)
        End Get
    End Property

    Public Sub Update(dt As GameTime) ' Implements IAnimation.Update
        Dim anim As IAnimation = CurIAnim
        If Not IsNothing(anim) Then
            anim.Update(dt)
        End If
    End Sub
    ''' <summary>
    ''' Draws the current animation.
    ''' </summary>
    ''' <param name="sb">SpriteBatch instance./</param>
    ''' <param name="loc">The location, in screen space, that the image should be drawn.</param>
    ''' <param name="layer">The Layer the image should be drawn at. See Cam.ScaleLayer and Spritebatch.SpriteSortMode</param>
    ''' <param name="Color">Optional color parameter for drawing. B</param>
    ''' <remarks></remarks>
    Public Sub Draw(sb As SpriteBatch, loc As Vector2, layer As Single, Optional Color As Color = Nothing) ' Implements IAnimation.Draw
        Dim anim As IAnimation = CurIAnim
        If Not IsNothing(anim) Then
            anim.Draw(sb, loc, layer, Color)
        End If
    End Sub

    Public Sub SetFacing(facing As FacingTypes)
        Me.Facing = facing
        Dim anim As IAnimation = CurIAnim
        If Not IsNothing(anim) Then
            anim.SetFacing(facing)
        End If
    End Sub
End Class
