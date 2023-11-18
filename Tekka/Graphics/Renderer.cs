using Silk.NET.OpenGL;

namespace Tekka.Graphics;

public static unsafe class Renderer
{
    public static void DrawObj(GL gl, Mesh mesh)
    {
        mesh.Vao.Bind();
        mesh.Texture.Bind(gl);
        
        gl.DrawElements(PrimitiveType.Triangles, (uint)mesh.Indices.Length, DrawElementsType.UnsignedInt, null);
        
        mesh.Texture.Unbind(gl);
        mesh.Vao.Unbind();
    }
    
    public static void DrawCube(GL gl, Mesh mesh)
    {
        mesh.Vao.Bind();
        mesh.Texture.Bind(gl);
        
        gl.DrawElements(PrimitiveType.Triangles, (uint)mesh.Indices.Length, DrawElementsType.UnsignedInt, null);
        
        mesh.Texture.Unbind(gl);
        mesh.Vao.Unbind();
    }
}