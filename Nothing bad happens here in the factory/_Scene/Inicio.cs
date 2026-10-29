using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using MonoGameGum;
using Gum.Forms.Controls;
using System;
using RenderingLibrary.Graphics;
using Gum.Converters;

public class InitGame
{
    private Texture2D _fondInit;
    StackPanel mainPanel;
    public Action OnIniciar;

    public InitGame()
    {
        _fondInit = GLobals.Content.Load<Texture2D>("Fondos/FondoInicio");
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
        iniciar.Text = "Iniciar";
        iniciar.Visual.XOrigin = HorizontalAlignment.Center;
        iniciar.Visual.YOrigin = VerticalAlignment.Center;
        mainPanel.Visual.XUnits = GeneralUnitType.PixelsFromMiddle;
        mainPanel.Visual.YUnits = GeneralUnitType.PixelsFromMiddle;
        iniciar.Visual.Width = 250;
        iniciar.Visual.Height = 70;
        iniciar.Click += (sender, args) =>
        {
            OnIniciar?.Invoke(); 
        };
        mainPanel.AddChild(iniciar);
    }

    public void Ocultar()
    {
        // Oculta el panel (y el botón) para que no se vea ni se pueda clickear en la otra escena.
        mainPanel.Visual.Visible = false;
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