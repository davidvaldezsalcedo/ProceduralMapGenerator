using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PGM
{

	public class MeshGenerator : MonoBehaviour {

		public SquareGrid SquareGrid;
		public MeshFilter Walls;
		[SerializeField]
		private MeshFilter _Cave;
		[SerializeField]
		private bool _Is2D;
		[SerializeField]
		private int _CaveTileAmount = 10;
		[SerializeField]
		private int _WallTileAmount = 5;

		private List<Vector3> _Vertices;
		private List<int> _Triangles;

		private Dictionary<int, List<MapTriangle>> _TriangleDictionary = new Dictionary<int, List<MapTriangle>>();
		private List<List<int>> _Outlines = new List<List<int>>();
		private HashSet<int> _CheckVertices = new HashSet<int>();

		public void GenerateMesh(int[,] map, float squareSize)
		{
			_TriangleDictionary.Clear();
			_Outlines.Clear();
			_CheckVertices.Clear();

			SquareGrid = new SquareGrid(map, squareSize);

			_Vertices = new List<Vector3>();
			_Triangles = new List<int>();

			for(int x = 0; x < SquareGrid.squares.GetLength(0); x++)
			{
				for(int y = 0; y < SquareGrid.squares.GetLength(1); y++)
				{
					TriangualteSquare(SquareGrid.squares[x, y]);
				}
			}

			Mesh mesh = new Mesh();
			mesh = _Cave.mesh;
			mesh.Clear();

			mesh.SetVertices(_Vertices);
			mesh.SetTriangles(_Triangles, 0);
			//mesh.vertices = _Vertices.ToArray();
			//mesh.triangles = _Triangles.ToArray();
			mesh.RecalculateNormals();

			Vector2[] uvs = new Vector2[_Vertices.Count];
			for (int i = 0; i < _Vertices.Count; i++)
			{
				float percentX = Mathf.InverseLerp(-map.GetLength(0) / 2 * squareSize, map.GetLength(0) / 2 * squareSize, _Vertices[i].x) * _CaveTileAmount;
				float percentY = Mathf.InverseLerp(-map.GetLength(0) / 2 * squareSize, map.GetLength(0) / 2 * squareSize, _Vertices[i].z) * _CaveTileAmount;
				uvs[i] = new Vector2(percentX, percentY);
			}
			mesh.uv = uvs;

			if(_Is2D)
			{
				Generate2DColliders();
			}
			else
			{
				CreateWallMesh(map, squareSize);
			}
		}

		private void Generate2DColliders()
		{
			EdgeCollider2D[] currentColliders = gameObject.GetComponents<EdgeCollider2D>();

			for (int i = 0; i < currentColliders.Length; i++)
			{
				Destroy(currentColliders[i]);
			}

			CalculateMeshOutlines();

			foreach (List<int> Outline in _Outlines)
			{
				EdgeCollider2D edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
				Vector2[] edgePoints = new Vector2[Outline.Count];

				for (int i = 0; i < Outline.Count; i++)
				{
					edgePoints[i] = new Vector2(_Vertices[Outline[i]].x, _Vertices[Outline[i]].z);
				}
				edgeCollider.points = edgePoints;
			}
		}

		private void CreateWallMesh(int[,] map, float squareSize)
		{
			CalculateMeshOutlines();

			List<Vector3> wallVertices = new List<Vector3>();
			List<int> wallTriangles = new List<int>();
			Mesh wallMesh = new Mesh();
			float wallHeight = 5;

			foreach(List<int> outline in _Outlines)
			{
				for (int i = 0; i < outline.Count - 1; i++)
				{
					int startIndex = wallVertices.Count;
					wallVertices.Add(_Vertices[outline[i]]);			 //Left
					wallVertices.Add(_Vertices[outline[i + 1]]);		//Right
					wallVertices.Add(_Vertices[outline[i]] - Vector3.up * wallHeight);			//Bottom Left
					wallVertices.Add(_Vertices[outline[i + 1]] - Vector3.up * wallHeight);		//Bottom Right

					wallTriangles.Add(startIndex + 0);
					wallTriangles.Add(startIndex + 2);
					wallTriangles.Add(startIndex + 3);

					wallTriangles.Add(startIndex + 3);
					wallTriangles.Add(startIndex + 1);
					wallTriangles.Add(startIndex + 0);
				}
			}
			wallMesh.vertices = wallVertices.ToArray();
			wallMesh.triangles = wallTriangles.ToArray();
			Walls.mesh = wallMesh;

			MeshCollider currentCollider = GetComponentInChildren<MeshCollider>();
			Destroy(currentCollider);

			MeshCollider wallCollider = Walls.gameObject.AddComponent<MeshCollider>();
			wallCollider.sharedMesh = wallMesh;

			Vector2[] uvs = new Vector2[wallVertices.Count];
			for (int i = 0; i < wallVertices.Count; i++)
			{
				float percentX = Mathf.InverseLerp(-map.GetLength(0) / 2 * squareSize, map.GetLength(0) / 2 * squareSize, wallVertices[i].x) * _WallTileAmount;
				float percentY = Mathf.InverseLerp(-map.GetLength(0) / 2 * squareSize, map.GetLength(0) / 2 * squareSize, wallVertices[i].y) * _WallTileAmount;
				
				uvs[i] = new Vector3(percentX, percentY);
			}
			wallMesh.uv = uvs;

		}

		private void TriangualteSquare(Square square)
		{
			switch(square._Configuration)
			{
			case 0:
				break;
				
			//1points:
			case 1:
				MeshFromPoints(square._centerLeft, square._centerBottom, square._bottomLeft);
				break;
			case 2:
				MeshFromPoints(square._bottomRight, square._centerBottom, square._centerRight);
				break;
			case 4:
				MeshFromPoints(square._topRight, square._centerRight, square._centerTop);
				break;			
			case 8:
				MeshFromPoints(square._topLeft, square._centerTop, square._centerLeft);
				break;

			//2point:
			case 3:
				MeshFromPoints(square._centerRight, square._bottomRight, square._bottomLeft, square._centerLeft);
				break;
			case 6:
				MeshFromPoints(square._centerTop, square._topRight, square._bottomRight, square._centerBottom);
				break;
			case 9:
				MeshFromPoints(square._topLeft, square._centerTop, square._centerBottom, square._bottomLeft);
				break;
			case 12:
				MeshFromPoints(square._topLeft, square._topRight, square._centerRight, square._centerLeft);
				break;
			case 5:
				MeshFromPoints(square._centerTop, square._topRight, square._centerRight, square._centerBottom, square._bottomLeft, square._centerLeft);
				break;
			case 10:
				MeshFromPoints(square._topLeft, square._centerTop, square._centerRight, square._bottomRight, square._centerBottom, square._centerLeft);
				break;

			//3 ponts:
			case 7:
				MeshFromPoints(square._centerTop, square._topRight, square._bottomRight, square._bottomLeft, square._centerLeft);
				break;
			case 11:
				MeshFromPoints(square._topLeft, square._centerTop, square._centerRight, square._bottomRight, square._bottomLeft);
				break;
			case 13:
				MeshFromPoints(square._topLeft, square._topRight, square._centerRight, square._centerBottom, square._bottomLeft);
				break;
			case 14:
				MeshFromPoints(square._topLeft, square._topRight, square._bottomRight, square._centerBottom, square._centerLeft);
				break;

			//4 points:
			case 15:
				MeshFromPoints(square._topLeft, square._topRight, square._bottomRight, square._bottomLeft);
				_CheckVertices.Add(square._topLeft._VertexIndex);
				_CheckVertices.Add(square._topRight._VertexIndex);
				_CheckVertices.Add(square._bottomRight._VertexIndex);
				_CheckVertices.Add(square._bottomLeft._VertexIndex);
				break;
			}			
		}

		private void MeshFromPoints(params MapNode[] points)
		{
			AssignVertices(points);

			if(points.Length >= 3)
				CreateTriangles(points[0], points[1], points[2]);
			if(points.Length >= 4)
				CreateTriangles(points[0], points[2], points[3]);
			if(points.Length >= 5)
				CreateTriangles(points[0], points[3], points[4]);
			if(points.Length >= 6)
				CreateTriangles(points[0], points[4], points[5]);		
		}

		private void AssignVertices(MapNode[] points)
		{
			for(int i = 0; i < points.Length; i++)
			{
				if(points[i]._VertexIndex == -1)
				{
					points[i]._VertexIndex = _Vertices.Count; 
					_Vertices.Add(points[i]._position);
				}
			}
		}

		void CreateTriangles(MapNode a, MapNode b, MapNode c)
		{
			_Triangles.Add(a._VertexIndex);
			_Triangles.Add(b._VertexIndex);
			_Triangles.Add(c._VertexIndex);

			MapTriangle triangle = new MapTriangle(a._VertexIndex, b._VertexIndex, c._VertexIndex);

			AddTriangleToDictionary(triangle.vertexIndexA, triangle);
			AddTriangleToDictionary(triangle.vertexIndexB, triangle);
			AddTriangleToDictionary(triangle.vertexIndexC, triangle);
		}

		private int GetConnectedOutlineVertext(int vertexIndex)
		{
			List<MapTriangle> triangelsContainingVertex = _TriangleDictionary[vertexIndex];

			for (int i = 0; i < triangelsContainingVertex.Count; i++)
			{
				MapTriangle triangle = triangelsContainingVertex[i];

				for (int j = 0; j < 3; j++)
				{
					int vertexB = triangle[j];

					if(vertexB != vertexIndex && !_CheckVertices.Contains(vertexB))
					{
						if(IsOutlineEdge(vertexIndex, vertexB))
						{
							return vertexB;
						}
					}
				}
			}
			return -1;
		}

		private bool IsOutlineEdge(int vertexA, int vertexB)
		{
			List<MapTriangle> trianglesContainingVertexA = _TriangleDictionary[vertexA];
			int sharedTriangleCount = 0;

			for (int i = 0; i < trianglesContainingVertexA.Count; i++)
			{
				if(trianglesContainingVertexA[i].Contains(vertexB))
				{
					sharedTriangleCount++;
					if(sharedTriangleCount > 1)
					{
						break;
					}
				}
			}
			return sharedTriangleCount == 1;
		}

		void AddTriangleToDictionary(int vertexIndexKey, MapTriangle triangle)
		{
			if(_TriangleDictionary.ContainsKey(vertexIndexKey))
			{
				_TriangleDictionary[vertexIndexKey].Add(triangle);
			}
			else
			{
				List<MapTriangle> triangleList = new List<MapTriangle>();
				triangleList.Add(triangle);
				_TriangleDictionary.Add(vertexIndexKey, triangleList);
			}
		}

		private void CalculateMeshOutlines()
		{
			for(int vertexIndex = 0; vertexIndex < _Vertices.Count; vertexIndex++)
			{
				if(!_CheckVertices.Contains(vertexIndex))
				{
					int newOutlineVertex = GetConnectedOutlineVertext(vertexIndex);
					if(newOutlineVertex != -1)
					{
						_CheckVertices.Add(vertexIndex);

						List<int> newOutline = new List<int>();
						newOutline.Add(vertexIndex);
						_Outlines.Add(newOutline);
						FollowOutline(newOutlineVertex, _Outlines.Count - 1);
						_Outlines[_Outlines.Count - 1].Add(vertexIndex);
					}
				}
			}
		}

		private void FollowOutline(int vertexIndex, int outlineIndex)
		{
			_Outlines[outlineIndex].Add(vertexIndex);
			_CheckVertices.Add(vertexIndex);
			int nextVertexIndex = GetConnectedOutlineVertext(vertexIndex);

			if(nextVertexIndex != -1)
			{
				FollowOutline(nextVertexIndex, outlineIndex);
			}
		}
	}
}	