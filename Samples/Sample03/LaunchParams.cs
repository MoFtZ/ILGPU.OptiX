using System;
using System.Collections.Generic;
using System.Text;

namespace Sample03
{
    public unsafe struct LaunchParams
    {
        public int FrameID;
        public uint* ColorBuffer;
        public int FbSizeX;
        public int FbSizeY;
    }
}
