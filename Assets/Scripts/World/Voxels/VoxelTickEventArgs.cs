using FactoryZero.Worlds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FactoryZero.Voxels
{
    public class VoxelTickEventArgs
    {
        Vector3Int voxelIndex;

        IVoxel voxel;
        
        WorldChunk chunk;
        GameWorld world;
        SaveData save;

        VoxelBiome biome;
        VoxelBiomeManager biomeManager;

        VoxelMaterial material;
        VoxelMaterialManager materialManager;

        public VoxelTickEventArgs(Vector3Int voxelIndex, IVoxel voxel, WorldChunk chunk, GameWorld world, SaveData save, VoxelBiome biome, VoxelBiomeManager biomeManager, VoxelMaterial material, VoxelMaterialManager materialManager)
        {
            this.voxelIndex = voxelIndex;
            this.voxel = voxel;
            this.chunk = chunk;
            this.world = world;
            this.save = save;
            this.biome = biome;
            this.biomeManager = biomeManager;
            this.material = material;
            this.materialManager = materialManager;
        }

        public IVoxel Voxel { get => voxel; }
        public Vector3Int VoxelIndex { get => voxelIndex; }
        public WorldChunk Chunk { get => chunk; }
        public GameWorld World { get => world; }
        public SaveData Save { get => save; }
        public VoxelBiome Biome { get => biome; }
        public VoxelBiomeManager BiomeManager { get => biomeManager; }
        public VoxelMaterial Material { get => material; }
        public VoxelMaterialManager MaterialManager { get => materialManager; }
    }
}
