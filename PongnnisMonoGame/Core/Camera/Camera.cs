namespace PongMonoGame
{
  using System;
  using Microsoft.Xna.Framework;
  using Microsoft.Xna.Framework.Graphics;
  using Microsoft.Xna.Framework.Input;

  /// <summary>
  ///   Represents game's camera TODO - camera cleanup
  /// </summary>
  public class Camera
  {
    private readonly GraphicsDevice _graphicsDevice;
    private readonly InputManager _inputManager;
    private readonly ScreenAdapter _screenAdapter;

    private float _maximumZoom = float.MaxValue;
    private float _minimumZoom;
    private float _zoom;

    public Camera(GraphicsDevice graphicsDevice, InputManager inputManager, ScreenAdapter screenAdapter)
    {
      if (null == graphicsDevice)
        throw new ArgumentNullException(nameof(graphicsDevice));
      if (null == inputManager)
        throw new ArgumentNullException(nameof(inputManager));
      if (null == screenAdapter)
        throw new ArgumentNullException(nameof(screenAdapter));
      
      _screenAdapter = screenAdapter;
      _graphicsDevice = graphicsDevice;
      _inputManager = inputManager;

      Origin = Vector2.Zero;
      Position = Vector2.Zero;
      Rotation = 0f;
      Zoom = 1f;
    }

    public Vector2 Position { get; set; }
    public float Rotation { get; set; }
    public Vector2 Origin { get; set; }

    public float Zoom
    {
      get { return _zoom; }
      set
      {
        if (value < MinimumZoom || value > MaximumZoom)
          return;
        _zoom = value;
      }
    }

    public float MinimumZoom
    {
      get { return _minimumZoom; }
      set
      {
        if (value < 0)
          return;
        if (Zoom < value)
          Zoom = MinimumZoom;

        _minimumZoom = value;
      }
    }

    public float MaximumZoom
    {
      get { return _maximumZoom; }
      set
      {
        if (value < 0)
          return;
        if (Zoom > value)
          Zoom = value;
        _maximumZoom = value;
      }
    }

    public Vector2 MouseWorldPosition => ScreenToWorld(new Vector2(_inputManager.MousePosition.X,
                                                                   _inputManager.MousePosition.Y));

    public bool Enabled { get; set; }

    public void Move(Vector2 direction)
    {
      Position += Vector2.Transform(direction, Matrix.CreateRotationZ(-Rotation));
    }

    public void Rotate(float deltaRadians)
    {
      Rotation += deltaRadians;
    }

    public void ZoomIn(float deltaZoom)
    {
      ClampZoom(Zoom + deltaZoom);
    }

    public void ZoomOut(float deltaZoom)
    {
      ClampZoom(Zoom - deltaZoom);
    }

    private void ClampZoom(float value)
    {
      if (value < MinimumZoom)
        Zoom = MinimumZoom;
      else if (value > MaximumZoom)
        Zoom = MaximumZoom;
      else
        Zoom = value;
    }

    public void LookAt(Vector2 position)
    {
      Position = position - new Vector2(_screenAdapter.VirtualWidth / 2f, _screenAdapter.VirtualHeight / 2f);
    }

    public Vector2 WorldToScreen(float x, float y)
    {
      return WorldToScreen(new Vector2(x, y));
    }

    public Vector2 WorldToScreen(Vector2 worldPosition)
    {
      var viewport = _screenAdapter.Viewport;
      var viewPortOffset = worldPosition + new Vector2(viewport.X, viewport.Y);
      viewPortOffset += new Vector2(_screenAdapter.VirtualWidth / 2f, _screenAdapter.VirtualHeight / 2f);
      return Vector2.Transform(viewPortOffset, GetViewMatrix());
    }

    public Vector2 ScreenToWorld(float x, float y)
    {
      return ScreenToWorld(new Vector2(x, y));
    }

    public Vector2 ScreenToWorld(Vector2 screenPosition)
    {
      var viewport = _screenAdapter.Viewport;
      var position = screenPosition - new Vector2(viewport.X, viewport.Y);
      position -= new Vector2(_screenAdapter.VirtualWidth / 2f, _screenAdapter.VirtualHeight / 2f);
      var transform = Vector2.Transform(position,
                                        Matrix.Invert(GetViewMatrix(Vector2.One)));
      //transform = Vector2.Transform(transform, Matrix.CreateScale(Zoom, Zoom, 1));
      return transform;
    }

    public Matrix GetViewMatrix(Vector2 parallaxFactor)
    {
      return GetVirtualViewMatrix(parallaxFactor) * _screenAdapter.GetScaleMatrix();
    }

    private Matrix GetVirtualViewMatrix(Vector2 parallaxFactor)
    {
      return
        Matrix.CreateTranslation(new Vector3(-Position * parallaxFactor, 0.0f)) *
        Matrix.CreateTranslation(new Vector3(-Origin, 0.0f)) *
        Matrix.CreateRotationZ(Rotation) *
        Matrix.CreateScale(Zoom, Zoom, 1) *
        Matrix.CreateTranslation(new Vector3(Origin, 0.0f));
    }

    private Matrix GetVirtualViewMatrix()
    {
      return GetVirtualViewMatrix(Vector2.Zero);
    }

    public Matrix GetViewMatrix()
    {
      return GetViewMatrix(Vector2.Zero);
    }

    public Matrix GetInverseViewMatrix()
    {
      return Matrix.Invert(GetViewMatrix());
    }

    public Matrix CreateProjection(Vector2 parallaxFactor)
    {
      var virtualView = Matrix.CreateTranslation(new Vector3(-Position * parallaxFactor, 0.0f)) *
                        //Matrix.CreateTranslation(new Vector3(-Origin, 0.0f)) *
                        //Matrix.CreateRotationZ(Rotation) *
                        //Matrix.CreateScale(Zoom, Zoom, 1) *
                        //Matrix.CreateTranslation(new Vector3(Origin, 0.0f)) *
                        Matrix.CreateOrthographic(_screenAdapter.VirtualWidth, -_screenAdapter.VirtualHeight, -1000f,
                                                  1000f);

      return virtualView * _screenAdapter.GetScaleMatrix();
    }

    private Matrix GetProjectionMatrix(Matrix viewMatrix)
    {
      var projection = Matrix.CreateOrthographicOffCenter(0, _screenAdapter.VirtualWidth, _screenAdapter.VirtualHeight,
                                                          0, -1, 0);
      Matrix.Multiply(ref viewMatrix, ref projection, out projection);
      return projection;
    }

    public BoundingFrustum GetBoundingFrustum()
    {
      var viewMatrix = GetVirtualViewMatrix();
      var projectionMatrix = GetProjectionMatrix(viewMatrix);
      return new BoundingFrustum(projectionMatrix);
    }

    public Rectangle GetRectangle()
    {
      return new Rectangle(0, 0, _screenAdapter.VirtualWidth, _screenAdapter.VirtualHeight);
    }

    public Rectangle GetBoundingRectangle()
    {
      var frustum = GetBoundingFrustum();
      var corners = frustum.GetCorners();
      var topLeft = corners[0];
      var bottomRight = corners[2];
      var width = bottomRight.X - topLeft.X;
      var height = bottomRight.Y - topLeft.Y;
      return new Rectangle((int) topLeft.X, (int) topLeft.Y, (int) width, (int) height);
    }

    public ContainmentType Contains(Point point)
    {
      return Contains(point.ToVector2());
    }

    public ContainmentType Contains(Vector2 vector2)
    {
      return GetBoundingFrustum().Contains(new Vector3(vector2.X, vector2.Y, 0));
    }

    public ContainmentType Contains(Rectangle rectangle)
    {
      var max = new Vector3(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height, 0.5f);
      var min = new Vector3(rectangle.X, rectangle.Y, 0.5f);
      var boundingBox = new BoundingBox(min, max);
      return GetBoundingFrustum().Contains(boundingBox);
    }

    public void Update(float timeStep)
    {
      if (! Enabled)
        return;

      var keyState = Keyboard.GetState();

      if (keyState.IsKeyDown(Keys.S))
        Move(new Vector2(-1f, 0f));
      if (keyState.IsKeyDown(Keys.F))
        Move(new Vector2(1f, 0f));
      if (keyState.IsKeyDown(Keys.E))
        Move(new Vector2(0f, -1f));
      if (keyState.IsKeyDown(Keys.D))
        Move(new Vector2(0f, 1f));

      if (keyState.IsKeyDown(Keys.W))
        Rotation -= 0.002f;
      if (keyState.IsKeyDown(Keys.R))
        Rotation += 0.002f;

      if (keyState.IsKeyDown(Keys.X))
        Zoom += 0.001f;
      if (keyState.IsKeyDown(Keys.V))
        Zoom -= 0.001f;

      if (keyState.IsKeyDown(Keys.Q))
        Reset();
    }

    public void Reset()
    {
      Origin = Vector2.Zero;
      Position = Vector2.Zero;
      Rotation = 0f;
      Zoom = 1f;
    }
  }
}