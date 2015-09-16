Imports System.IO
Imports System.Security.AccessControl
Public Class AssetManager

    Private Shared Assets() As Asset
    Public Const ASSET_MANIFEST_FILE_NAME = "Assets.manifest"
    Public Const ASSET_FILE_LINE_DELIMETER = "@"
    Public Const ASSET_FILE_FIELD_DELIMETER = "^"
    Private Const ASSET_NOT_FOUND_FILE = "missing_texture"
    Private Const ASSET_NOT_FOUND_SOUND = "User Interface/Sounds/battle/magic1" 'do we have a default sound
    Private Const ASSET_NOT_FOUND_FONT = "User Interface/Fonts/defaultFont"
    'Calls represents the number of times an asset has been requested by the AssetManager.
    Private Shared Calls As Integer = 0
    Public Sub New()
        'Important class that will load from a list of text values the locations and names of files.
    End Sub
    Public Sub fileSafetyCheck(Ingame As Game, CM As ContentManager) 'Verifies that we have some files loaded.
        Dim userName As String
        userName = Environment.UserName
        Dim folderAcl As New DirectorySecurity()
        Try
            folderAcl.AddAccessRule(New FileSystemAccessRule(userName, FileSystemRights.FullControl, AccessControlType.Allow))
        Catch ex As Exception
            MsgBox("Error! Cannot set access rule. Are you using a networked drive?", MsgBoxStyle.Critical, "Permissions Error")
            Exit Sub
            'IF we get here, the user is using an account that does not allow editing of the folder access rules.
        End Try

        Dim globalGameFilesDirectory As DirectoryInfo
        globalGameFilesDirectory = New DirectoryInfo(InitializationSettings.GetGameSettings.globalGameFilesFolder)
        If globalGameFilesDirectory.Exists = False Then
            'It's possible that the file exists but we lack permissions.
            Try
                globalGameFilesDirectory.SetAccessControl(folderAcl)
            Catch ex As Exception
                globalGameFilesDirectory.Create() 'Create it first if there's an exception.
                globalGameFilesDirectory.SetAccessControl(folderAcl)
            End Try
            globalGameFilesDirectory = New DirectoryInfo(InitializationSettings.GetGameSettings.globalGameFilesFolder)
            'Reinitialize it so that .exists becomes true.
            If globalGameFilesDirectory.Exists = False Then
                MsgBox("Error in installation. The program will now exit.", MsgBoxStyle.Critical, "Unhandled exception")
            End If
            Ingame.Exit()
        End If


        'Whoops. Asset file doesn't exist.



    End Sub
    ''' <summary>
    ''' Loads assets from the assets.manifest file located in the root gamefiles directory.
    ''' </summary>
    ''' <param name="CM">The ContentManager used for this method.</param>
    ''' <remarks>Method should only be run after fileSafetyCheck is run</remarks>
    Public Sub LoadAssets(CM As ContentManager)
        Dim fr As System.IO.StreamReader
        fr = New System.IO.StreamReader(CM.RootDirectory + "\" + ASSET_MANIFEST_FILE_NAME)
        Dim sr As System.IO.StreamWriter
        Dim totalString As String
        totalString = fr.ReadToEnd()
        Dim lineStrings() As String
        lineStrings = (totalString.Split(CChar(ASSET_FILE_LINE_DELIMETER))) 'split at line delimiter
        Dim i As Integer
        'iterate through each item in the list and split it by field
        Dim fieldStrings() As String
        Dim shouldsave As Boolean = False
        Dim addtoend() As String = {}
        For i = 0 To lineStrings.Length - 1 Step 1
            If lineStrings(i).IndexOf(ASSET_FILE_FIELD_DELIMETER, lineStrings(i).IndexOf(ASSET_FILE_FIELD_DELIMETER) + 1) = -1 Then
                DebugManagement.WriteLineToLog("Asset " & lineStrings(i) & "at about line " & i & ", is missing " & ASSET_FILE_FIELD_DELIMETER, SeverityLevel.DEBUG)
                Try
                    lineStrings(i) = lineStrings(i).Substring(0, lineStrings(i).IndexOf(Chr(34) & " " & Chr(34))) & ASSET_FILE_FIELD_DELIMETER & lineStrings(i).Substring(lineStrings(i).IndexOf(Chr(34) & Chr(34)) + 1)
                Catch
                    lineStrings(i) = lineStrings(i).Substring(0, lineStrings(i).IndexOf(Chr(34) & Chr(34)) + 1) & ASSET_FILE_FIELD_DELIMETER & lineStrings(i).Substring(lineStrings(i).IndexOf(Chr(34) & Chr(34)) + 1)
                End Try
                shouldsave = True
            End If
            If Not i = lineStrings.Length - 1 Then
                If countOf(lineStrings(i), CChar(Char.ConvertFromUtf32(34))) > 6 Then
                    For x As Integer = 0 To lineStrings(i).Length - 1
                        If countOf(lineStrings(i).Substring(0, x), CChar(Char.ConvertFromUtf32(34))) = 6 Then
                            addtoend(addtoend.Length) = lineStrings(i).Substring(i, (lineStrings(i).Length - 1) - i)
                        End If
                    Next
                    DebugManagement.WriteLineToLog("Asset " & lineStrings(i) & " at about line " & i & ", is missing " & ASSET_FILE_LINE_DELIMETER, SeverityLevel.SEVERE)
                ElseIf countOf(lineStrings(i), CChar(Char.ConvertFromUtf32(34))) < 6 Then
                    DebugManagement.WriteLineToLog("Asset " & lineStrings(i) & " at about line " & i & ", is missing a" & Char.ConvertFromUtf32(34), SeverityLevel.SEVERE)
                End If
            End If
            For x As Integer = 1 To addtoend.Length - 1
                lineStrings.ToList.Add(addtoend(x))
            Next
            fieldStrings = lineStrings(i).Split(CChar(ASSET_FILE_FIELD_DELIMETER))
            'We now have an array of strings that represent each field. They should already be in the proper order
            'First term should be the asset name. Reserve variables to represent each field.
            'Second term should always be the type, then the path, then the index.
            Dim name, type, path As String
            Dim index As Integer

            name = fieldStrings(0).Replace("""", "").Trim()

            type = fieldStrings(1).Replace("""", "").Trim() 'Clear the quotes used in reading the file.
            path = fieldStrings(2).Replace("""", "").Trim() '... and clean up the string

            AddAsset(name, type, path, index)

        Next
        Array.Sort(lineStrings)
        fr.Close()
        sr = New System.IO.StreamWriter(CM.RootDirectory + "\" + ASSET_MANIFEST_FILE_NAME, False)
        If shouldsave = True Then
            For i = 0 To lineStrings.Length - 1 Step 1
                sr.WriteLine(lineStrings(i))
                'thanks max
                'np
            Next
        End If
        sr.Close()
    End Sub
    Private Function countOf(str As String, character As Char) As Integer
        Dim sum As Integer = 0
        For i As Integer = 0 To str.Length - 1
            If str(i) = character Then
                sum += 1
            End If
        Next
        Return sum
    End Function
    ''' <summary>
    ''' Requests an asset from the asset list.
    ''' 
    ''' </summary>
    ''' <param name="name">The name of the desired asset.</param>
    ''' <remarks></remarks>
    ''' 
    Private Sub AddAsset(name As String, type As String, path As String, index As Integer)
        If IsNothing(Assets) Then
            ReDim Preserve Assets(0)
            Assets(0) = New Asset(name, type, path, index)
        Else
            ReDim Preserve Assets(Assets.Length)
            Assets(Assets.Length - 1) = New Asset(name, type, path, index)
        End If

    End Sub
    Public Overloads Shared Function RequestAsset(name As String) As String
        Calls = Calls + 1
        For i = 0 To Assets.Length - 1 Step 1
            If Assets(i).AssetName.ToLower() = name.ToLower() Or Assets(i).AssetPath.ToLower = name.ToLower Then
                Return Assets(i).AssetPath
            End If
        Next

        Return ASSET_NOT_FOUND_FILE
    End Function
    ''' <summary>
    ''' Request an asset of a specified type.
    ''' </summary>
    ''' <param name="name">The asset identifier.</param>
    ''' <param name="type">The Enum value of the type used.</param>
    ''' <returns>The asset. If the asset isn't found, it returns an appropriate substitute based on the type.</returns>
    ''' <remarks>Use this for loading non-Texture objects such as sounds and fonts. This method will return the appropriate missing_file object.</remarks>
    Public Overloads Shared Function RequestAsset(name As String, type As AssetTypes) As String
        Calls = Calls + 1
        For i = 0 To Assets.Length - 1 Step 1
            If Assets(i).AssetName = name Or Assets(i).AssetPath = name Then
                Return Assets(i).AssetPath
            End If

        Next
        Select Case type
            Case AssetTypes.SOUNDEFFECT
                Return ASSET_NOT_FOUND_SOUND
            Case AssetTypes.SOUNDTRACK
                Return ASSET_NOT_FOUND_SOUND
            Case AssetTypes.SPRITEFONT
                Return ASSET_NOT_FOUND_FONT
            Case AssetTypes.TEXTURE2D
                Return ASSET_NOT_FOUND_FILE
        End Select
        'If type = AssetTypes.SOUNDEFFECT Or type = AssetTypes.SOUNDTRACK Then
        '    Return ASSET_NOT_FOUND_SOUND
        'ElseIf type = AssetTypes.SPRITEFONT Then
        '    Return ASSET_NOT_FOUND_FONT
        'ElseIf type = AssetTypes.TEXTURE2D Then
        '    Return ASSET_NOT_FOUND_FILE
        'End If
        DebugManagement.WriteLineToLog("An asset failed to load, however no Type was found to return the proper NOT_FOUND.", SeverityLevel.CRITICAL)

        Return "Unknown Type specified."
    End Function
    Public Overloads Shared Function RequestAsset(name As String, index As Integer) As String
        Calls = Calls + 1
        For i = 0 To Assets.Length - 1 Step 1
            If Assets(i).AssetName = name Or Assets(i).AssetPath = name Then
                If Assets(i).AssetIndex = index Then
                    Return Assets(i).AssetPath
                End If
            End If

        Next
        Return ASSET_NOT_FOUND_FILE
    End Function
    Public Overloads Shared Function RequestAsset(index As Integer) As String
        Calls = Calls + 1
        For i = 0 To Assets.Length - 1 Step 1
            If Assets(i).AssetIndex = index Then
                Return Assets(i).AssetPath
            End If

        Next
        Return ASSET_NOT_FOUND_FILE
    End Function
    Public Overloads Shared Function RequestAsset(index As Integer, type As String) As String
        Calls = Calls + 1
        For i = 0 To Assets.Length - 1 Step 1
            If Assets(i).AssetIndex = index Then
                If Assets(i).AssetType = type Then
                    Return Assets(i).AssetPath
                End If
            End If

        Next
        Return ASSET_NOT_FOUND_FILE
    End Function
    Public Overloads Shared Function RequestAsset(name As String, index As Integer, type As String) As String
        Calls = Calls + 1
        For i = 0 To Assets.Length - 1 Step 1
            If Assets(i).AssetName = name Or Assets(i).AssetPath = name Then
                If Assets(i).AssetIndex = index Then
                    If Assets(i).AssetType = type Then
                        Return Assets(i).AssetPath
                    End If
                End If
            End If

        Next
        Return ASSET_NOT_FOUND_FILE
    End Function
    Public Shared Function GetCallCount() As Integer
        Return Calls
    End Function
End Class
Public Enum AssetTypes As Byte
    ''' <summary>
    ''' Regulatory. Does nothing and will return null.
    ''' </summary>
    ''' <remarks></remarks>
    NO_ASSET = 0
    TEXTURE2D = 1
    SPRITEFONT = 2
    SOUNDEFFECT = 3
    SOUNDTRACK = 4
End Enum