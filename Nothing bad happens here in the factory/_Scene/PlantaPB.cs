using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;

public class PlantaPB
{
     private Texture2D _texturefont;

    public PlantaPB()
    {
        _texturefont = GLobals.Content.Load<Texture2D>("Fondos/Recepcion");
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