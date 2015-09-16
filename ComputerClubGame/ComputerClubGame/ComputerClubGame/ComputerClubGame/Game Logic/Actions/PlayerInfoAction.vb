Public Class PlayerInfoAction
    Implements IAction

    Public ReadOnly Property CooldownName As String Implements IAction.CooldownName
        Get
            Return "Open PlayerInfo"
        End Get
    End Property

    Public Sub doAction(EM As EntityManagement, gt As Microsoft.Xna.Framework.GameTime, ms As Microsoft.Xna.Framework.Input.MouseState, ks As Microsoft.Xna.Framework.Input.KeyboardState, CM As Microsoft.Xna.Framework.Content.ContentManager, UI As UIOverlay, cam As IsoCamera, Optional CurrentPeriodicCountNumber As Integer = 0) Implements IAction.doAction
        If EM.CooldownList.IsCooldownExpired(CooldownName) Then
            UI.PlayerInfoWindow.ToggleOpening()
            EM.CooldownList.FireCooldown(CooldownName, False)
        End If

    End Sub
End Class
