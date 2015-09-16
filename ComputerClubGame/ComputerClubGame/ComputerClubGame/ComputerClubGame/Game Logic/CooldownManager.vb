Public Class CooldownManager
    Private Cooldowns As List(Of Cooldown)
    Public Sub New()
        Cooldowns = New List(Of Cooldown)
    End Sub
    Public Sub AddCooldown(MaximumTime As Integer, Identifier As String)

        Cooldowns.Add(New Cooldown(MaximumTime, MaximumTime, False, Identifier))



    End Sub
    Public Sub RemoveCooldown(Identifier As String)
        For i = 0 To Cooldowns.Count - 1 Step 1
            If Cooldowns(i).ID = Identifier Then
                Cooldowns.RemoveAt(i)
                Exit Sub

            End If
        Next
    End Sub

    Public Sub GarbageCollection()

    End Sub
    Public Sub UpdateCooldowns(gt As GameTime)
        For Each Cooldown In Cooldowns
            Cooldown.Update(gt)
        Next
    End Sub
    ''' <summary>
    ''' Activates the specified cooldown and starts its timer.
    ''' </summary>
    ''' <param name="SearchID">The name of the cooldown. Does nothing if the cooldown does not exist.</param>
    ''' <returns>A Boolean indicating whether or not the cooldown was fired successfully.</returns>
    ''' <remarks></remarks>
    Public Function FireCooldown(SearchID As String, Optional restartIfAlreadyRunning As Boolean = False) As Boolean
        Dim workingCd As Cooldown = getCooldown(SearchID)
        If IsNothing(workingCd) Then
            Return False
        Else
            workingCd.Fire(restartIfAlreadyRunning)
            Return True
        End If
    End Function
    Public Function getCooldown(SearchID As String, Optional SuppressDebuggingWarnings As Boolean = True) As Cooldown
        If IsNothing(SearchID) Then
            SearchID = ""
        End If
        Dim i As Integer
        For i = 0 To Cooldowns.Count - 1 Step 1
            If Cooldowns(i).ID = SearchID Then
                Return Cooldowns(i)
            End If
        Next
        If Not SuppressDebuggingWarnings Then
            DebugManagement.WriteLineToLog("CooldownManager had a String requested for a cooldown that doesn't exist! ID was " + SearchID, SeverityLevel.CRITICAL)
        End If
        Return Nothing
    End Function
    Public Function IsCooldownExpired(SearchID As String) As Boolean
        Dim workingCD As Cooldown
        workingCD = getCooldown(SearchID)
        If IsNothing(workingCD) Then
            Return True
        Else
            If workingCD.isDone Then
                Return True
            Else
                Return False
            End If
        End If

    End Function
End Class
