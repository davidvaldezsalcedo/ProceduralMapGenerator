# ProceduralMapGenerator
It creates a procedural map and mesh. Included is a procedural spawner for players, enemies, and Pickups as well as a navmesh bake on awake script.


## HowTo

## MapGenerator Set Up


**1.** Download the project and simply import the .unitypackage file into your project.

**2.** Create An empty game object in the scene and name it, this will be our holder for the scripts. Name it something like MapGenerator

**3.** Create two empty game objects inside the script holder object and give both of them a Mesh Filter and a Mesh Renderer

**4.** Name one the objects we just created Walls, since this will be the walls of the map, and the other Top or Outline, since this will be the outline of the map.

**5.** Once we gave both the objects a mesh renderer, under Materials. Assign a material for them.

**6.** In the script holder object, we will add two scripts, these will be under /ProceduralMapGeneration/GeneratorScripts/EditorScripts.

**7.** Drag and drop the MapGenerator and MeshGenerator scripts to the Scripts Holder object.

**8.** In the Scripts Holder object, under the MeshGenerator script, once the child objects have a mesh filter, drag and drop the Walls and Outline game objects into the Walls and Cave variable spaces.

**9.** Now you should be able to click play and generate a map.


## Spawner Set Up

