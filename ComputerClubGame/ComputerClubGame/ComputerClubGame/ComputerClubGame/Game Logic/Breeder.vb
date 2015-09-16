''' <summary>
''' Class for easily instantiating new Mob type classes by loading data from a file.
''' </summary>
''' <remarks></remarks>
Public Class Breeder
    Private MobData() As MobDataDescription
    Public Sub New(CM As ContentManager, fileAssetFromAssetMgr As String)
        'Start by loading the XML file.
        LoadMobData(CM, fileAssetFromAssetMgr)
    End Sub
    Private Sub LoadMobData(CM As ContentManager, fileAssetFromAssetMgr As String)
        MobData = CM.Load(Of MobDataDescription())(AssetManager.RequestAsset(fileAssetFromAssetMgr))

    End Sub
    ''' <summary>
    ''' Returns the data (raw default values, NOT a mob) associated with a single XML entry.
    ''' </summary>
    ''' <param name="name">The name of the mob to be requested. Case insensitive.</param>
    ''' <returns>A MobDataDescription. Returns zero index example mob if mob not found.</returns>
    ''' <remarks>Use this method for getting information on default values on mobs; do not use this to create a new mob. </remarks>
    Public Function GetMobDataByName(name As String) As MobDataDescription
        Dim i As Integer
        For i = 0 To MobData.Length - 1 Step 1
            If MobData(i).MobName.ToLower = name.ToLower Then
                Return MobData(i)
            End If
        Next
        Return MobData(0)
    End Function
    Private Function ParseSize(sizeString As String) As Size
        If sizeString.Contains(",") = False Then
            'Oops. Doesn't have the comma. default to 64x64
            Return New Size(64, 64)
        Else
            Dim splitStr() As String = sizeString.Split(CChar(","))
            If IsNumeric(splitStr(0)) And IsNumeric(splitStr(1)) Then
                Return New Size(CInt(splitStr(0)), CInt(splitStr(1)))
            Else
                Return New Size(64, 64)
            End If

        End If

    End Function
    Private Function ParseAnimationDefinitions(AnimationDefValue As String) As AnimationDefinition()
        Dim result(0) As AnimationDefinition
        'Format: StartFrame, EndFrame, Animation Name, DelayInMS; ... next entry
        'The goal here is maximum safety in parsing this string value.
        Dim startFrame, endFrame As Integer
        Dim animName As String
        Dim delayInMS As Integer
        Dim firstSplit() As String
        firstSplit = AnimationDefValue.Split(CChar(";"))
        Dim i As Integer
        For i = 0 To firstSplit.Length - 1 Step 1
            Dim secondSplit() As String = firstSplit(i).Split(CChar(","))
            If secondSplit.Length <> 4 Then
                'Oops. Looks like the fields are longer than they should be. Only 4 fields.
                'Raise a DebugMgmt error.
                DebugManagement.WriteLineToLog("Breeder has encountered an error! XML configuration for a Mob's AnimationDefinition is incorrectly defined!", SeverityLevel.CRITICAL)
                ReDim Preserve secondSplit(3)
                Dim k As Integer
                For k = 0 To secondSplit.Length - 1 Step 1
                    If IsNothing(secondSplit(i)) Then
                        secondSplit(i) = "!" 'Use ! to indicate an error value."
                    End If
                Next
            End If
            Dim j As Integer
            For j = 0 To secondSplit.Length - 1 Step 1
                If secondSplit(i) = "!" Then
                    Select Case i
                        Case 0
                            secondSplit(i) = "0"
                        Case 1
                            secondSplit(i) = "15"
                        Case 2
                            secondSplit(i) = "Stand"
                        Case 3
                            secondSplit(i) = "16"
                    End Select

                End If

            Next
            If IsNumeric(secondSplit(0)) Then
                startFrame = CInt(secondSplit(0))
            Else
                startFrame = 0
            End If
            If IsNumeric(secondSplit(1)) Then
                endFrame = CInt(secondSplit(1))
            Else
                endFrame = 15
            End If
            animName = secondSplit(2)
            If IsNumeric(secondSplit(3)) Then
                delayInMS = CInt(secondSplit(3))
            Else
                delayInMS = 16
            End If
            If IsNothing(result) Then
                ReDim result(0)
            Else
                ReDim Preserve result(result.Length)
            End If
            result(result.Length - 1) = New AnimationDefinition(CShort(startFrame), CShort(endFrame), animName, delayInMS)

        Next
        Return result
    End Function
    Public Function GenerateMobFromName(ID As String, name As String, CM As ContentManager, tileSpawnPosition As Vector3, cam As IsoCamera, em As EntityManagement) As Mob
        'First find the name
        Dim data As MobDataDescription
        data = GetMobDataByName(name)

        Dim output As Mob
        Dim anim As IsoAnimation
        Dim mobSize As Size = ParseSize(data.MobSize)
        Dim screenPos As Vector2 = cam.MapToScreen(tileSpawnPosition)
        anim = New IsoAnimation(CM.Load(Of Texture2D)(AssetManager.RequestAsset(data.MobSpriteSheetAssets)), mobSize, ParseAnimationDefinitions(data.AnimDefs), 0, New Rectangle(CInt(screenPos.X), CInt(screenPos.Y), mobSize.Width, mobSize.Height), New Vector2(data.MobSpriteOriginX, data.MobSpriteOriginY))
        Dim deathAnim As IsoAnimation
        deathAnim = New IsoAnimation(CM.Load(Of Texture2D)(AssetManager.RequestAsset("goblinSpearmanDeath")), mobSize, ParseAnimationDefinitions(data.AnimDefs), 0, New Rectangle(CInt(screenPos.X), CInt(screenPos.Y), mobSize.Width, mobSize.Height), New Vector2(data.MobSpriteOriginX, data.MobSpriteOriginY))
        ' output = New Mob(ID, mobSize, 100, 100, 5, anim, tileSpawnPosition, CM)
        Dim animations As EntityLivingAnimationSet = New EntityLivingAnimationSet()
        animations.Anims(EntityLivingAnimationSet.Animations.Stand) = anim
        animations.Anims(EntityLivingAnimationSet.Animations.Die) = deathAnim
        output = New Mob(CM, animations, True, em)
        Return output
    End Function
End Class
