using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] CharacterController controller;

    [SerializeField] int speed;
    [SerializeField] int sprintMod;
    [SerializeField] int jumpSpeed;
    [SerializeField] int jumpMax;
    [SerializeField] int gravity;
    [SerializeField] int maxTilt; // suggested 45 degrees
    [SerializeField] int tiltSpeed;

    Vector3 moveDirection;
    Vector3 playerVelocity;

    int jumpCount;

    bool isSprinting;

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    void Movement()
    {
        Sprint();

        moveDirection = (Input.GetAxis("Horizontal") * transform.right) +
                  (Input.GetAxis("Vertical") * transform.forward);

        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVelocity = Vector3.zero;
        }
        else
        {
            playerVelocity.y -= gravity * Time.deltaTime;

            // If the player is moving left or right, lean away from that direction to make wall running possible
            Tilt();
        }

        controller.Move(moveDirection * speed * Time.deltaTime);

        Jump();

        controller.Move(playerVelocity * Time.deltaTime);
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVelocity.y = jumpSpeed;
        }
    }

    void Sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
            isSprinting = false;
        }
    }

    void Tilt()
    {
        float rotz = Input.GetAxisRaw("Horizontal") * tiltSpeed * Time.deltaTime;
        rotz = Mathf.Clamp(rotz, -maxTilt, maxTilt);
        //transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, rotz);
        transform.Rotate(Vector3.forward * rotz);
    }
}
