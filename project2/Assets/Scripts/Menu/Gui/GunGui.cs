using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GunGui : MonoBehaviour {

    // Crosshair
    [SerializeField] private GameObject crosshair = null;
    [SerializeField] private GameObject killConfirm = null;

    // Ammo
    [SerializeField] private GameObject ammoCount = null;
    [SerializeField] private TextMeshProUGUI ammoText = null;


    public void Show(bool show) {
        crosshair.SetActive(show);
        ammoCount.SetActive(show);
    }

    public void ToggleCrosshair(bool show) {
        crosshair.SetActive(show);
    }

    public void KillConfirm() {
        StartCoroutine(ShowKillConfirm());
    }

    private IEnumerator ShowKillConfirm() {
        killConfirm.SetActive(true);

        yield return new WaitForSeconds(1.0f);

        killConfirm.SetActive(false);
    }

    public void ChangeCrosshair(Sprite sprite) {
        crosshair.GetComponent<Image>().sprite = sprite;
    }

    public void SetAmmo(int ammo) {
        ammoText.text = "" + ammo;
    }
}
