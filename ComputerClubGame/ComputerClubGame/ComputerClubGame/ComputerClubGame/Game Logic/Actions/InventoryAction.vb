Public Class InventoryAction
    Implements IAction

    Public Sub doAction(EM As EntityManagement, gt As Microsoft.Xna.Framework.GameTime, ms As Microsoft.Xna.Framework.Input.MouseState, ks As Microsoft.Xna.Framework.Input.KeyboardState, CM As Microsoft.Xna.Framework.Content.ContentManager, UI As UIOverlay, cam As IsoCamera, Optional CurrentPeriodicCountNumber As Integer = 0) Implements IAction.doAction
        'Opens the player's inventory.
        If UI.PanelIndex = PanelTypes.GAME_OVERLAY Then
            UI.PlayerInventory.ToggleInventoryOpening()
        End If
    End Sub
    Public ReadOnly Property CooldownName As String Implements IAction.CooldownName
        Get
            Return "Open Inventory"
        End Get
    End Property
End Class
