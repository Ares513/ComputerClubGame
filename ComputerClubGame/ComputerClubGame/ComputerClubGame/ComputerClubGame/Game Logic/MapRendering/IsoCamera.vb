Public Class IsoCamera
    Private Offset As Vector2

    Private iVec As Vector2 = New Vector2(-32, 16), jVec As Vector2 = New Vector2(32, 16), kVec As Vector2 = New Vector2(0, -32)
    Private Const tileUnit As Integer = 32
    Public MinLayer, MaxLayer As Single

    'Private WriteOnly Property TileUnit As Integer
    '    Set(value As Integer)
    '        tileSize = value
    '        iVec = New Vector2(-value, value / 2.0F)
    '        jVec = New Vector2(value, value / 2.0F)
    '        kVec = New Vector2(0, -value)
    '    End Set
    'End Property

    Public Sub New(screenCoord As Vector2, mapCoord As Vector3)
        Align(screenCoord, mapCoord)
    End Sub

    Public Sub New()
        Align(Vector2.Zero, Vector3.Zero)
    End Sub

    Public Sub Align(screenCoord As Vector2, mapCoord As Vector3)
        'TileUnit = tileSize
        Offset =
            mapCoord.X * iVec +
            mapCoord.Y * jVec +
            mapCoord.Z * kVec -
            screenCoord
    End Sub

    Public Sub AlignScreenCenter(mapCoord As Vector3, Screen As Viewport)
        Align(New Vector2(Screen.Bounds.Center.X, Screen.Bounds.Center.Y), mapCoord)
    End Sub

    Public Function MapToScreen(mapCoord As Vector3) As Vector2
        Return mapCoord.X * iVec + mapCoord.Y * jVec + mapCoord.Z * kVec - Offset
    End Function

    Public Function ScreenToWorldRay(screenCoord As Vector2) As IsoRay
        Return New IsoRay((screenCoord + Offset) / tileUnit)
    End Function

    Public Function ScreenToMap(screenCoord As Vector2, z As Single) As Vector3
        Return ScreenToWorldRay(screenCoord).AtZ(z)
    End Function

    Public Function ScaleLayer(depth As Single) As Single
        Return (depth - MinLayer) / (MaxLayer - MinLayer)
    End Function
End Class

Public Class IsoRay
    Private XY, YZ, ZX As Vector3

    Public Sub New(scaled As Vector2)
        XY = New Vector3(scaled.Y - scaled.X / 2, scaled.Y + scaled.X / 2, 0)
        YZ = New Vector3(0, scaled.X, -scaled.Y + scaled.X / 2)
        ZX = New Vector3(-scaled.X, 0, -scaled.Y - scaled.X / 2)
    End Sub

    Public Function AtZ(z As Single) As Vector3
        Return XY + New Vector3(z)
    End Function
    Public Function AtX(x As Single) As Vector3
        Return YZ + New Vector3(x)
    End Function
    Public Function AtY(y As Single) As Vector3
        Return ZX + New Vector3(y)
    End Function

    Public Function Intersects(box As BoundingBox) As Boolean
        Return box.Contains(AtZ(box.Max.Z)) <> ContainmentType.Disjoint Or
            box.Contains(AtY(box.Max.Y)) <> ContainmentType.Disjoint Or
            box.Contains(AtX(box.Max.X)) <> ContainmentType.Disjoint
    End Function
End Class