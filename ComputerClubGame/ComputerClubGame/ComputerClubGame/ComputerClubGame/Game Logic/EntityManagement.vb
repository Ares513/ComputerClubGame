Public Class EntityManagement
#Region "Lists"
    'Handles the organization of all of the entities in the program. Each entity category (FriendlyNPC, EntityPlayer, HostileNPC, InteractiveObject, etc) has its own array.
    Public Players() As EntityPlayer
    Public CombatMobs As List(Of Mob)
    Public Projectiles As List(Of Projectile)
    'Sorry, but these variables will need to be accessible, especially for damage management in IActions and such.
    Public ActiveEffects As EffectManagement
    Private HitboxDebugTexture As Texture2D
    Public CooldownList As CooldownManager
    Public WithEvents DroppedItems As DroppedItemManagement
    ''' <summary>
    ''' Raised whenver any entity does damage to another entity. Useful for floating text values.
    ''' </summary>
    ''' <param name="crit"></param>
    ''' <param name="dmg"></param>
    ''' <param name="worldCoord">Tne world coordinatnes location </param>
    ''' <remarks></remarks>
    Public Event DamageDone(crit As Boolean, message As String, dmg As Damage, worldCoord As Vector3)
#End Region
#Region "Generators"
    Private ProjectileGen As ProjectileFactory

#End Region
    Private isNetworkLobby As Boolean
    Public ReadOnly DefaultPlayerSize As Size = New Size(2, 2)
    Private isLobbyActive As Boolean
    Private map As Map
    Public ReadOnly Property CurrentMap As Map
        Get
            Return map
        End Get
    End Property
    Public ReadOnly Property HilightedEntity(ms As MouseState, cam As IsoCamera) As String
        Get
            Return getSelectedEntityID(ms, cam)
        End Get
    End Property
    Public ReadOnly Property LobbyActive As Boolean
        Get
            Return isLobbyActive
        End Get
    End Property
    Public ReadOnly Property PlayerCount As Byte
        Get
            Return CByte(Players.Length - 1)
        End Get

    End Property

    Public ReadOnly Property LocalPlayerInfo As EntityPlayer
        Get
            Return Players(0)
        End Get
    End Property
#Region "Event Handlers"
    Private Sub MobDamageDealt(sender As Mob, e As System.EventArgs, Target As Integer)
        'Implement mob damage values here
        Dim dmg As Damage = New Damage(10, 20)
        Players(Target).TakeDamage(dmg)
        RaiseEvent DamageDone(False, dmg.LastCalculatedDamage.ToString(), dmg, Players(Target).Pos)
    End Sub
    Private Sub MobTargetAcquired(sender As Mob, e As System.EventArgs, Target As Integer)

    End Sub
    Private Sub PlayerDamageDealt(sender As EntityPlayer, e As System.EventArgs, target As Mob)

    End Sub

#End Region
    ''' <summary>
    ''' Creates a new EntityManagement instance.
    ''' </summary>
    ''' <param name="PlayerData">PlayerData instance. The first index in the PlayerData array is always assumed to be the local player.</param>
    ''' <param name="isNetworkLobby">Whether or not the game is local. If it's local, the length of PlayerData should probably be one.</param>
    ''' <param name="CM">ContentManager instance</param>
    ''' <remarks>Always load localPlayerData.</remarks>
    Public Sub New(PlayerData() As Player, isNetworkLobby As Boolean, CM As ContentManager, map As Map)
        Me.map = map
        Dim i As Integer
        If IsNothing(PlayerData) Then
            DebugManagement.WriteLineToLog("PlayerData is empty! The program cannot continue.", SeverityLevel.FATAL)
            MsgBox("Fatal error occured! PlayerData is empty. The program will now exit.", MsgBoxStyle.Critical, "FATAL ERROR")
        Else
            For i = 0 To PlayerData.Length - 1 Step 1
                If IsNothing(PlayerData(i)) Then
                    DebugManagement.WriteLineToLog("PlayerData has an empty index!", SeverityLevel.CRITICAL)

                End If
            Next
        End If
        'Safety check complete
        ReDim Preserve Players(PlayerData.Length - 1)
        For i = 0 To PlayerData.Length - 1 Step 1

            Players(i) = New EntityPlayer(IDGenerator.Generate(New Random()), DefaultPlayerSize, PlayerData(i), CM, Me)
            Players(i).Pos = New Vector3(DefaultPlayerSize.Width * i, DefaultPlayerSize.Height * i, 0)
            'Offset player position based on player slot information. Player position should be handed down from the server here. 

        Next
        Players(0).MakeLocal() '1st index is always local player
        Me.isNetworkLobby = isNetworkLobby
        For i = 0 To Players.Length - 1 Step 1
            'AddHandler Players(i).DamageDealtToMob, AddressOf PlayerDamageDealt
        Next
        CombatMobs = New List(Of Mob)
        Projectiles = New List(Of Projectile)
        ProjectileGen = New ProjectileFactory()
        ActiveEffects = New EffectManagement()

        HitboxDebugTexture = CM.Load(Of Texture2D)("missing_texture")
        CooldownList = New CooldownManager()
        DroppedItems = New DroppedItemManagement(CM)
        ' DroppedItems.Items.Add(New DroppedItem(New Vector3(0, 3, 0), "Gold", "None", True, CM, map, 200))
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="character"></param>
    ''' <remarks></remarks>
    Public Sub AddMob(character As Mob)
        AddHandler character.DamageDealt, AddressOf MobDamageDealt
        AddHandler character.NewTargetAcquired, AddressOf MobTargetAcquired
        CombatMobs.Add(character)

    End Sub
    Private Sub AddProjectile(projectile As Projectile)
        AddHandler projectile.ProjectileDetonated, AddressOf ProjectileDetonated

        Projectiles.Add(projectile)
    End Sub
    Private Sub ProjectileDetonated(sender As Projectile, e As System.EventArgs, blastRadius As Single, maxTargets As Integer, dmg As Damage)
        'This code runs whenever a projectile detonates. 
        For Each Mob In CombatMobs
            If KernsMath.ExpensiveRangeCheck(sender.OffsetCenter, Mob.OffsetPosition, blastRadius) Then
                Mob.TakeDamage(dmg)
                'Mob.CurrentHealth -= dmg.CalculateDamage(Mob)
            End If
        Next

    End Sub

    Public Sub GenerateProjectile(SourceID As String, Type As ProjectileTypes, CM As ContentManager, startPosition As Vector3, targetPosition As Vector3, projectileSpeed As Single, projectileSize As Size, ImpactAction As IAction, DetonationAction As IAction, Cam As IsoCamera, despawnOnCollision As Boolean, projectileColor As Color, deathAnimationColor As Color, Optional Pathing As ProjectilePathingTypes = ProjectilePathingTypes.LINEAR)
        AddProjectile(ProjectileGen.MakeProjectile(SourceID, IDGenerator.Generate(New Random()), Type, CM, startPosition, targetPosition, projectileSpeed, projectileSize, ImpactAction, DetonationAction, Cam, despawnOnCollision, projectileColor, deathAnimationColor, Me, Pathing))

    End Sub

    Public Sub Draw(sb As SpriteBatch, ms As MouseState, gt As GameTime, Cam As IsoCamera)
        Dim i As Byte
        For i = 0 To CByte(Players.Length - 1) Step 1
            If Players(i).isLocalPlayer Then
                Players(i).Draw(sb, gt, Cam, True)
            Else
                Players(i).Draw(sb, gt, Cam, False)
            End If

        Next
        DroppedItems.Draw(sb, Cam, gt, ms)
        For Each character In CombatMobs
            character.Draw(sb, Cam)
            Dim hitboxpos As Vector2 = Cam.MapToScreen(character.Pos)
            Dim hitboxSize As Vector2 = Cam.MapToScreen(character.Pos + New Vector3(character.TileHitBox.X, character.TileHitBox.Y, 0))

            sb.Draw(HitboxDebugTexture, New Rectangle(CInt(hitboxpos.X), CInt(hitboxpos.Y), CInt(hitboxpos.X - hitboxSize.X), CInt(hitboxpos.Y - hitboxSize.Y)), Color.White)
        Next
        For Each Projectile In Projectiles
            Projectile.Draw(sb, Cam)
            Projectile.CheckCollision(CombatMobs.ToArray(), Cam)

        Next
        DrawScreenViewHitboxes(sb, Cam)

    End Sub
    Public Sub Update(ms As MouseState, gt As GameTime, Cam As IsoCamera, doUpdateOrders As Boolean, ks As KeyboardState, CM As ContentManager, UI As UIOverlay)
        Dim i As Integer
        CooldownList.UpdateCooldowns(gt)
        ActiveEffects.Update(Me, gt, ms, ks, CM, UI, Cam)
        For i = 0 To Players.Length - 1 Step 1


            If Not LocalPlayerInfo.Attacking And ms.LeftButton = ButtonState.Released Then
                'LocalPlayerInfo.StopMoving()
            End If

            'Before we move, check for collisions and deflect the velocity.


            Players(i).Move(ms, gt)
            Players(i).Update(gt)
        Next

        For Each Mob In CombatMobs
            Mob.Update(gt, Me, Cam)

        Next
        For Each Projectile In Projectiles
            Projectile.Update(gt, Cam)
            'Projectile.CheckCollision(CombatMobs.ToArray(), Cam)
        Next

    End Sub
    ''' <summary>
    ''' Returns the ID of the entity that is currently under the cursor.
    ''' </summary>
    ''' <param name="ms"></param>
    ''' <param name="Cam"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function getSelectedEntityID(ms As MouseState, Cam As IsoCamera) As String
        Dim mouseRay As IsoRay
        mouseRay = Cam.ScreenToWorldRay(New Vector2(ms.X, ms.Y))

        For Each Player In Players
            Dim playerBoundBox As BoundingBox

            playerBoundBox = New BoundingBox(New Vector3(Player.Pos.X + Player.TileHitBox.X, Player.Pos.Y + Player.TileHitBox.Y, Player.Pos.Z), Player.Pos)
            If mouseRay.Intersects(playerBoundBox) Then
                Return Player.ID
            End If

        Next
        For Each Mob In CombatMobs
            Dim mobBoundBox As BoundingBox
            mobBoundBox = New BoundingBox(New Vector3(Mob.Pos.X + Mob.TileHitBox.X + Mob.TileHitBoxOffset.X, Mob.Pos.Y + Mob.TileHitBox.Y + Mob.TileHitBoxOffset.Y, Mob.Pos.Z), Mob.Pos + New Vector3(Mob.TileHitBoxOffset.X, Mob.TileHitBoxOffset.Y, 0))
            If ms.LeftButton = ButtonState.Pressed Then
                Dim i As Integer = 0
                i = i
                REM WHAT A POINTLESS STATEMENT!
            End If
            If mouseRay.Intersects(mobBoundBox) Then
                Return Mob.ID
            End If
        Next

        Return ""
    End Function
    ''' <summary>
    ''' Selection search for a specified entityLiving. The entity will be returned.
    ''' </summary>
    ''' <param name="ID">The String ID that was generated when the entity was created.</param>
    ''' <returns>The Entity if found, Nothing if it doesn't.</returns>
    ''' <remarks></remarks>
    Public Function getEntityByID(ID As String) As EntityLiving
        Dim i As Integer
        For i = 0 To Players.Length - 1 Step 1
            If Players(i).ID = ID Then
                Return Players(i)
            End If
        Next
        For Each NPC In CombatMobs
            If NPC.ID = ID Then
                Return NPC
            End If
        Next

        Return Nothing
    End Function
    Public Function GetMobByID(ID As String) As Mob
        For Each NPC In CombatMobs
            If NPC.ID = ID Then
                Return NPC
            End If
        Next
        Return Nothing
    End Function
    Private Sub DrawScreenViewHitboxes(sb As SpriteBatch, cam As IsoCamera)

    End Sub
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="amount">Temporary value describing Damage class</param>
    ''' <param name="entID"></param>
    ''' <remarks></remarks>
    Public Function DamageEntity(amount As Damage, entID As String) As Boolean
        Dim target As EntityLiving = getEntityByID(entID)

        If IsNothing(target) Then
            Return False

        End If
        target.TakeDamage(amount)
        'Dim finalDamage As Single = amount.CalculateDamage(target)
        'target.CurrentHealth -= finalDamage
        'RaiseEvent DamageDone(amount.CritOnLastCalc, amount.LastCalculatedDamage.ToString(), amount, target.Pos)
        Return True
    End Function
    Private Sub ItemPickedUp(sender As DroppedItemManagement, e As EventArgs, pickedUpItem As DroppedItem) Handles DroppedItems.ItemPickedUp
        If pickedUpItem.IsGoldDrop Then
            LocalPlayerInfo.InitialPlayerData.Gold += pickedUpItem.GoldAmt
        End If
    End Sub

End Class
