Public Class FireNovaLaunchAction
    Implements IAction

    Public Sub doAction(EM As EntityManagement, gt As Microsoft.Xna.Framework.GameTime, ms As Microsoft.Xna.Framework.Input.MouseState, ks As Microsoft.Xna.Framework.Input.KeyboardState, CM As Microsoft.Xna.Framework.Content.ContentManager, UI As UIOverlay, cam As IsoCamera, Optional CurrentPeriodicCountNumber As Integer = 0) Implements IAction.doAction
        If EM.CooldownList.IsCooldownExpired(CooldownName) Then
            Dim PE As New PeriodicEffect(New BlankAction(), New FireNovaPeriodicAction(), 90, 0, 4, True)
            EM.ActiveEffects.AddEffect(PE)
            EM.CooldownList.FireCooldown(CooldownName)

        End If
    End Sub
    Public ReadOnly Property CooldownName As String Implements IAction.CooldownName
        Get
            Return "FireNovaCast"
        End Get
    End Property
End Class