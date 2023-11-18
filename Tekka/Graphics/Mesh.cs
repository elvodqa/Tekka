using System.Numerics;
using Silk.NET.Assimp;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.SDL;
using PrimitiveType = Silk.NET.OpenGL.PrimitiveType;

namespace Tekka.Graphics;

public struct Vertex : IEquatable<Vertex>
{
    public Vector3D<float> Position;
    public Vector3D<float> Color;
    public Vector2D<float> TexCoord;

    public bool Equals(Vertex other)
    {
        return Position.Equals(other.Position) && Color.Equals(other.Color) && TexCoord.Equals(other.TexCoord);
    }

    public override bool Equals(object? obj)
    {
        return obj is Vertex other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Position, Color, TexCoord);
    }
}

public struct Mesh
{
    public Transform Transform;
    public Vertex[] Vertices;
    public uint[] Indices;

    public BufferObject<float> Vbo;
    public BufferObject<uint> Ebo;
    public VertexArrayObject<float, uint> Vao;
    public Texture Texture;
}
