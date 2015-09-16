Public Class QuestWindow
    Inherits SlidingWindow
    Protected background As Texture2D
    Protected listBox As scrollBox
    Protected infoBox As scrollBox

    Public Sub New(screenSize As Rectangle, CM As ContentManager)
        MyBase.New(screenSize, False, False)
        Dim swAssets() As String = {AssetManager.RequestAsset("scrololol"), AssetManager.RequestAsset("scrololol"), AssetManager.RequestAsset("scrololol")}

        background = CM.Load(Of Texture2D)(AssetManager.RequestAsset("parchmentexpanded"))
        listBox = New scrollBox(New Vector2(GetSafeRectangle.X + currentOffset.X, GetSafeRectangle.Y + currentOffset.Y), New Size(GetSafeRectangle.Width, GetSafeRectangle.Height), "You don't have any quests to accept.", CM.Load(Of SpriteFont)(AssetManager.RequestAsset("defaultFont", AssetTypes.SPRITEFONT)), swAssets, CM)
        infoBox = New scrollBox(New Vector2(GetSafeRectangle.X + currentOffset.X, GetSafeRectangle.Y + currentOffset.Y), New Size(GetSafeRectangle.Width, GetSafeRectangle.Height), "No quest selected.", CM.Load(Of SpriteFont)(AssetManager.RequestAsset("defaultFont", AssetTypes.SPRITEFONT)), swAssets, CM)

    End Sub
    Public Sub Draw(sb As SpriteBatch, ms As MouseState, gt As GameTime)
        CheckSliding()
        sb.Draw(background, GetRectangle, Color.White)
        listBox.position = New Vector2(GetSafeRectangle.X + currentOffset.X, GetSafeRectangle.Y)
        infoBox.position = New Vector2(GetSafeRectangle.X + currentOffset.X, GetSafeRectangle.Y)
        listBox.updateObjects(ms, gt)
        infoBox.updateObjects(ms, gt)
        listBox.draw(sb)
        infoBox.draw(sb)
    End Sub
End Class
