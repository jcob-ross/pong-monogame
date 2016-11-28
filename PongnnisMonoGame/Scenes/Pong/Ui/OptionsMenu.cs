namespace PongMonoGame
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Microsoft.Xna.Framework;
  using Microsoft.Xna.Framework.Graphics;
  using Microsoft.Xna.Framework.Input;

  /// <summary>
  ///   Options UI menu
  /// </summary>
  public sealed class OptionsMenu : SceneUi
  {
    private readonly PongScene _pongScene;
    private const int VerticalSpacing = 40;
    private readonly SpritesBatch _batch;
    private readonly Color _selectedItemColor = Color.White;
    private readonly LinkedList<ValueMenuItem> _menuItems = new LinkedList<ValueMenuItem>();
    private LinkedListNode<ValueMenuItem> _currentItem;
    private bool _disposed;
    private SpriteFont _font;
    private Texture2D _blackTransparentTexture;

    public OptionsMenu(PongScene pongScene, SceneManager sceneManager, Camera camera) : base(sceneManager, camera)
    {
      if (null == pongScene)
        throw new ArgumentNullException(nameof(pongScene));
      _pongScene = pongScene;
      _batch = new SpritesBatch(SceneManager.Device);
    }

    public override void LoadContent()
    {
      _font = SceneManager.ContentManager.Load<SpriteFont>("AtariFont");
      var masterVolume = "Master volume";
      var pointsToWin = "Points to win";
      var masterVolumeMi = new ValueMenuItem(masterVolume, 1f, .1f, 0f, 1f, _font.MeasureString(masterVolume)) { Selected = true };
      var pointsToWinMi = new ValueMenuItem(pointsToWin, _pongScene.PointsToWin, 1f, 1f, 999f, _font.MeasureString(pointsToWin));
      _menuItems.AddFirst(masterVolumeMi);
      _currentItem = _menuItems.First;
      _menuItems.AddLast(pointsToWinMi);

      var tex = new Texture2D(SceneManager.Device, 1, 1);
      tex.SetData(new[] { Color.Black });
      _blackTransparentTexture = tex;
    }

    public override void UnloadContent()
    {
      foreach (var menuItem in _menuItems)
      {
        if (menuItem.HasChildUi)
          menuItem.ChildUi.UnloadContent();
      }
      _menuItems.Clear();
      _blackTransparentTexture.Dispose();
    }

    public override void Update(float timeStep)
    {
      if (SceneManager.Input.KeyWasReleased(Keys.Escape))
      {
        SceneManager.PopUi();
        var volume = _menuItems.First(i => i.Text == "Master volume");
        var points = _menuItems.First(i => i.Text == "Points to win");
        SceneManager.SoundPlayer.MasterVolume = volume.Value;
        _pongScene.PointsToWin = (int)points.Value;
      }

      if (SceneManager.Input.KeyWasReleased(Keys.Up))
      {
        _currentItem.Value.Selected = false;
        if (_currentItem == _menuItems.First)
          _currentItem = _menuItems.Last;
        else
          _currentItem = _currentItem.Previous;

        _currentItem.Value.Selected = true;
      }

      if (SceneManager.Input.KeyWasReleased(Keys.Down))
      {
        _currentItem.Value.Selected = false;
        if (_currentItem == _menuItems.Last)
          _currentItem = _menuItems.First;
        else
          _currentItem = _currentItem.Next;

        _currentItem.Value.Selected = true;
      }

      if (SceneManager.Input.KeyWasReleased(Keys.Left))
      {
        _currentItem.Value.Decrement();
      }

      if (SceneManager.Input.KeyWasReleased(Keys.Right))
      {
        _currentItem.Value.Increment();
      }
    }

    public override void Draw(float timeStep)
    {
      var frustrum = Camera.GetRectangle();
      _batch.Begin();
      _batch.DrawSprite(_blackTransparentTexture, destinationRectangle: frustrum);
      var current = _menuItems.First;
      var count = 0;

      while (current != null)
      {
        var position = new Vector2(frustrum.Width / 4f,
                           frustrum.Height / 2f + VerticalSpacing * count - _menuItems.Count * VerticalSpacing / 2f);

        _batch.DrawString(_font, position, $"{current.Value.Text}: {current.Value.Value:0.#}",
                          current.Value.Selected ? _selectedItemColor : Color.BlueViolet);
        count += 1;
        current = current.Next;
      }
      _batch.End();
    }

    public override void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }
    private void Dispose(bool disposing)
    {
      if (!_disposed)
      {
        if (disposing)
        {
          _batch.Dispose();
          _blackTransparentTexture.Dispose();
        }
        _disposed = true;
      }
    }
  }
}