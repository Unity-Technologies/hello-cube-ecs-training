using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public struct RotationSpeed_EntitiesForEach : IComponentData
{
    public float Value;
}

// ComponentSystems are automatically created for you and added to a list of systems to update every frame.
// If you need more control over when the system is created and when OnUpdate() is called, then you can add
// the [DisableAutoCreation] attribute and do the work yourself.
public class RotatingCubeSpawnerSystem_EntitiesForEach : ComponentSystem
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
        // Entities.ForEach() is an easy to use programming interface to work with ECS.  You provide a function
        // which specifies the component data required as arguments and fill in the function body with the logic
        // you want to perform.
        //
        // The main disadvantage with this approach is that Entities.ForEach() executes on the main thread and does
        // not take advantage of the job system.  Furthermore, burst compilation is not possible.
        Entities.ForEach((Entity entity, ref RotatingCubeSpawnerData_EntitiesForEach spawnerData) =>
        {
            // This code is very similar to the MonoBehaviour version.
            for (int i = 0; i < spawnerData.NumCubes; ++i)
            {
                float rad = ((float)i / (float)spawnerData.NumCubes) * Mathf.PI * 2.0f;
                float posX = spawnerData.SpawnRadius * math.sin(rad);
                float posZ = spawnerData.SpawnRadius * math.cos(rad);

                // Actually create a rotating cube entity from the prefab.
                var rotatingCubeEntity = EntityManager.Instantiate(spawnerData.RotatingCubePrefabEntity);

                // Set the position of the rotating cube.
                EntityManager.SetComponentData(rotatingCubeEntity, new Translation { Value = new float3(posX, 0.0f, posZ) });
                EntityManager.AddComponentData(rotatingCubeEntity, new RotationSpeed_EntitiesForEach { Value = spawnerData.RotationSpeed });
            }

            // We should destroy this spawner entity because if we don't, the spawner will run again on the next frame
            // and spawn another NumCubes rotating cubes!
            EntityManager.DestroyEntity(entity);
        });
    }
}
