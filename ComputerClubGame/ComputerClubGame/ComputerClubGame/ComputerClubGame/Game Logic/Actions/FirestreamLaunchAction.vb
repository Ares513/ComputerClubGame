Public Class FirestreamLaunchAction
    Implements IAction

    Public Sub doAction(EM As EntityManagement, gt As Microsoft.Xna.Framework.GameTime, ms As Microsoft.Xna.Framework.Input.MouseState, ks As Microsoft.Xna.Framework.Input.KeyboardState, CM As Microsoft.Xna.Framework.Content.ContentManager, UI As UIOverlay, cam As IsoCamera, Optional CurrentPeriodicCountNumber As Integer = 0) Implements IAction.doAction
        EM.ActiveEffects.AddEffect(New PeriodicEffect(New BlankAction(), New FireballLaunchAction(), 15, 1000))

    End Sub
    Public ReadOnly Property CooldownName As String Implements IAction.CooldownName
        Get
            Return "Firestream Launch"
        End Get
    End Property
End Class
