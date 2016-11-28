namespace PongMonoGame
{
  using System;
  using Microsoft.Xna.Framework;
  using Microsoft.Xna.Framework.Graphics;
  using Microsoft.Xna.Framework.Input;

  /// <summary>
  ///   UI overlay for finished game state
  /// </summary>
  public sealed class WinUi : SceneUi
  {
    private readonly PongScene _pongScene;
    private readonly SpritesBatch _batch;
    private bool _disposed;
    private SpriteFont _font;
    private Texture2D _blackTransparentTexture;
    public WinUi(PongScene pongScene, SceneManager sceneManager, Camera camera) : base(sceneManager, camera)
    {
      _pongScene = pongScene;
      _batch = new SpritesBatch(SceneManager.Device);
    }

    public override void LoadContent()
    {
      _font = SceneManager.ContentManager.Load<SpriteFont>("AtariFont");

      var tex = new Texture2D(SceneManager.Device, 1, 1);
      tex.SetData(new[] { Color.Black });
      _blackTransparentTexture = tex;
    }

    public override void UnloadContent()
    {
      _blackTransparentTexture.Dispose();
    }

    public override void Update(float timeStep)
    {
      if (SceneManager.Input.KeyWasReleased(Keys.Escape) ||
          SceneManager.Input.KeyWasReleased(Keys.Space) ||
          SceneManager.Input.KeyWasReleased(Keys.Enter))
      {
        _pongScene.Reset(popUi: 1);
      }
    }

    public override void Draw(float timeStep)
    {
      var frustrum = Camera.GetRectangle();
      var message = $"{Enum.GetName(typeof(PlayerSide), _pongScene.Winner)} side won! ({_pongScene.ScoreLeft}:{_pongScene.ScoreRight})";
      var size = _font.MeasureString(message);
      var position = new Vector2(frustrum.Width / 2f - size.X / 2f, frustrum.Height / 2f - size.Y / 2f);

      _batch.Begin();
      _batch.DrawSprite(_blackTransparentTexture, destinationRectangle: frustrum);
      _batch.DrawString(_font, position, message, Color.White);
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