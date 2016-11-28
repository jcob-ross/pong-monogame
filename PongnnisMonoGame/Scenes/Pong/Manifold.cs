namespace PongMonoGame
{
  using Microsoft.Xna.Framework;

  /// <summary>
  ///   Manifold holds information about collision event
  /// </summary>
  public struct Manifold
  {
    /// <summary>
    ///   Normal vector of the contact
    /// </summary>
    public Vector2 Normal;
    /// <summary>
    ///   New direction the ball should take
    /// </summary>
    public Vector2 NewDirection;
    /// <summary>
    ///   Depth the ball penetrated into paddle
    /// </summary>
    public float PenetrationDepth;
  }
}