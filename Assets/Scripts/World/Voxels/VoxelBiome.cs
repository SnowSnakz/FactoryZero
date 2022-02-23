using FactoryZero.Noise;
using FactoryZero.Worlds.Decoration;
using UnityEngine;

namespace FactoryZero.Voxels
{
    public class VoxelBiome : MonoBehaviour
    {
        public string internalName;

        [Header("Mapping")]
        public bool isMapped = true;
        public Color32 mappedColor;

        [Header("Generation")]
        public NoiseImpl noise;

        [Header("Atmosphere")]
        public float scatterDensity;
        public float scatterHeight;
        public float scatterPowerOverHeight;
        public Gradient scatterColorOverDensity;

        [Header("Decoration")]
        public DecoTree[] treePrefabs;
    }
}
