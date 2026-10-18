
using Microsoft.Xna.Framework;
using MonoGameLibrary;
using Nothing_bad_happens_here_in_the_factory.Models;

namespace Nothing_bad_happens_here_in_the_factory._Managers;
public class GameManager
{
    private Player _player;
    public void Init()
    {
        _player = new (new(200, 200));
    }

    public void Update(GameTime gt)
    {
        InputManager.Update();
        _player.Update(gt);
    }

    public void Draw()
    {
        _player.Draw();
    }
}