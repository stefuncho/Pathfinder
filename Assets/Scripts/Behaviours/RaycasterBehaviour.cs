using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Behaviours
{
    public class RaycasterBehaviour : MonoBehaviour
    {
        public event Action<SegmentBehaviour> SegmentSelected;
        public event Action SelectEnded;
        
        public SegmentBehaviour HighlightedSegment { get; private set; }
        
        protected void Awake()
        {
            _camera = GetComponent<Camera>();
            _selectReference.action.started += OnSelectStarted;
            _selectReference.action.canceled += OnSelectEnded;
            //_selectReference.action.performed += OnSelectEnded;
        }

        protected void OnDestroy()
        {
            if (_selectReference == null) 
                return;
            
            _selectReference.action.started -= OnSelectStarted;
            _selectReference.action.canceled -= OnSelectEnded;
            _selectReference.action.performed -= OnSelectEnded;
        }

        protected void Update()
        {
            RaycastHit hit;
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            
            SegmentBehaviour segment = null;
        
            if (Physics.Raycast(ray, out hit)) {
                var objectHit = hit.transform;

                _segments.TryGetValue(objectHit.GetInstanceID(), out segment);
                if (segment == null)
                {
                    segment = objectHit.GetComponent<SegmentBehaviour>();
                    _segments.TryAdd(objectHit.GetInstanceID(), segment);
                }
            }

            SetHighlight(segment);
        }
        
        private readonly Dictionary<int, SegmentBehaviour> _segments = new();
        
        [SerializeField] private InputActionReference _selectReference = default;
        
        private Camera _camera;

        private void SetHighlight(SegmentBehaviour segment)
        {
            if (HighlightedSegment == segment)
                return;
            
            HighlightedSegment?.SetHighlight(false);
            HighlightedSegment = segment;
            HighlightedSegment?.SetHighlight(true);

            if (_selectReference.action.inProgress)
                TrySelect();
        }

        private void TrySelect()
        {
            if (HighlightedSegment == null)
                return;

            SegmentSelected?.Invoke(HighlightedSegment);
        }

        private void OnSelectStarted(InputAction.CallbackContext _) => TrySelect();
        private void OnSelectEnded(InputAction.CallbackContext _) => SelectEnded?.Invoke();
    }
}