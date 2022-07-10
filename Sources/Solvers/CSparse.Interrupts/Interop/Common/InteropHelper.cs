namespace CSparse.Interop.Common
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    internal static class InteropHelper
    {
        /// <summary>
        /// Pin given object and add handle to list.
        /// </summary>
        /// <param name="data">Object to pin.</param>
        /// <param name="handles">List of handles.</param>
        /// <returns>Address of pinned object.</returns>
        public static IntPtr Pin(object data, List<GCHandle> handles)
        {
            if (data == null) return IntPtr.Zero;
            
            var h = GCHandle.Alloc(data, GCHandleType.Pinned);

            handles.Add(h);

            return h.AddrOfPinnedObject();
        }

        /// <summary>
        /// Free all handles of given list.
        /// </summary>
        /// <param name="handles">List of handles.</param>
        public static void Free(List<GCHandle> handles)
        {
            if (handles == null) return;

            foreach (var h in handles)
            {
                h.Free();
            }

            handles.Clear();
        }
    }
}
