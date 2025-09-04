using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] bool showDebug;
    [SerializeField] Rigidbody body;

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
    float currentGravityVelocity;

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
        playerVelocity = Vector3.zero;
        Sprint();

        moveDirection = (Input.GetAxis("Horizontal") * transform.right) +
                  (Input.GetAxis("Vertical") * transform.forward);

        Debug.Log(IsGrounded());

        if (IsGrounded())
        {
            jumpCount = 0;
            currentGravityVelocity = 0;
        }
        else
        {
            currentGravityVelocity += gravity * Time.deltaTime;
        }

        // controller.Move(moveDirection * speed * Time.deltaTime);
        playerVelocity += (moveDirection * speed);

        Jump();

        //controller.Move(playerVelocity * Time.deltaTime);
        playerVelocity += gravityDirection * currentGravityVelocity;

        body.linearVelocity = playerVelocity;
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            currentGravityVelocity = -jumpSpeed;
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
        if (Physics.Raycast(transform.position, -transform.up, groundedDistance, groundLayers) && currentGravityVelocity >= 0)
            return true;
        else 
            return false;
    }

    void ShowDebug()
    {
        Debug.DrawRay(transform.position, -transform.up * groundedDistance, Color.blue);
    }
}
