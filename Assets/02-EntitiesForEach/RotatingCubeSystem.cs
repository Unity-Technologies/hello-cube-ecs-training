using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class RotatingCubeSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref Rotation rotation) =>
        {
            // Notice we work in radians here instead of degrees.
            // Most Unity.Mathematics functions working with angles will want radians.
            float rotationRate = math.PI * 0.5f;
            float rotationThisFrame = Time.deltaTime * rotationRate;
            var q = quaternion.AxisAngle(new float3(0.0f, 1.0f, 0.0f), rotationThisFrame);
            rotation.Value = math.mul(q, rotation.Value);
        });
    }
}