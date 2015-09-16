Public Class FireballLaunchAction
    Implements IAction

    Public Sub doAction(EM As EntityManagement, gt As Microsoft.Xna.Framework.GameTime, ms As Microsoft.Xna.Framework.Input.MouseState, ks As Microsoft.Xna.Framework.Input.KeyboardState, CM As Microsoft.Xna.Framework.Content.ContentManager, UI As UIOverlay, cam As IsoCamera, Optional CurrentPeriodicCountNumber As Integer = 0) Implements IAction.doAction
        If EM.CooldownList.IsCooldownExpired(CooldownName) Then

            EM.CooldownList.FireCooldown(CooldownName, False)
            Dim castCost = New Cost(0, 8, 0)

            If castCost.CanAfford(0, EM.LocalPlayerInfo.CurrentMana, EM.LocalPlayerInfo.CurrentHealth) Then
                EM.GenerateProjectile(EM.LocalPlayerInfo.ID, ProjectileTypes.FIREBALL_GENERIC, CM, EM.LocalPlayerInfo.Center(cam), cam.ScreenToMap(New Vector2(ms.X, ms.Y), 0), 0.05, New Size(64, 64), New AttackAction(), New AttackAction(), cam, True, Color.Blue, Color.Blue, ProjectilePathingTypes.ACCELERATED)
                Dim sh As SoundHelper = New SoundHelper(AssetManager.RequestAsset("castFireball", AssetTypes.SOUNDEFFECT), False, CM)
                sh.volume = 0.0
                sh.Play()
                EM.LocalPlayerInfo.TakeMana(CInt(castCost.ManaCost))
            Else
                Dim sh As SoundHelper = New SoundHelper(AssetManager.RequestAsset("castFireball", AssetTypes.SOUNDEFFECT), False, CM)
                sh.volume = 0.05
                sh.Play()
            End If
        End If
    End Sub
    Public ReadOnly Property CooldownName As String Implements IAction.CooldownName
        Get
            Return "FireballCast"
        End Get
    End Property
End Class
