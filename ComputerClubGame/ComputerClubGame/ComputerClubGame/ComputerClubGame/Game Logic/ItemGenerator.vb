Public Class ItemGenerator
    Dim itemTypeStr As String
    Dim typeTypeStr As String
    Dim magicStr As String
    Dim qualityStr As String
    Public Sub New()
        
    End Sub
    Public Sub Generate(itemType As ItemGenerationType, typeType As Integer, magicType As Integer, quality As Integer)
        Select Case itemType
            Case ItemGenerationType.WEAPON
                Select Case typeType
                    Case WeaponType.LONGSWORD
                        itemTypeStr = "longsword"
                    Case WeaponType.SHORTSWORD
                        itemTypeStr = "shortsword"
                    Case WeaponType.SPEAR
                        itemTypeStr = "spear"
                End Select
                Select Case magicType
                    Case Magic.FIRE

                    Case Magic.ICE

                    Case Magic.LIGHTNING

                    Case Magic.NONE

                End Select
            Case ItemGenerationType.ARMOR
                Select Case typeType
                    Case ArmorType.BOTTOM

                    Case ArmorType.FULL

                    Case ArmorType.SHOE

                    Case ArmorType.TOP

                End Select
                Select Case magicType
                    Case Magic.FIRE

                    Case Magic.ICE

                    Case Magic.LIGHTNING

                    Case Magic.NONE

                End Select
            Case ItemGenerationType.ACCESSORY
                Select Case typeType
                    Case AccessoryType.HAT

                    Case AccessoryType.NECKLACE

                    Case AccessoryType.RING

                End Select
        End Select

    End Sub
End Class

Public Enum ItemGenerationType
    WEAPON = 1
    ARMOR = 2
    ACCESSORY = 3
End Enum
Public Enum WeaponType
    LONGSWORD = 1
    SHORTSWORD = 2
    SPEAR = 3
End Enum
Public Enum ArmorType
    TOP = 1
    BOTTOM = 2
    FULL = 3
    SHOE = 4
End Enum
Public Enum AccessoryType
    RING = 1
    NECKLACE = 2
    HAT = 3
End Enum
Public Enum Magic
    NONE = 0
    FIRE = 1
    ICE = 2
    LIGHTNING = 3
End Enum
Public Enum Quality
    POOR = 1
    BAD = 2
    NORMAL = 3
    GREAT = 4
    EXCELLENT = 5
End Enum