Public Class Asset 'Simple class that contains manifest information
    'regarding where data is stored.
    Public Sub New(name As String, type As String, path As String, index As Integer)
        AssetName = name
        AssetType = type
        AssetPath = path
        AssetIndex = index
    End Sub
    Public AssetName As String
    Public AssetType As String
    Public AssetPath As String
    Public AssetIndex As Integer
End Class