using System;
using System.Collections.Generic;
using System.Text;

namespace Sample04
{
    public unsafe struct LaunchParams
    {
        public int FrameID;
        public uint* ColorBuffer;
        public Camera camera;
        public IntPtr traversable;
    }
}
