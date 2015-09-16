''' <summary>
''' This class stores static SoundHelper objects that are used frequently so that you don't have to juggle the Content Manager constantly.
''' </summary>
''' <remarks></remarks>
Public Class SoundLibrary
    Shared soundLib As Dictionary(Of String, SoundHelper)
    Shared DefaultSound As SoundHelper
    Shared Sub New()
        soundLib = New Dictionary(Of String, SoundHelper)

    End Sub
    Public Shared Sub AddSound(SoundName As String, Path As String, CM As ContentManager, IsDirectory As Boolean)
        soundLib.Add(SoundName, New SoundHelper(Path, IsDirectory, CM))
    End Sub
    Public Shared Sub SetDefaultSound(Path As String, CM As ContentManager)
        DefaultSound = New SoundHelper(Path, False, CM)

    End Sub
    Public Shared Function GetSoundByName(soundName As String) As SoundHelper
        If IsNothing(soundLib.Item(soundName)) Then
            DebugManagement.WriteLineToLog("Someone requested a null sound!", SeverityLevel.CRITICAL)

            Return DefaultSound
        Else
            Return soundLib.Item(soundName)
        End If

    End Function
End Class
