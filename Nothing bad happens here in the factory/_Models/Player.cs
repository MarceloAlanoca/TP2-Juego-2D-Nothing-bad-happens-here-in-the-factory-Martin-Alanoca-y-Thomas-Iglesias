using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using MonoGameLibrary;

namespace Nothing_bad_happens_here_in_the_factory.Models;

public class Player
{
    private DetectionManager _detection = new();
    private Vector2 _position;
    private float _dirX = 1f;
    private readonly float _speed = 200f;
    private readonly AnimationManager _anims = new();
    public CuadradoColision Hitbox = new CuadradoColision(new Vector2(25, 20), new Vector2(60, 20));
    public List<Rectangle> Muros = new();
    public Player(Vector2 position)
    {
        _position = position;

        var sheet = GLobals.Content.Load<Texture2D>("Assets/Jugador");

        // Los 6 frames recortados uno por uno y puestos en celdas del mismo ancho
        var strip = BuildStrip(sheet,
            new Rectangle(68,  4,  96, 144),   // 0
            new Rectangle(190, 4, 110, 144),   // 1
            new Rectangle(334, 4,  78, 144),   // 2
            new Rectangle(450, 4,  96, 144),   // 3
            new Rectangle(334, 4,  78, 144));   // 2

        var anim = new Animation(strip, 5, 1, 0.2f);

        _anims.AddAnimation(new Vector2(-1, 0),  anim);
        _anims.AddAnimation(new Vector2(1, 0),  anim);
    }

    public void Update(GameTime gameTime)
    {
        var anterior = _position;

        if (InputManager.Moving)
            _position += Vector2.Normalize(InputManager.Direction) * _speed * GLobals.TotalSeconds;

        Hitbox.Actualizar(_position);

        foreach (var muro in Muros)
        {
            if (Hitbox.Intersecta(muro))
            {
                _position = anterior;
                Hitbox.Actualizar(_position);
                break;
            }
        }

        if (InputManager.Direction.X > 0.01f) _dirX = 1f;
        else if (InputManager.Direction.X < -0.01f) _dirX = -1f;

        _anims.Update(InputManager.Direction, gameTime);
    }

    public void Draw()
    {
        var effects = _dirX < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

        _anims.Draw(_position, 1f, effects);
    }

    private static Texture2D BuildStrip(Texture2D sheet, params Rectangle[] frames)
    {
        int cellW = frames.Max(f => f.Width);
        int h = frames[0].Height;
        int stripW = cellW * frames.Length;
        var data = new Color[stripW * h];

        for (int i = 0; i < frames.Length; i++)
        {
            var f = frames[i];
            var px = new Color[f.Width * f.Height];
            sheet.GetData(0, f, px, 0, px.Length);

            int offsetX = i * cellW + (cellW - f.Width) / 2;
            for (int y = 0; y < f.Height; y++)
                for (int x = 0; x < f.Width; x++)
                    data[y * stripW + offsetX + x] = px[y * f.Width + x];
        }

        var tex = new Texture2D(sheet.GraphicsDevice, stripW, h);
        tex.SetData(data);
        return tex;
    }
}