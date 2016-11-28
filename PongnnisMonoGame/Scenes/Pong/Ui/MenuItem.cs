namespace PongMonoGame
{
  using System;
  using Microsoft.Xna.Framework;
  using Microsoft.Xna.Framework.Graphics;

  /// <summary>
  ///   Represents selectable UI item element
  /// </summary>
  public class MenuItem
  {
    /// <summary>
    ///   <c>True</c> if the item is currently selected, <c>False</c> otherwise.
    /// </summary>
    public bool Selected;

    /// <summary>
    ///   Text data of the element.
    /// </summary>
    public string Text { get; }

    /// <summary>
    ///   Measured text of the element. Result of <see cref="SpriteFont.MeasureString(string)"/>
    /// </summary>
    public Vector2 MeasuredSize { get; }

    /// <summary>
    ///   Child UI element
    /// </summary>
    public SceneUi ChildUi { get; }
    public bool HasChildUi => ChildUi != null;
    
    /// <summary>
    ///   Creates new instance of <see cref="MenuItem"/>
    /// </summary>
    /// <param name="text">Text of the item</param>
    /// <param name="measuredSize">Measured size of the text</param>
    /// <param name="childUi">Optional child UI</param>
    public MenuItem(string text, Vector2 measuredSize, SceneUi childUi = null)
    {
      Text = text;
      ChildUi = childUi;
      MeasuredSize = measuredSize;
    }

    /// <summary>
    ///   Action to be taken upon item selection.
    /// </summary>
    public Action SelectAction;
  }
}