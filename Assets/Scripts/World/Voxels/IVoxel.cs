using FactoryZero.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace FactoryZero.Voxels
{
    public interface IVoxel : ISaveFile
    {
        Vector2 BiomeParameters { get; set; }
        bool IsGrassy { get; set; }
        float GrassAmount { get; set; }
        VoxelMaterial Material { get; set; }
        VoxelBiome Biome { get; set; }
        float Volume { get; set; }
    }
}