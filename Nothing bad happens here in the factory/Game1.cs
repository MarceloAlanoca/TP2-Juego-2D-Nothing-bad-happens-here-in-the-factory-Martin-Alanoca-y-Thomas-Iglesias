using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using Nothing_bad_happens_here_in_the_factory._Managers;

namespace Nothing_bad_happens_here_in_the_factory;

public class Game1 : Core
{
    private GameManager _gameManager;

    public Game1() : base("Nothing bad happens here in the factory", 720, 600, false)
    {

    }

    protected override void Initialize()
    {
        base.Initialize();   // aquí se ejecuta LoadContent y luego Core crea el SpriteBatch

        GLobals.SpriteBatch = SpriteBatch;   // ahora sí existe
    }

    protected override void LoadContent()
    {
        GLobals.Content = Content;

        _gameManager = new GameManager();
        _gameManager.Init();

        base.LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        GLobals.Update(gameTime);
        _gameManager.Update(gameTime);

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        _gameManager.Draw();
        SpriteBatch.End();

        base.Draw(gameTime);
    }
}
