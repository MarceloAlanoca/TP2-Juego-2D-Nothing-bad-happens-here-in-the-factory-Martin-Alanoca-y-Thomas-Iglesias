using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using Nothing_bad_happens_here_in_the_factory.Models;
using MonoGameGum;
using Gum.Forms.Controls;
using Microsoft.Xna.Framework.Content;

namespace Nothing_bad_happens_here_in_the_factory._Managers;

// Controla todo el flujo del juego: menú, partida, victoria y derrota.
public class GameManager
{
    private const float TiempoTotal = 60f;   // segundos para completar el juego

    // ---------------- Pantallas ----------------
    private InitGame _inicio;                // pantalla de inicio (menú)
    private PantallaVictoria _victoria;      // pantalla al ganar
    private PantallaDerrota _derrota;        // pantalla al perder
    public TileMap _mapa;                    // mapa del escenario actual
    // private readonly DetectionManager _detection = new();

    // ---------------- Temporizador ----------------
    private float _tiempoRestante;           // segundos que quedan
    private Label _labelTiempo;              // texto del temporizador (Gum)

    private Texture2D _pixel;                // textura blanca de 1x1 para dibujar fondos y líneas
    // private Guard _guard;
    private Player _player;

    // ---------------- Llave de la oficina ----------------
    private Texture2D llaveOficina;
    private bool _tieneLlave = false;        // true cuando ya la recogimos

    // ---------------- Nota del vestuario ----------------
    private Texture2D nota;
    private bool notaVista;                  // ya se leyó (desaparece del mapa)
    private bool notaActiva;                 // se está mostrando en pantalla ahora

    // Puerta al jardín (se crea al usar el escritorio).
    private bool _puertaOficinaAbierta;
    private readonly Rectangle _rectPuertaOficina = new Rectangle(40, 240, 118, 257);

    private juegoCables _cables;             // minijuego de cables de la caja de fusibles
    private Scene _escenario;                // escenario actual (PlantaPB, SalaMaq, etc.)
    private readonly Dictionary<string, BotonE> _botones = new();   // un aviso "E" por puerta o zona
    private string _puertaUsada;             // puerta apretada en este frame (se procesa al final)
    private string _puertaOrigen;            // "maquina", "vestuario" o "jardin": por dónde entramos
    private float _bloqueoPuertas;           // tiempo en que no se pueden usar puertas

    // private bool _jugadorDetectadoAnteriormente = false;
    // private bool _colisionAnterior = false;

    private bool _cajaAbierta = false;       // true mientras se ve la caja de fusibles
    private KeyboardState _tecladoAnterior;  // teclado del frame anterior (para detectar pulsaciones)

    // Estados posibles del juego.
    private enum Estado { Menu, Juego, Victoria, Derrota }
    private Estado _estado = Estado.Menu;

    // Se llama una sola vez al abrir el juego: crea las pantallas y el temporizador.
    public void Init()
    {
        _inicio = new InitGame();
        _inicio.Init();
        _inicio.OnIniciar = IniciarJuego;        // botón "Iniciar"

        _victoria = new PantallaVictoria();
        _victoria.OnRejugar = IniciarJuego;      // botón "Volver a jugar"

        _derrota = new PantallaDerrota();
        _derrota.OnRejugar = IniciarJuego;       // botón "Volver a jugar"

        // Texto del temporizador, arriba a la izquierda.
        _labelTiempo = new Label();
        _labelTiempo.Visual.AddToRoot();
        _labelTiempo.Visual.X = 10;
        _labelTiempo.Visual.Y = 10;
        _labelTiempo.Text = "1:00";
        _labelTiempo.Visual.Visible = false;     // arranca oculto
    }

