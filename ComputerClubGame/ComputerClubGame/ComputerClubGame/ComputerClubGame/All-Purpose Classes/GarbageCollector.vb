Public Class GarbageCollector
    Private remaining As Integer
    Private maximum As Integer
    Private cleanupAmount As Integer
    Public ReadOnly Property RemainingLoops As Integer
        Get
            Return remaining
        End Get
    End Property
    Public ReadOnly Property MaximumLoops As Integer
        Get
            Return Maximum
        End Get
    End Property
    Public Event CollectionStarted(e As System.EventArgs, sender As Object)
    Public Sub New(LoopsBetweenEachCollection As UInteger, Optional MaximumCleanup As Integer = 10000)
        maximum = CInt(LoopsBetweenEachCollection)
        remaining = CInt(LoopsBetweenEachCollection)
        cleanupAmount = MaximumCleanup
    End Sub
    Public Sub Update(EM As EntityManagement)
        remaining -= 1
        If remaining = 0 Then
            remaining = maximum
            RaiseEvent CollectionStarted(New EventArgs(), Me)
            Collect(EM)
        End If
    End Sub
    Private Sub Collect(EM As EntityManagement)
        Dim i As Integer = 0
        For i = 0 To cleanupAmount Step 1
            For Each Projectile In EM.Projectiles
                If Projectile.Completed Then
                    EM.Projectiles.Remove(Projectile)
                    Exit For
                End If
            Next
            If EM.Projectiles.Count = 0 Then
                Exit For
                'no sense in sticking around
            End If
        Next

        
    End Sub
End Class
