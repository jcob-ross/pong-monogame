namespace PongMonoGame
{
  using System;
  using System.Collections.Generic;
  using Microsoft.Xna.Framework;
  using Microsoft.Xna.Framework.Graphics;
  using Microsoft.Xna.Framework.Input;

  /// <summary>
  ///   Main menu user interface
  /// </summary>
  public sealed class MainMenuUi : SceneUi
  {
    private readonly PongScene _pongScene;
    private const int VerticalSpacing = 70;
    private readonly SpritesBatch _batch;
    private readonly Color _selectedItemColor = Color.White;
    private readonly LinkedList<MenuItem> _menuItems = new LinkedList<MenuItem>();
    private LinkedListNode<MenuItem> _currentItem;
    private bool _disposed;
    private SpriteFont _font;
    private Texture2D _blackTransparentTexture;
    private SceneUi _optionsUi;
    private SpriteFont _titleFont;
    private string _titleText;
    private Vector2 _titleMeasured;

    /// <summary>
    ///   Creates new instance of <see cref="MainMenuUi"/>
    /// </summary>
    /// <param name="pongScene"></param>
    /// <param name="sceneManager"></param>
    /// <param name="camera"></param>
    public MainMenuUi(PongScene pongScene, SceneManager sceneManager, Camera camera) : base(sceneManager, camera)
    {
      if (null == pongScene)
        throw new ArgumentNullException(nameof(pongScene));
      _pongScene = pongScene;
      _batch = new SpritesBatch(SceneManager.Device);
    }

    public override void LoadContent()
    {
      _titleFont = SceneManager.ContentManager.Load<SpriteFont>("ErbosDracoL");
      _titleText = "P O N G";
      _titleMeasured = _titleFont.MeasureString(_titleText);
      _font = SceneManager.ContentManager.Load<SpriteFont>("AtariFont");
      var optionsSubMenu = new OptionsMenu(_pongScene, SceneManager, Camera);
      _optionsUi = optionsSubMenu;
      optionsSubMenu.LoadContent();
      var newGame = "New Game";
      var options = "Options";
      var exit = "Exit";
      var newGameMi = new MenuItem(newGame, _font.MeasureString(newGame)) {Selected = true};
      var optionsMi = new MenuItem(options, _font.MeasureString(options), optionsSubMenu);
      var exitMi = new MenuItem(exit, _font.MeasureString(exit));

      newGameMi.SelectAction += SelectNewGame;
      optionsMi.SelectAction += SelectOptions;
      exitMi.SelectAction += SelectExit;
      _menuItems.AddFirst(newGameMi);
      _currentItem = _menuItems.First;
      _menuItems.AddLast(optionsMi);
      _menuItems.AddLast(exitMi);

      var tex = new Texture2D(SceneManager.Device, 1, 1);
      var black = Color.Black;
      tex.SetData(new[] { black });
      _blackTransparentTexture = tex;
    }

    private void SelectOptions()
    {
      SceneManager.PushUi(_optionsUi);
    }

    private void SelectExit()
    {
      SceneManager.CallingExit = true;
    }

    private void SelectNewGame()
    {
      _pongScene.Reset();
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
        SceneManager.PopUi();

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

      if (SceneManager.Input.KeyWasReleased(Keys.Enter))
        _currentItem.Value.SelectAction?.Invoke();
    }

    public override void Draw(float timeStep)
    {
      var frustrum = Camera.GetRectangle();
      _batch.Begin();
      _batch.DrawSprite(_blackTransparentTexture, destinationRectangle: frustrum);
      _batch.DrawString(_titleFont, new Vector2(frustrum.Width / 2f - _titleMeasured.X / 2.05f,
        frustrum.Height / 4f + _titleMeasured.Y / 2f), _titleText);
      var current = _menuItems.First;
      var count = 0;
      while (current != null)
      {
        var position = new Vector2(frustrum.Width / 2f - current.Value.MeasuredSize.X / 2f,
                           (frustrum.Height / 3f * 2) + VerticalSpacing * count - _menuItems.Count * VerticalSpacing / 2f);

        _batch.DrawString(_font, position, current.Value.Text, current.Value.Selected ? _selectedItemColor : Color.BlueViolet);

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