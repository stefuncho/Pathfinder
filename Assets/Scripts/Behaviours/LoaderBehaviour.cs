using System;
using Model;
using Services;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Behaviours
{
    public class LoaderBehaviour : MonoBehaviour
    {
        protected void Start()
        {
            _toggleEditModeReference.action.performed += OnToggleEditPerformed;
            
            _raycasterBehaviour.SegmentSelected += OnSegmentSelected;
            _raycasterBehaviour.SelectEnded += OnSegmentSelectEnded;
            
            GenerateNewMap();
        }

        protected void OnDestroy()
        {
            if (_toggleEditModeReference != null)
            {
                _toggleEditModeReference.action.performed -= OnToggleEditPerformed;
            }
            
            if (_raycasterBehaviour != null)
            {
                _raycasterBehaviour.SegmentSelected -= OnSegmentSelected;
                _raycasterBehaviour.SelectEnded -= OnSegmentSelectEnded;
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
                    
                    if (_config.MapWidth < 4 || _config.MapLength < 4)
                        GUILayout.Label("Minimal Map Size is 4x4");
                    else if (GUILayout.Button("Generate Map"))
                        _newMapRequested = true;
                }
            }
            else
            {
                GUILayout.Label("Press F2 for Camera Mode");
            }
        }

        private enum CharacterType
        {
            Player = 0,
            Enemy,
        }

        private const float SegmentSize = 1f;
        
        private readonly Config _config = new();
        
        [SerializeField] private CameraBehaviour _cameraBehaviour = default;
        [SerializeField] private RaycasterBehaviour _raycasterBehaviour = default;
        [SerializeField] private GameObject _segmentPrefab = default;
        [SerializeField] private InputActionReference _toggleEditModeReference = default;
        [SerializeField] private Transform _playerCharacter = default;
        [SerializeField] private Transform _enemyCharacter = default;
        
        private bool _isInEditMode = false;
        private Map _map;
        private bool _newMapRequested;
        private CharacterType? _draggedCharacter = null;
        
        private Vector2Int[] _characterPosition = new Vector2Int[2];

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
            
            SetPosition(CharacterType.Player, new Vector2Int(1, 1));
            SetPosition(CharacterType.Enemy, new Vector2Int(_map.Width - 2, _map.Length - 2));

            CameraService.Bounds = new Rect(
                x: 0, y: 0, 
                width: _map.Width * SegmentSize, height: _map.Length * SegmentSize);
        }

        private void SetPosition(CharacterType character, Vector2Int position)
        {
            (character switch { CharacterType.Player => _playerCharacter, _ => _enemyCharacter })
                .position = new Vector3(position.x * SegmentSize, 0f, position.y * SegmentSize);
            
            _characterPosition[(int)character] = position;
        }

        private void OnToggleEditPerformed(InputAction.CallbackContext obj) 
            => _isInEditMode = !_isInEditMode;
        

        private void OnSegmentSelectEnded() 
            => _draggedCharacter = null;

        private void OnSegmentSelected(SegmentBehaviour obj)
        {
            if (_isInEditMode)
            {
                if (_draggedCharacter == null)
                {
                    if (obj.Position == _characterPosition[(int)CharacterType.Player])
                    {
                        _draggedCharacter = CharacterType.Player;
                        return;
                    }
                    
                    if (obj.Position == _characterPosition[(int)CharacterType.Enemy])
                    {
                        _draggedCharacter = CharacterType.Enemy;
                        return;
                    }
                    
                    obj.ToggleSegmentType();
                    return;
                }

                if (obj.Type != Segment.SegmentType.Floor
                    || obj.Position == _characterPosition[(int)CharacterType.Player]
                    || obj.Position == _characterPosition[(int)CharacterType.Enemy])
                {
                    return;
                }
                    
                SetPosition(_draggedCharacter.Value, obj.Position);
                return;
            }
            
            // PathFind
        }
    }
}