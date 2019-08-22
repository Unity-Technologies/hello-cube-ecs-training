using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class RotatingCubeSystem_IJobForEach : JobComponentSystem
{
    // RotatingCubeJob is a job that operates on each entity with a Rotation and RotationSpeed_IJobForEach component.

    // Try uncommenting this [BurstCompile] attribute!
    //[BurstCompile]
    public struct RotatingCubeJob : IJobForEach<Rotation, RotationSpeed_IJobForEach>
    {
        public float DeltaTime;

        // Notice the ref and use of the [ReadOnly] attribute.  These are ref parameters because you may want
        // to change the entity component data.  The [ReadOnly] attribute signals to the job system that the
        // RotationSpeed_IJobForEach component will only be read and thus can be scheduled with other jobs
        // that read this component.  Any job that is read/write on the component will cause a race condition
        // and you will receive a warning if you try to schedule a job that has write access to this component
        // at the same time as another job that has read or write access.
        public void Execute(ref Rotation rotation, [ReadOnly] ref RotationSpeed_IJobForEach rotationSpeed)
        {
            float rotationThisFrame = DeltaTime * rotationSpeed.Value;
            var q = quaternion.AxisAngle(new float3(0.0f, 1.0f, 0.0f), rotationThisFrame);
            rotation.Value = math.mul(q, rotation.Value);
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        // Scheduling a job requires you to create the job struct and call schedule on it with
        // a JobHandle.  The JobHandle you pass in is the dependency of the job you pass in, so in
        // this case, RotatingCubeJob is dependent on whatever job is represented by inputDeps.
        // When you schedule a job, it returns a new JobHandle which you can use as dependencies to other
        // jobs.
        return new RotatingCubeJob { DeltaTime = Time.deltaTime }.Schedule(this, inputDeps);
    }
}