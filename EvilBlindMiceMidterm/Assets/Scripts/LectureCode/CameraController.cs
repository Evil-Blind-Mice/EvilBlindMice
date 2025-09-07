using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] int sensitivity;
    [SerializeField] int lockVerticalMaximum, lockVerticalMinimum;
    [SerializeField] bool invertY;

    float rotateX;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        // get input
        float mouseX = Input.GetAxisRaw("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensitivity * Time.deltaTime;

        // use invertY to give options of look up/down
        if (invertY)
            rotateX += mouseY;
        else
            rotateX -= mouseY;

        // clamp the camera on the x-axis
        rotateX = Mathf.Clamp(rotateX, lockVerticalMaximum, lockVerticalMinimum);

        // rotate the camera to look up and down
        transform.localRotation = Quaternion.Euler(rotateX, 0, 0);

        // rotate the player to look left and right
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}