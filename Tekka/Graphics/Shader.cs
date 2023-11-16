using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace elvo.Engine.Graphics;

public unsafe class Shader
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
    
    public void SetInt(string name, int value)
    {
        int location = gl.GetUniformLocation(Handle, name);
        gl.Uniform1(location, value);
    }
    
    public  void SetFloat(string name, float value)
    {
        int location = gl.GetUniformLocation(Handle, name);
        gl.Uniform1(location, value);
    }
    
    public void SetVector2(string name, Vector2D<int> value)
    {
        int location = gl.GetUniformLocation(Handle, name);
        gl.Uniform2(location, value.X, value.Y);
    }
    
    public void SetVector2(string name, Vector2D<float> value)
    {
        int location = gl.GetUniformLocation(Handle, name);
        gl.Uniform2(location, value.X, value.Y);
    }
    
    public void SetVector3(string name, Vector3D<int> value)
    {
        int location = gl.GetUniformLocation(Handle, name);
        gl.Uniform3(location, value.X, value.Y, value.Z);
    }
    
    public void SetVector3(string name, Vector3D<float> value)
    {
        int location = gl.GetUniformLocation(Handle, name);
        gl.Uniform3(location, value.X, value.Y, value.Z);
    }
    
    public void SetVector4(string name, Vector4D<int> value)
    {
        int location = gl.GetUniformLocation(Handle, name);
        gl.Uniform4(location, value.X, value.Y, value.Z, value.W);
    }
    
    public void SetVector4(string name, Vector4D<float> value)
    {
        int location = gl.GetUniformLocation(Handle, name);
        gl.Uniform4(location, value.X, value.Y, value.Z, value.W);
    }
    
    public void SetMatrix4(string name, Matrix4X4<float> value)
    {
        int location = gl.GetUniformLocation(Handle, name);
        gl.UniformMatrix4(location, 1, false, (float*)&value);
    }
    
    public void SetMatrix4(string name, Matrix4X4<double> value)
    {
        int location = gl.GetUniformLocation(Handle, name);
        gl.UniformMatrix4(location, 1, false, (double*)&value);
    }
    
    public void SetSampler2D(string name, int value)
    {
        int location = gl.GetUniformLocation(Handle, name);
        gl.Uniform1(location, value);
    }
    
    public void SetSampler2D(string name, Texture value)
    {
        int location = gl.GetUniformLocation(Handle, name);
        gl.Uniform1(location, value.Handle);
    }
}