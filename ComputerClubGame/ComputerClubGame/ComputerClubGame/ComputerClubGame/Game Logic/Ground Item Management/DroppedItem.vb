Public Class DroppedItem
    Inherits Entity
    Private ItemDisplayName As String
    Private isGold As Boolean
    Private defaultAnim As Animation
    Private hilighted As Boolean
    Private goldAmount As Integer
    Friend CollisionInset As Rectangle = New Rectangle(16, 64, 32, 64)
    Private isPickedUp As Boolean
    Private inItem As Item
    Public Event ItemPickedUp(sender As DroppedItem, e As System.EventArgs)
    Public ReadOnly Property PickedUp As Boolean
        Get
            Return isPickedUp
        End Get
    End Property
    Public ReadOnly Property SelectedOnLastUpdate As Boolean
        Get
            Return hilighted
        End Get
    End Property
    Public ReadOnly Property getItem As Item
        Get
            return inItem
        End Get
    End Property
    Public ReadOnly Property GoldAmt As Integer
        Get
            If Not isGold Then
                Return -1
            End If
            Return goldAmount
        End Get
    End Property
    Public ReadOnly Property IsGoldDrop As Boolean
        Get
            Return isGold
        End Get
    End Property
    Public Overrides Property Name As String
        Get
            Return ItemDisplayName
        End Get
        Set(value As String)

        End Set
    End Property
    Public Sub New(worldLocation As Vector3, ItemName As String, asset As String, isGold As Boolean, CM As ContentManager, map As Map, inputItem As item,Optional GoldValue As Integer = 100)
        MyBase.New(IDGenerator.Generate(New Random) + ".item", New Size(64, 128), map)
        ItemDisplayName = ItemName
        If isGold Then
            Dim goldAnimDefs(0) As AnimationDefinition
            goldAnimDefs(0) = New AnimationDefinition(0, 6, "Birth", 40)
            defaultAnim = New Animation(CM.Load(Of Texture2D)(AssetManager.RequestAsset("CoinsBig")), New Size(64, 128), goldAnimDefs, 0, New Rectangle(0, 0, 0, 0))
            'always drop big coins for now, make them vary based on amounts relative to player level later
            ItemDisplayName = GoldValue.ToString() + " Gold"
            defaultAnim.PlayAnimationOnce("Birth", False, AnimationRevertState.PAUSE_ANIMATION)

        Else
            Dim loadedAnimationAsset As String = AssetManager.RequestAsset(ItemAssetLookup.GetAsset(inputItem.itemType))
            Dim def(0) As AnimationDefinition
            def(0) = New AnimationDefinition(0, 6, "Birth", 40)
            defaultAnim = New Animation(CM.Load(Of Texture2D)(loadedAnimationAsset), New Size(64, 128), def, 0, New Rectangle(0, 0, 0, 0))

        End If
        goldAmount = GoldValue
        inItem = inputItem
        If inItem.itemType = ItemTypes.greatsword Then
            'defaultAnim.
        End If

    End Sub
    Public Shadows Sub Update(gt As GameTime, ms As MouseState, Cam As IsoCamera)

        If isGold Then
            defaultAnim.Update(gt)
        End If
        Dim screenPos As Vector2 = Cam.MapToScreen(Pos)
        screenPos.X += CollisionInset.X
        screenPos.Y += CollisionInset.Y
        hilighted = New Rectangle(CInt(screenPos.X), CInt(screenPos.Y), CollisionInset.Width, CollisionInset.Height).Contains(New Point(ms.X, ms.Y))
        If hilighted And ms.LeftButton = ButtonState.Pressed And Not isPickedUp Then
            isPickedUp = True
            RaiseEvent ItemPickedUp(Me, New EventArgs())
        End If
    End Sub
    Public Shadows Sub Draw(sb As SpriteBatch, Cam As IsoCamera)

        If hilighted Then
            Color = Color.Yellow

        Else
            Color = Color.White
        End If
        If isGold Then
            defaultAnim.Color = Color
        End If
        Dim screenPos As Vector2 = Cam.MapToScreen(Pos)

        defaultAnim.Draw(sb, New Rectangle(CInt(screenPos.X), CInt(screenPos.Y), Size.Width, Size.Height))

    End Sub
End Class
Public Class ItemAssetLookup

    Public Sub New()

    End Sub
    Private Shared hasLoadedDictionary As Boolean = False
    Private Shared lookupDictionary As Dictionary(Of ItemTypes, String)
    Public Shared Function GetAsset(itemEnumEntry As ItemTypes) As String
        Return [Enum].GetName(GetType(ItemTypes), itemEnumEntry)
    End Function


End Class
