namespace PongMonoGame
{
  using Microsoft.Xna.Framework;

  /// <summary>
  ///   Represents selectable UI item element that holds a value
  /// </summary>
  public class ValueMenuItem : MenuItem
  {
    private readonly float _stepSize;
    private readonly float _min;
    private readonly float _max;

    /// <summary>
    ///   Value the menu item wraps
    /// </summary>
    public float Value { get; private set; }
    
    /// <summary>
    ///   Creates new instance of <see cref="ValueMenuItem"/>
    /// </summary>
    /// <param name="text">Text of the item</param>
    /// <param name="value">Acompanying value of the item</param>
    /// <param name="stepSize">
    ///   Size of the step when <see cref="ValueMenuItem.Increment"/> and <see cref="ValueMenuItem.Decrement"/>
    ///   are called
    /// </param>
    /// <param name="min">Minimum value the item can hold</param>
    /// <param name="max">Maximum value the item can hold</param>
    /// <param name="measuredSize">Measured size of the text</param>
    /// <param name="childUi">Optional child Ui</param>
    public ValueMenuItem(string text, float value, float stepSize, float min, float max, Vector2 measuredSize, SceneUi childUi = null) 
      : base(text, measuredSize, childUi)
    {
      _stepSize = stepSize;
      _min = min;
      _max = max;
      Value = value;
    }

    /// <summary>
    ///   Increments <see cref="ValueMenuItem.Value"/> by step size, up to max, both specified in the ctor.
    /// </summary>
    public void Increment()
    {
      Value += _stepSize;
      Value = MathHelper.Clamp(Value, _min, _max);
    }

    /// <summary>
    ///   Decrements <see cref="ValueMenuItem.Value"/> by step size, up to min, both specified in the ctor.
    /// </summary>
    public void Decrement()
    {
      Value -= _stepSize;
      Value = MathHelper.Clamp(Value, _min, _max);
    }
  }
}