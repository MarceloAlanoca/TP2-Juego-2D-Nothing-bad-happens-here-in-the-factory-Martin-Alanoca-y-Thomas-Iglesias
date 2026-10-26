using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameGum;
using Gum.Forms.Controls;
using Nothing_bad_happens_here_in_the_factory._Managers;
using System.Net.NetworkInformation;
using System;

public class InitGame
{
    private Texture2D _fondInit;
    GumService Gum => GumService.Default;

    StackPanel mainPanel;
    
    public InitGame()
    {
        _fondInit = GLobals.Content.Load<Texture2D>("Fondos/Salida_1");

    }
    public void _Gum()
    {
        mainPanel = new StackPanel();
        mainPanel.Visual.AddToRoot();

        var iniciar = new Button();
        iniciar.Text = "hola";
        iniciar.Click += (sender, args) =>
        {
            Console.WriteLine("hola");
        };
        mainPanel.AddChild(iniciar);
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

    public void Update()
    {
        // vacío por ahora
    }

    public void Init()
    {
        _Gum();
    }
}