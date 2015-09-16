'Public Class Inventory
'    Dim inventorySprite As InventorySprite
'    Dim itemSpriteList As New List(Of ItemSprite)
'    Dim itemExampleTexture As Texture2D     'needs to be deleted after a proper input of items is found
'    Dim inventoryKey As Keys = Keys.I
'    Dim inventorySpeed As Integer = 4
'    Dim open As Boolean = False
'    Dim clientBounds As Rectangle


'    Public Sub New(inventorySpeed As Integer, clientBounds As Rectangle, inventoryTexture As Texture2D)
'        Me.inventorySpeed = inventorySpeed
'        Me.clientBounds = clientBounds
'        inventorySprite = New InventorySprite(inventorySpeed, inventoryTexture, clientBounds)
'    End Sub


'    Public Overloads Sub Update(gameTime As GameTime, newKeyboardState As KeyboardState, oldKeyboardState As KeyboardState)
'        If oldKeyboardState.IsKeyUp(inventoryKey) Then
'            If newKeyboardState.IsKeyDown(inventoryKey) Then
'                inventorySprite.OpenBool = Not (inventorySprite.OpenBool)
'            End If
'        End If
'        'End If
'        inventorySprite.Update(gameTime)
'        'For counter As Integer = 0 To itemSpriteList.Count - 1
'        'itemSpriteList(0).Update(gameTime, inventorySprite.PositionX)
'        'Next
'    End Sub

'    Public Sub Draw(spriteBatch As SpriteBatch)
'        inventorySprite.Draw(spriteBatch)
'    End Sub

'End Class
Public Class Inventory
    Dim heldItems As Item ()
    Dim Font As SpriteFont   
    Dim cm As ContentManager
    Dim linkedItemheights As Dictionary(Of Item, integer)
    Dim offset As Integer = 0
    Dim screen As Rectangle
    Public Sub New(gameFont As SpriteFont, CM As ContentManager, screenSize As Rectangle)
        Font = gameFont
        screen = screenSize
        linkedItemheights = New Dictionary(Of Item, integer)
        'invScrollWheel = New ScrollWheel(
    End Sub
    
    Public Sub addItem(input As Item, CM As ContentManager, invScrollWheel As ScrollWheel)
        'add item to held items array
        If (IsNothing(heldItems)) Then
            ReDim heldItems(0)
            heldItems(0) = input
        Else
            ReDim Preserve heldItems(heldItems.Length)
            heldItems(heldItems.Length - 1) = input
        End If

        'fix the height and pos of scroll wheel
        createRelativePos()
        Dim wheelSize As double = screen.Height^2/(linkedItemheights(heldItems(heldItems.Length-1))+10 +heldItems(heldItems.Length-1).itemSize.Height)
        If (invScrollWheel.entitySize.Height/3 > wheelSize -((linkedItemheights(heldItems(heldItems.Length-1))+10 +heldItems(heldItems.Length-1).itemSize.Height) - screen.Height*1.25))
            invScrollWheel.entitySize.Height = CInt(wheelSize - ((linkedItemheights(heldItems(heldItems.Length - 1)) + 10 + heldItems(heldItems.Length - 1).itemSize.Height) - screen.Height * 1.25))
        end if
        If (invScrollWheel.entitySize.Height > screen.Height) Then
            invScrollWheel.entitySize.Height = screen.Height
        End If
    End Sub
    Private Sub createRelativePos()
        If IsNothing(heldItems)
        else
            linkedItemheights = New Dictionary(Of Item, integer)
            For i As Integer = 0 To heldItems.Length - 1 Step 1
                If (i = 0) Then
                    linkedItemheights.Add(heldItems(i), 0)
                else
                    linkedItemheights.Add(heldItems(i), linkedItemheights.Item(heldItems(i - 1)) + 10 + heldItems(i - 1).itemSize.Height)
                end if
            Next
        end if
    End Sub
    Public Function getRelativePos(Optional input As integer = -1) As Integer
        If input = -1
            Return linkedItemheights(heldItems(heldItems.Length-1))
        End If
        return linkedItemheights(heldItems(input))
    End Function
    Public Sub updateObjects(MS As MouseState, gameTime As Microsoft.Xna.Framework.GameTime, invScrollWheel As ScrollWheel, xpos As integer)
        If IsNothing(heldItems)
        else
            invScrollWheel.IsPressed(MS, gameTime)
            offset = CInt(invScrollWheel.Pos.Y)
        end if
    End Sub
    Public Sub draw(sb As SpriteBatch, ms As MouseState, xpos As integer)
        If (Not (IsNothing(heldItems))) Then
            For i As Integer = 0 To heldItems.Length - 1 Step 1
                If (linkedItemheights.Item(heldItems(i)) < screen.Height + offset And linkedItemheights.Item(heldItems(i)) >= offset) Then
                    sb.Draw(heldItems(i).itemTexture, New Rectangle(xpos, 10 + linkedItemheights(heldItems(i)) - offset, heldItems(i).itemSize.Width, heldItems(i).itemSize.Height), Color.White)
                    'sb.DrawString(Font, "weight: " + "insert weight value here", New Vector2(20 + heldItems(i).itemSize.Width, linkedItemheights.Item(heldItems(i))+(20 +  heldItems(i).itemSize.Height) / 12), Color.White)
                    'sb.DrawString(Font, "Value: " + "insert Value value here", New Vector2(20 + heldItems(i).itemSize.Width, linkedItemheights.Item(heldItems(i))+5 * (20 + heldItems(i).itemSize.Height) / 12), Color.White)
                    'sb.DrawString(Font, "Gear Score: " + "insert gear score value here", New Vector2(20 + heldItems(i).itemSize.Width, linkedItemheights.Item(heldItems(i))+9 * (20 + heldItems(i).itemSize.Height) / 12), Color.White)
                End If
            Next
        End If
    End Sub

End Class