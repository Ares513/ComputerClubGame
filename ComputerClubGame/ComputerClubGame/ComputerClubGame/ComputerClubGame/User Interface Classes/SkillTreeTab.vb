Public Class SkillTreeTab
    Public CoreButton As Button
    Public CorePopup As PopupBox
    Public LeftButtons() As Button
    Public RightButtons() As Button
    Public LeftPopups() As PopupBox
    Public RightPopups() As PopupBox
    Public TabName As String
    Private workingArea As Rectangle
    Private RegularFont As SpriteFont
    Private ItalicFont As SpriteFont
    Private BoldFont As SpriteFont
    Friend leftUnderDrawnSkillLevels(3) As Integer
    Friend RightUnderDrawnSkillLevels(3) As Integer
    Friend LeftHotkeyAssignment(3) As Keys
    Friend RightHotkeyAssignment(3) As Keys
    Friend CoreHotkeyAssignment As Keys
    Private CoreUnderDrawnSkillLevel As Integer
    Public buttonBackTexture As Texture2D
    Private LockedSkillTexture As Texture2D
    Private internalOffset As Integer
    ''' <summary>
    ''' The number of buttons and popups on each side of the tree.
    ''' </summary>
    ''' <remarks></remarks>
    Public Const TreeSideCount As Integer = 4


#Region "Events"
    Public Event ButtonPressed(sender As Object, e As System.EventArgs, pressedButton As Button)

