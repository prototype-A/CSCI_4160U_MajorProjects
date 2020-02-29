using System;

[System.Serializable]
public class GameSave {

    public PlayerData playerData;
    public int floorLevel;
    public float[] playerPos;
    public int[,] terrainMap;
    public string saveDate;

    public GameSave(PlayerData playerData, int floorLevel, float[] playerPos, int[,] terrainMap) {
        this.playerData = playerData;
        this.floorLevel = floorLevel;
        this.playerPos = playerPos;
        this.terrainMap = terrainMap;
        this.saveDate = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
    }
}
