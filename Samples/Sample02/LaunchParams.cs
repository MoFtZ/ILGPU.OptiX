namespace Sample02
{
    public unsafe struct LaunchParams
    {
        public int FrameID;
        public uint* ColorBuffer;
        public int FbSizeX;
        public int FbSizeY;
    }
}
