using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] float MoveSpeed;
    [SerializeField] float AimSensitivity;
    [SerializeField] GameObject Cam;
    [SerializeField] Transform Feet;
    [SerializeField] GameObject TerminalUI;
    // [SerializeField] Transform Hand;
    // [SerializeField] GameObject KeyPrefab;
    // [SerializeField] GameObject KeyRigidbodyPrefab;
    // [SerializeField] GameObject Text;
    Rigidbody rb;

    LayerMask terminalMask;
    LayerMask itemMask;
    LayerMask groundMask;

    float rotationX;

    [SerializeField] float Gravity = -9.81f;
    [SerializeField] float JumpVelocity = 8f;
    [SerializeField] float ThrowForce = 10;

    [SerializeField] float TextDuration = 1;
    float lastInteract = -100;
    

    float velocityY = 0;

    CharacterController controller;

    bool isGroundedLastFrame = false;

    bool hasKey = false;

    void Awake() {
        terminalMask = LayerMask.GetMask("Terminal");
        groundMask = LayerMask.GetMask("Default");
        // itemMask = LayerMask.GetMask("Item");

        rb = GetComponent<Rigidbody>();
        controller = GetComponent<CharacterController>();


        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        TerminalUI.SetActive(false);
    }

    void FixedUpdate()
    {

    }

    void Update() {
        // Cam.transform.localRotation *= Quaternion.AngleAxis(, Vector3.right);





        if (Input.GetKeyDown(KeyCode.E)) {
            if (Physics.Raycast(Cam.transform.position, Cam.transform.forward, out var hit, 6, terminalMask)) {
                TerminalUI.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                // var door = hit.transform.GetComponentInParent<Door>();

                // if (door.locked) {
                //     if (hasKey) {
                //         door.locked = false;
                //         var key = Hand.GetChild(0);
                //         key.transform.parent = door.Keyhole.transform;
                //         key.transform.localPosition = Vector3.zero;
                //         key.transform.localRotation = Quaternion.identity;
                //         hasKey = false;
                //     } else { 
                //         lastInteract = Time.time;
                //     }

                // } else {
                //     door.Toggle();
                // }

            }

            // if (Physics.Raycast(Cam.transform.position, Cam.transform.forward, out hit, 10, itemMask)) {
            //     Destroy(hit.transform.gameObject);

            //     Instantiate(KeyPrefab, Hand);
            //     hasKey = true;
            // }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && TerminalUI.activeSelf) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            TerminalUI.SetActive(false);
        }

        // if (Time.time - lastInteract < TextDuration) {
        //     Text.SetActive(true);
        // } else {
        //     Text.SetActive(false);
        // }

        // if (Input.GetKeyDown(KeyCode.G)) {
        //     if (hasKey) {
        //         var keyRigidbody = Instantiate(KeyRigidbodyPrefab, Cam.transform.position + Cam.transform.forward * 0.5f, Quaternion.identity);
        //         keyRigidbody.GetComponentInChildren<Rigidbody>().AddForce(Cam.transform.forward * ThrowForce, ForceMode.Impulse);

        //         Destroy(Hand.GetChild(0).gameObject);

        //         hasKey = false;
        //     }
        // }
        if (TerminalUI.activeSelf) {
            return;
        }

        var rotationY = Input.GetAxisRaw("Mouse X") * AimSensitivity;
        transform.Rotate(new Vector3(0, rotationY, 0), Space.Self);

        rotationX += -Input.GetAxisRaw("Mouse Y") * AimSensitivity;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        Cam.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);


        bool isGrounded = false;

        {
            if (Physics.Raycast(Feet.position, Vector3.down, out var hit, 0.2f, groundMask))
            {
                isGrounded = true;
            }
        }
        // Debug.Log(isGrounded);


        if (!isGrounded) {
            velocityY += Gravity * Time.deltaTime;
            if (isGroundedLastFrame) {
                // velocityY = 0;
            }

            isGroundedLastFrame = false;
        } else {
            if (velocityY < 0) {
                velocityY = 0;
            }
            isGroundedLastFrame = true;
            if (Input.GetKeyDown(KeyCode.Space)) { 
                velocityY = JumpVelocity;

            }
        }


        Vector3 moveInput =  (transform.forward * Input.GetAxisRaw("Vertical") + transform.right * Input.GetAxisRaw("Horizontal")).normalized;

        Vector3 move = Vector3.zero;
        move += moveInput * MoveSpeed * Time.deltaTime;

        controller.Move(move);
        controller.Move(Vector3.up * velocityY * Time.deltaTime);

        // Rigidbody.linearVelocity += Vector3.zero;

        // Vector3 moveWorld = Vector3.zero;


        // // Vector3.ProjectOnPlane()
        // moveWorld += transform.forward * move.z;
        // moveWorld += transform.right * move.x;
        // moveWorld *= MoveSpeed; //* Time.fixedDeltaTime;
        // moveWorld.y = Rigidbody.linearVelocity.y;
        // Rigidbody.linearVelocity = moveWorld;

    }
}
