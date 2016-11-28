namespace PongMonoGame
{
  using Microsoft.Xna.Framework;

  /// <summary>
  ///   Collection of helper methods for misc collisions.
  ///   Does float tolerance and line collisions with lines and AABBs.
  /// </summary>
  public static class LineTools
  {
    public static float DistancePointLineSegment(Vector2 point, Vector2 start, Vector2 end, out Vector2 pointOnLine)
    {
      pointOnLine = Vector2.Zero;

      if (start == end)
        return Vector2.Distance(point, start);

      var v = Vector2.Subtract(end, start);
      var w = Vector2.Subtract(point, start);

      var c1 = Vector2.Dot(w, v);
      if (c1 <= 0) return Vector2.Distance(point, start);

      var c2 = Vector2.Dot(v, v);
      if (c2 <= c1) return Vector2.Distance(point, end);

      var b = c1 / c2;
      pointOnLine = Vector2.Add(start, Vector2.Multiply(v, b));
      return Vector2.Distance(point, pointOnLine);
    }
  }
}