
Public Class DebugScreenWriter
    Private Font As SpriteFont
    Private baseScreenPos As Vector2 'The position to base the draw methods for all of these strings.
    Public Sub New(ByVal inFont As SpriteFont, ByVal baseScreenPosition As Vector2)
        Font = inFont
        baseScreenPos = baseScreenPosition
    End Sub
    Public Sub New(ByVal Asset As String, CM As ContentManager, ByVal baseScreenPosition As Vector2)
        Font = CM.Load(Of SpriteFont)(Asset)
        baseScreenPos = baseScreenPosition
    End Sub
    Public Sub WriteGameDebugInfo(EM As EntityManagement, sb As SpriteBatch, GD As GraphicsDevice, ks As KeyboardState, ms As MouseState, ByVal UI As UIOverlay, ByVal gt As GameTime, ByVal Game As Game1, ByVal LocalPlayer As EntityPlayer, Cam As IsoCamera, frmLocation As System.Drawing.Point, gc As GarbageCollector, map As Map)
        sb.DrawString(Font, GD.Viewport.Width.ToString() + "," + GD.Viewport.Height.ToString(), baseScreenPos, Color.Orange)
        sb.DrawString(Font, GetPressedKeys(ks), baseScreenPos + New Vector2(0, 20), Color.Orange)
        sb.DrawString(Font, ms.X.ToString() + " , " + ms.Y.ToString(), New Vector2(0, 50), Color.Orange)
        sb.DrawString(Font, "Asset Manager Calls: " + AssetManager.GetCallCount.ToString(), New Vector2(0, 80), Color.Orange)
        sb.DrawString(Font, "FPS: " + CStr(Math.Round(1 / gt.ElapsedGameTime.TotalSeconds)), New Vector2(0, 100), Color.Orange)
        sb.DrawString(Font, "Elapsed Game Time: " + Math.Round(gt.TotalGameTime.TotalSeconds).ToString(), New Vector2(0, 120), Color.Orange)
        If DebugManagement.getCalls() > 0 Then 'Change the color if log lines have been thrown.
            sb.DrawString(Font, "Log lines written: " + DebugManagement.getCalls().ToString(), New Vector2(0, 150), Color.Red)
        Else
            sb.DrawString(Font, "Log lines written: " + DebugManagement.getCalls().ToString(), New Vector2(0, 150), Color.Orange)
        End If
        sb.DrawString(Font, "Window Handle: " + Game.Window.Handle.ToString(), New Vector2(0, 180), Color.Orange)
        sb.DrawString(Font, "Scroll Wheel Value: " + ms.ScrollWheelValue.ToString(), New Vector2(0, 200), Color.Orange)
        sb.DrawString(Font, "Velocity: " + LocalPlayer.Vel.ToString(), New Vector2(0, 220), Color.Orange)
        sb.DrawString(Font, "Center: " + LocalPlayer.Center(Cam).ToString(), New Vector2(0, 240), Color.Orange)
        sb.DrawString(Font, "Position: " + LocalPlayer.Pos.ToString(), New Vector2(0, 260), Color.Orange)
        sb.DrawString(Font, "WinForm Mouse Position" + frmLocation.ToString(), New Vector2(0, 280), Color.Orange)
        sb.DrawString(Font, "Selected Entity: " + EM.HilightedEntity(ms, Cam), New Vector2(0, 300), Color.Orange)
        sb.DrawString(Font, "Player Screen Position: " + Cam.MapToScreen(EM.LocalPlayerInfo.Pos).ToString(), New Vector2(0, 320), Color.Orange)
        sb.DrawString(Font, "Projectile Count " + EM.Projectiles.Count.ToString(), New Vector2(0, 340), Color.Orange)
        sb.DrawString(Font, "Attacking " + EM.LocalPlayerInfo.Attacking.ToString(), New Vector2(0, 360), Color.Orange)
        sb.DrawString(Font, "Remaining Garbage Collection Loops " + gc.RemainingLoops.ToString(), New Vector2(0, 380), Color.Orange)
        sb.DrawString(Font, "Mouse Tile Position " + Cam.ScreenToMap(New Vector2(ms.X, ms.Y), 0).ToString(), New Vector2(0, 400), Color.Orange)
        Dim walkLoc As NavLoc = map.GetWalkableLoc(Cam.ScreenToWorldRay(New Vector2(ms.X, ms.Y)))
        sb.DrawString(Font, "Mouse Walkable Position " + If(IsNothing(walkLoc), "Nothing", walkLoc.Position.ToString()), New Vector2(0, 420), Color.Orange)
        sb.DrawString(Font, "Loaded Asset File " + AssetManager.ASSET_MANIFEST_FILE_NAME, New Vector2(0, 440), Color.Orange)
        sb.DrawString(Font, "SkillTreeState " + UI.PlayerSkillTree.TreeLocationState.ToString(), New Vector2(0, 480), Color.Orange)
        sb.DrawString(Font, "PartyState " + UI.Party.CurrentState.ToString(), New Vector2(0, 500), Color.Orange)
        sb.DrawString(Font, "GameTime Elapsed MS " + gt.ElapsedGameTime.Milliseconds.ToString() + " " + EM.LocalPlayerInfo.LastPathingDuration.ToString(), New Vector2(0, 520), Color.Orange)
        sb.DrawString(Font, "Current Player Anim " + EM.LocalPlayerInfo.animation.CurrentAnimation.ToString(), New Vector2(0, 540), Color.Orange)

        If Not IsNothing(DebugManagement.lastLine) Then
            sb.DrawString(Font, DebugManagement.lastLine, New Vector2(0, 500), Color.Orange)
        End If
    End Sub
    Private Function GetPressedKeys(ks As KeyboardState) As String
        Dim i As Integer
        Dim outputString As String = ""
        For i = 0 To ks.GetPressedKeys.Length - 1 Step 1
            outputString += ks.GetPressedKeys()(i).ToString() + ","

        Next
        Return outputString
    End Function
End Class
