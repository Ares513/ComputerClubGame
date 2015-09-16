Public Class NavLoc
    Implements IEquatable(Of NavLoc)
    Implements IComparable(Of NavLoc)

    Private centerX, centerY As Integer
    Private mapRef As Map
    ' Add subdivisions
    Public Sub New(x As Integer, y As Integer, map As Map)
        centerX = x
        centerY = y
        mapRef = map
    End Sub

    Public ReadOnly Property Position As Vector3
        Get
            Return New Vector3(centerX + 0.5F, centerY + 0.5F, mapRef.GetHeight(centerX + 0.5F, centerY + 0.5F))
        End Get
    End Property

    Public ReadOnly Property Map As Map
        Get
            Return mapRef
        End Get
    End Property

    Public ReadOnly Property Neighbors As IEnumerable(Of NavLoc)
        Get
            Dim out As List(Of NavLoc) = New List(Of NavLoc)
            If Map.CanMovePosX(centerX, centerY) Then out.Add(New NavLoc(centerX + 1, centerY, Map))
            If Map.CanMoveNegX(centerX, centerY) Then out.Add(New NavLoc(centerX - 1, centerY, Map))
            If Map.CanMovePosY(centerX, centerY) Then out.Add(New NavLoc(centerX, centerY + 1, Map))
            If Map.CanMoveNegY(centerX, centerY) Then out.Add(New NavLoc(centerX, centerY - 1, Map))
            Return out

        End Get
    End Property

#Region "Comparison Operators"
    Public Overrides Function GetHashCode() As Integer
        Return centerX.GetHashCode() Xor centerX.GetHashCode()
    End Function
    Public Overloads Function Equals(ByVal other As NavLoc) As Boolean Implements System.IEquatable(Of NavLoc).Equals
        Return centerX = other.centerX And centerY = other.centerY
    End Function
    Public NotOverridable Overrides Function Equals(ByVal obj As Object) As Boolean
        Dim temp = TryCast(obj, NavLoc)
        If temp IsNot Nothing Then
            Return Me.Equals(temp)
        Else
            Return False
        End If
    End Function
    Public Shared Operator =(ByVal point1 As NavLoc, ByVal point2 As NavLoc) As Boolean
        Return Object.Equals(point1, point2)
    End Operator
    Public Shared Operator <>(ByVal point1 As NavLoc, ByVal point2 As NavLoc) As Boolean
        Return Not (point1 = point2)
    End Operator

    Public Function CompareTo(other As NavLoc) As Integer Implements System.IComparable(Of NavLoc).CompareTo
        If centerX = other.centerX Then
            Return centerY.CompareTo(other.centerY)
        End If
        Return centerX.CompareTo(other.centerX)
    End Function
    Public Shared Operator <(ByVal point1 As NavLoc, ByVal point2 As NavLoc) As Boolean
        Return point1.CompareTo(point2) < 0
    End Operator
    Public Shared Operator >(ByVal point1 As NavLoc, ByVal point2 As NavLoc) As Boolean
        Return point1.CompareTo(point2) > 0
    End Operator
    Public Shared Operator <=(ByVal point1 As NavLoc, ByVal point2 As NavLoc) As Boolean
        Return point1.CompareTo(point2) <= 0
    End Operator
    Public Shared Operator >=(ByVal point1 As NavLoc, ByVal point2 As NavLoc) As Boolean
        Return point1.CompareTo(point2) >= 0
    End Operator
#End Region
End Class

Public Class NavPath
    'Delegate Function NextNodeFunc(cur As NavLoc) As NavLoc
    Private ReadOnly map As Map
    'Private ReadOnly nextNode As NextNodeFunc
    'Private oldNode, newNode As NavLoc
    Private ReadOnly nodes As List(Of NavLoc)
    Private index As Integer
    Private part As Single

    Public ReadOnly Property IsDone As Boolean
        Get
            Return index >= nodes.Count - 1

        End Get
    End Property
    Private Sub New(path As IEnumerable(Of NavLoc))
        Me.map = path(0).Map
        nodes = New List(Of NavLoc)(path)
        index = 0
        part = 0
        ' every node in path should have the same map
    End Sub
    Public Shared Function StandAt(loc As NavLoc) As NavPath
        Dim nodes As List(Of NavLoc) = New List(Of NavLoc)
        nodes.Add(loc)
        nodes.Add(loc)
        Return New NavPath(nodes)
    End Function

    Private Class NodeWithDistance
        Implements IEquatable(Of NodeWithDistance)
        Implements IComparable(Of NodeWithDistance)

        Public ReadOnly node As NavLoc
        Public ReadOnly dist As Single
        Public ReadOnly parent As NavLoc
        Public Sub New(loc As NavLoc, d As Single, parent As NavLoc)
            node = loc
            dist = d
            Me.parent = parent
        End Sub

        Public Function Neighbors() As IEnumerable(Of NodeWithDistance)
            Dim out As List(Of NodeWithDistance) = New List(Of NodeWithDistance)
            For Each e As NavLoc In node.Neighbors
                out.Add(New NodeWithDistance(e, dist + NavPath.NodeDistance(node, e), node))
            Next
            Return out
        End Function

