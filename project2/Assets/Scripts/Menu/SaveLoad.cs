using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoad : MonoBehaviour {

    public enum Mode {
        SAVE,
        LOAD
    }

    public Mode mode;
    public Transform saves;
    public GameSystem game;


    // Individual save profile clicked
    public void SaveLoadGame(int profileNum) {
        if (mode == Mode.SAVE) {
            // Save game
            SaveGame(profileNum);
        } else {
            // Load game
            LoadSavedGame(profileNum);
        }
    }

    // Save game
    public void SaveGame(int saveNum) {
        string savePath = Application.persistentDataPath + "/Save" + saveNum + ".sav";

        // Build save data
        SaveData saveData = game.BuildSaveData();

        // Write save to binary file
        BinaryFormatter binFormatter = new BinaryFormatter();
        FileStream saveFile = File.Create(savePath);
        binFormatter.Serialize(saveFile, saveData);
        saveFile.Close();

        // Reload saves to show new save
        PopulateSaves();
    }

    // Load saved game
    public void LoadSavedGame(int saveNum) {
        string savePath = Application.persistentDataPath + "/Save" + saveNum + ".sav";

        if (File.Exists(savePath)) {
            // Save found for profile
            Debug.Log("Loading game from \"" + savePath + "\"");

            // Read game save from binary
            GameSystem.saveData = LoadSaveFromFile(savePath);

            // Load game scene
            SceneManager.LoadScene(GameSystem.GAME_SCENE_NAME);
        }
    }

    // Read game save from binary
    private SaveData LoadSaveFromFile(string savePath) {
        BinaryFormatter binFormatter = new BinaryFormatter();
        FileStream saveFile = File.Open(savePath, FileMode.Open);
        SaveData save = (SaveData)binFormatter.Deserialize(saveFile);
        saveFile.Close();

        return save;
    }

    // Show save info
    private void PopulateSaves() {
        for (int save = 1; save <= saves.childCount; save++) {
            string savePath = Application.persistentDataPath + "/Save" + (save - 1) + ".sav";
            SaveProfile saveProfile = saves.Find("Save " + save).gameObject.GetComponent<SaveProfile>();

            if (File.Exists(savePath)) {
                // Save found
                SaveData saveData = LoadSaveFromFile(savePath);
                saveProfile.ShowSave(saveData.playerName, saveData.saveDate);
            } else {
                // Save not found
                saveProfile.SetNoSave();
            }
        }
    }
}
