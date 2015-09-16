Public Class Mob
    Inherits EntityLiving
    Public ReadOnly AcquisitionRange As Single
    Private TargetSlop As Single
    Public ReadOnly AttackRange As Single
    Private AttackDelayInMs As Single
    Private RemainingAttackTime As Single
    Private TargetAcquired As Boolean
    Private CurrentTarget As EntityPlayer
    Private CurrentTargetIndex As Integer
    Private MovementLoops As Single
    Private AttackAnimFinishedOnLastLoop As Boolean
    Public Event NewTargetAcquired(sender As Mob, e As System.EventArgs, TargetIndex As Integer)
    Public Event DamageTakenFromPlayer(sender As Mob, e As System.EventArgs, source As EntityPlayer)
    Public Event DamageTakenFromGeneric(sender As Mob, e As System.EventArgs, source As EntityLiving)
    Public Event DamageDealt(sender As Mob, e As System.EventArgs, TargetIndex As Integer)
    'Private Shadows isAlive As Boolean = True
    Public MobDamage As Damage = Damage.DefaultDamage()
    'Public Shadows Property Alive As Boolean
    '    Get
    '        Return isAlive
    '    End Get
    '    Set(value As Boolean)
    '        isAlive = value
    '    End Set
    'End Property

    Public Overrides ReadOnly Property Visible As Boolean
        Get
            Return Alive
        End Get
    End Property
    Public Sub New(CM As ContentManager, anim As EntityLivingAnimationSet, useDeathAnim As Boolean, em As EntityManagement)
        MyBase.New(IDGenerator.Generate(New Random()), New Size(128, 128), 250, em)
        TileHitBox = New Vector2(-1, -1)
        'TileHitBoxOffset = New Vector2(2, 4)
        'defaultAnimation = anim
        'MyBase.UseDeathAnimation = useDeathAnim
        'deathAnimation = deathAnim
        animation = anim

        AttackAnimationSpeed = 100
        AttackDelayInMs = 500
        RemainingAttackTime = AttackDelayInMs
        TargetSlop = 2
        AttackRange = 2

        AcquisitionRange = 7
        MovementSpeed = 0.0075

        Pos = New Vector3(10, 10, 0)
        Dim r As Random
        r = New Random()
        Dim size As New Vector2
        size.X = em.CurrentMap.width
        size.Y = em.CurrentMap.height

        Dim x As Integer
        x = r.Next(CInt(size.X))
        Dim y As Integer
        y = r.Next(CInt(size.Y))
        Pos = New Vector3(x, y, map.GetHeight(x, y))


        AIAction = New MobGuardAction(Me, em)
        'SetAnimations("Stand", False)

    End Sub

    Public Shadows Sub Update(gt As GameTime, EM As EntityManagement, Cam As IsoCamera)
        MyBase.Update(gt)

        'If IsNothing(AIAction) Then
        'AIAction = New MobGuardAction(Me, EM)
        'End If

        'AIAction.increment(gt.ElapsedGameTime)
        'animation.Update(gt)

        ApplyModifiers(gt)
        If Not Alive Then
            Exit Sub
        End If
        'If CurrentHealth <= 0 And Not Dying And UseDeathAnimation Then

        '    Dying = True
        '    deathAnimation.PlayAnimationOnce("Death", True, AnimationRevertState.PAUSE_ANIMATION)

        'End If
        'If Dying And UseDeathAnimation Then
        '    deathAnimation.Update(gt)
        'End If
        'Dim result As TargetSearchResponse
        'result = AcquireTargets(gt, EM.Players, Cam)
        'defaultAnimation.Update(gt)

        'Dim beginAttackAnimation, hasMoved As Boolean
        'If RemainingAttackTime >= 0 Then
        '    RemainingAttackTime -= gt.ElapsedGameTime.Milliseconds
        '    If RemainingAttackTime < 0 Then
        '        RemainingAttackTime = 0
        '    End If
        'End If
        'If TargetAcquired Then
        '    SetFacing(DegreesToFacing(KernsMath.AngleBetweenTwoPoints(CurrentTarget.OffsetCenter, OffsetCenter)))
        '    If Vector3.Distance(CurrentTarget.OffsetCenter, OffsetCenter) <= AttackRange And Not Attacking And RemainingAttackTime <= 0 Then
        '        beginAttackAnimation = True
        '        isAttacking = True
        '        RemainingAttackTime += AttackDelayInMs
        '    Else
        '        MovementLoops = CSng(UpdateOrders(CurrentTarget.OffsetCenter, Cam))
        '    End If
        '    If Not Attacking Then
        '        If MovementLoops >= gt.ElapsedGameTime.Milliseconds Then
        '            ' Pos += Vel * gt.ElapsedGameTime.Milliseconds
        '            '  MovementLoops -= gt.ElapsedGameTime.Milliseconds
        '            hasMoved = True
        '        Else
        '            If MovementLoops > 0 And MovementLoops < gt.ElapsedGameTime.Milliseconds Then

        '                'Pos += Vel * (MovementLoops / gt.ElapsedGameTime.Milliseconds)
        '                ' MovementLoops = 0
        '                If CurrentTarget.Vel = Vector3.Zero Then
        '                    hasMoved = False
        '                Else
        '                    hasMoved = True

        '                End If


        '            Else
        '                MovementLoops = 0
        '                hasMoved = False
        '            End If
        '        End If
        '    End If
        'End If
        'UpdateAnimations(hasMoved, beginAttackAnimation)
    End Sub

    'Private Sub UpdateAnimations(hasMoved As Boolean, beginAttackAnimation As Boolean)
    '    If defaultAnimation.CurrentAnimationName = "Death" Then
    '        Exit Sub
    '    End If
    '    If AttackAnimFinishedOnLastLoop Then
    '        isAttacking = False
    '        AttackAnimFinishedOnLastLoop = False

    '    End If
    '    If beginAttackAnimation Then
    '        isAttacking = True
    '        SetAnimations("Attack", False)
    '        beginAttackAnimation = False
    '        Exit Sub

    '    End If

    '    If Attacking Then
    '        Exit Sub
    '    End If
    '    If hasMoved Then
    '        SetAnimations("Running", True)
    '    Else
    '        SetAnimations("Stand", False)
    '    End If
    'End Sub

    'Private Sub AnimDone(definition As AnimationDefinition, CurrentFrame As Short) Handles defaultAnimation.AnimDone
    '    If definition.AnimationName = "Attack" Then
    '        isAttacking = False
    '        If Not AttackAnimFinishedOnLastLoop Then
    '            RaiseEvent DamageDealt(Me, New EventArgs(), CurrentTargetIndex)
    '        End If
    '        AttackAnimFinishedOnLastLoop = True

    '    End If

    'End Sub
    'Private Sub DeathAnimDone(definition As AnimationDefinition, CurrentFrame As Short) Handles deathAnimation.AnimDone
    '    Alive = False
    '    Dying = False
    'End Sub

    Private Function WithinRange(target As EntityPlayer) As Boolean
        Dim finalRange As Single = AcquisitionRange
        'If alreadyAcquired Then
        '    finalRange = AcquisitionRange + AttackRange + TargetSlop

        'Else
        '    finalRange = AcquisitionRange + AttackRange
        'End If
        Return Vector3.Distance(Pos, target.Pos) <= finalRange
        
    End Function
    Private Function AcquireTargets(gt As GameTime, Players() As EntityPlayer, Cam As IsoCamera) As TargetSearchResponse

        TargetAcquired = False
        Dim returnResponse As TargetSearchResponse = TargetSearchResponse.NO_TARGET_FOUND
        For i = 0 To Players.Length - 1 Step 1
            If Not Players(i).Alive Then
                Continue For ' cannot aquire dead targets
            End If
            If WithinRange(Players(i)) Then
                CurrentTarget = Players(i)
                CurrentTargetIndex = i
                If TargetAcquired Then
                    returnResponse = TargetSearchResponse.EXISTING_TARGET_FOUND
                    Dim targetNavLoc As NavLoc = map.GetWalkableLoc(Cam.ScreenToWorldRay(Cam.MapToScreen(CurrentTarget.Pos)))
                    If Not IsNothing(targetNavLoc) Then
                        MyBase.UpdateOrders(targetNavLoc)
                    End If
                Else
                    Dim targetNavLoc As NavLoc = map.GetWalkableLoc(Cam.ScreenToWorldRay(Cam.MapToScreen(CurrentTarget.Pos)))
                    If Not IsNothing(targetNavLoc) Then
                        MyBase.UpdateOrders(targetNavLoc)
                    End If
                    returnResponse = TargetSearchResponse.NEW_TARGET_FOUND
                End If


                TargetAcquired = True

            Else
                If i = CurrentTargetIndex Then
                    TargetAcquired = False
                    returnResponse = TargetSearchResponse.OLD_TARGET_LOST
                End If
            End If
        Next

        Return returnResponse
        'True for target found, false for otherwise
    End Function
    Private Enum TargetSearchResponse As Integer
        NO_TARGET_FOUND = 0
        NEW_TARGET_FOUND = 1
        EXISTING_TARGET_FOUND = 2
        OLD_TARGET_LOST = 3
    End Enum

    Public Shadows Sub Draw(sb As SpriteBatch, cam As IsoCamera)
        Dim translatedPos As Vector2 = cam.MapToScreen(Pos)
        Dim layer As Single = cam.ScaleLayer(Pos.X + Pos.Y + Pos.Z)
        animation.Draw(sb, translatedPos, layer)
        'If Dying Or Not Alive Then
        '    deathAnimation.Draw(sb, translatedPos, layer)'deathAnimation.Draw(sb, New Rectangle(CInt(translatedPos.X), CInt(translatedPos.Y), Size.Width, Size.Height), layer)
        'Else
        '    defaultAnimation.Draw(sb, translatedPos, layer)'defaultAnimation.Draw(sb, New Rectangle(CInt(translatedPos.X), CInt(translatedPos.Y), Size.Width, Size.Height), layer)
        'End If

    End Sub
    Public Overloads Function UpdateOrders(targetPoint As Vector3, Cam As IsoCamera) As Double
        'Eventually this and Move will work in harmony with the pathing system. For now, we'll assume that there aren't any obstacles.
        Dim orderCalcResult() As Double = New Double(4) {}

        


        velocity = Vector3.Subtract(targetPoint, OffsetCenter)
        velocity.Normalize()
        velocity *= MovementSpeed
        Dim Loops As Double
        If MovementSpeed = 0 Then
            Return 0.0
            'Don't want to divide by zero.
        End If
        Loops = Vector3.Distance(OffsetCenter, targetPoint) / MovementSpeed
        Return Loops
    End Function
