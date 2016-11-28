namespace PongMonoGame
{
  using System;
  using System.Collections.Generic;

  /// <summary>
  ///   Contains event data about UI menu value that changed.
  /// </summary>
  public class OptionsChangedEventArgs : EventArgs
  {
    public LinkedList<ValueMenuItem> Items { get; private set; }

    public OptionsChangedEventArgs(LinkedList<ValueMenuItem> items)
    {
      Items = items;
    }
  }
}