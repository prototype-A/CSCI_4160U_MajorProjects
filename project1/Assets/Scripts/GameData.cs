using UnityEngine;

public static class GameData {
    // JSON
    private static TextAsset classJson = Resources.Load("Json/Classes") as TextAsset;
    public static TextAsset enemyJson = Resources.Load("Json/Enemies") as TextAsset;
    private static TextAsset consumableJson = Resources.Load("Json/Consumables") as TextAsset;
    private static TextAsset weaponJson = Resources.Load("Json/Weapons") as TextAsset;

    // Enemies
    public static Enemy[] enemies = JsonUtility.FromJson<Enemies>(enemyJson.text).enemies;

    // Player
    public static Classes playableClasses = JsonUtility.FromJson<Classes>(classJson.text);
    public static Class[] playerClasses = playableClasses.classes;
    public static PlayerStats playerStats;
    public static Inventory playerInventory;
    public static bool inBattle;

    // Items

}
