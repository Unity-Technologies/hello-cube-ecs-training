# Hello Cube DOTS Training Notes for the Instructor

This document serves as an instructional guide to introduce trainees to key Data Oriented Design (DOD) concepts and Unity's Data Oriented Technology Stack (DOTS) and the Entities API.  Along with this document is a Unity project designed to give the first hands on exposure to DOTS.  It is set up to walk through three ways of rotating a large number of cubes in Unity to illustrate how DOTS can be used in a small, self contained example.  Included are three scenes:

1. 01-MonoBehaviour
2. 02-EntitiesForEach
3. 03-IJobForEach

The intent is for you to start with the `01-MonoBehaviour` scene and code already completed and then modify the code toward the next version live in front of training participants and have them follow along on their own computers.  Wherever possible, the code has been commented to give more context and background.

The remainder of this document will describe in detail the recommended teaching sequence for this sample.

### Data Oriented Design
This is the **most crucial** concept to get across for the whole training.  It is not merely doing everything with arrays (although that tends to be the case since linear array accesses are fast) or using ECS; it's a higher level concept where you understand all of your input data and what transform must be done to get the desired output data.  Present the following problem to trainees:

> You want to rotate some cubes about the Y axis at a specified rotation rate.  The number of cubes to rotate is unknown, but it is reasonable to assume it can be any integer in the range [0, 100000]

All trainees should be asked to think about the rotating cube problem and be able to answer the following questions:

1. What is your input data?
2. What is your output data?
3. What transform must you do in order to generate the output data from the input data?
4. What is the minimum set of input/output for the problem?
5. What ranges of values do you expect for your inputs and outputs?

Trainees should not use arbitrary world modeling nouns.  For example, if a cube class is mentioned, they have failed.  They should be identifying concrete data that the computer must interact with to achieve the desired outcome.

Answers for this specific rotating cube problem:

> Inputs:
>
> 1. Rotation quaternions.
> 2. Delta time (single precision float).
> 3. Rotation rates (single precision float).
> 4. Number of cubes to rotate (integer).
>
> Outputs:
>
> 1. Rotation quaternions.
>
> (The above inputs and outputs are also minimal)
>
> Transformation:
>
> r = q * r, where q is the quaternion representing the rotation about the Y axis by (rotation rate * delta time) and r is the cube's current rotation quaternion.
>
> Expected input ranges:
>
> 1. Rotation quaternion.y will vary in range [-1, 1] and quaternion.w will vary in range [-1, 1].
> 2. Time is frame time, so we expect it to be in the range (0, 33.33] milliseconds, but the upper bound could be larger.
> 3. Rotation rate is constant, but it could vary with user input at edit time.
> 4. Number of cubes to rotate is [0, 100000] as given by problem statement.
>
> Expected output ranges:
>
> 1. Quaternion output range is same as input range.

Spend some time to go over trainee responses and compare with the given solutions.  After this is done, pose a question for trainees to think about:

> How do these answers change if this sample is modified to handle mouse input?  For example, the cubes should only rotate if the mouse is hovering over a cube (assume that hovering is determined by a sphere vs ray intersection or some other simple intersection test).

## 01-MonoBehaviour
![](markdown-resources/01-MonoBehaviour-PlayMode.png)

This scene represents what everyone knows in Unity today.  Using GameObjects and MonoBehaviours, we spawn a number of cubes which will then rotate.  The Unity project already has the following packages installed but you should begin this exercise with them uninstalled (go to Window > Package Manager):

1. Entities.
2. Hybrid Renderer.

In the hierarchy, you will find a CubeSpawner and a Cube game object.  The CubeSpawner will perform the actual work of spawning the rotating cubes.  The Cube game object merely serves as positional reference for the camera and doesn't serve any functional purpose:

![](markdown-resources/01-MonoBehaviour.png)

In the project, there should be a scene and two scripts:

![](markdown-resources/01-MonoBehaviour-Project.png)

