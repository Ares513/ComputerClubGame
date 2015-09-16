Public Class PlayerClassFactory
    'Generates a brand new player class useful for making new character classes.
    Public Sub New()

    End Sub
    Public Shared Function Generate(ClassToGen As PlayerClassType) As PlayerClass
        Dim output As PlayerClass
        Select Case ClassToGen
            Case PlayerClassType.Knight
                Dim skillTrees(2) As PlayerSkillTree
                skillTrees(0) = New PlayerSkillTree("Centurion", SkillTreeTabTypes.OFFENSE, New Skill("Shield Bash", IDGenerator.Generate(New Random()), 0, AssetManager.RequestAsset("firebolt")))
                skillTrees(1) = New PlayerSkillTree("Forgot The Name", SkillTreeTabTypes.DEFENSE, New Skill("HelloWorld!", IDGenerator.Generate(New Random()), 0, AssetManager.RequestAsset("firebolt")))
                skillTrees(2) = New PlayerSkillTree("HelloWorld", SkillTreeTabTypes.SUPPORT, New Skill("Herp Derp", IDGenerator.Generate(New Random()), 0, AssetManager.RequestAsset("firebolt")))

                output = New PlayerClass(PlayerClassType.Knight, skillTrees)

                'PRECONDITION REQUIRED: 8 SKILLS MUST BE ADDED BEFORE DATA CAN BE LOADED TO THE UI.
                output.AddSkill(SkillTreeTabTypes.OFFENSE, New Skill("Bonebreaker", IDGenerator.Generate(New Random), 0, AssetManager.RequestAsset("ThunderSlamIcon")), False, True, True)
                output.AddSkill(SkillTreeTabTypes.OFFENSE, New Skill("Bullrush", IDGenerator.Generate(New Random), 5, AssetManager.RequestAsset("firepits")), False, True, False)
                output.AddSkill(SkillTreeTabTypes.OFFENSE, New Skill("Hueuhuehue long strings best", IDGenerator.Generate(New Random), 5, AssetManager.RequestAsset("firebolt")), False, True, False)
                output.AddSkill(SkillTreeTabTypes.OFFENSE, New Skill("Hueuhuehue long strings best", IDGenerator.Generate(New Random), 5, AssetManager.RequestAsset("firebolt")), False, True, False)
                output.AddSkill(SkillTreeTabTypes.OFFENSE, New Skill("Hueuhuehue long strings best", IDGenerator.Generate(New Random), 5, AssetManager.RequestAsset("firebolt")), False, True, False)
                output.AddSkill(SkillTreeTabTypes.OFFENSE, New Skill("Thunder Slam", IDGenerator.Generate(New Random), 100, AssetManager.RequestAsset("ThunderSlamIcon")), True, True, False)
                output.AddSkill(SkillTreeTabTypes.OFFENSE, New Skill("Hueuhuehue long strings best", IDGenerator.Generate(New Random), 200, AssetManager.RequestAsset("firebolt")), True, True, False)
                output.AddSkill(SkillTreeTabTypes.OFFENSE, New Skill("Hueuhuehue long strings best", IDGenerator.Generate(New Random), 300, AssetManager.RequestAsset("firebolt")), True, True, False)
                output.AddSkill(SkillTreeTabTypes.OFFENSE, New Skill("Hueuhuehue long strings best", IDGenerator.Generate(New Random), 400, AssetManager.RequestAsset("firepits")), True, True, False)
                'DEFENSE TREE
                output.AddSkill(SkillTreeTabTypes.DEFENSE, New Skill("Defensive huehue", IDGenerator.Generate(New Random), 0, AssetManager.RequestAsset("fireburst")), False, True, True)
                output.AddSkill(SkillTreeTabTypes.DEFENSE, New Skill("Defensive huehue", IDGenerator.Generate(New Random), 0, AssetManager.RequestAsset("firewall")), False, True, False)
                output.AddSkill(SkillTreeTabTypes.DEFENSE, New Skill("Defensive huehue", IDGenerator.Generate(New Random), 0, AssetManager.RequestAsset("firebolt")), False, True, False)
                output.AddSkill(SkillTreeTabTypes.DEFENSE, New Skill("Defensive huehue", IDGenerator.Generate(New Random), 0, AssetManager.RequestAsset("firebolt")), False, True, False)
                output.AddSkill(SkillTreeTabTypes.DEFENSE, New Skill("Defensive huehue", IDGenerator.Generate(New Random), 0, AssetManager.RequestAsset("firebolt")), False, True, False)
                output.AddSkill(SkillTreeTabTypes.DEFENSE, New Skill("Defensive huehue", IDGenerator.Generate(New Random), 0, AssetManager.RequestAsset("firebolt")), True, True, False)
                output.AddSkill(SkillTreeTabTypes.DEFENSE, New Skill("Defensive huehue", IDGenerator.Generate(New Random), 0, AssetManager.RequestAsset("firebolt")), True, True, False)
                output.AddSkill(SkillTreeTabTypes.DEFENSE, New Skill("Defensive huehue", IDGenerator.Generate(New Random), 0, AssetManager.RequestAsset("firebolt")), True, True, False)
                output.AddSkill(SkillTreeTabTypes.DEFENSE, New Skill("Defensive huehue", IDGenerator.Generate(New Random), 0, AssetManager.RequestAsset("firebolt")), True, True, False)
                'SUPPORT

                output.AddSkill(SkillTreeTabTypes.SUPPORT, New Skill("May I borrow your earpiece? Zis is scout! Rainbows make me cry!", IDGenerator.Generate(New Random), 0, AssetManager.RequestAsset("firebolt")), False, True, True)

                output.AddSkill(SkillTreeTabTypes.SUPPORT, New Skill("May I borrow your earpiece? Zis is scout! Rainbows make me cry!", IDGenerator.Generate(New Random), 0, AssetManager.RequestAsset("firebolt")), False, True, False)
                output.AddSkill(SkillTreeTabTypes.SUPPORT, New Skill("May I borrow your earpiece? Zis is scout! Rainbows make me cry!", IDGenerator.Generate(New Random), 0, AssetManager.RequestAsset("firebolt")), False, True, False)
                output.AddSkill(SkillTreeTabTypes.SUPPORT, New Skill("May I borrow your earpiece? Zis is scout! Rainbows make me cry!", IDGenerator.Generate(New Random), 0, AssetManager.RequestAsset("firebolt")), False, True, False)
                output.AddSkill(SkillTreeTabTypes.SUPPORT, New Skill("May I borrow your earpiece? Zis is scout! Rainbows make me cry!", IDGenerator.Generate(New Random), 0, AssetManager.RequestAsset("firebolt")), False, True, False)

                output.AddSkill(SkillTreeTabTypes.SUPPORT, New Skill("May I borrow your earpiece? Zis is scout! Rainbows make me cry!", IDGenerator.Generate(New Random), 0, AssetManager.RequestAsset("firebolt")), True, True, False)
                output.AddSkill(SkillTreeTabTypes.SUPPORT, New Skill("May I borrow your earpiece? Zis is scout! Rainbows make me cry!", IDGenerator.Generate(New Random), 0, AssetManager.RequestAsset("firebolt")), True, True, False)
                output.AddSkill(SkillTreeTabTypes.SUPPORT, New Skill("May I borrow your earpiece? Zis is scout! Rainbows make me cry!", IDGenerator.Generate(New Random), 0, AssetManager.RequestAsset("firebolt")), True, True, False)
                output.AddSkill(SkillTreeTabTypes.SUPPORT, New Skill("May I borrow your earpiece? Zis is scout! Rainbows make me cry!", IDGenerator.Generate(New Random), 0, AssetManager.RequestAsset("firebolt")), True, True, False)





                Return output
        End Select
        DebugManagement.WriteLineToLog("Someone requested a class that simply hasn't been coded yet. Gotta stop.", SeverityLevel.FATAL)
        Debug.Assert(False)
        Return Nothing
    End Function
End Class
Public Enum PlayerClassType
    Knight
    Rogue
    Necromancer
    Mage
    Tactician
    Ranger
    Monk
    Brute

End Enum