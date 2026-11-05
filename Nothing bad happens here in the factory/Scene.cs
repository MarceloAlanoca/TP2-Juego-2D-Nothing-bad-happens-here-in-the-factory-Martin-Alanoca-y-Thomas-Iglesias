using Microsoft.Xna.Framework.Graphics;
namespace Nothing_bad_happens_here_in_the_factory;

public abstract class Scene
{
    public TileMap Mapa { get; } = new();
    public abstract void Cargar();
}