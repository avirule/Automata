﻿#region

using System;
using System.IO;
using Automata.Engine;
using Automata.Engine.Components;
using Automata.Engine.Entities;
using Automata.Engine.Input;
using Automata.Engine.Numerics;
using Automata.Engine.Rendering;
using Automata.Engine.Rendering.GLFW;
using Automata.Engine.Rendering.OpenGL;
using Automata.Engine.Rendering.OpenGL.Textures;
using Automata.Engine.Systems;
using Automata.Engine.Worlds;
using Automata.Game.Blocks;
using Automata.Game.Chunks;
using Automata.Game.Chunks.Generation;
using ConcurrencyPools;
using Serilog;
using Silk.NET.Windowing.Common;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;
using Texture = Automata.Engine.Rendering.OpenGL.Textures.Texture;

#endregion


namespace Automata.Game
{
    public class Program
    {
        private static readonly string _LocalDataPath =
            $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData, Environment.SpecialFolderOption.Create)}\Automata\";

        private static void Main()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            Log.Debug("Logger initialized.");

            if (!Directory.Exists(_LocalDataPath))
            {
                Log.Information("Application data folder missing. Creating.");
                Directory.CreateDirectory(_LocalDataPath);
            }

            Initialize();

            AutomataWindow.Instance.Run();
        }

        private static void InitializeSingletons()
        {
            WindowOptions options = WindowOptions.Default;
            options.Title = "Automata";
            options.Size = new Size(800, 600);
            options.Position = new Point(500, 400);
            options.VSync = VSyncMode.Off;
            options.PreferredDepthBufferBits = 24;

            Singleton.CreateSingleton<GLFWAPI>();
            Singleton.CreateSingleton<InputManager>();
            Singleton.CreateSingleton<AutomataWindow>();
            AutomataWindow.Instance.CreateWindow(options);
            AutomataWindow.Instance.Closing += ApplicationCloseCallback;

            Singleton.CreateSingleton<GLAPI>();
            Singleton.CreateSingleton<BlockRegistry>();
            Singleton.CreateSingleton<TextureRegistry>();
        }

        private static void InitializeBlocks()
        {
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
        }

        private static void InitializeDefaultWorld(out World world)
        {
            world = new GameWorld(true);
            world.SystemManager.RegisterSystem<ChunkRegionLoaderSystem, DefaultOrderSystem>(SystemRegistrationOrder.After);
            world.SystemManager.RegisterSystem<ChunkGenerationSystem, ChunkRegionLoaderSystem>(SystemRegistrationOrder.After);
            World.RegisterWorld("core", world);
        }

        private static void InitializePlayerEntity(World world)
        {
            IEntity player = new Entity();
            world.EntityManager.RegisterEntity(player);
            world.EntityManager.RegisterComponent<Translation>(player);
            world.EntityManager.RegisterComponent<Rotation>(player);
            world.EntityManager.RegisterComponent<Camera>(player);

            world.EntityManager.RegisterComponent(player, new KeyboardListener
            {
                Sensitivity = 400f
            });

            world.EntityManager.RegisterComponent(player, new MouseListener
            {
                Sensitivity = 40f
            });

            world.EntityManager.RegisterComponent(player, new ChunkLoader
            {
                Radius = 1
            });
        }

        private static void Initialize()
        {
            BoundedAsyncPool.SetActivePool();
            BoundedPool.Active.DefaultThreadPoolSize();

            InitializeSingletons();

            Image<Rgba32> image = Image.Load<Rgba32>("Resources/Textures/BlockAtlas.png");
            Image<Rgba32> slice = new Image<Rgba32>(16, 16);

            for (int x = 0; x < slice.Width; x++)
            for (int y = 0; y < slice.Width; y++)
                slice[x, y] = image[x, y];

            Texture2DArray<Rgba32> texture = new Texture2DArray<Rgba32>(new Vector3i(16, 16, BlockRegistry.Instance.BlockDefinitions.Count),
                Texture.WrapMode.Repeat, Texture.FilterMode.Point);
            TextureRegistry.Instance.AddTexture("blocks", texture);

            InitializeBlocks();

            InitializeDefaultWorld(out World world);

            InitializePlayerEntity(world);
        }

        private static void ApplicationCloseCallback(object sender)
        {
            BoundedPool.Active.Stop();
            GLAPI.Instance.GL.Dispose();
        }
    }
}
