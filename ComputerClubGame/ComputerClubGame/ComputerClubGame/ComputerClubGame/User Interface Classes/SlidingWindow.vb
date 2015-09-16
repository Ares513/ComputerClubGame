Public MustInherit Class SlidingWindow
#Region "Protected Members"
    Protected workingArea As Rectangle
    Protected state As SkillTreeLocationTypes
    Protected currentOffset As Vector2
    Protected openSpeed As Single
    Private isLeftOrRight As Boolean
    Private isOpenOrClosed As SlidingWindowState
    Private startOpen As Boolean
    Protected BorderInsetAmount As Rectangle
#End Region
#Region "Constructor"
    ''' <summary>
    ''' Creates a new instance of a SlidingWindow object.
    ''' </summary>
    ''' <param name="ScreenSize">The screen's size.</param>
    ''' <param name="startOpen">True to start with the window open, false for closed.</param>
    ''' <param name="leftOrRight">False for starting on the left side, TRUE for right.</param>
    ''' <remarks></remarks>
    Public Sub New(ScreenSize As Rectangle, startOpen As Boolean, leftOrRight As Boolean)
        workingArea = New Rectangle(0, 0, CInt(ScreenSize.Width / 2), ScreenSize.Height)
        If startOpen Then
            'Starting opened.
            state = SkillTreeLocationTypes.FULLY_OPEN
            If Not leftOrRight Then
                'Left Side.
                currentOffset.X = 0
            Else
                'Right side.
                workingArea.X = ScreenSize.Center.X
            End If
        Else
            state = SkillTreeLocationTypes.FULLY_CLOSED
            'Starting closed.
            If Not leftOrRight Then
                'Left Side.
                currentOffset.X = -workingArea.Width
            Else
                'Right side.
                currentOffset.X = workingArea.Width
                workingArea.X = ScreenSize.Center.X
            End If

        End If
        isLeftOrRight = leftOrRight
        openSpeed = 40
        Me.startOpen = startOpen
        BorderInsetAmount = New Rectangle(0, 0, CInt(workingArea.Width * 0.1), CInt(workingArea.Height * 0.1))

    End Sub

#End Region
#Region "Properties"
    Public ReadOnly Property GetRectangle() As Rectangle
        Get
            Return New Rectangle(workingArea.X + CInt(currentOffset.X), CInt(workingArea.Y + currentOffset.Y), workingArea.Width, workingArea.Height)
        End Get
    End Property
    Public ReadOnly Property GetSafeRectangle() As Rectangle
        Get
            Return New Rectangle(GetRectangle.X + BorderInsetAmount.Width, GetRectangle.Y + BorderInsetAmount.Height, GetRectangle.Width - (BorderInsetAmount.Width * 2), GetRectangle.Height - (BorderInsetAmount.Height * 2))

        End Get
    End Property
    Public ReadOnly Property LeftOrRight As Boolean
        Get
            Return isLeftOrRight
        End Get
    End Property
    Public ReadOnly Property OpenOrClosed As SlidingWindowState
        Get
            Return isOpenOrClosed
        End Get
    End Property
    Public ReadOnly Property CurrentState As SkillTreeLocationTypes
        Get
            Return state
        End Get
    End Property
#End Region
#Region "Methods And Events"


    Public Sub ToggleOpening()

        'If Not LeftOrRight Then

        '    'Left side sliding check.

        'End If
        'If currentOffset.X >= -workingArea.Width And state = InventoryLocationTypes.IDLE Then

        'End If
        Select Case state
            Case SkillTreeLocationTypes.CLOSING
                state = SkillTreeLocationTypes.OPENING
                Exit Select

            Case SkillTreeLocationTypes.OPENING
                state = SkillTreeLocationTypes.CLOSING
                Exit Select
            Case SkillTreeLocationTypes.FULLY_OPEN
                state = SkillTreeLocationTypes.CLOSING
                Exit Select
            Case SkillTreeLocationTypes.FULLY_CLOSED
                state = SkillTreeLocationTypes.OPENING
                Exit Select
            Case Else
                DebugManagement.WriteLineToLog("I'm pretty sure we've covered all the bases here.", SeverityLevel.WARNING)
        End Select

        'If (currentOffset.X >= -workingArea.Width And state = InventoryLocationTypes.IDLE) Then
        '    state = SkillTreeLocationTypes.CLOSING
        'ElseIf (currentOffset.X <= workingArea.Width / 2 - 1 And state = InventoryLocationTypes.IDLE) Then
        '    state = SkillTreeLocationTypes.OPENING
        'End If
        'If Not startOpen Then
        '    'Starting closed.


        'End If
        'If state = SkillTreeLocationTypes.CLOSING Then
        '    state = SkillTreeLocationTypes.OPENING
        'ElseIf state <> SkillTreeLocationTypes.IDLE Then
        '    state = SkillTreeLocationTypes.CLOSING
        'End If

    End Sub
    Protected Sub CheckSliding()
        If LeftOrRight = False Then
            If state = SkillTreeLocationTypes.FULLY_CLOSED Or state = SkillTreeLocationTypes.FULLY_OPEN Then
                Exit Sub
            End If
            If workingArea.X + currentOffset.X < -1 * workingArea.Width Then
                'All the way closed.
                state = SkillTreeLocationTypes.FULLY_CLOSED
                isOpenOrClosed = SlidingWindowState.FULLY_CLOSED
            Else
                If state = SkillTreeLocationTypes.CLOSING Then
                    'Still not all the way closed.
                    currentOffset.X -= openSpeed
                    If currentOffset.X < -1 * workingArea.Width Then
                        currentOffset.X = -1 * workingArea.Width
                        state = SkillTreeLocationTypes.FULLY_CLOSED

                    End If
                End If
            End If
            If workingArea.X + currentOffset.X = workingArea.X Then
                'All the way open.
                state = SkillTreeLocationTypes.FULLY_OPEN
                isOpenOrClosed = SlidingWindowState.FULLY_OPEN
            Else
                If state = SkillTreeLocationTypes.OPENING Then
                    currentOffset.X += openSpeed
                    If currentOffset.X > 0 Then
                        currentOffset.X = 0
                        state = SkillTreeLocationTypes.FULLY_OPEN
                    End If
                End If
            End If

            isOpenOrClosed = SlidingWindowState.NO_ENTRY


        Else
            If state = SkillTreeLocationTypes.FULLY_CLOSED Or state = SkillTreeLocationTypes.FULLY_OPEN Then
                Exit Sub
            End If
            If state = SkillTreeLocationTypes.OPENING Then
                If currentOffset.X <= 0 Then
                    state = SkillTreeLocationTypes.FULLY_OPEN
                Else
                    currentOffset.X -= openSpeed
                End If

            End If
            If state = SkillTreeLocationTypes.CLOSING Then
                If workingArea.X + currentOffset.X <= workingArea.Right Then
                    state = SkillTreeLocationTypes.FULLY_CLOSED
                Else
                    currentOffset.X += openSpeed
                End If
            End If
        End If
    End Sub
#End Region
End Class
Public Enum SlidingWindowState As Integer
    NO_ENTRY
    FULLY_OPEN
    FULLY_CLOSED
End Enum
Public Enum SkillTreeLocationTypes As Integer
    FULLY_OPEN
    FULLY_CLOSED
    OPENING
    CLOSING
End Enum
