namespace PongMonoGame
{
  using Microsoft.Xna.Framework;

  public class Ball : BaseObject
  {
    public Ball(string name = "") : base(name)
    {
      Position = Vector2.Zero;
      Rotation = 0f;
      Enabled = true;
      Visible = true;
    }
  }
}