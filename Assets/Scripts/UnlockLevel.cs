using UnityEngine;

public class UnlockLevel : MonoBehaviour
{
    public int levelToUnlock; // The level this trigger will unlock
    public static bool[] levelsUnlocked = new bool[4] { true, false, false,false }; // Level 1 is always unlocked

    private void OnTriggerEnter(Collider other)
    {
       
        levelsUnlocked[levelToUnlock - 1] = true; // Unlock the specific level
        Debug.Log("Level " + levelToUnlock + " unlocked by " + other.name);
    }
}

