namespace PongMonoGame
{
  using System;
  using Microsoft.Xna.Framework;
  using Microsoft.Xna.Framework.Graphics;

  public class SpriteComponent : Component, IDrawable
  {
    public Texture2D Texture { get; protected set; }

    public bool Visible { get; protected set; } = true;
    public Vector2 Origin { get; protected set; }
    public AABB Bounds { get; protected set; }

    public override ComponentType ComponentType => ComponentType.Model;

    public SpriteComponent(Texture2D texture)
    {
      if (null == texture)
        throw new ArgumentNullException(nameof(texture));
      Texture = texture;
    }

    public override void Init(BaseObject root)
    {
      base.Init(root);
      Bounds = Texture.Bounds.ToAABB();
      Origin = new Vector2(Texture.Width / 2f, Texture.Height / 2f);
    }

    public virtual void DrawSprites(SpritesBatch batch, float timeStep)
    {
      if (!Visible || !Base.Visible)
        return;

      batch.DrawSprite(Texture, Base.Position, origin: Origin, rotation: Base.Rotation);
    }

    public virtual void DrawPrimitives(PrimitivesBatch batch, float timeStep)
    {
    }
  }
}