Public MustInherit Class UIEntity

    Friend position As Vector2
    Friend entitySize As Size
    Friend texture As Texture2D
    Protected isVisible As Boolean
    Public Property Visible As Boolean
        Get
            Return isVisible
        End Get
        Set(value As Boolean)
            isVisible = value
        End Set
    End Property
    'Getters and Setters
    Public Property Pos As Vector2
        Get
            Return position

        End Get
        Set(value As Vector2)
            position = value
        End Set
    End Property
    Public Property Center As Vector2
        Get
            Return position + New Vector2(entitySize.Width, entitySize.Height) / 2
        End Get
        Set(value As Vector2)
            position = value - New Vector2(entitySize.Width, entitySize.Height) / 2
        End Set
    End Property
    Public Property getRectangle As Rectangle
        Get
            Return New Rectangle(CInt(position.X), CInt(position.Y), entitySize.Width, entitySize.Height)
        End Get
        Set(value As Rectangle)

        End Set
    End Property
    Public Property Size As Size
        Get
            Return entitySize
        End Get
        Set(value As Size)
            entitySize = value
        End Set
    End Property

    'Constructor
    Public Sub New(defaultSize As Size)
        position = New Vector2(0, 0)
        entitySize = defaultSize
    End Sub


    'Draw Function
    Public Sub Draw(spriteBatch As SpriteBatch)
        spriteBatch.Draw(texture, New Rectangle(CInt(Pos.X), CInt(Pos.Y), Size.Width, Size.Height), Color.White)
    End Sub
End Class