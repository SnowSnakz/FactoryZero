using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactoryZero.Noise
{
    public class NoiseFunctionArgs
    {
        public enum SamplerType
        {
            Is2D, Is3D
        }

        int seed;
        public int Seed { get => seed; }

        Vector3 pos;
        public Vector3 Position { get => pos; }

        SamplerType dimensions;
        public SamplerType PositionType { get => dimensions; }

        float returnValue;
        public float Value { get => returnValue; set => returnValue = value; }

        public NoiseFunctionArgs(int seed, Vector3 pos, SamplerType posType)
        {
            this.seed = seed;
            this.pos = pos;
            dimensions = posType;
        }
    }
}
