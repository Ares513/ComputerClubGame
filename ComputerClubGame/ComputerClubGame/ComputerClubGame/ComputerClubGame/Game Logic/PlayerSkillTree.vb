Public Class PlayerSkillTree
    Public CoreSkill As Skill
    Public LeftSideSkills As List(Of Skill)
    Public RightSideSkills As List(Of Skill)
    Private PrestigeName As String
    Private Type As SkillTreeTabTypes
    Public ReadOnly Property tabType As SkillTreeTabTypes
        Get
            Return Type
        End Get
    End Property
    'The idea is that you add items to the list in a specific order and set their level requirements
    'externally.
    Public Sub New(SkillTreePrestigeName As String, TabType As SkillTreeTabTypes, coreTreeSkill As Skill)
        PrestigeName = SkillTreePrestigeName
        Type = TabType
        CoreSkill = coreTreeSkill
        CoreSkill.IgnoreRequirements = True
        LeftSideSkills = New List(Of Skill)
        RightSideSkills = New List(Of Skill)
    End Sub
    Public Function GetSkill(SkillName As String) As Skill
        If CoreSkill.Name.ToLower = SkillName.ToLower Then
            Return CoreSkill
        End If
        For Each Skill In LeftSideSkills
            If Skill.Name.ToLower = SkillName.ToLower Then
                Return Skill
            End If
        Next
        For Each Skill In RightSideSkills
            If Skill.Name.ToLower = SkillName.ToLower Then
                Return Skill
            End If
        Next
        Return CoreSkill
    End Function
    ''' <summary>
    ''' Adds a new skill.
    ''' </summary>
    ''' <param name="inSkill">The skill to add</param>
    ''' <param name="LeftOrRight">Whether the left side of the tree or the right</param>
    ''' <param name="useCoreSkill"></param>
    ''' <param name="useStandardRequirements">Whether the skill requirements should be standardized (i.e, 0, 5, 10, 25, etc) </param>
    ''' <remarks></remarks>
    Public Sub AddSkill(inSkill As Skill, LeftOrRight As Boolean, useStandardRequirements As Boolean, useCoreSkill As Boolean)
        If useCoreSkill Then
            CoreSkill = inSkill
            Exit Sub
        End If
        If LeftOrRight = False Then
            'True for left, false for right

            LeftSideSkills.Add(inSkill)
            If useStandardRequirements Then
                Select Case LeftSideSkills.Count
                    Case 1
                        LeftSideSkills(LeftSideSkills.Count - 1).SetRequirement = 5
                    Case 2
                        LeftSideSkills(LeftSideSkills.Count - 1).SetRequirement = 7
                    Case 3
                        LeftSideSkills(LeftSideSkills.Count - 1).SetRequirement = 15
                    Case 4
                        LeftSideSkills(LeftSideSkills.Count - 1).SetRequirement = 20
                End Select



            End If


        Else



            RightSideSkills.Add(inSkill)

            Select Case RightSideSkills.Count
                Case 1
                    RightSideSkills(RightSideSkills.Count - 1).SetRequirement = 5
                Case 2
                    RightSideSkills(RightSideSkills.Count - 1).SetRequirement = 10
                Case 3
                    RightSideSkills(RightSideSkills.Count - 1).SetRequirement = 15
                Case 4
                    RightSideSkills(RightSideSkills.Count - 1).SetRequirement = 25
            End Select





        End If
    End Sub
End Class