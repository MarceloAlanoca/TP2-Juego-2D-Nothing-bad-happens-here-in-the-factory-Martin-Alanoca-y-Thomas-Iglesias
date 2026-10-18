using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
namespace MonoGameLibrary;

public static class InputManager
{
    private static Vector2 _direction;
    public static Vector2 Direction => _direction;
    public static bool Moving => _direction != Vector2.Zero;
    
    public static void Update()
    {
        _direction = Vector2.Zero;
        var keyboardState = Keyboard.GetState();

        if(keyboardState.IsKeyDown(Keys.A)) _direction.X -= 1;
        if(keyboardState.IsKeyDown(Keys.D)) _direction.X += 1;
    }
}