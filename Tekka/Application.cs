using System.Numerics;
using ImGuiNET;
using Silk.NET.GLFW;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Tekka.Graphics;
using Tekka.Helper;
using Silk.NET.OpenGL.Extensions.ImGui;
using Shader = Tekka.Graphics.Shader;


namespace Tekka;

public class Application
{
    private static Camera Camera;

    private static Vector2 LastMousePosition;
    
    public GL Gl;
    public IKeyboard primaryKeyboard;
    public IWindow window;
    private bool isMouseCaptured = true;
    
    private ImGuiController controller = null;
    private IInputContext inputContext = null;

    private LightSource light1 = new();
    private LightSource light2 = new();
    private LightSource light3 = new();
    private LightSource light4 = new();
    private LightSource[] lights = new LightSource[4];

    private Mesh saul;
    private Mesh floor;
    private Mesh backWall;
    private Mesh skybox;
    private Shader lightShader;
    
    public void Run()
    {
        var options = WindowOptions.Default;
        options.Size = new Vector2D<int>(1280, 720);
        options.Title = "Tekka";
        options.VSync = false;
        window = Window.Create(options);

        window.Load += OnLoad;
        window.Update += OnUpdate;
        window.Render += OnRender;
        window.Resize += OnResize;
        window.Closing += OnClose;

        window.Run();

        window.Dispose();
    }

    private void OnLoad()
    {
        var input = window.CreateInput();
        primaryKeyboard = input.Keyboards.FirstOrDefault();
        if (primaryKeyboard != null) primaryKeyboard.KeyDown += KeyDown;
        for (var i = 0; i < input.Mice.Count; i++)
        {
            input.Mice[i].Cursor.CursorMode = CursorMode.Raw;
            input.Mice[i].MouseMove += OnMouseMove;
            input.Mice[i].Scroll += OnMouseWheel;
        }

        Gl = GL.GetApi(window);
        
        controller = new ImGuiController(
            Gl, 
            window, // pass in our window
            inputContext = window.CreateInput() // create an input context
        );
        
        //Start a camera at position 3 on the Z axis, looking at position -1 on the Z axis
        Camera = new Camera(Vector3.UnitZ * 3, Vector3.UnitZ * 15, Vector3.UnitY * 6, 19 / 9);
        

        light1.Position = new Vector3(0.0f, 0.0f, 0.0f);
        light1.DiffuseColor = new Vector3(80.0f, 2000.0f, 2000.0f);
        light1.SpecularColor = new Vector3(1.0f, 1.0f, 1.0f);
        light1.LightColor = new Vector3(1.0f, 1.0f, 1.0f);
        
        light2.Position = new Vector3(100.0f, 50.0f, -10.0f);
        light2.DiffuseColor =  new Vector3(2000.0f, 10.0f, 2000.0f);
        light2.SpecularColor = new Vector3(1.0f, 1.0f, 1.0f);
        light2.LightColor = new Vector3(1.0f, 1.0f, 1.0f);
        
        light3.Position = new Vector3(-100.0f, 50.0f, -10.0f);
        light3.DiffuseColor =  new Vector3(2000.0f, 10.0f, 2000.0f);
        light3.SpecularColor = new Vector3(1.0f, 1.0f, 1.0f);
        light3.LightColor = new Vector3(1.0f, 1.0f, 1.0f);
        
        light4.Position = new Vector3(0.0f, 50.0f, -100.0f);
        light4.DiffuseColor =  new Vector3(2000.0f, 10.0f, 2000.0f);
        light4.SpecularColor = new Vector3(1.0f, 1.0f, 1.0f);
        
        lights = new LightSource[] {light1, light2, light3, light4};
        
        lightShader = new Shader(Gl, "Shaders/model.vert", "Shaders/model.frag");
        
        saul = Loader.LoadMeshFromObj(Gl, "Assets/Models/saulgoodman.obj", "Assets/Models/saulgoodman.png");
        saul.Transform.Scale = new Vector3(0.05f, 0.05f, 0.05f);

        floor = Loader.LoadMeshAsCube(Gl, "Assets/Textures/texture_02.png");
        floor.Transform.Scale = new Vector3(20, 0.1f, 20);
        floor.Transform.Position = new Vector3(0, -1, 0);
        
        backWall = Loader.LoadMeshAsCube(Gl, "Assets/Textures/texture_02.png");
        backWall.Transform.Scale = new Vector3(20, 20, 0.1f);
        backWall.Transform.Position = new Vector3(0, 0, -10);
        
        skybox = Loader.LoadMeshAsCube(Gl, "Assets/Skybox/italy.png");
        skybox.Transform.Scale = new Vector3(100, 100, 100);
        skybox.Transform.Position = new Vector3(0, 0, 0);
    }

