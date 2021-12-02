using Exiled.API.Features;
using System;

namespace PixelScreens
{
    public class MainClass : Plugin<PluginConfig>
    {
        public override string Name { get; } = "PixelScreens";
        public override string Author { get; } = "Killers0992";
        public override string Prefix { get; } = "pixelscreens";
        public override Version RequiredExiledVersion { get; } = new Version(4, 0, 0);
        public override Version Version { get; } = new Version(1, 0, 0);
    }
}
