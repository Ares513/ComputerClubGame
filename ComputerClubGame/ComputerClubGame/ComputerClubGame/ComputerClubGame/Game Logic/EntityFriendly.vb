Public Class EntityFriendly
    Inherits NonPlayerEntity
    Private Property CharacterPortrait As Texture2D
    Private RemainingFidgetTime As Integer
    Private FidgetMinimumTime As Integer
    Private FidgetMaximumTime As Integer
    Public Selected As Boolean
    Public ReadOnly Property Portrait As Texture2D
        Get
            Return CharacterPortrait
        End Get
    End Property
    Public Overrides ReadOnly Property Visible As Boolean
        Get
            Return True
        End Get
    End Property
    Public Sub New(ID As String, EntitySize As Size, friendlyNPCreferenceName As FriendlyEntityReference, CM As ContentManager, initialPos As Vector3, EM As EntityManagement)
        MyBase.New(ID, EntitySize, EM)
        Process(CM, friendlyNPCreferenceName, initialPos, EM)
    End Sub
    Private Sub Process(CM As ContentManager, nameRef As FriendlyEntityReference, initialPosition As Vector3, EM As EntityManagement)
        'For now, the entities are loaded by a select case statement. In the future, they will be loaded by an XML file.
        OverheadDrawFont = CM.Load(Of SpriteFont)(AssetManager.RequestAsset("overheadFont", AssetTypes.SPRITEFONT))
        OverheadBoxBackground = CM.Load(Of Texture2D)(AssetManager.RequestAsset("defaultprogressbarfull"))
        Select Case nameRef
            Case FriendlyEntityReference.NO_ENTITY
                DebugManagement.WriteLineToLog("Someone requested to generate an NPC that doesn't exist!", SeverityLevel.FATAL)
                Debug.Assert(False, "Someone generated an NPC that doesn't exist!")
            Case FriendlyEntityReference.Blademaster_Ziso
                Dim animDefs(1) As AnimationDefinition

                ' defaultAnimation = New Animation(CM.Load(Of Texture2D)(AssetManager.RequestAsset("ziso_texture")), New Size(128,128), _
                'Incomplete. Need to trim assets.
            Case FriendlyEntityReference.Crazy_Hasaad
                Dim animDef(0) As AnimationDefinition
                animDef(0) = New AnimationDefinition(0, 5, "Stand", 70)
                animation = New EntityLivingAnimationSet(
                    New IsoAnimation(CM.Load(Of Texture2D)(AssetManager.RequestAsset("hasaad_texture")), New Size(32, 64), animDef, 0, New Rectangle(0, 0, 0, 0), New Vector2(16, 56)))
                Pos = initialPosition
                FidgetMinimumTime = 400
                FidgetMaximumTime = 2500
                RemainingFidgetTime = New Random().Next(FidgetMinimumTime, FidgetMaximumTime)
                CharacterName = "Hasaad Al-Mamar"

        End Select
    End Sub
    Private Const NPCFidgetCooldownID As String = "NPC Fidget ID "
    'Private Sub Fidget(CD As CooldownManager)

    '    defaultAnimation.Unpause()

    'End Sub
    'Private Sub AnimDone() ' Handles defaultAnimation.AnimDone
    '    defaultAnimation.Pause()
    'End Sub



    Public Shadows Sub Draw(sb As SpriteBatch, Cam As IsoCamera)
        Dim screenPos As Vector2 = Cam.MapToScreen(Pos)
        Dim layer As Single = Cam.ScaleLayer(Pos.X + Pos.Y + Pos.Z)
        Dim Rect As Rectangle = New Rectangle(CInt(screenPos.X), CInt(screenPos.Y), Size.Width, Size.Height)
        animation.Draw(sb, screenPos, layer)
        'defaultAnimation.Draw(sb, Rect)
        Dim nameSize As Vector2 = OverheadDrawFont.MeasureString(CharacterName)
        Dim boxRectangle As Rectangle = New Rectangle(0, 0, CInt(nameSize.X), CInt(nameSize.Y))
        
        boxRectangle.X = CInt(Rect.Center.X - (nameSize.X / 2))
        boxRectangle.Y = CInt(Rect.Y - nameSize.Y)

        sb.Draw(OverheadBoxBackground, boxRectangle, New Color(0, 0, 0, 125))
        sb.DrawString(OverheadDrawFont, CharacterName, New Vector2(boxRectangle.X, boxRectangle.Y), Color.Black)
    End Sub
    'Public Shadows Sub Update(gt As GameTime, EM As EntityManagement)
    '    animation.Update(gt)
    '    'defaultAnimation.Update(gt)
    '    'If RemainingFidgetTime <= 0 Then
    '    '    Fidget(EM.CooldownList)
    '    '    RemainingFidgetTime += New Random().Next(FidgetMinimumTime, FidgetMaximumTime)
    '    'Else
    '    '    RemainingFidgetTime -= gt.ElapsedGameTime.Milliseconds
    '    'End If

    'End Sub
    ''' <summary>
    ''' A list of different names for friendly NPCs. A Select Case statement determines which NPC to create.
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum FriendlyEntityReference
        NO_ENTITY = 0
        Blademaster_Ziso = 1
        Crazy_Hasaad = 2
    End Enum
End Class

