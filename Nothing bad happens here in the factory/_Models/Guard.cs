
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;

namespace Nothing_bad_happens_here_in_the_factory;

public class Guard
{
    public float Scale = 1f;
    public Vector2 _position;
    public float _speed = 100f;
    public bool Loop = true;

    public CuadradoColision Hitbox =
        new CuadradoColision(
            new Vector2(4, 2),
            new Vector2(26, 33)
        );

    public ConoColision Vision =
        new ConoColision(300f, 30f);

    public bool Persiguiendo { get; private set; }

    private readonly AnimationManager _anims = new();
    private readonly List<Vector2> _waypoints;
    private int _target = 0;
    private float _dirX = 1f;

    public Guard(List<Vector2> waypoints)
    {
        var sheet = GLobals.Content.Load<Texture2D>("Assets/Guard");

        var strip = BuildStrip(sheet,
            new Rectangle(27, 55, 29, 35),
            new Rectangle(61, 55, 34, 35),
            new Rectangle(104, 55, 31, 35),
            new Rectangle(140, 55, 26, 35));

        var anim = new Animation(strip, 4, 1, 0.2f);

        _anims.AddAnimation(new Vector2(-1, 0), anim);
        _anims.AddAnimation(new Vector2(1, 0), anim);

        _waypoints = waypoints;

        if (_waypoints.Count > 0)
            _position = _waypoints[0];

        _target = _waypoints.Count > 1 ? 1 : 0;

        Hitbox.Escala = Scale;
        Hitbox.Actualizar(_position);

        Vision.Actualizar(
            Hitbox.Centro,
            new Vector2(_dirX, 0)
        );
    }

    public void Update(
        GameTime gameTime,
        Vector2 playerPosition,
        bool jugadorVisible)
    {
        float step = _speed * GLobals.TotalSeconds;

        Persiguiendo = jugadorVisible;

        if (Persiguiendo)
        {
            // Perseguir al jugador en X e Y.
            // playerPosition es el centro de la hitbox del jugador.
            Vector2 direccion = playerPosition - Hitbox.Centro;
            float distancia = direccion.Length();

            if (distancia > 1f)
            {
                direccion.Normalize();

                float movimiento = System.Math.Min(step, distancia);

                _position += direccion * movimiento;

                if (direccion.X > 0.01f)
                    _dirX = 1f;
                else if (direccion.X < -0.01f)
                    _dirX = -1f;
            }
        }
        else
        {
            // Si no ve al jugador, sigue su patrulla.
            Patrullar(step);
        }

        // Actualizar la hitbox después del movimiento.
        Hitbox.Escala = Scale;
        Hitbox.Actualizar(_position);

        // Orientar el cono hacia el jugador durante la persecución.
        Vector2 direccionVision;

        if (Persiguiendo)
        {
            direccionVision = playerPosition - Hitbox.Centro;
        }
        else
        {
            direccionVision = new Vector2(_dirX, 0);
        }

        Vision.Actualizar(
            Hitbox.Centro,
            direccionVision
        );

        _anims.Update(new Vector2(_dirX, 0), gameTime);
    }

    private void Patrullar(float step)
    {
        if (_waypoints.Count < 2)
            return;

        Vector2 destino = _waypoints[_target];
        Vector2 direccion = destino - _position;
        float distancia = direccion.Length();

        if (distancia <= step || distancia < 0.001f)
        {
            _position = destino;

            _target++;

            if (_target >= _waypoints.Count)
            {
                if (Loop)
                    _target = 0;
                else
                    _target = _waypoints.Count - 1;
            }
        }
        else
        {
            direccion.Normalize();
            _position += direccion * step;

            if (direccion.X > 0.01f)
                _dirX = 1f;
            else if (direccion.X < -0.01f)
                _dirX = -1f;
        }
    }

    public void Draw(GameTime gt)
    {
        var effects = _dirX < 0
            ? SpriteEffects.FlipHorizontally
            : SpriteEffects.None;

        _anims.Draw(_position, Scale, effects);
    }

    private static Texture2D BuildStrip(
        Texture2D sheet,
        params Rectangle[] frames)
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
            {
                for (int x = 0; x < f.Width; x++)
                {
                    data[y * stripW + offsetX + x] =
                        px[y * f.Width + x];
                }
            }
        }

        var tex = new Texture2D(
            sheet.GraphicsDevice,
            stripW,
            h
        );

        tex.SetData(data);

        return tex;
    }
}