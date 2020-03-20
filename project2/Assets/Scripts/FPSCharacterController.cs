using UnityEngine;
using TMPro;

[RequireComponent(typeof(CharacterController))]
public class FPSCharacterController : MonoBehaviour {

    // Camera movement
    public Transform cameraT;
    public float mouseSensitivity = 100.0f;
    private float hLook = 0.0f;
    private float vLook = 0.0f;
    private float hRecoil = 0.0f;
    private float vRecoil = 0.0f;
    private float recoilReturnSpeed;
    private float fireV = 0.0f;

    // Movement
    private CharacterController charController;
    public float walkSpeed = 4.5f;
    public float runSpeed = 8.0f;

    // GUI
    public GameObject inventory;
    public GameObject crosshair;

    void Start() {
        // Lock and hide cursor at center of screen
        Cursor.lockState = CursorLockMode.Locked;

        // Get Character Controller component
        this.charController = GetComponent<CharacterController>();

        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Casings"));
    }

    void Update() {
        // Camera movement
        if (!inventory.activeSelf) {
            float hCameraMovement = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float vCameraMovement = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
            hLook += hCameraMovement;
            vLook -= vCameraMovement;
            vLook = Mathf.Clamp(vLook, -75, 75);
            transform.localRotation = Quaternion.Euler(vLook - vRecoil, hLook - hRecoil, 0.0f);
        }

        // Player movement
        float hMovement = Input.GetAxis("Horizontal");
        float vMovement = Input.GetAxis("Vertical");
        Vector3 movement = transform.right * hMovement + transform.forward * vMovement;
        if (Input.GetButton("Run")) {
            // Running
            charController.SimpleMove(movement * runSpeed);
        } else {
            // Walking
            charController.SimpleMove(movement * walkSpeed);
        }

        // Open inventory
        if (Input.GetButtonDown("Inventory")) {
            if (!inventory.activeSelf) {
                inventory.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
            } else {
                inventory.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        // Original vertical rotation of player camera before firing
        if (Input.GetButtonDown("Fire")) {
            fireV = transform.rotation.x;
        }

        // Change cursor lock modes
        /*
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (Cursor.lockState == CursorLockMode.Locked) {
                Cursor.lockState = CursorLockMode.None;
            } else {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
        */
    }

    void FixedUpdate() {
        // Return from recoil
        if (transform.rotation.x > fireV && !Input.GetButton("Fire")) {
            // Controlled recoil
            vLook -= vRecoil;
            hRecoil = 0;
            vRecoil = 0;
        }
        if (recoilReturnSpeed != null && vRecoil > 0 && transform.localRotation.x < fireV) {
            // Uncontrolled recoil
            vRecoil -= Time.deltaTime * recoilReturnSpeed;
            if (vRecoil < 0) {
                vRecoil = 0;
            }
        }
    }

    public void AddRecoil(float hRecoil, float vRecoil, float recoilReturnSpeed) {
        this.recoilReturnSpeed = recoilReturnSpeed;
        this.hRecoil = hRecoil;
        this.vRecoil += vRecoil;
    }

    public Transform GetCameraTransform() {
        return transform.Find("Camera");
    }

    public void ToggleCrosshair(bool showCrosshair) {
        crosshair.SetActive(showCrosshair);
    }
}
