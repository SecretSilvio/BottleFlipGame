using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DragAndDrop : MonoBehaviour
{
    Vector3 mousePos;
    Vector2 startPos;
    Vector2 endPos;
    float dragStartTime;
    float dragEndTime;

    public float launchForceMultiplier = 10f; // Tweak as needed
    public Rigidbody rb; // Assign in inspector or via GetComponent

    private void OnMouseDown()
    {
        mousePos = Input.mousePosition;
        startPos = new Vector2(mousePos.x, mousePos.y);
        dragStartTime = Time.unscaledTime;
    }

    private void OnMouseDrag()
    {
        // Optional: Visual feedback or object movement
    }

    private void OnMouseUp()
    {
        mousePos = Input.mousePosition;
        endPos = new Vector2(mousePos.x, mousePos.y);
        dragEndTime = Time.unscaledTime;

        Vector2 dragVector = endPos - startPos;
        float dragDuration = Mathf.Max(dragEndTime - dragStartTime, 0.01f); // Prevent divide by zero

        // Calculate launch direction and force
        Vector3 launchDir = new Vector3(dragVector.x, dragVector.y, 0).normalized;
        float launchSpeed = dragVector.magnitude / dragDuration;
        Vector3 launchForce = launchDir * launchSpeed * launchForceMultiplier;

        if (rb == null)
            rb = GetComponent<Rigidbody>();

        rb.AddForce(launchForce, ForceMode.Impulse);
    }
}