using FactoryZero.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FactoryZero
{
    public static class Utility
    {
        public static void ExecuteOnMainThread(this Action action)
        {
            MainThreadActionExecutor.mainThreadActions.Enqueue(action);
        }
        

        public static void ExecuteAsync(this Action action)
        {
            MainThreadActionExecutor.alternateThreadActions.Enqueue(action);
        }

        public static void GenerateGridMesh(Quaternion direction, List<Vector3> vertices, List<int> indices, List<Vector2> uvs, Vector3 offset, int resolutionX, int resolutionY, float cellSizeX, float cellSizeY, bool clockwise = true)
        {
            for(int x = 0; x < resolutionX; x++)
            {
                for(int y = 0; y < resolutionY; y++)
                {
                    float nx, ny;
                    nx = cellSizeX * x;
                    ny = cellSizeY * y;

                    Vector3 p1, p2, p3, p4;
                    p1 = offset + new Vector3(nx, ny, 0);
                    p2 = offset + new Vector3(nx+cellSizeX, ny, 0);
                    p3 = offset + new Vector3(nx, ny+cellSizeY, 0);
                    p4 = offset + new Vector3(nx+cellSizeX, ny+cellSizeY, 0);

                    Vector3 rp1, rp2, rp3, rp4;
                    rp1 = direction * p1;
                    rp2 = direction * p2;
                    rp3 = direction * p3;
                    rp4 = direction * p4;

                    int id1, id2, id3, id4;
                    id1 = vertices.IndexOf(rp1);
                    id2 = vertices.IndexOf(rp2);
                    id3 = vertices.IndexOf(rp3);
                    id4 = vertices.IndexOf(rp4);

                    if(id1 == -1)
                    {
                        id1 = vertices.Count;
                        vertices.Add(rp1);
                        uvs.Add(new Vector2(p1.x / (resolutionX * cellSizeX), p1.y / (resolutionY * cellSizeY)));
                    }
                    if(id2 == -1)
                    {
                        id2 = vertices.Count;
                        vertices.Add(rp2);
                        uvs.Add(new Vector2(p2.x / (resolutionX * cellSizeX), p2.y / (resolutionY * cellSizeY)));
                    }
                    if(id3 == -1)
                    {
                        id3 = vertices.Count;
                        vertices.Add(rp3);
                        uvs.Add(new Vector2(p3.x / (resolutionX * cellSizeX), p3.y / (resolutionY * cellSizeY)));
                    }
                    if(id4 == -1)
                    {
                        id4 = vertices.Count;
                        vertices.Add(rp4);
                        uvs.Add(new Vector2(p4.x / (resolutionX * cellSizeX), p4.y / (resolutionY * cellSizeY)));
                    }

                    if(clockwise)
                    {
                        indices.Add(id1);
                        indices.Add(id2);
                        indices.Add(id3);
                        indices.Add(id2);
                        indices.Add(id4);
                        indices.Add(id3);
                    }
                    else
                    {
                        indices.Add(id3);
                        indices.Add(id2);
                        indices.Add(id1);
                        indices.Add(id3);
                        indices.Add(id4);
                        indices.Add(id2);
                    }
                }
            }
        }
    }
}