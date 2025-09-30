using System;
using System.Collections.Generic;
using System.ComponentModel;
using Model;
using UnityEngine;
using UnityEngine.Rendering;

public class SegmentBehaviour : MonoBehaviour
{
    public void Init(Segment segment)
    {
        if (_segment != null)
            throw new ArgumentException("Segment already initialized.");
        
        _segment = segment;
        _segment.Disposed += OnSegmentDisposed;
        _segment.PropertyChanged += OnSegmentPropertyChanged;
        
        UpdateType();
        SetHighlight(false);
    }

    public void SetHighlight(bool tf)
    {
        var renderers = transform.GetComponentsInChildren<Renderer>(includeInactive: true);

        foreach (var renderer in renderers)
        {
            renderer.material.SetColor("_BaseColor", tf ? _highlightedColor : _normalColor);
        }
    }

    public void ToggleSegmentType()
        => _segment.Toggle();

    protected void OnDestroy()
    {
        if (_segment == null)
            return;
        
        _segment.Disposed -= OnSegmentDisposed;
        _segment.PropertyChanged -= OnSegmentPropertyChanged;
    }
    
    [SerializeField] private Color _normalColor = new();
    [SerializeField] private Color _highlightedColor = new();
    [SerializeField] private Color _inRangeColor = new();
    [SerializeField] private Color _outRangeColor = new();
    
    [SerializeField] private List<GameObject> _visuals = new();

    private Segment _segment;

    private void OnSegmentDisposed()
    {
        _segment.Disposed -= OnSegmentDisposed;
        _segment.PropertyChanged -= OnSegmentPropertyChanged;
        Destroy(gameObject);
    }

    private void UpdateColor()
    {
        
    }

    private void UpdateType()
    {
        for (int i = 0; i < _visuals.Count; i++)
        {
            _visuals[i].SetActive(i == (int)_segment.Type);
        }
    }

    private void OnSegmentPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_segment.Type))
            UpdateType();
    }
}
