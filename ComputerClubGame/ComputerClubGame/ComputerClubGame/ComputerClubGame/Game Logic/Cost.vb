Public Class Cost
    Public GoldCost As Single
    Public ManaCost As Single
    Public HealthCost As Single
    Public Sub New(inGoldCost As Single, inManaCost As Single, inHealthCost As Single)
        GoldCost = inGoldCost
        ManaCost = inManaCost
        HealthCost = inHealthCost
    End Sub
    Public Function CanAfford(Gold As Single, Mana As Single, Health As Single) As Boolean
        If Gold >= GoldCost And Health > HealthCost And Mana >= ManaCost Then
            Return True
        Else
            Return False
        End If
    End Function
End Class
