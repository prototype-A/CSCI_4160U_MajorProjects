using System;
using System.Collections.Generic;
using System.Numerics;

[System.Serializable]
public class SaveData {

    public string saveDate;

    // Player
    public string playerName;
    public Vector3 playerPos;
    public float playerHealth;
    public float playerHunger;
    public float playerThirst;
    public List<ItemSave> items;

    // Settings
    public int crosshair;

    public SaveData(string name, Vector3 playerPos, float playerHealth,
                    float playerHunger, float playerThirst, int crosshair) {
        this.saveDate = DateTime.Now.ToString("MM/dd/yyyy HH:mm");

        this.playerName = name;
        this.playerPos = playerPos;
        this.playerHealth = playerHealth;
        this.playerHunger = playerHunger;
        this.playerThirst = playerThirst;
        items = new List<ItemSave>();

        this.crosshair = crosshair;
    }

    public void SaveItem(Item item) {
        items.Add(new ItemSave(item));
    }
}
