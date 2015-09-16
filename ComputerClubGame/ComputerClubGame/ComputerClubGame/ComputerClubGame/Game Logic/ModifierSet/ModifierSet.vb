''' <summary>
''' Interface that contains a series of properties that can be applied to the player for a specified period of time. They will alter the player properties, sometimes permanently.
''' 
''' </summary>
''' <remarks></remarks>
Public Interface IModifierSet
    ''' <summary>
    ''' Returns the number of seconds that remain in the modifier. -1 if infinite.
    ''' </summary>
    ''' <param name="gt">GameTime instance for checking</param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Function Update(gt As GameTime) As Integer
    Sub ApplyModifiers(ByRef Modified As EntityLiving)
    Sub UnApplyModifiers(ByRef Modified As EntityLiving)
    ReadOnly Property HasAppliedModifiers As Boolean
    ReadOnly Property CD As Cooldown
    ReadOnly Property RemainingDuration As Integer
    ReadOnly Property ModifierIdentifier As String
End Interface
