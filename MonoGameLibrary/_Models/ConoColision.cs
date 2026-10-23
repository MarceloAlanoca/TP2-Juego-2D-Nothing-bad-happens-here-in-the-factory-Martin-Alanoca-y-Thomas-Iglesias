using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary;

public class ConoColision
{
    public float Distancia;      // alcance en píxeles
    public float Angulo;         // ángulo total del cono, en grados

    public Vector2 Origen { get; private set; }
    public Vector2 Direccion { get; private set; } = Vector2.UnitX;

    public ConoColision(float distancia, float angulo)
    {
        Distancia = distancia;
        Angulo = angulo;
    }

    // Llamar cada frame con la posición y la dirección actuales del dueño
    public void Actualizar(Vector2 origen, Vector2 direccion)
    {
        Origen = origen;
        if (direccion != Vector2.Zero)
            Direccion = Vector2.Normalize(direccion);
    }

    public bool Contiene(Vector2 punto)
    {
        Vector2 haciaPunto = punto - Origen;
        float distancia = haciaPunto.Length();

        if (distancia > Distancia) return false;   // fuera de alcance
        if (distancia == 0) return true;           // justo encima

        haciaPunto /= distancia;

        // Dot = coseno del ángulo entre la mirada y la dirección al punto
        float cosAngulo = Vector2.Dot(Direccion, haciaPunto);
        float cosMitadCono = MathF.Cos(MathHelper.ToRadians(Angulo / 2f));

        return cosAngulo >= cosMitadCono;
    }

    // Para depurar: dibuja los dos bordes y el arco
    public void Dibujar(Texture2D pixel, Color color)
    {
        float mitad = MathHelper.ToRadians(Angulo / 2f);

        Vector2 izquierda = Origen + Rotar(Direccion, -mitad) * Distancia;
        Vector2 derecha = Origen + Rotar(Direccion, mitad) * Distancia;

        Linea(pixel, Origen, izquierda, color);
        Linea(pixel, Origen, derecha, color);

        const int segmentos = 12;
        Vector2 anterior = izquierda;
        for (int i = 1; i <= segmentos; i++)
        {
            float t = -mitad + 2f * mitad * i / segmentos;
            Vector2 punto = Origen + Rotar(Direccion, t) * Distancia;
            Linea(pixel, anterior, punto, color);
            anterior = punto;
        }
    }

    private static Vector2 Rotar(Vector2 v, float radianes)
    {
        float cos = MathF.Cos(radianes), sin = MathF.Sin(radianes);
        return new Vector2(v.X * cos - v.Y * sin, v.X * sin + v.Y * cos);
    }

    private static void Linea(Texture2D pixel, Vector2 a, Vector2 b, Color color)
    {
        Vector2 borde = b - a;
        float angulo = MathF.Atan2(borde.Y, borde.X);
        GLobals.SpriteBatch.Draw(pixel, a, null, color, angulo, Vector2.Zero,
            new Vector2(borde.Length(), 2f), SpriteEffects.None, 0);
    }
}