using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryZero.Worlds
{
    public class GenerateFunctionArgs
    {
        GameWorld world;
        WorldChunk chunk;

        public GameWorld World { get => world; }
        public WorldChunk Chunk { get => chunk; }

        public GenerateFunctionArgs(GameWorld world, WorldChunk chunk)
        {
            this.world = world;
            this.chunk = chunk;
        }
    }
}
