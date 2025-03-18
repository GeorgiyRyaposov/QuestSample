using Code.Scripts.App.Common;
using Code.Scripts.Persistence;
using Code.Scripts.Services.Common;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Code.Scripts.Services
{
    public class InputService : IService, InputSystemActions.IPlayerActions, IInitializable
    {
        private InputState State => Mediator.InputState;
        
        private InputSystemActions _actions;
        
        public void Initialize()
        {
            _actions = new InputSystemActions();
            _actions.Player.AddCallbacks(this);
        }
        
        public void EnablePlayerInput()
        {
            _actions.Player.Enable();
        }
        public void DisablePlayerInput()
        {
            _actions.Player.Disable();
        }
        
        #region IPlayerActions

        public void OnMove(InputAction.CallbackContext context)
        {
            MoveInput(context.ReadValue<Vector2>());
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            LookInput(context.ReadValue<Vector2>());
        }

        public void OnZoom(InputAction.CallbackContext context)
        {
            ZoomInput(-context.ReadValue<Vector2>().y);
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            InteractInput(context.ReadValueAsButton());
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            SprintInput(context.ReadValueAsButton());
        }
        
        #endregion //IPlayerActions
        
        private void MoveInput(Vector2 newMoveDirection)
        {
            State.Move = newMoveDirection;
        } 

        private void LookInput(Vector2 newLookDirection)
        {
            State.Look = newLookDirection;
        }
        
        private void ZoomInput(float value)
        {
            State.Zoom = value;
        }

        private void SprintInput(bool newSprintState)
        {
            State.Sprint = newSprintState;
        }
        
        private void InteractInput(bool value)
        {
            State.Interact = value;
        }
    }
}