using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtonController : MonoBehaviour {

    private bool inMenu = false;
    private bool inMenuScreen = false;
    private GameObject menu;
    private GameObject inventory;
    private GameObject characterCustomize;
    private GameObject saves;
    private GameObject quitConfirmation;

    private enum SaveState { Save, Load }
    private SaveState saveloadMode;

    void Start() {
        if (SceneManager.GetActiveScene().name != "MainMenu") {
            // Playing game
            menu = transform.Find("Menu").gameObject;
            inventory = transform.Find("Inventory").gameObject;
            characterCustomize = transform.Find("CharacterCustomize").gameObject;
            saves = transform.Find("SavegameMenu").gameObject;
            quitConfirmation = transform.Find("QuitConfirmation").gameObject;
        }
    }

    public void PlayGame() {
        Debug.Log("Playing Game");
    }

    public bool IsInMenu() {
        return this.inMenu;
    }

    public bool IsInMenuScreen() {
        return this.inMenuScreen;
    }

    public void ShowMenu() {
        menu.SetActive(true);
        inMenu = true;
        inMenuScreen = true;
    }

    private void HideMenu() {
        menu.SetActive(false);
        inMenuScreen = false;
    }

    public void CloseMenu() {
        HideMenu();
        inMenu = false;
        inMenuScreen = false;
    }

    public void BackToMenu(GameObject buttonClicked) {
        buttonClicked.transform.parent.gameObject.SetActive(false);
        ShowMenu();
    }

    public void OpenInventory() {
        HideMenu();
        inventory.SetActive(true);
    }

    public void OpenCharacterCustomize() {
        HideMenu();
        characterCustomize.SetActive(true);
    }

    public void SaveLoadGame(int saveNum) {
        // For each individual game save profile
        if (saveloadMode == SaveState.Save) {
            Debug.Log("Saving game");
        } else if (saveloadMode == SaveState.Load) {
            Debug.Log("Loading Saved Game");
        }
    }

    private void ResetSaveLoadPosition() {
        // Scroll back to top
        Vector3 pos = saves.transform.Find("Panel").Find("Saves").localPosition;
        float x = pos.x;
        float z = pos.z;
        saves.transform.Find("Panel").Find("Saves").localPosition = new Vector3(x, -210.0f, z);
    }

    public void SaveGame() {
        Debug.Log("Saving game");
        saveloadMode = SaveState.Save;
        HideMenu();
        ResetSaveLoadPosition();
        saves.SetActive(true);
    }

    public void LoadGame() {
        Debug.Log("Loading Saved Game");
        saveloadMode = SaveState.Load;
        HideMenu();
        ResetSaveLoadPosition();
        saves.SetActive(true);
    }

    public void ShowQuitConfirmation() {
        HideMenu();
        quitConfirmation.SetActive(true);
    }

    public void Quit() {
        Debug.Log("Quitting game");
    }
}
