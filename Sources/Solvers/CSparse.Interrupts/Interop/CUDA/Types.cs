
namespace CSparse.Interop.CUDA
{
    using System;

    #region Core

    /// <summary>
    /// Compute mode that device is currently in.
    /// </summary>
    internal enum ComputeMode
    {
        /// <summary>
        /// Default compute mode (Multiple threads can use cudaSetDevice() with this device)
        /// </summary>
        Default = 0,

        /// <summary>
        /// Compute-exclusive-thread mode (Only one thread in one process will be able to use cudaSetDevice() with this device)
        /// </summary>
        Exclusive = 1,

        /// <summary>
        /// Compute-prohibited mode (No threads can use cudaSetDevice() with this device)
        /// </summary>
        Prohibited = 2,

        /// <summary>
        /// Compute-exclusive-process mode (Many threads in one process will be able to use cudaSetDevice() with this device)
        /// </summary>
        ExclusiveProcess = 3
    }

    /// <summary>
    /// Device properties
    /// </summary>
    public enum DeviceAttribute
    {
        /// <summary>
        /// Maximum number of threads per block.
        /// </summary>
        MaxThreadsPerBlock = 1,

        /// <summary>
        /// Maximum block dimension X.
        /// </summary>
        MaxBlockDimX = 2,

        /// <summary>
        /// Maximum block dimension Y.
        /// </summary>
        MaxBlockDimY = 3,

        /// <summary>
        /// Maximum block dimension Z.
        /// </summary>
        MaxBlockDimZ = 4,

        /// <summary>
        /// Maximum grid dimension X.
        /// </summary>
        MaxGridDimX = 5,

        /// <summary>
        /// Maximum grid dimension Y.
        /// </summary>
        MaxGridDimY = 6,

        /// <summary>
        /// Maximum grid dimension Z.
        /// </summary>
        MaxGridDimZ = 7,

        /// <summary>
        /// Maximum shared memory available per block in bytes.
        /// </summary>
        MaxSharedMemoryPerBlock = 8,

        /// <summary>
        /// Memory available on device for __constant__ variables in a CUDA C kernel in bytes.
        /// </summary>
        TotalConstantMemory = 9,

        /// <summary>
        /// Warp size in threads.
        /// </summary>
        WarpSize = 10,

        /// <summary>
        /// Maximum pitch in bytes allowed by memory copies.
        /// </summary>
        MaxPitch = 11,

        /// <summary>
        /// Maximum number of 32-bit registers available per block.
        /// </summary>
        MaxRegistersPerBlock = 12,

        /// <summary>
        /// Peak clock frequency in kilohertz.
        /// </summary>
        ClockRate = 13,

        /// <summary>
        /// Alignment requirement for textures.
        /// </summary>
        TextureAlignment = 14,

        /// <summary>
        /// Device can possibly copy memory and execute a kernel concurrently.
        /// </summary>
        GpuOverlap = 15,

        /// <summary>
        /// Number of multiprocessors on device.
        /// </summary>
        MultiProcessorCount = 16,

        /// <summary>
        /// Specifies whether there is a run time limit on kernels.
        /// </summary>
        KernelExecTimeout = 17,

        /// <summary>
        /// Device is integrated with host memory.
        /// </summary>
        Integrated = 18,

        /// <summary>
        /// Device can map host memory into CUDA address space.
        /// </summary>
        CanMapHostMemory = 19,

        /// <summary>
        /// Compute mode (See cudaComputeMode for details).
        /// </summary>
        ComputeMode = 20,

        /// <summary>
        /// Maximum 1D texture width.
        /// </summary>
        MaxTexture1DWidth = 21,

        /// <summary>
        /// Maximum 2D texture width.
        /// </summary>
        MaxTexture2DWidth = 22,

        /// <summary>
        /// Maximum 2D texture height.
        /// </summary>
        MaxTexture2DHeight = 23,

        /// <summary>
        /// Maximum 3D texture width.
        /// </summary>
        MaxTexture3DWidth = 24,

        /// <summary>
        /// Maximum 3D texture height.
        /// </summary>
        MaxTexture3DHeight = 25,

        /// <summary>
        /// Maximum 3D texture depth.
        /// </summary>
        MaxTexture3DDepth = 26,

        /// <summary>
        /// Maximum 2D layered texture width.
        /// </summary>
        MaxTexture2DLayeredWidth = 27,

        /// <summary>
        /// Maximum 2D layered texture height.
        /// </summary>
        MaxTexture2DLayeredHeight = 28,

        /// <summary>
        /// Maximum layers in a 2D layered texture.
        /// </summary>
        MaxTexture2DLayeredLayers = 29,

