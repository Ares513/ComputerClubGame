Public Class FireNovaPeriodicAction
    Implements IAction

    Public Sub doAction(EM As EntityManagement, gt As Microsoft.Xna.Framework.GameTime, ms As Microsoft.Xna.Framework.Input.MouseState, ks As Microsoft.Xna.Framework.Input.KeyboardState, CM As Microsoft.Xna.Framework.Content.ContentManager, UI As UIOverlay, cam As IsoCamera, Optional CurrentPeriodicCountNumber As Integer = 0) Implements IAction.doAction
        Dim translatedPos As Vector3 = EM.LocalPlayerInfo.Pos
        translatedPos.X += CSng(7 * Math.Cos(MathHelper.ToRadians(CurrentPeriodicCountNumber)))
        translatedPos.Y += CSng(7 * Math.Sin(MathHelper.ToRadians(CurrentPeriodicCountNumber)))
        EM.GenerateProjectile(EM.LocalPlayerInfo.ID, ProjectileTypes.FIREBALL_GENERIC, CM, EM.LocalPlayerInfo.Center(cam), translatedPos, 0.01, New Size(64, 64), New AttackAction(), New AttackAction(), cam, True, Color.White, Color.White)
    End Sub
    Public ReadOnly Property CooldownName As String Implements IAction.CooldownName
        Get
            Return "Fire Nova Periodic"
        End Get
    End Property
End Class
