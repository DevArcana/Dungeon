using ProceduralNoiseProject;
using UnityEngine;
using World.Mesh;

namespace World
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    public class Cave : MonoBehaviour
    {
        public int size = 16;
        public int seed = 0;
        public float frequency = 1.0f;
        public float jitter = 1.0f;
        public int amplitude = 4;

        private void Randomize(int[,] heightmap)
        {
            var noise = new WorleyNoise(seed, frequency, jitter, amplitude);
            
            for (var z = 0; z < size; z++)
            {
                for (var x = 0; x < size; x++)
                {
                    heightmap[x, z] = (int) noise.Sample2D(x, z);
                }
            }
        }
        
        public void Generate()
        {
            var filter = GetComponent<MeshFilter>();
            var cave = new CaveBuilder();

            var heightmap = new int[size, size];
            Randomize(heightmap);

            for (var z = 0; z < size; z++)
            {
                for (var x = 0; x < size; x++)
                {
                    var y = heightmap[x, z];
                    cave.AddFloor(new Vector3(x, y, z), new Vector3(x + 1, y, z + 1));

                    if (x == 0)
                    {
                        cave.AddWall(new Vector3(x, 0.0f, z + 1), new Vector3(x, y, z));
                    }
                    else if (x == size - 1)
                    {
                        cave.AddWall(new Vector3(x + 1, 0.0f, z), new Vector3(x + 1, y, z + 1));
                    }

                    if (z == 0)
                    {
                        cave.AddWall(new Vector3(x, 0.0f, z), new Vector3(x + 1, y, z));
                    }
                    else if (z == size - 1)
                    {
                        cave.AddWall(new Vector3(x + 1, 0.0f, z + 1), new Vector3(x, y, z + 1));
                    }

                    if (x < size - 1)
                    {
                        var h = heightmap[x + 1, z];
                        if (y != h)
                        {
                            cave.AddWall(new Vector3(x + 1, y, z + 1), new Vector3(x + 1, h, z));
                        }
                    }

                    if (z < size - 1)
                    {
                        var h = heightmap[x, z + 1];
                        if (y != h)
                        {
                            cave.AddWall(new Vector3(x, y, z + 1), new Vector3(x + 1, h, z + 1));
                        }
                    }
                }
            }
            
            // cave.AddWall(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(1.0f, 1.0f, 1.0f));
            // cave.AddFloor(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(1.0f, 0.0f, 1.0f));
            
            filter.mesh = cave.Build();
        }
    }
}