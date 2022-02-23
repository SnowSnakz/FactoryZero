using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace FactoryZero.Interfaces
{
    public interface ISaveFile
    {
        void Read(BinaryReader reader);
        void Write(BinaryWriter writer);
    }
}