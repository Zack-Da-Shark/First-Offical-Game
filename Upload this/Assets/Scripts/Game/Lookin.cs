using UnityEngine;
using UnityEngine.InputSystem;

public class Lookin : MonoBehaviour
{
    private Camera cam;

    void Start()
    {
        Debug.Log("Player looking at mouse");
        cam = Camera.main;
    }
    
    void Update()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mousePos = cam.ScreenToWorldPoint(mouseScreenPos);

        float angleRad = Mathf.Atan2(
            mousePos.y - transform.position.y,
            mousePos.x - transform.position.x
        );

        float angleDeg = angleRad * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angleDeg);
    }
}
