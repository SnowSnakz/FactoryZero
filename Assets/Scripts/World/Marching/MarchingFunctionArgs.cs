using FactoryZero.Interfaces;
using FactoryZero.Voxels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FactoryZero.Marching
{
    public class MarchingFunctionArgs
    {
        IGrid3D<IVoxel> voxels;

        public IVoxel this[int x, int y, int z]
        {
            get
            {
                return voxels[x, y, z];
            }
        }
        public IVoxel this[Vector3Int pos]
        {
            get
            {
                return voxels[pos.x, pos.y, pos.z];
            }
        }

        public List<MarchingMeshVertex[]> Triangles { get; } = new List<MarchingMeshVertex[]>();

        public int Width { get => voxels.Width; }
        public int Height { get => voxels.Height; }
        public int Length { get => voxels.Length; }

        float isoLevel;
        public float IsoLevel { get => isoLevel; }

        public MarchingFunctionArgs(IGrid3D<IVoxel> voxels, float isoLevel)
        {
            this.voxels = voxels;
            this.isoLevel = isoLevel;
        }
    }
}
