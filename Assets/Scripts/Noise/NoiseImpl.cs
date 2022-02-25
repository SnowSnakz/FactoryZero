using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Object = UnityEngine.Object;

namespace FactoryZero.Noise
{
    public class NoiseImpl : MonoBehaviour
    {
        [Serializable]
        public class NoiseFunction : SerializableFunction<NoiseFunctionArgs, NoiseFunction.Event>
        {
            [Serializable]
            public class Event : UnityEvent<NoiseFunctionArgs>
            {
            }
        }

        public NoiseFunction onGenerateNoise;

        private void Start()
        {
            onGenerateNoise.Init();
        }
    }
}