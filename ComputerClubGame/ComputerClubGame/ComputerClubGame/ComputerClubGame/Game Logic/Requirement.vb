Public Class Requirement
    'This class uses node-based requirements.

    Private RequirementName As String
    Public LevelRequirement As Integer
    Private LevelEQ As EqualityType
    Public ReadOnly Property LevelEqualityType As EqualityType
        Get
            Return LevelEQ
        End Get
    End Property
    Public Sub New(reqName As String, LevelEqualityType As EqualityType)
        LevelEQ = LevelEqualityType
        RequirementName = reqName

    End Sub
    Public Function MeetsRequirements(Level As Integer) As Boolean
        Select Case LevelEQ
            Case EqualityType.EQUAL_TO
                If Level = LevelRequirement Then
                    Return True
                Else
                    Return False
                End If
            Case EqualityType.GREATER_THAN
                If Level > LevelRequirement Then
                    Return True
                Else
                    Return False
                End If
            Case EqualityType.GREATER_THAN_OR_EQUAL_TO
                If Level >= LevelRequirement Then
                    Return True
                Else
                    Return False
                End If
            Case EqualityType.LESS_THAN
                If Level < LevelRequirement Then
                    Return True
                Else
                    Return False
                End If
            Case EqualityType.LESS_THAN_OR_EQUAL_TO
                If Level <= LevelRequirement Then
                    Return True
                Else
                    Return False
                End If
            Case Else
                If Level = LevelRequirement Then
                    Return True
                Else
                    Return False
                End If
        End Select
        Return True
    End Function
End Class
'Public Class Node
'    Private obj As Object
'    Private objType As System.Type
'    ''' <summary>
'    ''' Creates a new Requirement Node.
'    ''' </summary>
'    ''' <param name="Equality">How the type being compared is evaluated.</param>
'    ''' <param name="Comparison">The type of object being compared.</param>
'    ''' <param name="Comparable">The object being compared.</param>
'    ''' <remarks></remarks>
'    Public Sub New(Equality As EqualityType, Comparison As CompareType, Comparable As Object)
'        objType = Comparable.GetType()
'        obj = Comparable
'    End Sub
'    Private Sub CanBeCompared(Type As Object)
'        If Type <> GetType(String) Or Type <> GetType(Integer) Or Type <> GetType(Double) Or Type <> GetType(Single) Or Type <> GetType(Byte) Then
'            'This object cannot be compared!
'            DebugManagement.WriteLineToLog("Someone created a Node object with a Type that cannot be compared! Reverting to Integer.", SeverityLevel.SEVERE)
'            Type = GetType(Integer)
'        Else

'        End If
'    End Sub
'    Public Sub Compare(Comparison As Object)

'    End Sub
'End Class
Public Enum EqualityType As Integer
    NO_COMPARISON
    LESS_THAN
    LESS_THAN_OR_EQUAL_TO
    EQUAL_TO
    GREATER_THAN
    GREATER_THAN_OR_EQUAL_TO
End Enum
Public Enum CompareType As Integer
    NO_COMPARISON
    INTEGER_COMPARISON
    STRING_COMPARISON

End Enum