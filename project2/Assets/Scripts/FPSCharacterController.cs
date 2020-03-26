using System;
using System.Collections;
using TMPro;
using UnityEngine;

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
    private Menu gui;
    public GameObject healthBar;
    public GameObject hungerBar;
    public GameObject thirstBar;

    // Status
    public float health = 100.0f;
    public float hunger = 100.0f;
    public float thirst = 100.0f;
    private int statusMax = 100;

    void Start() {
        // Lock and hide cursor at center of screen
        Cursor.lockState = CursorLockMode.Locked;

        // Get Character Controller component
        this.charController = GetComponent<CharacterController>();

        gui = transform.Find("GUI").GetComponent<Menu>();

        // Ignore collisions with bullet casings
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Casings"));
    }

    void Update() {
        // Camera movement
        if (!gui.menu.activeSelf) {
            float hCameraMovement = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float vCameraMovement = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
            hLook += hCameraMovement;
            vLook -= vCameraMovement;
            vLook = Mathf.Clamp(vLook, -75, 75);
            transform.localRotation = Quaternion.Euler(vLook - vRecoil, hLook - hRecoil, 0.0f);
        }

        // Return from recoil
        if (transform.rotation.x > fireV && !Input.GetButton("Fire")) {
            // Controlled recoil
            vLook -= vRecoil;
            hRecoil = 0;
            vRecoil = 0;
        }
        if (recoilReturnSpeed != null && vRecoil > 0 && transform.localRotation.x < fireV && !Input.GetButton("Fire")) {
            // Uncontrolled recoil
            vRecoil -= Time.deltaTime * recoilReturnSpeed;
            hRecoil -= Time.deltaTime * hRecoil;
            if (vRecoil < 0) {
                vRecoil = 0;
            }
            if (hRecoil < 0) {
                hRecoil = 0;
            }
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
            if (!gui.menu.activeSelf) {
                gui.menu.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
            } else {
                gui.menu.SetActive(false);
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
        // Drain hunger
        if (hunger > 0) {
            hunger -= Time.deltaTime * 0.03f;
            if (hunger < 0) {
                hunger = 0;
            }
        }
        // Drain thirst
        if (thirst > 0) {
            thirst -= Time.deltaTime * 0.01f;
            if (thirst < 0) {
                thirst = 0;
            }
        }
        UpdateBars();
    }

    public void UpdateBars() {
        UpdateBar(hungerBar, hunger);
        UpdateBar(thirstBar, thirst);
    }

    public void TakeDamage(float damage) {
        health -= damage;
        UpdateBar(healthBar, health);
        if (health <= 0) {
            // Player died
            health = 0;
        }
    }

    public void AddRecoil(float hRecoil, float vRecoil, float recoilReturnSpeed) {
        // Add visual recoil to player camera
        this.recoilReturnSpeed = recoilReturnSpeed;
        this.hRecoil = hRecoil;
        this.vRecoil += vRecoil;
    }

    public void KillConfirm() {
        StartCoroutine(ShowKillConfirm());
    }

    private IEnumerator ShowKillConfirm() {
        gui.killConfirm.SetActive(true);

        yield return new WaitForSeconds(1.0f);

        gui.killConfirm.SetActive(false);
    }

    public Transform GetCameraTransform() {
        return transform.Find("Camera");
    }

    public void ToggleCrosshair(bool showCrosshair) {
        gui.crosshair.SetActive(showCrosshair);
    }

    private float CalculateRectRight(float currVal, int maxVal, int rectRightAtZero, int rectRightAtFull) {
        // Calculates the RectTransform's 'right' value to properly "deplete" bars such as hp bars
        return rectRightAtZero - (currVal / maxVal) * (Math.Abs(rectRightAtFull) + Math.Abs(rectRightAtZero));
    }

    private void SetRectRight(RectTransform rect, float val, int maxVal, int rectRightAtZero, int rectRightAtFull) {
        rect.offsetMax = new Vector2(-CalculateRectRight(val, maxVal, rectRightAtZero, rectRightAtFull), rect.offsetMax.y);
    }

    private void UpdateBar(GameObject bar, float val) {
        SetRectRight(bar.GetComponent<RectTransform>(), val, statusMax, 155, -95);
    }
}
