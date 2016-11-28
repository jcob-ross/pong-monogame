namespace PongMonoGame
{
  using Microsoft.Xna.Framework.Graphics;

  public class BallSprite : SpriteComponent
  {
    public BallSprite(Texture2D texture) : base(texture) { }

    public override void DrawPrimitives(PrimitivesBatch batch, float timeStep)
    {
      batch.DrawMarkerSquare(Base.Position, size: 3);
      batch.DrawRectangleVerticesMarkers(Base.Position - Origin, Bounds.Width, Bounds.Height, size: 4);
    }
  }
}