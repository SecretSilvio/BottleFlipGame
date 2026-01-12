using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UICircleWobble : MonoBehaviour
{
    public float strength = 5f;
    public float speed = 2f;

    private RectTransform rect;
    private Vector3 baseScale;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        baseScale = rect.localScale; // store original size
    }

    void Update()
    {
        float wobble = Mathf.Sin(Time.time * speed) * strength * 0.01f;
        rect.localScale = baseScale * (1 + wobble);
    }
}