using UnityEngine;

public class SpaceWormIdle : MonoBehaviour
{
    [Header("Body Segments")]
    public Transform[] segments;

    [Header("Wriggle Settings")]
    public float waveAmplitude = 0.2f; // how far it moves
    public float waveFrequency = 2f;   // wave density
    public float waveSpeed = 2f;       // speed of wriggle

    [Header("Floating Settings")]
    public float floatAmplitude = 0.1f;
    public float floatFrequency = 1f;

    private Vector3[] basePositions;

    void Start()
    {
        // Store the starting local positions of each segment
        basePositions = new Vector3[segments.Length];
        for (int i = 0; i < segments.Length; i++)
        {
            basePositions[i] = segments[i].localPosition;
        }
    }

    void Update()
    {
        float t = Time.time * waveSpeed;

        for (int i = 0; i < segments.Length; i++)
        {
            float offset = i * waveFrequency * 0.3f;
            float y = Mathf.Sin(t + offset) * waveAmplitude;
            float x = Mathf.Cos(t + offset) * waveAmplitude * 0.5f;
            // Floating (whole worm bobbing gently)
            float floatY = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;

            segments[i].localPosition = basePositions[i] + new Vector3(x, y + floatY, 0);
        }
    }
}
