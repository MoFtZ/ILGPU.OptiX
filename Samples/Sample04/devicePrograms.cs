using ILGPU;
using ILGPU.OptiX;

namespace Sample04
{
    public static class devicePrograms
    {
        public unsafe static void __raygen__renderFrame(LaunchParams launchParams)
        {
            var ix = OptixGetLaunchIndex.X;
            var iy = OptixGetLaunchIndex.Y;
            int frameID = launchParams.FrameID;

            uint r = ((uint)((ix + frameID) % 256));
            uint g = ((uint)((iy + frameID) % 256));
            uint b = ((uint)((ix + iy + frameID) % 256));

            // convert to 32-bit bgra value (we explicitly set alpha to 0xff
            // to make stb_image_write happy ...
            uint rgba = 0xff000000 | (b << 0) | (g << 8) | (r << 16);

            // and write to frame buffer ...
            long fbIndex = ix + iy * launchParams.camera.width;
            launchParams.ColorBuffer[fbIndex] = rgba;
        }

        public static void __miss__radiance(LaunchParams launchParams)
        { }

        public static void __closest__radiance(LaunchParams launchParams)
        { }

        public static void __anyhit__radiance(LaunchParams launchParams)
        { }

        public static void flipBitmap(Index1D index, int width, int height, ArrayView<byte> source, ArrayView<byte> dest)
        {


            int x = index % width;
            int y = (height - 1) - (index / width);

            int newIndex = ((y * width) + x) * 4;
            int oldIndexStart = index * 4;

            dest[newIndex] = source[oldIndexStart];
            dest[newIndex + 1] = source[oldIndexStart + 1];
            dest[newIndex + 2] = source[oldIndexStart + 2];
            dest[newIndex + 3] = source[oldIndexStart + 3];
        }
    }
}
