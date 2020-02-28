using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameData {
    // JSON
    public static TextAsset classJson = Resources.Load("Json/Classes") as TextAsset;
    public static TextAsset enemyJson = Resources.Load("Json/Enemies") as TextAsset;
    public static TextAsset consumablesJson = Resources.Load("Json/Consumables") as TextAsset;
    public static TextAsset weaponJson = Resources.Load("Json/Weapons") as TextAsset;

    // Enemies
    public static Enemy[] enemies = JsonUtility.FromJson<Enemies>(enemyJson.text).enemies;

    // Player
    public static Classes playableClasses = JsonUtility.FromJson<Classes>(classJson.text);
    public static Class[] playerClasses = playableClasses.classes;
    public static PlayerStats playerStats;
    public static Inventory playerInventory;
    public static bool inBattle;
    public static GameObject playerSprite;

    // Items
    public static Dictionary<string, Consumable> consumables = JsonUtility.FromJson<Consumables>(consumablesJson.text).GetDict();
    public static Dictionary<string, Item> weapons = JsonUtility.FromJson<Weapons>(weaponJson.text).GetDict();
    public static Prefix[] weaponPrefixes = JsonUtility.FromJson<Weapons>(weaponJson.text).prefixes;
}
