﻿using Avalonia.Media.Imaging;
using Avalonia.Platform;
using PhotoTool.Shared.Constants;

namespace PhotoTool.Shared.Resources
{
    public interface IAssetProvider
    {
        Bitmap GetImage(Assets asset);
    }

    public class AssetProvider : IAssetProvider
    {
        public Bitmap GetImage(Assets asset)
        {
            return new Bitmap(AssetLoader.Open(new System.Uri(asset.Uri)));
        }
    }
}
