
namespace PongMonoGame
{
  using Microsoft.Xna.Framework;

  /// <summary>
  ///   Pong game main class
  /// </summary>
  public class PongGame : Game
  {
    private readonly GraphicsDeviceManager _graphics;
    private SceneManager _sceneManager;

    public PongGame()
    {
      _graphics = new GraphicsDeviceManager(this)
                  {
                    PreferredBackBufferWidth = 800,
                    PreferredBackBufferHeight = 600,
                    SynchronizeWithVerticalRetrace = false
                  };
      IsFixedTimeStep = false;
      Window.AllowUserResizing = false;
      Content.RootDirectory = "Content";
      IsMouseVisible = true;
    }

    protected override void Initialize()
    {

      Services.AddService(typeof(GraphicsDeviceManager), _graphics);
      var input = new InputManager(this);
      _sceneManager = new SceneManager(Window, Services, Content, input);
      base.Initialize();
    }

    protected override void LoadContent()
    {
      _sceneManager.Init();
    }

    protected override void UnloadContent()
    {
      _sceneManager.UnloadContent();
    }

    protected override void Update(GameTime gameTime)
    {
      if (_sceneManager.CallingExit)
        Exit();

      _sceneManager.Update(gameTime);
      base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
      _graphics.GraphicsDevice.Clear(Color.Black);
      _sceneManager.Draw(gameTime);
      base.Draw(gameTime);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        _sceneManager?.Dispose();
      }
      base.Dispose(disposing);
    }
  }
}

