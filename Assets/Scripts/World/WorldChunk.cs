using FactoryZero.Interfaces;
using FactoryZero.Marching;
using FactoryZero.Voxels;
using System.IO;
using UnityEngine;

namespace FactoryZero.Worlds
{
    public class WorldChunk : MonoBehaviour, ISaveFile
    {
        public GameWorld world;
        public MarchingImpl marchingImpl;

        public Material material;

        public bool hasGenerated;
        public bool inUse;

        public Vector3Int voxelBitSize;
        public Vector3Int size;

        public Vector2Int index;

        IGrid3D<IVoxel>[] grids;

        Vector3Int gridNum;

        bool hasInitialized;
        void InitAll()
        {
            if(!hasInitialized)
            {
                hasInitialized = true;

                if (inUse)
                {
                    gridNum = new Vector3Int(size.x / voxelBitSize.x, size.y / voxelBitSize.y, size.z / voxelBitSize.z);

                    int gridCount = gridNum.x * gridNum.y * gridNum.z;
                    grids = new IGrid3D<IVoxel>[gridCount];

                    for (int x = 0; x < gridNum.x; x++)
                    {
                        for (int z = 0; z < gridNum.z; z++)
                        {
                            for (int y = 0; y < gridNum.y; y++)
                            {
                                Vector3Int subGridIndex = new Vector3Int(x, y, z);
                                int subGridIndex1d = subGridIndex.x + subGridIndex.y * gridNum.x + subGridIndex.z * gridNum.x * gridNum.y;

                                GameObject go = new GameObject();
                                go.name = $"{name}_Bit_{subGridIndex.x}x_{subGridIndex.y}y_{subGridIndex.z}z";
                                go.transform.parent = transform;
                                go.transform.localPosition = new Vector3(subGridIndex.x * voxelBitSize.x, subGridIndex.y * voxelBitSize.y, subGridIndex.z * voxelBitSize.z);

                                VoxelGrid vg = go.AddComponent<VoxelGrid>();
                                vg.chunk = this;
                                vg.bitIndex = subGridIndex;
                                vg.size = voxelBitSize;
                                vg.material = material;

                                vg.InitAll();

                                grids[subGridIndex1d] = vg;
                            }
                        }
                    }
                }
            }
        }

        public IVoxel GetVoxel(Vector3Int pos)
        {
            InitAll();

            Vector3Int subGridIndex = new Vector3Int(pos.x / voxelBitSize.x, pos.y / voxelBitSize.y, pos.z / voxelBitSize.z);
            int subGridIndex1d = subGridIndex.x + subGridIndex.y * gridNum.x + subGridIndex.z * gridNum.x * gridNum.y;

            Vector3Int sub = subGridIndex * voxelBitSize;

            if(pos.x < 0 || pos.y < 0 || pos.z < 0)
            {
                return null;
            }

            if(pos.x >= size.x || pos.y >= size.y || pos.z >= size.z)
            {
                return null;
            }

            IGrid3D<IVoxel> voxelGrid = grids[subGridIndex1d];
            IVoxel voxel = voxelGrid[pos.x - sub.x, pos.y - sub.y, pos.z - sub.z];

            return voxel;
        }

        public void Read(BinaryReader reader)
        {
            InitAll();

           // hasGenerated = reader.ReadBoolean();
        }

        public void Write(BinaryWriter writer)
        {
            
        }

        // Start is called before the first frame update
        void Start()
        {
            InitAll();
        }

        // Update is called once per frame
        void Update()
        {
            if(inUse)
            {

            }
        }
    }
}
