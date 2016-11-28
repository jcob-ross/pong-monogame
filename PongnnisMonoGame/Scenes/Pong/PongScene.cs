namespace PongMonoGame
{
  using System;
  using Microsoft.Xna.Framework;
  using Microsoft.Xna.Framework.Graphics;
  using Microsoft.Xna.Framework.Input;

  /// <summary>
  ///   Game scene for the Pong game
  /// </summary>
  public class PongScene : GameScene
  {
    private const int MaxPoints = 999;
    private const int MinPoints = 1;
    private const int WorldWidth = 1200;
    private const int WorldHeight = 570;
    private PongBallBehavior _ballAi;
    private BasicEffect _effect;
    private float _elapsedTime;
    private SpriteFont _erbosFont;
    private float _fps;
    private int _frameCount;
    private SceneUi _mainMenuUi;
    private SceneUi _pausedUi;
    private SceneUi _winUi;
    private PongBall _pongBall;

    private int _pointsToWin = 3;
    private int _scoreLeft;
    private int _scoreRight;

    /// <summary>
    ///   Creates new instance of <see cref="PongScene"/>
    /// </summary>
    /// <param name="sceneManager"><see cref="SceneManager"/></param>
    /// <param name="screenAdapter"><see cref="ScreenAdapter"/> to be used for camera</param>
    public PongScene(SceneManager sceneManager, ScreenAdapter screenAdapter)
      : base("PongScene", sceneManager, screenAdapter)
    {
    }

    /// <summary>
    ///   Number of points a player has to score to win.
    /// </summary>
    public int PointsToWin
    {
      get { return _pointsToWin; }
      set { _pointsToWin = MathHelper.Clamp(value, MinPoints, MaxPoints); }
    }
    /// <summary>
    ///   Bounds of the Pong game scene
    /// </summary>
    public AABB WorldBounds { get; private set; }
    public Paddle LeftPaddle { get; private set; }
    public Paddle RightPaddle { get; private set; }
    public InputManager Input => SceneManager.Input;
    public SoundPlayer SoundPlayer => SceneManager.SoundPlayer;

    /// <summary>
    ///   Winner of the game. Returns <see cref="PlayerSide.None"/> if the game is in progress.
    /// </summary>
    public PlayerSide Winner { get; private set; } = PlayerSide.None;

    /// <summary>
    ///   Current score of the left paddle
    /// </summary>
    public int ScoreLeft => _scoreLeft;
    /// <summary>
    ///   Current score of the right paddle
    /// </summary>
    public int ScoreRight => _scoreRight;

    /// <summary>
    ///   Loads assets needed by the game scene
    /// </summary>
    public override void LoadContent()
    {
      WorldBounds = new AABB(Vector2.Zero, WorldWidth, WorldHeight);

      SceneManager.SoundPlayer.LoadSounds();
      _erbosFont = SceneManager.ContentManager.Load<SpriteFont>("ErbosDraco");

      _effect = new BasicEffect(SceneManager.Device);
      Camera = new Camera(SceneManager.Device, SceneManager.Input, ScreenAdapter);

      var pongBallBounds = new AABB(new Vector2(-4, -4), new Vector2(8, 8));
      var pongBall = new PongBall(pongBallBounds, "pong_ball");

      var pongBallBehavior = new PongBallBehavior(pongBallBounds.Width / 2f, this);
      var pongBallSprite = new PongBallModel(pongBallBounds.Width / 2f);

      pongBall.AddComponent(pongBallSprite);
      pongBall.AddComponent(pongBallBehavior);

      var paddleLeftSprite = new PaddleModel(2, 40);
      var paddleRightSprite = new PaddleModel(2, 40);

      var paddleLeftBehavior = new PaddlePlayerBehavior(Input, WorldBounds);
      var paddleRightAiBehavior = new PaddleAiBehavior(pongBall, WorldBounds);

      var paddleLeft = new Paddle(4, 80, "paddle_left");
      var paddleRight = new Paddle(4, 80, "paddle_right");

      paddleLeft.AddComponent(paddleLeftSprite);
      paddleLeft.AddComponent(paddleLeftBehavior);

      paddleRight.AddComponent(paddleRightSprite);
      paddleRight.AddComponent(paddleRightAiBehavior);

      _pongBall = pongBall;
      _ballAi = pongBallBehavior;
      _ballAi.PlayerScored += OnPlayerScored;
      LeftPaddle = paddleLeft;
      RightPaddle = paddleRight;

      Actors.Add(paddleLeft);
      Actors.Add(paddleRight);
      Actors.Add(pongBall);
      Reset();
      Enabled = false;

      _mainMenuUi = new MainMenuUi(this, SceneManager, Camera);
      _mainMenuUi.LoadContent();
      SceneManager.PushUi(_mainMenuUi);
      _pausedUi = new PausedPongUi(SceneManager, Camera);
      _pausedUi.LoadContent();
      _winUi = new WinUi(this, SceneManager, Camera);
      _winUi.LoadContent();
      SceneManager.UiEnter += SceneManagerOnUiEnter;
      SceneManager.UiExit += SceneManagerOnUiExit;
      base.LoadContent();
    }

    public override void UnloadContent()
    {
      _ballAi.PlayerScored -= OnPlayerScored;
      SceneManager.UiEnter -= SceneManagerOnUiEnter;
      SceneManager.UiExit -= SceneManagerOnUiExit;

      Actors.Clear();

      _effect.Dispose();

      SceneManager.SoundPlayer.UnloadSounds();
      _pausedUi.UnloadContent();
      base.UnloadContent();
    }

    public override void Update(float timeStep)
    {
      base.Update(timeStep);

      _elapsedTime += timeStep;
      if (_elapsedTime > 1000.0f)
      {
        _fps = _frameCount;
        _frameCount = 0;
        _elapsedTime = 0;
      }

      if (SceneManager.Input.KeyWasReleased(Keys.F4))
        Camera.Enabled = ! Camera.Enabled;

      if (SceneManager.Input.KeyWasReleased(Keys.F5))
        SceneManager.LoadScene("BallScene");

      Winner = GetWinner();
      if (Winner != PlayerSide.None)
      {
        SceneManager.PushUi(_mainMenuUi);
        SceneManager.PushUi(_winUi);
      }

      if (Input.KeyWasReleased(Keys.P))
        SceneManager.PushUi(_pausedUi);
      if (Input.KeyWasReleased(Keys.Escape))
        SceneManager.PushUi(_mainMenuUi);

      for (var i = 0; i < Actors.Count; i++)
        Actors[i].Update(timeStep);
    }

    private void OnPlayerScored(GoalEventArgs goalEventArgs)
    {
      float offset = LeftPaddle.Width / 2f + _pongBall.Radius;

      if (goalEventArgs.ScoringPlayer == PlayerSide.Left)
      {
        _scoreLeft += 1;
        _pongBall.Move(-_pongBall.Position);
        _pongBall.Move(LeftPaddle.Position + new Vector2(offset, 0));
        _ballAi.Direction = new Vector2(1, 0);
      }
      else
      {
        _scoreRight += 1;
        _pongBall.Move(-_pongBall.Position);
        _pongBall.Move(RightPaddle.Position - new Vector2(offset, 0));
        _ballAi.Direction = new Vector2(-1, 0);
      }
    }

    public override void Draw(float timeStep)
    {
      if (! ContentLoaded)
        return;
      ++_frameCount;

      _effect.World = Matrix.CreateScale(1, 1, 1) * Matrix.CreateTranslation(Vector3.Zero);
      _effect.Projection = Camera.CreateProjection(Vector2.Zero);
      _effect.View = Camera.GetViewMatrix(Vector2.One);
      _effect.TextureEnabled = false;
      _effect.VertexColorEnabled = true;

      DrawBackground(SceneManager.PrimitivesBatch);
      DrawEntities(SceneManager.PrimitivesBatch, timeStep);
      DrawInterface(SceneManager.SpritesBatch);
    }

    /// <summary>
    ///   Resets the game state.
    /// </summary>
    /// <param name="popUi"></param>
    public void Reset(int popUi = 2)
    {
      _scoreRight = _scoreLeft = 0;
      _pongBall.Move(-_pongBall.Position);
      _ballAi.Direction = new Vector2(1, 0);
      _ballAi.Fired = false;
      LeftPaddle.Move(-LeftPaddle.Position);
      LeftPaddle.Move(new Vector2(-335, 0));
      RightPaddle.Move(-RightPaddle.Position);
      RightPaddle.Move(new Vector2(335, 0));

      popUi = Math.Abs(popUi);
      for (var i = 0; i < popUi; ++i)
        SceneManager.PopUi();
    }

    private void DrawBackground(PrimitivesBatch primitivesBatch)
    {
      primitivesBatch.Begin(_effect);
      var lineHeight = 2f;
      float lineWidth = WorldBounds.Width;
      float verticalOffset = WorldBounds.Height / 2f + lineHeight;

      // top line
      primitivesBatch.DrawRectangle(new Vector2(-lineWidth / 2f, -verticalOffset - lineHeight),
                                    new Vector2(lineWidth / 2f, -verticalOffset + lineHeight), Color.White);

      // bottom line
      primitivesBatch.DrawRectangle(new Vector2(-lineWidth / 2f, verticalOffset - lineHeight),
                                    new Vector2(lineWidth / 2f, verticalOffset + lineHeight), Color.White);
      primitivesBatch.End();
    }

    private void DrawEntities(PrimitivesBatch primitivesBatch, float timeStep)
    {
      primitivesBatch.Begin(_effect);
      for (var i = 0; i < Actors.Count; i++)
      {
        if (Actors[i].Visible)
          Actors[i].DrawPrimitives(primitivesBatch, timeStep);
      }
      primitivesBatch.End();
    }

    private void DrawInterface(SpritesBatch spritesBatch)
    {
      string text = $"{_scoreLeft} | {_scoreRight}";
      var textSize = _erbosFont.MeasureString(text);
      spritesBatch.Begin();
      spritesBatch.DrawString(_erbosFont, new Vector2(400 - textSize.X / 2, 15), text);
      spritesBatch.End();
    }

    private void SceneManagerOnUiExit(object sender, UiChangedEventArgs uiChangedEventArgs)
    {
      if (! SceneManager.UiActive)
        Enabled = true;
    }

    private void SceneManagerOnUiEnter(object sender, UiChangedEventArgs uiChangedEventArgs)
    {
      if (_ballAi.Fired && uiChangedEventArgs.UserInterface is MainMenuUi)
        SceneManager.PushUi(_pausedUi);

      Enabled = false;
    }

    private PlayerSide GetWinner()
    {
      if (_scoreLeft >= _pointsToWin)
        return PlayerSide.Left;
      if (_scoreRight >= _pointsToWin)
        return PlayerSide.Right;

      return PlayerSide.None;
    }
  }
}