using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace FactoryZero.Marching
{
    public class MarchingImpl : MonoBehaviour
    {
        [Serializable]
        public class MarchFunction : SerializableFunction<MarchingFunctionArgs, MarchFunction.Event>
        {
            [Serializable]
            public class Event : UnityEvent<MarchingFunctionArgs>
            {
            }
        }

        public MarchFunction onMarch;

        private void Start()
        {
            onMarch.Init();
        }
    }
}
