using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FactoryZero.Noise
{
    public class NoiseImpl : MonoBehaviour
    {
        [Serializable]
        public class NoiseFunction : UnityEvent<NoiseFunctionArgs>
        {
        }

        public NoiseFunction onGenerateNoise;
    }
}