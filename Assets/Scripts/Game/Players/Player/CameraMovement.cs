using System;
using Core.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Players.Player
{
    public class CameraMovement : MonoBehaviour
    {
        #region Inspector

        [Title("Input")]

        [SerializeField] private KeyCode positionKeyCode = KeyCode.Mouse1;
        [SerializeField] private KeyCode rotationKeyCode = KeyCode.Mouse0;

        [Space] [SerializeField] private float positionSensitivity = 10;
        [SerializeField] private float rotationSensitivity = 100;
        [SerializeField] private float zoomSensitivity = 0.1f;

        [Title("Range")]

        [SerializeField] private float maxPositionDistance = 25;

        [Space] [SerializeField] [PropertyRange(1, nameof(maxRotationAngleX))] private int minRotationAngleX = 15;
        [SerializeField] [PropertyRange(nameof(minRotationAngleX), 89)] private int maxRotationAngleX = 85;

        [Space] [SerializeField] private float minZoomDistance = 1;
        [SerializeField] private float maxZoomDistance = 25;

        [Title("Debug")]

        [NonSerialized] [ShowInInspector] [ReadOnly] public Vector2 Position;
        [NonSerialized] [ShowInInspector] [ReadOnly] public Vector2 Rotation;
        [NonSerialized] [ShowInInspector] [ReadOnly] public float ZoomDistance;

        private void OnValidate()
        {
            InitializeMovement();
        }

        #endregion

        private void Start()
        {
            ResetMouseInput();
            InitializeSensitivity();

            InitializeMovement();
            UpdateMovement();
        }

        private void InitializeSensitivity()
        {
            var screenSize = (Screen.width + Screen.height) / 2.0f;

            positionSensitivity /= screenSize;
            rotationSensitivity /= screenSize;
        }

        private void InitializeMovement()
        {
            transform.GetLocalPositionAndRotation(out var position, out var rotation);

            Rotation = rotation.eulerAngles;
            ZoomDistance = -position.Rotate(-Rotation).z;
            var zoomPosition = new Vector3(0f, 0f, -ZoomDistance).Rotate(Rotation);
            Position = (position - zoomPosition).GetXZ();
        }

        private void Update()
        {
            if (UpdateInputs())
            {
                UpdateMovement();
            }
        }

        public void UpdateMovement()
        {
            var position = Position.GetXZ();
            var zoomPosition = new Vector3(0f, 0f, -ZoomDistance).Rotate(Rotation);
            var rotation = Quaternion.Euler(Rotation);

            transform.SetLocalPositionAndRotation(position + zoomPosition, rotation);
        }

        public Vector2 ClampPosition(Vector2 position)
        {
            var positionDistance = position.magnitude;
            if (positionDistance > maxPositionDistance)
            {
                return position * (maxPositionDistance / positionDistance);
            }

            return position;
        }

        public float ClampZoomDistance(float zoomDistance)
        {
            return Mathf.Clamp(zoomDistance, minZoomDistance, maxZoomDistance);
        }

        #region Input

        private bool _inputEnabled = true;

        public bool InputEnabled
        {
            get => _inputEnabled;
            set
            {
                if (_inputEnabled == value)
                {
                    return;
                }

                _inputEnabled = value;
                if (_inputEnabled)
                {
                    ResetMouseInput();

                    // InitializeMovement();
                    // UpdateMovement();
                }
            }
        }

        private bool UpdateInputs()
        {
            if (!_inputEnabled)
            {
                return false;
            }

            UpdateOverUIChecks();
            return UpdateMovementInputs();
        }

        #region OverUI Checks

        private bool _isBeganOverUI = false;
        private bool _isOverUI = false;

        private void UpdateOverUIChecks()
        {
            _isOverUI = BaseExtensions.IsOverUI();
            if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.Mouse2))
            {
                _isBeganOverUI = _isOverUI;
            }
        }

        #endregion

        private bool UpdateMovementInputs()
        {
            if (_isBeganOverUI)
            {
                ResetMouseInput();
            }
            else
            {
                UpdateMouseInput();
            }

            var isPositionUpdated = UpdatePositionInput();
            var isRotationUpdated = UpdateRotationInput();
            var isZoomUpdated = UpdateZoomInput();

            return isPositionUpdated || isRotationUpdated || isZoomUpdated;
        }

        #region Mouse

        private Vector2 _currentMousePosition;
        private Vector2 _deltaMousePosition;
        private Vector2 _previousMousePosition;

        private void UpdateMouseInput()
        {
            _currentMousePosition = Input.mousePosition;
            _deltaMousePosition = _currentMousePosition - _previousMousePosition;
            _previousMousePosition = _currentMousePosition;
        }

        private void ResetMouseInput()
        {
            _currentMousePosition = Input.mousePosition;
            _deltaMousePosition = Vector2.zero;
            _previousMousePosition = _currentMousePosition;
        }

        #endregion

        #region Movement

        private bool UpdatePositionInput()
        {
            if (_isBeganOverUI)
            {
                return false;
            }

            if (_deltaMousePosition.magnitude <= 0)
            {
                return false;
            }

            if (!Input.GetKey(positionKeyCode))
            {
                return false;
            }

            var positionDelta = _deltaMousePosition.GetXZ().Rotate(Rotation * Vector2.up).GetXZ() * positionSensitivity;
            Position = ClampPosition(Position - positionDelta);

            return true;
        }

        private bool UpdateRotationInput()
        {
            if (_isBeganOverUI)
            {
                return false;
            }

            if (_deltaMousePosition.magnitude <= 0)
            {
                return false;
            }

            if (!Input.GetKey(rotationKeyCode))
            {
                return false;
            }

            var rotationDelta = _deltaMousePosition * rotationSensitivity;
            Rotation = new Vector2(Rotation.x - rotationDelta.y, Rotation.y + rotationDelta.x);
            Rotation.x = Mathf.Clamp(Rotation.x, minRotationAngleX, maxRotationAngleX);

            return true;
        }

        private bool UpdateZoomInput()
        {
            if (_isOverUI)
            {
                return false;
            }

            if (Input.mouseScrollDelta.y == 0)
            {
                return false;
            }

            var zoomDelta = Input.mouseScrollDelta.y * zoomSensitivity;
            ZoomDistance = ClampZoomDistance(ZoomDistance - zoomDelta);

            return true;
        }

        #endregion

        #endregion
    }
}