using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nothing_bad_happens_here_in_the_factory.Models;

namespace Nothing_bad_happens_here_in_the_factory;

public class juegoCables
{
    private const int Filas = 6;
    private const int Columnas = 2;

    private Rectangle[] cables = new Rectangle[12];

    private readonly BotonE _botonE = new();
   public bool JuegoCompletado { get; private set; }=false;

    private Rectangle _posicionBoton;

    private int primerCable = -1;

    private List<(int, int)> conexiones = new List<(int, int)>();

    private bool[] conectados = new bool[12];

    // 0 = rojo, 1 = amarillo, 2 = verde
    // 3 = azul, 4 = violeta, 5 = naranja
    private int[] colores =
    {
        0, 1, 2, 3, 4, 5,
        4, 5, 0, 1, 2, 3
    };

    private Color[] coloresLinea =
    {
        Color.Red,
        Color.Yellow,
        Color.Green,
        Color.Blue,
        Color.Purple,
        Color.Orange
    };

    public int CableSeleccionado { get; private set; }

    public juegoCables()
    {
        cables[0] = new Rectangle(295, 210, 1, 1);
        cables[1] = new Rectangle(295, 250, 1, 1);
        cables[2] = new Rectangle(295, 295, 1, 1);
        cables[3] = new Rectangle(295, 340, 1, 1);
        cables[4] = new Rectangle(295, 385, 1, 1);
        cables[5] = new Rectangle(295, 425, 1, 1);

        for (int i = 6; i < 12; i++)
        {
            cables[i] = new Rectangle(464, cables[i - 6].Y, 1, 1);
        }

        _posicionBoton = cables[0];
    }

    public void Abrir()
    {
        CableSeleccionado = 0;
        primerCable = -1;

        _botonE.Update(true, cables[0]);
    }

    public void Cerrar()
    {
        _botonE.Update(false, _posicionBoton);
    }
    private void ComprobarJuego()
{
    for (int i = 0; i < conectados.Length; i++)
    {
        if (conectados[i] == false)
        {
            JuegoCompletado = false;
            return;
        }
    }

    JuegoCompletado = true;
}

    public void Update(KeyboardState actual, KeyboardState anterior)
    {
        bool Apretada(Keys k) =>
            actual.IsKeyDown(k) && anterior.IsKeyUp(k);

        int fila = CableSeleccionado % Filas;
        int col = CableSeleccionado / Filas;

        if (Apretada(Keys.Down))
            fila = Math.Min(fila + 1, Filas - 1);

        if (Apretada(Keys.Up))
            fila = Math.Max(fila - 1, 0);

        if (Apretada(Keys.Right))
            col = Math.Min(col + 1, Columnas - 1);

        if (Apretada(Keys.Left))
            col = Math.Max(col - 1, 0);

        CableSeleccionado = col * Filas + fila;

        _posicionBoton = cables[CableSeleccionado];

        _botonE.Update(true, _posicionBoton);

        // Seleccionar y conectar cables con E.
        if (Apretada(Keys.E))
        {
            if (primerCable == -1)
            {
                if (!conectados[CableSeleccionado])
                {
                    primerCable = CableSeleccionado;
                }
            }
            else
            {
                bool distintoLado =
                    (primerCable < 6 && CableSeleccionado >= 6) ||
                    (primerCable >= 6 && CableSeleccionado < 6);

                bool mismoColor =
                    colores[primerCable] == colores[CableSeleccionado];

                bool disponible =
                    !conectados[CableSeleccionado];

                if (distintoLado && mismoColor && disponible)
                {
                    conexiones.Add((primerCable, CableSeleccionado));

                    conectados[primerCable] = true;
                    conectados[CableSeleccionado] = true;
                    ComprobarJuego();
                }

                primerCable = -1;
            }
        }
    }

    public void Draw(SpriteBatch sb, Texture2D pixel)
    {
        foreach (var conexion in conexiones)
        {
            Vector2 inicio = new Vector2(
                cables[conexion.Item1].X,
                cables[conexion.Item1].Y
            );

            Vector2 final = new Vector2(
                cables[conexion.Item2].X,
                cables[conexion.Item2].Y
            );

            Vector2 diferencia = final - inicio;

            float longitud = diferencia.Length();

            float rotacion = (float)Math.Atan2(
                diferencia.Y,
                diferencia.X
            );

            sb.Draw(
                pixel,
                inicio,
                null,
                coloresLinea[colores[conexion.Item1]],
                rotacion,
                Vector2.Zero,
                new Vector2(longitud, 4),
                SpriteEffects.None,
                0
            );
        }
    }
}
