Public Class RespawnAction
    Implements IAction

    Public Sub doAction(EM As EntityManagement, gt As Microsoft.Xna.Framework.GameTime, ms As Microsoft.Xna.Framework.Input.MouseState, ks As Microsoft.Xna.Framework.Input.KeyboardState, CM As Microsoft.Xna.Framework.Content.ContentManager, UI As UIOverlay, cam As IsoCamera, Optional CurrentPeriodicCountNumber As Integer = 0) Implements IAction.doAction
        If Not EM.LocalPlayerInfo.Alive Then
            EM.LocalPlayerInfo.Alive = True
            'Take gold here if needed- move the player back to the starting location which will be stored in the map management or something
            'Any further required implementation for respawning will be here.
            EM.LocalPlayerInfo.Pos = New Vector3(0, 0, 0)

            'EM.LocalPlayerInfo.UnpauseAnims()
            'EM.LocalPlayerInfo.SetAnimations("Stand", False)
            'EM.LocalPlayerInfo.EnsureAnimationsMatch()
            EM.LocalPlayerInfo.AIAction = New EntityActionStand(EM.LocalPlayerInfo)

            EM.LocalPlayerInfo.CurrentHealth = EM.LocalPlayerInfo.MaxHealth
            EM.LocalPlayerInfo.CurrentMana = EM.LocalPlayerInfo.MaxMana



        Else

            'Later, bring up the menu for exiting the lobby. etc.

        End If
    End Sub
    Public ReadOnly Property CooldownName As String Implements IAction.CooldownName
        Get
            Return "Respawn"
        End Get
    End Property
End Class
