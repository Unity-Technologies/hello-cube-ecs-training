using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class RotatingCubeSystem_IJobForEach : JobComponentSystem
{
    //[BurstCompile]
    public struct RotatingCubeJob : IJobForEach<Rotation, RotationSpeed_IJobForEach>
    {
        public float DeltaTime;

        public void Execute(ref Rotation rotation, [ReadOnly] ref RotationSpeed_IJobForEach rotationSpeed)
        {
            float rotationThisFrame = DeltaTime * rotationSpeed.Value;
            var q = quaternion.AxisAngle(new float3(0.0f, 1.0f, 0.0f), rotationThisFrame);
            rotation.Value = math.mul(q, rotation.Value);
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return new RotatingCubeJob { DeltaTime = Time.deltaTime }.Schedule(this, inputDeps);
    }
}