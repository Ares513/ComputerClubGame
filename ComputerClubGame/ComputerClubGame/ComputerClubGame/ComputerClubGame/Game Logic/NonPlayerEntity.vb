Public MustInherit Class NonPlayerEntity
    Inherits EntityLiving

    Public Sub New(ID As String, entitySize As Size, em As EntityManagement)
        MyBase.New(ID, entitySize, 100, em)

    End Sub

End Class