End Class

#Region "Chaff"
'    ' charachter properties

'    Private Shadows texture As IsoTexture

'    Friend Degree As Integer = 0
'    Friend Angle As Double = Math.PI * Degree / 180    'Radian value of Degree


'    Private hasStartedWanderCD As Boolean 'Whether or not the mob has begun wandering. 
'    Private NeedToValidateWanderCD As Boolean = False 'Whether or not the wander duration has changed, meaning that on the next game loop the cooldown needs to be updated.
'    Private WanderDuration As Integer
'    Private WanderRange As Vector2
'    Private hasStartedAcquireCD As Boolean
'    Private NeedToValidateAcquireCD As Boolean = False
'    Private AcquireDuration As Integer
'    Private RemainingAttackTime As Integer

'    Private MovementLoops As Double
'    Private Acquired As Boolean
'    Private CurrentTarget As Vector3
'    Private AcquisitionDistance As Single

'    Private AttackRange As Single 'The circular range in tiles, for an attack.
'    Private CurrentTargetIndex As Integer

'    Public Overrides Property Name As String
'        Get
'            Return CharacterName
'        End Get
'        Set(value As String)
'            CharacterName = value
'        End Set
'    End Property
'    Public ReadOnly Property TargetRange As Single
'        Get
'            Return AcquisitionDistance + AttackRange
'        End Get
'    End Property

