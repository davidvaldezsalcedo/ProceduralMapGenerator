using System;
using System.Collections.Generic;

namespace PGM
{
	public class Room : IComparable<Room>
	{
		public List<Coord> Tiles;
		public List<Coord> EdgeTiles;
		public List<Room> ConnectedRooms;
		public int RoomSize;
		public bool IsAccesibleFromMainRoom;
		public bool IsMainRoom;

		public Room(){}

		public Room(List<Coord> roomTiles, int[,] map)
		{
			Tiles = roomTiles;
			RoomSize = Tiles.Count;
			ConnectedRooms = new List<Room>();

			EdgeTiles = new List<Coord>();
			foreach (Coord tile in Tiles)
			{
				for(int x = tile.TileX - 1; x <= tile.TileX + 1; x++)
				{
					for(int y = tile.TileY - 1; y <= tile.TileY + 1; y++)
					{
						if(x == tile.TileX || y == tile.TileY )
						{
							if(map[x, y] == 1)
							{
								EdgeTiles.Add(tile);
							}
						}
					}
				}
			}
		}

	private void SetAccesibleFromMainRoom()
		{
			if(!IsAccesibleFromMainRoom)
			{
				IsAccesibleFromMainRoom = true;
				foreach (Room connectedRoom in ConnectedRooms)
				{
					connectedRoom.IsAccesibleFromMainRoom = true;
				}
			}
		}

		public static void ConnectRooms(Room roomA, Room roomB)
		{
			if(roomA.IsAccesibleFromMainRoom)
			{
				roomB.SetAccesibleFromMainRoom();
			}
			else if(roomB.IsAccesibleFromMainRoom)
			{
				roomA.SetAccesibleFromMainRoom();
			}

			roomA.ConnectedRooms.Add(roomB);
			roomB.ConnectedRooms.Add(roomA);
		}

		public bool IsConnected(Room otherRoom)
		{
			return ConnectedRooms.Contains(otherRoom);
		}

		public int CompareTo(Room otherRoom)
		{
			return otherRoom.RoomSize.CompareTo(RoomSize);
		}
	}
}