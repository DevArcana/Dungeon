using System;
using UnityEngine;

namespace Camera
{
    public class DivineCamera : MonoBehaviour
    {
        public float turnMod = 0.3f;
        public float speed = 15.0f;
        public float smoothing = 0.05f;
        public float minDistance = 1.0f;
        public float maxDistance = 15.0f;
        public float groundOffset = 1.0f;

        private float _distanceSmooth;
        private float _distanceVelocity = 0.0f;
        private float _distance = 10.0f;
        private float _phi = 15.0f;
        private float _theta = 0.0f;        
        
        private Vector3 _origin;
        private Vector3 _smoothOrigin;
        private Vector3 _smoothingVelocity;
        private Vector3 _towardsCamera;
        private Vector3 _right;

        private Vector2 _lastMousePos;

        private bool _isMoving;
        private bool _isLocked;

        private void Start()
        {
            _origin = transform.position;
            _smoothingVelocity = Vector3.zero;

            _distanceSmooth = _distance;

            AdjustOriginToGround();
            _smoothOrigin = _origin;
            LookAtOrigin();
            FindPlayer();
        }

        private void FindPlayer()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                var pos = player.transform.position;
                _origin.x = pos.x;
                _origin.z = pos.z;
            }
        }

        private void AdjustOriginToGround()
        {
            if (Physics.Raycast(new Ray(_origin, Vector3.down), out var hit))
            {
                _origin = hit.point + Vector3.up * groundOffset;
            }
        }

        private void LookAtOrigin()
        {
            _towardsCamera = Quaternion.AngleAxis(_theta, Vector3.up) * Vector3.right;
            _right = Vector3.Cross(_towardsCamera, Vector3.up);
            
            transform.position = _smoothOrigin + Quaternion.AngleAxis(_phi, _right) * _towardsCamera * _distanceSmooth;
            transform.LookAt(_smoothOrigin);
        }

        public void LockMovement()
        {
            _isLocked = true;
        }

        public void UnlockMovement()
        {
            _isLocked = false;
        }

        public void PanToLocation(Vector3? location)
        {
            if (location.HasValue)
            {
                _origin = location.Value;
            }
            else
            {
                FindPlayer();
            }
            
            _isMoving = true;
        }

        public bool IsMoving()
        {
            return _isMoving;
        }

        private void Update()
        {
            _smoothOrigin = Vector3.SmoothDamp(_smoothOrigin, _origin, ref _smoothingVelocity, smoothing);
            _distanceSmooth = Mathf.SmoothDamp(_distanceSmooth, _distance, ref _distanceVelocity, smoothing);

            _isMoving = (_smoothOrigin - _origin).sqrMagnitude > 0.001f || Math.Abs(_distanceSmooth - _distance) > 0.001f;
            var updateCamera = _isMoving;

            var moveRight = Input.GetAxisRaw("Horizontal");
            var moveForward = Input.GetAxisRaw("Vertical");

            if (!_isLocked && (moveRight != 0.0f || moveForward != 0.0f))
            {
                var move = Vector3.Normalize(_towardsCamera * -moveForward + _right * moveRight);
                _origin += move * (speed * _distanceSmooth * Time.deltaTime);
                updateCamera = true;
            }

            var mouseScrollDelta = -Input.mouseScrollDelta.y;

            if (mouseScrollDelta != 0.0f)
            {
                _distance += mouseScrollDelta;
                _distance = Mathf.Clamp(_distance, minDistance, maxDistance);
                updateCamera = true;
            }
            
            if (Input.GetMouseButtonDown(2))
            {
                _lastMousePos = Input.mousePosition;
            }

            if (Input.GetMouseButton(2))
            {
                var mousePos = Input.mousePosition;
                var dx = mousePos.x - _lastMousePos.x;
                var dy = mousePos.y - _lastMousePos.y;
                _lastMousePos = mousePos;

                _theta += dx * turnMod;
                _phi += -dy * turnMod;

                if (_theta < 0.0f) _theta += 360.0f;
                if (_theta >= 360.0f) _theta -= 360.0f;
                
                _phi = Mathf.Clamp(_phi, 1.0f, 89.0f);
                updateCamera = true;
            }

            if (updateCamera)
            {
                LookAtOrigin();
            }
        }
    }
}
