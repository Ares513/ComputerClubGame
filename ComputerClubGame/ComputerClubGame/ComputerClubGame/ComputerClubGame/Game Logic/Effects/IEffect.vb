''' <summary>
''' Effects serve as a way to manage how spells and abilities are cast and prevent duplicate code. A Periodic Effect will run a specified action a number of times, for example.
''' </summary>
''' <remarks></remarks>
Public Interface IEffect

    Property isUpdating As Boolean
    Sub Update(EM As EntityManagement, gt As Microsoft.Xna.Framework.GameTime, ms As Microsoft.Xna.Framework.Input.MouseState, ks As Microsoft.Xna.Framework.Input.KeyboardState, CM As Microsoft.Xna.Framework.Content.ContentManager, UI As UIOverlay, cam As IsoCamera)




End Interface
