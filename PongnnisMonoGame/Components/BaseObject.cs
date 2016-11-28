namespace PongMonoGame
{
  using System;
  using System.Collections.Generic;
  using Microsoft.Xna.Framework;

  public interface ICollidable
  {
    Vector2 Position { get;}
    AABB GetAABB();
  }

  /// <summary>
  ///   Base class of the entity component system
  /// </summary>
  public class BaseObject : IDrawable, IUpdateable
  {
    private readonly List<Component> _components = new List<Component>();
    public bool Enabled { get; set; } = true;
    public bool Visible { get; set; } = true;
    public Vector2 Position { get; protected set; } = Vector2.Zero;
    public float Rotation { get; protected set; }
    public string Name { get; private set; }
    public AABB Bounds { get; protected set; }

    public BaseObject(string name = "")
    {
      Name = name; 
    }

    /// <summary>
    ///   Gets axis aligned bounding box of the entity
    /// </summary>
    /// <returns></returns>
    public virtual AABB GetAABB()
    {
      return Bounds;
    }

    public void AddComponent(Component component)
    {
      if (null == component)
        throw new ArgumentNullException(nameof(component));
      
      if (!_components.Contains(component))
        _components.Add(component);
      component.Init(this);
    }

    public void RemoveComponent(Component component)
    {
      _components.Remove(component);
    }

    public T GetComponent<T>(ComponentType componentType) where T : Component
    {
      return _components.Find(c => c.ComponentType == componentType) as T;
    }

    public virtual void Move(Vector2 offset)
    {
      Position += offset;
    }

    public virtual void Rotate(float radians, bool localRotation = false)
    {
      Rotation += radians;
    }

    public virtual void Reset(bool localRotation = false)
    {
      Move(-Position);
      Rotate(0f, localRotation);
    }

    public virtual void DrawSprites(SpritesBatch batch, float timeStep)
    {
      if (!Visible)
        return;

      for (var i = 0; i < _components.Count; ++i)
        (_components[i] as IDrawable)?.DrawSprites(batch, timeStep);
    }

    public virtual void DrawPrimitives(PrimitivesBatch batch, float timeStep)
    {
      if (!Visible)
        return;

      for (var i = 0; i < _components.Count; ++i)
        (_components[i] as IDrawable)?.DrawPrimitives(batch, timeStep);
    }


    public virtual void Update(float timeStep)
    {
      if (! Enabled)
        return;

      for (var i = 0; i < _components.Count; ++i)
        (_components[i] as IUpdateable)?.Update(timeStep);
    }
  }
}