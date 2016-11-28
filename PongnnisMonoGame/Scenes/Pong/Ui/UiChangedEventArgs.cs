namespace PongMonoGame
{
  using System;

  /// <summary>
  ///   Holds event data about change in user interface
  /// </summary>
  public class UiChangedEventArgs : EventArgs
  { public SceneUi UserInterface { get; private set; }

    public UiChangedEventArgs(SceneUi userInterface)
    {
      UserInterface = userInterface;
    }
  }
}