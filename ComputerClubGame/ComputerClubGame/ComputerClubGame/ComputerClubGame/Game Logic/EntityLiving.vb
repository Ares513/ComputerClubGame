Public MustInherit Class EntityLiving
    Inherits Entity
#Region "Protected Variables"
    Protected Friend CurrentHealth As Single
    Protected Friend CurrentMaxHealth As Single
    Protected Friend CurrentMana As Single
    Protected Friend CurrentMaxMana As Single
    Protected Friend CurrentManaRegeneration As Single 'The amount of mana regenerated per 1000 miliseconds.
    Protected Buffs() As Buff
    Protected MovementSpeed As Single = 0.003

    Protected CharacterName As String
    Protected OverheadDrawFont As SpriteFont

    Protected OverheadBoxBackground As Texture2D
    Protected showName As Boolean 'Whether or not to show the name over the player's head
    Protected nameBackgroundColor As Color
    Protected nameForegroundColor As Color
    'Protected WithEvents defaultAnimation As IsoAnimation
    Public WithEvents animation As EntityLivingAnimationSet
    Protected isHitboxValidated As Boolean
    Public AIAction As EntityAction
    Protected em As EntityManagement
    Protected ModifierList As List(Of IModifierSet)
    Protected VisualEffects As List(Of VisualEffect)
    'Protected currentTarget As Map.NavLoc
#End Region
#Region "Properties"
    ' ''' <summary>
    ' ''' A seperate animation that can be used to store additional animations if the texture is too large.
    ' ''' Rigorous null checking should be used with this.
    ' ''' </summary>
    ' ''' <remarks></remarks>
    'Protected WithEvents deathAnimation As IsoAnimation
    'Protected UseDeathAnimation As Boolean

    Protected Dying As Boolean
    Public Event OnDeath(e As System.EventArgs, sender As Object)


    Protected isAttacking As Boolean
    Public Overridable Property MoveSpeed As Single
        Get
            Return MovementSpeed
        End Get
        Set(value As Single)
            MovementSpeed = value
        End Set
    End Property

    Public Overrides Property Name As String
        Get
            Return CharacterName
        End Get
        Set(value As String)
            CharacterName = value
        End Set
    End Property
    Public ReadOnly Property OffsetPosition As Vector3
        Get
            Return New Vector3(Pos.X + TileHitBoxOffset.X, Pos.Y + TileHitBoxOffset.Y, Pos.Z)
        End Get
    End Property
    ''' <summary>
    ''' The center of the player's tile hitbox.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property OffsetCenter As Vector3
        Get
            Return New Vector3(Pos.X + CSng(TileHitBoxOffset.X / 2), Pos.Y + CSng(TileHitBoxOffset.Y / 2), Pos.Z)
        End Get
    End Property
    ''' <summary>
    ''' The total number of milliseconds taken to run the entire attack animation.
    ''' </summary>
    ''' <remarks></remarks>
    Private WeaponAttackSpeed As Integer
    Private AttackDelay As Integer
    Public Property AttackSpeed As Integer
        Get
            Return AttackDelay
        End Get
        Set(value As Integer)
            AttackDelay = value
        End Set
    End Property
    Public Property AttackAnimationSpeed As Integer
        Get
            Return WeaponAttackSpeed
        End Get
        Set(value As Integer)
            WeaponAttackSpeed = value
            Dim weaponAnim As IAnimation = animation.Anims(EntityLivingAnimationSet.Animations.Melee)
            If Not IsNothing(weaponAnim) Then weaponAnim.SetAnimationSpeed(value)
            'defaultAnimation.SetAnimationSpeed(WeaponAttackSpeed, "Attack")
            'animation()
        End Set
    End Property
    ''' <summary>
    ''' The player's hit box. Depending on the image asset, you may need to use positive or negative values.
    ''' </summary>
    ''' <remarks></remarks>
    Public TileHitBox As Vector2
    Public TileHitBoxOffset As Vector2
    Public ReadOnly Property Attacking As Boolean
        Get
            Return isAttacking
        End Get
    End Property
    'Public Overridable Shadows ReadOnly Property Texture As Texture2D
    '    Get
    '        Return defaultAnimation.Texture
    '    End Get
    'End Property
    Public Property Health As Single
        Get
            Return CurrentHealth
        End Get
        Set(value As Single)
            CurrentHealth = value
        End Set
    End Property
    Public Property MaxHealth As Single
        Get
            Return CurrentMaxHealth
        End Get
        Set(value As Single)
            CurrentMaxHealth = value
        End Set
    End Property
    Public Property Mana As Single
        Get
            Return CurrentMana
        End Get
        Set(value As Single)
            CurrentMana = value
        End Set
    End Property
    Public Property MaxMana As Single
        Get
            Return CurrentMaxMana
        End Get
        Set(value As Single)
            CurrentMaxMana = value
        End Set
    End Property
    Public MustOverride ReadOnly Property Visible As Boolean
