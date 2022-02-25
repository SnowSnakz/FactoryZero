using FactoryZero.Noise;
using FactoryZero.Voxels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactoryZero.Worlds.Generators
{
    [RequireComponent(typeof(WorldGenerator))]
    public class Version0WorldGenerator : MonoBehaviour
    {
        WorldGenerator generator;

        public float biomeParamsXConstant;
        public float biomeParamsYConstant;

        public VoxelMaterial dirt;
        public VoxelMaterial stone;

        public VoxelBiome defaultBiome;

        public void OnGenerate(GenerateFunctionArgs args)
        {
            // Debug.Log($"{nameof(Version0WorldGenerator)}.{nameof(OnGenerate)}({nameof(GenerateFunctionArgs)} {nameof(args)}) [chunkIndex=({args.Chunk.index.x}, {args.Chunk.index.y}),world={args.World.worldName}]");

            if(generator == null)
            {
                generator = GetComponent<WorldGenerator>();
            }

            WorldChunk chunk = args.Chunk;
            GameWorld world = args.World;
            int seed = world.seed;

            Vector2Int chunkOffset = chunk.index * new Vector2Int(chunk.size.x, chunk.size.z);

            for (int x = 0; x < chunk.size.x; x++)
            {
                for (int z = 0; z < chunk.size.z; z++)
                {
                    Vector2 biomeParams = new Vector2();

                    NoiseFunctionArgs xParam = new NoiseFunctionArgs(seed, new Vector3(chunkOffset.x + x, chunkOffset.y + z, biomeParamsXConstant), NoiseFunctionArgs.SamplerType.Is3D);
                    NoiseFunctionArgs yParam = new NoiseFunctionArgs(seed, new Vector3(chunkOffset.x + x, chunkOffset.y + z, biomeParamsYConstant), NoiseFunctionArgs.SamplerType.Is3D);

                    generator.noise.onGenerateNoise.Invoke(xParam);
                    generator.noise.onGenerateNoise.Invoke(yParam);

                    biomeParams.x = (xParam.Value + 1f) * 0.5f;
                    biomeParams.y = (yParam.Value + 1f) * 0.5f;

                    float height = world.biomeManager.GetHeightByParameters(biomeParams.x, biomeParams.y) * chunk.size.y;

                    for(int y = 0; y < Mathf.Min(height, chunk.size.y); y++)
                    {
                        IVoxel voxel = chunk.GetVoxel(new Vector3Int(x, y, z));
                        voxel.Volume = Mathf.Clamp01((height - y) / 3f);
                        voxel.BiomeParameters = biomeParams;
                        voxel.Biome = world.biomeManager.GetBiomeByParameters(biomeParams.x, biomeParams.y, height, false) ?? defaultBiome;
                        
                        if(voxel.Volume != 1)
                        {
                            voxel.IsGrassy = true;
                            voxel.GrassAmount = 0.5f;
                        }

                        if(y > (height - 4))
                        {
                            voxel.Material = dirt;
                        }
                        else
                        {
                            voxel.Material = stone;
                        }
                    }
                }
            }
        }
    }
}
