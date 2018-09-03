using System.Collections.Generic;
using PGM;

public interface  ISpawnObjectAtCoord
{
    void SpawnAtCoord(int width, int height, int roomIndex, List<Coord> spawnCoord);
}
