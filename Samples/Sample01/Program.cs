using ILGPU;
using ILGPU.OptiX;
using ILGPU.Runtime.Cuda;

namespace Sample01
{
    public class Program
    {
        static void Main()
        {
            using var context = new Context();
            using var accelerator = new CudaAccelerator(context).InitOptiX();
        }
    }
}
