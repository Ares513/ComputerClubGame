Public Class InventorySprite
    Dim keyboardState As KeyboardState
    Dim inventoryKey As Keys = Keys.I
    Dim inventoryPos As Integer
    Dim inventorySpeed As Integer
    Dim open As Boolean = False
    Dim clientBounds As Rectangle

    Dim inventoryTexture As Texture2D   ' I need a texture for inventory

    Public Sub New(InventorySpeed As Integer, InventoryTexture As Texture2D, clientBounds As Rectangle)
        Me.inventorySpeed = InventorySpeed
        Me.inventoryTexture = InventoryTexture
        Me.clientBounds = clientBounds
        Me.inventoryPos = CInt(clientBounds.Width / 2)
    End Sub

    Public Property OpenBool() As Boolean
        Get
            Return open
        End Get
        Set(ByVal open As Boolean)
            Me.open = open
        End Set
    End Property

    Public Property PositionX() As Integer
        Get
            Return inventoryPos
        End Get
        Set(value As Integer)
            Me.inventoryPos = value
        End Set
    End Property

    Public Property SpeedX() As Integer
        Get
            Return inventorySpeed
        End Get
        Set(value As Integer)
            Me.inventorySpeed = value
        End Set
    End Property

    Public Overloads Sub Update(gameTime As GameTime)
        If open = True Then
            If inventoryPos > CInt(clientBounds.Width / 2) Then
                inventoryPos = inventoryPos - inventorySpeed
            End If
            If inventoryPos < CInt(clientBounds.Width / 2) Then
                inventoryPos = CInt(clientBounds.Width / 2)
            End If
        End If
        If open = False Then
            If inventoryPos < clientBounds.Width Then
                inventoryPos = inventoryPos + inventorySpeed
            End If
            If inventoryPos > clientBounds.Width Then
                inventoryPos = clientBounds.Width
            End If
        End If
    End Sub

    Public Overloads Sub Draw(spriteBatch As SpriteBatch)
        spriteBatch.Draw(inventoryTexture, New Rectangle(inventoryPos, 0, CInt(clientBounds.Width / 2), clientBounds.Height), Color.White)
    End Sub
End Class
