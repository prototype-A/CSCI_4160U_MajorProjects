using System;
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
    protected Animator gunAnimator;
    public GameObject[] bulletHoles;

    // Casing ejection
    public GameObject casingPrefab;
    public Transform casingEjectionPos;

    // Attachments
    public InventoryItem[] attachments;
    public GameSystem.ItemType[] attachmentTypes;

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
        gui = GetPlayerController().gui;
    }

    protected void Update() {
        // Aim-down sights
        if (Input.GetButtonDown("Aim-Down Sight")) {
            if (!ads) {
                ads = true;
                gui.gunGui.ToggleCrosshair(false);
                // Get sight position for camera
                adsPos = transform.Find("Model/Scope/AdsScopePos");
                if (adsPos == null) {
                    adsPos = transform.Find("Model/AdsSightPos");
                }
            } else {
                ads = false;
                cameraT.localPosition = GetPlayerController().defaultCameraPos;
                gui.gunGui.ToggleCrosshair(true);
            }
        }
        if (ads) {
            // Get ain-down sight position
            cameraT.position = adsPos.position;
            cameraT.rotation = adsPos.rotation;
        }

        // Change firing mode
        if (Input.GetButtonDown("Change Firing Mode")) {
            ChangeFireMode();
        }
    }

    // Gun methods
    protected void Fire() {
        // Magazine is in and has ammo
        Magazine mag = GetMagazine();
        if (mag != null && mag.ammoCount > 0) {
            gunAnimator.SetBool("TriggerPulled", true);
        }
    }

    private void FireBullet() {

        gunAnimator.SetInteger("BulletCount", --GetMagazine().ammoCount);
        gui.gunGui.SetAmmo(gunAnimator.GetInteger("BulletCount"));

        // Only hit the ground, buildings, or enemies
        LayerMask enemyMask = LayerMask.GetMask("Enemies");
        LayerMask destructablesMask = LayerMask.GetMask("Destructable");
        LayerMask buildingMask = LayerMask.GetMask("Buildings");

        // Detect collision
        RaycastHit hit;
        if (Physics.Raycast(cameraT.position, cameraT.forward, out hit, range, enemyMask)) {
            // Hit an enemy

        } else if (Physics.Raycast(cameraT.position, cameraT.forward, out hit, range, buildingMask)) {
            // Make a bullet hole if hit a building
            GameObject bulletHole = Instantiate(bulletHoles[UnityEngine.Random.Range(0, bulletHoles.Length)],
                                    hit.point + (0.01f * hit.normal),
                                    Quaternion.LookRotation(-1 * hit.normal, hit.transform.up));
        } else if (Physics.Raycast(cameraT.position, cameraT.forward, out hit, range, destructablesMask)) {
            // Make destructable object take damage
            hit.collider.GetComponent<DestructableObject>().TakeDamage(GetDamage());
        }

        // Recoil
        float recoilX = UnityEngine.Random.Range(minRecoilX, maxRecoilX + 0.1f);
        float recoilY = UnityEngine.Random.Range(minRecoilY, maxRecoilY + 0.1f);
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
        fireMode = fireModes[(Array.IndexOf(fireModes, fireMode) + 1) % fireModes.Length];
    }

    protected void EjectCasing() {
        if (casingPrefab != null) {
            GameObject bulletCasing = Instantiate(casingPrefab,
                                                casingEjectionPos.position,
                                                Quaternion.Euler(casingEjectionPos.eulerAngles.x, casingEjectionPos.eulerAngles.y, casingEjectionPos.eulerAngles.z));

            // Give casing a spin and force
            bulletCasing.GetComponent<Rigidbody>().AddRelativeForce(100.0f, 100.0f, 0.0f);
            bulletCasing.GetComponent<Rigidbody>().AddTorque(transform.up * UnityEngine.Random.Range(0.1f, 1.1f));
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

    protected Magazine GetMagazine() {
        return (Magazine)attachments[Array.IndexOf(attachmentTypes, GameSystem.ItemType.Magazine)].item;
    }


    // Public methods
    public bool AddAttachment(InventoryItem attach) {
        GameSystem.ItemType attachmentType = attach.item.itemInfo.itemType;
        if (Array.Exists(attachmentTypes, a => a == attachmentType)) {
            attachments[Array.IndexOf(attachmentTypes, attachmentType)] = attach;
            switch (attachmentType) {
                case GameSystem.ItemType.Magazine:
                    gui.gunGui.SetAmmo(((Magazine)attach.item).ammoCount);
                    break;
            }
            return true;
        }

        return false;
    }

    public void ShowModel(bool show) {
        gameObject.transform.Find("Model").gameObject.SetActive(show);
    }
}
