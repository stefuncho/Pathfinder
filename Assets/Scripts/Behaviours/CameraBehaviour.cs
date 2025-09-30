using System;
using Cinemachine;
using Services;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Behaviours
{
    public class CameraBehaviour : MonoBehaviour, ICameraService
    {
        public Rect Bounds
        {
            get => _bounds;
            set
            {
                _bounds = value;
                Transform.SetPositionAndRotation(new Vector3(_bounds.center.x, 0f, _bounds.center.y), Quaternion.identity);
            }
        }
        
        protected void Awake()
        {
            _zoomAction = _zoomActionReference.action;
            _moveAction = _moveActionReference.action;
            _rotateAction = _rotateActionReference.action;
            _rotateDragAction = _rotateDragActionReference.action;
        }
        
        protected void Update()
        {
            var zoomValue = _zoomAction.ReadValue<Vector2>();
            var rotateDragValue = _rotateDragAction.ReadValue<Vector2>();
            var moveValue = _moveAction.ReadValue<Vector2>();
            var tempTransform = Transform;

            if (Mathf.Abs(zoomValue.y) > IgnorableValue)
            {
                _zoom = Mathf.Clamp01(_zoom + zoomValue.y * _zoomSpeed * Time.deltaTime);
                _cinemachineMixingCamera.m_Weight0 = 1 - _zoom;
                _cinemachineMixingCamera.m_Weight1 = _zoom;
            }

            if (_rotateAction.IsPressed() && Mathf.Abs(rotateDragValue.x) > IgnorableValue)
            {
                var eulerAngles = _transform.eulerAngles;
                eulerAngles.y += rotateDragValue.x * _rotationSpeed * Time.deltaTime;
                tempTransform.localEulerAngles = eulerAngles;
            }
            
            var moveVector = new Vector3(moveValue.x, 0f, moveValue.y) * (_moveSpeed * Time.deltaTime);
            
            tempTransform.Translate(moveVector);
            tempTransform.position = Clamp(tempTransform.position, Bounds);
        }

        private const float IgnorableValue = .01f;
        
        [SerializeField] private float _zoomSpeed = .5f;
        [SerializeField] private float _rotationSpeed = .5f;
        [SerializeField] private float _moveSpeed = .5f;

        [SerializeField] private CinemachineMixingCamera _cinemachineMixingCamera = default;
        
        [SerializeField] private InputActionReference _zoomActionReference = default;
        [SerializeField] private InputActionReference _moveActionReference = default;
        [SerializeField] private InputActionReference _rotateActionReference = default;
        [SerializeField] private InputActionReference _rotateDragActionReference = default;
        
        private InputAction _zoomAction = default;
        private InputAction _moveAction = default;
        private InputAction _rotateAction = default;
        private InputAction _rotateDragAction = default;
        
        private float _zoom = 0f;
        private Transform _transform;
        private Rect _bounds;
        
        private Transform Transform
        {
            get
            {
                _transform ??= GetComponent<Transform>();
                return _transform;
            }
        }
        
        private static Vector3 Clamp(Vector3 value, Rect bounds)
        {
            value.x = Mathf.Clamp(value.x, bounds.xMin, bounds.xMax);
            value.z = Mathf.Clamp(value.z, bounds.yMin, bounds.yMax);
            return value;
        }
    }
}