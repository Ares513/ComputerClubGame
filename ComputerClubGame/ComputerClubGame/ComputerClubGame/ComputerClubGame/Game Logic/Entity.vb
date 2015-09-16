Public MustInherit Class Entity
    Friend drawColor As Color
    Friend map As Map
    Friend position As Vector3
    Friend entitySize As Size
    Friend velocity As Vector3
    Friend texture As IsoTexture
    Friend isAlive As Boolean
    Private EntityID As String
    Protected BaseType As EntityBaseType
    'Getters and Setters 
    Public MustOverride Property Name As String
    Public ReadOnly Property ID As String
        Get
            Return EntityID
        End Get
    End Property
    Public ReadOnly Property EntityType As EntityBaseType
        Get
            Return BaseType
        End Get
    End Property
    Public Property Alive As Boolean
        Get
            Return isAlive
        End Get
        Set(value As Boolean)
            isAlive = value
        End Set
    End Property

    Public Property CurrentMap As Map
        Get
            Return map
        End Get
        Set(value As Map)
            map = value
        End Set
    End Property

    Public Property Pos As Vector3
        Get
            Return position

        End Get
        Set(value As Vector3)
            position = value
        End Set
    End Property

    Public Property Vel As Vector3
        Get
            Return velocity
        End Get
        Set(value As Vector3)
            velocity = value
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
    ''' <summary>
    ''' Sets the drawn color. Default is Color.White.
    ''' </summary>
    ''' <value></value>
    ''' <returns>A Color indicating the current color.</returns>
    ''' <remarks>May not work with all implementations of Entity.</remarks>
    Public Property Color As Color
        Get
            Return drawColor
        End Get
        Set(value As Color)
            drawColor = value
        End Set
    End Property

    'Constructor
    Public Sub New(inEntityID As String, defaultSize As Size, map As Map)
        EntityID = inEntityID
        position = New Vector3(0, 0, 0)
        entitySize = defaultSize
        isAlive = True
        CurrentMap = map
    End Sub

    Public ReadOnly Property SizeInTiles(Cam As IsoCamera) As Size
        Get
            Dim result As Vector3 = Cam.ScreenToMap(New Vector2(Size.Width, Size.Height), 0)
            Return New Size(Math.Abs(CInt(result.X)), Math.Abs(CInt(result.Y)))
        End Get
    End Property
    Public Property Center(Cam As IsoCamera) As Vector3
        Get
            Dim screenPos As Vector2 = Cam.MapToScreen(Pos)
            Dim screenCenter = New Vector2(CSng(screenPos.X), CSng(screenPos.Y))
            Dim output As Vector3 = Cam.ScreenToMap(screenCenter, 0)
        End Get
        Set(value As Vector3)
            Dim targetScreenPos As Vector2 = Cam.MapToScreen(value) - New Vector2(CSng(Size.Width / 2), CSng(Size.Height / 2))
            Pos = Cam.ScreenToMap(targetScreenPos, map.GetHeight(Pos.X, Pos.Y))

        End Set
    End Property


    'Update Function
    Public Overridable Sub Update(gt As GameTime)
        position += velocity * gt.ElapsedGameTime.Milliseconds
    End Sub

    'Draw Function
    Public Overridable Sub Draw(spriteBatch As SpriteBatch, cam As IsoCamera)
        texture.Draw(spriteBatch, cam, position)
    End Sub

    ''' <summary>
    ''' Checks for collision between players
    ''' </summary>
    ''' <param name="inRect"></param>
    ''' <returns>a boolean value determining whether or not collision has occurred</returns>
    ''' <remarks>This may not work properly with isometric images.</remarks>
    Friend Overridable Function isColliding(inRect As Rectangle) As Boolean
        If inRect.Contains(New Point(CInt(position.X), CInt(position.Y))) Or inRect.Contains(New Point(CInt(position.X + Size.Width), CInt(position.Y))) Or
 inRect.Contains(New Point(CInt(position.X), CInt(position.Y + Size.Height))) Or inRect.Contains(New Point(CInt(position.X + Size.Width), CInt(position.Y + Size.Height))) Then
            Return True
        End If
        Return False
    End Function

    Public ReadOnly Property getRectangle As Rectangle
        Get
            Return New Rectangle(CInt(Pos.X), CInt(Pos.Y), Size.Width, Size.Height)

        End Get
    End Property


End Class

'Subclass for size
Public Class Size
    Public Sub New(inWidth As Integer, inHeight As Integer)
        Width = inWidth
        Height = inHeight
    End Sub
    Public Sub New()

    End Sub
    Public Function ToRectangle() As Rectangle
        Return New Rectangle(0, 0, Width, Height)
    End Function
    Public Function ToRectangleF() As RectangleF
        Return New RectangleF(0.0, 0.0, CSng(Width), CSng(Height))
    End Function
    Public Width As Integer
    Public Height As Integer
End Class
Public Enum EntityBaseType As Byte
    ''' <summary>
    ''' An instance of the Entity base class. Indicates that the property was never set.
    ''' </summary>
    ''' <remarks></remarks>
    Standard_Entity = 0
    ''' <summary>
    ''' A livingEntity. May indicate improper initialization.
    ''' </summary>
    ''' <remarks></remarks>
    Living_Entity = 1
    ''' <summary>
    ''' An EntityPlayer instance.
    ''' </summary>
    ''' <remarks></remarks>
    Player = 2
    ''' <summary>
    ''' A mob instance.
    ''' </summary>
    ''' <remarks></remarks>
    Mob = 3
    ''' <summary>
    ''' A friendly NPC such as a shopkeeper.
    ''' </summary>
    ''' <remarks></remarks>
    FriendlyNPC = 4
End Enum


