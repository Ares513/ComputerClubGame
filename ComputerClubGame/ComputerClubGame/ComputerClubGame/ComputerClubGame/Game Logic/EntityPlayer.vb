''' <summary>
''' Defines all of the traits the current game session needs to know about the player.
''' Information is loaded from the Player class.
''' </summary> 
''' <remarks></remarks>
Public Class EntityPlayer
    Inherits EntityLiving
    Public InitialPlayerData As Player
    Private Range As Integer
    Private MeleeRange As Single 'Range in tiles of the player's melee attack.

    Public LastPathingDuration As Single
    Protected LocalPlayer As Boolean

#Region "Properties"
    Public Overrides ReadOnly Property Visible As Boolean
        Get
            If Alive Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property

    Public ReadOnly Property isLocalPlayer As Boolean
        Get

            Return LocalPlayer
        End Get

    End Property
    Public Sub MakeLocal()
        LocalPlayer = True
    End Sub
    Public ReadOnly Property AttackRange As Single
        Get
            Return MeleeRange
        End Get
    End Property
    Public Overrides Property Name As String
        Get
            Return CharacterName
        End Get
        Set(value As String)
            CharacterName = value
        End Set
    End Property
    

#End Region


#Region "Constructor"
    ''' <summary>
    ''' Creates a New EntityPlayer instance.
    ''' </summary>
    ''' <param name="defaultSize">The Size of the entity</param>
    ''' <param name="inputPlayer">PlayerData loaded from local profile or cloud info.</param>
    ''' <param name="CM">ContentManager instance.</param>
    ''' <remarks></remarks>
    Public Sub New(ID As String, defaultSize As Size, inputPlayer As Player, CM As ContentManager, em As EntityManagement)
        MyBase.New(ID, defaultSize, inputPlayer.getPlayerHealth, em)
        InitialPlayerData = inputPlayer
        CharacterName = inputPlayer.Name
        OverheadBoxBackground = CM.Load(Of Texture2D)(AssetManager.RequestAsset("defaultProgressBarFull"))
        OverheadDrawFont = CM.Load(Of SpriteFont)(AssetManager.RequestAsset("overheadFont", AssetTypes.SPRITEFONT))
        nameBackgroundColor = New Color(0, 0, 0, 100)
        nameForegroundColor = Color.LightSlateGray
        Process(inputPlayer, CM)
        CurrentMana = 50
        CurrentMaxHealth = 250
        CurrentHealth = 250
        CurrentMaxMana = 400
        CurrentManaRegeneration = 30
        MeleeRange = 4
        MovementSpeed = 0.008
        BaseType = EntityBaseType.Player

    End Sub
#End Region
    '#Region "Events"
    '    Public Event DamageDealtToGeneric(sender As EntityPlayer, e As System.EventArgs, target As EntityLiving)
    '    Public Event DamageDealtToMob(sender As EntityPlayer, e As System.EventArgs, target As Mob)
    '    Public Event DamageDealtToPlayer(sender As EntityPlayer, e As System.EventArgs, target As EntityPlayer)
    '#End Region
#Region "Data Processing"
    Private Sub Process(inputPlayerData As Player, CM As ContentManager)
        InitialPlayerData = inputPlayerData
        Dim AnimationDefinitionDefaults(6) As AnimationDefinition
        AnimationDefinitionDefaults(0) = New AnimationDefinition(0, 3, "Stand", 150)
        AnimationDefinitionDefaults(1) = New AnimationDefinition(4, 12, "Running", 50)
        AnimationDefinitionDefaults(2) = New AnimationDefinition(13, 17, "Attack", 50)
        AnimationDefinitionDefaults(3) = New AnimationDefinition(18, 20, "Block", 50)
        AnimationDefinitionDefaults(4) = New AnimationDefinition(21, 24, "Death", 50)
        AnimationDefinitionDefaults(5) = New AnimationDefinition(27, 31, "Cast Spell", 50)
        AnimationDefinitionDefaults(6) = New AnimationDefinition(32, 36, "Shoot Projectile", 50)
        Select Case inputPlayerData.getClassName

            Case "Knight"
                animation = New EntityLivingAnimationSet()
                Dim BodyTexture, HeadTexture, WeaponTexture As Texture2D
                BodyTexture = CM.Load(Of Texture2D)(AssetManager.RequestAsset("noArmorMaleBody"))
                HeadTexture = CM.Load(Of Texture2D)(AssetManager.RequestAsset("KnightHead"))
                WeaponTexture = CM.Load(Of Texture2D)(AssetManager.RequestAsset("Longsword"))
                Dim StandAnim As EntityMultiAnimationSet =
                    New EntityMultiAnimationSet(HeadTexture, BodyTexture, WeaponTexture,
                                                New Size(128, 128), AnimationDefinitionDefaults(0), New Vector2(64, 96))
                Dim WalkAnim As EntityMultiAnimationSet =
                    New EntityMultiAnimationSet(HeadTexture, BodyTexture, WeaponTexture,
                                                New Size(128, 128), AnimationDefinitionDefaults(1), New Vector2(64, 96))
                Dim MeleeAnim As EntityMultiAnimationSet =
                    New EntityMultiAnimationSet(HeadTexture, BodyTexture, WeaponTexture,
                                                New Size(128, 128), AnimationDefinitionDefaults(2), New Vector2(64, 96))
                Dim DyingAnim As EntityMultiAnimationSet =
                    New EntityMultiAnimationSet(HeadTexture, BodyTexture, WeaponTexture,
                                                New Size(128, 128), AnimationDefinitionDefaults(4), New Vector2(64, 96))
                Dim DeadAnim As EntityMultiAnimationSet =
                    New EntityMultiAnimationSet(HeadTexture, BodyTexture, WeaponTexture,
                                                New Size(128, 128), New AnimationDefinition(23, 23, "Dead", 50), New Vector2(64, 96))
                animation.Anims(EntityLivingAnimationSet.Animations.Stand) = StandAnim
                animation.Anims(EntityLivingAnimationSet.Animations.Walk) = WalkAnim
                animation.Anims(EntityLivingAnimationSet.Animations.Melee) = MeleeAnim
                animation.Anims(EntityLivingAnimationSet.Animations.Die) = DyingAnim
                animation.Anims(EntityLivingAnimationSet.Animations.Dead) = DeadAnim
                'Eventually this will be loaded from the EquippedItem the player is using.
                TileHitBox = New Vector2(-1, -1)
            Case Else
                DebugManagement.WriteLineToLog("Someone tried to load a class that doesn't exist! The program will not continue", SeverityLevel.FATAL)
                Throw New ApplicationException("Someone tried to load a class that doesn't exist! The program will not continue.")
        End Select
        'Based on which class, we load data regarding the player data differently.

    End Sub
