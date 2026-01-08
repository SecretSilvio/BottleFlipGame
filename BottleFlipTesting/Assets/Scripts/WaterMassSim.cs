using UnityEngine;

public class WaterMassSim : MonoBehaviour
{
    [Header("References")]
    public Rigidbody bottleRb;

    [Header("Water Setup")]
    [Range(1, 32)]
    public int nodeCount = 10;

    public float waterMass = 0.5f;
    [Range(0f, 1f)]
    public float fillLevel = 0.5f;

    [Header("Slosh Forces")]
    public float gravityStrength = 9.81f;
    public float inertiaStrength = 2.0f;

    [Header("Damping")]
    public float damping = 0.96f;

    [Header("Water Volume Behavior")]
    public float restSpacing = 0.02f;
    public float repulsionStrength = 6f;

    [Header("Bottle Interior (Local Space)")]
    public float radius = 0.035f;
    public float height = 0.18f;

    // Used to track movement of walls / bottle
    private Vector3 prevPosition;
    private Quaternion prevRotation;

    // Internal state
    Vector3[] nodePositions;
    Vector3[] nodeVelocities;

    Vector3 baseCenterOfMass;

    void Awake()
    {
        prevPosition = transform.position;
        prevRotation = transform.rotation;

        if (!bottleRb)
            bottleRb = GetComponentInParent<Rigidbody>();

        baseCenterOfMass = bottleRb.centerOfMass;

        nodePositions = new Vector3[nodeCount];
        nodeVelocities = new Vector3[nodeCount];

        // Initialize nodes near bottom
        for (int i = 0; i < nodeCount; i++)
        {
            Vector2 r = Random.insideUnitCircle * radius * 0.8f;
            nodePositions[i] = new Vector3(
                r.x,
                Random.Range(-height * 0.5f, -height * 0.5f + fillLevel * height),
                r.y
            );
        }
    }

    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        // --- 1. Orientation-aware gravity ---
        Vector3 worldGravity = Vector3.down * gravityStrength;
        Vector3 localGravity = transform.InverseTransformDirection(worldGravity);

        // --- 2. Bottle angular velocity in local space ---
        Vector3 localAngularVel = transform.InverseTransformDirection(bottleRb.angularVelocity);

        // --- 3. Apply gravity + inertia to each node ---
        for (int i = 0; i < nodeCount; i++)
        {
            nodeVelocities[i] += localGravity * dt;
            nodeVelocities[i] -= Vector3.Cross(localAngularVel, nodePositions[i]) * inertiaStrength * dt;
        }

        // --- 4. Node-to-node repulsion ---
        for (int i = 0; i < nodeCount; i++)
        {
            for (int j = i + 1; j < nodeCount; j++)
            {
                Vector3 delta = nodePositions[i] - nodePositions[j];

                // Ignore vertical pressure for sideways spreading
                //delta.y = 0f;

                float dist = delta.magnitude;
                if (dist < restSpacing && dist > 0.0001f)
                {
                    Vector3 push = delta.normalized * (restSpacing - dist) * repulsionStrength * dt;
                    nodeVelocities[i] += push;
                    nodeVelocities[j] -= push;
                }
            }
        }

        // --- 5. Damping ---
        for (int i = 0; i < nodeCount; i++)
        {
            nodeVelocities[i] *= damping;
        }

        // --- 6. Integrate positions and clamp inside bottle ---
        Vector3 avgPos = Vector3.zero;
        for (int i = 0; i < nodeCount; i++)
        {
            nodePositions[i] += nodeVelocities[i] * dt;
            Vector3 unclamped = nodePositions[i];
            Vector3 clamped = ClampToBottle(unclamped);

            if (unclamped != clamped)
            {
                nodeVelocities[i] = GetWallVelocity(clamped);
                nodePositions[i] = clamped;
            }
            else
            {
                nodePositions[i] = clamped;
            }
            avgPos += nodePositions[i];
        }
        avgPos /= nodeCount;

        // --- 7. Update Rigidbody center of mass ---
        float effectiveWaterMass = waterMass * fillLevel;
        bottleRb.centerOfMass = baseCenterOfMass + avgPos * effectiveWaterMass;

        // --- 8. Apply torque from water inertia ---
        Vector3 worldWaterPos = transform.TransformPoint(avgPos);
        Vector3 leverArm = worldWaterPos - bottleRb.worldCenterOfMass;
        Vector3 torque = Vector3.Cross(leverArm, localGravity) * effectiveWaterMass;
        bottleRb.AddTorque(torque, ForceMode.Force);
        prevPosition = transform.position;
        prevRotation = transform.rotation;
    }

    Vector3 ClampToBottle(Vector3 p)
    {
        // Clamp radial distance
        Vector2 r = new Vector2(p.x, p.z);
        if (r.magnitude > radius)
            r = r.normalized * radius;

        p.x = r.x;
        p.z = r.y;

        // Clamp vertical along local Y
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
        if (nodePositions == null) return;
        Gizmos.color = Color.cyan;
        foreach (var p in nodePositions)
        {
            Gizmos.DrawSphere(transform.TransformPoint(p), 0.1f);
        }
    }
#endif
}
