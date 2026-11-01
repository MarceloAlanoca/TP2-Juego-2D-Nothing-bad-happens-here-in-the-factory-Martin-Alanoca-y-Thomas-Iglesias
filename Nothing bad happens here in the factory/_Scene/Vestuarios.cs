using Microsoft.Xna.Framework;

namespace Nothing_bad_happens_here_in_the_factory;

public class Vestuarios : Scene
{
    public override void Cargar()
    {
        Mapa.Cargar(
            new int[,] { { 1, } },
            "Fondos/Vestuarios", "Vestidores");

        Mapa.AgregarMurosLaterales();
        Mapa.AgregarPuerta("salida", new Rectangle(100, 240,118, 257));
    }
}