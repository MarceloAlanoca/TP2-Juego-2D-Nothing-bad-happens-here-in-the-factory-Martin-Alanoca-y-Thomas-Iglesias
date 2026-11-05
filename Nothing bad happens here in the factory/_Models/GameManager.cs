
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using Nothing_bad_happens_here_in_the_factory.Models;
using MonoGameGum;
using Gum.Forms.Controls;

namespace Nothing_bad_happens_here_in_the_factory._Managers;

public class GameManager
{
    private InitGame _inicio;
    public TileMap _mapa;
    private readonly DetectionManager _detection = new();

    private Texture2D _pixel;
    private Guard _guard;
    private Player _player;

    private Scene _escenario;
    private readonly Dictionary<string, BotonE> _botones = new();
    private string _puertaUsada;
    private float _bloqueoPuertas;

    private bool _jugadorDetectadoAnteriormente = false;
    private bool _colisionAnterior = false;

    private bool _cajaAbierta = false;
    private KeyboardState _tecladoAnterior;

    private enum Estado { Menu, Juego }
    private Estado _estado = Estado.Menu;

    public void Init()
    {
        _inicio = new InitGame();
        _inicio.Init();

        _inicio.OnIniciar = IniciarJuego;
    }

    private void IniciarJuego()
    {
        _inicio.Ocultar();

        _escenario = new PlantaPB();
        _escenario.Cargar();
        _mapa = _escenario.Mapa;

        _player = new Player(new Vector2(200, 440), _mapa);

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
        _puertaUsada = null;
        _bloqueoPuertas = 0.5f;
        _cajaAbierta = false;

        _estado = Estado.Juego;
    }

    public void Update(GameTime gt)
    {
        InputManager.Update();
        Gum.GumService.Default.Update(gt);

        KeyboardState teclado = Keyboard.GetState();

        // Pausar el juego mientras la caja está abierta.
        if (_cajaAbierta)
        {
            bool presionoEnter =
                teclado.IsKeyDown(Keys.Enter) &&
                _tecladoAnterior.IsKeyUp(Keys.Enter);

            if (presionoEnter)
            {
                _cajaAbierta = false;
            }

            _tecladoAnterior = teclado;
            return;
        }

        // En el menú solo se actualiza Gum.
        if (_estado != Estado.Juego)
        {
            _tecladoAnterior = teclado;
            return;
        }

        if (_bloqueoPuertas > 0)
        {
            _bloqueoPuertas -= (float)gt.ElapsedGameTime.TotalSeconds;
        }

        // Actualizar al jugador.
        _player.Update(gt);

        // La cámara sigue al jugador.
        _mapa.SeguirA(_player.Hitbox.Centro.X);

        // Avisos E de las puertas.
        foreach (var (nombre, rect) in _mapa.Puertas)
        {
            if (!_botones.TryGetValue(nombre, out var boton))
            {
                _botones[nombre] = boton = new BotonE();
            }

            bool apretada = ActualizarBoton(boton, rect);

            if (apretada && _bloqueoPuertas <= 0 && _puertaUsada == null)
            {
                _puertaUsada = nombre;
            }
        }

        // Avisos E de las zonas.
        foreach (var (nombre, rect) in _mapa.Zonas)
        {
            if (!_botones.TryGetValue(nombre, out var boton))
            {
                _botones[nombre] = boton = new BotonE();
            }

            bool intersecta = _player.Hitbox.Intersecta(rect);

            boton.Update(intersecta, rect, _mapa.Camara);
        }

        // Detectar si el jugador puede abrir la caja de fusibles.
        bool presionoE =
            teclado.IsKeyDown(Keys.E) &&
            _tecladoAnterior.IsKeyUp(Keys.E);

        if (_escenario is SalaMaq sala &&
            sala.Mapa.Zonas.TryGetValue("cables", out Rectangle zonaCables))
        {
            bool dentroZona = _player.Hitbox.Intersecta(zonaCables);

            if (dentroZona && presionoE)
            {
               _cajaAbierta = true;

             if (_botones.TryGetValue("cables", out var boton))
                {
                    boton.Update(false, Rectangle.Empty, 0);
                }
            }
        }

        // Comprobar si el jugador está dentro del cono de visión.
        bool detectado = _detection.DentroDelCono(
            _guard.Vision,
            _player.Hitbox
        );

        // Actualizar guardia.
        _guard.Update(
            gt,
            _player.Hitbox.Centro,
            detectado
        );

        // Detectar colisión entre jugador y guardia.
        bool colision = _detection.HayColision(
            _player.Hitbox,
            _guard.Hitbox
        );

        if (colision && !_colisionAnterior)
        {
            Console.WriteLine("¡El jugador ha chocado con el guardia!");
        }

        _colisionAnterior = colision;

        // Mostrar mensajes cuando cambia el estado de detección.
        if (detectado && !_jugadorDetectadoAnteriormente)
        {
            Console.WriteLine("¡El guardia ha detectado al jugador!");
            _guard._speed = 150f;
        }
        else if (!detectado && _jugadorDetectadoAnteriormente)
        {
            Console.WriteLine("El guardia ha perdido de vista al jugador.");
            _guard._speed = 100f;
        }

        _jugadorDetectadoAnteriormente = detectado;

        // Cambiar de escenario.
        if (_puertaUsada != null)
        {
            string puerta = _puertaUsada;
            _puertaUsada = null;
            Entrar(puerta);
        }

        // Guardar el estado anterior del teclado.
        _tecladoAnterior = teclado;
    }

