Public Class ChatBox
    Dim value As String
    Dim backTexture As Texture2D
    ''' <summary>
    ''' Return
    ''' </summary>
    ''' <remarks></remarks>
    Private Font As SpriteFont
    Private rect As Rectangle
    Public EnteredText As String
    ''' <summary>
    ''' Whether or not the window is open.
    ''' </summary>
    ''' <remarks></remarks>
    Public Active As Boolean
    Public ReadOnly Property LoadedFont As SpriteFont
        Get
            Return Font
        End Get
    End Property
    Public Sub New(CM As ContentManager, screenSize As Rectangle)
        Dim loadedSpriteFont As SpriteFont = CM.Load(Of SpriteFont)(AssetManager.RequestAsset("ChatFont", AssetTypes.SPRITEFONT))
        Dim height As Integer = CInt(loadedSpriteFont.MeasureString("test String is AMAAZING!").Y)
        'Set the height of the string to the height of the box.
        rect.Width = screenSize.Width
        rect.Height = height
        rect.Y = screenSize.Height - height
        Font = loadedSpriteFont
        backTexture = CM.Load(Of Texture2D)(AssetManager.RequestAsset("defaultProgressBarFull"))
        EnteredText = ""

    End Sub
    Public Sub Draw(sb As SpriteBatch)
        If Active Then
            sb.Draw(backTexture, rect, Color.Black)
            sb.DrawString(Font, EnteredText, New Vector2(CSng(0.02 * rect.Width), rect.Y), Color.White)
        End If
    End Sub
    Public Sub Update(gt As GameTime)

    End Sub
End Class
