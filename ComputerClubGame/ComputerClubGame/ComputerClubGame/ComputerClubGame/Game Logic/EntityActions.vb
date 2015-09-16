Public MustInherit Class EntityAction
    Public Sub New()
        currentActionUnit = New NullActionUnit()
    End Sub
    Public Sub increment(dt As TimeSpan) ' dt in milliseconds
        While dt > TimeSpan.Zero
            dt = currentActionUnit.increment(dt)
            If dt > TimeSpan.Zero Then
                currentActionUnit = nextActionUnit()
            End If
        End While
    End Sub

    Protected Interface IEntityActionUnit
        Sub start()
        Function increment(dt As TimeSpan) As TimeSpan ' dt in milliseconds, returns time leftover
        Sub finish()
    End Interface

    Protected Class NullActionUnit
        Implements IEntityActionUnit

        Public Sub finish() Implements IEntityActionUnit.finish

        End Sub

        Public Function increment(dt As System.TimeSpan) As System.TimeSpan Implements IEntityActionUnit.increment
            Return dt
        End Function

        Public Sub start() Implements IEntityActionUnit.start

        End Sub
    End Class

    Protected Class FixedTimeActionUnit
        Implements IEntityActionUnit

        Private length, part As TimeSpan

        Public Sub New(length As TimeSpan)
            Me.length = length
            part = TimeSpan.Zero
        End Sub

        Public Overridable Sub finish() Implements IEntityActionUnit.finish

        End Sub

        Public Overridable Function increment(dt As System.TimeSpan) As System.TimeSpan Implements IEntityActionUnit.increment
            part += dt
            Return If(part > length, part - length, TimeSpan.Zero)
        End Function

        Public Overridable Sub start() Implements IEntityActionUnit.start
            part = TimeSpan.Zero
        End Sub
    End Class

    Protected Class EntityStand
        Inherits FixedTimeActionUnit
        Implements IEntityActionUnit

        Private entity As EntityLiving
        Public Sub New(entity As EntityLiving)
            MyClass.New(TimeSpan.FromSeconds(0.1), entity)
        End Sub
        Public Sub New(length As TimeSpan, entity As EntityLiving)
            MyBase.New(length)
            Me.entity = entity
        End Sub

        Public Overrides Sub start()
            MyBase.start()
            entity.ContinueAnimation(EntityLivingAnimationSet.Animations.Stand, TimeSpan.FromSeconds(1))
        End Sub

        Public Overrides Function increment(dt As TimeSpan) As TimeSpan
            'entity.Pos = New Vector3(entity.Pos.X, entity.Pos.Y, entity.map.GetHeight(entity.Pos.X, entity.Pos.Y))
            Return MyBase.increment(dt)
        End Function
    End Class

    Protected Class EntityWalk
        Implements IEntityActionUnit

        Private entity As EntityLiving
        Private target As NavLoc
        Public Sub New(entity As EntityLiving, target As NavLoc)
            Me.entity = entity
            Me.target = target
        End Sub

        Public Sub finish() Implements IEntityActionUnit.finish

        End Sub

        Public Function increment(dt As TimeSpan) As TimeSpan Implements IEntityActionUnit.increment
            Dim loc As Vector3 = entity.Pos
            Dim tarloc As Vector3 = target.Position
            Dim dif As Vector3 = tarloc - loc
            dif.Z = 0
            Dim dis As Single = dif.Length ' game units
            Dim speed As Single = entity.MoveSpeed ' game units per millisecond
            Dim movedis As Single = speed * CSng(dt.TotalMilliseconds) ' game units
            Dim newPos As Vector3
            If movedis < dis Then
                Dim movevec As Vector3 = dif * movedis / dis
                newPos = loc + movevec
                newPos.Z = entity.map.GetHeight(newPos.X, newPos.Y)
                entity.UpdateFacingToTarget(newPos)
                entity.Pos = newPos
                Return TimeSpan.Zero
            Else
                newPos = tarloc
                newPos.Z = entity.map.GetHeight(newPos.X, newPos.Y)
                entity.UpdateFacingToTarget(newPos)
                entity.Pos = newPos
                Return dt - TimeSpan.FromMilliseconds(dis / speed)
            End If
        End Function

        Public Sub start() Implements IEntityActionUnit.start
            entity.ContinueAnimation(EntityLivingAnimationSet.Animations.Walk, TimeSpan.FromSeconds(0.3))
        End Sub
    End Class

    Protected Class EntityMeleeAttack
        Inherits FixedTimeActionUnit

        Private att, def As EntityLiving
        Public Sub New(attacker As EntityLiving, defender As EntityLiving)
            MyBase.New(TimeSpan.FromSeconds(0.5))
            att = attacker
            def = defender
        End Sub

        Public Overrides Sub start()
            att.StartAnimation(EntityLivingAnimationSet.Animations.Melee, TimeSpan.FromSeconds(0.5))
        End Sub

        Public Overrides Function increment(dt As System.TimeSpan) As System.TimeSpan
            att.UpdateFacingToTarget(def.Pos)
            Return MyBase.increment(dt)
        End Function

        Public Overrides Sub finish()
            MyBase.finish()
            def.TakeDamage(New Damage(5, 10))
        End Sub
    End Class

    Protected Class EntityDying
        Inherits FixedTimeActionUnit

        Private entity As EntityLiving
        Public Sub New(entity As EntityLiving)
            MyBase.New(TimeSpan.FromSeconds(0.5))
            Me.entity = entity
        End Sub

        Public Overrides Sub start()
            MyBase.start()
            entity.StartAnimation(EntityLivingAnimationSet.Animations.Die, TimeSpan.FromSeconds(0.5))
        End Sub
    End Class

    Protected Class EntityDead
        Inherits FixedTimeActionUnit

        Private entity As EntityLiving
        Public Sub New(entity As EntityLiving)
            MyBase.New(TimeSpan.FromSeconds(1))
            Me.entity = entity
        End Sub

        Public Overrides Sub start()
            MyBase.start()
            entity.ContinueAnimation(EntityLivingAnimationSet.Animations.Dead, TimeSpan.FromSeconds(1))
        End Sub
    End Class

    Private actionUnit As IEntityActionUnit
    Private Property currentActionUnit As IEntityActionUnit
        Get
            Return actionUnit
        End Get
        Set(value As IEntityActionUnit)
            If Not IsNothing(actionUnit) Then
                actionUnit.finish()
            End If
            actionUnit = value
            If Not IsNothing(actionUnit) Then
                actionUnit.start()
            End If
        End Set
    End Property

    Protected MustOverride Function nextActionUnit() As IEntityActionUnit
