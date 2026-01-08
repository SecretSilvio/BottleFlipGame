using UnityEngine;

public class RandomFlip : MonoBehaviour
{
    public GameObject targetObject;
    public float flipStrength;
    public float torqueStrengthX;
    public float torqueStrengthY;
    public float torqueStrengthZ;

    public void Flip()
    {
        Vector3 randomTorque = new Vector3(
            torqueStrengthX,
            torqueStrengthY,
            torqueStrengthZ
        );
        if (targetObject != null)
        {
            Rigidbody rb = targetObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(Vector3.up * flipStrength, ForceMode.Impulse);
                rb.AddTorque(randomTorque, ForceMode.Impulse);
            }
        }
    }
}
