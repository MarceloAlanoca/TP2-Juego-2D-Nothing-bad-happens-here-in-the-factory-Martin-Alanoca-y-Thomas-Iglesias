using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary;
using MonoGameGum;
using Gum.Forms.Controls;
using System;
using RenderingLibrary.Graphics;
using Gum.Converters;

public class PantallaDerrota
{
    private Texture2D _fondo;
    private StackPanel _panel;
    public Action OnRejugar;

    public PantallaDerrota()
    {
        // Cambiá por tu imagen. Mientras no la tengas, podés usar "Fondos/FondoInicio".
        _fondo = GLobals.Content.Load<Texture2D>("Fondos/FondoDerrota");

        _panel = new StackPanel();
        _panel.Visual.AddToRoot();
        _panel.Visual.XUnits = GeneralUnitType.PixelsFromMiddle;
        _panel.Visual.YUnits = GeneralUnitType.PixelsFromMiddle;

        var rejugar = new Button();
        rejugar.Text = "Volver a jugar";
        rejugar.Visual.XOrigin = HorizontalAlignment.Center;
        rejugar.Visual.YOrigin = VerticalAlignment.Center;
        rejugar.Visual.Width = 250;
        rejugar.Visual.Height = 70;
        rejugar.Click += (sender, args) =>
        {
            OnRejugar?.Invoke();
        };
        _panel.AddChild(rejugar);

        // Arranca oculto: solo se ve cuando se gana.
        _panel.Visual.Visible = false;
    }

    public void Mostrar() => _panel.Visual.Visible = true;
    public void Ocultar() => _panel.Visual.Visible = false;

    public void Draw()
    {
        var vp = GLobals.SpriteBatch.GraphicsDevice.Viewport;
        GLobals.SpriteBatch.Draw(
            _fondo,
            new Rectangle(0, 0, vp.Width, vp.Height),
            Color.White);
    }
}