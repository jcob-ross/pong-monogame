namespace PongMonoGame
{
  using System;
  using Microsoft.Xna.Framework;

  public static class MathUtils
  {
    public static void Swap<T>(ref T a, ref T b)
    {
      var tmp = a;
      a = b;
      b = tmp;
    }

    public static Vector2 Abs(Vector2 v)
    {
      return new Vector2(Math.Abs(v.X), Math.Abs(v.Y));
    }

    /// <summary>
    ///   Return the angle between two vectors on a plane
    ///   The angle is from vector 1 to vector 2, positive anticlockwise
    ///   The result is between -pi -> pi
    /// </summary>
    public static double VectorAngle(ref Vector2 p1, ref Vector2 p2)
    {
      var theta1 = Math.Atan2(p1.Y, p1.X);
      var theta2 = Math.Atan2(p2.Y, p2.X);
      var dtheta = theta2 - theta1;
      while (dtheta > Math.PI)
        dtheta -= 2 * Math.PI;
      while (dtheta < -Math.PI)
        dtheta += 2 * Math.PI;

      return dtheta;
    }

    /// <summary>
    ///   Returns a positive number if c is to the left of the line going from a to b.
    /// </summary>
    /// <returns>
    ///   Positive number if point is left, negative if point is right,
    ///   and 0 if points are collinear.
    /// </returns>
    public static float Area(Vector2 a, Vector2 b, Vector2 c)
    {
      return Area(ref a, ref b, ref c);
    }

    /// <summary>
    ///   Returns a positive number if c is to the left of the line going from a to b.
    /// </summary>
    /// <returns>
    ///   Positive number if point is left, negative if point is right,
    ///   and 0 if points are collinear.
    /// </returns>
    public static float Area(ref Vector2 a, ref Vector2 b, ref Vector2 c)
    {
      return a.X * (b.Y - c.Y) + b.X * (c.Y - a.Y) + c.X * (a.Y - b.Y);
    }

    public static bool IsValid(this Vector2 x)
    {
      return IsValid(x.X) && IsValid(x.Y);
    }

    /// <summary>
    ///   This function is used to ensure that a floating point number is
    ///   not a NaN or infinity.
    /// </summary>
    /// <param name="x">The x.</param>
    /// <returns>
    ///   <c>true</c> if the specified x is valid; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsValid(float x)
    {
      if (float.IsNaN(x))
      {
        // NaN.
        return false;
      }

      return ! float.IsInfinity(x);
    }

    public static bool FloatEquals(float value1, float value2)
    {
      return Math.Abs(value1 - value2) <= Settings.Epsilon;
    }

    /// <summary>
    ///   Checks if a floating point Value is equal to another,
    ///   within a certain tolerance.
    /// </summary>
    /// <param name="value1">The first floating point Value.</param>
    /// <param name="value2">The second floating point Value.</param>
    /// <param name="delta">The floating point tolerance.</param>
    /// <returns>True if the values are "equal", false otherwise.</returns>
    public static bool FloatEquals(float value1, float value2, float delta)
    {
      return FloatInRange(value1, value2 - delta, value2 + delta);
    }

    /// <summary>
    ///   Checks if a floating point Value is within a specified
    ///   range of values (inclusive).
    /// </summary>
    /// <param name="value">The Value to check.</param>
    /// <param name="min">The minimum Value.</param>
    /// <param name="max">The maximum Value.</param>
    /// <returns>
    ///   True if the Value is within the range specified,
    ///   false otherwise.
    /// </returns>
    public static bool FloatInRange(float value, float min, float max)
    {
      return value >= min && value <= max;
    }
  }
}