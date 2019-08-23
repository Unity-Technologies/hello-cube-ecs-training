# Hello Cube DOTS Training Notes for the Instructor

This code sample is an introduction to DOTS and the Entities API.  It is set up to walk through three ways of rotating a large number of cubes in Unity to illustrate how DOTS can be used in a small, self contained example.  Included are three scenes:

1. 01-MonoBehaviour
2. 02-EntitiesForEach
3. 03-IJobForEach

The intent is for you to start with `01-MonoBehaviour` already completed and then modify the code toward the next version live in front of training participants and have them follow along on their own computers.  Wherever possible, the code has been commented to give more context and background.

The following sections will serve as an instructional guide for each scene and its respective code.

## 01-MonoBehaviour
This scene represents what everyone knows in Unity today.  Using GameObjects and MonoBehaviours, we spawn a number of cubes which will then rotate.

![](markdown-resources/01-MonoBehaviour.png)

In the hierarchy, you will find a CubeSpawner and a Cube game object.  The CubeSpawner will perform the actual work of spawning the rotating cubes.  The Cube game object merely serves as positional reference for the camera and doesn't serve any functional purpose.