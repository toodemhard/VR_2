using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToLobby : MonoBehaviour
{
    public Vector3 lobbyPosition = new Vector3(509, 2, 516); // The coordinates in the Lobby scene

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Setting the target position for the player in the Lobby scene
            PlayerLocation.targetPosition = lobbyPosition;
            SceneManager.LoadScene("Lobby");
        }
    }
}
