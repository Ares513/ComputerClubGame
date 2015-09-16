Public Class PeriodicEffect
    Implements IEffect
    Dim Updating As Boolean
    Dim Action As IAction
    Dim CompletionAction As IAction
    Dim Number As Integer
    Dim DelayInMS As Integer
    Dim remainingTime As Integer
    Dim remainingNumber As Integer
    Dim NumberScaleAmount As Integer
    Dim Instant As Boolean
    Public Sub Start()
        Updating = True
    End Sub
    Public Sub Pause()
        Updating = False
    End Sub
    Public Sub New(CompleteAction As IAction, UpdateAction As IAction, UpdateNumber As UInteger, UpdateDelayInMS As UInteger, Optional remainingNumberScaleAmount As Integer = 1, Optional RunInstantly As Boolean = False)
        Action = UpdateAction
        Number = CInt(UpdateNumber)
        DelayInMS = CInt(UpdateDelayInMS)
        remainingTime = DelayInMS
        remainingNumber = Number
        NumberScaleAmount = remainingNumberScaleAmount
        Instant = RunInstantly
        Start()
    End Sub
    Public Property isUpdating As Boolean Implements IEffect.isUpdating
        Get
            Return Updating
        End Get
        Set(value As Boolean)
            Updating = value
        End Set
    End Property

    Public Sub Update(EM As EntityManagement, gt As Microsoft.Xna.Framework.GameTime, ms As Microsoft.Xna.Framework.Input.MouseState, ks As Microsoft.Xna.Framework.Input.KeyboardState, CM As Microsoft.Xna.Framework.Content.ContentManager, UI As UIOverlay, cam As IsoCamera) Implements IEffect.Update
        'If marked as instant, run the entire action immediately.
        If Instant Then
            While remainingNumber >= 0
                remainingNumber -= 1
                Action.doAction(EM, gt, ms, ks, CM, UI, cam, remainingNumber * NumberScaleAmount)

            End While
            isUpdating = False
        End If
        If isUpdating Then
            If remainingNumber <= 0 Then
                isUpdating = False
                Exit Sub
            End If
            remainingTime -= gt.ElapsedGameTime.Milliseconds
            If remainingTime < 0 Then
                remainingTime = CInt(DelayInMS + remainingTime)
                'Run the action, take away a loop number.
                remainingNumber -= 1

                Action.doAction(EM, gt, ms, ks, CM, UI, cam, remainingNumber * NumberScaleAmount)
            End If
        End If

    End Sub
End Class
