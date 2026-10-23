using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary;

public class CuadradoColision
{
    // Posición y tamaño del hitbox relativos a la esquina del sprite (sin escala)
    public Vector2 Desfase;
    public Vector2 Tamano;
    public float Escala = 1f;

    public Vector2 Posicion { get; private set; }

    public CuadradoColision(Vector2 desfase, Vector2 tamano, float escala = 1f)
    {
        Desfase = desfase;
        Tamano = tamano;
        Escala = escala;
    }

    // Rectángulo en coordenadas del mundo
    public Rectangle Limites => new Rectangle(
        (int)(Posicion.X + Desfase.X * Escala),
        (int)(Posicion.Y + Desfase.Y * Escala),
        (int)(Tamano.X * Escala),
        (int)(Tamano.Y * Escala));

    public Vector2 Centro => new Vector2(
        Limites.X + Limites.Width / 2f,
        Limites.Y + Limites.Height / 2f);

    // Llamar cada frame con la posición actual del dueño (esquina superior izquierda del sprite)
    public void Actualizar(Vector2 posicion)
    {
        Posicion = posicion;
    }

    public bool Intersecta(CuadradoColision otro) => Limites.Intersects(otro.Limites);

    public bool Intersecta(Rectangle rectangulo) => Limites.Intersects(rectangulo);

    public bool Contiene(Vector2 punto) => Limites.Contains(punto);

    // Para depurar: dibuja el contorno del hitbox
    public void Dibujar(Texture2D pixel, Color color)
    {
        Rectangle r = Limites;
        GLobals.SpriteBatch.Draw(pixel, new Rectangle(r.X, r.Y, r.Width, 2), color);                 // arriba
        GLobals.SpriteBatch.Draw(pixel, new Rectangle(r.X, r.Bottom - 2, r.Width, 2), color);        // abajo
        GLobals.SpriteBatch.Draw(pixel, new Rectangle(r.X, r.Y, 2, r.Height), color);                // izquierda
        GLobals.SpriteBatch.Draw(pixel, new Rectangle(r.Right - 2, r.Y, 2, r.Height), color);        // derecha
    }
}