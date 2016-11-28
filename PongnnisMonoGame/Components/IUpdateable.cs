namespace PongMonoGame
{
  public interface IUpdateable
  {
    bool Enabled { get; }
    void Update(float timeStep);
  }
}