
Public Class DebugManagement
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' <list type="number">
    ''' <item><description><c>DEBUG</c> indicates an error used only for informing the developers of what's going on.</description></item>
    ''' <item><description><c>WARNING</c> indicates an error that is a possible threat to program function</description></item>
    ''' <item><description><c>SEVERE</c> indicates that the game's function is altered by the error.</description></item>
    ''' <item><description><c>CRITICAL</c> indicates that severe lapses in gameplay may occur as a result of the error.</description></item>
    ''' <item><description><c>FATAL</c> indicates that the game must halt because of the error.</description></item>
    ''' </list>
    ''' </remarks>
    ''' 
    Private Shared lastWrittenDebugLine As String
    Public Shared ReadOnly Property lastLine As String
        Get
            Return lastWrittenDebugLine
        End Get
    End Property
    Private Shared debugWriter As TextWriterTraceListener
    'Debug information is stacked vertically under this base point.
    Private Shared debugInfoBasePosition As Vector2 = New Vector2(0, 0)
    Private Const LOGFILE_NAME = "localDebug.log"
    Private Shared LoggerDebugCalls As Integer 'The number of times something has been written to debug.
    Private Shared NonWarningNonDebugLoggerCalls As Integer 'The number of times a non-warning call has occurred. SEVERE or higher.
    Public Sub New()


    End Sub
    Public Shared Sub WriteLineToLog(message As String, level As SeverityLevel)
        If level <> SeverityLevel.DEBUG And level <> SeverityLevel.WARNING Then
            LoggerDebugCalls = LoggerDebugCalls + 1
        Else
            NonWarningNonDebugLoggerCalls += 1
        End If
        debugWriter = New TextWriterTraceListener(InitializationSettings.GetGameSettings().globalGameFilesFolder + "\" + LOGFILE_NAME)
        Dim levelPrefix As String = "[DEFAULT]"
        Select Case level
            Case SeverityLevel.DEBUG

                levelPrefix = "[Debug]"
            Case SeverityLevel.WARNING
                levelPrefix = "[WARNING]"
            Case SeverityLevel.SEVERE
                levelPrefix = "[SEVERE]"
            Case SeverityLevel.CRITICAL
                levelPrefix = "[CRITICAL]"
            Case SeverityLevel.FATAL
                levelPrefix = "[FATAL ERROR!]"
        End Select
        Dim dateStamp As String
        dateStamp = Date.Now.ToString()
        lastWrittenDebugLine = "[" + dateStamp + "]" + " " + levelPrefix + ": " + message
        debugWriter.WriteLine(lastWrittenDebugLine)

        System.Diagnostics.Debug.Print(lastWrittenDebugLine)
        debugWriter.Dispose()


    End Sub
    ''' <summary>
    ''' The number of times ANY log line has been written.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function getCalls() As Integer

        Return LoggerDebugCalls
    End Function
    Public Shared Function getAllCalls() As Integer
        Return NonWarningNonDebugLoggerCalls + LoggerDebugCalls
    End Function
End Class

Public Enum SeverityLevel
    ''' <summary>
    ''' 
    ''' Indicates an error used only for informing the developers of what's going on.
    ''' </summary>
    ''' <remarks></remarks>
    DEBUG = 0
    ''' <summary>
    ''' Possible threat to program function.
    ''' </summary>
    ''' <remarks></remarks>
    WARNING = 1
    ''' <summary>
    ''' Indicates the game's function is altered by the error.
    ''' </summary>
    ''' <remarks></remarks>
    SEVERE = 2
    ''' <summary>
    ''' Severe lapses in gameplay may occur because this error was thrown.
    ''' </summary>
    ''' <remarks></remarks>
    CRITICAL = 3
    ''' <summary>
    ''' The program cannot continue.
    ''' </summary>
    ''' <remarks></remarks>
    FATAL = 4


End Enum
