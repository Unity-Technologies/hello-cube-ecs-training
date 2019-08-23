# Hello Cube DOTS Training Notes for the Instructor

This code sample is an introduction to DOTS and the Entities API.  It is set up to walk through three ways of rotating a large number of cubes in Unity to illustrate how DOTS can be used in a small, self contained example.  Included are three scenes:

1. 01-MonoBehaviour
2. 02-EntitiesForEach
3. 03-IJobForEach

The intent is for you to start with `01-MonoBehaviour` already completed and then modify the code toward the next version live in front of training participants and have them follow along on their own computers.  Wherever possible, the code has been commented to give more context and background.

The following sections will serve as an instructional guide for each scene and its respective code.

## 01-MonoBehaviour
This scene represents what everyone knows in Unity today.  Using GameObjects and MonoBehaviours, we spawn a number of cubes which will then rotate.  In the hierarchy, you will find a CubeSpawner and a Cube game object.  The CubeSpawner will perform the actual work of spawning the rotating cubes.  The Cube game object merely serves as positional reference for the camera and doesn't serve any functional purpose:

![](markdown-resources/01-MonoBehaviour.png)

In the project, there should be a scene and two scripts:

![](markdown-resources/01-MonoBehaviour-Project.png)

You should start in `RotatingCubeSpawner.cs` and quickly explain how the CubeSpawner will spawn the rotating cubes.  After showing the cube spawning logic, go to `RotatingCube.cs` and show how the cube rotates around the Y axis.

### Data Oriented Design
Here, you should take a few moments to talk about data oriented design, which is the **most crucial** concept to get across for the whole training.  It is not merely doing everything with arrays (although that tends to be the case since linear array accesses are fast); it's a higher level concept where you understand all the data you have to start with and what form you must transform it to in order to solve the problem.  When you know your data, what it starts off as and what you're trying to transform it to, many things are revealed that either makes implementing a solution very easy or forces you to redesign the data in a way to make a high quality, performant solution.  All trainees should be asked to think about the rotating cube problem and be able to answer the following questions:

1. What is your input data?
2. What is your output data?
3. What is the minimum set of input/output for the problem?
4. What ranges of values do you expect for your inputs and outputs?

Answers for this specific rotating cube problem:

> Inputs:
>
> 1. Transform (specifically, the rotation quaternion).
> 2. Time.
> 3. Rotation rate.
>
> Outputs:
>
> 1. Transform.
>
> (The above inputs and outputs are also minimal)
>
> Expected input ranges:
>
> 1. Transform
>    - Position should not change from cube's original spawn position.
>    - Scale should remain 1.0f.
>    - Rotation will vary with the quaternion.y in range [-1, 1] and quaternion.w in range [-1, 1].
> 2. Time is frame time, so we expect it to be in the range (0, 33.33] milliseconds.
> 3. Rotation rate is constant and we expect it to be 90 degrees per second.
>
> Expected output ranges:
>
> 1. Transform expected output is same as input.

Spend some time to go over trainee responses and compare with the given solutions.  After this is done, pose a question for trainees to think about:

> How do these answers change if this sample is modified to handle mouse input?  For example, the cubes should only rotate if the mouse is hovering over a cube (assume that hovering is determined by a sphere vs ray intersection or some other simple intersection test).

Parameters such as the number of cubes in the X axis, Z axis, and speed of rotation can be set on the CubeSpawner's `RotatingCubeSpawner` component:

![](markdown-resources/01-MonoBehaviour-Parameters.png)

This is a good opportunity to have trainees modify the number of cubes spawned to see how many cubes can be supported in Unity today.  For the remainder of this exercise, I will assume everyone is using the default values of `NumXCubes = 150` and `NumZCubes = 150` for a total of 22,500 cubes.  All trainees should enter play mode and open the Profiler to see how the performance breaks down:

![](markdown-resources/01-MonoBehaviour-Profile.png)

About 80 milliseconds are spent on the CPU with about 27 milliseconds spent on update logic and the rest on CPU rendering logic.  The screenshot shows the highlighted portion which pertains specifically to BehaviourUpdate which takes nearly 21 milliseconds of time on the main thread.  Zero work is scheduled on the job threads while this is running.