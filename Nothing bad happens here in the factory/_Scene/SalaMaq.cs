using Microsoft.Xna.Framework;

namespace Nothing_bad_happens_here_in_the_factory;

public class SalaMaq : Scene
{
        public override void Cargar()
    {
        Mapa.Cargar(
            new int[,] { { 1,2 } },
            "Fondos/SalaMaq", "SalaMaquinas_1", "SalaMaquinas_2");

        Mapa.AgregarMurosLaterales();
        Mapa.AgregarPuerta("salida", new Rectangle(100, 240, 118, 257));
    }
}