using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class RotatingCubeEntitiesForEach : ComponentSystem
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity, ref RotatingCubeSpawnerData spawnerData) =>
        {
            for (int i = 0; i < spawnerData.NumCubes; ++i)
            {
                float rad = ((float)i / (float)spawnerData.NumCubes) * Mathf.PI * 2.0f;
                float posX = spawnerData.SpawnRadius * math.sin(rad);
                float posZ = spawnerData.SpawnRadius * math.cos(rad);

                // Actually create a rotating cube entity from the prefab.
                var rotatingCubeEntity = EntityManager.Instantiate(spawnerData.RotatingCubePrefabEntity);

                // Set the position of the rotating cube.
                EntityManager.SetComponentData(rotatingCubeEntity, new Translation { Value = new float3(posX, 0.0f, posZ) });
            }

            // We should destroy this spawner entity because if we don't, the spawner will run again on the next frame
            // and spawn another NumCubes rotating cubes!
            EntityManager.DestroyEntity(entity);
        });
    }
}