    // Empieza (o reinicia) la partida desde cero.
    private void IniciarJuego()
    {
        // Ocultar las pantallas y los avisos de partidas anteriores.
        _inicio.Ocultar();
        _victoria.Ocultar();
        _derrota.Ocultar();

        LimpiarAvisos();

        // Escenario inicial.
        _escenario = new SalaMaq();
        _escenario.Cargar();
        _mapa = _escenario.Mapa;

        _player = new Player(new Vector2(200, 440), _mapa);
        _cables = new juegoCables();
        llaveOficina = GLobals.Content.Load<Texture2D>("Assets/Llave_oficina");
        nota = GLobals.Content.Load<Texture2D>("Assets/nota");

        // Reiniciar el progreso.
        _tieneLlave = false;
        notaVista = false;
        notaActiva = false;
        _puertaOficinaAbierta = false;

        // var waypoints = new List<Vector2>
        // {
        //     new Vector2(100, 400),
        //     new Vector2(200, 400),
        //     new Vector2(300, 400),
        //     new Vector2(400, 400),
        //     new Vector2(500, 400),
        //     new Vector2(600, 400)
        // };
        //
        // _guard = new Guard(waypoints)
        // {
        //     Scale = 4.5f
        // };
        //
        // _jugadorDetectadoAnteriormente = false;
        // _colisionAnterior = false;

        _puertaUsada = null;
        _puertaOrigen = "maquina";
        _bloqueoPuertas = 0.5f;
        _cajaAbierta = false;

        // Arrancar el temporizador.
        _tiempoRestante = TiempoTotal;
        _labelTiempo.Visual.Visible = true;

        _estado = Estado.Juego;
    }

    // Ocultar todos los avisos E.
    private void LimpiarAvisos()
    {
        foreach (var b in _botones.Values)
        {
            b.Update(false, Rectangle.Empty, 0);
        }
    }

    // Se llama al cruzar la puerta "fin" del jardín.
    private void Ganar()
    {
        LimpiarAvisos();
        _cajaAbierta = false;
        notaActiva = false;
        _labelTiempo.Visual.Visible = false;
        _estado = Estado.Victoria;
        _victoria.Mostrar();
    }

    // Se llama cuando el temporizador llega a 0.
    private void Perder()
    {
        LimpiarAvisos();
        _cajaAbierta = false;
        notaActiva = false;
        _labelTiempo.Visual.Visible = false;
        _estado = Estado.Derrota;
        _derrota.Mostrar();
    }

    public void Update(GameTime gt)
    {
        InputManager.Update();
        Gum.GumService.Default.Update(gt);

        KeyboardState teclado = Keyboard.GetState();

        // Temporizador: sigue corriendo aunque haya un minijuego o la nota abiertos.
        if (_estado == Estado.Juego)
        {
            _tiempoRestante -= (float)gt.ElapsedGameTime.TotalSeconds;

            // Se acabó el tiempo: derrota.
            if (_tiempoRestante <= 0)
            {
                _tiempoRestante = 0;
                Perder();
                _tecladoAnterior = teclado;
                return;
            }

            // Mostrar el tiempo como minutos:segundos (se redondea hacia arriba).
            int segundos = (int)MathF.Ceiling(_tiempoRestante);
            _labelTiempo.Text = $"{segundos / 60}:{segundos % 60:00}";
        }

        // Pausar el juego mientras la caja está abierta.
        if (_cajaAbierta)
        {
            bool presionoEnter =
                teclado.IsKeyDown(Keys.Enter) &&
                _tecladoAnterior.IsKeyUp(Keys.Enter);

            // Enter cierra la caja; si no, se sigue jugando a los cables.
            if (presionoEnter)
            {
                _cajaAbierta = false;
                _cables.Cerrar();
            }
            else
            {
                _cables.Update(teclado, _tecladoAnterior);
            }

            // Al completar los cables se elimina el muro y se cierra la caja.
            if (_cables.JuegoCompletado)
            {
                if (_escenario is SalaMaq s) s.eliminarMuro();
                _cajaAbierta = false;
                _cables.Cerrar();
            }

            _tecladoAnterior = teclado;
            return;
        }

        // Pausar el juego mientras se lee la nota (Enter la cierra).
        if (notaActiva)
        {
            bool presionoEnter =
                teclado.IsKeyDown(Keys.Enter) &&
                _tecladoAnterior.IsKeyUp(Keys.Enter);

            if (presionoEnter)
            {
                notaActiva = false;
            }

            _tecladoAnterior = teclado;
            return;
        }

        // En el menú, la victoria y la derrota solo se actualiza Gum.
        if (_estado != Estado.Juego)
        {
            _tecladoAnterior = teclado;
            return;
        }

        // Descontar el bloqueo de puertas.
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
            // Crear el aviso la primera vez que se ve la puerta.
            if (!_botones.TryGetValue(nombre, out var boton))
            {
                _botones[nombre] = boton = new BotonE();
            }

            bool apretada = ActualizarBoton(boton, rect);

            if (apretada && _bloqueoPuertas <= 0 && _puertaUsada == null)
            {
                // El vestuario está cerrado hasta tener la llave.
                if (nombre == "vestuario" && !_tieneLlave)
                {
                    Console.WriteLine("La puerta está cerrada con llave.");
                }
                else
                {
                    _puertaUsada = nombre;
                }
            }
        }

