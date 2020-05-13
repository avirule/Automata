﻿#region

using System;
using System.Drawing;
using System.IO;
using System.Numerics;
using Automata;
using Automata.Core;
using Automata.Core.Components;
using Automata.Core.Systems;
using Automata.Rendering;
using Automata.Rendering.OpenGL;
using AutomataTest.Blocks;
using AutomataTest.Chunks;
using Serilog;
using Silk.NET.GLFW;
using Silk.NET.Input;
using Silk.NET.Input.Common;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Silk.NET.Windowing.Common;

#endregion

namespace AutomataTest
{
    internal class Program
    {
        private static readonly string _LocalDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData,
            Environment.SpecialFolderOption.Create);

        private static IWindow _Window;
        private static GL _GL;

        private static Shader _Shader;

        //Vertex data, uploaded to the VBO.
        private static readonly Vector3[] _vertices =
        {
            new Vector3(0f, 0f, 0f),
            new Vector3(0f, 1f, 0f),
            new Vector3(1f, 0f, 0f),
            new Vector3(1f, 1f, 0f)
        };

        private static readonly Color64[] _colors =
        {
            new Color64(0f, 0f, 0f, 1f),
            new Color64(1f, 1f, 1f, 1f),
            new Color64(1f, 1f, 1f, 1f),
            new Color64(0f, 0f, 0f, 1f),
        };

        private static readonly uint[] _indices =
        {
            0,
            2,
            1,
            2,
            3,
            1
        };

        private static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
            Log.Information("Static logger initialized.");

            if (!Directory.Exists($@"{_LocalDataPath}/Wyd/"))
            {
                Log.Information("Local data folder missing, creating...");
                Directory.CreateDirectory($@"{_LocalDataPath}/Wyd/");
            }

            WindowOptions options = WindowOptions.Default;
            options.Title = "Wyd: A Journey";
            options.Size = new Size(800, 600);
            options.Position = new Point(500, 400);

            _Window = Window.Create(options);
            //_Window.Render += OnRender;
            _Window.Closing += OnClose;
            _Window.Resize += AdjustPerspective;

            static void AdjustPerspective(Size size)
            {
                _Projection = Matrix4x4.CreatePerspective(AutomataMath.ToRadians(90f), (float)size.Width / (float)size.Height, 0.1f, 100f);
            }

            AdjustPerspective(_Window.Size);

            _Window.Initialize();
            Initialize();

            _Window.VSync = VSyncMode.Off;

            IInputContext inputContext = _Window.CreateInput();
            IMouse mouse = inputContext.Mice[0];

            while (!_Window.IsClosing)
            {
                _Window.DoEvents();

                if (!_Window.IsClosing)
                {
                    World.GlobalUpdate();
                }

                mouse.Position = new PointF(_Window.Size.Width / 2f, _Window.Size.Height / 2f);
            }
        }

        private static void Initialize()
        {
            Singleton.InstantiateSingleton<Diagnostics>();
            Singleton.InstantiateSingleton<Input>();
            Singleton.InstantiateSingleton<BlockRegistry>();

            Diagnostics.Instance.RegisterDiagnosticTimeEntry("NoiseRetrieval");
            Diagnostics.Instance.RegisterDiagnosticTimeEntry("TerrainGeneration");

            Input.Instance.RegisterView(_Window);

            BlockRegistry.Instance.RegisterBlockDefinition("bedrock", null,
                BlockDefinition.Property.Collideable);

            BlockRegistry.Instance.RegisterBlockDefinition("grass", null,
                BlockDefinition.Property.Collectible, BlockDefinition.Property.Collideable,
                BlockDefinition.Property.Destroyable);

            BlockRegistry.Instance.RegisterBlockDefinition("dirt", null,
                BlockDefinition.Property.Collectible, BlockDefinition.Property.Collideable,
                BlockDefinition.Property.Destroyable);

            BlockRegistry.Instance.RegisterBlockDefinition("dirt_coarse", null,
                BlockDefinition.Property.Collectible, BlockDefinition.Property.Collideable,
                BlockDefinition.Property.Destroyable);

            BlockRegistry.Instance.RegisterBlockDefinition("stone", null,
                BlockDefinition.Property.Collectible, BlockDefinition.Property.Collideable,
                BlockDefinition.Property.Destroyable);

            BlockRegistry.Instance.RegisterBlockDefinition("glass", null,
                BlockDefinition.Property.Transparent, BlockDefinition.Property.Collectible,
                BlockDefinition.Property.Collideable, BlockDefinition.Property.Destroyable);

            BlockRegistry.Instance.RegisterBlockDefinition("coal_ore", null,
                BlockDefinition.Property.Collectible, BlockDefinition.Property.Collideable,
                BlockDefinition.Property.Destroyable);

            World world = new GameWorld(true);
            world.SystemManager.RegisterSystem<ViewDoUpdateSystem, DefaultOrderSystem>();
            world.SystemManager.RegisterSystem<ViewDoRenderSystem, LastOrderSystem>();
            world.SystemManager.RegisterSystem<InputCameraViewMoverSystem, RenderOrderSystem>();
            world.SystemManager.RegisterSystem<ChunkBuildingSystem, ViewDoUpdateSystem>();
            World.RegisterWorld("core", world);

            Entity gameEntity = new Entity();
            world.EntityManager.RegisterEntity(gameEntity);
            world.EntityManager.RegisterComponent(gameEntity, new WindowIViewProvider(_Window));
            world.EntityManager.RegisterComponent(gameEntity, new PendingMeshDataComponent
            {
                Vertices = _vertices,
                Colors = _colors,
                Indices = _indices
            });

            _Shader = new Shader("default.vert", "shader.frag");
            _Shader.SetUniform("model", Matrix4x4.Identity);
            _Shader.SetUniform("projection", _Projection);
            _Shader.SetUniform("view", Matrix4x4.CreateLookAt(new Vector3(0f, 0f, 3f), Vector3.Zero, Vector3.UnitY));

            world.EntityManager.RegisterComponent(gameEntity, new RenderedShader
            {
                Shader = _Shader
            });

            Entity playerEntity = new Entity();
            world.EntityManager.RegisterEntity(playerEntity);
            world.EntityManager.RegisterComponent(playerEntity, new Translation
            {
                Value = new Vector3(0f, 0f, -1f)
            });
            world.EntityManager.RegisterComponent<Rotation>(playerEntity);
            world.EntityManager.RegisterComponent<Camera>(playerEntity);

            Entity chunk = new Entity();
            world.EntityManager.RegisterComponent<Translation>(chunk);
            world.EntityManager.RegisterComponent<BlockCollection>(chunk);
            world.EntityManager.RegisterComponent<GenerationState>(chunk);
        }

        private static Matrix4x4 _View;
        private static Matrix4x4 _Projection;
        private static readonly Glfw _glfw = Glfw.GetApi();

        private static void OnClose()
        {
            //SystemManager.Destroy();
        }
    }
}
