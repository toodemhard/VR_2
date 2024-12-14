using UnityEngine;
using UnityEngine.SceneManagement;

public class Teleport : MonoBehaviour
{
    private bool isPlayerInRange = false;
    public string sceneName = "Room1"; // The name of the scene you want to load
    public Vector3 targetPosition = new Vector3(-0.65f, 2, 8); // The coordinates in the new scene

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            // Debug.Log("E key pressed");
            ChangeScene();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            // Debug.Log("Player is in range");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            //Debug.Log("Player is out of range");
        }
    }

    private void ChangeScene()
    {
        Debug.Log("Changing scene to: " + sceneName);
        // Save the target position to a static variable
        PlayerLocation.targetPosition = targetPosition;
        SceneManager.LoadScene(sceneName); // Load the specified scene
    }
}