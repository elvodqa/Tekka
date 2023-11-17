using System.Numerics;
using System.Runtime.InteropServices;
using Silk.NET.OpenGL;

namespace Tekka.Graphics;

public class Cube : Drawable
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct Light
    {
        public Vector3 Position;
        public Vector3 Diffuse;
        public Vector3 Specular;
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct PointLights
    {
        public Light lights0;
        public Light lights1;
        public Light lights2;
        public Light lights3;
    }
    
    private readonly float[] Vertices =
    {
        //X    Y      Z       Normals
        -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
        0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
        0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
        0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
        -0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
        -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,

        -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
        0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
        0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
        0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
        -0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
        -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,

        -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,
        -0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
        -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
        -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
        -0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,
        -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,

        0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,
        0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
        0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
        0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
        0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,
        0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,

        -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,
        0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,
        0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
        0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
        -0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
        -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,

        -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,
        0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,
        0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
        0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
        -0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
        -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f
    };

    private readonly uint[] Indices =
    {
       0, 1, 3,
       1, 2, 3
    };
    
    private BufferObject<float> Vbo;
    private BufferObject<uint> Ebo;
    private VertexArrayObject<float, uint> VaoCube;
    
    public void Load(GL Gl)
    {
        Ebo = new BufferObject<uint>(Gl, Indices, BufferTargetARB.ElementArrayBuffer);
        Vbo = new BufferObject<float>(Gl, Vertices, BufferTargetARB.ArrayBuffer);
        VaoCube = new VertexArrayObject<float, uint>(Gl, Vbo, Ebo);

        VaoCube.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 6, 0);
        VaoCube.VertexAttributePointer(1, 3, VertexAttribPointerType.Float, 6, 3);
        
        DefaultShader = new Shader(Gl, "Shaders/shader.vert", "Shaders/lighting.frag");
        
        VaoCube.Unbind();
    }

    public override void Render(GL Gl, Camera camera, LightSource[] lights)
    {
        VaoCube.Bind();
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
        
        DefaultShader.SetUniform("world_color", new Vector3(0.4f, 0.4f, 0.4f));
        
        Gl.DrawArrays(PrimitiveType.Triangles, 0, 36);
        
        VaoCube.Unbind();
        DefaultShader.Unbind();
    }
}