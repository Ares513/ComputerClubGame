Public Class PlayerClass
    Dim ClassType As PlayerClassType
    Private SkillTrees() As PlayerSkillTree

    Public Sub New(PlayerClassType As PlayerClassType, inSkillTrees() As PlayerSkillTree)
        ClassType = PlayerClassType

        SkillTrees = inSkillTrees
    End Sub
    Public Function GetSkill(skillName As String) As Skill
        For i = 0 To SkillTrees.Length - 1 Step 1
            If SkillTrees(i).GetSkill(skillName).Name = skillName Then
                Return SkillTrees(i).GetSkill(skillName)
            End If
        Next
        DebugManagement.WriteLineToLog("Someone tried to lookup a skill in class type " + PlayerClassType.Knight.ToString() + " that doesn't exist!", SeverityLevel.SEVERE)
        Return SkillTrees(0).CoreSkill
        'Backup.
    End Function
    Public Property SkillTree(index As SkillTreeTabTypes) As PlayerSkillTree
        Get
            Try
                Return SkillTrees(index)
            Catch ex As Exception
                DebugManagement.WriteLineToLog("Someone requested a Skill Tree tab index that doesn't exist!", SeverityLevel.CRITICAL)
            End Try
            Return SkillTrees(0)
        End Get
        Set(value As PlayerSkillTree)
            Try
                SkillTrees(index) = value
            Catch ex As Exception
                DebugManagement.WriteLineToLog("Someone tried to set a Skill Tree tab index that doesn't exist!", SeverityLevel.CRITICAL)
            End Try

        End Set
    End Property
    ''' <summary>
    ''' Adds a skill to the skill trees.
    ''' </summary>
    ''' <param name="skillToAdd">The skill.</param>
    ''' <param name="leftOrRight">True for the left side of the tree, false for the left. Ignored if CoreSkill is true.</param>
    ''' <param name="CoreSkill">True if you're setting the first skill</param>
    ''' <remarks></remarks>
    Public Sub AddSkill(SkillTree As SkillTreeTabTypes, skillToAdd As Skill, leftOrRight As Boolean, OverrideRequirements As Boolean, Optional CoreSkill As Boolean = False)


        SkillTrees(SkillTree).AddSkill(skillToAdd, leftOrRight, OverrideRequirements, CoreSkill)


    End Sub
    ''' <summary>
    ''' Returns the number of points the player has put into the tree before that skill.
    ''' </summary>
    ''' <param name="depth"></param>
    ''' <param name="leftOrRight">False for the left side of the tree, true for the right</param>
    ''' <param name="tree">Which tree is being edited.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetSumOfLevelsBeforeIndex(depth As Integer, tree As SkillTreeTabTypes, leftOrRight As Boolean, Optional CoreSkill As Boolean = False) As Integer
        Dim sum As Integer = SkillTrees(tree).CoreSkill.Level
        If CoreSkill Or depth = 0 Then
            'if depth is zero, 
            Return sum
        End If
        For i = depth - 1 To 0 Step -1
            If leftOrRight = False Then
                sum += SkillTree(tree).LeftSideSkills(i).Level
            Else
                sum += SkillTree(tree).RightSideSkills(i).Level
            End If
        Next
        Return sum
    End Function
    Public Function GetTotalLevelsInSideOftree(leftOrRight As Boolean, tree As SkillTreeTabTypes, Optional includeCoreSkill As Boolean = True) As Integer
        'False for left, true for right
        Dim sum As Integer = 0
        If includeCoreSkill Then
            sum += SkillTrees(tree).CoreSkill.Level
        End If
        If leftOrRight = False Then
            For i = 0 To SkillTrees(tree).LeftSideSkills.Count - 1 Step 1
                sum += SkillTrees(tree).LeftSideSkills(i).Level
            Next
        Else
            For i = 0 To SkillTrees(tree).RightSideSkills.Count - 1 Step 1
                sum += SkillTrees(tree).RightSideSkills(i).Level
            Next
        End If
        Return sum
    End Function
        ''' <summary>
        ''' Whether or not the skills before this in the tree provided have at least one point in them.
        ''' </summary>
        ''' <param name="LeftOrRight">False for the left side, true for the right.</param>
        ''' <param name="tab">The SkillTreeTab that is being accessed. See SkillTreeTabTypes enum.</param>
        ''' <param name="depth">How deep in the skill tree to go- with 3 being the 'deepest' skill.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
    Public Function HasPreviousSkills(LeftOrRight As Boolean, tab As SkillTreeTabTypes, depth As Integer) As Boolean
        If depth = 0 Then
            Return True
        End If
        If depth > 4 Then
            DebugManagement.WriteLineToLog("Someone requested a depth in HasPreviousSkills that is greater than the number of skills!", SeverityLevel.WARNING)
            depth = 4
        End If
        Dim result As Boolean = True
        'True unless proven otherwise.
        If LeftOrRight = False Then
            For i = depth - 1 To 0 Step -1
                If SkillTrees(tab).LeftSideSkills(i).Level = 0 Then
                    result = False

                End If
            Next
        Else
            For i = depth - 1 To 0 Step -1
                If SkillTrees(tab).RightSideSkills(i).Level = 0 Then
                    result = False

                End If
            Next
        End If


        Return result
    End Function
    Public Function GetCoreSkillLevel(tab As SkillTreeTabTypes) As Integer
        Return SkillTrees(tab).CoreSkill.Level
    End Function
End Class
