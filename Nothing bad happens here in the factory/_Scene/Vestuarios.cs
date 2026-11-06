using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using RenderingLibrary.Content;

namespace Nothing_bad_happens_here_in_the_factory;

public class Vestuarios : Scene
{
    public Rectangle notaRec = new Rectangle(550, 340, 35, 150);
    private Texture2D dibujo;
    public override void Cargar()
    {
        Mapa.Cargar(
            new int[,] { { 1, } },
            "Fondos/Vestuarios", "Vestidores");
        dibujo = GLobals.Content.Load<Texture2D>("Assets/nota_contenido");
        Mapa.AgregarMurosLaterales();
        Mapa.AgregarPuerta("salida", new Rectangle(100, 240,118, 257));
        Mapa.AgregarZona("nota",new Rectangle(550, 340, 35, 150));
    }
    public void DibujarNota(SpriteBatch sb, Viewport viewport)
{
    int ancho = 500;
    int alto = 400;

    int x = (viewport.Width - ancho) / 2;
    int y = (viewport.Height - alto) / 2;

    sb.Draw(
        dibujo,
        new Rectangle(x, y, ancho, alto),
        Color.White
    );
}
}