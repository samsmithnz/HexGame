using UnityEngine;

// Attach this script to your Camera GameObject
public class CameraControl : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float panSpeed = 10f; // Speed for edge panning
    public int edgeSize = 20; // Pixels from edge to start panning
    public float minX = -100f;//-28f;
    public float maxX = 100f;//0f;
    public float minZ = -100f;//-28f;
    public float maxZ = 100f;//0f;

    private void Start()
    {
        // Set top-down angle and rotation
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }

    void Update()
    {
        // Use world axes for top-down camera movement
        Vector3 move = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            move += new Vector3(0, 0, 1); // Move forward (up)
        }
        if (Input.GetKey(KeyCode.S))
        {
            move += new Vector3(0, 0, -1); // Move backward (down)
        }
        if (Input.GetKey(KeyCode.A))
        {
            move += new Vector3(-1, 0, 0); // Move left
        }
        if (Input.GetKey(KeyCode.D))
        {
            move += new Vector3(1, 0, 0); // Move right
        }

        if (move != Vector3.zero)
        {
            move.Normalize();
        }

        // Calculate the new position
        Vector3 newPos = transform.position + move * moveSpeed * Time.deltaTime;

        // Clamp x and z to bounds, y always 10
        newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        newPos.z = Mathf.Clamp(newPos.z, minZ, maxZ);
        newPos.y = 10f;

        transform.position = newPos;
    }
}