You should start in `RotatingCubeSpawner.cs` and quickly explain how the CubeSpawner will spawn the rotating cubes.  After showing the cube spawning logic, go to `RotatingCube.cs` and show how the cube rotates around the Y axis.  At this point, you should reference data oriented design and the activity with inputs and outputs to the cube rotation problem.  Although the input and output data here is relatively easy to determine, point out that in a real game this function could be extremely complicated and finding that data could be difficult or time consuming.  This is one of the pitfalls of object oriented design: it doesn't focus on the data but on arbitrary abstractions that hide what the computer must actually do to compute results.

Parameters such as the number of cubes in the X axis, Z axis, and speed of rotation can be set on the CubeSpawner's `RotatingCubeSpawner` component:

![](markdown-resources/01-MonoBehaviour-Parameters.png)

This is a good opportunity to have trainees modify the number of cubes spawned to see how many cubes can be supported in Unity today.  For the remainder of this exercise, I will assume everyone is using the default values of `NumXCubes = 150` and `NumZCubes = 150` for a total of 22,500 cubes.  All trainees should enter play mode and open the Profiler to see how the performance breaks down:

![](markdown-resources/01-MonoBehaviour-Profile.png)

About 80 milliseconds are spent on the CPU with about 27 milliseconds spent on update logic and the rest on CPU rendering logic.  The screenshot shows the highlighted portion which pertains specifically to BehaviourUpdate which takes nearly 21 milliseconds of time on the main thread.  Zero work is scheduled on the job threads while this is running.

You have reached the end of the material to be presented for this scene.

## 02-EntitiesForEach
This scene contains the first use of DOTS and the Entities API.  You should take the previous scene as the starting point and modify the code and assets to what you see in this scene, live in front of the trainees and have them follow along on their computers.  This guide will walk you through the recommended sequence to modify the previous scene into this one.  Take note: the code in this scene serves only as reference and the names of some structs and classes may differ from what is presented below.

### Installing DOTS packages
To start, have the trainees install the Entities and Hybrid Renderer packages.  Go to Window > Package Manager (you may need to show all packages and show preview packages):

![](markdown-resources/02-EntitiesForEach-PackMan1.png)

![](markdown-resources/02-EntitiesForEach-PackMan2.png)

![](markdown-resources/02-EntitiesForEach-PackMan3.png)

![](markdown-resources/02-EntitiesForEach-PackMan4.png)

### Converting CubeSpawner
We will begin by porting the cube spawning logic in `RotatingCubeSpawner.cs`.  The final ported code can be found in:

* `RotatingCubeSpawnerConverter_EntitiesForEach.cs`
* `RotatingCubeSpawnerSystem_EntitiesForEach.cs`

In `RotatingCubeSpawner.cs`, rename `RotatingCubeSpawnerConverter` (and rename the file if necessary) and make it also implement this interface:

* `IConvertGameObjectToEntity`

```
public class RotatingCubeSpawner : MonoBehaviour
```

becomes:

```
public class RotatingCubeSpawnerConverter : MonoBehaviour, IConvertGameObjectToEntity
```

In the editor, find the CubeSpawner game object and be sure it has a `Rotating Cube Spawner Converter` component and add a new `Convert To Entity` component:

![](markdown-resources/02-EntitiesForEach-CubeSpawnerComponents.png)

(You may need to change the conversion mode to `Convert And Destroy`.  These conversion modes describe what must happen to the game object after conversion is performed.)

Adding the `Convert To Entity` component tells the Entities conversion system that this game object will be converted into an entity and as a result of implementing `IConvertGameObjectToEntity`, you will have to implement this function to describe what happens when the game object is converted:

```
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
    }
```

`entity` is the Entity that was created by the conversion system for this object and you must interact with the EntityManager `dstManager` to create the appropriate components for the converted entity and the GameObjectConversionSystem `conversionSystem` to deal with prefab objects.  Implement `Convert()` as follows:

```
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var rotatingCubePrefabEntity = conversionSystem.GetPrimaryEntity(RotatingCubePrefab);

        var cubeSpawnerData = new RotatingCubeSpawnerData
        {
            NumXCubes = NumXCubes,
            NumZCubes = NumZCubes,
            RotationSpeed = math.radians(RotationSpeed),
            RotatingCubePrefabEntity = rotatingCubePrefabEntity,
        };

        dstManager.AddComponentData(entity, cubeSpawnerData);
    }
```

