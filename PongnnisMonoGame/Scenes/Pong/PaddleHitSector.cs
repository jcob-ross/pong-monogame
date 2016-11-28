namespace PongMonoGame
{
  /// <summary>
  ///   Represents area of the paddle the ball can hit.
  ///   Used for collision response.
  /// </summary>
  public enum PaddleHitSector
  {
    None, 
    OuterUp,
    InnerUp,
    InnerDown,
    OuterDown
  }
}