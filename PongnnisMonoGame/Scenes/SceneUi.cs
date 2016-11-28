namespace PongMonoGame
{
  using System;

  /// <summary>
  ///   User interface base class
  /// </summary>
  public abstract class SceneUi : IDisposable
  {
    protected readonly SceneManager SceneManager;
    protected readonly Camera Camera;
    protected SceneUi(SceneManager sceneManager, Camera camera)
    {
      if (null == sceneManager)
        throw new ArgumentNullException(nameof(sceneManager));
      if (null == camera)
        throw new ArgumentNullException(nameof(camera));
      
      SceneManager = sceneManager;
      Camera = camera;
    }

    public abstract void LoadContent();
    public abstract void UnloadContent();

    public abstract void Update(float timeStep);
    public abstract void Draw(float timeStep);
    public abstract void Dispose();
  }
}