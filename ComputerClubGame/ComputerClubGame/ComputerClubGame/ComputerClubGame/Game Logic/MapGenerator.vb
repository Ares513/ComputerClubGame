Public Class MapGenerator
    ''' <summary>
    ''' Takes a pre-defined Area, and generates its map component
    ''' </summary>
    ''' <param name="ProtoMap">An Area that has the information necessary for map generation, such as a valid TerrainType</param>
    ''' <remarks></remarks>
    Public Shared Sub generate(ProtoMap As Area, Optional inMapShape As MapShape = MapShape.Square)
        Dim rand As Random = New Random((Now.Hour * 1000) + (Now.Minute * 10) + Now.Millisecond)

        Select Case inMapShape
            Case MapShape.Square
                ProtoMap.InitializeTiles(100, 100)
                GenSquareMap(ProtoMap)
            Case MapShape.Circle

            Case MapShape.Triangle

            Case Else
                DebugManagement.WriteLineToLog("MapGenerator found a wild error! Wild error used random selected nonexistant shape! MapGenerator crashed and made an empty world! Everyone Fainted!", SeverityLevel.CRITICAL)
        End Select
    End Sub

    Private Shared Sub GenSquareMap(ProtoMap As Area)
        Dim type(6) As Integer
        Select Case ProtoMap.TerrainType
            Case EnumTerrainType.TerrainTypes.BloodPlains
                ReDim type(1)
                type(0) = TileOld.TileTypes.BloodGrass
                type(1) = TileOld.TileTypes.BloodSoil
            Case EnumTerrainType.TerrainTypes.Cave
                ReDim type(1)
                type(0) = TileOld.TileTypes.Cave
                type(1) = TileOld.TileTypes.Rock
            Case EnumTerrainType.TerrainTypes.CrystalForest
                ReDim type(1)
                type(0) = TileOld.TileTypes.Grass
                type(1) = TileOld.TileTypes.Dirt
            Case EnumTerrainType.TerrainTypes.Forest
                ReDim type(1)
                type(0) = TileOld.TileTypes.Grass
                type(1) = TileOld.TileTypes.Dirt
            Case EnumTerrainType.TerrainTypes.Plains
                ReDim type(3)
                type(0) = TileOld.TileTypes.Grass
                type(1) = TileOld.TileTypes.Dirt
                'type(2) = Tile.TileTypes.Marsh don't use marsh on plains, looks bad
                type(2) = TileOld.TileTypes.Grass_2
                type(3) = TileOld.TileTypes.Grass_3
            Case EnumTerrainType.TerrainTypes.LoadedFromFile
                DebugManagement.WriteLineToLog("MapGenerator found a wild error! Wild error used can't generate terrain supposed to be loaded from file! MapGenerator Crashed a made an empty world! Everyone Fainted!", SeverityLevel.CRITICAL)
            Case Else
                DebugManagement.WriteLineToLog("MapGenerator found a wild Error! Wild Error used Protomap has no defined TerrainType! MapGenerator crashed and made an empty world! Wild Error used TELEPORT! Wild error got away and Crashed everything! MapGenerator and Game1 Fainted!", SeverityLevel.CRITICAL)
        End Select
        For I As Integer = 0 To CInt(ProtoMap.Width - 1) Step 1
            For J As Integer = 0 To CInt(ProtoMap.Height - 1) Step 1
                'Always wall the entire thing in.
                If I = CInt(ProtoMap.Width - 1) Or J = CInt(ProtoMap.Height - 1) Or I = 0 Or J = 0 Then
                    ProtoMap.SetTile(I, J, New TileOld(TileOld.TileTypes.Wall, True, False))
                Else
                    ProtoMap.SetTile(I, J, New TileOld(type(KernsMath.RandInt(0, type.Length - 1)), False, False))
                End If
            Next
        Next
        If Not IsNothing(ProtoMap.linkedAreas) Then
            For I As Integer = 0 To ProtoMap.linkedAreas.Length - 1 Step 1
                ProtoMap.SetTile(KernsMath.RandInt(0, ProtoMap.Width - 1), KernsMath.RandInt(0, ProtoMap.Height - 1), New TileOld(ProtoMap.linkedAreas(I)))
            Next
        End If
        'Dim rand As Random
        'rand = New Random()
        'Dim I1, J1, K1 As Integer
        'I1 = rand.Next(7)
        'J1 = rand.Next(CInt(ProtoMap.Height)) - 5
        'K1 = rand.Next(CInt(ProtoMap.Width)) - 5
        'For I2 = I1 To 0 Step -1
        '    For J2 = J1 To J1 + 5
        '        For K2 = K1 To K1 + 5
        '            ProtoMap.SetTile(J2, K2, New Tile(type(1), False, False))
        '        Next
        '    Next
        'Next
    End Sub
End Class

Public Enum MapShape As Integer
    Square = 1
    Circle = 2
    Triangle = 3
    MAX_MAPSHAPE = 4 'must always be last
End Enum