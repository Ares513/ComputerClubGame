Public Class Launcher


    Dim thread As System.Threading.Thread
    Dim ts As Threading.ThreadStart
    Private Sub StartButton_Click(sender As System.Object, e As System.EventArgs) Handles StartButton.Click
        thread.Start()
    End Sub

    Private Sub Launcher_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        ts = New Threading.ThreadStart(AddressOf threadstart)
        thread = New Threading.Thread(ts)
    End Sub

    Sub threadstart()
        Using g As New Game1
            g.Run()
        End Using
    End Sub

    Private Sub ExitButton_Click(sender As System.Object, e As System.EventArgs) Handles ExitButton.Click
        Close()
    End Sub
End Class