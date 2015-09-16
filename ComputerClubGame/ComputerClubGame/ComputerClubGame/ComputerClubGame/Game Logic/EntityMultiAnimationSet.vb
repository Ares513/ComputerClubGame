Public Class EntityMultiAnimationSet
    Implements IAnimation
    Public Enum AnimSlots
        Head
        Body
        Weapon
    End Enum

    Public Sub New()
        ani = New Dictionary(Of AnimSlots, IAnimation)
    End Sub

    Public Sub New(HeadTexture As Texture2D, BodyTexture As Texture2D, WeaponTexture As Texture2D, size As Size, def As AnimationDefinition, origin As Vector2)
        MyClass.New()
        Dim defs(0) As AnimationDefinition
        defs(0) = def
        If Not IsNothing(HeadTexture) Then

            Animations(AnimSlots.Head) = New IsoAnimation(HeadTexture, size, defs, 0, New Rectangle(0, 0, 0, 0), origin)
            AddHandler Animations(AnimSlots.Head).AnimationDone, AddressOf AnimationDone
        End If
        If Not IsNothing(BodyTexture) Then
            Animations(AnimSlots.Body) = New IsoAnimation(BodyTexture, size, defs, 0, New Rectangle(0, 0, 0, 0), origin)
            AddHandler Animations(AnimSlots.Body).AnimationDone, AddressOf AnimationDone
        End If
        If Not IsNothing(WeaponTexture) Then
            Animations(AnimSlots.Weapon) = New IsoAnimation(WeaponTexture, size, defs, 0, New Rectangle(0, 0, 0, 0), origin)
            AddHandler Animations(AnimSlots.Weapon).AnimationDone, AddressOf AnimationDone
        End If
    End Sub

    Private ani As Dictionary(Of AnimSlots, IAnimation)
    Public Property Animations(key As AnimSlots) As IAnimation
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

    Private ReadOnly Property AnimationCollection As Dictionary(Of AnimSlots, IAnimation).ValueCollection
        Get
            Return ani.Values
        End Get
    End Property

    Public Sub Update(dt As GameTime) Implements IAnimation.Update
        For Each anim As IAnimation In AnimationCollection
            anim.Update(dt)
        Next
    End Sub

    Public Sub Draw(sb As SpriteBatch, loc As Vector2, layer As Single, Optional drawColor As Color = Nothing) Implements IAnimation.Draw
        For Each anim As IAnimation In AnimationCollection
            anim.Draw(sb, loc, layer)
        Next
    End Sub

    Public Sub SetAnimationSpeed(speed As Integer) Implements IAnimation.SetAnimationSpeed
        For Each anim As IAnimation In AnimationCollection
            anim.SetAnimationSpeed(speed)
        Next
    End Sub

    Public Sub SetFacing(facing As FacingTypes) Implements IAnimation.SetFacing
        For Each anim As IAnimation In AnimationCollection
            anim.SetFacing(facing)
        Next
    End Sub

    Public Sub Reset() Implements IAnimation.Reset
        For Each anim As IAnimation In AnimationCollection
            anim.Reset()
        Next
    End Sub
    Public Sub SetColor(Color As Color) Implements IAnimation.SetColor
        Animations(AnimSlots.Body).SetColor(Color)
        Animations(AnimSlots.Head).SetColor(Color)
        Animations(AnimSlots.Weapon).SetColor(Color)
    End Sub
    Public Sub SetVisible(isVisible As Boolean) Implements IAnimation.SetVisible
        Animations(AnimSlots.Body).SetVisible(isVisible)
        Animations(AnimSlots.Head).SetVisible(isVisible)
        Animations(AnimSlots.Weapon).SetVisible(isVisible)
    End Sub
    Public ReadOnly Property Color As Color Implements IAnimation.Color
        Get
            Return Animations(AnimSlots.Head).Color
        End Get
    End Property
    Public ReadOnly Property Visible As Boolean Implements IAnimation.Visible
        Get
            Return Animations(AnimSlots.Head).Visible
        End Get
    End Property
    Private Sub AnimationDone(sender As IAnimation, e As System.EventArgs)
        RaiseEvent AnimDone(Me, New EventArgs())
    End Sub
    Public Event AnimDone(sender As IAnimation, e As System.EventArgs) Implements IAnimation.AnimationDone

End Class

