using System.Collections.Generic;
using UnityEngine;
using PGM;

public class PlayerSpawner : MonoBehaviour, IGetCoordToSpawnObject, ISpawnObjectAtCoord
{
    [SerializeField]
    private GameObject _Player;

    private GameObject _PlayerClone;
    private bool _PlayerSpawned;

    private void OnEnable()
    {        
		UEvents.SpawnPlayerEvent += GetCoords;
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
        int rand = UnityEngine.Random.Range(0, spawnCoord.Count);

       Vector3 pos = new Vector3(-width / 2 + spawnCoord[rand].TileX, -4f, -height / 2 + spawnCoord[rand].TileY);

       if (_PlayerSpawned == false)
       {
           _PlayerClone = Instantiate(_Player, pos, Quaternion.identity);

           _PlayerSpawned = true;
       }
       else
       {
           _PlayerClone.transform.position = pos;
       }
    }
}
