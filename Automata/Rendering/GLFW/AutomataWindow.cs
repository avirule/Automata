#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using Automata.Collections;
using Automata.Input;
using Automata.Numerics;
using Automata.Worlds;
using Serilog;
using Silk.NET.Core.Contexts;
using Silk.NET.Input.Common;
using Silk.NET.Windowing.Common;

#endregion

namespace Automata.Rendering.GLFW
{
    public class AutomataWindow : Singleton<AutomataWindow>
    {
        #region Private Member Variables

        private readonly Stopwatch _DeltaTimer;

        private IWindow? _Window;
        private TimeSpan _MinimumFrameTime;

        private IWindow Window
        {
            get
            {
                if (_Window == null)
                {
                    throw new NullReferenceException(nameof(Window));
                }
                else
                {
                    return _Window;
                }
            }
            set =>
                throw new InvalidOperationException(
                    $"Property '{nameof(Window)}' cannot be set. Use '{nameof(CreateWindow)}' or one of its overloads instead."
                );
        }

        #endregion


        #region Public Member Variables

        public IGLContext? GLContext => Window.GLContext;

        public IVkSurface Surface
        {
            get
            {
                Debug.Assert(Window != null);

                if (Window.VkSurface == null)
                {
                    throw new NotSupportedException("Vulkan is not supported by windowing.");
                }

                return Window.VkSurface;
            }
        }

        public Vector2i Size { get; private set; }
        public bool Focused { get; private set; }

        public Vector2i Position => (Vector2i)Window.Position;

        #endregion

        #region Events

        public event WindowResizedEventHandler? Resized;
        public event WindowFocusChangedEventHandler? FocusChanged;
        public event WindowClosingEventHandler? Closing;

        #endregion

        public AutomataWindow()
        {
            AssignSingletonInstance(this);

            _DeltaTimer = new Stopwatch();
        }

        public void CreateWindow(WindowOptions windowOptions)
        {
            _Window = Silk.NET.Windowing.Window.Create(windowOptions);
            _Window.Resize += OnWindowResized;
            _Window.FocusChanged += OnWindowFocusedChanged;
            _Window.Closing += OnWindowClosing;

            Resized += (sender, newSize) => Size = newSize;
            FocusChanged += (sender, isFocused) => Focused = isFocused;

            Size = new Vector2i(Window.Size.Width, Window.Size.Height);
            Focused = true;

            Window.Initialize();
            InputManager.Instance.RegisterView(Window);

            DetermineVSyncRefreshRate();
        }

        private void DetermineVSyncRefreshRate()
        {
            const double default_refresh_rate = 60d;

            double refreshRate;
            if (Window.Monitor.VideoMode.RefreshRate.HasValue)
            {
                refreshRate = Window.Monitor.VideoMode.RefreshRate.Value;
            }
            else
            {
                refreshRate = default_refresh_rate;
                Log.Error("No monitor detected VSync framerate will be set to default value.");
            }

            _MinimumFrameTime = TimeSpan.FromSeconds(1d / refreshRate);
            Log.Debug(string.Format(FormatHelper.DEFAULT_LOGGING, nameof(AutomataWindow), $"VSync framerate configured to {refreshRate} FPS."));
        }

        public void Run()
        {
            try
            {
                FixedConcurrentQueue<double> averageFPS = new FixedConcurrentQueue<double>(1000);

                while (!Window.IsClosing)
                {
                    _DeltaTimer.Restart();

                    Window.DoEvents();

                    if (InputManager.Instance.IsKeyPressed(Key.Escape))
                    {
                        Window.Close();
                    }

                    if (!Window.IsClosing)
                    {
                        World.GlobalUpdate(_DeltaTimer);
                    }

                    Window.DoEvents();
                    Window.SwapBuffers();

                    if (CheckWaitForNextMonitorRefresh())
                    {
                        WaitForNextMonitorRefresh();
                    }

                    averageFPS.Enqueue(1d / _DeltaTimer.Elapsed.TotalSeconds);
                    Window.Title = $"Automata {averageFPS.Average():0.00} FPS";
                }
            }
            catch (Exception ex)
            {
                Log.Error(string.Format(_LogFormat, $"exception occured: {ex}"));
                throw;
            }
        }

        private bool CheckWaitForNextMonitorRefresh() =>
            Window.VSync switch
            {
                VSyncMode.On => true,
                VSyncMode.Off => false,
                VSyncMode.Adaptive => throw new NotSupportedException("Adaptive VSync is deprecated."),
                _ => throw new ArgumentOutOfRangeException()
            };

        private void WaitForNextMonitorRefresh()
        {
            TimeSpan frameWait = _MinimumFrameTime - _DeltaTimer.Elapsed;
            Thread.Sleep(frameWait <= TimeSpan.Zero ? TimeSpan.Zero : frameWait);
        }


        #region Event Subscriptors

        private void OnWindowResized(Size newSize) => Resized?.Invoke(this, (Vector2i)newSize);

        private void OnWindowFocusedChanged(bool focused) => FocusChanged?.Invoke(this, Focused);

        private void OnWindowClosing() => Closing?.Invoke(this);

        #endregion
    }
}
