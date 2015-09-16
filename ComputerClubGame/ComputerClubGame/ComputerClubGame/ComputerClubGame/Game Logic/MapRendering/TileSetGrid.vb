Public Class TileSetGrid
    Implements ITileSet

    Private Const TileSize As Integer = 64

    Private tex As Texture2D
    Private tiles As IsoTexture()

    Public Sub New(texture As Texture2D, tileSetWidth As Integer, tileSetHeight As Integer)
        tex = texture
        tiles = New IsoTexture(tileSetWidth * tileSetHeight) {}
        Dim i, j As Integer
        For j = 0 To tileSetHeight - 1
            For i = 0 To tileSetWidth - 1
                tiles(j * tileSetWidth + i) = New IsoTexture(tex, New Rectangle(i * TileSize, j * TileSize, TileSize, TileSize), New Vector2(TileSize / 2.0F, TileSize / 2.0F))
            Next
        Next
    End Sub

    Public Function getTileTexture(tile As TileOld) As IsoTexture Implements ITileSet.getTileTexture
        Return tiles(TileSetFactory.MapToTileSet(EnumTerrainType.TerrainTypes.Plains, CInt(tile.Type)))
    End Function

End Class
