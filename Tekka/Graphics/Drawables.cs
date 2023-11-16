using System.Numerics;
using Silk.NET.OpenGL;

namespace Tekka.Graphics;

public class Drawables
{
    public Transform Transform = new Transform();
    
    
    public bool IsLightSource = false;
    public Shader DefaultShader;
    
    public Vector3 LightColor;
    public Vector3 DiffuseColor;
    public Vector3 SpecularColor;

    public virtual void Render(GL Gl, Camera camera, List<Drawables> drawables) {}
}