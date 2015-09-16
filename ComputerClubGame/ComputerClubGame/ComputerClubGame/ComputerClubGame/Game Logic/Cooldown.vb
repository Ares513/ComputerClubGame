Public Class Cooldown
    Private RemainingTime As Integer
    Private MaximumTime As Integer
    Private isRepeating As Boolean
    Private Done As Boolean
    Private Identifier As String
    Public ReadOnly Property ID As String
        Get
            Return Identifier
        End Get
    End Property
    Public ReadOnly Property Remaining As Integer
        Get
            Return RemainingTime
        End Get
    End Property
    Public ReadOnly Property Maximum As Integer
        Get
            Return MaximumTime
        End Get
    End Property
    Public ReadOnly Property isDone As Boolean
        Get
            Return Done
        End Get
    End Property
    Public Sub New(inRemainingTime As Integer, inMaximumTime As Integer, inIsRepeating As Boolean, inIdentifier As String)
        RemainingTime = inRemainingTime
        MaximumTime = inMaximumTime
        isRepeating = inIsRepeating
        Identifier = inIdentifier
    End Sub
    Public Sub Update(gt As GameTime)
        If Not Done Then


            RemainingTime = RemainingTime - gt.ElapsedGameTime.Milliseconds
            If RemainingTime <= 0 Then
                Done = True
            End If
        End If
    End Sub
    Public Sub Fire(restartIfAlreadyRunning As Boolean)
        If Done = True Or restartIfAlreadyRunning Then
            RemainingTime = MaximumTime
            Done = False
        End If
        
        'Restart the cooldown.
    End Sub
    Public Function isExpired() As Boolean
        If RemainingTime <= 0 Then
            Return True
        End If
        Return False
    End Function
    Public ReadOnly Property Percentage As Double
        Get
            Return (Maximum - Remaining) / Maximum
        End Get
    End Property
End Class
