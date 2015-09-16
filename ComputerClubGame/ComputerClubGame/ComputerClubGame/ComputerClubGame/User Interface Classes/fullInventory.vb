Public Class fullInventory
    Dim popups As PopupBox ()
    Dim relativeItemSizes As Vector2 ()
    Dim Font As SpriteFont   
    Dim invScrollWheel As ScrollWheel
    Dim inventories As Inventory ()
    Dim tracker As Integer = 0
    Dim offset As Integer = 0
    Dim screen As Rectangle
    Dim background As Texture2D
    Private state As InventoryLocationTypes
    Private currentOffset As Integer
    Private moveSpeed As Integer
    Public Sub New(gameFont As SpriteFont, scrollWheelAssetNames() As String, CM As ContentManager, screenSize As Rectangle,  Optional movingSpeed As Single = 30)
        Font = gameFont
        invScrollWheel = New ScrollWheel("invScroll", New Size(20, 100), scrollWheelAssetNames, CM,New Vector2(0,screenSize.Height), 10)
        invScrollWheel.position.X = screenSize.Width - invScrollWheel.entitySize.Width
        screen = screenSize
        ReDim inventories (3)
        For i as integer =0 to inventories.Length-1 step 1
            inventories(i)= New Inventory(gameFont,CM, screenSize)
        next
        'invScrollWheel = New ScrollWheel(
        background = CM.Load(Of Texture2D)(AssetManager.RequestAsset("parchmentexpanded"))
        moveSpeed = CInt(movingSpeed)
        currentOffset = CInt(screenSize.Width / 2 + 1)
        state = InventoryLocationTypes.IDLE
    End Sub
    Public Sub updateInventory(MS As MouseState, gameTime As Microsoft.Xna.Framework.GameTime)
        For i as integer =0 to inventories.Length-1 step 1
            inventories(i).updateObjects(MS,gameTime, invScrollWheel,900+ 256*i)
        next
        invScrollWheel.position.x = screen.Width - invScrollWheel.entitySize.Width + currentOffset
        invScrollWheel.IsPressed(MS, gameTime)
        CheckSliding()
        Dim trackingNum As Integer
        trackingNum = 0
        Dim storeNum As integer
        storeNum = 0
        If Not IsNothing(popups)
            For i As Integer = 0 to popups.Length-1 Step 1
            
                 popups(i).position = New Vector2(900+256*trackingNum+currentOffset+relativeItemSizes(i).x ,inventories(trackingNum).getRelativePos(storeNum)+relativeItemSizes(i).y/2)
                popups(i).setSelectionRectangle = New Rectangle(900 + 256 * trackingNum + currentOffset, inventories(trackingNum).getRelativePos(storeNum), CInt(relativeItemSizes(i).X), CInt(relativeItemSizes(i).Y))
           
                 trackingNum+=1
                if (trackingNum = 3)
                    trackingNum = 0
                    storeNum +=1
                End If
            Next
        end if
    End Sub
    ''' <summary>
    ''' dont add items with a size bigger than 256 pixels in width
    ''' </summary>
    ''' <param name="input"></param>
    ''' <param name="CM"></param>
    ''' <remarks></remarks>
    Public Sub addItem(input As Item, CM As ContentManager)
        inventories(tracker).addItem(input, CM, invScrollWheel)

        If IsNothing(popups)
            ReDim popups(0)
            ReDim relativeItemSizes(0)
        Else
            ReDim Preserve popups(popups.Length)
            ReDim Preserve relativeItemSizes(relativeItemSizes.Length)
        End If
        relativeItemSizes(relativeItemSizes.Length-1) = New Vector2 (input.itemSize.Width, input.itemSize.Height)
        popups(popups.Length-1) = New PopupBox(New Size(250, 250),New Vector2(0,0),AssetManager.RequestAsset("defaultfont", AssetTypes.SPRITEFONT), CM, New Rectangle(0,0,0,0),"NOTHING THIS IS DYNAMIC", CM.Load(of Texture2D)(AssetManager.RequestAsset("dialogborder")),True,50, 50)
        popups(popups.Length - 1).position = New Vector2(900 + 256 * tracker + currentOffset + input.itemSize.Width, CSng(inventories(tracker).getRelativePos() + input.itemSize.Height / 2))
        popups(popups.Length-1).setSelectionRectangle = New rectangle(900+256*tracker+currentOffset,inventories(tracker).getRelativePos(), input.itemSize.Width,input.itemSize.Height)
        popups(popups.Length-1).Message = "weight: " + "insert weight value here" +" ` "+"DID THIS WORK?"
            
        tracker += 1
        If (tracker >=3)
            tracker = 0
        End If
    End Sub
    Public Sub draw(sb As SpriteBatch, ms As MouseState)
        sb.Draw(background, New Rectangle(CInt(screen.Width / 2 + currentOffset), 0, CInt(screen.Width / 2), screen.Height), Color.White)
        For i As Integer = 0 to inventories.Length-1 Step 1
            inventories(i).draw(sb, ms,900+ 256*i+currentOffset) 'DONT ADD ITEMS BIGGER THAN 256
        Next
        invScrollWheel.Draw(sb)
        If Not IsNothing(popups)
            For i As Integer = 0 to popups.Length-1 Step 1
                popups(i).Draw(sb,ms)
            Next
        end if
    End Sub
        Private Sub CheckSliding()
        If state = InventoryLocationTypes.IDLE Then
            Exit Sub
        End If

        If state = InventoryLocationTypes.CLOSING Then
            'Still not all the way closed.
            currentOffset += moveSpeed
            If currentOffset > screen.Width/2 Then
                currentOffset = CInt(screen.Width / 2 + 1)
                state = InventoryLocationTypes.IDLE  
            End If
        End If

        If state = InventoryLocationTypes.OPENING Then
            currentOffset -= moveSpeed
            If currentOffset <= 1 Then
                currentOffset = 1
                state = InventoryLocationTypes.IDLE                    
            End If
        End If
    End Sub
    Public Sub ToggleInventoryOpening()
        If (currentOffset <= 1 and state = InventoryLocationTypes.IDLE)
            state = InventoryLocationTypes.CLOSING
        Else If (currentOffset >= screen.Width/2 and state = InventoryLocationTypes.IDLE)
            state = InventoryLocationTypes.OPENING
        End If


    End Sub
End Class
Public Enum InventoryLocationTypes As Integer
    IDLE
    OPENING
    CLOSING
End Enum