namespace PongMonoGame
{
  using System;
  using System.Diagnostics;
  using Microsoft.Xna.Framework;

  /// <summary>
  ///   Represents Pong Paddle the AI or Human can control
  /// </summary>
  public class Paddle : BaseObject
  {
    private readonly Random _random = new Random();
    /// <summary>
    ///   Width of the paddle
    /// </summary>
    public int Width { get; private set; }
    /// <summary>
    ///   Height of the paddle
    /// </summary>
    public int Height { get; private set; }

    /// <summary>
    ///   Creates new instance of <see cref="Paddle"/>
    /// </summary>
    /// <param name="width">Width of the paddle</param>
    /// <param name="height">Height of the paddle</param>
    /// <param name="name">Name for the base object</param>
    public Paddle(int width, int height, string name) : base(name)
    {
      Width = width;
      Height = height;
    }

    public override void Update(float timeStep)
    {
      base.Update(timeStep);
      Bounds = new AABB(Position, Width, Height);
    }

    /// <summary>
    ///   Checks wheteher paddle is colliding with ball.
    ///   Fills <paramref name="collisionManifold"/> struct if true.
    /// </summary>
    /// <param name="ball">Ball to check collision against</param>
    /// <param name="direction">Direction tha ball is currently traveling in</param>
    /// <param name="collisionManifold"><see cref="Manifold"/> to be filled with info about collision event</param>
    /// <returns><c>True</c> if the paddle is colliding with ball, <c>False</c> otherwise</returns>
    public bool IsColliding(BaseObject ball, Vector2 direction, ref Manifold collisionManifold)
    {
      var paddleAABB = GetAABB();
      var ballAABB = ball.GetAABB();
      if (AABB.TestOverlap(ref paddleAABB, ref ballAABB))
      {
        collisionManifold.Normal = GetCollisionNormal(direction);
        collisionManifold.PenetrationDepth = GetBallPenetrationDepth(ball, direction);

        var spread = .3f + 1 * (float)_random.NextDouble();
        var hitSector = GetHitSector(ball.Position.Y);
        switch (hitSector)
        {
          case PaddleHitSector.OuterUp:
            collisionManifold.NewDirection = VectorMath.NormalizeVector2(new Vector2(collisionManifold.Normal.X, -spread));
            break;
          case PaddleHitSector.InnerUp:
          case PaddleHitSector.InnerDown:
            collisionManifold.NewDirection = Vector2.Reflect(direction, collisionManifold.Normal);
            break;
          case PaddleHitSector.OuterDown:
            collisionManifold.NewDirection = VectorMath.NormalizeVector2(new Vector2(collisionManifold.Normal.X, spread));
            break;
        }
        return true;
      }
      return false;
    }

    private static Vector2 GetCollisionNormal(Vector2 direction)
    {
      // collision from top and bottom is ignored
      if (direction.X > 0)
        return new Vector2(-1f, 0f);
      else
        return new Vector2(1f, 0f);
    }

    private float GetBallPenetrationDepth(BaseObject ball, Vector2 direction)
    {
      var pongBall = ball as PongBall;
      Debug.Assert(pongBall != null);

      float depth = 0f;
      if (direction.X > 0)
      {
        depth = pongBall.Position.X + pongBall.Radius - (Position.X - Width / 2f);
      }
      else if (direction.X < 0)
      {
        depth = (Position.X + Width / 2f) - (pongBall.Position.X - pongBall.Radius);
      }
      return depth;
    }

    private PaddleHitSector GetHitSector(float ballYPosition)
    {
      var heightOver4 = Height / 4;
      if (ballYPosition >= Position.Y + heightOver4) // Outer Down
        return PaddleHitSector.OuterDown;
      if (ballYPosition <= (Position.Y - heightOver4)) // Outer Up
        return PaddleHitSector.OuterUp;
      if (ballYPosition >= Position.Y && ballYPosition <= Position.Y + heightOver4) // Inner Down
        return PaddleHitSector.InnerDown;
      if (ballYPosition <= Position.Y && ballYPosition >= Position.Y - heightOver4) // Inner Up
        return PaddleHitSector.InnerUp;

      Debug.Assert(false, $"Paddle.GetHitSector - Position: Relative(ball): [{ballYPosition}], Paddle: [{Position.Y}]");
      return PaddleHitSector.None;
    }
  }
}