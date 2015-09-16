Public Class HotKeys
    Dim activeKeys As Keys()
    Dim listeningForKey As Boolean = false
    Public Sub new()
        ReDim activeKeys(0)
        activeKeys(0) = Nothing
    End Sub

    Public Sub setListening(input As Boolean)
        listeningForKey = input
    End Sub
    Public function isListeningForKey() As Boolean
        Return listeningForKey
    End function
    Public function getKeysInUse(Currentkeyboard As KeyboardState) As Keys()
        Dim inUseKeys As Keys()
        ReDim inUseKeys(0)
        inUseKeys(0) = Nothing
        For i = 0 to CurrentKeyboard.GetPressedKeys.Length -1 Step 1
            For j = 0 to getKeys.Length -1 Step 1
                If (CurrentKeyboard.GetPressedKeys(i) = getKeys(j))
                    If (inUseKeys(0) = Nothing)
                                inUseKeys(0) = getKeys(j)
                            Else
                                ReDim inUseKeys(inUseKeys.Length)
                                 inUseKeys(inUseKeys.Length -1) = getKeys(j)
                            End if
                End If
            next
        next
        Return inUseKeys
    End function
    Public Function checkIfKeyInUse(inKey As Keys, currentKeyboard As KeyboardState) As Boolean
        Dim inUseKeys As Keys() = getKeysInUse(currentKeyboard)
        For i = 0 to inUseKeys.Length -1 Step 1
            If (inKey = inUseKeys(i))
                Return true
            End If
        Next
        Return False
    End Function
    Public Sub setKey(oldKeyboardState As KeyboardState, newKeyboardState As KeyboardState)
            Dim newKeys As Keys()
            ReDim newKeys(0)
            newKeys(0) = nothing
            If (Not(oldKeyboardState.GetPressedKeys().Length = newKeyboardState.GetPressedKeys.Length()))
            For i=0 to newKeyboardState.GetPressedKeys().Length -1 Step 1
                    If (oldKeyboardState.GetPressedKeys().Length = 0)
                        newKeys(0) = newKeyboardState.GetPressedKeys(i)
                    End If
                for j=0 to oldKeyboardState.GetPressedKeys().Length-1 Step 1
                    If (newKeyboardState.GetPressedKeys(i) = oldKeyboardState.GetPressedKeys(j))
                        j = oldKeyboardState.GetPressedKeys().Length-1
                    Else
                        If ( j = oldKeyboardState.GetPressedKeys().Length-1)
                            If (newKeys(0) = Nothing)
                                newKeys(0) = newKeyboardState.GetPressedKeys(i)
                            Else
                                ReDim Preserve newKeys(newKeys.Length)
                                newKeys(newKeys.length -1) = newKeyboardState.GetPressedKeys(i)
                            End If
                        End If
                    End If
                Next
            Next

            For i =0 to newKeys.Length -1 Step 1
                addKey(newKeys(i))
                setListening(False)
            Next
            end if
    End Sub
    Public Sub addKey(inKey As Keys)
        If (activeKeys(0) = Nothing)
            activeKeys(0) = inKey
        Else
            ReDim Preserve activeKeys(activeKeys.Length)
            activeKeys(activeKeys.Length - 1) = inKey
        end if
    End Sub
    Public Sub removeKey(inKey As Keys)
        Dim storeKeys As Keys()
        ReDim storeKeys(0)
        storeKeys(0) = Nothing
        For i = 0 to activeKeys.Length Step 1
            If (activeKeys(i) = inKey)
            Else
                If (storeKeys(0) = Nothing)
                    storeKeys(0) = activeKeys(i)
                Else
                    ReDim Preserve storeKeys(storeKeys.Length)
                    storeKeys(storeKeys.Length - 1) = activeKeys(i)
                end if
            End If
        Next
        ReDim activeKeys(storeKeys.Length - 1)
        activeKeys = storeKeys
    End Sub
    Public Function getKeys() As Keys()
        Return activeKeys
    End Function
    Public Function checkKey(inKey As Keys) As Boolean
        For i = 0 To activeKeys.Length - 1 Step 1
            If (activeKeys(i) = inKey) Then
                Return True
            End If
        Next
        Return false
    End Function
End Class
