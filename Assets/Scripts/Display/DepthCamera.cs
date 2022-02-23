using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FactoryZero.Display
{
    [ExecuteAlways]
    class DepthCamera : MonoBehaviour
    {
        private void Start()
        {
            Camera cam = GetComponent<Camera>();
            cam.depthTextureMode = cam.depthTextureMode | DepthTextureMode.Depth;
        }
    }

}