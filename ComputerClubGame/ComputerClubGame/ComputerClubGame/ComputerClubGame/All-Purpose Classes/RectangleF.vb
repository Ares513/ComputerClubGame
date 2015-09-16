Public Structure RectangleF
    Public X As Single
    Public Y As Single
    Public Width As Single
    Public Height As Single
    Public Sub New(x As Single, y As Single, width As Single, height As Single)
        Me.X = x
        Me.Y = y
        Me.Width = width
        Me.Height = height
    End Sub
    Public Function ToRectangle() As Rectangle
        Return New Rectangle(CInt(X), CInt(Y), CInt(Width), CInt(Height))
    End Function
End Structure
