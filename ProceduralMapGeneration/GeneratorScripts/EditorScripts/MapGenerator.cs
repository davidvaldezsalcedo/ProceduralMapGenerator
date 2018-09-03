using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//NOTE: Enable for use with Navmesh Components
// using UnityEngine.AI;

namespace PGM
{
	public class MapGenerator : MonoBehaviour {

		[SerializeField]
		private int _Width = 150;
		[SerializeField]
		private int _Height = 100;
		[SerializeField]
		private int _SmoothPassMax = 0;
		[SerializeField]
		private int _PassageRadius = 3;
		[SerializeField]
		protected string _Seed;
		[SerializeField]
		private bool _UseRandomSeed = true;

		[SerializeField, Range(0, 100)]
		private int _RandomFillPercent = 55;

		private int[,] _Map;

		private void Awake()
		{
			GenerateMap();

			/*NOTE: Use this if Navmesh Components are being use 
			with included NavmeshBake script to bake Navmesh on Awake

			GetComponent<NavMeshSurface>().BuildNavMesh();		
			*/
		}

		private void Update()
		{
			// if(Input.GetMouseButtonDown(0))
			// {
			// 	GenerateMap();
			// }
		}

		private void GenerateMap()
		{
			_Map = new int[_Width, _Height];
			RandomFillMap();

			for(int i = 0; i < _SmoothPassMax; i++)
			{
				SmoothMap();
			}

			ProcessMap();

			int borderSize = 2;
			int[,] borderedMap = new int[_Width + borderSize * 2, _Height + borderSize * 2]; 

			for(int x = 0; x < borderedMap.GetLength(0); x++)
			{
				for(int y = 0; y < borderedMap.GetLength(1); y++)
				{
					if(x >= borderSize && x < _Width + borderSize && y >= borderSize && y < _Height + borderSize)
					{
						borderedMap[x, y] = _Map[x - borderSize, y - borderSize];
					}
					else
					{
						borderedMap[x, y] = 1;
					}
				}
			}

			MeshGenerator meshGen = GetComponent<MeshGenerator>();
			meshGen.GenerateMesh(borderedMap, 1);
		}
		//remove pillars and fill rooms
		private void ProcessMap()
		{
			List<List<Coord>> wallRegions = GetRegions(1);

			int wallThresholdSize = 50;
			foreach (List<Coord> wallRegion in wallRegions)
			{
				if(wallRegion.Count < wallThresholdSize)
				{
					foreach(Coord tile in wallRegion)
					{
						_Map[tile.TileX, tile.TileY] = 0;
					}
				}
			}

			List<List<Coord>> roomRegions = GetRegions(0);

			int roomThresholdSize = 50;

			List<Room> survivingRooms = new List<Room>();

			foreach (List<Coord> roomRegion in roomRegions)
			{
				if(roomRegion.Count < roomThresholdSize)
				{
					foreach(Coord tile in roomRegion)
					{
						_Map[tile.TileX, tile.TileY] = 1;
					}
				}
				else
				{
					survivingRooms.Add(new Room(roomRegion, _Map));
				}
			}

			survivingRooms.Sort();
			survivingRooms[0].IsMainRoom = true;
			survivingRooms[0].IsAccesibleFromMainRoom = true;

			ConnectClosestRooms(survivingRooms);

			//NOTE: Lines 121 to 126 are for spawning Players/Enemies/Pickups
			UEvents.Trigger_SpawnPlayerEvent(_Width, _Height, 0, survivingRooms);

			for(int i = 0; i < survivingRooms.Count; ++i)
			{
				UEvents.Trigger_SpawnGenericObjectEvent(_Width, _Height, UnityEngine.Random.Range(0, survivingRooms.Count), survivingRooms);			
			}
			//NOTE: END
		}

