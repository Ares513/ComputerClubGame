''' <summary>
''' Class Representing an Area of a Chapter
''' </summary>
''' <remarks></remarks>
Public NotInheritable Class Area
    Implements IMap
    Public ReadOnly TerrainType As Integer
    Public ReadOnly UID As Integer
    Public ReadOnly linkedAreas() As Integer
    Dim TerrainTiles As TileOld(,)
    ReadOnly Property Width As Integer Implements IMap.Width
        Get
            Return TerrainTiles.GetLength(0)
        End Get
    End Property
    ReadOnly Property Height As Integer Implements IMap.Height
        Get
            Return TerrainTiles.GetLength(1)
        End Get
    End Property
    ''' <summary>
    ''' Instantiates a new Area
    ''' </summary>
    ''' <param name="ID">The Unique ID of the Area, 0 is reserved</param>
    ''' <param name="Terrain">The TerrainType of the Area</param>
    ''' <param name="travelableAreas">Areas this Area Links to</param>
    ''' <remarks></remarks>
    Public Sub New(ID As Integer, Terrain As Integer, travelableAreas() As Integer)
        TerrainType = Terrain
        UID = ID
        linkedAreas = travelableAreas
        MapGenerator.generate(Me)
    End Sub

    ''' <summary>
    ''' Instantiates an Isolated Area, cannot be entered or exited aside from Teleportation
    ''' </summary>
    ''' <param name="ID">Unique ID of the Area, 0 is Reserved</param>
    ''' <param name="Terrain">TerraainType of this Area</param>
    ''' <remarks></remarks>
    Public Sub New(ID As Integer, Terrain As Integer)
        TerrainType = Terrain
        UID = ID

        MapGenerator.generate(Me)
    End Sub

    ''' <summary>
    ''' Initializes the Area's Array of Tiles
    ''' </summary>
    ''' <param name="Width">The width of the Array/map</param>
    ''' <param name="Height">The height of the Array/map</param>
    ''' <remarks></remarks>
    Public Sub InitializeTiles(Width As Integer, Height As Integer)

        ReDim Preserve TerrainTiles(Width, Height)
    End Sub

    ''' <summary>
    ''' Set a tile in this area to a value
    ''' </summary>
    ''' <param name="X">the x value of the tile to set.</param>
    ''' <param name="Y">the y value of the tile to set.</param>
    ''' <param name="SetTo">a tile to set the tile to. Tiles are copied, not set to the same instance.</param>
    ''' <remarks></remarks>
    Public Sub SetTile(X As Integer, Y As Integer, SetTo As TileOld)
        Try
            If Not SetTo.Type = TileOld.TileTypes.Teleporter Then
                TerrainTiles(X, Y) = New TileOld(SetTo.Type, SetTo.Tall, SetTo.IsFloorTile)
            Else
                TerrainTiles(X, Y) = New TileOld(SetTo.outAreaID)
            End If
        Catch ex As Exception
            DebugManagement.WriteLineToLog("Index Out of Bounds while setting a tile, check the size of a map or if it is initialized. Class:Area", SeverityLevel.WARNING)
        End Try

    End Sub

    Public Function TileAt(x As Integer, y As Integer) As TileOld Implements IMap.TileAt
        If x > Width Or x < 0 Then
            DebugManagement.WriteLineToLog("Trying to get a tile that is outside the array. CCG used Crash! It's super effective! Computer Fainted! Class:Area, Function: TileAt", SeverityLevel.WARNING)
            Return Nothing
        End If
        If y > Height Or y < 0 Then
            DebugManagement.WriteLineToLog("Trying to get a tile that is outside the array. CCG used Crash! It's super effective! Computer Fainted! Class:Area, Function: TileAt", SeverityLevel.WARNING)
            Return Nothing
        End If
        Dim out As TileOld
        Try
            out = TerrainTiles(x, y)
        Catch ex As Exception
            DebugManagement.WriteLineToLog("Something Weird happened! CCG fell out of the universe! CCG, no longer being present, Caused an NPE! NPE was Fatal to Computer! User used Rage! COmputer fainted and then fell apart! Everyone Lost!", SeverityLevel.FATAL)
            Return Nothing
        End Try
        Return out
    End Function

    Public Function Update(mouseState As MouseState, cam As IsoCamera, CM As ContentManager, sb As SpriteBatch) As TileOld 'add content manager, then finish textprocesor creation and drawing
        Dim mouseMapCoords As Vector3 = cam.ScreenToMap(New Vector2(mouseState.X, mouseState.Y), 0)
        If mouseMapCoords.X < 0 Or mouseMapCoords.Y < 0 Or mouseMapCoords.X > Width Or mouseMapCoords.Y > Height Then
            Return Nothing
        End If
        Dim mouseTile As TileOld = TileAt(CInt(Math.Floor(mouseMapCoords.X)), CInt(Math.Floor(mouseMapCoords.Y)))
        If Not IsNothing(mouseTile) Then
            If mouseTile.Type = TileOld.TileTypes.Teleporter Then
                Dim str As String = mouseTile.outAreaID.ToString()
                Dim pb As PopupBox = New PopupBox(New Size(50, 50), New Vector2(mouseState.X, mouseState.Y - 50), AssetManager.RequestAsset("defaultFont"), CM, New Rectangle(mouseState.X - 1, mouseState.Y - 1, 2, 2), str, CM.Load(Of Texture2D)(AssetManager.RequestAsset("dialogborder")), True, 20, 20)
                pb.Draw(sb, mouseState)
                If mouseState.LeftButton = ButtonState.Pressed Then
                    Return mouseTile
                End If
            Else
                Return Nothing
            End If
        Else
            Return Nothing
        End If
        Return Nothing
    End Function

End Class
