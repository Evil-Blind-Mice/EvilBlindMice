using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] int sens;
    [SerializeField] int lockVertMaxX, lockVertMinX;
    [SerializeField] int lockVertMaxY, lockVertMinY;
    [SerializeField] bool invertY;

    float rotX;
    float rotY;

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
        float mouseX = Input.GetAxisRaw("Mouse X") * sens;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sens;

        // use invertY to give options of look up/down
        if (invertY)
            rotX += mouseY;
        else
            rotX -= mouseY;

        rotY += mouseX;

        // clamp the camera on the x-axis
        rotX = Mathf.Clamp(rotX, lockVertMinX, lockVertMaxX);

        rotY = Mathf.Clamp(rotY, lockVertMinY, lockVertMaxY);

        // rotate the camera to look up, down, left, and right
        transform.localRotation = Quaternion.Euler(rotX, rotY, 0);
    }
}