namespace PongMonoGame
{
  using System;
  using System.Collections.Generic;

  /// <summary>
  ///   Base class of single game scene
  /// </summary>
  public abstract class GameScene
  {
    /// <summary>
    ///   Name of the scene
    /// </summary>
    public string Name { get; }
    protected readonly SceneManager SceneManager;
    protected readonly List<BaseObject> Actors = new List<BaseObject>(64);
    protected Camera Camera;

    /// <summary>
    ///   <c>True</c> if content was loaded, <c>False</c> otherwise
    /// </summary>
    public bool ContentLoaded { get; private set; }

    /// <summary>
    ///   Indicates whether the scene should be updated
    /// </summary>
    public bool Enabled { get; protected set; } = true;

    /// <summary>
    ///   Indicates whether the scene should be drawed
    /// </summary>
    public bool Visible { get; protected set; } = true;

    /// <summary>
    ///   Screen adapter for <see cref="Camera"/> to use
    /// </summary>
    public ScreenAdapter ScreenAdapter { get; private set; }

    protected GameScene(string name, SceneManager sceneManager, ScreenAdapter screenAdapter)
    {
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentNullException(nameof(name));
      if (null == sceneManager)
        throw new ArgumentNullException(nameof(sceneManager));
      if (null == screenAdapter)
        throw new ArgumentNullException(nameof(screenAdapter));
      
      Name = name;
      SceneManager = sceneManager;
      ScreenAdapter = screenAdapter;
    }

    /// <summary>
    ///   Loads assets necessary for the scene
    /// </summary>
    public virtual void LoadContent()
    {
      if (ContentLoaded)
        throw new InvalidOperationException("Cannot load content more than once before unloading");

      ContentLoaded = true;
    }


    /// <summary>
    ///   Unloads scene's assets
    /// </summary>
    public virtual void UnloadContent()
    {
      if (!ContentLoaded)
        throw new InvalidOperationException("Content must be loaded first before unloading");

      ContentLoaded = false;
      GC.Collect();
    }

    /// <summary>
    ///   Updates the scene
    /// </summary>
    /// <param name="timeStep">Time delta from the last call to the <see cref="Update"/></param>
    public virtual void Update(float timeStep)
    {
      if (!ContentLoaded)
        throw new InvalidOperationException("Content is not loaded");

      Camera.Update(timeStep);
    }

    /// <summary>
    ///   Draws the scene
    /// </summary>
    /// <param name="timeStep">Time delta from the last call to the <see cref="Update"/></param>
    public virtual void Draw(float timeStep)
    {
      if (!ContentLoaded)
        throw new InvalidOperationException("Content is not loaded");
    }
  }
}