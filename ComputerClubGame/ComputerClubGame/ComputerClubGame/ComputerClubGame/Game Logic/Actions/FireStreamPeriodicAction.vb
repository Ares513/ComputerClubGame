Public Class FireStreamPeriodicAction
    Implements IAction

    Public Sub doAction(EM As EntityManagement, gt As Microsoft.Xna.Framework.GameTime, ms As Microsoft.Xna.Framework.Input.MouseState, ks As Microsoft.Xna.Framework.Input.KeyboardState, CM As Microsoft.Xna.Framework.Content.ContentManager, UI As UIOverlay, cam As IsoCamera, Optional CurrentPeriodicCountNumber As Integer = 0) Implements IAction.doAction
        EM.GenerateProjectile(EM.LocalPlayerInfo.ID, ProjectileTypes.FIREBALL_GENERIC, CM, EM.LocalPlayerInfo.Center(cam), cam.ScreenToMap(New Vector2(ms.X, ms.Y), 0), 0.1, New Size(64, 64), New AttackAction(), New AttackAction(), cam, True, Color.White, Color.White)
    End Sub
    Public ReadOnly Property CooldownName As String Implements IAction.CooldownName
        Get
            Return "FirestreamLaunch"
        End Get
    End Property
End Class
