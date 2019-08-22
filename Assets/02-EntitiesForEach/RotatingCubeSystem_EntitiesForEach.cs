using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class RotatingCubeSystem_EntitiesForEach : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref Rotation rotation, ref RotationSpeed_EntitiesForEach rotationSpeed) =>
        {
            float rotationThisFrame = Time.deltaTime * rotationSpeed.Value;
            var q = quaternion.AxisAngle(new float3(0.0f, 1.0f, 0.0f), rotationThisFrame);
            rotation.Value = math.mul(q, rotation.Value);
        });
    }
}