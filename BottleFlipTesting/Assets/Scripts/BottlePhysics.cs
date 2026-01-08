using UnityEngine;
using UnityEngine.UIElements;

public class BottlePhysics : MonoBehaviour
{
    [SerializeField] private Vector3 centerOfGravityOffset;
    [SerializeField] private float cogYOffset;

    public GameObject[] waterBalls;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        waterBalls = GameObject.FindGameObjectsWithTag("WaterBall");
        // Set center of gravity near the base in local space (Y axis points up in local space)
        centerOfGravityOffset = new Vector3(0, cogYOffset * transform.localScale.y, 0); // Adjust -0.45f as needed
        rb.centerOfMass = centerOfGravityOffset;
        Debug.Log("Center of Gravity set to: " + rb.centerOfMass);
    }

    private void LateUpdate()
    {
        Vector3 sum = Vector3.zero;
        foreach (GameObject waterBall in waterBalls)
        {
            sum += waterBall.transform.position;
        }

        Vector3 averagePos = sum / waterBalls.Length;
        Vector3 localAveragePos = transform.InverseTransformPoint(averagePos);
        rb.centerOfMass = localAveragePos + centerOfGravityOffset;
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Vector3 worldCenterOfGravity = transform.TransformPoint(rb.centerOfMass);
    //    Gizmos.DrawSphere(worldCenterOfGravity, 0.05f);
    //}
}
