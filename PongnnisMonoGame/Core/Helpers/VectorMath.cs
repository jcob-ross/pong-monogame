namespace PongMonoGame
{
  using System;
  using Microsoft.Xna.Framework;

  /// <summary>
  ///   Contains common mathematical operations with Vectors
  /// </summary>
  public static class VectorMath
  {
    /// <summary>
    ///   Computes intersection point of two lines
    /// </summary>
    /// <param name="p1">Start of first line</param>
    /// <param name="p2">End of first line</param>
    /// <param name="q1">Start of second line</param>
    /// <param name="q2">End of second line</param>
    /// <returns>Intersection point as <see cref="Vector2" /></returns>
    public static Vector2 LineLineIntersect(Vector2 p1, Vector2 p2, Vector2 q1, Vector2 q2)
    {
      // http://http://paulbourke.net/geometry/
      var s = ((q2.X - q1.X) * (p1.Y - q1.Y) - (q2.Y - q1.Y) * (p1.X - q1.X)) /
              ((q2.Y - q1.Y) * (p2.X - p1.X) - (q2.X - q1.X) * (p2.Y - p1.Y));

      return new Vector2(p1.X + s * (p2.X - p1.X), p1.Y + s * (p2.Y - p1.Y));
    }

    /// <summary>
    ///   Checks if point is on the left side of a line
    /// </summary>
    /// <param name="point">Point to check</param>
    /// <param name="p1">Start of the line</param>
    /// <param name="p2">End of the line</param>
    /// <remarks>
    ///   Does not consider colinearity.
    /// </remarks>
    /// <returns><c>true</c> if the point is on the left side, false otherwise</returns>
    public static bool IsLeftOf(Vector2 point, Vector2 p1, Vector2 p2)
    {
      var cross = (p2.X - p1.X) * (point.Y - p1.Y) -
                  (p2.Y - p1.Y) * (point.X - p1.X);

      return cross < 0;
    }

    public static Vector2 NormalizeVector2(Vector2 vector)
    {
      float num = 1f / (float)Math.Sqrt(vector.X * (double)vector.X + vector.Y * (double)vector.Y);
      return new Vector2(vector.X * num, vector.Y * num);
    }

    public static Vector2 Vector2Rotate(Vector2 direction, float radians)
    {
      // x*cos(T) - y*sin(T), x*sin(T) + y*cos(T)
      return new Vector2((float)(direction.X * Math.Cos(radians) - direction.Y * Math.Sin(radians)),
                         (float)(direction.X * Math.Sin(radians) + direction.Y * Math.Cos(radians)));
    }

    public static float Vector2Magnitude(Vector2 start, Vector2 end)
    {
      var direction = end - start;
      return direction.Length();
    }

    public static Vector2 Vector2Direction(Vector2 start, Vector2 end, out float magnitude)
    {
      var direction = end - start;
      magnitude = direction.Length();
      direction.Normalize();
      return direction;
    }
    public static Vector2 Vector2Direction(Vector2 start, Vector2 end)
    {
      var direction = end - start;
      direction.Normalize();
      return direction;
    }

    public static void Vector2Normal(Vector2 direction, out Vector2 leftHanded, out Vector2 rightHanded)
    {
      leftHanded = new Vector2(direction.Y, -direction.X);
      rightHanded = new Vector2(-direction.Y, direction.X);
    }

    public static Vector2 Vector2NormalLeftHanded(Vector2 direction)
    {
      return new Vector2(direction.Y, -direction.X); // CW rotation
    }
    public static Vector2 Vector2NormalRightHanded(Vector2 direction)
    {
      return new Vector2(-direction.Y, direction.X); // CCW rotation
    }
  }
}