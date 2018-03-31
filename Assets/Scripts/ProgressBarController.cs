using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBarController : MonoBehaviour {

    [SerializeField]
    GameObject filler;

    [SerializeField]
    Camera cameraAnchor;

    // Use this for initialization
    void Start()
    {
        if(cameraAnchor == null)
            cameraAnchor = Camera.main;
    }

    //void Update()
    //{
    //    //float depth = Vector3.Project(transform.position - camera.transform.position, camera.transform.forward).magnitude;

    //    float depth = (cameraAnchor.transform.position - transform.position).magnitude;
    //    float desiredSize = Mathf.Tan(0.5f * Mathf.Deg2Rad * 0.5f) * 2 * depth;
    //    float requiredScale = desiredSize / 960;
    //    transform.localScale = new Vector3(requiredScale, requiredScale, requiredScale);
    //}

    public void SetValue(float value)
    {
        filler.transform.localScale = new Vector3(value, filler.transform.localScale.y, filler.transform.localScale.z);
    }
}