This code will not compile since we have not yet defined `RotatingCubeSpawnerData`.  Define it now:

```
public struct RotatingCubeSpawnerData : IComponentData
{
    public int NumXCubes;
    public int NumZCubes;
    public float RotationSpeed;
    public Entity RotatingCubePrefabEntity;
}
```

We have defined our first component!  This component contains all the data necessary for the cube spawner to spawn cubes.  The conversion function should now compile and we can explain what it does to the trainees.  Recall that `entity` was created for us by the conversion system to represent the game object CubeSpawner.  Our conversion code takes the cube spawning data on the MonoBehaviour and puts it into component data and adds that component data to the entity so we know how many cubes will need to be spawned when we process this spawner entity later.  The one mystery is this line dealing with the prefab:

```
var rotatingCubePrefabEntity = conversionSystem.GetPrimaryEntity(RotatingCubePrefab);
```

Somehow, the conversion system should know about our `RotatingCubePrefab` and give us an entity.  But we haven't done anything yet that deals with this prefab, so the entity we get back is null.  Try running the code and breaking here:

![](markdown-resources/02-EntitiesForEach-NoReferencedPrefab.png)

To fix this, make `RotatingCubeSpawnerConverter` also implement `IDeclareReferencedPrefabs`:

```
public class RotatingCubeSpawnerConverter : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
```

Define this function:

```
    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(RotatingCubePrefab);
    }
```

This makes the conversion system aware of the `RotatingCubePrefab` so it knows to create an entity version of that prefab.  When you execute the code now, you will get a valid entity back for the prefab:

![](markdown-resources/02-EntitiesForEach-AfterDeclaringPrefab.png)

We have now converted the CubeSpawner game object to an entity!

### Implementing CubeSpawner Logic with Entities.ForEach
The cube spawner is now an entity, but when you enter play mode, no cubes are spawned:

![](markdown-resources/02-EntitiesForEach-NoSpawning.png)

Trainees might be confused why no cubes are spawning but recall that we only made the cube spawner into an entity but never actually implemented the logic to create new cubes.  In order to make the cube spawner do something, we must implement a ComponentSystem which will operate on the entities that have a `RotatingCubeSpawnerData` component.  Create a new script file named `RotatingCubeSpawner` and define this class:

```
public class RotatingCubeSpawner : ComponentSystem
{
    protected override void OnUpdate()
    {
    }
}
```

Also define this component:

```
public struct RotationSpeed : IComponentData
{
    public float Value;
}
```

ComponentSystems are where you work with entities and components to implement your data transformations.  `OnUpdate()` is called every frame and runs on the main thread.  Fill in that function like so:

```
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity, ref RotatingCubeSpawnerData spawnerData) =>
        {
            for (int x = 0; x < spawnerData.NumXCubes; ++x)
            {
                float posX = x - (spawnerData.NumXCubes / 2);

                for (int z = 0; z < spawnerData.NumZCubes; ++z)
                {
                    float posZ = z - (spawnerData.NumZCubes / 2);

                    var rotatingCubeEntity = EntityManager.Instantiate(spawnerData.RotatingCubePrefabEntity);

                    EntityManager.SetComponentData(rotatingCubeEntity, new Translation { Value = new float3(posX, 0.0f, posZ) });
                    EntityManager.AddComponentData(rotatingCubeEntity, new RotationSpeed { Value = spawnerData.RotationSpeed });
                }
            }

            EntityManager.DestroyEntity(entity);
        });
    }
```

`Entities.ForEach` is a convenient and easy API for working with ECS.  You just define a lambda with the components you're interested as inputs and the body should do the data transform you want.  In this case, we want to access every entity with a `RotatingCubeSpawnerData` component and spawn all the cubes.  The spawning of a cube is done with this line:

