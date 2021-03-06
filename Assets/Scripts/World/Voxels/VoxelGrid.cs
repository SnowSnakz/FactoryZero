using FactoryZero.Interfaces;
using FactoryZero.Marching;
using FactoryZero.Worlds;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FactoryZero.Voxels
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    public class VoxelGrid : MonoBehaviour, IGrid3D<IVoxel>
    {
        public Vector3Int size;
        public Vector3Int bitIndex;
        public bool isReadOnly;

        public Material material;

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
            InitAll();

            if (x < 0 || y < 0 || z < 0 || x >= size.x || y >= size.y || z >= size.z)
            {
                throw new IndexOutOfRangeException($"Index [{x}, {y}, {z}] does not fall within the expected range of [0..{size.x - 1}, 0..{size.y - 1}, 0..{size.z - 1}]");
            }

            int index1d = GetIndex(x, y, z);
            if (voxels.Length <= index1d)
            {
                throw new IndexOutOfRangeException($"Index [{x}, {y}, {z}] appears to be in the expected range [0..{size.x - 1}, 0..{size.y - 1}, 0..{size.z - 1}], but internally the translated index [{index1d}] is out of bounds [0..{voxels.Length - 1}]");
            }

            return index1d;
        }

        public IVoxel this[int x, int y, int z]
        {
            get
            {
                InitAll();

                int index1d = TranslateIndex(x, y, z);
                return voxels[index1d];
            }
            set
            {
                InitAll();

                hasChanged = true;

                int index1d = TranslateIndex(x, y, z);
                voxels[index1d] = value;
            }
        }

        public int Width => size.x;
        public int Height => size.y;
        public int Length => size.z;

        private bool readOnly;
        public bool IsReadOnly => readOnly;

        public bool exportMesh;

        Action changeAction;

        Mesh mesh;

        MeshFilter mFilter;
        MeshRenderer mRenderer;
        MeshCollider mCollider;

        private void OnDrawGizmosSelected()
        {
            for(int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    for (int z = 0; z < size.z; z++)
                    {
                        Vector3 pos = new Vector3(x, y, z) + (Vector3.one * 0.5f);
                        if(this[x, y, z].Volume > 0.5f)
                        {
                            Gizmos.DrawCube(transform.position + pos, Vector3.one);
                        }
                    }
                }
            }
        }

        bool hasInit = false;
        public void InitAll()
        {
            if(!hasInit)
            {
                hasInit = true;

                voxels = new IVoxel[size.x * size.y * size.z];

                for (int x = 0; x < size.x; x++)
                {
                    for (int y = 0; y < size.y; y++)
                    {
                        for (int z = 0; z < size.z; z++)
                        {
                            this[x, y, z] = new SimpleVoxel(chunk, changeAction: changeAction);
                        }
                    }
                }
            }
        }

        void Start()
        {
            mFilter = GetComponent<MeshFilter>();
            mRenderer = GetComponent<MeshRenderer>();
            mCollider = GetComponent<MeshCollider>();

            regenMesh = RegenMesh;
            updateMesh = UpdateMesh;

            changeAction = () =>
            {
                hasChanged = true;
            };

            hasChanged = true;

            InitAll();

            mesh = new Mesh();
            mesh.name = $"Chunk_{chunk.index.x}x_{chunk.index.y}z__ChunkBit_{bitIndex.x}x_{bitIndex.y}y_{bitIndex.z}z_VoxelMesh";

            mesh.MarkDynamic();

            mFilter.mesh = mesh;

            readOnly = isReadOnly;
        }

        float lastUpdateTime;

        Action regenMesh;
        Action updateMesh;

        bool isMarching;
        MarchingFunctionArgs marching;

        class MarchingGrid : IGrid3D<IVoxel>
        {
            Vector3Int size;

            IVoxel[] voxels;

            public enum NeighborChunk
            {
                main = 0,
                forwardX = 1, 
                forwardZ = 2
            }

            public MarchingGrid(WorldChunk[] neighbors, Vector3Int offset, Vector3Int size)
            {
                this.size = size;

                voxels = new IVoxel[size.x * size.y * size.z];

                for(int x = 0; x < size.x; x++)
                {
                    for (int y = 0; y < size.y; y++)
                    {
                        for (int z = 0; z < size.z; z++)
                        {
                            NeighborChunk neighborIndex = NeighborChunk.main;


                            Vector3Int worldPosition = new Vector3Int(x, offset.y + y, z);

                            if (x >= neighbors[0].size.x)
                            {
                                neighborIndex |= NeighborChunk.forwardX;
                                worldPosition.x -= neighbors[0].size.x;

                            }
                            if (z >= neighbors[0].size.z)
                            {
                                neighborIndex |= NeighborChunk.forwardZ;
                                worldPosition.z -= neighbors[0].size.z;
                            }

                            WorldChunk nc = neighbors[(int)neighborIndex];

                            IVoxel v;
                            if (nc != null)
                            {
                                v = nc.GetVoxel(worldPosition);
                            }
                            else
                            {
                                v = neighbors[0].GetVoxel(Vector3Int.Min(Vector3Int.Max(Vector3Int.zero, new Vector3Int(x, y + offset.y, z)), neighbors[0].size - Vector3Int.one));
                            }


                            voxels[x + y * size.x + z * size.x * size.y] = v;
                        }
                    }
                }
            }

            public IVoxel this[int x, int y, int z] 
            {
                get
                {
                    IVoxel v = voxels[x + y * size.x + z * size.x * size.y];
                    return v;
                }
                set {}
            }

            public int Width => size.x;

            public int Height => size.y;

            public int Length => size.z;

            public bool IsReadOnly => true;
        }

        void RegenMesh()
        {
            WorldChunk[] chunks = new WorldChunk[4];

            chunks[0] = chunk;
            chunks[1] = chunk.world.GetChunk(chunk.index + new Vector2Int(1, 0));
            chunks[2] = chunk.world.GetChunk(chunk.index + new Vector2Int(0, 1));
            chunks[3] = chunk.world.GetChunk(chunk.index + new Vector2Int(1, 1));

            marching = new MarchingFunctionArgs(new MarchingGrid(chunks, new Vector3Int(chunk.index.x * chunk.size.x + bitIndex.x * chunk.voxelBitSize.x, bitIndex.y * chunk.voxelBitSize.y, chunk.index.y * chunk.size.z + bitIndex.z * chunk.voxelBitSize.z), size + Vector3Int.one), 0.5f);
            chunk.marchingImpl.onMarch.Invoke(marching);
            updateMesh.ExecuteOnMainThread();
        }

        void UpdateMesh()
        {

            List<MarchingMeshVertex[]> tris = marching.Triangles;

            List<Vector3> verts = new List<Vector3>();
            List<List<int>> inds = new List<List<int>>();

            List<Vector4> uv0 = new List<Vector4>();
            List<Vector4> uv1 = new List<Vector4>();
            List<Vector4> uv2 = new List<Vector4>();

            inds.Add(new List<int>());

            //int tc = 0;
            int smi = 0;
            foreach(MarchingMeshVertex[] tri in tris)
            {
                foreach(MarchingMeshVertex vert in tri)
                {
                    int vi = verts.IndexOf(vert.position);

                    if(vi != -1)
                    {
                        inds[smi].Add(vi);
                    }
                    else
                    {
                        inds[smi].Add(verts.Count);
                        verts.Add(vert.position);
                        uv0.Add(vert.UV0);
                        uv1.Add(vert.UV1);
                        uv2.Add(vert.UV2);
                    }
                }
            }

            if(tris.Count > 0)
            {
                // Debug.Log($"chunkBit{bitIndex.x}x{bitIndex.y}y{bitIndex.z}z.triangleCount = {tris.Count}");
            }

            mesh.SetVertices(verts);
            mesh.SetUVs(0, uv0);
            mesh.SetUVs(1, uv1);
            mesh.SetUVs(2, uv2);

            int i;

            Material[] mats = new Material[smi+1];
            for(i = 0; i < smi+1; i++)
            {
                mats[i] = material;
            }

            i = 0;
            foreach (List<int> sinds in inds)
            {
                mesh.SetIndices(sinds, MeshTopology.Triangles, i++);
            }

            mesh.RecalculateNormals();

            mesh.bounds = new Bounds(new Vector3(size.x * 0.5f, size.y * 0.5f, size.z * 0.5f), new Vector3(size.x, size.y, size.z));

            mRenderer.materials = mats;
            mFilter.mesh = mesh;
            mCollider.sharedMesh = mesh;

            isMarching = false;
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
                        hasChanged = false;
                        isMarching = true;
                        lastUpdateTime = 0.25f;
                        regenMesh.ExecuteAsync();
                    }
                }
            }
        }
    }
}