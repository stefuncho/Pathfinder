using System.Collections.Generic;
using System.Linq;
using EpPathFinding.cs;
using UnityEngine;

namespace Model
{
    public static class MapPathfinder
    {
        public enum PathType
        {
            Move,
            Attack,
        }
        
        public static IReadOnlyList<Segment> FindPath(Map map, PathType type,
            Vector2Int start, Vector2Int end)
        {
            BaseGrid searchGrid = new StaticGrid(map.Width, map.Length);

            var jumpParam = new JumpPointParam(searchGrid, EndNodeUnWalkableTreatment.DISALLOW, DiagonalMovement.Never, HeuristicMode.MANHATTAN)
                {
                    CurIterationType = IterationType.RECURSIVE
                };

            var startPos = new GridPos(start.x, start.y);
            var endPos = new GridPos(end.x, end.y);
            
            for (int i = 0; i < map.Width; i++)
            {
                for (int j = 0; j < map.Length; j++)
                {
                    var isWalkable = (map.GetSegment(i, j).Type, type) switch
                    {
                        (Segment.SegmentType.Wall, _) => false,
                        (Segment.SegmentType.Obstacle, PathType.Move) => false,
                        _ => true
                    };
                    
                    searchGrid.SetWalkableAt(new GridPos(i, j), isWalkable);
                }
            }
            
            jumpParam.Reset(startPos, endPos);
            return JumpPointFinder.GetFullPath(JumpPointFinder.FindPath(jumpParam))
                .ConvertAll(gridPos => map.GetSegment(gridPos.x, gridPos.y));
        }
    }
}