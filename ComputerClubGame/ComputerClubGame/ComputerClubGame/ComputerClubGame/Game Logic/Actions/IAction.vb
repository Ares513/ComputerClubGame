Imports Microsoft.VisualBasic
Public Interface IAction

    'overloading for doAction
    'Sub doAction()
    Sub doAction(EM As EntityManagement, gt As GameTime, ms As MouseState, ks As KeyboardState, CM As ContentManager, UI As UIOverlay, cam As IsoCamera, Optional CurrentPeriodicCountNumber As Integer = 0)

    ReadOnly Property CooldownName As String

End Interface
