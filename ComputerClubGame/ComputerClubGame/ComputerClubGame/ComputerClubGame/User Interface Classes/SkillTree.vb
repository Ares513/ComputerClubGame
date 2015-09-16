Public Class SkillTree
    Inherits SlidingWindow
    Private backgroundTexture As Texture2D
    Private tabTexture As Texture2D

    Private skillTabs(2) As SkillTreeTab
    Private ActiveTabIndex As SkillTreeTabTypes
    Private TabSelectionArea As Rectangle
    Private RegularFont As SpriteFont
    Private ItalicFont As SpriteFont
    Private BoldFont As SpriteFont
    Private TabRectangles(2) As Rectangle
    Private SpentSkillPointsOnLastLoop As Boolean
    'Prevent the player from spam-spending their points.
    Private ValidateSpellHotbar As Boolean

    Public CurrentPlayerSkillPoints As Integer
    Public ReadOnly Property NeedToValidateSpellHotbar As Boolean
        Get
            Return ValidateSpellHotbar
        End Get
    End Property

    'The number of skill points the player crently has available to spend.
    Public ReadOnly Property TreeLocationState As SkillTreeLocationTypes
        Get
            Return state
        End Get
    End Property
    Public ReadOnly Property isPanelClicked(ms As MouseState) As Boolean
        Get
            Dim workingRectangle As Rectangle = New Rectangle(CInt(workingArea.X + currentOffset.X), CInt(workingArea.Y + currentOffset.Y), workingArea.Width, workingArea.Height)

            Return workingRectangle.Contains(New Point(ms.X, ms.Y))

        End Get

    End Property

    'Used to display the player's skill info.
    Public Sub New(CM As ContentManager, backTexture As Texture2D, inputButtonBackTexture As Texture2D, inputLockedSkillTexture As Texture2D, buttonAssetsFromAssetManager As String(), popupAssetFromAssetManager As String, fontAsset As String, screen As Viewport, Optional openSpeed As Single = 30, Optional italicFontAsset As String = "SkillTreeFontItalic", Optional boldFontAsset As String = "SkillTreeFontBold")
        MyBase.New(screen.Bounds, False, False)
        RegularFont = CM.Load(Of SpriteFont)(AssetManager.RequestAsset(fontAsset, AssetTypes.SPRITEFONT))
        ItalicFont = CM.Load(Of SpriteFont)(AssetManager.RequestAsset(italicFontAsset, AssetTypes.SPRITEFONT))
        BoldFont = CM.Load(Of SpriteFont)(AssetManager.RequestAsset(boldFontAsset, AssetTypes.SPRITEFONT))

        backgroundTexture = backTexture
        Dim viewportSetup As Rectangle = New Rectangle(0, 0, CInt(screen.Width / 2), screen.Height)
        workingArea = viewportSetup

        currentOffset.X = -workingArea.Width

        TabSelectionArea = New Rectangle(workingArea.X, workingArea.Y, CInt(workingArea.Width / 4), workingArea.Height)
        Dim treeArea As Rectangle = New Rectangle(workingArea.X + TabSelectionArea.Width, workingArea.Y, workingArea.Width - TabSelectionArea.Width, workingArea.Height)
        skillTabs(SkillTreeTabTypes.OFFENSE) = New SkillTreeTab(CM, buttonAssetsFromAssetManager, popupAssetFromAssetManager, fontAsset, treeArea, "Test Tab", inputButtonBackTexture, inputLockedSkillTexture, italicFontAsset, boldFontAsset)
        skillTabs(SkillTreeTabTypes.DEFENSE) = New SkillTreeTab(CM, buttonAssetsFromAssetManager, popupAssetFromAssetManager, fontAsset, treeArea, "Test Tab 2", inputButtonBackTexture, inputLockedSkillTexture, italicFontAsset, boldFontAsset)
        skillTabs(SkillTreeTabTypes.SUPPORT) = New SkillTreeTab(CM, buttonAssetsFromAssetManager, popupAssetFromAssetManager, fontAsset, treeArea, "Test Tab 3", inputButtonBackTexture, inputLockedSkillTexture, italicFontAsset, boldFontAsset)
        ActiveTabIndex = SkillTreeTabTypes.OFFENSE
        Me.openSpeed = openSpeed
        For i = 0 To 2 Step 1
            TabRectangles(i) = New Rectangle(TabSelectionArea.X, TabSelectionArea.Y + CInt(i * (TabSelectionArea.Height / 3)), TabSelectionArea.Width, CInt(TabSelectionArea.Height / 3))
        Next

        tabTexture = CM.Load(Of Texture2D)(AssetManager.RequestAsset("SkillTreeTab"))

    End Sub
    Public Sub LoadSkills(InitialPlayerData As Player, CM As ContentManager)
        Dim i As SkillTreeTabTypes
        'Apologies for the enum. Deals with the compiler warning. Think of them as ints.
        For i = 0 To CType(skillTabs.Length - 1, SkillTreeTabTypes) Step SkillTreeTabTypes.DEFENSE
            skillTabs(i).LoadSkills(InitialPlayerData.playerClass.SkillTree(i).CoreSkill, InitialPlayerData.playerClass.SkillTree(i).LeftSideSkills, InitialPlayerData.playerClass.SkillTree(i).RightSideSkills, CM)
        Next
    End Sub
    Public Sub ToggleSkillTreeOpening()
        ToggleOpening()

    End Sub
    Public Sub Draw(sb As SpriteBatch, ms As MouseState, LocalPlayer As Player)
        sb.Draw(backgroundTexture, New Rectangle(CInt(workingArea.X + currentOffset.X), workingArea.Y, workingArea.Width, workingArea.Height), Color.White)
        For i = 0 To 2 Step 1
            Dim message As String = ""
            Select Case i
                Case SkillTreeTabTypes.OFFENSE
                    message = "Offense"
                Case SkillTreeTabTypes.DEFENSE
                    message = "Defense"
                Case SkillTreeTabTypes.SUPPORT
                    message = "Support"
            End Select
            Dim pos As Vector2
            Dim size As Vector2 = RegularFont.MeasureString(message)
            'Center the middle of the text on the tab.
            'pos = New Vector2(currentOffset.X + TabSelectionArea.X + CInt(TabSelectionArea.Width / 2) - CInt(size.X / 2), currentOffset.Y + TabSelectionArea.Y + CSng((i + 1) * TabSelectionArea.Height / 3.75)) '+ CSng(i * (TabSelectionArea.Height / 3)
            pos = New Vector2(TabRectangles(i).Center.X - (RegularFont.MeasureString(message).X / 2) + currentOffset.X, TabRectangles(i).Center.Y - RegularFont.MeasureString(message).Y + currentOffset.Y)

            Dim updatedRectangle As Rectangle = New Rectangle(CInt(TabRectangles(i).X + currentOffset.X), CInt(TabRectangles(i).Y + currentOffset.Y), TabRectangles(i).Width, TabRectangles(i).Height)
            If i = ActiveTabIndex Then
                sb.Draw(tabTexture, updatedRectangle, Color.Yellow)
            Else
                sb.Draw(tabTexture, updatedRectangle, Color.White)
            End If

            sb.DrawString(RegularFont, message, pos, Color.Black)

        Next

        skillTabs(ActiveTabIndex).Draw(sb, currentOffset, ms, LocalPlayer, ActiveTabIndex, CurrentPlayerSkillPoints)

        CheckSliding()
    End Sub
    Public Sub Update(ms As MouseState, ks As KeyboardState, gt As GameTime, LocalPlayer As Player)
        CheckSwitchTabs(ms)
        UpdateUnderDrawnSkillForActiveTree(LocalPlayer)
        Dim result As SkillTreeTabReturn
        result = skillTabs(ActiveTabIndex).Update(gt, ms, ks, CurrentPlayerSkillPoints)




        If Not IsNothing(result.clickedButton) Then
            If result.buttonState = ButtonReturnTypes.HOVERING Then
                'A button is hovered. 
                HandleHotkeyPresses(ks, result)
            End If
        End If





        If Not IsNothing(skillTabs(ActiveTabIndex)) Then
            CurrentPlayerSkillPoints = LocalPlayer.SkillPoints
          



            If Not IsNothing(result.clickedButton) Then
                If result.buttonState = ButtonReturnTypes.CLICKED Then


                    'A button was pressed!
                    'The SkillTreeTabReturn will tell us which index was pressed. Check the LocalPlayer's skill data.
                    'First check the core
                    If result.isCore Then
                        'The core button was pressed.
                        If LocalPlayer.playerClass.SkillTree(ActiveTabIndex).CoreSkill.MeetsRequirements(LocalPlayer.Level) Then
                            If LocalPlayer.CanTakeSkillPoint Then
                                LocalPlayer.TakeSkillPoint()
                                LocalPlayer.playerClass.SkillTree(ActiveTabIndex).CoreSkill.RankSkill(LocalPlayer.Level)
                                ValidateTreeAndPlaySound(LocalPlayer)
                                'Done early.
                                Exit Sub
                            End If
                        End If

                    End If



                    If result.LeftOrRight Then
                        'Right side.
                        If CheckSkillRequirementsRight(LocalPlayer, result, ActiveTabIndex) Then
                            LocalPlayer.TakeSkillPoint()
                            LocalPlayer.playerClass.SkillTree(ActiveTabIndex).RightSideSkills(result.Index).RankSkill(LocalPlayer.Level)
                            ValidateTreeAndPlaySound(LocalPlayer)
                        End If
                    Else
                        'Left side
                        If CheckSkillRequirementsLeft(LocalPlayer, result, ActiveTabIndex) Then
                            LocalPlayer.TakeSkillPoint()
                            LocalPlayer.playerClass.SkillTree(ActiveTabIndex).LeftSideSkills(result.Index).RankSkill(LocalPlayer.Level)

                            ValidateTreeAndPlaySound(LocalPlayer)
                            'And that's what you need to do to get the UI to reflect a learned skill.
                        End If

                    End If


                Else
                    SpentSkillPointsOnLastLoop = False
                End If
            Else
                SpentSkillPointsOnLastLoop = False
            End If
        End If




    End Sub
    Private Sub HandleHotkeyPresses(ks As KeyboardState, result As SkillTreeTabReturn)
        If ks.IsKeyDown(Keys.Q) Then
            ClearDuplicateHotkeys(Keys.Q)
            SetHotkeyText(Keys.Q, result)

        End If
        If ks.IsKeyDown(Keys.W) Then
            ClearDuplicateHotkeys(Keys.W)
            SetHotkeyText(Keys.W, result)

        End If
        If ks.IsKeyDown(Keys.E) Then
            ClearDuplicateHotkeys(Keys.E)
            SetHotkeyText(Keys.E, result)
        End If
        If ks.IsKeyDown(Keys.R) Then
            ClearDuplicateHotkeys(Keys.R)
            SetHotkeyText(Keys.R, result)
        End If
        If ks.IsKeyDown(Keys.A) Then
            ClearDuplicateHotkeys(Keys.A)
            SetHotkeyText(Keys.A, result)

        End If
        If ks.IsKeyDown(Keys.S) Then
            ClearDuplicateHotkeys(Keys.S)
            SetHotkeyText(Keys.S, result)
        End If
        If ks.IsKeyDown(Keys.D) Then
            ClearDuplicateHotkeys(Keys.D)
            SetHotkeyText(Keys.D, result)
        End If
        If ks.IsKeyDown(Keys.F) Then
            ClearDuplicateHotkeys(Keys.F)
            SetHotkeyText(Keys.F, result)
        End If

    End Sub
    Private Sub SetHotkeyText(key As Keys, result As SkillTreeTabReturn)
        If result.isCore Then
            skillTabs(ActiveTabIndex).CoreHotkeyAssignment = key
        Else
            If Not result.LeftOrRight Then
                'False for left, true for right
                skillTabs(ActiveTabIndex).LeftHotkeyAssignment(result.Index) = key
            Else
                skillTabs(ActiveTabIndex).RightHotkeyAssignment(result.Index) = key
            End If
        End If

    End Sub
    Private Sub ClearDuplicateHotkeys(key As Keys)

        For Each currentTab In skillTabs
            For i = 0 To 3 Step 1
                If currentTab.LeftHotkeyAssignment(i) = key Then
                    currentTab.LeftHotkeyAssignment(i) = Keys.None
                End If
                If currentTab.RightHotkeyAssignment(i) = key Then
                    currentTab.RightHotkeyAssignment(i) = Keys.None
                End If


            Next
            If currentTab.CoreHotkeyAssignment = key Then
                currentTab.CoreHotkeyAssignment = Keys.None
            End If
        Next
    End Sub
    Private Sub CheckSwitchTabs(ms As MouseState)
        For i = 0 To TabRectangles.Length - 1 Step 1
            Dim offsetRectangle As Rectangle = New Rectangle(CInt(TabRectangles(i).X + currentOffset.X), CInt(TabRectangles(i).Y + currentOffset.Y), TabRectangles(i).Width, TabRectangles(i).Height)
            If ms.LeftButton = ButtonState.Pressed And offsetRectangle.Contains(New Point(ms.X, ms.Y)) Then
                'Clicking on the tab.
                SetTab = CType(i, SkillTreeTabTypes)

            End If
        Next
    End Sub
    Private WriteOnly Property SetTab As SkillTreeTabTypes
        Set(value As SkillTreeTabTypes)
            ActiveTabIndex = value
        End Set
    End Property
    ''' <summary>
    ''' Code refactoring for the skill spending,.
    ''' </summary>
    ''' <param name="LocalPlayer">The LocalPlayer instance.</param>
    ''' <remarks></remarks>
    Private Sub ValidateTreeAndPlaySound(LocalPlayer As Player)
        SoundLibrary.GetSoundByName("interface1").Play()
        SpentSkillPointsOnLastLoop = True

    End Sub
    ''' <summary>
    ''' Updates the skill levels written to the left of each skill. Run when increasing skill levels.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub UpdateUnderDrawnSkillForActiveTree(LocalPlayer As Player)
        skillTabs(ActiveTabIndex).UpdateUnderDrawnSkillLevels(LocalPlayer.playerClass.SkillTree(ActiveTabIndex))
    End Sub

    Private Function CheckSkillRequirementsLeft(LocalPlayer As Player, result As SkillTreeTabReturn, tree As SkillTreeTabTypes) As Boolean
        If LocalPlayer.playerClass.SkillTree(ActiveTabIndex).LeftSideSkills(result.Index).MeetsRequirements(LocalPlayer.playerClass.GetTotalLevelsInSideOftree(False, tree)) And LocalPlayer.playerClass.HasPreviousSkills(False, ActiveTabIndex, result.Index) = True Then
            'The skill can be ranked, but does the player have skill points?
            If LocalPlayer.CanTakeSkillPoint Then
                Return True
            End If

        End If
        Return False
    End Function
    Private Function CheckSkillRequirementsRight(LocalPlayer As Player, result As SkillTreeTabReturn, tree As SkillTreeTabTypes) As Boolean
        If LocalPlayer.playerClass.SkillTree(ActiveTabIndex).RightSideSkills(result.Index).MeetsRequirements(LocalPlayer.playerClass.GetTotalLevelsInSideOftree(True, tree)) And LocalPlayer.playerClass.HasPreviousSkills(True, ActiveTabIndex, result.Index) = True Then
            'The skill can be ranked, but does the player have skill points?
            If LocalPlayer.CanTakeSkillPoint Then
                Return True
            End If

        End If
        Return False
    End Function
End Class
Public Enum SkillTreeTabTypes As Integer
    OFFENSE
    DEFENSE
    SUPPORT

End Enum
