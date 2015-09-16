Public Class Panel
    'The panel class is an organizational class that makes it easy to group types of UI together. They can then be drawn seperately or together.
    Public Buttons() As Button
    Public ProgressBars() As ProgressBar
    Public Textures() As MobileTexture
    Public scrollBoxes() As scrollBox
    Public isPanelClicked As Boolean = False
    Private SpellHotbar As SpellHotbar
    Private useSpellHotbar As Boolean
    Public Background As Texture2D
    Private incrementAmount As Integer
    ''' <summary>
    ''' The amount to increment ProgressBar objects per tick.
    ''' </summary>
    ''' <value>An integer representing the amount to increase ProgressBar objects by.</value>
    ''' <returns>The current increment amount.</returns>
    ''' <remarks>You can also manually adjust individual progress bars by using ProgressBars(index).Increment. </remarks>
    Public Property ProgressBarIncrementAmount As Integer
        Get
            Return incrementAmount
        End Get
        Set(value As Integer)
            incrementAmount = value
        End Set
    End Property
    Public WriteOnly Property ActivateHotbar(iconAssetsFromAssetMgr() As String, CM As ContentManager, sb As SpriteBatch, ms As MouseState, gd As GraphicsDevice) As Boolean
        Set(value As Boolean)
            useSpellHotbar = value
            SetupSpellHotbar(iconAssetsFromAssetMgr, "defaultFont", CM, gd)
        End Set
    End Property
    ''' <summary>
    ''' Create a new Panel instance/
    ''' </summary>
    ''' <param name="inButtons">The buttons to be stored in this instance.</param>
    ''' <param name="inProgressBars">The progress bars to be stored in this instance.</param>
    ''' <param name="inMobileTextures">The textures (simple images that have no user interaction) to be stored.</param>
    ''' <remarks></remarks>
    Public Sub New(inButtons() As Button, inProgressBars() As ProgressBar, inScrollBoxes() As scrollBox,inMobileTextures() As MobileTexture)
        Buttons = inButtons
        ProgressBars = inProgressBars
        scrollBoxes = inScrollBoxes
        Textures = inMobileTextures
    End Sub
    ''' <summary>
    ''' Initializes the SpellHotbar. Only one panel will be using this, so it makes sense for it to have its own section instead of forcing every object 
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SetupSpellHotbar(iconAssetsFromAssetMgr() As String, fontAsset As String, CM As ContentManager, gd As GraphicsDevice)
        useSpellHotbar = True
        SpellHotbar = New SpellHotbar(iconAssetsFromAssetMgr, fontAsset, CM, gd)
    End Sub
    Public Sub New(inButtons() As Button, inProgressBars() As ProgressBar, inScrollBoxes() As scrollBox, inTextures() As Texture2D, inTextureRectangles() As Rectangle)
        Buttons = inButtons
        ProgressBars = inProgressBars
        ReDim Textures(inTextures.Length - 1)
        Dim i As Integer
        For i = 0 To inTextures.Length - 1 Step 1
            Textures(i) = New MobileTexture(inTextures(i), inTextureRectangles(i))
        Next

    End Sub

    Public Sub UpdateProgressBars(amountToIncrement As Integer)
        Dim i As Integer
        If Not ProgressBars Is Nothing Then
            For i = 0 To ProgressBars.Length - 1 Step 1
                If Not ProgressBars(i).isDone Then 'Don't bother running the Increment if the ProgressBar is done iterating.
                    ProgressBars(i).Increment(amountToIncrement)
                    If ProgressBars(i).isDone Then
                        'If it has become Done since the last increment, it's finished.
                        RaiseEvent ProgressBarComplete(ProgressBars(i))
                    End If
                End If

            Next
        End If

    End Sub
    Public Sub DrawAll(sb As SpriteBatch, screenSize As Size, ms As MouseState, gt As GameTime, hkSet As HotKeyHandler, EM As EntityManagement, ks As KeyboardState, cm As ContentManager, UI As UIOverlay, Cam As IsoCamera)
        isPanelClicked = False
        If useSpellHotbar Then
            SpellHotbar.Draw(sb, ms, gt, hkSet, EM, ks, cm, UI, Cam)
            If SpellHotbar.getRectangle.Contains(New Point(ms.X, ms.Y)) Then
                isPanelClicked = True
            End If
        End If
        Dim i As Integer
        If Not IsNothing(Background) Then
            sb.Draw(Background, New Rectangle(0, 0, screenSize.Width, screenSize.Height), Color.White)
        End If
        If Not Buttons Is Nothing Then
            For i = 0 To Buttons.Length - 1 Step 1
                Dim isPressed As Boolean = False
                If ms.LeftButton = ButtonState.Pressed Then
                    isPressed = True
                End If
                If Buttons(i).IsPressed(New Vector2(ms.X, ms.Y), isPressed, gt) = ButtonReturnTypes.CLICKED Then
                    RaiseEvent ButtonPressed(Buttons(i))
                    isPanelClicked = True
                End If
                Buttons(i).Draw(sb)



            Next
        End If
        If Not ProgressBars Is Nothing Then
            For i = 0 To ProgressBars.Length - 1 Step 1
                If New Rectangle(CInt(ProgressBars(i).Position.X), CInt(ProgressBars(i).Position.Y), ProgressBars(i).Size.Width, ProgressBars(i).Size.Height).Contains(New Point(ms.X, ms.Y)) Then
                    isPanelClicked = True
                End If
                ProgressBars(i).Draw(sb, True, ms)

            Next
        End If
        If Not scrollBoxes Is Nothing Then
            For i = 0 To scrollBoxes.Length - 1 Step 1
                scrollBoxes(i).updateObjects(ms, gt)
                scrollBoxes(i).draw(sb)
                If scrollBoxes(i).getRectangle.Contains(New Point(ms.X, ms.Y)) Then
                    isPanelClicked = True
                End If
            Next
        End If
        If Not Textures Is Nothing Then
            For i = 0 To Textures.Length - 1 Step 1

                Textures(i).Draw(sb)
                If Textures(i).internalRectangle.Contains(New Point(ms.X, ms.Y)) Then
                    isPanelClicked = True
                End If

            Next
        End If
    End Sub
#Region "Events"
    Public Event ButtonPressed(pressedButton As Button)
    Public Event ButtonReleased(releasedButton As Button)
    Public Event ButtonHilighted(releasedButton As Button)
    Public Event ProgressBarComplete(CompletedBar As ProgressBar)

#End Region

End Class

Public Class MobileTexture
    Public Value As Texture2D
    Public internalRectangle As Rectangle
    Public internalColor As Color = Color.White
    Public Sub New(texture As Texture2D, inRectangle As Rectangle)
        Value = texture
        internalRectangle = inRectangle
    End Sub
    Public Sub Draw(sb As SpriteBatch)
        sb.Draw(Value, internalRectangle, internalColor)
    End Sub
End Class
