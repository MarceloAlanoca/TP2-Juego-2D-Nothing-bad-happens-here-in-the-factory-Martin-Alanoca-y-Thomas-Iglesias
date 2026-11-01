using Microsoft.Xna.Framework;

namespace Nothing_bad_happens_here_in_the_factory;

public class PlantaPB : Scene
{
    public override void Cargar()
    {
        Mapa.Cargar(
            new int[,] { { 1, 2 } },
            "Fondos", "Recepcion", "Recepcion_2");

        Mapa.AgregarMurosLaterales();
        Mapa.AgregarPuerta("maquina",   new Rectangle(Mapa.AnchoTotal - 580, 240, 118, 257));
        Mapa.AgregarPuerta("vestuario", new Rectangle(Mapa.AnchoTotal - 330, 240, 118, 257));
    }
}