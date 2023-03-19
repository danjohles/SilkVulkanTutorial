﻿namespace Chapter17Loading3DModels;

public class FirstApp : IDisposable
{
    // Window stuff
    private IView window = null!;
    private int width = 1800;
    private int height = 1200;
    private string windowName = "Vulkan Tut";
    private long fpsUpdateInterval = 5 * 10_000;
    private long fpsLastUpdate;
    //private IInputContext inputContext = null!;

    // Vk api
    private readonly Vk vk = null!;

    private LveDevice device = null!;
    private LveRenderer lveRenderer = null!;
    private List<LveGameObject> gameObjects = new();

    private OrthographicCamera camera = null!;

    private bool disposedValue;

    private SimpleRenderSystem simpleRenderSystem = null!;

    //private LveGameObject viewerObject = null!;
    //private KeyboardMovementController cameraController = null!;
    private CameraController cameraController = null!;


    //long currentTime = 0;

    public FirstApp()
    {
        log.RestartTimer();
        log.d("startup", "starting up");

        vk = Vk.GetApi();
        log.d("startup", "got vk");

        initWindow();
        log.d("startup", "got window");

        device = new LveDevice(vk, window);
        log.d("startup", "got device");

        lveRenderer = new LveRenderer(vk, window, device, useFifo: true);
        log.d("startup", "got renderer");

        loadGameObjects();
        log.d("startup", "objects loaded");

        //inputContext = window.CreateInput();
        //viewerObject = LveGameObject.CreateGameObject();
        //cameraController = new((IWindow)window);
        //log.d("startup", "got input");


        simpleRenderSystem = new(vk, device, lveRenderer.GetSwapChainRenderPass());
        log.d("startup", "got render system");

        camera = new(Vector3.Zero, 10f, -20f, -140f, window.FramebufferSize);
        cameraController = new(camera, (IWindow)window);
        resize(window.FramebufferSize);

        //Console.WriteLine("");
        //var viewMatrix = camera.GetViewMatrix();
        //Console.WriteLine("view matrix");
        //Console.WriteLine(viewMatrix.PrintRows());
        //Console.WriteLine("");
        //var projectionMatrix = camera.GetProjectionMatrix();
        //Console.WriteLine("projection matrix");
        //Console.WriteLine(projectionMatrix.PrintRows());
        //Console.WriteLine("");

        //cameraController.OnMouseStateChanged += OnMouseStateChanged;
        //camera.SetViewDirection(Vector3.Zero, new(0.5f, 0f, 1f), -Vector3.UnitY);
        //camera.SetViewTarget(new(-1f, -2f, 2f), new(0f, 0f, 2.5f), -Vector3.UnitY);
        log.d("startup", "got camera");
    }

    public void Run()
    {
        MainLoop();
        CleanUp();
    }


    // mouse stuff
    private MouseState mouseLast;


    private void render(double delta)
    {
        mouseLast = cameraController.GetMouseState();

        var commandBuffer = lveRenderer.BeginFrame();

        if (commandBuffer is not null)
        {
            lveRenderer.BeginSwapChainRenderPass(commandBuffer.Value);
            simpleRenderSystem.RenderGameObjects(commandBuffer.Value, ref gameObjects, camera);
            lveRenderer.EndSwapChainRenderPass(commandBuffer.Value);
            lveRenderer.EndFrame();
        }
    }

    private void MainLoop()
    {
        window.Run();

        vk.DeviceWaitIdle(device.VkDevice);
    }

    private void CleanUp()
    {
        window.Dispose();
    }

    private void initWindow()
    {
        //Create a window.
        var options = WindowOptions.DefaultVulkan with
        {
            Size = new Vector2D<int>(width, height),
            Title = windowName
        };

        window = Window.Create(options);
        window.Initialize();

        if (window.VkSurface is null)
        {
            throw new Exception("Windowing platform doesn't support Vulkan.");
        }

        fpsLastUpdate = DateTime.Now.Ticks;

        window.FramebufferResize += resize;
        window.Update += updateWindow;
        window.Render += render;
    }

    private void updateWindow(double frametime)
    {

        if (DateTime.Now.Ticks - fpsLastUpdate < fpsUpdateInterval) return;

        fpsLastUpdate = DateTime.Now.Ticks;
        if (window is IWindow w)
        {
            //w.Title = $"{windowName} | W {window.Size.X}x{window.Size.Y} | FPS {Math.Ceiling(1d / obj)} | ";
            w.Title = $"{windowName} | {mouseLast.Debug} | {1d / frametime,-8: 0,000.0} fps";
        }

    }

    private void resize(Vector2D<int> newsize)
    {
        camera.Resize(0, 0, (uint)newsize.X, (uint)newsize.Y);
        cameraController.Resize(newsize);
    }

    private void loadGameObjects()
    {
        var cube = LveGameObject.CreateGameObject();
        //cube.Model = ModelUtils.LoadModelFromFile(vk, device, "Assets/colored_cube.obj");
        cube.Model = ModelUtils.CreateCubeModel3(vk, device);
        cube.Transform.Translation = new(0.0f, 0.0f, 0.0f);

        //cube.Model = CreateCubeModel(vk, device, Vector3.Zero);
        //cube.Transform.Translation = new(0.0f, 0.0f, 0.0f);
        //cube.Transform.Scale = new(0.5f);

        gameObjects.Add(cube);
    }


    protected unsafe virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            window.Dispose();
            //vk.DestroyPipelineLayout(device.VkDevice, pipelineLayout, null);

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~FirstApp()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}