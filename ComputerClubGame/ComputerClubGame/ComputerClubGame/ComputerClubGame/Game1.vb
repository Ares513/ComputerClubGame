
Imports System.Windows.Forms

''' <summary>
''' This is the main type for your game
''' </summary>
''' <remarks>
''' The cursor is 48x48.
''' </remarks>
'''
Public Class Game1

    Inherits Microsoft.Xna.Framework.Game

    Private WithEvents graphics As GraphicsDeviceManager
    Private WithEvents spriteBatch As SpriteBatch

#Region "Declared Variables"
    ''' <summary>
    ''' Please, dear God, keep this list clear and commented. Add line breaks. Avoid wall of text syndrome.
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Dim CurrentMap As Map

    Dim WithEvents UI As UIOverlay
    Dim WithEvents Entities As EntityManagement
    Private Const UIOverlayLoadingScreenLoopCount As Integer = 50
    Dim Assets As AssetManager

    Dim oldMouseState, newMouseState As MouseState
    Dim oldKeyboardState, newKeyboardState As KeyboardState
    Dim GameCam As IsoCamera
    Dim frm As Form
    Dim GarbageCollection As GarbageCollector
    Dim WithEvents MessageFilter As InputMessageFilter
    Dim Background As Texture2D
#Region "Unit Testing"
    Dim TestFriendlyEntity As EntityFriendly
    Dim UnitTestFFT As FloatingFadingText
    Dim UnitTestDialogBox As Dialog

#End Region
#End Region
















    Public Sub New()
        graphics = New GraphicsDeviceManager(Me)

        Content.RootDirectory = "Content"
        Assets = New AssetManager()
        Assets.LoadAssets(Content)

        ' Dim testMobData() As GameDataLibrary.MobDataDescription
        'testMobData = Content.Load(Of GameDataLibrary.MobDataDescription())("Data Specifications/MobDefinitions")
        'testMobData(0).MobName = "Test"
        graphics.PreferredBackBufferWidth = Screen.PrimaryScreen.WorkingArea.Width - (SystemInformation.Border3DSize.Width * 2)
        graphics.PreferredBackBufferHeight = Screen.PrimaryScreen.WorkingArea.Height - SystemInformation.CaptionHeight - SystemInformation.Border3DSize.Height

        graphics.ApplyChanges()
        frm = CType(System.Windows.Forms.Control.FromHandle(Me.Window.Handle), Form)
        frm.Location = New System.Drawing.Point(0, 0)

        Mouse.WindowHandle = Me.Window.Handle
        GarbageCollection = New GarbageCollector(1000)
        MessageFilter = New InputMessageFilter()

        System.Windows.Forms.Application.AddMessageFilter(MessageFilter)

    End Sub
    ''' <summary>
    ''' Allows the game to perform any initialization it needs to before starting to run.
    ''' This is where it can query for any required services and load any non-graphic
    ''' related content.  Calling MyBase.Initialize will enumerate through any components
    ''' and initialize them as well.
    ''' </summary>
    ''' 

    Protected Overrides Sub Initialize()
        ' TODO: Add your initialization logic here
        MyBase.Initialize()
        Assets.fileSafetyCheck(Me, Content)
        GameCam = New IsoCamera()


        'Now we can generate whole classes just fine.
        Dim plrClass As PlayerClass = PlayerClassFactory.Generate(PlayerClassType.Knight)
        Dim IntegrationTestPlayerData(1) As Player
        IntegrationTestPlayerData(0) = New Player(New PlayerRace(), plrClass, New PlayerAttribute() {}, New PlayerGuild(), New Skill() {}, "Meow")
        'IntegrationTestPlayerData(0) = New Player()
        IntegrationTestPlayerData(1) = New Player(New PlayerRace(), plrClass, New PlayerAttribute() {}, New PlayerGuild(), New Skill() {}, "Cat")
        'IntegrationTestPlayerData(1) = New Player()
        Entities = New EntityManagement(IntegrationTestPlayerData, False, Content, CurrentMap)
        UI = New UIOverlay(GraphicsDevice, Content, Entities, 0, New Size(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), UIOverlayLoadingScreenLoopCount, AssetManager.RequestAsset("defaultFont"), "debugFont")

        'Dim anim As IsoAnimation
        Dim animDefs(4) As AnimationDefinition
        animDefs(0) = New AnimationDefinition(0, 4, "Stand", 128)
        animDefs(1) = New AnimationDefinition(5, 15, "Jump", 128)
        animDefs(2) = New AnimationDefinition(15, 20, "Running", 128)
        animDefs(3) = New AnimationDefinition(21, 25, "Attack", 128)
        animDefs(4) = New AnimationDefinition(26, 30, "Throw", 128)

        Dim goblinTex As Texture2D = Content.Load(Of Texture2D)(AssetManager.RequestAsset("goblinSpearmanLiveWalkAttack"))
        Dim goblinDeathTex As Texture2D = Content.Load(Of Texture2D)(AssetManager.RequestAsset("goblinSpearmanDeath"))

        'animDefs(1).MilisecondsPerFrame = 30
        'animDefs(2).MilisecondsPerFrame = 16
        'anim = New IsoAnimation(Content.Load(Of Texture2D)(AssetManager.RequestAsset("goblinSpearmanLiveWalkAttack")), New Size(128, 128), animDefs, 0, New Microsoft.Xna.Framework.Rectangle(0, 0, 128, 128), New Vector2(64, 96))
        Dim deathAnimDefs(0) As AnimationDefinition
        deathAnimDefs(0) = New AnimationDefinition(0, 8, "Death", 50)
        'Dim deathAnim As New IsoAnimation(Content.Load(Of Texture2D)(AssetManager.RequestAsset("goblinSpearmanDeath")), New Size(128, 128), deathAnimDefs, 0, New Microsoft.Xna.Framework.Rectangle(0, 0, 128, 128), New Vector2(64, 96))

        Dim animations As EntityLivingAnimationSet = New EntityLivingAnimationSet()
        Dim defs(0) As AnimationDefinition
        defs(0) = animDefs(0)
        animations.Anims(EntityLivingAnimationSet.Animations.Stand) =
            New IsoAnimation(goblinTex, New Size(128, 128), defs, 0, New Rectangle(0, 0, 0, 0), New Vector2(64, 96))
        defs(0) = animDefs(2)
        animations.Anims(EntityLivingAnimationSet.Animations.Walk) =
            New IsoAnimation(goblinTex, New Size(128, 128), defs, 0, New Rectangle(0, 0, 0, 0), New Vector2(64, 96))
        defs(0) = animDefs(3)
        animations.Anims(EntityLivingAnimationSet.Animations.Melee) =
            New IsoAnimation(goblinTex, New Size(128, 128), defs, 0, New Rectangle(0, 0, 0, 0), New Vector2(64, 96))
        defs(0) = deathAnimDefs(0)
        animations.Anims(EntityLivingAnimationSet.Animations.Die) =
            New IsoAnimation(goblinDeathTex, New Size(128, 128), defs, 0, New Rectangle(0, 0, 0, 0), New Vector2(64, 96))
        defs(0) = New AnimationDefinition(7, 7, "Dead", 50)
        animations.Anims(EntityLivingAnimationSet.Animations.Dead) =
            New IsoAnimation(goblinDeathTex, New Size(128, 128), defs, 0, New Rectangle(0, 0, 0, 0), New Vector2(64, 96))

        Dim Mob As New Mob(Content, animations, True, Entities)
        Dim mob2 As New Mob(Content, animations, True, Entities)
        'Mob.SetAnimations("Stand", False)
        Mob.SetFacing(FacingTypes.SOUTH)
        mob2.SetFacing(FacingTypes.SOUTH)

        Entities.AddMob(mob2)
        Entities.AddMob(Mob)
        GameCam.AlignScreenCenter(Entities.LocalPlayerInfo.Center(GameCam), GraphicsDevice.Viewport)
        Dim iconAshHets(7) As String
        Dim i As Integer
        For i = 0 To iconAshHets.Length - 1 Step 1
            iconAshHets(i) = AssetManager.RequestAsset("firewall")
        Next


        UI.PlayerSkillTree.LoadSkills(Entities.LocalPlayerInfo.InitialPlayerData, Content)
        Background = Content.Load(Of Texture2D)(AssetManager.RequestAsset("FoggyBackground"))

        TestFriendlyEntity = New EntityFriendly(IDGenerator.Generate(New Random()), New Size(32, 64), EntityFriendly.FriendlyEntityReference.Crazy_Hasaad, Content, New Vector3(5, 5, 0), Entities)
        'UnitTestFormattedTP = New FormattedTextProcessor(Content.Load(Of SpriteFont)(AssetManager.RequestAsset("overheadFont", AssetTypes.SPRITEFONT)))

        Mouse.SetPosition(0, 0)


        UnitTestDialogBox = New Dialog(GraphicsDevice.Viewport.Bounds, Content.Load(Of Texture2D)(AssetManager.RequestAsset("parchmentExpanded")), Content, "Test", New FireballLaunchAction())
        UnitTestFFT = New FloatingFadingText(New Vector2(600, 300), "HelloWorld", 2500, Content.Load(Of SpriteFont)(AssetManager.RequestAsset("defaultFont")), Color.Black, True, 5.0)

        Entities.DroppedItems.AddItem(New DroppedItem(New Vector3(0, 0, 2), "TEST SWORD", AssetManager.RequestAsset("shortsword"), False, Content, CurrentMap, New Item(New Size(50, 50), Content.Load(Of Texture2D)(AssetManager.RequestAsset("greatsword")), 100, ItemTypes.greatsword)))
    End Sub
    ''' <summary>
    ''' This method will run automatically whenever UI reports that the game is done loading.
    ''' </summary>
    ''' <remarks>We show the mouse cursor here.</remarks>
    Protected Sub LoadingComplete() Handles UI.LoadingComplete
        InitializationSettings.SetupSettings(Me)

    End Sub
    ''' <summary>
    ''' LoadContent will be called once per game and is the place to load
    ''' all of your content.
    ''' </summary>
    Protected Overrides Sub LoadContent()
        ' Create a new SpriteBatch, which can be used to draw textures.
        spriteBatch = New SpriteBatch(GraphicsDevice)
        CurrentMap = Content.Load(Of Map)("World/Maps/map2")

        ' TODO: use Me.Content to load your game content here
    End Sub

    ''' <summary>
    ''' UnloadContent will be called once per game and is the place to unload
    ''' all content.
    ''' </summary>
    Protected Overrides Sub UnloadContent()
        ' TODO: Unload any non ContentManager content here
    End Sub

    ''' <summary>
    ''' Allows the game to run logic such as updating the world,
    ''' checking for collisions, gathering input, and playing audio.
    ''' </summary>
    ''' <param name="gameTime">Provides a snapshot of timing values.</param>
    Protected Overrides Sub Update(ByVal gameTime As GameTime)

        ' TODO: Add your update logic here

        newKeyboardState = Keyboard.GetState()
        oldKeyboardState = newKeyboardState
        newMouseState = Mouse.GetState()
        oldMouseState = newMouseState

        UI.UpdateHotkeyState(Entities, gameTime, oldMouseState, oldKeyboardState, Content, UI, GameCam)
        UI.UpdateUI(gameTime, Entities.LocalPlayerInfo, newMouseState, newKeyboardState)

        Entities.Update(newMouseState, gameTime, GameCam, Not UI.isUIclicked(newMouseState), newKeyboardState, Content, UI)


        TestFriendlyEntity.Update(gameTime)

        Mouse.WindowHandle = Me.Window.Handle

        GarbageCollection.Update(Entities)
        MyBase.Update(gameTime)

    End Sub
    ''' <summary>
    ''' This is called when the game should draw itself.
    ''' </summary>
    ''' <param name="gameTime">Provides a snapshot of timing values.</param>
    Protected Overrides Sub Draw(ByVal gameTime As GameTime)
        'Draw order: UI last (to draw on top of everything else)

        GameCam.AlignScreenCenter(Entities.LocalPlayerInfo.Pos, GraphicsDevice.Viewport)
        GameCam.MinLayer = -10
        GameCam.MaxLayer = CurrentMap.MaxLayer + 10

        GraphicsDevice.Clear(Color.DarkSlateGray)


        spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend) ' draw sort by z
        spriteBatch.Draw(Background, GraphicsDevice.Viewport.Bounds, Color.White)
        CurrentMap.Draw(spriteBatch, GameCam)
        TestFriendlyEntity.Draw(spriteBatch, GameCam)
        Entities.Draw(spriteBatch, oldMouseState, gameTime, GameCam)
        spriteBatch.End()

        spriteBatch.Begin()
        UI.DrawGameDebugInfo(Entities, spriteBatch, GraphicsDevice, oldKeyboardState, oldMouseState, gameTime, Me, Entities.LocalPlayerInfo, GameCam, Control.MousePosition, GarbageCollection, CurrentMap)

        UnitTestDialogBox.drawText(spriteBatch, GraphicsDevice.Viewport.Bounds)
        UnitTestDialogBox.Update(Entities, gameTime, newMouseState, newKeyboardState, Content, UI, GameCam)

        UI.drawUI(spriteBatch, Content, oldMouseState, oldKeyboardState, gameTime, Entities, GameCam)
        UI.UpdateCurrentPanelProgressBars(1)
        UnitTestFFT.Update(gameTime, spriteBatch)
        spriteBatch.End()

        MyBase.Draw(gameTime)

    End Sub
    'Private Sub ItemPickedFromDroppedItemMgmt(sender As DroppedItemManagement, e As EventArgs, pickedUpItem As DroppedItem) Handles Entities.ItemWasPickedUp
    '    UI.PlayerInventory.addItem(pickedUpItem.getItem, content)

    'End Sub
    Private Sub KeyDown(sender As Object, key As Char, backSpace As Boolean) Handles MessageFilter.OnKeyDown
        If key = "c"c And Not UI.Chat.Active Then
            'Opened the chat window.

            UI.Chat.Active = True
            UI.Chat.EnteredText = ""
            MessageFilter.ConsumeKeyDownMessages = True
        Else
            UI.Chat.EnteredText += key.ToString()
        End If

    End Sub
    Private Sub ReturnPressed(sender As Object, e As EventArgs) Handles MessageFilter.ReturnPressed
        'Message sent.
        UI.RecentMessages.addLine(UI.Chat.EnteredText)
        UI.Chat.EnteredText = ""
        MessageFilter.ConsumeKeyDownMessages = False
        UI.Chat.Active = False

    End Sub
    Private Sub BackSpacePressed(sender As Object, e As EventArgs) Handles MessageFilter.BackspacePressed
        If UI.Chat.EnteredText.Length > 0 Then
            UI.Chat.EnteredText = UI.Chat.EnteredText.Remove(UI.Chat.EnteredText.Length - 1)

        End If
    End Sub
    Private Sub CancelPressed(sender As Object, e As EventArgs) Handles MessageFilter.CancelPressed
        UI.Chat.Active = False
        UI.Chat.EnteredText = ""

    End Sub
    Protected Overrides Sub OnExiting(sender As Object, args As System.EventArgs)
        MyBase.OnExiting(sender, args)

    End Sub
    Private Sub EntityDamaged(crit As Boolean, message As String, dmg As Damage, center As Vector3) Handles Entities.DamageDone
        Dim drawColor As Color = Color.Black
        If crit Then
            drawColor = Color.Red
            message += "!"
        End If
        UI.AddFloatingFadingText(GameCam.MapToScreen(center), message, CInt(4000), Content, drawColor, "defaultFont", True, 4.0)
    End Sub
End Class

