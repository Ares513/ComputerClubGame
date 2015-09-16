Public Class MapRenderer
    Private tileSet As ITileSet

    Public Sub Draw(CurMap As IMap, spriteBatch As SpriteBatch, Camera As IsoCamera)
        Dim i, j As Integer
        For i = 0 To CurMap.Width - 1
            For j = 0 To CurMap.Height - 1
                tiles.getTileTexture(CurMap.TileAt(i, j)).Draw(spriteBatch, Camera, New Vector3(i, j, 0))
                'DrawTileAt(CurMap.TileAt(i, j), spriteBatch, Camera.MapToScreen(New Vector3(i, j, 0)))
            Next
        Next
    End Sub
    Public Property tiles As ITileSet
        Get
            Return tileSet
        End Get
        Set(value As ITileSet)
            tileSet = value
        End Set
    End Property
End Class