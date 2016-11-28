namespace PongMonoGame
{
  using Microsoft.Xna.Framework;
  using Microsoft.Xna.Framework.Input;

  public class BallBehavior : Component, IUpdateable
  {
    public override ComponentType ComponentType => ComponentType.Behavior;
    public bool Enabled { get; private set; } = true;
    public void Update(float timeStep)
    {
      var keyState = Keyboard.GetState();
      
      if (keyState.IsKeyDown(Keys.Left))
        Base.Move(new Vector2(-1f, 0f));
      if (keyState.IsKeyDown(Keys.Right))
        Base.Move(new Vector2(1f, 0f));
      if (keyState.IsKeyDown(Keys.Up))
        Base.Move(new Vector2(0f, -1f));
      if (keyState.IsKeyDown(Keys.Down))
        Base.Move(new Vector2(0f, 1f));

      if (keyState.IsKeyDown(Keys.PageUp))
        Base.Rotate(0.01f);
      if (keyState.IsKeyDown(Keys.PageDown))
        Base.Rotate(-0.01f);

      if (keyState.IsKeyDown(Keys.Home))
        Reset();
    }

    private void Reset()
    {
      Base.Move(-Base.Position);
      Base.Rotate(-Base.Rotation);
    }
  }
}