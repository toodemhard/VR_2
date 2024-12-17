using TMPro;
using UnityEngine;

public class UnlockLevel : MonoBehaviour
{
    [SerializeField]
    Robot robot;
    [SerializeField]
    int levelToUnlock; // The level this trigger will unlock

    [SerializeField]
    TMP_Text text;
    public static bool[] levelsUnlocked = new bool[4] { true, false, false,false }; // Level 1 is always unlocked

    private void Awake() {
        if (text != null) {
            text.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
       
        levelsUnlocked[levelToUnlock - 1] = true; // Unlock the specific level
        if (robot != null) {
            robot.isRunning = false;

        }

        if (text != null) {
            if (levelToUnlock == levelsUnlocked.Length) {
                text.text = $"Credits Unlocked";
            } else {
                text.text = $"Level {levelToUnlock} Unlocked!";
            }
            text.gameObject.SetActive(true);
        }

        Debug.Log("Level " + levelToUnlock + " unlocked by " + other.name);
    }
}

