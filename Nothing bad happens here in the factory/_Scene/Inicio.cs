using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using MonoGameGum;
using Gum.Forms.Controls;
using System;

public class InitGame
{
    private Texture2D _fondInit;
    StackPanel mainPanel;
    public Action OnIniciar;

    public InitGame()
    {
        _fondInit = GLobals.Content.Load<Texture2D>("Fondos/Salida_1");
    }

    public void Init()
    {
        _Gum(); // Gum ya fue inicializado en Game1
    }

    private void _Gum()
    {
        mainPanel = new StackPanel();
        mainPanel.Visual.AddToRoot();

        var iniciar = new Button();
        iniciar.Text = "hola";
        iniciar.Visual.Width = 200;
        iniciar.Visual.Height = 200;
        iniciar.Click += (sender, args) =>
        {
            OnIniciar?.Invoke(); 
        };
        mainPanel.AddChild(iniciar);
    }

    public void Update()
    {
        // vacío por ahora
    }

    public void Draw()
    {
        GLobals.SpriteBatch.Draw(
            _fondInit,
            new Rectangle(0, 0,
                GLobals.SpriteBatch.GraphicsDevice.Viewport.Width,
                GLobals.SpriteBatch.GraphicsDevice.Viewport.Height),
            Color.White);
    }
}