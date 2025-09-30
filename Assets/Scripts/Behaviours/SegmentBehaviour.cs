using System;
using System.Collections.Generic;
using Model;
using UnityEngine;

public class SegmentBehaviour : MonoBehaviour
{
    public void Init(Segment segment)
    {
        if (_segment != null)
            throw new ArgumentException("Segment already initialized.");
        
        _segment = segment;
        _segment.Disposed += OnSegmentDisposed;
        
        for (int i = 0; i < _visuals.Count; i++)
        {
            _visuals[i].SetActive(i == (int)segment.Type);
        }
    }

    protected void OnDestroy()
    {
        if (_segment != null)
            _segment.Disposed -= OnSegmentDisposed;
    }
    
    [SerializeField] private List<GameObject> _visuals = new();

    private Segment _segment;

    private void OnSegmentDisposed()
    {
        _segment.Disposed -= OnSegmentDisposed;
        Destroy(gameObject);
    }
}
