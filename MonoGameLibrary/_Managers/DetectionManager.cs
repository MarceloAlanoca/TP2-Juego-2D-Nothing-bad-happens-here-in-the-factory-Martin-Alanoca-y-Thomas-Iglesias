
using Microsoft.Xna.Framework;

namespace MonoGameLibrary;

public class DetectionManager
{
    // Detecta si dos hitboxes se superponen.
    public bool HayColision(
        CuadradoColision a,
        CuadradoColision b)
    {
        return a.Intersecta(b);
    }

    // Detecta si una hitbox entra en un área rectangular.
    public bool DentroDelArea(
        CuadradoColision objeto,
        Rectangle area)
    {
        return objeto.Intersecta(area);
    }

    // Detecta si un punto está dentro del cono de visión.
    public bool DentroDelCono(
        ConoColision cono,
        Vector2 punto)
    {
        return cono.Contiene(punto);
    }

    // Detecta si el centro de una hitbox está dentro del cono.
    public bool DentroDelCono(
        ConoColision cono,
        CuadradoColision objeto)
    {
        Rectangle r = objeto.Limites;

        // Comprobar las esquinas de la hitbox.
        return cono.Contiene(new Vector2(r.Left, r.Top))
            || cono.Contiene(new Vector2(r.Right, r.Top))
            || cono.Contiene(new Vector2(r.Left, r.Bottom))
            || cono.Contiene(new Vector2(r.Right, r.Bottom));
    }
}