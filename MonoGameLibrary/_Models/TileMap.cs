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
    private Texture2D _pixel;   
    public int Filas => _mapa.GetLength(0);
    public int Columnas => _mapa.GetLength(1);
    public int AnchoTotal => Columnas * _destino;
    public int AltoTotal => Filas * _destino;
    // Si el mapa es más angosto que la ventana, lo centra.
    public int OffsetX
    {
        get
        {
            int anchoVentana = GLobals.SpriteBatch.GraphicsDevice.Viewport.Width;
            return Math.Max(0, (anchoVentana - AnchoTotal) / 2);
        }
    }

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
        Zonas.Clear();

        _pixel = new Texture2D(GLobals.SpriteBatch.GraphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });
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
    public void eliminarMuro(Rectangle r) => Muros.Remove(r);
    public void AgregarPuerta(string nombre, Rectangle r) => Puertas[nombre] = r;
    public Dictionary<string, Rectangle> Zonas { get; } = new(); //para que se pueda poner nombre a cada zona interactuable
    public void AgregarZona(string nombre, Rectangle r) => Zonas[nombre] = r;

    // Centra la cámara en una posición X del mapa sin salirse de los bordes.
    public void SeguirA(float x)
    {
        int anchoVentana = GLobals.SpriteBatch.GraphicsDevice.Viewport.Width;
        float max = Math.Max(0, AnchoTotal - anchoVentana);
        Camara = MathF.Round(MathHelper.Clamp(x - anchoVentana / 2f, 0, max));
    }

    public void DibujarColisiones()
{
    var sb = GLobals.SpriteBatch;
    int cam = (int)Camara;

    foreach (var m in Muros)
    {
        var r = m;
        r.X = r.X - cam + OffsetX;
        sb.Draw(_pixel, r, Color.Red * 0.4f);
    }
    foreach (var p in Puertas.Values)
    {
        var r = p;
        r.X = r.X - cam + OffsetX;
        sb.Draw(_pixel, r, Color.Lime * 0.4f);
    }
        foreach (var z in Zonas.Values)
    {
        var r = z; r.X = r.X - (int)Camara + OffsetX;
        sb.Draw(_pixel, r, Color.Yellow * 0.4f);
    }
}

public void Draw()
{
    int anchoVentana = GLobals.SpriteBatch.GraphicsDevice.Viewport.Width;
    int cam = (int)Camara;
    int offset = OffsetX;

    int desde = Math.Max(0, cam / _destino);
    int hasta = Math.Min(Columnas - 1, (cam + anchoVentana) / _destino);

    for (int fila = 0; fila < Filas; fila++)
    {
        for (int col = desde; col <= hasta; col++)
        {
            int id = _mapa[fila, col];
            if (id == 0) continue;

            var destino = new Rectangle(
                col * _destino - cam + offset,
                fila * _destino,
                _destino + 1,   // 1 px de solape: tapa la línea blanca entre tiles
                _destino + 1);

            GLobals.SpriteBatch.Draw(_tiles[id - 1], destino, Color.White);
        }
    }
}
}