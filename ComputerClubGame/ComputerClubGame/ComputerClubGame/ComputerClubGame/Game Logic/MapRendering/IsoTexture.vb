Public Class IsoTexture
    Private tex As Texture2D
    Private rect As Rectangle
    Private orig As Vector2

    Public Sub New(ByRef texture As Texture2D, rectangle As Rectangle, origin As Vector2)
        tex = texture
        rect = rectangle
        orig = origin
    End Sub

    Public Sub Draw(spriteBatch As SpriteBatch, loc As Vector2, Optional layerdepth As Single = 0)
        'spriteBatch.Draw(tex, loc, rect, Color.White)
        spriteBatch.Draw(tex, loc, rect, Color.White, 0, orig, 1, SpriteEffects.None, layerdepth)
    End Sub

    Public Sub Draw(spriteBatch As SpriteBatch, cam As IsoCamera, loc As Vector3, Optional layerHint As Single = 0)
        Draw(spriteBatch, cam.MapToScreen(loc), cam.ScaleLayer(loc.X + loc.Y + loc.Z) + CSng(layerHint * 0.001))
    End Sub

End Class
