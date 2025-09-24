using UnityEngine;
using System.Collections;

public class RotationHandler : MonoBehaviour
{
    Coroutine activeRotation;
    public float rotationSpeed;
    [SerializeField] Transform body;
    [HideInInspector] public bool isUpright;

    public void RotateSmooth(Quaternion _lookRotation, float _rotationSpeed = -1)
    {
        if (activeRotation != null) StopCoroutine(activeRotation);
        if (_rotationSpeed == -1) _rotationSpeed = rotationSpeed;
        activeRotation = StartCoroutine(RotateSmoothCoroutine(_lookRotation, _rotationSpeed));
    }

    IEnumerator RotateSmoothCoroutine(Quaternion _lookRotation, float _rotationSpeed)
    {
        isUpright = false;
        float timeCount = 0f;
        float slerpProgress = 0f;
        Quaternion startRotation = body.rotation;
        float totalRotDegrees = Quaternion.Angle(_lookRotation, startRotation);

        while (slerpProgress < 1)
        {
            timeCount += Time.deltaTime;

            // rotate by rotationSpeed divided by the total number of degrees of rotation that will occur, multipled by time
            slerpProgress = (timeCount * _rotationSpeed) / (totalRotDegrees);

            body.rotation = Quaternion.Slerp(startRotation, _lookRotation, slerpProgress);

            yield return new WaitForEndOfFrame();
        }
        body.rotation = _lookRotation;
        isUpright = true;
    }
}
