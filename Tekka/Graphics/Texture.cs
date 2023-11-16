using System.Runtime.InteropServices;
using Silk.NET.OpenGL;
using StbImageSharp;

namespace elvo.Engine.Graphics;

public unsafe class Texture
{
    public ImageResult Result;
    public uint Handle;
    public IntPtr ToIntPtr() => (IntPtr) Handle;
    public static Texture LoadFromFile(GL gl, string path)
    {
        ImageResult result = ImageResult.FromMemory(File.ReadAllBytes(path), ColorComponents.RedGreenBlueAlpha);
        Texture texture = new Texture();
        texture.Result = result;

        texture.Handle = gl.GenTexture();
        gl.ActiveTexture(TextureUnit.Texture0);
        gl.BindTexture(TextureTarget.Texture2D, texture.Handle);
        
        fixed (byte* ptr = result.Data)
        {
            gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, 
                (uint)result.Width, (uint)result.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, ptr);
        }
        
        // if macos
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) TextureWrapMode.Repeat);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) TextureWrapMode.Repeat);
            
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.LinearMipmapLinear);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Linear);
        }
        else
        {
            gl.TextureParameter(texture.Handle, TextureParameterName.TextureWrapS, (int) TextureWrapMode.Repeat);
            gl.TextureParameter(texture.Handle, TextureParameterName.TextureWrapT, (int) TextureWrapMode.Repeat);
        
            gl.TextureParameter(texture.Handle, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.LinearMipmapLinear);
            gl.TextureParameter(texture.Handle, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Linear);
        }
        
        
        gl.GenerateMipmap(TextureTarget.Texture2D);
        gl.BindTexture(TextureTarget.Texture2D, 0);
        
        
        return texture;
    }
}