using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;

namespace Nothing_bad_happens_here_in_the_factory;

public class TileMap
{
    // Tiles cargados del Content. El número del mapa apunta a esta lista:
    // 0 = vacío, 1 = _tiles[0], 2 = _tiles[1], etc.
    private readonly List<Texture2D> _tiles = new();

    // Recorrido horizontal: una fila de tiles de izquierda a derecha.
    // 1 = Fondos/Salida_1 | 2 = Cuarto/Cuarto_1 | 3 = Cuarto/Cuarto_2
    private readonly int[,] _mapa =
    {
        { 1,2,3 }
    };

    private const int TileSize = 128;   // tamaño original de cada imagen en píxeles
    private int _destino;               // tamaño del tile ya escalado en pantalla

    // Posición X de la cámara dentro del mapa (en píxeles de pantalla).
    public float Camara { get; private set; }

    public int Filas => _mapa.GetLength(0);
    public int Columnas => _mapa.GetLength(1);
    public int AnchoTotal => Columnas * _destino;
    public int AltoTotal => Filas * _destino;

    public void Cargar()
    {
        // Imágenes de Content/Fondos
        CargarCarpeta("Fondos", "Salida_1","Recepcion", "Recepcion_2");

        // Imágenes de Content/Cuarto

        // El mapa se escala para que ocupe todo el alto de la ventana.
        int altoVentana = GLobals.SpriteBatch.GraphicsDevice.Viewport.Height;
        _destino = (int)MathF.Ceiling(altoVentana / (float)Filas);
    }

    private void CargarCarpeta(string carpeta, params string[] nombres)
    {
        foreach (var nombre in nombres)
            _tiles.Add(GLobals.Content.Load<Texture2D>($"{carpeta}/{nombre}"));
    }

    // Centra la cámara en una posición X del mapa sin salirse de los bordes.
    public void SeguirA(float x)
    {
        int anchoVentana = GLobals.SpriteBatch.GraphicsDevice.Viewport.Width;
        float max = Math.Max(0, AnchoTotal - anchoVentana);
        Camara = MathHelper.Clamp(x - anchoVentana / 2f, 0, max);
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
