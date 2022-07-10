
namespace CSparse.Interop.MKL
{
    using System;
    using System.Text;

    public static class Helper
    {
        /// <summary>
        /// Gets the MKL version.
        /// </summary>
        public static Version GetVersion()
        {
            MKLVersion version = default(MKLVersion);

            // NOTE: leaks memory (char* pointers in MKLVersion struct).

            NativeMethods.mkl_get_version(ref version);

            return new Version(version.MajorVersion, version.MinorVersion, version.BuildNumber);
        }

        /// <summary>
        /// Gets the MKL version string.
        /// </summary>
        public static string GetVersionString()
        {
            var buffer = new StringBuilder(200);

            NativeMethods.mkl_get_version_string(buffer, 200);

            return buffer.ToString().TrimEnd();
        }

        /// <summary>
        /// Sets the number of threads to use.
        /// </summary>
        /// <param name="nt">The number of threads.</param>
        public static void SetNumThreads(int nt)
        {
            NativeMethods.mkl_set_num_threads(nt);
        }

        /// <summary>
        /// Gets the number of threads targeted for parallelism.
        /// </summary>
        public static int GetMaxThreads()
        {
            return NativeMethods.mkl_get_max_threads();
        }

        /*

        public static int SetCWBR(CBWR cbwr)
        {
            return NativeMethods.mkl_cbwr_set((int)cbwr);
        }

        public static CBWR GetCWBR()
        {
            return (CBWR)NativeMethods.mkl_cbwr_get(MKL_CBWR_ALL);
        }

        public enum CBWR
        {
            UNSET_ALL = 0,
            OFF = 0,
            BRANCH_OFF = 1,
            AUTO = 2,
            COMPATIBLE = 3,
            SSE2 = 4,
            SSE3 = 5,
            SSSE3 = 6,
            SSE4_1 = 7,
            SSE4_2 = 8,
            AVX = 9,
            AVX2 = 10
        }

        const int MKL_CBWR_BRANCH = 1;
        const int MKL_CBWR_ALL = ~0;

        // error codes
        const int CBWR_SUCCESS = 0;
        const int CBWR_ERR_INVALID_SETTINGS = -1;
        const int CBWR_ERR_INVALID_INPUT = -2;
        const int CBWR_ERR_UNSUPPORTED_BRANCH = -3;
        const int CBWR_ERR_UNKNOWN_BRANCH = -4;
        const int CBWR_ERR_MODE_CHANGE_FAILURE = -8;

        //*/
    }
}
