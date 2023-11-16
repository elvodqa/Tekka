using System.Numerics;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace Tekka.Graphics;

public unsafe class Shader : IDisposable
{
    public uint Handle { get; private set; }
    private GL gl;
    
    public Shader(GL gl, string shaderName)
    {
        this.gl = gl;
        string vertexSource = File.ReadAllText(Path.Combine("Resources", "Shaders", $"{shaderName}.vert"));
        string fragmentSource = File.ReadAllText(Path.Combine("Resources", "Shaders", $"{shaderName}.frag"));
        load(vertexSource, fragmentSource);
    }

    public Shader(GL gl, string vertPath, string fragPath, bool a)
    {
        this.gl = gl;
        string vertexSource = File.ReadAllText(vertPath);
        string fragmentSource = File.ReadAllText(fragPath);
        load(vertexSource, fragmentSource);
    }
    
    public Shader(GL gl, string vertexSource, string fragmentSource)
    {
        this.gl = gl;
        load(vertexSource, fragmentSource);
    }
    
    private void load(string vertexSource, string fragmentSource)
    {
        uint vertexShader = gl.CreateShader(ShaderType.VertexShader);
        gl.ShaderSource(vertexShader, vertexSource);
        gl.CompileShader(vertexShader);
        
        string infoLog = gl.GetShaderInfoLog(vertexShader);
        if (!string.IsNullOrWhiteSpace(infoLog))
        {
            throw new Exception($"Error compiling vertex shader: {infoLog}");
        }
        
        uint fragmentShader = gl.CreateShader(ShaderType.FragmentShader);
        gl.ShaderSource(fragmentShader, fragmentSource);
        gl.CompileShader(fragmentShader);
        
        infoLog = gl.GetShaderInfoLog(fragmentShader);
        if (!string.IsNullOrWhiteSpace(infoLog))
        {
            throw new Exception($"Error compiling fragment shader: {infoLog}");
        }
        
        Handle = gl.CreateProgram();
        gl.AttachShader(Handle, vertexShader);
        gl.AttachShader(Handle, fragmentShader);
        gl.LinkProgram(Handle);
        
        infoLog = gl.GetProgramInfoLog(Handle);
        if (!string.IsNullOrWhiteSpace(infoLog))
        {
            throw new Exception($"Error linking shader program: {infoLog}");
        }
        
        gl.DetachShader(Handle, vertexShader);
        gl.DetachShader(Handle, fragmentShader);
        gl.DeleteShader(vertexShader);
        gl.DeleteShader(fragmentShader);
    }
    
    public void Use()
    {
        gl.UseProgram(Handle);
    }
    
    public void SetUniform(string name, int value)
    {
        int location = gl.GetUniformLocation(Handle, name);
        if (location == -1)
        {
            throw new Exception($"{name} uniform not found on shader.");
        }
        gl.Uniform1(location, value);
    }

    public unsafe void SetUniform(string name, Matrix4x4 value)
    {
        //A new overload has been created for setting a uniform so we can use the transform in our shader.
        int location = gl.GetUniformLocation(Handle, name);
        if (location == -1)
        {
            throw new Exception($"{name} uniform not found on shader.");
        }
        gl.UniformMatrix4(location, 1, false, (float*) &value);
    }

    public void SetUniform(string name, float value)
    {
        int location = gl.GetUniformLocation(Handle, name);
        if (location == -1)
        {
            throw new Exception($"{name} uniform not found on shader.");
        }
        gl.Uniform1(location, value);
    }
    
    public void SetUniform(string name, Vector3 value)
    {
        int location = gl.GetUniformLocation(Handle, name);
        if (location == -1)
        {
            throw new Exception($"{name} uniform not found on shader.");
        }
        gl.Uniform3(location, value.X, value.Y, value.Z);
    }
    
  
    public void Dispose()
    {
        gl.DeleteProgram(Handle);
    }
}