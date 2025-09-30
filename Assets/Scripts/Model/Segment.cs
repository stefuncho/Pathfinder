using System;

namespace Model
{
    public class Segment : IDisposable
    {
        public event Action Disposed;
        
        public enum SegmentType
        {
            Floor = 0,
            Obstacle,
            Wall,
        }
        
        public SegmentType Type { get; private set; }

        public Segment(SegmentType type)
        {
            Type = type;
        }

        public void Dispose()
        {
            Disposed?.Invoke();
        }
    }
}