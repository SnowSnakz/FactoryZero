using FactoryZero.Interfaces;
using FactoryZero.Worlds;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FactoryZero.Voxels
{
    public class SimpleVoxel : IVoxel, IPotentiallyReadOnly
    {
        const string readOnlyException = "Trying to modify a read-only voxel.";

        WorldChunk chunk;

        bool hasGrass;
        VoxelMaterial material;
        VoxelBiome biome;
        float volume;

        bool readOnly;
        public bool IsReadOnly { get => readOnly; }

        public bool IsGrassy { 
            get => hasGrass;
            set 
            {
                if (readOnly)
                {
                    throw new AccessViolationException(readOnlyException);
                }

                if(hasGrass != value)
                {
                    changeAction?.Invoke();
                    hasGrass = value;
                }
            }
        }
        public VoxelMaterial Material 
        {
            get => material;
            set 
            {
                if (readOnly)
                {
                    throw new AccessViolationException(readOnlyException);
                }

                if(material != value)
                {
                    changeAction?.Invoke();
                    material = value;
                }
            }
        }

        public VoxelBiome Biome 
        { 
            get => biome; 
            set
            {
                if (readOnly)
                {
                    throw new AccessViolationException(readOnlyException);
                }

                if(biome != value)
                {
                    changeAction?.Invoke();
                    biome = value;
                }
            }
        }
        public float Volume 
        { 
            get => volume;
            set
            {
                if (readOnly)
                {
                    throw new AccessViolationException(readOnlyException);
                }

                if(volume != value)
                {
                    changeAction?.Invoke();
                    volume = value;
                }
            }
        }

        Vector2 biomeParams;
        float grassAmount;

        public Vector2 BiomeParameters 
        { 
            get => biomeParams; 
            set 
            {
                if (readOnly)
                {
                    throw new AccessViolationException(readOnlyException);
                }

                if (biomeParams != value)
                {
                    changeAction?.Invoke();
                    biomeParams = value;
                }
            }
        }

        public float GrassAmount
        {
            get => grassAmount;
            set
            {
                if (readOnly)
                {
                    throw new AccessViolationException(readOnlyException);
                }

                if (grassAmount != value)
                {
                    changeAction?.Invoke();
                    grassAmount = value;
                }
            }
        }

        Action changeAction;

        public SimpleVoxel(WorldChunk chunk, Action changeAction = null, float volume = 0f, bool hasGrass = false, int material = 0, int biome = 0, bool readOnly = false) 
            : this(chunk, volume, hasGrass, chunk.world.materialManager.GetMaterialByIndex(material), chunk.world.biomeManager.GetBiomeByIndex(biome), readOnly)
        {
        }

        public SimpleVoxel(WorldChunk chunk, float volume, bool hasGrass, VoxelMaterial material, VoxelBiome biome, bool readOnly = false) 
        {
            this.chunk = chunk;
            this.volume = volume;
            this.readOnly = readOnly;
            this.material = material;
            this.hasGrass = hasGrass;
            this.biome = biome;
        }

        public void Freeze()
        {
            if (readOnly)
            {
                throw new AccessViolationException(readOnlyException);
            }

            readOnly = true;
        }

        public void Read(BinaryReader reader)
        {
            if(readOnly)
            {
                throw new AccessViolationException(readOnlyException);
            }

            byte v = reader.ReadByte();

            hasGrass = ((byte)(v << 7) & 1) != 0;
            volume = ((byte)(v >> 1) << 1) * 0.00787401574f; // v[exclude 8th bit] / 127;

            int biomeIndex = reader.ReadInt32();
            biome = chunk.world.biomeManager.GetBiomeByIndex(biomeIndex);
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(material);
            writer.Write(biome);

            byte v = (byte)((Mathf.FloorToInt(Mathf.Clamp01(volume) * 127)) | (hasGrass ? 128 : 0));
            writer.Write(v);
        }

    }
}
