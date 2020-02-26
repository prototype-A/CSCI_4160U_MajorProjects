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

    private void SaveGame(string savePath, GameSave gameSave) {
        // Write save as binary file
        BinaryFormatter binFormatter = new BinaryFormatter();
        FileStream saveFile = File.Create(savePath);
        binFormatter.Serialize(saveFile, gameSave);
        saveFile.Close();
    }

    private GameSave LoadSave(string savePath) {
        // Read game save from binary
        BinaryFormatter binFormatter = new BinaryFormatter();
        FileStream saveFile = File.Open(savePath, FileMode.Open);
        GameSave save = (GameSave)binFormatter.Deserialize(saveFile);
        saveFile.Close();

        return save;
    }

    public void SaveLoadGame(int saveNum) {
        // For each individual game save profile
        string savePath = Application.persistentDataPath + "Save" + saveNum + ".sav";
        if (saveloadMode == SaveState.Save) {
            Debug.Log("Saving game to \"" + savePath + "\"");

            // Create save
            GameObject player = transform.parent.Find("Player").gameObject;
            Vector3 currPlayerPos = player.transform.position;
            Vector3 playerPos = new Vector3(currPlayerPos.x, currPlayerPos.y, currPlayerPos.z);
            FloorGenerator floorGenerator = transform.parent.Find("Map").GetComponent<FloorGenerator>();
            GameSave gameSave = new GameSave(player.GetComponent<PlayerStats>().playerClass, playerPos, floorGenerator.GetTerrainMap());
            SaveGame(savePath, gameSave);
        } else if (saveloadMode == SaveState.Load) {
            if (File.Exists(savePath)) {
                // Save found for profile
                Debug.Log("Loading game from \"" + savePath + "\"");

                // Restore game save
                GameSave gameSave = LoadSave(savePath);
                Debug.Log("Save loaded");
            } else {
                // No save found
                Debug.Log("No save found in profile " + saveNum);
            }
        }
    }

    private void ResetSaveLoadPosition() {
        // Scroll save profiles back to top
        Vector3 pos = saves.transform.Find("Scroll Viewport").Find("Saves").localPosition;
        float x = pos.x;
        float z = pos.z;
        saves.transform.Find("Scroll Viewport").Find("Saves").localPosition = new Vector3(x, -120.0f, z);
    }

    private void PopulateSaves() {
        // Show saved class and save date if save data exists for profile
        Transform saveProfiles = saves.transform.Find("Scroll Viewport").Find("Saves");
        int numProfiles = saveProfiles.childCount;
        for (int save = 1; save < numProfiles; save++) {
            string savePath = Application.persistentDataPath + "Save" + save + ".sav";
            // Check if "Save#.sav" exists for profile #
            if (File.Exists(savePath)) {
                GameSave gameSave = LoadSave(savePath);
                saveProfiles.Find("Save" + save).Find("NoSave").gameObject.SetActive(false);
                saveProfiles.Find("Save" + save).Find("SaveData").Find("SavedClass").gameObject.GetComponent<TextMeshProUGUI>().text = gameSave.playerClass.className;
                saveProfiles.Find("Save" + save).Find("SaveData").Find("DateSaved").gameObject.GetComponent<TextMeshProUGUI>().text = gameSave.saveDate;
                saveProfiles.Find("Save" + save).Find("SaveData").gameObject.SetActive(true);
            } else {
                saveProfiles.Find("Save" + save).Find("SaveData").gameObject.SetActive(false);
                saveProfiles.Find("Save" + save).Find("NoSave").gameObject.SetActive(true);
            }
        }
    }

    public void SaveGame() {
        // Populate save profiles
        PopulateSaves();

        // Show save menu
        saveloadMode = SaveState.Save;
        HideMenu();
        ResetSaveLoadPosition();
        saves.transform.Find("Label").gameObject.GetComponent<TextMeshProUGUI>().text = "Save Game";
        saves.SetActive(true);
    }

    public void LoadGame() {
        // Populate save profiles
        PopulateSaves();

        // Show load menu
        saveloadMode = SaveState.Load;
        HideMenu();
        ResetSaveLoadPosition();
        saves.transform.Find("Label").gameObject.GetComponent<TextMeshProUGUI>().text = "Load Game";
        saves.SetActive(true);
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
        // Changes the confirmation question text and shows the confirmation window
        confirmation.transform.Find("WarningLabel").gameObject.GetComponent<TextMeshProUGUI>().text = titleText;
        HideMenu();
        confirmation.SetActive(true);
    }

    public void SetConfirmationBodyText(string bodyText) {
        // Changes the body text of the confirmation window
        confirmation.transform.Find("DescriptionLabel").gameObject.GetComponent<TextMeshProUGUI>().text = bodyText;
    }

    public void SetConfirmationYesButtonText(string confirmButtonText) {
        // Changes the text of the confirm button
        confirmation.transform.Find("ConfirmButton").Find("Text (TMP)").gameObject.GetComponent<TextMeshProUGUI>().text = confirmButtonText;
    }

    public void MainMenu() {
        // Intention to return to main menu after confirming
        this.confirmAction = Confirmation.MainMenu;
    }

    public void Quit() {
        // Intention to quit game after confirming
        this.confirmAction = Confirmation.Quit;
    }

    public void Confirm() {
        // Confirm button clicked
        if (this.confirmAction == Confirmation.MainMenu) {
            // Return to main menu
            LoadMainMenu();
        } else if (this.confirmAction == Confirmation.Quit) {
            // Quit game
            QuitGame();
        }
    }
}
