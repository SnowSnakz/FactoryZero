using FactoryZero.Interfaces;
using FactoryZero.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FactoryZero.Voxels
{
    public class VoxelBiomeManager : MonoBehaviour
    {
        [Tooltip("An temperature/moisture map which specifies the exact biome for a given combination.")]
        public Texture2D biomeMap;

        public Texture2D caveBiomeMap;
        
        public Texture2D caveMinHeightMap;
        public Texture2D caveMaxHeightMap;

        public Texture2D heightMap;

        [Tooltip("The biome to be used in-place of a biome with an unrecognized id.")]
        public VoxelBiome defaultBiome;

        List<VoxelBiome> biomes = new List<VoxelBiome>();
        VoxelBiome[] biomesArray;

        IGrid2D<int> convertedBiomeMap;

        IGrid2D<int> convertedCaveBiomeMap;

        IGrid2D<float> convertedCaveMaxHeightMap;
        IGrid2D<float> convertedCaveMinHeightMap;

        IGrid2D<float> convertedHeightMap;

        public void Rearrange(string[] newOrder)
        {
            VoxelBiome[] newArray = new VoxelBiome[Math.Max(newOrder.Length, biomes.Count)];

            int count = 1;
            for (int i = 0; i < biomes.Count; i++)
            {
                int nextPos = newOrder.Length - (count++);
                string mn = biomes[i].name;

                for (int j = 0; j < newOrder.Length; j++)
                {
                    if (newOrder[j] == mn)
                    {
                        nextPos = j;
                    }
                }

                newArray[nextPos] = biomes[i];
            }

            biomesArray = newArray;

            biomes.Clear();
            biomes.AddRange(biomesArray);
        }


        public VoxelBiome GetBiomeByIndex(int index)
        {
            return Biomes[index];
        }

        public float GetHeightByParameters(float x, float y)
        {
            x = Mathf.Clamp01(x);
            y = Mathf.Clamp01(y);

            float xl = Mathf.Clamp(x * convertedHeightMap.Width, 0, convertedHeightMap.Width - 1);
            float yl = Mathf.Clamp(y * convertedHeightMap.Height, 0, convertedHeightMap.Height - 1);
            float xh = Mathf.Clamp(x * convertedHeightMap.Width + 1, 0, convertedHeightMap.Width - 1);
            float yh = Mathf.Clamp(y * convertedHeightMap.Height + 1, 0, convertedHeightMap.Height - 1);

            float x1y1, x1y2, x2y1, x2y2;
            x1y1 = convertedHeightMap[Mathf.FloorToInt(xl), Mathf.FloorToInt(yl)];
            x2y1 = convertedHeightMap[Mathf.FloorToInt(xh), Mathf.FloorToInt(yl)];
            x1y2 = convertedHeightMap[Mathf.FloorToInt(xl), Mathf.FloorToInt(yh)];
            x2y2 = convertedHeightMap[Mathf.FloorToInt(xh), Mathf.FloorToInt(yh)];

            return Mathf.Lerp(Mathf.Lerp(x1y1, x2y1, xl), Mathf.Lerp(x1y2, x2y2, xl), yl);
        }

        public VoxelBiome GetBiomeByParameters(float x, float y, float height, bool includeCaves = true)
        {
            int xx = Mathf.FloorToInt(Mathf.Abs(x) * (convertedBiomeMap.Width - 1));
            int yy = Mathf.FloorToInt(Mathf.Abs(y) * (convertedBiomeMap.Height - 1));

            VoxelBiome surfaceBiome = GetBiomeByIndex(convertedBiomeMap[xx, yy]);
            
            if(includeCaves)
            {
                VoxelBiome caveBiome = GetBiomeByIndex(convertedCaveBiomeMap[xx, yy]);

                if (height <= convertedCaveMaxHeightMap[xx, yy] && height >= convertedCaveMinHeightMap[xx, yy])
                {
                    return caveBiome;
                }
            }

            return surfaceBiome;
        }

        public string[] BiomeOrder
        {
            get
            {
                string[] ids = new string[biomes.Count];

                for(int i = 0; i < ids.Length; i++)
                {
                    ids[i] = biomes[i].name;
                }

                return ids;
            }
        }

        public VoxelBiome[] Biomes
        {
            get
            {
                if(biomesArray == null)
                {
                    biomesArray = biomes.ToArray();
                }
                else
                {
                    if(biomesArray.Length != biomes.Count)
                    {
                        biomesArray = biomes.ToArray();
                    }
                }

                return biomesArray;
            }
        }

        private void Start()
        {
            biomesArray = FindObjectsOfType<VoxelBiome>();
            biomes.AddRange(biomesArray);

            Dictionary<Color32, VoxelBiome> mappedBiomes = new Dictionary<Color32, VoxelBiome>();

            foreach(VoxelBiome b in biomes)
            {
                if(b.isMapped)
                {
                    mappedBiomes[b.mappedColor] = b;
                }
            }

            if(defaultBiome == null)
            {
                Debug.LogWarning($"It might be a good idea to specify a Default Biome.");
            }

            if(biomeMap == null)
            {
                throw new NullReferenceException($"Please specify a Biome Map before starting.");
            }

            if(!biomeMap.isReadable)
            {
                throw new InvalidOperationException($"Please make the texture \"{biomeMap.name}\" readable by navigating to it in the asset browser and ticking the Read/Write Checkbox and hitting Apply.");
            }

            if(biomeMap.format != TextureFormat.RGBA32)
            {
                throw new InvalidOperationException($"Please ensure that the pixel format of \"{biomeMap.name}\" is RGBA32 and that it is uncompressed.");
            }

            convertedCaveBiomeMap = new IntGrid2D(caveBiomeMap.width, caveBiomeMap.height);
            for(int x = 0; x < convertedCaveBiomeMap.Width; x++)
            {
                for (int y = 0; y < convertedCaveBiomeMap.Height; y++)
                {
                    Color32 pixel = caveBiomeMap.GetPixel(x, y);
                    if (mappedBiomes.ContainsKey(pixel))
                    {
                        convertedCaveBiomeMap[x, y] = biomes.IndexOf(mappedBiomes[pixel]);
                    }
                    else
                    {
                        convertedCaveBiomeMap[x, y] = -1;
                    }
                }
            }
            
            convertedBiomeMap = new IntGrid2D(biomeMap.width, biomeMap.height);
            for(int x = 0; x < convertedBiomeMap.Width; x++)
            {
                for (int y = 0; y < convertedBiomeMap.Height; y++)
                {
                    Color32 pixel = biomeMap.GetPixel(x, y);
                    if (mappedBiomes.ContainsKey(pixel))
                    {
                        convertedBiomeMap[x, y] = biomes.IndexOf(mappedBiomes[pixel]);
                    }
                    else
                    {
                        convertedBiomeMap[x, y] = -1;
                    }
                }
            }

            convertedCaveMaxHeightMap = new FloatGrid2D(caveMaxHeightMap.width, caveMaxHeightMap.height);
            for (int x = 0; x < convertedCaveMaxHeightMap.Width; x++)
            {
                for (int y = 0; y < convertedCaveMaxHeightMap.Height; y++)
                {
                    convertedCaveMaxHeightMap[x, y] = caveMaxHeightMap.GetPixel(x, y).r;
                }
            }

            convertedCaveMinHeightMap = new FloatGrid2D(caveMinHeightMap.width, caveMinHeightMap.height);
            for (int x = 0; x < convertedCaveMinHeightMap.Width; x++)
            {
                for (int y = 0; y < convertedCaveMinHeightMap.Height; y++)
                {
                    convertedCaveMinHeightMap[x, y] = caveMinHeightMap.GetPixel(x, y).r;
                }
            }

            convertedHeightMap = new FloatGrid2D(heightMap.width, heightMap.height);
            for (int x = 0; x < convertedHeightMap.Width; x++)
            {
                for (int y = 0; y < convertedHeightMap.Height; y++)
                {
                    convertedHeightMap[x, y] = heightMap.GetPixel(x, y).r;
                }
            }
        }
    }
}
