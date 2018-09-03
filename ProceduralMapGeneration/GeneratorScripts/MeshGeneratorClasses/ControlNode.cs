using System;
using UnityEngine;

namespace PGM
{
    public class ControlNode : MapNode 
    {
        public bool _active;
        public MapNode _above, _right;

        public ControlNode(Vector3 pos, bool active, float squareSize) : base(pos)
        {
            _active = active;
            _above = new MapNode(_position + Vector3.forward * squareSize/2f);
            _right = new MapNode(_position + Vector3.right * squareSize/2f);
        }
    }
}