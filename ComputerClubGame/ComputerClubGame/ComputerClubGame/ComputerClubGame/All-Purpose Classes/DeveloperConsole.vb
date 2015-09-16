Imports System.Reflection
''' <summary>
''' Runs commands asynchronously when entered by the message filter.
''' Once commands are entered, they are added to the queue and are run the following tick.
''' </summary>
''' <remarks></remarks>
Public Class DeveloperConsole

    Private PendingCommands As List(Of PendingConsoleCommand)
    Private ICommandListing As List(Of ICommand)
    Public Sub New()
        PendingCommands = New List(Of PendingConsoleCommand)
        ICommandListing = New List(Of ICommand)
        ICommandListing.Add(New SetPlayerProperty())
        ICommandListing.Add(New InvokeMethodOnPlayer())
        ICommandListing.Add(New PrintPlayerMembers())

    End Sub

    ''' <summary>
    ''' Adds a new command to the queue. It will be run automatically on the next tick.
    ''' </summary>
    ''' <param name="value">The string value of the command, including all arguments.</param>
    ''' <param name="ignoreCase">Whether or not to ignore case.</param>
    ''' <param name="delay">How many milliseconds to wait before executing the command.</param>
    ''' <remarks></remarks>
    Public Sub AddToQueue(value As String, ignoreCase As Boolean, delay As Integer)
        PendingCommands.Add(New PendingConsoleCommand(value, ignoreCase, delay))
    End Sub
    ''' <summary>
    ''' Registers a new command with this instance of DeveloperConsole.
    ''' </summary>
    ''' <param name="Command">ICommand instance. See ICommand interface for creating new commands.</param>
    ''' <remarks></remarks>
    Public Sub RegisterCommand(Command As ICommand)
        ICommandListing.Add(Command)
        ICommandListing.Sort()
    End Sub
    ''' <summary>
    ''' Updats the DeveloperConsole to process any pending commands.
    ''' </summary>
    ''' <param name="gt">GameTime instance</param>
    ''' <param name="chatWindow">The output chat box. May or may not be integrated with UIOverlay.</param>
    ''' <param name="EM">EntityManagement instance</param>
    ''' <param name="Game">The base game object, for actions that hide the mouse.</param>
    ''' <param name="UI">UIOverlay instance</param>
    ''' <param name="ms">Up to date MouseState</param>
    ''' <param name="ks">Up to date KeyboardState</param>
    ''' <remarks></remarks>
    Public Sub Update(gt As GameTime, chatWindow As FormattedTextProcessor, EM As EntityManagement, Game As Game1, UI As UIOverlay, ms As MouseState, ks As KeyboardState)
        For Each Pending In PendingCommands
            Pending.Update(gt)
        Next
        ExecutePending(chatWindow, EM, Game, UI, ms, ks)
        For i = 0 To PendingCommands.Count - 1 Step 1
            If PendingCommands.Count = 0 Then
                Exit For
            End If
            If PendingCommands(i).Executed = True Then
                PendingCommands.RemoveAt(i)
                i = 0
            End If
        Next
    End Sub

    Private Sub ExecutePending(chatWindow As FormattedTextProcessor, EM As EntityManagement, Game As Game1, UI As UIOverlay, ms As MouseState, ks As KeyboardState)
        For Each Pending In PendingCommands
            If Pending.Ready And Not Pending.Executed Then
                ProcessCommand(Pending, chatWindow, EM, Game, UI, ms, ks)
                'Will be removed on the next tick.
                Pending.Executed = True

            End If
        Next

    End Sub
    Private Sub ProcessCommand(value As PendingConsoleCommand, chatWindow As FormattedTextProcessor, EM As EntityManagement, Game As Game1, UI As UIOverlay, ms As MouseState, ks As KeyboardState)


        Dim workingStr As String = value.Command
        If value.processAsLowerCase Then
            workingStr = workingStr.ToLower()
        End If
        'ConHelp command processing- this command must be seperate because it needs to know info about other commands
        If workingStr.Trim() = "ConHelp" Or workingStr.Trim() = "conhelp" Then
            chatWindow.addLine("Showing names and descriptions of all commands.")
            For Each comm In ICommandListing
                chatWindow.addLine(comm.Name + ": " + comm.Description)

            Next

            Exit Sub
        End If

        For Each possibleCommand In ICommandListing
            If workingStr.Contains(possibleCommand.Name) Then
                If workingStr.Length > possibleCommand.Name.Length Then
                    workingStr = workingStr.Remove(0, possibleCommand.Name.Length + 1)
                Else
                    workingStr = workingStr.Remove(0, possibleCommand.Name.Length)
                End If
                chatWindow.addLine("Executing command " + value.Command)
                possibleCommand.Execute(EM, Game, UI, ms, ks, workingStr.Split(" "c))

            End If
        Next



    End Sub
