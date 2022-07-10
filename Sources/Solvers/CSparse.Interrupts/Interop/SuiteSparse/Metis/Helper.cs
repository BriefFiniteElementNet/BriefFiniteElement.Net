
namespace CSparse.Interop.SuiteSparse.Metis
{
    static class Helper
    {
        public static int[] CreateArray(int n, int value)
        {
            var data = new int[n];

            for (int i = 0; i < n; i++)
            {
                data[i] = value;
            }

            return data;
        }
    }
}
