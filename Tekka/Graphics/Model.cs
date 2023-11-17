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

public class Model : Drawable
{
    private Vertex[] vertices;
    private uint[] indices;
    
    private BufferObject<float> Vbo;
    private BufferObject<uint> Ebo;
    private VertexArrayObject<float, uint> Vao;
    private Texture texture;
    
    public Model(GL gl, string path)
    {
        LoadMesh(path);
        
        Vbo = new BufferObject<float>(gl, GetVertexData(), BufferTargetARB.ArrayBuffer);
        Ebo = new BufferObject<uint>(gl, indices, BufferTargetARB.ElementArrayBuffer);
        
        Vao = new VertexArrayObject<float, uint>(gl, Vbo, Ebo);
        Vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 8, 0);
        Vao.VertexAttributePointer(1, 3, VertexAttribPointerType.Float, 8, 3);
        Vao.VertexAttributePointer(2, 2, VertexAttribPointerType.Float, 8, 6);
        
        DefaultShader = new Shader(gl, "Shaders/model.vert", "Shaders/model.frag");
        
        texture = Texture.LoadFromFile(gl, "Assets/Models/saulgoodman.png");
        
        Vao.Unbind(); 
    }
    
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
        
        
        
        DefaultShader.SetUniform("world_color", new Vector3(0.4f, 0.4f, 0.4f));
        DefaultShader.SetUniform("modelTexture", 0);
        
        texture.Bind(Gl);
        
        Gl.DrawElements(PrimitiveType.Triangles, (uint)indices.Length, DrawElementsType.UnsignedInt, null);
        
        texture.Unbind(Gl);
        
        Vao.Unbind();
        DefaultShader.Unbind();
    }
    
    private Span<float> GetVertexData()
    {
        var vertexData = new float[vertices.Length * 8];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertexData[i * 8 + 0] = vertices[i].Position.X;
            vertexData[i * 8 + 1] = vertices[i].Position.Y;
            vertexData[i * 8 + 2] = vertices[i].Position.Z;
            vertexData[i * 8 + 3] = vertices[i].Color.X;
            vertexData[i * 8 + 4] = vertices[i].Color.Y;
            vertexData[i * 8 + 5] = vertices[i].Color.Z;
            vertexData[i * 8 + 6] = vertices[i].TexCoord.X;
            vertexData[i * 8 + 7] = vertices[i].TexCoord.Y;
        }

        return vertexData;
    }
    
    public unsafe void LoadMesh(string path)
    {
        using var assimp = Assimp.GetApi()!;
        
        var scene=assimp.ImportFile(path, (uint)PostProcessPreset.TargetRealTimeMaximumQuality)!;
        
        var vertexMap = new Dictionary<Vertex, uint>();
        var _vertices = new List<Vertex>();
        var _indices = new List<uint>();
        
        VisitSceneNode(scene->MRootNode);
        
        assimp.ReleaseImport(scene);
        
        vertices = _vertices.ToArray();
        indices = _indices.ToArray();
        
        void VisitSceneNode(Node* node)
        {
            for (int m = 0; m < node->MNumMeshes; m++)
            {
                var mesh = scene->MMeshes[node->MMeshes[m]];
        
                for (int f = 0; f < mesh->MNumFaces; f++)
                {
                    var face = mesh->MFaces[f];
        
                    for (int i = 0; i < face.MNumIndices; i++)
                    {
                        uint index = face.MIndices![i];
            
                        var position = mesh->MVertices[index];
                        var texture = mesh->MTextureCoords![0]![(int)index];
        
                        Vertex vertex = new()
                        {
                            Position = new Vector3D<float>(position.X, position.Y, position.Z),
                            Color = new Vector3D<float>(1, 1, 1),
                            TexCoord = new Vector2D<float>(texture.X, 1.0f - texture.Y)
                        };
        
                        if (vertexMap.TryGetValue(vertex, out var meshIndex))
                        {
                            _indices.Add(meshIndex);
                        }
                        else
                        {
                            _indices.Add((uint)_vertices.Count);
                            vertexMap[vertex] = (uint)_vertices.Count;
                            _vertices.Add(vertex);
                        }
                    }
                }
            }
        
            for (int c = 0; c < node->MNumChildren; c++)
            {
                VisitSceneNode(node->MChildren[c]!);
            }
        }
    }
}