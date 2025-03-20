using Code.Scripts.App.Common;
using Code.Scripts.Configs;
using Code.Scripts.Persistence;
using Code.Scripts.Services;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.Scripts.Components
{
    public class PlayerCharacter : MonoBehaviour
    {
        [SerializeField]
        private bool _grounded;
        
        [Tooltip("Useful for rough ground")]
        [SerializeField]
        private float _groundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        [SerializeField]
        private float _groundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        [SerializeField]
        private LayerMask _groundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        [SerializeField]
        private GameObject _cinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        [SerializeField]
        private float _topClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        [SerializeField]
        private float _bottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        [SerializeField]
        private float _cameraAngleOverride;

        [Tooltip("For locking the camera position on all axis")]
        [SerializeField]
        private bool _lockCameraPosition;

        private PlayerCharacterConfig _cfg;
        
        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation;
        private float _rotationVelocity;
        private float _velocity;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

        [SerializeField] private Animator _animator;
        [SerializeField] private CharacterController _controller;
        
        private InputState _input;
        private InputSettings _inputSettings;
        private Transform _mainCamera;

        private const float _threshold = 0.01f;

        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
            }
        }

        private void Start()
        {
            _cinemachineTargetYaw = _cinemachineCameraTarget.transform.rotation.eulerAngles.y;

            _input = Mediator.InputState;
            _inputSettings = Mediator.InputSettings;
            _cfg = Mediator.Get<AssetsService>().playerCharacterConfig;
            
            AssignAnimationIDs();
        }

        private void Update()
        {
            Gravity();
            GroundedCheck();
            Move();
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            var spherePosition = new Vector3(transform.position.x, transform.position.y - _groundedOffset, transform.position.z);
            _grounded = Physics.CheckSphere(spherePosition, _groundedRadius, _groundLayers, QueryTriggerInteraction.Ignore);

            // update animator if using character
            _animator.SetBool(_animIDGrounded, _grounded);
        }

        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (_input.Look.sqrMagnitude >= _threshold && !_lockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                var deltaTimeMultiplier = _input.IsCurrentDeviceMouse ? _inputSettings.MouseSensitivity : Time.deltaTime;
                var pitchSign = _inputSettings.InvertMouse ? -1 : 1;
                
                _cinemachineTargetYaw += _input.Look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.Look.y * deltaTimeMultiplier * pitchSign;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, _bottomClamp, _topClamp);

            // Cinemachine will follow this target
            _cinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + _cameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private void Move()
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            var targetSpeed = _input.Sprint ? _cfg.SprintSpeed : _cfg.MoveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.Move == Vector2.zero)
            {
                targetSpeed = 0.0f;
            }

            // a reference to the players current horizontal velocity
            var currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            var speedOffset = 0.1f;
            var inputMagnitude = _input.Move.magnitude;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * _cfg.SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * _cfg.SpeedChangeRate);
            if (_animationBlend < 0.01f)
            {
                _animationBlend = 0f;
            }

            // normalise input direction
            var inputDirection = new Vector3(_input.Move.x, 0.0f, _input.Move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.Move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.eulerAngles.y;
                var rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    _cfg.RotationSmoothTime);

                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }


            var targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // move the player
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0, _velocity * Time.deltaTime, 0));

            // update animator if using character
            _animator.SetFloat(_animIDSpeed, _animationBlend);
            _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
        }
        
        private void Gravity()
        {
            if (_grounded)
            {
                _fallTimeoutDelta = _cfg.FallTimeout;

                _animator.SetBool(_animIDJump, false);
                _animator.SetBool(_animIDFreeFall, false);

                // stop our velocity dropping infinitely when grounded
                if (_velocity < 0.0f)
                {
                    _velocity = -2f;
                }
            }
            else
            {
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    _animator.SetBool(_animIDFreeFall, true);
                }
            }

            // apply gravity over time (multiply by delta time twice to linearly speed up over time)
            _velocity += _cfg.Gravity * Time.deltaTime;
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            // if (animationEvent.animatorClipInfo.weight > 0.5f)
            // {
            //     if (FootstepAudioClips.Length > 0)
            //     {
            //         var index = Random.Range(0, FootstepAudioClips.Length);
            //         AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
            //     }
            // }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            // if (animationEvent.animatorClipInfo.weight > 0.5f)
            // {
            //     AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            // }
        }
    }
}