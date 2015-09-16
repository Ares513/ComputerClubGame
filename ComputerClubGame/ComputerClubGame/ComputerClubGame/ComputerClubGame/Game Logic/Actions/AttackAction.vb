Public Class AttackAction 
    Implements IAction


    Public Sub doAction(EM As EntityManagement, gt As GameTime, ms As MouseState, ks As KeyboardState, CM As ContentManager, UI As UIOverlay, cam As IsoCamera, Optional CurrentPeriodicCountNumber As Integer = 0) Implements IAction.doAction
        'EM.LocalPlayerInfo.UpdateOrders(ms, cam, True, True)
        If Not EM.LocalPlayerInfo.Attacking Then
            If Not EM.LocalPlayerInfo.Alive Then
                Exit Sub
                'YOU DEAD BOI
            End If
            'If EM.LocalPlayerInfo.CurrentAnimation = "Block" Then
            '    'No moving or attacking while blocking
            '    'EM.LocalPlayerInfo.StopMoving()
            '    Exit Sub
            'End If
            If EM.HilightedEntity(ms, cam) <> "" And EM.HilightedEntity(ms, cam) <> EM.LocalPlayerInfo.ID Then

                'There is a hilighted entity when this IAction ran. If the distance is close enough, attack them.
                Dim target As EntityLiving

                target = EM.getEntityByID(EM.HilightedEntity(ms, cam))
                If target.Alive = True Then
                    EM.LocalPlayerInfo.AIAction = New EntityActionMelee(EM.LocalPlayerInfo, target, EM.LocalPlayerInfo.AttackRange)
                    'If KernsMath.GetDistanceSquareRoot(New Vector2(target.Pos.X, target.Pos.Y), New Vector2(EM.LocalPlayerInfo.Pos.X, EM.LocalPlayerInfo.Pos.Y)) <= EM.LocalPlayerInfo.AttackRange Then
                    '    'Okay. The player is in range, AND the target is hilighted. Commence attack.
                    '    'EM.LocalPlayerInfo.StopMoving()
                    '    EM.LocalPlayerInfo.PlayAnimationOnce("Attack", False)
                    '    EM.LocalPlayerInfo.SetFacing(EM.LocalPlayerInfo.DegreesToFacing(KernsMath.AngleBetweenTwoPoints(target.OffsetCenter, EM.LocalPlayerInfo.OffsetCenter)))
                    '    EM.LocalPlayerInfo.Attack()
                    '    EM.DamageEntity(New Damage(5, 10), target.ID)
                    '    'More elegant attack animation later.
                    'End If
                End If
            ElseIf Not UI.isUIclicked(ms) Then

                EM.LocalPlayerInfo.UpdateOrders(ms, cam, True, True)
            End If

        End If
    End Sub
    Public ReadOnly Property CooldownName As String Implements IAction.CooldownName
        Get
            Return "Attack"
        End Get
    End Property
End Class
