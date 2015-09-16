Public Class Damage
    Implements IEffect

    Private minDmg, maxDmg As Single
    Public LastCalculatedDamage As Single = 0.0
    Public CritOnLastCalc As Boolean
    Private chanceToCrit As Double
    Public Const CritModifier As Single = 3.0
    Public ReadOnly Property CritChance As Double
        Get

            Return chanceToCrit
        End Get
    End Property
    Public ReadOnly Property MinimumDamage As Single
        Get
            Return minDmg
        End Get
    End Property
    Public ReadOnly Property MaximumDamage As Single
        Get
            Return maxDmg
        End Get
    End Property
    Public ReadOnly Property LastCalculatedDamagePercentage As Single
        Get
            Return (LastCalculatedDamage - minDmg) / (maxDmg - minDmg)
        End Get
    End Property
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="minDamage">Minimum damage inclusively.</param>
    ''' <param name="maxDamage">The maximum possible damage inclusively.</param>
    ''' <param name="critChance">The percentage, from 1 to 100 to crit the opponent before crit resitance. Crits do not bypass armor but they always do maximum damage.</param>
    ''' <remarks></remarks>
    Public Sub New(minDamage As Single, maxDamage As Single, Optional critChance As Single = 1.0)
        minDmg = minDamage
        maxDmg = maxDamage
        chanceToCrit = critChance
    End Sub
    Public Function CalculateDamage(target As EntityLiving) As Single
        'Resistance processing here
        LastCalculatedDamage = New System.Random().Next(CInt(Math.Round(minDmg)), CInt(Math.Round(maxDmg)))
        If New System.Random().NextDouble * 100 <= chanceToCrit Then
            CritOnLastCalc = True
            LastCalculatedDamage *= 3 'Crit multiplier.
        Else
            CritOnLastCalc = False
        End If


        Return LastCalculatedDamage

    End Function
    Public Shared Function DefaultDamage() As Damage
        Return New Damage(5, 10)
    End Function

    Public Property isUpdating As Boolean Implements IEffect.isUpdating
        Get
            Return False
        End Get
        Set(value As Boolean)

        End Set
    End Property

    Public Sub Update(EM As EntityManagement, gt As Microsoft.Xna.Framework.GameTime, ms As Microsoft.Xna.Framework.Input.MouseState, ks As Microsoft.Xna.Framework.Input.KeyboardState, CM As Microsoft.Xna.Framework.Content.ContentManager, UI As UIOverlay, cam As IsoCamera) Implements IEffect.Update

    End Sub
End Class
Public Enum DamageTypes As Integer
    GENERIC
    ''' <summary>
    ''' Indicates that all resistances will be totally ignored when processing damage.
    ''' </summary>
    ''' <remarks></remarks>
    TRUE_DAMAGE = 1
    'other damage types here
End Enum