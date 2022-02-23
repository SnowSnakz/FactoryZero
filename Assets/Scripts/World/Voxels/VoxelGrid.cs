using FactoryZero.Interfaces;
using FactoryZero.Marching;
using FactoryZero.Worlds;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FactoryZero.Voxels
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class VoxelGrid : MonoBehaviour, IGrid3D<IVoxel>
    {
        public Vector3Int size;
        public Vector3Int bitIndex;
        public bool isReadOnly;

        public WorldChunk chunk;

        IVoxel[] voxels;

        bool hasChanged;

        // Converts a 2D index to 1D index for `values` array
        int GetIndex(int x, int y, int z)
        {
            return x + y * size.x + z * size.x * size.y;
        }

        // A wrapper around GetIndex which performs checks and throws exceptions.
        int TranslateIndex(int x, int y, int z)
        {

            if (x < 0 || y < 0 || z < 0 || x >= size.x || y >= size.y || z >= size.z)
            {
                throw new IndexOutOfRangeException($"Index [{x}, {y}, {z}] does not fall within the expected range of [0..{size.x - 1}, 0..{size.y - 1}, 0..{size.z}]");
            }

            int index1d = GetIndex(x, y, z);
            if (voxels.Length <= index1d)
            {
                throw new IndexOutOfRangeException($"Index [{x}, {y}, {z}] appears to be in the expected range [0..{size.x - 1}, 0..{size.y - 1}, 0..{size.z}], but internally the translated index [{index1d}] is out of bounds [0..{voxels.Length - 1}]");
            }

            return index1d;
        }

        public IVoxel this[int x, int y, int z]
        {
            get
            {
                int index1d = TranslateIndex(x, y, z);
                return voxels[index1d];
            }
            set
            {
                int index1d = TranslateIndex(x, y, z);
                voxels[index1d] = value;
            }
        }

        public int Width => size.x;
        public int Height => size.y;
        public int Length => size.z;

        private bool readOnly;
        public bool IsReadOnly => readOnly;

        Action changeAction;

        Mesh mesh;

        MeshFilter mFilter;
        MeshRenderer mRenderer;

        void Start()
        {
            mFilter = GetComponent<MeshFilter>();
            mRenderer = GetComponent<MeshRenderer>();

            regenMesh = RegenMesh;
            updateMesh = UpdateMesh;

            changeAction = () =>
            {
                hasChanged = true;
            };

            voxels = new IVoxel[size.x * size.y * size.z];

            for(int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    for (int z = 0; z < size.z; z++)
                    {
                        this[x, y, z] = new SimpleVoxel(chunk, changeAction: changeAction);
                    }
                }
            }

            mesh = new Mesh();
            mesh.name = $"Chunk_{chunk.index.x}x_{chunk.index.y}z__ChunkBit_{bitIndex.x}x_{bitIndex.y}y_{bitIndex.z}z_VoxelMesh";
            mesh.bounds = new Bounds(new Vector3(size.x * 0.5f, size.y * 0.5f, size.z * 0.5f), new Vector3(size.x, size.y, size.z));

            readOnly = isReadOnly;
        }

        float lastUpdateTime;

        Action regenMesh;
        Action updateMesh;

        bool isMarching;
        MarchingFunctionArgs marching;

        void RegenMesh()
        {
            marching = new MarchingFunctionArgs(this, 0.5f);
            chunk.marchingImpl.onMarch.Invoke(marching);
            updateMesh.ExecuteOnMainThread();
        }

        void UpdateMesh()
        {
            List<MarchingMeshVertex> vert;
        }

        void FixedUpdate()
        {
            if (lastUpdateTime > 0)
            {
                lastUpdateTime -= Time.deltaTime;
            }
            else
            {
                if (!isMarching)
                {
                    if (hasChanged)
                    {
                        isMarching = true;
                        lastUpdateTime = 0.25f;
                    }
                }
            }
        }
    }
}