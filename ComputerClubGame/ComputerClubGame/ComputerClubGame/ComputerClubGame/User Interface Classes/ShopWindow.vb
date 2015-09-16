Public Class ShopWindow
    Inherits SlidingWindow

    Dim background As Texture2D
    Dim screen As Rectangle

    Public Sub New(trader As Player, itemsForSale As Item(), Screensize As Rectangle, cm As ContentManager, font As SpriteFont, inbackground As Texture2D)
        MyBase.New(Screensize, False, True)
        screen = Screensize
        background = inbackground
    End Sub

    Public Sub Draw(sb As SpriteBatch)
        sb.Draw(background, New Rectangle(CInt(screen.Width / 2 + currentOffset.X), 0, CInt(screen.Width / 2), screen.Height), Color.White)
    End Sub

    Public Sub Update()

    End Sub

End Class
