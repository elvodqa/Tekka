using Silk.NET.Assimp;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace Tekka.Graphics;

public static class Loader
{
    public static Mesh LoadMeshFromObj(GL gl, string modelPath, string texturePath)
    {
        Mesh mesh = new Mesh();
        mesh.Transform = new Transform();
        mesh.Vertices = Array.Empty<Vertex>();
        mesh.Indices = Array.Empty<uint>();
        
        LoadMesh(ref mesh, modelPath);

        mesh.Vbo = new BufferObject<float>(gl, GetVertexData(mesh.Vertices), BufferTargetARB.ArrayBuffer);
        mesh.Ebo = new BufferObject<uint>(gl, mesh.Indices, BufferTargetARB.ElementArrayBuffer);

        mesh.Vao = new VertexArrayObject<float, uint>(gl, mesh.Vbo, mesh.Ebo);
        mesh.Vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 8, 0);
        mesh.Vao.VertexAttributePointer(1, 3, VertexAttribPointerType.Float, 8, 3);
        mesh.Vao.VertexAttributePointer(2, 2, VertexAttribPointerType.Float, 8, 6);

        mesh.Texture = Texture.LoadFromFile(gl, texturePath);

        mesh.Vao.Unbind();
        
        return mesh;
    }

    public static Mesh LoadMeshAsCube(GL gl, string texturePath)
    {
        Mesh mesh = new Mesh();
        mesh.Transform = new Transform();
        mesh.Vertices = new Vertex[]
        {
            // Front
            new Vertex {Position = new Vector3D<float>(-0.5f, -0.5f, 0.5f), Color = new Vector3D<float>(1, 1, 1), TexCoord = new Vector2D<float>(0, 0)},
            new Vertex {Position = new Vector3D<float>(0.5f, -0.5f, 0.5f), Color = new Vector3D<float>(1, 1, 1), TexCoord = new Vector2D<float>(1, 0)},
            new Vertex {Position = new Vector3D<float>(0.5f, 0.5f, 0.5f), Color = new Vector3D<float>(1, 1, 1), TexCoord = new Vector2D<float>(1, 1)},
            new Vertex {Position = new Vector3D<float>(-0.5f, 0.5f, 0.5f), Color = new Vector3D<float>(1, 1, 1), TexCoord = new Vector2D<float>(0, 1)},
            
            // Back
            new Vertex {Position = new Vector3D<float>(-0.5f, -0.5f, -0.5f), Color = new Vector3D<float>(1, 1, 1), TexCoord = new Vector2D<float>(0, 0)},
            new Vertex {Position = new Vector3D<float>(0.5f, -0.5f, -0.5f), Color = new Vector3D<float>(1, 1, 1), TexCoord = new Vector2D<float>(1, 0)},
            new Vertex {Position = new Vector3D<float>(0.5f, 0.5f, -0.5f), Color = new Vector3D<float>(1, 1, 1), TexCoord = new Vector2D<float>(1, 1)},
            new Vertex {Position = new Vector3D<float>(-0.5f, 0.5f, -0.5f), Color = new Vector3D<float>(1, 1, 1), TexCoord = new Vector2D<float>(0, 1)},
            
            // Left
            new Vertex {Position = new Vector3D<float>(-0.5f, -0.5f, -0.5f), Color = new Vector3D<float>(1, 1, 1), TexCoord = new Vector2D<float>(0, 0)},
            new Vertex {Position = new Vector3D<float>(-0.5f, -0.5f, 0.5f), Color = new Vector3D<float>(1, 1, 1), TexCoord = new Vector2D<float>(1, 0)},
            new Vertex {Position = new Vector3D<float>(-0.5f, 0.5f, 0.5f), Color = new Vector3D<float>(1, 1, 1), TexCoord = new Vector2D<float>(1, 1)},
            new Vertex {Position = new Vector3D<float>(-0.5f, 0.5f, -0.5f), Color = new Vector3D<float>(1, 1, 1), TexCoord = new Vector2D<float>(0, 1)},
            
            // Right
            new Vertex {Position = new Vector3D<float>(0.5f, -0.5f, -0.5f), Color = new Vector3D<float>(1, 1, 1), TexCoord = new Vector2D<float>(0, 0)},
            new Vertex {Position = new Vector3D<float>(0.5f, -0.5f, 0.5f), Color = new Vector3D<float>(1, 1, 1), TexCoord = new Vector2D<float>(1, 0)},
            new Vertex {Position = new Vector3D<float>(0.5f, 0.5f, 0.5f), Color = new Vector3D<float>(1, 1, 1), TexCoord = new Vector2D<float>(1, 1)},
            new Vertex {Position = new Vector3D<float>(0.5f, 0.5f, -0.5f), Color = new Vector3D<float>(1, 1, 1), TexCoord = new Vector2D<float>(0, 1)},
            
            // Top
            new Vertex {Position = new Vector3D<float>(-0.5f, 0.5f, -0.5f), Color = new Vector3D<float>(1, 1, 1), TexCoord = new Vector2D<float>(0, 0)},
            new Vertex {Position = new Vector3D<float>(0.5f, 0.5f, -0.5f), Color = new Vector3D<float>(1, 1, 1), TexCoord = new Vector2D<float>(1, 0)},
            new Vertex {Position = new Vector3D<float>(0.5f, 0.5f, 0.5f), Color = new Vector3D<float>(1, 1, 1), TexCoord = new Vector2D<float>(1, 1)},
            new Vertex {Position = new Vector3D<float>(-0.5f, 0.5f, 0.5f), Color = new Vector3D<float>(1, 1, 1), TexCoord = new Vector2D<float>(0, 1)},
            
            // Bottom
            new Vertex {Position = new Vector3D<float>(-0.5f, -0.5f, -0.5f), Color = new Vector3D<float>(1, 1, 1), TexCoord = new Vector2D<float>(0, 0)},
            new Vertex {Position = new Vector3D<float>(0.5f, -0.5f, -0.5f), Color = new Vector3D<float>(1, 1, 1), TexCoord = new Vector2D<float>(1, 0)},
            new Vertex {Position = new Vector3D<float>(0.5f, -0.5f, 0.5f), Color = new Vector3D<float>(1, 1, 1), TexCoord = new Vector2D<float>(1, 1)},
            new Vertex {Position = new Vector3D<float>(-0.5f, -0.5f, 0.5f), Color = new Vector3D<float>(1, 1, 1), TexCoord = new Vector2D<float>(0, 1)},
        };
        
        mesh.Indices = new uint[]
        {
            // Front
            0, 1, 2,
            2, 3, 0,
            
            // Back
            4, 5, 6,
            6, 7, 4,
            
            // Left
            8, 9, 10,
            10, 11, 8,
            
            // Right
            12, 13, 14,
            14, 15, 12,
            
            // Top
            16, 17, 18,
            18, 19, 16,
            
            // Bottom
            20, 21, 22,
            22, 23, 20,
        };
        
        mesh.Vbo = new BufferObject<float>(gl, GetVertexData(mesh.Vertices), BufferTargetARB.ArrayBuffer);
        mesh.Ebo = new BufferObject<uint>(gl, mesh.Indices, BufferTargetARB.ElementArrayBuffer);
        
        mesh.Vao = new VertexArrayObject<float, uint>(gl, mesh.Vbo, mesh.Ebo);
        mesh.Vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 8, 0);
        mesh.Vao.VertexAttributePointer(1, 3, VertexAttribPointerType.Float, 8, 3);
        mesh.Vao.VertexAttributePointer(2, 2, VertexAttribPointerType.Float, 8, 6);
        
        mesh.Texture = Texture.LoadFromFile(gl, texturePath);
        
        mesh.Vao.Unbind();
        
        return mesh;
    }

    private static unsafe void LoadMesh(ref Mesh mesh, string path)
    {
        using var assimp = Assimp.GetApi()!;

        var scene = assimp.ImportFile(path, (uint)PostProcessPreset.TargetRealTimeMaximumQuality)!;

        var vertexMap = new Dictionary<Vertex, uint>();
        var _vertices = new List<Vertex>();
        var _indices = new List<uint>();

        VisitSceneNode(scene->MRootNode);

        assimp.ReleaseImport(scene);

        mesh.Vertices = _vertices.ToArray();
        mesh.Indices = _indices.ToArray();

        void VisitSceneNode(Node* node)
        {
            for (int m = 0; m < node->MNumMeshes; m++)
            {
                var _mesh = scene->MMeshes[node->MMeshes[m]];

                for (int f = 0; f < _mesh->MNumFaces; f++)
                {
                    var face = _mesh->MFaces[f];

                    for (int i = 0; i < face.MNumIndices; i++)
                    {
                        uint index = face.MIndices![i];

                        var position = _mesh->MVertices[index];
                        var texture = _mesh->MTextureCoords![0]![(int)index];

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

    private static Span<float> GetVertexData(Vertex[] vertices)
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
        
}