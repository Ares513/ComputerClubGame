
Public Class PopupBox
    Inherits TextProcessor
    Private isSelected As Boolean
    Private SelectionRect As Rectangle
    Private backTexture As Texture2D
    Public drawBackTexture As Boolean
    Public Offset As Vector2 = New Vector2(0, 0)
    Public Property Selected As Boolean
        Get
            Return isSelected
        End Get
        Set(value As Boolean)
            isSelected = value
        End Set
    End Property
    Public ReadOnly Property SelectionRectangle As Rectangle
        Get
            Return SelectionRect
        End Get
    End Property
    Public WriteOnly Property setSelectionRectangle As Rectangle
        Set(value As Rectangle)
            SelectionRect = value
        End Set
    End Property
    Public Sub New(boxSize As Size, defaultCenter As Vector2, fontAsset As String, CM As ContentManager, rectangleSelection As Rectangle, message As String, inBackTexture As Texture2D, useBackTexture As Boolean, insetAmountWidth As Integer, insetAmountHeight As Integer)

        MyBase.New(boxSize, New Vector2(defaultCenter.X - CInt(boxSize.Width / 2), defaultCenter.Y - CInt(boxSize.Height / 2)), CM.Load(Of SpriteFont)(AssetManager.RequestAsset(fontAsset, AssetTypes.SPRITEFONT)), CM, message, insetAmountWidth, insetAmountHeight)
        Dim assetMcLoaderson As String = AssetManager.RequestAsset(fontAsset, AssetTypes.SPRITEFONT)
        SelectionRect = rectangleSelection
        backTexture = inBackTexture
        drawBackTexture = useBackTexture


    End Sub
    Public Shadows Sub Draw(sb As SpriteBatch, ms As MouseState)
       
        If SelectionRectangle.Contains(New Point(ms.X, ms.Y)) Then
            isSelected = True
        Else
            isSelected = False
        End If
        If isSelected Then
            If drawBackTexture Then
                If IsNothing(backTexture) Then
                    DebugManagement.WriteLineToLog("drawBackground is enabled, but the texture isn't loaded!", SeverityLevel.WARNING)
                Else
                    sb.Draw(backTexture, New Rectangle(CInt(Pos.X + Offset.X), CInt(Pos.Y + Offset.Y), Size.Width, Size.Height), Color.White)
                End If
            End If

            Dim textRect As Rectangle
            textRect = New Rectangle(CInt(position.X + Offset.X), CInt(position.Y + Offset.Y), Size.Width, Size.Height)
            Dim textSize As Vector2 = Font.MeasureString(displayMessage)
            corLengthStrings = wrapTextLines(Message, Me.entitySize.Width - 2 * BorderWidth)
            For i As Integer = 0 To corLengthStrings.Length - 1 Step 1
                Dim textPos As Vector2 = New Vector2(position.X + Offset.X + BorderWidth, position.Y + Offset.Y + BorderHeight + i * textSize.Y)
                sb.DrawString(Font, corLengthStrings(i), textPos, Color)
            Next
        End If
    End Sub
End Class
