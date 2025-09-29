using System.Collections.Generic;
using Model;
using UnityEngine;

public class SegmentBehaviour : MonoBehaviour
{
    public void Init(Segment segment)
    {
        for (int i = 0; i < _visuals.Count; i++)
        {
            _visuals[i].SetActive(i == (int)segment.Type);
        }
    }
    
    [SerializeField] private List<GameObject> _visuals = new();
}
