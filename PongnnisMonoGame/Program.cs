namespace PongMonoGame
{
  using System;
  using System.Windows.Forms;

  public static class Program
  {
    [STAThread]
    private static void Main()
    {
      using (var game = new PongGame())
      {
        try
        {
          game.Run();
        }
        catch (Exception e)
        {
          MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
          throw;
        }
      }
    }
  }
}