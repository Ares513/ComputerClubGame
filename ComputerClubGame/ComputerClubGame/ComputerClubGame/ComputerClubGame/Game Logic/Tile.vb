Public Class TileOld

    Dim TypeProperty As Integer

    Public Property Type As Integer    ' used for texture
        Private Set(value As Integer)
            TypeProperty = value
        End Set
        Get
            Return TypeProperty
        End Get
    End Property

    Public ReadOnly Tall As Boolean ' use true to denote a tile that can't be walked through
    Public ReadOnly IsFloorTile As Boolean ' if false, dissapears when you come close, like a roof
    Public ReadOnly outAreaID As Integer ' used if type = teleporter
    Public ReadOnly outCoords As Vector2 ' used if type = teleporter

    Public Sub New(inType As Integer, Tallness As Boolean, inFloor As Boolean)
        Type = inType
        Tall = Tallness
        IsFloorTile = inFloor
    End Sub

    Public Sub New(outLoc As Vector2, outArea As Integer)
        outAreaID = outArea
        Type = TileTypes.Teleporter
        outCoords = outLoc
        IsFloorTile = True
        Tall = False
    End Sub

    Public Sub New(outArea As Integer)
        outAreaID = outArea
        Type = TileTypes.Teleporter
        IsFloorTile = True
        Tall = False
    End Sub

    Public Enum TileTypes As Integer
        Marsh = 0                        ' use Type to denote Texture 
        Dirt                         ' Feel free to add more to the list
        Rock
        Grass
        Grass_2
        Grass_3
        Grass_4
        Grass_5
        Grass_6
        Mountain
        Mountain_Water
        Mountain_2
        BloodGrass
        BloodSoil
        Wall
        Cave
        Teleporter
        NumOfTypes ' Always the last element in th enum, denotes its size
    End Enum

End Class