        /// <summary>
        /// Alignment requirement for surfaces.
        /// </summary>
        SurfaceAlignment = 30,

        /// <summary>
        /// Device can possibly execute multiple kernels concurrently.
        /// </summary>
        ConcurrentKernels = 31,

        /// <summary>
        /// Device has ECC support enabled.
        /// </summary>
        EccEnabled = 32,

        /// <summary>
        /// PCI bus ID of the device.
        /// </summary>
        PciBusId = 33,

        /// <summary>
        /// PCI device ID of the device.
        /// </summary>
        PciDeviceId = 34,

        /// <summary>
        /// Device is using TCC driver model.
        /// </summary>
        TccDriver = 35,

        /// <summary>
        /// Peak memory clock frequency in kilohertz.
        /// </summary>
        MemoryClockRate = 36,

        /// <summary>
        /// Global memory bus width in bits.
        /// </summary>
        GlobalMemoryBusWidth = 37,

        /// <summary>
        /// Size of L2 cache in bytes.
        /// </summary>
        L2CacheSize = 38,

        /// <summary>
        /// Maximum resident threads per multiprocessor.
        /// </summary>
        MaxThreadsPerMultiProcessor = 39,

        /// <summary>
        /// Number of asynchronous engines.
        /// </summary>
        AsyncEngineCount = 40,

        /// <summary>
        /// Device shares a unified address space with the host.
        /// </summary>
        UnifiedAddressing = 41,

        /// <summary>
        /// Maximum 1D layered texture width.
        /// </summary>
        MaxTexture1DLayeredWidth = 42,

        /// <summary>
        /// Maximum layers in a 1D layered texture.
        /// </summary>
        MaxTexture1DLayeredLayers = 43,

        /// <summary>
        /// Maximum 2D texture width if cudaArrayTextureGather is set.
        /// </summary>
        MaxTexture2DGatherWidth = 45,

        /// <summary>
        /// Maximum 2D texture height if cudaArrayTextureGather is set.
        /// </summary>
        MaxTexture2DGatherHeight = 46,

        /// <summary>
        /// Alternate maximum 3D texture width.
        /// </summary>
        MaxTexture3DWidthAlt = 47,

        /// <summary>
        /// Alternate maximum 3D texture height.
        /// </summary>
        MaxTexture3DHeightAlt = 48,

        /// <summary>
        /// Alternate maximum 3D texture depth.
        /// </summary>
        MaxTexture3DDepthAlt = 49,

        /// <summary>
        /// PCI domain ID of the device.
        /// </summary>
        PciDomainId = 50,

        /// <summary>
        /// Pitch alignment requirement for textures.
        /// </summary>
        TexturePitchAlignment = 51,

        /// <summary>
        /// Maximum cubemap texture width/height.
        /// </summary>
        MaxTextureCubemapWidth = 52,

        /// <summary>
        /// Maximum cubemap layered texture width/height.
        /// </summary>
        MaxTextureCubemapLayeredWidth = 53,

        /// <summary>
        /// Maximum layers in a cubemap layered texture.
        /// </summary>
        MaxTextureCubemapLayeredLayers = 54,

        /// <summary>
        /// Maximum 1D surface width.
        /// </summary>
        MaxSurface1DWidth = 55,

        /// <summary>
        /// Maximum 2D surface width.
        /// </summary>
        MaxSurface2DWidth = 56,

        /// <summary>
        /// Maximum 2D surface height.
        /// </summary>
        MaxSurface2DHeight = 57,

        /// <summary>
        /// Maximum 3D surface width.
        /// </summary>
        MaxSurface3DWidth = 58,

        /// <summary>
        /// Maximum 3D surface height.
        /// </summary>
        MaxSurface3DHeight = 59,

        /// <summary>
        /// Maximum 3D surface depth.
        /// </summary>
        MaxSurface3DDepth = 60,

        /// <summary>
        /// Maximum 1D layered surface width.
        /// </summary>
        MaxSurface1DLayeredWidth = 61,

        /// <summary>
        /// Maximum layers in a 1D layered surface.
        /// </summary>
        MaxSurface1DLayeredLayers = 62,

        /// <summary>
        /// Maximum 2D layered surface width.
        /// </summary>
        MaxSurface2DLayeredWidth = 63,

        /// <summary>
        /// Maximum 2D layered surface height.
        /// </summary>
        MaxSurface2DLayeredHeight = 64,

        /// <summary>
        /// Maximum layers in a 2D layered surface.
        /// </summary>
        MaxSurface2DLayeredLayers = 65,

        /// <summary>
        /// Maximum cubemap surface width.
        /// </summary>
        MaxSurfaceCubemapWidth = 66,

        /// <summary>
        /// Maximum cubemap layered surface width.
        /// </summary>
        MaxSurfaceCubemapLayeredWidth = 67,

