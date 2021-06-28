using System.Collections.Generic;
using ILGPU;
using ILGPU.Runtime;

namespace Sample04
{
    public class TriangleMesh
    {
        public Accelerator accelerator;
        public List<Vec3> vertexBuffer;
        public List<Vec3i> triangleIndexBuffer;

        public MemoryBuffer1D<Vec3, Stride1D.Dense> d_vertexBuffer;
        public MemoryBuffer1D<Vec3i, Stride1D.Dense> d_triangleIndexBuffer;

        private static int[] indices =
            {
                0, 1, 3,
                2, 3, 0,
                5, 7, 6,
                5, 6, 4,
                0, 4, 5,
                0, 5, 1,
                2, 3, 7,
                2, 7, 6,
                1, 5, 7,
                1, 7, 3,
                4, 0, 2,
                4, 2, 6
            };

        public TriangleMesh(Accelerator accelerator)
        {
            this.accelerator = accelerator;
            vertexBuffer = new List<Vec3>();
            triangleIndexBuffer = new List<Vec3i>();
        }

        public void addUnitCube(Vec3 center, Vec3 size)
        {
            AddCube(new Affine3f(center, size));
        }

        public void AddCube(Affine3f xfm)
        {
            int firstVertexID = vertexBuffer.Count;
            vertexBuffer.Add(xfm.xfmPoint(new Vec3(0, 0, 0)));
            vertexBuffer.Add(xfm.xfmPoint(new Vec3(1, 0, 0)));
            vertexBuffer.Add(xfm.xfmPoint(new Vec3(0, 1, 0)));
            vertexBuffer.Add(xfm.xfmPoint(new Vec3(1, 1, 0)));
            vertexBuffer.Add(xfm.xfmPoint(new Vec3(0, 0, 1)));
            vertexBuffer.Add(xfm.xfmPoint(new Vec3(1, 0, 1)));
            vertexBuffer.Add(xfm.xfmPoint(new Vec3(0, 1, 1)));
            vertexBuffer.Add(xfm.xfmPoint(new Vec3(1, 1, 1)));

            for (int i = 0; i < 12; i++)
            {
                triangleIndexBuffer.Add(firstVertexID + new Vec3i(indices[3 * i + 0], indices[3 * i + 1], indices[3 * i + 2]));
            }

            if (d_triangleIndexBuffer != null)
            {
                d_triangleIndexBuffer.Dispose();
                d_vertexBuffer.Dispose();
            }

            d_vertexBuffer = accelerator.Allocate1D(vertexBuffer.ToArray());
            d_triangleIndexBuffer = accelerator.Allocate1D(triangleIndexBuffer.ToArray());
        }
    }
}