End Class

Public Class EntityActionStand
    Inherits EntityAction

    Private entity As EntityLiving
    Public Sub New(entity As EntityLiving)
        Me.entity = entity
    End Sub

    Protected Overrides Function nextActionUnit() As EntityAction.IEntityActionUnit
        Return New EntityStand(entity)
    End Function
End Class

Public Class EntityActionWalk
    Inherits EntityAction

    Private target As NavLoc
    Private nodes As List(Of NavLoc)
    Private nodeIndex As Integer

    Private entity As EntityLiving
    Public Sub New(entity As EntityLiving, target As NavLoc)
        Me.entity = entity

        nodes = NavPath.GeneratePathBetween(entity.map.GetClosestNavLoc(entity.Pos), target)
        nodeIndex = 0
    End Sub

    Protected Overrides Function nextActionUnit() As EntityAction.IEntityActionUnit
        If IsNothing(nodes) Then Return New EntityStand(entity)
        If nodeIndex < nodes.Count Then
            nodeIndex = nodeIndex + 1
            Return New EntityWalk(entity, nodes(nodeIndex - 1))
        Else
            Return New EntityStand(entity)
        End If
    End Function
End Class

Public Class EntityActionMelee
    Inherits EntityAction

    Private entity, target As EntityLiving
    Private range As Single

    Public Sub New(entity As EntityLiving, target As EntityLiving, range As Single)
        Me.entity = entity
        Me.target = target
        Me.range = range
    End Sub

    Protected Overrides Function nextActionUnit() As EntityAction.IEntityActionUnit
        If Not target.Alive Then Return New EntityStand(entity)
        Dim dist As Single = Vector2.Distance(New Vector2(entity.Pos.X, entity.Pos.Y), New Vector2(target.Pos.X, target.Pos.Y))
        If dist <= range Then
            Return New EntityMeleeAttack(entity, target)
        Else
            Dim nodes As List(Of NavLoc) = NavPath.GeneratePathBetween(entity.map.GetClosestNavLoc(entity.Pos),
                                                                       entity.map.GetClosestNavLoc(target.Pos))
            If IsNothing(nodes) OrElse nodes.Count < 2 Then
                Return New EntityStand(entity)
            Else
                Return New EntityWalk(entity, nodes(1))
            End If
        End If
    End Function
