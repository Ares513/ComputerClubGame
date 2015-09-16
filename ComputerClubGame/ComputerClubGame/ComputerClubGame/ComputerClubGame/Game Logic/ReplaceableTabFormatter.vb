Public Class ReplaceableTabFormatter
    Public Shared Function SkillFormat(description As String, skill As Skill) As String
        Dim output As String
        output = description
        output.Replace("$LVL", skill.Level.ToString())
        Return output
    End Function
End Class