        /// <summary>
        /// Maximum layers in a cubemap layered surface.
        /// </summary>
        MaxSurfaceCubemapLayeredLayers = 68,

        /// <summary>
        /// Maximum 1D linear texture width.
        /// </summary>
        MaxTexture1DLinearWidth = 69,

        /// <summary>
        /// Maximum 2D linear texture width.
        /// </summary>
        MaxTexture2DLinearWidth = 70,

        /// <summary>
        /// Maximum 2D linear texture height.
        /// </summary>
        MaxTexture2DLinearHeight = 71,

        /// <summary>
        /// Maximum 2D linear texture pitch in bytes.
        /// </summary>
        MaxTexture2DLinearPitch = 72,

        /// <summary>
        /// Maximum mipmapped 2D texture width.
        /// </summary>
        MaxTexture2DMipmappedWidth = 73,

        /// <summary>
        /// Maximum mipmapped 2D texture height.
        /// </summary>
        MaxTexture2DMipmappedHeight = 74,

        /// <summary>
        /// Major compute capability version number.
        /// </summary>
        ComputeCapabilityMajor = 75,

        /// <summary>
        /// Minor compute capability version number.
        /// </summary>
        ComputeCapabilityMinor = 76,

        /// <summary>
        /// Maximum mipmapped 1D texture width.
        /// </summary>
        MaxTexture1DMipmappedWidth = 77,

        /// <summary>
        /// Device supports stream priorities.
        /// </summary>
        StreamPrioritiesSupported = 78,

        /// <summary>
        /// Device supports caching globals in L1.
        /// </summary>
        GlobalL1CacheSupported = 79,

        /// <summary>
        /// Device supports caching locals in L1.
        /// </summary>
        LocalL1CacheSupported = 80,

        /// <summary>
        /// Maximum shared memory available per multiprocessor in bytes.
        /// </summary>
        MaxSharedMemoryPerMultiprocessor = 81,

        /// <summary>
        /// Maximum number of 32-bit registers available per multiprocessor.
        /// </summary>
        MaxRegistersPerMultiprocessor = 82,

        /// <summary>
        /// Device can allocate managed memory on this system.
        /// </summary>
        ManagedMemory = 83,

        /// <summary>
        /// Device is on a multi-GPU board.
        /// </summary>
        IsMultiGpuBoard = 84,

        /// <summary>
        /// Unique identifier for a group of devices on the same multi-GPU board.
        /// </summary>
        MultiGpuBoardGroupID = 85,

        /// <summary>
        /// Link between the device and the host supports native atomic operations.
        /// </summary>
        HostNativeAtomicSupported = 86,

        /// <summary>
        /// Ratio of single precision performance (in floating-point operations per second) to double precision performance.
        /// </summary>
        SingleToDoublePrecisionPerfRatio = 87,

        /// <summary>
        /// Device supports coherently accessing pageable memory without calling cudaHostRegister on it.
        /// </summary>
        PageableMemoryAccess = 88,

        /// <summary>
        /// Device can coherently access managed memory concurrently with the CPU.
        /// </summary>
        ConcurrentManagedAccess = 89,

        /// <summary>
        /// Device supports Compute Preemption.
        /// </summary>
        ComputePreemptionSupported = 90,

        /// <summary>
        /// Device can access host registered memory at the same virtual address as the CPU.
        /// </summary>
        CanUseHostPointerForRegisteredMem = 91,

        Reserved92 = 92,
        Reserved93 = 93,
        Reserved94 = 94,

        /// <summary>
        /// Device supports launching cooperative kernels via cudaLaunchCooperativeKernel.
        /// </summary>
        CooperativeLaunch = 95,

        /// <summary>
        /// Device can participate in cooperative kernels launched via cudaLaunchCooperativeKernelMultiDevice.
        /// </summary>
        CooperativeMultiDeviceLaunch = 96,

        /// <summary>
        /// The maximum optin shared memory per block. This value may vary by chip. See cudaFuncSetAttribute.
        /// </summary>
        MaxSharedMemoryPerBlockOptin = 97,

        /// <summary>
        /// Device supports flushing of outstanding remote writes.
        /// </summary>
        CanFlushRemoteWrites = 98,

        /// <summary>
        /// Device supports host memory registration via cudaHostRegister.
        /// </summary>
        HostRegisterSupported = 99,

        /// <summary>
        /// Device accesses pageable memory via the host's page tables.
        /// </summary>
        PageableMemoryAccessUsesHostPageTables = 100,

        /// <summary>
        /// Host can directly access managed memory on the device without migration.
        /// </summary>
        cudaDevAttrDirectManagedMemAccessFromHost = 101
    }

