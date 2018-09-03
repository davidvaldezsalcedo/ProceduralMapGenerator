using UnityEngine.Events;
using UnityEngine;
using System.Collections.Generic;
using System;
using PGM;

public static class UEvents 
{
	public static event Action<int, int, int, List<Room>> SpawnPlayerEvent = delegate { };
	public static void Trigger_SpawnPlayerEvent(int width, int height, int roomIndex, List<Room> survivingRooms)
	{
		SpawnPlayerEvent.Invoke(width, height, roomIndex, survivingRooms);
	}


	public static event Action<int, int, int, List<Room>> SpawGenericObjectEvent = delegate { };
	public static void Trigger_SpawnGenericObjectEvent(int width, int height, int roomIndex, List<Room> survivingRooms)
	{
		SpawGenericObjectEvent.Invoke(width, height, roomIndex, survivingRooms);
	}
}
