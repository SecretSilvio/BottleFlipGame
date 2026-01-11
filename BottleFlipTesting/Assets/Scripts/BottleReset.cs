using System.Collections;
using UnityEngine;

public class BottleReset : MonoBehaviour
{
    public Transform resetPosition;
    public GameObject bottleObject;
    private Rigidbody rb;
    private SuccessCheck successCheck;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        resetPosition.position = bottleObject.transform.position;
        resetPosition.rotation = bottleObject.transform.rotation;
        rb = bottleObject.GetComponent<Rigidbody>();
        successCheck = bottleObject.GetComponent<SuccessCheck>();
    }

    public void Reset()
    {
        StartCoroutine(ResetCoroutine());
    }

    IEnumerator ResetCoroutine()
    {
        rb.isKinematic = true;
        successCheck.previousGrounded = true;
        successCheck.timer = 1f;
        bottleObject.transform.position = resetPosition.position;
        bottleObject.transform.rotation = resetPosition.rotation;

        yield return new WaitForSeconds(0.2f);
        rb.isKinematic = false;
    }
}
