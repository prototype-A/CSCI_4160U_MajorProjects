using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuButtonController : MonoBehaviour {

    private bool inMenu = false;
    private bool inMenuScreen = false;
    private GameObject menu;
    private GameObject inventory;
    private GameObject characterCustomize;
    private GameObject saves;

    private enum Confirmation { MainMenu, Quit }
    private Confirmation confirmAction;
    private GameObject confirmation;

    public TextAsset classJson;
    private GameObject classSelect;
    private Classes charClasses;
    private int classIndex;

    private enum SaveState { Save, Load }
    private SaveState saveloadMode;


    void Start() {
        if (SceneManager.GetActiveScene().name == "MainMenu") {
            // At main menu
            menu = transform.Find("MainMenu").gameObject;
            saves = transform.Find("SavegameMenu").gameObject;
            classSelect = transform.Find("ClassSelect").gameObject;
        } else if (SceneManager.GetActiveScene().name == "PlayGame") {
            // Playing game
            menu = transform.Find("Menu").gameObject;
            inventory = transform.Find("Inventory").gameObject;
            characterCustomize = transform.Find("CharacterCustomize").gameObject;
            saves = transform.Find("SavegameMenu").gameObject;
            confirmation = transform.Find("ConfirmationWindow").gameObject;
        }
    }


    // Class selection functions
    private void SetClassName() {
        classSelect.transform.Find("Selection").Find("ClassText").gameObject.GetComponent<TextMeshProUGUI>().text = charClasses.classes[classIndex].className;
    }

    public void PrevClass() {
        classIndex--;
        if (classIndex < 0) {
            classIndex = charClasses.classes.Length - 1;
        }
        SetClassName();
    }

    public void NextClass() {
        classIndex = (classIndex + 1) % charClasses.classes.Length;
        SetClassName();
    }

    public void ClassSelect() {
        // Load classes
        charClasses = JsonUtility.FromJson<Classes>(classJson.text);
        classIndex = 0;

        SetClassName();
        HideMenu();
        classSelect.SetActive(true);
    }


    // Main menu functions
    public void PlayGame() {
        string chosenClass = classSelect.transform.Find("Selection").Find("ClassText").gameObject.GetComponent<TextMeshProUGUI>().text;
        Debug.Log("Starting game as a " + chosenClass);
        SceneManager.LoadScene("PlayGame");
    }


    // In-game menu functions
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
        string savePath = Application.persistentDataPath + "Save" + saveNum + ".sav";
        if (saveloadMode == SaveState.Save) {
            Debug.Log("Saving game to \"" + savePath + "\"");

            BinaryFormatter binFormatter = new BinaryFormatter();
            FileStream saveFile = File.Create(savePath);
            binFormatter.Serialize(saveFile, "");

            saveFile.Close();
        } else if (saveloadMode == SaveState.Load) {
            Debug.Log("Saving game to \"" + savePath + "\"");

            BinaryFormatter binFormatter = new BinaryFormatter();
            FileStream saveFile = File.Open(savePath, FileMode.Open);
            binFormatter.Deserialize(saveFile);

            saveFile.Close();
        }
    }

    private void ResetSaveLoadPosition() {
        // Scroll back to top
        Vector3 pos = saves.transform.Find("Scroll Viewport").Find("Saves").localPosition;
        float x = pos.x;
        float z = pos.z;
        saves.transform.Find("Scroll Viewport").Find("Saves").localPosition = new Vector3(x, -190.0f, z);
    }

    public void SaveGame() {
        // Show save menu
        saveloadMode = SaveState.Save;
        HideMenu();
        ResetSaveLoadPosition();
        saves.transform.Find("Label").gameObject.GetComponent<TextMeshProUGUI>().text = "Save Game";
        saves.SetActive(true);

        // Populate saves
    }

    public void LoadGame() {
        // Show load menu
        saveloadMode = SaveState.Load;
        HideMenu();
        ResetSaveLoadPosition();
        saves.transform.Find("Label").gameObject.GetComponent<TextMeshProUGUI>().text = "Load Game";
        saves.SetActive(true);

        // Populate saves

    }

    public void LoadMainMenu() {
        Debug.Log("Loading main menu");
        //SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame() {
        Debug.Log("Quitting game");
    }


    // Confirmation window functions
    public void ShowConfirmation(string titleText) {
        confirmation.transform.Find("WarningLabel").gameObject.GetComponent<TextMeshProUGUI>().text = titleText;
        HideMenu();
        confirmation.SetActive(true);
    }

    public void SetConfirmationBodyText(string bodyText) {
        confirmation.transform.Find("DescriptionLabel").gameObject.GetComponent<TextMeshProUGUI>().text = bodyText;
    }

    public void SetConfirmationYesButtonText(string confirmButtonText) {
        confirmation.transform.Find("ConfirmButton").Find("Text (TMP)").gameObject.GetComponent<TextMeshProUGUI>().text = confirmButtonText;
    }

    public void MainMenu() {
        this.confirmAction = Confirmation.MainMenu;
    }

    public void Quit() {
        this.confirmAction = Confirmation.Quit;
    }

    public void Confirm() {
        if (this.confirmAction == Confirmation.MainMenu) {
            // Return to main menu
            LoadMainMenu();
        } else if (this.confirmAction == Confirmation.Quit) {
            // Quit game
            QuitGame();
        }
    }
}