#Region "Comparison Operators"
        Public Overrides Function GetHashCode() As Integer
            Return node.GetHashCode() Xor dist.GetHashCode()
        End Function
        Public Overloads Function Equals(ByVal other As NodeWithDistance) As Boolean Implements System.IEquatable(Of NodeWithDistance).Equals
            Return node = other.node And dist = other.dist
        End Function
        Public NotOverridable Overrides Function Equals(ByVal obj As Object) As Boolean
            Dim temp = TryCast(obj, NodeWithDistance)
            If temp IsNot Nothing Then
                Return Me.Equals(temp)
            Else
                Return False
            End If
        End Function
        Public Shared Operator =(ByVal point1 As NodeWithDistance, ByVal point2 As NodeWithDistance) As Boolean
            Return Object.Equals(point1, point2)
        End Operator
        Public Shared Operator <>(ByVal point1 As NodeWithDistance, ByVal point2 As NodeWithDistance) As Boolean
            Return Not (point1 = point2)
        End Operator

        Public Function CompareTo(other As NodeWithDistance) As Integer Implements System.IComparable(Of NodeWithDistance).CompareTo
            If dist = other.dist Then
                Return node.CompareTo(other.node)
            End If
            Return dist.CompareTo(other.dist)
        End Function
        Public Shared Operator <(ByVal point1 As NodeWithDistance, ByVal point2 As NodeWithDistance) As Boolean
            Return point1.CompareTo(point2) < 0
        End Operator
        Public Shared Operator >(ByVal point1 As NodeWithDistance, ByVal point2 As NodeWithDistance) As Boolean
            Return point1.CompareTo(point2) > 0
        End Operator
        Public Shared Operator <=(ByVal point1 As NodeWithDistance, ByVal point2 As NodeWithDistance) As Boolean
            Return point1.CompareTo(point2) <= 0
        End Operator
        Public Shared Operator >=(ByVal point1 As NodeWithDistance, ByVal point2 As NodeWithDistance) As Boolean
            Return point1.CompareTo(point2) >= 0
        End Operator
#End Region
    End Class
    Public Shared Function GeneratePathBetween(start As NavLoc, finish As NavLoc) As List(Of NavLoc)
        If start.Map Is finish.Map Then
            Dim dists As Dictionary(Of NavLoc, Single) = New Dictionary(Of NavLoc, Single)
            Dim prev As Dictionary(Of NavLoc, NavLoc) = New Dictionary(Of NavLoc, NavLoc)
            Dim queue As SortedSet(Of NodeWithDistance) = New SortedSet(Of NodeWithDistance)
            queue.Add(New NodeWithDistance(start, 0, Nothing))
            While queue.Count > 0
                Dim node As NodeWithDistance = queue.First()
                queue.Remove(node)
                If Not dists.ContainsKey(node.node) Then
                    dists(node.node) = node.dist
                    prev(node.node) = node.parent
                    If node.node = finish Then Exit While
                    For Each e In node.Neighbors
                        queue.Add(e)
                    Next
                End If
            End While

            If Not dists.ContainsKey(finish) Then Return Nothing
            Dim nodes As List(Of NavLoc) = New List(Of NavLoc)
            Dim nextNode As NavLoc = finish
            While Not IsNothing(nextNode)
                nodes.Add(nextNode)
                nextNode = prev(nextNode)
            End While
            nodes.Reverse()

            Return nodes
        Else
            Return Nothing
        End If
    End Function

    Public Shared Function WalkBetween(start As NavLoc, finish As NavLoc) As NavPath
        Dim nodes As List(Of NavLoc) = GeneratePathBetween(start, finish)
        If IsNothing(nodes) Then
            Return Nothing
        Else
            Return New NavPath(nodes)
        End If
    End Function

    Public ReadOnly Property CurrentPosition As Vector3
        Get
            If index >= nodes.Count - 1 Then
                Return nodes.Last().Position
            Else
                Dim a As NavLoc = nodes(index), b As NavLoc = nodes(index + 1)
                Dim pos As Vector3 = a.Position * (1 - part) + b.Position * part
                Return New Vector3(pos.X, pos.Y, map.GetHeight(pos.X, pos.Y))
            End If
        End Get
    End Property
    Public Function IncrementPosition(distance As Single) As Vector3
        If index >= nodes.Count - 1 Then
            Return CurrentPosition
        End If

        Dim curSegLen As Single = NodeDistance(nodes(index), nodes(index + 1))
        If part * curSegLen + distance >= curSegLen Then
            Dim remainingDist As Single = distance + part * curSegLen - curSegLen
            index += 1
            part = 0
            Return IncrementPosition(remainingDist)
        Else
            part = (part * curSegLen + distance) / curSegLen
            Return CurrentPosition
        End If
    End Function
    Private Shared Function NodeDistance(a As NavLoc, b As NavLoc) As Single
        Dim dif As Vector3 = b.Position - a.Position
        dif.Z = 0
        Return dif.Length()
    End Function
End Class
