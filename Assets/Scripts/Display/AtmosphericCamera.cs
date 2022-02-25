﻿using FactoryZero.Voxels;
using FactoryZero.Worlds;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactoryZero.Display
{
    public class AtmosphericCamera : MonoBehaviour
    {
        public int resolutionX;
        public int resolutionY;

        public float distance;

        public int sampleCount;
        public float sampleInterval;

        public Material overlayMaterial;
        public MeshRenderer overlayRenderer;

        public Texture2D texture;
        Camera mcamera;

        public GameWorld world;
        Mesh mesh;

        // Start is called before the first frame update
        void Start()
        {

            mcamera = GetComponent<Camera>();
            texture = new Texture2D(resolutionX, resolutionY);

            mcamera.depthTextureMode = mcamera.depthTextureMode | DepthTextureMode.Depth;

            overlayRenderer.material = overlayMaterial;
            overlayMaterial.mainTexture = texture;

            Vector3[] frustumCorners = new Vector3[4];
            mcamera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), distance + 1f, Camera.MonoOrStereoscopicEye.Mono, frustumCorners);

            mesh = new Mesh();

            List<Vector3> verts = new List<Vector3>();
            List<int> inds = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            verts.Add(frustumCorners[0]);
            verts.Add(frustumCorners[1]);
            verts.Add(frustumCorners[2]);
            verts.Add(frustumCorners[3]);

            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(1, 0));

            inds.Add(0);
            inds.Add(1);
            inds.Add(2);

            inds.Add(3);
            inds.Add(0);
            inds.Add(2);

            mesh.SetVertices(verts);
            mesh.SetUVs(0, uvs);
            mesh.SetIndices(inds, MeshTopology.Triangles, 0);

            mesh.Optimize();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            overlayRenderer.transform.localPosition = new Vector3(0, 0, -0.1f);
            overlayRenderer.GetComponent<MeshFilter>().mesh = mesh;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            Light[] lights = FindObjectsOfType<Light>();

            WorldChunk currentChunk = null;

            for (int x = 0; x < resolutionX; x++)
            {
                for (int y = 0; y < resolutionY; y++)
                {
                    Vector3 direction = transform.rotation * new Vector3(((x - (resolutionX / 2f)) / resolutionX) * (mcamera.fieldOfView / 180) * (mcamera.pixelWidth / distance), (y - (resolutionY / 2f)) / resolutionY * (mcamera.fieldOfView / 180) * (mcamera.pixelHeight / distance), distance + sampleCount * sampleInterval).normalized;

                    Color lightColor = Color.white;
                    foreach(Light light in lights)
                    {
                        switch(light.type)
                        {
                            case LightType.Directional:
                                lightColor -= Mathf.Max(0, Vector3.Dot(direction, light.transform.forward) * light.intensity) * light.color;
                                break;
                        } 
                    }


                    Dictionary<VoxelBiome, float> scatters = new Dictionary<VoxelBiome, float>();
                    for (int s = sampleCount - 1; s >= 0; s--)
                    {
                        VoxelBiome biome = world.biomeManager.defaultBiome;
                        Vector3 samplePosition = direction * (distance + s * sampleInterval);
                        Vector2Int sampleChunkIndex = new Vector2Int(Mathf.FloorToInt(samplePosition.x / world.chunkPrefab.size.x), Mathf.FloorToInt(samplePosition.z / world.chunkPrefab.size.z));

                        bool changedChunks = false;
                        if(currentChunk == null)
                        {
                            changedChunks = true;
                        }
                        else
                        {
                            if (currentChunk.index != sampleChunkIndex)
                            {
                                changedChunks = true;
                            }
                        }

                        if(changedChunks)
                        {
                            currentChunk = world.GetChunk(sampleChunkIndex);
                            if(currentChunk == null)
                            {
                                if (!scatters.ContainsKey(biome))
                                {
                                    scatters.Add(biome, 0f);
                                }

                                float scatterIntensity = biome.scatterDensity * (Mathf.Pow(Mathf.Clamp01((biome.scatterHeight - samplePosition.y) / biome.scatterHeight), biome.scatterPowerOverHeight));
                                lightColor -= biome.scatterColorOverDensity.Evaluate(scatterIntensity);
                                continue;
                            }
                        }

                        foreach (Light light in lights)
                        {
                            switch (light.type)
                            {
                                case LightType.Point:
                                    lightColor += (Mathf.Max(0, 1 - Vector3.Distance(samplePosition, light.transform.position) / light.range) * light.color) * (s / (sampleCount * sampleInterval)) * light.intensity;
                                    break;
                            }
                        }


                        Vector3Int voxelIndex = new Vector3Int(Mathf.FloorToInt(samplePosition.x - currentChunk.index.x * currentChunk.size.x), Mathf.FloorToInt(samplePosition.y), Mathf.FloorToInt(samplePosition.z - currentChunk.index.y * currentChunk.size.z));
                        voxelIndex = Vector3Int.Min(Vector3Int.Max(Vector3Int.zero, voxelIndex), currentChunk.size - Vector3Int.one);
                        
                        IVoxel voxel = currentChunk.GetVoxel(voxelIndex);

                        if(voxel != null)
                        {
                            biome = voxel.Biome;

                            if (biome != null)
                            {
                                if (voxel.Volume >= 0.5f)
                                {
                                    lightColor = Color.white;
                                    scatters[voxel.Biome] = 0f;
                                    continue;
                                }
                                else
                                {
                                    if (!scatters.ContainsKey(biome))
                                    {
                                        scatters.Add(biome, 0f);
                                    }

                                    float scatterIntensity = biome.scatterDensity * (Mathf.Pow(Mathf.Clamp01((biome.scatterHeight - samplePosition.y) / biome.scatterHeight), biome.scatterPowerOverHeight));
                                    lightColor *= biome.scatterColorOverDensity.Evaluate(scatterIntensity);
                                }
                            }
                        }
                    }

                    texture.SetPixel(x, y, lightColor);
                }
            }

            texture.Apply();
        }
    }
}