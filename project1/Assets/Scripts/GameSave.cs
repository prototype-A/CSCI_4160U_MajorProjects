using System;
using UnityEngine;

[System.Serializable]
public class GameSave {

    public Class playerClass;
    public Vector3 playerPos;
    public int[,] terrainMap;
    public string saveDate;

    public GameSave(Class playerClass, Vector3 playerPos, int[,] terrainMap) {
        this.playerClass = playerClass;
        this.playerPos = playerPos;
        this.terrainMap = terrainMap;
        this.saveDate = new DateTime().Date.ToString("MM/dd/yyyy HH:mm");
    }
}
