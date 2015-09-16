Public Class SearchArea
    Implements IEffect
    Dim Paused As Boolean = False
    Public Property isUpdating As Boolean Implements IEffect.isUpdating
        Get
            Return Paused
        End Get
        Set(value As Boolean)
            Paused = value
        End Set
    End Property
    Public Sub New(searchRadius As Single)

    End Sub
    Public Sub Update(EM As EntityManagement, gt As Microsoft.Xna.Framework.GameTime, ms As Microsoft.Xna.Framework.Input.MouseState, ks As Microsoft.Xna.Framework.Input.KeyboardState, CM As Microsoft.Xna.Framework.Content.ContentManager, UI As UIOverlay, cam As IsoCamera) Implements IEffect.Update

    End Sub
    'Incomplete. Will be worked on once we have entities. Otherwise, it's a little pointless.
End Class
