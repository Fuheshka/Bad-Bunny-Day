using UnityEngine;
using UnityEngine.Events;

namespace CDB.Player.Components
{
    /// <summary>
    /// Компонент движения игрока.
    /// Отвечает за ходьбу, прыжки, рывки и Game Feel механики.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Данные")]
        [SerializeField] private Data.PlayerMovementData _movementData;

        [Header("Ссылки")]
        [SerializeField] private PlayerStamina _stamina;
        [SerializeField] private PlayerWeight _weight;
        
        [Header("Слои")]
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private LayerMask _obstacleLayer;

        [Header("События")]
        public UnityEvent OnJump;
        public UnityEvent OnDash;
        public UnityEvent<Vector3> OnNoise;

        // Компоненты
        private Rigidbody _rb;
        private Collider _collider;
        private Transform _cameraTransform;

        // Game Feel состояния
        private float _lastGroundedTime;
        private float _lastJumpPressTime;
        private float _lastDashPressTime;
        private bool _hasJumped;
        private bool _isJumpHeld;
        private Vector3 _bufferedDashDirection;
        
        // Состояния
        private bool _isDashing;
        private bool _isEnabled = true;

        // Кэш инпута
        private Vector2 _inputVector;

        public bool IsGrounded => CheckGrounded();
        public bool IsDashing => _isDashing;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
        }

        private void Start()
        {
            if (Camera.main != null)
                _cameraTransform = Camera.main.transform;
            else
                Debug.LogError("[PlayerMovement] MainCamera не найдена!");
        }

        private void FixedUpdate()
        {
            if (!_isEnabled) return;
            
            Move();
            ApplyCornerSliding();
            ApplyWallSlide();
        }

        private void Update()
        {
            if (!_isEnabled) return;
            
            UpdateCoyoteTime();
            ProcessJumpBuffer();
            ProcessDashBuffer();
        }

        /// <summary>
        /// Установить направление движения
        /// </summary>
        public void SetInput(Vector2 input)
        {
            _inputVector = input;
        }

        /// <summary>
        /// Запрос на прыжок
        /// </summary>
        public void RequestJump()
        {
            _lastJumpPressTime = Time.time;
            TryExecuteJump();
        }

        /// <summary>
        /// Отпускание кнопки прыжка (для Variable Jump Height)
        /// </summary>
        public void ReleaseJump()
        {
            if (_movementData == null) return;
            
            if (_isJumpHeld && _rb.linearVelocity.y > 0)
            {
                Vector3 vel = _rb.linearVelocity;
                vel.y *= _movementData.jumpCutMultiplier;
                _rb.linearVelocity = vel;
            }
            _isJumpHeld = false;
        }

        /// <summary>
        /// Запрос на рывок
        /// </summary>
        public void RequestDash()
        {
            _lastDashPressTime = Time.time;
            _bufferedDashDirection = GetDashDirection();
            TryExecuteDash();
        }

        /// <summary>
        /// Включить/выключить движение
        /// </summary>
        public void SetEnabled(bool enabled)
        {
            _isEnabled = enabled;
            if (!enabled)
            {
                _rb.linearVelocity = Vector3.zero;
            }
        }

        /// <summary>
        /// Остановить движение
        /// </summary>
        public void Stop()
        {
            _rb.linearVelocity = Vector3.zero;
        }

        private void Move()
        {
            if (_isDashing || _movementData == null) return;

            Vector3 moveDirection = GetMoveDirection();
            float speedMultiplier = _weight != null ? _weight.SpeedMultiplier : 1f;
            float currentMaxSpeed = _movementData.baseSpeed * speedMultiplier;
            
            Vector3 targetVelocity = moveDirection * currentMaxSpeed;
            targetVelocity.y = _rb.linearVelocity.y;
            _rb.linearVelocity = targetVelocity;

            // Поворот в сторону движения
            if (moveDirection.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 15f * Time.fixedDeltaTime);
            }
        }

        private Vector3 GetMoveDirection()
        {
            if (_inputVector.magnitude < 0.1f || _cameraTransform == null)
                return Vector3.zero;

            Vector3 camForward = _cameraTransform.forward;
            Vector3 camRight = _cameraTransform.right;
            camForward.y = 0;
            camRight.y = 0;
            camForward.Normalize();
            camRight.Normalize();

            return (camForward * _inputVector.y + camRight * _inputVector.x).normalized;
        }

        private Vector3 GetDashDirection()
        {
            Vector3 dir = GetMoveDirection();
            return dir.magnitude > 0.1f ? dir : transform.forward;
        }

        // === Coyote Time ===
        private void UpdateCoyoteTime()
        {
            if (CheckGrounded())
            {
                _hasJumped = false;
                _lastGroundedTime = Time.time;
            }
        }

        private bool CanJumpWithCoyote()
        {
            if (_hasJumped || _movementData == null) return false;
            return Time.time - _lastGroundedTime <= _movementData.coyoteTime;
        }

        // === Input Buffering ===
        private void ProcessJumpBuffer()
        {
            if (_movementData != null && Time.time - _lastJumpPressTime <= _movementData.jumpBufferTime)
            {
                TryExecuteJump();
            }
        }

        private void ProcessDashBuffer()
        {
            if (_movementData != null && Time.time - _lastDashPressTime <= _movementData.dashBufferTime)
            {
                TryExecuteDash();
            }
        }

        // === Прыжок ===
        private void TryExecuteJump()
        {
            if (CanJumpWithCoyote() && !_isDashing && _movementData != null)
            {
                ExecuteJump();
                _lastJumpPressTime = 0f;
            }
        }

        private void ExecuteJump()
        {
            _hasJumped = true;
            _isJumpHeld = true;
            
            Vector3 vel = _rb.linearVelocity;
            vel.y = 0;
            _rb.linearVelocity = vel;
            
            _rb.AddForce(Vector3.up * _movementData.jumpForce, ForceMode.Impulse);
            OnJump?.Invoke();
        }

        // === Рывок ===
        private void TryExecuteDash()
        {
            if (_movementData == null) return;
            if (_isDashing) return;
            if (_stamina != null && !_stamina.HasEnough(_movementData.dashCost)) return;
            
            StartCoroutine(DashRoutine(_bufferedDashDirection));
            _lastDashPressTime = 0f;
        }

        private System.Collections.IEnumerator DashRoutine(Vector3 direction)
        {
            _isDashing = true;
            
            if (_stamina != null)
                _stamina.TryConsume(_movementData.dashCost);
            
            _rb.AddForce(direction * _movementData.dashForce, ForceMode.Impulse);
            OnNoise?.Invoke(transform.position);
            OnDash?.Invoke();
            
            yield return new WaitForSeconds(_movementData.dashDuration);
            
            _isDashing = false;
        }

        // === Corner Sliding ===
        private void ApplyCornerSliding()
        {
            if (_isDashing || _movementData == null) return;
            if (_inputVector.magnitude < 0.1f) return;

            Vector3 moveDir = GetMoveDirection();
            float playerRadius = _collider.bounds.extents.x;
            float checkDist = _movementData.cornerCheckDistance + playerRadius;

            Vector3 origin = transform.position + Vector3.up * 0.5f;
            bool centerHit = Physics.Raycast(origin, moveDir, checkDist, _obstacleLayer);

            if (!centerHit) return;

            Vector3 rightDir = Quaternion.Euler(0, 45, 0) * moveDir;
            Vector3 leftDir = Quaternion.Euler(0, -45, 0) * moveDir;

            bool rightHit = Physics.Raycast(origin, rightDir, checkDist, _obstacleLayer);
            bool leftHit = Physics.Raycast(origin, leftDir, checkDist, _obstacleLayer);

            Vector3 slideForce = Vector3.zero;

            if (!rightHit && leftHit)
            {
                slideForce = Vector3.Cross(Vector3.up, moveDir).normalized * _movementData.cornerSlideForce;
            }
            else if (!leftHit && rightHit)
            {
                slideForce = -Vector3.Cross(Vector3.up, moveDir).normalized * _movementData.cornerSlideForce;
            }

            if (slideForce != Vector3.zero)
            {
                _rb.AddForce(slideForce, ForceMode.Acceleration);
            }
        }

        // === Wall Slide ===
        private void ApplyWallSlide()
        {
            if (CheckGrounded()) return;
            
            float playerRadius = _collider.bounds.extents.x;
            Vector3 origin = transform.position + Vector3.up * 0.5f;
            
            bool touchingWall = 
                Physics.Raycast(origin, transform.forward, playerRadius + 0.1f, _obstacleLayer) ||
                Physics.Raycast(origin, -transform.forward, playerRadius + 0.1f, _obstacleLayer) ||
                Physics.Raycast(origin, transform.right, playerRadius + 0.1f, _obstacleLayer) ||
                Physics.Raycast(origin, -transform.right, playerRadius + 0.1f, _obstacleLayer);
            
            if (touchingWall && _rb.linearVelocity.y <= 0)
            {
                Vector3 vel = _rb.linearVelocity;
                vel.x *= 0.9f;
                vel.z *= 0.9f;
                
                if (Mathf.Abs(vel.y) < 0.5f)
                {
                    vel.y = -2f;
                }
                
                _rb.linearVelocity = vel;
            }
        }

        private bool CheckGrounded()
        {
            if (_collider == null || _movementData == null) return false;
            float distanceToEdge = _collider.bounds.extents.y;
            return Physics.Raycast(transform.position, Vector3.down, distanceToEdge + 0.1f, _groundLayer);
        }
    }
}
