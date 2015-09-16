Public Class InitializationSettings
    Public Sub New()


    End Sub
    Public Shared Function SetupSettings(inputGame As Game) As Game
        inputGame.InactiveSleepTime = New TimeSpan(2, 0, 0)
        inputGame.IsMouseVisible = False
        inputGame.InactiveSleepTime = TimeSpan.Zero

        Return (inputGame)
    End Function
    Public Shared Function GetGameSettings() As GameSettings
        Dim gs As GameSettings
        gs = New GameSettings()
        Return gs
    End Function
End Class
Public Class GameSettings
    Public GameName As String = "ComputerClubGame"
    Public Version As Double = 0.01
    Public globalGameFilesFolder As String
    Public isDebugging As Boolean
    Public Sub New()
        'Check to see if the user is on a 32 bit or 64 bit configuration. If they are we need to divert the Asset folder.
        globalGameFilesFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\" + GameName


    End Sub
End Class