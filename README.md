# ProceduralMapGenerator
It creates a procedural map and mesh for dungeon crawlers. Included is a procedural spawner for players, enemies, and Pickups as well as a navmesh bake on awake script.


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

**1.** Create two empty game objects, one for the Player Spawner and one for the Enemy Spawner (**NOTE:** other empty game objects can be added for use to spawn pick ups or other objects)

**2.** Drag the PlayerSpawner script located in /ProceduralMapGeneration/ProceduralSpawner and drop it into the Player Spawner object and give it a Player prefab

**3.** Drag the GenericObjectSpawner script located in /ProceduralMapGeneration/ProceduralSpawner and drop it into the Enemy Spawner object and give it an Enemy prefab (this also applies to any other enemies or pick ups created).

**4.** you should be able to spawn objects now.


## Tweaking Settings

You can use this as a 2d or 3d map generator by ticking on the Is 2D bool variable in the inspector for the Mesh Generator script.

You can set the size of the map by setting its Width and Height in the inspector

The Smooth Pass Max changes how smooth the map will be, if set to 0 it will contain pillars in halls and edges

The passage radius will define how thick the halls will be

The Seed is what defines the maps outline, this is done by strings, so defining it as "Hello World" will give you a different map as "Hello Everyone"

You can use the Use Random Seed to have it select a random map for you

Random Fill Percent will set how empty or how full the map is

Wall and Cave tile amounts define the tiling of the textures used


## Aditional Notes

**You can enable using a navmesh by adding the Navmesh Components for your Unity version and uncommenting it in the Map Generator Script in the Awake function and uncommenting the using UnityEngine.AI in the script.**

**You can then add the Navmesh Bake script located in the /ProceduralMapGeneration folder to the Map Generator object and give it a navmesh surface component to bake on awake**

**NOTE: if you would not like to use this spawner, you can comment out or delete lines 121 to 126 in the MapGenerator script**


### Known Bugs

The UVs on the Z axis for the textures seem to be streched, this still needs to be fixed.



