namespace Model
{
    public class Map
    {
        public Map(int width, int height)
        {
            _map = new Segment[width, height];
            
            for (int i = 0; i < _map.GetLength(0); i++)
                for (int j = 0; j < _map.GetLength(1); j++)
                    _map[i, j] = new Segment(
                        (i == 0 || j == 0 || i == width - 1 || j == height - 1)
                            ? Segment.SegmentType.Wall
                            : Segment.SegmentType.Floor);
        }

        private Segment[,] _map = null;
    }
}