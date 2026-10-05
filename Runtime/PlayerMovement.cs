using PurrNet.Prediction;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Warwlock.PlayerController
{
    public class PlayerMovement : PredictedIdentity<PlayerMovement.Input, PlayerMovement.State>, IMovement
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float sprintSpeed = 8f;
        [SerializeField] private float jumpForce = 1f;
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private float groundCheckDistance = 0.2f;
        [SerializeField] private CharacterController characterController;

        [Header("Player Camera")]
        [SerializeField] private CameraController cameraController;

        [Header("Input References")]
        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private InputActionReference jumpAction;
        [SerializeField] private InputActionReference sprintAction;


        // View state cached variables
        private bool _isGrounded;
        private bool _isJumped;
        private Vector3 _movementDirectionSpeed;

        public bool IsGrounded => _isGrounded;
        public bool IsJumped => _isJumped;
        public Vector3 MovementDirectionSpeed => _movementDirectionSpeed;

        protected override void LateAwake()
        {
            if (isOwner)
                cameraController.Init();
        }

        public void SetMovementEnabledSimulate(bool isEnabled) => currentState.isMovementEnabled = isEnabled;
        
        protected override void Simulate(Input input, ref State state, float delta)
        {
            if (currentState.isMovementEnabled)
            {
                HandleRotation(input);
                HandleMovement(input, ref state, delta);
            }
        }

        private void HandleMovement(Input input, ref State state, float delta)
        {
            bool isGrounded = GetIsGrounded();
            state.isGrounded = isGrounded;
            if (isGrounded && state.verticalVelocity < 0)
            {
                state.isJumped = false;
                state.verticalVelocity = -1f;
            }

            Vector3 moveDirection = transform.right * input.moveDirection.x + transform.forward * input.moveDirection.y;
            moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

            float currentSpeed = input.sprint ? sprintSpeed : moveSpeed;
            state.movementDirectionSpeed = moveDirection * currentSpeed;

            if (input.jump && isGrounded)
            {
                state.isJumped = true;
                state.verticalVelocity = Mathf.Sqrt(jumpForce * -2f * gravity);
            }

            state.verticalVelocity += gravity * delta;
            Vector3 finalMove = (moveDirection * currentSpeed) + (state.verticalVelocity * Vector3.up);
            characterController.Move(finalMove * delta);
        }

        private void HandleRotation(Input input)
        {
            Vector3 camForward = input.cameraForward;
            camForward.y = 0;

            if (camForward.sqrMagnitude > 0.0001f)
                transform.rotation = Quaternion.LookRotation(camForward.normalized);
        }

        private bool GetIsGrounded()
        {
            return Physics.Raycast(transform.position + Vector3.up * 0.03f, Vector3.down, groundCheckDistance);
        }

        protected override void UpdateView(State viewState, State? verified)
        {
            if (!isOwner && !verified.HasValue) return;
            State state = isOwner ? viewState : verified.Value;

            _isJumped = state.isJumped;
            _isGrounded = state.isGrounded;
            _movementDirectionSpeed = state.movementDirectionSpeed;
        }

        // Inputs
        protected override void GetFinalInput(ref Input input)
        {
            input.moveDirection = moveAction.action.ReadValue<Vector2>();
            input.cameraForward = cameraController.forward;
        }

        protected override void UpdateInput(ref Input input)
        {
            input.jump |= jumpAction.action.WasPerformedThisFrame();
            if (sprintAction)
                input.sprint |= sprintAction.action.WasPerformedThisFrame();
        }

        protected override void SanitizeInput(ref Input input)
        {
            if (input.moveDirection.magnitude > 1)
                input.moveDirection.Normalize();
        }

        protected override void ModifyExtrapolatedInput(ref Input input)
        {
            input.jump = false;
        }

        public struct State : IPredictedData<State>
        {
            public float verticalVelocity;
            public Vector3 movementDirectionSpeed;
            public bool isGrounded;
            public bool isJumped;
            public bool isMovementEnabled;

            public void Dispose()
            {

            }

            public override string ToString()
            {
                string result = $"velocity: {verticalVelocity}";
                result += $"\n isGrounded: {isGrounded}";
                result += $"\n isJumped: {isJumped}";

                return result;
            }
        }

        public struct Input : IPredictedData<Input>
        {
            public Vector2 moveDirection;
            public Vector3 cameraForward;
            public bool jump;
            public bool sprint;

            public void Dispose()
            {

            }
        }
    }
}
