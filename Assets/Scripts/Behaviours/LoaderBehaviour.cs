using System;
using Model;
using Services;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Behaviours
{
    public class LoaderBehaviour : MonoBehaviour
    {
        protected void Start()
        {
            _toggleEditModeReference.action.performed += OnToggleEditPerformed;
            GenerateNewMap();
        }

        protected void OnDestroy()
        {
            if (_toggleEditModeReference != null)
            {
                _toggleEditModeReference.action.performed -= OnToggleEditPerformed;
            }
        }

        protected void Update()
        {
            if (_newMapRequested)
            {
                _newMapRequested = false;
                GenerateNewMap();
            }
        }

        protected void OnGUI()
        {
            if (_isInEditMode)
            {
                GUILayout.Label("EDIT MODE");
                
                GUILayout.Label("Move Range");
                uint.TryParse(GUILayout.TextField(_config.MoveRange.ToString()), out _config.MoveRange);
                GUILayout.Label("Attack Range");
                uint.TryParse(GUILayout.TextField(_config.AttackRange.ToString()), out _config.AttackRange);
                
                using (new GUILayout.VerticalScope("box"))
                {
                    GUILayout.Label("Map Width:");
                    uint.TryParse(GUILayout.TextField(_config.MapWidth.ToString()), out _config.MapWidth);
                    GUILayout.Label("Map Length:");
                    uint.TryParse(GUILayout.TextField(_config.MapLength.ToString()), out _config.MapLength);

                    if (GUILayout.Button("Generate Map"))
                        _newMapRequested = true;
                }
            }
            else
            {
                GUILayout.Label("Press F2 for Camera Mode");
            }
        }

        private const float SegmentSize = 1f;
        
        private readonly Config _config = new();
        
        [SerializeField] private CameraBehaviour _cameraBehaviour = default;

        [SerializeField] private GameObject _segmentPrefab = default;
        
        [SerializeField] private InputActionReference _toggleEditModeReference = default;
        
        private bool _isInEditMode = false;
        private Map _map;
        private bool _newMapRequested;

        private ICameraService CameraService => _cameraBehaviour;

        private void GenerateNewMap()
        {
            _map?.Dispose();
            _map = new Map((int)_config.MapWidth, (int)_config.MapLength);

            for (int i = 0; i < _map.Width; i++)
            {
                for (int j = 0; j < _map.Length; j++)
                {
                    var instance = Instantiate(_segmentPrefab, 
                        new Vector3(i * SegmentSize, 0f, j * SegmentSize), 
                        Quaternion.identity);
                    
                    instance.GetComponent<SegmentBehaviour>().Init(_map.GetSegment(i, j));
                }
            }

            CameraService.Bounds = new Rect(
                x: 0, y: 0, 
                width: _map.Width * SegmentSize, height: _map.Length * SegmentSize);
        }

        private void OnToggleEditPerformed(InputAction.CallbackContext obj) 
            => _isInEditMode = !_isInEditMode;
    }
}