Public Interface IAnimation
    Sub Update(dt As GameTime)
    Sub Draw(sb As SpriteBatch, loc As Vector2, layer As Single, Optional drawColor As Color = Nothing)
    Sub SetAnimationSpeed(speed As Integer)
    Sub SetFacing(facing As FacingTypes)
    Sub SetColor(Color As Color)
    Sub SetVisible(isVisible As Boolean)
    Event AnimationDone(sender As IAnimation, e As System.EventArgs)
    ReadOnly Property Visible As Boolean
    ReadOnly Property Color As Color
    Sub Reset()
End Interface

