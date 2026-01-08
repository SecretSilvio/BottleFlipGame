using UnityEngine;

public class BottleReset : MonoBehaviour
{
    public Transform resetPosition;
    public GameObject bottleObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        resetPosition.position = bottleObject.transform.position;
        resetPosition.rotation = bottleObject.transform.rotation;
    }

    public void Reset()
    {
        bottleObject.transform.position = resetPosition.position;
        bottleObject.transform.rotation = resetPosition.rotation;
    }
}
