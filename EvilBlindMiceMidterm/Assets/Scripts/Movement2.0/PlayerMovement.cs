using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] bool showDebug;
    [SerializeField] CharacterController controller;

    [SerializeField] LayerMask groundLayers;
    [SerializeField] float groundedDistance;

    [SerializeField] int speed;
    [SerializeField] int sprintMod;
    [SerializeField] int jumpSpeed;
    [SerializeField] int jumpMax;
    [SerializeField] int gravity;
    [SerializeField] int maxTilt; // suggested 45 degrees
    [SerializeField] int tiltSpeed;

    Vector3 moveDirection;
    Vector3 playerVelocity;
    Vector3 gravityDirection;

    int jumpCount;

    bool isSprinting;

    private void Start()
    {
        gravityDirection = -transform.up;
    }

    // Update is called once per frame
    void Update()
    {
        if (showDebug) ShowDebug();
        Movement();
    }

    void Movement()
    {
        Sprint();

        moveDirection = (Input.GetAxis("Horizontal") * transform.right) +
                  (Input.GetAxis("Vertical") * transform.forward);

        if (IsGrounded())
        {
            jumpCount = 0;
            playerVelocity = Vector3.zero;
        }
        else
        {
            playerVelocity += gravityDirection * gravity * Time.deltaTime;
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
            playerVelocity = -gravityDirection * jumpSpeed;
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

    bool IsGrounded()
    {
        if (Physics.Raycast(transform.position, -transform.up, groundedDistance, groundLayers))
            return true;
        else 
            return false;
    }

    void ShowDebug()
    {
        Debug.DrawRay(transform.position, -transform.up * groundedDistance, Color.blue);
    }
}
