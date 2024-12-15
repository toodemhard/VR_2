using UnityEngine;

public class Sensor : MonoBehaviour
{
    public bool Innit;

    void OnTriggerEnter(Collider collider) {
        Innit = true;
    }

    void OnTriggerExit(Collider collider) {
        Innit = false;
    }
}
