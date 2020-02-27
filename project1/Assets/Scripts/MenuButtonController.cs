using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuButtonController : MonoBehaviour {

    private bool inMenu = false;
    private bool inMenuScreen = false;
    private GameObject menu;
    private GameObject prevMenu;
    private GameObject inventory;
    private GameObject characterCustomize;
    private GameObject saves;

    private enum Confirmation { MainMenu, SaveLoad, Quit }
    private int saveNum;
    private Confirmation confirmAction;
    private GameObject confirmation;

    private GameObject classSelect;
    private Class[] charClasses;
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
    public void ShowClassSelect() {
        // Load classes
        charClasses = GameData.playerClasses;
        classIndex = 0;

        SetClassName();
        HideMenu();
        classSelect.SetActive(true);
    }

    private void SetClassName() {
        classSelect.transform.Find("Selection").Find("ClassText").gameObject.GetComponent<TextMeshProUGUI>().text = charClasses[classIndex].className;
    }

    public void PrevClass() {
        classIndex--;
        if (classIndex < 0) {
            classIndex = charClasses.Length - 1;
        }
        SetClassName();
    }

    public void NextClass() {
        classIndex = (classIndex + 1) % charClasses.Length;
        SetClassName();
    }

    public void PlayGame() {
        // Start new game
        string chosenClass = classSelect.transform.Find("Selection").Find("ClassText").gameObject.GetComponent<TextMeshProUGUI>().text;
        GameData.playerStats = new PlayerStats(chosenClass);
        GameData.playerInventory = new Inventory();
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
        this.menu.SetActive(true);
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

    public void SetPreviousMenu(GameObject prevMenu) {
        this.prevMenu = prevMenu;
    }

    public void BackToMenu(GameObject menuToHide) {
        if (menuToHide != null) {
            menuToHide.SetActive(false);
        }

        if (this.prevMenu != null) {
            // Show previous menu
            this.prevMenu.SetActive(true);
            this.prevMenu = null;
        } else {
            // Show game menu
            ShowMenu();
        }
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
        SaveLoadGame(saveNum, false);
    }

    private void SaveLoadGame(int saveNum, bool confirmed) {
        // For each individual game save profile
        string savePath = Application.persistentDataPath + "/Save" + saveNum + ".sav";

        // Saving
        if (saveloadMode == SaveState.Save) {
            if (File.Exists(savePath) && !confirmed) {
                // Show confirmation to override previous save in profle if it exists
                this.saveNum = saveNum;
                SetPreviousMenu(saves);
                saves.SetActive(false);
                SetConfirmOverwriteOrLoadSave();
                SetConfirmationYesButtonText("Save");
                SetConfirmationBodyText("You are about to overwrite a saved game. By overwriting it, you will lose all data in that save. Are you sure you want to continue saving?");
                ShowConfirmation("Overwrite save?");
            } else if (confirmed) {
                Debug.Log("Saving game to \"" + savePath + "\"");

                // Create save
                GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
                GameObject player = null;
                FloorGenerator floorGenerator = null;
                for (int obj = 0; obj < rootObjects.Length; obj++) {
                    if (rootObjects[obj].name == "Player") {
                        player = rootObjects[obj].gameObject;
                    } else if (rootObjects[obj].name == "Map") {
                        floorGenerator = rootObjects[obj].gameObject.GetComponent<FloorGenerator>();
                    }
                }
                Vector3 currPlayerPos = player.transform.position;
                float[] playerPos = {currPlayerPos.x, currPlayerPos.y, currPlayerPos.z};
                GameSave gameSave = new GameSave(playerPos, floorGenerator.terrainMap);
                SaveGame(savePath, gameSave);

                if (prevMenu == saves) {
                    // Return to save menu after confirming to overwrite save
                    BackToMenu(confirmation);
                }

                // Reload saves to show new save
                PopulateSaves();
            }
        } else if (saveloadMode == SaveState.Load) {
            if (File.Exists(savePath)) {
                // Save found for profile
                if (SceneManager.GetActiveScene().name == "MainMenu" || confirmed) {
                    // No need to show load confirmation in main menu
                    // or
                    // Player confirmed to load
                    Debug.Log("Loading game from \"" + savePath + "\"");

                    // Load and restore save data
                    GameSave gameSave = LoadSave(savePath);
                    GameData.playerStats = gameSave.playerStats;


                    Debug.Log("Save loaded");
                } else if (!confirmed) {
                    // Show confirmation to load save if in game
                    this.saveNum = saveNum;
                    SetPreviousMenu(saves);
                    saves.SetActive(false);
                    SetConfirmOverwriteOrLoadSave();
                    SetConfirmationYesButtonText("Load");
                    SetConfirmationBodyText("You are about to load a saved game. Make sure you have saved your game, or you will lose all progress made.");
                    ShowConfirmation("Load game?");
                }
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
        saves.transform.Find("Scroll Viewport").Find("Saves").localPosition = new Vector3(x, -110.0f, z);
    }

    private void PopulateSaves() {
        // Show saved class and save date if save data exists for profile
        Transform saveProfiles = saves.transform.Find("Scroll Viewport").Find("Saves");
        int numProfiles = saveProfiles.childCount;
        for (int save = 1; save < numProfiles + 1; save++) {
            string savePath = Application.persistentDataPath + "/Save" + (save - 1) + ".sav";
            // Check if "Save#.sav" exists for profile number #
            if (File.Exists(savePath)) {
                GameSave gameSave = LoadSave(savePath);
                saveProfiles.Find("Save" + save).Find("NoSave").gameObject.SetActive(false);
                saveProfiles.Find("Save" + save).Find("SaveData").Find("SavedClass").gameObject.GetComponent<TextMeshProUGUI>().text = gameSave.playerStats.playerClass.className;
                saveProfiles.Find("Save" + save).Find("SaveData").Find("DateSaved").gameObject.GetComponent<TextMeshProUGUI>().text = gameSave.saveDate;
                saveProfiles.Find("Save" + save).Find("SaveData").gameObject.SetActive(true);
            } else {
                try {
                    // TODO: Why does this fail only on the last profile?
                    saveProfiles.Find("Save" + save).Find("SaveData").gameObject.SetActive(false);
                } catch {}
                saveProfiles.Find("Save" + save).Find("NoSave").gameObject.SetActive(true);
            }
        }
    }

    private void SaveGame(string savePath, GameSave gameSave) {
        // Write save to binary file
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

    public void ShowSaveMenu() {
        // Populate save profiles
        PopulateSaves();

        // Show save menu
        saveloadMode = SaveState.Save;
        HideMenu();
        ResetSaveLoadPosition();
        saves.transform.Find("Label").gameObject.GetComponent<TextMeshProUGUI>().text = "Save Game";
        saves.SetActive(true);
    }

    public void ShowLoadMenu() {
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

    public void SetConfirmMainMenu() {
        // Intention to return to main menu after confirming
        this.confirmAction = Confirmation.MainMenu;
    }

    private void SetConfirmOverwriteOrLoadSave() {
        // Intention to overwrite a saved game after confirming
        // or
        // Intention to load saved game after confirming
        this.confirmAction = Confirmation.SaveLoad;
    }

    public void SetConfirmQuit() {
        // Intention to quit game after confirming
        this.confirmAction = Confirmation.Quit;
    }

    public void Confirm() {
        // Confirm button clicked
        if (this.confirmAction == Confirmation.MainMenu) {
            // Return to main menu
            LoadMainMenu();
        } else if (this.confirmAction == Confirmation.SaveLoad) {
            // Overwrite a saved game with a new save
            // or
            // Load another saved game while in game
            SaveLoadGame(this.saveNum, true);
        } else if (this.confirmAction == Confirmation.Quit) {
            // Quit game
            QuitGame();
        }
    }
}
