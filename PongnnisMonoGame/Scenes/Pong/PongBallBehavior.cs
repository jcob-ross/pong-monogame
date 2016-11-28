namespace PongMonoGame
{
  using System;
  using Microsoft.Xna.Framework;
  using Microsoft.Xna.Framework.Input;

  /// <summary>
  ///   Represents behavior component of the <see cref="PongBall"/>
  /// </summary>
  public class PongBallBehavior : Component, IUpdateable
  {
    private const float Speed = 350f / 1000;
    private const float MaxSpeedMultiplier = 4f;
    private readonly float _radius;
    private readonly PongScene _world;

    private float _collisionPenetrationDepth;

    private Vector2 _direction = new Vector2(1f, 0f);
    private int _paddleHitCounter;
    private float _speedMultiplier = 1f;

    /// <summary>
    ///   Creates new instance of <see cref="PongBallBehavior"/>
    /// </summary>
    /// <param name="radius">Radius of the ball. (size of side in this case)</param>
    /// <param name="world"><see cref="PongScene"/> the ball exists in</param>
    public PongBallBehavior(float radius, PongScene world)
    {
      if (null == world)
        throw new ArgumentNullException(nameof(world));
      _world = world;
      _radius = radius;
      Direction.Normalize();
    }

    /// <summary>
    ///   Indicates whether the ball was fired and is flying
    /// </summary>
    public bool Fired { get; set; }
    public override ComponentType ComponentType => ComponentType.Behavior;

    /// <summary>
    ///   The direction the ball is traveling in.
    /// </summary>
    public Vector2 Direction
    {
      get { return _direction; }
      set
      {
        value.Normalize();
        _direction = value;
      }
    }

    /// <summary>
    ///   Indicates whether the <see cref="PongBallBehavior"/> should be updated
    /// </summary>
    public bool Enabled { get; set; } = true;

    public void Update(float timeStep)
    {
      if (_world.Input.KeyWasReleased(Keys.Space))
        Fired = true;

      if (! Fired)
        return;

      HandlePaddleCollision();
      HandleWorldBoundsCollision();

      Base.Move(Direction * _speedMultiplier * Speed * timeStep);
    }

    public event Action<GoalEventArgs> PlayerScored;

    private void HandleWorldBoundsCollision()
    {
      if (Base.Position.X - _radius < _world.WorldBounds.LowerBound.X) // left wall collision
      {
        PlayCollisionSound(CollisionType.GoalZone);
        _collisionPenetrationDepth = _world.WorldBounds.LowerBound.X - (Base.Position.X - _radius);
        Direction = new Vector2(-Direction.X, Direction.Y);
        Base.Move(new Vector2(_collisionPenetrationDepth * 2, 0));
        OnPlayerScored(new GoalEventArgs(PlayerSide.Right));
      }
      if (Base.Position.X + _radius > _world.WorldBounds.UpperBound.X) // right wall collision
      {
        PlayCollisionSound(CollisionType.GoalZone);
        _collisionPenetrationDepth = Base.Position.X + _radius - _world.WorldBounds.UpperBound.X;
        Direction = new Vector2(-Direction.X, Direction.Y);
        Base.Move(new Vector2(-_collisionPenetrationDepth * 2, 0));
        OnPlayerScored(new GoalEventArgs(PlayerSide.Left));
      }
      if (Base.Position.Y - _radius < _world.WorldBounds.LowerBound.Y) // top wall collision
      {
        PlayCollisionSound(CollisionType.Wall);
        _collisionPenetrationDepth = _world.WorldBounds.LowerBound.Y - (Base.Position.Y - _radius);
        Direction = new Vector2(Direction.X, -Direction.Y);
        Base.Move(new Vector2(0, _collisionPenetrationDepth * 2));
      }
      if (Base.Position.Y + _radius > _world.WorldBounds.UpperBound.Y) // bottom wall collision
      {
        PlayCollisionSound(CollisionType.Wall);
        _collisionPenetrationDepth = Base.Position.Y + _radius - _world.WorldBounds.UpperBound.Y;
        Direction = new Vector2(Direction.X, -Direction.Y);
        Base.Move(new Vector2(0, -_collisionPenetrationDepth * 2));
      }
    }

    private void PlayCollisionSound(CollisionType collisionType)
    {
      switch (collisionType)
      {
        case CollisionType.Wall:
          _world.SoundPlayer.PlaySound("8bit_plop", "collision_wall");
          break;

        case CollisionType.Paddle:
          _world.SoundPlayer.PlaySound("8bit_plop", "collision_paddle", 1f);
          break;

        case CollisionType.GoalZone:
          _world.SoundPlayer.PlaySound("8bit_plop", "collision_goalzone", -1f);
          break;
      }
    }

    private void HandlePaddleCollision()
    {
      var leftPaddle = _world.LeftPaddle;
      var rightPaddle = _world.RightPaddle;

      var manifold = new Manifold();
      if (leftPaddle.IsColliding(Base, Direction, ref manifold) ||
          rightPaddle.IsColliding(Base, Direction, ref manifold))
      {
        _paddleHitCounter += 1;
        if (_paddleHitCounter % 4 == 0)
        {
          _speedMultiplier += .2f;
          _speedMultiplier = MathHelper.Clamp(_speedMultiplier, 1f, MaxSpeedMultiplier);
        }

        PlayCollisionSound(CollisionType.Paddle);
        Base.Move(manifold.Normal * (manifold.PenetrationDepth * 2 + 0.5f));
        Direction = manifold.NewDirection;
      }
    }

    protected virtual void OnPlayerScored(GoalEventArgs obj)
    {
      PlayerScored?.Invoke(obj);
      _paddleHitCounter = 0;
      _speedMultiplier = 1f;
    }
    
    private enum CollisionType
    {
      None = 0,
      Wall,
      Paddle,
      GoalZone
    }
  }
}