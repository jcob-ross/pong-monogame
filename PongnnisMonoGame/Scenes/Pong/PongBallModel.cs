namespace PongMonoGame
{
  using Microsoft.Xna.Framework;

  /// <summary>
  ///   Drawable component of the ball
  /// </summary>
  public class PongBallModel : Component, IDrawable
  {
    public override ComponentType ComponentType => ComponentType.Model;

    /// <summary>
    ///   Radius of the ball.
    /// </summary>
    public float Radius { get; private set; }

    /// <summary>
    ///   Indicates whether the ball is visible and should be drawed
    /// </summary>
    public bool Visible { get; set; } = true;

    /// <summary>
    ///   Creates new instance of <see cref="PongBallModel"/>
    /// </summary>
    /// <param name="radius">Length of the ball's side. (Ball is not a ball, how revolting)</param>
    public PongBallModel(float radius)
    {
      Radius = radius;
    }

    /// <summary>
    ///   Draws the component using <see cref="SpritesBatch"/>
    /// </summary>
    /// <param name="batch">Batch to use for drawing</param>
    /// <param name="timeStep">Time delta form the last update</param>
    public void DrawSprites(SpritesBatch batch, float timeStep)
    {
      
    }
    /// <summary>
    ///   Draws the component using <see cref="PrimitivesBatch"/>
    /// </summary>
    /// <param name="batch">Batch to use for drawing</param>
    /// <param name="timeStep">Time delta form the last update</param>
    public void DrawPrimitives(PrimitivesBatch batch, float timeStep)
    {
      var topleft = new Vector2(Base.Position.X - Radius, Base.Position.Y - Radius);
      var bottomright = topleft + new Vector2(2 * Radius, 2 * Radius);
      batch.DrawRectangle(topleft, bottomright, Color.BlueViolet);
    }
  }
}