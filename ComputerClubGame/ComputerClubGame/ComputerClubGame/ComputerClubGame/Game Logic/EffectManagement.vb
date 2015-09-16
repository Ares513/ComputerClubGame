Public Class EffectManagement
    Private Effects As List(Of IEffect)
    Public Sub New()
        Effects = New List(Of IEffect)
    End Sub
    Public Sub Update(EM As EntityManagement, gt As Microsoft.Xna.Framework.GameTime, ms As Microsoft.Xna.Framework.Input.MouseState, ks As Microsoft.Xna.Framework.Input.KeyboardState, CM As Microsoft.Xna.Framework.Content.ContentManager, UI As UIOverlay, cam As IsoCamera)
        For Each Effect In Effects
            If Effect.isUpdating Then
                Effect.Update(EM, gt, ms, ks, CM, UI, cam)
            End If
        Next
    End Sub
    Public Sub AddEffect(inEffect As IEffect)
        Effects.Add(inEffect)
    End Sub
End Class
