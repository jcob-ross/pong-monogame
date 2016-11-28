namespace PongMonoGame
{
  using System;
  using Microsoft.Xna.Framework;

  /// <summary>
  ///   Represents AI behavior controll of the paddle
  /// </summary>
  public class PaddleAiBehavior : Component, IUpdateable
  {
    private const float MaxSpeed = 300f / 1000;
    private const float DeadzoneDistance = 10f;
    private const float SlowzoneDistance = 200f;

    private readonly AABB _worldBounds;
    private readonly BaseObject _ball;
    private Paddle _paddle;
    private Vector2 _lastBallPosition = Vector2.Zero;
    public override ComponentType ComponentType => ComponentType.Behavior;
    public bool Enabled { get; set; } = true;

    /// <summary>
    ///   Creates new instance of <see cref="PaddleAiBehavior"/>.
    /// </summary>
    /// <param name="ball"><see cref="BaseObject"/> representing Pong Ball for the behavior to track.</param>
    /// <param name="worldBounds">Dimmensions of the world</param>
    public PaddleAiBehavior(BaseObject ball, AABB worldBounds)
    {
      if (null == ball)
        throw new ArgumentNullException(nameof(ball));

      _worldBounds = worldBounds;
      _ball = ball;
    }

    /// <summary>
    ///   Initializes <see cref="PaddleAiBehavior"/>.
    /// </summary>
    /// <param name="root"></param>
    public override void Init(BaseObject root)
    {
      base.Init(root);
      _paddle = root as Paddle;
      if (_paddle == null)
        throw new InvalidCastException("Component base is not type Paddle");
    }

    public void Update(float timeStep)
    {
      var maxStepSize = MaxSpeed * timeStep;

      var verticalDelta = _ball.Position.Y - Base.Position.Y;
      var absVerticalDelta = Math.Abs(verticalDelta);

      if (absVerticalDelta < DeadzoneDistance)
        return;

      var stepModVertical = GetSpeedModifierFromVerticalDistance(maxStepSize, absVerticalDelta);
      var stepModHorizontal = GetSpeedModifierFromHorizontalDistance(maxStepSize, _ball.Position);
      var stepMod = Math.Max(stepModHorizontal, stepModVertical);
      
      Vector2 calculatedStep;
      if (verticalDelta > 0)
        calculatedStep = new Vector2(0f, 1f) * stepMod; // move up
      else
        calculatedStep = new Vector2(0f, -1f) * stepMod; // move down

      if (calculatedStep.Y > 0 && CanMoveDown())
        Base.Move(calculatedStep);
      if (calculatedStep.Y < 0 && CanMoveUp())
        Base.Move(calculatedStep);

      _lastBallPosition = _ball.Position;
    }

    private float GetSpeedModifierFromVerticalDistance(float maxStepSize, float distance)
    {
      // calculate distance to slow zone border
      // map 0 .. x to 0 .. 1f 
      var zoneDistance = Math.Abs(SlowzoneDistance - distance);
      var ammount = 1f;
      if (zoneDistance > 0)
        ammount = 1 / SlowzoneDistance * zoneDistance;

      return MathHelper.Lerp(maxStepSize, 0f, ammount);
    }

    private float GetSpeedModifierFromHorizontalDistance(float maxStepSize, Vector2 ballPosition)
    {
      var ammount = 1f;
      if ((_lastBallPosition - ballPosition).X < 0)
        ammount = 0;

      return MathHelper.Lerp(maxStepSize, 0f, ammount);
    }

    private bool CanMoveUp()
    {
      return ! (_paddle.Position.Y - _paddle.Bounds.Height / 2f <= _worldBounds.LowerBound.Y);
    }

    private bool CanMoveDown()
    {
      return ! (_paddle.Position.Y + _paddle.Bounds.Height / 2f >= _worldBounds.UpperBound.Y);
    }

  }
}