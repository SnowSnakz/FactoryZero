using FactoryZero.Voxels;
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
        }
    }
}