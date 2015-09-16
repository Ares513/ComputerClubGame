''' <summary>
''' Generates projectiles, the easy way.
''' </summary>
''' <remarks></remarks>
Public Class ProjectileFactory
    Public Sub New()

    End Sub
    Public Function MakeProjectile(SourceID As String, ID As String, Type As ProjectileTypes, CM As ContentManager, startPosition As Vector3, targetPosition As Vector3, projectileSpeed As Single, projectileSize As Size, ImpactAction As IAction, DetonationAction As IAction, Cam As IsoCamera, despawnOnCollision As Boolean, projectileColor As Color, deathAnimationColor As Color, em As EntityManagement, Optional PathingType As ProjectilePathingTypes = ProjectilePathingTypes.LINEAR) As Projectile
        Dim output As Projectile
        Select Case Type
            Case ProjectileTypes.BLANK_PROJECTILE
                Return Nothing
            Case ProjectileTypes.FIREBALL_GENERIC

                Dim newAnimdefs(0) As AnimationDefinition
                newAnimdefs(0) = New AnimationDefinition(0, 7, "Walk", 36)
                Dim lifeAnim As IsoAnimation = New IsoAnimation(CM.Load(Of Texture2D)(AssetManager.RequestAsset("fireballProjectile")), New Size(64, 64), newAnimdefs, 0, New Rectangle(0, 0, 64, 64), New Vector2(32, 32))
                Dim deathAnimdefs(0) As AnimationDefinition
                deathAnimdefs(0) = New AnimationDefinition(0, 5, "Death", 36)
                Dim deathAnim As IsoAnimation
                deathAnim = New IsoAnimation(CM.Load(Of Texture2D)(AssetManager.RequestAsset("explosion")), New Size(256, 128), deathAnimdefs, 0, New Rectangle(0, 0, 128, 128), New Vector2(128, 64))
                lifeAnim.SetColor(projectileColor)
                deathAnim.SetColor(deathAnimationColor)
                output = New Projectile(SourceID, ID, DetonationAction, ImpactAction, New Size(64, 64), startPosition + New Vector3(0, 0, 1), targetPosition + New Vector3(0, 0, 1), projectileSpeed, lifeAnim, deathAnim, Cam, em, despawnOnCollision, PathingType)
                Return output
        End Select
        Return Nothing
    End Function
End Class
Public Enum ProjectileTypes As Integer
    BLANK_PROJECTILE
    FIREBALL_GENERIC
End Enum