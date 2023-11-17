using System.Numerics;
using Silk.NET.OpenGL;

namespace Tekka.Graphics;

public class Drawable
{
    public Transform Transform = new Transform();
    public Shader DefaultShader;
    public virtual void Render(GL Gl, Camera camera, LightSource[] lights) {}
}