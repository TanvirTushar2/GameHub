using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace GameHub.Services
{
    /// <summary>
    /// Loads local artwork without keeping files locked and reuses it across cards/pages.
    /// </summary>
    internal static class ImageCacheService
    {
        private static readonly object Sync = new object();
        private static readonly Dictionary<string, Image> Cache =
            new Dictionary<string, Image>(StringComparer.OrdinalIgnoreCase);

        public static Image Load(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path)) return null;

            lock (Sync)
            {
                Image cached;
                if (Cache.TryGetValue(path, out cached)) return cached;

                byte[] data = File.ReadAllBytes(path);
                using (MemoryStream ms = new MemoryStream(data))
                using (Image source = Image.FromStream(ms))
                {
                    Image clone = new Bitmap(source);
                    Cache[path] = clone;
                    return clone;
                }
            }
        }

        public static void Clear()
        {
            lock (Sync)
            {
                foreach (Image image in Cache.Values)
                    if (image != null) image.Dispose();
                Cache.Clear();
            }
        }
    }
}
