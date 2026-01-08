using UnityEngine;

public class DotProductWater : MonoBehaviour
{
    public Rigidbody bottleRb;
    public float uprightTorqueStrength = 5f;
    public float freeFallThreshold = 2.0f; // m/s^2, tweak as needed
    public float dragAlignmentExponent = 3.0f; // Higher = more sensitive to being upright
    public float maxDrag = 4.0f; // Maximum angular drag when upright
    public float torqueAlignmentExponent = 4.0f; // Exponent for torque scaling

    private Vector3 prevVelocity;

    void Awake()
    {
        if (!bottleRb)
            bottleRb = GetComponent<Rigidbody>();
        prevVelocity = bottleRb.linearVelocity;
    }

    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        // Calculate bottle acceleration in world space
        Vector3 velocity = bottleRb.linearVelocity;
        Vector3 acceleration = (velocity - prevVelocity) / dt;
        prevVelocity = velocity;

        // Check if bottle is in free fall (acceleration close to gravity)
        Vector3 worldGravity = Vector3.down * Physics.gravity.magnitude;
        bool isFreeFalling = velocity.y < freeFallThreshold;

        // Alignment of bottle's base (down) with world down
        Vector3 bottleDown = -transform.up;
        float alignment = Mathf.Clamp01(Vector3.Dot(bottleDown, Vector3.down)); // 1 = vertical, 0 = horizontal

        // Exponential scaling for drag
        float dragScale = Mathf.Pow(alignment, dragAlignmentExponent);

        // Set angular drag and torque based on alignment and free fall
        if (isFreeFalling)
        {
            bottleRb.angularDamping = Mathf.Lerp(0.05f, 4f, dragScale); // Adjust min/max as needed

            Vector3 torqueAxis = Vector3.Cross(bottleDown, Vector3.down);
            float torqueScale = Mathf.Pow(alignment, torqueAlignmentExponent);
            Vector3 uprightTorque = torqueAxis.normalized * torqueScale * uprightTorqueStrength;
            if (torqueAxis.sqrMagnitude > 1e-4f)
                bottleRb.AddTorque(uprightTorque, ForceMode.Force);
        }
        else
        {
            bottleRb.angularDamping = 0.05f; // Default when not in free fall
        }
    }
}