        // Avisos E de las zonas.
        foreach (var (nombre, rect) in _mapa.Zonas)
        {
            // Crear el aviso la primera vez que se ve la zona.
            if (!_botones.TryGetValue(nombre, out var boton))
            {
                _botones[nombre] = boton = new BotonE();
            }

            // Si ya se usó el objeto, no mostrar más su aviso.
            if ((nombre == "llave" && _tieneLlave) ||
                (nombre == "nota" && notaVista) ||
                (nombre == "escritorio" && _puertaOficinaAbierta))
            {
                boton.Update(false, Rectangle.Empty, 0);
                continue;
            }

            bool intersecta = _player.Hitbox.Intersecta(rect);

            boton.Update(intersecta, rect, _mapa.Camara);
        }

        // Detectar si el jugador presionó E (solo en el frame en que se aprieta).
        bool presionoE =
            teclado.IsKeyDown(Keys.E) &&
            _tecladoAnterior.IsKeyUp(Keys.E);

        // Abrir la caja de fusibles.
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

        // Recoger la llave de la oficina.
        if (_escenario is SalaMaq salaLlave &&
            !_tieneLlave &&
            presionoE &&
            _player.Hitbox.Intersecta(salaLlave.LlaveRect))
        {
            _tieneLlave = true;

            if (_botones.TryGetValue("llave", out var botonLlave))
            {
                botonLlave.Update(false, Rectangle.Empty, 0);
            }
        }

        // Leer la nota del vestuario.
        if (_escenario is Vestuarios ves &&
            !notaVista &&
            presionoE &&
            _player.Hitbox.Intersecta(ves.notaRec))
        {
            notaVista = true;
            notaActiva = true;

            if (_botones.TryGetValue("nota", out var botonNota))
            {
                botonNota.Update(false, Rectangle.Empty, 0);
            }
        }

        // Usar el escritorio: abre la puerta al jardín a la izquierda.
        if (_escenario is PlantaPB &&
            !_puertaOficinaAbierta &&
            presionoE &&
            _mapa.Zonas.TryGetValue("escritorio", out Rectangle zonaEscritorio) &&
            _player.Hitbox.Intersecta(zonaEscritorio))
        {
            _puertaOficinaAbierta = true;
            _mapa.AgregarPuerta("jardin", _rectPuertaOficina);

            if (_botones.TryGetValue("escritorio", out var botonEscritorio))
            {
                botonEscritorio.Update(false, Rectangle.Empty, 0);
            }
        }

        // // Comprobar si el jugador está dentro del cono de visión.
        // bool detectado = _detection.DentroDelCono(
        //     _guard.Vision,
        //     _player.Hitbox
        // );
        //
        // // Actualizar guardia.
        // _guard.Update(
        //     gt,
        //     _player.Hitbox.Centro,
        //     detectado
        // );
        //
        // // Detectar colisión entre jugador y guardia.
        // bool colision = _detection.HayColision(
        //     _player.Hitbox,
        //     _guard.Hitbox
        // );
        //
        // if (colision && !_colisionAnterior)
        // {
        //     Console.WriteLine("¡El jugador ha chocado con el guardia!");
        // }
        //
        // _colisionAnterior = colision;
        //
        // // Mostrar mensajes cuando cambia el estado de detección.
        // if (detectado && !_jugadorDetectadoAnteriormente)
        // {
        //     Console.WriteLine("¡El guardia ha detectado al jugador!");
        //     _guard._speed = 150f;
        // }
        // else if (!detectado && _jugadorDetectadoAnteriormente)
        // {
        //     Console.WriteLine("El guardia ha perdido de vista al jugador.");
        //     _guard._speed = 100f;
        // }
        //
        // _jugadorDetectadoAnteriormente = detectado;

        // Cambiar de escenario si se apretó una puerta.
        if (_puertaUsada != null)
        {
            string puerta = _puertaUsada;
            _puertaUsada = null;
            Entrar(puerta);
        }

