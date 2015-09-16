Public Class ScrollWheel

    Inherits UIEntity
    Protected Friend drawPaths(2) As String
    Dim previousState As ScrollWheelReturnTypes
    Dim ScrollWheelImageToDraw As Integer
    Dim ScrollWheelName As String
    Dim ScrollWheelAssetName As String
    Dim ScrollWheelTextures() As Texture2D
    Dim mousePos As Vector2
    Dim mouseState As MouseState
    Dim scrollPreviousState As Long

    Dim thisWheelSelected As Boolean
    Dim heightLimits As Vector2 = New Vector2(-1, -1)
    Dim outOfRange As Boolean =false
    Dim mouseOutOfRange As Boolean = false

    Dim scrollSpeedConstant As Integer


    Public Property Name As String
        Get
            Return ScrollWheelName
        End Get
        Set(value As String)
            ScrollWheelName = value
        End Set
    End Property
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sWName"></param>
    ''' <param name="scrollSize"></param>
    ''' <param name="assetNames"></param>
    ''' <param name="CM"></param>
    ''' <param name="scrollspeed"> this is the divisor for scroll speed. the higher this is the less the wheel will move per size</param>
    ''' <remarks></remarks>
    Public Sub New(sWName As String, scrollSize As size,assetNames() As String, CM As ContentManager, scrollspeed As integer)
        MyBase.New(New Size())
        thisWheelSelected = false
        ScrollWheelName = sWName
        entitySize = scrollSize
        Dim i As Integer
        ReDim ScrollWheelTextures(assetNames.Length - 1)
        For i = 0 To assetNames.Length - 1 Step 1
            ScrollWheelTextures(i) = CM.Load(Of Texture2D)(assetNames(i))
        Next
        scrollSpeedConstant = scrollspeed
    End Sub
    ''' <summary>
    ''' creates an instance of the scroll wheel class
    ''' </summary>
    ''' <param name="sWName"></param>
    ''' <param name="assetNames"></param>
    ''' <param name="CM"></param>
    ''' <param name="hl">small number first to set height limits</param>
    ''' <param name="scrollspeed"> this is the divisor for scroll speed. the higher this is the less the wheel will move per size</param>
    ''' <remarks></remarks>
        Public Sub New(sWName As String, scrollSize As size, assetNames() As String, CM As ContentManager, hl As Vector2,  scrollspeed As integer)
        MyBase.New(New Size())
        thisWheelSelected = false
        ScrollWheelName = sWName
        entitySize = scrollSize
        heightLimits = hl
        Dim i As Integer
        ReDim ScrollWheelTextures(assetNames.Length - 1)
        For i = 0 To assetNames.Length - 1 Step 1
            ScrollWheelTextures(i) = CM.Load(Of Texture2D)(assetNames(i))
        Next
        scrollSpeedConstant = scrollspeed
    End Sub



    ''' <summary>
    ''' Checks if the ScrollWheel is pressed and raises events.
    ''' </summary>
    ''' <param name="MS">Gives the mouse State for position and clicked.</param>
    ''' <param name="gTime">Game1.GameTime instance.</param>
    ''' <returns>Integer representing whether or not the ScrollWheel has been pressed.</returns>
    ''' <remarks></remarks>
    Public Function IsPressed(MS As MouseState, gTime As GameTime) As Integer    
        mousePos = New Vector2(MS.x, MS.Y)
        mouseState = MS
        'check if the position is ouside of the height limits
        If (heightLimits.X <> -1) then
            If (MS.Y < heightLimits.X + Me.entitySize.Height/2)
                mouseOutOfRange = True
            Else If (MS.Y > heightLimits.Y - Me.entitySize.Height/2)
                mouseOutOfRange = true
            Else
                mouseOutOfRange = false
            End If
            If (heightLimits.X <> -1 and Me.entitySize.Height < (heightLimits.Y - heightLimits.X) )
                If (Me.position.y < heightLimits.X)
                    Me.position.Y = heightLimits.X
                    outOfRange = true
                    previousState = ScrollWheelReturnTypes.NOT_SELECTED
                Else If (Me.position.Y + Me.entitySize.Height > heightLimits.Y)
                    Me.position.Y = heightLimits.Y - Me.entitySize.Height
                    outOfRange = true
                    previousState = ScrollWheelReturnTypes.NOT_SELECTED
                Else
                    outOfRange = false
                End If
            end if
        end if
       ' If (gTime.TotalGameTime.Milliseconds Mod delay = 0) Then
            Dim checkRect As Rectangle
            Dim checkPoint As Point
            checkRect = New Rectangle(CInt(Position.X), CInt(Position.Y), entitySize.Width, entitySize.Height)
            checkPoint = New Point(CInt(mousePos.X), CInt(mousePos.Y))
            If (scrollPreviousState <> MS.ScrollWheelValue) then
                RaiseEvent Scrolled
                Return ScrollWheelReturnTypes.HOVERING
            'check to see if mouse is still pressed after clicking the wheel but then moving the mouse
        ElseIf (MS.LeftButton = ButtonState.Pressed And previousState = ScrollWheelReturnTypes.CLICKED) Then
            RaiseEvent Pressed()
            previousState = ScrollWheelReturnTypes.CLICKED
            ScrollWheelImageToDraw = ScrollWheelReturnTypes.CLICKED
            Return ScrollWheelReturnTypes.CLICKED
            Else If checkRect.Contains(checkPoint) Then
            If (MS.LeftButton = ButtonState.Pressed) Then
                RaiseEvent Pressed()
                thisWheelSelected = True
                previousState = ScrollWheelReturnTypes.CLICKED
                ScrollWheelImageToDraw = ScrollWheelReturnTypes.CLICKED
                Return ScrollWheelReturnTypes.CLICKED
                'CLICKED
            Else
                If (previousState = ScrollWheelReturnTypes.NOT_SELECTED) Then
                    RaiseEvent Selected()
                ElseIf (previousState = ScrollWheelReturnTypes.CLICKED) Then
                    RaiseEvent Activated()
                End If
                previousState = ScrollWheelReturnTypes.HOVERING
                ScrollWheelImageToDraw = ScrollWheelReturnTypes.HOVERING
                Return ScrollWheelReturnTypes.HOVERING
                'HOVERING
            End If
            Else
            If (MS.LeftButton = ButtonState.Pressed) Then
                thisWheelSelected = False
            End If
                If (previousState = ScrollWheelReturnTypes.HOVERING Or previousState = ScrollWheelReturnTypes.CLICKED) Then
                    RaiseEvent deselected()
                End If
                previousState = ScrollWheelReturnTypes.NOT_SELECTED
                ScrollWheelImageToDraw = ScrollWheelReturnTypes.NOT_SELECTED
                Return ScrollWheelReturnTypes.NOT_SELECTED
                'NOT_SELECTED
            End If
        'End If


    End Function

    Public Event Pressed()
    Public Event Selected()
    Public Event Deselected()
    Public Event Activated()
    Public Event Scrolled()

    Public Sub moved() Handles Me.Pressed
        If (Not(mouseOutOfRange))
            Me.position.Y = mousePos.Y - CSng(Me.Size.Height / 2)
        Else If (mousePos.Y< heightLimits.X + Me.Size.Height/2)
            Me.position.Y = heightLimits.X
        'Else If (mousePos.Y> heightLimits.y - Me.Size.Height/2)
            'Me.position.Y = heightLimits.Y
        end if
    End Sub

    Public Sub scroll() Handles Me.Scrolled
        If thisWheelSelected and Not(outOfRange) then
            If (Me.position.Y +Me.Size.Height < heightLimits.Y - 1 or (mouseState.ScrollWheelValue - scrollPreviousState)> 0)
                If (Me.position.Y > heightLimits.x + 10 or (mouseState.ScrollWheelValue - scrollPreviousState)< 0)
                    If ((mouseState.ScrollWheelValue - scrollPreviousState)> 0)
                        Me.position.Y -= scrollSpeedConstant
                    Else
                        Me.position.Y += scrollSpeedConstant
                    End If
                end if
            end if
            scrollPreviousState = mouseState.ScrollWheelValue
        Else
            scrollPreviousState = mouseState.ScrollWheelValue
        end if
    End Sub


    Public Overloads Sub Draw(sb As SpriteBatch)
        Dim ScrollWheelRect As Rectangle
        ScrollWheelRect = New Rectangle(CInt(position.X), CInt(position.Y), Size.Width, Size.Height)   
        sb.Draw(ScrollWheelTextures(ScrollWheelImageToDraw), ScrollWheelRect, Color.White)
    End Sub



End Class
Public Enum ScrollWheelReturnTypes
    NOT_SELECTED = 0
    HOVERING = 1
    CLICKED = 2
End Enum