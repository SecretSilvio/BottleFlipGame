using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(TrailRenderer))]
public class FollowCursor : MonoBehaviour
{
    TrailRenderer trail;

    void Awake()
    {
        trail = GetComponent<TrailRenderer>();
        trail.emitting = false; // start off
    }

    void Start()
    {
        Cursor.visible = true;
    }

    void Update()
    {
        if (Mouse.current == null) return;

        // Follow mouse
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(
            new Vector3(mousePos.x, mousePos.y, -Camera.main.transform.position.z)
        );
        worldPos.z = 0f;
        transform.position = worldPos;

        // Emit trail while mouse button is held
        trail.emitting = Mouse.current.leftButton.isPressed;
    }
}