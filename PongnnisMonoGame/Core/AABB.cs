namespace PongMonoGame
{
  using Microsoft.Xna.Framework;

  /// <summary>
  /// An axis aligned bounding box.
  /// </summary>
  public struct AABB
  {
    /// <summary>
    ///   The lower vertex
    /// </summary>
    public Vector2 LowerBound;

    /// <summary>
    ///   The upper vertex
    /// </summary>
    public Vector2 UpperBound;

    public AABB(Vector2 min, Vector2 max)
        : this(ref min, ref max)
    {
    }

    public AABB(ref Vector2 min, ref Vector2 max)
    {
      LowerBound = min;
      UpperBound = max;
    }

    public AABB(Vector2 center, float width, float height)
    {
      LowerBound = center - new Vector2(width / 2, height / 2);
      UpperBound = center + new Vector2(width / 2, height / 2);
    }

    public float Width => UpperBound.X - LowerBound.X;

    public float Height => UpperBound.Y - LowerBound.Y;

    /// <summary>
    ///   Get the center of the AABB.
    /// </summary>
    public Vector2 Center => 0.5f * (LowerBound + UpperBound);

    /// <summary>
    /// Get the extents of the AABB (half-widths).
    /// </summary>
    public Vector2 Extents => 0.5f * (UpperBound - LowerBound);

    /// <summary>
    ///   Get the perimeter length
    /// </summary>
    public float Perimeter
    {
      get
      {
        float wx = UpperBound.X - LowerBound.X;
        float wy = UpperBound.Y - LowerBound.Y;
        return 2.0f * (wx + wy);
      }
    }

    /// <summary>
    ///   Offsets AABB by specified ammount
    /// </summary>
    /// <param name="ammount">Distance to offset</param>
    public void Offset(Vector2 ammount)
    {
      LowerBound += ammount;
      UpperBound += ammount;
    }

    public AABB OffsetCreate(Vector2 ammount)
    {
      return new AABB(LowerBound + ammount, UpperBound + ammount);
    }

    /// <summary>
    /// First quadrant
    /// </summary>
    public AABB Q1 => new AABB(Center, UpperBound);

    /// <summary>
    /// Second quadrant
    /// </summary>
    public AABB Q2 => new AABB(new Vector2(LowerBound.X, Center.Y), new Vector2(Center.X, UpperBound.Y));

    /// <summary>
    /// Third quadrant
    /// </summary>
    public AABB Q3 => new AABB(LowerBound, Center);

    /// <summary>
    /// Forth quadrant
    /// </summary>
    public AABB Q4 => new AABB(new Vector2(Center.X, LowerBound.Y), new Vector2(UpperBound.X, Center.Y));

    /// <summary>
    /// Verify that the bounds are sorted. And the bounds are valid numbers (not NaN).
    /// </summary>
    /// <returns>
    /// 	<c>true</c> if this instance is valid; otherwise, <c>false</c>.
    /// </returns>
    public bool IsValid()
    {
      var d = UpperBound - LowerBound;
      var valid = d.X >= 0.0f && d.Y >= 0.0f;
      valid = valid && LowerBound.IsValid() && UpperBound.IsValid();
      return valid;
    }

    /// <summary>
    ///   Combine an AABB into this one.
    /// </summary>
    /// <param name="aabb">The aabb.</param>
    public void Combine(ref AABB aabb)
    {
      LowerBound = Vector2.Min(LowerBound, aabb.LowerBound);
      UpperBound = Vector2.Max(UpperBound, aabb.UpperBound);
    }

    /// <summary>
    ///   Combine two AABBs into this one.
    /// </summary>
    /// <param name="aabb1">The aabb1.</param>
    /// <param name="aabb2">The aabb2.</param>
    public void Combine(ref AABB aabb1, ref AABB aabb2)
    {
      LowerBound = Vector2.Min(aabb1.LowerBound, aabb2.LowerBound);
      UpperBound = Vector2.Max(aabb1.UpperBound, aabb2.UpperBound);
    }

    /// <summary>
    ///   Does this aabb contain the provided AABB.
    /// </summary>
    /// <param name="aabb">The aabb.</param>
    /// <returns>
    /// 	<c>true</c> if it contains the specified aabb; otherwise, <c>false</c>.
    /// </returns>
    public bool Contains(ref AABB aabb)
    {
      var result = true;
      result = LowerBound.X <= aabb.LowerBound.X;
      result = result && LowerBound.Y <= aabb.LowerBound.Y;
      result = result && aabb.UpperBound.X <= UpperBound.X;
      result = result && aabb.UpperBound.Y <= UpperBound.Y;
      return result;
    }

    /// <summary>
    ///   Determines whether the AAABB contains the specified point.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <returns>
    /// 	<c>true</c> if it contains the specified point; otherwise, <c>false</c>.
    /// </returns>
    public bool Contains(ref Vector2 point)
    {
      //using epsilon to try and gaurd against float rounding errors.
      return (point.X > (LowerBound.X + Settings.Epsilon) && point.X < (UpperBound.X - Settings.Epsilon) &&
             (point.Y > (LowerBound.Y + Settings.Epsilon) && point.Y < (UpperBound.Y - Settings.Epsilon)));
    }

    /// <summary>
    ///   Test if the two AABBs overlap.
    /// </summary>
    /// <param name="a">The first AABB.</param>
    /// <param name="b">The second AABB.</param>
    /// <returns>True if they are overlapping.</returns>
    public static bool TestOverlap(ref AABB a, ref AABB b)
    {
      var d1 = b.LowerBound - a.UpperBound;
      var d2 = a.LowerBound - b.UpperBound;

      if (d1.X > 0.0f || d1.Y > 0.0f)
        return false;

      if (d2.X > 0.0f || d2.Y > 0.0f)
        return false;

      return true;
    }
  }
}