#End Region


    Public Sub New(CM As ContentManager, buttonAssetsFromAssetManager() As String, popupAssetFromAssetManager As String, FontAsset As String, availableArea As Rectangle, tabName As String, inputButtonBackTexture As Texture2D, inputLockedSkillTexture As Texture2D, Optional italicFontAsset As String = "SkillTreeFontItalic", Optional boldFontAsset As String = "SkillTreeFontBold", Optional inputInternalOffset As Integer = 20)
        RegularFont = CM.Load(Of SpriteFont)(AssetManager.RequestAsset(FontAsset))
        ItalicFont = CM.Load(Of SpriteFont)(AssetManager.RequestAsset(italicFontAsset))
        BoldFont = CM.Load(Of SpriteFont)(AssetManager.RequestAsset(boldFontAsset))
        internalOffset = inputInternalOffset
        buttonBackTexture = inputButtonBackTexture
        LockedSkillTexture = inputLockedSkillTexture
        workingArea = availableArea
        Me.TabName = tabName
        'All buttons have the same base texture and are set by another method.
        ReDim Preserve LeftButtons(3) 'Always 9 buttons. 9 skills per tree
        ReDim Preserve RightButtons(3)
        'one level 1 ability, 4 on each side after that at levels 5, 10, 15 and 30
        ReDim Preserve LeftPopups(3)
        ReDim Preserve RightPopups(3)
        Dim i As Integer
        Dim boxSize As Size = New Size(CInt(workingArea.Width / 8), CInt(workingArea.Width / 8))
        Dim PopupBoxSize As Size = New Size(boxSize.Width * 4, boxSize.Height * 4)
        Dim coreBoxPos As Vector2
        coreBoxPos = (New Vector2(workingArea.X + CSng(workingArea.Width / 4), workingArea.Y + boxSize.Height))
        CoreButton = New Button("Skill Core", buttonAssetsFromAssetManager, AssetManager.RequestAsset(FontAsset), CM, False, 1, 1)
        CoreButton.Pos = coreBoxPos
        CoreButton.Size = boxSize
        CorePopup = New PopupBox(PopupBoxSize, New Vector2(0, 0), AssetManager.RequestAsset(FontAsset), CM, CoreButton.GetRectangle, "No spell data loaded.", CM.Load(Of Texture2D)(popupAssetFromAssetManager), True, 50, 50)
        CorePopup.Pos = New Vector2(CoreButton.Pos.X + CoreButton.Size.Width, CoreButton.Pos.Y)

        For i = 0 To 3 Step 1
            LeftHotkeyAssignment(i) = Keys.None
            RightHotkeyAssignment(i) = Keys.None

        Next
        LeftHotkeyAssignment(0) = Keys.None
        CoreHotkeyAssignment = Keys.None
        For i = 0 To LeftButtons.Length - 1 Step 1

            'zero is the center ability- it's position is special
            'even numbered ones are to the left, odd are to the right




            Dim boxPos As Vector2

            boxPos = New Vector2(workingArea.X, workingArea.Y + boxSize.Height * 2 * CSng(i + 1))

            LeftButtons(i) = New Button("Skill Button " + i.ToString(), buttonAssetsFromAssetManager, AssetManager.RequestAsset(FontAsset), CM, False, 1, 1)
            LeftButtons(i).Pos = boxPos
            LeftButtons(i).Size = boxSize
            LeftButtons(i).HilightedColor = Color.Yellow


            'Button data complete. Set the popup data.

            LeftPopups(i) = New PopupBox(PopupBoxSize, New Vector2(0, 0), FontAsset, CM, LeftButtons(i).GetRectangle, "No spell data loaded.", CM.Load(Of Texture2D)(popupAssetFromAssetManager), True, 50, 50)
            LeftPopups(i).Pos = New Vector2(LeftButtons(i).Pos.X + LeftButtons(i).Size.Width, LeftButtons(i).Pos.Y)
            LeftPopups(i).setSelectionRectangle = New Rectangle(CInt(LeftButtons(i).Pos.X), CInt(LeftButtons(i).Pos.Y), boxSize.Width, boxSize.Height)
            'Popups(i).Center = New Vector2(Buttons(i).Center.X + Buttons(i).Size.Width + Popups(i).Size.Width / 2, Buttons(i).Center.Y + Buttons(i).Size.Height + Popups(i).Size.Height / 2)

        Next
        For i = 0 To RightButtons.Length - 1 Step 1



            Dim boxPos As Vector2
            boxPos = New Vector2(workingArea.X + CSng(workingArea.Width * 2 * (1 / 4)), workingArea.Y + boxSize.Height * 2 * CSng(i + 1)) 'Add a space of one box between each one.
            RightButtons(i) = New Button("Skill Button " + i.ToString(), buttonAssetsFromAssetManager, AssetManager.RequestAsset(FontAsset), CM, False, 1, 1)
            RightButtons(i).Pos = boxPos
            RightButtons(i).Size = boxSize
            RightButtons(i).HilightedColor = Color.Yellow


            'Button data complete. Set the popup data.

            RightPopups(i) = New PopupBox(PopupBoxSize, New Vector2(0, 0), FontAsset, CM, RightButtons(i).GetRectangle, "No spell data loaded.", CM.Load(Of Texture2D)(popupAssetFromAssetManager), True, 50, 50)
            RightPopups(i).Pos = New Vector2(RightButtons(i).Pos.X + RightButtons(i).Size.Width, RightButtons(i).Pos.Y)
            'Popups(i).Center = New Vector2(Buttons(i).Center.X + Buttons(i).Size.Width + Popups(i).Size.Width / 2, Buttons(i).Center.Y + Buttons(i).Size.Height + Popups(i).Size.Height / 2)
            RightPopups(i).setSelectionRectangle = New Rectangle(CInt(RightButtons(i).Pos.X), CInt(RightButtons(i).Pos.Y), RightButtons(i).Size.Width, RightButtons(i).Size.Height)
        Next
    End Sub
    Public Sub LoadSkills(CoreSkill As Skill, LeftSideSkills As List(Of Skill), RightSideSkills As List(Of Skill), CM As ContentManager)
        CoreUnderDrawnSkillLevel = CoreSkill.Level
        CorePopup.Message = CoreSkill.Description
        For i = 0 To LeftSideSkills.Count - 1 Step 1
            Try
                leftUnderDrawnSkillLevels(i) = LeftSideSkills(i).Level
                LeftPopups(i).Message = LeftSideSkills(i).Description
                Dim loadedTexture As Texture2D = CM.Load(Of Texture2D)(LeftSideSkills(i).Image)
                LeftButtons(i).ReloadTextures(loadedTexture, loadedTexture, loadedTexture)

            Catch ex As Exception
                DebugManagement.WriteLineToLog("There are too many skills to be put into this skill tree!", SeverityLevel.CRITICAL)
            End Try

        Next
        For i = 0 To RightSideSkills.Count - 1 Step 1
            Try
                RightUnderDrawnSkillLevels(i) = LeftSideSkills(i).Level
                RightPopups(i).Message = RightSideSkills(i).Description
                RightButtons(i).texture = CM.Load(Of Texture2D)(RightSideSkills(i).Image)
            Catch ex As Exception
                DebugManagement.WriteLineToLog("There are too many skills to be put into this skill tree!", SeverityLevel.CRITICAL)
            End Try

        Next

    End Sub

    Public Sub Draw(sb As SpriteBatch, ByVal slidingOffset As Vector2, ms As MouseState, LocalPlayer As Player, tab As SkillTreeTabTypes, Optional PlayerSkillPts As Integer = 0)

        For i = 0 To TreeSideCount - 1 Step 1
            LeftButtons(i).Offset = New Vector2(slidingOffset.X + internalOffset, slidingOffset.Y)
            sb.Draw(buttonBackTexture, LeftButtons(i).GetRectangle, Color.White)
            LeftButtons(i).Draw(sb)
            sb.DrawString(RegularFont, leftUnderDrawnSkillLevels(i).ToString, New Vector2(LeftButtons(i).GetRectangle.Right, LeftButtons(i).GetRectangle.Center.Y - CInt(RegularFont.MeasureString(leftUnderDrawnSkillLevels(i).ToString).Y / 2)), Color.Black)
            RightButtons(i).Offset = New Vector2(slidingOffset.X + internalOffset, slidingOffset.Y)
            sb.Draw(buttonBackTexture, RightButtons(i).GetRectangle, Color.White)

            RightButtons(i).Draw(sb)
            If LocalPlayer.playerClass.SkillTree(tab).LeftSideSkills(i).MeetsRequirements(LocalPlayer.playerClass.GetTotalLevelsInSideOftree(False, tab)) = False Or LocalPlayer.playerClass.HasPreviousSkills(False, tab, i) = False Then
                'Does NOT meet requirements. Draw the locked texture.
                LeftButtons(i).Enabled = False
                sb.Draw(LockedSkillTexture, LeftButtons(i).GetRectangle, Color.White)
                Dim message As String = LocalPlayer.playerClass.SkillTree(tab).LeftSideSkills(i).LevelRequirement.ToString()
                Dim messageSize As Vector2
                messageSize = RegularFont.MeasureString(message)
                sb.DrawString(RegularFont, message, New Vector2(LeftButtons(i).GetRectangle.Left - messageSize.X, LeftButtons(i).Center.Y - CInt(messageSize.Y / 2)), Color.Red)



            Else
                Dim message As String = LocalPlayer.playerClass.SkillTree(tab).LeftSideSkills(i).LevelRequirement.ToString()
                Dim messageSize As Vector2
                messageSize = RegularFont.MeasureString(message)
                sb.DrawString(RegularFont, LeftHotkeyAssignment(i).ToString(), New Vector2(LeftButtons(i).GetRectangle.Center.X - CInt(RegularFont.MeasureString(LeftHotkeyAssignment(i).ToString()).X / 2), LeftButtons(i).GetRectangle.Top - messageSize.Y), Color.Orange)

                LeftButtons(i).Enabled = True
            End If

            If LocalPlayer.playerClass.SkillTree(tab).RightSideSkills(i).MeetsRequirements(LocalPlayer.playerClass.GetTotalLevelsInSideOftree(True, tab)) = False Or LocalPlayer.playerClass.HasPreviousSkills(True, tab, i) = False Then
                'Does NOT meet requirements. Draw the locked texture.
                RightButtons(i).Enabled = False
                sb.Draw(LockedSkillTexture, RightButtons(i).GetRectangle, Color.White)
                Dim message As String = LocalPlayer.playerClass.SkillTree(tab).RightSideSkills(i).LevelRequirement.ToString()
                Dim messageSize As Vector2
                messageSize = RegularFont.MeasureString(message)
                sb.DrawString(RegularFont, message, New Vector2(RightButtons(i).GetRectangle.Left - messageSize.X, RightButtons(i).Center.Y - CInt(messageSize.Y / 2)), Color.Red)

            Else
                Dim message As String = LocalPlayer.playerClass.SkillTree(tab).RightSideSkills(i).LevelRequirement.ToString()
                Dim messageSize As Vector2
                messageSize = RegularFont.MeasureString(message)

                sb.DrawString(RegularFont, RightHotkeyAssignment(i).ToString(), New Vector2(RightButtons(i).GetRectangle.Center.X - CInt(RegularFont.MeasureString(RightHotkeyAssignment(i).ToString()).X), RightButtons(i).GetRectangle.Top - messageSize.Y), Color.Orange)
                RightButtons(i).Enabled = True
            End If

            sb.DrawString(RegularFont, RightUnderDrawnSkillLevels(i).ToString, New Vector2(RightButtons(i).GetRectangle.Center.X - CInt(RegularFont.MeasureString(RightUnderDrawnSkillLevels(i).ToString()).X / 2), RightButtons(i).GetRectangle.Center.Y - CInt(RegularFont.MeasureString(RightUnderDrawnSkillLevels(i).ToString).Y)), Color.Black)
        Next
        For i = 0 To TreeSideCount - 1 Step 1
            RightPopups(i).Offset = New Vector2(slidingOffset.X + internalOffset, slidingOffset.Y)
            RightPopups(i).setSelectionRectangle = RightButtons(i).GetRectangle
            LeftPopups(i).Offset = New Vector2(slidingOffset.X + internalOffset, slidingOffset.Y)
            LeftPopups(i).setSelectionRectangle = LeftButtons(i).GetRectangle
            RightPopups(i).Draw(sb, MS)

            LeftPopups(i).Draw(sb, MS)
        Next
        CoreButton.Offset = New Vector2(slidingOffset.X + internalOffset, slidingOffset.Y)


        CorePopup.Offset = New Vector2(slidingOffset.X + internalOffset, slidingOffset.Y)
        CorePopup.setSelectionRectangle = CoreButton.GetRectangle
        sb.Draw(buttonBackTexture, CoreButton.GetRectangle, Color.White)
        CoreButton.Draw(sb)
        sb.DrawString(RegularFont, CoreHotkeyAssignment.ToString(), New Vector2(CoreButton.GetRectangle.Center.X - CInt(RegularFont.MeasureString(CoreHotkeyAssignment.ToString()).X / 2), CoreButton.GetRectangle.Top - RegularFont.MeasureString(CoreHotkeyAssignment.ToString()).Y), Color.Orange)
        If LocalPlayer.playerClass.SkillTree(tab).CoreSkill.MeetsRequirements(LocalPlayer.playerClass.GetCoreSkillLevel(tab)) = False Then
            CoreButton.Enabled = False
            sb.Draw(LockedSkillTexture, CoreButton.GetRectangle, Color.White)

        Else
            CoreButton.Enabled = True
        End If
        sb.DrawString(RegularFont, CoreUnderDrawnSkillLevel.ToString, New Vector2(CoreButton.GetRectangle.Right, CoreButton.GetRectangle.Center.Y - CInt(RegularFont.MeasureString(CoreUnderDrawnSkillLevel.ToString).Y / 2)), Color.Black)
        CorePopup.Draw(sb, MS)





        'Finally, draw the player skill points.
        Dim skillPtRectangle As Rectangle
        skillPtRectangle = New Rectangle(workingArea.Right - CoreButton.Size.Width + CInt(slidingOffset.X), workingArea.Y + CoreButton.Size.Height, CoreButton.Size.Width, CoreButton.Size.Height)
        sb.Draw(buttonBackTexture, skillPtRectangle, Color.White)
        sb.DrawString(BoldFont, PlayerSkillPts.ToString(), New Vector2(skillPtRectangle.Center.X - (BoldFont.MeasureString(PlayerSkillPts.ToString()).X / 2), slidingOffset.Y +
        skillPtRectangle.Center.Y - (BoldFont.MeasureString(PlayerSkillPts.ToString()).Y / 2)), Color.Black)

    End Sub
    Public Function Update(gt As GameTime, ms As MouseState, ks As KeyboardState, skillTreePoints As Integer) As SkillTreeTabReturn
        Dim state As ButtonReturnTypes
        Dim finalState As ButtonReturnTypes
        Dim finalButton As Button = Nothing
        Dim lIndex, rIndex, finalIndex As Integer
        Dim LeftOrRight As Boolean 'False for left, true for right
        Dim isCoreButton As Boolean = False
        lIndex = -1
        rIndex = -1
        state = CoreButton.IsPressed(New Vector2(ms.X, ms.Y), CBool(ms.LeftButton), gt)
        If state = ButtonReturnTypes.CLICKED Then
            finalState = state
            finalButton = CoreButton
            finalIndex = 0
            RaiseEvent ButtonPressed(Me, New EventArgs(), CoreButton)
            LeftOrRight = False
            isCoreButton = True
            Return New SkillTreeTabReturn(finalButton, finalState, LeftOrRight, finalIndex, True)
        End If


        For Each Button In LeftButtons
            lIndex += 1
            state = Button.IsPressed(New Vector2(ms.X, ms.Y), CBool(ms.LeftButton), gt)
            If state = ButtonReturnTypes.CLICKED Then
                finalState = state
                finalButton = Button
                finalIndex = lIndex
                RaiseEvent ButtonPressed(Me, New EventArgs(), Button)
                LeftOrRight = False
            ElseIf state = ButtonReturnTypes.HOVERING Then
                If IsNothing(finalButton) Then
                    finalButton = Button
                    finalState = state
                    finalIndex = lIndex
                    LeftOrRight = False
                End If
            End If
        Next
        For Each Button In RightButtons
            rIndex += 1
            state = Button.IsPressed(New Vector2(ms.X, ms.Y), CBool(ms.LeftButton), gt)
            If state = ButtonReturnTypes.CLICKED Then

                RaiseEvent ButtonPressed(Me, New EventArgs(), Button)
                finalState = state
                finalButton = Button
                finalIndex = rIndex
                LeftOrRight = True
            ElseIf state = ButtonReturnTypes.HOVERING Then
                If IsNothing(finalButton) Then
                    finalButton = Button
                    finalState = state
                    finalIndex = rIndex
                    LeftOrRight = True
                End If
            End If
        Next

        If CoreButton.IsPressed(New Vector2(ms.X, ms.Y), CBool(ms.LeftButton), gt) = ButtonReturnTypes.HOVERING And IsNothing(finalButton) Then
            'If hovering over the core button afer all of this, return that instead.
            Return New SkillTreeTabReturn(CoreButton, ButtonReturnTypes.HOVERING, False, 0, True)
        End If
        If LeftOrRight Then
            Return New SkillTreeTabReturn(finalButton, finalState, LeftOrRight, finalIndex)
        Else
            Return New SkillTreeTabReturn(finalButton, finalState, LeftOrRight, finalIndex)
        End If

    End Function
    Public Sub UpdateUnderDrawnSkillLevels(tree As PlayerSkillTree)
        For i = 0 To leftUnderDrawnSkillLevels.Length - 1 Step 1
            leftUnderDrawnSkillLevels(i) = tree.LeftSideSkills(i).Level

        Next
        For i = 0 To RightUnderDrawnSkillLevels.Length - 1 Step 1
            RightUnderDrawnSkillLevels(i) = tree.RightSideSkills(i).Level

        Next
        CoreUnderDrawnSkillLevel = tree.CoreSkill.Level
    End Sub
End Class
Public Class SkillTreeTabReturn
    Public clickedButton As Button
    Public buttonState As ButtonReturnTypes
    Public LeftOrRight As Boolean
    Public isCore As Boolean
    Public Index As Integer
    Public Sub New(button As Button, state As ButtonReturnTypes, LeftOrRight As Boolean, Index As Integer, Optional IsCoreButton As Boolean = False)
        clickedButton = button
        buttonState = state
        Me.LeftOrRight = LeftOrRight
        Me.Index = Index
        isCore = IsCoreButton
    End Sub
End Class