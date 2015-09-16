Public Class SkillTreeAction
    Implements IAction

    Public Sub doAction(EM As EntityManagement, gt As Microsoft.Xna.Framework.GameTime, ms As Microsoft.Xna.Framework.Input.MouseState, ks As Microsoft.Xna.Framework.Input.KeyboardState, CM As Microsoft.Xna.Framework.Content.ContentManager, UI As UIOverlay, cam As IsoCamera, Optional CurrentPeriodicCountNumber As Integer = 0) Implements IAction.doAction
        If UI.PanelIndex = PanelTypes.GAME_OVERLAY And EM.CooldownList.IsCooldownExpired("Open Skill Tree") And UI.Party.CurrentState = SkillTreeLocationTypes.FULLY_CLOSED Then
            UI.PlayerSkillTree.ToggleSkillTreeOpening()
            EM.CooldownList.FireCooldown(CooldownName, True)
        End If
    End Sub
    Public ReadOnly Property CooldownName As String Implements IAction.CooldownName
        Get
            Return "Open Skill Tree"
        End Get
    End Property
End Class
