Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Content
Imports Microsoft.Xna.Framework.Graphics

''' <summary>
''' This class will be instantiated by the XNA Framework Content
''' Pipeline to read the specified data type from binary .xnb format.
''' 
''' Unlike the other Content Pipeline support classes, this should
''' be a part of your main game project, and not the Content Pipeline
''' Extension Library project.
''' </summary>
Public Class TileSetReader
    Inherits ContentTypeReader(Of TileSet)

    Public Loaded As Dictionary(Of String, TileSet) = New Dictionary(Of String, TileSet)

    Protected Overrides Function Read(input As ContentReader, existingInstance As TileSet) As TileSet
        If Not Loaded.ContainsKey(input.AssetName) Then
            Loaded.Add(input.AssetName, New TileSet(input))
        End If
        Return Loaded.Item(input.AssetName)
    End Function
End Class

''' <summary>
''' This class will be instantiated by the XNA Framework Content
''' Pipeline to read the specified data type from binary .xnb format.
''' 
''' Unlike the other Content Pipeline support classes, this should
''' be a part of your main game project, and not the Content Pipeline
''' Extension Library project.
''' </summary>
Public Class MapReader
    Inherits ContentTypeReader(Of Map)
    Protected Overrides Function Read(input As ContentReader, existingInstance As Map) As Map
        Return New Map(input)
    End Function
End Class

