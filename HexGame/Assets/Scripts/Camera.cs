using UnityEngine;

// Attach this script to your Camera GameObject
public class Camera : MonoBehaviour
{
    public float moveSpeed = 10f;
    public Vector2 minBounds = new Vector2(-50, -50);
    public Vector2 maxBounds = new Vector2(50, 50);
    public float isoAngle = 30f; // Degrees from ground
    public float isoRotation = 45f; // Yaw rotation for isometric view

    private void Start()
    {
        // Set isometric angle and rotation
        transform.rotation = Quaternion.Euler(isoAngle, isoRotation, 0);
    }

    void Update()
    {
        Vector3 move = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
            move += new Vector3(1, 0, 1); // Forward in isometric
        if (Input.GetKey(KeyCode.S))
            move += new Vector3(-1, 0, -1); // Backward
        if (Input.GetKey(KeyCode.A))
            move += new Vector3(-1, 0, 1); // Left
        if (Input.GetKey(KeyCode.D))
            move += new Vector3(1, 0, -1); // Right

        move.Normalize();
        transform.position += move * moveSpeed * Time.deltaTime;

        // Clamp camera position to bounds
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minBounds.x, maxBounds.x);
        pos.z = Mathf.Clamp(pos.z, minBounds.y, maxBounds.y);
        transform.position = pos;
    }
}
