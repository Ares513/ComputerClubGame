Public Class Item

    Dim itemWeight As Double    ' Weight of item
    'Dim tiles As Point      ' Tiles occupied by item x by y
    Public itemTexture As Texture2D
    Public itemSize As Size
    Private type As ItemTypes
    Public ReadOnly Property itemType As ItemTypes
        Get
            Return type
        End Get
    End Property
    Public Sub New(mySize As Size, inTexture As Texture2D, weight As Double, intype As ItemTypes)
        itemSize = mySize
        itemTexture = inTexture
        itemWeight = weight
        type = intype
    End Sub
End Class
Public Enum ItemTypes
    belt
    boots
    buckler
    clothes
    coins100
    coins25
    coins5
    dagger
    gem
    greatbow
    greatstaff
    greatsword
    health_potion
    leather_armor
    longbow
    longsword
    mana_potion
    ring
    rod
    shield
    shortbow
    shortsword
    slingshot
    staff
    steel_armor
    wand
End Enum