		private void ConnectClosestRooms(List<Room> allRooms, bool forceAccesibilityFromMainRoom = false)
		{
			List<Room> roomListA = new List<Room>();
			List<Room> roomListB = new List<Room>();

			if(forceAccesibilityFromMainRoom)
			{
				foreach (Room room in allRooms)
				{
					if(room.IsAccesibleFromMainRoom)
					{
						roomListB.Add(room);
					}
					else
					{
						roomListA.Add(room);
					}
				}
			}
			else
			{
				roomListA = allRooms;
				roomListB = allRooms;
			}

			int bestDistance = 0;
			Coord bestTileA = new Coord();
			Coord bestTileB = new Coord();
			Room bestRoomA = new Room();
			Room bestRoomB = new Room();
			bool possibleConnectionFound = false;

			foreach (Room roomA in roomListA)
			{
				if(!forceAccesibilityFromMainRoom)
				{
					possibleConnectionFound = false;

					if(roomA.ConnectedRooms.Count > 0)
					{
						continue;
					}
				}

				foreach (Room roomB in roomListB)
				{
					if(roomA == roomB || roomA.IsConnected(roomB))
					{
						continue;
					}

					for (int tileIndexA = 0; tileIndexA < roomA.EdgeTiles.Count; tileIndexA++)
					{
						for (int tileIndexB = 0; tileIndexB < roomB.EdgeTiles.Count; tileIndexB++)
						{
							Coord tileA = roomA.EdgeTiles[tileIndexA];
							Coord tileB = roomB.EdgeTiles[tileIndexB];
							int distanceBetweenRooms = (int)(Mathf.Pow(tileA.TileX - tileB.TileX, 2) + Mathf.Pow(tileA.TileY - tileB.TileY, 2));

							if(distanceBetweenRooms < bestDistance || !possibleConnectionFound)
							{
								bestDistance = distanceBetweenRooms;
								possibleConnectionFound = true;
								bestTileA = tileA;
								bestTileB = tileB;
								bestRoomA = roomA;
								bestRoomB = roomB;
							}
						}
					}
				}
				if(possibleConnectionFound && !forceAccesibilityFromMainRoom)
				{
					CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
				}
			}

			if(possibleConnectionFound && forceAccesibilityFromMainRoom)
			{
				CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
				ConnectClosestRooms(allRooms, true);
			}

			if(!forceAccesibilityFromMainRoom)
			{
				ConnectClosestRooms(allRooms, true);
			}
		}

		private void CreatePassage(Room roomA, Room roomB, Coord tileA, Coord tileB)
		{
			Room.ConnectRooms(roomA, roomB);
			//Debug.DrawLine(CoordToWorldPoint(tileA), CoordToWorldPoint(tileB), Color.green, 100);

			List<Coord> line = GetLine(tileA, tileB);
			foreach (Coord c in line)
			{
				DrawCircle(c, _PassageRadius);
			}
		}

		void DrawCircle(Coord c, int r)
		{
			for (int x = -r; x <= r; x++)
			{
				for (int y = -r; y <= r; y++)
				{
					int drawX = c.TileX + x;
					int drawY = c.TileY + y;
					if(IsInMapRange(drawX, drawY))
					{
						_Map[drawX, drawY] = 0;
					}
				}
				
			}
		}

		private List<Coord> GetLine(Coord from, Coord to)
		{
			List<Coord> line = new List<Coord>();

			int x = from.TileX;
			int y = from.TileY;

			int dx = to.TileX - from.TileX;
			int dy = to.TileY - from.TileY;

			bool inverted = false;
			int step = Math.Sign(dx);
			int gradientStep = Math.Sign(dy);

			int longest = Mathf.Abs(dx);
			int shortest = Mathf.Abs(dy);

			if(longest < shortest)
			{
				inverted = true;
				longest = Mathf.Abs(dy);
				shortest = Mathf.Abs(dx);

				step = Math.Sign(dy);
				gradientStep = Math.Sign(dx);
			}
			int gradientAccumulation = longest / 2;
			for(int i = 0; i < longest; i++)
			{
				line.Add(new Coord(x, y));

				if(inverted)
				{
					y += step;
				}
				else
				{
					x += step;
				}

				gradientAccumulation += shortest;
				if(gradientAccumulation >= longest)
				{
					if(inverted)
					{
						x += gradientStep;
					}
					else
					{
						y += gradientStep;
					}
					gradientAccumulation -= longest;
				}
			}
			return line;
		}

