using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Sample03
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int width = 1200;
        public int height = 800;
        public WriteableBitmap wBitmap;
        public Int32Rect rect;

        public SampleRenderer sampleRenderer;
        public Thread renderThread;

        public bool run = true;

        public MainWindow()
        {
            InitializeComponent();

            wBitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
            rect = new Int32Rect(0, 0, width, height);
            Frame.Source = wBitmap;

            sampleRenderer = new SampleRenderer(width, height, this);
            Closing += MainWindow_Closing;

            renderThread = new Thread(renderThreadMain);
            renderThread.Start();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            run = false;
            sampleRenderer.Dispose();
        }

        public void renderThreadMain()
        {
            while (run)
            {
                sampleRenderer.render();
            }
        }

        public void draw(ref byte[] data)
        {
            if (data.Length == wBitmap.PixelWidth * wBitmap.PixelHeight * 4)
            {
                wBitmap.Lock();
                IntPtr pBackBuffer = wBitmap.BackBuffer;
                Marshal.Copy(data, 0, pBackBuffer, data.Length);
                wBitmap.AddDirtyRect(rect);
                wBitmap.Unlock();
            }
        }
    }
}
