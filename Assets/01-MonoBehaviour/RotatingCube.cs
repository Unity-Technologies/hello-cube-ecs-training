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
        // Notes to instructor:
        //
        // Data design and thinking about inputs and outputs to your problem is crucial to Data Oriented Design (DOD)
        // and ECS.  This simple problem of rotating a cube should be called out and training participants should
        // be asked to think about what the inputs and outputs are for this problem.
        //
        // Questions such as:
        //
        // 1. What is your input data?
        // 2. What is your output data?
        // 3. What is the minimum set of input/output for the problem?
        // 4. What ranges of values do you expect for your inputs and outputs?
        //
        // Should all have answers and will inform the solution that will be implemented.
        //
        // Inputs:
        //
        // 1. Transform (specifically, the rotation quaternion).
        // 2. Time.
        // 3. Rotation rate.
        //
        // Outputs:
        //
        // 1. Transform.
        //
        // (Above inputs and outputs are also minimal)
        //
        // Expected input ranges:
        //
        // 1. Transform
        //    - Position should not change from cube's original spawn position.
        //    - Scale should remain 1.0f.
        //    - Rotation will vary with the quaternion.y in range [-1, 1] and quaternion.w in range [-1, 1].
        // 2. Time is frame time, so we expect it to be in the range (0, 33.33] milliseconds.
        // 3. Rotation rate is constant and we expect it to be 90 degrees per second.
        //
        // Expected output ranges:
        //
        // 1. Transform expected output is same as input.
        //
        // Instructor should pose this question after previous four questions are answered:
        //
        // How do these answers change when this sample is modified to handle mouse input?  For example, the cubes
        // should only rotate if the mouse is hovering over a cube (imagine a simple test like a sphere vs ray test
        // to determine if the mouse is hovering over a cube).
        float rotationRate = 90.0f;
        float rotationThisFrame = Time.deltaTime * rotationRate;
        transform.rotation *= Quaternion.AngleAxis(rotationThisFrame, Vector3.up);
    }
}