End Class

Public Class MobGuardAction
    Inherits EntityAction

    Private em As EntityManagement
    Private mob As Mob
    Public Sub New(entity As Mob, em As EntityManagement)
        mob = entity

        Me.em = em
    End Sub

    Private Class EntityLookAt
        Inherits FixedTimeActionUnit

        Private target As Vector3

        Private entity As EntityLiving
        Public Sub New(entity As EntityLiving, target As Vector3)
            MyBase.New(TimeSpan.FromSeconds(0.1))
            Me.entity = entity
            Me.target = target
        End Sub

        Public Overrides Function increment(dt As TimeSpan) As TimeSpan
            entity.UpdateFacingToTarget(target)
            'mob.Pos = New Vector3(mob.Pos.X, mob.Pos.Y, mob.map.GetHeight(mob.Pos.X, mob.Pos.Y))
            Return MyBase.increment(dt)
        End Function
    End Class

    Protected Overrides Function nextActionUnit() As EntityAction.IEntityActionUnit
        Dim entityMapLoc As Vector2 = New Vector2(mob.Pos.X, mob.Pos.Y)
        Dim closest As EntityPlayer = Nothing
        Dim mindist As Single = Single.PositiveInfinity
        For Each player As EntityPlayer In em.Players
            If Not player.Alive Then Continue For
            Dim dist As Single = Vector2.Distance(entityMapLoc, New Vector2(player.Pos.X, player.Pos.Y))
            If dist < mindist Then
                closest = player
                mindist = dist
            End If
        Next
        If mindist <= mob.AttackRange Then
            Return New EntityMeleeAttack(mob, closest)
            'Return New EntityLookAt(mob, closest.Pos)
        ElseIf mindist <= mob.AcquisitionRange Then
            Dim nodes As List(Of NavLoc) = NavPath.GeneratePathBetween(mob.map.GetClosestNavLoc(mob.Pos),
                                                                       mob.map.GetClosestNavLoc(closest.Pos))
            If IsNothing(nodes) OrElse nodes.Count < 2 Then
                Return New EntityStand(mob)
            Else
                Return New EntityWalk(mob, nodes(1))
            End If
        Else
            Return New EntityStand(mob)
        End If
    End Function
End Class

Public Class EntityActionDie
    Inherits EntityAction

    Private entity As EntityLiving
    Private died As Boolean = False
    Public Sub New(entity As EntityLiving)
        Me.entity = entity
    End Sub

    Protected Overrides Function nextActionUnit() As EntityAction.IEntityActionUnit
        If died Then
            Return New EntityDead(entity)
        Else
            died = True
            Return New EntityDying(entity)
        End If
    End Function
End Class