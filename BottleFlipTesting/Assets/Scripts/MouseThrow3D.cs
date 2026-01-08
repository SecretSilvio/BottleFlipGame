using UnityEngine;

public class ScreenDrawThrow3D : MonoBehaviour
{
    [Header("Flick Thresholds")]
    public float minFlickSpeed = 300f;      // pixels per second
    public float minFlickDistance = 60f;    // pixels
    public float maxFlickSpeed = 2000f;     // pixels per second

    [Header("Force Ranges")]
    public float minXForce = 1f;
    public float maxXForce = 6f;

    public float minYForce = 3f;
    public float maxYForce = 10f;

    public float forwardForce = 5f;

    [Header("Torque (Spin)")]
    public float minTorque = 1f;
    public float maxTorque = 8f;

    private Vector2 flickStartPos;
    private float flickStartTime;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            flickStartPos = Input.mousePosition;
            flickStartTime = Time.time;
        }

        if (Input.GetMouseButtonUp(0))
        {
            Vector2 flickEndPos = Input.mousePosition;
            float flickDuration = Mathf.Max(Time.time - flickStartTime, 0.01f);

            Vector2 flickDelta = flickEndPos - flickStartPos;
            float flickDistance = flickDelta.magnitude;
            Vector2 flickVelocity = flickDelta / flickDuration;
            float flickSpeed = flickVelocity.magnitude;

            // Ignore accidental input
            if (flickSpeed < minFlickSpeed) return;
            if (flickDistance < minFlickDistance) return;

            float t = Mathf.InverseLerp(minFlickSpeed, maxFlickSpeed, flickSpeed);

            Throw(flickDelta.normalized, t);
        }
    }

    void Throw(Vector2 direction, float t)
    {
        float xForce = Mathf.Lerp(minXForce, maxXForce, t) * direction.x;
        float yForce = Mathf.Lerp(minYForce, maxYForce, t) * direction.y;

        Vector3 force =
            Camera.main.transform.right * xForce +
            Vector3.up * yForce +
            Camera.main.transform.forward * forwardForce;

        Vector3 flatForce = new Vector3(force.x, 0f, force.z);

        if (flatForce.sqrMagnitude > 0.001f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(-flatForce.normalized, Vector3.up);
            transform.rotation = lookRotation;
        }

        float torqueStrength = Mathf.Lerp(minTorque, maxTorque, t);
        Vector3 torque = transform.right * torqueStrength;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.AddForce(force, ForceMode.Impulse);
        rb.AddTorque(torque, ForceMode.Impulse);
    }
}