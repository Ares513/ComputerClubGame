Public Class DroppedItemManagement
    Protected Friend Items As List(Of DroppedItem)
    Private overDrawnFont As SpriteFont
    Private boxBackground As Texture2D
    Public Event ItemPickedUp(sender As DroppedItemManagement, e As System.EventArgs, pickedUpItem As DroppedItem)
    Public Sub New(CM As ContentManager)
        Items = New List(Of DroppedItem)
        boxBackground = CM.Load(Of Texture2D)(AssetManager.RequestAsset("defaultProgressBarFull"))
        overDrawnFont = CM.Load(Of SpriteFont)(AssetManager.RequestAsset("defaultFont", AssetTypes.SPRITEFONT))
    End Sub
    Public Sub AddItem(item As DroppedItem)
        Items.Add(item)
        AddHandler item.ItemPickedUp, AddressOf NewItemPickedUp
    End Sub
    Private Sub NewItemPickedUp(sender As DroppedItem, e As System.EventArgs)
        RaiseEvent ItemPickedUp(Me, New EventArgs(), sender)
    End Sub
    Public Sub Draw(sb As SpriteBatch, Cam As IsoCamera, gt As GameTime, ms As MouseState)
        Dim collisionBoxes As List(Of Rectangle) = New List(Of Rectangle)
        If Items.Count > 0 Then
            For i = 0 To Items.Count - 1 Step 1


                Items(i).Update(gt, ms, Cam)
                If Items(i).SelectedOnLastUpdate Then
                    Dim nameSize As Vector2 = overDrawnFont.MeasureString(Items(i).Name)
                    Dim screenDrawnPos As Vector2 = Cam.MapToScreen(Items(i).Pos)
                    Dim screenCenterPos As Point = New RectangleF(screenDrawnPos.X + Items(i).CollisionInset.X, screenDrawnPos.Y + Items(i).CollisionInset.Y, Items(i).CollisionInset.Width, Items(i).CollisionInset.Height).ToRectangle().Center
                    screenDrawnPos.Y -= nameSize.Y
                    screenCenterPos.X -= CInt(nameSize.X / 2)
                    screenCenterPos.Y -= CInt(nameSize.Y)
                    sb.DrawString(overDrawnFont, Items(i).Name, New Vector2(screenCenterPos.X, screenCenterPos.Y), Color.Black)

                End If
                If Not Items(i).PickedUp Then
                    Items(i).Draw(sb, Cam)
                End If


            Next
        End If
        Dim pred As System.Predicate(Of DroppedItem)
        pred = AddressOf DroppedItemManagement.IsItemCollected
        'cleanup
        Items.RemoveAll(pred)
    End Sub
    Private Shared Function IsItemCollected(item As DroppedItem) As Boolean
        Return item.PickedUp
    End Function
End Class
