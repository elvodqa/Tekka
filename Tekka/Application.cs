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


namespace Tekka;

public class Application
{
    private static Camera Camera;

    private static Vector2 LastMousePosition;
    
    public GL Gl;
    public IKeyboard primaryKeyboard;
    public IWindow window;
    
    private ImGuiController controller = null;
    private IInputContext inputContext = null;

    public List<Drawables> drawables = new();

    public Cube cube = new();
    public DummyLight light = new();
    
    public void Run()
    {
        var options = WindowOptions.Default;
        options.Size = new Vector2D<int>(1280, 720);
        options.Title = "Tekka";
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
        Camera = new Camera(Vector3.UnitZ * -2, Vector3.UnitZ * -3, Vector3.UnitY, window.Size.X / window.Size.Y);
        
        cube.Load(Gl);
        light.Load(Gl);
        
        cube.Transform.Position = new Vector3(0.0f, 0.0f, 0.0f);
        light.Transform.Position = new Vector3(0.0f, 2.0f, 0.0f);
        
        drawables.Add(cube);
        drawables.Add(light);
    }

    private void OnUpdate(double deltaTime)
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

    private void OnRender(double deltaTime)
    {
        controller.Update((float) deltaTime);
        
        Gl.Enable(EnableCap.DepthTest);
        Gl.Clear((uint)(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));
        
        for (var i = 0; i < drawables.Count; i++)
        {
            drawables[i].Render(Gl, Camera, drawables);
        }
        
        
        
        DisplayFps();   
        
        controller.Render();
    }

    private void OnMouseMove(IMouse mouse, Vector2 position)
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

    private void OnMouseWheel(IMouse mouse, ScrollWheel scrollWheel)
    {
        Camera.ModifyZoom(scrollWheel.Y);
    }
    
    private void OnResize(Vector2D<int> size)
    {
        Camera.AspectRatio = size.X / size.Y;
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
                }
                else
                {
                    input.Mice[i].Cursor.CursorMode = CursorMode.Raw;
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
            ImGui.Text($"Application average {1000.0f / ImGui.GetIO().Framerate:F3} ms/frame ({ImGui.GetIO().Framerate:F1} FPS)");
            ImGui.Text($"Drawable count: {drawables.Count}");
            ImGui.Text($"Camera position: {Camera.Position}");
            
            ImGui.End();
        }

    }
}