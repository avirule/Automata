using Silk.NET.Core.Attributes;
using Silk.NET.Core.Contexts;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;

namespace Automata.Engine.Rendering.Vulkan
{
    [Extension(EXTENSION_NAME)]
    public abstract class SurfaceExtension : NativeExtension<Vk>
    {
        public const string EXTENSION_NAME = "VK_KHR_surface";

        public SurfaceExtension(INativeContext nativeContext) : base(nativeContext) { }

        /// <summary>To be added.</summary>
        [NativeApi(EntryPoint = "vkDestroySurfaceKHR")]
        public abstract unsafe void DestroySurface(
            [Count(Count = 0)] Instance instance,
            [Count(Count = 0)] SurfaceKHR surface,
            [Count(Count = 0), Flow(FlowDirection.In)]
            AllocationCallbacks* pAllocator);

        /// <summary>To be added.</summary>
        [NativeApi(EntryPoint = "vkDestroySurfaceKHR")]
        public abstract void DestroySurface(
            [Count(Count = 0)] Instance instance,
            [Count(Count = 0)] SurfaceKHR surface,
            [Count(Count = 0), Flow(FlowDirection.In)]
            ref AllocationCallbacks pAllocator);

        /// <summary>To be added.</summary>
        [NativeApi(EntryPoint = "vkGetPhysicalDeviceSurfaceCapabilitiesKHR")]
        public abstract unsafe Result GetPhysicalDeviceSurfaceCapabilities(
            [Count(Count = 0)] PhysicalDevice physicalDevice,
            [Count(Count = 0)] SurfaceKHR surface,
            [Count(Count = 0), Flow(FlowDirection.Out)]
            SurfaceCapabilitiesKHR* pSurfaceCapabilities);

        /// <summary>To be added.</summary>
        [NativeApi(EntryPoint = "vkGetPhysicalDeviceSurfaceCapabilitiesKHR")]
        public abstract Result GetPhysicalDeviceSurfaceCapabilities(
            [Count(Count = 0)] PhysicalDevice physicalDevice,
            [Count(Count = 0)] SurfaceKHR surface,
            [Count(Count = 0), Flow(FlowDirection.Out)]
            out SurfaceCapabilitiesKHR pSurfaceCapabilities);

        /// <summary>To be added.</summary>
        [NativeApi(EntryPoint = "vkGetPhysicalDeviceSurfaceFormatsKHR")]
        public abstract unsafe Result GetPhysicalDeviceSurfaceFormats(
            [Count(Count = 0)] PhysicalDevice physicalDevice,
            [Count(Count = 0)] SurfaceKHR surface,
            [Count(Count = 0)] uint* pSurfaceFormatCount,
            [Count(Computed = "pSurfaceFormatCount"), Flow(FlowDirection.Out)]
            SurfaceFormatKHR* pSurfaceFormats);

        /// <summary>To be added.</summary>
        [NativeApi(EntryPoint = "vkGetPhysicalDeviceSurfaceFormatsKHR")]
        public abstract Result GetPhysicalDeviceSurfaceFormats(
            [Count(Count = 0)] PhysicalDevice physicalDevice,
            [Count(Count = 0)] SurfaceKHR surface,
            [Count(Count = 0)] ref uint pSurfaceFormatCount,
            [Count(Computed = "pSurfaceFormatCount"), Flow(FlowDirection.Out)]
            out SurfaceFormatKHR pSurfaceFormats);

        /// <summary>To be added.</summary>
        [NativeApi(EntryPoint = "vkGetPhysicalDeviceSurfacePresentModesKHR")]
        public abstract unsafe Result GetPhysicalDeviceSurfacePresentModes(
            [Count(Count = 0)] PhysicalDevice physicalDevice,
            [Count(Count = 0)] SurfaceKHR surface,
            [Count(Count = 0)] uint* pPresentModeCount,
            [Count(Computed = "pPresentModeCount"), Flow(FlowDirection.Out)]
            PresentModeKHR* pPresentModes);

        /// <summary>To be added.</summary>
        [NativeApi(EntryPoint = "vkGetPhysicalDeviceSurfacePresentModesKHR")]
        public abstract Result GetPhysicalDeviceSurfacePresentModes(
            [Count(Count = 0)] PhysicalDevice physicalDevice,
            [Count(Count = 0)] SurfaceKHR surface,
            [Count(Count = 0)] ref uint pPresentModeCount,
            [Count(Computed = "pPresentModeCount"), Flow(FlowDirection.Out)]
            out PresentModeKHR pPresentModes);

        /// <summary>To be added.</summary>
        [NativeApi(EntryPoint = "vkGetPhysicalDeviceSurfaceSupportKHR")]
        public abstract unsafe Result GetPhysicalDeviceSurfaceSupport(
            [Count(Count = 0)] PhysicalDevice physicalDevice,
            [Count(Count = 0)] uint queueFamilyIndex,
            [Count(Count = 0)] SurfaceKHR surface,
            [Count(Count = 0), Flow(FlowDirection.Out)]
            Bool32* pSupported);

        /// <summary>To be added.</summary>
        [NativeApi(EntryPoint = "vkGetPhysicalDeviceSurfaceSupportKHR")]
        public abstract Result GetPhysicalDeviceSurfaceSupport(
            [Count(Count = 0)] PhysicalDevice physicalDevice,
            [Count(Count = 0)] uint queueFamilyIndex,
            [Count(Count = 0)] SurfaceKHR surface,
            [Count(Count = 0), Flow(FlowDirection.Out)]
            out Bool32 pSupported);
    }
}
