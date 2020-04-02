using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FPSCharacterController : MonoBehaviour {

    // Camera movement
    public Transform cameraT;
    public Vector3 defaultCameraPos = new Vector3(0.0f, 0.7f, -0.25f);
    public float mouseSensitivity = 100.0f;
    private float hLook = 0.0f;
    private float vLook = 0.0f;
    private float hRecoil = 0.0f;
    private float vRecoil = 0.0f;
    private float recoilReturnSpeed = 1.0f;
    private float fireV = 0.0f;

    // Movement
    private CharacterController charController;
    public float walkSpeed = 4.5f;
    public float runSpeed = 8.0f;

    // GUI
    public Menu gui;
    public GameObject healthBar;
    public GameObject hungerBar;
    public GameObject thirstBar;

    // Status
    public float health = 100.0f;
    public float hunger = 100.0f;
    public float thirst = 100.0f;
    private int statusMax = 100;

    // Equipped guns
    public Gun[] guns;
    public int equippedGun = 0;

    void Start() {
        // Lock and hide cursor at center of screen
        Cursor.lockState = CursorLockMode.Locked;

        // Get Character Controller component
        this.charController = GetComponent<CharacterController>();

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
        if (vRecoil > 0 && transform.localRotation.x < fireV && !Input.GetButton("Fire")) {
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

        // Change weapons
        if (Input.GetButtonDown("Main Weapon") && guns[0] != null) {
            guns[0].ShowModel(true);
            if (guns[1] != null) {
                guns[1].ShowModel(false);
            }
            equippedGun = 0;
        } else if (Input.GetButtonDown("Second Weapon") && guns[1] != null) {
            guns[1].ShowModel(true);
            if (guns[0] != null) {
                guns[0].ShowModel(false);
            }
            equippedGun = 1;
        }
    }

    void FixedUpdate() {
        // Drain hunger

        if (hunger > 0) {
            Famish(Time.deltaTime * 0.03f);
        } else {
            // Take damage if famished
            TakeDamage(0.05f);
        }
        // Drain thirst
        if (thirst > 0) {
            Dehydrate(Time.deltaTime * 0.01f);
        } else {
            // Take damage if dehydrated
            TakeDamage(0.15f);
        }
        UpdateBars();
    }

    public void UpdateBars() {
        UpdateBar(healthBar, health);
        UpdateBar(hungerBar, hunger);
        UpdateBar(thirstBar, thirst);
    }

    public void TakeDamage(float damage) {
        health -= damage;
        if (health <= 0) {
            // Player died
            health = 0;
        }
    }

    public void Famish(float damage) {
        hunger -= damage;
        if (hunger <= 0) {
            // Player famished
            hunger = 0;
        }
    }

    public void Dehydrate(float damage) {
        thirst -= damage;
        if (thirst <= 0) {
            // Player dehydrated
            thirst = 0;
        }
    }

    public void Heal(int health, int hunger, int thirst) {
        this.health += health;
        if (this.health > 100.0f) {
            this.health = 100.0f;
        }
        this.hunger += hunger;
        if (this.hunger > 100.0f) {
            this.hunger = 100.0f;
        }
        this.thirst += thirst;
        if (this.thirst > 100.0f) {
            this.thirst = 100.0f;
        }
    }

    public void EquipGun(Gun gun, int slotNum) {
        guns[slotNum] = gun;
        gun.gameObject.SetActive(true);
        gui.gunGui.Show(true);
    }

    public void UnequipGun(Gun gun, int slotNum) {
        gun.ads = false;
        guns[slotNum] = null;
        gun.gameObject.SetActive(false);
        gui.gunGui.Show(false);
    }

    public void AddRecoil(float hRecoil, float vRecoil, float recoilReturnSpeed) {
        // Add visual recoil to player camera
        this.recoilReturnSpeed = recoilReturnSpeed;
        this.hRecoil = hRecoil;
        this.vRecoil += vRecoil;
    }

    public Transform GetCameraTransform() {
        return transform.Find("Camera");
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
