using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using Nothing_bad_happens_here_in_the_factory.Models;
using MonoGameGum;
using Gum.Forms.Controls;

namespace Nothing_bad_happens_here_in_the_factory._Managers;

public class GameManager
{
    private InitGame _inicio;
    private TileMap _mapa;
    private readonly DetectionManager _detection = new();

    private Texture2D _pixel;
    private Guard _guard;
    private Player _player;

    private bool _jugadorDetectadoAnteriormente = false;
    private bool _colisionAnterior = false;

    private enum Estado { Menu, Juego }
    private Estado _estado = Estado.Menu;

    public void Init()
    {
        // Al abrir el juego solo se crea el menú. Todo lo demás espera al botón.
        _inicio = new InitGame();
        _inicio.Init();   // crea el botón

        _inicio.OnIniciar = IniciarJuego;
    }

    private void IniciarJuego()
    {
        _inicio.Ocultar();

        _mapa = new TileMap();
        _mapa.Cargar();
        _player = new Player(new Vector2(200, 440));

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

        _jugadorDetectadoAnteriormente = false;
        _colisionAnterior = false;

        _estado = Estado.Juego;
    }

    public void Update(GameTime gt)
    {
        InputManager.Update();
         Gum.GumService.Default.Update(gt);

        // En el menú solo se actualiza Gum (el botón); la lógica del juego espera.
        if (_estado != Estado.Juego) return;

        // Actualizar al jugador.
        _player.Update(gt);

        // La cámara sigue al jugador a lo largo del mapa horizontal.
        _mapa.SeguirA(_player.Hitbox.Centro.X);

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
        if (_estado == Estado.Menu)
        {
            _inicio.Draw();
            return;
        }

        if (_pixel == null)
        {
            _pixel = new Texture2D(
                GLobals.SpriteBatch.GraphicsDevice,
                1,
                1
            );

            _pixel.SetData(new[] { Color.White });
        }

        // El mapa ya se dibuja restando la cámara.
        _mapa.Draw();

        // Personajes: se reinicia el SpriteBatch con la cámara aplicada,
        // así Player y Guard siguen usando sus coordenadas del mundo.
        var sb = GLobals.SpriteBatch;
        sb.End();
        sb.Begin(
            samplerState: SamplerState.PointClamp,
            transformMatrix: Matrix.CreateTranslation(-_mapa.Camara, 0, 0)
        );

        _player.Draw();
        _guard.Draw(gt);

        // Dibujar hitboxes.
        //_player.Hitbox.Dibujar(_pixel, Color.Lime);
        //_guard.Hitbox.Dibujar(_pixel, Color.Blue);

        // Dibujar el cono de visión.
        //_guard.Vision.Dibujar(_pixel, Color.Red);

        // Se vuelve al Begin normal para que el End de Game1 siga funcionando.
        sb.End();
        sb.Begin(samplerState: SamplerState.PointClamp);
    }
}