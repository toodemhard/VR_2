using System.Data.Common;
using UnityEngine;
using UnityEngine.AI;

public class Laser : MonoBehaviour
{
    [SerializeField]
    float MinInterval = 2;
    float MaxInterval = 5;

    float offDuration = 3;


    MeshRenderer meshRenderer;

    void Start() {
        
    }

    void NextOffTime() {

    }
    void Update() {
        if (gameObject.activeSelf) {

        } else {

        }
    }

    void OnTriggerEnter(Collider collider) {
        if (collider.CompareTag("Robot")) {
            collider.gameObject.SetActive(false);
        }
    }
}
