
Public Class SoundHelper
#Region "Properties and declarations"
    Private currentLoop As Integer 'The loop number.
    Private currentLoopMax As Integer 'The maximum number of loops.
    Private currentInterval As Integer 'The current amount of interval remaining in a loop.
    Private initialInterval As Integer 'The interval for each loop.
    Private sounds() As SoundEffect
    Private currentIndex As Integer = 0 'The index to play the sound on. Usually zero.
    Private looping As Boolean = False
    Public pitch, volume, pan As Single
    Public PlayRandomSound As Boolean
    Public ReadOnly Property isLooping As Boolean
        Get
            Return looping
        End Get
    End Property
#End Region

    ''' <summary>
    ''' Creates a new SoundHelper instance.
    ''' </summary>
    ''' <param name="soundOrDirectoryPath">Path of the sound or directory you wish to load.</param>
    ''' <param name="isDirectory">Whether or not the path is a directory. If it is, it will load all files in the directory.</param>
    ''' <param name="CM">ContentManager</param>
    ''' <remarks></remarks>
    Public Sub New(soundOrDirectoryPath As String, isDirectory As Boolean, CM As ContentManager)
        If soundOrDirectoryPath = "" Then
            'Allow the user to not set the String value.
            Exit Sub

        End If
        If isDirectory = True Then
            Dim currentDirInfo As System.IO.DirectoryInfo
            currentDirInfo = New IO.DirectoryInfo(CM.RootDirectory + "\\" + soundOrDirectoryPath)
            Dim contents() As IO.FileInfo
            contents = currentDirInfo.GetFiles()
            If IsNothing(contents) Then

                DebugManagement.WriteLineToLog("SoundHelper encountered an error: sound path not found!", SeverityLevel.WARNING)
                Exit Sub
            End If
            Dim i As Integer
            For i = 0 To contents.Length - 1 Step 1
                'Iterate through the contents and load by path.
                AddSound(soundOrDirectoryPath + "\" + contents(i).Name.ToString().Replace(contents(i).Extension, ""), CM)
            Next
        Else
            'If NOT LOADING a directory but rather a single file
            AddSound(soundOrDirectoryPath, CM)

        End If
        pitch = 0
        volume = 0.5
        pan = 0
    End Sub

    Public Overloads Sub Play()
        If PlayRandomSound Then
            Dim rndSelect As Integer
            rndSelect = KernsMath.RandInt(0, sounds.Length - 1) 'Get the random sound we selected.
            If IsNothing(sounds(rndSelect)) Then
                'Oops. Can't play this sound.
                DebugManagement.WriteLineToLog("SoundHelper encountered an error: A term is empty in the array for Sounds. Randomly selected.", SeverityLevel.WARNING)
                Exit Sub
            End If
            sounds(rndSelect).Play(volume, pitch, pan)

        Else
            'We're playing the default now, not random.
            If IsNothing(sounds(0)) Then
                'The sound is being loaded before sounds have been added. Send to debugger and write a line to the log.
                DebugManagement.WriteLineToLog("SoundHelper encountered an error: No sounds are loaded to this instance!", SeverityLevel.SEVERE)
                Exit Sub
            End If
            sounds(0).Play(volume, pitch, pan) 'Plays the sound once with no effects
        End If

    End Sub
    ''' <summary>
    ''' Begins to play a looped sound. Only call this method once, otherwise it will restart the loop. 
    ''' </summary>
    ''' <param name="interval">Duration in miliseconds between each play. </param>
    ''' <param name="count">Use -1 for an infinite loop. The number of times to play the sound.</param>
    ''' <remarks>Call IterateLoop to update the loop.</remarks>
    Public Overloads Sub BeginPlayAtLoopedInterval(interval As Integer, count As Integer)
        If IsNothing(sounds(0)) Then
            'The sound is being loaded before sounds have been added. Send to debugger and write a line to the log.
            DebugManagement.WriteLineToLog("SoundHelper encountered an error: No sounds are loaded to this instance!", SeverityLevel.SEVERE)
            Exit Sub
        End If
        looping = True
        currentLoopMax = count
        initialInterval = interval

    End Sub
    ''' <summary>
    ''' Begins to play a sound from the specified index. Should only be called each time the loop should run. You cannot have multiple loops at once.
    ''' </summary>
    ''' <param name="interval">Duration in miliseconds between each loop.</param>
    ''' <param name="count">Number of times to loop. -1 for infinite.</param>
    ''' <param name="index">Index to play sounds from. Will not play if unloaded.</param>
    ''' <remarks>Call IterateLoop to update the loop.</remarks>
    Public Overloads Sub BeginPlayAtLoopedInterval(interval As Integer, count As Integer, index As Integer)
        If IsNothing(sounds(index)) Then
            'The sound is being loaded before sounds have been added. Send to debugger and write a line to the log.
            DebugManagement.WriteLineToLog("SoundHelper encountered an error: No sounds are loaded to this instance!", SeverityLevel.SEVERE)
            Exit Sub
        End If
        looping = True
        currentLoopMax = count
        initialInterval = interval
        currentIndex = index
    End Sub
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="gt">GameTime passed from Game1</param>
    ''' <remarks></remarks>
    Public Sub IterateLoop(gt As GameTime)
        If Not looping Then
            Exit Sub 'No sense in continuing if we're not looping. 
        End If
        'Take the elapsed time from the last update and subtract it from the current interval's remaining time.
        currentInterval = currentInterval - gt.ElapsedGameTime.Milliseconds
        If currentInterval < 0 Then
            'We've gotten to the end of a loop. Play the sound.
            Play()
            currentInterval = initialInterval 'Start from the beginning of the interval.
            currentLoop += 1
        End If
        If currentLoop >= currentLoopMax Then
            looping = False
            currentIndex = 0
            'Stop looping, we're done.
        End If
    End Sub
    ''' <summary>
    ''' Plays a sound from the index of the list specified. Does nothing if the index doesn't exist.
    ''' </summary>
    ''' <param name="index">The index to play.</param>
    ''' <remarks></remarks>
    Public Overloads Sub Play(index As Integer)
        If IsNothing(sounds(index)) Then
            'The sound is being loaded before sounds have been added. Send to debugger and write a line to the log.
            DebugManagement.WriteLineToLog("SoundHelper encountered an error: No sounds are loaded to this instance!", SeverityLevel.SEVERE)
            Exit Sub
        End If
        sounds(index).Play(volume, pitch, pan)
    End Sub

    ''' <summary>
    ''' Plays a sound from a point instead of directly. Useful for mob sound effects, spells, etc
    ''' </summary>
    ''' <param name="Position">The position at which to play the sound</param>
    ''' <param name="playerPosition">The player's current position</param>
    ''' <remarks></remarks>
    Public Overloads Sub Play(Position As Vector2, playerPosition As Vector2)

    End Sub
    Public Function soundFalloff(ScreenDim As Vector2, TileDim As Vector2, PlayerPos As Vector2, EventPos As Vector2) As Double
        'This Function makes sounds get quieter the farther away the player is from the occurance
        Dim SoundMod As Double
        'SoundMod is a number between 0 and one that you multiply by the sound's voliume to get the new sound


        Return SoundMod
    End Function
    Public Sub AddSound(Asset As String, CM As ContentManager)
        If IsNothing(sounds) Then
            ReDim Preserve sounds(0)
            sounds(0) = CM.Load(Of SoundEffect)(AssetManager.RequestAsset(Asset, AssetTypes.SOUNDEFFECT))
        Else
            ReDim Preserve sounds(sounds.Length)
            sounds(sounds.Length - 1) = CM.Load(Of SoundEffect)(AssetManager.RequestAsset(Asset, AssetTypes.SOUNDEFFECT))
        End If
    End Sub
End Class
