using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameGum;
using MonoGameGum.GueDeriving;
using Gum.Managers;
using MonoGameLibrary;

namespace Nothing_bad_happens_here_in_the_factory.Models;

// Aviso "E" hecho con un SpriteRuntime de Gum.
// La sheet (128x64) tiene 2 frames de 64x64: 0 = normal, 1 = presionado.
// Gum dibuja en pantalla (sin cámara), por eso la posición se calcula restando la cámara.
public class BotonE
{
    private const int FrameAncho = 64;
    private const int FrameAlto = 64;
    private const float Escala = 1.5f;

    private readonly SpriteRuntime _sprite;
    private bool _eAnterior;

    public bool Cerca { get; private set; }
    public bool Presionado { get; private set; }     // true mientras se mantiene E
    public bool Interactuar { get; private set; }    // true solo el frame en que se aprieta E

    public BotonE()
    {
        var tex = GLobals.Content.Load<Texture2D>("Assets/Boton/BotonE-Sheet");

        _sprite = new SpriteRuntime();
        _sprite.Texture = tex;
        _sprite.TextureAddress = TextureAddress.Custom;
        _sprite.SourceRectangle = new Rectangle(0, 0, FrameAncho, FrameAlto);
        _sprite.Width = FrameAncho * Escala;
        _sprite.Height = FrameAlto * Escala;
        _sprite.Visible = false;
        _sprite.AddToRoot();
    }

    // cerca: el jugador está en la zona de la puerta. puerta: rectángulo de la puerta en el mundo.
    public void Update(bool cerca, Rectangle puerta, float camaraX)
    {
        Cerca = cerca;
        _sprite.Visible = cerca;

        bool e = Keyboard.GetState().IsKeyDown(Keys.E);
        Presionado = cerca && e;
        Interactuar = Presionado && !_eAnterior;
        _eAnterior = e;

        if (!cerca) return;

        // Mundo -> pantalla: se resta la cámara. Queda centrado sobre la puerta (X e Y).
        _sprite.X = puerta.Center.X - camaraX - _sprite.Width / 2f;
        _sprite.Y = puerta.Center.Y - _sprite.Height / 2f;

        // Frame 0 = normal, frame 1 = presionado (solo mientras se mantiene E).
        int frame = Presionado ? 1 : 0;
        _sprite.SourceRectangle = new Rectangle(frame * FrameAncho, 0, FrameAncho, FrameAlto);
    }
}