namespace PongMonoGame
{
  using System;
  using Microsoft.Xna.Framework;
  using Microsoft.Xna.Framework.Graphics;
  using Microsoft.Xna.Framework.Input;

  /// <summary>
  ///   Ui overlay when game is paused
  /// </summary>
  public sealed class PausedPongUi: SceneUi
  {
    private readonly SpritesBatch _batch;

    private bool _disposed;
    private SpriteFont _font;
    private string _text;
    private Vector2 _measuredText;
    private Texture2D _blackTransparentTexture;
    public PausedPongUi(SceneManager sceneManager, Camera camera) 
      : base(sceneManager, camera)
    {
      _batch = new SpritesBatch(SceneManager.Device);
    }

    public override void LoadContent()
    {
      _font = SceneManager.ContentManager.Load<SpriteFont>("AtariFont");
      _text = "Game is [P]aused";
      _measuredText = _font.MeasureString(_text);
      var tex = new Texture2D(SceneManager.Device, 1, 1);
      var black = Color.Black;
      black.A = 200;
      tex.SetData(new[] { black });
      _blackTransparentTexture = tex;
    }

    public override void UnloadContent()
    {
      _blackTransparentTexture.Dispose();
    }

    public override void Update(float timeStep)
    {
      if (SceneManager.Input.KeyWasReleased(Keys.P) || SceneManager.Input.KeyWasReleased(Keys.Escape))
        SceneManager.PopUi();
    }

    public override void Draw(float timeStep)
    {
      var frustrum = Camera.GetRectangle();
      _batch.Begin();
      _batch.DrawSprite(_blackTransparentTexture, destinationRectangle: frustrum);
      _batch.DrawString(_font, new Vector2(frustrum.Width / 2f - _measuredText.X / 2f, frustrum.Height / 2f - _measuredText.Y / 2f), _text);
      _batch.End();
    }

    public override void Dispose()
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
          _batch.Dispose();
          _blackTransparentTexture.Dispose();
        }
        _disposed = true;
      }
    }
  }
}