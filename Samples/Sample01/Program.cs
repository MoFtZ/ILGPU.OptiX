using ILGPU;
using ILGPU.OptiX;
using ILGPU.Runtime.Cuda;

namespace Sample01
{
    public class Program
    {
        static void Main()
        {
            using var context = Context.Create(b => b.Cuda().InitOptiX());
            using var accelerator = context.CreateCudaAccelerator(0);
        }
    }
}
