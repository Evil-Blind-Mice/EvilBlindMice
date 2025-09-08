using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] int sensitivity;
    [SerializeField] int lockVerticalMax, lockVerticalMin;
    [SerializeField] int lockHorizontalMax, lockHorizontalMin;
    [SerializeField] bool invertY;

    float rotateX;
    float rotateY;

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

        rotateY += mouseX;

        // clamp the camera on the x-axis
        rotateX = Mathf.Clamp(rotateX, lockVerticalMin, lockVerticalMax);
        rotateY = Mathf.Clamp(rotateY, lockHorizontalMin, lockHorizontalMax);

        // rotate the camera to look up and down left and right
        transform.localRotation = Quaternion.Euler(rotateX, rotateY, 0);
    }
}