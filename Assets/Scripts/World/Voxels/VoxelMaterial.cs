using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FactoryZero.Voxels
{
    public class VoxelMaterial : MonoBehaviour
    {
        [Serializable]
        public class VoxelMaterialTickEvent : UnityEvent<VoxelTickEventArgs>
        {
        }

        public string internalName;
        public string displayName;

        public float biomeColorStrength;
        public Color color;

        public bool allowGrass;
        public float grassColorBiomeStrength;
        public Color grassColor;

        public string toolType;
        public int toolLevel;

        public float resistance;
        public float blastResistance;

        public VoxelMaterialTickEvent onVoxelTick;
    }
}