Public Class ItemStats
    Private Life As Single              ' Driven straight from the Player class
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
    Public Sub New(Life As Single, LifeRegen As Double, TurningSpeed As Double, Acceleration As Double,
                    MovementSpeed As Double, Mana As Single, ManaRegen As Double, AttackPeriod As Single,
                    CastingRate As Double, CastingPeriod As Single, PhysicalDefence As Double, SpellDefence As Double,
                    PlayerLevel As Integer)
        Me.Life = Life
        Me.LifeRegen = LifeRegen
        Me.TurningSpeed = TurningSpeed
        Me.Acceleration = Acceleration
        Me.MovementSpeed = MovementSpeed
        Me.Mana = Mana
        Me.ManaRegen = ManaRegen
        Me.AttackPeriod = AttackPeriod
        Me.CastingRate = CastingRate
        Me.CastingPeriod = CastingPeriod
        Me.PhysicalDefence = PhysicalDefence
        Me.SpellDefence = SpellDefence
    End Sub

End Class
