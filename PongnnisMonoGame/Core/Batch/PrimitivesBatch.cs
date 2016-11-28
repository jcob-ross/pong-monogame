namespace PongMonoGame
{
  using System;
  using Microsoft.Xna.Framework;
  using Microsoft.Xna.Framework.Graphics;

  public class PrimitivesBatch : IDisposable
  {
    private bool _disposed;
    private bool _hasBegun;
    private readonly int _bufferSize;
    private readonly GraphicsDevice _device;

    private int _lineVerticesCount;
    private readonly VertexPositionColor[] _lineVertices;

    private int _triangleVerticesCount;
    private readonly VertexPositionColor[] _triangleVertices;

    public bool IsReady => !_hasBegun;

    public PrimitivesBatch(GraphicsDevice device, int bufferSize = 500)
    {
      if (null == device)
        throw new ArgumentNullException(nameof(device));
      bufferSize = MathHelper.Clamp(bufferSize, 100, 3000);

      _device = device;
      _bufferSize = bufferSize;

      _lineVertices = new VertexPositionColor[_bufferSize - _bufferSize % 2];
      _triangleVertices = new VertexPositionColor[_bufferSize - _bufferSize % 3];
    }

    public void Begin(Effect effect = null)
    {
      if (_hasBegun)
        return;

      // todo - this is not how you apply effect
      for (var i = 0; i < effect?.CurrentTechnique.Passes.Count; ++i)
        effect.CurrentTechnique.Passes[i].Apply();

      _hasBegun = true;
    }

    public void End()
    {
      if (! _hasBegun)
        return;

      FlushTriangles();
      FlushLines();

      _hasBegun = false;
    }

    public void DrawAABB(AABB boundingBox, Color color)
    {
      AddVertex(boundingBox.LowerBound, color, PrimitiveType.LineList);
      AddVertex(new Vector2(boundingBox.UpperBound.X, boundingBox.LowerBound.Y), color, PrimitiveType.LineList);

      AddVertex(new Vector2(boundingBox.UpperBound.X, boundingBox.LowerBound.Y), color, PrimitiveType.LineList);
      AddVertex(new Vector2(boundingBox.UpperBound.X, boundingBox.UpperBound.Y), color, PrimitiveType.LineList);

      AddVertex(new Vector2(boundingBox.UpperBound.X, boundingBox.UpperBound.Y), color, PrimitiveType.LineList);
      AddVertex(new Vector2(boundingBox.LowerBound.X, boundingBox.UpperBound.Y), color, PrimitiveType.LineList);

      AddVertex(new Vector2(boundingBox.LowerBound.X, boundingBox.UpperBound.Y), color, PrimitiveType.LineList);
      AddVertex(boundingBox.LowerBound, color, PrimitiveType.LineList);

      DrawMarkerSquare(boundingBox.LowerBound, 6);
      DrawMarkerSquare(new Vector2(boundingBox.UpperBound.X, boundingBox.LowerBound.Y), 6);
      DrawMarkerSquare(new Vector2(boundingBox.LowerBound.X, boundingBox.UpperBound.Y), 6);
      DrawMarkerSquare(boundingBox.UpperBound, 6);
    }

    public void DrawMarkerCross(Vector2 position, int size)
    {
      if (!_hasBegun)
        return;

      var offset = (float)size / 2;
      AddVertex(new Vector2(position.X - offset, position.Y), Color.Green, PrimitiveType.LineList);
      AddVertex(new Vector2(position.X + offset, position.Y), Color.Green, PrimitiveType.LineList);

      AddVertex(new Vector2(position.X, position.Y - offset), Color.Red, PrimitiveType.LineList);
      AddVertex(new Vector2(position.X, position.Y + offset), Color.Red, PrimitiveType.LineList);
    }

    public void DrawRectangleVerticesMarkers(Vector2 topLeft, float width, float height, int size)
    {
      if (!_hasBegun)
        return;

      DrawMarkerCross(topLeft, size);
      DrawMarkerCross(topLeft + new Vector2(width, 0), size);
      DrawMarkerCross(topLeft + new Vector2(width, height), size);
      DrawMarkerCross(topLeft + new Vector2(0, height), size);
    }

    public void DrawMarkerSquare(Vector2 position, int size = 5)
    {
      if (!_hasBegun)
        return;

      var offset = (float) size / 2;
      AddVertex(new Vector2(position.X - offset, position.Y - offset), Color.Green, PrimitiveType.LineList);
      AddVertex(new Vector2(position.X + offset, position.Y - offset), Color.Red, PrimitiveType.LineList);

      AddVertex(new Vector2(position.X + offset, position.Y - offset), Color.Red, PrimitiveType.LineList);
      AddVertex(new Vector2(position.X + offset, position.Y + offset), Color.Yellow, PrimitiveType.LineList);

      AddVertex(new Vector2(position.X + offset, position.Y + offset), Color.Yellow, PrimitiveType.LineList);
      AddVertex(new Vector2(position.X - offset, position.Y + offset), Color.HotPink, PrimitiveType.LineList);

      AddVertex(new Vector2(position.X - offset, position.Y + offset), Color.HotPink, PrimitiveType.LineList);
      AddVertex(new Vector2(position.X - offset, position.Y - offset), Color.Green, PrimitiveType.LineList);
    }

    public void DrawRectangleOutline(Vector2 minimum, Vector2 maximum, Color color)
    {
      if (!_hasBegun)
        return;

      AddVertex(minimum, color, PrimitiveType.LineList);
      AddVertex(new Vector2(maximum.X, minimum.Y), color, PrimitiveType.LineList);

      AddVertex(new Vector2(maximum.X, minimum.Y), color, PrimitiveType.LineList);
      AddVertex(maximum, color, PrimitiveType.LineList);

      AddVertex(maximum, color, PrimitiveType.LineList);
      AddVertex(new Vector2(minimum.X, maximum.Y), color, PrimitiveType.LineList);

      AddVertex(new Vector2(minimum.X, maximum.Y), color, PrimitiveType.LineList);
      AddVertex(minimum, color, PrimitiveType.LineList);
    }
    
    public void DrawRectangle(Vector2 minimum, Vector2 maximum, Color color)
    {
      if (!_hasBegun)
        return;

      AddVertex(minimum, color, PrimitiveType.TriangleList);
      AddVertex(new Vector2(maximum.X, minimum.Y), color, PrimitiveType.TriangleList);
      AddVertex(new Vector2(minimum.X, maximum.Y), color, PrimitiveType.TriangleList);

      AddVertex(new Vector2(maximum.X, minimum.Y), color, PrimitiveType.TriangleList);
      AddVertex(maximum, color, PrimitiveType.TriangleList);
      AddVertex(new Vector2(minimum.X, maximum.Y), color, PrimitiveType.TriangleList);
    }

    public void DrawCircleOutline(Vector2 position, float radius, Color color, int segments = 16)
    {
      if (!_hasBegun)
        return;

      var increment = Math.PI * 2.0 / segments;
      var theta = 0.0d;

      for (int i = 0; i < segments; i++)
      {
        var v1 = position + radius * new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
        var v2 = position + radius * new Vector2((float)Math.Cos(theta + increment), (float)Math.Sin(theta + increment));

        AddVertex(v1, color, PrimitiveType.LineList);
        AddVertex(v2, color, PrimitiveType.LineList);

        theta += increment;
      }
    }

    public void DrawCircle(Vector2 position, float radius, Color color, int segments = 16)
    {
      if (!_hasBegun)
        return;

      var increment = Math.PI * 2.0 / segments;
      var theta = 0.0d;

      for (int i = 0; i < segments; i++)
      {
        var v1 = position + radius * new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta));
        var v2 = position + radius * new Vector2((float)Math.Cos(theta + increment), (float)Math.Sin(theta + increment));

        AddVertex(position, color, PrimitiveType.TriangleList);
        AddVertex(v1, color, PrimitiveType.TriangleList);
        AddVertex(v2, color, PrimitiveType.TriangleList);

        theta += increment;
      }
    }

    public void AddVertex(Vector2 vertex, Color color, PrimitiveType type, float rotation = 0)
    {
      if (!_hasBegun)
        return;

      if (type == PrimitiveType.TriangleList)
      {
        if (_triangleVerticesCount >= _triangleVertices.Length)
          FlushTriangles();
        _triangleVertices[_triangleVerticesCount].Position = new Vector3(vertex, -0.01f);
        _triangleVertices[_triangleVerticesCount].Color = color;
        _triangleVerticesCount++;
      }
      if (type == PrimitiveType.LineList)
      {
        if (_lineVerticesCount >= _lineVertices.Length)
          FlushLines();
        _lineVertices[_lineVerticesCount].Position = new Vector3(vertex, 0f);
        _lineVertices[_lineVerticesCount].Color = color;
        _lineVerticesCount++;
      }
    }

    private void FlushTriangles()
    {
      if (! _hasBegun)
        return;

      if (_triangleVerticesCount < 3)
        return;

      var primitiveCount = _triangleVerticesCount / 3;
      _device.DrawUserPrimitives(PrimitiveType.TriangleList, _triangleVertices, 0, primitiveCount);
      _triangleVerticesCount -= primitiveCount * 3;
    }

    private void FlushLines()
    {
      if (! _hasBegun)
        return;

      if (_lineVerticesCount < 2)
        return;

      var primitiveCount = _lineVerticesCount / 2;
      _device.DrawUserPrimitives(PrimitiveType.LineList, _lineVertices, 0, primitiveCount);
      _lineVerticesCount -= primitiveCount * 2;
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
      if (!_disposed)
      {
        if (disposing) { }

        _disposed = true;
      }
    }
  }
}