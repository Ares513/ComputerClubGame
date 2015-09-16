Public Class PlayerAttribute
    'atributes are STRenght, WISdom, INTelligence, DEXterity, CHArisma, CONstitution, LUCK (in that order)
    Dim Atribute() As String
    'The HP, Mana, Stamina
    Dim VitalPoints() As Integer


    Public Sub New(inAtributes As Integer)
        ReDim VitalPoints(2)
        ReDim Atribute(6)
        'Health

    End Sub

End Class
