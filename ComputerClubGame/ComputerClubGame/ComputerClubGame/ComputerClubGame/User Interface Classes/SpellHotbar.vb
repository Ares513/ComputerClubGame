''' <summary>
''' This class manages the player's eight spell hotbar, bound to Q, W, E, R, A, S, D and F.
''' </summary>
''' <remarks></remarks>
Public Class SpellHotbar
    Inherits UIEntity
    Dim HotbarButtons(7) As SpellButton
    Dim HotbarPopUps(7) As PopupBox
    Dim hotbarHotkeys() As Keys = {Keys.Q, Keys.W, Keys.E, Keys.R, Keys.A, Keys.S, Keys.D, Keys.F}


    Dim backTexture As Texture2D
    Dim boxTexture As Texture2D
    Dim cooldownCoverTexture As Texture2D
    Public Event HotbarClicked(sender As Object, e As System.EventArgs)
    Public Event HotbarButtonClicked(sender As Object, e As System.EventArgs, clickedButton As Button)
    Public Event OnEnable(sender As Object, e As System.EventArgs)
    Private PrimedSpell As Integer = -1
    Private SelectedLastTick As Boolean
    Private Font As SpriteFont
    Private lastState As ButtonReturnTypes
    'The player's hotbar is situated in between the health and mana bars. It stores up to eight spells and can be activated by clicking or by
    'pressing the hotkeys. 
    Public Sub New(iconAssetsFromAssetMgr() As String, fontAsset As String, CM As ContentManager, gd As GraphicsDevice)
        'The pressed, hovering and depressed images for all buttons are the same.
        MyBase.New(New Size(0, 0)) 'Temporary to satisfy superclass. Property is set later.
        Size = New Size(CInt(gd.Viewport.Width * 0.7), CInt(gd.Viewport.Height * 0.1))
        Center = New Vector2(CInt(gd.Viewport.Width * 0.5), CInt(gd.Viewport.Height * 0.85))
        If IsNothing(iconAssetsFromAssetMgr) Then
        DebugManagement.WriteLineToLog("The spell hotbar was loaded without String asset values!", SeverityLevel.FATAL)
            Debug.Assert(False) 'If this happens something is seriously wrong in our pipeline. We can remove this as we get closer to release.
        End If
        If iconAssetsFromAssetMgr.Length < 8 Then
            ReDim Preserve iconAssetsFromAssetMgr(7)
        End If
        If IsNothing(fontAsset) Then
            fontAsset = "defaultFont"
        End If
        Dim i As Integer
        For i = 0 To HotbarButtons.Length - 1 Step 1
            Dim currentIconAssets(2) As String
            'Color of depressed and undepressed buttons changes- image does not
            currentIconAssets(0) = iconAssetsFromAssetMgr(i)
            currentIconAssets(1) = iconAssetsFromAssetMgr(i)
            currentIconAssets(2) = iconAssetsFromAssetMgr(i)
            HotbarButtons(i) = New SpellButton("Spell Slot " + i.ToString(), currentIconAssets, fontAsset, CM, False, 0.0, 0.0, SpellTypes.Unassigned)
            HotbarButtons(i).Size = New Size(CInt(Size.Width / 8), Size.Height)
            HotbarButtons(i).Pos = New Vector2(Pos.X + HotbarButtons(i).Size.Width * i, Pos.Y)
            Dim popUpSize As Size = New Size(CInt(gd.Viewport.Width * 0.15), CInt(gd.Viewport.Height * 0.3))
            HotbarPopUps(i) = New PopupBox(popUpSize, Vector2.Subtract(HotbarButtons(i).Center, New Vector2(0, CInt(HotbarButtons(i).Size.Height * 2))), fontAsset, CM, HotbarButtons(i).getRectangle, "Spell data has not been loaded yet. Please wait...", CM.Load(Of Texture2D)(AssetManager.RequestAsset("dialogborder")), True, 45, 40)
        Next

        backTexture = CM.Load(Of Texture2D)(AssetManager.RequestAsset("parchmentexpanded"))
        boxTexture = CM.Load(Of Texture2D)(AssetManager.RequestAsset("SpellBoxEmpty"))
        cooldownCoverTexture = CM.Load(Of Texture2D)(AssetManager.RequestAsset("grayPanel"))
        Font = CM.Load(Of SpriteFont)(AssetManager.RequestAsset("SpellHotBarFont"))

    End Sub
    Public Shadows Sub Draw(sb As SpriteBatch, ms As MouseState, gt As GameTime, hkSet As HotKeyHandler, EM As EntityManagement, ks As KeyboardState, CM As ContentManager, UI As UIOverlay, cam As IsoCamera)

        If PrimedSpell <> -1 And lastState = ButtonReturnTypes.NOT_SELECTED Then
            'Spell is primed.
            If ms.LeftButton = ButtonState.Pressed Then
                UI.setCursor(0)
                ActivateSpell(PrimedSpell, EM, gt, hkSet, ms, ks, CM, UI, cam)
                PrimedSpell = -1

            End If

        End If
        If ms.LeftButton = ButtonState.Released Then
            lastState = ButtonReturnTypes.NOT_SELECTED
        End If
        Dim i As Integer
        For i = 0 To HotbarButtons.Length - 1 Step 1

            Dim hotbarColorHilight As Color = Color.SlateGray
            Dim brt As ButtonReturnTypes = HotbarButtons(i).IsPressed(New Vector2(ms.X, ms.Y), CBool(ms.LeftButton), gt)

            If brt = ButtonReturnTypes.CLICKED Then
                UI.setCursor(1)
                PrimedSpell = i
                hotbarColorHilight = Color.Green
                lastState = ButtonReturnTypes.CLICKED
            ElseIf brt = ButtonReturnTypes.HOVERING Then
                hotbarColorHilight = Color.Yellow
            End If
            sb.Draw(backTexture, New Rectangle(CInt(HotbarButtons(i).Pos.X), CInt(HotbarButtons(i).Pos.Y), HotbarButtons(i).Size.Width, HotbarButtons(i).Size.Height), hotbarColorHilight)
            HotbarButtons(i).Draw(sb)
            sb.Draw(boxTexture, New Rectangle(CInt(HotbarButtons(i).Pos.X), CInt(HotbarButtons(i).Pos.Y), HotbarButtons(i).Size.Width, HotbarButtons(i).Size.Height), Color.White)
            HotbarPopUps(i).Draw(sb, ms)
            DrawOverlayForCooldownSpells(sb, EM, hkSet)
        Next
    End Sub
    Private Sub SetCursorSettingsBySpellType()


    End Sub
    Private Sub DrawOverlayForCooldownSpells(sb As SpriteBatch, EM As EntityManagement, hKeySet As HotKeyHandler)
        Dim i As Integer
        For i = 0 To HotbarButtons.Length - 1 Step 1
            Dim cooldownHilight = Color.Yellow
            cooldownHilight.A = 20
            Dim actionVal As IAction = hKeySet.GetAssociatedActions(hotbarHotkeys(i))
            If Not IsNothing(actionVal) Then
                Dim cdVal As Cooldown = EM.CooldownList.getCooldown(actionVal.CooldownName)
                If IsNothing(cdVal) Then
                Else
                    Dim current As Integer, max As Integer
                    current = cdVal.Remaining
                    max = cdVal.Maximum
                    Dim percentage As Double = (max - current) / max
                    If percentage < 1 Then


                        Dim destRect As Rectangle = HotbarButtons(i).GetRectangle
                        destRect.Height = CInt(destRect.Height * percentage)
                        destRect.Y += CInt(HotbarButtons(i).GetRectangle.Height * (1 - percentage))

                        sb.Draw(cooldownCoverTexture, destRect, cooldownHilight)
                        Dim message As String = Math.Round(percentage * 100).ToString() + "%"
                        Dim textPos As Vector2 = New Vector2(HotbarButtons(i).GetRectangle.Center.X, HotbarButtons(i).GetRectangle.Center.Y)
                        textPos -= New Vector2(CInt(Font.MeasureString(message).X / 2), CInt(Font.MeasureString(message).Y / 2))
                        sb.DrawString(Font, message, textPos, Color.Black)

                    End If
                End If
            End If

        Next
    End Sub
    Private Sub ActivateSpell(index As Integer, EM As EntityManagement, gt As GameTime, hkSet As HotKeyHandler, ms As MouseState, ks As KeyboardState, CM As ContentManager, UI As UIOverlay, Cam As IsoCamera)
        Select Case index
            Case 0
                hkSet.EmulateKey(Keys.Q, EM, gt, ms, ks, CM, UI, Cam)
            Case 1
                hkSet.EmulateKey(Keys.W, EM, gt, ms, ks, CM, UI, Cam)
            Case 2
                hkSet.EmulateKey(Keys.E, EM, gt, ms, ks, CM, UI, Cam)
            Case 3
                hkSet.EmulateKey(Keys.R, EM, gt, ms, ks, CM, UI, Cam)
            Case 4
                hkSet.EmulateKey(Keys.A, EM, gt, ms, ks, CM, UI, Cam)
            Case 5
                hkSet.EmulateKey(Keys.S, EM, gt, ms, ks, CM, UI, Cam)
            Case 6
                hkSet.EmulateKey(Keys.D, EM, gt, ms, ks, CM, UI, Cam)
            Case 7
                hkSet.EmulateKey(Keys.F, EM, gt, ms, ks, CM, UI, Cam)
        End Select
    End Sub
    Public Sub SetSpell(Slot As Byte, SpellEnum As SpellIDs, iconAssetFromAssetMgr As String)
        If Slot > 7 Then
            Slot = 7
        End If
        If Slot < 0 Then
            Slot = 0
        End If

    End Sub

End Class
