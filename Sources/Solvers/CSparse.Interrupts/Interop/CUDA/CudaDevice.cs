
namespace CSparse.Interop.CUDA
{
    using System;

    // Taken from CUDASamples\common\inc\helper_cuda.h

    static class CudaDevice
    {
        public static int Initialize()
        {
            // Otherwise pick the device with highest Gflops/s
            int devID = GetMaxGflopsDevice();

            Check(NativeMethods.cudaSetDevice(devID));
            
            return devID;
        }

        public static int GetAttribute(DeviceAttribute attribute, int device)
        {
            int value = 0;

            Check(NativeMethods.cudaDeviceGetAttribute(ref value, attribute, device));
            
            return value;
        }

        // This function returns the best GPU (with maximum GFLOPS)
        private static int GetMaxGflopsDevice()
        {
            int current_device = 0, sm_per_multiproc = 0;
            int max_perf_device = 0;
            int max_compute_perf = 0;
            int device_count = 0, best_SM_arch = 0;
            int devices_prohibited = 0;

            Check(NativeMethods.cudaGetDeviceCount(ref device_count));

            if (device_count == 0)
            {
                throw new CudaException(CudaResult.NoDevice); // CUDA error: no devices supporting CUDA.
            }

            // Find the best major SM Architecture GPU device
            while (current_device < device_count)
            {
                int computeMode = 0;
                NativeMethods.cudaDeviceGetAttribute(ref computeMode, DeviceAttribute.ComputeMode, current_device);

                int major = 0;
                NativeMethods.cudaDeviceGetAttribute(ref major, DeviceAttribute.ComputeCapabilityMajor, current_device);

                // If this GPU is not running on Compute Mode prohibited,
                // then we can add it to the list
                if (computeMode != (int)ComputeMode.Prohibited)
                {
                    if (major > 0 && major < 9999)
                    {
                        best_SM_arch = Math.Max(best_SM_arch, major);
                    }
                }
                else
                {
                    devices_prohibited++;
                }

                current_device++;
            }

            if (devices_prohibited == device_count)
            {
                throw new CudaException(CudaResult.DevicesUnavailable); // CUDA error: all devices have compute mode prohibited.
            }

            // Find the best CUDA capable GPU device
            current_device = 0;

            while (current_device < device_count)
            {
                int computeMode = 0;
                NativeMethods.cudaDeviceGetAttribute(ref computeMode, DeviceAttribute.ComputeMode, current_device);

                int major = 0;
                NativeMethods.cudaDeviceGetAttribute(ref major, DeviceAttribute.ComputeCapabilityMajor, current_device);

                int minor = 0;
                NativeMethods.cudaDeviceGetAttribute(ref minor, DeviceAttribute.ComputeCapabilityMinor, current_device);

                int multiProcessorCount = 0;
                NativeMethods.cudaDeviceGetAttribute(ref multiProcessorCount, DeviceAttribute.MultiProcessorCount, current_device);

                int clockRate = 0;
                NativeMethods.cudaDeviceGetAttribute(ref clockRate, DeviceAttribute.ClockRate, current_device);

                // If this GPU is not running on Compute Mode prohibited, then we can add it to the list
                if (computeMode != (int)ComputeMode.Prohibited)
                {
                    if (major == 9999 && minor == 9999)
                    {
                        sm_per_multiproc = 1;
                    }
                    else
                    {
                        sm_per_multiproc = ConvertSMVer2Cores(major, minor);
                    }

                    int compute_perf = multiProcessorCount * sm_per_multiproc * clockRate;

                    if (compute_perf > max_compute_perf)
                    {
                        // If we find GPU with SM major > 2, search only these
                        if (best_SM_arch > 2)
                        {
                            // If our device==dest_SM_arch, choose this, or else pass
                            if (major == best_SM_arch)
                            {
                                max_compute_perf = compute_perf;
                                max_perf_device = current_device;
                            }
                        }
                        else
                        {
                            max_compute_perf = compute_perf;
                            max_perf_device = current_device;
                        }
                    }
                }

                ++current_device;
            }

            return max_perf_device;
        }
        
        private static int ConvertSMVer2Cores(int major, int minor)
        {
            // Defines for GPU Architecture types (using the SM version to determine the # of cores per SM).

            int[][] nGpuArchCoresPerSM = {
                new int[] {0x30, 192},  // Kepler Generation (SM 3.0) GK10x class
                new int[] {0x32, 192},  // Kepler Generation (SM 3.2) GK10x class
                new int[] {0x35, 192},  // Kepler Generation (SM 3.5) GK11x class
                new int[] {0x37, 192},  // Kepler Generation (SM 3.7) GK21x class
                new int[] {0x50, 128},  // Maxwell Generation (SM 5.0) GM10x class
                new int[] {0x52, 128},  // Maxwell Generation (SM 5.2) GM20x class
                new int[] {0x53, 128},  // Maxwell Generation (SM 5.3) GM20x class
                new int[] {0x60, 64},   // Pascal Generation (SM 6.0) GP100 class
                new int[] {0x61, 128},  // Pascal Generation (SM 6.1) GP10x class
                new int[] {0x62, 128},  // Pascal Generation (SM 6.2) GP10x class
                new int[] {0x70, 64},   // Volta Generation (SM 7.0) GV100 class
                new int[] {0x72, 64},   // Volta Generation (SM 7.2) GV11b class
                new int[] {-1, -1}
            };

            int index = 0;

            while (nGpuArchCoresPerSM[index][0] != -1)
            {
                if (nGpuArchCoresPerSM[index][0] == ((major << 4) + minor))
                {
                    return nGpuArchCoresPerSM[index][1];
                }

                index++;
            }

            // If we don't find the values, we default use the previous one to run properly.
            return nGpuArchCoresPerSM[index - 1][1];
        }

        // Initialization code to find the best CUDA Device
        private static void Check(CudaResult result)
        {
            if (result != CudaResult.Success)
            {
                throw new CudaException(result);
            }
        }
    }
}
