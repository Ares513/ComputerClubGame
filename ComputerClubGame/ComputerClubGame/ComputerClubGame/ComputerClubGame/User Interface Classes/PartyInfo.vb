'NEED MULTIPLAYER TO COMPLETE
Public Class PartyInfo
    Inherits SlidingWindow
    Dim backTexture As Texture2D
    Dim safeAreaTexture As Texture2D
    Dim DrawColor As Color
    Dim Font As SpriteFont
    Dim Parties As List(Of PlayerParty)
    Dim Players As List(Of PartyMember)
    Dim lineSpacing As Integer 'The amount to shift each line down by.
    Dim TitleFont As SpriteFont
    Dim Title As String
    Dim Buttons As List(Of Button)
    Dim ProgBars As List(Of ProgressBar)
    Public Sub New(screenSize As Rectangle, CM As ContentManager)
        MyBase.New(screenSize, False, False)
        backTexture = CM.Load(Of Texture2D)(AssetManager.RequestAsset("parchmentExpanded"))
        safeAreaTexture = CM.Load(Of Texture2D)(AssetManager.RequestAsset("PartyBackground"))

        DrawColor = Color.White
        Font = CM.Load(Of SpriteFont)(AssetManager.RequestAsset("defaultFont", AssetTypes.SPRITEFONT))
        TitleFont = CM.Load(Of SpriteFont)(AssetManager.RequestAsset("TitleFont", AssetTypes.SPRITEFONT))
        Parties = New List(Of PlayerParty)
        Buttons = New List(Of Button)
        ProgBars = New List(Of ProgressBar)
        Parties.Add(New PlayerParty())
        Players = New List(Of PartyMember)
        Players.Add(New PartyMember("Test", 20, "Knight", IDGenerator.Generate(New Random())))
        lineSpacing = 20

        Title = "Party"
    End Sub
    Public Sub Draw(sb As SpriteBatch)
        sb.Draw(backTexture, GetRectangle, DrawColor)
        sb.Draw(safeAreaTexture, GetSafeRectangle, DrawColor)
        Dim titleLength As Vector2 = TitleFont.MeasureString(Title)
        sb.DrawString(TitleFont, Title, New Vector2(GetRectangle.Center.X - (titleLength.X / 2), GetRectangle.Y), Color.Black)

        For i = 0 To Players.Count - 1 Step 1
            Dim nameMeasurement As Vector2 = Font.MeasureString(Players(i).Name)

            sb.DrawString(Font, Players(i).Name, New Vector2(lineSpacing + GetSafeRectangle.X, GetSafeRectangle.Y + (nameMeasurement.Y * i)), Color.Black)
            Dim ClassNameMeasurement As Vector2 = Font.MeasureString(Players(i).ClassName)
            sb.DrawString(Font, Players(i).ClassName, New Vector2(lineSpacing + GetSafeRectangle.Center.X - ClassNameMeasurement.X, GetSafeRectangle.Y + ((ClassNameMeasurement.Y + lineSpacing) * i)), Color.Black)
            Dim LevelString As String = "Level " + Players(i).Level.ToString()
            Dim levelMeasurement As Vector2 = Font.MeasureString(LevelString)

            sb.DrawString(Font, LevelString, New Vector2(lineSpacing + GetSafeRectangle.Center.X + levelMeasurement.X, GetSafeRectangle.Y + ((ClassNameMeasurement.Y + lineSpacing) * i)), Color.Black)
            If Players(i).isInParty Then

            End If
        Next

    End Sub
    Public Sub Update()
        CheckSliding()
    End Sub
End Class
Public Class PlayerParty
    'Woo! Party!
    Public PartyID As String
    Public Members As List(Of PartyMember)

    Public Sub New()
        Members = New List(Of PartyMember)
    End Sub
    Public Sub AddMember(MemberDisplayName As String, MemberID As String, MemberClassName As String, MemberLevel As Integer)
        Members.Add(New PartyMember(MemberDisplayName, MemberLevel, MemberClassName, MemberID))
    End Sub
End Class
Public Class PartyMember
    Public Name As String
    Public Level As Integer
    Public ClassName As String

    Private PlayerID As String
    Public isInParty As Boolean = False
    Public ReadOnly Property EntityIDOfUnderlyingPlayer As String
        Get
            Return PlayerID
        End Get
    End Property
    Public Sub New(PlayerName As String, PlayerLvl As Integer, PlayerClassName As String, ID As String)
        Name = PlayerName
        Level = PlayerLvl
        ClassName = PlayerClassName
        PlayerID = ID
    End Sub
End Class
