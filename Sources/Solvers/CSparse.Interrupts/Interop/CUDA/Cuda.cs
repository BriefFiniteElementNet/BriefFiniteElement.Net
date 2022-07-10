using System;

using size_t = System.Int32; // TODO: x64

namespace CSparse.Interop.CUDA
{
    public class Cuda
    {
        /// <summary>
        /// Initializes the CUDA device with maximum GFLOPS.
        /// </summary>
        /// <returns>The CUDA device id.</returns>
        /// <exception cref="CudaException">Will throw, if no device is found.</exception>
        public static int Initialize()
        {
            return CudaDevice.Initialize();
        }

        /// <summary>
        /// Returns the requested CUDA device attribute.
        /// </summary>
        /// <param name="attribute">The <see cref="DeviceAttribute"/>.</param>
        /// <param name="device">The device id.</param>
        /// <returns></returns>
        public static int GetDeviceAttribute(DeviceAttribute attribute, int device)
        {
            int value = 0;

            Check(NativeMethods.cudaDeviceGetAttribute(ref value, attribute, device));

            return value;
        }

        public static int GetDriverVersion()
        {
            int version = 0;
            Check(NativeMethods.cudaDriverGetVersion(ref version));
            return version;
        }

        public static int GetRuntimeVersion()
        {
            int version = 0;
            Check(NativeMethods.cudaRuntimeGetVersion(ref version));
            return version;
        }

        internal static void Malloc(ref IntPtr p, size_t size)
        {
            Check(NativeMethods.cudaMalloc(ref p, size));
        }

        internal static void CopyToDevice(IntPtr dest, IntPtr src, size_t size)
        {
            Check(NativeMethods.cudaMemcpy(dest, src, size, MemcpyKind.HostToDevice));
        }

        internal static void CopyToHost(IntPtr dest, IntPtr src, size_t size)
        {
            Check(NativeMethods.cudaMemcpy(dest, src, size, MemcpyKind.DeviceToHost));
        }

        internal static void Free(IntPtr p)
        {
            NativeMethods.cudaFree(p);
        }

        private static void Check(CudaResult result)
        {
            if (result != CudaResult.Success)
            {
                throw new Exception("CudaResult " + result);
            }
        }
    }
}