    /// <summary>
    /// CUDA stream flags
    /// </summary>
    [Flags]
    internal enum StreamFlags : uint
    {
        /// <summary>
        /// For compatibilty with pre Cuda 5.0, equal to Default
        /// </summary>
        None = 0,
        /// <summary>
        /// Default stream flag
        /// </summary>
        Default = 0x0,
        /// <summary>
        /// Stream does not synchronize with stream 0 (the NULL stream)
        /// </summary>
        NonBlocking = 0x1,
    }

    /// <summary>
    /// Error codes returned by CUDA driver API calls.
    /// </summary>
    public enum CudaResult
    {
        /// <summary>
        /// The API call returned with no errors.
        /// </summary>
        Success = 0,

        /// <summary>
        /// The device function being invoked (usually via cudaLaunchKernel()) was not
        /// previously configured via the cudaConfigureCall() function.
        /// </summary>
        MissingConfiguration = 1,

        /// <summary>
        /// The API call failed because it was unable to allocate enough memory to perform the
        /// requested operation.
        /// </summary>
        MemoryAllocation = 2,

        /// <summary>
        /// The API call failed because the CUDA driver and runtime could not be initialized.
        /// </summary>
        InitializationError = 3,

        /// <summary>
        /// An exception occurred on the device while executing a kernel.
        /// </summary>
        LaunchFailure = 4,

        /// <summary>
        /// DEPRECATED: This indicated that a previous kernel launch failed .
        /// </summary>
        PriorLaunchFailure = 5,

        /// <summary>
        /// This indicates that the device kernel took too long to execute.
        /// </summary>
        LaunchTimeout = 6,

        /// <summary>
        /// This indicates that a launch did not occur because it did not have appropriate resources.
        /// </summary>
        LaunchOutOfResources = 7,

        /// <summary>
        /// The requested device function does not exist or is not compiled for the proper device
        /// architecture.
        /// </summary>
        InvalidDeviceFunction = 8,

        /// <summary>
        /// This indicates that a kernel launch is requesting resources that can never be satisfied
        /// by the current device.
        /// </summary>
        InvalidConfiguration = 9,

        /// <summary>
        /// This indicates that the device ordinal supplied by the user does not correspond to a
        /// valid CUDA device.
        /// </summary>
        InvalidDevice = 10,

        /// <summary>
        /// This indicates that one or more of the parameters passed to the API call is not within
        /// an acceptable range of values.
        /// </summary>
        InvalidValue = 11,

        /// <summary>
        /// This indicates that one or more of the pitch-related parameters passed to the API call
        /// is not within the acceptable range for pitch.
        /// </summary>
        InvalidPitchValue = 12,

        /// <summary>
        /// This indicates that the symbol name/identifier passed to the API call is not a valid
        /// name or identifier.
        /// </summary>
        InvalidSymbol = 13,

        /// <summary>
        /// This indicates that the buffer object could not be mapped.
        /// </summary>
        MapBufferObjectFailed = 14,

        /// <summary>
        /// This indicates that the buffer object could not be unmapped.
        /// </summary>
        UnmapBufferObjectFailed = 15,

        /// <summary>
        /// This indicates that at least one host pointer passed to the API call is not a valid host
        /// pointer.
        /// </summary>
        InvalidHostPointer = 16,

        /// <summary>
        /// This indicates that at least one device pointer passed to the API call is not a valid
        /// device pointer.
        /// </summary>
        InvalidDevicePointer = 17,

        /// <summary>
        /// This indicates that the texture passed to the API call is not a valid texture.
        /// </summary>
        InvalidTexture = 18,

        /// <summary>
        /// This indicates that the texture binding is not valid.
        /// </summary>
        InvalidTextureBinding = 19,

        /// <summary>
        /// This indicates that the channel descriptor passed to the API call is not valid.
        /// </summary>
        InvalidChannelDescriptor = 20,

        /// <summary>
        /// This indicates that the direction of the memcpy passed to the API call is not one of the
        /// types specified by cudaMemcpyKind.
        /// </summary>
        InvalidMemcpyDirection = 21,

        /// <summary>
        /// DEPRECATED: This indicated that the user has taken the address of a constant variable, which was
        /// forbidden up until the CUDA 3.1 release.
        /// </summary>
        AddressOfConstant = 22,

        /// <summary>
        /// DEPRECATED: This indicated that a texture fetch was not able to be performed. This was previously
        /// used for device emulation of texture operations.
        /// </summary>
        TextureFetchFailed = 23,

        /// <summary>
        /// DEPRECATED: This indicated that a texture was not bound for access. This was previously used for
        /// device emulation of texture operations.
        /// </summary>
        TextureNotBound = 24,

