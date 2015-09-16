Public Class HotKeyHandler
    Dim keyDictionary As Dictionary(Of Keys, IAction)
    Dim hotKeyList As HotKeys
    Dim leftmouseState As Dictionary(Of Boolean, IAction)
    Dim rightmouseState As Dictionary(Of Boolean, IAction)
    Dim leftmouseDown As IAction

    Public Property leftMouseClick As IAction
        Get
            Return leftmouseDown
        End Get
        Set(value As IAction)
            leftmouseDown = value
        End Set
    End Property

    Private oldms As MouseState = New MouseState(0, 0, 0,
                                                 ButtonState.Released, ButtonState.Released, ButtonState.Released,
                                                 ButtonState.Released, ButtonState.Released)

    Public Sub New()
        
        hotKeyList = New HotKeys()
        keyDictionary = New Dictionary(Of Keys, IAction)
        leftmouseState = New Dictionary(Of Boolean, IAction)
        rightmouseState = New Dictionary(Of Boolean, IAction)

    End Sub
    Public Sub addKey(inKey As Keys, inAction As IAction)
        If (hotKeyList.checkKey(inKey))
            keyDictionary.Remove(inKey)
            keyDictionary.Add(inKey, inAction)
        else
            hotKeyList.addKey(inKey)
            keyDictionary.Add(inKey, inAction)
        end if
    End Sub
    Public Sub addLeftMouseState(inAction As IAction)
        If (leftmouseState.ContainsKey(True))
            leftmouseState.Remove(true)
            leftmouseState.Add(true, inAction)
        else
            leftmouseState.Add(true, inAction)
        end if
    End Sub
    Public Sub addRightMouseState(inAction As IAction)
        If (rightmouseState.ContainsKey(True))
            rightmouseState.Remove(true)
            rightmouseState.Add(true, inAction)
        else
            rightmouseState.Add(true, inAction)
        end if
    End Sub
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="side"> True = Left mouse button. False = right mouse button</param>
    ''' <remarks></remarks>
    Public Sub removemouseState(side As Boolean)
        If side 
            leftmouseState.Remove(True)
        Else
            rightmouseState.Remove(True)
        End If
    End Sub
    Public Sub runActions(EM As EntityManagement, gt As GameTime, MS As MouseState, ks As KeyboardState, CM As ContentManager, UI As UIOverlay, cam As IsoCamera)
        'mouse method
        If MS.LeftButton = ButtonState.Pressed And oldms.LeftButton = ButtonState.Released And Not IsNothing(leftmouseDown) Then
            leftmouseDown.doAction(EM, gt, MS, ks, CM, UI, cam)
        End If

        If (MS.LeftButton = ButtonState.Pressed And leftmouseState.ContainsKey(True)) Then
            leftmouseState.Item(True).doAction(EM, gt, MS, ks, CM, UI, cam)
        End If
        If (MS.RightButton = ButtonState.Pressed And rightmouseState.ContainsKey(True)) Then
            rightmouseState.Item(True).doAction(EM, gt, MS, ks, CM, UI, cam)
        End If
        'keyboard method
        Dim activeKeys As Keys() = hotKeyList.getKeysInUse(ks)
        For i = 0 To activeKeys.Length - 1 Step 1
            If (keyDictionary.ContainsKey(activeKeys(i))) Then
                keyDictionary.Item(activeKeys(i)).doAction(EM, gt, MS, ks, CM, UI, cam)
            End If
        Next

        oldms = MS
    End Sub
    Public Sub EmulateKey(key As Keys, EM As EntityManagement, gt As GameTime, MS As MouseState, ks As KeyboardState, CM As ContentManager, UI As UIOverlay, cam As IsoCamera)
        If (keyDictionary.ContainsKey(key)) Then
            keyDictionary.Item(key).doAction(EM, gt, MS, ks, CM, UI, cam)
        End If
    End Sub
    ''' <summary>
    ''' Gets the IAction associated with a key.
    ''' </summary>
    ''' <param name="key">Which KeyValue to check. </param>
    ''' <returns>Null for no value found. THe Iaction associated with a key will be returned otherwise. </returns>
    ''' <remarks></remarks>

    Public Function GetAssociatedActions(key As Keys) As IAction
        If (keyDictionary.ContainsKey(key)) Then
            Return keyDictionary.Item(key)
        Else
            Return Nothing
        End If
    End Function
End Class
