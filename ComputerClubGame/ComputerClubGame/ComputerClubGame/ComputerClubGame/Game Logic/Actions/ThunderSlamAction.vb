Public Class ThunderSlamAction
    Inherits SpellAction
    Public Sub New()
        MyBase.New("ThunderSlam")
    End Sub
    Public Overrides Sub PerformFinalActions(EM As EntityManagement, gt As Microsoft.Xna.Framework.GameTime, ms As Microsoft.Xna.Framework.Input.MouseState, ks As Microsoft.Xna.Framework.Input.KeyboardState, CM As Microsoft.Xna.Framework.Content.ContentManager, UI As UIOverlay, cam As IsoCamera, Optional CurrentPeriodicCountNumber As Integer = 0)
        Dim VEAnim As Animation = New Animation(CM.Load(Of Texture2D)(AssetManager.RequestAsset("ThunderSlam")), New Size(128, 128), New AnimationDefinition() {New AnimationDefinition(0, 8, "Birth", 10)}, 0, New Rectangle(0, 0, 0, 0))
        VEAnim.SetAnimationSpeed(2500, "Birth")
        Dim VE As VisualEffect
        EM.CooldownList.FireCooldown("ThunderSlam", False)
        Dim VELeft As VisualEffect
        Dim VETop As VisualEffect
        Dim VERight As VisualEffect
        Dim VEBottom As VisualEffect

        VE = New VisualEffect(New Cooldown(10000, 10000, False, "ThunderSlamVE"), VEAnim, New Size(1024, 1024), "ThunderSlam")
        VELeft = New VisualEffect(New Cooldown(10000, 10000, False, "ThunderSlamVE"), VEAnim, New Size(1024, 1024), "ThunderSlam")
        VETop = New VisualEffect(New Cooldown(10000, 10000, False, "ThunderSlamVE"), VEAnim, New Size(1024, 1024), "ThunderSlam")
        VERight = New VisualEffect(New Cooldown(10000, 10000, False, "ThunderSlamVE"), VEAnim, New Size(1024, 1024), "ThunderSlam")
        VEBottom = New VisualEffect(New Cooldown(10000, 10000, False, "ThunderSlamVE"), VEAnim, New Size(1024, 1024), "ThunderSlam")
        VELeft.OffsetValueVector3 = New Vector3(-3, 0, 0)
        VERight.OffsetValueVector3 = New Vector3(3, 0, 0)
        VETop.OffsetValueVector3 = New Vector3(0, 3, 0)
        VEBottom.OffsetValueVector3 = New Vector3(0, -3, 0)
        EM.LocalPlayerInfo.AddVisualEffect(1, VE)
        EM.LocalPlayerInfo.AddVisualEffect(1, VELeft)
        EM.LocalPlayerInfo.AddVisualEffect(1, VERight)
        EM.LocalPlayerInfo.AddVisualEffect(1, VETop)
        EM.LocalPlayerInfo.AddVisualEffect(1, VEBottom)

        'EM.LocalPlayerInfo.PlayAnimationOnce("Cast Spell", False, AnimationRevertState.RETURN_TO_LAST_ANIMATION)
    End Sub
    Public Overrides ReadOnly Property SpellCost As Cost
        Get
            Return New Cost(0, 100, 10)
        End Get
    End Property

End Class
