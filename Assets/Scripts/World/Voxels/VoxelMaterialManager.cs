using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactoryZero.Voxels
{
    public class VoxelMaterialManager : MonoBehaviour
    {
        VoxelMaterial[] materialsArray;
        List<VoxelMaterial> materials = new List<VoxelMaterial>();

        public VoxelMaterial GetMaterialByIndex(int index)
        {
            return materials[index];
        }

        public void Rearrange(string[] newOrder)
        {
            VoxelMaterial[] newArray = new VoxelMaterial[Math.Max(newOrder.Length, materials.Count)];

            int count = 1;
            for(int i = 0; i < materials.Count; i++)
            {
                int nextPos = newOrder.Length - (count++);
                string mn = materials[i].name;

                for(int j = 0; j < newOrder.Length; j++)
                {
                    if(newOrder[j] == mn)
                    {
                        nextPos = j;
                    }
                }

                newArray[nextPos] = materials[i];
            }

            materialsArray = newArray;

            materials.Clear();
            materials.AddRange(materialsArray);
        }

        public string[] MaterialOrder
        {
            get
            {
                string[] ids = new string[materials.Count];

                for (int i = 0; i < ids.Length; i++)
                {
                    ids[i] = materials[i].name;
                }

                return ids;
            }
        }

        public VoxelMaterial[] Materials
        {
            get
            {
                if (materialsArray == null)
                {
                    materialsArray = materials.ToArray();
                }
                else
                {
                    if (materialsArray.Length != materials.Count)
                    {
                        materialsArray = materials.ToArray();
                    }
                }

                return materialsArray;
            }
        }

        void Start()
        {
            materialsArray = FindObjectsOfType<VoxelMaterial>();
            materials.AddRange(materialsArray);
        }
    }
}
