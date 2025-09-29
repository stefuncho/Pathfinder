namespace Model
{
    public class Segment
    {
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
    }
}