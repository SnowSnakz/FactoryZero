using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAttachment : MonoBehaviour
{
    public bool usePosition;
    public bool positionX;
    public bool positionY;
    public bool positionZ;

    public bool useRotation;
    public bool rotateX;
    public bool rotateY;
    public bool rotateZ;

    public Vector3 positionOffset;
    public Vector3 rotationOffset;

    public GameObject parent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Transform pt = parent.transform;

        if(usePosition)
        {
            Vector3 op = transform.position;
            Vector3 pp = pt.position;

            transform.position = new Vector3(positionX ? pp.x : op.x, positionY ? pp.y : op.y, positionZ ? pp.z : op.z) + positionOffset;
        }

        if(useRotation)
        {
            Vector3 op = transform.rotation.eulerAngles;
            Vector3 pp = pt.rotation.eulerAngles;

            transform.rotation = Quaternion.Euler(new Vector3(rotateX ? pp.x : op.x, rotateY ? pp.y : op.y, rotateZ ? pp.z : op.z) + rotationOffset);
        }
    }
}
