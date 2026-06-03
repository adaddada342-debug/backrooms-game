using UnityEngine;

namespace Backrooms.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class SimpleFirstPersonController : MonoBehaviour
    {
        [SerializeField]
        private float moveSpeed = 4.5f;

        [SerializeField]
        private float lookSensitivity = 2f;

        [SerializeField]
        private float gravity = -18f;

        [SerializeField]
        private Transform cameraTransform = null;

        private CharacterController characterController;
        private float verticalVelocity;
        private float cameraPitch;

        public void SetCameraTransform(Transform newCameraTransform)
        {
            cameraTransform = newCameraTransform;
        }

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();

            if (cameraTransform == null)
            {
                Camera childCamera = GetComponentInChildren<Camera>();
                if (childCamera != null)
                {
                    cameraTransform = childCamera.transform;
                }
            }
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            UpdateLook();
            UpdateMovement();
        }

        private void UpdateLook()
        {
            if (cameraTransform == null)
            {
                return;
            }

            float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

            transform.Rotate(Vector3.up * mouseX);
            cameraPitch = Mathf.Clamp(cameraPitch - mouseY, -80f, 80f);
            cameraTransform.localEulerAngles = new Vector3(cameraPitch, 0f, 0f);
        }

        private void UpdateMovement()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            Vector3 move = transform.right * horizontal + transform.forward * vertical;
            if (move.sqrMagnitude > 1f)
            {
                move.Normalize();
            }

            if (characterController.isGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }

            verticalVelocity += gravity * Time.deltaTime;
            Vector3 velocity = move * moveSpeed;
            velocity.y = verticalVelocity;

            characterController.Move(velocity * Time.deltaTime);
        }
    }
}
