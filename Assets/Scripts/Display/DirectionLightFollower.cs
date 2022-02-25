using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionLightFollower : MonoBehaviour
{
    public VoxelLight parent;
    public float distance;

    public int index;
    public Camera shadowCamera;

    void Start()
    {
        shadowCamera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Quaternion targetRotation = parent.transform.rotation;

        Vector3 targetPosition = Camera.main.transform.position;
        targetPosition -= targetRotation * Vector3.back * distance;

        transform.position = targetPosition;

        shadowCamera.Render();
    }
}
