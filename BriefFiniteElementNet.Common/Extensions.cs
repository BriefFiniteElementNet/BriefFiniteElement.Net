using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CSparse.Double;

using CSparse.Storage;

namespace BriefFiniteElementNet.Common
{

    public static class Extensions
    {
        /// <summary>
        /// Multiplies the <see cref="matrix"/> by a constant value.
        /// </summary>
        /// <param name="matrix">The Matrix</param>
        /// <param name="constant">The constant value</param>
        public static void MultiplyByConstant(this Matrix matrix, double constant)
        {
            for (var i = 0; i < matrix.CoreArray.Length; i++)
            {
                matrix.CoreArray[i] *= constant;
            }
        }

        public static void MultiplyRowByConstant(this Matrix matrix, int row, double constant)
        {
            for (int j = 0; j < matrix.ColumnCount; j++)
                matrix[row, j] *= constant;
        }

        public static void MultiplyColumnByConstant(this Matrix matrix, int column, double constant)
        {
            for (int i = 0; i < matrix.RowCount; i++)
                matrix[i, column] *= constant;
        }
    }

}