using UnityEngine;

public class LightControl : MonoBehaviour
{
    public int levelToWatch; // The level that this light is watching for
    private Light pointLight; // Reference to the Point Light component

    void Start()
    {
        // Finding the Point Light component in the child
        pointLight = GetComponentInChildren<Light>();

        // Initial check to set the light color based on current unlock status
        if (UnlockLevel.levelsUnlocked[levelToWatch - 1])
        {
            pointLight.color = Color.green; // Set to green if unlocked
        }
        else
        {
            pointLight.color = Color.red; // Default color if not unlocked (or any color you prefer)
        }
    }

    void Update()
    {
        // Continuously check if the level is unlocked and update the light color
        if (UnlockLevel.levelsUnlocked[levelToWatch - 1])
        {
            pointLight.color = Color.green; // Set to green if unlocked
        }
        else
        {
            pointLight.color = Color.red; // Default color if not unlocked
        }
    }
}