'    Public Property AcquisitionCooldown As Single
'        Get
'            Return AcquireDuration
'        End Get
'        Set(value As Single)
'            AcquisitionDistance = value
'            Acquired = False
'            NeedToValidateAcquireCD = True
'        End Set
'    End Property
'#Region "Events"
'    Public Event TargetAcquired(sender As Mob, e As System.EventArgs, TargetIndex As Integer)
'    Public Event DamageTakenFromPlayer(sender As Mob, e As System.EventArgs, source As EntityPlayer)
'    Public Event DamageTakenFromGeneric(sender As Mob, e As System.EventArgs, source As EntityLiving)
'    Public Event DamageDealt(sender As Mob, e As System.EventArgs, TargetIndex As Integer)

'#End Region
'    Public Property WanderDistance As Vector2
'        Get
'            Return WanderRange
'        End Get
'        Set(value As Vector2)
'            WanderRange = value
'        End Set
'    End Property
'    Public Property WanderTime As Integer
'        Get
'            Return WanderDuration
'        End Get
'        Set(value As Integer)
'            WanderDuration = value
'            NeedToValidateWanderCD = True
'        End Set
'    End Property
'    Public ReadOnly Property CanAttack As Boolean
'        Get
'            If isAttacking THen
'                Return True
'            End If
'            If RemainingAttackTime <= 0 Then
'                Return True
'            Else
'                Return False
'            End If
'        End Get
'    End Property
'    Public Sub New(ID As String, defaultSize As Size, MHealth As Integer, MMana As Integer, mSpeed As Single, anim As Animation, pos As Vector3, CM As ContentManager)
'        MyBase.New(ID, defaultSize, MHealth)
'        MyBase.CurrentMaxMana = MMana
'        MyBase.CurrentMana = MMana
'        BaseType = EntityBaseType.Mob
'        MovementSpeed = mSpeed
'        defaultAnimation = anim
'        position = pos
'        TileHitBox = New Vector2(-1, -1)
'        TileHitBoxOffset = New Vector2(2, 4)
'        WanderRange = New Vector2(15, 15)
'        WanderDuration = 25000
'        AcquireDuration = 2500
'        AcquisitionDistance = 7
'        OverheadDrawFont = CM.Load(Of SpriteFont)(AssetManager.RequestAsset("overheadFont", AssetTypes.SPRITEFONT))
'        AttackRange = 3
'        AttackAnimationSpeed = 500
'        AttackSpeed = 1000 'The delay in between each attack.
'        RemainingAttackTime = AttackSpeed
'        CharacterName = "Test Mob"
'    End Sub