    private void OnUpdate(double deltaTime)
    {
        if (isMouseCaptured)
        {
            var moveSpeed = 5f * (float)deltaTime;
            if (primaryKeyboard.IsKeyPressed(Key.W))
                //Move forwards
                Camera.Position += moveSpeed * Camera.Front;
            if (primaryKeyboard.IsKeyPressed(Key.S))
                //Move backwards
                Camera.Position -= moveSpeed * Camera.Front;
            if (primaryKeyboard.IsKeyPressed(Key.A))
                //Move left
                Camera.Position -= Vector3.Normalize(Vector3.Cross(Camera.Front, Camera.Up)) * moveSpeed;
            if (primaryKeyboard.IsKeyPressed(Key.D))
                //Move right
                Camera.Position += Vector3.Normalize(Vector3.Cross(Camera.Front, Camera.Up)) * moveSpeed;
        }
    }

    private void OnRender(double deltaTime)
    {
        controller.Update((float) deltaTime);
        
        Gl.Enable(EnableCap.DepthTest);
        Gl.Clear((uint)(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));
        
        lightShader.Use();
        
        Uniform(saul);
        Renderer.DrawObj(Gl, saul);
        
        Uniform(floor);
        Renderer.DrawCube(Gl, floor);
        
        Uniform(backWall);
        Renderer.DrawCube(Gl, backWall);
        
        Uniform(skybox);
        Renderer.DrawCube(Gl, skybox);
        
        lightShader.Unbind();
        
        DisplayFps();   
        
        controller.Render();
    }

    private void Uniform(Mesh mesh)
    {
        #region Uniforms // TODO: fix
        lightShader.SetUniform("uModel", mesh.Transform.Model);
        lightShader.SetUniform("uView", Camera.GetViewMatrix());
        lightShader.SetUniform("uProjection", Camera.GetProjectionMatrix());
        lightShader.SetUniform("viewPos", Camera.Position);
        
        lightShader.SetUniform("material.ambient", new Vector3(1.0f, 0.5f, 0.31f));
        lightShader.SetUniform("material.diffuse", new Vector3(1.0f, 0.5f, 0.31f));
        lightShader.SetUniform("material.specular", new Vector3(0.5f, 0.5f, 0.5f));
        lightShader.SetUniform("material.shininess", 32.0f);
            
        
        lightShader.SetUniform("light1.position", lights[0].Position);
        lightShader.SetUniform("light1.diffuse", lights[0].DiffuseColor);
        lightShader.SetUniform("light1.specular", lights[0].SpecularColor);
        
        lightShader.SetUniform("light2.position", lights[1].Position);
        lightShader.SetUniform("light2.diffuse", lights[1].DiffuseColor);
        lightShader.SetUniform("light2.specular", lights[1].SpecularColor);
        
        lightShader.SetUniform("light3.position", lights[2].Position);
        lightShader.SetUniform("light3.diffuse", lights[2].DiffuseColor);
        lightShader.SetUniform("light3.specular", lights[2].SpecularColor);
        
        lightShader.SetUniform("light4.position", lights[3].Position);
        lightShader.SetUniform("light4.diffuse", lights[3].DiffuseColor);
        lightShader.SetUniform("light4.specular", lights[3].SpecularColor);

        
        lightShader.SetUniform("world_color", new Vector3(0.5f, 0.5f, 0.5f));
        lightShader.SetUniform("modelTexture", 0);
        #endregion
    }

    private void OnMouseMove(IMouse mouse, Vector2 position)
    {
        if (isMouseCaptured)
        {
            var lookSensitivity = 0.1f;
            if (LastMousePosition == default)
            {
                LastMousePosition = position;
            }
            else
            {
                var xOffset = (position.X - LastMousePosition.X) * lookSensitivity;
                var yOffset = (position.Y - LastMousePosition.Y) * lookSensitivity;
                LastMousePosition = position;

                Camera.ModifyDirection(xOffset, yOffset);
            }
        }
        
    }

    private void OnMouseWheel(IMouse mouse, ScrollWheel scrollWheel)
    {
        Camera.ModifyZoom(scrollWheel.Y);
    }
    
    private void OnResize(Vector2D<int> size)
    {
        Camera.AspectRatio = size.X / (float)size.Y;
        Gl.Viewport(size);
    }

