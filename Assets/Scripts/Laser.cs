using System.Data.Common;
using UnityEngine;
using UnityEngine.AI;

public class Laser : MonoBehaviour
{
    [SerializeField]
    float MinInterval = 2;
    [SerializeField]
    float MaxInterval = 5;

    float offDuration = 3;
    float nextOffTime;
    float accumulator = 0;

    public bool On = true;
    MeshRenderer meshRenderer;

    void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
        NextOffTime();
    }

    void NextOffTime() {
        nextOffTime = Random.Range(MinInterval, MaxInterval);
    }

    void Update() {
        accumulator += Time.deltaTime;
        if (On) {
            if (accumulator >= nextOffTime) {
                On = false;
                meshRenderer.enabled = false;
                accumulator = 0;
            }
        } else {
            if (accumulator >= offDuration) {
                On = true;
                meshRenderer.enabled = true;
                accumulator = 0;
            }
        }
    }

    void OnTriggerEnter(Collider collider) {
        if (On == collider.CompareTag("Robot")) {
            collider.gameObject.SetActive(false);
        }
    }
}