'    'Public Sub setVelocity()
'    '    Degree = KernsMath.RandInt(0, 359)      'Random angle in degree
'    '    Angle = Math.PI * Degree / 180          'Random angle in radian
'    '    velocity = New Vector3(CSng(MovementSpeed / Math.Cos(Angle)), CSng(MovementSpeed / Math.Sin(Angle)), 0)    'Set velocity with proper movement speed and angle

'    'End Sub
'    Private Const MOB_WANDER_COOLDOWN_PREFIX As String = "Mob Wander Cooldown ID "
'    Private Const MOB_TARGET_COOLDOWN_PREFIX As String = "Mob Acquire Cooldown ID "
'    Private Const MOB_ATTACKSPEED_COOLDOWN_PREFIX As String = "Mob Attack Cooldown ID "
'    Public Overridable Sub Wander(gt As GameTime, Cam As IsoCamera, CD As CooldownManager, Players() As EntityPlayer)
'        If NeedToValidateAcquireCD Then
'            CD.RemoveCooldown(MOB_TARGET_COOLDOWN_PREFIX + ID)
'            CD.AddCooldown(AcquireDuration, MOB_TARGET_COOLDOWN_PREFIX + ID)
'        End If
'        If hasStartedAcquireCD = False Then
'            CD.AddCooldown(AcquireDuration, MOB_TARGET_COOLDOWN_PREFIX + ID)
'            hasStartedAcquireCD = True
'        End If
'        If CD.IsCooldownExpired(MOB_TARGET_COOLDOWN_PREFIX + ID) Then
'            AcquireTargets(Players, Cam, CD)
'            'Only fire if target is found.
'        End If

'        If Not Acquired Then

