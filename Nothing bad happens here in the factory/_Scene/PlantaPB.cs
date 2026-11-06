using Microsoft.Xna.Framework;

namespace Nothing_bad_happens_here_in_the_factory;

public class PlantaPB : Scene
{
    private bool notaVista;
    public PlantaPB(bool n)
    {
        notaVista = n;
    }
    public override void Cargar()
    {
        Mapa.Cargar(
            new int[,] { { 1, 2 } },
            "Fondos", "Recepcion", "Recepcion_2");

        Mapa.AgregarMurosLaterales();
        Mapa.AgregarPuerta("maquina",   new Rectangle(Mapa.AnchoTotal - 580, 240, 118, 257));
        Mapa.AgregarPuerta("vestuario", new Rectangle(Mapa.AnchoTotal - 330, 240, 118, 257));
        if (notaVista)
        {
            Mapa.AgregarZona("escritorio",new Rectangle(190,400,100,90));
        }
    }
}