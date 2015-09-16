''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class UIOverlay

#Region "Variable Delcarations"
    Private CurrentCursorTexture As Texture2D
    'The texture currently loaded into memory for the cursor. Changes as animations do.
    Private Cursors() As Texture2D 'Fixed length array of cursors.

    Private MousePointerSize As Size

    Private MousePointerColor As Color = Color.LightSlateGray
    Private WithEvents CurrentPanel As Panel

    Private CurrentPanelIndex As PanelTypes
    Private Panels() As Panel

    Private DebugWriter As DebugScreenWriter

    Private ShowDebugInfo As Boolean = True 'Whether or not to draw text to the screen indicating FPS, performance, etc

    Private defaultFont As SpriteFont

    Public ScreenSize As Size

    Dim currentLoadingSteps As Integer

    Public Const maxLoadingSteps As Integer = 500
    Dim PlayerHotkeySet As HotKeyHandler

    Public PlayerSkillTree As SkillTree

    Public PlayerInventory As fullInventory

    Public Party As PartyInfo

    Public Shop As ShopWindow

    Public Quests As QuestWindow
    Public PlayerInfoWindow As PlayerInfo
    Public WithEvents Chat As ChatBox
    Public WithEvents RecentMessages As FormattedTextProcessor
    Private PlayerDeadFont As SpriteFont
    Private PlayerDeadFontSmall As SpriteFont
    Private PlayerDeathScreenLocation As Vector2
    Private CurrentDeadFlavorText As String
    Private PossibleDeadFlavorText As List(Of String)
    Public UpdateChangedDeadText As Boolean = True
    Private ActiveFloatingText As List(Of FloatingFadingText)
