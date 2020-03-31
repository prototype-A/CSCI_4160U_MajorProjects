using TMPro;
﻿using UnityEngine;

public class SaveProfile : MonoBehaviour {

    [SerializeField] private GameObject noSave = null;

    // Save data
    [SerializeField] private GameObject saveData = null;
    [SerializeField] private TextMeshProUGUI charName = null;
    [SerializeField] private TextMeshProUGUI saveDate = null;


    public void SetNoSave() {
        SetCharacterName("");
        SetSaveDate("");
        saveData.SetActive(false);
        noSave.SetActive(true);
    }

    public void ShowSave(string charName, string saveDate) {
        SetCharacterName(charName);
        SetSaveDate(saveDate);
        saveData.SetActive(true);
        noSave.SetActive(false);
    }

    private void SetCharacterName(string name) {
        charName.text = name;
    }

    private void SetSaveDate(string date) {
        saveDate.text = date;
    }
}
