using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class VoxelLight : MonoBehaviour
{
    public enum LightType
    {
        Directional, Point, Spot
    }

    public LightType type;
    LightType prevType;

    List<GameObject> objects;

    void Start()
    {
        objects = new List<GameObject>();
        Init();
    }

    void Init()
    {
        if(objects.Count > 0)
        {
            Clear();
        }

        type = prevType;
        switch(type)
        {
            case LightType.Directional:
                GameObject no = new GameObject();
                no.name = name + "_ShadowCamera";

                Camera c = no.AddComponent<Camera>();
                RenderTexture target = Addressables.LoadAssetAsync<RenderTexture>("ShadowsDirectional0").WaitForCompletion();
                c.targetTexture = target;

                no.AddComponent<DirectionLightFollower>();

                objects.Add(no);

                break;

            case LightType.Point:

                break;
        }
    }

    void Clear()
    {
        foreach(GameObject obj in objects)
        {
            Destroy(obj);
        }

        objects.Clear();
    }

    void Update()
    {
        if(prevType != type)
        {
            Clear();
            Init();
        }
    }

}
