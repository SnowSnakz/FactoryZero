using FactoryZero.Voxels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace FactoryZero.Worlds
{
    public class GameWorld : MonoBehaviour
    {
        public static GameWorld world;

        public VoxelBiomeManager biomeManager;
        public VoxelMaterialManager materialManager;

        public string worldName;
        public string dimension;

        public float dayCycleLength;

        public WorldGenerator generator;
        public WorldChunk chunkPrefab;

        Dictionary<Vector2Int, WorldChunk> chunks = new Dictionary<Vector2Int, WorldChunk>();

        public int seed;

        public Light sunLight;
        public Light moonLight;

        public string dataPath;
        string worldFolder;

        public float currentCycleTime;
        public float moonCycleOffset;

        public float sunCenterDistance;
        public float moonCenterDistance;

        public Vector3 sunRotationDirection;
        public Vector3 moonRotationDirection;

        public AnimationCurve sunStrengthOverCycle;
        public AnimationCurve moonStrengthOverCycle;

        public string GetWorldFilePath()
        {
            return $"{dataPath}/{worldFolder}/dims/{dimension}/info.json";
        }

        public string GetMetadataFilePath()
        {
            return $"{dataPath}/{worldFolder}/info.json";
        }

        public string GetChunkFilePath(Vector2Int chunkIndex)
        {
            return $"{dataPath}/{worldFolder}/dims/{dimension}/chunks/chunk_x{chunkIndex.x}_z{chunkIndex.y}.dat";
        }

        public WorldChunk GetChunk(Vector2Int chunkIndex)
        {
            WorldChunk v;

            if (!chunks.TryGetValue(chunkIndex, out v))
            {
                v = null;
            }

            return v;
        }

        public WorldChunk LoadChunk(Vector2Int chunkIndex, bool generateOnLoad = true)
        {
            string chunkFile = GetChunkFilePath(chunkIndex);

            if(chunks.ContainsKey(chunkIndex))
            {
                return chunks[chunkIndex];
            }

            WorldChunk newChunk = Instantiate(chunkPrefab);
            newChunk.name = $"Chunk_{chunkIndex.x}x_{chunkIndex.y}z";
            newChunk.index = chunkIndex;
            newChunk.world = this;
            newChunk.inUse = true;
            newChunk.transform.position = new Vector3(chunkIndex.x * newChunk.size.x, 0, chunkIndex.y * newChunk.size.z);
            chunks.Add(chunkIndex, newChunk);

            /*
            if (chunks.ContainsKey(chunkIndex - Vector2Int.left))
            {
                WorldChunk c = chunks[chunkIndex - Vector2Int.left];
                
                for(int y = 0; y < c.size.y; y++)
                {
                    for (int z = 0; z < c.size.z; z++)
                    {
                        c.SetVoxel(new Vector3Int(c.size.x, y, z), newChunk.GetVoxel(new Vector3Int(0, y, z)));
                    }
                }
            }

            if (chunks.ContainsKey(chunkIndex - Vector2Int.up))
            {
                WorldChunk c = chunks[chunkIndex - Vector2Int.up];

                for (int y = 0; y < c.size.y; y++)
                {
                    for (int x = 0; x < c.size.z; x++)
                    {
                        c.SetVoxel(new Vector3Int(c.size.x, y, x), newChunk.GetVoxel(new Vector3Int(0, y, x)));
                    }
                }

            }

            if (chunks.ContainsKey(chunkIndex - Vector2Int.down))
            {
                WorldChunk c = chunks[chunkIndex - Vector2Int.down];

                for (int y = 0; y < c.size.y; y++)
                {
                    for (int x = 0; x < c.size.z; x++)
                    {
                        newChunk.SetVoxel(new Vector3Int(c.size.x, y, x), c.GetVoxel(new Vector3Int(0, y, x)));
                    }
                }

            }

            if (chunks.ContainsKey(chunkIndex - Vector2Int.right))
            {
                WorldChunk c = chunks[chunkIndex - Vector2Int.right];

                for (int y = 0; y < c.size.y; y++)
                {
                    for (int z = 0; z < c.size.z; z++)
                    {
                        newChunk.SetVoxel(new Vector3Int(c.size.x, y, z), c.GetVoxel(new Vector3Int(0, y, z)));
                    }
                }

            }
            */

            if (File.Exists(chunkFile))
            {
                using(FileStream fs = File.OpenRead(chunkFile))
                {
                    BinaryReader rdr = new BinaryReader(fs);
                    newChunk.Read(rdr);
                }
            }

            if(generateOnLoad)
            {
                GenerateChunk(newChunk, false);
            }

            return newChunk;
        }

        public WorldChunk GenerateChunk(WorldChunk chunk, bool force = false)
        {
            if(!chunk.hasGenerated || force)
            {
                generator.onGenerate.Invoke(new GenerateFunctionArgs(this, chunk));
                chunk.hasGenerated = true;
            }

            return chunk;
        }

        public WorldChunk GenerateChunk(Vector2Int chunkIndex, bool allowLoading = true, bool force = false)
        {

            WorldChunk targetChunk = null;
            bool shouldGenerate;

            if (chunks.ContainsKey(chunkIndex))
            {
                targetChunk = chunks[chunkIndex];
                shouldGenerate = !targetChunk.hasGenerated || force;
            }
            else
            {
                if(allowLoading)
                {
                    targetChunk = LoadChunk(chunkIndex);
                    shouldGenerate = !targetChunk.hasGenerated || force;
                }
                else
                {
                    shouldGenerate = false;
                }
            }

            if(shouldGenerate)
            {
                GenerateChunk(targetChunk);
            }

            return targetChunk;
        }

        public IVoxel GetVoxel(int x, int y, int z)
        {
            int nx, nz;
            if (Math.Sign(x) < 0)
            {
                nx = Mathf.CeilToInt((float)Mathf.Abs(x) / chunkPrefab.size.x) * Math.Sign(x);
            }
            else
            {
                nx = Mathf.FloorToInt((float)Mathf.Abs(x) / chunkPrefab.size.x) * Math.Sign(x);
            }
            
            if (Math.Sign(z) < 0)
            {
                nz = Mathf.CeilToInt((float)Mathf.Abs(z) / chunkPrefab.size.z) * Math.Sign(z);
            }
            else
            {
                nz = Mathf.FloorToInt((float)Mathf.Abs(z) / chunkPrefab.size.z) * Math.Sign(z);
            }

            Vector2Int chunkIndex = new Vector2Int(nx, nz);

            if(chunks.ContainsKey(chunkIndex))
            {
                WorldChunk chunk = chunks[chunkIndex];

                Vector2Int offset = chunkIndex * new Vector2Int(chunkPrefab.size.x, chunkPrefab.size.z);

                Vector3Int pos = new Vector3Int(x - offset.x, y, z - offset.y);

                return chunk.GetVoxel(pos);
            }

            return null;
        }

        public WorldChunk GetChunk(int x, int y)
        {
            return GetChunk(new Vector2Int(x, y));
        }

        // Start is called before the first frame update
        void Start()
        {
            world = this;

            dataPath = $"{Application.persistentDataPath}/{dataPath}";

            foreach(char c in worldName)
            {
                if(char.IsLetterOrDigit(c))
                {
                    worldFolder += c;
                }
                else
                {
                    worldFolder += '_';
                }
            }
        }

        void OnDestroy()
        {
            world = null;
        }

        void OnApplicationQuit()
        {
            world = null;
        }

        ~GameWorld()
        {
            if (world == this)
            {
                world = null;
            }
        }

        bool init = true;
        // Update is called once per frame
        void FixedUpdate()
        {
            if(init)
            {
                init = false;


                chunks = new Dictionary<Vector2Int, WorldChunk>();

                for (int x = -5; x < 6; x++)
                {
                    for (int z = -5; z < 6; z++)
                    {
                        GenerateChunk(new Vector2Int(x, z), true, false);
                    }
                }
            }

            currentCycleTime += Time.fixedDeltaTime;

            float cycleTimeNormalized = currentCycleTime / dayCycleLength;

            sunLight.transform.localRotation = Quaternion.Euler(sunRotationDirection * 360f * cycleTimeNormalized);
            sunLight.transform.localPosition = sunLight.transform.localRotation * Vector3.back * sunCenterDistance;
            sunLight.intensity = Mathf.Clamp01(Vector3.Dot(sunLight.transform.forward, Vector3.down));

            moonLight.transform.localRotation = Quaternion.Euler(moonRotationDirection * 360f * (cycleTimeNormalized + moonCycleOffset));
            moonLight.transform.localPosition = moonLight.transform.localRotation * Vector3.back * moonCenterDistance;
            moonLight.intensity = Mathf.Clamp01(Vector3.Dot(moonLight.transform.forward, Vector3.down));
        }
    }
}
