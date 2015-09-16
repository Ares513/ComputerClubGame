Public Class MapManager

    Dim Areas() As Area
    Dim Chapter As Integer
    Dim Current As Integer
    Public Event AreaChanged(sender As Object, e As System.EventArgs, changedArea As Area)

    ''' <summary>
    ''' The Current Area, When Setting Only UID is used, will ignore other properties of the area being set as curent area
    ''' </summary>
    ''' Please be more clear with this comment.
    ''' <remarks></remarks>
    Public Overloads Property CurrentArea As Area
        Get
            Return Areas(Current)
        End Get
        Set(value As Area)
            For i As Integer = 0 To Areas.Length
                If Areas(i).UID = value.UID Then
                    Current = i
                    Exit For
                End If
            Next
        End Set
    End Property
    Public Sub SetCurrentArea(ID As Integer)
        For I As Integer = 0 To Areas.Length - 1 Step 1
            If Areas(I).UID = ID Then
                Current = I
                Exit For
            ElseIf I >= Areas.Length Then
                DebugManagement.WriteLineToLog("Someone tried to set an area at an invalid ID!", SeverityLevel.WARNING)
            End If
        Next
    End Sub
    Public Sub New(inChapter As Integer)
        Chapter = inChapter
        readAreas()
        'GenerateAreas()
        'linkAreas()
    End Sub

    ''' <summary>
    ''' Constructs and initializes new MapManager
    ''' </summary>
    ''' <param name="inChapter">The chapter this MapManager represents</param>
    ''' <param name="SavedAreas">Whether or not this instance has any Areas (Map Sections) Saved to the disk</param>
    ''' <remarks></remarks>
    Public Sub New(inChapter As Integer, SavedAreas As Boolean, firstArea As Area)
        Chapter = inChapter
        If Not SavedAreas Then
            readAreas()
        End If
        ReDim Areas(0)
        Areas(0) = firstArea
        'GenerateAreas()
        'linkAreas()
    End Sub

    Public Function readAreas() As Area()

        'Internal note: We actually only need to load a single area into memory at a time, not the whole chapter. The player can only be in once place at a time.
        'That being said, we may minimize issues transitioning between areas if we load them now. That means fewer loading screens, just a change of setting.
        'Premature optimization is the root of all evil.
        'On a side note, interesting choice to use the ForEach instead of a For. It's fine, but you might want to add a comment.

        'Tiles will be stored as a csv with properties of tiles seperated by colons and rows seperated by |, in the following format, Boolean stored as 1 or 0:
        'First Line Is Dimensions of array stired as \XVAL/YVAL
        'Second line is properties of the area stored as UID$PREVID$NEXTID
        'TileType:Tall:FloorTile, NextTile ... | NextRow | NextRow |...
        'All properties required
        'WhiteSpace, including newlines,  will be ignored
        Dim FilePath As System.IO.DirectoryInfo
        FilePath = New System.IO.DirectoryInfo(InitializationSettings.GetGameSettings.globalGameFilesFolder + "/map/" + CStr(Chapter) + "/")
        If FilePath.Exists Then
            Dim J As Integer
            Dim File() As System.IO.FileInfo = FilePath.GetFiles()
            'For Each File As System.IO.FileInfo In FilePath.GetFiles()
            Dim LoadedAreas(File.Length) As Area
            For J = 0 To File.Length - 1 Step 1
                Dim x, y As Integer
                x = 0
                y = 0
                Dim FileLines() As String = System.IO.File.ReadAllLines(File(J).FullName) ' read lines of text from the file
                Dim ArrayDims As String() = FileLines(0).Split("/".ToCharArray()) 'read from first line the size of the array
                Dim AreaProperties As String() = FileLines(1).Split("$".ToCharArray()) 'read from the second line the propertiues of the area
                LoadedAreas(J) = New Area(CInt(AreaProperties(0)), EnumTerrainType.TerrainTypes.LoadedFromFile) ' CInt(AreaProperties(2)), CInt(AreaProperties(1)))
                LoadedAreas(J).InitializeTiles(CInt(ArrayDims(0)), CInt(ArrayDims(1)))
                Dim I As Integer
                For I = 1 To FileLines.Length - 1 Step 1 'Ignore the first line, it is array size
                    For Each TileRow As String In FileLines(I).Split("|".ToCharArray())
                        For Each Tile As String In TileRow.Split(",".ToCharArray())
                            Dim TileProperties() As String = Tile.Split(":".ToCharArray())
                            LoadedAreas(J).SetTile(x, y, New TileOld(CInt(TileProperties(0)), CBool(TileProperties(1) = "1"), CBool(TileProperties(2) = "1")))
                            x += 1
                        Next
                        y += 1
                    Next
                Next
            Next
            Return LoadedAreas
        Else
            Try
                FilePath.Create()
            Catch ex As Exception
                DebugManagement.WriteLineToLog("Error in Map instance. Cannot create file. Insufficient permissions or stream is already open. Did you remember to dispose your stream? Class: Map", SeverityLevel.CRITICAL)
            End Try
            Return New Area(0) {}
        End If
    End Function

    ''' <summary>
    ''' Generates the areas not loaded from a file
    ''' </summary>
    ''' <param name="AreaCount">The Number of Areas to Generate, usually the number of areas in the chapter noot loaded from a file</param>
    ''' <param name="DistFromStart">Array of integers representing the Distance, in areas, from the town of all the areas to be generated. Therefore, this should always be at least 1.</param>
    ''' <param name="TerrainType"> Array of integers representing of the terrain types of the areas, where each index represents the area at the same index of DistFromStart</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GenerateAreas(AreaCount As Integer, DistFromStart As Integer(), TerrainType As Integer()) As TileOld()(,)
        'Dim GenAreas()(,) As Tile
        Return New TileOld(0)(,) {}
    End Function

    Public Sub AddArea(AreaToAdd As Area)
        ReDim Preserve Areas(Areas.Length)
        Areas(Areas.Length - 1) = AreaToAdd
    End Sub

    Public Sub update(mouseState As MouseState, cam As IsoCamera, CM As ContentManager, sb As SpriteBatch)
        Dim T As TileOld = Areas(Current).Update(mouseState, cam, CM, sb)
        If Not IsNothing(T) Then
            SetCurrentArea(T.outAreaID)
            RaiseEvent AreaChanged(Me, New EventArgs(), CurrentArea)
        End If
    End Sub

    Public Function IsInMap(tile As Vector3) As Boolean
        If tile.X < CurrentArea.Width And tile.Y < CurrentArea.Height And tile.X < 0 And tile.Y < 0 Then
            Return True
        End If
        Return False
    End Function
End Class