#End Region
#Region "Constructor"
    Public Sub New(gd As GraphicsDevice, CM As ContentManager, EM As EntityManagement, defaultCursorIndex As Integer, screenSize As Size, gameLoadingLoops As Integer, defaultFontAsset As String, Optional debugFontAsset As String = "defaultFont") 'Loads all the cursors the game uses, in order. Cursors are enumerated.


        Me.ScreenSize = screenSize
        SetupCursors(defaultCursorIndex, CM)
        SetupPanels(CM, gameLoadingLoops)
        SetupSoundLib(CM)
        'Setup DebugWriter to draw info to the screen.
        Me.defaultFont = CM.Load(Of SpriteFont)(defaultFontAsset)

        AddDebugHotkeys(EM)

        Dim debugFont As SpriteFont = CM.Load(Of SpriteFont)(AssetManager.RequestAsset(debugFontAsset, AssetTypes.SPRITEFONT))

        DebugWriter = New DebugScreenWriter(debugFont, New Vector2(0, 0))

        Dim buttonPaths(2) As String
        buttonPaths(0) = AssetManager.RequestAsset("firewall")
        buttonPaths(1) = AssetManager.RequestAsset("firewall")
        buttonPaths(2) = AssetManager.RequestAsset("firewall")

        PlayerSkillTree = New SkillTree(CM, CM.Load(Of Texture2D)(AssetManager.RequestAsset("parchmentexpanded")), CM.Load(Of Texture2D)(AssetManager.RequestAsset("dialogborder")), CM.Load(Of Texture2D)(AssetManager.RequestAsset("LockedSkill")), buttonPaths, AssetManager.RequestAsset("dialogborder"), "defaultFont", gd.Viewport)
        Party = New PartyInfo(New Rectangle(0, 0, screenSize.Width, screenSize.Height), CM)
        Dim SellingItems() As Item = {New Item(New Size(20, 20), CM.Load(Of Texture2D)(AssetManager.RequestAsset("orbFull")), 1, ItemTypes.rod), New Item(New Size(20, 20), CM.Load(Of Texture2D)(AssetManager.RequestAsset("orbFull")), 1, ItemTypes.rod), New Item(New Size(20, 20), CM.Load(Of Texture2D)(AssetManager.RequestAsset("orbFull")), 1, ItemTypes.rod), New Item(New Size(20, 20), CM.Load(Of Texture2D)(AssetManager.RequestAsset("orbFull")), 1, ItemTypes.rod)}
        Shop = New ShopWindow(EM.LocalPlayerInfo.InitialPlayerData, SellingItems, New Rectangle(0, 0, screenSize.Width, screenSize.Height), CM, debugFont, CM.Load(Of Texture2D)(AssetManager.RequestAsset("parchmentexpanded")))
        Quests = New QuestWindow(screenSize.ToRectangle(), CM)
        EM.CooldownList.AddCooldown(500, "Open Party Info")
        EM.CooldownList.AddCooldown(500, "Open Skill Tree")

        PlayerInventory = New FullInventory(CM.Load(Of SpriteFont)(AssetManager.RequestAsset("defaultFont")), New String() {AssetManager.RequestAsset("Test"), AssetManager.RequestAsset("helloWorld"), AssetManager.RequestAsset("Test")}, CM, gd.Viewport.Bounds)

        SetPanel(screenSize, PanelTypes.GAME_OVERLAY)
        Dim defaultSpells(7) As String
        Dim i As Integer
        For i = 0 To defaultSpells.Length - 1 Step 1
            defaultSpells(i) = AssetManager.RequestAsset("firewall")
        Next

        Chat = New ChatBox(CM, New Rectangle(0, 0, gd.Viewport.Bounds.Width, gd.Viewport.Bounds.Height))
        PlayerInfoWindow = New PlayerInfo(gd.Viewport.Bounds, CM)
        EM.CooldownList.AddCooldown(500, "Open PlayerInfo")
        RecentMessages = New FormattedTextProcessor(Chat.LoadedFont)
        CurrentPanel.SetupSpellHotbar(defaultSpells, AssetManager.RequestAsset("defaultFont"), CM, gd)
        PlayerDeadFont = CM.Load(Of SpriteFont)(AssetManager.RequestAsset("DeadFontLarge"))
        PlayerDeadFontSmall = CM.Load(Of SpriteFont)(AssetManager.RequestAsset("DeadFontSmall"))
        PossibleDeadFlavorText = New List(Of String)
        SetupDeadFlavorText()
        PlayerDeathScreenLocation = New Vector2(gd.Viewport.Bounds.Center.X, gd.Viewport.Bounds.Center.Y)
        ActiveFloatingText = New List(Of FloatingFadingText)
    End Sub
    Private Sub SetupCursors(defaultCursorIndex As Integer, CM As ContentManager)
        Dim cursorPaths(1) As String
        cursorPaths(0) = AssetManager.RequestAsset("cursorBase")
        cursorPaths(1) = AssetManager.RequestAsset("cursorGrabbing")
        ReDim Cursors(cursorPaths.Length - 1) 'The length of the cursor array should match the parameters we pass to New.
        If defaultCursorIndex > Cursors.Length - 1 Then
            defaultCursorIndex = Cursors.Length - 1 'Prevent accidental setting to cursor indicies that do not exist.
        End If
        For i = 0 To Cursors.Length - 1 Step 1
            Cursors(i) = CM.Load(Of Texture2D)(cursorPaths(i))

        Next

        CurrentCursorTexture = Cursors(defaultCursorIndex)
        'We now have an array of cursors.

        CursorSize = New Size()
        CursorSize.Height = 32
        CursorSize.Width = 32
    End Sub
    Private Sub SetupSoundLib(CM As ContentManager)
        'Load the interface sounds the easyway
        For i = 1 To 6 Step 1
            SoundLibrary.AddSound("interface" + i.ToString(), AssetManager.RequestAsset("interface" + i.ToString(), AssetTypes.SOUNDEFFECT), CM, False)

        Next

    End Sub
    Private Sub SetupDeadFlavorText()
        PossibleDeadFlavorText.Add("Don't get blood on the floor.")
        PossibleDeadFlavorText.Add("Pity.")
        PossibleDeadFlavorText.Add("You've decorated the ground with your internal organs!")
        PossibleDeadFlavorText.Add("These monsters have to get their gear somehow.")
        PossibleDeadFlavorText.Add("You could always get some ChineseLunch.")
        PossibleDeadFlavorText.Add("I'm guessing your IPad won't turn on, too.")
        PossibleDeadFlavorText.Add("Have you tried hitting it until it stops moving?")


    End Sub
    Private Sub AddDebugHotkeys(EM As EntityManagement)
        PlayerHotkeySet = New HotKeyHandler()
        Dim fireBallAction As FireballLaunchAction
        fireBallAction = New FireballLaunchAction()
        PlayerHotkeySet.addKey(Keys.Q, fireBallAction)
        PlayerHotkeySet.addKey(Keys.Q, fireBallAction)
        EM.CooldownList.AddCooldown(1000, "FireballCast")
        EM.CooldownList.AddCooldown(1000, "ChargeAction")
        PlayerHotkeySet.addKey(Keys.W, New ChargeAction())
        PlayerHotkeySet.addKey(Keys.E, New FireNovaLaunchAction())
        EM.CooldownList.AddCooldown(3000, "FireNovaCast")
        PlayerHotkeySet.leftMouseClick = New AttackAction()
        PlayerHotkeySet.addKey(Keys.T, New SkillTreeAction())
        PlayerHotkeySet.addKey(Keys.I, New InventoryAction())
        PlayerHotkeySet.addKey(Keys.P, New PartyInfoAction())
        PlayerHotkeySet.addKey(Keys.Escape, New RespawnAction())
        PlayerHotkeySet.addKey(Keys.Y, New QuestInfoAction())
        PlayerHotkeySet.addKey(Keys.M, New PlayerInfoAction())
    End Sub