        /// <summary>
        /// DEPRECATED: This indicated that a synchronization operation had failed.
        /// </summary>
        SynchronizationError = 25,

        /// <summary>
        /// This indicates that a non-float texture was being accessed with linear filtering. This is
        /// not supported by CUDA.
        /// </summary>
        InvalidFilterSetting = 26,

        /// <summary>
        /// This indicates that an attempt was made to read a non-float texture as a normalized
        /// float. This is not supported by CUDA.
        /// </summary>
        InvalidNormSetting = 27,

        /// <summary>
        /// DEPRECATED: Mixing of device and device emulation code was not allowed.
        /// </summary>
        MixedDeviceExecution = 28,

        /// <summary>
        /// This indicates that a CUDA Runtime API call cannot be executed because it is being
        /// called during process shut down, at a point in time after CUDA driver has been
        /// unloaded.
        /// </summary>
        CudartUnloading = 29,

        /// <summary>
        /// This indicates that an unknown internal error has occurred.
        /// </summary>
        Unknown = 30,

        /// <summary>
        /// DEPRECATED: This indicates that the API call is not yet implemented. Production releases of CUDA
        /// will never return this error.
        /// </summary>
        NotYetImplemented = 31,

        /// <summary>
        /// DEPRECATED: This indicated that an emulated device pointer exceeded the 32-bit address range.
        /// </summary>
        MemoryValueTooLarge = 32,

        /// <summary>
        /// This indicates that a resource handle passed to the API call was not valid.
        /// </summary>
        InvalidResourceHandle = 33,

        /// <summary>
        /// This indicates that asynchronous operations issued previously have not completed
        /// yet. This result is not actually an error, but must be indicated differently than
        /// cudaSuccess (which indicates completion). Calls that may return this value include
        /// cudaEventQuery() and cudaStreamQuery().
        /// </summary>
        NotReady = 34,

        /// <summary>
        /// This indicates that the installed NVIDIA CUDA driver is older than the CUDA
        /// runtime library.
        /// </summary>
        InsufficientDriver = 35,

        /// <summary>
        /// </summary>
        SetOnActiveProcess = 36,

        /// <summary>
        /// This indicates that the surface passed to the API call is not a valid surface.
        /// </summary>
        InvalidSurface = 37,

        /// <summary>
        /// This indicates that no CUDA-capable devices were detected by the installed CUDA
        /// driver.
        /// </summary>
        NoDevice = 38,

        /// <summary>
        /// This indicates that an uncorrectable ECC error was detected during execution.
        /// </summary>
        ECCUncorrectable = 39,

        /// <summary>
        /// This indicates that a link to a shared object failed to resolve.
        /// </summary>
        SharedObjectSymbolNotFound = 40,

        /// <summary>
        /// This indicates that initialization of a shared object failed.
        /// </summary>
        SharedObjectInitFailed = 41,

        /// <summary>
        /// This indicates that the cudaLimit passed to the API call is not supported by the active
        /// device.
        /// </summary>
        UnsupportedLimit = 42,

        /// <summary>
        /// This indicates that multiple global or constant variables (across separate CUDA
        /// source files in the application) share the same string name.
        /// </summary>
        DuplicateVariableName = 43,

        /// <summary>
        /// This indicates that multiple textures (across separate CUDA source files in the
        /// application) share the same string name.
        /// </summary>
        DuplicateTextureName = 44,

        /// <summary>
        /// This indicates that multiple surfaces (across separate CUDA source files in the
        /// application) share the same string name.
        /// </summary>
        DuplicateSurfaceName = 45,

        /// <summary>
        /// This indicates that all CUDA devices are busy or unavailable at the current time.
        /// </summary>
        DevicesUnavailable = 46,

        /// <summary>
        /// This indicates that the device kernel image is invalid.
        /// </summary>
        InvalidKernelImage = 47,

        /// <summary>
        /// This indicates that there is no kernel image available that is suitable for the device.
        /// </summary>
        NoKernelImageForDevice = 48,

        /// <summary>
        /// This indicates that the current context is not compatible with this the CUDA Runtime.
        /// This can only occur if you are using CUDA Runtime/Driver interoperability and have
        /// created an existing Driver context using the driver API.
        /// </summary>
        IncompatibleDriverContext = 49,

        /// <summary>
        /// This error indicates that a call to cudaDeviceEnablePeerAccess() is trying to re-enable
        /// peer addressing on from a context which has already had peer addressing enabled.
        /// </summary>
        PeerAccessAlreadyEnabled = 50,

        /// <summary>
        /// This error indicates that cudaDeviceDisablePeerAccess() is trying to disable peer
        /// addressing which has not been enabled yet via cudaDeviceEnablePeerAccess().
        /// </summary>
        PeerAccessNotEnabled = 51,

