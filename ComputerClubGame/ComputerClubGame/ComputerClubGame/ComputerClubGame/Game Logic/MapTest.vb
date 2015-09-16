Public Class MapTest
    Implements IMap

    Public Function TileAt(x As Integer, y As Integer) As TileOld Implements IMap.TileAt
        Return New TileOld(0, False, False)
    End Function

    Public ReadOnly Property Width As Integer Implements IMap.Width
        Get
            Return 10
        End Get
    End Property

    Public ReadOnly Property Height As Integer Implements IMap.Height
        Get
            Return 10
        End Get
    End Property

End Class