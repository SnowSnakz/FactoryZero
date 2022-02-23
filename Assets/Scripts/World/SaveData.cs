using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactoryZero.Worlds
{
    public class SaveData : MonoBehaviour
    {
        public class JSON
        {
            public string name;
            public string displayName;

            public int gameVersion;
            public int formatVersion;

            public string[] biomeOrder;
            public string[] materialOrder;
            public string[] itemOrder;
        }
        
        public string saveName;
        public string spawnDimension;
    }
}