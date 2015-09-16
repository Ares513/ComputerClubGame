Public Class ItemList
    Dim itemList As List(Of ItemData)
    Public Sub New()
    End Sub
    Public Sub Add(itemData As ItemData)
        itemList.Add(itemData)
    End Sub
End Class