    // Mostrar botón cerca de una puerta.
    private bool ActualizarBoton(BotonE boton, Rectangle puerta)
    {
        var zona = puerta;
        zona.Inflate(20, 0);

        boton.Update(
            _player.Hitbox.Intersecta(zona),
            puerta,
            _mapa.Camara
        );

        return boton.Interactuar;
    }

    // Qué escenario abre cada puerta.
    private void Entrar(string puerta)
    {
        switch (puerta)
        {
            case "maquina":
                CambiarEscenario(new SalaMaq(), new Vector2(400, 440));
                break;

            case "vestuario":
                CambiarEscenario(new Vestuarios(), new Vector2(400, 440));
                break;

            case "salida":
                CambiarEscenario(new PlantaPB(), new Vector2(200, 440));
                break;
        }
    }

    // Cambiar de escenario.
    private void CambiarEscenario(Scene nuevo, Vector2 posicionJugador)
    {
        foreach (var b in _botones.Values)
        {
            b.Update(false, Rectangle.Empty, 0);
        }

        _escenario = nuevo;
        _escenario.Cargar();
        _mapa = _escenario.Mapa;

        _player.IrA(_mapa, posicionJugador);

        _bloqueoPuertas = 0.5f;
        _cajaAbierta = false;
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

        // Dibujar mapa.
        _mapa.Draw();

        var sb = GLobals.SpriteBatch;

        // Dibujar personajes con la cámara.
        sb.End();

        sb.Begin(
            samplerState: SamplerState.PointClamp,
            transformMatrix: Matrix.CreateTranslation(-_mapa.Camara, 0, 0)
        );

        _player.Draw();
        _guard.Draw(gt);

        // Terminar el dibujo con la cámara.
        sb.End();

        // Dibujar interfaz sin transformación de cámara.
        sb.Begin(samplerState: SamplerState.PointClamp);

        if (_cajaAbierta && _escenario is SalaMaq sala)
        {
            // Fondo oscuro detrás de la caja.
            sb.Draw(
                _pixel,
                sb.GraphicsDevice.Viewport.Bounds,
                Color.Black * 0.65f
            );

            // Caja centrada en la pantalla.
            sala.DibujarCajaFusibles(
                sb,
                sb.GraphicsDevice.Viewport
            );
        }

        // Dejar SpriteBatch preparado para Game1.
        sb.End();
        sb.Begin(samplerState: SamplerState.PointClamp);
    }
}
