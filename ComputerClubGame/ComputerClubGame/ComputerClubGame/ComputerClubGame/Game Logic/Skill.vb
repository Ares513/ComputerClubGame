Public Class Skill

    Private CurrentLevel As Integer

    Private LevelRequirementBase As Integer
    Private SkillName As String
    Private skillID As String
    Private SkillDescription As String
    Public IgnoreRequirements As Boolean
    'Information and calculation about what the skill actually *does* occurs in IActions.
    Dim ImageAsset As String
    Public Sub New(Name As String, ID As String, LvlReq As Integer, imageAsset As String)
        LevelRequirementBase = LvlReq
        Description = Name

        SkillName = Name
        skillID = ID
        Me.ImageAsset = imageAsset
    End Sub
    Public ReadOnly Property Image As String
        Get
            Return ImageAsset
        End Get
    End Property
    Public Property Description As String
        Get
            Return SkillDescription
        End Get
        Set(value As String)
            SkillDescription = value
        End Set
    End Property

    Public ReadOnly Property Level As Integer
        Get
            Return CurrentLevel
        End Get
    End Property
    Public ReadOnly Property Name As String
        Get
            Return SkillName
        End Get
    End Property
    Public ReadOnly Property ID As String
        Get
            Return skillID
        End Get
    End Property
    Public ReadOnly Property MeetsRequirements(PreviousPointsInTree As Integer) As Boolean
        Get
            If IgnoreRequirements Then
                Return True
            End If
            If PreviousPointsInTree >= LevelRequirement And CurrentLevel < 10 Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property
    Public ReadOnly Property LevelRequirement As Integer
        Get
            Return CurrentLevel + LevelRequirementBase
        End Get
    End Property
    Public WriteOnly Property SetRequirement As Integer
        Set(value As Integer)
            LevelRequirementBase = value
        End Set
    End Property
    ''' <summary>
    ''' Increases the skill's level by 1. Max level 10; skill is not ranked if you do not meet the level requirement.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub RankSkill(Level As Integer)
        If CurrentLevel < 10 And MeetsRequirements(Level) Then
            CurrentLevel += 1
        End If
    End Sub

End Class
