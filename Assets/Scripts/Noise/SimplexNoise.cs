using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactoryZero.Noise
{
    public class SimplexNoise : MonoBehaviour
    {        
        public enum DomainWarpType
        {
            None = -1,
            DomainWarpProgressive = FastNoiseLite.FractalType.DomainWarpProgressive,
            DomainWarpIndependent = FastNoiseLite.FractalType.DomainWarpIndependent
        }
        
        public enum FractalType
        {
            None = FastNoiseLite.FractalType.None,
            FBm = FastNoiseLite.FractalType.FBm,
            Ridged = FastNoiseLite.FractalType.Ridged,
            PingPong = FastNoiseLite.FractalType.PingPong
            
        }

        FastNoiseLite noise;
        FastNoiseLite domainWarp;

        [Header("General")]
        public FastNoiseLite.NoiseType noiseType;
        public FastNoiseLite.RotationType3D rotationType3D;
        public int seed = 1337;
        public float frequency;

        [Header("Fractal")]
        public FractalType fractalType;
        public int octaves;
        public float lacunarity;
        public float gain;
        public float weightedStrength;
        public float pingPongStrength;

        [Header("Cellular")]
        public FastNoiseLite.CellularDistanceFunction distanceFunction;
        public FastNoiseLite.CellularReturnType returnType;
        public float jitter;

        [Header("Domain Warp")]
        public bool enableDomainWarp;
        public FastNoiseLite.DomainWarpType domainWarpType;
        public FastNoiseLite.RotationType3D domainWarpRotationType3D;
        public float domainWarpAmplitude;
        public float domainWarpFrequency;

        [Header("Domain Warp Fractal")]
        public DomainWarpType domainWarpFractalType;
        public int domainWarpOctaves;
        public float domainWarpLacunarity;
        public float domainWarpGain;

        [Header("Debug")]
        public string lastGenerationTime;

        void InitNoise()
        {
            // General
            noise.SetNoiseType(noiseType);
            noise.SetFrequency(frequency);
            noise.SetRotationType3D(rotationType3D);

            // Fractal
            noise.SetFractalType((FastNoiseLite.FractalType)fractalType);
            noise.SetFractalOctaves(octaves);
            noise.SetFractalLacunarity(lacunarity);
            noise.SetFractalGain(gain);
            noise.SetFractalWeightedStrength(weightedStrength);
            noise.SetFractalPingPongStrength(pingPongStrength);

            // Cellular
            noise.SetCellularDistanceFunction(distanceFunction);
            noise.SetCellularReturnType(returnType);
            noise.SetCellularJitter(jitter);
        }

        void InitDomainWarp()
        {
            // General
            domainWarp.SetNoiseType(noiseType);
            domainWarp.SetFrequency(domainWarpFrequency);
            domainWarp.SetRotationType3D(domainWarpRotationType3D);

            // Domain Warp
            domainWarp.SetDomainWarpType(domainWarpType);
            domainWarp.SetDomainWarpAmp(domainWarpAmplitude);

            // Domain Warp Fractal
            domainWarp.SetFractalType((FastNoiseLite.FractalType)domainWarpFractalType);
            domainWarp.SetFractalOctaves(domainWarpOctaves);
            domainWarp.SetFractalLacunarity(domainWarpLacunarity);
            domainWarp.SetFractalGain(domainWarpGain);
        }

        public void OnGenerateNoise(NoiseFunctionArgs args)
        {
            DateTime startTime = DateTime.UtcNow;

            if(noise == null)
            {
                noise = new FastNoiseLite(args.Seed);
                InitNoise();
            }
            else
            {
                if(seed != args.Seed)
                {
                    noise.SetSeed(args.Seed);
                }
            }

            if (domainWarp == null)
            {
                noise = new FastNoiseLite(args.Seed);
                InitDomainWarp();
            }
            else
            {
                if (seed != args.Seed)
                {
                    domainWarp.SetSeed(args.Seed);
                }
            }

            seed = args.Seed;

            float px, py, pz;
            px = args.Position.x;
            py = args.Position.y;
            pz = args.Position.z;

            switch (args.PositionType)
            {
                case NoiseFunctionArgs.SamplerType.Is2D:

                    if(enableDomainWarp)
                    {
                        domainWarp.DomainWarp(ref px, ref py);
                    }

                    args.Value = noise.GetNoise(px, py);

                    break;

                default:
                case NoiseFunctionArgs.SamplerType.Is3D:

                    if (enableDomainWarp)
                    {
                        domainWarp.DomainWarp(ref px, ref py, ref pz);
                    }

                    args.Value = noise.GetNoise(px, py, pz);

                    break;
            }

            DateTime endTime = DateTime.UtcNow;
            TimeSpan timeTook = endTime - startTime;

            lastGenerationTime = $"{timeTook.TotalSeconds:F3}s / {timeTook.TotalMilliseconds:F3}ms / {(timeTook.TotalSeconds / Application.targetFrameRate):F3} frames";
        }
    }
}