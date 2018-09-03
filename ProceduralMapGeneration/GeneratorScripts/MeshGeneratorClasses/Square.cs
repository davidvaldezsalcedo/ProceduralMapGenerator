using System;

namespace PGM
{
    public class Square
    {
        public int _Configuration;
        public ControlNode _topLeft, _topRight, _bottomRight, _bottomLeft;
        public MapNode _centerTop, _centerRight, _centerBottom, _centerLeft;
        

        public Square(ControlNode topLeft, ControlNode topRight, ControlNode bottomRight, ControlNode bottomLeft)
        {
            _topLeft = topLeft;
            _topRight = topRight;
            _bottomRight = bottomRight;
            _bottomLeft = bottomLeft;

            _centerTop = _topLeft._right;
            _centerRight = _bottomRight._above;
            _centerBottom = _bottomLeft._right;
            _centerLeft = _bottomLeft._above;

            if(_topLeft._active)
                _Configuration += 8;
            
            if(_topRight._active)
                _Configuration += 4;
            
            if(_bottomRight._active)
                _Configuration += 2;
            
            if(_bottomLeft._active)
                _Configuration += 1;
            
        }
        
        
    }
}