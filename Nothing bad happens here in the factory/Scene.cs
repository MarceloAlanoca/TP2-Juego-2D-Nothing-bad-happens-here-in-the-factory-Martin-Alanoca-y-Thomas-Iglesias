using Microsoft.Xna.Framework;

namespace Nothing_bad_happens_here_in_the_factory.Scenes;

public abstract class Scene
{
    public abstract void Init();

    public abstract void Update(GameTime gameTime);

    public abstract void Draw(GameTime gameTime);
}