Imports Microsoft.VisualBasic

Public Interface IMap

    ReadOnly Property Width() As Integer
    ReadOnly Property Height() As Integer

    Function TileAt(x As Integer, y As Integer) As TileOld

End Interface