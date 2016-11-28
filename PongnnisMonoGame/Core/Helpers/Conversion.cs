namespace PongMonoGame
{
  using Microsoft.Xna.Framework;

  public static class Conversion
  {
    public static Rectangle ToRectangle(this AABB aabb)
    {
      return new Rectangle((int) aabb.LowerBound.X, (int) aabb.LowerBound.Y,
                           (int) aabb.UpperBound.X, (int) aabb.UpperBound.Y);
    }


    public static AABB ToAABB(this Rectangle rectangle)
    {
      var minimum = rectangle.Location.ToVector2();
      var maximum = minimum + new Vector2(rectangle.Width, rectangle.Height);
      return new AABB(minimum, maximum);
    }
  }
}