#End Region

    Public Sub New(ID As String, entitySize As Size, initialHealth As Single, em As EntityManagement)
        MyBase.New(ID, entitySize, em.CurrentMap)
        CurrentHealth = initialHealth
        CurrentMaxHealth = initialHealth
        ModifierList = New List(Of IModifierSet)
        VisualEffects = New List(Of VisualEffect)
        Me.em = em
    End Sub
    Public Sub TakeMana(amount As Integer)
        CurrentMana -= amount
    End Sub
    Public Overridable Sub UpdateOrders(target As NavLoc)
        If target.Map Is map Then
            AIAction = New EntityActionWalk(Me, target)
        Else
            AIAction = New EntityActionStand(Me)
        End If
    End Sub

    Public Sub TakeDamage(damage As Damage)
        CurrentHealth -= damage.CalculateDamage(Me)
        If CurrentHealth <= 0 Then Die()
    End Sub

    Public Sub Die()
        CurrentHealth = 0
        If Alive Then
            Alive = False
            AIAction = New EntityActionDie(Me)
        End If
    End Sub
    Public Sub UpdateFacingToTarget(targetPoint As Vector3)
        Dim dif As Vector3 = targetPoint - Pos
        Dim facingInRad As Double = Math.Atan2(dif.Y, dif.X)
        Dim facingInDeg As Double
        facingInDeg = MathHelper.ToDegrees(CSng(facingInRad))
        SetFacing(DegreesToFacing(KernsMath.AngleConvert360(CSng(facingInDeg))))
    End Sub

    Public Overridable Shadows Sub Update(gt As GameTime)
        If CurrentMana < CurrentMaxMana Then
            CurrentMana += CSng(gt.ElapsedGameTime.Milliseconds / 1000) * CurrentManaRegeneration
        End If

        If IsNothing(AIAction) Then
            AIAction = New EntityActionStand(Me)
        End If
        AIAction.increment(gt.ElapsedGameTime)

        animation.Update(gt)
        'If CurrentHealth <= 0 And Alive Then
        '    RaiseEvent OnDeath(New System.EventArgs(), Me)
        '    Alive = False
        'End If
    End Sub

    Public Function DegreesToFacing(degValue As Single) As FacingTypes

        Select Case degValue
            Case 0 To 22.5
                Return FacingTypes.SOUTHWEST
            Case 22.5 To 67.5
                Return FacingTypes.SOUTH
            Case 67.5 To 112.5
                Return FacingTypes.SOUTHEAST
            Case 112.5 To 157.5
                Return FacingTypes.EAST
            Case 157.5 To 202.5
                Return FacingTypes.NORTHEAST
            Case 202.5 To 247.5
                Return FacingTypes.NORTH
            Case 247.5 To 292.5
                Return FacingTypes.NORTHWEST
            Case 292.5 To 337.5
                Return FacingTypes.WEST
            Case 337.5 To 360
                Return FacingTypes.SOUTHWEST
            Case Else
                DebugManagement.WriteLineToLog("Someone tried to set Facing to a value that doesn't exist... Reverting to North.", SeverityLevel.WARNING)
                Return FacingTypes.NORTH
        End Select


    End Function
    'Public Overridable Sub SetDrawColor(drawColor As Color)
    '    defaultAnimation.Color = drawColor
    'End Sub
    Public Overridable Sub SetFacing(FacingToSet As FacingTypes)
        'If Not IsNothing(defaultAnimation) Then
        '    defaultAnimation.SetFacing(FacingToSet)
        'End If
        animation.SetFacing(FacingToSet)
    End Sub
    Public Sub StartAnimation(anim As EntityLivingAnimationSet.Animations, length As TimeSpan)
        animation.StartAnimation(anim, length)
    End Sub
    Public Sub ContinueAnimation(anim As EntityLivingAnimationSet.Animations, length As TimeSpan)
        If animation.CurrentAnimation <> anim Then animation.StartAnimation(anim, length)
    End Sub
    'Public Sub SetAnimations(AnimationName As String, allowFinish As Boolean)
    '    'Set animations for all the parts
    '    'Shield and Weapon may be nothing, but Head and Body had better exist! We can gloss over the bottom two- they may or may not be loaded.

    '    If Not IsNothing(defaultAnimation) Then
    '        defaultAnimation.SetAnimation(AnimationName, allowFinish)
    '    End If
    'End Sub
    ''' <summary>
    ''' Old function used to prevent the player from clicking on himself.
    ''' </summary>
    ''' <param name="targetPoint"></param>
    ''' <param name="ScaleFactor"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Function IsTooCloseToPoint(targetPoint As Point, ScaleFactor As Single) As Boolean
        If ScaleFactor <= 0 Then
            DebugManagement.WriteLineToLog("Someone tried to pass an invalid value to AnimationMarginCheck! Reverting to 1.0.", SeverityLevel.WARNING)
            ScaleFactor = 1
        End If
        If New Rectangle(CInt(position.X + entitySize.Width / ScaleFactor), CInt(position.Y + entitySize.Height / ScaleFactor), CInt(entitySize.Width / ScaleFactor), CInt(entitySize.Height / ScaleFactor)).Contains(targetPoint) Then
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' Returns a vector2 indicating where the rectangles 
    ''' </summary>
    ''' <param name="pointAndTileSizeA"></param>
    ''' <param name="pointAndTileSizeB"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CalculateFacingWhenClose(pointAndTileSizeA As Rectangle, pointAndTileSizeB As Rectangle) As Vector2
        Dim pointResult As Vector2 = Vector2.Zero
        'This way we can edit each component individually. Much spacial reasoning. Very good.
        If pointAndTileSizeA.Contains(pointAndTileSizeB) Then
            'Oops. They're on top of each other.


        End If
        If pointAndTileSizeB.Bottom <= pointAndTileSizeA.Top Then
            pointResult.Y = 1

        End If
        If pointAndTileSizeB.Top >= pointAndTileSizeA.Bottom Then
            pointResult.Y = -1

        End If
        If pointAndTileSizeB.Left >= pointAndTileSizeA.Right Then
            pointResult.X = 1
        End If
        If pointAndTileSizeB.Right <= pointAndTileSizeA.Left Then
            pointResult.X = -1
        End If
        Return pointResult
    End Function
    ''' <summary>
    ''' Adds a new modifier that will be applied during the player's Draw loop.
    ''' The maxStackCount variable is used to determine the number of times duplicate modifiers can be applied.
    ''' Use large numbers to imply infinity.
    ''' </summary>
    ''' <param name="maxStackCount">The maximum number of times a modifier can be applied on a player.</param>
    ''' <param name="Modifier">The IModifierSet object to apply.</param>
    ''' <remarks></remarks>
    Public Sub AddModifier(maxStackCount As Integer, Modifier As IModifierSet)
        If IsNothing(Modifier) Then
            DebugManagement.WriteLineToLog("Someone passed an empty value to EntityLiving.AddModifier!", SeverityLevel.SEVERE)
            Exit Sub
        End If
        If CountDuplicateModifierStacks(Modifier.ModifierIdentifier) < maxStackCount Then
            ModifierList.Add(Modifier)
        End If
    End Sub
    Public Function CountDuplicateModifierStacks(name As String) As Integer
        Dim finalCount As Integer
        For i = 0 To ModifierList.Count - 1 Step 1
            If ModifierList(i).ModifierIdentifier = name Then
                finalCount += 1
            End If
        Next
        Return finalCount
    End Function
    Protected Sub ApplyModifiers(gt As GameTime)

        For i = 0 To ModifierList.Count - 1 Step 1
            Try
                ModifierList(i).Update(gt)
                If Not ModifierList(i).HasAppliedModifiers Then
                    ModifierList(i).ApplyModifiers(Me)
                End If
                If ModifierList(i).CD.isDone Then
                    ModifierList(i).UnApplyModifiers(Me)
                    ModifierList.RemoveAt(i)
                    i = -1
                End If
            Catch ex As Exception

            End Try


        Next
    End Sub
    ''' <summary>
    ''' See documentation/implementation for IModifierSet. Code is almost the same.
    ''' </summary>
    ''' <param name="maxStackCount"></param>
    ''' <param name="Effect"></param>
    ''' <remarks></remarks>
    Public Sub AddVisualEffect(maxStackCount As Integer, Effect As VisualEffect)
        If IsNothing(Effect) Then
            DebugManagement.WriteLineToLog("Someone passed an empty value to EntityLiving.AddVisualEffect!", SeverityLevel.SEVERE)
            Exit Sub
        End If
        If CountDuplicateModifierStacks(Effect.ID) < maxStackCount Then
            VisualEffects.Add(Effect)
        End If
    End Sub
    Public Function CountDuplicateVisualEffectStacks(name As String) As Integer
        Dim finalCount As Integer
        For i = 0 To ModifierList.Count - 1 Step 1
            If ModifierList(i).ModifierIdentifier = name Then
                finalCount += 1
            End If
        Next
        Return finalCount
    End Function
    Protected Overloads Sub ApplyVisualEffects(gt As GameTime, sb As SpriteBatch, Pos As Vector3, Cam As IsoCamera)

        For i = 0 To VisualEffects.Count - 1 Step 1
            VisualEffects(i).Update(gt)
            If VisualEffects(i).CD.isExpired() Then
                VisualEffects.RemoveAt(i)
                i = -1
            End If
            VisualEffects(i).Draw(sb, Pos, Cam)
        Next
    End Sub
End Class



