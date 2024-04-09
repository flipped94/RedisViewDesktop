using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;

namespace RedisViewDesktop.Helpers
{
    public class ImageHelper
    {
        public static Bitmap LoadFromResources(Uri resourceUri)
        {
            return new Bitmap(AssetLoader.Open(resourceUri));
        }
    }
}