```
var rotatingCubeEntity = EntityManager.Instantiate(spawnerData.RotatingCubePrefabEntity);
```

To set the position of the newly instantiated cube, we set the `Translation` component:

```
EntityManager.SetComponentData(rotatingCubeEntity, new Translation { Value = new float3(posX, 0.0f, posZ) });
```

Finally, we add a new component of our own, `RotationSpeed` which will contain the radians per second each cube should rotate at:

```
EntityManager.AddComponentData(rotatingCubeEntity, new RotationSpeed { Value = spawnerData.RotationSpeed });
```

Most of this code should be straightforward to understand and mirrors the original MonoBehaviour logic.  The last line might be confusing:

```
EntityManager.DestroyEntity(entity);
```

Why are we destroying an entity?  Remember that the entity we are working with is the cube spawner entity.  If we don't destroy it, then on the second frame of the game, this system will run again since it sees an entity with a `RotatingCubeSpawnerData` on it still and will run the spawning logic.  By destroying the entity, we ensure the data for it doesn't exist so the system can't possibly run again on that entity.  This is a common way of running logic that should occur only once in ECS; create the data, have a system process it, and then destroy the data.  If there is no data, there is nothing to do!

You should now be spawning cubes, but no rotation will occur:

![](markdown-resources/02-EntitiesForEach-Spawning.png)

### Rotating cubes with Entities.ForEach
We are finally ready to rotate some cubes with Entities.ForEach.  Create a new script file and name it `RotatingCubeSystem` and define this class:

```
public class RotatingCubeSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach(() =>
        {
        });
    }
}
```

Ask the trainees to think about the questions they answered at the beginning of the session regarding the input and output data for the rotating cube problem.  The answers they came up with should essentially be the implementation in this code:

```
public class RotatingCubeSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref Rotation rotation, ref RotationSpeed rotationSpeed) =>
        {
            float rotationThisFrame = Time.deltaTime * rotationSpeed.Value;
            var q = quaternion.AxisAngle(new float3(0.0f, 1.0f, 0.0f), rotationThisFrame);
            rotation.Value = math.mul(q, rotation.Value);
        });
    }
}
```

That's it, we're rotating cubes now!

![](markdown-resources/02-EntitiesForEach-Rotating.png)

Point out to the trainees how this code makes the data input and output much more explicit:

```
        Entities.ForEach((ref Rotation rotation, ref RotationSpeed rotationSpeed) =>
        {
            float rotationThisFrame = Time.deltaTime * rotationSpeed.Value;
            var q = quaternion.AxisAngle(new float3(0.0f, 1.0f, 0.0f), rotationThisFrame);
            rotation.Value = math.mul(q, rotation.Value);
        });
```

We've explicitly named the `Rotation` component (a quaternion) and the `RotationSpeed` as inputs.  Delta time remains an implicit input here.  The number of cubes is not explicitly handled here by us; the `Entities.ForEach` API handles this for us.  Being more explicit with your data makes understanding your problem much easier and opens up optimization opportunities.  What we've done here is a basic but very fundamental application of data oriented design and the Entities package provides you tools to apply this.

### Performance of Entities.ForEach
Enter play mode and open the profiler.  You should see something similar to this:

![](markdown-resources/02-EntitiesForEach-Profiler.png)

The overall frame time has decreased significantly but those gains come from CPU rendering work, not the rotating cube update.  In the screenshot, the highlighted portion is the RotatingCubeSystem and again, everything is on the main thread and the job worker threads are idle.  If you compare with the BehaviourUpdate time in the MonoBehaviour version, it is very similar (~20.6 ms vs ~23.5 ms).

### Entity Debugger
When using the Entities API, it can be very helpful to use the Entity Debugger to see what systems are running, what entities exist, and what the chunk utilization looks like.  Open the Entity Debugger by going to Window > Analysis > Entity Debugger:

![](markdown-resources/02-EntitiesForEach-EntityDebuggerMenu.png)

The Entity Debugger looks like this:

![](markdown-resources/02-EntitiesForEach-EntityDebugger1.png)

