using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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
    public static PlayerData playerData;
    public static Inventory playerInventory;
    public static bool inBattle;
    public static GameObject playerSprite;
    public static GameObject playerGui;
    public static GameSave gameSave;

    // Items
    public static Dictionary<string, Consumable> consumables = JsonUtility.FromJson<Consumables>(consumablesJson.text).GetDict();
    public static Dictionary<string, Item> weapons = JsonUtility.FromJson<Weapons>(weaponJson.text).GetDict();
    public static Prefix[] weaponPrefixes = JsonUtility.FromJson<Weapons>(weaponJson.text).prefixes;


    public static void LoadSaveData() {
        // Get objects at root of scene
        GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

        if (gameSave != null) {
            // Restore game savedata
            playerData = gameSave.playerData;
            Transform map = null;
            Transform player = null;
            for (int i = 0; i < rootObjects.Length; i++) {
                if (rootObjects[i].name == "Map") {
                    map = rootObjects[i].transform;
                } else if (rootObjects[i].name == "Player") {
                    player = rootObjects[i].transform;
                }
            }
            map.Find("Ground").gameObject.GetComponent<Tilemap>().ClearAllTiles();
            map.Find("Chasm").gameObject.GetComponent<Tilemap>().ClearAllTiles();
            FloorGenerator floorGenerator = map.gameObject.GetComponent<FloorGenerator>();
            floorGenerator.terrainMap = gameSave.terrainMap;
            floorGenerator.FillTiles();
            player.position = new Vector3(gameSave.playerPos[0], gameSave.playerPos[1], gameSave.playerPos[2]);
            gameSave = null;
        } else {
            // Generate a map if there is no savedata to load
            GameObject map = null;
            for (int i = 0; i < rootObjects.Length; i++) {
                if (rootObjects[i].name == "Map") {
                    map = rootObjects[i];
                    break;
                }
            }
            map.GetComponent<FloorGenerator>().generateMap();
        }
    }

    public static Consumable GetConsumable(string name) {
        return GameData.consumables[name];
    }

    public static Weapon GenerateWeapon(string name) {
        // Randomly generate a weapon
        System.Random rngesus = new System.Random();
        Item baseWeapon = GameData.weapons[name];
        Prefix prefix = GameData.weaponPrefixes[rngesus.Next(GameData.weaponPrefixes.Length)];
        if (rngesus.Next(101) > prefix.chance) {
            // Chance of not obtaining boosted gear
            prefix = null;
        }

        return new Weapon(baseWeapon, prefix);
    }
}
