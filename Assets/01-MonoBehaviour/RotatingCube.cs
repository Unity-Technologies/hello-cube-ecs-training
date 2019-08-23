using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingCube : MonoBehaviour
{
    public float RotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float rotationThisFrame = Time.deltaTime * RotationSpeed;
        transform.rotation *= Quaternion.AngleAxis(rotationThisFrame, Vector3.up);
    }
}
