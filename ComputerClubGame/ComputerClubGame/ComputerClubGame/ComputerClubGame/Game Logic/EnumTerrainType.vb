''' <summary>
''' Contains An Enum of Possible terrain types of areas, each area can only have one terrain type
''' </summary>
''' <remarks></remarks>
Public Class EnumTerrainType
    ''' <summary>
    ''' An Enum of Terrain Types defining the terrain of the generated areas, Names should be self-explanatory
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum TerrainTypes As Integer
        LoadedFromFile = 0
        Plains = 1
        BloodPlains = 2
        Cave = 3
        Forest = 4
        CrystalForest = 5
    End Enum
End Class
