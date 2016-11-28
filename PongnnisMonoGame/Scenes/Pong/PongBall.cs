namespace PongMonoGame
{
  /// <summary>
  ///   Represent Pong ball
  /// </summary>
  public class PongBall : BaseObject
  {
    /// <summary>
    ///   Radius of the ball. In this case length of the side as ball is actually square. Who knew.
    /// </summary>
    public float Radius { get; private set; }

    /// <summary>
    ///   Creates new instance of <see cref="PongBall"/>
    /// </summary>
    /// <param name="bounds"><see cref="AABB"/> bounds of the ball</param>
    /// <param name="name">Name of the underlying <see cref="BaseObject"/></param>
    public PongBall(AABB bounds, string name) : base(name)
    {
      Bounds = bounds;
      Radius = bounds.Width / 2f;
    }

    public override void Update(float timeStep)
    {
      base.Update(timeStep);
      Bounds = new AABB(Position, Radius * 2, Radius * 2);
    }
  }
}