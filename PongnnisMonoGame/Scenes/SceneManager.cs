namespace PongMonoGame
{
  using System;
  using System.Collections.Generic;
  using Microsoft.Xna.Framework;
  using Microsoft.Xna.Framework.Content;
  using Microsoft.Xna.Framework.Graphics;

  /// <summary>
  ///   Manages game scenes.
  /// </summary>
  public class SceneManager : IDisposable
  {
    private readonly GraphicsDeviceManager _deviceManager;
    private readonly GameWindow _gameWindow;
    private readonly Stack<SceneUi> _uiStack = new Stack<SceneUi>();
    private GameScene _currentScene;
    private bool _disposed;
    public GameServiceContainer Services { get; }
    public GraphicsDevice Device { get; }
    public ContentManager ContentManager { get; }
    public InputManager Input { get; }
    public SoundPlayer SoundPlayer { get; private set; }
    public SpritesBatch SpritesBatch { get; private set; }
    public PrimitivesBatch PrimitivesBatch { get; private set; }
    public Dictionary<string, GameScene> Scenes { get; } = new Dictionary<string, GameScene>();

    /// <summary>
    ///   Indicates whether the scene called for exit and the program should close
    /// </summary>
    public bool CallingExit { get; set; }

    /// <summary>
    ///   Called upon pushing UI on the UI stack
    /// </summary>
    public event EventHandler<UiChangedEventArgs> UiEnter;
    /// <summary>
    ///   Called upon popping UI from the UI stack
    /// </summary>
    public event EventHandler<UiChangedEventArgs> UiExit;

    /// <summary>
    ///   Creates new instance of the <see cref="SceneManager"/>
    /// </summary>
    /// <param name="window"><see cref="GameWindow"/> for the program to use</param>
    /// <param name="services"><see cref="GameServiceContainer"/> containg services</param>
    /// <param name="contentManager"><see cref="ContentManager"/> that manages loading and unloading game assets</param>
    /// <param name="inputManager"><see cref="InputManager"/> for managing input from external devices</param>
    public SceneManager(GameWindow window, GameServiceContainer services, ContentManager contentManager, InputManager inputManager)
    {
      if (null == window)
        throw new ArgumentNullException(nameof(window));
      if (null == services)
        throw new ArgumentNullException(nameof(services));
      if (null == contentManager)
        throw new ArgumentNullException(nameof(contentManager));
      if (null == inputManager)
        throw new ArgumentNullException(nameof(inputManager));
      
      var deviceManager = services.GetService<GraphicsDeviceManager>();
      if (null == deviceManager)
        throw new Exception("Graphic device manager not present in services collection");

      _gameWindow = window;
      Services = services;
      Device = deviceManager.GraphicsDevice;
      _deviceManager = deviceManager;
      ContentManager = contentManager;
      Input = inputManager;
      SoundPlayer = new SoundPlayer(ContentManager);
    }

    /// <summary>
    ///   Initializes the <see cref="SceneManager"/> for use
    /// </summary>
    public void Init()
    {
      _deviceManager.ApplyChanges();
      var defaultScreenAdapter = new DefaultScreenAdapter(Device);
      var basic = new BallScene(this, defaultScreenAdapter);
      var letterBoxScreenAdapter = new LetterBoxScreenAdapter(_gameWindow, _deviceManager, 800, 600);
      var pong = new PongScene(this, letterBoxScreenAdapter);
      Scenes.Add(basic.Name, basic);
      Scenes.Add(pong.Name, pong);
      LoadScene(pong.Name);

      SpritesBatch = new SpritesBatch(Device);
      PrimitivesBatch = new PrimitivesBatch(Device);
    }

    /// <summary>
    ///   Loads scene specified by name
    /// </summary>
    /// <param name="sceneName">Name of the scene to load</param>
    public void LoadScene(string sceneName)
    {
      if (! Scenes.ContainsKey(sceneName))
        return;
      var scene = Scenes[sceneName];
      if (scene == _currentScene)
        return;

      _currentScene?.UnloadContent();
      ContentManager.Unload();
      GC.Collect();

      scene.LoadContent();
      _currentScene = scene;
    }

    /// <summary>
    ///   Shows UI by pushing it to the Ui stack
    /// </summary>
    /// <param name="userInterface"></param>
    public void PushUi(SceneUi userInterface)
    {
      OnUiEnter(userInterface);
      _uiStack.Push(userInterface);
    }

    /// <summary>
    ///   Indicates whether there is an UI on the Ui stack that is currently being shown.
    /// </summary>
    public bool UiActive => _uiStack.Count > 0;

    /// <summary>
    ///   Dismisses UI by pushing from the Ui stack.
    /// </summary>
    public void PopUi()
    {
      if (_uiStack.Count > 0)
      {
        var ui = _uiStack.Pop();
        OnUiExit(ui);
      }
    }

    /// <summary>
    ///   Updates the <see cref="SceneManager"/>
    /// </summary>
    /// <param name="gameTime"><see cref="GameTime"/> containg time data about update frequency</param>
    public void Update(GameTime gameTime)
    {
      var time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

      if (_currentScene.Enabled)
        _currentScene.Update(time);

      if (_uiStack.Count > 0)
        _uiStack.Peek().Update(time);
    }

    /// <summary>
    ///   Draws the current scene and all it's entities
    /// </summary>
    /// <param name="gameTime"></param>
    public void Draw(GameTime gameTime)
    {
      var time = (float) gameTime.TotalGameTime.TotalMilliseconds;
      _currentScene.Draw(time);

      if (_uiStack.Count > 0)
        _uiStack.Peek().Draw(time);
    }

    /// <summary>
    ///   Unloads current scene and clears all scenes.
    /// </summary>
    public void UnloadContent()
    {
      _currentScene?.UnloadContent();
      Scenes.Clear();
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
      if (! _disposed)
      {
        if (disposing)
        {
          _currentScene?.UnloadContent();
          SoundPlayer?.Dispose();
        }
        _disposed = true;
      }
    }

    protected virtual void OnUiEnter(SceneUi userInterface)
    {
      Input.Update(new GameTime());
      UiEnter?.Invoke(this, new UiChangedEventArgs(userInterface));
    }

    protected virtual void OnUiExit(SceneUi userInterface)
    {
      Input.Update(new GameTime());
      UiExit?.Invoke(this, new UiChangedEventArgs(userInterface));
    }
  }
}