using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingCubeSpawner : MonoBehaviour
{
    public int NumCubes;
    public float SpawnRadius;
    public GameObject RotatingCubePrefab;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < NumCubes; ++i)
        {
            float rad = ((float)i / (float)NumCubes) * Mathf.PI * 2.0f;
            float posX = SpawnRadius * Mathf.Sin(rad);
            float posZ = SpawnRadius * Mathf.Cos(rad);

            var obj = Instantiate(RotatingCubePrefab);
            var transform = obj.GetComponent<Transform>();
            transform.position = new Vector3(posX, 0.0f, posZ);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
