using TMPro;
﻿using UnityEngine;

public class GunCustomization : MonoBehaviour {

    private Gun gun;
    [SerializeField] private TextMeshProUGUI nameText = null;
    [SerializeField] private Transform muzzleSlot = null;
    [SerializeField] private Transform magazineSlot = null;
    [SerializeField] private Transform underbarrelSlot = null;
    [SerializeField] private Transform scopeSlot = null;
    [SerializeField] private Transform attachments = null;
    public GameObject attachmentPrefab;

    private void SetGunName(string name) {
        this.nameText.text = name;
    }

    public void CustomizeGun(Gun gun) {
        // Shows gun name
        this.gun = gun;
        SetGunName(gun.itemInfo.name);

        // Show gun attachments
        if (gun.attachments != null && gun.attachments.Length > 0) {
            foreach (InventoryItem attachment in gun.attachments) {
                GameObject gunAttachment = Instantiate(attachmentPrefab);
                switch (attachment.item.itemInfo.itemType) {
                    case Types.ItemType.Muzzle:
                        gunAttachment.transform.position = muzzleSlot.transform.position;
                        break;
                    case Types.ItemType.Scope:
                        gunAttachment.transform.position = scopeSlot.position;
                        break;
                    case Types.ItemType.Magazine:
                        gunAttachment.transform.position = magazineSlot.position;
                        break;
                    case Types.ItemType.Underbarrel:
                        gunAttachment.transform.position = underbarrelSlot.position;
                        break;
                }
            }
        }
    }

    public void ClearGunCustomization() {
        // Clears gun customization panel of the current gun
        SetGunName("Gun Name");
        foreach (Transform attachment in attachments) {
            Destroy(attachment);
        }
    }
}