    private void OnClose()
    {
       
    }

    private void KeyDown(IKeyboard keyboard, Key key, int arg3)
    {
        if (key == Key.Escape)
        {
            var input = window.CreateInput();
            for (var i = 0; i < input.Mice.Count; i++)
            {
                //input.Mice[i].Cursor.CursorMode = CursorMode.Raw;
                if (input.Mice[i].Cursor.CursorMode == CursorMode.Raw)
                {
                    input.Mice[i].Cursor.CursorMode = CursorMode.Normal;
                    isMouseCaptured = false;
                }
                else
                {
                    input.Mice[i].Cursor.CursorMode = CursorMode.Raw;
                    isMouseCaptured = true;
                }
            }
        }
    }
    
    private void DisplayFps(string sceneName = "Demo Scene")
    {
        int location = 0;
        var io = ImGui.GetIO();
        var flags = ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoNav | ImGuiWindowFlags.AlwaysAutoResize |
                    ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoSavedSettings;

        if (location >= 0)
        {
            const float PAD = 10.0f;
            var viewport = ImGui.GetMainViewport();
            var work_pos = viewport.WorkPos; // Use work area to avoid menu-bar/task-bar, if any!
            var work_size = viewport.WorkSize;
            Vector2 window_pos, window_pos_pivot;
            window_pos.X = (location & 1) != 0 ? (work_pos.X + work_size.X - PAD) : (work_pos.X + PAD);
            window_pos.Y = (location & 2) != 0 ? (work_pos.Y + work_size.Y - PAD) : (work_pos.Y + PAD);
            window_pos_pivot.X = (location & 1) != 0 ? 1.0f : 0.0f;
            window_pos_pivot.Y = (location & 2) != 0 ? 1.0f : 0.0f;

            ImGui.SetNextWindowPos(window_pos, ImGuiCond.Always, window_pos_pivot);
            flags |= ImGuiWindowFlags.NoMove;
        } else if (location == -2)
        {
            ImGui.SetNextWindowPos(io.DisplaySize - new Vector2(0.0f, 0.0f), ImGuiCond.Always, new Vector2(1.0f, 1.0f));
            flags |= ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove;
        }
        ImGui.SetNextWindowBgAlpha(0.35f);
        if (ImGui.Begin("Example: Simple overlay", ImGuiWindowFlags.NoDecoration |
                                                    ImGuiWindowFlags.AlwaysAutoResize |
                                                    ImGuiWindowFlags.NoSavedSettings |
                                                    ImGuiWindowFlags.NoFocusOnAppearing |
                                                    ImGuiWindowFlags.NoNav))
        {
            // TODO: Rightclick to change pos
            ImGui.Text($"Scene: {sceneName}");
            ImGui.Separator();
            //ImGui.Text($"Application average {1000.0f / ImGui.GetIO().Framerate:F3} ms/frame ({ImGui.GetIO().Framerate:F1} FPS)");
            ImGui.Text("Application average ");
            ImGui.SameLine(0.0f, 0.0f);
            ImGui.TextColored(new Vector4(1.0f, 1.0f, 0.0f, 1.0f), $"{1000.0f / ImGui.GetIO().Framerate:F3} ms/frame");
            ImGui.SameLine(0.0f, 0.0f);
            ImGui.Text(" (");
            ImGui.SameLine(0.0f, 0.0f);
            ImGui.TextColored(new Vector4(1.0f, 1.0f, 0.0f, 1.0f), $"{ImGui.GetIO().Framerate:F1} FPS");
            ImGui.SameLine(0.0f, 0.0f);
            ImGui.Text(")");
            ImGui.Separator();
            ImGui.Text($"Camera position: {Camera.Position}");
            
            ImGui.Separator();
            
            if (ImGui.Button("Toggle VSync"))
            {
                var isV = window.VSync;
                window.VSync = !isV;
            }
            ImGui.SameLine();
            ImGui.Text($"VSync: {window.VSync}");
            
            ImGui.Separator();
            
            ImGui.DragFloat3("Light position", ref light1.Position, 0.1f);
            ImGui.DragFloat3("Light diffuse", ref light1.DiffuseColor, 0.1f);
            ImGui.DragFloat3("Light specular", ref light1.SpecularColor, 0.1f);
            ImGui.DragFloat3("Light color", ref light1.LightColor, 0.1f);
            
            ImGui.Separator();
            
            
            ImGui.End();
        }

    }
}