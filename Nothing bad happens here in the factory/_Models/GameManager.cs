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
    public TileMap _mapa;
    private readonly DetectionManager _detection = new();

    private Texture2D _pixel;
    private Guard _guard;
    private Player _player;

    private Scene _escenario;
    private readonly Dictionary<string, BotonE> _botones = new();
    private string _puertaUsada;        // se cambia de escenario al final del Update
    private float _bloqueoPuertas;      // segundos sin aceptar E tras cambiar de escenario

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

        _estado = Estado.Juego;
    }

    public void Update(GameTime gt)
    {
        InputManager.Update();
        Gum.GumService.Default.Update(gt);

        // En el menú solo se actualiza Gum (el botón); la lógica del juego espera.
        if (_estado != Estado.Juego) return;

        if (_bloqueoPuertas > 0)
            _bloqueoPuertas -= (float)gt.ElapsedGameTime.TotalSeconds;

        // Actualizar al jugador.
        _player.Update(gt);

        // La cámara sigue al jugador a lo largo del mapa horizontal.
        _mapa.SeguirA(_player.Hitbox.Centro.X);

        // Avisos "E" de Gum, uno por cada puerta del escenario actual.
        foreach (var (nombre, rect) in _mapa.Puertas)
        {
            if (!_botones.TryGetValue(nombre, out var boton))
                _botones[nombre] = boton = new BotonE();

            bool apretada = ActualizarBoton(boton, rect);

            // Solo se acepta si pasó el bloqueo y no hay otra puerta ya elegida.
            if (apretada && _bloqueoPuertas <= 0 && _puertaUsada == null)
                _puertaUsada = nombre;
        }

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

        // Cambio de escenario al final, para no modificar el mapa mientras se recorre.
        if (_puertaUsada != null)
        {
            string puerta = _puertaUsada;
            _puertaUsada = null;
            Entrar(puerta);
        }
    }

    // Muestra el botón cuando el jugador está cerca de la puerta; devuelve true si apretó E.
    private bool ActualizarBoton(BotonE boton, Rectangle puerta)
    {
        var zona = puerta;
        zona.Inflate(20, 0);   // aparece 40 px antes de tocar la puerta

        boton.Update(_player.Hitbox.Intersecta(zona), puerta, _mapa.Camara);
        return boton.Interactuar;
    }

    // Qué escenario abre cada puerta.
    // La posición del jugador queda lejos de la puerta de salida del cuarto nuevo.
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

    private void CambiarEscenario(Scene nuevo, Vector2 posicionJugador)
    {
        // Apaga los avisos "E" del mapa anterior para que no queden en pantalla.
        foreach (var b in _botones.Values)
            b.Update(false, Rectangle.Empty, 0);

        _escenario = nuevo;
        _escenario.Cargar();
        _mapa = _escenario.Mapa;
        _player.IrA(_mapa, posicionJugador);

        _bloqueoPuertas = 0.5f;   // evita que el mismo E active otra puerta enseguida
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

        // Colisiones de muros y puertas (descomentar para verlas).
        //_mapa.DibujarColisiones(_pixel);

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