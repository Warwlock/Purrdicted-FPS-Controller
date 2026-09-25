using PurrNet.Prediction;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Warwlock.Prediction.PlayerController
{
    public class PlayerMovement : PredictedIdentity<PlayerMovement.Input, PlayerMovement.State>
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float sprintSpeed = 8f;
        [SerializeField] private float jumpForce = 1f;
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private float groundCheckDistance = 0.2f;
        [SerializeField] private CharacterController characterController;
        [SerializeField] private Animator characterAnimator;

        [Header("Player Camera")]
        [SerializeField] private CameraController cameraController;

        [Header("Input References")]
        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private InputActionReference jumpAction;
        [SerializeField] private InputActionReference sprintAction;

        protected override void LateAwake()
        {
            if (isOwner)
                cameraController.Init();
        }

        protected override void Simulate(Input input, ref State state, float delta)
        {
            HandleRotation(input);
            HandleMovement(input, ref state, delta);
        }

        private void HandleMovement(Input input, ref State state, float delta)
        {
            bool isGrounded = IsGrounded();
            if (isGrounded && state.velocity.y < 0)
            {
                state.velocity.y = 0f;
            }

            Vector3 moveDirection = transform.right * input.moveDirection.x + transform.forward * input.moveDirection.y;
            moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

            state.isMoving = moveDirection.sqrMagnitude > 0.001f;

            float currentSpeed = input.sprint ? sprintSpeed : moveSpeed;

            if (input.jump && isGrounded)
            {
                state.velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            }

            state.velocity.y += gravity * delta;
            Vector3 finalMove = (moveDirection * currentSpeed) + (state.velocity.y * Vector3.up);
            characterController.Move(finalMove * delta);
        }

        private void HandleRotation(Input input)
        {
            Vector3 camForward = input.cameraForward;
            camForward.y = 0;

            if (camForward.sqrMagnitude > 0.0001f)
                transform.rotation = Quaternion.LookRotation(camForward.normalized);
        }

        private bool IsGrounded()
        {
            return Physics.Raycast(transform.position + Vector3.up * 0.03f, Vector3.down, groundCheckDistance);
        }

        protected override void UpdateView(State viewState, State? verified)
        {
            if (!characterAnimator) return;
            
            if (!isOwner && !verified.HasValue) return;
            State state = isOwner ? viewState : verified.Value;

            characterAnimator.SetBool("isMoving", state.isMoving);

        }


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
            public Vector3 velocity;

            public bool isMoving;
            public void Dispose()
            {

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
