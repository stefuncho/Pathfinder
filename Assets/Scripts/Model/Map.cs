using System;
using UnityEngine;

namespace Model
{
    public class Map : IDisposable
    {
        public int Width => _map.GetLength(0);
        public int Length => _map.GetLength(1);
        
        public Segment GetSegment(int x, int y) => _map[x, y];
        
        public Map(int width, int length)
        {
            _map = new Segment[width, length];
            
            for (int i = 0; i < _map.GetLength(0); i++)
                for (int j = 0; j < _map.GetLength(1); j++)
                    _map[i, j] = new Segment(
                        (i == 0 || j == 0 || i == width - 1 || j == length - 1)
                            ? Segment.SegmentType.Wall
                            : Segment.SegmentType.Floor,
                        new Vector2Int(i, j));
        }

        public void Dispose()
        {
            for (int i = 0; i < _map.GetLength(0); i++)
                for (int j = 0; j < _map.GetLength(1); j++)
                    _map[i, j].Dispose();
            
            _map = null;
        }

        private Segment[,] _map = null;
    }
}