#End Region

    Public Sub Attack()
        isAttacking = True
    End Sub
    Public Shadows Sub Draw(sb As SpriteBatch, gt As GameTime, Cam As IsoCamera, isLocal As Boolean)
        'Change ALL property values BEFORE THIS CALL! Changing property values (movement speed, life, etc)
        'may alter how modifiers behave.
        ApplyModifiers(gt)

        Dim stringInfo As Vector2 = OverheadDrawFont.MeasureString(CharacterName)
        Dim strLoc, location As Vector2

        location = Cam.MapToScreen(Pos)
        Dim layer As Single = Cam.ScaleLayer(Pos.X + Pos.Y + Pos.Z)
        strLoc = location
        strLoc.X += Size.Width
        strLoc.X -= stringInfo.X
        strLoc.Y -= CSng(Size.Height / 2)

        strLoc -= New Vector2(stringInfo.X, stringInfo.Y)
        'PICARD: WHAT THE HELL IS THIS SHIT
        'Use the goddamn center function
        Dim screenPos As Vector2 = Cam.MapToScreen(Pos)
        Dim Rect As Rectangle = New Rectangle(CInt(screenPos.X), CInt(screenPos.Y), Size.Width, Size.Height)
        Dim nameSize As Vector2 = OverheadDrawFont.MeasureString(CharacterName)
        Dim boxRectangle As Rectangle = New Rectangle(0, 0, CInt(nameSize.X), CInt(nameSize.Y))
        Dim Ymod As Double = 0
        boxRectangle.X = CInt(Rect.Center.X - (nameSize.X / 2) - 65)
        boxRectangle.Y = CInt(Rect.Y - (nameSize.Y / 2) - 60 + Ymod)

        sb.Draw(OverheadBoxBackground, boxRectangle, nameBackgroundColor)
        sb.DrawString(OverheadDrawFont, CharacterName, New Vector2(boxRectangle.X, boxRectangle.Y), nameForegroundColor)

        animation.Draw(sb, location, layer)
        End Sub
    Public Shadows Sub Move(ms As MouseState, gt As GameTime)
        Dim sw As Stopwatch = New Stopwatch()
        sw.Start()
        
        LastPathingDuration = sw.Elapsed.Milliseconds

        sw.Stop()
    End Sub
    ''' <summary>
    ''' Sets the velocity from the MouseState and properly adjusts the facing to reflect
    ''' </summary>
    ''' <remarks></remarks>
    Private oldms As MouseState = New MouseState(0, 0, 0, ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released)
    Public Shadows Sub UpdateOrders(ms As MouseState, Cam As IsoCamera, useOffset As Boolean, ignoreMouseState As Boolean)
        '  If ignoreMouseState Or (ms.LeftButton = ButtonState.Pressed And oldms.LeftButton = ButtonState.Released) Then
        Dim target As NavLoc = map.GetWalkableLoc(Cam.ScreenToWorldRay(New Vector2(ms.X, ms.Y)))
        If Not IsNothing(target) Then
            MyBase.UpdateOrders(target)
        End If

        '    SetAnimations("Running", True)




        oldms = ms
    End Sub
    Public ReadOnly Property getRange As Integer
        Get
            Return Range
        End Get
    End Property
    
End Class