		private Vector3 CoordToWorldPoint(Coord tile)
		{
			return new Vector3(-_Width / 2 + 0.5f + tile.TileX, 2, -_Height / 2 + 0.5f + tile.TileY);
		}

		private List<List<Coord>> GetRegions(int TileType)
		{
			List<List<Coord>> regions = new List<List<Coord>>();
			int[,] mapFlags = new int[_Width, _Height];

			for(int x = 0; x < _Width; x++)
			{
				for(int y = 0; y < _Height; y++)
				{
					if(mapFlags[x, y] == 0 && _Map[x, y] == TileType)
					{
						List<Coord> newRegion = GetRegionTiles(x, y);
						regions.Add(newRegion);

						foreach (Coord tile in newRegion)
						{
							mapFlags[tile.TileX, tile.TileY] = 1;
						}
					}
				}
			}
			return regions;
		}

		private List<Coord> GetRegionTiles(int startX, int startY)
		{
			List<Coord> tiles = new List<Coord>();
			int[,] mapFlags = new int[_Width, _Height];
			int tileType = _Map[startX, startY];

			Queue<Coord>  queue = new Queue<Coord>();
			queue.Enqueue(new Coord(startX, startY));
			mapFlags[startX, startY] = 1;

			while(queue.Count > 0)
			{
				Coord tile = queue.Dequeue();
				tiles.Add(tile);

				for(int x = tile.TileX - 1; x <= tile.TileX + 1; x++)
				{
					for(int y = tile.TileY - 1; y <= tile.TileY + 1; y++)
					{
						if(IsInMapRange(x, y) && (y == tile.TileY || x == tile.TileX))
						{
							if(mapFlags[x, y] == 0 && _Map[x, y] == tileType)
							{
								mapFlags[x, y] = 1;
								queue.Enqueue(new Coord(x, y));
							}
						}
					}
				}
			}
			return tiles;
		}

		private bool IsInMapRange(int x, int y)
		{
			return x >= 0 && x < _Width && y >= 0 && y < _Height;
		}

		private void RandomFillMap()
		{
			if(_UseRandomSeed)
			{
				_Seed = Time.time.ToString();
			}
			
			System.Random randNum = new System.Random(_Seed.GetHashCode());

			for(int x = 0; x < _Width; x++)
			{
				for(int y = 0; y < _Height; y++)
				{
					if(x == 0 || x == _Width - 1 || y == 0 || y == _Height - 1)
					{
						_Map[x,y] = 1;
					}
					else {
						_Map[x,y] = (randNum.Next(0, 100) < _RandomFillPercent)? 1: 0;
					}	
				}

			}
		}

		private void SmoothMap()
		{
			for(int x = 0; x < _Width; x++)
			{
				for(int y = 0; y < _Height; y++)
				{
					int neighbourWallTiles = GetSurroundingWallCount(x, y);

					if(neighbourWallTiles > 4)
					{
						_Map[x, y] = 1;
					}
					else if(neighbourWallTiles < 4)
					{
						_Map[x, y] = 0;
					}
				}
			}
		}
		
		private int GetSurroundingWallCount(int gridX, int gridY)
		{
			int wallCount = 0;
			for(int neighbourX = gridX - 1; neighbourX <= gridX +1; neighbourX++)
			{
				for(int neighbourY = gridY - 1; neighbourY <= gridY +1; neighbourY++)
				{
					if(IsInMapRange(neighbourX, neighbourY))
					{
						if(neighbourX != gridX || neighbourY != gridY)
						{
							wallCount += _Map[neighbourX, neighbourY];
						}
					}
					else{
						wallCount++;
					}
				}
			}
			return wallCount;

		}
	}
}