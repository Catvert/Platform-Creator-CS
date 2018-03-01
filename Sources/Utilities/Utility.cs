using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace Platform_Creator_CS.Utility {
    public static class Utility {
        /// <summary>
        /// Inspired by Manuel in https://stackoverflow.com/questions/6539571/how-to-resize-multidimensional-2d-array-in-c
        /// </summary>
        public static void Resize2DArray<T>(ref T[,] array, int newXSize, int newYSize) {
            var newArray = new T[newXSize, newYSize];
            var minX = Math.Min(array.GetLength(0), newArray.GetLength(0));
            var minY = Math.Min(array.GetLength(1), newArray.GetLength(1));

            for(var i = 0; i < minY; ++i)
                Array.Copy(array, i * array.GetLength(0), newArray, i * newArray.GetLength(0), minX);

            array = newArray;
        }
    }
}