using UnityEngine;

public class PlayerLocation : MonoBehaviour
{
    public static Vector3 targetPosition; // store the target position

    void Start()
    {
       // Debug.Log("PlayerLocation Start - Target Position: " + targetPosition);

        // Check if targetPosition is set
        if (targetPosition != Vector3.zero)
        {
            // Move the player to the target position
            transform.position = targetPosition;
            targetPosition = Vector3.zero; // Resets the target position
        }
    }
}
