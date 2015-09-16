''' <summary>
''' Abstract class that partially implements Sub DoAction so that the basics of spellcasting are covered.
''' The spell is only cast if it is off cooldown, the player is alive and they have enough mana.
''' Instead of overriding DoAction, they override PerformFinalActions.
''' </summary>
''' <remarks></remarks>
Public MustInherit Class SpellAction
    Implements IAction
    Dim CDName As String
    ''' <summary>
    ''' The cost to cast the spell. You may want to update this each time PerformFinalActions is called, in case a change of skill level would alter mana costs.
    ''' </summary>
    ''' <remarks></remarks>
    Protected CostValue As Cost
    Public Sub New(Name As String)
        CDName = Name
    End Sub
    Public ReadOnly Property CooldownName As String Implements IAction.CooldownName
        Get
            Return CDName
        End Get
    End Property
    Public MustOverride ReadOnly Property SpellCost As Cost

    Public Sub doAction(EM As EntityManagement, gt As Microsoft.Xna.Framework.GameTime, ms As Microsoft.Xna.Framework.Input.MouseState, ks As Microsoft.Xna.Framework.Input.KeyboardState, CM As Microsoft.Xna.Framework.Content.ContentManager, UI As UIOverlay, cam As IsoCamera, Optional CurrentPeriodicCountNumber As Integer = 0) Implements IAction.doAction
        If EM.LocalPlayerInfo.Alive Then
            If EM.CooldownList.IsCooldownExpired(CDName) Then
                If SpellCost.CanAfford(EM.LocalPlayerInfo.InitialPlayerData.Gold, EM.LocalPlayerInfo.CurrentMana, EM.LocalPlayerInfo.CurrentHealth) Then
                    EM.CooldownList.FireCooldown(CDName)

                    PerformFinalActions(EM, gt, ms, ks, CM, UI, cam, CurrentPeriodicCountNumber)
                End If
            End If
        End If
    End Sub
    MustOverride Sub PerformFinalActions(EM As EntityManagement, gt As Microsoft.Xna.Framework.GameTime, ms As Microsoft.Xna.Framework.Input.MouseState, ks As Microsoft.Xna.Framework.Input.KeyboardState, CM As Microsoft.Xna.Framework.Content.ContentManager, UI As UIOverlay, cam As IsoCamera, Optional CurrentPeriodicCountNumber As Integer = 0)

End Class
