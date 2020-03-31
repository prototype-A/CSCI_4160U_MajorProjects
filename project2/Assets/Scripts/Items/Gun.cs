using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : Item {

    // Base gun properties
    public Vector3 instantiatePos;
    public int baseDamage;
    public int damageModifier = 0;
    public float range;
    [SerializeField] protected GameSystem.FireMode[] fireModes = { GameSystem.FireMode.Safety, GameSystem.FireMode.Single };
    [SerializeField] protected GameSystem.FireMode fireMode = GameSystem.FireMode.Safety;
    public InventoryItem[] attachments { get; }
    protected Animator gunAnimator;
    public GameObject casingPrefab;
    public Transform casingEjectionPos;
    public GameObject[] bulletHoles;

    // Recoil
    [SerializeField] protected float minRecoilX = 0.0f;
    [SerializeField] protected float maxRecoilX = 0.0f;
    [SerializeField] protected float minRecoilY = 0.0f;
    [SerializeField] protected float maxRecoilY = 0.0f;
    [SerializeField] protected float recoilReturnSpeed = 1.0f;

    // Aim-down Sights
    public bool ads = false;
    private Transform cameraT;
    protected Transform adsPos;

    protected Menu gui;

    void Start() {
        // Get gun animator
        gunAnimator = GetComponent<Animator>();

        // Get camera transform for aim-down sights
        this.cameraT = GetPlayerController().GetCameraTransform();

        // Get player gui
        gui = transform.parent.Find("GUI").GetComponent<Menu>();
    }

    protected void Update() {
        // Aim-down sights
        if (Input.GetButtonDown("Aim-Down Sight")) {
            if (!ads) {
                ads = true;
                gui.ToggleCrosshair(false);
                // Get sight position for camera
                adsPos = transform.Find("Scope/AdsScopePos");
                if (adsPos == null) {
                    adsPos = transform.Find("AdsSightPos");
                }
            } else {
                ads = false;
                cameraT.localPosition = GetPlayerController().defaultCameraPos;
                gui.ToggleCrosshair(true);
            }
        }
        if (ads) {
            // Get ain-down sight position
            cameraT.position = adsPos.position;
            cameraT.rotation = adsPos.rotation;
        }

        // Reload
        if (Input.GetButtonDown("Reload")) {
            Reload();
        }

        // Change firing mode
        if (Input.GetButtonDown("Change Firing Mode")) {
            ChangeFireMode();
        }
    }

    // Gun methods
    protected void Fire() {
        // Play animation
        gunAnimator.SetBool("TriggerPulled", true);
    }

    private void FireBullet() {
        // Only hit the ground, buildings, or enemies
        LayerMask enemyMask = LayerMask.GetMask("Enemies");
        LayerMask destructablesMask =  LayerMask.GetMask("Destructable");
        LayerMask buildingMask = LayerMask.GetMask("Buildings");

        // Detect collision
        RaycastHit hit;
        if (Physics.Raycast(cameraT.position, cameraT.forward, out hit, range, enemyMask)) {
            // Hit an enemy

        } else if (Physics.Raycast(cameraT.position, cameraT.forward, out hit, range, buildingMask)) {
            // Make a bullet hole if hit a building
            GameObject bulletHole = Instantiate(bulletHoles[Random.Range(0, bulletHoles.Length)],
                                    hit.point + (0.01f * hit.normal),
                                    Quaternion.LookRotation(-1 * hit.normal, hit.transform.up));
        } else if (Physics.Raycast(cameraT.position, cameraT.forward, out hit, range, destructablesMask)) {
            // Make destructable object take damage
            hit.collider.GetComponent<DestructableObject>().TakeDamage(GetDamage());
        }

        // Recoil
        float recoilX = Random.Range(minRecoilX, maxRecoilX + 0.1f);
        float recoilY = Random.Range(minRecoilY, maxRecoilY + 0.1f);
        GetPlayerController().AddRecoil(recoilX, recoilY, recoilReturnSpeed);
    }

    protected void StopFiring() {
        gunAnimator.SetBool("TriggerPulled", false);
    }

    protected void Reload() {
        gunAnimator.SetTrigger("Reload");
        gunAnimator.SetBool("Reloading", true);
    }

    protected void ReloadFinished() {
        gunAnimator.SetBool("Reloading", false);
    }

    protected void ChangeFireMode() {

    }

    protected void EjectCasing() {
        if (casingPrefab != null) {
            GameObject bulletCasing = Instantiate(casingPrefab,
                                                casingEjectionPos.position,
                                                Quaternion.Euler(casingEjectionPos.eulerAngles.x, casingEjectionPos.eulerAngles.y, casingEjectionPos.eulerAngles.z));

            // Give casing spin and force
            bulletCasing.GetComponent<Rigidbody>().AddRelativeForce(100.0f, 100.0f, 0.0f);
            bulletCasing.GetComponent<Rigidbody>().AddTorque(transform.up * Random.Range(0.1f, 1.1f));
        }
    }

    protected void DischargeGun() {
        gunAnimator.SetBool("Charged", false);
    }

    protected void ChargeGun() {
        gunAnimator.SetBool("Charged", true);
    }

    protected int GetDamage() {
        return baseDamage + damageModifier;
    }


    // Public methods
    public void ShowModel(bool show) {
        gameObject.transform.Find("Model").gameObject.SetActive(show);
    }
}
