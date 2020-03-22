using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : Item {

    [SerializeField] protected Types.FireMode[] fireModes;
    [SerializeField] protected Types.FireMode fireMode;
    [SerializeField] private float minRecoilX;
    [SerializeField] private float maxRecoilX;
    [SerializeField] private float minRecoilY;
    [SerializeField] private float maxRecoilY;
    [SerializeField] protected float recoilReturnSpeed;
    [SerializeField] protected Item[] attachments;
    protected Magazine mag;
    protected Animator gunAnimator;

    public GameObject casingPrefab;
    public Transform casingEjectionPos;
    public GameObject[] bulletHoles;
    private Transform cameraT;
    public int baseDamage;
    public int damageModifier = 0;
    public float range;
    private bool ads = false;
    protected Transform adsPos;

    void Start() {
        gunAnimator = GetComponent<Animator>();

        // Get camera transform
        this.cameraT = GetPlayerController().GetCameraTransform();
    }

    protected void Update() {
        // Aim-down sights
        if (Input.GetButtonDown("Aim-Down Sight")) {
            if (!ads) {
                ads = true;
                GetPlayerController().ToggleCrosshair(false);
                // Get sight position for camera
                adsPos = transform.Find("Scope/ScopePos");
                if (adsPos == null) {
                    adsPos = transform.Find("SightPos");
                }
            } else {
                ads = false;
                cameraT.localPosition = new Vector3(0.0f, 0.7f, -0.25f);
                GetPlayerController().ToggleCrosshair(true);
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
        GameObject bulletCasing = Instantiate(casingPrefab,
                                            casingEjectionPos.position,
                                            Quaternion.Euler(casingEjectionPos.eulerAngles.x, casingEjectionPos.eulerAngles.y, casingEjectionPos.eulerAngles.z));

        bulletCasing.GetComponent<Rigidbody>().AddRelativeForce(100.0f, 100.0f, 0.0f);
        bulletCasing.GetComponent<Rigidbody>().AddTorque(transform.up * Random.Range(0.1f, 1.1f));
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


    private Transform GetPlayerTransform() {
        return transform.parent;
    }

    private FPSCharacterController GetPlayerController() {
        return GetPlayerTransform().gameObject.GetComponent<FPSCharacterController>();
    }
}