On the left is the list of systems that are running, center is a list of entities, and the right contains chunk info for the entities.  In the system menu, click on `RotatingCubeSystem` and you will see the list of entities change and will probably be blank at first.  If it is blank, select the top item marked `(RW Rotation) (RW Rotation Speed)`:

![](markdown-resources/02-EntitiesForEach-EntityDebugger2.png)

You should see a full list of entities like this:

![](markdown-resources/02-EntitiesForEach-EntityDebugger3.png)

You can select a specific entity in the list and view their component values in the inspector:

![](markdown-resources/02-EntitiesForEach-EntityDebugger4.png)

On the right, in the chunk list, you can see statistics about chunk utilizations of a particular archetype:

![](markdown-resources/02-EntitiesForEach-EntityDebugger5.png)

The chunk utilization graph is a histogram.  In this case, there are 549 total chunks for this archetype, 548 of those chunks have 41 entities in them and 1 chunk with less than 41 entities.  We spawned 22,500 cube entities so we should expect that adding up all the chunk's utilizations should add up to 22,500.  Doing some math, we can see that 548 * 41 = 22,468.  1 chunk has less than 41, but we can find out how much that chunk has from 22,500 - 22,468 = 32.  This chunk histogram can be very useful when debugging memory usage problems; if there are a lot of chunks with low chunk utilization, then you are wasting huge amounts of memory for each chunk only to hold a small number of entities.  In this case, we have very good utilization where almost every chunk is full.

Another useful feature of the Entity Debugger is enabling or disabling systems.  On the left side in the sytem list, there is a checkbox which can turn systems on or off:

![](markdown-resources/02-EntitiesForEach-EntityDebugger6.png)

Disable the RotatingCubeSystem and you can see that all the cubes stop rotating.  The ability to turn systems on or off can be very useful in narrowing down the causes of bugs.

## 03-IJobForEach
Now that we have our data in components and are using the Entities API, we can start jobifying the code to make use of all the cores on the machine.  Return to `RotatingCubeSystem` and instead of inheriting from `ComponentSystem`, inherit from `JobComponentSystem`:

```
public class RotatingCubeSystem : JobComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref Rotation rotation, ref RotationSpeed rotationSpeed) =>
        {
            float rotationThisFrame = Time.deltaTime * rotationSpeed.Value;
            var q = quaternion.AxisAngle(new float3(0.0f, 1.0f, 0.0f), rotationThisFrame);
            rotation.Value = math.mul(q, rotation.Value);
        });
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
    }
}
```

This code will no longer compile and the `Entities.ForEach()` API is no longer usable.  Instead, we must create a job.  Define a `RotatingCubeJob`:

```
    public struct RotatingCubeJob : IJobForEach<Rotation, RotationSpeed>
    {
        public void Execute(ref Rotation rotation, [ReadOnly] ref RotationSpeed rotationSpeed)
        {
        }
    }
```

The `Execute()` function here is what will be executed by each job thread, in parallel.  The `Rotation` component is assumed to have read/write access by the job system.  The `RotationSpeed` component is assumed to be read only by the job system due to the `[ReadOnly]`  attribute (be sure you have `using Unity.Collections;`).  This `[ReadOnly]` attribute is a hint to the job safety system that we don't intend to write to the `RotationSpeed` component, so any other job which only reads for that component should be safe to run in parallel.  However, if another job is scheduled with read/write access to `RotationSpeed` at the same time as this job, then an error will be generated to indicate a race condition.

We should fill out the rest of the job code like so:

```
    public struct RotatingCubeJob : IJobForEach<Rotation, RotationSpeed>
    {
        public float DeltaTime;
        
        public void Execute(ref Rotation rotation, [ReadOnly] ref RotationSpeed rotationSpeed)
        {
            float rotationThisFrame = DeltaTime * rotationSpeed.Value;
            var q = quaternion.AxisAngle(new float3(0.0f, 1.0f, 0.0f), rotationThisFrame);
            rotation.Value = math.mul(q, rotation.Value);
        }
    }
```

