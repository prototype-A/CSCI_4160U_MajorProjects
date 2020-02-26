using System;
using UnityEngine;

[System.Serializable]
public class GameSave {

    public PlayerStats playerStats;
    public float[] playerPos;
    public int[,] terrainMap;
    public string saveDate;

    public GameSave(PlayerStats playerStats, float[] playerPos, int[,] terrainMap) {
        this.playerStats = playerStats;
        this.playerPos = playerPos;
        this.terrainMap = terrainMap;
        this.saveDate = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
    }
}
