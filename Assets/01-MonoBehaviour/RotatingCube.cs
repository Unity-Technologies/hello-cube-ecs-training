using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingCube : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float rotationRate = 2.0f;
        float rotationThisFrame = Time.deltaTime * rotationRate;
        transform.rotation *= Quaternion.AngleAxis(rotationThisFrame, Vector3.up);
    }
}
