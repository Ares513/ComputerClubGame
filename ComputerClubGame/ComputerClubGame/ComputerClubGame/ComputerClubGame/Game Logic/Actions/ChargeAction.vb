Public Class ChargeAction
    Inherits SpellAction

    Public Sub New()
        MyBase.New("ChargeAction")
        CostValue = New Cost(0, 10, 10)
    End Sub
    Public Overrides Sub PerformFinalActions(EM As EntityManagement, gt As Microsoft.Xna.Framework.GameTime, ms As Microsoft.Xna.Framework.Input.MouseState, ks As Microsoft.Xna.Framework.Input.KeyboardState, CM As Microsoft.Xna.Framework.Content.ContentManager, UI As UIOverlay, cam As IsoCamera, Optional CurrentPeriodicCountNumber As Integer = 0)
        EM.LocalPlayerInfo.AddModifier(1000, New ChargeModifier(New Cooldown(10000, 10000, False, "Charge"), 1.5))
        'Dim effectAnim As Animation = New Animation(CM.Load(Of Texture2D)(AssetManager.RequestAsset("KnightHead")), EM.LocalPlayerInfo.Size, EM.LocalPlayerInfo, 0, New Rectangle(0, 0, 0, 0))

        'effectAnim.Color = Color.Yellow
        'Dim VE As VisualEffect
        'VE = New VisualEffect(New Cooldown(10000, 10000, False, "ChargeVE"), effectAnim, EM.LocalPlayerInfo.Size, "ChargeVE")

        '   EM.LocalPlayerInfo.AddVisualEffect(1, VE)
    End Sub

    Public Overrides ReadOnly Property SpellCost As Cost
        Get
            Return CostValue
        End Get
    End Property
End Class
