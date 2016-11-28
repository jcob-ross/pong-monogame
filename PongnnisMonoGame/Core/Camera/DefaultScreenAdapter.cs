namespace PongMonoGame
{
  using Microsoft.Xna.Framework;
  using Microsoft.Xna.Framework.Graphics;

  /// <summary>
  ///   Default screen adapter that's using native window's dimmensions
  /// </summary>
  public class DefaultScreenAdapter : ScreenAdapter
  {
    public DefaultScreenAdapter(GraphicsDevice device) : base(device)
    {
    }

    public override int VirtualWidth => GraphicsDevice.Viewport.Width;
    public override int VirtualHeight => GraphicsDevice.Viewport.Height;
    public override int ViewportWidth => GraphicsDevice.Viewport.Width;
    public override int ViewportHeight => GraphicsDevice.Viewport.Height;

    public override Matrix GetScaleMatrix()
    {
      return Matrix.Identity;
    }
  }
}