Public Class ItemSprite
    Dim texture As Texture2D
    Dim position As Vector2
    Dim relativePosition As Vector2
    Dim sheetSize As Point
    Dim itemSize As Integer
    Dim size As Vector2
    Dim speed As Integer
    Dim relativeItemStartPosition As Vector2
    Private _itemExampleTexture As Texture2D
    Private _vector2 As Vector2

    Public Sub New(position As Vector2)

    End Sub

    Public Sub New(texture As Texture2D, inventorySpeed As Integer, sequence As Integer)
        Me.texture = texture
        Me.position = position
        Me.speed = inventorySpeed
        relativePosition = New Vector2(itemSize * (sheetSize.X Mod (1 + sequence)), itemSize * CInt(Fix(sheetSize.X / (sequence + 1))))
    End Sub

    Public Sub Update(gameTime As GameTime, inventoryPos As Integer)
        position = relativeItemStartPosition + relativePosition + New Vector2(inventoryPos, 0)
    End Sub

    Public Sub Draw(spriteBatch As SpriteBatch)
        spriteBatch.Draw(texture, New Rectangle(CInt(position.X), CInt(position.Y), CInt(size.X), CInt(size.Y)), Color.White)
    End Sub
End Class
