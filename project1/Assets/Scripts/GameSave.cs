using System;
using UnityEngine;

[System.Serializable]
public class GameSave {

    public PlayerData playerData;
    public float[] playerPos;
    public int[,] terrainMap;
    public string saveDate;

    public GameSave(float[] playerPos, int[,] terrainMap) {
        this.playerData = GameData.playerData;
        this.playerPos = playerPos;
        this.terrainMap = terrainMap;
        this.saveDate = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
    }
}
