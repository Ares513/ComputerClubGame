Public Class Dialog
    Inherits UIEntity
    Dim options As Dictionary(Of String, IAction)
    Dim rec() As Rectangle
    Private Font As SpriteFont
    Dim Buttons() As Button
    Public BorderWidth As Integer = 20
    Public BorderHeight As Integer = 20
    Public Spacing As Integer = 10
    Public Sub New(ScreenSize As Rectangle, background As Texture2D, CM As ContentManager, firstDialogText As String, firstDialogAction As IAction)
        MyBase.New(New Size(0, 0))
        Size = New Size(CInt(ScreenSize.Width / 4), CInt(ScreenSize.Height / 4))
        Center = New Vector2(ScreenSize.Center.X, ScreenSize.Center.Y)

        texture = background
        Font = CM.Load(Of SpriteFont)(AssetManager.RequestAsset("defaultFont", AssetTypes.SPRITEFONT))
        Dim ButtonAssets(2) As String
        ButtonAssets(0) = "missing_texture"
        ButtonAssets(1) = "missing_texture"
        ButtonAssets(2) = "missing_texture"
        ReDim Preserve Buttons(0)
        Buttons(0) = New Button(firstDialogText, ButtonAssets, AssetManager.RequestAsset("defaultFont", AssetTypes.SPRITEFONT), CM, True, 1.0, 1.0)
        Buttons(0).Center = New Vector2(Center.X + BorderWidth, Pos.Y + BorderHeight)
        Buttons(0).Visible = True
    End Sub
    Private ReadOnly Property ComputeRectangle(NumberInDialog As Integer, DialogSize As Size) As Rectangle
        Get

        End Get

    End Property

    Public Sub addOption(text As String, action As IAction, CM As ContentManager)
        options.Add(text, action)
        ReDim Preserve Buttons(Buttons.Length)
        Buttons(Buttons.Length - 1) = New Button(text, New String() {"missing_texture", "missing_texture", "missing_texture"}, AssetManager.RequestAsset("defaultFont", AssetTypes.SPRITEFONT), CM, False, 1, 1)

    End Sub

    Public Sub drawText(sb As SpriteBatch, screenSize As Rectangle)
        ' Dim rect As Rectangle = New Rectangle(CInt((screenSize.Width / 2) - (screenSize.Width / 20)), CInt((screenSize.Width / 2) - (screenSize.Width / 20)), CInt(screenSize.Width / 10), CInt(screenSize.Height / 10))
        Try
            For i As Integer = 0 To options.ToArray.Length - 1

                Buttons(i).Draw(sb)

            Next
        Catch e As Exception

        End Try


    End Sub

    'Public Function haveClickText(mouse As MouseState) As IAction
    '    For i As Integer = 0 To rec.Length - 1
    '        If mouse.X < rec(i).Right Then
    '            If mouse.X > rec(i).Left Then
    '                If mouse.Y < rec(i).Bottom Then
    '                    If mouse.Y > rec(i).Top Then
    '                        Return options.ElementAt(i).Value
    '                    End If
    '                End If
    '            End If
    '        End If
    '    Next
    'End Function
    Public Sub Update(EM As EntityManagement, gt As GameTime, ms As MouseState, ks As KeyboardState, CM As ContentManager, UI As UIOverlay, cam As IsoCamera)
        For Each Button In Buttons
            If Button.IsPressed(New Vector2(ms.X, ms.Y), CBool(ms.LeftButton), gt) = ButtonReturnTypes.CLICKED Then
                options(Button.Name).doAction(EM, gt, ms, ks, CM, UI, cam)
            End If
        Next
    End Sub

End Class