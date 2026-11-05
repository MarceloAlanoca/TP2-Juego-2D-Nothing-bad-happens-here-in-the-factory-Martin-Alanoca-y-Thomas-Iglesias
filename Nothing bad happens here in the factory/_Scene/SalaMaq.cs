using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using RenderingLibrary.Content;

namespace Nothing_bad_happens_here_in_the_factory;

public class SalaMaq : Scene
{
    private Texture2D _cajaFusibles;
    public override void Cargar()
    {
        _cajaFusibles = GLobals.Content.Load<Texture2D>("Assets/CajaFusibles");

        Mapa.Cargar(
            new int[,] { { 1, 2 } },
            "Fondos/SalaMaq", "SalaMaquinas_1", "SalaMaquinas_2");

        Mapa.AgregarMurosLaterales();
        Mapa.AgregarPuerta("salida", new Rectangle(100, 240, 118, 257));
        Mapa.AgregarZona("cables", new Rectangle(600, 320, 100, 170));
    }
    public void DibujarCajaFusibles(SpriteBatch sb, Viewport viewport)
{
    int ancho = 500;
    int alto = 400;

    int x = (viewport.Width - ancho) / 2;
    int y = (viewport.Height - alto) / 2;

    sb.Draw(
        _cajaFusibles,
        new Rectangle(x, y, ancho, alto),
        Color.White
    );
}
}