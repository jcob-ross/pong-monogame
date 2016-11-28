namespace PongMonoGame
{
  using System;
  using Microsoft.Xna.Framework;
  using Microsoft.Xna.Framework.Input;

  /// <summary>
  ///   Represents player controller component for manipulating <see cref="Paddle"/>
  /// </summary>
  public class PaddlePlayerBehavior : Component, IUpdateable
  {
    private const float Speed = 250f;
    private Paddle _paddle;
    private readonly InputManager _input;
    private readonly AABB _worldBounds;
    public override ComponentType ComponentType => ComponentType.Behavior;
    public bool Enabled { get; private set; } = true;

    /// <summary>
    ///   Creates new instance of <see cref="PaddlePlayerBehavior"/>
    /// </summary>
    /// <param name="input"><see cref="InputManager"/> for user input interaction</param>
    /// <param name="worldBounds">Dimmensions of the world</param>
    public PaddlePlayerBehavior(InputManager input, AABB worldBounds)
    {
      _input = input;
      _worldBounds = worldBounds;
    }

    /// <summary>
    ///   Initializes <see cref="PaddlePlayerBehavior"/>.
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
      var stepSize = (Speed / 1000) * timeStep;

      if (_input.IsKeyDown(Keys.Up) && CanMoveUp())
        _paddle.Move(new Vector2(0f, -1f) * stepSize);
      if (_input.IsKeyDown(Keys.Down) && CanMoveDown())
        _paddle.Move(new Vector2(0f, 1f) * stepSize);
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