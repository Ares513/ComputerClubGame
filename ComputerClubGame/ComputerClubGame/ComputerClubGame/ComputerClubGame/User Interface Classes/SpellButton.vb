Public Class SpellButton
    Inherits Button
    Private SpellType As SpellTypes
    Public Property Type As SpellTypes
        Get
            Return SpellType
        End Get
        Set(value As SpellTypes)
            SpellType = value
        End Set
    End Property
    Public Sub New(bName As String, assetNames() As String, fontAsset As String, CM As ContentManager, showTitle As Boolean, titleScaleWidth As Single, titleScaleHeight As Single, spellType As SpellTypes)
        MyBase.New(bName, assetNames, fontAsset, CM, showTitle, titleScaleWidth, titleScaleHeight)
        Me.SpellType = spellType
    End Sub
End Class
Public Enum SpellTypes As Byte
    Unassigned = 0
    Instant = 1
    Line = 2
    AoE = 3
    Target_Ground = 4
    Target_Ground_Enemy = 5
    Target_Ground_Ally = 6
    Target_Ground_Any = 7
    Target_Entity = 8
    Target_Entity_Ally = 9
    Target_Entity_Enemy = 10
End Enum