#End Region
#Region "Properties"
    ''' <summary>
    ''' Retrieves the currently active panel. Useful for checking values of buttons and such.
    ''' </summary>
    ''' <returns>CurrentPanel, the object currently being actively drawn in the frame.</returns>
    ''' <remarks></remarks>
    Public ReadOnly Property ActivePanel As Panel
        Get
            Return CurrentPanel
        End Get
    End Property
    Public ReadOnly Property PanelIndex As PanelTypes
        Get
            Return CurrentPanelIndex
        End Get
    End Property
    Public ReadOnly Property Cursor As Texture2D
        Get
            Return CurrentCursorTexture
        End Get

    End Property
    Public Property CursorSize As Size
        Get
            Return MousePointerSize
        End Get
        Set(value As Size)
            MousePointerSize = value
        End Set
    End Property
    Public Property DebugMode As Boolean
        Get
            Return ShowDebugInfo
        End Get
        Set(value As Boolean)
            ShowDebugInfo = value
        End Set
    End Property
    Public Property Hotkeys As HotKeyHandler
        Get
            Return PlayerHotkeySet
        End Get
        Set(value As HotKeyHandler)
            PlayerHotkeySet = value
        End Set
    End Property
#End Region


#Region "Methods"

    Public Overloads Sub setCursor(cursorIndex As Integer)
        If cursorIndex > Cursors.Length - 1 Then
            cursorIndex = Cursors.Length - 1
        End If
        If cursorIndex < 0 Then
            cursorIndex = 0
        End If
        CurrentCursorTexture = Cursors(cursorIndex)

    End Sub
    Public Overloads Sub SetCursor(inTexture As Texture2D)
        CurrentCursorTexture = inTexture
    End Sub
    ''' <summary>
    ''' Entry point for drawing UI.
    ''' </summary>
    ''' <param name="sb">Spritebatch used to draw various objects</param>
    ''' <param name="CM">ContentManager used to load any objects. May be unneccessary.</param>
    ''' <param name="ms">MouseState used to determine the position of the mouse</param>
    ''' <param name="ks">KeyboardState used to determine whether or not to highlight keys.</param>
    ''' <remarks></remarks>
    '''

    Public Sub drawUI(sb As SpriteBatch, CM As ContentManager, ms As MouseState, ks As KeyboardState, gt As GameTime, EM As EntityManagement, Cam As IsoCamera)
        'Entry point for drawing all the various parts of the UI. Should be as refactored as possible.
        'If the game is still loading, process loading loop information.

        HandleLoadingConditionals(sb, CM)
        CurrentPanel.DrawAll(sb, ScreenSize, ms, gt, PlayerHotkeySet, EM, ks, CM, Me, Cam)
        PlayerInventory.draw(sb, ms)
        If CurrentPanelIndex = PanelTypes.GAME_OVERLAY Then
            PlayerSkillTree.Draw(sb, ms, EM.LocalPlayerInfo.InitialPlayerData)
            Party.Draw(sb)
            Shop.Draw(sb)
            PlayerInfoWindow.Update(sb, EM.LocalPlayerInfo)
   
        End If
        UpdateMobHealthBar(ms, EM, Cam)
        Chat.Draw(sb)
        Quests.Draw(sb, ms, gt)
        RecentMessages.draw(sb)
        UpdateDeadText(sb, EM)
        UpdateFloatingDamageText(gt, sb)
        drawCursor(sb, ms)
      
    End Sub
    Public Sub UpdateUI(gt As GameTime, LocalPlayer As EntityPlayer, ms As MouseState, ks As KeyboardState)
        SetHealthBar(CInt(LocalPlayer.CurrentHealth), CInt(LocalPlayer.CurrentMaxHealth), 0)
        SetManaBar(CInt(LocalPlayer.CurrentMana), CInt(LocalPlayer.CurrentMaxMana), 0)

        PlayerSkillTree.Update(ms, ks, gt, LocalPlayer.InitialPlayerData)
        PlayerInventory.updateInventory(ms, gt)
        Party.Update()
        Shop.Update()
        RecentMessages.update(gt)

        GarbageCollectCompletedFloatingFadingTexts()
    End Sub
    Public Function isUIclicked(ms As MouseState) As Boolean
        Return CurrentPanel.isPanelClicked Or PlayerSkillTree.isPanelClicked(ms)
    End Function
    Private Sub UpdateFloatingDamageText(gt As GameTime, sb As SpriteBatch)
        For Each value In ActiveFloatingText
            value.Update(gt, sb)
        Next
    End Sub
    Public Sub UpdateHotkeyState(EM As EntityManagement, gt As GameTime, ms As MouseState, ks As KeyboardState, CM As ContentManager, UI As UIOverlay, cam As IsoCamera)
        If Not Chat.Active Then
            PlayerHotkeySet.runActions(EM, gt, ms, ks, CM, UI, cam)
        End If
    End Sub
    Private Const DeathMessage As String = "You have died. Press %KEY% to continue."
    Private Const KeyValue As Keys = Keys.Escape
    ''' <summary>
    ''' Decides whether or not to draw the "You are dead" message
    ''' </summary>
    ''' <param name="EM">EntityManagement instance</param>
    ''' <remarks></remarks>
    Private Sub UpdateDeadText(sb As SpriteBatch, EM As EntityManagement)
        If EM.LocalPlayerInfo.Alive = False Then
            'Oops. Looks like you're dead.
            If UpdateChangedDeadText Then
                UpdateChangedDeadText = False
                CurrentDeadFlavorText = PossibleDeadFlavorText(KernsMath.RandInt(0, PossibleDeadFlavorText.Count - 1))

            End If
            Dim finalTopMsg As String = DeathMessage.Replace("%KEY%", KeyValue.ToString())
            Dim finalTopMsgSize As Vector2 = PlayerDeadFont.MeasureString(finalTopMsg)
            Dim finalTopMsgLocation = PlayerDeathScreenLocation - finalTopMsgSize
            finalTopMsgLocation.X += CInt(finalTopMsgSize.X / 2)
            sb.DrawString(PlayerDeadFont, DeathMessage.Replace("%KEY%", KeyValue.ToString()), finalTopMsgLocation, Color.Black)
            Dim bottomMsgSize As Vector2 = PlayerDeadFontSmall.MeasureString(CurrentDeadFlavorText)
            Dim bottomMsgLocation As Vector2 = PlayerDeathScreenLocation
            bottomMsgLocation.Y += CInt(finalTopMsgSize.Y)
            bottomMsgLocation.X -= CInt(bottomMsgSize.X / 2)
            sb.DrawString(PlayerDeadFontSmall, CurrentDeadFlavorText, bottomMsgLocation, Color.Black)
        End If
    End Sub
    Private Const Index_Of_Mob_Prog_Bar As Integer = 2
    Private Sub UpdateMobHealthBar(ms As MouseState, EM As EntityManagement, Cam As IsoCamera)
        Dim SelectionResult As String = EM.HilightedEntity(ms, Cam)
        Dim SelectedEntity As EntityLiving = EM.GetMobByID(SelectionResult)
        If IsNothing(SelectedEntity) Then
            If CurrentPanelIndex = PanelTypes.GAME_OVERLAY Then
                CurrentPanel.ProgressBars(Index_Of_Mob_Prog_Bar).Visible = False

            End If
        ElseIf TypeOf SelectedEntity Is Mob Then
            'Wouldn't want to make the progress bar appear for non-mob types
            Dim visible As Boolean = SelectedEntity.CurrentHealth > 0

            
            CurrentPanel.ProgressBars(Index_Of_Mob_Prog_Bar).Visible = visible
            CurrentPanel.ProgressBars(Index_Of_Mob_Prog_Bar).Setup(0, CInt(CType(SelectedEntity, Mob).MaxHealth))
            CurrentPanel.ProgressBars(Index_Of_Mob_Prog_Bar).SetPercentage(CType(SelectedEntity, Mob).CurrentHealth / CType(SelectedEntity, Mob).MaxHealth)
            CurrentPanel.ProgressBars(Index_Of_Mob_Prog_Bar).Title = SelectedEntity.Name
            CurrentPanel.ProgressBars(Index_Of_Mob_Prog_Bar).WriteCurrentValueAsTitle = True
        End If



    End Sub

    ''' <summary>
    ''' Draws information related to debugging information to the screen.
    ''' </summary>
    ''' <param name="sb"></param>
    ''' <param name="GD"></param>
    ''' <param name="ks"></param>
    ''' <param name="ms"></param>
    ''' <param name="gt"></param>
    ''' <param name="game"></param>
    ''' <remarks></remarks>
    Public Sub DrawGameDebugInfo(EM As EntityManagement, sb As SpriteBatch, GD As GraphicsDevice, ks As KeyboardState, ms As MouseState, gt As GameTime, game As Game1, localPlayer As EntityPlayer, Cam As IsoCamera, frmLocation As System.Drawing.Point, gc As GarbageCollector, map As Map)
        If ShowDebugInfo Then
            DebugWriter.WriteGameDebugInfo(EM, sb, GD, ks, ms, Me, gt, game, localPlayer, Cam, frmLocation, gc, map)
        End If


    End Sub
    ''' <summary>
    ''' Refactored code that handles loading screen draw logic.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub HandleLoadingConditionals(sb As SpriteBatch, CM As ContentManager)
        If CurrentPanelIndex = PanelTypes.LOADING_SCREEN Then
            If CurrentPanel.ProgressBars(0).isDone = False Then
                LoadingBarPanelUpdate(sb, CM)
                UpdateCurrentPanelProgressBars(1)
            Else
                SetPanel(ScreenSize, PanelTypes.MAIN_MENU)

            End If

        End If
    End Sub

    Public Sub UpdateCurrentPanelProgressBars(incrementAmount As Integer)
        CurrentPanel.UpdateProgressBars(incrementAmount)
    End Sub
    Private Sub drawCursor(sb As SpriteBatch, ms As MouseState)
        Dim mouseRect As Rectangle
        mouseRect = New Rectangle(CInt(ms.X), CInt(ms.Y), MousePointerSize.Width, MousePointerSize.Height)
        sb.Draw(CurrentCursorTexture, mouseRect, MousePointerColor)
    End Sub
    Public Sub SetHealthBar(current As Integer, maximum As Integer, minimum As Integer)
        If CurrentPanelIndex = PanelTypes.GAME_OVERLAY Then
            'Can only set the health when in the game overlay.
            CurrentPanel.ProgressBars(0).MinValue = minimum
            CurrentPanel.ProgressBars(0).MaxValue = maximum
            CurrentPanel.ProgressBars(0).SetPercentage(current / maximum)

        End If
    End Sub
    Public Sub SetManaBar(current As Integer, maximum As Integer, minimum As Integer)
        If CurrentPanelIndex = PanelTypes.GAME_OVERLAY Then
            'Can only set the health when in the game overlay.
            CurrentPanel.ProgressBars(1).MinValue = minimum
            CurrentPanel.ProgressBars(1).MaxValue = maximum
            CurrentPanel.ProgressBars(1).SetPercentage(current / maximum)

        End If
    End Sub
    ''' <summary>
    ''' Adds a new floatingFadingText which will run until it fades entirely.
    ''' </summary>
    ''' <param name="location">The top left corner to begin drawing at.</param>
    ''' <param name="message">The message to be displayed.</param>
    ''' <param name="duration">The duration in MS to keep the image. This will also affect how fast it fades.</param>
    ''' <param name="CM">ContentManager instance</param>
    ''' <param name="drawColor">The color to draw the image as. Changing the A value will NOT do anything.</param>
    ''' <param name="FontAsset">The asset to use for the font.</param>
    ''' <param name="ShiftTextUpwards">Whether or not to shift the text progressively upwards as the image fades. Default true.</param>
    ''' <param name="ShiftMultiplier">The maximum height to shift (measured in the height of the string; for example, a value of 3 would shift it by 3 * the height of the message). Default 3</param>
    ''' <remarks></remarks>
    Public Sub AddFloatingFadingText(location As Vector2, message As String, duration As Integer, CM As ContentManager, drawColor As Color, Optional FontAsset As String = "defaultFont", Optional ShiftTextUpwards As Boolean = True, Optional ShiftMultiplier As Single = 3.0)
        ActiveFloatingText.Add(New FloatingFadingText(location, message, duration, CM.Load(Of SpriteFont)(AssetManager.RequestAsset(FontAsset, AssetTypes.SPRITEFONT)), drawColor, ShiftTextUpwards, ShiftMultiplier))
    End Sub
    Private Sub GarbageCollectCompletedFloatingFadingTexts()


    End Sub
#Region "Panel Default Constants"
    'Panel default data should never be changed except here.
    ''' <summary>
    ''' This method will set up the panel based on specific values.
    ''' </summary>
    ''' <param name="screenSize">The size of the screen, in pixels.</param>
    ''' <param name="Type">The PanelType to load. PanelTypes are explicitly defined and contain a series of default acceptable panel values.  These organize things like each object's screen position.</param>
    ''' <remarks>At some point, values for each panel should be serialized. For now, we're going to do it here. Remember to run this method if the window is resized.</remarks>
    Public Sub SetPanel(screenSize As Size, Type As PanelTypes)
        Me.ScreenSize = screenSize
        CurrentPanel = Panels(Type)
        CurrentPanelIndex = Type
    End Sub
    Private Sub SetupPanels(CM As ContentManager, gameLoadLoops As Integer)
        'ButtonPaths is static, and represents all the button paths.
        'The plan is eventually to store each Panel object as an XML file. This method is temporary. That being said, Panels don't really need to be loaded from a file. They'll never change.

        Dim buttonPaths(2) As String
        buttonPaths(0) = AssetManager.RequestAsset("buttonHighLight")
        buttonPaths(1) = AssetManager.RequestAsset("buttonNormal")
        buttonPaths(2) = AssetManager.RequestAsset("buttonPressed")


        Const TOTAL_PANELS As Integer = 12 'Change this if you intend to add an additional panel
        'I know this code is shit. As soon as all the panels are complete, I can remove this scaffolding and save it as an XML.


        ReDim Preserve Panels(TOTAL_PANELS - 1)
        Dim loadingScreenProgBars(0) As ProgressBar
        Dim emptyTexture, fullTexture As Texture2D
        emptyTexture = CM.Load(Of Texture2D)(AssetManager.RequestAsset("defaultProgressBar"))
        fullTexture = CM.Load(Of Texture2D)(AssetManager.RequestAsset("defaultProgressBarFull"))
        Dim loadingBarSize As Size
        loadingBarSize = New Size(CInt(ScreenSize.Width * 0.5), CInt(ScreenSize.Height * 0.1))
        Dim loadingBarPosition As Vector2
        loadingBarPosition = New Vector2(CInt(ScreenSize.Width * 0.25), CInt(ScreenSize.Height * 0.8))
        loadingScreenProgBars(0) = New ProgressBar(emptyTexture, fullTexture, loadingBarSize, loadingBarPosition, 0, gameLoadLoops)
        'Indicates how many times the ProgressBar in the loading screen will run. Handle the LoadingLoopComplete event to perform actions during the loading phase.

        'Used for loading new content after the panels have been initialized.
        Dim loadingScreenLogo(0) As MobileTexture
        Dim loadingScreenTexture As Texture2D
        loadingScreenTexture = CM.Load(Of Texture2D)(AssetManager.RequestAsset("GameLogo"))
        Dim loadingLogoRectangle As Rectangle
        loadingLogoRectangle = New Rectangle(CInt(ScreenSize.Width * 0.25), CInt(ScreenSize.Height * 0.1), CInt(ScreenSize.Width * 0.5), CInt(ScreenSize.Height * 0.6))
        loadingScreenLogo(0) = New MobileTexture(loadingScreenTexture, loadingLogoRectangle)

        Panels(0) = New Panel(Nothing, loadingScreenProgBars, Nothing, loadingScreenLogo)





        Dim MainMenuButtons(2) As Button

        MainMenuButtons(0) = New Button("Play Multiplayer", buttonPaths, AssetManager.RequestAsset("defaultFont"), CM, True, 0.3, 0.4)
        MainMenuButtons(0).Size = New Size(CInt(ScreenSize.Width * 0.4), CInt(ScreenSize.Height * 0.2))
        MainMenuButtons(0).position = New Vector2(CInt(ScreenSize.Width * 0.3), CInt(ScreenSize.Height * 0.15))

        MainMenuButtons(1) = New Button("Play Local Game", buttonPaths, AssetManager.RequestAsset("defaultFont"), CM, True, 0.3, 0.4)

        MainMenuButtons(1).Size = New Size(CInt(ScreenSize.Width * 0.4), CInt(ScreenSize.Height * 0.2))
        MainMenuButtons(1).position = New Vector2(CInt(ScreenSize.Width * 0.3), CInt(ScreenSize.Height * 0.35))

        MainMenuButtons(2) = New Button("Game Options", buttonPaths, AssetManager.RequestAsset("defaultFont"), CM, True, 0.3, 0.4)
        MainMenuButtons(2).Size = New Size(CInt(ScreenSize.Width * 0.4), CInt(ScreenSize.Height * 0.2))
        MainMenuButtons(2).position = New Vector2(CInt(ScreenSize.Width * 0.3), CInt(ScreenSize.Height * 0.55))
        Panels(1) = New Panel(MainMenuButtons, Nothing, Nothing, Nothing)

        'Main menu panel complete.

        Dim SinglePlayerMenuButtons(1) As Button

        SinglePlayerMenuButtons(0) = New Button("Join/Create A Local Game", buttonPaths, AssetManager.RequestAsset("defaultFont"), CM, True, 0.3, 0.4)
        SinglePlayerMenuButtons(0).Size = New Size(CInt(ScreenSize.Width * 0.4), CInt(ScreenSize.Height * 0.2))
        SinglePlayerMenuButtons(0).position = New Vector2(CInt(ScreenSize.Width * 0.3), CInt(ScreenSize.Height * 0.15))

        SinglePlayerMenuButtons(1) = New Button("Back", buttonPaths, AssetManager.RequestAsset("defaultFont"), CM, True, 0.3, 0.4)

        SinglePlayerMenuButtons(1).Size = New Size(CInt(ScreenSize.Width * 0.2), CInt(ScreenSize.Height * 0.2))
        SinglePlayerMenuButtons(1).position = New Vector2(CInt(0), CInt(ScreenSize.Height * 0.8))
        Panels(2) = New Panel(SinglePlayerMenuButtons, Nothing, Nothing, Nothing)

        'Single player menu (for playing unauthenticated single player lobbies)



        Dim GameOverlayHealthBars(2) As VerticalProgressBar
        GameOverlayHealthBars(0) = New VerticalProgressBar(CM.Load(Of Texture2D)(AssetManager.RequestAsset("healthOrbEmpty")), _
                                    CM.Load(Of Texture2D)(AssetManager.RequestAsset("healthOrbFull")), New Size(CInt(ScreenSize.Width * 0.13), CInt(ScreenSize.Width * 0.13)), New Vector2(CInt(ScreenSize.Width - ScreenSize.Width * 0.8), _
                                    ScreenSize.Height - CInt(ScreenSize.Height * 0.2)))
        GameOverlayHealthBars(0).Setup(0, 1000)
        GameOverlayHealthBars(0).BackColor = New Color(Color.Gray.R, Color.Gray.G, Color.Gray.B, Color.Gray.A)

        GameOverlayHealthBars(0).Center = New Vector2(CInt(ScreenSize.Width * 0.1), CInt(ScreenSize.Height * 0.85))
        GameOverlayHealthBars(0).Color = Color.Red
        GameOverlayHealthBars(0).SetPercentage(0.05)
        GameOverlayHealthBars(1) = New VerticalProgressBar(CM.Load(Of Texture2D)(AssetManager.RequestAsset("healthOrbEmpty")), _
                                    CM.Load(Of Texture2D)(AssetManager.RequestAsset("healthOrbFull")), New Size(CInt(ScreenSize.Width * 0.13), CInt(ScreenSize.Width * 0.13)), New Vector2(CInt(ScreenSize.Width * 0.2), _
                                    ScreenSize.Height - CInt(ScreenSize.Height * 0.2)))
        GameOverlayHealthBars(1).Setup(0, 1000)
        GameOverlayHealthBars(1).BackColor = New Color(Color.Gray.R, Color.Gray.G, Color.Gray.B, Color.Gray.A)

        GameOverlayHealthBars(1).Color = Color.Blue
        GameOverlayHealthBars(1).Center = New Vector2(ScreenSize.Width - CInt(ScreenSize.Width * 0.1), CInt(ScreenSize.Height * 0.85))
        GameOverlayHealthBars(0).Paused = True
        GameOverlayHealthBars(1).Paused = True
        GameOverlayHealthBars(1).SetPercentage(0.05)
        GameOverlayHealthBars(0).ShowTitle = True
        GameOverlayHealthBars(1).ShowTitle = True
        GameOverlayHealthBars(0).WriteCurrentValueAsTitle = True
        GameOverlayHealthBars(1).WriteCurrentValueAsTitle = True
        GameOverlayHealthBars(0).FontColor = Color.Black
        GameOverlayHealthBars(1).FontColor = Color.Black
        GameOverlayHealthBars(0).Font = CM.Load(Of SpriteFont)(AssetManager.RequestAsset("defaultFont", AssetTypes.SPRITEFONT))
        GameOverlayHealthBars(1).Font = CM.Load(Of SpriteFont)(AssetManager.RequestAsset("defaultFont", AssetTypes.SPRITEFONT))
        GameOverlayHealthBars(0).MouseHoverShowsTitle = True
        GameOverlayHealthBars(1).MouseHoverShowsTitle = True
        Dim SubMitProgressBars(3) As ProgressBar
        'SubMitProgressBars(0) = GameOverlayHealthBars(0)
        'SubMitProgressBars(1) = GameOverlayHealthBars(1)
        'SubMitProgressBars(2) = GameOverlayHealthBars(2)
        'SubMitProgressBars(3) = expBar(0)
        Array.Copy(GameOverlayHealthBars, SubMitProgressBars, 2)
        SubMitProgressBars(2) = New ProgressBar(CM.Load(Of Texture2D)(AssetManager.RequestAsset("defaultProgressBar")), CM.Load(Of Texture2D)(AssetManager.RequestAsset("defaultProgressBarFull")), New Size(CInt(ScreenSize.Width * 0.35), CInt(ScreenSize.Height * 0.1)), New Vector2(0, 0))
        SubMitProgressBars(2).Font = CM.Load(Of SpriteFont)(AssetManager.RequestAsset("defaultFont", AssetTypes.SPRITEFONT))
        SubMitProgressBars(2).Center = New Vector2(CInt(ScreenSize.Width * 0.5), CInt(ScreenSize.Height * 0.1))
        SubMitProgressBars(2).ShowTitle = True
        SubMitProgressBars(2).Title = "Monster Name"


        SubMitProgressBars(3) = New ProgressBar(CM.Load(Of Texture2D)(AssetManager.RequestAsset("defaultProgressBar")), CM.Load(Of Texture2D)(AssetManager.RequestAsset("defaultProgressBarFull")), New Size(CInt(ScreenSize.Width * 0.35), CInt(ScreenSize.Height * 0.05)), New Vector2(0, 0))
        SubMitProgressBars(3).Font = CM.Load(Of SpriteFont)(AssetManager.RequestAsset("defaultFont", AssetTypes.SPRITEFONT))
        SubMitProgressBars(3).Center = New Vector2(CInt(ScreenSize.Width * 0.5), CInt(ScreenSize.Height * 0.915))
        SubMitProgressBars(3).ShowTitle = True
        SubMitProgressBars(3).WriteCurrentValueAsTitle = True
        SubMitProgressBars(3).Setup(0, 1000)
        SubMitProgressBars(3).SetPercentage(0.05)
        SubMitProgressBars(3).BackColor = Color.Black
        SubMitProgressBars(3).Color = Color.SlateGray

        Panels(PanelTypes.GAME_OVERLAY) = New Panel(Nothing, SubMitProgressBars, Nothing, Nothing)

        'GameOverlayHealthBars

    End Sub
    ''' <summary>
    ''' Manages the events from each 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub HandlePanelTransitions()

    End Sub
    ''' <summary>
    ''' Call in Initialize to load content one loop at a time, while drawing and updating everything in between. This allows the game to load one step at a time.
    ''' </summary>
    ''' <param name="sb">SpriteBatch instance.</param>
    ''' <param name="CM">ContentManager instance</param>
    ''' <remarks></remarks>
    ''' 

    Private Sub LoadingBarPanelUpdate(sb As SpriteBatch, CM As ContentManager)
        'Raises the event for Game1 to process loading events. This prevents clutter and will also stop raising the event when it's done.
        RaiseEvent LoadLoopComplete(CurrentPanel.ProgressBars(0).Current)

    End Sub



    Public Event LoadLoopComplete(currentStep As Integer)
    Public Event LoadingComplete()

    ''' <summary>
    ''' Handles how Panels process button events and what to do when a button from a specific panel is pressed.
    ''' </summary>
    ''' <param name="pressedButton"></param>
    ''' <remarks></remarks>
    Private Sub HandlePanelButtonEvents(pressedButton As Button) Handles CurrentPanel.ButtonPressed
        'There has to be a better way to do this.
        If CurrentPanelIndex = PanelTypes.MAIN_MENU Then
            If pressedButton.Name = "Play Local Game" Then
                SetPanel(ScreenSize, PanelTypes.PLAY_LOCAL_GAME)
            End If
            If pressedButton.Name = "Play Multiplayer" Then

            End If
        End If
        If CurrentPanelIndex = PanelTypes.PLAY_LOCAL_GAME Then
            If pressedButton.Name = "Back" Then
                SetPanel(ScreenSize, PanelTypes.MAIN_MENU)
            End If
        End If
    End Sub

#End Region
#End Region
End Class
Public Enum Cursors
    POINTING = 0
    OPENHAND = 1
    GRABBING = 2
End Enum
Public Enum PanelTypes
    LOADING_SCREEN
    MAIN_MENU
    PLAY_LOCAL_GAME
    PLAY_LOCAL_GAME_SELECT_PROFILE
    PLAY_LOCAL_GAME_LOBBY_LIST
    PLAY_LOCAL_GAME_CREATE_NEW_LOBBY
    PLAY_MULTIPLAYER_PREAUTH
    PLAY_MULTIPLAYER_POSTAUTH
    PLAY_MULTIPLAYER_LOBBY_LIST
    PLAY_MULTIPLAYER_CREATE_NEW_LOBBY
    PLAY_MULTIPLAYER_JOINING_GAME
    GAME_OVERLAY


End Enum