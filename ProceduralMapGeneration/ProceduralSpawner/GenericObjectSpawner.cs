using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using PGM;

public class GenericObjectSpawner : MonoBehaviour, IGetCoordToSpawnObject, ISpawnObjectAtCoord
{
    [SerializeField, Tooltip("Can be used for Enemy spawning and PickUps")]
    private GameObject _GenericObject;
    [SerializeField, Tooltip("How many objects per room")]
    private int _ObjectsPerRoom = 5;

    private void OnEnable()
    {        
		UEvents.SpawGenericObjectEvent += GetCoords;
    }

    public void GetCoords(int width, int height, int roomIndex, List<Room> survivingRooms)
    {
        List<Coord> spawnCoord = new List<Coord>();

        foreach (var c in survivingRooms[roomIndex].Tiles)
        {
           if (!survivingRooms[roomIndex].EdgeTiles.Contains(c))
           {
               spawnCoord.Add(c);
           }
        }
        SpawnAtCoord(width, height, roomIndex, spawnCoord);
    }

    public void SpawnAtCoord(int width, int height, int roomIndex, List<Coord> spawnCoord)
    {
        for(int i = 0; i < _ObjectsPerRoom; ++i)
        {
           int rand = UnityEngine.Random.Range(0, spawnCoord.Count);

           Vector3 pos = new Vector3(-width / 2 + spawnCoord[rand].TileX, -5f, -height / 2 + spawnCoord[rand].TileY);
           Instantiate(_GenericObject, pos, Quaternion.identity);
        }
    }
}
