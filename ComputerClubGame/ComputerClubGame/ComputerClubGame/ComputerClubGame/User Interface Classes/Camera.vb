Public Class Camera
    'Contains shared methods for managing the camera.
    Private cameraPosition As Vector2
    Public Property Offset As Vector2
        Get
            Return cameraPosition
        End Get
        Set(value As Vector2)
            cameraPosition = value
        End Set
    End Property
    ''' <summary>
    ''' Calculates what the offset should be based on the player's current position. This ensures that the player always remains in the center of the screen.
    ''' </summary>
    ''' <param name="PlayerCenterPos">The CENTER of the player, not the top-left corner!</param>
    ''' <param name="Screen">Viewport instance used to ensure the player is in the right spot.</param>
    ''' <remarks></remarks>
    Public Sub CalculateOffsetForPlayerCam(PlayerCenterPos As Vector2, Screen As Viewport)
        'Find the screen center.
        Dim distDifference As Vector2
        distDifference = PlayerCenterPos - New Vector2(Screen.Bounds.Center.X, Screen.Bounds.Center.Y)
        'This *should* be the offset.
        cameraPosition = distDifference
    End Sub
End Class
