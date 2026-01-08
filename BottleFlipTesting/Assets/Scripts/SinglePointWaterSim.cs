using UnityEngine;

public class SinglePointWaterSim : MonoBehaviour
{
    [Header("References")]
    public Rigidbody bottleRb;

    [Header("Water Properties")]
    public float waterMass = 0.5f;
    [Range(0f, 1f)]
    public float fillLevel = 0.5f;

    [Header("Motion")]
    public float gravityStrength = 9.81f;
    public float inertiaStrength = 2.0f;
    public float torqueStrength = 0.1f;
    public float damping = 0.95f;

    [Header("Bottle Interior (Local Space)")]
    public float radius = 0.035f;
    public float height = 0.18f;

    // Internal state
    private Vector3 waterPosWorld; // now in world space
    private Vector3 waterVelWorld; // now in world space
    private Vector3 baseCenterOfMass;

    private Vector3 prevPosition;
    private Quaternion prevRotation;

    void Awake()
    {
        if (!bottleRb)
            bottleRb = GetComponentInParent<Rigidbody>();

        baseCenterOfMass = bottleRb.centerOfMass;

        prevPosition = transform.position;
        prevRotation = transform.rotation;

        // Start near the bottom, based on fill level (in local space, then convert to world)
        Vector3 localStart = new Vector3(
            0f,
            -height * 0.5f + fillLevel * height,
            0f
        );
        waterPosWorld = transform.TransformPoint(localStart);
        waterVelWorld = Vector3.zero;
    }

    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        // --- Gravity in world space ---
        Vector3 worldGravity = Vector3.down * gravityStrength;

        // --- Angular inertia (sloshing lag) in world space ---
        Vector3 localWaterPos = transform.InverseTransformPoint(waterPosWorld);
        Vector3 localAngularVel = transform.InverseTransformDirection(bottleRb.angularVelocity);
        Vector3 inertiaForceLocal = -Vector3.Cross(localAngularVel, localWaterPos) * inertiaStrength;
        Vector3 inertiaForceWorld = transform.TransformDirection(inertiaForceLocal);

        // --- Apply forces in world space ---
        waterVelWorld += worldGravity * dt;
        waterVelWorld += inertiaForceWorld * dt;

        // --- Damping ---
        waterVelWorld *= damping;

        // --- Integrate ---
        waterPosWorld += waterVelWorld * dt;

        // --- Clamp inside bottle ---
        Vector3 unclampedWorld = waterPosWorld;
        Vector3 localPos = transform.InverseTransformPoint(waterPosWorld);
        Vector3 clampedLocal = ClampToBottle(localPos);
        waterPosWorld = transform.TransformPoint(clampedLocal);

        // If we hit a wall, inherit bottle motion
        if (localPos != clampedLocal)
        {
            waterVelWorld = GetWallVelocity(clampedLocal);
        }

        // --- Update center of mass ---
        float effectiveMass = waterMass * fillLevel;
        Vector3 localCoM = transform.InverseTransformPoint(waterPosWorld);
        bottleRb.centerOfMass = baseCenterOfMass + localCoM * effectiveMass;

        // --- Apply slosh torque ---
        Vector3 leverArm = waterPosWorld - bottleRb.worldCenterOfMass;
        Vector3 torque = Vector3.Cross(leverArm, worldGravity) * (effectiveMass * torqueStrength);
        bottleRb.AddTorque(torque, ForceMode.Force);

        prevPosition = transform.position;
        prevRotation = transform.rotation;
    }

    Vector3 ClampToBottle(Vector3 p)
    {
        // Radial clamp (X/Z)
        Vector2 r = new Vector2(p.x, p.z);
        if (r.magnitude > radius)
            r = r.normalized * radius;

        p.x = r.x;
        p.z = r.y;

        // Vertical clamp (Y)
        float minY = -height * 0.5f;
        float maxY = height * 0.5f;
        p.y = Mathf.Clamp(p.y, minY, maxY);

        return p;
    }

    Vector3 GetWallVelocity(Vector3 localPoint)
    {
        Vector3 worldPoint = transform.TransformPoint(localPoint);
        Vector3 prevWorldPoint = prevRotation * localPoint + prevPosition;
        return (worldPoint - prevWorldPoint) / Time.fixedDeltaTime;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(waterPosWorld, 0.1f);
        Gizmos.DrawCube(transform.TransformPoint(bottleRb.centerOfMass), Vector3.one * 0.2f);
    }
#endif
}