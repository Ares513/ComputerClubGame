Public Class ItemData
    Dim mySize As Size
    Dim code As Integer
    Dim weight As Double
    Dim stats As ItemStats
    Dim itemName As String
    Dim itemType As Integer
    Dim unique As Boolean
    Public Sub New(mySize As Size, itemName As String, weight As Double, stat As ItemStats, itemType As Integer, unique As Boolean)
        Me.mySize = mySize
        Me.weight = weight
        Me.stats = stats
        Me.itemType = itemType
        Me.itemName = itemName
        Me.unique = unique
    End Sub
End Class
