namespace PongMonoGame
{
  public abstract class Component
  {
    private BaseObject _baseObject;
    protected BaseObject Base => _baseObject;
    public abstract ComponentType ComponentType { get; }

    public virtual void Init(BaseObject root)
    {
      _baseObject = root;
    }

    public virtual void RemoveMe()
    {
      _baseObject.RemoveComponent(this);
    }
    public T GetComponent<T>(ComponentType componentType) where T : Component
    {
      return _baseObject.GetComponent<T>(componentType);
    }
  }
}