using System.Runtime.InteropServices;
using Automata.Engine.Extensions;
using Serilog;
using Silk.NET.Core.Contexts;
using Silk.NET.Core.Native;
using Silk.NET.GLFW;
using Silk.NET.Vulkan;

namespace Automata.Engine.Rendering.Vulkan
{
    public record VulkanInstance
    {
        private static readonly string _LogFormat = string.Format(FormatHelper.DEFAULT_LOGGING, nameof(VulkanInstance), "{0}");

        private readonly Instance _VKInstance;

        internal Vk VK { get; }
        public string ApplicationName { get; }
        public Version32 ApplicationVersion { get; }
        public string EngineName { get; }
        public Version32 EngineVersion { get; }
        public Version32 APIVersion { get; }
        public SwapchainExtension SwapchainExtension { get; }
        public SurfaceExtension SurfaceExtension { get; }
        public VkHandle Handle { get; }

        public VulkanInstance(Vk vk, IVkSurface vkSurface, string applicationName, Version32 applicationVersion, string engineName, Version32 engineVersion,
            Version32 apiVersion, string[] requestedExtensions, bool validation, string[] validationLayers)
        {
            VK = vk;

            ApplicationName = applicationName;
            ApplicationVersion = applicationVersion;
            EngineName = engineName;
            EngineVersion = engineVersion;
            APIVersion = apiVersion;

            _VKInstance = Create(vkSurface, requestedExtensions, validation, validationLayers);
            Handle = _VKInstance.ToHandle();

            if (!TryGetInstanceExtension(out SwapchainExtension? swapchainExtension))
            {
                throw new VulkanException(Result.ErrorExtensionNotPresent, "KHR_swapchain extension not found.");
            }
            else
            {
                SwapchainExtension = swapchainExtension!;
            }

            if (!TryGetInstanceExtension(out SurfaceExtension? surfaceExtension))
            {
                throw new VulkanException(Result.ErrorExtensionNotPresent, $"{SurfaceExtension.EXTENSION_NAME} extension not found.");
            }
            else
            {
                SurfaceExtension = surfaceExtension!;
            }

            // todo extend this debug info
            Log.Debug(_LogFormat, "Created.");
        }

        private unsafe Instance Create(IVkSurface vkSurface, string[] requestedExtensions, bool validation, string[] validationLayers)
        {
            nint applicationName = ApplicationName.MarshalANSI();
            nint engineName = EngineName.MarshalANSI();

            ApplicationInfo applicationInfo = new ApplicationInfo
            {
                SType = StructureType.ApplicationInfo,
                PApplicationName = (byte*)applicationName,
                ApplicationVersion = ApplicationVersion,
                PEngineName = (byte*)engineName,
                EngineVersion = EngineVersion,
                ApiVersion = APIVersion
            };

            string[] requiredExtensions = VKAPI.GetRequiredExtensions(vkSurface, requestedExtensions);
            nint requiredExtensionsPointer = SilkMarshal.MarshalStringArrayToPtr(requiredExtensions);
            string[] requiedExtensions = SilkMarshal.MarshalPtrToStringArray(requiredExtensionsPointer, requestedExtensions.Length);

            InstanceCreateInfo createInfo = new InstanceCreateInfo
            {
                SType = StructureType.InstanceCreateInfo,
                PApplicationInfo = &applicationInfo,
                PpEnabledExtensionNames = (byte**)requiredExtensionsPointer,
                EnabledExtensionCount = (uint)requiredExtensions.Length,
                EnabledLayerCount = 0,
                PpEnabledLayerNames = (byte**)null!,
                PNext = (void*)null!
            };

            if (validation)
            {
                VKAPI.Validate(VK, ref createInfo, validationLayers);
            }

            Log.Information(string.Format(_LogFormat, "allocating instance."));
            Instance instance;
            Result result = VK.CreateInstance(&createInfo, (AllocationCallbacks*)null!, &instance);

            if (result is not Result.Success)
            {
                throw new VulkanException(result, "Failed to create Vulkan instance.");
            }

            Marshal.FreeHGlobal(applicationName);
            Marshal.FreeHGlobal(engineName);
            SilkMarshal.FreeStringArrayPtr(requiredExtensionsPointer, requestedExtensions.Length);

            return instance;
        }

        public TExtension GetInstanceExtension<TExtension>() where TExtension : NativeExtension<Vk>
        {
            if (VK.TryGetInstanceExtension(_VKInstance, out TExtension? extension))
            {
                return extension!;
            }
            else
            {
                throw new VulkanException(Result.ErrorExtensionNotPresent, $"{typeof(TExtension).Name} extension not present.");
            }
        }

        public bool TryGetInstanceExtension<T>(out T? extenstion) where T : NativeExtension<Vk> => VK.TryGetInstanceExtension(_VKInstance, out extenstion);

        public bool TryGetDeviceExtension<T>(out T? extension, Device device) where T : NativeExtension<Vk> =>
            VK.TryGetDeviceExtension(_VKInstance, device, out extension);


        #region Conversions

        public static explicit operator Instance(VulkanInstance vulkanInstance) => vulkanInstance._VKInstance;

        #endregion
    }
}
