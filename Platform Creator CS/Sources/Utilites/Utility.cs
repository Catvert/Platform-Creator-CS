using System;
using System.IO;

namespace Platform_Creator_CS.Utility {
    public static class Utility {
        public static void Resize2DArray<T>(ref T[][] array, int newXSize, int newYSize) {
            Array.Resize(ref array, newXSize);

            for (var i = 0; i < array.Length; ++i) Array.Resize(ref array[i], newYSize);
        }

        public static void CopyDirectoryRecursively(DirectoryInfo sourceDir, DirectoryInfo targetDir) {
            foreach (var dir in sourceDir.GetDirectories())
                CopyDirectoryRecursively(dir, targetDir.CreateSubdirectory(dir.Name));
            foreach (var file in sourceDir.GetFiles())
                file.CopyTo(Path.Combine(targetDir.FullName, file.Name));
        }
    }
}