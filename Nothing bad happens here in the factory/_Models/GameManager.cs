
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using Nothing_bad_happens_here_in_the_factory.Models;

namespace Nothing_bad_happens_here_in_the_factory._Managers;

public class GameManager
{
    private Jardin _jardin;
    private readonly DetectionManager _detection = new();

    private Texture2D _pixel;
    private Guard _guard;
    private Player _player;

    private bool _jugadorDetectadoAnteriormente = false;
    private bool _colisionAnterior = false;

    public void Init()
    {
        _player = new Player(new Vector2(200, 440));
        
        //poniendo los waypoint por donde pasara el guardia
        var waypoints = new List<Vector2>
        {
            new Vector2(100, 400),
            new Vector2(200, 400),
            new Vector2(300, 400),
            new Vector2(400, 400),
            new Vector2(500, 400),
            new Vector2(600, 400)
        };

        _guard = new Guard(waypoints)
        {
            Scale = 4.5f
        };
        _jardin = new Jardin();
    }

    public void Update(GameTime gt)
    {
        InputManager.Update();

        // Actualizar al jugador.
        _player.Update(gt);

        // Comprobar si el jugador está dentro del cono.
        bool detectado = _detection.DentroDelCono(
            _guard.Vision,
            _player.Hitbox
        );

        // Actualizar el guardia: patrulla o persigue.
        _guard.Update(
            gt,
            _player.Hitbox.Centro,
            detectado
        );

        // Detectar colisión física entre ambos.
        bool colision = _detection.HayColision(
            _player.Hitbox,
            _guard.Hitbox
        );

        if (colision && !_colisionAnterior)
        {
            Console.WriteLine(
                "¡El jugador ha chocado con el guardia!"
            );
    
            
        }

        _colisionAnterior = colision;

        // Mostrar mensajes cuando cambia el estado de detección.
        if (detectado && !_jugadorDetectadoAnteriormente)
        {
            Console.WriteLine(
                "¡El guardia ha detectado al jugador!"
            );
            _guard._speed = 150f;
        }
        else if (!detectado && _jugadorDetectadoAnteriormente)
        {
            Console.WriteLine(
                "El guardia ha perdido de vista al jugador."
            );
            _guard._speed = 100f;
        }

        _jugadorDetectadoAnteriormente = detectado;
    }

    public void Draw(GameTime gt)
    {
        _jardin.Draw();
        if (_pixel == null)
        {
            _pixel = new Texture2D(
                GLobals.SpriteBatch.GraphicsDevice,
                1,
                1
            );

            _pixel.SetData(new[] { Color.White });
        }

        // Dibujar personajes.
        _player.Draw();
        _guard.Draw(gt);

        // Dibujar hitboxes.
        _player.Hitbox.Dibujar(_pixel, Color.Lime);
        _guard.Hitbox.Dibujar(_pixel, Color.Blue);

        // Dibujar el cono de visión.
        _guard.Vision.Dibujar(_pixel, Color.Red);
        
    }
}