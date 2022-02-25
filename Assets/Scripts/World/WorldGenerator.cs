using System.Collections;
using UnityEngine;
using FactoryZero.Noise;
using FactoryZero.Voxels;
using UnityEngine.Events;
using System;

namespace FactoryZero.Worlds
{
    public class WorldGenerator : MonoBehaviour
    {
        [Serializable]
        public class GenerateFunction : SerializableFunction<GenerateFunctionArgs, GenerateFunction.Event>
        {
            [Serializable]
            public class Event : UnityEvent<GenerateFunctionArgs>
            {
            }
        }

        public NoiseImpl noise;

        public VoxelBiomeManager biomes;
        public VoxelMaterialManager materials;

        public Texture2D heightMap;

        public GenerateFunction onGenerate;
    }
}