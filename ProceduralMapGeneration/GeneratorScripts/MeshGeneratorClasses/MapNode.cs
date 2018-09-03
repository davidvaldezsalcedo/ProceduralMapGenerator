using System;
using UnityEngine;

namespace PGM
{
    public class MapNode
    {
        public Vector3 _position;
        public int _VertexIndex = -1;

        public MapNode(Vector3 _pos)
        {
            _position = _pos;
        }	
        
    }
}