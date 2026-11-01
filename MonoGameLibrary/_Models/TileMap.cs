using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;

namespace Nothing_bad_happens_here_in_the_factory;

public class TileMap
{
    private readonly List<Texture2D> _tiles = new();
    private int[,] _mapa;
    private int _destino;   // tamaño del tile ya escalado en pantalla

    // Posición X de la cámara dentro del mapa (en píxeles de pantalla).
    public float Camara { get; private set; }

    public int Filas => _mapa.GetLength(0);
    public int Columnas => _mapa.GetLength(1);
    public int AnchoTotal => Columnas * _destino;
    public int AltoTotal => Filas * _destino;

    // Colisiones que define cada escenario
    public List<Rectangle> Muros { get; } = new();
    public Dictionary<string, Rectangle> Puertas { get; } = new();

    // El escenario pasa el mapa y las imágenes (1 = primera imagen, 2 = segunda...)
    public void Cargar(int[,] mapa, string carpeta, params string[] nombres)
    {
        _mapa = mapa;
        _tiles.Clear();
        Muros.Clear();
        Puertas.Clear();

        foreach (var nombre in nombres)
            _tiles.Add(GLobals.Content.Load<Texture2D>($"{carpeta}/{nombre}"));

        // El mapa se escala para que ocupe todo el alto de la ventana.
        int altoVentana = GLobals.SpriteBatch.GraphicsDevice.Viewport.Height;
        _destino = (int)MathF.Ceiling(altoVentana / (float)Filas);
    }

    public void AgregarMurosLaterales(int grosor = 35)
    {
        Muros.Add(new Rectangle(0, 0, grosor, AltoTotal));
        Muros.Add(new Rectangle(AnchoTotal - grosor, 0, grosor, AltoTotal));
    }

    public void AgregarMuro(Rectangle r) => Muros.Add(r);
    public void AgregarPuerta(string nombre, Rectangle r) => Puertas[nombre] = r;

    // Centra la cámara en una posición X del mapa sin salirse de los bordes.
    public void SeguirA(float x)
    {
        int anchoVentana = GLobals.SpriteBatch.GraphicsDevice.Viewport.Width;
        float max = Math.Max(0, AnchoTotal - anchoVentana);
        Camara = MathHelper.Clamp(x - anchoVentana / 2f, 0, max);
    }

    public void DibujarColisiones(Texture2D pixel)
    {
        var sb = GLobals.SpriteBatch;
        foreach (var m in Muros)
            sb.Draw(pixel, m, Color.Red * 0.4f);
        foreach (var p in Puertas.Values)
            sb.Draw(pixel, p, Color.Lime * 0.4f);
    }

    public void Draw()
    {
        int anchoVentana = GLobals.SpriteBatch.GraphicsDevice.Viewport.Width;

        // Solo se dibujan las columnas visibles.
        int desde = Math.Max(0, (int)(Camara / _destino));
        int hasta = Math.Min(Columnas - 1, (int)((Camara + anchoVentana) / _destino));

        for (int fila = 0; fila < Filas; fila++)
        {
            for (int col = desde; col <= hasta; col++)
            {
                int id = _mapa[fila, col];
                if (id == 0) continue;

                var destino = new Rectangle(
                    (int)(col * _destino - Camara),
                    fila * _destino,
                    _destino,
                    _destino);

                GLobals.SpriteBatch.Draw(_tiles[id - 1], destino, Color.White);
            }
        }
    }
}