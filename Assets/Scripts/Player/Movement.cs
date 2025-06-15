using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    public class Movement : NetworkBehaviour
    {
        [SerializeField] private CharacterController controller = null;
        [SerializeField] private float speed = 5f;
        private float gravity = -9.81f;
        private Vector3 velocity;

        [SerializeField] private GameObject feet;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private float jumpPower = 2f;
        private float checkRadius = 0.4f;

        [SerializeField] private Animator fpsAnimator = null, tpAnimator = null;


        void Update()
        {
            if (!isLocalPlayer)
                return;

            bool isGrounded = Physics.CheckSphere(feet.transform.position, checkRadius, groundMask);

            if (isGrounded && velocity.y <= 0)
            {
                velocity.y = -2;
            }

            float X = Input.GetAxis("Horizontal");
            float Z = Input.GetAxis("Vertical");

            if (X != 0 || Z != 0)
            {
                fpsAnimator.SetBool("isWalking", true);
                tpAnimator.SetBool("isWalking", true);
            }
            else
            {
                fpsAnimator.SetBool("isWalking", false);
                tpAnimator.SetBool("isWalking", false);
            }

            Vector3 movement = transform.right * X + transform.forward * Z;

            controller.Move(movement * speed * Time.deltaTime);

            if (Input.GetButtonDown("Jump"))
            {
                if (!isGrounded)
                    return;
                velocity.y = Mathf.Sqrt(jumpPower * -2 * gravity);
            }

            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }
    }
}