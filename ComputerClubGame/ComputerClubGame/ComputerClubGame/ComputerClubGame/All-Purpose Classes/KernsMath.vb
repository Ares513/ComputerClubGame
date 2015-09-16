Public Class KernsMath
    Private Sub New()

    End Sub

    Public Shared Function RandInt(lowerBound As Integer, upperBound As Integer) As Integer
        'Dim randomValue As Integer
        'randomValue = CInt(Math.Floor((upperBound - lowerBound + 1) * Rnd())) + lowerBound

        Return New Random().Next(lowerBound, upperBound)
    End Function
    ''' <summary>
    ''' Uses a more efficient version of the square root function where you can determine the precision.
    ''' </summary>
    ''' <param name="DigitsLeftDecimal"></param>
    ''' <param name="Radicand"></param>
    ''' <param name="precision"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function KernsRoot(DigitsLeftDecimal As Integer, Radicand As Double, precision As Integer) As Double
        'Please add comments to explain what these variables actually do.
        Dim I As Integer
        Dim J As Double
        Dim stringnum As String
        Dim N As Integer
        Dim Appx As Double
        Dim lastnum As Double
        stringnum = CStr(Radicand)
        If Math.Round(Radicand) Mod 2 = 0 Then
            N = CInt((DigitsLeftDecimal - 2) / 2)
            Appx = 6 * (10 ^ N)
        Else
            N = CInt((DigitsLeftDecimal - 2) / 2)
            Appx = 2 * (10 ^ N)
        End If
        lastnum = Appx
        For I = 1 To precision Step 1
            J = 0.5 * (lastnum + (Radicand / lastnum))
            lastnum = J
        Next
        Return J
    End Function
    ''' <summary>
    ''' Returns the distance between two points on a line.
    ''' </summary>
    ''' <param name="point1">The first point to compute.</param>
    ''' <param name="point2">The second point to compute.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetDistance(point1 As Vector2, point2 As Vector2) As Double
        Dim distance As Double
        distance = ((point2.X - point1.X) ^ 2 + (point2.Y - point2.X) ^ 2)
        Return distance
    End Function
    Public Shared Function GetDistanceSquareRoot(point1 As Vector2, point2 As Vector2) As Double
        Dim distance As Double
        Dim distancePoint1 As Double
        Dim distancePoint2 As Double
        distancePoint1 = (point2.X - point1.X) ^ 2
        distancePoint2 = (point2.Y - point1.Y) ^ 2
        distance = Math.Sqrt(distancePoint1 + distancePoint2)
        Return distance


    End Function
    ''' <summary>
    ''' Returns an angle that was in 360 degree format (0 to 360) in 180 degree format (0 to 180 to -180)
    ''' </summary>
    ''' <param name="inAngle">The angle to compute.</param>
    ''' <returns>The new angle.</returns>
    ''' <remarks></remarks>
    Public Shared Function AngleConvertTo180(inAngle As Single) As Single
        Dim outAngle As Single
        If inAngle > 180 Then
            outAngle = -1 * (inAngle - 180)
        Else
            outAngle = inAngle
        End If
        Return outAngle
    End Function
    ''' <summary>
    ''' Returns an angle that was in 180 degree format (where -180 and 180 are the respective axes) in 360 deg format.
    ''' </summary>
    ''' <param name="inAngle">The angle to compute.</param>
    ''' <returns>A single ranging from zero to 360.</returns>
    ''' <remarks></remarks>
    Public Shared Function AngleConvert360(inAngle As Single) As Single
        Dim outAngle As Single
        If inAngle < 0 Then
            outAngle = (-1 * inAngle)
            outAngle = (180 - outAngle) + 180
        Else
            outAngle = inAngle
        End If
        Return outAngle
    End Function
    ''' <summary>
    ''' Calculates the Degree value of the angle between two points in 3D space.
    ''' </summary>
    ''' <param name="pt1">The first point to compare.</param>
    ''' <param name="pt2">The second point to compare.</param>
    ''' <returns>An angle from 0 to 360 indicating the angle.</returns>
    ''' <remarks>This function should be used in substitute to heavily repeated code in the UpdateOrders method.</remarks>
    Public Shared Function AngleBetweenTwoPoints(pt1 As Vector3, pt2 As Vector3) As Single
        Dim result As Single
        Dim deltaP As Vector3 = pt1 - pt2
        Dim angle As Double = Math.Atan2(deltaP.Y, deltaP.X)
        result = MathHelper.ToDegrees(CSng(angle))
        Return AngleConvert360(result)

    End Function
    Public Shared Function UnitRotation(StartPos As Vector2, TargetPos As Vector2, UnitSpeed As Double) As Double()
        'The imputs are the map cordnantes, not the screen cordnates
        Dim Distance As Double
        Distance = GetDistance(StartPos, TargetPos)
        Distance = Math.Sqrt(Distance)
        Dim XVel As Double
        Dim YVel As Double
        Dim Rotation As Double
        Dim Loops As Double
        Dim XDist As Double
        Dim YDist As Double
        Dim TurnResult(3) As Double
        '(Rotation(degrees), XVel, YVel,# of loops to reach the destination)
        Loops = Distance / UnitSpeed
        XDist = StartPos.X - TargetPos.X
        YDist = StartPos.Y - TargetPos.Y
        Rotation = Math.Atan2(YDist, XDist)
        XVel = UnitSpeed * Math.Cos(KernsMath.AngleConvert360(CSng(Rotation)))
        YVel = UnitSpeed * Math.Sin(KernsMath.AngleConvert360(CSng(Rotation)))

        Dim vectorCalc As Vector2
        vectorCalc = New Vector2(CSng(XDist), CSng(YDist))
        vectorCalc.Normalize() 'yay, unit vectors!
        vectorCalc.X *= CSng(UnitSpeed)
        vectorCalc.Y *= CSng(UnitSpeed)
        XVel = vectorCalc.X
        YVel = vectorCalc.Y
        TurnResult(0) = Rotation
        TurnResult(1) = XVel * -1
        TurnResult(2) = YVel * -1
        TurnResult(3) = Loops

        Return TurnResult
    End Function

    ''' <summary>
    ''' Creates an isometric Prjection Matrix
    ''' </summary>
    ''' <param name="gd"></param>
    ''' <returns></returns>
    ''' <remarks> to Use, multiply vector by matrix, then by Orthographic Matrix to place on oprthogonal plane</remarks>
    Public Shared Function CreateIsometricProjection(gd As GraphicsDevice) As Matrix
        Dim MatA, MatB As Matrix
        Dim A, B As Double
        A = Math.Asin(Math.Tan(degToRad(30)))
        B = degToRad(45)
        MatA = New Matrix(1, 0, 0, Matrix.Identity.M14, 0, CSng(Math.Cos(A)), CSng(Math.Sin(A)), Matrix.Identity.M24, 0, CSng(-Math.Sin(A)), CSng(Math.Cos(A)), Matrix.Identity.M34, Matrix.Identity.M41, Matrix.Identity.M42, Matrix.Identity.M43, Matrix.Identity.M44)
        MatB = New Matrix(CSng(Math.Cos(B)), 0, -CSng(Math.Sin(B)), Matrix.Identity.M14, 0, 1, 0, Matrix.Identity.M24, CSng(Math.Sin(B)), 0, CSng(Math.Cos(B)), Matrix.Identity.M34, Matrix.Identity.M41, Matrix.Identity.M42, Matrix.Identity.M43, Matrix.Identity.M44)
        Return MatA * MatB
    End Function

    Public Shared Function degToRad(deg As Double) As Double
        Return deg * (Math.PI / 180)
    End Function
    ''' <summary>
    ''' Checks whether or not the specified point is within range of the epicenter.
    ''' This function uses a simplified method of checking a circular radius and may not be exact. However, its performance should be much
    ''' less intensive.
    ''' </summary>
    ''' <param name="epicenter">The center of the radius.</param>
    ''' <param name="pointLocation">The point location to check.</param>
    ''' <param name="radius">The radius to check.</param>
    ''' <returns>A boolean indicating if the point is within range.</returns>
    ''' <remarks></remarks>
    Private Shared Function InexpensiveRangeCheck(epicenter As Vector3, pointLocation As Vector3, radius As Single) As Boolean
        If epicenter.X - radius < pointLocation.X Then

        End If
        Return False
        'Need to write this method.
        'Private because NYI
    End Function
    Public Shared Function ExpensiveRangeCheck(center As Vector3, point As Vector3, radius As Single) As Boolean
        Return Vector3.Distance(center, point) <= radius

    End Function

End Class
Public Class UnitRotationCalcResult
    Public vectorCalc As Vector2
    Public Rotation As Double
    Public Loops As Double
    Public Sub New(vCalc As Vector2, inRot As Double, inLoops As Double)
        vectorCalc = vCalc
        Rotation = inRot
        Loops = inLoops
    End Sub

End Class