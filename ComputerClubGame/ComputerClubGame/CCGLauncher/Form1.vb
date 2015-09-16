Public Class Form1

    Private Sub StartButton_Click(sender As System.Object, e As System.EventArgs) Handles StartButton.Click
        Dim game As ComputerClubGame.Game1
        game = New ComputerClubGame.Game1()
        game.Run()
    End Sub
End Class
