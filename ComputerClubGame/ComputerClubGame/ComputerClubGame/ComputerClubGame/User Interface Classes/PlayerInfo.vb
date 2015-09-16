Public Class PlayerInfo
    Inherits SlidingWindow
    Private Font As SpriteFont
    Private Backgound As Texture2D
    Private goldIcon As Texture2D
    Private goldIconSize As Size = New Size(32, 32)
    Public Sub New(screenSize As Rectangle, CM As ContentManager)
        MyBase.New(screenSize, False, False)
        Font = CM.Load(Of SpriteFont)(AssetManager.RequestAsset("defaultFont"))
        Backgound = CM.Load(Of Texture2D)(AssetManager.RequestAsset("parchmentExpanded"))
        goldIcon = CM.Load(Of Texture2D)(AssetManager.RequestAsset("goldIcon"))

    End Sub
    Public Sub Update(sb As SpriteBatch, LPInfo As EntityPlayer)
        Dim targetLoc As Vector2 = New Vector2(GetSafeRectangle.X, CSng(GetSafeRectangle.Y + 0.1 * (GetSafeRectangle.Height)))
        sb.Draw(Backgound, GetRectangle, Color.White)
        sb.DrawString(Font, LPInfo.InitialPlayerData.Gold.ToString(), targetLoc, Color.Black)
        targetLoc += Font.MeasureString(LPInfo.InitialPlayerData.Gold.ToString()) / 2
        sb.Draw(goldIcon, New Rectangle(CInt(targetLoc.X - goldIconSize.Width), CInt(targetLoc.Y - goldIconSize.Height / 2), goldIconSize.Width, goldIconSize.Height), Color.White)

        CheckSliding()
    End Sub
End Class
