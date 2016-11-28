namespace PongMonoGame
{
  /// <summary>
  ///   Drawable component interface
  /// </summary>
  public interface IDrawable
  {
    bool Visible { get; }
    void DrawSprites(SpritesBatch batch, float timeStep);
    void DrawPrimitives(PrimitivesBatch batch, float timeStep);
  }
}