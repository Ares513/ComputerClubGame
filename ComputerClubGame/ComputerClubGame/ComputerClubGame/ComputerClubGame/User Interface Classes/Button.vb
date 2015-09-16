
Public Class Button
    Inherits UIEntity
    Protected Friend drawPaths(2) As String
    Dim delay As Integer = 1000
    Dim previousState As Integer
    Dim buttonImageToDraw As Integer
    Dim buttonName As String
    Dim buttonAssetName As String
    Dim buttonTextures() As Texture2D
    Dim internalFont As SpriteFont
    Private buttonActive As Boolean = True
    Private drawTitle As Boolean = True
    Private textScaleWidth, textScaleHeight As Single
    Public Tag As Object 'Tag can be any user-defined value for reference later.
    Public HilightedColor As Color = Color.White
    Public DisabledColor As Color = Color.Gray
    Public Offset As Vector2 = New Vector2(0, 0)

    Public Property ShowTitle As Boolean
        Get
            Return drawTitle
        End Get
        Set(value As Boolean)
            drawTitle = value
        End Set
    End Property
    Public Property Enabled As Boolean
        Get
            Return buttonActive
        End Get
        Set(value As Boolean)
            buttonActive = value
        End Set
    End Property
    Public Property Font As SpriteFont
        Get
            Return internalFont
        End Get
        Set(value As SpriteFont)
            internalFont = value
        End Set
    End Property
    Public Property Name As String
        Get
            Return buttonName
        End Get
        Set(value As String)
            buttonName = value
        End Set
    End Property
    Public Shadows ReadOnly Property GetRectangle As Rectangle
        Get
            Return New Rectangle(CInt(Pos.X + Offset.X), CInt(Pos.Y + Offset.Y), Size.Width, Size.Height)
        End Get
    End Property
    Public Sub New(bName As String, assetNames() As String, fontAsset As String, CM As ContentManager, showTitle As Boolean, titleScaleWidth As Single, titleScaleHeight As Single)
        MyBase.New(New Size())
        buttonName = bName
        Dim i As Integer
        ReDim buttonTextures(assetNames.Length - 1)
        For i = 0 To assetNames.Length - 1 Step 1
            buttonTextures(i) = CM.Load(Of Texture2D)(assetNames(i))
        Next
        internalFont = CM.Load(Of SpriteFont)(fontAsset)
        Me.textScaleHeight = textScaleHeight
        Me.textScaleWidth = textScaleWidth
        drawTitle = showTitle
    End Sub
    ''' <summary>
    ''' Resets the textures.
    ''' </summary>
    ''' <param name="clicked"></param>
    ''' <param name="hilighted"></param>
    ''' <param name="unclicked"></param>
    ''' <remarks></remarks>
    Public Sub ReloadTextures(clicked As Texture2D, hilighted As Texture2D, unclicked As Texture2D)
        buttonTextures(ButtonReturnTypes.NOT_SELECTED) = unclicked
        buttonTextures(ButtonReturnTypes.HOVERING) = hilighted
        buttonTextures(ButtonReturnTypes.CLICKED) = clicked
    End Sub

    ''' <summary>
    ''' Checks if the button is pressed and raises events.
    ''' </summary>
    ''' <param name="mouseLoc">The location of the mouse as a Vector2.</param>
    ''' <param name="mouseClicked">Whether or not the mouse has been clicked.</param>
    ''' <param name="gTime">Game1.GameTime instance.</param>
    ''' <returns>Integer representing whether or not the button has been pressed.</returns>
    ''' <remarks></remarks>
    Public Function IsPressed(mouseLoc As Vector2, mouseClicked As Boolean, gTime As GameTime) As ButtonReturnTypes
        'checks to see if button is active
        If buttonActive = False Then
            'button inactive, do nothing
        Else
            'button active run     

            ' If (gTime.TotalGameTime.Milliseconds Mod delay = 0) Then
            Dim checkRect As Rectangle
            Dim checkPoint As Point
            checkRect = New Rectangle(CInt(position.X + Offset.X), CInt(position.Y + Offset.X), entitySize.Width, entitySize.Height)
            checkPoint = New Point(CInt(mouseLoc.X), CInt(mouseLoc.Y))
            If checkRect.Contains(checkPoint) Then
                If mouseClicked = True Then
                    If previousState = ButtonReturnTypes.CLICKED Then
                    Else
                        RaiseEvent Pressed()
                        previousState = ButtonReturnTypes.CLICKED
                        buttonImageToDraw = ButtonReturnTypes.CLICKED
                        Return ButtonReturnTypes.CLICKED
                        'CLICKED
                    End If
                Else
                    If (previousState = ButtonReturnTypes.NOT_SELECTED) Then
                        RaiseEvent Selected()
                    ElseIf (previousState = ButtonReturnTypes.CLICKED) Then
                        RaiseEvent Activated()
                    End If
                    previousState = ButtonReturnTypes.HOVERING
                    buttonImageToDraw = ButtonReturnTypes.HOVERING
                    Return ButtonReturnTypes.HOVERING
                    'HOVERING
                End If
            Else
                If (previousState = ButtonReturnTypes.HOVERING Or previousState = ButtonReturnTypes.CLICKED) Then
                    RaiseEvent Deselected()
                End If
                previousState = ButtonReturnTypes.NOT_SELECTED
                buttonImageToDraw = ButtonReturnTypes.NOT_SELECTED
                Return ButtonReturnTypes.NOT_SELECTED
                'NOT_SELECTED
            End If
        End If
        Return ButtonReturnTypes.NOT_SELECTED
    End Function

    Public Event Pressed()
    Public Event Selected()
    Public Event Deselected()
    Public Event Activated()

    Public Overloads Sub Draw(sb As SpriteBatch)
        Dim buttonRect As Rectangle
        buttonRect = New Rectangle(CInt(position.X + Offset.X), CInt(position.Y + Offset.Y), Size.Width, Size.Height)
        If buttonActive Then
            If previousState = ButtonReturnTypes.CLICKED Then
                sb.Draw(buttonTextures(buttonImageToDraw), buttonRect, HilightedColor)
            Else
                sb.Draw(buttonTextures(buttonImageToDraw), buttonRect, Color.White)
            End If

        Else
            sb.Draw(buttonTextures(buttonImageToDraw), buttonRect, DisabledColor)
        End If
        If ShowTitle Then
            Dim textLengthInPixels As Vector2 = Font.MeasureString(buttonName)
            Dim textPos As Vector2 = New Vector2(position.X + Offset.X + (Math.Abs(Size.Width - textLengthInPixels.X) / 2), Offset.Y + position.Y + (Math.Abs(Size.Height - textLengthInPixels.Y) / 2))
            sb.DrawString(Font, buttonName, textPos, Color.Gray)
        End If
    End Sub


End Class
Public Enum ButtonReturnTypes
    NOT_SELECTED = 0
    HOVERING = 1
    CLICKED = 2
End Enum