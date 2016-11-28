namespace PongMonoGame
{
  using System;
  using System.Diagnostics;
  using System.Text;
  using Microsoft.Xna.Framework;
  using Microsoft.Xna.Framework.Graphics;

  public class SpritesBatch : IDisposable
  {
    private bool _disposed;
    private bool _hasBegun;
    private readonly SpriteBatch _batch;
    public bool IsReady => ! _hasBegun;
    public SpritesBatch(GraphicsDevice device)
    {
      if (null == device)
        throw new ArgumentNullException(nameof(device));

      _batch = new SpriteBatch(device);
    }

    public void Begin(Effect effect = null)
    {
      if (_hasBegun)
        return;
      
      _batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,
                   SamplerState.PointClamp, DepthStencilState.None, 
                   RasterizerState.CullNone, effect);
      Debug.Assert(_hasBegun == false);
      _hasBegun = true;
    }

    public void End()
    {
      if (! _hasBegun)
        return;
      _batch.End();
      _hasBegun = false;
    }


    public void DrawString(SpriteFont font, Vector2 position, string text, Color? color = null, float layerDepth = 0f, float scale = 1f, float rotation = 0f, Vector2? origin = null, float timeStep = 0f)
    {
      if (!_hasBegun)
        return;

      _batch.DrawString(font, text, position, color ?? Color.White, rotation, origin ?? Vector2.Zero, scale, SpriteEffects.None, layerDepth);
    }

    public void DrawString(SpriteFont font, Vector2 position, StringBuilder sb, Color? color = null, float layerDepth = 0f, float scale = 1f, float rotation = 0f, Vector2? origin = null, float timeStep = 0f)
    {
      if (!_hasBegun)
        return;

      _batch.DrawString(font, sb, position, color ?? Color.White, rotation, origin ?? Vector2.Zero, scale, SpriteEffects.None, layerDepth);
    }

    public void DrawSprite(Texture2D texture, Vector2? position = null, Rectangle? sourceRectangle = null, Rectangle? destinationRectangle = null, Color? color = null, float layerDepth = 0f, Vector2? origin = null, float rotation = 0f, Vector2? scale = null, float timeStep = 0f)
    {
      if (!_hasBegun)
        return;

      _batch.Draw(texture, position, destinationRectangle, sourceRectangle, origin, rotation, scale, color, SpriteEffects.None, layerDepth);
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
          _batch?.Dispose();
        }
        _disposed = true;
      }
    }
  }
}