namespace PongMonoGame
{
  using System.Linq;
  using System.Text;
  using Microsoft.Xna.Framework;
  using Microsoft.Xna.Framework.Graphics;
  using Microsoft.Xna.Framework.Input;

  public class BallScene : GameScene
  {
    public BallScene(SceneManager sceneManager, ScreenAdapter screenAdapter) : base("BallScene", sceneManager, screenAdapter) { }
    private BasicEffect _basicEffect;
    private SpriteFont _font;
    public override void LoadContent()
    {
      if (ContentLoaded)
        return;
      _font = SceneManager.ContentManager.Load<SpriteFont>("MainFont");
      Camera = new Camera(SceneManager.Device, SceneManager.Input, ScreenAdapter);

      _basicEffect = new BasicEffect(SceneManager.Device);

      var bTex = SceneManager.ContentManager.Load<Texture2D>("ball");

      var bSprite = new BallSprite(bTex);
      var bBeh = new BallBehavior();
      var ball = new Ball();
      ball.AddComponent(bSprite);
      ball.AddComponent(bBeh);
      Actors.Add(ball);

      base.LoadContent();
    }

    public override void UnloadContent()
    {
      if (! ContentLoaded)
        return;

      Actors.Clear();
      _basicEffect.Dispose();
      _basicEffect = null;

      base.UnloadContent();
    }


    public override void Update(float timeStep)
    {
      base.Update(timeStep);

      if (SceneManager.Input.KeyWasReleased(Keys.F5))
        SceneManager.LoadScene("PongScene");

      SolveScene(timeStep);
      foreach (var actor in Actors)
      {
        if (actor.Enabled)
          actor.Update(timeStep);
      }

    }

    public override void Draw(float timeStep)
    {
      SceneManager.Device.Clear(Color.CornflowerBlue);

      if (!ContentLoaded)
        return;

      _basicEffect.World = Matrix.CreateScale(1, 1, 1) * Matrix.CreateTranslation(Vector3.Zero);
      _basicEffect.Projection = Camera.CreateProjection(Vector2.Zero);

      DrawWorldSprites(timeStep);
      DrawWorldPrimitives(timeStep);

      var ball = Actors.First() as Ball;

      SceneManager.SpritesBatch.Begin();
      var ballScreenPosition = Camera.WorldToScreen(ball?.Position ?? Vector2.Zero);

      var rectangle = Camera.GetRectangle();
      var sb = new StringBuilder();

      sb.AppendLine($"Camera: ");
      sb.AppendLine($"EDSF: Move [{Camera.Position.X:0}, {Camera.Position.Y:0}]");
      sb.AppendLine($"WR: Rotate [{MathHelper.ToDegrees(Camera.Rotation):0.00}]");
      sb.AppendLine($"XV: Zoom [{Camera.Zoom:0.00}]");
      sb.AppendLine($"Viewport: {SceneManager.Device.Viewport}");

      sb.AppendLine($"Mouse: ");
      sb.AppendLine($"Mouse Pos: [{SceneManager.Input.MousePosition.X:0}, {SceneManager.Input.MousePosition.Y:0}]");
      sb.AppendLine($"Mouse World Pos: [{Camera.MouseWorldPosition.X:0}, {Camera.MouseWorldPosition.Y:0}]");
      sb.AppendLine($"Bounds(x,y, width, height): [{rectangle.X:0}, {rectangle.Y:0}, {rectangle.Width:0}, {rectangle.Height:0}]");

      sb.AppendLine($"Ball: ");
      sb.AppendLine($"Ball: [{ball?.Position}]");
      sb.AppendLine($"Ball rotation: [{ball?.Rotation}]");
      sb.AppendLine($"Ball screen pos: [{ballScreenPosition}]");
      
      SceneManager.SpritesBatch.DrawString(_font, new Vector2(5, 5), sb, Color.White);
      SceneManager.SpritesBatch.End();
    }

    private void SolveScene(float timeStep)
    {
      if (!ContentLoaded)
        return;

    }

    private void DrawWorldSprites(float timeStep)
    {
      _basicEffect.View = Camera.GetViewMatrix(Vector2.One);
      _basicEffect.TextureEnabled = true;
      _basicEffect.VertexColorEnabled = true;

      SceneManager.SpritesBatch.Begin(_basicEffect);
      foreach (var actor in Actors)
      {
        if (actor.Visible)
          actor.DrawSprites(SceneManager.SpritesBatch, timeStep);
      }
      SceneManager.SpritesBatch.End();
    }

    private void DrawWorldPrimitives(float timeStep)
    {
      _basicEffect.View = Camera.GetViewMatrix(Vector2.One);
      _basicEffect.TextureEnabled = false;
      _basicEffect.VertexColorEnabled = true;

      SceneManager.PrimitivesBatch.Begin(_basicEffect);
      foreach (var actor in Actors)
      {
        if (actor.Visible)
          actor.DrawPrimitives(SceneManager.PrimitivesBatch, timeStep);
      }
      SceneManager.PrimitivesBatch.End();
    }

    private void DrawUiSprites()
    {
      _basicEffect.View = Matrix.Identity;
      _basicEffect.TextureEnabled = true;
      _basicEffect.VertexColorEnabled = true;

      SceneManager.SpritesBatch.Begin(_basicEffect);

      SceneManager.SpritesBatch.End();
    }

    private void DrawUiPrimitives()
    {
      _basicEffect.View = Matrix.Identity;
      _basicEffect.TextureEnabled = false;
      _basicEffect.VertexColorEnabled = true;
      SceneManager.PrimitivesBatch.Begin(_basicEffect);
      
      SceneManager.PrimitivesBatch.End();
    }
  }
}