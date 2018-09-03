using System.Collections.Generic;
using PGM;

public interface  IGetCoordToSpawnObject
{
    void GetCoords(int width, int height, int roomIndex, List<Room> survivingRooms);
}