        /// <summary>
        /// This indicates that a call tried to access an exclusive-thread device that is already in
        /// use by a different thread.
        /// </summary>
        DeviceAlreadyInUse = 54,

        /// <summary>
        /// This indicates profiler is not initialized for this run. This can happen when the
        /// application is running with external profiling tools like visual profiler.
        /// </summary>
        ProfilerDisabled = 55,

        /// <summary>
        /// DEPRECATED: This error return is deprecated as of CUDA 5.0. It is no longer an error
        /// to attempt to enable/disable the profiling via cudaProfilerStart or cudaProfilerStop
        /// without initialization.
        /// </summary>
        ProfilerNotInitialized = 56,

        /// <summary>
        /// DEPRECATED: This error return is deprecated as of CUDA 5.0. It is no longer an error to
        /// call cudaProfilerStart() when profiling is already enabled.
        /// </summary>
        ProfilerAlreadyStarted = 57,

        /// <summary>
        /// DEPRECATED: This error return is deprecated as of CUDA 5.0. It is no longer an error to
        /// call cudaProfilerStop() when profiling is already disabled.
        /// </summary>
        ProfilerAlreadyStopped = 58,

        /// <summary>
        /// An assert triggered in device code during kernel execution. The device cannot be
        /// used again until cudaThreadExit() is called. All existing allocations are invalid and
        /// must be reconstructed if the program is to continue using CUDA.
        /// </summary>
        Assert = 59,

        /// <summary>
        /// This error indicates that the hardware resources required to enable peer access have
        /// been exhausted for one or more of the devices passed to cudaEnablePeerAccess().
        /// </summary>
        TooManyPeers = 60,

        /// <summary>
        /// This error indicates that the memory range passed to cudaHostRegister() has already
        /// </summary>
        HostMemoryAlreadyRegistered = 61,

        /// been registered.
        /// <summary>
        /// This error indicates that the pointer passed to cudaHostUnregister() does not
        /// correspond to any currently registered memory region.
        /// </summary>
        HostMemoryNotRegistered = 62,

        /// <summary>
        /// This error indicates that an OS call failed.
        /// </summary>
        OperatingSystem = 63,

        /// <summary>
        /// This error indicates that P2P access is not supported across the given devices.
        /// </summary>
        PeerAccessUnsupported = 64,

        /// <summary>
        /// This error indicates that a device runtime grid launch did not occur because the
        /// depth of the child grid would exceed the maximum supported number of nested grid
        /// launches.
        /// </summary>
        LaunchMaxDepthExceeded = 65,

        /// <summary>
        /// This error indicates that a grid launch did not occur because the kernel uses filescoped
        /// textures which are unsupported by the device runtime.
        /// </summary>
        LaunchFileScopedTex = 66,

        /// <summary>
        /// This error indicates that a grid launch did not occur because the kernel uses filescoped
        /// surfaces which are unsupported by the device runtime.
        /// </summary>
        LaunchFileScopedSurf = 67,

        /// <summary>
        /// This error indicates that a call to cudaDeviceSynchronize made from the
        /// device runtime failed because the call was made at grid depth greater
        /// than than either the default (2 levels of grids) or user specified device
        /// limit cudaLimitDevRuntimeSyncDepth.
        /// </summary>
        SyncDepthExceeded = 68,

        /// <summary>
        /// This error indicates that a device runtime grid launch failed because the launch
        /// would exceed the limit cudaLimitDevRuntimePendingLaunchCount.
        /// </summary>
        LaunchPendingCountExceeded = 69,

        /// <summary>
        /// This error indicates the attempted operation is not permitted.
        /// </summary>
        NotPermitted = 70,

        /// <summary>
        /// This error indicates the attempted operation is not supported on the current system
        /// or device.
        /// </summary>
        NotSupported = 71,

        /// <summary>
        /// Device encountered an error in the call stack during kernel execution, possibly due
        /// to stack corruption or exceeding the stack size limit.
        /// </summary>
        HardwareStackError = 72,

        /// <summary>
        /// The device encountered an illegal instruction during kernel execution.
        /// </summary>
        IllegalInstruction = 73,

        /// <summary>
        /// The device encountered a load or store instruction on a memory address which is not
        /// aligned.
        /// </summary>
        MisalignedAddress = 74,

        /// <summary>
        /// While executing a kernel, the device encountered an instruction which can only
        /// operate on memory locations in certain address spaces (global, shared, or local),
        /// but was supplied a memory address not belonging to an allowed address space.
        /// </summary>
        InvalidAddressSpace = 75,

        /// <summary>
        /// The device encountered an invalid program counter.
        /// </summary>
        InvalidPc = 76,

