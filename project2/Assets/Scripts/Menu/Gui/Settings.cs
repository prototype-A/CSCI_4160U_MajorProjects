using UnityEngine;﻿
using UnityEngine.UI;

public class Settings : MonoBehaviour {

    public Menu gui;

    // Crosshair settings
    [SerializeField] public Sprite[] crosshairs = { };
    [SerializeField] private Image crosshairPreview = null;
    private int currCrosshairNum = 0;

    // Change player crosshair to previous crosshair
    public void PrevCrosshair() {
        currCrosshairNum--;
        if (currCrosshairNum < 0) {
            currCrosshairNum = crosshairs.Length - 1;
        }
        ShowCrosshairPreview();
        ChangeCrosshair();
    }

    // Change player crosshair to next crosshair
    public void NextCrosshair() {
        currCrosshairNum = (currCrosshairNum + 1) % crosshairs.Length;
        ShowCrosshairPreview();
        ChangeCrosshair();
    }

    // Show new crosshair in preview
    private void ShowCrosshairPreview() {
        crosshairPreview.sprite = crosshairs[currCrosshairNum];
    }

    // Set crosshair sprite
    private void ChangeCrosshair() {
        gui.ChangeCrosshair(crosshairs[currCrosshairNum]);
        PlayerPrefs.SetInt(GameSystem.CROSSHAIR_SETTING_KEY, currCrosshairNum);
    }

    // Restore crosshair setting from saved game
    public void ChangeCrosshair(int num) {
        currCrosshairNum = num;
        gui.ChangeCrosshair(crosshairs[currCrosshairNum]);
    }
}
