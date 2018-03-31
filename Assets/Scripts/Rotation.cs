using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour {
    enum RotationAxisType
    {
        Up,
        Forward,
        Right,
    }

    [SerializeField]
    float rotateSpeed = 30f;
    [SerializeField]
    RotationAxisType rotationAxisType = RotationAxisType.Up;
    Vector3 rotationAxisVector;

    // Use this for initialization
    void Start () {
        switch (rotationAxisType)
        {
            case RotationAxisType.Up:
                rotationAxisVector = transform.up;
                break;
            case RotationAxisType.Forward:
                rotationAxisVector = transform.forward;
                break;
            case RotationAxisType.Right:
                rotationAxisVector = transform.right;
                break;
        }
	}
	
	// Update is called once per frame
	void Update () {
        transform.RotateAround(transform.position, rotationAxisVector, rotateSpeed * Time.deltaTime);
     
    }
}
