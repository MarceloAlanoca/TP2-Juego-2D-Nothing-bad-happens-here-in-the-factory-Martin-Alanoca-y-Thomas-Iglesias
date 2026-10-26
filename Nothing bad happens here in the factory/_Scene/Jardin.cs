using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;

namespace Nothing_bad_happens_here_in_the_factory;

public class Jardin
{
    private Texture2D _texturefont;

    public Jardin()
    {
        _texturefont = GLobals.Content.Load<Texture2D>("Fondos/Salida_1");
    }

    public void Draw()
    {
        GLobals.SpriteBatch.Draw(
            _texturefont,
            new Rectangle(
                0,
                0,
                GLobals.SpriteBatch.GraphicsDevice.Viewport.Width,
                GLobals.SpriteBatch.GraphicsDevice.Viewport.Height
            ),Color.White);
    }
}