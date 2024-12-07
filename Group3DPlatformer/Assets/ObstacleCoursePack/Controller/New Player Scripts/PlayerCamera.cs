using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform player;             // Reference to the player object
    public float followSpeed = 5f;       // Speed of camera follow
    public Vector3 offset = new Vector3(0f, 5f, -10f);  // Default camera offset

    private void FixedUpdate()
    {
        FollowPlayer();
    }

    private void FollowPlayer()
    {
        // Calculate the target position based on the player's position and offset
        Vector3 targetPosition = player.position + offset;

        // Smoothly move the camera to the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.fixedDeltaTime * followSpeed);

        // Optionally, make the camera look at the player
        transform.LookAt(player);
    }
}