Public Class Map
    Private Class TileStack
        Private ReadOnly tiles As List(Of Tile)
        Private ReadOnly lastTile As Tile
        Public ReadOnly depth, back, left, right, front As Integer
        Public ReadOnly Property effectiveDepth As Integer
            Get
                Return depth + If(tiles.Last().IsBlock, 1, 0)
            End Get
        End Property
        Public ReadOnly Property passable As Boolean
            Get
                Return lastTile.passable
            End Get
        End Property

        Public Sub New(ByRef tileSets As List(Of TileSet), input As ContentReader)
            Dim numtiles As Integer = (input.ReadByte() << 8) Or input.ReadByte()
            tiles = New List(Of Tile)(numtiles)
            Dim z As Integer = 0
            For i As Integer = 0 To numtiles - 1
                Dim t As Tile = tileSets(input.ReadByte()).GetTile(input.ReadByte())
                tiles.Add(t)
                If t.IsBlock Then
                    z += 1
                End If
            Next
            If numtiles = 0 Then
                lastTile = New Tile(Nothing, 0)
            Else
                lastTile = tiles.Last()
            End If
            If lastTile.IsBlock Then
                z -= 1
            End If
            depth = z
            back = z + If(lastTile.back, 1, 0)
            left = z + If(lastTile.left, 1, 0)
            right = z + If(lastTile.right, 1, 0)
            front = z + If(lastTile.front, 1, 0)
        End Sub

        Public Sub Draw(sb As SpriteBatch, cam As IsoCamera, x As Integer, y As Integer)
            Dim z As Integer = 0
            Dim layerHint = 0
            For Each t As Tile In tiles
                t.texture.Draw(sb, cam, New Vector3(x, y, z), layerHint - 10)
                layerHint += 1
                If t.IsBlock Then
                    z += 1
                    layerHint = 0
                End If
            Next
        End Sub

        Public Function GetHeight(x As Single, y As Single) As Single
            ' 0 <= x < 1 && 0 <= y < 1
            Return depth + lastTile.Height(x, y)
        End Function

        Public Function IsWalkableAndFlatAt(z As Integer) As Boolean
            Return effectiveDepth = z AndAlso lastTile.passable AndAlso lastTile.IsFlat
        End Function

        Public Shared Function MatchX(first As TileStack, second As TileStack) As Boolean
            Return first.left = second.back And first.front = second.right
        End Function
        Public Shared Function MatchY(first As TileStack, second As TileStack) As Boolean
            Return first.right = second.back And first.front = second.left
        End Function
    End Class

    
    Private ReadOnly tileSets As List(Of TileSet)
    Private ReadOnly tiles As List(Of List(Of TileStack)) ' tiles(y)(x)
    Public ReadOnly width As Integer, height As Integer, maxDepth As Integer
    Public ReadOnly Property MaxLayer As Integer
        Get
            Return width + height + maxDepth
        End Get
    End Property

    Public Sub New(input As ContentReader)
        Dim numtsets As Integer = input.ReadByte()
        tileSets = New List(Of TileSet)(numtsets)
        For i As Integer = 0 To numtsets - 1
            Dim tsetnamelen As Integer = input.ReadByte()
            Dim tsetname As String = Encoding.UTF8.GetString(input.ReadBytes(tsetnamelen))
            tileSets.Add(input.ContentManager.Load(Of TileSet)(tsetname))
        Next
        width = ReadHalf(input)
        height = ReadHalf(input)
        tiles = New List(Of List(Of TileStack))(height)
        For y As Integer = 0 To height - 1
            tiles.Add(New List(Of TileStack)(width))
            For x As Integer = 0 To width - 1
                tiles(y).Add(New TileStack(tileSets, input))
                If tiles(y)(x).effectiveDepth > maxDepth Then
                    maxDepth = tiles(y)(x).effectiveDepth
                End If
            Next
        Next
    End Sub

    Private Function ReadHalf(input As ContentReader) As Integer ' Big Endian
        Return (input.ReadByte()) << 8 Or input.ReadByte()
    End Function

    Public Sub Draw(sb As SpriteBatch, cam As IsoCamera)
        For y As Integer = 0 To height - 1
            For x As Integer = 0 To width - 1
                tiles(y)(x).Draw(sb, cam, x, y)
                
            Next
        Next
    End Sub

    Public Function GetHeight(x As Single, y As Single) As Single
        Dim xi As Integer = CInt(Math.Floor(x)), yi As Integer = CInt(Math.Floor(y))
        Dim output As Single
        If yi > tiles.Count - 1 Then
            yi = tiles.Count - 1
        ElseIf yi < 0 Then
            yi = 0
        End If

        If xi > tiles(yi).Count - 1 Then
            xi = tiles(yi).Count - 1
        ElseIf xi < 0 Then
            xi = 0
        End If
        output = tiles(yi)(xi).GetHeight(x - xi, y - yi)

        Return output
    End Function

    Public Function GetWalkableLoc(ray As IsoRay) As NavLoc
        For z As Integer = maxDepth To 0 Step -1
            Dim loc As Vector3 = ray.AtZ(z)
            Dim xi As Integer = CInt(Math.Floor(loc.X)), yi As Integer = CInt(Math.Floor(loc.Y))
            If ValidPos(xi, yi) AndAlso tiles(yi)(xi).IsWalkableAndFlatAt(z) Then
                Return New NavLoc(xi, yi, Me)
            End If
        Next
        Return Nothing
    End Function

    Public Function GetClosestNavLoc(pos As Vector3) As NavLoc
        Return New NavLoc(CInt(Math.Floor(pos.X)), CInt(Math.Floor(pos.Y)), Me)
    End Function

    Public Function ValidPos(x As Integer, y As Integer) As Boolean
        Return x >= 0 And x < width And y >= 0 And y < height
    End Function
    Private Function MatchSide(oldPt As Boolean, newPt As Boolean, depthDif As Integer) As Boolean 'depthDif = newD-oldD
        If depthDif = 0 Then
            Return oldPt = newPt
        ElseIf depthDif = 1 Then
            Return oldPt And Not newPt
        ElseIf depthDif = -1 Then
            Return Not oldPt And newPt
        Else
            Return False
        End If
    End Function

    Public Enum Directions
        PosX
        NegX
        PosY
        NegY
    End Enum

    Public Function CanMove(x As Integer, y As Integer, d As Directions) As Boolean
        Select Case d
            Case Directions.PosX
                Return CanMovePosX(x, y)
            Case Directions.NegX
                Return CanMoveNegX(x, y)
            Case Directions.PosY
                Return CanMovePosY(x, y)
            Case Directions.NegY
                Return CanMoveNegY(x, y)
            Case Else
                Return False
        End Select
    End Function
    Public Function CanMovePosX(x As Integer, y As Integer) As Boolean
        Return (ValidPos(x, y) And ValidPos(x + 1, y)) AndAlso (TileStack.MatchX(tiles(y)(x), tiles(y)(x + 1)) And tiles(y)(x + 1).passable)
    End Function
    Public Function CanMoveNegX(x As Integer, y As Integer) As Boolean
        Return (ValidPos(x, y) And ValidPos(x - 1, y)) AndAlso (TileStack.MatchX(tiles(y)(x - 1), tiles(y)(x)) And tiles(y)(x - 1).passable)
    End Function
    Public Function CanMovePosY(x As Integer, y As Integer) As Boolean
        Return (ValidPos(x, y) And ValidPos(x, y + 1)) AndAlso (TileStack.MatchY(tiles(y)(x), tiles(y + 1)(x)) And tiles(y + 1)(x).passable)
    End Function
    Public Function CanMoveNegY(x As Integer, y As Integer) As Boolean
        Return (ValidPos(x, y) And ValidPos(x, y - 1)) AndAlso (TileStack.MatchY(tiles(y - 1)(x), tiles(y)(x)) And tiles(y - 1)(x).passable)
    End Function