End Class
Public Class PendingConsoleCommand
    Public Command As String
    Public processAsLowerCase As Boolean
    Private initialDelay As Integer
    Private currentDelay As Integer
    Public Executed As Boolean
    Public ReadOnly Property Ready As Boolean
        Get
            If initialDelay = 0 Then
                Return True
            End If
            If currentDelay <= 0 Then
                Return True
            End If
            Return False
        End Get
    End Property


    Public Sub New(value As String, lowerCase As Boolean, delay As Integer)
        initialDelay = delay
        currentDelay = delay
        Command = value
        processAsLowerCase = lowerCase

    End Sub
    Public Sub Update(gt As GameTime)
        If currentDelay >= 0 Then
            currentDelay -= gt.ElapsedGameTime.Milliseconds
        End If
    End Sub


End Class
Public Interface ICommand
    Sub Execute(EM As EntityManagement, Game As Game1, UI As UIOverlay, ms As MouseState, ks As KeyboardState, additionalArgs() As String)
    ReadOnly Property Name As String
    ReadOnly Property Description As String

End Interface



Public Class InvokeMethodOnPlayer
    Implements ICommand

    Public ReadOnly Property Description As String Implements ICommand.Description
        Get
            Return "Invokes the specified method on the player. The method MUST have no arguments or an error will be thrown."
        End Get

    End Property

    Public Sub Execute(EM As EntityManagement, Game As Game1, UI As UIOverlay, ms As Microsoft.Xna.Framework.Input.MouseState, ks As Microsoft.Xna.Framework.Input.KeyboardState, args() As String) Implements ICommand.Execute
        Dim mi As MethodInfo

        'Get method info on the specified object.
        Try

            mi = EM.LocalPlayerInfo.GetType.GetMethod(args(0))
            If Not IsNothing(mi) Then
                mi.Invoke(EM.LocalPlayerInfo, Nothing)
            End If
        Catch Ex As Exception

        End Try
    End Sub

    Public ReadOnly Property Name As String Implements ICommand.Name
        Get
            Return "InvokeMethod"
        End Get

    End Property
End Class
Public Class SetPlayerProperty
    Implements ICommand

    Public ReadOnly Property Description As String Implements ICommand.Description
        Get
            Return "Sets a player property to a value. If you set a property that isn't a primitive, an error will be thrown"
        End Get
    End Property

    Public Sub Execute(EM As EntityManagement, Game As Game1, UI As UIOverlay, ms As Microsoft.Xna.Framework.Input.MouseState, ks As Microsoft.Xna.Framework.Input.KeyboardState, additionalArgs() As String) Implements ICommand.Execute
        Try
            Dim pi As PropertyInfo = EM.LocalPlayerInfo.GetType.GetProperty(additionalArgs(0))

            pi.SetValue(EM.LocalPlayerInfo, Convert.ChangeType(additionalArgs(1), pi.PropertyType), Nothing)
        Catch

        End Try
    End Sub

    Public ReadOnly Property Name As String Implements ICommand.Name
        Get
            Return "SetPlayerProperty"
        End Get
    End Property
End Class
Public Class PrintPlayerMembers
    Implements ICommand

    Public ReadOnly Property Description As String Implements ICommand.Description
        Get
            Return "Prints all accessible members."
        End Get
    End Property

    Public Sub Execute(EM As EntityManagement, Game As Game1, UI As UIOverlay, ms As Microsoft.Xna.Framework.Input.MouseState, ks As Microsoft.Xna.Framework.Input.KeyboardState, additionalArgs() As String) Implements ICommand.Execute
        Try
            For Each Prop In EM.LocalPlayerInfo.GetType().GetProperties()
                UI.RecentMessages.addLine(Prop.Name + ": " + Prop.GetValue(EM.LocalPlayerInfo, Nothing).ToString())
            Next
        Catch ex As Exception
        End Try
    End Sub

    Public ReadOnly Property Name As String Implements ICommand.Name
        Get
            Return "SetPlayer"
        End Get
    End Property
End Class
