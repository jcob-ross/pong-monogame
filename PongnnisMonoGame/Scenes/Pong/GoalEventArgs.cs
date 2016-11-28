namespace PongMonoGame
{
  using System;

  /// <summary>
  ///   Contains event data about goal that was scored
  /// </summary>
  public class GoalEventArgs : EventArgs
  {
    /// <summary>
    ///   Side the winning point should go to
    /// </summary>
    public PlayerSide ScoringPlayer;

    /// <summary>
    ///   Creates new instance of <see cref="GoalEventArgs"/>
    /// </summary>
    /// <param name="scoringPlayer">The <see cref="PlayerSide"/> that scored the point</param>
    public GoalEventArgs(PlayerSide scoringPlayer)
    {
      ScoringPlayer = scoringPlayer;
    }
  }
}