        /// <summary>
        /// The device encountered a load or store instruction on an invalid memory address.
        /// </summary>
        IllegalAddress = 77,

        /// <summary>
        /// A PTX compilation failed. The runtime may fall back to compiling PTX if an
        /// application does not contain a suitable binary for the current device.
        /// </summary>
        InvalidPtx = 78,

        /// <summary>
        /// This indicates an error with the OpenGL or DirectX context.
        /// </summary>
        InvalidGraphicsContext = 79,

        /// <summary>
        /// This indicates that an uncorrectable NVLink error was detected during the execution.
        /// </summary>
        NvlinkUncorrectable = 80,

        /// <summary>
        /// This indicates that the PTX JIT compiler library was not found.
        /// </summary>
        JitCompilerNotFound = 81,

        /// <summary>
        /// This error indicates that the number of blocks launched per grid for a
        /// kernel that was launched via either cudaLaunchCooperativeKernel or
        /// cudaLaunchCooperativeKernelMultiDevice exceeds the maximum number
        /// of blocks.
        /// </summary>
        CooperativeLaunchTooLarge = 82,

        /// <summary>
        /// This indicates an internal startup failure in the CUDA runtime.
        /// </summary>
        StartupFailure = 0x7f,

        /// <summary>
        /// Any unhandled CUDA driver error is added to this value and returned via the
        /// runtime. Production releases of CUDA should not return such errors. Deprecated
        /// This error return is deprecated as of CUDA 4.1.
        /// </summary>
        ApiFailureBase = 10000
    }

    /// <summary>
    /// CUDA memory copy types.
    /// </summary>
    internal enum MemcpyKind
    {
        /// <summary>
        /// Host -> Host
        /// </summary>
        HostToHost = 0,

        /// <summary>
        /// Host -> Device
        /// </summary>
        HostToDevice = 1,

        /// <summary>
        /// Device -> Host
        /// </summary>
        DeviceToHost = 2,

        /// <summary>
        /// Device -> Device
        /// </summary>
        DeviceToDevice = 3,

        /// <summary>
        /// Direction of the transfer
        /// </summary>
        Default = 4
    }

    #endregion

    #region Sparse

    public enum SparseStatus
    {
        /// <summary>
        /// The operation completed successfully.
        /// </summary>
        Success = 0,
        /// <summary>
        /// The CUSPARSE library was not initialized. 
        /// </summary>
        /// <remarks>
        /// This is usually caused by the lack of a prior cusparseCreate() call, an error in the
        /// CUDA Runtime API called by the CUSPARSE routine, or an error in the hardware setup.
        /// To correct: call cusparseCreate() prior to the function call; and check that the hardware,
        /// an appropriate version of the driver, and the CUSPARSE library are correctly installed.
        /// </remarks>
        NotInitialized = 1,
        /// <summary>
        /// Resource allocation failed inside the CUSPARSE library. 
        /// </summary>
        /// <remarks>
        /// This is usually caused by a 
        /// cudaMalloc() failure. To correct: prior to the function call, deallocate previously allocated
        /// memory as much as possible.
        /// </remarks>
        AllocFailed = 2,
        /// <summary>
        /// An unsupported value or parameter was passed to the function (a negative vector size, for example). 
        /// </summary>
        /// <remarks>
        /// To correct: ensure that all the parameters being passed have valid values.
        /// </remarks>
        InvalidValue = 3,
        /// <summary>
        /// The function requires a feature absent from the device architecture; 
        /// </summary>
        /// <remarks>
        /// Usually caused by the lack of support for atomic operations or double precision.
        /// To correct: compile and run the application on a device with appropriate compute capability,
        /// which is 1.1 for 32-bit atomic operations and 1.3 for double precision.
        /// </remarks>
        ArchMismatch = 4,
        /// <summary>
        /// An access to GPU memory space failed, which is usually caused by a failure to bind a texture.
        /// </summary>
        /// <remarks>
        /// To correct: prior to the function call, unbind any previously bound textures.
        /// </remarks>
        MappingError = 5,
        /// <summary>
        /// The GPU program failed to execute. 
        /// </summary>
        /// <remarks>
        /// This is often caused by a launch failure of the kernel on the GPU, which can be
        /// caused by multiple reasons. To correct: check that the hardware, an appropriate
        /// version of the driver, and the CUSPARSE library are correctly installed.
        /// </remarks>
        ExecutionFailed = 6,
        /// <summary>
        /// An internal CUSPARSE operation failed. 
        /// </summary>
        /// <remarks>
        /// This error is usually caused by a cudaMemcpyAsync() failure. To correct: check that the hardware,
        /// an appropriate version of the driver, and the CUSPARSE library are correctly installed. Also,
        /// check that the memory passed as a parameter to the routine is not being deallocated prior to
        /// the routine's completion.
        /// </remarks>
        InternalError = 7,
        /// <summary>
        /// The matrix type is not supported by this function. 
        /// </summary>
        /// <remarks>
        /// This is usually caused by passing an invalid matrix descriptor to the function.
        /// To correct: check that the fields in IntPtr_t descrA were set correctly.
        /// </remarks>
        MatrixTypeNotSupported = 8,
        /// <summary>
        ///
        /// </summary>
        ZeroPivot = 9
    }

