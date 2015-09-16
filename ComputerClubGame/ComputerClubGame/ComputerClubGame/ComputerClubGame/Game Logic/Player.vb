Public Class Player
    Private Life As Single
    Private LifeRegen As Double
    Private TurningSpeed As Double
    Private Acceleration As Double
    Private MovementSpeed As Double
    Private Mana As Single
    Private ManaRegen As Double
    Private AttackPeriod As Single
    Private CastingRate As Double
    Private CastingPeriod As Single
    Private PhysicalDefence As Double
    Private SpellDefence As Double
    Private PlayerLevel As Integer
    Private PlayerSkillPoints As Integer
    Private PlayerGold As Integer
#Region "Experience"
    Private CurrentExperience As Double
    Private ExperienceToNextLevel As Double
    Private TotalExperience As Double
#End Region
    Dim playerRace As PlayerRace
    Public playerClass As PlayerClass
    Public ReadOnly Property SkillPoints As Integer
        Get
            Return PlayerSkillPoints
        End Get
    End Property
    Public ReadOnly Property getPlayerHealth As Single
        Get
            Return Life
        End Get

    End Property
    Public ReadOnly Property Level As Integer
        Get
            Return PlayerLevel
        End Get
    End Property
    Private CharacterName As String
    Public ReadOnly Property Name As String
        Get
            Return CharacterName
        End Get
    End Property

    Public Sub New(inRace As PlayerRace, inClass As PlayerClass, inAttributes() As PlayerAttribute, inGuild As PlayerGuild, inSKill() As Skill, inCharacterName As String)
        playerRace = inRace
        playerClass = inClass
        CharacterName = inCharacterName
        LevelUp() 'So the player isn't level zero.
        PlayerSkillPoints = 500
        PlayerLevel = 500
    End Sub
    ''' <summary>
    ''' Levels up the player immediately, allocating the appropriate skill and attribute points and resetting experience.
    ''' This does not give a whole level of experience. It merely sets the current experience to zero and advances the player a level.   
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub LevelUp()
        PlayerLevel += 1
        CurrentExperience = 0
        'Level algorithm here.
        PlayerSkillPoints += 1


    End Sub
    'Takes a single skill point.
    Public Sub TakeSkillPoint()
        If SkillPoints > 0 Then
            PlayerSkillPoints -= 1
        End If
    End Sub
    Public ReadOnly Property CanTakeSkillPoint As Boolean
        Get
            If SkillPoints > 0 Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property
    'End Sub
    Public ReadOnly Property getClassName As String
        Get
            Return "Knight"
        End Get

    End Property

    Public Property Gold As Integer
        Get
            Return PlayerGold

        End Get
        Set(value As Integer)
            PlayerGold = value
        End Set
    End Property

End Class