'            If hasStartedWanderCD = False Then
'                CD.AddCooldown(WanderDuration, MOB_WANDER_COOLDOWN_PREFIX + ID)
'                hasStartedWanderCD = True
'            End If
'            If NeedToValidateWanderCD Then
'                NeedToValidateWanderCD = False
'                CD.RemoveCooldown(MOB_WANDER_COOLDOWN_PREFIX + ID)
'                CD.AddCooldown(WanderDuration, MOB_WANDER_COOLDOWN_PREFIX + ID)
'            End If
'            If CD.IsCooldownExpired(MOB_WANDER_COOLDOWN_PREFIX + ID) Then
'                'Time to wander.
'                CD.FireCooldown(MOB_WANDER_COOLDOWN_PREFIX + ID)
'                Dim PossibleTargetX, PossibleTargetY As Integer
'                PossibleTargetX = KernsMath.RandInt(-1 * CInt(WanderRange.X), CInt(WanderRange.X))
'                PossibleTargetY = KernsMath.RandInt(-1 * CInt(WanderRange.Y), CInt(WanderRange.Y))
'                'Okay, now we know the wander point.
'                If Not isAttacking Then
'                    ' MobUpdateOrders(TargetPositionAdjustment(Players(closestIndex), Cam), Cam, Players(closestIndex))
'                End If
'            End If
'        Else
'            Dim breakpoint As ChineseLunch = New ChineseLunch()

'        End If
'        UpdateFacingToTarget(CurrentTarget, Cam)
'        AcquireTargets(Players, Cam, CD)
'    End Sub
'    ''' <summary>
'    ''' Searches for nearby players within its Acquisition Range. If one is found, it stops wandering and pursues the player.
'    ''' </summary>
'    ''' <param name="Players"></param>
'    ''' <returns></returns>
'    ''' <remarks></remarks>
'    Private Function AcquireTargets(Players() As EntityPlayer, Cam As IsoCamera, CD As CooldownManager) As Boolean

'        Acquired = False
'        Dim counter As Integer
'        Dim closestIndex As Integer 'Find the closest player.
'        Dim closestValue As Single = Single.MaxValue
'        Dim targetFound As Boolean
'        For Each Player In Players
'            Dim distance = Vector3.Distance(OffsetPosition, Player.OffsetCenter)
'            If distance <= TargetRange Then
'                If distance < closestValue Then
'                    closestIndex = counter
'                    closestValue = distance
'                End If
'                targetFound = True



'            End If
'            counter += 1
'        Next
'        If targetFound and isAttacking then
'                CurrentTarget = Players(closestIndex).OffsetCenter
'               ' CurrentTargetIndex = closestIndex
'        Else If targetFound And Not isAttacking Then
'            Acquired = True
'            RaiseEvent TargetAcquired(Me, New EventArgs(), closestIndex)
'            MovementLoops = MobUpdateOrders(Players(closestIndex).OffsetCenter, Cam, Players(closestIndex))

'            CurrentTarget = Players(closestIndex).OffsetCenter
'            'CD.FireCooldown(MOB_TARGET_COOLDOWN_PREFIX + ID)
'            CurrentTargetIndex = closestIndex
'        End If
'        Return Acquired
'    End Function
'    Private Function TargetPositionAdjustment(Player As EntityPlayer, Cam As IsoCamera) As Vector3
'        Dim finalResult As Vector3 = Vector3.Zero
'        Dim TileAndPositionA As RectangleF = New RectangleF(OffsetPosition.X, OffsetPosition.Y, TileHitBox.X, TileHitBox.Y)

'        Return Player.OffsetPosition
'    End Function
'    Public Overrides Sub Update(gt As GameTime)
'        If Vector3.Distance(CurrentTarget, OffsetCenter) <= AttackRange And CanAttack Then
'            'Attack instead of moving.
'            velocity = Vector3.Zero
'            'RemainingAttackTime += AttackSpeed
'            isAttacking = True
'            If defaultAnimation.CurrentAnimationName <> "Attack" Then
'            SetAnimations("Attack", False)
'                End IF


'        Else
'            isAttacking = False
'            If MovementLoops < 1 And MovementLoops > 0 Then
'                Dim tempVel As Vector3 = New Vector3(velocity.X * CSng(MovementLoops / gt.ElapsedGameTime.Milliseconds), velocity.Y * CSng(MovementLoops / gt.ElapsedGameTime.Milliseconds), velocity.Z)