    public enum MatrixType
    {
        /// <summary>
        /// the matrix is general.
        /// </summary>
        General = 0,
        /// <summary>
        /// the matrix is symmetric.
        /// </summary>
        Symmetric = 1,
        /// <summary>
        /// the matrix is Hermitian.
        /// </summary>
        Hermitian = 2,
        /// <summary>
        /// the matrix is triangular.
        /// </summary>
        Triangular = 3
    }

    internal enum FillMode
    {
        /// <summary>
        /// the lower triangular part is stored.
        /// </summary>
        Lower = 0,
        /// <summary>
        /// the upper triangular part is stored.
        /// </summary>
        Upper = 1
    }

    internal enum DiagonalType
    {
        /// <summary>
        /// the matrix diagonal has non-unit elements.
        /// </summary>
        NonUnit = 0,
        /// <summary>
        /// the matrix diagonal has unit elements.
        /// </summary>
        Unit = 1
    }

    internal enum IndexBase
    {
        /// <summary>
        /// the base index is zero.
        /// </summary>
        Zero = 0,
        /// <summary>
        /// the base index is one.
        /// </summary>
        One = 1
    }

    #endregion

    #region Solver

    public enum SolverStatus
    {
        /// <summary>
        /// The operation completed successfully
        /// </summary>
        Success = 0,
        /// <summary>
        /// The cuSolver library was not initialized. 
        /// </summary>
        /// <remarks>
        /// This is usually caused by the lack of a prior call, an error in the CUDA Runtime
        /// API called by the cuSolver routine, or an error in the hardware setup.
        /// To correct: call cusolverCreate() prior to the function call; and
        /// check that the hardware, an appropriate version of the driver, and the
        /// cuSolver library are correctly installed.
        /// </remarks>
        NotInititialized = 1,
        /// <summary>
        /// Resource allocation failed inside the cuSolver library. 
        /// </summary>
        /// <remarks>
        /// This is usually caused by a cudaMalloc() failure.
        /// To correct: prior to the function call, deallocate previously allocated
        /// memory as much as possible.
        /// </remarks>
        AllocFailed = 2,
        /// <summary>
        /// An unsupported value or parameter was passed to the function (a negative vector size, for example).
        /// </summary>
        /// <remarks>
        /// To correct: ensure that all the parameters being passed have valid values.
        /// </remarks>
        InvalidValue = 3,
        /// <summary>
        /// The function requires a feature absent from the device architecture.
        /// <remarks>
        /// Usually caused by the lack of support for atomic operations or double precision.
        /// To correct: compile and run the application on a device with compute capability 2.0 or above.
        /// </remarks>
        /// </summary>
        ArchMismatch = 4,
        /// <summary>
        /// 
        /// </summary>
        MappingError = 5,
        /// <summary>
        /// The GPU program failed to execute.
        /// </summary>
        /// <remarks>
        /// This is often caused by a launch failure of the kernel on the GPU, which
        /// can be caused by multiple reasons.
        /// To correct: check that the hardware, an appropriate version of the
        /// driver, and the cuSolver library are correctly installed.
        /// </remarks>
        ExecutionFailed = 6,
        /// <summary>
        /// An internal cuSolver operation failed.
        /// </summary>
        /// <remarks>
        /// This error is usually caused by a cudaMemcpyAsync() failure.
        /// To correct: check that the hardware, an appropriate version of the
        /// driver, and the cuSolver library are correctly installed. Also, check
        /// that the memory passed as a parameter to the routine is not being
        /// deallocated prior to the routine's completion.
        /// </remarks>
        InternalError = 7,
        /// <summary>
        /// The matrix type is not supported by this function.
        /// </summary>
        /// <remarks>
        /// This is usually caused by passing an invalid matrix descriptor to the function.
        /// To correct: check that the fields in descrA were set correctly.
        /// </remarks>
        MatrixTypeNotSupported = 8,
        /// <summary>
        /// 
        /// </summary>
        NotSupported = 9,
        /// <summary>
        /// 
        /// </summary>
        ZeroPivot = 10,
        /// <summary>
        /// 
        /// </summary>
        InvalidLicense = 11
    }
    
    #endregion
}
