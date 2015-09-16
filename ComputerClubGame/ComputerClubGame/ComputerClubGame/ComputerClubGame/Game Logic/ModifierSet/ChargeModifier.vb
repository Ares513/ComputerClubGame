Public Class ChargeModifier
    Implements IModifierSet
    Private MovementSpeedMult As Single
    Private CooldownValue As Cooldown
    Private Fired As Boolean
    Private oldColor As Color
    Public Sub New(CD As Cooldown, MovementSpeedMult As Single)
        Me.MovementSpeedMult = MovementSpeedMult
        CooldownValue = CD
    End Sub
    Public Sub ApplyModifiers(ByRef Modified As EntityLiving) Implements IModifierSet.ApplyModifiers
        If Not HasAppliedModifiers Then
            Modified.MoveSpeed = (Modified.MoveSpeed * MovementSpeedMult)
            Fired = True
            oldColor = Modified.animation.Anims(Modified.animation.CurrentAnimation).Color
            Modified.animation.Anims(Modified.animation.CurrentAnimation).SetColor(Color.Yellow)

        End If
    End Sub

    Public ReadOnly Property HasAppliedModifiers As Boolean Implements IModifierSet.HasAppliedModifiers
        Get
            Return Fired
        End Get
    End Property

    Public ReadOnly Property RemainingDuration As Integer Implements IModifierSet.RemainingDuration
        Get
            Return CooldownValue.Remaining
        End Get
    End Property
    Public ReadOnly Property CD As Cooldown Implements IModifierSet.CD
        Get
            Return CooldownValue
        End Get
    End Property
    Public ReadOnly Property ModifierIdentifier As String Implements IModifierSet.ModifierIdentifier
        Get
            Return "Charge"
        End Get
    End Property
    Public Sub UnApplyModifiers(ByRef Modified As EntityLiving) Implements IModifierSet.UnApplyModifiers
        If HasAppliedModifiers Then
            Modified.MoveSpeed = Modified.MoveSpeed / MovementSpeedMult
            Fired = False
            Modified.animation.Anims(Modified.animation.CurrentAnimation).SetColor(oldColor)
        End If
    End Sub

    Public Function Update(gt As Microsoft.Xna.Framework.GameTime) As Integer Implements IModifierSet.Update
        CooldownValue.Update(gt)
        Return CooldownValue.Remaining
    End Function

End Class
