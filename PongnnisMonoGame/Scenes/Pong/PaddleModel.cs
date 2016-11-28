namespace PongMonoGame
{
  using Microsoft.Xna.Framework;

  /// <summary>
  ///   Represents drawable component of the paddle
  /// </summary>
  public class PaddleModel : Component, IDrawable
  {
    public Color Color { get; set; } = Color.White;
    public int HalfWidth { get; private set; }
    public int HalfHeight { get; private set; }
    public override ComponentType ComponentType => ComponentType.Model;

    /// <summary>
    ///   Creates new instance of the <see cref="PaddleModel"/>
    /// </summary>
    /// <param name="halfWidth">Half the width of the <see cref="Paddle"/></param>
    /// <param name="halfHeight">Half the height of the <see cref="Paddle"/></param>
    public PaddleModel(int halfWidth, int halfHeight)
    {
      HalfWidth = halfWidth;
      HalfHeight = halfHeight;
    }

    /// <summary>
    ///   Indicates whether <see cref="PaddleModel"/> is visible and should be drawed.
    /// </summary>
    public bool Visible { get; } = true;

    /// <summary>
    ///   Draws the <see cref="PaddleModel"/>
    /// </summary>
    /// <param name="batch"><see cref="SpritesBatch"/> to use for drawing</param>
    /// <param name="timeStep">Time delta from the last update</param>
    public void DrawSprites(SpritesBatch batch, float timeStep)
    {

    }

    /// <summary>
    ///   Draws the <see cref="PaddleModel"/>
    /// </summary>
    /// <param name="batch"><see cref="PrimitivesBatch"/> to use for drawing</param>
    /// <param name="timeStep">Time delta from the last update</param>
    public void DrawPrimitives(PrimitivesBatch batch, float timeStep)
    {
      var topleft = new Vector2(Base.Position.X - HalfWidth, Base.Position.Y - HalfHeight);
      var bottomright = topleft + new Vector2(2 * HalfWidth, 2 * HalfHeight);

      batch.DrawRectangle(topleft, bottomright, Color);
    }
  }
}