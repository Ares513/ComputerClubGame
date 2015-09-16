Public Class MobDataDescription
    Public MobSpriteSheetAssets As String 'Delimited by a comma.
    Public MobName As String
    Public Unique As Boolean
    Public MobHealth As Single
    Public MobSpeed As Single
    Public MobSize As String
    Public AnimDefs As String 'Format: StartFrame, EndFrame, Animation Name, DelayInMS; ... next entry
    Public MobSpriteOriginX As Single
    Public MobSpriteOriginY As Single
End Class