Public Class FriendlyNPCDataDescription
    Public Sub New()

    End Sub
    Public npcName As String
    Public npcDefaultLocation As Vector2
    Public npcAttacksNearbyEnemies As Boolean
    Public npcSpriteSheetAssetName As String
    Public npcSpriteSheetAnimationDefinitionString As String
    Public isVulnerable As Boolean
    Public SpeechFileAssets() As String
    Public canAttack As Boolean
    Public DefaultSpeechMenuOptions() As String
    Public SpeechTextValues() As String
    Public Quests() As String
End Class