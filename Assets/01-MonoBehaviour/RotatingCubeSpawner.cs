using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingCubeSpawner : MonoBehaviour
{
    public int NumXCubes;
    public int NumZCubes;
    public float RotationSpeed;
    public GameObject RotatingCubePrefab;

    // Start is called before the first frame update
    void Start()
    {
        for (int x = 0; x < NumXCubes; ++x)
        {
            float posX = x - (NumXCubes / 2);

            for (int z = 0; z < NumZCubes; ++z)
            {
                float posZ = z - (NumZCubes / 2);

                var obj = Instantiate(RotatingCubePrefab);
                obj.GetComponent<RotatingCube>().RotationSpeed = RotationSpeed;
                var transform = obj.GetComponent<Transform>();
                transform.position = new Vector3(posX, 0.0f, posZ);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
