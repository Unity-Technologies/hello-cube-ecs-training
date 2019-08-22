using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct ECSCubeSpawnerData : IComponentData
{
    public int NumCubes;
    public float SpawnRadius;
    public Entity RotatingCubePrefabEntity;
}

// This attribute isn't necessary for conversion, but can help ensure that the ConvertToEntity component is
// present on the GameObject in the UnityEditor. The ConvertToEntity component starts the conversion process
// to produce an entity and you can specify what happens to the GameObject after conversion is complete.
// See the ConvertToEntity component on the CubeSpawner GameObject to see the different conversion modes.
[RequiresEntityConversion]
public class ECSCubeSpawner : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public int NumCubes;
    public float SpawnRadius;
    public GameObject RotatingCubePrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        // This MonoBehaviour references a prefab which will be used to spawn cubes.  But ECS doesn't work with
        // GameObjects, only entities and components!  For this prefab to be usable, you must declare it to ECS
        // by adding it to a list of referenced prefabs so the conversion system knows to create an entity for
        // that prefab.
        referencedPrefabs.Add(RotatingCubePrefab);
    }

    // 'entity' represents the entity that was created for this GameObject.  This Convert() function must be
    // implemented by you to properly transform your GameObject into an equivalent ECS representation.
    // By the end of this function, 'entity' and its component data should be complete enough for a ComponentSystem
    // to later run the cube spawning logic.  This means that 'entity' should have components with the correct data
    // on it about what to spawn and how much.
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        // The GameObjectConversionSystem helps you convert MonoBehaviours and GameObjects to entities.
        // We grab the rotating cube prefab as an entity with the same prefab that we declared earlier in
        // DeclareReferencedPrefabs().
        var rotatingCubePrefabEntity = conversionSystem.GetPrimaryEntity(RotatingCubePrefab);

        // Now that we have the prefab as an entity, we can save it in a component for another system
        // to use later.
        var cubeSpawnerData = new ECSCubeSpawnerData
        {
            NumCubes = NumCubes,
            SpawnRadius = SpawnRadius,
            RotatingCubePrefabEntity = rotatingCubePrefabEntity,
        };

        // Ask the EntityManager to actually add this component data to the spawner entity.
        dstManager.AddComponentData(entity, cubeSpawnerData);
    }
}
