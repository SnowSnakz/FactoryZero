using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactoryZero.Marching
{
    public struct MarchingMeshVertex
    {
        public static bool operator==(MarchingMeshVertex a, MarchingMeshVertex b)
        {
            return a.position == b.position;
        }

        public static bool operator!=(MarchingMeshVertex a, MarchingMeshVertex b)
        {
            return a.position != b.position;
        }

        public override bool Equals(object obj)
        {
            Vector3 otherPosition = Vector3.zero;
            if(obj is MarchingMeshVertex mv)
            {
                otherPosition = mv.position;
            }
            
            if(obj is Vector3 p)
            {
                otherPosition = p; 
            }

            return position.Equals(otherPosition);
        }

        public override int GetHashCode()
        {
            return 1206833562 + position.GetHashCode();
        }

        public Vector3 position;

        public Color grassColor;
        public Color color;

        public bool hasGrass;

        public float biomeColorStrengthGrass;
        public float biomeColorStrength;

        public float grassAmount;

        public Vector2 biomeParameters;

        public Vector4 UV0 { get => new Vector4(grassColor.r, grassColor.g, grassColor.b, grassAmount); }
        public Vector4 UV1 { get => new Vector4(color.r, color.g, color.b, hasGrass ? 1f : 0f); }
        public Vector4 UV2 { get => new Vector4(biomeParameters.x, biomeParameters.y, biomeColorStrengthGrass, biomeColorStrength); }
    }
}
