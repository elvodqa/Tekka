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


public class Modela
{ 
    /*
    public override unsafe void Render(GL Gl, Camera camera, LightSource[] lights)
    {
        Vao.Bind();
        DefaultShader.Use();
        
        DefaultShader.SetUniform("uModel", Transform.Model);
        DefaultShader.SetUniform("uView", camera.GetViewMatrix());
        DefaultShader.SetUniform("uProjection", camera.GetProjectionMatrix());
        DefaultShader.SetUniform("viewPos", camera.Position);
        
        DefaultShader.SetUniform("material.ambient", new Vector3(1.0f, 0.5f, 0.31f));
        DefaultShader.SetUniform("material.diffuse", new Vector3(1.0f, 0.5f, 0.31f));
        DefaultShader.SetUniform("material.specular", new Vector3(0.5f, 0.5f, 0.5f));
        DefaultShader.SetUniform("material.shininess", 32.0f);
            
        
        DefaultShader.SetUniform("light1.position", lights[0].Position);
        DefaultShader.SetUniform("light1.diffuse", lights[0].DiffuseColor);
        DefaultShader.SetUniform("light1.specular", lights[0].SpecularColor);
        
        DefaultShader.SetUniform("light2.position", lights[1].Position);
        DefaultShader.SetUniform("light2.diffuse", lights[1].DiffuseColor);
        DefaultShader.SetUniform("light2.specular", lights[1].SpecularColor);
        
        DefaultShader.SetUniform("light3.position", lights[2].Position);
        DefaultShader.SetUniform("light3.diffuse", lights[2].DiffuseColor);
        DefaultShader.SetUniform("light3.specular", lights[2].SpecularColor);
        
        DefaultShader.SetUniform("light4.position", lights[3].Position);
        DefaultShader.SetUniform("light4.diffuse", lights[3].DiffuseColor);
        DefaultShader.SetUniform("light4.specular", lights[3].SpecularColor);


        byte* a = stackalloc byte[4];
        
        DefaultShader.SetUniform("world_color", new Vector3(0.4f, 0.4f, 0.4f));
        DefaultShader.SetUniform("modelTexture", 0);
        
        texture.Bind(Gl);
        
        Gl.DrawElements(PrimitiveType.Triangles, (uint)indices.Length, DrawElementsType.UnsignedInt, null);
        
        texture.Unbind(Gl);
        
        Vao.Unbind();
        DefaultShader.Unbind();
    } */
    
    
    
    
}