'                position += tempVel
'            Else
'                If MovementLoops < gt.ElapsedGameTime.Milliseconds Then
'                    position += velocity * CSng(MovementLoops / gt.ElapsedGameTime.Milliseconds)
'                Else
'                    position += velocity * CSng(gt.ElapsedGameTime.Milliseconds)
'                End If
'                SetAnimations("Running", False)
'                MovementLoops = 0
'            End If


'            If RemainingAttackTime >= 0 Then
'                RemainingAttackTime -= gt.ElapsedGameTime.Milliseconds
'            End If
'            If MovementLoops < gt.ElapsedGameTime.Milliseconds Then
'                velocity = Vector3.Zero
'                MovementLoops = 0
'            Else

'                MovementLoops -= gt.ElapsedGameTime.Milliseconds
'            End If
'            If velocity = Vector3.Zero Then

'            End If
'        End If

'        defaultAnimation.Update(gt)

'    End Sub
'    'Public Function MobUpdateOrders(targetPoint As Vector3, Cam As IsoCamera, player As EntityPlayer) As Double
'    '        'Eventually this and Move will work in harmony with the pathing system. For now, we'll assume that there aren't any obstacles.
'    '        Dim orderCalcResult() As Double = New Double(4) {}

'    '        'Kerns Math may be broken
'    '        'orderCalcResult = KernsMath.UnitRotation(New Vector2(Center.X, Center.Y), New Vector2(targetPoint.X, targetPoint.Y), MovementSpeed)

'    '        Dim deltaP As Vector3 = targetPoint - OffsetPosition
'    '        Dim angle As Double = Math.Atan2(deltaP.Y, deltaP.X)
'    '        orderCalcResult(0) = angle
'    '        orderCalcResult(1) = MovementSpeed * Math.Cos(angle)
'    '        orderCalcResult(2) = MovementSpeed * Math.Sin(angle)

'    '        'rotation, xvel, yvel, # of movement loops
'    '        'planned pathing :D
'    '        'Shitty, shitty abstraction. We'll cross that defect when we come to it.
'    '        'Update the facing based on the integer value of the facing.
'    '        Dim facingInDeg As Double
'    '        facingInDeg = MathHelper.ToDegrees(CSng(orderCalcResult(0)))
'    '        velocity = New Vector3(CSng(orderCalcResult(1)), CSng(orderCalcResult(2)), 0)

'    '    Dim Loops As Double
'    '    If MovementSpeed = 0 Then
'    '        Return 0.0
'    '        'Don't want to divide by zero.
'    '        'REM Why Not?
'    '    End If

'    '    Dim distance As Single = Vector3.Distance(player.OffsetPosition, OffsetPosition)
'    '    If distance <= AttackRange Then
'    '        SetAnimations("Attack", False)

'    '    Else
'    '        SetAnimations("Running", False)
'    '    End If
'    '    Loops = Vector3.Distance(OffsetPosition, targetPoint) / MovementSpeed
'    '    Return Loops
'    'End Function

'    Public Function MobUpdateOrders(targetPoint As Vector3, Cam As IsoCamera, player As EntityPlayer) As Double
'        Dim vel As Vector3 = player.OffsetPosition - Me.OffsetPosition
'        Me.velocity = vel
'        velocity.Normalize()
'        SetFacing(DegreesToFacing(KernsMath.AngleConvert360(CSng(AngleBetweenTwoPoints(OffsetPosition, player.OffsetPosition)))))
'        Dim dist As Single = Vector3.Distance(OffsetPosition, player.OffsetPosition)
'        If dist <= AttackRange Then
'            SetAnimations("Attack", False)
'        Else
'            SetAnimations("Running", False)
'        End If
'        If MovementSpeed = 0 Then
'            Return 0
'        End If
'        Return Vector3.Distance(OffsetPosition, player.OffsetPosition) / MovementSpeed
'    End Function

'    Private Sub AnimDone(animation As AnimationDefinition, currentFrame As Short) Handles defaultAnimation.AnimDone


'        If animation.AnimationName = "Attack" Then
'            If Vector3.Distance(OffsetCenter, CurrentTarget) <= AttackRange Then
'                RaiseEvent DamageDealt(Me, New EventArgs(), CurrentTargetIndex)
'                NeedToValidateAcquireCD = TRue
'            Else
'                isAttacking = False
'            End If

