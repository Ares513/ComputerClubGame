''' <summary>
''' This class manages the local XML location stored in the Content directory. It is accessed through InitializationSettings.
''' </summary>
''' <remarks></remarks>
Public Class LocalConfiguration
    Dim XDoc As XDocument
    Dim ScrollWheelSensitivity As Single
    Dim inputXML As XNode
    ''' <summary>
    ''' The XML file to load. Sets values as not found and loads the default if the files aren't found.
    ''' </summary>
    ''' <param name="XMLfilePath">The exact file path to load.</param>
    ''' <remarks></remarks>
    Public Sub New(XMLfilePath As String)
        Dim reader As System.Xml.XmlReader

        Try
            reader = System.Xml.XmlReader.Create(XMLfilePath)
            reader.MoveToContent()
            inputXML = XDocument.ReadFrom(reader)
            reader.Close()
        Catch ex As Exception
            DebugManagement.WriteLineToLog("User configuration failed to load in LocalConfiguration.vb", SeverityLevel.CRITICAL)
        End Try

    End Sub
    ''' <summary>
    ''' Handles how the XDocument is loaded for this class
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetupXDocument()

        ScrollWheelSensitivity = CSng(inputXML.Document.<ScrollWheelSensitivity>.Value)
        If IsNothing(ScrollWheelSensitivity) Then
            'The inputXML document doesn't have an entry for ScrollWheelSensitivity. Set it to the default
            ScrollWheelSensitivity = 1.0
        End If
    End Sub
End Class
