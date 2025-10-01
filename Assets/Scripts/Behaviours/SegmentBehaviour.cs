using System;
using System.Collections.Generic;
using System.ComponentModel;
using Model;
using UnityEngine;

public class SegmentBehaviour : MonoBehaviour
{
    public Vector2Int Position => _segment?.Position ?? Vector2Int.zero;
    public Segment.SegmentType Type => _segment?.Type ?? default;
    
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
        _isHighlighted = tf;
        UpdateColor();
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
    private bool _isHighlighted;

    private void UpdateColor()
    {
        var color = (_isHighlighted, _segment.Color) switch
        {
            (true, _) => _highlightedColor,
            (false, Segment.ColorType.InRange) => _inRangeColor,
            (false, Segment.ColorType.OutOfRange) => _outRangeColor,
            _ => _normalColor,
        };
        
        var renderers = transform.GetComponentsInChildren<Renderer>(includeInactive: true);

        foreach (var renderer in renderers)
        {
            renderer.material.SetColor("_BaseColor", color);
        }
    }

    private void UpdateType()
    {
        for (int i = 0; i < _visuals.Count; i++)
        {
            _visuals[i].SetActive(i == (int)_segment.Type);
        }
    }

    private void OnSegmentDisposed()
    {
        _segment.Disposed -= OnSegmentDisposed;
        _segment.PropertyChanged -= OnSegmentPropertyChanged;
        Destroy(gameObject);
    }

    private void OnSegmentPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_segment.Type))
            UpdateType();
        
        if (e.PropertyName == nameof(_segment.Color))
            UpdateColor();
    }
}