End Class

Public Class TileSet
    Private ReadOnly texture As Texture2D
    Private ReadOnly tiles As Dictionary(Of Byte, Tile)
    Public Sub New(input As ContentReader)
        Dim texnamelen As Integer = input.ReadByte()
        Dim texname As String = Encoding.UTF8.GetString(input.ReadBytes(texnamelen))
        texture = input.ContentManager.Load(Of Texture2D)(texname)
        tiles = New Dictionary(Of Byte, Tile)
        Dim numtiles As Integer = input.ReadByte()
        For i As Integer = 0 To numtiles - 1
            Dim Origin As Byte = input.ReadByte()
            Dim OriginX As Integer = Origin And 15, OriginY As Integer = (Origin >> 4) And 15
            Dim Flags As Byte = input.ReadByte()
            Dim tex As IsoTexture
            If (Flags And 64) <> 0 Then
                Dim TopLeft As Byte = input.ReadByte()
                Dim LeftX As Integer = TopLeft And 15, TopY As Integer = (TopLeft >> 4) And 15
                Dim BottomRight As Byte = input.ReadByte()
                Dim RightX As Integer = BottomRight And 15, BottomY As Integer = (BottomRight >> 4) And 15
                tex = New IsoTexture(texture, New Rectangle(64 * LeftX, 64 * TopY, 64 * (RightX - LeftX + 1), 64 * (BottomY - TopY + 1)),
                                     New Vector2(64 * (OriginX - LeftX) + 32, 64 * (OriginY - TopY) + 32))
            Else
                tex = New IsoTexture(texture, New Rectangle(64 * OriginX, 64 * OriginY, 64, 64), New Vector2(32, 32))
            End If
            tiles(Origin) = New Tile(tex, Flags)
        Next
    End Sub
    Public Function GetTile(tileIndex As Byte) As Tile
        Return tiles(tileIndex)
    End Function
End Class

Public Class Tile
    Public ReadOnly texture As IsoTexture
    Public ReadOnly back, left, right, front, passable As Boolean
    'Public ReadOnly topper As Boolean
    Public ReadOnly Property IsBlock As Boolean
        Get
            Return back And left And right And front
        End Get
    End Property
    Public ReadOnly Property IsFloor As Boolean
        Get
            Return Not back And Not left And Not right And Not front
        End Get
    End Property
    Public ReadOnly Property IsFlat As Boolean
        Get
            Return IsBlock Or IsFloor
        End Get
    End Property

    Public Sub New(tex As IsoTexture, flags As Byte)
        texture = tex
        back = (flags And 1) <> 0
        left = (flags And 2) <> 0
        right = (flags And 4) <> 0
        front = (flags And 8) <> 0
        'topper = (flags And 16) <> 0
        passable = (flags And 32) <> 0
    End Sub

    Public Function Height(x As Single, y As Single) As Single
        Return (If(right, y, 0) + If(back, (1 - y), 0)) * (1 - x) + (If(front, y, 0) + If(left, (1 - y), 0)) * x
    End Function
End Class
