Public Class TileSetFactory
    Public Shared Function MapToTileSet(currTerrainType As Integer, TileID As Integer) As Integer
        Select Case currTerrainType
            Case EnumTerrainType.TerrainTypes.Plains
                Return mapToPlains(TileID)
            Case EnumTerrainType.TerrainTypes.Forest
                Return mapToForest(TileID)
            Case EnumTerrainType.TerrainTypes.CrystalForest
                Return mapToCrystalForest(TileID)
            Case Else
                Return mapToPlains(TileID)
        End Select
    End Function

    Private Shared Function mapToPlains(TileType As Integer) As Integer
        Select Case TileType
            Case TileOld.TileTypes.Grass
                Return 0
            Case TileOld.TileTypes.Grass_2
                Return 1
            Case TileOld.TileTypes.Grass_3
                Return 2
            Case TileOld.TileTypes.Grass_4
                Return 3
            Case TileOld.TileTypes.Grass_5
                Return 4
            Case TileOld.TileTypes.Grass_6
                Return 5
            Case TileOld.TileTypes.Marsh
                Return 114
            Case TileOld.TileTypes.Dirt
                Return 6
            Case TileOld.TileTypes.Rock
                Return 55
            Case TileOld.TileTypes.Mountain
                Return 34
            Case TileOld.TileTypes.Mountain_2
                Return 75
            Case TileOld.TileTypes.Mountain_Water
                Return 60
            Case TileOld.TileTypes.BloodGrass
                Return 0
            Case TileOld.TileTypes.BloodSoil
                Return 0
            Case TileOld.TileTypes.Cave
                Return 50
            Case TileOld.TileTypes.Wall
                Return 55
            Case Else
                Return 75
        End Select
    End Function

    Private Shared Function mapToForest(TileType As Integer) As Integer
        Return Nothing 'temp for now
    End Function

    Private Shared Function mapToCrystalForest(TileType As Integer) As Integer
        Return Nothing ' temp for now
    End Function
End Class
