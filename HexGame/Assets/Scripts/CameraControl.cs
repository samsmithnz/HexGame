using UnityEngine;

// Attach this script to your Camera GameObject
public class CameraControl : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float panSpeed = 10f; // Speed for edge panning
    public int edgeSize = 20; // Pixels from edge to start panning
    public float minX = -28f;
    public float maxX = 0f;
    public float minZ = -28f;
    public float maxZ = 0f;
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
        {
            move += new Vector3(1, 0, 1); // Forward in isometric
        }
        if (Input.GetKey(KeyCode.S))
        {
            move += new Vector3(-1, 0, -1); // Backward
        }
        if (Input.GetKey(KeyCode.A))
        {
            move += new Vector3(-1, 0, 1); // Left
        }
        if (Input.GetKey(KeyCode.D))
        {
            move += new Vector3(1, 0, -1); // Right
        }

        //// Edge panning
        //Vector3 edgeMove = Vector3.zero;
        //Vector3 mousePos = Input.mousePosition;
        //if (mousePos.x <= edgeSize)
        //{
        //    edgeMove += new Vector3(-1, 0, 1); // Pan left
        //}
        //if (mousePos.x >= Screen.width - edgeSize)
        //{
        //    edgeMove += new Vector3(1, 0, -1); // Pan right
        //}
        //if (mousePos.y <= edgeSize)
        //{
        //    edgeMove += new Vector3(-1, 0, -1); // Pan down
        //}
        //if (mousePos.y >= Screen.height - edgeSize)
        //{
        //    edgeMove += new Vector3(1, 0, 1); // Pan up
        //}

        //if (edgeMove != Vector3.zero)
        //{
        //    edgeMove.Normalize();
        //    move += edgeMove * (panSpeed / moveSpeed); // Keep panning speed consistent
        //}

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
