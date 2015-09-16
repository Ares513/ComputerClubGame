Imports System.Windows.Forms
Imports System.Runtime.InteropServices
Public Class InputMessageFilter
    Implements IMessageFilter
    Private Const WM_KEYDOWN = 256
    Private Const WM_KEYUP = 257
    Private Const LShiftVirt = 16
    Private Const RShiftVirt = 161
    Private Const VK_BACKSPACE = 8
    Public Event OnKeyDown(sender As Object, key As Char, backSpace As Boolean)
    Public Event BackspacePressed(sender As Object, e As System.EventArgs)
    Public Event ReturnPressed(sender As Object, e As System.EventArgs)
    Public Event CancelPressed(sender As Object, E As System.EventArgs)
    Private LShifted As Boolean
    Private RShifted As Boolean
    Private Const VK_SHIFT As Short = 16
    Private Const VK_SPACE As Short = 32
    Private Const VK_RETURN = 13
    Private Const VK_ESCAPE = 27
    ''' <summary>
    ''' Whether or not to consume all keyboard messages.
    ''' Hitting Return or Escape will unfocus.
    ''' </summary>
    ''' <remarks></remarks>
    Public ConsumeKeyDownMessages As Boolean
    'Get the KeyState of this and it should tell us if it's shifted.
    Public Function PreFilterMessage(ByRef m As System.Windows.Forms.Message) As Boolean Implements System.Windows.Forms.IMessageFilter.PreFilterMessage
        Dim msg As Integer = m.Msg
        If msg = WM_KEYDOWN Then
            ' RaiseEvent OnKeyDown(Me, New KeyEventArgs
            Select Case m.WParam
                Case New IntPtr(VK_ESCAPE)
                    ConsumeKeyDownMessages = False
                    RaiseEvent CancelPressed(Me, New EventArgs())
                    Return ConsumeKeyDownMessages
                Case New IntPtr(VK_RETURN)
                    ConsumeKeyDownMessages = False
                    RaiseEvent ReturnPressed(Me, New EventArgs())
                    Return ConsumeKeyDownMessages
                Case New IntPtr(LShiftVirt)
                    LShifted = True
                    Return ConsumeKeyDownMessages
                Case New IntPtr(RShiftVirt)
                    RShifted = True
                    Return ConsumeKeyDownMessages

                Case New IntPtr(VK_BACKSPACE)
                    RaiseEvent BackspacePressed(Me, New EventArgs)
                    Return ConsumeKeyDownMessages
                Case New IntPtr(VK_SPACE)
                    RaiseEvent OnKeyDown(Me, CChar(" "), False)
                    Return ConsumeKeyDownMessages

            End Select

            If m.WParam = New IntPtr(VK_BACKSPACE) Then
               
            End If
            Dim scannedKey = MapVirtualKey(m.WParam, MapVirtualKeyMapTypes.MAPVK_VK_TO_CHAR)

            Dim result As Char
            If LShifted Or RShifted Then
                result = ChrW(CInt(scannedKey))
            Else

                result = CChar(ChrW(CInt(scannedKey)).ToString.ToLower())

            End If
            If scannedKey <> 0 Then
                RaiseEvent OnKeyDown(Me, result, False)

                Return ConsumeKeyDownMessages
            End If



        ElseIf msg = WM_KEYUP Then
            Dim scannedKey = MapVirtualKey(m.WParam, MapVirtualKeyMapTypes.MAPVK_VK_TO_CHAR)
            If m.WParam = New IntPtr(LShiftVirt) Then
                'LShift key pressed
                LShifted = False
                Return ConsumeKeyDownMessages
            ElseIf m.WParam = New IntPtr(RShiftVirt) Then
                RShifted = False
                Return ConsumeKeyDownMessages
            End If
        End If



            Return False
    End Function
    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)> _
    Public Shared Function MapVirtualKey(ByVal uCode As IntPtr, ByVal uMapType As MapVirtualKeyMapTypes) As UInt32

    End Function
    ' <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)> 

End Class
Public Enum MapVirtualKeyMapTypes As UInt32
    MAPVK_VK_TO_VSC = &H0
    MAPVK_VSC_TO_VK = &H1
    MAPVK_VK_TO_CHAR = &H2
    MAPVK_VSC_TO_VK_EX = &H3
    MAPVK_TO_VSC_EX = &H4
End Enum