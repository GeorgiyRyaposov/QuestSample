using Cinemachine;
using Code.Scripts.App.Common;
using Code.Scripts.Persistence;
using UnityEngine;

namespace Code.Scripts.Components
{
    public class CharacterCinemachineWrapper : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _virtualCamera;
        [SerializeField] private float _minDistance;
        [SerializeField] private float _maxDistance;
        [SerializeField] private float _zoomSpeed = 5f;

        private InputState _input;
        private Cinemachine3rdPersonFollow _followComponent;
        
        private float _targetZoom;

        private void Start()
        {
            _input = Mediator.InputState;
            
            _followComponent = _virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();

            _targetZoom = _followComponent.CameraDistance;
        }

        private void LateUpdate()
        {
            CameraDistance();
        }

        private void CameraDistance()
        {
            _targetZoom = Mathf.Clamp(_targetZoom + _input.Zoom, _minDistance, _maxDistance);
            _followComponent.CameraDistance = Mathf.Lerp(_followComponent.CameraDistance, _targetZoom, Time.deltaTime * _zoomSpeed);
        }
    }
}