The body of the `Execute()` function is nearly the same as the body of the lambda from the `Entities.ForEach()` version, the only difference being the use of `DeltaTime` from the job struct instead of `Time.deltaTime`.  We must do this because jobs cannot access static data; any static data or data from a managed object must be retrieved from the main thread and passed in as data on the job struct.  This is done to ensure that we are safely accessing managed objects, which can only be guaranteed on the main thread.

Notice how in the job version of this code, the inputs and the outputs are even more explicit by passing delta time as a data member on the job struct.  When you spend the time to think about your data and how it needs to be transformed, before you write *any* code, you know a lot about your problem.  Writing the code (which can also be easily jobified) is almost a formality.

Now we need to finish implementing `RotatingCubeSystem.OnUpdate()`:

```
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return new RotatingCubeJob { DeltaTime = Time.deltaTime }.Schedule(this, inputDeps);
    }
```

Here we create the `RotatingCubeJob` struct and set `DeltaTime` for the job to use.  The job struct has a `Schedule()` method which takes the system and a JobHandle as its dependency.  In this case, our `RotatingCubeJob` is dependent on whatever job is represented by `inputDeps` so it will execute after those jobs finish.  Each JobComponentSystem must return a JobHandle so that other systems that come after it can properly set up a dependency chain.

The final code should look like this:

```
public class RotatingCubeSystem : JobComponentSystem
{
    public struct RotatingCubeJob : IJobForEach<Rotation, RotationSpeed>
    {
        public float DeltaTime;

        public void Execute(ref Rotation rotation, [ReadOnly] ref RotationSpeed rotationSpeed)
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
```

### Performance of IJobForEach
Enter play mode and open the profiler:

![](markdown-resources/03-IJobForEach-Parallel.png)

Performance is massively improved due to usage of jobs.  The actual work of rotating the cubes now takes about 2.72 ms of wall clock time (instead of ~23.5 ms on the main thread with Entities.ForEach).  The overall frame time has gone from ~40 ms with Entities.ForEach and now we are achieving about 20 ms, a speedup of 2x!

### But wait, there's more!
As impressive as this is, we can improve performance even further.  Go back to `RotatingCubeJob` and add the `[BurstCompile]` attribute to the struct:

```
    [BurstCompile]
    public struct RotatingCubeJob : IJobForEach<Rotation, RotationSpeed>
    {
        public float DeltaTime;

        public void Execute(ref Rotation rotation, [ReadOnly] ref RotationSpeed rotationSpeed)
        {
            float rotationThisFrame = DeltaTime * rotationSpeed.Value;
            var q = quaternion.AxisAngle(new float3(0.0f, 1.0f, 0.0f), rotationThisFrame);
            rotation.Value = math.mul(q, rotation.Value);
        }
    }
```

Go back into play mode and open the profiler.  Look around in one of the job threads for `RotatingCubeSystem:RotatingCubeJob (Burst)`:

![](markdown-resources/03-IJobForEach-Burst.png)

The RotatingCubeJob is now *even faster*, going from a total execution time of 31.89 ms without Burst to 1 ms with Burst!  This is the power of machine code.  For those inclined to see the machine code, they can go into the Burst Inspector via Jobs > Burst > Open inspector:

![](markdown-resources/03-IJobForEach-BurstInspector1.png)

You can select various jobs and then click `Refresh Disassembly` to see the compiled code:

![](markdown-resources/03-IJobForEach-BurstInspector2.png)

Going all the way back to the original MonoBehaviour version, we had 80 ms frame times for 22,500 rotating cubes.  Now we can do many times more cubes at even faster frame times.

## Conclusion
You have reached the end of this exercise and introduced DOTS.  If time permits, you can have trainees attempt to implement the question we posed at the beginning during the data oriented design section:

> How do these answers change if this sample is modified to handle mouse input?  For example, the cubes should only rotate if the mouse is hovering over a cube (assume that hovering is determined by a sphere vs ray intersection or some other simple intersection test).

Trainees should come up with the data required first, just like in the exercise and then write the code to implement their data transformation.  The point is for this to be **data oriented design**.