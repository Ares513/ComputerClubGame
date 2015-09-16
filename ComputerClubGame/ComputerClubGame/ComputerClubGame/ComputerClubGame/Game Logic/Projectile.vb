
Public Class Projectile
    Inherits EntityLiving
    Protected reachTargetAction As IAction
    Protected projectileCollideAction As IAction
    Protected collisionDespawn As Boolean
    Protected useProjectileDeathAnim As Boolean
    Protected Shadows WithEvents deathAnimation As Animation
    Protected SourceID As String
    Protected initialMovementSpeed As Single
    Protected Speed As Vector2 'how far to move the projectile(x,y) each millisecond
    Protected Timer As Double 'this is the thyme left untill detonation
    Protected Postion As Vector2 ' Where in the world is carmen sandiago( in tiles)
    Protected blastRadius As Single = 2.0
    Protected MaxTargets As Integer = -1
    Protected dmg As Damage = New Damage(7, 10)
    Protected doCollideInMotion As Boolean
    Protected initialHeight As Single
    Protected doCollideInMotionRaidus As Single = 0.5
    Protected WithEvents DeathAnimationTimer As Timers.Timer
    ''' <summary>
    ''' Indicates that the animation for a detonating projectile has begun
    ''' </summary>
    ''' <param name="sender">Projectile instance</param>
    ''' <param name="e">EventArgs instance</param>
    ''' <param name="blastRadius">The distance from the detonation location that enemies should be damaged</param>
    ''' <param name="maxTargets">The maximum number of targets that should be hit by it.</param>
    ''' <param name="dmg">Damage instance indicating how much damage it will do.</param>
    ''' <remarks></remarks>
    Public Event ProjectileDetonated(sender As Projectile, e As System.EventArgs, blastRadius As Single, maxTargets As Integer, dmg As Damage)
    ''' <summary>
    ''' Returns the creator (entity that launched the projectile.)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Source As String
        Get
            Return SourceID
        End Get
    End Property
    Public Overrides Property Name As String
        Get
            Return ID
        End Get
        Set(value As String)

        End Set
    End Property
    Public ReadOnly Property ActivateOnCollide As Boolean
        Get
            Return collisionDespawn
        End Get
    End Property
    Public ReadOnly Property hasDetonated As Boolean
        Get
            Return Detonated
        End Get
    End Property
    Public ReadOnly Property Completed As Boolean
        Get
            Return deathAnimationDone
        End Get
    End Property
    Public Overrides ReadOnly Property Visible As Boolean
        Get
            Return Completed
        End Get
    End Property
    Protected Target As Vector3
    Protected Detonated As Boolean
    Protected deathAnimationDone As Boolean
    Private Loops As Double 'The number of movement loops.
    Private LoopsOccured As Integer
    Private Pathing As ProjectilePathingTypes
    Private DeathDuration As Single
    Private doAllowJumpHeight As Boolean
    Public Sub New(SourceID As String, ID As String, inReachTargetAction As IAction, inProjectileCollideAction As IAction, projectileSize As Size, startTilePosition As Vector3, targetTilePosition As Vector3, projectileSpeedTilesPerSecond As Single, projectileAnim As IsoAnimation, projectileDeathAnimation As IsoAnimation, Cam As IsoCamera, em As EntityManagement, Optional despawnOnCollision As Boolean = False, Optional PathingType As ProjectilePathingTypes = ProjectilePathingTypes.LINEAR, Optional detonationRadius As Single = 2.0, Optional targetLimit As Integer = -1, Optional CollideWhileInMotion As Boolean = True, Optional MotionCollideRadius As Single = 1, Optional DetonationTime As Single = 0.5, Optional doAllowJumpHeight As Boolean = True)
        MyBase.New(ID, projectileSize, 0.0001, em)
        Me.SourceID = SourceID
        useProjectileDeathAnim = True
        'reachTargetAction = inReachTargetAction
        projectileCollideAction = inProjectileCollideAction
        animation = New EntityLivingAnimationSet()
        animation.Anims(EntityLivingAnimationSet.Animations.Stand) = projectileAnim
        animation.Anims(EntityLivingAnimationSet.Animations.Die) = projectileDeathAnimation
        'defaultAnimation = projectileAnim
        collisionDespawn = despawnOnCollision
        MovementSpeed = projectileSpeedTilesPerSecond
        initialMovementSpeed = MovementSpeed
        Center(Cam) = startTilePosition
        'Loops = UpdateOrders(targetTilePosition, Cam) 'We will count down the number of loops.
        'defaultAnimation.SetAnimation("Move", False)
        'If IsNothing(projectileDeathAnimation) Then
        '    'Essentially a nullable parameter.
        '    useProjectileDeathAnim = False
        'Else
        '    deathAnimation = projectileDeathAnimation
        '    useProjectileDeathAnim = True

        'End If
        Target = targetTilePosition
        Dim start As Vector3
        start = startTilePosition
        Pathing = PathingType
        Me.StartDetonationTimer(OffsetCenter, Target, 1)
        Loops = UpdateOrders(Target, Cam)
        doCollideInMotion = CollideWhileInMotion
        doCollideInMotionRaidus = MotionCollideRadius
        DeathDuration = DetonationTime
        initialHeight = startTilePosition.Z
        Me.doAllowJumpHeight = doAllowJumpHeight
    End Sub
    ''' <summary>
    ''' Prematurely detonates the projectile.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Impact()
        Die()
        If useProjectileDeathAnim Then
            'deathAnimation.SetAnimation("Death", False)

        End If
        animation.StartAnimation(EntityLivingAnimationSet.Animations.Die, New TimeSpan(0, 0, CInt(DeathDuration)))
        DeathAnimationTimer = New Timers.Timer(DeathDuration)
        DeathAnimationTimer.Start()
        Detonated = True
        RaiseEvent ProjectileDetonated(Me, New EventArgs, blastRadius, MaxTargets, dmg)
    End Sub

    Public Sub Move(Cam As IsoCamera, gt As GameTime)
        'Update the height.
        Dim currentHeight As Single = map.GetHeight(Pos.X, Pos.Y)
        If doAllowJumpHeight Then
            Pos = New Vector3(Pos.X, Pos.Y, currentHeight)

        ElseIf currentHeight > initialHeight Or currentHeight < initialHeight Then
            'Not allowed to jump height, and the currentHeight is different from the initial height
            'Impact
            Impact()
        End If
        Dim ElapsedSinceLastLoop As Integer = gt.ElapsedGameTime.Milliseconds
        If Pathing = ProjectilePathingTypes.LINEAR Then
            'Do nothing.
        ElseIf Pathing = ProjectilePathingTypes.HOMING Then
            DebugManagement.WriteLineToLog("Homing Projectiles Unimplemented", SeverityLevel.WARNING)
            'Loops = MyBase.UpdateOrders(Target, Cam)
            'Needs to be worked on.
        ElseIf Pathing = ProjectilePathingTypes.ACCELERATED Then
            DebugManagement.WriteLineToLog("Accelerated Projectiles Unimplemented", SeverityLevel.WARNING)
            'MovementSpeed = initialMovementSpeed + LoopsOccured * initialMovementSpeed
            'MyBase.UpdateOrders(New Vector3(Target.X + CSng(Math.Sin(MathHelper.ToRadians(LoopsOccured))), Target.Y, Target.Z), Cam)
            'REM assigned to Noah Kerns- Complex Projectile Pathing Engines
        End If
        'UpdatePosition(CDbl(gt.ElapsedGameTime.Milliseconds))
        'Timer = Timer - (ElapsedSinceLastLoop / 10)
        'If Timer <= 0 Then
        '    Impact()
        'End If
        CheckImpact(Cam, gt)
        If Loops > gt.ElapsedGameTime.Milliseconds Then
            Loops -= gt.ElapsedGameTime.Milliseconds
            lastPosition = Pos
            Pos += Vel * gt.ElapsedGameTime.Milliseconds
        Else
            lastPosition = Pos
            Pos += Vel * CSng(Loops / gt.ElapsedGameTime.Milliseconds)
            Loops = 0
            Impact()
        End If
    End Sub
    Private lastPosition As Vector3
    Private Sub CheckImpact(cam As IsoCamera, gt As GameTime)
        If cam.MapToScreen(Pos).Equals(cam.MapToScreen(lastPosition)) Then
            'Hasn't moved!
            Impact()
        End If

    End Sub
    Public Sub CheckCollision(HostileEntities() As EntityLiving, Cam As IsoCamera)
        'For Each ent In HostileEntities
        '    If Rectangle.Intersect(New Rectangle(CInt(ent.OffsetPosition.X), CInt(ent.OffsetPosition.X), CInt(ent.TileHitBox.X), CInt(ent.TileHitBox.Y)), New Rectangle(CInt(OffsetCenter.X), CInt(OffsetCenter.Y), CInt(ent.SizeInTiles(Cam).Width), CInt(ent.SizeInTiles(Cam).Height))).IsEmpty = False Then
        '        'OK. There's a rectangle that intersects these two objects and it isn't completely empty.
        '        Impact()

        '    End If
        'Next
        For Each ent In HostileEntities
            If doCollideInMotion Then
                If Not Detonated Then
                    If Vector3.Distance(OffsetCenter, ent.OffsetCenter) <= doCollideInMotionRaidus Then
                        'Looks like we collided! W00t!
                        Impact()

                    End If
                End If
            End If
        Next

    End Sub
    Private Sub OnDeathAnimationDone() Handles DeathAnimationTimer.Elapsed
        DeathAnimationTimer.Dispose()
        animation.StartAnimation(EntityLivingAnimationSet.Animations.Dead, TimeSpan.MaxValue)

    End Sub
    Public Shadows Sub Update(gt As GameTime, cam As IsoCamera)
        If Detonated Then


        Else

            Move(cam, gt)
            CheckImpact(cam, gt)
            animation.Update(gt)
        End If
        ' SetFacing(DegreesToFacing(KernsMath.AngleConvert360(CSng(Math.Atan2(Vector3.Subtract(Target, Center(cam)).X, Vector3.Subtract(Target, Center(cam)).Y)))))


    End Sub

    Public Shadows Sub Draw(sb As SpriteBatch, cam As IsoCamera)
        Dim v2pos As Vector2
        Dim cen As Vector3 = Center(cam)
        v2pos = cam.MapToScreen(cen)
        Dim layer As Single = cam.ScaleLayer(cen.X + cen.Y + cen.Z)

        animation.Draw(sb, v2pos, layer)

    End Sub
    Public Shadows Function UpdateOrders(targetPoint As Vector3, Cam As IsoCamera) As Double
        velocity = Vector3.Subtract(OffsetCenter, Target)
        velocity.Normalize()
        velocity *= MovementSpeed
        SetFacing(DegreesToFacing(KernsMath.AngleBetweenTwoPoints(OffsetCenter, Target)))
        'Loops = KernsMath.GetDistance(New Vector2(Center(Cam).X, Center(Cam).Y), New Vector2(targetPoint.X, targetPoint.Y)) '/ MovementSpeed
        Dim distance As Double = Vector3.Distance(OffsetCenter, Target)

        Loops = distance / MovementSpeed

        Return Loops
    End Function
    Public Sub StartDetonationTimer(MousePos As Vector3, Playerpos As Vector3, ProjectileVel As Double) 'do this to shoop da woop at the mouse Pos; Projectile Velocity is in tiles/millisecond 
        Dim distance As Double
        distance = Vector3.Distance(MousePos, Playerpos)
        Dim angle As Double
        angle = Math.Atan2(MousePos.X - Playerpos.X, MousePos.X - Playerpos.X)
        If MousePos.X - Playerpos.X < 0 And MousePos.Y - Playerpos.Y < 0 Or MousePos.X - Playerpos.X > 0 And MousePos.Y - Playerpos.Y < 0 Then
            angle = Math.PI + Math.Atan2(Math.Abs(MousePos.X - Playerpos.X), Math.Abs(MousePos.X - Playerpos.X))
        End If
        velocity.X = CSng((distance / ProjectileVel) * Math.Cos(angle))
        velocity.Y = CSng((distance / ProjectileVel) * Math.Sin(angle))
        Timer = distance / ProjectileVel

    End Sub
    Public Sub UpdatePosition(ElapsedThyme As Double) 'Do this each game loop; elapsed thyme is in milliseconds
        Timer = Timer - ElapsedThyme
        position.X = CSng(position.X + (ElapsedThyme * velocity.X))
        position.Y = CSng(position.Y + (ElapsedThyme * velocity.Y))

        If Timer <= 0 Then
            Impact()
        End If
    End Sub
End Class
Public Enum ProjectilePathingTypes As Byte
    LINEAR
    HOMING
    ACCELERATED
End Enum
