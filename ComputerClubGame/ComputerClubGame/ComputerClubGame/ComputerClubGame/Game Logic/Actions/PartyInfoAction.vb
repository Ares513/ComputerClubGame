Public Class PartyInfoAction
    Implements IAction

    Public Sub doAction(EM As EntityManagement, gt As Microsoft.Xna.Framework.GameTime, ms As Microsoft.Xna.Framework.Input.MouseState, ks As Microsoft.Xna.Framework.Input.KeyboardState, CM As Microsoft.Xna.Framework.Content.ContentManager, UI As UIOverlay, cam As IsoCamera, Optional CurrentPeriodicCountNumber As Integer = 0) Implements IAction.doAction
        If UI.PanelIndex = PanelTypes.GAME_OVERLAY And UI.PlayerSkillTree.LeftOrRight = False And UI.PlayerSkillTree.CurrentState = SkillTreeLocationTypes.FULLY_CLOSED Then

            'If the playerskilltree is on the left side of the screen
            '  If UI.PlayerSkillTree.OpenOrClosed = SlidingWindowState.FULLY_CLOSED Then
            'Only open the party info if the skillTree is all the way closed.
            If EM.CooldownList.IsCooldownExpired(CooldownName) Then
                UI.Party.ToggleOpening()
                EM.CooldownList.FireCooldown(CooldownName, False)
            End If

            'End If
        End If
    End Sub
    Public ReadOnly Property CooldownName As String Implements IAction.CooldownName
        Get
            Return "Open Party Info"
        End Get
    End Property
End Class
