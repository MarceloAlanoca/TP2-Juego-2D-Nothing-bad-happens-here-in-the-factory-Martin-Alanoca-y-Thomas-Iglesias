using Microsoft.Xna.Framework;

namespace Nothing_bad_happens_here_in_the_factory;

public class Jardin : Scene
{
    public override void Cargar()
    {
        // Cambiá los nombres por los de tus assets de fondo.
        Mapa.Cargar(
            new int[,] { { 1 } },
           "Fondos", "Salida_1");

        Mapa.AgregarMurosLaterales();
        Mapa.AgregarPuerta("fin", new Rectangle(100, 240, 118, 257));
        Mapa.AgregarPuerta("salida", new Rectangle(500, 240, 118, 257));
    }
}