'        End If
'        If animation.AnimationName = "Running" Then
'            isAttacking = False
'        End If
'    End Sub
'    Private Sub AnimChanged(from As AnimationDefinition, toAnim As AnimationDefinition) Handles defaultAnimation.AnimChanged
'        If from.AnimationName = "Attack" And toAnim.AnimationName <> "Attack" Then
'            isAttacking = False
'        End If
'    End Sub

'    Public Overrides Sub Draw(sb As SpriteBatch, cam As IsoCamera)
'        defaultAnimation.Draw(sb, New Rectangle(CInt(cam.MapToScreen(Pos).X), CInt(cam.MapToScreen(Pos).Y), Size.Width, Size.Height))
'        sb.DrawString(OverheadDrawFont, MovementLoops.ToString(), Vector2.Subtract(cam.MapToScreen(Pos), (OverheadDrawFont.MeasureString(MovementLoops.ToString))), Color.White)
'    End Sub







'Public Shadows Sub Update(gt As GameTime, Players() As EntityPlayer, Cam As IsoCamera)
'    defaultAnimation.Update(gt)

'    If RemainingAttackTime >= 0 And Not defaultAnimation.CurrentAnimationName = "Attack" Then
'        RemainingAttackTime -= gt.ElapsedGameTime.Milliseconds

'    End If

'    If defaultAnimation.CurrentAnimationName = "Attack" Then
'        'Already attacking, don't intervene.
'        Exit Sub
'    End If

'    'Double check that the player is still in range.
'    TargetAcquired = AcquireTargets(gt, Players)
'    If TargetAcquired Then
'        MovementLoops = CSng(UpdateOrders(CurrentTarget.OffsetCenter, Cam))
'        If Vector3.Distance(CurrentTarget.OffsetCenter, OffsetCenter) <= AttackRange Then
'            'No more remaining loops or closer than one loop.
'            If RemainingAttackTime <= gt.ElapsedGameTime.Milliseconds Then

'                defaultAnimation.SetAnimation("Attack", True)
'                RemainingAttackTime += AttackDelayInMs
'                Exit Sub
'            Else
'                'Within range, but not ready to attack.
'                If Not defaultAnimation.CurrentAnimationName = "Attack" Then
'                    ' defaultAnimation.PlayAnimationOnce("Stand", True)

'                End If
'            End If
'        Else




'        End If
'    Else
'        'No target found!
'        defaultAnimation.SetAnimation("Stand", False)

'    End If
'    'Pursue the current target.
'    If MovementLoops > 0 Then
'        If MovementLoops >= gt.ElapsedGameTime.Milliseconds Then
'            Pos += Vel
'            MovementLoops -= gt.ElapsedGameTime.Milliseconds
'            'More than one full loop remaining.
'            If MovementLoops < 1 Then
'                MovementLoops = 0
'            End If
'            SetAnimations("Running", False)
'        Else

'            Pos += Vel * (MovementLoops / gt.ElapsedGameTime.Milliseconds)
'            'Move the remaining percentage of a loop
'            MovementLoops = 0

'        End If

'    Else
'        SetAnimations("Stand", False)
'    End If

'End Sub
'Private Sub AnimDone(definition As AnimationDefinition, currentFrame As Short) Handles defaultAnimation.AnimDone
'    If definition.AnimationName = "Attack" Then
'        SetAnimations("Stand", False)
'        RaiseEvent DamageDealt(Me, New EventArgs(), CurrentTargetIndex)
'    End If
'End Sub
'Private Function AcquireTargets(gt As GameTime, Players() As EntityPlayer) As Boolean
'    For i = 0 To Players.Length - 1 Step 1
'        Dim finalRange As Single
'        If TargetAcquired Then
'            finalRange = AcquisitionRange + AttackRange + TargetSlop
'        Else
'            finalRange = AcquisitionRange + AttackRange
'        End If
'        If Vector3.Distance(OffsetCenter, Players(i).OffsetCenter) <= finalRange Then
'            CurrentTarget = Players(i)
'            CurrentTargetIndex = i
'            TargetAcquired = True
'            Return True
'        End If
'    Next
'    Return False
'    'True for target found, false for otherwise
'End Function
#End Region
