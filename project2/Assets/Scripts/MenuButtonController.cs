using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtonController : MonoBehaviour {

    public GameObject mainMenuScreen;
    public GameObject characterCustomizationScreen;
    public GameObject loadSaveScreen;
    public GameObject currentScreen;

    public TextMeshProUGUI playerNameInput;

    public TextMeshProUGUI characterCustomizationErrorText;
    public float errorDuration = 3.0f;
    public float errorFadeStep = 0.1f;
    private float errorShowTime;
    private Color errorColor;


    void Start() {
        currentScreen = mainMenuScreen;
        errorColor = characterCustomizationErrorText.color;
    }

    // Update character name
    public void UpdatePlayerName(string name) {

    }

    // Show main menu
    public void BackToMainMenu() {
        currentScreen.SetActive(false);
        currentScreen = mainMenuScreen;
        currentScreen.SetActive(true);
    }

    // Show character customization screen
    public void ShowCharacterCustomization() {
        currentScreen.SetActive(false);
        currentScreen = characterCustomizationScreen;
        currentScreen.SetActive(true);
    }

    // Show save loading screen
    public void ShowLoadMenu() {
        currentScreen.SetActive(false);
        currentScreen = loadSaveScreen;
        currentScreen.SetActive(true);
    }

    // Start a new game
    public void StartGame() {
        // Check if empty character name
        if (playerNameInput.text.Length == 1) {
            // Show error
            characterCustomizationErrorText.color = new Color(errorColor.r, errorColor.g, errorColor.b, 1.0f);
            errorShowTime = Time.time;
            StartCoroutine(FadeError());
        } else {
            // Start game
            PlayerPrefs.SetString("PlayerName", playerNameInput.text);
            SceneManager.LoadScene("Game");
        }
    }

    // Load saved game
    public void LoadGame() {

    }

    // Quit the game
    public void QuitGame() {
        Application.Quit(0);
    }


    private IEnumerator FadeError() {
        // Wait until error has been shown for sufficient amount of time
        while (Time.time - errorShowTime < errorDuration) {
            yield return new WaitForSeconds(1.0f);
        }

        // Fade error message
        while (characterCustomizationErrorText.color.a > 0.0f) {
            yield return new WaitForSeconds(errorFadeStep / 30);
            characterCustomizationErrorText.color = new Color(errorColor.r, errorColor.g, errorColor.b, characterCustomizationErrorText.color.a - errorFadeStep);
        }
    }
}