        // Guardar el estado anterior del teclado.
        _tecladoAnterior = teclado;
    }

    // Mostrar botón cerca de una puerta. Devuelve true si se apretó E.
    private bool ActualizarBoton(BotonE boton, Rectangle puerta)
    {
        // La zona de aviso es 20 píxeles más ancha que la puerta a cada lado.
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
                _puertaOrigen = "maquina";
                CambiarEscenario(new SalaMaq(), new Vector2(400, 440));
                break;

            case "vestuario":
                _puertaOrigen = "vestuario";
                CambiarEscenario(new Vestuarios(), new Vector2(400, 440));
                break;

            case "jardin":
                _puertaOrigen = "jardin";
                CambiarEscenario(new Jardin(), new Vector2(400, 440));
                break;

            case "fin":
                // Puerta final del jardín: se gana la partida.
                Ganar();
                break;

            case "salida":
                // Volver a la planta, parado en la puerta de la que venías.
                CambiarEscenario(new PlantaPB(notaVista), new Vector2(200, 440), _puertaOrigen);
                break;
        }
    }

    // Cambiar de escenario.
    private void CambiarEscenario(Scene nuevo, Vector2 posicionJugador, string puertaAparicion = null)
    {
        LimpiarAvisos();

        _escenario = nuevo;
        _escenario.Cargar();
        _mapa = _escenario.Mapa;

        // Si la puerta al jardín ya fue abierta, volver a crearla en la planta
        // (la planta se crea de cero cada vez que se entra).
        if (_escenario is PlantaPB && _puertaOficinaAbierta)
        {
            _mapa.AgregarPuerta("jardin", _rectPuertaOficina);
        }

        // Si hay una puerta de destino, aparecer frente a ella.
        if (puertaAparicion != null &&
            _mapa.Puertas.TryGetValue(puertaAparicion, out Rectangle puerta))
        {
            posicionJugador = new Vector2(puerta.Center.X - 40, 440);
        }

        _player.IrA(_mapa, posicionJugador);

        _bloqueoPuertas = 0.5f;
        _cajaAbierta = false;
    }

    public void Draw(GameTime gt)
    {
        // Pantallas completas: solo se dibuja su fondo (los botones los dibuja Gum).
        if (_estado == Estado.Menu)
        {
            _inicio.Draw();
            return;
        }

        if (_estado == Estado.Victoria)
        {
            _victoria.Draw();
            return;
        }

        if (_estado == Estado.Derrota)
        {
            _derrota.Draw();
            return;
        }

        // Crear la textura blanca de 1x1 la primera vez.
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

        // Dibujar la nota del vestuario si todavía no fue leída.
        if (_escenario is Vestuarios vest && !notaVista)
        {
            sb.Draw(nota, new Rectangle(550, 340, 35, 50), Color.White);
        }

        // Dibujar la llave de la oficina si todavía no fue recogida.
        if (_escenario is SalaMaq salaDibujo && !_tieneLlave)
        {
            sb.Draw(llaveOficina, salaDibujo.LlaveRect, Color.White);
        }

        _player.Draw();
        // _guard.Draw(gt);

        // Terminar el dibujo con la cámara.
        sb.End();

        // Dibujar interfaz sin transformación de cámara.
        sb.Begin(samplerState: SamplerState.PointClamp);

        // Mostrar la llave en la esquina cuando ya la tenemos.
        // Está más abajo que el temporizador para que no se pisen.
        if (_tieneLlave)
        {
            sb.Draw(llaveOficina, new Rectangle(0, 40, 100, 170), Color.White);
        }

        // Caja de fusibles con el minijuego de cables.
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
            _cables.Draw(sb, _pixel);
        }

        // Nota del vestuario ampliada.
        if (notaActiva && _escenario is Vestuarios v)
        {
            // Fondo oscuro detrás de la nota.
            sb.Draw(
                _pixel,
                sb.GraphicsDevice.Viewport.Bounds,
                Color.Black * 0.65f
            );

            v.DibujarNota(
                sb,
                sb.GraphicsDevice.Viewport
            );
        }

        // Dejar SpriteBatch preparado para Game1.
        sb.End();
        sb.Begin(samplerState: SamplerState.PointClamp);
    }
}