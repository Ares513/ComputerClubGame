Public Class DialogBox
    Private text As String
    Private options() As String
    Private speaker As String
    Public Property message As String
        Get
            Return text
        End Get
        Set(value As String)
            text = value
        End Set
    End Property

    Public Property choices As String()
        Get
            Return options
        End Get
        Set(value As String())
            options = choices
        End Set
    End Property
    Public ReadOnly Property speaking As String
        Get
            Return speaker
        End Get
    End Property

    Public Sub New(message As String, options() As String, speaker As String)
        text = message
        Me.options = options
        Me.speaker = speaker
    End Sub
End Class
